using FluentNHibernate.Mapping;
using NSoft.NFramework.Data.NHibernateEx.Domain.UserTypes;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    public class FluentProjectMap : ClassMap<FluentProject> {
        public FluentProjectMap() {
            Table("FluentProject");

            Id(x => x.Id).GeneratedBy.Native();

            Map(x => x.Code).CustomType("AnsiString").Not.Nullable();
            Map(x => x.Name).Not.Nullable();

            Map(x => x.Description).Length(MappingContext.MaxStringLength);
            Map(x => x.UpdateTimestamp).CustomType("Timestamp");

            HasOne(x => x.Field).Cascade.All();

            HasMany(x => x.LocaleMap)
                .Table("FluentProjectLocale".AsNamingText())
                .Cascade.AllDeleteOrphan()
                .LazyLoad()
                .AsMap<CultureUserType>("LocaleKey".AsNamingText())
                .Component(ce => {
                               ce.Map(x => x.Name);
                               ce.Map(x => x.Description).Length(MappingContext.MaxStringLength);
                           });
        }

        public override NaturalIdPart<FluentProject> NaturalId() {
            return base.NaturalId().Property(x => x.Code);
        }
    }
}