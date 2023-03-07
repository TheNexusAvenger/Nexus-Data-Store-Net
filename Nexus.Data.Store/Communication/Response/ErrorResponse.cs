using System.Collections.Generic;

namespace Nexus.Data.Store.Communication.Response
{
    public class ErrorResponse
    {
        /// <summary>
        /// Computer-readable error message.
        /// </summary>
        public string Error { get; set; } = null!;
        
        /// <summary>
        /// Human-readable error message.
        /// </summary>
        public string Message { get; set; } = null!;

        /// <summary>
        /// Details about the error.
        /// </summary>
        public List<ErrorResponseDetail> ErrorDetails { get; set; } = null!;
    }
}