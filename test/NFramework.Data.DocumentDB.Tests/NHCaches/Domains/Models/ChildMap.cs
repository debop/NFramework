using FluentNHibernate.Mapping;
using NSoft.NFramework.Data.NHibernateEx;

namespace NSoft.NFramework.Data.MongoDB.NHCaches.Domains.Models {
    public class ChildMap : ClassMap<Child> {
        public ChildMap() {
            Table("NH_CHILD");
            LazyLoad();
            DynamicInsert();
            DynamicUpdate();

            Id(x => x.Id).Column("CHILD_ID").GeneratedBy.Assigned();

            Version(x => x.Version);

            Map(x => x.Name);
            Map(x => x.Description).Length(MappingContext.MaxStringLength);

            References(x => x.Parent)
                .Column("PARENT_ID")
                .Cascade.SaveUpdate()
                .Fetch.Select()
                .LazyLoad(Laziness.Proxy);
        }
    }
}