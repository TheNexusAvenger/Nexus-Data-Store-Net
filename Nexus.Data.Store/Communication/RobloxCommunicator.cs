using System;
using System.Net.Http;
using System.Threading.Tasks;
using Nexus.Data.Store.Communication.Exception;
using Nexus.Data.Store.Communication.Response;

namespace Nexus.Data.Store.Communication
{
    public class RobloxCommunicator : IRobloxCommunicator
    {
        /// <summary>
        /// Static HTTP client for making requests.
        /// </summary>
        private static readonly HttpClient HttpClient = new HttpClient();
        
        /// <summary>
        /// Base URL used for Open Cloud.
        /// </summary>
        public string BaseUrl { get; set; } = "https://apis.roblox.com/";

        /// <summary>
        /// API key for Open Cloud requests.
        /// </summary>
        private string ApiKey { get; set; } = null!;
        
        /// <summary>
        /// Sends a request to Roblox Open Cloud.
        /// </summary>
        /// <param name="request">Request to send.</param>
        /// <typeparam name="T">Type of the response body.</typeparam>
        /// <returns>Response from Roblox.</returns>
        public async Task<HttpResponse<T>> RequestAsync<T>(HttpRequestMessage request) where T : class
        {
            // Send the request and get the response.
            request.Headers.Add("x-api-key", this.ApiKey);
            var response = await HttpClient.SendAsync(request);
            
            // Throw an exception for error responses.
            if (!response.IsSuccessStatusCode)
            {
                throw OpenCloudResponseException.GetException(await HttpResponse<ErrorResponse>.FromResponseAsync(response));
            }
            
            // Parse and return the response.
            return await HttpResponse<T>.FromResponseAsync(response);
        }

        /// <summary>
        /// Returns the URI for the given URL.
        /// </summary>
        /// <param name="url">URL to create the URI for.</param>
        /// <returns>The URI for the URL.</returns>
        public Uri GetUri(string url)
        {
            return new Uri(url.StartsWith("http") ? url : BaseUrl + url);
        }

        /// <summary>
        /// Sets the API key used with Roblox Open Cloud.
        /// </summary>
        /// <param name="apiKey">Roblox Open Cloud API key to use.</param>
        public void SetApiKey(string apiKey)
        {
            this.ApiKey = apiKey;
        }
    }
}