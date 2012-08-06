using FluentNHibernate.Mapping;
using NSoft.NFramework.Data.NHibernateEx;
using NSoft.NFramework.Data.NHibernateEx.Domain.UserTypes;

namespace NSoft.NFramework.Data.DevartOracle.NH.Domains.Models {
    public class ProjectFieldMap : ClassMap<ProjectField> {
        public ProjectFieldMap() {
            Id(x => x.Id).GeneratedBy.Foreign("Project");

            HasOne(x => x.Project).Constrained().ForeignKey("FK_PrjFld_Prj".AsNamingText());

            Map(x => x.Name);
            Map(x => x.Description).Length(MappingContext.MaxStringLength);

            Map(x => x.UpdateTimestamp).CustomType("Timestamp");

            HasMany(x => x.LocaleMap)
                .Table("ProjectFieldLocale".AsNamingText())
                .Cascade.AllDeleteOrphan()
                .LazyLoad()
                .ForeignKeyConstraintName("FK_PrjFldLoc_PrjFld".AsNamingText())
                .AsMap<CultureUserType>("LocaleKey".AsNamingText())
                .Component(ce => {
                               ce.Map(x => x.Name).Column("ProjectFieldName");
                               ce.Map(x => x.Description).Length(MappingContext.MaxStringLength);
                           });
        }
    }
}