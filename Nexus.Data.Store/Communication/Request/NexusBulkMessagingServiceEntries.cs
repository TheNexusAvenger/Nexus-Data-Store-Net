using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Nexus.Data.Store.Communication.Request
{
    public class NexusBulkMessagingServiceEntries
    {
        /// <summary>
        /// Entries used to send in the bulk MessagingService implementation.
        /// </summary>
        public List<List<string>> Entries { get; set; } = new List<List<string>>();

        /// <summary>
        /// Adds an entry.
        /// </summary>
        /// <param name="entry">Entry to add.</param>
        public void AddEntry(NexusDataStoreUpdateEntry entry)
        {
            // Add a new entry list if there are no previous entries or it is too long.
            var entryString = JsonConvert.SerializeObject(entry);
            if (this.Entries.Count == 0 || JsonConvert.SerializeObject(this.Entries.Last()).Length + entryString.Length > 500)
            {
                this.Entries.Add(new List<string>());
            }
            
            // Add the existing entry.
            this.Entries.Last().Add(entryString);
        }
    }
}