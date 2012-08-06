using System;
using System.Collections.Generic;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    [Serializable]
    public class Store : DataEntityBase<Int32> {
        protected Store() {}

        public Store(string name) {
            name.ShouldNotBeWhiteSpace("name");
            Name = name;
        }

        public virtual string Name { get; set; }

        private IList<NHEmployee> _staff;

        public virtual IList<NHEmployee> Staff {
            get { return _staff ?? (_staff = new List<NHEmployee>()); }
        }

        private IList<FluentProduct> _products;

        public virtual IList<FluentProduct> Products {
            get { return _products ?? (_products = new List<FluentProduct>()); }
        }

        public virtual void AddProduct(FluentProduct fluentProduct) {
            fluentProduct.StoresStockedIn.Add(this);
            Products.Add(fluentProduct);
        }

        public virtual void AddEmployee(NHEmployee nhEmployee) {
            nhEmployee.Store = this;
            Staff.Add(nhEmployee);
        }

        public override int GetHashCode() {
            if(IsSaved)
                return base.GetHashCode();

            return HashTool.Compute(Name);
        }
    }
}