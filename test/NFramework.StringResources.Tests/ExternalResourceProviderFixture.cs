using System.Web.Compilation;
using NSoft.NFramework.InversionOfControl;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework.StringResources {
    [TestFixture]
    public class ExternalResourceProviderFixture {
        private DefaultResourceProviderFactory _factory;
        private IResourceProvider _provider;

        [TestFixtureSetUp]
        public void ChangeGlobalProvider() {
            IoC.Reset();
            IoC.Initialize();

            _factory = IoC.Resolve<IResourceProviderFactory>() as DefaultResourceProviderFactory;

            Assert.IsNotNull(_factory);

            _factory.GlobalResourceProviderName = "ExternalResourceProvider";
            _provider = _factory.CreateGlobalResourceProvider("NSoft.NFramework.StringResources.Sample.ExtResources|Glossary");
            Assert.IsNotNull(_provider);
        }

        [Test]
        [Culture("ko-KR,en-CA,en-US")]
        public void ExternalResourceTest() {
            var provider = _factory.CreateGlobalResourceProvider("NSoft.NFramework.StringResources.Sample.ExtResources|Glossary");

            var homePage = (string)provider.GetObject("HomePage", null);
            Assert.IsNotEmpty(homePage);
            Assert.IsTrue(homePage.Contains("external"));
        }

        [Test]
        [Culture("ko-KR,en-CA,en-US")]
        public void LoadParameterizedValue() {
            // _provider = _factory.CreateGlobalResourceProvider("Sample.ExtResources|Glossary");

            var homePage = (string)_provider.GetObject("HomePage", null);
            Assert.IsNotEmpty(homePage);

            var welcome = (string)_provider.GetObject("Welcome", null);
            Assert.IsNotEmpty(welcome);

            Assert.IsTrue(welcome.Contains(homePage));
        }

        [Test]
        [Culture("ko-KR,en-CA,en-US")]
        public void LoadParameterizedValueInOtherResourceName() {
            var glossaryProvider = _factory.CreateGlobalResourceProvider("Glossary");

            var homePage = (string)glossaryProvider.GetObject("HomePage", null);
            Assert.IsNotEmpty(homePage);

            var commonTermsProvider = _factory.CreateGlobalResourceProvider("CommonTerms");
            var hello = (string)commonTermsProvider.GetObject("Hello", null);

            Assert.IsNotEmpty(hello);
            Assert.IsTrue(hello.Contains(homePage), "hello={0}, homePage={1}", hello, homePage);
        }

        [Test]
        public void Thread_Test() {
            TestTool.RunTasks(3, () => {
                                     ExternalResourceTest();
                                     LoadParameterizedValue();
                                     LoadParameterizedValueInOtherResourceName();
                                 });
        }
    }
}