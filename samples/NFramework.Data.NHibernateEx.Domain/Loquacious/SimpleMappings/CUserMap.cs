using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NSoft.NFramework.Data.NHibernateEx.Domain.UserTypes;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Loquacious {
    public class CUserMap : ClassMapping<CUser> {
        public CUserMap() {
            Table("CUser");
            Lazy(false);
            DynamicInsert(true);
            DynamicUpdate(true);

            Id(x => x.Id, c => {
                              c.Column("UserId");
                              c.Generator(Generators.GuidComb);
                          });

            ManyToOne(x => x.Company, c => {
                                          c.Column("CompanyId");
                                          c.Lazy(LazyRelation.NoLazy);
                                          c.Fetch(FetchKind.Select);

                                          //! Cascade="save-update" 와 같은 게 없다. 그래서 Persist 로 해결했음.
                                          c.Cascade(Cascade.Persist);
                                      });

            Property(x => x.Code, c => {
                                      c.Column("UserCode");
                                      c.NotNullable(true);
                                      c.Length(32);
                                      c.UniqueKey("UQ_UserCode");
                                  });
            Property(x => x.Name, c => {
                                      c.Column("UserName");
                                      c.NotNullable(true);
                                      c.Length(48);
                                  });

            Property(x => x.IsActive);
            Property(x => x.Description, c => c.Length(9999));
            Property(x => x.ExAttr, c => c.Length(9999));

            Property(x => x.JsonData, c => {
                                          c.Type<JsonSerializableUserType>();
                                          c.Length(9999);
                                      });

            Property(x => x.UpdateTimestamp, c => c.Type(NHibernateUtil.Timestamp));


            Map(x => x.LocaleMap,
                mm => {
                    mm.Table("CUserLocale");
                    mm.Cascade(Cascade.All.Include(Cascade.DeleteOrphans));
                    mm.Lazy(CollectionLazy.Lazy);
                    mm.Key(key => key.Column("UserId"));
                    mm.Inverse(false);
                },
                km => {
                    km.Element(mk => {
                                   mk.Column("LocaleKey");
                                   mk.Type<CultureUserType>();
                               });
                },
                cr => cr.Component(ce => {
                                       ce.Class<CUserLocale>();
                                       ce.Property(x => x.Name, c => {
                                                                    c.Column("UserName");
                                                                    c.NotNullable(true);
                                                                });
                                       ce.Property(x => x.Description, c => c.Length(9999));
                                       ce.Property(x => x.ExAttr, c => c.Length(9999));
                                   }));

            Map(x => x.MetadataMap,
                mm => {
                    mm.Table("CUserMetadata");
                    mm.Cascade(Cascade.All.Include(Cascade.DeleteOrphans));
                    mm.Lazy(CollectionLazy.Lazy);
                    mm.Key(key => key.Column("UserId"));
                    mm.Inverse(false);
                },
                km => km.Element(mk => mk.Column("MetaKey")),
                cr => cr.Component(ce => {
                                       ce.Class<MetadataValue>();
                                       ce.Property(x => x.Value,
                                                   c => {
                                                       c.Column("MetaValue");
                                                       c.Length(9999);
                                                   });
                                       ce.Property(x => x.ValueType,
                                                   c => {
                                                       c.Column("MetaValueType");
                                                       c.Length(9999);
                                                   });
                                   }));
        }
    }
}