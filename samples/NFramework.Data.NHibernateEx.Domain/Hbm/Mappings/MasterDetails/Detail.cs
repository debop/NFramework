using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Mappings {
    [Serializable]
    public class Detail : DataEntityBase<Int64> {
        public string X { get; set; }

        public int I { get; set; }

        public Master Master { get; set; }

        private Iesi.Collections.Generic.ISet<Detail> _subDetails;

        public Iesi.Collections.Generic.ISet<Detail> SubDetails {
            get { return _subDetails ?? (_subDetails = new Iesi.Collections.Generic.HashedSet<Detail>()); }
            protected set { _subDetails = value; }
        }

        public override int GetHashCode() {
            return IsSaved
                       ? base.GetHashCode()
                       : HashTool.Compute(X, I);
        }
    }
}