using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Nexus.Data.Store.Communication;
using Nexus.Data.Store.Data;

namespace Nexus.Data.Store
{
    public class NexusDataStore
    {
        /// <summary>
        /// Game id used with Roblox.
        /// </summary>
        public readonly ulong GameId;

        /// <summary>
        /// Communicator used with Roblox.
        /// </summary>
        public readonly IRobloxCommunicator Communicator;

        /// <summary>
        /// Semaphore for accessing cached SaveData entries.
        /// </summary>
        private readonly SemaphoreSlim _saveDataCacheSemaphore = new SemaphoreSlim(1);

        /// <summary>
        /// Cache of SaveData instances for the data store.
        /// </summary>
        private readonly Dictionary<string, Dictionary<string, SaveData>> _saveDataCache = new Dictionary<string, Dictionary<string, SaveData>>();

        /// <summary>
        /// Semaphore for accessing cached NexusDataStore entries.
        /// </summary>
        private static readonly SemaphoreSlim NexusDataStoreCacheSemaphore = new SemaphoreSlim(1);

        /// <summary>
        /// Cache of NexusDataStore instances for the data store.
        /// </summary>
        private static readonly Dictionary<ulong, NexusDataStore> NexusDataStoreCache = new Dictionary<ulong, NexusDataStore>();

        /// <summary>
        /// Creates a NexusDataStore instance.
        /// </summary>
        /// <param name="gameId">Id of the game to access DataStores.</param>
        /// <param name="apiKey">API key for Roblox Open Cloud.</param>
        public NexusDataStore(ulong gameId, string apiKey)
        {
            this.GameId = gameId;
            this.Communicator = new RobloxCommunicator();
            this.Communicator.SetApiKey(apiKey);
        }

        /// <summary>
        /// Creates a NexusDataStore instance or returns a cached version.
        /// </summary>
        /// <param name="gameId">Id of the game to access DataStores.</param>
        /// <param name="apiKey">API key for Roblox Open Cloud.</param>
        public static NexusDataStore Get(ulong gameId, string apiKey)
        {
            NexusDataStoreCacheSemaphore.Wait();
            if (!NexusDataStoreCache.ContainsKey(gameId))
            {
                NexusDataStoreCache[gameId] = new NexusDataStore(gameId, apiKey);
            }
            var nexusDataStore = NexusDataStoreCache[gameId];
            nexusDataStore.Communicator.SetApiKey(apiKey);
            NexusDataStoreCacheSemaphore.Release();
            return nexusDataStore;
        }

        /// <summary>
        /// Returns a SaveData for a DataStore name and key.
        /// </summary>
        /// <param name="dataStoreName">Name of the DataStore.</param>
        /// <param name="dataStoreKey">Key of the DataStore.</param>
        /// <returns>SaveData for the DataStore name and key.</returns>
        public async Task<SaveData> GetDataStoreAsync(string dataStoreName, string dataStoreKey)
        {
            await this._saveDataCacheSemaphore.WaitAsync();
            if (!this._saveDataCache.ContainsKey(dataStoreName))
            {
                this._saveDataCache[dataStoreName] = new Dictionary<string, SaveData>();
            }
            if (!this._saveDataCache[dataStoreName].ContainsKey(dataStoreKey))
            {
                var saveData = new SaveData()
                {
                    GameId = this.GameId,
                    DataStoreName = dataStoreName,
                    DataStoreKey = dataStoreKey,
                    RobloxCommunicator = this.Communicator,
                };
                await saveData.ReloadAsync();
                saveData.Disconnected += () =>
                {
                    this._saveDataCacheSemaphore.Wait();
                    this._saveDataCache[dataStoreName].Remove(dataStoreKey);
                    this._saveDataCacheSemaphore.Release();
                };
                this._saveDataCache[dataStoreName][dataStoreKey] = saveData;
            }
            this._saveDataCacheSemaphore.Release();
            return this._saveDataCache[dataStoreName][dataStoreKey];
        }

        /// <summary>
        /// Returns a SaveData for a Roblox user.
        /// </summary>
        /// <param name="userId">User id of the Roblox user.</param>
        /// <returns>SaveData for the user.</returns>
        public async Task<SaveData> GetSaveDataByIdAsync(ulong userId)
        {
            return await this.GetDataStoreAsync("PlayerDataStore_PlayerData", "PlayerList$" + userId);
        }
    }
}