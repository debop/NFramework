using System;
using System.Web;
using System.Web.Configuration;
using System.Web.SessionState;
using NSoft.NFramework.InversionOfControl;
using NSoft.NFramework.Reflections;
using NSoft.NFramework.Tools;
using NSoft.NFramework.Web.Caching;
using NSoft.NFramework.Web.Tools;

namespace NSoft.NFramework.Web {
    /// <summary>
    /// ASP.NET 웹 Application의 세션 정보를  캐시 서버에 저장해주는 클래스입니다. <br />
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
    public abstract class AbstractSessionStateStoreProvider : SessionStateStoreProviderBase {
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

        private const string SessionStateSectionNode = @"system.web/sessionState";

        private TimeSpan _sessionTimeout;
        private SessionStateSection _sessionStateSection;

        /// <summary>
        /// 생성자
        /// </summary>
        protected AbstractSessionStateStoreProvider() : this(_defaultCacheRepositoryFactory) {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="repositoryFactory"></param>
        protected AbstractSessionStateStoreProvider(Func<ICacheRepository> repositoryFactory) {
            repositoryFactory.ShouldNotBeNull("repositoryFactory");

            CacheRepository = repositoryFactory.Invoke();
        }

        /// <summary>
        /// Cache Repository
        /// </summary>
        public ICacheRepository CacheRepository { get; protected set; }

        /// <summary>
        /// 현재 요청에 사용할 새 <see cref="T:System.Web.SessionState.SessionStateStoreData"/> 개체를 만듭니다.
        /// </summary>
        /// <returns>
        /// 현재 요청에 대한 새 <see cref="T:System.Web.SessionState.SessionStateStoreData"/>입니다.
        /// </returns>
        /// <param name="context">현재 요청에 대한 <see cref="T:System.Web.HttpContext"/>입니다.</param><param name="timeout">새 <see cref="T:System.Web.SessionState.SessionStateStoreData"/>의 세션 상태 <see cref="P:System.Web.SessionState.HttpSessionState.Timeout"/> 값입니다.</param>
        public override SessionStateStoreData CreateNewStoreData(HttpContext context, int timeout) {
            var staticObjects = SessionStateUtility.GetSessionStaticObjects(context);
            return new SessionStateStoreData(new SessionStateItemCollection(), staticObjects, timeout);
        }

        /// <summary>
        /// 데이터 저장소에 새 세션 상태 항목을 추가합니다.
        /// </summary>
        /// <param name="context">현재 요청에 대한 <see cref="T:System.Web.HttpContext"/>입니다.</param><param name="id">현재 요청에 대한 <see cref="P:System.Web.SessionState.HttpSessionState.SessionID"/>입니다.</param><param name="timeout">현재 요청에 대한 세션 <see cref="P:System.Web.SessionState.HttpSessionState.Timeout"/>입니다.</param>
        public override void CreateUninitializedItem(HttpContext context, string id, int timeout) {
            var dummy = CreateNewStoreData(context, timeout);
            SaveSessionStateStoreData(context, id, dummy, TimeSpan.FromMinutes(timeout));
        }

        /// <summary>
        /// 요청이 끝날 때 <see cref="T:System.Web.SessionState.SessionStateModule"/> 개체에 의해 호출됩니다.
        /// </summary>
        /// <param name="context">현재 요청에 대한 <see cref="T:System.Web.HttpContext"/>입니다.</param>
        public override void EndRequest(HttpContext context) {
            // Nothing to do.
        }

        /// <summary>
        /// 세션 데이터 저장소에서 읽기 전용 세션 상태 데이터를 반환합니다.
        /// </summary>
        /// <returns>
        /// 세션 데이터 저장소의 세션 값과 정보로 채워진 <see cref="T:System.Web.SessionState.SessionStateStoreData"/>입니다.
        /// </returns>
        /// <param name="context">현재 요청에 대한 <see cref="T:System.Web.HttpContext"/>입니다.</param><param name="id">현재 요청에 대한 <see cref="P:System.Web.SessionState.HttpSessionState.SessionID"/>입니다.</param><param name="locked">이 메서드가 반환될 때 요청된 세션 항목이 세션 데이터 저장소에서 잠겨 있으면 true로 설정된 부울 값이 포함되고, 그렇지 않으면 false로 설정된 부울 값이 포함됩니다.</param><param name="lockAge">이 메서드가 반환될 때 세션 데이터 저장소의 항목이 잠겨 있는 시간으로 설정된 <see cref="T:System.TimeSpan"/> 개체가 포함됩니다.</param><param name="lockId">이 메서드가 반환될 때 현재 요청에 대한 잠금 식별자로 설정된 개체가 포함됩니다.잠금 식별자에 대한 자세한 내용은 <see cref="T:System.Web.SessionState.SessionStateStoreProviderBase"/> 클래스 요약에서 "세션 저장소 데이터 잠금"을 참조하십시오.</param><param name="actions">이 메서드가 반환될 때 현재 세션이 초기화되지 않은 쿠키 없는 세션인지 여부를 나타내는 <see cref="T:System.Web.SessionState.SessionStateActions"/> 값 중 하나가 포함됩니다.</param>
        public override SessionStateStoreData GetItem(HttpContext context, string id, out bool locked, out TimeSpan lockAge,
                                                      out object lockId,
                                                      out SessionStateActions actions) {
            return GetSessionStoreItem(context, id, out locked, out lockAge, out lockId, out actions);
        }

        /// <summary>
        /// 세션 데이터 저장소에서 읽기 전용 세션 상태 데이터를 반환합니다.
        /// </summary>
        /// <returns>
        /// 세션 데이터 저장소의 세션 값과 정보로 채워진 <see cref="T:System.Web.SessionState.SessionStateStoreData"/>입니다.
        /// </returns>
        /// <param name="context">현재 요청에 대한 <see cref="T:System.Web.HttpContext"/>입니다.</param>
        /// <param name="id">현재 요청에 대한 <see cref="P:System.Web.SessionState.HttpSessionState.SessionID"/>입니다.</param><
        /// param name="locked">이 메서드가 반환될 때 성공적으로 잠기면 true로 설정된 부울 값이 포함되고, 그렇지 않으면 false로 설정된 부울 값이 포함됩니다.</param>
        /// <param name="lockAge">이 메서드가 반환될 때 세션 데이터 저장소의 항목이 잠겨 있는 시간으로 설정된 <see cref="T:System.TimeSpan"/> 개체가 포함됩니다.</param>
        /// <param name="lockId">이 메서드가 반환될 때 현재 요청에 대한 잠금 식별자로 설정된 개체가 포함됩니다.
        /// 잠금 식별자에 대한 자세한 내용은 <see cref="T:System.Web.SessionState.SessionStateStoreProviderBase"/> 클래스 요약에서 "세션 저장소 데이터 잠금"을 참조하십시오.</param>
        /// <param name="actions">이 메서드가 반환될 때 현재 세션이 초기화되지 않은 쿠키 없는 세션인지 여부를 나타내는 <see cref="T:System.Web.SessionState.SessionStateActions"/> 값 중 하나가 포함됩니다.</param>
        public override SessionStateStoreData GetItemExclusive(HttpContext context,
                                                               string id,
                                                               out bool locked,
                                                               out TimeSpan lockAge,
                                                               out object lockId,
                                                               out SessionStateActions actions) {
            return GetSessionStoreItem(context, id, out locked, out lockAge, out lockId, out actions);
        }

        /// <summary>
        /// 공급자를 초기화합니다.
        /// </summary>
        /// <param name="name">공급자의 이름입니다.</param><param name="config">이 공급자에 대해 구성에 지정된 공급자별 특성을 나타내는 이름/값 쌍의 컬렉션입니다.</param>
        /// <exception cref="T:System.ArgumentNullException">공급자 이름이 null인 경우</exception><exception cref="T:System.ArgumentException">공급자 이름의 길이가 0인 경우</exception>
        /// <exception cref="T:System.InvalidOperationException">공급자가 이미 초기화된 후 공급자에 대해 <see cref="M:System.Configuration.Provider.ProviderBase.Initialize(System.String,System.Collections.Specialized.NameValueCollection)"/>를 호출하려고 한 경우</exception>
        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config) {
            if(IsDebugEnabled)
                log.Debug("MemcachedSessionStateStoreProvider를 초기화를 시작합니다... name=[{0}], config=[{1}]", name,
                          config.CollectionToString());

            if(name.IsWhiteSpace())
                name = GetType().Name;

            base.Initialize(name, config);

            var applicationName = System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath;
            _sessionStateSection =
                (SessionStateSection)WebConfigurationManager.OpenWebConfiguration(applicationName).GetSection(SessionStateSectionNode);

            _sessionTimeout = _sessionStateSection.Timeout;

            if(log.IsInfoEnabled)
                log.Info("MemcachedSessionStateStoreProvider 초기화를 완료했습니다!!! applicationName=[{0}], _sessionStateSection=[{1}]",
                         applicationName, _sessionStateSection);
        }

