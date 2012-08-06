using FluentNHibernate.Mapping;
using NSoft.NFramework.Data.NHibernateEx;
using NSoft.NFramework.Data.NHibernateEx.Domain.UserTypes;

namespace NSoft.NFramework.StringResources.NHibernate {
    public class NHResourceMap : ClassMap<NHResource> {
        public NHResourceMap() {
            Table("RCL_RESOURCE");
            DynamicInsert();
            DynamicUpdate();
            LazyLoad();

            Id(x => x.Id).Column("RES_ID").GeneratedBy.Native().UnsavedValue(0);

            Map(x => x.AssemblyName).Column("ASSEMBLY_NAME").Length(128).Index("IX_RESOURCE_KEY");
            Map(x => x.Section).Column("SECTION_NAME").Length(128).Index("IX_RESOURCE_KEY");
            Map(x => x.ResourceKey).Column("RES_KEY").Length(255).Index("IX_RESOURCE_KEY");

            Map(x => x.ResourceValue).Column("RES_VALUE").Length(MappingContext.MaxStringLength);

            HasMany<NHResourceLocale>(x => x.LocaleMap)
                .Table("RCL_RESOURCE_LOCALE")
                .Cascade.AllDeleteOrphan()
                .LazyLoad()
                .KeyColumn("RES_ID")
                .AsMap<CultureUserType>("LOCALE_KEY")
                .Component(locale => { locale.Map(x => x.ResourceValue).Column("RES_VALUE").Length(MappingContext.MaxStringLength); });
        }

        public override NaturalIdPart<NHResource> NaturalId() {
            return
                base.NaturalId()
                    .Property(x => x.AssemblyName)
                    .Property(x => x.Section)
                    .Property(x => x.ResourceKey);
        }
    }
}