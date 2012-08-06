using NUnit.Framework;

namespace NSoft.NFramework.DataServices.Adapters {
    [TestFixture]
    public class IoCDataServiceAdapterFixture : AbstractDataServiceAdapterFixture {
        public override IDataServiceAdapter GetDataServiceAdapter() {
            return DataServiceTool.ResolveDataServiceAdapter(ProductName);
        }
    }
}