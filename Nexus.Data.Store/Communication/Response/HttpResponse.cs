using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Nexus.Data.Store.Communication.Response
{
    public class HttpResponse<T> where T : class
    {
        /// <summary>
        /// Status code of the response.
        /// </summary>
        public HttpStatusCode Status { get; set; }
        
        /// <summary>
        /// Body of the response.
        /// </summary>
        public T? Body { get; set; }

        /// <summary>
        /// Parses a response.
        /// </summary>
        /// <param name="response">Response to parse from.</param>
        /// <returns>A parsed HTTP response.</returns>
        public static async Task<HttpResponse<T>> FromResponseAsync(HttpResponseMessage response)
        {
            return new HttpResponse<T>()
            {
                Status = response.StatusCode,
                Body = JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync()),
            };
        }
    }
}