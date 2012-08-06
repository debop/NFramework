using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using NHibernate;
using NHibernate.Cfg;
using NSoft.NFramework.IO;
using NSoft.NFramework.InversionOfControl;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.Reflections;

namespace NSoft.NFramework.Data.NHibernateEx {
    /// <summary>
    /// Factory for <see cref="IUnitOfWork"/> (기본적으로 <see cref="NHUnitOfWorkAdapter"/>를 생성한다.
    /// </summary>
    public class NHUnitOfWorkFactory : IUnitOfWorkFactory {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;
        private static readonly bool IsInfoEnabled = log.IsInfoEnabled;

        #endregion

        /// <summary>
        /// NHibernate default configuration file name
        /// </summary>
        public const string DEFAULT_CONFIG_FILENAME = @"hibernate.cfg.xml";

        /// <summary>
        /// 현재 활성화된 NHibernate.ISession을 <see cref="Local"/>저장소에 저장하기 위한 키
        /// </summary>
        public readonly string CurrentNHibernateSessionKey = Guid.NewGuid().ToString(); //= "CurrentNHibernateSession.Key";

        [CLSCompliant(false)] protected readonly object _syncLock = new object();

        [CLSCompliant(false)] protected ISessionFactory _sessionFactory;

        [CLSCompliant(false)] protected Configuration _cfg;

        [CLSCompliant(false)] protected string _cfgFilename = DEFAULT_CONFIG_FILENAME;

        [CLSCompliant(false)] protected readonly Assembly[] _assemblies;

        /// <summary>
        /// NHibernate SessionFactory 초기화
        /// </summary>
        public INHInitializationAware[] InitializationAwares { get; set; }

        /// <summary>
        /// Initialize a new instance of NHUnitOfWorkFactory with <see cref="DEFAULT_CONFIG_FILENAME"/> (hibernate.cfg.xml)
        /// </summary>
        public NHUnitOfWorkFactory() : this(DEFAULT_CONFIG_FILENAME) {}

        /// <summary>
        /// Initialize a new instance of NHUnitOfWorkFactory with the specified configuration file path
        /// </summary>
        /// <param name="cfgFilename">NHibernate configuration file path</param>
        public NHUnitOfWorkFactory(string cfgFilename) {
            if(log.IsInfoEnabled)
                log.Info("NHUnitOfWorkFactory 를 빌드합니다. 환경설정 파일=[{0}]", cfgFilename);

            _cfgFilename = cfgFilename ?? DEFAULT_CONFIG_FILENAME;
            _sessionFactory = null;
            CurrentSession = null;
        }

        /// <summary>
        ///  Initialize a new instance of NHUnitOfWorkFactory with the specified mapping assemblies
        /// </summary>
        /// <param name="assemblies">Configuration 파일에서 Mapping 한 Assembly외에 추가로 Castle IoC를 통해서 추가할 수 있다.</param>
        public NHUnitOfWorkFactory(Assembly[] assemblies) : this(assemblies, DEFAULT_CONFIG_FILENAME) {}

        /// <summary>
        ///	 Initialize a new instance of NHUnitOfWorkFactory with the specified configuration file path and mapping assemblies
        /// </summary>
        /// <param name="assemblies">Configuration 파일에서 Mapping 한 Assembly외에 추가로 Castle IoC를 통해서 추가할 수 있다.</param>
        /// <param name="cfgFilename">NHibernate configuration file path</param>
        public NHUnitOfWorkFactory(Assembly[] assemblies, string cfgFilename)
            : this(cfgFilename) {
            if(assemblies != null)
                if(IsDebugEnabled)
                    log.Debug("NHUnitOfWorkFactory 를 빌드합니다. 부가 Assemblies=[{0}]",
                              assemblies.Select(a => a.FullName).CollectionToString());

            _assemblies = assemblies;
            //_sessionFactory = null;
            //CurrentSession = null;
        }

        public NHUnitOfWorkFactory(string[] assemblyNames) : this(assemblyNames, DEFAULT_CONFIG_FILENAME) {}

        public NHUnitOfWorkFactory(string[] assemblyNames, string cfgFilename)
            : this(cfgFilename) {
            if(assemblyNames != null)
                if(IsDebugEnabled)
                    log.Debug("NHUnitOfWorkFactory 를 빌드합니다. 부가 AssemblyNames=[{0}], cfgFilename=[{1}]",
                              assemblyNames.CollectionToString(), cfgFilename);

            var assemblies = new HashSet<Assembly>();

            foreach(var asmName in assemblyNames) {
                var asm = With.TryFunction(() => Assembly.Load(asmName));

                if(asm != null)
                    assemblies.Add(asm);
            }
            _assemblies = assemblies.ToArray();
        }

