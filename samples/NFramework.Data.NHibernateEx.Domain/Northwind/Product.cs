using System;
using Iesi.Collections.Generic;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Northwind {
    [Serializable]
    public class Product : DataEntityBase<int> {
        protected Product() {}

        public Product(Category category, string name) {
            category.ShouldNotBeNull("category");
            name.ShouldNotBeEmpty("name");

            Category = category;
            Name = name;
        }

        public virtual Category Category { get; protected set; }
        public virtual string Name { get; protected set; }

        public virtual Supplier SuppliedBy { get; set; }

        private Iesi.Collections.Generic.ISet<OrderDetail> _orderDetails;

        public virtual Iesi.Collections.Generic.ISet<OrderDetail> OrderDetails {
            get { return _orderDetails ?? (_orderDetails = new HashedSet<OrderDetail>()); }
            protected set { _orderDetails = value; }
        }

        public virtual string QuantityPerUnit { get; set; }
        public virtual decimal? UnitPrice { get; set; }
        public virtual short? UnitsInStock { get; set; }
        public virtual short? UnitsOnOrder { get; set; }
        public virtual short? ReorderLevel { get; set; }
        public virtual bool? Discontinued { get; set; }

        public override int GetHashCode() {
            return IsSaved ? base.GetHashCode() : HashTool.Compute(Name, Category);
        }
    }

    [Serializable]
    public class ProductSet : HashedSet<Product> {}
}