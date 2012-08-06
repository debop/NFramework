using NSoft.NFramework.InversionOfControl;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework.Data.Ado.Repositories {
    /// <summary>
    /// 두개 이상의 AdoRepository와 QueryStringFile 에 대해서
    /// </summary>
    [TestFixture]
    public class MultiAdoRepositoryFixture : IoCSetUpBase {
        private IAdoRepository _northwindAdo;
        private IAdoRepository _pubsAdo;

        public IAdoRepository NorthwindAdo {
            get { return _northwindAdo ?? (_northwindAdo = IoC.Resolve<IAdoRepository>("NorthwindAdoRepository.SqlServer")); }
        }

        public IAdoRepository PubsAdo {
            get { return _pubsAdo ?? (_pubsAdo = IoC.Resolve<IAdoRepository>("PubsAdoRepository.SqlServer")); }
        }

        [Test]
        public void Can_Resolve_AdoRepositoryById() {
            Assert.IsNotNull(NorthwindAdo);
            Assert.AreEqual(AdoTool.DefaultDatabaseName, NorthwindAdo.DbName);
        }

        [Test]
        public void Can_Load_QueryString_By_QueryStringProvider() {
            Assert.IsNotNull(NorthwindAdo);

            Assert.IsNotNull(NorthwindAdo.QueryProvider);
            var getAllCustomers = NorthwindAdo.QueryProvider.GetQuery("Customer", "GetAll");
            Assert.IsNotNull(getAllCustomers);
            Assert.AreEqual("SELECT * FROM Customers with (nolock)", getAllCustomers);
            Assert.IsTrue(NorthwindAdo.QueryProvider.QueryFilePath.Contains("Northwind"));


            Assert.IsNotNull(PubsAdo);
            Assert.IsNotNull(PubsAdo.QueryProvider);

            var getAllAuthors = PubsAdo.QueryProvider.GetQuery("Authors", "GetAll");
            Assert.IsNotNull("getAllAuthors");
            Assert.AreEqual("SELECT * FROM Authors", getAllAuthors);
            Assert.IsTrue(PubsAdo.QueryProvider.QueryFilePath.Contains("Pubs"));
        }

        [Test]
        public void Thread_Test() {
            TestTool.RunTasks(4,
                              Can_Resolve_AdoRepositoryById,
                              Can_Load_QueryString_By_QueryStringProvider);
        }
    }
}