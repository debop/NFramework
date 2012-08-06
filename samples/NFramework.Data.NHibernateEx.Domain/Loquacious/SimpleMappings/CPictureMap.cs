using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Loquacious.SimpleMappings {
    public class CPictureMap : ClassMapping<CPicture> {
        public CPictureMap() {
            Table("CPicture");
            Lazy(false);
            DynamicInsert(true);
            DynamicUpdate(true);

            //! NOTE: one-to-one 의 연결 고리가 이상하다!!!
            Id(x => x.Id, c => {
                              c.Column("EmployeeId");
                              c.Generator(Generators.Foreign<CPicture>(x => x.Employee));
                          });

            Property(x => x.Sign, c => c.Length(1024));
            Property(x => x.Stamp, x => x.Length(1024));

            OneToOne(x => x.Employee, c => {
                                          c.Cascade(Cascade.Persist);
                                          c.Constrained(true);
                                      });
        }
    }
}