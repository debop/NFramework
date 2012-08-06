using System;
using System.Collections.Generic;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Loquacious.SimpleMappings {
    [Serializable]
    public class CStore : DataEntityBase<Int32> {
        protected CStore() {}

        public CStore(string name) {
            name.ShouldNotBeWhiteSpace("name");
            Name = name;
        }

        public virtual string Name { get; set; }

        private IList<CEmployee> _staff;

        public virtual IList<CEmployee> Staff {
            get { return _staff ?? (_staff = new List<CEmployee>()); }
            protected set { _staff = value; }
        }

        private IList<CProduct> _products;

        public virtual IList<CProduct> Products {
            get { return _products ?? (_products = new List<CProduct>()); }
            protected set { _products = value; }
        }

        public virtual void AddProduct(CProduct CProduct) {
            CProduct.StoresStockedIn.Add(this);
            Products.Add(CProduct);
        }

        public virtual void AddEmployee(CEmployee employee) {
            employee.Store = this;
            Staff.Add(employee);
        }

        public override int GetHashCode() {
            if(IsSaved)
                return base.GetHashCode();

            return HashTool.Compute(Name);
        }
    }
}