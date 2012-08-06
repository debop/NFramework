using System;
using System.Collections.Generic;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    public class FluentCustomer : DataEntityBase<Int32> {
        private IList<FluentProduct> _products;
        private Address _workAddress;
        private Address _homeAddress;

        public virtual string Name { get; set; }

        public virtual string Credit { get; set; }

        public virtual IList<FluentProduct> Products {
            get { return _products ?? (_products = new List<FluentProduct>()); }
            set { _products = value; }
        }

        public virtual Address WorkAddress {
            get { return _workAddress ?? (_workAddress = new Address()); }
            protected set { _workAddress = value; }
        }

        public virtual Address HomeAddress {
            get { return _homeAddress ?? (_homeAddress = new Address()); }
            protected set { _homeAddress = value; }
        }

        public override int GetHashCode() {
            if(IsSaved)
                return base.GetHashCode();

            return HashTool.Compute(Name, Credit);
        }
    }
}