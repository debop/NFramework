using System.Web.Compilation;
using NSoft.NFramework.Data.NHibernateEx;
using NSoft.NFramework.InversionOfControl;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework.StringResources.NHibernate {
    /// <summary>
    /// NHibernate용 ResourceProvider를 사용하기 위해서는 
    /// 1. DB 생성 (NFramework.StringResources.NHibernate project에 Resource.sql 을 대상 DB에 실행시킨다. 
    /// 2. NHibernate 환경설정을 변경한다. (/NHibernateConfigs/hibernate.resources.cfg.xml)
    /// 3. Castle.Windsor 환경설정을 변경한다. ( Windsor.ResourceProvider.config, Windsor.NHibernate.config) 
    /// </summary>
    [TestFixture]
    public class NHResourceProviderFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        private DefaultResourceProviderFactory _factory;
        private IResourceProvider _provider;

        [TestFixtureSetUp]
        public void ClassSetUp() {
            if(IoC.IsNotInitialized)
                IoC.Initialize();

            _factory = IoC.Resolve<IResourceProviderFactory>() as DefaultResourceProviderFactory;
            Assert.IsNotNull(_factory);

            _factory.GlobalResourceProviderName = "NHResourceProvider";

            if(UnitOfWork.IsStarted == false)
                UnitOfWork.Start();
        }

        [TestFixtureTearDown]
        public void ClassCleanUp() {
            IoC.Reset();
        }

        [Test]
        public void LoadComponent() {
            _provider = _factory.CreateGlobalResourceProvider("Glossary");
            Assert.IsNotNull(_provider);
        }

        [Test]
        [Culture("ko-KR,en-CA,en-US")]
        public void LoadLocalizedValues() {
            var provider = _factory.CreateGlobalResourceProvider("Glossary");
            Assert.IsNotNull(provider);

            Assert.IsNotEmpty((string)provider.GetObject("HomePage", null));
            Assert.IsNotEmpty((string)provider.GetObject("SiteMap", null));
            Assert.IsNotEmpty((string)provider.GetObject("Welcome", null));

            Assert.IsNull(provider.GetObject("NotExist", null));
        }

        [Test]
        [Culture("ko-KR,en-CA,en-US")]
        public void LoadParameterizedValues() {
            var provider = _factory.CreateGlobalResourceProvider("Glossary");
            Assert.IsNotNull(provider);

            var homePage = (string)provider.GetObject("HomePage", null);
            Assert.IsNotEmpty(homePage);

            var welcome = (string)provider.GetObject("Welcome", null);
            Assert.IsNotEmpty(welcome);

            Assert.IsTrue(welcome.Contains(homePage), "welcome=" + welcome);
        }

        [Test]
        [Culture("ko-KR,en-CA,en-US")]
        public void LoadParameterizedValuesInOtherResourceName() {
            var glossaryProvider = _factory.CreateGlobalResourceProvider("Glossary");
            Assert.IsNotNull(glossaryProvider);

            var homePage = (string)glossaryProvider.GetObject("HomePage", null);
            Assert.IsNotEmpty(homePage);

            var commonTermsProvider = _factory.CreateGlobalResourceProvider("CommonTerms");
            Assert.IsNotNull(commonTermsProvider);

            var hello = (string)commonTermsProvider.GetObject("Hello", null);
            Assert.IsNotEmpty(hello);

            Assert.IsTrue(hello.Contains(homePage), "hello={0}, homePage={1}", hello, homePage);
        }

        [Test]
        public void Thread_Test() {
            TestTool.RunTasks(3, () => {
                                     LoadComponent();
                                     LoadLocalizedValues();
                                     LoadParameterizedValues();
                                     LoadParameterizedValuesInOtherResourceName();
                                 });
        }
    }
}