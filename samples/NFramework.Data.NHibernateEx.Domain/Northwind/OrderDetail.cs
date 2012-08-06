using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Northwind {

    #region << OrderDetail을 Composite-Id key-many-to-one 으로 구현시>>

    #endregion

    [Serializable]
    public class OrderDetail : StateEntityBase {
        protected OrderDetail() {}

        public OrderDetail(Order order, Product product) {
            order.ShouldNotBeNull("order");
            product.ShouldNotBeNull("product");

            Order = order;
            Product = product;
        }

        public virtual Order Order { get; protected set; }
        public virtual Product Product { get; protected set; }
        public virtual decimal? UnitPrice { get; set; }
        public virtual int? Quantity { get; set; }
        public virtual float? Discount { get; set; }

        public virtual decimal TotalAmount {
            get { return (Quantity.GetValueOrDefault(0) * UnitPrice.GetValueOrDefault(0)) * (1 - (decimal)Discount.GetValueOrDefault(0)); }
        }

        public override int GetHashCode() {
            return IsSaved ? base.GetHashCode() : HashTool.Compute(Order, Product);
        }
    }

    #region << OrderDatilIdentity 로 구현 시 (Composite-Id에서 key-property 사용)>>

    /*
	[Serializable]
	public class OrderDetail : DataEntityBase<OrderDetailIdentity>
	{
		public OrderDetail() { }
		public OrderDetail(OrderDetailIdentity id)
		{
			this.Id = id;
		}

		#region << Properties >>

		public Order Order { get; set; }
		public Product Product { get; set; }
		public decimal UnitPrice { get; set; }
		public short Quantity { get; set; }
		public float Discount { get; set; }
		public decimal TotalAmount
		{
			get
			{
				return ((decimal)Quantity * UnitPrice) * (1.0 - Discount);
			}
		}

		#endregion

		#region << Overrides >>

		public override int GetHashCode()
		{
			if (IsSaved)
				return base.GetHashCode();
			else
			{
				int hash = (Order != null) ? Order.GetHashCode() : 0;
				hash ^= 27 ^ (Product != null) ? Product.GetHashCode() : 0;
				hash ^= 27 ^ TotalAmount.GetHashCode();

				return hash;
			}
		}
		#endregion
	}

	[Serializable]
	public class OrderDetailIdentity
	{
		public OrderDetailIdentity() { }
		public OrderDetailIdentity(int orderId, int productId)
		{
			OrderId = orderId;
			ProductId = productId;
		}

		public virtual int OrderId { get; set; }
		public virtual int ProductId { get; set; }

		public override string ToString()
		{
			return string.Format("{0}|{1}", OrderId, ProductId);
		}
		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}
		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is OrderDetailIdentity))
				return false;

			return GetHashCode().Equals(obj.GetHashCode());
		}
	}
*/

    #endregion
}