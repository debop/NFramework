using NSoft.NFramework.InversionOfControl;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Data.MongoDB {
    [TestFixture]
    public class IoCFixture : MongoFixtureBase {
        [Test]
        public void Can_Resolve_MongoRepository() {
            var repository = IoC.Resolve<IMongoRepository>("MongoRepository.Default");
            Assert.IsNotNull(repository);
            repository.DatabaseName.Should().Not.Be.Empty();
        }

        [Test]
        public void Can_Resolve_MongoRepository_By_Id() {
            var defaultRepository = DefaultRepository;
            Assert.IsNotNull(defaultRepository);
            Assert.IsNotEmpty(defaultRepository.Database.Name);

            var rclCacheRepository = CacheRepository;
            Assert.IsNotNull(rclCacheRepository);
            Assert.IsNotEmpty(rclCacheRepository.Database.Name);
        }

        [Test]
        public void Lifestyle_Thread_Is_Corrected() {
            var defaultRepository = DefaultRepository;

            System.Threading.Thread.Sleep(10);

            var defaultRepository2 = DefaultRepository;

            Assert.IsTrue(Equals(defaultRepository, defaultRepository2));
        }

        [Test]
        public void Can_ResolveAll_MongoRepository() {
            var repositories = IoC.ResolveAll<IMongoRepository>();

            CollectionAssert.AllItemsAreNotNull(repositories);

            foreach(var repository in repositories)
                repository.DatabaseName.Should().Contain("rcl");
        }
    }
}