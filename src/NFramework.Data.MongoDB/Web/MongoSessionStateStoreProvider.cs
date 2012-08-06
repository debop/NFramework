using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.SessionState;
using Castle.Core;
using MongoDB.Driver.Builders;
using NSoft.NFramework.InversionOfControl;
using NSoft.NFramework.Reflections;
using NSoft.NFramework.Tools;
using NSoft.NFramework.Web.Tools;

namespace NSoft.NFramework.Data.MongoDB.Web {
    /// <summary>
    /// SessionState 정보를 MongoDB 시스템에 저장하도록 합니다.
    /// 예제 : http://msdn.microsoft.com/en-us/library/ms178589.aspx
    /// </summary>
    /// <example>
    /// <code>
    /// // web.config 에서 다음과 같이 환경 설정하면 됩니다.
    /// <system.web>
    /// 	<sessionState cookieless="true" regenerateExpiredSessionId="true" mode="Custom" customProvider="MongoSessionStateStoreProvider">
    /// 		<providers>
    /// 			<add name="MongoSessionStateStoreProvider" type="NSoft.NFramework.Data.MongoDB.Web.MongoSessionStateStoreProvider, NSoft.NFramework.Data.MongoDB" />
    /// 		</providers>
    /// 	</sessionState>
    /// </system.web>
    /// </code>
    /// </example>
    public class MongoSessionStateStoreProvider : SessionStateStoreProviderBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// Session 상태 정보가 저장될 Database의 기본 ConnectionString
        /// </summary>
        public const string ConnectionString = @"server=localhost;database=AspSessionState;safe=true";

        private readonly object _syncLock = new object();

        public static IMongoRepository CreateMongoRepository() {
            var repository = (IoC.IsInitialized)
                                 ? IoC.TryResolve<IMongoRepository>(() => new MongoRepositoryImpl(ConnectionString),
                                                                    true,
                                                                    LifestyleType.Thread)
                                 : new MongoRepositoryImpl(ConnectionString);

            repository.CollectionName = System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath.Replace("/", "_");
            return repository;
        }

        private TimeSpan _sessionTimeout;
        private SessionStateSection _sessionStateSection;

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

        public override void CreateUninitializedItem(HttpContext context, string id, int timeout) {
            var dummy = CreateNewStoreData(context, timeout);

            using(var repository = CreateMongoRepository())
                SaveSessionStateStoreData(context, repository, id, dummy, TimeSpan.FromMinutes(timeout));
        }

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

        public override SessionStateStoreData GetItemExclusive(HttpContext context, string id, out bool locked, out TimeSpan lockAge,
                                                               out object lockId,
                                                               out SessionStateActions actions) {
            return GetSessionStoreItem(context, id, out locked, out lockAge, out lockId, out actions);
        }

