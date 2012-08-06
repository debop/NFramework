using System;
using System.Collections.Generic;
using System.Threading;
using NHibernate.Cache;
using NUnit.Framework;

namespace NSoft.NFramework.Caching.Memcached.NHCaches {
    [TestFixture]
    public class MemcachedCacheClientFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        private MemcachedCacheProvider _provider;
        private Dictionary<string, string> _props;

        private static readonly object _syncLock = new object();

        [TestFixtureSetUp]
        public void FixtureSetUp() {
            _props = new Dictionary<string, string>();
            _provider = new MemcachedCacheProvider();

            _provider.Start(_props);
        }

        [TestFixtureTearDown]
        public void FixtureTearDown() {
            _provider.Stop();
        }

        [Test]
        public void Clear_Cache_Value() {
            const string key = "key1";
            const string value = "value";

            ICache cache = _provider.BuildCache("mbunit", _props);
            Assert.IsNotNull(cache, "no cache returned");

            lock(_syncLock) {
                // 캐시에 저장
                cache.Put(key, value);
                Thread.Sleep(100);

                // 캐시에 저장되었는지 확인
                var item = cache.Get(key);
                Assert.IsNotNull(item, "캐시에서 정보를 찾을 수 없습니다. key=" + key);

                // 캐시 정보 모두 삭제
                // cache.Clear();
                cache.Remove(key);

                item = cache.Get(key);
                Assert.IsNull(item, "캐시에서 정보를 아직 있습니다. key=" + key);
            }
        }

        [Test]
        public void Can_DefaultConstructor() {
            var cache = new MemcachedCacheClient();
            Assert.IsNotNull(cache);
        }

        [Test]
        public void Build_With_Empty_Properties() {
            var cache = _provider.BuildCache("rcl", new Dictionary<string, string>());
            Assert.IsNotNull(cache);
        }

        [Test]
        public void Build_Without_Properties() {
            var cache = new MemcachedCacheClient("rcl");
            Assert.IsNotNull(cache);
        }

        [Test]
        public void Retrive_By_Null_Key_Return_Null() {
            var cache = new MemcachedCacheClient();
            cache.Put("rcl", "value");

            Thread.Sleep(100);

            object item = cache.Get(null);
            Assert.IsNull(item);
        }

        [Test]
        public void Put_Null_Key() {
            Assert.Throws<ArgumentNullException>(() => new MemcachedCacheClient().Put(null, "value"));
        }

        [Test]
        public void Put_Null_Value() {
            Assert.Throws<ArgumentNullException>(() => new MemcachedCacheClient().Put("key", null));
        }

        [Test]
        public void Remove_Null_Key() {
            var cache = new MemcachedCacheClient();
            cache.Remove(null);
        }

        [Test]
        public void Put_And_Get() {
            const string key = "key1";
            const string value = "value";

            ICache cache = _provider.BuildCache("rcl", _props);
            Assert.IsNotNull(cache, "no cache returned");

            lock(_syncLock) {
                cache.Remove(key);
                Assert.IsNull(cache.Get(key));

                // 캐시에 저장
                cache.Put(key, value);
                Thread.Sleep(100);

                // 캐시에 저장되었는지 확인
                var item = cache.Get(key);
                Assert.IsNotNull(item, "캐시에서 정보를 찾을 수 없습니다. key=" + key);
            }
        }

        [Test]
        public void Put_By_Region() {
            const string key = "key";
            const string s1 = "test1";
            const string s2 = "test2";

            var cache1 = _provider.BuildCache("rcl1", _props);
            var cache2 = _provider.BuildCache("rcl2", _props);

            lock(_syncLock) {
                cache1.Put(key, s1);
                cache2.Put(key, s2);

                Thread.Sleep(100);

                Assert.AreNotEqual(cache1.Get(key), cache2.Get(key));
            }
        }

        [Test]
        public void Remove() {
            const string key = "key1";
            const string value = "value";

            var cache = _provider.BuildCache("rcl", _props);
            Assert.IsNotNull(cache, "no cache returned");

            lock(_syncLock) {
                // add the item
                cache.Put(key, value);
                Thread.Sleep(100);

                // make sure it's there
                var item = cache.Get(key);
                Assert.IsNotNull(item, "item just added is not there");

                // remove it
                cache.Remove(key);

                // make sure it's not there
                item = cache.Get(key);
                Assert.IsNull(item, "item still exists in cache");
            }
        }
    }
}