using System.Threading.Tasks;

namespace Nexus.Data.Store.Data
{
    public interface ISaveData
    {
        /// <summary>
        /// Returns the value for a key.
        /// </summary>
        /// <param name="key">Key to get.</param>
        /// <typeparam name="T">Type of the value.</typeparam>
        /// <returns>The value in the save data.</returns>
        public T? Get<T>(string key);

        /// <summary>
        /// Sets the value for a key.
        /// </summary>
        /// <param name="key">Key to set.</param>
        /// <param name="value">Value to set.</param>
        /// <typeparam name="T">Type of the value to set.</typeparam>
        public Task SetAsync<T>(string key, T value);

        /// <summary>
        /// Sets the value for a key.
        /// </summary>
        /// <param name="key">Key to set.</param>
        /// <param name="value">Value to set.</param>
        /// <typeparam name="T">Type of the value to set.</typeparam>
        public void Set<T>(string key, T value) => this.SetAsync<T>(key, value).Wait();
    }
}