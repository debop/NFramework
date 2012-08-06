using System;
using System.Web;
using System.Web.Caching;
using NSoft.NFramework.Caching;

namespace NSoft.NFramework.Web.Caching {
    /// <summary>
    ///	<see cref="System.Web.Caching.Cache"/>를 저장소로 사용하는 CacheRepository
    /// </summary>
    public class SysCacheRepository : AbstractCacheRepository {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private static Cache _cache;
        private static readonly object _syncLock = new object();

        /// <summary>
        /// 생성자
        /// </summary>
        public SysCacheRepository() : this(null) {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="serializer">객체 Serializer</param>
        /// <param name="expiry">유효기간</param>
        public SysCacheRepository(ISerializer serializer = null, TimeSpan expiry = default(TimeSpan)) : base(serializer, expiry) {}

        public Cache Cache {
            get {
                if(HttpContext.Current != null)
                    return HttpContext.Current.Cache;
                return _cache ?? (_cache = new Cache());
            }
        }

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

            lock(_syncLock)
                item = Cache.Get(key);

            var isSerialized = (Serializer != null) && (item != null) && (item is byte[]);

            return isSerialized ? Serializer.Deserialize((byte[])item) : item;
        }

        /// <summary>
        /// 캐시에 항목을 저장합니다.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="item"></param>
        /// <param name="validFor"></param>
        public override void Set(string key, object item, TimeSpan validFor) {
            key.ShouldNotBeWhiteSpace("key");

            if(IsDebugEnabled)
                log.Debug("캐시에 값을 저장합니다. key=[{0}], item=[{1}], Expiry=[{2}]", key, item, validFor);

            object value = (Serializer != null) ? Serializer.Serialize(item) : item;

            lock(_syncLock)
                Cache.Insert(key, value, null, Cache.NoAbsoluteExpiration, validFor);
        }

        /// <summary>
        /// 캐시에서 항목을 제거합니다.
        /// </summary>
        /// <param name="key"></param>
        public override void Remove(string key) {
            key.ShouldNotBeWhiteSpace("key");

            if(IsDebugEnabled)
                log.Debug("캐시의 키[{0}]에 해당하는 값을 삭제합니다...", key);

            lock(_syncLock)
                Cache.Remove(key);
        }

        /// <summary>
        /// 캐시의 모든 항목을 삭제합니다.
        /// </summary>
        public override void Clear() {
            if(IsDebugEnabled)
                log.Debug("캐시의 모든 항목을 삭제합니다...");

            lock(_syncLock)
                _cache = new Cache();
        }
    }
}