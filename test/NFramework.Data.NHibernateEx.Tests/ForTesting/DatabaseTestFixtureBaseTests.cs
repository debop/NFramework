using System;
using System.IO;
using System.Reflection;
using Castle.Windsor;
using NHibernate;
using NHibernate.Cfg;
using NSoft.NFramework.Data.NHibernateEx.Domain;
using NSoft.NFramework.Data.NHibernateEx.Facilities;
using NSoft.NFramework.InversionOfControl;
using NSoft.NFramework.Tools;
using NUnit.Framework;

namespace NSoft.NFramework.Data.NHibernateEx.ForTesting {
    /// <summary>
    /// DatabaseTestFixtureBase 를 테스트 하기 위한 Class
    /// </summary>
    [TestFixture]
    public class DatabaseTestFixtureBaseTests : FluentDatabaseTestFixtureBase {
        [SetUp]
        public void TestInitlaize() {
            // NOTE : 정상적으로는 Context를 Dispose 할 필요없다. (속도에 영향을 미친다.)
            // 여기서는 오로지 테스트를 위해서만 사용한다.
            //IoC.Reset();
            //DisposeAndRemoveAllUoWTestContexts();
        }

        [TearDown]
        public void TestCleanUp() {
            // NOTE : 정상적으로는 Context를 Dispose 할 필요없다. (속도에 영향을 미친다.)
            // 여기서는 오로지 테스트를 위해서만 사용한다.
            IoC.Reset();
            DisposeAndRemoveAllUoWTestContexts();
        }

        [Test]
        public void CanCreateUnitOfWorkContextFor_SQLite() {
            VerifyCanCreateUnitOfWorkContextFor(null, DatabaseEngine.SQLiteForFile);
            VerifyCanCreateUseAndDisposeSession();
        }

        [Test]
        public void CanCreateUnitOfWorkContextFor_SQLite_IoC() {
            VerifyCanCreateUnitOfWorkContextFor(ContainerFilePath, DatabaseEngine.SQLiteForFile);
            VerifyCanCreateUseAndDisposeSession();
            VerifyCanCreateUseAndDisposeUnitOfWork();
        }

        //[Test]
        //public  void CanCreateUnitOfWorkContextFor_SQLiteForFile()
        //{
        //    VerifyCanCreateUnitOfWorkContextFor(null, DatabaseEngine.SQLiteForFile);
        //    VerifyCanCreateUseAndDisposeSession();
        //}
        //[Test]
        //public  void CanCreateUnitOfWorkContextFor_SQLiteForFile_IoC()
        //{
        //    VerifyCanCreateUnitOfWorkContextFor(ContainerFilePath, DatabaseEngine.SQLiteForFile);

        //    VerifyCanCreateUseAndDisposeSession();
        //    VerifyCanCreateUseAndDisposeUnitOfWork();
        //}
        [Test]
        public void CanCreateUnitOfWorkContextFor_MsSql2005() {
            if(UnitOfWorkTestContextDbStrategy.IsSqlServer2005OrAboveInstalled()) {
                VerifyCanCreateUnitOfWorkContextFor(null, DatabaseEngine.MsSql2005);
                VerifyCanCreateUseAndDisposeSession();
            }
        }

        [Test]
        public void CanCreateUnitOfWorkContextFor_MsSql2005_IoC() {
            if(UnitOfWorkTestContextDbStrategy.IsSqlServer2005OrAboveInstalled()) {
                VerifyCanCreateUnitOfWorkContextFor(ContainerFilePath, DatabaseEngine.MsSql2005);
                VerifyCanCreateUseAndDisposeSession();
                VerifyCanCreateUseAndDisposeUnitOfWork();
            }
        }

        [Test]
        [Ignore("SQLExpress 가 실행되고 있어야 합니다.")]
        public void CanCreateUnitOfWorkContextFor_MsSql2005Express() {
            if(UnitOfWorkTestContextDbStrategy.IsSqlServer2005OrAboveInstalled()) {
                VerifyCanCreateUnitOfWorkContextFor(null, DatabaseEngine.MsSql2005Express);
                VerifyCanCreateUseAndDisposeSession();
            }
        }

        [Test]
        [Ignore("SQLExpress 가 실행되고 있어야 합니다.")]
        public void CanCreateUnitOfWorkContextFor_MsSql2005Express_IoC() {
            if(UnitOfWorkTestContextDbStrategy.IsSqlServer2005OrAboveInstalled()) {
                VerifyCanCreateUnitOfWorkContextFor(ContainerFilePath, DatabaseEngine.MsSql2005Express);
                VerifyCanCreateUseAndDisposeSession();
                VerifyCanCreateUseAndDisposeUnitOfWork();
            }
        }

        [Test]
        public void EachUnitOfWorkContextConfigurationWillBeCreatedOnlyOnce() {
            InitializeNHibernateAndIoC(ContainerFilePath, DatabaseEngine.SQLite, string.Empty);
            InitializeNHibernateAndIoC(ContainerFilePath, DatabaseEngine.SQLite, string.Empty);

            Assert.AreEqual(1, Contexts.Count);
        }

