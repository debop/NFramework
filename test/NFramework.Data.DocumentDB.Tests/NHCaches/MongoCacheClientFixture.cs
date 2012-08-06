using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Cache;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Data.MongoDB.NHCaches {
    [TestFixture]
    public class MongoCacheClientFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        private const string DefaultConnectionString = MongoTool.DefaultConnectionString;

        private MongoCacheProvider _provider;
        private Dictionary<string, string> _props;

        [TestFixtureSetUp]
        public void FixtureSetUp() {
            _props = new Dictionary<string, string>
                     {
                         { "connectionString", DefaultConnectionString },
                         { "expiration", "00:00:03" },
                         { "compressThreshold", "4096" }
                     };

            _provider = new MongoCacheProvider();
        }

        [SetUp]
        public void SetUp() {
            var cache = _provider.BuildCache("nunit", _props);
            cache.Clear();
        }

        [Test]
        public void Can_Put() {
            const string key = "key1";
            const string value = "value, some value 입니다.";

            ICache cache = _provider.BuildCache("nunit", _props);

            Assert.IsNotNull(cache);
            cache.Remove(key);
            Assert.IsNull(cache.Get(key));

            cache.Put(key, value);

            object loaded = cache.Get(key);
            Assert.IsNotNull(loaded, "not founded!!!");
            Assert.AreEqual(value, loaded, "loaded=" + loaded);
        }

        [Test]
        public void Can_Remove() {
            const string key = "key1";
            const string value = "value, some value 입니다.";

            ICache cache = _provider.BuildCache("nunit", _props);

            Assert.IsNotNull(cache);
            Assert.IsNull(cache.Get(key));

            cache.Put(key, value);

            object loaded = cache.Get(key);
            Assert.IsNotNull(loaded, "not founded!!!");
            Assert.AreEqual(value, loaded, "loaded=" + loaded);

            cache.Remove(key);

            var removed = cache.Get(key);
            Assert.IsNull(removed);
        }

        [Test]
        public void Can_Clear() {
            const int KeyCount = 100;

            ICache cache = _provider.BuildCache("nunit", _props);

            var keys = Enumerable.Range(0, KeyCount).ToList();

            keys.ForEach(i => cache.Put("Key_" + i.ToString(), "value"));

            keys.TrueForAll(i => cache.Get("Key_" + i.ToString()) != null).Should().Be.True();

            cache.Clear();

            keys.TrueForAll(i => cache.Get("Key_" + i.ToString()) == null).Should().Be.True();
        }

        [Test]
        public void Can_Expiration() {
            const string key = "key1";
            const string value = "value, some value 입니다.";

            ICache cache = _provider.BuildCache("nunit", _props);

            // 아직 캐시에 저장하지 않았음.
            Assert.IsNull(cache.Get(key));

            // 캐시에 값을 저장합니다.
            cache.Put(key, value);

            // 보관하자마자 얻으니 값이 있어야 한다.
            Assert.IsNotNull(cache.Get(key));

            // 유효기간 지나도록 기다린다.
            System.Threading.Thread.Sleep(TimeSpan.Parse(_props["expiration"]).TotalMilliseconds.AsInt(3000) + 1000);

            // Expired 되어서 null이 반환되어야 합니다.
            Assert.IsNull(cache.Get(key));
        }
    }
}