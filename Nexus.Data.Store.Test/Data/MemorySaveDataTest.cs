using Nexus.Data.Store.Data;
using NUnit.Framework;

namespace Nexus.Data.Store.Test.Data
{
    public class MemorySaveDataTest
    {
        /// <summary>
        /// Test save data that is tested.
        /// </summary>
        private MemorySaveData _testSaveData = null!;

        /// <summary>
        /// Sets up the tests.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            this._testSaveData = new MemorySaveData();
        }
        
        /// <summary>
        /// Tests getting an unpopulated value.
        /// </summary>
        [Test]
        public void TestGetUnpopulated()
        {
            Assert.That(this._testSaveData.Get<double>("testKey") == default);
        }
        
        /// <summary>
        /// Tests setting an unpopulated value.
        /// </summary>
        [Test]
        public void TestSetNew()
        {
            this._testSaveData.SetAsync<double>("testKey", 2);
            Assert.That(this._testSaveData.Get<double>("testKey"), Is.EqualTo(2));
        }
        
        /// <summary>
        /// Tests setting an existing value.
        /// </summary>
        [Test]
        public void TestSetExisting()
        {
            this._testSaveData.SetAsync<double>("testKey", 2);
            this._testSaveData.SetAsync<int>("testKey", 3);
            Assert.That(this._testSaveData.Get<int>("testKey"), Is.EqualTo(3));
        }
        
        /// <summary>
        /// Tests setting an existing value to null.
        /// </summary>
        [Test]
        public void TestSetRemove()
        {
            this._testSaveData.SetAsync<double>("testKey", 2);
            Assert.That(this._testSaveData.Get<double>("testKey"), Is.EqualTo(2));
            this._testSaveData.SetAsync<double?>("testKey", null);
            Assert.That(this._testSaveData.Get<double>("testKey") == default);
        }
        
        /// <summary>
        /// Tests clearing the data.
        /// </summary>
        [Test]
        public void TestClear()
        {
            this._testSaveData.SetAsync<double>("testKey", 2);
            Assert.That(this._testSaveData.Get<double>("testKey"), Is.EqualTo(2));
            this._testSaveData.Clear();
            Assert.That(this._testSaveData.Get<double>("testKey") == default);
        }
    }
}