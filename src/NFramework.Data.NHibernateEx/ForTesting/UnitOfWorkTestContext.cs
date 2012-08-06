using System;
using Castle.Windsor;
using FluentNHibernate.Conventions;
using NHibernate;
using NSoft.NFramework.InversionOfControl;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Data.NHibernateEx.ForTesting {
    /// <summary>
    /// 여러 Database에 대한 UnitOfWork Testing 을 위한 UnitOfWork 의 context이다.
    /// Runtime시에 이 클래스를 대체하여 여러가지 Database 에 대한 Testing를 수행할 수 있다.
    /// </summary>
    public abstract class UnitOfWorkTestContext : IDisposable {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 현재 활성화된 NHibernate.ISession을 <see cref="Local"/>저장소에 저장하기 위한 키
        /// </summary>
        public readonly string CurrentNHibernateSessionKey = Guid.NewGuid().ToString(); //= "CurrentNHibernateSession.Key";

        #region << Static Method >>

        /// <summary>
        /// 인자에 맞는 <see cref="UnitOfWorkTestContext"/>의 인스턴스를 빌드한다.
        /// </summary>
        /// <param name="containerConfigPath">IoC 환경설정 파일경로</param>
        /// <param name="dbStrategy">테스트용 DB 생성 전략</param>
        /// <param name="mappingInfo">NHibernate용 Entity 정보</param>
        /// <returns>테스트용 UnitOfWork</returns>
        /// <param name="configAction">추가 환경설정 작업용 델리게이트</param>
        public static UnitOfWorkTestContext For(string containerConfigPath,
                                                UnitOfWorkTestContextDbStrategy dbStrategy,
                                                MappingInfo mappingInfo,
                                                Action<NHibernate.Cfg.Configuration> configAction) {
            return new NHUnitOfWorkTestContext(dbStrategy, containerConfigPath, mappingInfo, configAction);
        }

        /// <summary>
        /// 인자에 맞는 <see cref="UnitOfWorkTestContext"/>의 인스턴스를 빌드한다.
        /// </summary>
        /// <param name="container">Container</param>
        /// <param name="dbStrategy">테스트용 DB 생성 전략</param>
        /// <param name="mappingInfo">NHibernate용 Entity 정보</param>
        /// <param name="configAction">추가 환경설정 작업용 델리게이트</param>
        /// <returns>테스트용 UnitOfWork</returns>
        public static UnitOfWorkTestContext For(IWindsorContainer container,
                                                UnitOfWorkTestContextDbStrategy dbStrategy,
                                                MappingInfo mappingInfo,
                                                Action<NHibernate.Cfg.Configuration> configAction) {
            var context = For(string.Empty, dbStrategy, mappingInfo, configAction);
            context._container = container;
            return context;
        }

        /// <summary>
        /// 인자에 맞는 <see cref="UnitOfWorkTestContext"/>의 인스턴스를 빌드한다.
        /// </summary>
        /// <param name="containerConfigPath">IoC 환경설정 파일경로</param>
        /// <param name="dbStrategy">테스트용 DB 생성 전략</param>
        /// <param name="mappingInfo">NHibernate용 Entity 정보</param>
        /// <returns>테스트용 UnitOfWork</returns>
        /// <param name="configAction">추가 환경설정 작업용 델리게이트</param>
        /// <param name="conventions">명명규칙</param>
        public static UnitOfWorkTestContext FluentFor(string containerConfigPath,
                                                      UnitOfWorkTestContextDbStrategy dbStrategy,
                                                      MappingInfo mappingInfo,
                                                      Action<NHibernate.Cfg.Configuration> configAction,
                                                      params IConvention[] conventions) {
            return new FluentNHUnitOfWorkTestContext(dbStrategy, containerConfigPath, mappingInfo, configAction, conventions);
        }

        /// <summary>
        /// 인자에 맞는 <see cref="UnitOfWorkTestContext"/>의 인스턴스를 빌드한다.
        /// </summary>
        /// <param name="container">Container</param>
        /// <param name="dbStrategy">테스트용 DB 생성 전략</param>
        /// <param name="mappingInfo">NHibernate용 Entity 정보</param>
        /// <param name="configAction">추가 환경설정 작업용 델리게이트</param>
        /// <param name="conventions">명명규칙</param>
        /// <returns>테스트용 UnitOfWork</returns>
        public static UnitOfWorkTestContext FluentFor(IWindsorContainer container,
                                                      UnitOfWorkTestContextDbStrategy dbStrategy,
                                                      MappingInfo mappingInfo,
                                                      Action<NHibernate.Cfg.Configuration> configAction,
                                                      params IConvention[] conventions) {
            var context = FluentFor(string.Empty, dbStrategy, mappingInfo, configAction, conventions);
            context._container = container;
            return context;
        }

        #endregion

        /// <summary>
        /// IoC Container
        /// </summary>
        [CLSCompliant(false)] protected IWindsorContainer _container;

        /// <summary>
        /// Castle Windsor 환경설정 파일 경로
        /// </summary>
        [CLSCompliant(false)] protected readonly string _containerConfigPath;

        private UnitOfWorkTestContextDbStrategy _dbStrategy;
        private MappingInfo _mappingInfo;
        private int _unitOfWorkNestingLevel = -1;

        /// <summary>
        /// 인자에 맞는 <see cref="UnitOfWorkTestContext"/>의 인스턴스를 빌드한다.
        /// </summary>
        /// <param name="dbStrategy">테스트용 DB 생성 전략</param>
        /// <param name="containerConfigPath">IoC 환경설정 파일경로</param>
        /// <param name="mappingInfo">NHibernate 매핑 정보</param>
        /// <param name="configAction">Configuration 빌드 시 추가할 사항을 정의한 Action</param>
        protected UnitOfWorkTestContext(UnitOfWorkTestContextDbStrategy dbStrategy,
                                        string containerConfigPath,
                                        MappingInfo mappingInfo,
                                        Action<NHibernate.Cfg.Configuration> configAction) {
            _containerConfigPath = containerConfigPath ?? string.Empty;
            _dbStrategy = dbStrategy;
            _mappingInfo = mappingInfo;

            _dbStrategy.TestContext = this;

            ConfigAction = configAction;
        }

        /// <summary>
        /// NHibernate 매핑 정보
        /// </summary>
        internal MappingInfo MappingInfo {
            get { return _mappingInfo; }
        }

        /// <summary>
        /// NHibernate 환경설정 정보
        /// </summary>
        public abstract NHibernate.Cfg.Configuration NHConfiguration { get; }

        public Action<NHibernate.Cfg.Configuration> ConfigAction { get; protected set; }

        /// <summary>
        /// Database name for testing
        /// </summary>
        public string DatabaseName {
            get { return _dbStrategy.DatabaseName; }
        }

        /// <summary>
        /// Database Engine
        /// </summary>
        public DatabaseEngine DatabaseEngine {
            get { return _dbStrategy.DatabaseEngine; }
        }

        /// <summary>
        /// UnitOfWorkTestContextDbStrategy
        /// </summary>
        internal UnitOfWorkTestContextDbStrategy DbStrategy {
            get { return _dbStrategy; }
        }

        /// <summary>
        /// IoC Container
        /// </summary>
        public IWindsorContainer Container {
            get {
                if(_containerConfigPath.IsWhiteSpace() && _container == null)
                    return null;

                return _container ?? (_container = new WindsorContainer(_containerConfigPath));
            }
        }

        /// <summary>
        /// Configuration file path of IoC Configuration
        /// </summary>
        public string ContainerConfigPath {
            get { return _containerConfigPath; }
        }

        /// <summary>
        /// Nesting level of Unit Of Work
        /// </summary>
        public int UnitOfWorkNestingLevel {
            get { return _unitOfWorkNestingLevel; }
        }

        /// <summary>
        /// NHibernate ISessionFactory for Testing
        /// </summary>
        public abstract ISessionFactory SessionFactory { get; }

        /// <summary>
        /// Starts a <see cref="UnitOfWork" /> and creates the db schema.
        /// </summary>
        /// <seealso cref="IoC" />
        /// <seealso cref="UnitOfWork" />
        public virtual void CreateUnitOfWork() {
            Guard.Assert(_unitOfWorkNestingLevel != 0, "중첩된 UnitOfWork를 만들려면 CreateNextedUnitOfWork() 메소드를 호출하세요.");

            if(log.IsInfoEnabled)
                log.Info("현 Session을 사용하여 UnitOfWork를 시작하고, Database를 설정합니다");

            UnitOfWork.Stop();
            UnitOfWork.DisposeUnitOfWorkFactory();

            UnitOfWork.Start();

            _dbStrategy.CreateDatabaseMedia();
            SetupDatabase(UnitOfWork.CurrentSession);

            _unitOfWorkNestingLevel = 0;
        }

        /// <summary>
        ///  Starts a nested <see cref="UnitOfWork" />
        /// </summary>
        /// <returns></returns>
        public IUnitOfWork CreateNestedUnitOfWork() {
            Guard.Assert(_unitOfWorkNestingLevel != -1, "부모 UnitOfWork가 존재하지 않습니다. 먼저 CreateUnitOfWork()를 호출하셔야 합니다.");

            //if(_unitOfWorkNestingLevel == -1)
            //    throw new InvalidOperationException("previous unit of work does not exist. first of all you need to call CreateUnitOfWork()");

            var uow = UnitOfWork.Start(UnitOfWork.CurrentSession.Connection,
                                       UnitOfWorkNestingOptions.CreateNewOrNestUnitOfWork);
            _unitOfWorkNestingLevel++;
            return uow;
        }

        /// <summary>
        /// Opens an NHibernate session and creates the db schema.
        /// </summary>
        /// <returns>The open NHibernate session.</returns>
        public virtual ISession CreateSession() {
            var session = DbStrategy.CreateSession();

            //if(session != null)
            //    SetupDatabase(session);

            return session;
        }

        /// <summary>
        /// Close the specified session
        /// </summary>
        /// <param name="sessionToClose">session to close</param>
        public void DisposeSession(ISession sessionToClose) {
            if(IsDebugEnabled)
                log.Debug("NHibernate 세션 및 Connection을 닫습니다...");

            //explicit disposing of connection is required (??) for in memory databases
            //
            if(sessionToClose != null) {
                var conn = sessionToClose.Connection;
                sessionToClose.Dispose();
                if(conn != null)
                    conn.Dispose();

                if(IsDebugEnabled)
                    log.Debug("NHibernate 세션 및 Connection을 닫았습니다.");
            }
        }

        /// <summary>
        /// Dispose current UnitOfWork
        /// </summary>
        public virtual void DisposeUnitOfWork() {
            UnitOfWork.Stop();

            _unitOfWorkNestingLevel--;
        }

        /// <summary>
        /// IoC Container와 UnitOfWorkFactory를 초기화한다.
        /// </summary>
        public abstract void InitializeContainerAndUnitOfWorkFactory();

        /// <summary>
        /// Creates the in db schema using the session.
        /// </summary>
        /// <param name="session">An open NHibernate session.</param>
        public void SetupDatabase(ISession session) {
            _dbStrategy.SetUpDatabase(session);
        }

        /// <summary>
        /// 인스턴스의 정보를 문자열로 반환한다.
        /// </summary>
        public override string ToString() {
            return string.Format("DatabaseEngine=[{0}] DatabaseName=[{1}] Castle.Windsor Configuration FilePath=[{2}]",
                                 DatabaseEngine, DatabaseName, ContainerConfigPath ?? string.Empty);
        }

        #region IDisposable Members

        /// <summary>
        /// Release unmanaged resources. 내부 Container를 메모리에서 해제한다.
        /// </summary>
        public void Dispose() {
            _dbStrategy = null;
            _mappingInfo = null;

            UnitOfWork.Stop();
            //UnitOfWork.CurrentSession = null;

            if(_container != null) {
                _container.Dispose();
                _container = null;
            }
        }

        #endregion
    }
}