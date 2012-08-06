using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using NHibernate;
using NHibernate.Engine;
using NSoft.NFramework.InversionOfControl;
using NSoft.NFramework.Reflections;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Data.NHibernateEx {
    /// <summary>
    /// Multiple UnitOfWorkFactory
    /// </summary>
    public class NHMultipleUnitOfWorkFactory : List<NHUnitOfWorkFactory>, IUnitOfWorkFactory {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;
        private static readonly bool IsInfoEnabled = log.IsInfoEnabled;

        #endregion

        /// <summary>
        /// Exception message of User Provided connection
        /// </summary>
        public const string USER_PROVIDED_CONNECTION_EXCEPTION_MESSAGE
            = @"NHMultipleUnitOfWorkFactory does not support user supplied connections " +
              @"because it cannot associate the connection to a factory.";

        /// <summary>
        /// Current NHibernate Session Key for long conversation
        /// </summary>
        public const string CurrentNHibernateSessionKey = "CurrentMultipleNHibernateSession.Key";

        public NHMultipleUnitOfWorkFactory(params string[] cfgFilenames) {
            if(IsInfoEnabled)
                log.Info("NHMultipleUnitOfWorkFactory를 생성합니다. cfgFilenames=[{0}]", cfgFilenames.CollectionToString());

            foreach(var cfgFilename in cfgFilenames)
                Add(new NHUnitOfWorkFactory(cfgFilename));
        }

        /// <summary>
        /// Current session
        /// </summary>
        public ISession CurrentSession {
            get {
                if(CurrentName != null)
                    return GetCurrentSessionFor(CurrentName);

                return GetDefaultFactory().CurrentSession;
            }
            set {
                if(CurrentName != null) {
                    SetCurrentSessionFor(CurrentName, value);
                    return;
                }
                GetDefaultFactory().CurrentSession = value;
            }
        }

        /// <summary>
        /// NHibernate configuration filename
        /// </summary>
        public string ConfigurationFileName {
            // get { return this[0].ConfigurationFileName; }
            get {
                if(CurrentName.IsNotWhiteSpace())
                    return GetUnitOfWorkFactory(CurrentName).ConfigurationFileName;

                return GetDefaultFactory().ConfigurationFileName;
            }
        }

        /// <summary>
        /// NHibernate configuration
        /// </summary>
        public NHibernate.Cfg.Configuration Configuration {
            get {
                if(CurrentName.IsNotWhiteSpace()) {
                    return GetUnitOfWorkFactory(CurrentName).Configuration;
                }
                return GetDefaultFactory().Configuration;
            }
        }

        /// <summary>
        /// NHibernate SessionFactory
        /// </summary>
        public ISessionFactory SessionFactory {
            get { return CurrentSession.SessionFactory; }
        }

        private string CurrentName {
            get { return (string)Local.Data[this]; }
            set { Local.Data[this] = value; }
        }

        /// <summary>
        /// initialize unit of work factory.
        /// </summary>
        public void Init() {
            if(IsDebugEnabled)
                log.Debug("NHMultipleUnitOfWorkFactory를 초기화합니다...");

            ForEach(factory => factory.Init());
        }

        /// <summary>
        /// Create a new unit of work implementation.
        /// </summary>
        /// <param name="maybeUserProvidedConnection"></param>
        /// <param name="previous"></param>
        /// <returns></returns>
        public IUnitOfWorkImplementor Create(IDbConnection maybeUserProvidedConnection, IUnitOfWorkImplementor previous) {
            Guard.Assert<NotSupportedException>(maybeUserProvidedConnection == null,
                                                USER_PROVIDED_CONNECTION_EXCEPTION_MESSAGE);

            if(IsDebugEnabled)
                log.Debug("NHMultipleUnitOfWorkFactory에서 [{0}]개의 IUnitOfWorkImplementor를 새로 생성합니다...", Count);

            var previousImplementors = previous as NHMultipleUnitOfWorkImplementor;
            var currentImplementor = new NHMultipleUnitOfWorkImplementor();

            for(var i = 0; i < Count; i++) {
                IUnitOfWorkImplementor previousImplementor = null;

                if(previousImplementors != null)
                    previousImplementor = previousImplementors[i];

                currentImplementor.Add(this[i].Create(null, previousImplementor));
            }

            if(IsDebugEnabled)
                log.Debug("NHMultipleUnitOfWorkFactory에서 [{0}]개의 IUnitOfWorkImplementor를 새로 생성했습니다!!!", Count);

            return currentImplementor;
        }

        /// <summary>
        /// Disposing a given Unit of work
        /// </summary>
        /// <param name="adapter">instance of IUnitOfWork to dispose.</param>
        public virtual void DisposeUnitOfWork(IUnitOfWorkImplementor adapter) {
            adapter.ShouldNotBeNull("adapter");

            if(IsDebugEnabled)
                log.Debug("[{0}]를 Dispose합니다.", adapter.GetType().FullName);

            ISession session = null;

            if(adapter.Previous != null)
                session = adapter.Previous.Session;

            CurrentSession = session;
            UnitOfWork.DisposeUnitOfWork(adapter);
        }

        /// <summary>
        /// Long Conversation 시에 ASP.NET Session에 보관된 IUnitOfWork 인스턴스를 가져와 HttpRequest의 Context에 전달한다.
        /// </summary>
        /// <param name="hashtable">UnitOfWork 인스턴스들을 캐시한 Hashtable</param>
        /// <param name="unitOfWork">현 Thread Context에 해당하는 <see cref="IUnitOfWork"/>의 인스턴스</param>
        /// <param name="longConversationId">현 Thread Context에 해당하는 Long Conversation Id </param>
        public void LoadUnitOfWorkFromHashtable(Hashtable hashtable, out IUnitOfWork unitOfWork, out Guid? longConversationId) {
            if(IsDebugEnabled)
                log.Debug("Load unit of work instance from Hashtable");

            unitOfWork = (IUnitOfWork)hashtable[UnitOfWork.CurrentUnitOfWorkKey];
            longConversationId = (Guid?)hashtable[UnitOfWork.CurrentLongConversationIdKey];

            var sessions = (ISession[])hashtable[CurrentNHibernateSessionKey];

            // restore NHibernate session 
            for(var i = 0; i < Count - 1; i++)
                this[i].CurrentSession = sessions[i];
        }

        /// <summary>
        /// Long Conversation 시에 현재 활성화 된 IUnitOfWork 인스턴스를 Session에 저장한다.
        /// </summary>
        public void SaveUnitOfWorkToHashtable(Hashtable hashtable) {
            if(IsDebugEnabled)
                log.Debug("store unit of work to asp.net session.");

            hashtable[UnitOfWork.CurrentUnitOfWorkKey] = UnitOfWork.Current;
            hashtable[UnitOfWork.CurrentLongConversationIdKey] = UnitOfWork.CurrentLongConversationId;
            hashtable[CurrentNHibernateSessionKey] = ConvertAll<ISession>(FactoryToSession).ToArray();
        }

        /// <summary>
        /// 지정된 Entity 형식이 매핑된 현재 Session을 반환한다.
        /// </summary>
        /// <param name="typeOfEntity"></param>
        /// <returns></returns>
        public ISession GetCurrentSessionFor(Type typeOfEntity) {
            if(IsDebugEnabled)
                log.Debug("지정된 Entity형식이 매핑된 NHibernate SessionFactory를 찾아서 Current Session을 반환을 시작합니다.. Entity type=[{0}]",
                          typeOfEntity);

            typeOfEntity.ShouldNotBeNull("typeOfEntity");

            var uowFactory = FindUnitOfWorkFactory(typeOfEntity.FullName);

            if(IsDebugEnabled)
                log.Debug("엔티티[{0}]가 매핑된 SessionFactory의 NHUnitOfWorkFactory[{1}]를 찾았습니다.", typeOfEntity.FullName, uowFactory);

            if(uowFactory != null) {
                return uowFactory.GetCurrentSessionFor(typeOfEntity);
            }

            throw new InvalidOperationException("지정된 Entity가 매핑된 IUnitOfWorkFactory를 찾을 수 없습니다.");
        }

        /// <summary>
        /// 지정한 이름의 Session Factory의 현재 Session을 구합니다.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ISession GetCurrentSessionFor(string name) {
            name.ShouldNotBeWhiteSpace("name");

            var uowFactory = Find(factory => name.EqualTo(factory.GetSessionFactoryName(), true));
            Guard.Assert(uowFactory != null, "No stored factory exists. factoryName=[{0}]", name);

            return uowFactory.CurrentSession;
        }

        /// <summary>
        /// 지정된 Entity 형식을 해당 Session에 매핑시킨다.
        /// </summary>
        /// <param name="typeOfEntity"></param>
        /// <param name="session"></param>
        public void SetCurrentSession(Type typeOfEntity, ISession session) {
            typeOfEntity.ShouldNotBeNull("typeOfEntity");

            var uowFactory = FindUnitOfWorkFactory(typeOfEntity.FullName);

            if(uowFactory != null)
                uowFactory.CurrentSession = session;
        }

        private IIniQueryProvider _queryProvider;

        /// <summary>
        /// NHibernate HQL 문장을 제공하는 Provider
        /// </summary>
        public IIniQueryProvider QueryProvider {
            get { return _queryProvider ?? (_queryProvider ?? IoC.TryResolve<IIniQueryProvider>(null)); }
            set { _queryProvider = value; }
        }

        /// <summary>
        /// 다중 Database에 대해 기본 Session Factory Name을 나타낸다.
        /// </summary>
        public string DefaultFactoryName { get; set; }

        /// <summary>
        /// 지정한 SessionFactoryName을 가진 UnitOfWorkFactory를 찾습니다.
        /// </summary>
        /// <param name="sessionFactoryName"></param>
        /// <returns></returns>
        public IUnitOfWorkFactory GetUnitOfWorkFactory(string sessionFactoryName) {
            sessionFactoryName.ShouldNotBeWhiteSpace("sessionFactoryName");

            return Find(factory => sessionFactoryName.EqualTo(factory.GetSessionFactoryName(), true));
            //return Find(factory => factory.GetSessionFactoryName().Equals(sessionFactoryName, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// 지정된 형식이 매핑된 Session을 반환한다.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public ISession GetCurrentSessionFor<TEntity>() {
            return GetCurrentSessionFor(typeof(TEntity));
        }

        private IUnitOfWorkFactory _defaultUowFactory;

        /// <summary>
        /// Multiple UnitOfWorkFactory에서 기본 Factory로 지정된 것의 Session을 가져온다.
        /// </summary>
        /// <returns></returns>
        public IUnitOfWorkFactory GetDefaultFactory() {
            if(_defaultUowFactory == null)
                if(DefaultFactoryName.IsNotWhiteSpace())
                    _defaultUowFactory = GetUnitOfWorkFactory(DefaultFactoryName);
                        //Find(factory => factory.GetSessionFactoryName().EqualTo(DefaultFactoryName, true));

            if(_defaultUowFactory == null)
                if(Count > 0)
                    _defaultUowFactory = this[0];

            return _defaultUowFactory;
        }

        /// <summary>
        /// 지정된 Entity 형식을 해당 Session에 매핑시킨다.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="session"></param>
        public void SetCurrentSessionFor<TEntity>(ISession session) {
            SetCurrentSession(typeof(TEntity), session);
        }

        /// <summary>
        /// 현재 사용할 Session을 지정한 이름의 SessionFactory와 session으로 할당한다.
        /// </summary>
        /// <param name="name">session factory name</param>
        /// <param name="session">session</param>
        public void SetCurrentSessionFor(string name, ISession session) {
            name.ShouldNotBeWhiteSpace("name");
            session.ShouldNotBeNull("session");

            var uowFactory = Find(factory => {
                                      var settings =
                                          ((ISessionFactoryImplementor)factory.SessionFactory).Settings;
                                      return settings.SessionFactoryName == name;
                                  });

            Guard.Assert(uowFactory != null, "No stored factory exists. factoryName=[{0}]", name);

            uowFactory.CurrentSession = session;
        }

        /// <summary>
        /// 해당이름을 현재 Session 명으로 하는 DisposableAction을 반환합니다.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IDisposable SetCurrentSessionName(string name) {
            var savedCurrentName = CurrentName;
            CurrentName = name;
            return new DisposableAction(() => CurrentName = savedCurrentName);
        }

        private static ISession FactoryToSession(IUnitOfWorkFactory factory) {
            return factory.CurrentSession;
        }

        private void AssertExactlyOneSessionFactory() {
            Guard.Assert(Count == 1, "{0} units of work exists, pick one using GetCurrentSessionFor<TEntity>", Count);
        }

        private IUnitOfWorkFactory FindUnitOfWorkFactory(string className) {
            className.ShouldNotBeWhiteSpace("className");

            if(IsDebugEnabled)
                log.Debug("지정된 Entity 형식을 Mapping한 SessionFactory를 가지는 NHUnitOfWorkFactory를 검색합니다. Entity Type=[{0}]", className);

            try {
                foreach(IUnitOfWorkFactory factory in this) {
                    if(factory.SessionFactory.GetAllClassMetadata().Keys.Contains(className))
                        return factory;
                }
            }
            catch(Exception ex) {
                if(log.IsWarnEnabled) {
                    log.Warn("Entity[{0}] Mapping한 SessionFactory를 가지는 NHUnitOfWorkFactory를 검색하는데 실패했습니다.", className);
                    log.Warn(ex);
                }
            }
            return null;
        }

        /// <summary>
        /// 인스턴스의 내부 내용을 문자열로 표현한다.
        /// </summary>
        /// <param name="showDetail"></param>
        /// <returns></returns>
        public string ToString(bool? showDetail) {
            if(showDetail.GetValueOrDefault(false) == false)
                return base.ToString();

            var sb = new StringBuilder();
            sb.AppendFormat("{0}# Count=[{1}]", GetType().FullName, Count).AppendLine();

            foreach(IUnitOfWorkFactory uowFactory in this) {
                sb.AppendFormat(uowFactory.ToString()).AppendLine();
                if(uowFactory.CurrentSession != null) {
                    sb.AppendLine().AppendFormat("\tCurrent Session=[{0}]", uowFactory.CurrentSession);
                    if(uowFactory.CurrentSession.Connection != null) {
                        sb.AppendFormat("\t\tConnectionString=[{0}]", uowFactory.CurrentSession.Connection.ConnectionString).AppendLine();
                    }

                    if(uowFactory.CurrentSession.SessionFactory != null) {
                        sb.AppendFormat("\tSessionFactory ClassMetadata").AppendLine();

                        // NHibernate 2.0.1 GA에서 GetAllClassMetadata 는 IDictionary<Type, IClassMetadata> 이고 Type은 Mapping된 entity의 Type이다.
                        // for NH 2.0.1
                        // foreach (Type entityType in uowFactory.CurrentSession.SessionFactory.GetAllClassMetadata().Keys)

                        // NHibernate 2.1.0 Alpha 에서는 IDictionary<string, ICalssMetadata>이고, Key는 Mapping된 Entity의 Type.FullName이다.
                        // for NH 2.1.0
                        foreach(string entityType in uowFactory.CurrentSession.SessionFactory.GetAllClassMetadata().Keys) {
                            sb.AppendFormat("\t\tEntity=[{0}]", entityType).AppendLine();
                        }
                    }
                }
            }
            return sb.ToString();
        }
    }
}