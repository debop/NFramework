using NSoft.NFramework.Data.NHibernateEx;
using NSoft.NFramework.Data.NHibernateEx.Domain.Northwind;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework.Data.Tests.Domain.Northwind {
    [TestFixture]
    public class IniNHQueryStringProviderFixture : NorthwindDbTestFixtureBase {
        [Test]
        public void WasSettedQueryStringProvider() {
            Assert.IsNotNull(UnitOfWork.QueryProvider);

            var queryStringTable = UnitOfWork.QueryProvider.GetQueries();

            Assert.IsNotNull(queryStringTable);
            Assert.Greater(queryStringTable.Count, 0);
            // Console.WriteLine(queryStringTable.ToString(true));
        }

        [Test]
        public void GetQuery() {
            Assert.IsNotNull(UnitOfWork.QueryProvider);

            var orders = Repository<Order>.FindAllByHql(UnitOfWork.QueryProvider.GetQuery("Order, GetAll"));

            Assert.AreEqual(orders.Count, Repository<Order>.Count());
            Print(orders);
        }

        [Test]
        public void GetQuery_With_Parameters() {
            Assert.IsNotNull(UnitOfWork.QueryProvider);

            var orders
                = Repository<Order>.FindAllByHql(UnitOfWork.QueryProvider.GetQuery("Order, GetByCustomer"),
                                                 base.CustomerParameter);

            Assert.Greater(orders.Count, 1);
            Print(orders);
        }

        [Test]
        public void MultiThread() {
            TestTool.RunTasks(4,
                              () => {
                                  using(UnitOfWork.Start(UnitOfWorkNestingOptions.CreateNewOrNestUnitOfWork)) {
                                      WasSettedQueryStringProvider();
                                      GetQuery();
                                      GetQuery_With_Parameters();
                                  }
                              });
        }
    }
}