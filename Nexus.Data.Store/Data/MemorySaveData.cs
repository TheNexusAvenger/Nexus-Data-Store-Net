using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Nexus.Data.Store.Data
{
    public class MemorySaveData : ISaveData
    {
        /// <summary>
        /// Values stored in the save datta.
        /// </summary>
        public readonly Dictionary<string, object> Values = new Dictionary<string, object>();

        /// <summary>
        /// Returns the value for a key.
        /// </summary>
        /// <param name="key">Key to get.</param>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <returns>The value in the save data.</returns>
        public T? Get<T>(string key)
        {
            // Return default if the key doesn't exist.
            if (!this.Values.TryGetValue(key, out var value))
            {
                return default;
            }

            // Return the value.
            if (value is JToken token)
            {
                return token.ToObject<T>();
            }
            return (T) this.Values[key];
        }

        /// <summary>
        /// Sets the value for a key.
        /// </summary>
        /// <param name="key">Key to set.</param>
        /// <param name="value">Value to set.</param>
        /// <typeparam name="T">Type of the value to set.</typeparam>
        public Task SetAsync<T>(string key, T value)
        {
            if (value == null)
            {
                this.Values.Remove(key);
            }
            else
            {
                this.Values[key] = value;
            }
            return Task.CompletedTask;
        }
        
        /// <summary>
        /// Updates multiple keys together and saves the result in one request.
        /// Due to limitations in C#, the usage is very different
        /// </summary>
        /// <param name="updateFunction">Update function with the SaveData to operate on.</param>
        public Task UpdateAsync(Action<ISaveData> updateFunction)
        {
            updateFunction.Invoke(this);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Clears the stored save data.
        /// </summary>
        public void Clear()
        {
            this.Values.Clear();
        }
        
        /// <summary>
        /// Clears the SaveData from the parent NexusDataStore cache.
        /// </summary>
        public void Disconnect()
        {
            // No implementation.
        }
    }
}