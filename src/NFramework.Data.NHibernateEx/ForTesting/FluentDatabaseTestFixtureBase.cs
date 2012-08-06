using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Castle.Windsor;
using FluentNHibernate.Conventions;
using FluentNHibernate.Testing;
using NSoft.NFramework.Data.NHibernateEx.Fluents;
using NSoft.NFramework.InversionOfControl;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Data.NHibernateEx.ForTesting {
    /// <summary>
    /// FluentNHibernate를 사용하여, NHibernate Database 단위 테스트를 수행하는 기본 클래스입니다. 
    /// </summary>
    public abstract class FluentDatabaseTestFixtureBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// <see cref="PersistenceSpecification{T}"/> 수행 시 값 비교에 쓰기 위해 사용합니다.
        /// </summary>
        public static IEqualityComparer EntityEqualityComparer = new ValueObjectEqualityComparer();

        /// <summary>
        /// Test Contexts of UnitOfWork
        /// </summary>
        public static List<UnitOfWorkTestContext> Contexts = new List<UnitOfWorkTestContext>();

        /// <summary>
        /// Current Test Context
        /// </summary>
        public static UnitOfWorkTestContext CurrentContext;

        private const string TestModeKey = @"NSoft.NFramework.Data.NH.ForTesting.FluentDatabaseTestFixtureBase.IsRunningInTestMode";

        /// <summary>
        /// 테스트모드에서 실행중인지 검사
        /// </summary>
        public static bool IsRunningInTestMode {
            get { return (bool)(Local.Data[TestModeKey] ?? false); }
            private set { Local.Data[TestModeKey] = value; }
        }

        /// <summary>
        /// NHibernate 용 Domain을 테스트하기 위해, 환경을 초기화합니다.
        /// </summary>
        /// <param name="containerConfigPath">Castle Windsor configuration file path</param>
        /// <param name="databaseEngine">Database 종류</param>
        /// <param name="databaseName">Database name</param>
        /// <param name="mappingInfo">Domain Model의 Mapping information</param>
        /// <param name="configAction">configuration 부가 작업</param>
        /// <param name="conventions">Fluent NHibernate의 명명규칙</param>
        public static void InitializeNHibernateAndIoC(string containerConfigPath,
                                                      DatabaseEngine databaseEngine,
                                                      string databaseName,
                                                      MappingInfo mappingInfo,
                                                      Action<NHibernate.Cfg.Configuration> configAction,
                                                      params IConvention[] conventions) {
            NHibernateInitializer
                .Initialize(mappingInfo, configAction, conventions)
                .Using(databaseEngine, databaseName)
                .AndIoC(containerConfigPath);
        }

        /// <summary>
        /// NHibernate 용 Domain을 테스트하기 위해, 환경을 초기화합니다.
        /// </summary>
        /// <param name="containerConfigPath">Castle Windsor configuration file path</param>
        /// <param name="databaseEngine">Database 종류</param>
        /// <param name="databaseName">Database name</param>
        /// <param name="mappingInfo">Domain Model의 Mapping information</param>
        /// <param name="properties">NHibernate configuration 정보</param>
        /// <param name="conventions">Fluent NHibernate의 명명규칙</param>
        public static void InitializeNHibernateAndIoC(string containerConfigPath,
                                                      DatabaseEngine databaseEngine,
                                                      string databaseName,
                                                      MappingInfo mappingInfo,
                                                      IDictionary<string, string> properties,
                                                      params IConvention[] conventions) {
            NHibernateInitializer
                .Initialize(mappingInfo, null, conventions)
                .Using(databaseEngine, databaseName)
                .ConfiguredBy(properties)
                .AndIoC(containerConfigPath);
        }

        /// <summary>
        /// NHibernate 용 Domain을 테스트하기 위해, 환경을 초기화합니다.
        /// </summary>
        /// <param name="containerConfigPath">Castle Windsor configuration file path</param>
        /// <param name="databaseEngine">Database 종류</param>
        /// <param name="databaseName">Database name</param>
        /// <param name="mappingInfo">Domain Model의 Mapping information</param>
        /// <param name="properties">NHibernate configuration 정보</param>
        /// <param name="configAction">NHIbernate configuration 을 추가해야 할 경우 (Listner 등)</param>
        /// <param name="conventions">Fluent NHibernate의 명명규칙</param>
        public static void InitializeNHibernateAndIoC(string containerConfigPath,
                                                      DatabaseEngine databaseEngine,
                                                      string databaseName,
                                                      MappingInfo mappingInfo,
                                                      IDictionary<string, string> properties,
                                                      Action<NHibernate.Cfg.Configuration> configAction,
                                                      params IConvention[] conventions) {
            NHibernateInitializer
                .Initialize(mappingInfo, configAction, conventions)
                .Using(databaseEngine, databaseName)
                .ConfiguredBy(properties)
                .AndIoC(containerConfigPath);
        }

        /// <summary>
        /// NHibernate 용 Domain을 테스트하기 위해, 환경을 초기화합니다.
        /// </summary>
        /// <param name="containerConfigPath">Castle Windsor configuration file path</param>
        /// <param name="mappingInfo">Domain Model의 Mapping information</param>
        /// <param name="configAction">부가적인 Configuration 관련 작업 (Listener 추가 등)</param>
        /// <param name="conventions">Fluent NHibernate의 명명규칙</param>
        public static void InitializeNHibernateAndIoC(string containerConfigPath,
                                                      MappingInfo mappingInfo,
                                                      Action<NHibernate.Cfg.Configuration> configAction,
                                                      params IConvention[] conventions) {
            NHibernateInitializer.Initialize(mappingInfo, configAction, conventions).AndIoC(containerConfigPath);
        }

        /// <summary>
        /// NHibernate 용 Domain을 테스트하기 위해, 환경을 초기화합니다.
        /// </summary>
        /// <param name="containerConfigPath">Castle Windsor configuration file path</param>
        /// <param name="databaseEngine">Database 종류</param>
        /// <param name="mappingInfo">Domain Model의 Mapping information</param>
        /// <param name="configAction">부가적인 Configuration 관련 작업 (Listener 추가 등)</param>
        /// <param name="conventions">Fluent NHibernate의 명명규칙</param>
        public static void InitializeNHibernateAndIoC(string containerConfigPath,
                                                      DatabaseEngine databaseEngine,
                                                      MappingInfo mappingInfo,
                                                      Action<NHibernate.Cfg.Configuration> configAction,
                                                      params IConvention[] conventions) {
            NHibernateInitializer
                .Initialize(mappingInfo, configAction, conventions)
                .Using(databaseEngine, null)
                .AndIoC(containerConfigPath);
        }

        /// <summary>
        /// NHibernate 를 초기화 한다. 
        /// </summary>
        /// <param name="mappingInfo">엔티티 매핑 정보</param>
        /// <param name="configAction">환경설정 추가 작업용 델리게이트</param>
        /// <param name="conventions">Fluent NHibernate의 명명규칙</param>
        public static void InitializeNHibernate(MappingInfo mappingInfo,
                                                Action<NHibernate.Cfg.Configuration> configAction,
                                                params IConvention[] conventions) {
            NHibernateInitializer.Initialize(mappingInfo, configAction, conventions);
        }

        /// <summary>
        /// Throw away all <see cref="UnitOfWorkTestContext"/> objects within <see cref="Contexts"/>
        /// and referenced by <see cref="CurrentContext"/>. WARNING: Subsequent calls to  <see cref="InitializeNHibernateAndIoC(string,DatabaseEngine,string,NFramework.Data.NHibernateEx.ForTesting.MappingInfo,System.Action{NHibernate.Cfg.Configuration})"/>
        /// and all its overloads will now take considerably longer as the persistent framework will
        /// be initialised a fresh.
        /// </summary>
        /// <remarks>
        /// This method should be used vary sparingly. It is highly unlikely that you will need to
        /// call this method between every test.
        /// <para>
        /// Calling this method will dispose of <see cref="UnitOfWorkTestContext"/> objects within
        /// <see cref="Contexts"/>. Each context maintains a reference to a <see cref="WindsorContainer"/>. 
        /// If this container object is referenced by <see cref="IoC"/>.
        /// <see cref="IoC.Container"/> then any subsequent calls to <see cref="IoC"/>.
        /// <see cref="IoC.Resolve(Type)"/> and any of the overloads will throw.
        /// </para>
        /// </remarks>
        public static void DisposeAndRemoveAllUoWTestContexts() {
            if(IsDebugEnabled)
                log.Debug("Dispose and remove all unit of work testing context is starting...");

            foreach(var context in Contexts) {
                var context1 = context;

                With.TryAction(() => {
                                   if(context1 != null) {
                                       context1.Dispose();
                                       context1 = null;
                                   }
                               },
                               ex => {
                                   if(log.IsWarnEnabled)
                                       log.Warn(ex);
                               });
            }

            CurrentContext.DisposeUnitOfWork();
            CurrentContext = null;
            IsRunningInTestMode = false;
            Contexts.Clear();

            if(log.IsInfoEnabled)
                log.Info("All testing context of unit of work is removed.");
        }

        /// <summary>
        /// NHibernate 초기화를 수행하는 Initializer 를 제공합니다.
        /// </summary>
        /// <param name="mappingInfo">매핑 정보</param>
        /// <param name="configAction">환경 설정 추가 작업용 델리게이트</param>
        /// <returns>NHibernateInitializer 인스턴스</returns>
        /// <param name="conventions">Fluent NHibernate의 명명규칙</param>
        public static NHibernateInitializer Initialize(MappingInfo mappingInfo, Action<NHibernate.Cfg.Configuration> configAction,
                                                       params IConvention[] conventions) {
            return new NHibernateInitializer(mappingInfo, configAction, conventions);
        }

#pragma warning disable 1591

        /// <summary>
        /// NHibernate 초기화를 수행합니다.
        /// </summary>
        public class NHibernateInitializer {
            private readonly MappingInfo _mappingInfo;
            private DatabaseEngine _databaseEngine = DatabaseEngine.SQLite;
            private string _databaseName;
            private IDictionary<string, string> _nhibernateConfigurationProperties = new Dictionary<string, string>();
            private readonly IoCInitializer _ioc;

            public DatabaseEngine DatabaseEngine {
                get { return _databaseEngine; }
            }

            public string DatabaseName {
                get { return _databaseName; }
            }

            public IDictionary<string, string> NHibernateConfigurationProperties {
                get { return _nhibernateConfigurationProperties; }
            }

            public MappingInfo MappingInfo {
                get { return _mappingInfo; }
            }

            public Action<NHibernate.Cfg.Configuration> ConfigAction { get; set; }

            public IConvention[] Conventions { get; set; }

            protected internal NHibernateInitializer(MappingInfo mappingInfo,
                                                     Action<NHibernate.Cfg.Configuration> configAction,
                                                     params IConvention[] conventions) {
                mappingInfo.ShouldNotBeNull("mappingInfo");
                _mappingInfo = mappingInfo;
                _ioc = new IoCInitializer(this);


                Conventions = conventions ?? new IConvention[] { new PascalNamingConvention() };
                ConfigAction = configAction;
            }

            public static NHibernateInitializer Initialize(MappingInfo mappingInfo,
                                                           Action<NHibernate.Cfg.Configuration> configAction,
                                                           params IConvention[] conventions) {
                IoC.Reset();
                return new NHibernateInitializer(mappingInfo, configAction, conventions);
            }

            public NHibernateInitializer Using(DatabaseEngine databaseEngine, string databaseName) {
                MappingContext.DatabaseEngine = databaseEngine;

                _databaseEngine = databaseEngine;
                _databaseName = databaseName;

                return this;
            }

            public NHibernateInitializer ConfiguredBy(IDictionary<string, string> nhibernateConfigurationProperties) {
                _nhibernateConfigurationProperties = nhibernateConfigurationProperties;
                return this;
            }

            public void Now() {
                InternalComplete();
            }

            public void AndIoC(string containerConfigPath) {
                _ioc.With(containerConfigPath);
                InternalComplete();
            }

            public void AndIoC(IWindsorContainer container) {
                _ioc.With(container);
                InternalComplete();
            }

            private void InternalComplete() {
                if(_databaseName.IsWhiteSpace())
                    _databaseName = DeriveDatabaseNameFrom(_databaseEngine, _mappingInfo.MappingAssemblies[0]);

                var context = _ioc.GetUnitOfWorkTestContext();
                IsRunningInTestMode = true;

                if(Equals(context, CurrentContext) == false && IsInversionOfControlContainerOutOfSyncWith(context))
                    context.InitializeContainerAndUnitOfWorkFactory();

                CurrentContext = context;
                Debug.Print("CurrentContext is: " + CurrentContext);
            }

            public static string DeriveDatabaseNameFrom(DatabaseEngine databaseEngine, Assembly assembly) {
                string dbName;

                if(databaseEngine == DatabaseEngine.SQLite)
                    dbName = UnitOfWorkTestContextDbStrategy.SQLiteDbName;
                    //else if(databaseEngine == DatabaseEngine.MsSqlCe)
                    //    dbName = "TempDB.sdf";
                else
                    dbName = DeriveDatabaseNameFrom(assembly) + @"_Test";

                if(IsDebugEnabled)
                    log.Debug("Drived Database name is [{0}]", dbName);

                return dbName;
            }

            private static string DeriveDatabaseNameFrom(Assembly assembly) {
                var assemblyNameParts = assembly.GetName().Name.Split('.');
                if(assemblyNameParts.Length > 1) {
                    // assumes that the first part of the assembly name is the Company name
                    // and the second part is the Project name
                    return assemblyNameParts[1];
                }

                return assemblyNameParts[0];
            }

            private static bool IsInversionOfControlContainerOutOfSyncWith(UnitOfWorkTestContext context) {
                return (IoC.IsNotInitialized) != (context.Container == null);
            }
        }

        protected class IoCInitializer {
            private readonly NHibernateInitializer _root;
            private string _containerConfigPath;
            private IWindsorContainer _container;

            protected internal IoCInitializer(NHibernateInitializer nHibernateInitializer) {
                _root = nHibernateInitializer;
            }

            protected internal NHibernateInitializer With(string containerConfigPath) {
                _containerConfigPath = containerConfigPath;
                return _root;
            }

            protected internal NHibernateInitializer With(IWindsorContainer container) {
                _container = container;
                return _root;
            }

            protected internal UnitOfWorkTestContext GetUnitOfWorkTestContext() {
                Predicate<UnitOfWorkTestContext> criteria;

                if(_container == null) {
                    criteria = ctx => ctx.ContainerConfigPath == StringOrEmpty(_containerConfigPath) &&
                                      ctx.DatabaseEngine == _root.DatabaseEngine &&
                                      ctx.DatabaseName == _root.DatabaseName;
                }
                else {
                    criteria = ctx => ctx.Container == _container &&
                                      ctx.DatabaseEngine == _root.DatabaseEngine &&
                                      ctx.DatabaseName == _root.DatabaseName;
                }

                var context = Contexts.Find(criteria);

                if(context == null) {
                    var dbStrategy = UnitOfWorkTestContextDbStrategy.For(_root.DatabaseEngine,
                                                                         _root.DatabaseName,
                                                                         _root.NHibernateConfigurationProperties);

                    context = (_container != null)
                                  ? UnitOfWorkTestContext.FluentFor(_container, dbStrategy, _root.MappingInfo, _root.ConfigAction,
                                                                    _root.Conventions)
                                  : UnitOfWorkTestContext.FluentFor(_containerConfigPath, dbStrategy, _root.MappingInfo,
                                                                    _root.ConfigAction, _root.Conventions);

                    Contexts.Add(context);

                    if(IsDebugEnabled)
                        log.Debug("Create another UnitOfWorkContext for: [{0}]", context);
                }
                return context;
            }

            private static string StringOrEmpty(string s) {
                return s ?? string.Empty;
            }
        }

#pragma warning restore 1591
    }
}