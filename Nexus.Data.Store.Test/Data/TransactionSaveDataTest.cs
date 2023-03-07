using System.Collections.Generic;
using Nexus.Data.Store.Data;
using NUnit.Framework;

namespace Nexus.Data.Store.Test.Data
{
    public class TransactionSaveDataTest
    {
        /// <summary>
        /// SaveData used by the transaction save data.
        /// </summary>
        private MemorySaveData _testMemorySaveData = null!;

        /// <summary>
        /// Test save data that is tested. 
        /// </summary>
        private TransactionSaveData _testSaveData = null!;

        /// <summary>
        /// Sets up the tests.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            this._testMemorySaveData = new MemorySaveData();
            this._testSaveData = new TransactionSaveData(this._testMemorySaveData);
            this._testMemorySaveData.SetAsync("testKey1", "test1").Wait();
            this._testMemorySaveData.SetAsync("testKey2", "test2").Wait();
            this._testMemorySaveData.SetAsync("testKey3", "test3").Wait();
        }

        /// <summary>
        /// Tests getting and setting values.
        /// </summary>
        [Test]
        public void TestGetSet()
        {
            this._testSaveData.SetAsync("testKey2", "test3").Wait();
            this._testSaveData.SetAsync<string>("testKey3", null).Wait();
            Assert.That(this._testSaveData.Get<string>("testKey"), Is.Null);
            Assert.That(this._testSaveData.Get<string>("testKey1"), Is.EqualTo("test1"));
            Assert.That(this._testSaveData.Get<string>("testKey2"), Is.EqualTo("test3"));
            Assert.That(this._testSaveData.Get<string>("testKey3"), Is.Null);
        }

        /// <summary>
        /// Tests applying changes.
        /// </summary>
        [Test]
        public void TestApply()
        {
            this._testSaveData.SetAsync("testKey2", "test3").Wait();
            this._testSaveData.SetAsync<string>("testKey3", null).Wait();
            Assert.That(this._testMemorySaveData.Get<string>("testKey1"), Is.EqualTo("test1"));
            Assert.That(this._testMemorySaveData.Get<string>("testKey2"), Is.EqualTo("test2"));
            Assert.That(this._testMemorySaveData.Get<string>("testKey3"), Is.EqualTo("test3"));
            var updatedKeys = this._testSaveData.Apply();
            Assert.That(this._testMemorySaveData.Get<string>("testKey1"), Is.EqualTo("test1"));
            Assert.That(this._testMemorySaveData.Get<string>("testKey2"), Is.EqualTo("test3"));
            Assert.That(this._testMemorySaveData.Get<string>("testKey3"), Is.Null);
            Assert.That(updatedKeys, Is.EqualTo(new List<string>() {"testKey2", "testKey3"}));
        }
    }
}