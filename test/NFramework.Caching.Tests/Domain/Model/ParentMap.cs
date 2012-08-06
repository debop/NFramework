using FluentNHibernate.Mapping;
using NSoft.NFramework.Data.NHibernateEx;

namespace NSoft.NFramework.Caching.Domain.Model {
    public class ParentMap : ClassMap<Parent> {
        public ParentMap() {
            Id(x => x.Id).GeneratedBy.Assigned();

            Version(x => x.Version).Column("ParentVer".AsNamingText());

            Map(x => x.Age).Column("ParentAge".AsNamingText());
            Map(x => x.Name);
            Map(x => x.Description).Column("ParentDesc".AsNamingText()).Length(MappingContext.MaxStringLength);

            HasMany(x => x.Children)
                .Cascade.AllDeleteOrphan()
                .Inverse()
                .LazyLoad()
                .AsBag();
        }
    }
}