using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NSoft.NFramework.Data.NHibernateEx.Domain.UserTypes;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Loquacious {
    /// <summary>
    /// <see cref="CCompany"/>의 NHibernate Mapping 파일입니다.
    /// </summary>
    public class CCompanyMap : ClassMapping<CCompany> {
        public CCompanyMap() {
            Table("CCompany");
            Lazy(false);
            DynamicInsert(true);
            DynamicUpdate(true);

            Id(x => x.Id, c => {
                              c.Column("CompanyId");
                              c.Generator(Generators.GuidComb);
                          });

            Property(x => x.Code, c => {
                                      c.Column("CompanyCode");
                                      c.NotNullable(true);
                                      c.Length(64);
                                      c.UniqueKey("UQ_Company_Code");
                                  });
            Property(x => x.Name, c => {
                                      c.Column("CompanyName");
                                      c.NotNullable(true);
                                      c.Length(64);
                                  });

            Property(x => x.IsActive);
            Property(x => x.Description, c => c.Length(9999));
            Property(x => x.ExAttr, c => c.Length(9999));

            Property(x => x.UpdateTimestamp, c => c.Type(NHibernateUtil.Timestamp));

            Bag(x => x.Users,
                bag => {
                    bag.Key(key => key.Column("CompanyId"));
                    bag.Inverse(true);
                    bag.Cascade(Cascade.All.Include(Cascade.DeleteOrphans));
                    bag.Lazy(CollectionLazy.Lazy);
                },
                rel => rel.OneToMany());

            #region << LocaleMap >>

            Map(x => x.LocaleMap,
                mm => {
                    mm.Table("CCompanyLocale");
                    mm.Cascade(Cascade.All.Include(Cascade.DeleteOrphans));
                    mm.Lazy(CollectionLazy.Lazy);
                    mm.Key(key => key.Column("CompanyId"));
                    mm.Inverse(false);
                },
                km => {
                    km.Element(mk => {
                                   mk.Column("LocaleKey");
                                   mk.Type<CultureUserType>();
                               });
                },
                cr => cr.Component(ce => {
                                       ce.Class<CCompanyLocale>();

                                       ce.Property(x => x.Name, c => {
                                                                    c.Column("CompanyName");
                                                                    c.NotNullable(true);
                                                                });
                                       ce.Property(x => x.Description, c => c.Length(9999));
                                       ce.Property(x => x.ExAttr, c => c.Length(9999));
                                   }));

            #endregion

            #region << MetadataMap >>

            Map(x => x.MetadataMap,
                mm => {
                    mm.Table("CCompanyMetadata");
                    mm.Cascade(Cascade.All.Include(Cascade.DeleteOrphans));
                    mm.Lazy(CollectionLazy.Lazy);
                    mm.Key(key => key.Column("CompanyId"));
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

            #endregion
        }
    }
}