using NSoft.NFramework.Web;

namespace NSoft.NFramework.Caching.Memcached.Web {
    /// <summary>
    /// ASP.NET 웹 Application의 세션 정보를 Memcached 캐시 서버에 저장해주는 클래스입니다. <br />
    /// 참고 : https://github.com/enyim/EnyimMemcached/wiki/MemcachedClient-Usage
    /// 참고 : http://msdn.microsoft.com/en-us/library/ms178589.aspx
    /// </summary>
    /// <example>
    /// <code>
    /// // web.config 에서 다음과 같이 환경 설정하면 됩니다.
    /// <system.web>
    /// 	<sessionState cookieless="true" regenerateExpiredSessionId="true" mode="Custom" customProvider="MemcachedSessionStateStoreProvider">
    /// 		<providers>
    /// 			<add name="MemcachedSessionStateStoreProvider" type="NSoft.NFramework.Memcached.Web.MemcachedSessionStateStoreProvider, NSoft.NFramework.Memcached" />
    /// 		</providers>
    /// 	</sessionState>
    /// </system.web>
    /// </code>
    /// </example>
    public class MemcachedSessionStateStoreProvider : AbstractSessionStateStoreProvider {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public MemcachedSessionStateStoreProvider() : base(() => new MemcachedRepository()) {}
    }

    //public class MemcachedSessionStateStoreProvider : SessionStateStoreProviderBase
    //{
    //    #region << logger >>

    //    private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
    //    private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

    //    #endregion

    //    private const string SessionStateSectionNode = @"system.web/sessionState";

    //    private TimeSpan _sessionTimeout;
    //    private SessionStateSection _sessionStateSection;

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
    //                        var cacheReppository = new MemcachedRepository();
    //                        Thread.MemoryBarrier();
    //                        _cacheRepository = cacheReppository;
    //                    }
    //            return _cacheRepository;
    //        }
    //    }

    //    public override SessionStateStoreData CreateNewStoreData(HttpContext context, int timeout)
    //    {
    //        var staticObjects = SessionStateUtility.GetSessionStaticObjects(context);
    //        return new SessionStateStoreData(new SessionStateItemCollection(), staticObjects, timeout);
    //    }

    //    public override void CreateUninitializedItem(HttpContext context, string id, int timeout)
    //    {
    //        var dummy = CreateNewStoreData(context, timeout);
    //        SaveSessionStateStoreData(context, id, dummy, TimeSpan.FromMinutes(timeout));
    //    }

    //    public override void EndRequest(HttpContext context)
    //    {
    //        // Nothing to do.
    //    }

    //    public override SessionStateStoreData GetItem(HttpContext context, string id, out bool locked, out TimeSpan lockAge, out object lockId,
    //                                                  out SessionStateActions actions)
    //    {
    //        return GetSessionStoreItem(context, id, out locked, out lockAge, out lockId, out actions);
    //    }

    //    public override SessionStateStoreData GetItemExclusive(HttpContext context, string id, out bool locked, out TimeSpan lockAge, out object lockId,
    //                                                           out SessionStateActions actions)
    //    {
    //        return GetSessionStoreItem(context, id, out locked, out lockAge, out lockId, out actions);
    //    }

    //    public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
    //    {
    //        if(IsDebugEnabled)
    //            log.Debug("MemcachedSessionStateStoreProvider를 초기화를 시작합니다... name=[{0}], config=[{1}]", name, config.CollectionToString());

    //        if(name.IsWhiteSpace())
    //            name = GetType().Name;

    //        base.Initialize(name, config);

    //        var applicationName = System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath;
    //        _sessionStateSection = (SessionStateSection) WebConfigurationManager.OpenWebConfiguration(applicationName).GetSection(SessionStateSectionNode);

    //        _sessionTimeout = _sessionStateSection.Timeout;

    //        if(log.IsInfoEnabled)
    //            log.Info("MemcachedSessionStateStoreProvider 초기화를 완료했습니다!!! applicationName=[{0}], _sessionStateSection=[{1}]",
    //                     applicationName, _sessionStateSection);
    //    }

    //    public override void InitializeRequest(HttpContext context)
    //    {
    //        if(IsDebugEnabled)
    //            log.Debug("요청을 초기화 합니다...");
    //    }

    //    public override void ReleaseItemExclusive(HttpContext context, string id, object lockId)
    //    {
    //        if(IsDebugEnabled)
    //            log.Debug("배타적 잠금을 해제합니다. id=[{0}], lockId=[{1}]", id, lockId);
    //    }

    //    public override void RemoveItem(HttpContext context, string id, object lockId, SessionStateStoreData item)
    //    {
    //        RemoveSessionStateStoreData(id);
    //    }

    //    public override void ResetItemTimeout(HttpContext context, string id)
    //    {
    //        if(IsDebugEnabled)
    //            log.Debug("세션 정보의 Timeout을 재설정합니다. id=[{0}]", id);

    //        var data = LoadSessionStateStoreData(context, id, _sessionTimeout);
    //        SaveSessionStateStoreData(context, id, data, _sessionTimeout);
    //    }

    //    public override bool SetItemExpireCallback(SessionStateItemExpireCallback expireCallback)
    //    {
    //        return false;
    //    }

