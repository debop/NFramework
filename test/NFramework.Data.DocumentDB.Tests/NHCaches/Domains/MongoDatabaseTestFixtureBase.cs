using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NHibernate.Criterion;
using NHibernate.Transform;
using NSoft.NFramework.Data.NHibernateEx;
using NSoft.NFramework.Data.NHibernateEx.ForTesting;
using NSoft.NFramework.LinqEx;
using NUnit.Framework;

namespace NSoft.NFramework.Data.MongoDB.NHCaches.Domains {
    /// <summary>
    /// 모든 NHibernate 테스트용은 이 클래스를 상속받고, 환경 변경은 각 함수들을 overriding 해서 사용하면 됩니다.
    /// </summary>
    public abstract class MongoDatabaseTestFixtureBase : FluentDatabaseTestFixtureBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        public const string MongoCacheProvider =
            @"NSoft.NFramework.Data.MongoDB.NHCaches.MongoCacheProvider, NSoft.NFramework.Data.MongoDB";

        #region << For Unit Testing >>

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
            InitializeNHibernateAndIoC(ContainerFilePath, GetDatabaseEngine(), GetDatabaseName(), GetMappingInfo(),
                                       GetNHibernateProperties());
            CurrentContext.CreateUnitOfWork();
        }

        protected virtual void OnSetUp() {}
        protected virtual void OnTearDown() {}

        protected virtual void OnTestFixtureTearDown() {
            DisposeAndRemoveAllUoWTestContexts();
        }

        #endregion

        #region << Overrides >>

        /// <summary>
        /// unit-of-work를 위한 IoC 설정 파일 경로입니다. 기본은 IoC.Testing.config 입니다.
        /// </summary>
        protected virtual string ContainerFilePath {
            get { return Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"IoC.NHibernate.config")); }
        }

        /// <summary>
        /// 테스트용 Database 를 지정합니다.
        /// </summary>
        /// <returns></returns>
        protected virtual DatabaseEngine GetDatabaseEngine() {
            return DatabaseEngine.MsSql2005;
        }

        /// <summary>
        /// 테스트에 쓰일 NHibernate용 Entity가 정의된 Assembly에서 매핑 정보를 반환합니다.
        /// </summary>
        /// <example>
        /// <code>
        /// return MappingInfo.From(typeof(User).Assembly, typeof(Project).Assembly);
        /// </code>
        /// </example>
        protected virtual MappingInfo GetMappingInfo() {
            // return MappingInfo.From(typeof(IActor).Assembly);
            return MappingInfo.From(Assembly.GetExecutingAssembly());
        }

        /// <summary>
        /// NHibernate 설정을 추가할 수 있습니다.
        /// </summary>
        /// <returns></returns>
        protected virtual IDictionary<string, string> GetNHibernateProperties() {
            var properties = new Dictionary<string, string>();

            properties.Add(NHibernate.Cfg.Environment.CacheProvider, MongoCacheProvider);
            properties.Add(NHibernate.Cfg.Environment.UseSecondLevelCache, "True"); // Pascal 방식으로 써야 한다.

            return properties;
        }

        /// <summary>
        /// 테스트할 DB 이름입니다.
        /// </summary>
        /// <returns></returns>
        protected virtual string GetDatabaseName() {
            return @"NSoft_NFramework_MONGO";
        }

        #endregion

        #region << Helper Methods >>

        protected static IList<T> LoadAllWithDistinct<T>(NHibernate.Criterion.Order order) {
            return
                DetachedCriteria
                    .For<T>()
                    .SetResultTransformer(new DistinctRootEntityResultTransformer())
                    .GetExecutableCriteria(UnitOfWork.CurrentSession)
                    .AddOrder(order)
                    .List<T>();
        }

        protected static void SaveAndFlushToDatabase(object objectToSave) {
            SaveAndFlushToDatabase(new[] { objectToSave });
        }

        protected static void SaveInCurrentSession<T>(IEnumerable<T> objectsToSave) {
            foreach(var obj in objectsToSave)
                UnitOfWork.CurrentSession.SaveOrUpdate(obj);
        }

        protected static void SaveAndFlushToDatabase<T>(IEnumerable<T> objectsToSave) {
            SaveInCurrentSession(objectsToSave);
            UnitOfWork.CurrentSession.Flush();

            // Flush 하고 Clear를 수행한다.
            UnitOfWork.CurrentSession.Clear();
        }

        protected static List<T> ExpectedList<T>(params T[] items) {
            var expected = new List<T>();
            expected.AddRange(items);
            return expected;
        }

        protected static void AssertCollectionEqual<T>(ICollection<T> expected, ICollection<T> actual) {
            Assert.AreEqual(expected.Count, actual.Count, "두 컬렉션의 요소수가 다릅니다.");

            foreach(var item in expected) {
                var target = item;
                if(actual.ItemExists(x => x.Equals(target)) == false)
                    Assert.Fail("Actual 컬렉션에 다음 요소가 존재하지 않습니다. expected item=" + target);
            }
        }

        protected static void AssertSorted<T>(ICollection<T> items, string sortOrder, Comparison<T> sortBy) {
            var actual = new List<T>(items);
            var sorted = new List<T>(items);

            sorted.Sort(sortBy);
            if(sortOrder.ToLower() == "desc")
                sorted.Reverse();

            for(int i = 0; i < sorted.Count; i++)
                Assert.AreEqual(sorted[i], actual[i], "요소가 정렬되지 않았습니다.");
        }

        #endregion
    }
}