using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nexus.Data.Store.Data
{
    public class TransactionSaveData : ISaveData
    {
        /// <summary>
        /// Base SaveData to write to.
        /// </summary>
        private readonly ISaveData _saveData;
        
        /// <summary>
        /// Overrides to apply as part of the transaction.
        /// </summary>
        protected readonly Dictionary<string, object?> Overrides = new Dictionary<string, object?>();
        
        /// <summary>
        /// Creates a transaction for SaveData.
        /// </summary>
        /// <param name="saveData">Save data to read from.</param>
        public TransactionSaveData(ISaveData saveData)
        {
            this._saveData = saveData;
        }
        
        /// <summary>
        /// Returns the value for a key.
        /// </summary>
        /// <param name="key">Key to get.</param>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <returns>The value in the save data.</returns>
        public T? Get<T>(string key)
        {
            if (this.Overrides.ContainsKey(key))
            {
                return (T) this.Overrides[key]!;
            }
            return this._saveData.Get<T>(key);
        }

        /// <summary>
        /// Sets the value for a key.
        /// Unlike Nexus Data Store, this will yield until the update is completed.
        /// No queueing of bulk writes is handled.
        /// </summary>
        /// <param name="key">Key to set.</param>
        /// <param name="value">Value to set.</param>
        /// <typeparam name="T">Type of the value to set.</typeparam>
        public Task SetAsync<T>(string key, T value)
        {
            this.Overrides[key] = value;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Applies the overrides to a dictionary of values.
        /// </summary>
        /// <returns>Keys that were changed.</returns>
        public List<string> Apply()
        {
            var updatedKeys = new List<string>();
            foreach (var (key, value) in this.Overrides)
            {
                updatedKeys.Add(key);
                this._saveData.SetAsync(key, value);
            }
            return updatedKeys;
        }
    }
}