using System.Web.Compilation;
using NSoft.NFramework.InversionOfControl;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework.StringResources {
    [TestFixture]
    public class FileResourceProviderFixture {
        private DefaultResourceProviderFactory _factory;
        private IResourceProvider _provider;

        [TestFixtureSetUp]
        public void ChangeGlobalProvider() {
            IoC.Initialize();

            _factory = IoC.Resolve<IResourceProviderFactory>() as DefaultResourceProviderFactory;

            Assert.IsNotNull(_factory);

            _factory.GlobalResourceProviderName = "FileResourceProvider";
            _provider = _factory.CreateGlobalResourceProvider("StringResources|Glossary");
            Assert.IsNotNull(_provider);
        }

        [Test]
        [Culture("ko-KR,en-CA,en-US")]
        public void FileResourceTest() {
            var glossaryProvider = _factory.CreateGlobalResourceProvider("Glossary");
            Assert.IsNotNull(glossaryProvider);

            var homepage = (string)glossaryProvider.GetObject("HomePage", null);
            Assert.IsNotEmpty(homepage);
        }

        [Test]
        [Culture("ko-KR,en-CA,en-US")]
        public void NotExistKey_Should_Return_Null() {
            var glossaryProvider = _factory.CreateGlobalResourceProvider("Glossary");
            Assert.IsNotNull(glossaryProvider);

            Assert.IsNull(glossaryProvider.GetObject("NotExist_Key", null));
        }

        [Test]
        [Culture("ko-KR,en-CA,en-US")]
        public void LoadParameterizedValue() {
            var glossaryProvider = _factory.CreateGlobalResourceProvider("Glossary");
            Assert.IsNotNull(glossaryProvider);

            var homepage = (string)glossaryProvider.GetObject("HomePage", null);
            Assert.IsNotEmpty(homepage);

            var welcome = (string)glossaryProvider.GetObject("Welcome", null);
            Assert.IsNotEmpty(welcome);

            Assert.IsTrue(welcome.Contains(homepage), "welcome=" + welcome);
        }

        [Test]
        [Culture("ko-KR,en-CA,en-US")]
        public void LoadParameterizedValueInOtherResourceName() {
            var glossaryProvider = _factory.CreateGlobalResourceProvider("Glossary");
            Assert.IsNotNull(glossaryProvider);

            var homepage = (string)glossaryProvider.GetObject("HomePage", null);
            Assert.IsNotEmpty(homepage);

            var commoneTermsProvider = _factory.CreateGlobalResourceProvider("CommonTerms");
            Assert.IsNotNull(commoneTermsProvider);

            var hello = (string)commoneTermsProvider.GetObject("Hello", null);
            Assert.IsNotEmpty(hello);

            Assert.IsTrue(hello.Contains(homepage), "hello=" + hello);
        }

        [Test]
        public void Thread_Test() {
            TestTool.RunTasks(3, () => {
                                     FileResourceTest();
                                     NotExistKey_Should_Return_Null();
                                     LoadParameterizedValue();
                                     LoadParameterizedValueInOtherResourceName();
                                 });
        }
    }
}