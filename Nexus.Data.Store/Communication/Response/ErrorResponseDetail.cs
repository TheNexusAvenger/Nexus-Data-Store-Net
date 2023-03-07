namespace Nexus.Data.Store.Communication.Response
{
    public class ErrorResponseDetail
    {
        /// <summary>
        /// Type of the error.
        /// </summary>
        public string ErrorDetailType { get; set; } = null!;
        
        /// <summary>
        /// Error detail specific for DataStores.
        /// </summary>
        public string? DatastoreErrorCode { get; set; }
    }
}