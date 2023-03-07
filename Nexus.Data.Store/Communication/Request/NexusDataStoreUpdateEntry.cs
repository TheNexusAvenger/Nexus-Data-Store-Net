using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nexus.Data.Store.Communication.Request
{
    public class NexusDataStoreUpdateEntry
    {
        /// <summary>
        /// Action for the update.
        /// </summary>
        public string Action { get; set; } = null!;
        
        /// <summary>
        /// Key for the update (Action = Set only).
        /// </summary>
        public string? Key { get; set; }
        
        /// <summary>
        /// Keys for the update (Action = Fetch only).
        /// </summary>
        public List<string>? Keys { get; set; }
        
        /// <summary>
        /// Value of the update.
        /// </summary>
        public object? Value { get; set; }
    }
}