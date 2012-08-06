using System;
using System.Collections.Generic;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Northwind {
    [Serializable]
    public class Category : DataEntityBase<Int32> {
        protected Category() {}

        public Category(string name) {
            name.ShouldNotBeEmpty("name");

            Name = name;
        }

        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual byte[] Picture { get; set; }

        private IList<Product> _products;

        public virtual IList<Product> Products {
            get { return _products ?? (_products = new List<Product>()); }
            protected set { _products = value; }
        }

        public override int GetHashCode() {
            return IsSaved ? base.GetHashCode() : HashTool.Compute(Name);
        }
    }
}