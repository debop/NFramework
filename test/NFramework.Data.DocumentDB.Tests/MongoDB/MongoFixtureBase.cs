using System;
using System.Threading;
using MongoDB.Driver;
using NLog;
using NSoft.NFramework.InversionOfControl;
using NUnit.Framework;

namespace NSoft.NFramework.Data.MongoDB {
    public abstract class MongoFixtureBase {
        #region << logger >>

        protected static readonly Logger log = LogManager.GetCurrentClassLogger();
        protected static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public static readonly string DefaultConnectionString = @"server=localhost;database=NFrameworkTest;safe=true;";
        public const string DefaultDatabaseName = @"NFrameworkTest";

        public const string TestCollectionName = "testcollection";

        public static MongoServer Server;
        public static MongoDatabase Database;

        [TestFixtureSetUp]
        public void TestFixtureSetUp() {
            OnTestFixtureSetUp();
        }

        [SetUp]
        public void SetUp() {
            OnSetUp();
        }

        [TearDown]
        public void TearDown() {
            OnTearDown();
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown() {
            OnTestFixtureTearDown();
        }

        protected virtual void OnTestFixtureSetUp() {

            if(IoC.IsNotInitialized)
                IoC.Initialize();

            if(IsDebugEnabled)
                log.Debug("MongoDB 서버에 접속합니다. connectionString=[{0}]", DefaultConnectionString);

            Server = MongoServer.Create(DefaultConnectionString);
            Server.Connect();
            Server.DropDatabase(DefaultDatabaseName);
            Database = Server[DefaultDatabaseName];
        }

        protected virtual void OnSetUp() {
            // Nothing To do
        }

        protected virtual void OnTearDown() {}

        protected virtual void OnTestFixtureTearDown() {
            if(_defaultRepository.IsValueCreated)
                _defaultRepository.Value.Dispose();

            if(_cacheRepository.IsValueCreated)
                _cacheRepository.Value.Dispose();

            if(IoC.IsInitialized)
                IoC.Reset();
        }

        private readonly ThreadLocal<IMongoRepository> _defaultRepository
            = new ThreadLocal<IMongoRepository>(() => IoC.Resolve<IMongoRepository>("MongoRepository.Default"));

        private readonly ThreadLocal<IMongoRepository> _cacheRepository
            = new ThreadLocal<IMongoRepository>(() => IoC.Resolve<IMongoRepository>("MongoRepository.NFramework.Cache"));

        /// <summary>
        /// IoC 를 통해 Resolve되었고, Lifestyle이 Thread 이므로, 같은 Thread Context에서는 같은 객체를 가르킵니다. 그러니 굳이 local 변수로 할당받지 않고, 사용해도 됩니다.
        /// </summary>
        public IMongoRepository DefaultRepository {
            get { return _defaultRepository.Value; }
        }

        /// <summary>
        /// IoC 를 통해 Resolve되었고, Lifestyle이 Thread 이므로, 같은 Thread Context에서는 같은 객체를 가르킵니다. 그러니 굳이 local 변수로 할당받지 않고, 사용해도 됩니다.
        /// </summary>
        public IMongoRepository CacheRepository {
            get { return _cacheRepository.Value; }
        }

        private static readonly Version version_1_7 = new Version(1, 7, 0, 0);

        public bool IsServer_1_7_or_Higher {
            get { return Server.BuildInfo.Version >= version_1_7; }
        }
    }
}