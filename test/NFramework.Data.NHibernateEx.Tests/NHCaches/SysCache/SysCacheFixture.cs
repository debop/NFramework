using System.Collections.Generic;
using NHibernate.Cache;
using NSoft.NFramework.Data.NHibernateEx.NHCaches.SysCache;
using NUnit.Framework;

namespace NSoft.NFramework.Data.NHCaches.SysCache {
    [TestFixture]
    public class SysCacheFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private SysCacheProvider _provider;
        private Dictionary<string, string> _props;

        [TestFixtureSetUp]
        public void FixtureSetUp() {
            _props = new Dictionary<string, string>
                     {
                         { "expiration", 120.ToString() },
                         { "priority", 4.ToString() }
                     };


            _provider = new SysCacheProvider();
        }

        [Test]
        public void Put() {
            const string key = "key1";
            const string value = "value";

            ICache cache = _provider.BuildCache("NSoft.NFramework", _props);
            Assert.IsNotNull(cache);

            Assert.IsNull(cache.Get(key));

            cache.Put(key, value);

            object item = cache.Get(key);

            Assert.IsNotNull(item);
            Assert.AreEqual(value, item);
        }
    }
}