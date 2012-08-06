using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Loquacious.SimpleMappings {
    public class CProductMap : ClassMapping<CProduct> {
        public CProductMap() {
            Table("CProduct");
            Lazy(false);
            DynamicInsert(true);
            DynamicInsert(true);

            Id(x => x.Id, c => c.Generator(Generators.Native));

            Property(x => x.Name, c => {
                                      c.Column("ProductName");
                                      c.NotNullable(true);
                                  });
            Property(x => x.ModelNo, c => c.NotNullable(true));

            Property(x => x.Price);
        }
    }
}