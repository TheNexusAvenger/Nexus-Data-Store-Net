using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Nexus.Data.Store.Communication;
using Nexus.Data.Store.Communication.Exception;
using Nexus.Data.Store.Communication.Request;
using Nexus.Data.Store.Communication.Response;

namespace Nexus.Data.Store.Data
{
    public class SaveData : ISaveData
    {
        /// <summary>
        /// Id of the game.
        /// </summary>
        public long GameId { get; set; }
        
        /// <summary>
        /// Name of the DataStore to save to.
        /// </summary>
        public string DataStoreName { get; set; } = null!;
        
        /// <summary>
        /// Key of the DataStore to save to.
        /// </summary>
        public string DataStoreKey { get; set; } = null!;

        /// <summary>
        /// Communicates with Roblox.
        /// </summary>
        public IRobloxCommunicator RobloxCommunicator { get; set; } = null!;

        /// <summary>
        /// Memory save data for the cached data.
        /// </summary>
        private readonly MemorySaveData _memorySaveData = new MemorySaveData();
        
        /// <summary>
        /// Event for the SaveData being disconnected.
        /// </summary>
        public event Action Disconnected;

        /// <summary>
        /// Reloads the data stored in the save data.
        /// MessagingService can't be subscribed to, so data must be manually fetched.
        /// </summary>
        public async Task ReloadAsync()
        {
            try
            {
                // Get the current data.
                var url = $"datastores/v1/universes/{this.GameId}/standard-datastores/datastore/entries/entry?datastoreName={this.DataStoreName}&entryKey={this.DataStoreKey}";
                var response = await this.RobloxCommunicator.RequestAsync<Dictionary<string, object>>(HttpMethod.Get, url);
                
                // Reload the values.
                this._memorySaveData.Clear();
                if (response.Body != null)
                {
                    foreach (var (key, value) in response.Body)
                    {
                        await this._memorySaveData.SetAsync(key, value);
                    }
                }
            }
            catch (OpenCloudDataStoreNotFoundException)
            {
                // Clear the saved data (DataStore does not exist).
                this._memorySaveData.Clear();
            }
            catch (OpenCloudDataStoreEntryNotFoundException)
            {
                // Clear the saved data (DataStore entry does not exist).
                this._memorySaveData.Clear();
            }
        }

        /// <summary>
        /// Returns the value for a key.
        /// </summary>
        /// <param name="key">Key to get.</param>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <returns>The value in the save data.</returns>
        public T? Get<T>(string key)
        {
            return this._memorySaveData.Get<T>(key);
        }

        /// <summary>
        /// Sets the value for a key.
        /// Unlike Nexus Data Store, this will yield until the update is completed.
        /// No queueing of bulk writes is handled.
        /// </summary>
        /// <param name="key">Key to set.</param>
        /// <param name="value">Value to set.</param>
        /// <typeparam name="T">Type of the value to set.</typeparam>
        public async Task SetAsync<T>(string key, T value)
        {
            await this._memorySaveData.SetAsync<T>(key, value);
            await this.FlushAsync(new List<string>() { key });
        }

        /// <summary>
        /// Updates multiple keys together and saves the result in one request.
        /// Due to limitations in C#, the usage is very different
        /// </summary>
        /// <param name="updateFunction">Update function with the SaveData to operate on.</param>
        public async Task UpdateAsync(Action<ISaveData> updateFunction)
        {
            var transaction = new TransactionSaveData(this._memorySaveData);
            updateFunction.Invoke(transaction);
            await this.FlushAsync(transaction.Apply());
        }

        /// <summary>
        /// Flushes the current data to Roblox's DataStore.
        /// </summary>
        /// <param name="keys">Keys to update.</param>
        public async Task FlushAsync(List<string> keys)
        {
            if (keys.Count == 0) return;
            
            // Store the new values.
            var dataStoreUrl = $"datastores/v1/universes/{this.GameId}/standard-datastores/datastore/entries/entry?datastoreName={this.DataStoreName}&entryKey={this.DataStoreKey}";
            await this.RobloxCommunicator.RequestAsync<DataStoreSetEntryResponse>(HttpMethod.Post, dataStoreUrl, this._memorySaveData.Values);
            
            // Build the entries to send.
            var setEntries = new List<NexusDataStoreUpdateEntry>();
            var fetchEntry = new NexusDataStoreUpdateEntry()
            {
                Action = "Fetch",
                Keys = new List<string>(),
            };
            foreach (var key in keys)
            {
                var value = this._memorySaveData.Get<object>(key);
                if (JsonConvert.SerializeObject(value).Length <= 500)
                {
                    setEntries.Add(new NexusDataStoreUpdateEntry()
                    {
                        Action = "Set",
                        Key = key,
                        Value = value,
                    });
                }
                else
                {
                    fetchEntry.Keys.Add(key);
                }
            }
            
            var entries = new NexusBulkMessagingServiceEntries();
            foreach (var entry in setEntries)
            {
                entries.AddEntry(entry);
            }
            if (fetchEntry.Keys.Count > 0)
            {
                entries.AddEntry(fetchEntry);
            }

            // Message the changes.
            var messageUrl = $"messaging-service/v1/universes/{this.GameId}/topics/NexusBulkMessagingService";
            foreach (var entriesList in entries.Entries)
            {
                var message = JsonConvert.SerializeObject(new Dictionary<string, List<string>>()
                {
                    { $"NSD_{this.DataStoreKey}", entriesList },
                });
                var body = new Dictionary<string, string>() { { "message", message } };
                await this.RobloxCommunicator.RequestAsync<object>(HttpMethod.Post, messageUrl, body);
            }
        }
        
        /// <summary>
        /// Clears the SaveData from the parent NexusDataStore cache.
        /// </summary>
        public void Disconnect()
        {
            this.Disconnected?.Invoke();
        }
    }
}