using System;

namespace NSoft.NFramework.Caching {

    /// <summary>
    /// 캐시 저장소 관리의 기본 클래스입니다.
    /// </summary>
    public abstract class AbstractCacheRepository : ICacheRepository {

        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 최소 유효 기간 (1분)
        /// </summary>
        public static readonly TimeSpan MinExpiry = TimeSpan.FromMinutes(1);

        /// <summary>
        /// 기본 유효 기간 (2시간)
        /// </summary>
        public static readonly TimeSpan DefaultExpiry = TimeSpan.FromHours(2);

        private TimeSpan _expiry = DefaultExpiry;

        /// <summary>
        /// 생성자
        /// </summary>
        protected AbstractCacheRepository() : this(null, default(TimeSpan)) {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="serializer">객체 Serializer</param>
        /// <param name="expiry">유효기간</param>
        protected AbstractCacheRepository(ISerializer serializer = null, TimeSpan expiry = default(TimeSpan)) {
            Serializer = serializer;
            Expiry = (expiry != default(TimeSpan)) ? expiry : DefaultExpiry;
        }

        /// <summary>
        /// 캐시 항목의 유효 기간
        /// </summary>
        public TimeSpan Expiry {
            get { return _expiry; }
            set { _expiry = (value > MinExpiry) ? value : MinExpiry; }
        }

        /// <summary>
        /// 캐시에 저장할 객체에 대해 직렬화/역직화를 수행할 경우에 사용합니다.
        /// </summary>
        public ISerializer Serializer { get; protected set; }

        /// <summary>
        /// 캐시에 저장된 항목을 반환합니다.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public abstract object Get(string key);

        /// <summary>
        /// 캐시에 항목을 저장합니다.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="item"></param>
        /// <param name="validFor"></param>
        public abstract void Set(string key, object item, TimeSpan validFor = default(TimeSpan));

        /// <summary>
        /// 캐시에서 항목을 제거합니다.
        /// </summary>
        /// <param name="key"></param>
        public abstract void Remove(string key);

        /// <summary>
        /// 캐시의 모든 항목을 삭제합니다.
        /// </summary>
        public abstract void Clear();
    }
}