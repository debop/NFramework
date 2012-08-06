using System;
using NSoft.NFramework.Data;

namespace NSoft.NFramework.DataServices {
    /// <summary>
    /// NameMapping 시에 TrimNameMapper를 이용해야 한다.
    /// </summary>
    [Serializable]
    public class OrderDetail : DataObjectBase {
        public virtual int OrderID { get; set; }
        public virtual int ProductID { get; set; }
        public virtual decimal? UnitPrice { get; set; }
        public virtual int? Quantity { get; set; }
        public virtual float? Discount { get; set; }

        public override int GetHashCode() {
            return HashTool.Compute(OrderID, ProductID);
        }
    }
}