        [Test]
        public void NewUnitOfWorkContextCreatedForDifferentDatabaseNames() {
            if(UnitOfWorkTestContextDbStrategy.IsSqlServer2005OrAboveInstalled()) {
                VerifyCanCreateUnitOfWorkContextFor(ContainerFilePath, DatabaseEngine.MsSql2005, "TestDb1");
                VerifyCanCreateUnitOfWorkContextFor(ContainerFilePath, DatabaseEngine.MsSql2005, "TestDb2");

                Assert.AreEqual(2, Contexts.Count);
            }
        }

        [Test]
        public void NewUnitOfWorkContextCreatedForDifferentWindsorConfigFiles() {
            VerifyCanCreateUnitOfWorkContextFor(ContainerFilePath, DatabaseEngine.SQLite);
            VerifyCanCreateUnitOfWorkContextFor(ContainerFilePath2, DatabaseEngine.SQLite);

            Assert.AreEqual(2, Contexts.Count);
        }

        //[Test]
        //public  void SwitchingBetweenExistingContextsHasAcceptablePerformance()
        //{
        //    // Create SQLite context for the first time. Use context to touch all moving parts
        //    InitializeNHibernateAndIoC(ContainerFilePath, DatabaseEngine.SQLiteForFile, string.Empty);
        //    VerifyCanCreateUseAndDisposeUnitOfWork();

        //    // Create another context and ensure all its component parts are used
        //    // We're doing this so that the SQLite context created above is no longer current.
        //    InitializeNHibernateAndIoC(ContainerFilePath, DatabaseEngine.SQLite, string.Empty);
        //    VerifyCanCreateUseAndDisposeUnitOfWork();

        //    // Reinstate and use existing SQLite context.
        //    double timing = With.OperationTimer(() =>
        //                                        {
        //                                            InitializeNHibernateAndIoC(ContainerFilePath, DatabaseEngine.SQLiteForFile, string.Empty);
        //                                            VerifyCanCreateUseAndDisposeUnitOfWork();
        //                                        });
        //    // less than 200 msecs
        //    Assert.Less(timing, 1000, "reinstating then using existsing context sufficiently performance.");
        //}
        //[Test]
        //public  void CanCreateNestedUnitOfWork()
        //{
        //    InitializeNHibernateAndIoC(ContainerFilePath, DatabaseEngine.SQLite, string.Empty);

        //    VerifyCanCreateUseAndDisposeNestedUnitOfWork();
        //}
        //[Test]
        //public  void CallingCreateUnitOfWorkMoreThanOnceIsNotAllowed()
        //{
        //    InitializeNHibernateAndIoC(ContainerFilePath, DatabaseEngine.SQLite, string.Empty);

        //    CurrentContext.CreateUnitOfWork();
        //    try
        //    {
        //        CurrentContext.CreateUnitOfWork();
        //        Assert.Fail("Exception was expected.");
        //    }
        //    catch(InvalidOperationException e)
        //    {
        //        Assert.AreEqual("Can't create a nested UnitOfWork with this method. Use CreateNestedUnitOfWork() instead.", e.Message);
        //    }
        //    finally
        //    {
        //        CurrentContext.DisposeUnitOfWork();
        //    }
        //}
        [Test]
        public void CanInitializeWithFluentInterfaceAndContainerInstance() {
            var mappingInfo = MappingInfo.FromAssemblyContaining<GuidEntityForTesting>();

            IWindsorContainer container = new WindsorContainer();
            container.AddFacility(
                new NHUnitOfWorkFacility(new NHUnitOfWorkFacilityConfig(Assembly.GetAssembly(typeof(GuidEntityForTesting)))));

            Initialize(mappingInfo, null).AndIoC(container);

            Assert.AreSame(container, CurrentContext.Container);
        }

        #region << Proptected >>

        protected void InitializeNHibernateAndIoC(string containerPath, DatabaseEngine databaseEngine, string databaseName) {
            InitializeNHibernateAndIoC(containerPath,
                                       databaseEngine,
                                       databaseName,
                                       MappingInfo.FromAssemblyContaining<GuidEntityForTesting>(),
                                       _ => { });
        }

        protected void VerifyCanCreateUnitOfWorkContextFor(string containerPath, DatabaseEngine databaseEngine) {
            VerifyCanCreateUnitOfWorkContextFor(containerPath, databaseEngine, string.Empty);
        }

