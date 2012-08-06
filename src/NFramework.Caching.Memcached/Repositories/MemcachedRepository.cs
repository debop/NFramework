using System;
using Enyim.Caching;
using Enyim.Caching.Memcached;
using NSoft.NFramework.Json;

namespace NSoft.NFramework.Caching.Memcached {
    /// <summary>
    /// Memcached 캐시 서버를 저장소로 사용하는 Cache Repository 입니다.
    /// 참고: https://github.com/enyim/EnyimMemcached/wiki/MemcachedClient-Usage
    /// </summary>
    public class MemcachedRepository : AbstractCacheRepository {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public MemcachedRepository() : base() { }

        public MemcachedRepository(ISerializer serializer) : base(serializer) { }

        private MemcachedClient _client;

        public MemcachedClient Client {
            get { return _client ?? (_client = new MemcachedClient()); }
            set { _client = value; }
        }

        /// <summary>
        /// 캐시에 저장된 항목을 반환합니다.
        /// </summary>
        /// <param name="key">캐시 키</param>
        /// <returns>캐시 값</returns>
        public override object Get(string key) {
            key.ShouldNotBeWhiteSpace("key");

            if(IsDebugEnabled)
                log.Debug("캐시에서 키[{0}]에 해당하는 값을 조회합니다.", key);

            var item = Client.Get(key);
            var isSerialized = (Serializer != null) && (item is CacheItem);

            if(isSerialized == false)
                return item;

            var cacheItem = (CacheItem)item;

            if(Serializer is IJsonSerializer)
                return ((IJsonSerializer)Serializer).Deserialize(cacheItem.ItemData, cacheItem.ItemType);

            return Serializer.Deserialize(cacheItem.ItemData);
        }

        /// <summary>
        /// 캐시에 항목을 저장합니다.
        /// </summary>
        /// <param name="key">캐시 키</param>
        /// <param name="item">캐시 값</param>
        /// <param name="validFor">캐시에 저장할 기간</param>
        public override void Set(string key, object item, TimeSpan validFor = default(TimeSpan)) {

            key.ShouldNotBeWhiteSpace("key");

            if(IsDebugEnabled)
                log.Debug("캐시에 값을 저장합니다. key=[{0}], item=[{1}], Expiry=[{2}]", key, item, validFor);

            // BUG: MemcachedClient는 유효기간을 지정하면 저장이 되지 않습니다.
            //
            if(Serializer != null)
                Client.Store(StoreMode.Set, key, CreateCacheItem(this, item));
            else
                Client.Store(StoreMode.Set, key, item);
        }

        /// <summary>
        /// 캐시에서 항목을 제거합니다.
        /// </summary>
        /// <param name="key">캐시 키</param>
        public override void Remove(string key) {

            key.ShouldNotBeWhiteSpace("key");
            if(IsDebugEnabled)
                log.Debug("캐시에 저장된 항목을 삭제합니다... key=[{0}]", key);

            Client.Remove(key);
        }

        /// <summary>
        /// 캐시의 모든 항목을 삭제합니다.
        /// </summary>
        public override void Clear() {

            if(IsDebugEnabled)
                log.Debug("캐시에 저장된 모든 항목을 삭제합니다...");

            Client.FlushAll();
        }

        private static CacheItem CreateCacheItem(MemcachedRepository repository, object item) {
            return
                new CacheItem {
                    ItemType = item.GetType(),
                    ItemData = repository.Serializer.Serialize(item)
                };
        }

        /// <summary>
        /// 캐시로 저장되는 정보
        /// </summary>
        [Serializable]
        private class CacheItem : ValueObjectBase {

            public Type ItemType { get; set; }

            public byte[] ItemData { get; set; }

            public override int GetHashCode() {
                return HashTool.Compute(ItemType, ItemData);
            }
        }
    }
}