using System;
using Castle.Windsor;
using Castle.Windsor.Installer;
using NSoft.NFramework.Compressions;
using NSoft.NFramework.LinqEx;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.InversionOfControl {
    [Microsoft.Silverlight.Testing.Tag("IoC")]
    [TestFixture]
    public class WindsorInstallerFixture {
        private IWindsorContainer _container;

        [SetUp]
        public void SetUp() {
            _container = new WindsorContainer();
        }

        [TearDown]
        public void TearDown() {
            if(_container != null)
                _container.Dispose();

            _container = null;
        }

        [Test]
        public void RegisterByInstaller() {
            // FromAssembly.This() : 현 Assembly 의 모든 IWindsorInstaller 로 부터
            // FromAssembly.Containing<CompressorInstaller>() : CompressorInstaller 가 정의된 Assembly의 모든 IWindsorInstaller 로 부터
            // FromAssembly.Named("NSoft.NFramework") : 지정된 Assembly명을 가진 Assembly에 있는 모든 IWindsorInstaller 로부터 //! Silverlight에서는 안된다.

            _container.Install(FromAssembly.This(),
                               FromAssembly.Containing<ICompressor>());


            _container.Resolve<ICompressor>().Should().Not.Be.Null();
            var compressors = _container.ResolveAll<ICompressor>();
            compressors.Length.Should().Be.GreaterThan(0);
            compressors.RunEach(c => Console.WriteLine(c.GetType().FullName));

            _container.Resolve<ISerializer<UserInfo>>().Should().Not.Be.Null();

            var serializers = _container.ResolveAll<ISerializer<UserInfo>>();
            serializers.Length.Should().Be.GreaterThan(0);
            serializers.RunEach(c => Console.WriteLine(c.GetType().FullName));
        }
    }
}