using System;
using System.Data.SqlClient;
using System.Threading;
using Castle.Core;
using Castle.Windsor;
using NSoft.NFramework.Compressions;
using NSoft.NFramework.Compressions.Compressors;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.InversionOfControl {
    [TestFixture]
    public class IoCFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        private static readonly object _syncLock = new object();

        [Test]
        public void LocalContainer_Override_GlobalOne() {
            lock(_syncLock) {
                var container = new WindsorContainer();

                IoC.Initialize(container);

                Assert.AreSame(container, IoC.Container);

                var localContainer = new WindsorContainer();

                using(IoC.UseLocalContainer(localContainer)) {
                    Assert.AreSame(localContainer, IoC.Container);
                }

                Assert.AreSame(container, IoC.Container);
            }
            IoC.Reset();
        }

        /// <summary>
        /// 정의되지 않은 Windsor Component를 Resolve할 때 예외를 발생시키니 않는다.
        /// </summary>
        [Test]
        public void Will_NotThrowIfTryingToResolveNonExistingComponent() {
            if(IoC.IsNotInitialized)
                IoC.InitializeWithEmptyContainer();

            var disposable = IoC.TryResolve<IDisposable>(); // 정의되어 있지 않다.
            Assert.IsNull(disposable);
            IoC.Reset();
        }

        [Test]
        public void Will_ResolveComponentsInTryResolveIfRegistered() {
            if(IoC.IsNotInitialized)
                IoC.InitializeWithEmptyContainer();

            Thread.Sleep(0);

            // IoC.Container.AddComponent<IDisposable, SqlConnection>(); // Component 등록
            var disposable = IoC.TryResolve<IDisposable, SqlConnection>(); // 없으면 Component를 등록한다.
            Assert.IsNotNull(disposable);
            IoC.Reset();
        }

        [Test]
        public void Can_SpecifyDefaultValueWhenTryingToResolve() {
            // 빈 Container 로 초기화
            IoC.InitializeWithEmptyContainer();

            // SqlConnection 을 Compoinent로 등록하지 않았으므로, 지정된 ServiceType의 기본 값을 반환한다.
            var disposable = IoC.TryResolve<IDisposable>(new SqlConnection());
            Assert.IsTrue(disposable is SqlConnection);
            Assert.IsNotNull(disposable);
            IoC.Reset();
        }

        [Test]
        public void Can_TryResolveNotDefinedComponent() {
            // 빈 Container 로 초기화
            IoC.InitializeWithEmptyContainer();

            // ICompressor 서비스로 등록하기
            var compressor = IoC.TryResolve<ICompressor, GZipCompressor>(LifestyleType.Transient);

            Assert.IsNotNull(compressor);
            Assert.IsInstanceOf<GZipCompressor>(compressor);

            // ICompress 서비스의 컴포넌트 (instance) 를 Container에서 제거하기
            IoC.Container.Release(compressor);

            // instance가 release된 것이지, container의 component 가 제거된게 아니다.
            Assert.IsNotNull(IoC.Resolve<ICompressor>());

            // ICompressor 서비스가 GZipCompressor type으로 이미 등록되어 있으므로, GZipCompressor의 instance를 반환한다.
            compressor = IoC.TryResolve<ICompressor, DeflateCompressor>(LifestyleType.Thread);

            Assert.IsNotInstanceOf<DeflateCompressor>(compressor);
            Assert.IsInstanceOf<GZipCompressor>(compressor);
            IoC.Reset();
        }

        [Test]
        public void Can_TryResolveNotDefinedComponentByDelegate() {
            if(IoC.IsNotInitialized)
                IoC.InitializeWithEmptyContainer();

            var compressor = IoC.TryResolve<ICompressor>(() => new GZipCompressor(), true);
            Assert.IsNotNull(compressor);
            Assert.IsInstanceOf<GZipCompressor>(compressor);

            // is registered
            compressor = IoC.Resolve<ICompressor>();

            Assert.IsNotNull(compressor);
            Assert.IsInstanceOf<GZipCompressor>(compressor);

            // register other instance
            //
            IoC.Container.Release(compressor);
            var compressor2 = IoC.TryResolve<ICompressor>();
            Assert.IsNotNull(compressor2);
            Assert.IsInstanceOf<GZipCompressor>(compressor);

            IoC.Reset();
        }

        [Test]
        public void RegisterAllTypes() {
            if(IoC.IsNotInitialized)
                IoC.InitializeWithEmptyContainer();

            IoC.RegisterComponents(typeof(ICompressor).Assembly, typeof(ICompressor), LifestyleType.Transient);

            var compressors = IoC.ResolveAll(typeof(ICompressor));
            Assert.IsNotNull(compressors);
            compressors.Length.Should().Be.GreaterThan(0);

            foreach(var compressor in compressors)
                Console.WriteLine(compressor.GetType().FullName);

            ICompressor gzip = IoC.Resolve<ICompressor>(typeof(GZipCompressor).FullName);
            Assert.IsNotNull(gzip);
            Assert.IsInstanceOf<GZipCompressor>(gzip);

            IoC.Reset();
        }

        [Test]
        public void Can_AddComponentInstance() {
            if(IoC.IsNotInitialized)
                IoC.InitializeWithEmptyContainer();

            var gzip = new GZipCompressor();

            IoC.AddComponentInstance(gzip);

            var compressor = IoC.Resolve<GZipCompressor>();
            Assert.IsNotNull(compressor);
            Assert.IsInstanceOf<GZipCompressor>(compressor);

            IoC.Reset();
        }

        [Test]
        public void Can_AddComponentInstance_With_ServiceType_And_ComponentType() {
            if(IoC.IsNotInitialized)
                IoC.InitializeWithEmptyContainer();

            var gzip = new GZipCompressor();

            IoC.AddComponentInstance<GZipCompressor>(typeof(ICompressor), gzip);

            var compressor = IoC.Resolve<ICompressor>();
            Assert.IsNotNull(compressor);
            Assert.IsInstanceOf<GZipCompressor>(compressor);

            IoC.Reset();
        }

        [Test]
        public void Can_AddComponentInstance_With_ServiceType() {
            if(IoC.IsNotInitialized)
                IoC.InitializeWithEmptyContainer();

            var gzip = new GZipCompressor();

            IoC.AddComponentInstance<ICompressor>(gzip);

            var compressor = IoC.Resolve<ICompressor>();
            Assert.IsNotNull(compressor);
            Assert.IsInstanceOf<GZipCompressor>(compressor);

            IoC.Reset();
        }
    }
}