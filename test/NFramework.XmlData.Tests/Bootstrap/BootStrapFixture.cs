using NSoft.NFramework.InversionOfControl;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.XmlData.Bootstrap {
    [TestFixture]
    public class BootStrapFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        [TestFixtureSetUp]
        public void FixtureSetUp() {
            if(IoC.IsNotInitialized)
                IoC.Initialize();
        }

        [Test]
        public void ResolveXmlDataManager() {
            var xmlDataManager = IoC.Resolve<IXmlDataManager>();
            xmlDataManager.Should().Not.Be.Null();
        }

        [Test]
        public void ResolveDataServiceAdapter() {
            var adapter = IoC.Resolve<IXmlDataManagerAdapter>();
            adapter.Should().Not.Be.Null();
        }
    }
}