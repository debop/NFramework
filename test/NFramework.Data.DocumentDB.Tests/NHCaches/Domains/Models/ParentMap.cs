using FluentNHibernate.Mapping;
using NSoft.NFramework.Data.NHibernateEx;

namespace NSoft.NFramework.Data.MongoDB.NHCaches.Domains.Models {
    public class ParentMap : ClassMap<Parent> {
        public ParentMap() {
            Table("NH_PARENT");
            LazyLoad();
            DynamicInsert();
            DynamicUpdate();

            Id(x => x.Id).Column("PARENT_ID").GeneratedBy.Assigned();

            Version(x => x.Version);

            Map(x => x.Age).Column("PARENT_AGE");
            Map(x => x.Name).Column("PARENT_NAME");
            Map(x => x.Description).Column("PARENT_DESC").Length(MappingContext.MaxStringLength);

            HasMany(x => x.Children)
                .KeyColumn("PARENT_ID")
                .Cascade.AllDeleteOrphan()
                .Inverse()
                .LazyLoad()
                .AsBag();
        }
    }
}