        protected void VerifyCanCreateUnitOfWorkContextFor(string containerPath, DatabaseEngine databaseEngine, string databaseName) {
            int nextContextPosition = Contexts.Count;

            // create the UnitOfWorkContext
            //

            var mappingInfo = MappingInfo.From(typeof(GuidEntityForTesting).Assembly, typeof(FluentProduct).Assembly);

            InitializeNHibernateAndIoC(containerPath,
                                       databaseEngine,
                                       databaseName,
                                       mappingInfo,
                                       _ => { });

            var context = Contexts[nextContextPosition];

            Assert.IsNotNull(context);

            if(containerPath != null)
                Assert.AreEqual(containerPath, context.ContainerConfigPath);
            else
                Assert.IsEmpty(context.ContainerConfigPath);

            Assert.AreEqual(databaseEngine, context.DatabaseEngine);

            if(databaseName.IsWhiteSpace())
                Assert.AreEqual(NHibernateInitializer.DeriveDatabaseNameFrom(databaseEngine, mappingInfo.MappingAssemblies[0]),
                                context.DatabaseName);
            else
                Assert.AreEqual(databaseName, context.DatabaseName);

            Assert.AreEqual(CurrentContext, context, "Context just build has been assigned to CurrentContext");
        }

        protected void VerifyCanCreateUseAndDisposeSession() {
            ISession session = null;
            try {
                if(IoC.IsNotInitialized)
                    IoC.Initialize();
                CurrentContext.CreateUnitOfWork();

                session = CurrentContext.CreateSession();
                Assert.IsNotNull(session);
                session.Save(new GuidEntityForTesting());
                session.Flush();
            }
            finally {
                CurrentContext.DisposeSession(session);
                CurrentContext.DisposeUnitOfWork();
            }
        }

        protected void VerifyCanCreateUseAndDisposeUnitOfWork() {
            try {
                CurrentContext.CreateUnitOfWork();
                Console.WriteLine(UnitOfWork.CurrentSession.Connection.ConnectionString);
                UnitOfWork.CurrentSession.Save(new GuidEntityForTesting());
                UnitOfWork.CurrentSession.Flush();
            }
            finally {
                CurrentContext.DisposeUnitOfWork();
            }
        }

        protected void VerifyCanCreateUseAndDisposeNestedUnitOfWork() {
            Assert.AreEqual(-1, CurrentContext.UnitOfWorkNestingLevel, "level before starting UnitOfWork = -1");

            CurrentContext.CreateUnitOfWork();
            Assert.AreEqual(0, CurrentContext.UnitOfWorkNestingLevel, "level before starting UnitOfWork = 0");

            CurrentContext.CreateNestedUnitOfWork();
            Assert.AreEqual(1, CurrentContext.UnitOfWorkNestingLevel, "level after staring Nested UnitOfWork = 1");

            // in nested unit-of-work
            UnitOfWork.CurrentSession.Save(new GuidEntityForTesting());
            UnitOfWork.CurrentSession.Flush();
            CurrentContext.DisposeUnitOfWork();

            // in original unit-of-work
            UnitOfWork.CurrentSession.Save(new GuidEntityForTesting());
            UnitOfWork.CurrentSession.Flush();
            CurrentContext.DisposeUnitOfWork();
        }

        protected virtual string ContainerFilePath {
            get { return Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @".\ForTesting\Windsor.config")); }
        }

        protected virtual string ContainerFilePath2 {
            get { return Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @".\ForTesting\Windsor2.config")); }
        }

        protected static string WindsorFilePath {
            get { return Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @".\ForTesting\Windsor.config")); }
        }

        #endregion

        #region << InitializeAware Test >>

        [Test]
        public void TestThatNHInitializeAwareIsCalled() {
            var mock = new NHInitAwareMock
                       {
                           ConfiguredWasCalled = 0,
                           InitializedWasCalled = 0
                       };

            InitializeNHibernateAndIoC(ContainerFilePath,
                                       DatabaseEngine.SQLite,
                                       ":memory:",
                                       MappingInfo.From().SetNHInitializationAware(mock),
                                       _ => { });
            Assert.AreEqual(1, mock.BeforeInitializationCalled);
            Assert.AreEqual(1, mock.ConfiguredWasCalled);
            Assert.AreEqual(1, mock.InitializedWasCalled);
        }

        #region << NHInitAwareMock >>

        private class NHInitAwareMock : INHInitializationAware {
            public int ConfiguredWasCalled;
            public int InitializedWasCalled;
            public int BeforeInitializationCalled;

            #region Implementation of INHInitializationAware

            /// <summary>
            /// 초기화 전에 수행해야 할 작업
            /// </summary>
            public void BeforeInitialzation() {
                BeforeInitializationCalled++;
            }

            /// <summary>
            /// NHibernate Configuration 작업에 추가할 내용들을 정의한다.
            /// </summary>
            /// <param name="cfg"></param>
            public void Configured(Configuration cfg) {
                ConfiguredWasCalled++;
            }

            /// <summary>
            /// NHibernate Session Factory 초기화 작업에 추가할 내용들을 정의한다.
            /// </summary>
            /// <param name="cfg"></param>
            /// <param name="sessionFactory"></param>
            public void Initialized(Configuration cfg, ISessionFactory sessionFactory) {
                InitializedWasCalled++;
            }

            #endregion
        }

        #endregion

        #endregion
    }
}