using Nexus.Data.Store.Communication.Request;
using NUnit.Framework;

namespace Nexus.Data.Store.Test.Communication.Request
{
    public class NexusBulkMessagingServiceEntriesTest
    {
        /// <summary>
        /// Tests adding entries.
        /// </summary>
        [Test]
        public void TestAddEntry()
        {
            var entries = new NexusBulkMessagingServiceEntries();
            entries.AddEntry(new NexusDataStoreUpdateEntry() {Action = "Set", Key = "testKey1", Value = new string('0', 10)});
            entries.AddEntry(new NexusDataStoreUpdateEntry() {Action = "Set", Key = "testKey2", Value = new string('0', 10)});
            entries.AddEntry(new NexusDataStoreUpdateEntry() {Action = "Set", Key = "testKey3", Value = new string('0', 200)});
            entries.AddEntry(new NexusDataStoreUpdateEntry() {Action = "Set", Key = "testKey4", Value = new string('0', 150)});
            entries.AddEntry(new NexusDataStoreUpdateEntry() {Action = "Set", Key = "testKey5", Value = new string('0', 150)});
            entries.AddEntry(new NexusDataStoreUpdateEntry() {Action = "Set", Key = "testKey6", Value = new string('0', 150)});
            entries.AddEntry(new NexusDataStoreUpdateEntry() {Action = "Set", Key = "testKey7", Value = new string('0', 10)});
            Assert.That(entries.Entries.Count, Is.EqualTo(3));
            Assert.That(entries.Entries[0].Count, Is.EqualTo(3));
            Assert.That(entries.Entries[1].Count, Is.EqualTo(2));
            Assert.That(entries.Entries[2].Count, Is.EqualTo(2));
        }
    }
}