using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    /// <summary>
    /// Name 속성과 Locale 속성을 가진 엔티티
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    /// <typeparam name="TLocale"></typeparam>
    [Serializable]
    public abstract class NamedLocaledEntityBase<TId, TLocale> : LocaledEntityBase<TId, TLocale>, INamedEntity
        where TLocale : ILocaleValue {
        /// <summary>
        /// Entity name
        /// </summary>
        public virtual string Name { get; set; }

        public bool Equals(INamedEntity other) {
            return (other != null) && GetHashCode().Equals(other.GetHashCode());
        }

        public override int GetHashCode() {
            return IsSaved ? base.GetHashCode() : HashTool.Compute(Name);
        }
        }
}