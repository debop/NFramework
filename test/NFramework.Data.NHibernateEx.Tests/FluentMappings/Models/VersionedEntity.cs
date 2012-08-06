using NSoft.NFramework.Data.NHibernateEx.Domain;

namespace NSoft.NFramework.Data.NHibernateEx.FluentMappings {
    public abstract class VersionedEntity : GuidEntityBase {
        public virtual int Version { get; set; }
    }
}