        /// <summary>
        /// 요청별 초기화를 위해 <see cref="T:System.Web.SessionState.SessionStateModule"/> 개체에 의해 호출됩니다.
        /// </summary>
        /// <param name="context">현재 요청에 대한 <see cref="T:System.Web.HttpContext"/>입니다.</param>
        public override void InitializeRequest(HttpContext context) {
            if(IsDebugEnabled)
                log.Debug("요청을 초기화 합니다...");
        }

        /// <summary>
        /// 세션 데이터 저장소의 항목에 대한 잠금을 해제합니다.
        /// </summary>
        /// <param name="context">현재 요청에 대한 <see cref="T:System.Web.HttpContext"/>입니다.</param><param name="id">현재 요청에 대한 세션 식별자입니다.</param><param name="lockId">현재 요청에 대한 잠금 식별자입니다. </param>
        public override void ReleaseItemExclusive(HttpContext context, string id, object lockId) {
            if(IsDebugEnabled)
                log.Debug("배타적 잠금을 해제합니다. id=[{0}], lockId=[{1}]", id, lockId);
        }

        /// <summary>
        /// 세션 데이터 저장소에서 항목 데이터를 삭제합니다.
        /// </summary>
        /// <param name="context">현재 요청에 대한 <see cref="T:System.Web.HttpContext"/>입니다.</param><param name="id">현재 요청에 대한 세션 식별자입니다.</param><param name="lockId">현재 요청에 대한 잠금 식별자입니다.</param><param name="item">데이터 저장소에서 삭제할 항목을 나타내는 <see cref="T:System.Web.SessionState.SessionStateStoreData"/>입니다.</param>
        public override void RemoveItem(HttpContext context, string id, object lockId, SessionStateStoreData item) {
            RemoveSessionStateStoreData(id);
        }

