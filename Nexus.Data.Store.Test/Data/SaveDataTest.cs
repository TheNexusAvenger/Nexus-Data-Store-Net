using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Nexus.Data.Store.Communication;
using Nexus.Data.Store.Communication.Exception;
using Nexus.Data.Store.Communication.Request;
using Nexus.Data.Store.Communication.Response;
using Nexus.Data.Store.Data;
using NUnit.Framework;

namespace Nexus.Data.Store.Test.Data
{
    public class SaveDataTestRobloxCommunicator : IRobloxCommunicator
    {
        public int TotalRequests { get; private set; }
        
        public async Task<HttpResponse<T>> RequestAsync<T>(HttpRequestMessage request) where T : class
        {
            this.TotalRequests += 1;
            var uri = request.RequestUri!.AbsoluteUri;
            if (uri == "https://apis.roblox.com/datastores/v1/universes/1/standard-datastores/datastore/entries/entry?datastoreName=TestDataStore&entryKey=TestKey")
            {
                return new HttpResponse<T>()
                {
                    Status = HttpStatusCode.OK,
                    Body = JsonConvert.DeserializeObject<T>("{\"testKey1\": 1, \"testKey2\": \"test\", \"testKey3\": [\"value1\", \"value2\"], \"testKey4\": {\"key\": [\"value1\", \"value2\"]}}"),
                };
            }
            else if (uri == "https://apis.roblox.com/datastores/v1/universes/2/standard-datastores/datastore/entries/entry?datastoreName=TestDataStore&entryKey=TestKey")
            {
                throw new OpenCloudDataStoreNotFoundException(new HttpResponse<ErrorResponse>());
            }
            else if (uri == "https://apis.roblox.com/datastores/v1/universes/3/standard-datastores/datastore/entries/entry?datastoreName=TestDataStore&entryKey=TestKey")
            {
                throw new OpenCloudDataStoreEntryNotFoundException(new HttpResponse<ErrorResponse>());
            }
            else if (uri == "https://apis.roblox.com/datastores/v1/universes/4/standard-datastores/datastore/entries/entry?datastoreName=TestDataStore&entryKey=TestKey")
            {
                throw new OpenCloudInsufficientScopeException(new HttpResponse<ErrorResponse>());
            }
            else if (uri == "https://apis.roblox.com/datastores/v1/universes/11/standard-datastores/datastore/entries/entry?datastoreName=TestDataStore&entryKey=TestKey")
            {
                var data = JsonConvert.DeserializeObject<Dictionary<string, long>>(await request.Content!.ReadAsStringAsync())!;
                Assert.That(data["testKey1"] == 3 || data["testKey1"] == 4);
                Assert.That(data.Values.Count, Is.EqualTo(1));
                return new HttpResponse<T>()
                {
                    Status = HttpStatusCode.OK,
                    Body = JsonConvert.DeserializeObject<T>("{}"),
                };
            }
            else if (uri == "https://apis.roblox.com/messaging-service/v1/universes/11/topics/NexusBulkMessagingService")
            {
                var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(await request.Content!.ReadAsStringAsync())!;
                var allMessages = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(data["message"])!["NSD_TestKey"];
                var firstMessage = JsonConvert.DeserializeObject<NexusDataStoreUpdateEntry>(allMessages[0])!;
                
                Assert.That(firstMessage.Action, Is.EqualTo("Set"));
                Assert.That(firstMessage.Key, Is.EqualTo("testKey1"));
                Assert.That((long) firstMessage.Value! == 3L || (long) firstMessage.Value == 4L);
                Assert.That(allMessages.Count, Is.EqualTo(1));
                return new HttpResponse<T>()
                {
                    Status = HttpStatusCode.OK,
                    Body = JsonConvert.DeserializeObject<T>("{}"),
                };
            }
            else if (uri == "https://apis.roblox.com/datastores/v1/universes/21/standard-datastores/datastore/entries/entry?datastoreName=TestDataStore&entryKey=TestKey")
            {
                var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(await request.Content!.ReadAsStringAsync())!;
                Assert.That(data["testKey1"] == "test1");
                Assert.That(data["testKey2"] == new string('0', 600));
                Assert.That(data["testKey3"] == "test3");
                Assert.That(data["testKey4"] == new string('0', 600));
                Assert.That(data.Keys.Count, Is.EqualTo(4));
                return new HttpResponse<T>()
                {
                    Status = HttpStatusCode.OK,
                    Body = JsonConvert.DeserializeObject<T>("{}"),
                };
            }
            else if (uri == "https://apis.roblox.com/messaging-service/v1/universes/21/topics/NexusBulkMessagingService")
            {
                var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(await request.Content!.ReadAsStringAsync())!;
                var allMessages = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(data["message"])!["NSD_TestKey"];
                var message1 = JsonConvert.DeserializeObject<NexusDataStoreUpdateEntry>(allMessages[0])!;
                var message2 = JsonConvert.DeserializeObject<NexusDataStoreUpdateEntry>(allMessages[1])!;
                var message3 = JsonConvert.DeserializeObject<NexusDataStoreUpdateEntry>(allMessages[2])!;
                
                Assert.That(message1.Action, Is.EqualTo("Set"));
                Assert.That(message1.Key, Is.EqualTo("testKey1"));
                Assert.That(message1.Value, Is.EqualTo("test1"));
                Assert.That(message2.Action, Is.EqualTo("Set"));
                Assert.That(message2.Key, Is.EqualTo("testKey3"));
                Assert.That(message2.Value, Is.EqualTo("test3"));
                Assert.That(message3.Action, Is.EqualTo("Fetch"));
                Assert.That(message3.Keys, Is.EqualTo(new List<string>() { "testKey2", "testKey4"}));
                Assert.That(allMessages.Count, Is.EqualTo(3));
                return new HttpResponse<T>()
                {
                    Status = HttpStatusCode.OK,
                    Body = JsonConvert.DeserializeObject<T>("{}"),
                };
            }
            else if (uri == "https://apis.roblox.com/datastores/v1/universes/22/standard-datastores/datastore/entries/entry?datastoreName=TestDataStore&entryKey=TestKey")
            {
                var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(await request.Content!.ReadAsStringAsync())!;
                Assert.That(data["testKey1"] == "test1");
                Assert.That(data["testKey2"] == "test2");
                Assert.That(data.Keys.Count, Is.EqualTo(2));
                return new HttpResponse<T>()
                {
                    Status = HttpStatusCode.OK,
                    Body = JsonConvert.DeserializeObject<T>("{}"),
                };
            }
            else if (uri == "https://apis.roblox.com/messaging-service/v1/universes/22/topics/NexusBulkMessagingService")
            {
                var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(await request.Content!.ReadAsStringAsync())!;
                var allMessages = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(data["message"])!["NSD_TestKey"];
                var message1 = JsonConvert.DeserializeObject<NexusDataStoreUpdateEntry>(allMessages[0])!;
                var message2 = JsonConvert.DeserializeObject<NexusDataStoreUpdateEntry>(allMessages[1])!;
                
                Assert.That(message1.Action, Is.EqualTo("Set"));
                Assert.That(message1.Key, Is.EqualTo("testKey1"));
                Assert.That(message1.Value, Is.EqualTo("test1"));
                Assert.That(message2.Action, Is.EqualTo("Set"));
                Assert.That(message2.Key, Is.EqualTo("testKey2"));
                Assert.That(message2.Value, Is.EqualTo("test2"));
                Assert.That(allMessages.Count, Is.EqualTo(2));
                return new HttpResponse<T>()
                {
                    Status = HttpStatusCode.OK,
                    Body = JsonConvert.DeserializeObject<T>("{}"),
                };
            }
            throw new NotImplementedException(uri);
        }

