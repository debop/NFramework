using System;
using System.Collections.Generic;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Loquacious.SimpleMappings {
    [Serializable]
    public class CCustomer : DataEntityBase<Int32> {
        public virtual string Name { get; set; }

        public virtual string Credit { get; set; }

        private IList<CProduct> _products;

        public virtual IList<CProduct> Products {
            get { return _products ?? (_products = new List<CProduct>()); }
            protected set { _products = value; }
        }

        private CAddress _workAddress;

        public virtual CAddress WorkAddress {
            get { return _workAddress ?? (_workAddress = new CAddress()); }
            protected set { _workAddress = value; }
        }

        private CAddress _homeAddress;

        public virtual CAddress HomeAddress {
            get { return _homeAddress ?? (_homeAddress = new CAddress()); }
            protected set { _homeAddress = value; }
        }

        public override int GetHashCode() {
            if(IsSaved)
                return base.GetHashCode();

            return HashTool.Compute(Name, Credit);
        }
    }
}