using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework.Data.Repositories {
    [TestFixture]
    public class AdoRepositoryFactoryFixture {
        [Test]
        public void CreateAdoRepositoryIsThreadSafe() {
            var defaultRepository = AdoRepositoryFactory.Instance.CreateRepository();
            var pubsRepository = AdoRepositoryFactory.Instance.CreateRepository("Pubs");

            Assert.IsNotNull(defaultRepository);
            Assert.IsNotNull(pubsRepository);
        }

        [Test]
        public void CanClearRepositoryCache() {
            var defaultRepository = AdoRepositoryFactory.Instance.CreateRepository();
            var pubsRepository = AdoRepositoryFactory.Instance.CreateRepository("Pubs");

            Assert.IsNotNull(defaultRepository);
            Assert.IsNotNull(pubsRepository);

            AdoRepositoryFactory.Instance.ClearRepositoryCache();

            defaultRepository = AdoRepositoryFactory.Instance.CreateRepository();
            pubsRepository = AdoRepositoryFactory.Instance.CreateRepository("Pubs");

            Assert.IsNotNull(defaultRepository);
            Assert.IsNotNull(pubsRepository);
        }

        [Test]
        public void Thread_Test() {
            TestTool.RunTasks(5,
                              CreateAdoRepositoryIsThreadSafe,
                              CanClearRepositoryCache);
        }
    }
}