        /// <summary>
        /// 세션 데이터 저장소에 있는 항목의 만료 날짜와 시간을 업데이트합니다.
        /// </summary>
        /// <param name="context">현재 요청에 대한 <see cref="T:System.Web.HttpContext"/>입니다.</param><param name="id">현재 요청에 대한 세션 식별자입니다.</param>
        public override void ResetItemTimeout(HttpContext context, string id) {
            if(IsDebugEnabled)
                log.Debug("세션 정보의 Timeout을 재설정합니다. id=[{0}]", id);

            var data = LoadSessionStateStoreData(context, id, _sessionTimeout);
            SaveSessionStateStoreData(context, id, data, _sessionTimeout);
        }

        /// <summary>
        /// Global.asax 파일에 정의된 Session_OnEnd 이벤트의 <see cref="T:System.Web.SessionState.SessionStateItemExpireCallback"/> 대리자에 대한 참조를 설정합니다.
        /// </summary>
        /// <returns>
        /// 세션 상태 저장소 공급자가 Session_OnEnd 이벤트의 호출을 지원하면 true이고, 그렇지 않으면 false입니다.
        /// </returns>
        /// <param name="expireCallback">Global.asax 파일에 정의된 Session_OnEnd 이벤트의 <see cref="T:System.Web.SessionState.SessionStateItemExpireCallback"/> 대리자입니다.</param>
        public override bool SetItemExpireCallback(SessionStateItemExpireCallback expireCallback) {
            return false;
        }

        /// <summary>
        /// 세션 상태 데이터 저장소의 세션 항목 정보를 현재 요청의 값으로 업데이트하고 데이터에 대한 잠금을 해제합니다.
        /// </summary>
        /// <param name="context">현재 요청에 대한 <see cref="T:System.Web.HttpContext"/>입니다.</param><param name="id">현재 요청에 대한 세션 식별자입니다.</param><param name="item">저장할 현재 세션 값이 포함된 <see cref="T:System.Web.SessionState.SessionStateStoreData"/> 개체입니다.</param><param name="lockId">현재 요청에 대한 잠금 식별자입니다. </param><param name="newItem">세션 항목이 새 항목임을 확인하면 true이고, 세션 항목이 기존 항목임을 확인하면 false입니다.</param>
        public override void SetAndReleaseItemExclusive(HttpContext context, string id, SessionStateStoreData item, object lockId,
                                                        bool newItem) {
            if(IsDebugEnabled)
                log.Debug("세션 정보를 저장합니다... session id=[{0}], item=[{1}]", id, item);

            SaveSessionStateStoreData(context, id, item, _sessionTimeout);
        }

