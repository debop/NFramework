using Enyim.Caching;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Caching.Memcached {
    [TestFixture]
    public class MemcachedToolFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private MemcachedClient Client;

        [TestFixtureSetUp]
        public void FixtureSetUp() {
            Client = new MemcachedClient();
        }

        [TestFixtureTearDown]
        public void FixtureTearDown() {
            if(Client != null)
                Client.Dispose();
        }

        [Test]
        public void StatisticsTest() {
            Client.FlushAll();

            Client.TrySetValue("x", "안녕하세요").Should().Be.True();

            object value;
            Client.TryGetValue("x", out value).Should().Be.True();

            Client.FlushAll();
        }
    }
}