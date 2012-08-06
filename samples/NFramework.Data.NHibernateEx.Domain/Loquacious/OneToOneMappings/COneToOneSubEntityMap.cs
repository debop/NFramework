using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NSoft.NFramework.Data.NHibernateEx.Domain.UserTypes;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Loquacious {
    public class COneToOneSubEntityMap : ClassMapping<COneToOneSubEntity> {
        public COneToOneSubEntityMap() {
            Table("C_One_To_One_Sub");
            Lazy(false);
            DynamicInsert(true);
            DynamicUpdate(true);

            Id(x => x.Id, c => {
                              c.Column("SubId");
                              c.Generator(Generators.Foreign<COneToOneSubEntity>(me => me.MainEntity));
                          });

            Property(x => x.Code, c => {
                                      c.Column("EntityCode");
                                      c.NotNullable(true);
                                      c.Length(32);
                                  });
            Property(x => x.Name, c => {
                                      c.Column("EntityName");
                                      c.NotNullable(true);
                                      c.Length(64);
                                  });

            Property(x => x.Description, c => c.Length(9999));

            OneToOne(x => x.MainEntity, map => {
                                            map.Cascade(Cascade.Persist);
                                            map.Constrained(true);
                                            map.Lazy(LazyRelation.NoLazy);
                                        });

            Property(x => x.UpdateTimestamp, c => c.Type(NHibernateUtil.Timestamp));

            #region << LocaleMap >>

            Map(x => x.LocaleMap,
                mm => {
                    mm.Table("C_One_To_One_Sub_Locale");
                    mm.Cascade(Cascade.All.Include(Cascade.DeleteOrphans));
                    mm.Lazy(CollectionLazy.Lazy);
                    mm.Key(key => key.Column("SubId"));
                    mm.Inverse(false);
                },
                km => {
                    km.Element(mk => {
                                   mk.Column("LocaleKey");
                                   mk.Type<CultureUserType>();
                               });
                },
                cr => cr.Component(ce => {
                                       ce.Property(x => x.Name, c => {
                                                                    c.Column("EntityName");
                                                                    c.NotNullable(true);
                                                                });
                                       ce.Property(x => x.Description, c => c.Length(9999));
                                   }));

            #endregion

            #region << MetadataMap >>

            Map(x => x.MetadataMap,
                mm => {
                    mm.Table("C_One_To_One_Sub_Meta");
                    mm.Cascade(Cascade.All.Include(Cascade.DeleteOrphans));
                    mm.Lazy(CollectionLazy.Lazy);
                    mm.Key(key => key.Column("SubId"));
                    mm.Inverse(false);
                },
                km => km.Element(mk => mk.Column("MetaKey")),
                cr => cr.Component(ce => {
                                       ce.Property(x => x.Value, c => {
                                                                     c.Column("MetaValue");
                                                                     c.Length(9999);
                                                                 });
                                       ce.Property(x => x.ValueType, c => {
                                                                         c.Column("MetaValueType");
                                                                         c.Length(9999);
                                                                     });
                                   }));

            #endregion
        }
    }
}