        /// <summary>
        /// configure with specified nhibernate configuration file.
        /// </summary>
        /// <param name="configFilePath">physical path of nhibernate configuration file</param>
        /// <returns></returns>
        protected virtual Configuration ConfigureCfgFile(string configFilePath) {
            configFilePath = FileTool.GetPhysicalPath(configFilePath);

            //if (Path.IsPathRooted(configFilePath))
            //    configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFilePath);

            if(File.Exists(configFilePath) == false)
                throw new FileNotFoundException("NHibernate 환경설정 파일을 찾을 수 없습니다. configFilePath=" + configFilePath);

            if(log.IsInfoEnabled)
                log.Info("NHibernate Configuration 빌드를 시작합니다... configFilePath=[{0}]", configFilePath);

            var cfg = new Configuration();

            try {
                cfg.Configure(configFilePath);

                // Configuration 파일에서 정의한 Mapping Assembly 외에 
                // IoC를 통해 NHUnitOfWorkFactory에 할당된 추가 Assembly들도 매핑이 가능하게 열어준다.
                //
                if(_assemblies != null && _assemblies.Length > 0) {
                    var loadedAssemblies = cfg.GetMappingAssemblies();

                    _assemblies
                        .Where(asm => loadedAssemblies.Contains(asm) == false)
                        .RunEach(asm => cfg.AddAssembly(asm));
                }
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled) {
                    log.Error("NHibernate 환경설정에 실패했습니다. configFilePath=[{0}]", configFilePath);
                    log.Error(ex);
                }

                throw;
            }

            if(log.IsInfoEnabled)
                log.Info("NHibernate Configuration 빌드를 완료했습니다!!! configFilePath=[{0}]", configFilePath);

            return cfg;
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

            // BUG:  _sessionFactory를 없애면 안됩니다.
            // _sessionFactory = null;
        }

        /// <summary>
        /// replace the default implementation of the Session Factory (read from configuration) with a user supplied one.
        /// NOTE : 이 작업은 Application 시작시에만 가능하다.
        /// </summary>
        /// <param name="factory">instance of ISessionFactory to regist</param>
        public void RegisterSessionFactory(ISessionFactory factory) {
            RegisterSessionFactory(null, factory);
        }

        /// <summary>
        /// replace the default implementation of the Session Factory (read from configuration) with a user supplied one.
        /// NOTE : 이 작업은 Application 시작시에만 가능하다.
        /// </summary>
        /// <param name="cfg">instance of Configuration</param>
        /// <param name="factory">instance of ISessionFactory to regist</param>
        public virtual void RegisterSessionFactory(Configuration cfg, ISessionFactory factory) {
            factory.ShouldNotBeNull("factory");

            if(log.IsInfoEnabled)
                log.Info("새로운 Session Factory 등록을 시도합니다... factory=[{0}]", factory.GetSessionFactoryName());

            var old = _sessionFactory;
            _cfg = cfg;
            _sessionFactory = factory;

            if(log.IsInfoEnabled)
                log.Info("새로운 SesscionFactory 등록 작업이 성공했습니다!!! factory=[{0}]", factory.GetSessionFactoryName());

            if(old != null) {
                old.Close();

                if(IsDebugEnabled)
                    log.Debug("기존 Session Factory는 Close 시켰습니다.");
            }
        }

        /// <summary>
        /// NHibernate configuration filename
        /// </summary>
        public string ConfigurationFileName {
            get { return _cfgFilename ?? (_cfgFilename = DEFAULT_CONFIG_FILENAME); }
        }

        /// <summary>
        /// NHibernate Configuration
        /// </summary>
        public virtual NHibernate.Cfg.Configuration Configuration {
            get {
                if(_cfg == null)
                    lock(_syncLock)
                        if(_cfg == null) {
                            if(IsInfoEnabled)
                                log.Info("파일[{0}]을 이용하여 NHibernate Conifuration을 빌드합니다...", ConfigurationFileName);

                            var cfg = ConfigureCfgFile(ConfigurationFileName);
                            System.Threading.Thread.MemoryBarrier();
                            _cfg = cfg;

                            if(IsInfoEnabled)
                                log.Info("파일[{0}]을 이용하여 NHibernate Conifuration을 빌드했습니다!!!", ConfigurationFileName);
                        }
                return _cfg;
            }
        }

