using System.Collections.Generic;
using NHibernate.Cache;
using NUnit.Framework;

namespace NSoft.NFramework.Caching.SharedCache.NHCaches {
    [TestFixture]
    public class SharedCacheProviderTestCase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        private SharedCacheProvider _provider;
        private Dictionary<string, string> _props;

        [SetUp]
        public void SetUp() {
            _provider = new SharedCacheProvider();

            _props = new Dictionary<string, string>
                     {
                         { "expiration", "00:01:00" },
                         { "compressThreshold", "40960" }
                     };
        }

        [Test]
        public void Build_Cache_From_AppConfig() {
            const string Region = @"NSoft.NFramework.Caching.SharedCache";

            ICache cache = _provider.BuildCache(Region, null);
            Assert.IsNotNull(cache);
            Assert.AreEqual(Region, cache.RegionName);
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