using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Nexus.Data.Store.Communication.Response;

namespace Nexus.Data.Store.Communication
{
    public interface IRobloxCommunicator
    {
        /// <summary>
        /// Sends a request to Roblox Open Cloud.
        /// </summary>
        /// <param name="request">Request to send.</param>
        /// <typeparam name="T">Type of the response body.</typeparam>
        /// <returns>Response from Roblox.</returns>
        public Task<HttpResponse<T>> RequestAsync<T>(HttpRequestMessage request) where T : class;

        /// <summary>
        /// Returns the URI for the given URL.
        /// </summary>
        /// <param name="url">URL to create the URI for.</param>
        /// <returns>The URI for the URL.</returns>
        public Uri GetUri(string url);

        /// <summary>
        /// Sets the API key used with Roblox Open Cloud.
        /// </summary>
        /// <param name="apiKey">Roblox Open Cloud API key to use.</param>
        public void SetApiKey(string apiKey);

        /// <summary>
        /// Sends a request to Roblox Open Cloud with no body.
        /// </summary>
        /// <param name="method">HTTP method to use.</param>
        /// <param name="url">URL to use.</param>
        /// <typeparam name="T">Type of the response body.</typeparam>
        /// <returns>Response from Roblox.</returns>
        public async Task<HttpResponse<T>> RequestAsync<T>(HttpMethod method, string url)
            where T : class =>
            await this.RequestAsync<T>(new HttpRequestMessage()
            {
                Method = method,
                RequestUri = this.GetUri(url),
            });
        
        /// <summary>
        /// Sends a request to Roblox Open Cloud.
        /// </summary>
        /// <param name="method">HTTP method to use.</param>
        /// <param name="url">URL to use.</param>
        /// <param name="body">Body to send.</param>
        /// <typeparam name="T">Type of the response body.</typeparam>
        /// <returns>Response from Roblox.</returns>
        public async Task<HttpResponse<T>> RequestAsync<T>(HttpMethod method, string url, object body)
            where T : class =>
            await this.RequestAsync<T>(new HttpRequestMessage()
            {
                Method = method,
                RequestUri = this.GetUri(url),
                Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json"),
            });
    }
}