        /// <summary>
        /// Session factory for the application.
        /// </summary>
        public ISessionFactory SessionFactory {
            get {
                if(_sessionFactory == null)
                    lock(_syncLock)
                        if(_sessionFactory == null) {
                            if(IsInfoEnabled)
                                log.Info("새로운 SessionFactory를 빌드합니다...");

                            LoadInitializationAware();
                            BeforeInitializeByInitializationAware();

                            // _cfg = ConfigureCfgFile(ConfigurationFileName);

                            var cfg = Configuration;

                            ConfigureByInitializationAware();

                            var factory = cfg.BuildSessionFactory();
                            System.Threading.Thread.MemoryBarrier();
                            _sessionFactory = factory;

                            if(IsInfoEnabled)
                                log.Info("새로운 SessionFactory를 빌드했습니다!!! SessionFactory=[{0}]", factory.GetSessionFactoryName());

                            InitializedByInitializationAware();
                        }

                return _sessionFactory;
            }
        }

        /// <summary>
        /// Current session
        /// </summary>
        public ISession CurrentSession {
            get {
                var session = Local.Data[CurrentNHibernateSessionKey] as ISession;
                Guard.Assert(session != null, "Session이 Current Thread Context에서 생성되지 않았습니다. UnitOfWork.Start()를 먼저 호출하셔야 합니다.");
                return session;
            }
            set { Local.Data[CurrentNHibernateSessionKey] = value; }
        }

        /// <summary>
        /// Query String을 제공하는 Provider
        /// </summary>
        public IIniQueryProvider QueryProvider { get; set; }

        /// <summary>
        /// 매핑 정보가 있는 Assembly 들
        /// </summary>
        public List<Assembly> MappingAssemblies {
            get { return (_assemblies != null) ? new List<Assembly>(_assemblies) : new List<Assembly>(); }
        }

        /// <summary>
        /// Initialize Factory of UnitOfWork
        /// </summary>
        public virtual void Init() {
            if(IsInfoEnabled)
                log.Info("NHibernate NHUnitOfWorkFactory를 초기화 합니다...");
        }

        /// <summary>
        /// Create a new unit of work implementation.
        /// </summary>
        /// <param name="maybeUserProvidedConnection">instance of IDbConnection.</param>
        /// <param name="previous">현재 사용중인 IUnitOfWorkImplementor의 인스턴스</param>
        /// <returns>새로 생성한 IUnitOfWorkImplementor의 인스턴스</returns>
        public IUnitOfWorkImplementor Create(IDbConnection maybeUserProvidedConnection, IUnitOfWorkImplementor previous) {
            if(IsDebugEnabled) {
                log.Debug("새로운 IUnitOfWorkImplentor 인스턴스 생성을 시작합니다...");
                log.Debug("새로운 ISession을 생성하여, CurrentSession으로 등록합니다...");
            }

            var session = CreateSession(maybeUserProvidedConnection);
            Local.Data[CurrentNHibernateSessionKey] = session;

            if(IsDebugEnabled)
                log.Debug("새로운 NHibernate Session을 Current Thread Context에 저장했습니다. Key=[{0}]", CurrentNHibernateSessionKey);

            return new NHUnitOfWorkAdapter(this, session, (NHUnitOfWorkAdapter)previous);
        }

        /// <summary>
        /// Long Conversation시에 Hashtable 에 저장된 UnitOfWork 인스턴스 정보를 가져온다.
        /// </summary>
        public void LoadUnitOfWorkFromHashtable(Hashtable hashtable, out IUnitOfWork unitOfWork, out Guid? longConversationId) {
            if(IsDebugEnabled)
                log.Debug("지정된 Hashtable에서 IUnitOfWork 인스턴스를 로드합니다....");

            unitOfWork = hashtable[UnitOfWork.CurrentUnitOfWorkKey] as IUnitOfWork;
            longConversationId = hashtable[UnitOfWork.CurrentLongConversationIdKey] as Guid?;
            UnitOfWork.CurrentSession = (ISession)hashtable[CurrentNHibernateSessionKey];
        }

        /// <summary>
        /// Long Conversation을 위해 UnitOfWork 인스턴스 정보를 ASP.NET Session 에 저장한다.
        /// </summary>
        public void SaveUnitOfWorkToHashtable(Hashtable hashtable) {
            if(IsDebugEnabled)
                log.Debug("지정된 Hashtable에 현재 UnitOfWork 인스턴스를 저장합니다.");

            hashtable[UnitOfWork.CurrentUnitOfWorkKey] = UnitOfWork.Current;
            hashtable[CurrentNHibernateSessionKey] = UnitOfWork.CurrentSession;
            hashtable[UnitOfWork.CurrentLongConversationIdKey] = UnitOfWork.CurrentLongConversationId;
        }

