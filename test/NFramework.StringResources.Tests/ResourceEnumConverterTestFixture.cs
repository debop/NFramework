using System;
using System.Web.Compilation;
using NSoft.NFramework.InversionOfControl;
using NUnit.Framework;

namespace NSoft.NFramework.StringResources {
    public enum Size {
        Small,
        Medium,
        Large
    }

    [TestFixture]
    public class ResourceEnumConverterTestFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        private DefaultResourceProviderFactory _factory;
        private IResourceProvider _provider;

        static ResourceEnumConverterTestFixture() {
            IoC.Initialize();
        }

        [TestFixtureSetUp]
        public void ClassSetUp() {
            _factory = IoC.Resolve<IResourceProviderFactory>() as DefaultResourceProviderFactory;

            Assert.IsNotNull(_factory);

            _factory.GlobalResourceProviderName = "FileResourceProvider";
            _provider = _factory.CreateGlobalResourceProvider("StringResources|Enums");
            Assert.IsNotNull(_provider);
        }

        [Test]
        [Culture("ko-KR,en-CA,en-US")]
        public void EnumLocalization() {
            foreach(var value in Enum.GetValues(typeof(Size))) {
                string valueName = value.ToString();
                var valueText = _provider.GetObject("Size_" + valueName, null);
                Console.WriteLine("Size_{0}={1}", valueName, valueText ?? valueName);
            }
        }
    }
}