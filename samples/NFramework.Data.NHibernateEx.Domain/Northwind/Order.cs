using System;
using Iesi.Collections.Generic;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Northwind {
    [Serializable]
    public class Order : DataEntityBase<int> {
        protected Order() {}

        public Order(Customer orderedBy) {
            orderedBy.ShouldNotBeNull("orderedBy");
            OrderedBy = orderedBy;
        }

        public virtual Employee Seller { get; set; }
        public virtual Customer OrderedBy { get; set; }
        public virtual Shipper ShipVia { get; set; }

        public virtual DateTime? OrderDate { get; set; }
        public virtual DateTime? RequiredDate { get; set; }
        public virtual DateTime? ShippedDate { get; set; }

        public virtual string ShipName { get; set; }

        private Iesi.Collections.Generic.ISet<OrderDetail> _orderDetails;

        public virtual Iesi.Collections.Generic.ISet<OrderDetail> OrderDetails {
            get { return _orderDetails ?? (_orderDetails = new HashedSet<OrderDetail>()); }
            protected set { _orderDetails = value; }
        }

        public override int GetHashCode() {
            return IsSaved ? base.GetHashCode() : HashTool.Compute(Seller, OrderedBy, OrderDate);
        }
    }
}