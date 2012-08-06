using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Loquacious.SimpleMappings {
    public class CCustomerMap : ClassMapping<CCustomer> {
        public CCustomerMap() {
            Table("CCustomer");
            Lazy(false);
            DynamicInsert(true);
            DynamicUpdate(true);

            Id(x => x.Id, im => {
                              im.Column("CustomerId");
                              im.Generator(Generators.Native);
                          });

            Property(x => x.Name, pm => {
                                      pm.Column("CustomerName");
                                      pm.NotNullable(true);
                                      pm.Index("IX_CCUSTOMER_NAME");
                                  });
            Property(x => x.Credit, pm => {
                                        pm.Column("CustomerCredit");
                                        pm.Length(64);
                                    });

            Bag(x => x.Products,
                bm => {
                    bm.Table("CProductOfCustomer");
                    bm.Lazy(CollectionLazy.Lazy);
                    bm.Cascade(Cascade.All);

                    bm.Key(k => {
                               k.Column("CustomerId");
                               k.ForeignKey("FK_CCustomer_CProduct");
                           });
                },
                cr => cr.ManyToMany(mm => {
                                        mm.Column("ProductId");
                                        mm.ForeignKey("FK_CProduct_CCustomer");
                                    }));


            // NOTE: 이게 문제네... FluentNHibernate에서 제공하는 ColumnPrefix를 제공하지 않네요!!!
            //

            Component(x => x.WorkAddress, cm => {
                                              cm.Class<CAddress>();
                                              cm.Property(x => x.AddressLine1, c => {
                                                                                   c.Column("WorkAddressLine1");
                                                                                   c.Length(1024);
                                                                               });
                                              cm.Property(x => x.AddressLine2, c => {
                                                                                   c.Column("WorkAddressLine2");
                                                                                   c.Length(1024);
                                                                               });
                                              cm.Property(x => x.City, c => {
                                                                           c.Column("WorkCity");
                                                                           c.Length(64);
                                                                       });
                                              cm.Property(x => x.Country, c => {
                                                                              c.Column("WorkCountry");
                                                                              c.Length(64);
                                                                          });
                                          });

            Component(x => x.HomeAddress, cm => {
                                              cm.Class<CAddress>();
                                              cm.Property(x => x.AddressLine1, c => {
                                                                                   c.Column("HomeAddressLine1");
                                                                                   c.Length(1024);
                                                                               });
                                              cm.Property(x => x.AddressLine2, c => {
                                                                                   c.Column("HomeAddressLine2");
                                                                                   c.Length(1024);
                                                                               });
                                              cm.Property(x => x.City, c => {
                                                                           c.Column("HomeCity");
                                                                           c.Length(64);
                                                                       });
                                              cm.Property(x => x.Country, c => {
                                                                              c.Column("HomeCountry");
                                                                              c.Length(64);
                                                                          });
                                          });
        }
    }
}