    //    public override void SetAndReleaseItemExclusive(HttpContext context, string id, SessionStateStoreData item, object lockId, bool newItem)
    //    {
    //        if(IsDebugEnabled)
    //            log.Debug("세션 정보를 저장합니다... session id=[{0}], item=[{1}]", id, item);

    //        SaveSessionStateStoreData(context, id, item, _sessionTimeout);
    //    }

    //    public bool IsDisposed { get; protected set; }

    //    public override void Dispose()
    //    {
    //        if(IsDisposed)
    //            return;

    //        if(IsDebugEnabled)
    //            log.Debug("MemcachedSessionStateStoreProvider 의 리소스를 해제했습니다.");

    //        IsDisposed = true;
    //    }

    //    private SessionStateStoreData GetSessionStoreItem(HttpContext context,
    //                                                      string id,
    //                                                      out bool locked,
    //                                                      out TimeSpan lockAge,
    //                                                      out object lockId,
    //                                                      out SessionStateActions actionFlags)
    //    {
    //        SessionStateStoreData item = null;
    //        locked = false;
    //        lockAge = TimeSpan.Zero;
    //        lockId = null;
    //        actionFlags = SessionStateActions.None;

    //        if(IsDebugEnabled)
    //            log.Debug("캐시에서 세션 정보를 로드합니다. id=[{0}]", id);

    //        try
    //        {
    //            item = LoadSessionStateStoreData(context, id, _sessionTimeout);
    //        }
    //        catch(Exception ex)
    //        {
    //            if(log.IsWarnEnabled)
    //            {
    //                log.Warn("캐시에서 세션 정보를 로드하는데 실패했습니다. session id=" + id);
    //                log.Warn(ex);
    //            }
    //        }

    //        return item ?? CreateNewStoreData(context, (int) _sessionTimeout.TotalMinutes);
    //    }

    //    private void SaveSessionStateStoreData(HttpContext context, string id, SessionStateStoreData sessionStateStoreData, TimeSpan sessionTimeout)
    //    {
    //        if(IsDebugEnabled)
    //            log.Debug("세션 정보를 캐시에 저장합니다... id=[{0}], sessionTimeout=[{1}]", id, sessionTimeout);

    //        try
    //        {
    //            byte[] cacheItem = null;

    //            // NOTE: SessionStateItemCollection 자체적으로 제공하는 SerializeSessionState/Deserialize를 사용해야 합니다. 
    //            // NOTE: SessionStateItemCollection 은 SerializableAttribte 가 정의되어 있지 않아 일반적인 방식으로는 직렬화를 수행할 수 없습니다.
    //            //
    //            if(sessionStateStoreData != null && sessionStateStoreData.Items != null)
    //                cacheItem = WebTool.SerializeSessionState((SessionStateItemCollection) sessionStateStoreData.Items);

    //            CacheRepository.Set(id, cacheItem ?? new byte[0], sessionTimeout);

    //            if(IsDebugEnabled)
    //                log.Debug("세션 정보를 캐시에 저장했습니다!!! id=[{0}], sessionTimeout=[{1}]", id, sessionTimeout);
    //        }
    //        catch(Exception ex)
    //        {
    //            if(log.IsErrorEnabled)
    //            {
    //                log.Error("캐시에 세션 정보를 저장하는데 실패했습니다. id=[{0}]", id);
    //                log.Error(ex);
    //            }
    //        }
    //    }

    //    private SessionStateStoreData LoadSessionStateStoreData(HttpContext context, string id, TimeSpan sessionTimeout)
    //    {
    //        if(IsDebugEnabled)
    //            log.Debug("세션 정보를 캐시에서 로드합니다... id=[{0}]", id);

    //        try
    //        {
    //            SessionStateItemCollection itemCollection = null;

    //            var bytes = CacheRepository.Get(id) as byte[];

    //            if(bytes != null)
    //            {
    //                itemCollection = WebTool.DeserializeSessionState(bytes);

    //                if(IsDebugEnabled)
    //                    log.Debug("세션 정보를 캐시에서 로드했습니다!!! id=[{0}]", id);
    //            }

    //            return new SessionStateStoreData(itemCollection ?? new SessionStateItemCollection(),
    //                                             SessionStateUtility.GetSessionStaticObjects(context),
    //                                             (int) sessionTimeout.TotalMinutes);
    //        }
    //        catch(Exception ex)
    //        {
    //            if(log.IsWarnEnabled)
    //            {
    //                log.Warn("캐시에 세션 정보를 로드하는데 실패했습니다. id=[{0}]", id);
    //                log.Warn(ex);
    //            }
    //        }
    //        return null;
    //    }

    //    private void RemoveSessionStateStoreData(string id)
    //    {
    //        id.ShouldNotBeWhiteSpace("id");

    //        if(IsDebugEnabled)
    //            log.Debug("세션 정보를 캐시에서 삭제합니다... session id=[{0}]", id);

    //        CacheRepository.Remove(id);

    //        if(IsDebugEnabled)
    //            log.Debug("세션 정보를 캐시에서 삭제했습니다!!! session id=[{0}]", id);
    //    }
    //}
}