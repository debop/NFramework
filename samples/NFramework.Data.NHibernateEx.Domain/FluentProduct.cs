using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    [Serializable]
    public class FluentProduct : DataEntityBase<Int32> {
        protected FluentProduct() {}

        public FluentProduct(string name) {
            Name = name;
            ModelNo = name;
        }

        public virtual string Name { get; set; }
        public virtual string ModelNo { get; set; }
        public virtual decimal? Price { get; set; }

        private Iesi.Collections.Generic.ISet<Store> _storesStockedIn;

        public virtual Iesi.Collections.Generic.ISet<Store> StoresStockedIn {
            get { return _storesStockedIn ?? (_storesStockedIn = new Iesi.Collections.Generic.HashedSet<Store>()); }
        }

        public override int GetHashCode() {
            if(IsSaved)
                return base.GetHashCode();

            return HashTool.Compute(Name, ModelNo);
        }
    }
}