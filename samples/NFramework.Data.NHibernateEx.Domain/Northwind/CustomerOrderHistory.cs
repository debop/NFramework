using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Northwind {
    [Serializable]
    public class CustomerOrderHistory : DataObjectBase {
        private int? _hashCode;

        public CustomerOrderHistory() : this(string.Empty, 0) {}

        public CustomerOrderHistory(string productName, int totalQuantity) {
            ProductName = productName;
            Total = totalQuantity;
        }

        public virtual string ProductName { get; protected set; }

        public virtual int? Total { get; protected set; }

        public override int GetHashCode() {
            return (_hashCode ?? (_hashCode = HashTool.Compute(ProductName, Total))).Value;
        }

        public override string ToString() {
            return string.Format("CustomerOrderHistory# Product=[{0}], Total=[{1}]", ProductName, Total);
        }
    }
}