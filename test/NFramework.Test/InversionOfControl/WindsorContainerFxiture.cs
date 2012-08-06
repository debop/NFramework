using System;
using System.Data.SqlClient;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using NUnit.Framework;

namespace NSoft.NFramework.InversionOfControl {
    [TestFixture]
    public class WindsorContainerFxiture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        [TearDown]
        public void TearDown() {
            if(IoC.IsInitialized)
                IoC.Reset();
        }

        [Test]
        public void LocalContainerOverrideGlobalOne() {
            var container = new WindsorContainer();

            IoC.Initialize(container);
            Assert.AreSame(container, IoC.Container);

            var localContainer = new WindsorContainer();

            using(IoC.UseLocalContainer(localContainer)) {
                Assert.AreSame(localContainer, IoC.Container);
            }

            Assert.AreSame(container, IoC.Container);
        }

        /// <summary>
        /// 정의되지 않은 Windsor Component를 Resolve할 때 예외를 발생시키니 않는다.
        /// </summary>
        [Test]
        public void WillNotThrowIfTryingToResolveNonExistiingComponent() {
            IoC.Initialize(new WindsorContainer());

            var disposable = IoC.TryResolve<IDisposable>(); // 정의되어 있지 않다.
            Assert.IsNull(disposable);
        }

        [Test]
        public void WillResolveComponentsInTryResolveIfRegistered() {
            IoC.Initialize(new WindsorContainer());

            // 컴포넌트 등록
            // IoC.Container.AddComponent<IDisposable, SqlConnection>(); // Component 등록
            IoC.Container.Register(Component.For<IDisposable>().ImplementedBy<SqlConnection>());

            // 컴포넌트 Resolve
            var disposable = IoC.TryResolve<IDisposable>();

            Assert.IsNotNull(disposable);
            Assert.IsInstanceOf<SqlConnection>(disposable);
        }

        [Test]
        public void CanSpecifyDefaultValueWhenTryingToResolve() {
            IoC.Initialize(new WindsorContainer());

            var disposable = IoC.TryResolve<IDisposable>(new SqlConnection());
            Assert.IsNotNull(disposable);
            Assert.IsInstanceOf<SqlConnection>(disposable);
        }
    }
}