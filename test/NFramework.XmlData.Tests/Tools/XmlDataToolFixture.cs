using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.XmlData.Tools {
    [TestFixture]
    public class XmlDataToolFixture : AbstractXmlDataFixture {
        [Test]
        public void ResolveXmlDataManagerAdapterTest() {
            var adapter = XmlDataTool.ResolveXmlDataManagerAdapter();
            adapter.Should().Not.Be.Null();
            adapter.XmlDataManager.Should().Not.Be.Null();
            adapter.XmlDataManager.Ado.Should().Not.Be.Null();
            adapter.XmlDataManager.Ado.QueryProvider.Should().Not.Be.Null();

            adapter.RequestSerializer.Should().Not.Be.Null();
            adapter.ResponseSerializer.Should().Not.Be.Null();
        }

        [Test]
        public void ResolveXmlDataManagerTest() {
            var manager = XmlDataTool.ResolveXmlDataManager();
            manager.Should().Not.Be.Null();
            manager.Ado.Should().Not.Be.Null();
            manager.Ado.QueryProvider.Should().Not.Be.Null();
        }
    }
}