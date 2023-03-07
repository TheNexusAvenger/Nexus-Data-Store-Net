using System.Net;
using Nexus.Data.Store.Communication.Response;

namespace Nexus.Data.Store.Communication.Exception
{
    public class OpenCloudResponseException : System.Exception
    {
        /// <summary>
        /// Response from Roblox.
        /// </summary>
        public HttpResponse<ErrorResponse> Response { get; set; } = null!;

        /// <summary>
        /// Creates an Open Cloud response exception with a message.
        /// </summary>
        /// <param name="response">Response the exception is for.</param>
        /// <param name="message">Message of the exception.</param>
        public OpenCloudResponseException(HttpResponse<ErrorResponse> response, string message = "Error occured with Open Cloud") : base(message)
        {
            this.Response = response;
        }

        /// <summary>
        /// Creates an exception for an error response.
        /// </summary>
        /// <param name="response">Response to build the exception for.</param>
        /// <returns>The exception instance for an error response.</returns>
        public static OpenCloudResponseException GetException(HttpResponse<ErrorResponse> response)
        {
            // Return a specific response.
            if (response.Status == HttpStatusCode.Unauthorized)
            {
                return new OpenCloudUnauthorizedException(response);
            }
            else if (response.Status == HttpStatusCode.Forbidden)
            {
                return new OpenCloudInsufficientScopeException(response);
            }
            else if (response.Status == HttpStatusCode.NotFound && response.Body != null && response.Body.ErrorDetails.Count > 0)
            {
                var errorType = response.Body.ErrorDetails[0].DatastoreErrorCode;
                if (errorType == "DatastoreNotFound")
                {
                    return new OpenCloudDataStoreNotFoundException(response);
                } else if (errorType == "EntryNotFound")
                {
                    return new OpenCloudDataStoreEntryNotFoundException(response);
                }
            }

            // Return the generic exception.
            return new OpenCloudResponseException(response);
        }
    }

    public class OpenCloudUnauthorizedException : OpenCloudResponseException
    {
        /// <summary>
        /// Creates an Open Cloud response exception.
        /// </summary>
        /// <param name="response">Response the exception is for.</param>
        public OpenCloudUnauthorizedException(HttpResponse<ErrorResponse> response) : base(response, "API key is invalid or expired.") { }
    }

    public class OpenCloudInsufficientScopeException : OpenCloudResponseException
    {
        /// <summary>
        /// Creates an Open Cloud response exception.
        /// </summary>
        /// <param name="response">Response the exception is for.</param>
        public OpenCloudInsufficientScopeException(HttpResponse<ErrorResponse> response) : base(response, "API key is valid but does not have permissions for either the game or operation.") { }
    }
    
    public class OpenCloudDataStoreNotFoundException : OpenCloudResponseException
    {
        /// <summary>
        /// Creates an Open Cloud response exception.
        /// </summary>
        /// <param name="response">Response the exception is for.</param>
        public OpenCloudDataStoreNotFoundException(HttpResponse<ErrorResponse> response) : base(response, "DataStore does not exist.") { }
    }
    
    public class OpenCloudDataStoreEntryNotFoundException : OpenCloudResponseException
    {
        /// <summary>
        /// Creates an Open Cloud response exception.
        /// </summary>
        /// <param name="response">Response the exception is for.</param>
        public OpenCloudDataStoreEntryNotFoundException(HttpResponse<ErrorResponse> response) : base(response, "DataStore entry does not exist.") { }
    }
}