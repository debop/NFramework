using Castle.Windsor;
using Castle.Windsor.Installer;
using NSoft.NFramework.Compressions;
using NSoft.NFramework.Data.Mappers;
using NSoft.NFramework.InversionOfControl;
using NUnit.Framework;

namespace NSoft.NFramework.DataServices {
    public abstract class AbstractDataServiceFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        [TestFixtureSetUp]
        public void FixtureSetUp() {
            OnFixtureSetUp();
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
        public void FixtureTearDown() {
            OnFixtureTearDown();
        }

        protected virtual void OnFixtureSetUp() {
            if(IoC.IsNotInitialized) {
                var container = new WindsorContainer();

                container.Install(FromAssembly.This(),
                                  FromAssembly.Containing<ICompressor>(),
                                  FromAssembly.Containing<INameMapper>());

                IoC.Initialize(container);
            }
        }

        protected virtual void OnSetUp() {}
        protected virtual void OnTearDown() {}
        protected virtual void OnFixtureTearDown() {}
    }
}