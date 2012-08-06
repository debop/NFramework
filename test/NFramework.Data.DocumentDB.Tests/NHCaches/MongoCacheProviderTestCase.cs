using System.Collections.Generic;
using NHibernate.Cache;
using NUnit.Framework;

namespace NSoft.NFramework.Data.MongoDB.NHCaches {
    [TestFixture]
    public class MongoCacheProviderTestCase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        private MongoCacheProvider _provider;
        private Dictionary<string, string> _props;

        [TestFixtureSetUp]
        public void FixtureSetUp() {
            _props = new Dictionary<string, string>
                     {
                         { "connectionString", "mongodb://localhost/?safe=true" },
                         { "database", "NSoft.NFrameworkTest" },
                         { "expiration", 3600.ToString() },
                         { "compressThreshold", 1024.ToString() }
                     };
        }

        [SetUp]
        public void SetUp() {
            _provider = new MongoCacheProvider();
        }

        [Test]
        public void Build_Cache_From_AppConfig() {
            ICache cache = _provider.BuildCache("NSoft.NFramework", null);
            Assert.IsNotNull(cache);
            Assert.AreEqual("NSoft.NFramework", cache.RegionName);
        }

        [Test]
        public void Build_CacheClient_With_NullRegionName_NullProperties() {
            ICache cache = _provider.BuildCache(null, null);
            Assert.IsNotNull(cache);
            Assert.IsEmpty(cache.RegionName);
        }

        [Test]
        public void Build_CacheClient_From_Dictionary() {
            var cache = _provider.BuildCache("another", _props);
            Assert.IsNotNull(cache);
        }

        [Test]
        public void Build_CacheClient_From_EmptyProperties() {
            var cache = _provider.BuildCache("another", null);
            Assert.IsNotNull(cache);
        }

        [Test]
        public void Provider_NextTimestamp() {
            long stamp = _provider.NextTimestamp();
            Assert.IsNotNull(stamp, "no timstamp returned");
        }
    }
}