using NHibernate.Criterion;
using NSoft.NFramework.Data.NHibernateEx;
using NSoft.NFramework.Data.NHibernateEx.Domain.Northwind;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework.Data.Tests.Domain.Northwind {
    [TestFixture]
    public class OrderDetailFixture : NorthwindDbTestFixtureBase {
        [Test]
        public void FindAll() {
            var details = Repository<OrderDetail>.FindAll();
            Assert.AreEqual(details.Count, Repository<OrderDetail>.Count());
        }

        [Test]
        public void FindAll_With_Paging() {
            var details = Repository<OrderDetail>.GetPage(1, 10, DetachedCriteria.For<OrderDetail>());
            Assert.GreaterOrEqual(details.TotalItemCount, 10, "TotalItemCount=" + details.TotalItemCount);
        }

        [Test]
        public void MultiTesting() {
            TestTool.RunTasks(3, () => {
                                     using(UnitOfWork.Start(UnitOfWorkNestingOptions.CreateNewOrNestUnitOfWork)) {
                                         FindAll();
                                         FindAll_With_Paging();
                                     }
                                 });
        }
    }
}