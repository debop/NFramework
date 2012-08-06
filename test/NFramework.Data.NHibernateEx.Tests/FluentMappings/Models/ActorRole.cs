namespace NSoft.NFramework.Data.NHibernateEx.FluentMappings {
    public class ActorRole : VersionedEntity {
        public virtual string Actor { get; set; }

        public virtual string Role { get; set; }

        public virtual Movie Movie { get; set; }
    }
}