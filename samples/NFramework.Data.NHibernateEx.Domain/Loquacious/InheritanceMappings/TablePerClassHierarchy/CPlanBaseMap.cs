using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Loquacious {
    public class CPlanBaseMap : ClassMapping<CPlanBase> {
        public CPlanBaseMap() {
            Table("C_PLAN_TABLE");
            Lazy(true);
            DynamicInsert(true);
            DynamicUpdate(true);

            Discriminator(dm => dm.Column("PlanKind"));

            Id(x => x.Id, c => {
                              c.Column("PlanId");
                              c.Generator(Generators.Native);
                          });

            Property(x => x.Title, c => c.NotNullable(true));
            Property(x => x.Content, c => c.Length(9999));

            Property(x => x.ReporterId, c => c.NotNullable(true));
            Property(x => x.ReportDate);
        }
    }
}