        /// <summary>
        /// 지정된 Entity 형식이 매핑된 현재 Session을 반환한다.
        /// </summary>
        /// <param name="typeOfEntity"></param>
        /// <returns></returns>
        public ISession GetCurrentSessionFor(Type typeOfEntity) {
            if(IsDebugEnabled)
                log.Debug("엔티티가 매핑된 현재 Session을 반환합니다... Entity=[{0}]", typeOfEntity.FullName);

            return CurrentSession;
        }

        /// <summary>
        /// <typeparamref name="TEntity"/> 수형이 매핑된 Current Session을 반환한다.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public ISession GetCurrentSessionFor<TEntity>() {
            return GetCurrentSessionFor(typeof(TEntity));
        }

        /// <summary>
        /// 현재 사용중인 Session을 반환합니다.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ISession GetCurrentSessionFor(string name) {
            return CurrentSession;
        }

        /// <summary>
        /// 지정된 Entity 형식을 해당 Session에 매핑시킨다.
        /// </summary>
        /// <param name="typeOfEntity"></param>
        /// <param name="session"></param>
        public void SetCurrentSession(Type typeOfEntity, ISession session) {
            CurrentSession = session;
        }

        /// <summary>
        /// 지정한 session factory name의 session을 현재 session으로 사용하는 <see cref="DisposableAction"/>을 반환합니다.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IDisposable SetCurrentSessionName(string name) {
            return new DisposableAction(delegate { });
        }

        /// <summary>
        /// 새로운 세션을 만든다.
        /// </summary>
        /// <param name="maybeUserProvidedConnection"></param>
        /// <returns></returns>
        private ISession CreateSession(IDbConnection maybeUserProvidedConnection) {
            if(IsDebugEnabled)
                log.Debug("지정된 IDbConnection으로 새로운 Session을 생성합니다. maybeUserProvidedConnection=[{0}]", maybeUserProvidedConnection);

            //
            // NOTE: Interceptor를 cfg.SetInterceptor로 옮겼었는데, 이렇게 하면 Interceptor내에서 Nesting 된 unit-of-work인 경우 중첩되는 Interceptor가 들어가서 무한루프에 빠지게 된다.
            // NOTE: 이를 방지하기 위해 Session을 만들때마다 Interceptor를 넣게 했다.
            // 
            // Interceptor 없는 순수 ISession을 만들고 싶으면, UnitOfWork.CurrentSessionFactory.OpenSession() 을 호출하면 된다.
            //

            //var interceptor = NHIoC.ResolveInterceptor();

            //if(interceptor != null)
            //{
            //    if(maybeUserProvidedConnection != null)
            //        return SessionFactory.OpenSession(maybeUserProvidedConnection, interceptor);

            //    return SessionFactory.OpenSession(interceptor);
            //}

            var interceptor = NHIoC.ResolveAllInterceptors();

            if(interceptor.Count > 0) {
                if(maybeUserProvidedConnection != null)
                    return SessionFactory.OpenSession(maybeUserProvidedConnection, interceptor);

                return SessionFactory.OpenSession(interceptor);
            }

            if(maybeUserProvidedConnection == null)
                return SessionFactory.OpenSession();

            return SessionFactory.OpenSession(maybeUserProvidedConnection);
        }

        private void LoadInitializationAware() {
            if(IoC.IsNotInitialized)
                return;

            if(InitializationAwares == null) {
                if(IoC.HasComponent(typeof(INHInitializationAware))) {
                    if(IsDebugEnabled)
                        log.Debug("환경설정에서 INHInitializationAware 서비스용 컴포넌트들을 Resolve 합니다.");

                    InitializationAwares = IoC.ResolveAll<INHInitializationAware>();
                }
            }
        }

        private void BeforeInitializeByInitializationAware() {
            if(IoC.IsNotInitialized)
                return;

            if(InitializationAwares != null)
                foreach(var initializer in InitializationAwares)
                    initializer.BeforeInitialzation();
        }

        private void InitializedByInitializationAware() {
            if(IoC.IsNotInitialized)
                return;

            if(InitializationAwares != null && _cfg != null) {
                if(IsDebugEnabled)
                    log.Debug("Session Factory에 대한 부가적인 Initializer에 대해 Initializing을 수행합니다.");

                foreach(INHInitializationAware initializer in InitializationAwares)
                    initializer.Initialized(_cfg, SessionFactory);
            }
        }

        private void ConfigureByInitializationAware() {
            if(IoC.IsNotInitialized)
                return;

            if(InitializationAwares != null)
                foreach(var initializer in InitializationAwares)
                    initializer.Configured(_cfg);
        }
    }
}