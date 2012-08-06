using FluentNHibernate.Mapping;
using NSoft.NFramework.Data.NHibernateEx.Domain.UserTypes;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    public class CompanyMap : ClassMap<Company> {
        public CompanyMap() {
            Table("`Company`");

            Id(x => x.Id).GeneratedBy.Native();

            Map(x => x.Code).Not.Nullable();
            Map(x => x.Name).Not.Nullable();
            Map(x => x.IsActive);
            Map(x => x.IsDefault);

            Map(x => x.Description).Length(MappingContext.MaxStringLength);
            Map(x => x.ExAttr).Length(MappingContext.MaxStringLength);

            HasMany(x => x.Users)
                .Cascade.AllDeleteOrphan()
                .Inverse()
                .LazyLoad()
                .AsSet();

            HasMany<Company.FCompanyLocale>(x => x.LocaleMap)
                .Table("CompanyLocale".AsNamingText())
                .Cascade.AllDeleteOrphan()
                .AsMap<CultureUserType>("LocaleKey".AsNamingText())
                .Component(loc => {
                               loc.Map(x => x.Name).Not.Nullable();
                               loc.Map(x => x.Description).Length(MappingContext.MaxStringLength);
                               loc.Map(x => x.ExAttr).Length(MappingContext.MaxStringLength);
                           });

            HasMany<MetadataValue>(x => x.MetadataMap)
                .Table("CompanyMeta".AsNamingText())
                .Cascade.AllDeleteOrphan()
                .AsMap<string>("MetaKey".AsNamingText())
                .Component(m => {
                               m.Map(x => x.Value).Column("MetaValue".AsNamingText()).Length(MappingContext.MaxStringLength);
                               m.Map(x => x.ValueType).Column("MetaValueType".AsNamingText());
                           });
        }
    }
}