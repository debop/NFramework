using NSoft.NFramework.DataServices.Adapters;
using NSoft.NFramework.InversionOfControl;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.DataServices {
    [TestFixture]
    public class IoCSetupFixture : AbstractDataServiceFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        [TestCase("DataService.Northwind")]
        [TestCase("DataService.Pubs")]
        public void ResolveDataServiceTest(string serviceComponentId) {
            var dataService = IoC.Resolve<IDataService>(serviceComponentId);
            dataService.Should().Not.Be.Null();

            dataService.AdoRepository.Should().Not.Be.Null();
        }

        [Test]
        public void ResolveStaticDataServiceTest() {
            if(IsDebugEnabled)
                log.Debug("DataService Static Class에 대한 테스트");

            DataService.AdoRepository.Should().Not.Be.Null();
            DataService.AdoRepository.QueryProvider.Should().Not.Be.Null();
        }

        [TestCase("DataServiceAdapter.Northwind")]
        [TestCase("DataServiceAdapter.Pubs")]
        public void ResolveDataServiceAdapter(string adapterComponentId) {
            var adapter = IoC.Resolve<IDataServiceAdapter>(adapterComponentId);

            adapter.Should().Not.Be.Null();
            adapter.DataService.Should().Not.Be.Null();
            adapter.DataService.AdoRepository.Should().Not.Be.Null();

            adapter.RequestSerializer.Should().Not.Be.Null();
            adapter.ResponseSerializer.Should().Not.Be.Null();
        }
    }
}