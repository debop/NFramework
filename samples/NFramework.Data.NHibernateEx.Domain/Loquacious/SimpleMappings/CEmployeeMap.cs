using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Loquacious.SimpleMappings {
    public class CEmployeeMap : ClassMapping<CEmployee> {
        public CEmployeeMap() {
            Table("CEmployee");
            Lazy(false);
            DynamicInsert(true);
            DynamicUpdate(true);

            Id(x => x.Id, im => im.Generator(Generators.Native));

            Property(x => x.FirstName, c => {
                                           c.NotNullable(true);
                                           c.Length(1024);
                                       });
            Property(x => x.LastName, c => {
                                          c.NotNullable(true);
                                          c.Length(1024);
                                      });

            OneToOne(x => x.Picture, c => {
                                         c.Cascade(Cascade.All);
                                         c.Lazy(LazyRelation.NoLazy);
                                     });

            ManyToOne(x => x.Store, c => {
                                        c.Cascade(Cascade.Persist);
                                        c.Fetch(FetchKind.Select);
                                        c.Lazy(LazyRelation.Proxy);
                                        c.NotNullable(false);
                                    });
        }
    }
}