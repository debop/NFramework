using System;
using NSoft.NFramework.Data.NHibernateEx.Domain;

namespace NSoft.NFramework.Data.NHibernateEx.ForTesting {
    /// <summary>
    /// 테스트용 Entity
    /// </summary>
    public class GuidEntityForTesting : DataEntityBase<Guid>, IAssignedIdEntity<Guid>
        // GuidEntityBase<GuidEntityForTesting>, IAssignedIdEntity<Guid>
    {
        public GuidEntityForTesting() {
            Id = Guid.NewGuid();
        }

        private int _version = -1;

        public virtual string Code { get; set; }
        public virtual string Name { get; set; }
        public virtual int Age { get; set; }

        public virtual int Version {
            get { return _version; }
            set { _version = value; }
        }

        /// <summary>
        /// Set new identity value.
        /// </summary>
        /// <param name="newId">new identity value</param>
        public virtual void SetIdentity(Guid newId) {
            base.Id = newId;
        }

        public override int GetHashCode() {
            if(IsSaved)
                return base.GetHashCode();

            return (Code ?? string.Empty).GetHashCode();
        }
    }
}