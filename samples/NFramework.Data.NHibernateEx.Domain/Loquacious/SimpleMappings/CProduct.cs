using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Loquacious.SimpleMappings {
    [Serializable]
    public class CProduct : DataEntityBase<Int32> {
        protected CProduct() {}

        public CProduct(string name) {
            Name = name;
            ModelNo = name;
        }

        public virtual string Name { get; set; }
        public virtual string ModelNo { get; set; }
        public virtual decimal? Price { get; set; }

        private Iesi.Collections.Generic.ISet<CStore> _storesStockedIn;

        public virtual Iesi.Collections.Generic.ISet<CStore> StoresStockedIn {
            get { return _storesStockedIn ?? (_storesStockedIn = new Iesi.Collections.Generic.HashedSet<CStore>()); }
            protected set { _storesStockedIn = value; }
        }

        public override int GetHashCode() {
            if(IsSaved)
                return base.GetHashCode();

            return HashTool.Compute(Name, ModelNo);
        }
    }
}