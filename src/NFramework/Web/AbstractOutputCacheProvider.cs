using System;
using System.Web.Caching;
using NSoft.NFramework.InversionOfControl;
using NSoft.NFramework.Web.Caching;

namespace NSoft.NFramework.Web {
    /// <summary>
    /// .NET 4.0 이상에서 ASP.NET Page의 OutputCache를 <see cref="CacheRepository"/>를 통해 저장/로드됩니다.
    /// 참고:
    ///      http://www.4guysfromrolla.com/articles/061610-1.aspx
    ///      http://weblogs.asp.net/gunnarpeipman/archive/2009/11/19/asp-net-4-0-writing-custom-output-cache-providers.aspx
    ///      http://weblogs.asp.net/scottgu/archive/2010/01/27/extensible-output-caching-with-asp-net-4-vs-2010-and-net-4-0-series.aspx
    /// </summary>
    public abstract class AbstractOutputCacheProvider : OutputCacheProvider, IOutputCacheProvider {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private static readonly Func<ICacheRepository> _defaultCacheRepositoryFactory =
            () => {
                ICacheRepository repository = null;
                try {
                    if(IoC.IsInitialized)
                        repository = IoC.Resolve<ICacheRepository>();
                }
                catch(Exception ex) {
                    if(log.IsWarnEnabled)
                        log.WarnException("IoC로부터 ICacheRepository를 Resolve하는데 실패했습니다.", ex);
                }

                if(repository == null)
                    repository = new SysCacheRepository();

                return repository;
            };

        protected AbstractOutputCacheProvider() : this(_defaultCacheRepositoryFactory) {}

        protected AbstractOutputCacheProvider(Func<ICacheRepository> cacheRepositoryFactory) {
            cacheRepositoryFactory.ShouldNotBeNull("cacheRepositoryFactory");

            CacheRepository = cacheRepositoryFactory();
        }

        /// <summary>
        /// 실제 캐시 저장소에 데이타를 저장/조회하는 API를 제공하는 Repository입니다.
        /// </summary>
        public ICacheRepository CacheRepository { get; protected set; }

        /// <summary>
        /// 출력 캐시에서 지정된 항목에 대한 참조를 반환합니다.
        /// </summary>
        /// <returns>
        /// 캐시에서 지정된 항목을 식별하는 <paramref name="key"/> 값이거나 캐시에 지정된 항목이 없는 경우 null입니다.
        /// </returns>
        /// <param name="key">출력 캐시에서 캐시된 항목에 대한 고유 식별자입니다. </param>
        public override object Get(string key) {
            if(IsDebugEnabled)
                log.Debug("ASP.NET Page OutputCache를 로드합니다... key=[{0}]", key);

            return CacheRepository.Get(key);
        }

        /// <summary>
        /// 지정된 항목을 출력 캐시에 삽입합니다. 
        /// </summary>
        /// <returns>
        /// 지정된 공급자에 대한 참조입니다. 
        /// </returns>
        /// <param name="key"><paramref name="entry"/>에 대한 고유 식별자입니다.</param><param name="entry">출력 캐시에 추가할 내용입니다.</param>
        /// <param name="utcExpiry">캐시된 항목이 만료되는 날짜와 시간입니다.</param>
        public override object Add(string key, object entry, DateTime utcExpiry) {
            if(IsDebugEnabled)
                log.Debug("ASP.NET Page OutputCache를 캐시에 추가합니다. key=[{0}], utcExpiry=[{1}]", key, utcExpiry);

            CacheRepository.Set(key, entry, utcExpiry.Subtract(DateTime.UtcNow));

            return entry;
        }

        /// <summary>
        /// 지정된 항목을 출력 캐시에 삽입하고 이미 캐시되어 있는 경우 해당 항목을 덮어씁니다.
        /// </summary>
        /// <param name="key"><paramref name="entry"/>에 대한 고유 식별자입니다.</param><param name="entry">출력 캐시에 추가할 내용입니다.</param>
        /// <param name="utcExpiry">캐시된 <paramref name="entry"/>가 만료되는 날짜와 시간입니다.</param>
        public override void Set(string key, object entry, DateTime utcExpiry) {
            if(IsDebugEnabled)
                log.Debug("ASP.NET Page OutputCache를 캐시에 설정합니다. key=[{0}], utcExpiry=[{1}]", key, utcExpiry);

            CacheRepository.Set(key, entry, utcExpiry.Subtract(DateTime.UtcNow));
        }

        /// <summary>
        /// 출력 캐시에서 지정된 항목을 제거합니다.
        /// </summary>
        /// <param name="key">출력 캐시에서 제거할 항목에 대한 고유 식별자입니다. </param>
        public override void Remove(string key) {
            if(IsDebugEnabled)
                log.Debug("ASP.NET Page OutputCache를 삭제합니다. key=[{0}]", key);

            CacheRepository.Remove(key);
        }
    }
}