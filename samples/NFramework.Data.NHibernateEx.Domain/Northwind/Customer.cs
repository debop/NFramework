using System;
using System.Collections.Generic;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Northwind {
    [Serializable]
    public class Customer : AssignedIdEntityBase<string> {
        protected Customer() {}
        public Customer(string customerId) : this(customerId, null) {}

        public Customer(string customerId, string companyName) {
            customerId.ShouldNotBeEmpty("customerId");
            SetIdentity(customerId);
            CompanyName = companyName;
        }

        public virtual string CompanyName { get; set; }
        public virtual string ContactName { get; set; }

        private IList<Order> _orders;

        public virtual IList<Order> Orders {
            get { return _orders ?? (_orders = new List<Order>()); }
            protected set { _orders = value; }
        }

        // 이 함수는 Northwind.DataService 로 뺀다.
        /// <summary>
        /// 현 주문자가 지정된 날짜의 주문정보
        /// </summary>
        public virtual IList<Order> GetOrdersOrderedOn(string factoryName, DateTime orderedDate) {
            // ICriteria 사용
            //ICriteria criteria = NHUtils.CreateCriteria<Order>(factoryName);

            //criteria.Add(Expression.Eq("OrderDate", orderedDate));
            //criteria.Add(Expression.Eq("OrderedBy", this));

            //return criteria.List<Order>();

            // DetachedCriteria 사용
            //DetachedCriteria detachedCriteria = DetachedCriteria.For<Order>();
            //detachedCriteria.Add(Expression.Eq("OrderDate", orderedDate));
            //detachedCriteria.Add(Expression.Eq("OrderedBy", this));

            //return NHRepositoryUtils<Order>.Get(factoryName, detachedCriteria);

            return new List<Order>();
        }

        public override int GetHashCode() {
            return IsSaved ? base.GetHashCode() : HashTool.Compute(Id);
        }
    }
}