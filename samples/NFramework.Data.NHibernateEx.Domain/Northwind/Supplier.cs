using System;
using System.Collections.Generic;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Northwind {
    [Serializable]
    public class Supplier : DataEntityBase<Int64> {
        protected Supplier() {}

        public Supplier(string companyName, string contactName) {
            companyName.ShouldNotBeWhiteSpace("companyName");

            CompanyName = companyName;
            ContactName = contactName;
        }

        public virtual string CompanyName { get; set; }
        public virtual string ContactName { get; set; }
        public virtual string ContactTitle { get; set; }

        private AddressInfo _addressInfo;

        public virtual AddressInfo AddressInfo {
            get { return _addressInfo ?? (_addressInfo = new AddressInfo()); }
            set { _addressInfo = value; }
        }

        public virtual string Phone { get; set; }
        public virtual string Fax { get; set; }
        public virtual string HomePage { get; set; }

        private IList<Product> _products;

        public virtual IList<Product> Products {
            get { return _products ?? (_products = new List<Product>()); }
            protected set { _products = value; }
        }

        public override int GetHashCode() {
            return IsSaved ? base.GetHashCode() : HashTool.Compute(CompanyName);
        }
    }
}