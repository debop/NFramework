using Enyim.Caching;
using Enyim.Caching.Memcached;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Caching.Memcached.Memcached {
    [TestFixture]
    public class MemcachedClientFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private MemcachedClient Client;

        [TestFixtureSetUp]
        public void FixtureSetUp() {
            Client = new MemcachedClient();
        }

        [SetUp]
        public void SetUp() {
            Client.FlushAll();
        }

        [TestFixtureTearDown]
        public void FixtureTearDown() {
            if(Client != null)
                Client.Dispose();
        }

        [Test]
        public void ConfigurationTest() {
            using(var client = new MemcachedClient()) {
                client.ShouldNotBeNull("client");

                //client.Cas(StoreMode.Set, "item1", 1);
                //client.Cas(StoreMode.Set, "item2", 2);
            }
        }

        [Test]
        public void StoreAndGet() {
            Client.Store(StoreMode.Set, "Test", "Hello World").Should().Be.True();
            Client.Get("Test").Should().Be("Hello World");
        }

        [Test]
        public void StoreModeTest() {
            // StoreMode.Set 은 Insert Or Update 기능을 수행합니다.
            //
            Client.Store(StoreMode.Set, "Item1", 1).Should().Be.True();
            Client.Store(StoreMode.Set, "Item2", 2).Should().Be.True();

            Client.Get("Item1").Should().Be(1);
            Client.Get("Item2").Should().Be(2);

            // StoreMode.Add 는 Key가 없을 때만 추가합니다. (Insert)
            //
            Client.Store(StoreMode.Add, "Item1", 4).Should().Be.False();
            Client.Get("Item1").Should().Be(1);

            // StoreMode.Replace는 기존에 Key가 있는 경우에만 설정이 가능하다. (Update)
            Client.Store(StoreMode.Replace, "Item1", 4).Should().Be.True();
            Client.Get("Item1").Should().Be(4);

            Client.Store(StoreMode.Replace, "Item3", 100).Should().Be.False();
            Client.Get("Item3").Should().Be.Null();
        }
    }
}