        public Uri GetUri(string url)
        {
            return new Uri("https://apis.roblox.com/" + url);
        }

        public void SetApiKey(string apiKey)
        {
            
        }
    }
    
    public class SaveDataTest
    {
        /// <summary>
        /// RobloxCommunicator used for the tests.
        /// </summary>
        private SaveDataTestRobloxCommunicator _robloxCommunicator = null!;
        
        /// <summary>
        /// SaveData used for the tests.
        /// </summary>
        private SaveData _testSaveData = null!;
        
        /// <summary>
        /// Sets up the test.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            this._robloxCommunicator = new SaveDataTestRobloxCommunicator();
            this._testSaveData = new SaveData()
            {
                DataStoreName = "TestDataStore",
                DataStoreKey = "TestKey",
                RobloxCommunicator = this._robloxCommunicator,
            };
        }

        /// <summary>
        /// Tests reloading data.
        /// </summary>
        [Test]
        public void TestReloadAsync()
        {
            this._testSaveData.GameId = 1;
            this._testSaveData.ReloadAsync().Wait();
            Assert.That(this._testSaveData.Get<long>("testKey1"), Is.EqualTo(1));
            Assert.That(this._testSaveData.Get<string>("testKey2"), Is.EqualTo("test"));
            Assert.That(this._testSaveData.Get<List<string>>("testKey3"), Is.EqualTo(new List<string>() {"value1", "value2"}));
            Assert.That(this._testSaveData.Get<Dictionary<string, List<string>>>("testKey4"), Is.EqualTo(new Dictionary<string, List<string>>
            {
                {"key", new List<string>() {"value1", "value2"}}
            }));
            Assert.That(this._testSaveData.Get<long>("testKey5") == default);
            Assert.That(this._robloxCommunicator.TotalRequests, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests reloading data with no DataStore found.
        /// </summary>
        [Test]
        public void TestReloadAsyncDataStoreNotFound()
        {
            this._testSaveData.GameId = 2;
            this._testSaveData.ReloadAsync().Wait();
            Assert.That(this._testSaveData.Get<long>("testKey1") == default);
            Assert.That(this._robloxCommunicator.TotalRequests, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests reloading data with no DataStore entry found.
        /// </summary>
        [Test]
        public void TestReloadAsyncDataStoreEntryNotFound()
        {
            this._testSaveData.GameId = 3;
            this._testSaveData.ReloadAsync().Wait();
            Assert.That(this._testSaveData.Get<long>("testKey1") == default);
            Assert.That(this._robloxCommunicator.TotalRequests, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests reloading data with incorrect permissions.
        /// </summary>
        [Test]
        public void TestReloadAsyncForbidden()
        {
            this._testSaveData.GameId = 4;
            Assert.Throws<AggregateException>(() => this._testSaveData.ReloadAsync().Wait());
            Assert.That(this._robloxCommunicator.TotalRequests, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests setting a key.
        /// </summary>
        [Test]
        public void TestSet()
        {
            this._testSaveData.GameId = 11;
            this._testSaveData.SetAsync("testKey1", 3).Wait();
            Assert.That(this._robloxCommunicator.TotalRequests, Is.EqualTo(2));
            
            // Send a different value and make sure it is sent.
            this._testSaveData.SetAsync("testKey1", 4).Wait();
            Assert.That(this._robloxCommunicator.TotalRequests, Is.EqualTo(4));
        }

        /// <summary>
        /// Tests updating multiple keys.
        /// </summary>
        [Test]
        public void TestUpdate()
        {
            this._testSaveData.GameId = 21;
            this._testSaveData.UpdateAsync(saveData =>
            {
                saveData.Set("testKey1", "test1");
                saveData.Set("testKey2", new string('0', 600));
                saveData.Set("testKey3", "test3");
                saveData.Set("testKey4", new string('0', 600));
            }).Wait();
            Assert.That(this._robloxCommunicator.TotalRequests, Is.EqualTo(2));
        }

        /// <summary>
        /// Tests updating multiple keys with only short keys.
        /// </summary>
        [Test]
        public void TestUpdateNoLongKeys()
        {
            this._testSaveData.GameId = 22;
            this._testSaveData.UpdateAsync(saveData =>
            {
                saveData.Set("testKey1", "test1");
                saveData.Set("testKey2", "test2");
            }).Wait();
            Assert.That(this._robloxCommunicator.TotalRequests, Is.EqualTo(2));
        }

        /// <summary>
        /// Tests updating multiple keys with no changes.
        /// </summary>
        [Test]
        public void TestUpdateNoChanges()
        {
            this._testSaveData.GameId = 23;
            this._testSaveData.UpdateAsync(_ =>
            {
                
            }).Wait();
            Assert.That(this._robloxCommunicator.TotalRequests, Is.EqualTo(0));
        }

        /// <summary>
        /// Tests updating multiple keys with an exception that blocks the update.
        /// </summary>
        [Test]
        public void TestUpdateError()
        {
            this._testSaveData.GameId = 24;
            Assert.Throws<AggregateException>(() =>
            {
                this._testSaveData.UpdateAsync(saveData =>
                {
                    saveData.Set("testKey1", "test1");
                    saveData.Set("testKey2", "test2");
                    throw new Exception("Test exception");
                }).Wait();
            });
            Assert.That(this._testSaveData.Get<string>("testKey1"), Is.Null);
            Assert.That(this._testSaveData.Get<string>("testKey2"), Is.Null);
            Assert.That(this._robloxCommunicator.TotalRequests, Is.EqualTo(0));
        }
    }
}