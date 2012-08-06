using System.Globalization;
using System.Web.Compilation;
using NSoft.NFramework.InversionOfControl;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework.StringResources {
    /// <summary>
    /// 점검 사항 
    ///		1. Parameterlized Value 값을 제대로 가져오는가?
    /// </summary>
    [TestFixture]
    public class AdoResourceProviderFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        private DefaultResourceProviderFactory _factory;
        private IResourceProvider _provider;

        static AdoResourceProviderFixture() {
            IoC.Initialize();
        }

        [TestFixtureSetUp]
        public void ClassSetUp() {
            _factory = IoC.Resolve<IResourceProviderFactory>() as DefaultResourceProviderFactory;
            Assert.IsNotNull(_factory);

            _factory.GlobalResourceProviderName = @"AdoResourceProvider";
            _provider = _factory.CreateGlobalResourceProvider("Glossary");
            Assert.IsNotNull(_provider);
        }

        [TestFixtureTearDown]
        public void ClassTearDown() {
            // IoC.Reset();
        }

        [Test]
        [Culture("ko-KR,en-CA,en-US")]
        public void LoadSimpleResource() {
            _provider = _factory.CreateGlobalResourceProvider("Glossary");

            Assert.IsNotNull(_provider);
            Assert.IsNotNull(_provider.GetObject("HomePage", null));
        }

        /// <summary>
        /// ko-KR에는 정의되어 있지 않고, ko 에만 정의도어 있을 때
        /// </summary>
        [Test]
        [Culture("ko-KR,en-CA,en-US")]
        public void LoadParentLocaleValues() {
            _provider = _factory.CreateGlobalResourceProvider("Glossary");
            Assert.IsNotNull(_provider);
            Assert.IsNotNull(_provider.GetObject("HomePage", new CultureInfo("ko-KR")));
        }

        [Test]
        [Culture("ko-KR,en-CA,en-US")]
        public void LoadParameterlizedValueInSameResourceName() {
            _provider = _factory.CreateGlobalResourceProvider("Glossary");
            Assert.IsNotNull(_provider);

            var homePage = (string)_provider.GetObject("HomePage", null);
            var welcome = (string)_provider.GetObject("Welcome", null);

            Assert.IsNotEmpty(homePage);
            Assert.IsNotEmpty(welcome);

            Assert.IsTrue(welcome.Contains(homePage));
        }

        [Test]
        [Culture("ko-KR,en-CA,en-US")]
        public void LoadParameterizedValueInOtherResourceName() {
            var glossaryProvider = _factory.CreateGlobalResourceProvider("Glossary");
            Assert.IsNotNull(glossaryProvider);

            var homePage = (string)glossaryProvider.GetObject("HomePage", null);
            Assert.IsNotEmpty(homePage);

            var commonProvider = _factory.CreateGlobalResourceProvider("CommonTerms");
            var hello = (string)commonProvider.GetObject("Hello", null);

            Assert.IsTrue(hello.Contains(homePage), @"hello={0}, homePage={1}", hello, homePage);
        }

        [Test]
        public void Thread_Test() {
            TestTool.RunTasks(3, () => {
                                     LoadSimpleResource();
                                     LoadParentLocaleValues();
                                     LoadParameterlizedValueInSameResourceName();
                                     LoadParameterizedValueInOtherResourceName();
                                 });
        }
    }
}