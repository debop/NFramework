using System;
using NSoft.NFramework.Json;
using SharedCache.WinServiceCommon.Provider.Cache;

namespace NSoft.NFramework.Caching.SharedCache {
    /// <summary>
    /// SharedCache 를 캐시 저장소로 사용하는 Repository 입니다. 
    /// <see cref="AbstractCacheRepository.Serializer"/>를 <see cref="BsonSerializer"/> 등을 사용하게 되면, 저장 항목이 <see cref="SerializableAttribute"/>로 지정되지 않아도 됩니다.
    /// </summary>
    public class SharedCacheRepository : AbstractCacheRepository {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        // public static TimeSpan DefaultExpiry = TimeSpan.FromSeconds(120);

        public SharedCacheRepository() : this(BsonSerializer.Instance) {}
        public SharedCacheRepository(ISerializer serializer) : base(serializer ?? BsonSerializer.Instance) {}

        /// <summary>
        /// 캐시에 저장된 항목을 반환합니다.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override object Get(string key) {
            key.ShouldNotBeWhiteSpace("key");

            if(IsDebugEnabled)
                log.Debug("Shared Cache 에서 키[{0}]에 해당하는 값을 조회합니다.", key);

            var item = IndexusDistributionCache.SharedCache.Get(key);
            var isSerialized = (Serializer != null) && (item is CacheItem);

            if(isSerialized == false)
                return item;

            var cacheItem = (CacheItem)item;

            return
                (Serializer is IJsonSerializer)
                    ? ((IJsonSerializer)Serializer).Deserialize(cacheItem.ItemData, cacheItem.ItemType)
                    : Serializer.Deserialize(cacheItem.ItemData);
        }

        /// <summary>
        /// 캐시에 항목을 저장합니다.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="item"></param>
        /// <param name="validFor"></param>
        public override void Set(string key, object item, TimeSpan validFor = default (TimeSpan)) {
            key.ShouldNotBeWhiteSpace("key");

            var expires = DateTime.Now.Add((validFor == default(TimeSpan)) ? Expiry : validFor);

            if(IsDebugEnabled)
                log.Debug("Shared Cache에 값을 저장합니다. key=[{0}],item=[{1}],expires=[{2}]", key, item, expires);

            IndexusDistributionCache.SharedCache.Add(key,
                                                     Serializer != null ? CreateCacheItem(this, item) : item,
                                                     expires);
        }

        /// <summary>
        /// 캐시에서 항목을 제거합니다.
        /// </summary>
        /// <param name="key"></param>
        public override void Remove(string key) {
            key.ShouldNotBeWhiteSpace("key");

            if(IsDebugEnabled)
                log.Debug("캐시의 키[{0}]에 해당하는 값을 삭제합니다.", key);

            IndexusDistributionCache.SharedCache.Remove(key);
        }

        /// <summary>
        /// 캐시의 모든 항목을 삭제합니다.
        /// </summary>
        public override void Clear() {
            if(IsDebugEnabled)
                log.Debug("캐시의 모든 항목을 삭제합니다");

            IndexusDistributionCache.SharedCache.Clear();
        }

        private static CacheItem CreateCacheItem(SharedCacheRepository repository, object item) {
            return
                new CacheItem
                {
                    ItemType = item.GetType(),
                    ItemData = repository.Serializer.Serialize(item)
                };
        }
    }
}