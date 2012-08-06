using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    /// <summary>
    /// Name 속성을 가진 엔티티
    /// </summary>
    /// <typeparam name="TId">Type of Entity Identity</typeparam>
    [Serializable]
    public abstract class NamedEntityBase<TId> : DataEntityBase<TId>, INamedEntity {
        /// <summary>
        /// Name
        /// </summary>
        public virtual string Name { get; set; }

        public bool Equals(INamedEntity other) {
            return (other != null) && GetHashCode().Equals(other.GetHashCode());
        }

        /// <summary>
        /// HashCode를 반환합니다.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() {
            return IsSaved ? base.GetHashCode() : HashTool.Compute(Name);
        }
    }
}