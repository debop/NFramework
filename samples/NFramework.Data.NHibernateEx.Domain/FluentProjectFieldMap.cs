using FluentNHibernate.Mapping;
using NSoft.NFramework.Data.NHibernateEx.Domain.UserTypes;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    public class FluentProjectFieldMap : ClassMap<FluentProjectField> {
        public FluentProjectFieldMap() {
            Table("FluentProjectField");

            Id(x => x.Id).GeneratedBy.Foreign("FluentProject");

            HasOne(x => x.FluentProject).Constrained().ForeignKey("FK_PrjFld_Prj".AsNamingText());

            Map(x => x.Name);
            Map(x => x.Description).Length(MappingContext.MaxStringLength);

            Map(x => x.UpdateTimestamp).CustomType("Timestamp");

            HasMany(x => x.LocaleMap)
                .Table("FluentProjectFieldLocale".AsNamingText())
                .Cascade.AllDeleteOrphan()
                .LazyLoad()
                .ForeignKeyConstraintName("FK_PRJFLD_LOC_PRJFLD".AsNamingText())
                .AsMap<CultureUserType>("LocaleKey".AsNamingText())
                .Component(ce => {
                               ce.Map(x => x.Name).Column("ProjectFieldName");
                               ce.Map(x => x.Description).Length(MappingContext.MaxStringLength);
                           });
        }
    }
}