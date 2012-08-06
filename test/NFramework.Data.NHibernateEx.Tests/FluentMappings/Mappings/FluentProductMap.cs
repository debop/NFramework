using System;
using FluentNHibernate.Mapping;

namespace NSoft.NFramework.Data.NHibernateEx.FluentMappings {
    [Serializable]
    public sealed class FluentProductMap : ClassMap<FVProduct> {
        public FluentProductMap() {
            Table("FNM_PRODUCT");

            Id(p => p.Id).GeneratedBy.GuidComb();

            DiscriminateSubClassesOnColumn("PRODUCT_KIND");
            Version(p => p.Version);

            NaturalId().Not.ReadOnly().Property(p => p.Name);

            Map(p => p.Description).Length(MappingContext.MaxStringLength);
            Map(p => p.UnitPrice).Not.Nullable().Default("0");
        }
    }
}