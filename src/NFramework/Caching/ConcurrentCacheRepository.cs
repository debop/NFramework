using System;
using System.Collections.Concurrent;

namespace NSoft.NFramework.Caching {
    /// <summary>
    /// <see cref="ConcurrentDictionary{TKey,TValue}" /> 를 캐시 저장소로 사용하는 CacheRepository입니다.
    /// </summary>
    public class ConcurrentCacheRepository : AbstractCacheRepository {

        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 저장소
        /// </summary>
        private static readonly ConcurrentDictionary<string, object> _cache = new ConcurrentDictionary<string, object>();

        /// <summary>
        /// 생성자
        /// </summary>
        public ConcurrentCacheRepository() : this(null) {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="serializer">객체 Serializer</param>
        /// <param name="expiry">유효기간</param>
        public ConcurrentCacheRepository(ISerializer serializer = null, TimeSpan expiry = default(TimeSpan)) : base(serializer, expiry) {}

        /// <summary>
        /// 캐시에 저장된 항목을 반환합니다.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override object Get(string key) {
            key.ShouldNotBeWhiteSpace("key");

            if(IsDebugEnabled)
                log.Debug("캐시에서 키[{0}]에 해당하는 값을 조회합니다.", key);

            object item;
            if(_cache.TryGetValue(key, out item)) {
                var isSerialized = (Serializer != null) && (item != null) && (item is byte[]);
                return isSerialized ? Serializer.Deserialize((byte[])item) : item;
            }
            return null;
        }

        /// <summary>
        /// 캐시에 항목을 저장합니다.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="item"></param>
        /// <param name="validFor"></param>
        public override void Set(string key, object item, TimeSpan validFor = default(TimeSpan)) {
            key.ShouldNotBeWhiteSpace("key");

            if(IsDebugEnabled)
                log.Debug("캐시에 값을 저장합니다. key=[{0}], item=[{1}], validFor=[{2}]", key, item, validFor);

            object value = (Serializer != null) ? Serializer.Serialize(item) : item;

            _cache.AddOrUpdate(key, value, (k, v) => value);
        }

        /// <summary>
        /// 캐시에서 항목을 제거합니다.
        /// </summary>
        /// <param name="key"></param>
        public override void Remove(string key) {
            key.ShouldNotBeWhiteSpace("key");

            if(IsDebugEnabled)
                log.Debug("캐시의 키[{0}]에 해당하는 값을 삭제합니다...", key);

            object item;
            _cache.TryRemove(key, out item);
        }

        /// <summary>
        /// 캐시의 모든 항목을 삭제합니다.
        /// </summary>
        public override void Clear() {
            if(IsDebugEnabled)
                log.Debug("캐시의 모든 항목을 삭제합니다...");

            _cache.Clear();
        }
    }
}