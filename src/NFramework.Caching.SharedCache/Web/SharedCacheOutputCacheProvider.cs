using NSoft.NFramework.Web;

namespace NSoft.NFramework.Caching.SharedCache.Web {
    /// <summary>
    /// .NET 4.0 이상에서 ASP.NET Page의 OutputCache를 SharedCache에 저장합니다.
    /// 참고:
    ///      http://www.4guysfromrolla.com/articles/061610-1.aspx
    ///      http://weblogs.asp.net/gunnarpeipman/archive/2009/11/19/asp-net-4-0-writing-custom-output-cache-providers.aspx
    ///      http://weblogs.asp.net/scottgu/archive/2010/01/27/extensible-output-caching-with-asp-net-4-vs-2010-and-net-4-0-series.aspx
    /// </summary>
    /// <example>
    /// <code>
    /// <system.web>
    ///		<compilation debug="true" targetFramework="4.0"/>
    ///		<caching>
    ///			<outputCache defaultProvider="SharedCacheOutputCacheProvider">
    ///				<providers>
    ///					<add name="SharedCacheOutputCacheProvider" type="NSoft.NFramework.Caching.SharedCache.Web.SharedCacheOutputCacheProvider, NSoft.NFramework.Caching.SharedCache"/>
    ///				</providers>
    ///			</outputCache>
    ///		</caching>
    /// </system.web>
    /// </code>
    /// </example>
    public class SharedCacheOutputCacheProvider : AbstractOutputCacheProvider {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public SharedCacheOutputCacheProvider() : base(() => new SharedCacheRepository()) {}
    }

    //public class SharedCacheOutputCacheProvider : OutputCacheProvider
    //{
    //    #region << logger >>

    //    private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

    //    private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

    //    #endregion

    //    private static readonly object _syncLock = new object();
    //    private static ICacheRepository _cacheRepository;

    //    /// <summary>
    //    /// Memcached 캐시 서버를 저장소로 사용하는 Cache Repository 입니다.
    //    /// </summary>
    //    public static ICacheRepository CacheRepository
    //    {
    //        get
    //        {
    //            if(_cacheRepository == null)
    //                lock(_syncLock)
    //                    if(_cacheRepository == null)
    //                    {
    //                        var cacheRepository = new SharedCacheRepository();
    //                        Thread.MemoryBarrier();
    //                        _cacheRepository = cacheRepository;
    //                    }
    //            return _cacheRepository;
    //        }
    //    }

    //    /// <summary>
    //    /// 출력 캐시에서 지정된 항목에 대한 참조를 반환합니다.
    //    /// </summary>
    //    /// <returns>
    //    /// 캐시에서 지정된 항목을 식별하는 <paramref name="key"/> 값이거나 캐시에 지정된 항목이 없는 경우 null입니다.
    //    /// </returns>
    //    /// <param name="key">출력 캐시에서 캐시된 항목에 대한 고유 식별자입니다. </param>
    //    public override object Get(string key)
    //    {
    //        key.ShouldNotBeWhiteSpace("key");

    //        if(IsDebugEnabled)
    //            log.Debug("ASP.NET Page OutputCache를 로드합니다... key=[{0}]", key);

    //        return CacheRepository.Get(key);

    //        //object result;
    //        //IndexusDistributionCache.SharedCache.TryGet(key, out result);
    //        //return result;
    //    }

    //    /// <summary>
    //    /// 지정된 항목을 출력 캐시에 삽입합니다. 
    //    /// </summary>
    //    /// <returns>
    //    /// 지정된 공급자에 대한 참조입니다. 
    //    /// </returns>
    //    /// <param name="key"><paramref name="entry"/>에 대한 고유 식별자입니다.</param><param name="entry">출력 캐시에 추가할 내용입니다.</param>
    //    /// <param name="utcExpiry">캐시된 항목이 만료되는 날짜와 시간입니다.</param>
    //    public override object Add(string key, object entry, DateTime utcExpiry)
    //    {
    //        key.ShouldNotBeWhiteSpace("key");

    //        if(IsDebugEnabled)
    //            log.Debug("ASP.NET Page OutputCache를 캐시에 추가합니다... key=[{0}]", key);

    //        CacheRepository.Set(key, entry, utcExpiry.ToLocalTime().Subtract(DateTime.Now));
    //        return Get(key);

    //        //if(IndexusDistributionCache.SharedCache.ExistsKey(key))
    //        //    return Get(key);

    //        //IndexusDistributionCache.SharedCache.TryAdd(key, entry, utcExpiry.ToLocalTime());
    //        //return entry;
    //    }

    //    /// <summary>
    //    /// 지정된 항목을 출력 캐시에 삽입하고 이미 캐시되어 있는 경우 해당 항목을 덮어씁니다.
    //    /// </summary>
    //    /// <param name="key"><paramref name="entry"/>에 대한 고유 식별자입니다.</param><param name="entry">출력 캐시에 추가할 내용입니다.</param>
    //    /// <param name="utcExpiry">캐시된 <paramref name="entry"/>가 만료되는 날짜와 시간입니다.</param>
    //    public override void Set(string key, object entry, DateTime utcExpiry)
    //    {
    //        key.ShouldNotBeWhiteSpace("key");

    //        if(IsDebugEnabled)
    //            log.Debug("ASP.NET Page OutputCache를 저장합니다. 기존에 존재하면 갱신합니다... key=[{0}]", key);

    //        CacheRepository.Set(key, entry, utcExpiry.ToLocalTime().Subtract(DateTime.Now));
    //        // IndexusDistributionCache.SharedCache.TryAdd(key, entry, utcExpiry.ToLocalTime());
    //    }

    //    /// <summary>
    //    /// 출력 캐시에서 지정된 항목을 제거합니다.
    //    /// </summary>
    //    /// <param name="key">출력 캐시에서 제거할 항목에 대한 고유 식별자입니다. </param>
    //    public override void Remove(string key)
    //    {
    //        if(key.IsWhiteSpace())
    //            return;

    //        if(IsDebugEnabled)
    //            log.Debug("ASP.NET Page OutputCache를 제거합니다... key=[{0}]", key);

    //        CacheRepository.Remove(key);
    //    }
    //}
}