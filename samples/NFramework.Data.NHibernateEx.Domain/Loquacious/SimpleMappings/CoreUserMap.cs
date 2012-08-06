using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Loquacious {
    public class CoreUserMap : ClassMapping<CoreUser> {
        public CoreUserMap() {
            Table("CoreUser");
            Lazy(false);
            DynamicInsert(true);
            DynamicUpdate(true);

            Id(x => x.Id, c => {
                              c.Column("UserId");
                              c.Generator(Generators.Native);
                          });

            Property(x => x.Name, c => {
                                      c.Column("UserName");
                                      c.NotNullable(true);
                                      c.Length(32);
                                      c.UniqueKey("UQ_CoreUser_Name");
                                  });

            Property(x => x.Password, c => {
                                          c.NotNullable(true);
                                          c.Length(64);
                                      });

            Property(x => x.Salt, c => {
                                      c.NotNullable(true);
                                      c.Length(64);
                                  });

            Property(x => x.Email, c => {
                                       c.NotNullable(true);
                                       c.Length(255);
                                       c.UniqueKey("UQ_CoreUser_Email");
                                   });

            Property(x => x.Locked);
            Property(x => x.UpdateTimestamp, c => c.Type(NHibernateUtil.Timestamp));

            //! BUG: 이게 왜 안되나 몰라 ==> 뒤에 r=>r.OneToMany() 를 꼭 해줘야 한다!!!
            //
            Bag(x => x.Tasks,
                bm => {
                    bm.Key(km => km.Column("UserId"));
                    bm.Lazy(CollectionLazy.Lazy);
                    bm.Inverse(true);
                    bm.Cascade(Cascade.All.Include(Cascade.DeleteOrphans));
                },
                ce => ce.OneToMany());
        }
    }
}