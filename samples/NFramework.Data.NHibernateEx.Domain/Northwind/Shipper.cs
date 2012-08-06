using System;
using System.Collections.Generic;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Northwind {
    [Serializable]
    public class Shipper : DataEntityBase<int> {
        protected Shipper() {}

        public Shipper(string companyName) {
            companyName.ShouldNotBeNull("companyName");
            CompanyName = companyName;
        }

        public virtual string CompanyName { get; set; }
        public virtual string Phone { get; set; }

        private IList<Order> _orders;

        public virtual IList<Order> Orders {
            get { return _orders ?? (_orders = new List<Order>()); }
            protected set { _orders = value; }
        }

        public override int GetHashCode() {
            return IsSaved ? base.GetHashCode() : HashTool.Compute(CompanyName);
        }
    }
}