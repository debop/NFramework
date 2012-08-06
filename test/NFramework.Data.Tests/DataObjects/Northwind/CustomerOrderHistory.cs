using System;

namespace NSoft.NFramework.Data.DataObjects.Northwind {
    [Serializable]
    internal class CustomerOrderHistory : DataObjectBase {

        public CustomerOrderHistory() {
            ProductName = string.Empty;
            Total = 0;
        }

        public CustomerOrderHistory(string productName, int totalQuantity) {
            ProductName = productName;
            Total = totalQuantity;
        }

        public virtual string ProductName { get; set; }

        public virtual float? Total { get; set; }

        public override int GetHashCode() {
            return HashTool.Compute(ProductName, Total);
        }

        public override string ToString() {
            return string.Format("CustomerOrderHistory# ProductName=[{0}], Total=[{1}]", ProductName, Total);
        }
    }
}