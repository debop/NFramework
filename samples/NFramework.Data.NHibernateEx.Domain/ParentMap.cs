using FluentNHibernate.Mapping;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    public class ParentMap : ClassMap<Parent> {
        public ParentMap() {
            Table("`Parent`");

            Cache.Region("NSoft.NFramework").IncludeAll().ReadWrite();

            Id(x => x.Id).GeneratedBy.Assigned();

            Version(x => x.Version).UnsavedValue("-1");

            Map(x => x.Name);
            Map(x => x.Age);

            HasMany(x => x.Children)
                .KeyColumn("ParentId")
                .Cascade.AllDeleteOrphan()
                .Inverse()
                .LazyLoad()
                .AsBag();
        }
    }
}