        /// <summary>
        /// 공급자를 초기화합니다.
        /// </summary>
        /// <param name="name">공급자의 이름입니다.</param><param name="config">이 공급자에 대해 구성에 지정된 공급자별 특성을 나타내는 이름/값 쌍의 컬렉션입니다.</param>
        /// <exception cref="T:System.ArgumentNullException">공급자 이름이 null인 경우</exception>
        /// <exception cref="T:System.ArgumentException">공급자 이름의 길이가 0인 경우</exception>
        /// <exception cref="T:System.InvalidOperationException">공급자가 이미 초기화된 후 공급자에 대해 <see cref="M:System.Configuration.Provider.ProviderBase.Initialize(System.String,System.Collections.Specialized.NameValueCollection)"/>를 호출하려고 한 경우</exception>
        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config) {
            if(IsDebugEnabled)
                log.Debug("MongoSessionStateStoreProvider를 초기화를 시작합니다... name=[{0}], config=[{1}]", name, config.CollectionToString());

            if(name.IsWhiteSpace())
                name = GetType().Name;

            base.Initialize(name, config);

            var applicationName = System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath;
            _sessionStateSection =
                (SessionStateSection)WebConfigurationManager.OpenWebConfiguration(applicationName).GetSection("system.web/sessionState");

            _sessionTimeout = _sessionStateSection.Timeout;

            if(log.IsInfoEnabled)
                log.Info(
                    "MongoSessionStateStoreProvider 초기화를 완료했습니다!!! applicationName=[{0}], sessionStateSection=[{1}], sessionTimeout=[{2}]",
                    applicationName, _sessionStateSection, _sessionTimeout);
        }

        /// <summary>
        /// 요청별 초기화를 위해 <see cref="T:System.Web.SessionState.SessionStateModule"/> 개체에 의해 호출됩니다.
        /// </summary>
        /// <param name="context">현재 요청에 대한 <see cref="T:System.Web.HttpContext"/>입니다.</param>
        public override void InitializeRequest(HttpContext context) {
            if(IsDebugEnabled)
                log.Debug("요청을 초기화 합니다...");
        }

        public override void ReleaseItemExclusive(HttpContext context, string id, object lockId) {
            if(IsDebugEnabled)
                log.Debug("배타적 잠금을 해제합니다. id=[{0}], lockId=[{1}]", id, lockId);
        }

        /// <summary>
        /// 세션 데이터 저장소에서 항목 데이터를 삭제합니다.
        /// </summary>
        /// <param name="context">현재 요청에 대한 <see cref="T:System.Web.HttpContext"/>입니다.</param>
        /// <param name="id">현재 요청에 대한 세션 식별자입니다.</param><param name="lockId">현재 요청에 대한 잠금 식별자입니다.</param>
        /// <param name="item">데이터 저장소에서 삭제할 항목을 나타내는 <see cref="T:System.Web.SessionState.SessionStateStoreData"/>입니다.</param>
        public override void RemoveItem(HttpContext context, string id, object lockId, SessionStateStoreData item) {
            using(var repository = CreateMongoRepository())
                RemoveSessionStateStoreData(repository, id);
        }

        public override void ResetItemTimeout(HttpContext context, string id) {
            if(IsDebugEnabled)
                log.Debug("세션 정보의 Timeout을 재설정합니다. id=[{0}]", id);

            using(var repository = CreateMongoRepository()) {
                var data = LoadSessionStateStoreData(context, repository, id, _sessionTimeout);
                SaveSessionStateStoreData(context, repository, id, data, _sessionTimeout);
            }
        }

        public override bool SetItemExpireCallback(SessionStateItemExpireCallback expireCallback) {
            return false;
        }

        public override void SetAndReleaseItemExclusive(HttpContext context, string id, SessionStateStoreData item, object lockId,
                                                        bool newItem) {
            if(IsDebugEnabled)
                log.Debug("세션 정보를 저장합니다... session id=[{0}], item=[{1}]", id, item);

            using(var repository = CreateMongoRepository())
                SaveSessionStateStoreData(context, repository, id, item, _sessionTimeout);
        }

        public bool IsDisposed { get; protected set; }

        public override void Dispose() {
            if(IsDisposed)
                return;

            if(IsDebugEnabled)
                log.Debug("MongoSessionStateStoreProvider 의 리소스를 해제했습니다.");

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
                using(var repository = CreateMongoRepository())
                    item = LoadSessionStateStoreData(context, repository, id, _sessionTimeout);
            }
            catch(Exception ex) {
                if(log.IsWarnEnabled) {
                    log.Warn("캐시에서 세션 정보를 로드하는데 실패했습니다. session id=[{0}]", id);
                    log.Warn(ex);
                }
            }

            return item ?? CreateNewStoreData(context, (int)_sessionTimeout.TotalMinutes);
        }

        private static void SaveSessionStateStoreData(HttpContext context, IMongoRepository repository, string id,
                                                      SessionStateStoreData sessionStateStoreData, TimeSpan sessionTimeout) {
            if(IsDebugEnabled)
                log.Debug("세션 정보를 캐시에 저장합니다... id=[{0}], sessionTimeout=[{1}]", id, sessionTimeout);

            With.TryAction(() => RemoveExpires(repository));

            try {
                byte[] cacheItem = null;

                // NOTE: SessionStateItemCollection 자체적으로 제공하는 Serialize/Deserialize를 사용해야 합니다. 
                //       SessionStateItemCollection 은 SerializableAttribte 가 정의되어 있지 않아 일반적인 방식으로는 직렬화를 수행할 수 없습니다.
                //
                if(sessionStateStoreData != null && sessionStateStoreData.Items != null)
                    cacheItem = WebTool.SerializeSessionState((SessionStateItemCollection)sessionStateStoreData.Items);

                var result = repository.Save(new MongoSessionStateEntry(id, cacheItem, DateTime.UtcNow.Add(sessionTimeout)));

                if(IsDebugEnabled)
                    log.Debug("세션 정보를 캐시에 저장했습니다!!! id=[{0}], 저장결과=[{1}], sessionTimeout=[{2}]", id, result.Ok, sessionTimeout);
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled) {
                    log.Error("캐시에 세션 정보를 저장하는데 실패했습니다. id=[{0}]", id);
                    log.Error(ex);
                }
            }
        }

        private static SessionStateStoreData LoadSessionStateStoreData(HttpContext context, IMongoRepository repository, string id,
                                                                       TimeSpan sessionTimeout) {
            if(IsDebugEnabled)
                log.Debug("세션 정보를 캐시에서 로드합니다... id=[{0}]", id);

            With.TryAction(() => RemoveExpires(repository));

            try {
                SessionStateItemCollection itemCollection = null;

                var entry = repository.FindOneByIdAs<MongoSessionStateEntry>(id);
                if(entry != null && entry.State != null) {
                    itemCollection = WebTool.DeserializeSessionState(entry.State);

                    if(IsDebugEnabled)
                        log.Debug("세션 정보를 캐시에서 로드했습니다!!! id=[{0}]", id);
                }

                return new SessionStateStoreData(itemCollection ?? new SessionStateItemCollection(),
                                                 SessionStateUtility.GetSessionStaticObjects(context),
                                                 (int)sessionTimeout.TotalMinutes);
            }
            catch(Exception ex) {
                if(log.IsWarnEnabled) {
                    log.Warn("MongDB에서 세션 정보를 로드하는데 실패했습니다. id=[{0}]", id);
                    log.Warn(ex);
                }
            }
            return null;
        }

        private static void RemoveSessionStateStoreData(IMongoRepository repository, string id) {
            id.ShouldNotBeWhiteSpace("id");

            if(IsDebugEnabled)
                log.Debug("세션 정보를 삭제합니다... session id=[{0}]", id);

            var removed = repository.RemoveByIdAs<MongoSessionStateEntry>(id);

            if(IsDebugEnabled)
                log.Debug("세션 정보를 삭제했습니다!!! session id=[{0}], 삭제 여부=[{1}]", id, removed.Ok);

            With.TryAction(() => RemoveExpires(repository));
        }

        private static void RemoveExpires(IMongoRepository repository) {
            repository.ShouldNotBeNull("repository");

            var query = Query.LTE("UtcExpiry", DateTime.UtcNow.ToMongoDateTime());
            Task.Factory.StartNew(() => With.TryAction(() => repository.Remove(query)),
                                  TaskCreationOptions.LongRunning);
        }
    }
}