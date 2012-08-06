using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Mappings {
    [Serializable]
    public class Master : DataEntityBase<Int64> {
        public DateTime? Version { get; protected set; }

        public string Name { get; set; }

        public int X { get; set; }

        public Master OtherMaster { get; set; }

        private Iesi.Collections.Generic.ISet<Detail> _details;

        public Iesi.Collections.Generic.ISet<Detail> Details {
            get { return _details ?? (_details = new Iesi.Collections.Generic.HashedSet<Detail>()); }
            protected set { _details = value; }
        }

        private Iesi.Collections.Generic.ISet<Detail> _moreDetails;

        public Iesi.Collections.Generic.ISet<Detail> MoreDetails {
            get { return _moreDetails ?? (_moreDetails = new Iesi.Collections.Generic.HashedSet<Detail>()); }
            protected set { _moreDetails = value; }
        }

        private Iesi.Collections.Generic.ISet<Master> _incoming;

        public Iesi.Collections.Generic.ISet<Master> Incoming {
            get { return _incoming ?? (_incoming = new Iesi.Collections.Generic.HashedSet<Master>()); }
            protected set { _incoming = value; }
        }

        private Iesi.Collections.Generic.ISet<Master> _outgoing;

        public Iesi.Collections.Generic.ISet<Master> Outgoing {
            get { return _outgoing ?? (_outgoing = new Iesi.Collections.Generic.HashedSet<Master>()); }
            protected set { _outgoing = value; }
        }

        public override int GetHashCode() {
            if(IsSaved)
                return base.GetHashCode();

            return HashTool.Compute(Name, X);
        }
    }
}