using System;
using System.Collections.Generic;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Northwind {
    public class CustomerRepository : NHRepository<Customer> {
        public DateTime? SearchOrderDate { get; set; }

        public IList<Order> GetOrdersOrderedOn(Customer customer, DateTime? orderedDate) {
            var criteria =
                NHibernate.Criterion.DetachedCriteria.For<Order>()
                    .Add(NHibernate.Criterion.Restrictions.Eq("OrderedBy", customer))
                    .Add(NHibernate.Criterion.Restrictions.Eq("OrderDate",
                                                              orderedDate.HasValue ? orderedDate.Value : SearchOrderDate));

            return Repository<Order>.FindAll(criteria);
        }
    }
}