        public bool IsDisposed { get; protected set; }

        public override void Dispose() {
            if(IsDisposed)
                return;

            if(IsDebugEnabled)
                log.Debug("[{0}] 의 리소스를 해제했습니다.", GetType().FullName);

            IsDisposed = true;
        }

        private SessionStateStoreData GetSessionStoreItem(HttpContext context,
                                                          string id,
                                                          out bool locked,
                                                          out TimeSpan lockAge,
                                                          out object lockId,
                                                          out SessionStateActions actionFlags) {
            SessionStateStoreData item = null;
            locked = false;
            lockAge = TimeSpan.Zero;
            lockId = null;
            actionFlags = SessionStateActions.None;

            if(IsDebugEnabled)
                log.Debug("캐시에서 세션 정보를 로드합니다. id=[{0}]", id);

            try {
                item = LoadSessionStateStoreData(context, id, _sessionTimeout);
            }
            catch(Exception ex) {
                if(log.IsWarnEnabled) {
                    log.Warn("캐시에서 세션 정보를 로드하는데 실패했습니다. session id=" + id);
                    log.Warn(ex);
                }
            }

            return item ?? CreateNewStoreData(context, (int)_sessionTimeout.TotalMinutes);
        }

        private void SaveSessionStateStoreData(HttpContext context,
                                               string id,
                                               SessionStateStoreData sessionStateStoreData,
                                               TimeSpan sessionTimeout) {
            if(IsDebugEnabled)
                log.Debug("세션 정보를 캐시에 저장합니다... id=[{0}], sessionTimeout=[{1}]", id, sessionTimeout);

            try {
                byte[] cacheItem = null;

                // NOTE: SessionStateItemCollection 자체적으로 제공하는 SerializeSessionState/Deserialize를 사용해야 합니다. 
                // NOTE: SessionStateItemCollection 은 SerializableAttribte 가 정의되어 있지 않아 일반적인 방식으로는 직렬화를 수행할 수 없습니다.
                //
                if(sessionStateStoreData != null && sessionStateStoreData.Items != null)
                    cacheItem = WebTool.SerializeSessionState((SessionStateItemCollection)sessionStateStoreData.Items);

                CacheRepository.Set(id, cacheItem ?? new byte[0], sessionTimeout);

                if(IsDebugEnabled)
                    log.Debug("세션 정보를 캐시에 저장했습니다!!! id=[{0}], sessionTimeout=[{1}]", id, sessionTimeout);
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled) {
                    log.Error("캐시에 세션 정보를 저장하는데 실패했습니다. id=[{0}]", id);
                    log.Error(ex);
                }
            }
        }

        private SessionStateStoreData LoadSessionStateStoreData(HttpContext context,
                                                                string id,
                                                                TimeSpan sessionTimeout) {
            if(IsDebugEnabled)
                log.Debug("세션 정보를 캐시에서 로드합니다... id=[{0}]", id);

            try {
                SessionStateItemCollection itemCollection = null;

                var bytes = CacheRepository.Get(id) as byte[];

                if(bytes != null) {
                    itemCollection = WebTool.DeserializeSessionState(bytes);

                    if(IsDebugEnabled)
                        log.Debug("세션 정보를 캐시에서 로드했습니다!!! id=[{0}]", id);
                }

                return new SessionStateStoreData(itemCollection ?? new SessionStateItemCollection(),
                                                 SessionStateUtility.GetSessionStaticObjects(context),
                                                 (int)sessionTimeout.TotalMinutes);
            }
            catch(Exception ex) {
                if(log.IsWarnEnabled) {
                    log.Warn("캐시에 세션 정보를 로드하는데 실패했습니다. id=[{0}]", id);
                    log.Warn(ex);
                }
            }
            return null;
        }

        private void RemoveSessionStateStoreData(string id) {
            id.ShouldNotBeWhiteSpace("id");

            if(IsDebugEnabled)
                log.Debug("세션 정보를 캐시에서 삭제합니다... session id=[{0}]", id);

            CacheRepository.Remove(id);

            if(IsDebugEnabled)
                log.Debug("세션 정보를 캐시에서 삭제했습니다!!! session id=[{0}]", id);
        }
    }
}