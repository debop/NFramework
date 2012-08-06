using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    [Serializable]
    public class SimpleObject : AssignedIdEntityBase<Guid> {
        public SimpleObject() {
            SetIdentity(Guid.NewGuid());
            ConcurrencyId = -1;
        }

        public virtual int ConcurrencyId { get; set; }

        public virtual string TwoCharactersMax { get; set; }
    }
}