using System.Threading;
using NSoft.NFramework.InversionOfControl;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework.Data.NHibernateEx.UnitOfWorks {
    [TestFixture]
    public class NHUnitOfWorkFactoryFixture : IoCSetUpBase {
        [Test]
        public void DefaultFactory() {
            Assert.AreEqual(new NHUnitOfWorkFactory().ConfigurationFileName, NHUnitOfWorkFactory.DEFAULT_CONFIG_FILENAME);
        }

        [Test]
        public void IoCFactory() {
            var factoryInLocal = new ThreadLocal<IUnitOfWorkFactory>(IoC.TryResolve<IUnitOfWorkFactory, NHUnitOfWorkFactory>);

            var factory = factoryInLocal.Value;

            Assert.IsNotNull(factory);
            Assert.IsInstanceOf<NHUnitOfWorkFactory>(factory);

            // Windsor.Hibernate.config 파일을 보면
            // Assert.AreEqual(NHUnitOfWorkFactory.DEFAULT_CONFIG_FILENAME, factory.ConfigurationFileName);
        }

        [Test]
        public void UnitOfWork_ThreadSafely() {
            using(UnitOfWork.Start(UnitOfWorkNestingOptions.CreateNewOrNestUnitOfWork)) {
                Assert.IsTrue(UnitOfWork.IsStarted);
                Assert.IsNotNull(UnitOfWork.Current);
            }
        }

        [Test]
        public void Thread_Test() {
            TestTool.RunTasks(15,
                              DefaultFactory,
                              IoCFactory,
                              UnitOfWork_ThreadSafely);
        }
    }
}