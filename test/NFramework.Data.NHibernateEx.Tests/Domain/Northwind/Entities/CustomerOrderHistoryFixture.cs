using System.Collections.Generic;
using NHibernate.Transform;
using NHibernate.Type;
using NSoft.NFramework.Data.NHibernateEx;
using NSoft.NFramework.Data.NHibernateEx.Domain.Northwind;
using NUnit.Framework;

namespace NSoft.NFramework.Data.Tests.Domain.Northwind {
    [TestFixture]
    public class CustomerOrderHistoryFixture : NorthwindDbTestFixtureBase {
        private static IList<CustomerOrderHistory> GetCustomerOrderHistory(string customerId, IResultTransformer transformer) {
            var query =
                UnitOfWork.CurrentSession.GetNamedQuery("NSoft.NFramework.Data.NHibernateEx.Domain.Northwind.GetCustomerOrderHistory",
                                                        new NHParameter("CustomerId", customerId, TypeFactory.GetStringType(255)));

            query.SetResultTransformer(transformer);
            return query.List<CustomerOrderHistory>();
        }

        private static IList<CustomerOrderHistory> GetCustomerOrderHistoryByDynamicResultTransformer(string customerId) {
            return GetCustomerOrderHistory(customerId,
                                           new DynamicResultTransformer<CustomerOrderHistory>());
        }

        private static IList<CustomerOrderHistory> GetCustomerOrderHistoryByTypedResultTransformer(string customerId) {
            return GetCustomerOrderHistory(customerId,
                                           new TypedResultTransformer<CustomerOrderHistory>());
        }

        [Test]
        public void TypedResultTransformerTest() {
            var historyList = GetCustomerOrderHistoryByTypedResultTransformer(TestCustomer.Id);
            Assert.Greater(historyList.Count, 0);
        }

        [Test]
        public void DynamicResultTransformerTest() {
            var historyList = GetCustomerOrderHistoryByDynamicResultTransformer(TestCustomer.Id);
            Assert.Greater(historyList.Count, 0);
        }
    }
}