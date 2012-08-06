using FluentNHibernate.Mapping;
using NSoft.NFramework.Data.NHibernateEx;

namespace NSoft.NFramework.Caching.Domain.Model {
    public class ChildMap : ClassMap<Child> {
        public ChildMap() {
            Id(x => x.Id).GeneratedBy.Assigned();

            Version(x => x.Version).Column("ChildVer".AsNamingText());

            Map(x => x.Name);
            Map(x => x.Description).Column("ChildDesc".AsNamingText()).Length(MappingContext.MaxStringLength);

            References(x => x.Parent)
                .Cascade.SaveUpdate()
                .Fetch.Select()
                .LazyLoad(Laziness.Proxy);
        }
    }
}