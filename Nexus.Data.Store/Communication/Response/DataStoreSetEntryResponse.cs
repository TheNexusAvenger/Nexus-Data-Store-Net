namespace Nexus.Data.Store.Communication.Response
{
    public class DataStoreSetEntryResponse
    {
        public string Version { get; set; } = null!;
        public bool Deleted { get; set; }
        public long ContentLength { get; set; }
        public string CreatedTime { get; set; } = null!;
        public string ObjectCreatedTime { get; set; } = null!;
    }
}