using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Loquacious.SimpleMappings {
    public class CStoreMap : ClassMapping<CStore> {
        public CStoreMap() {
            Table("CStore");
            Lazy(false);
            DynamicInsert(true);
            DynamicInsert(true);

            Id(x => x.Id, c => {
                              c.Column("StoreId");
                              c.Generator(Generators.Native);
                          });

            Property(x => x.Name);

            Bag(x => x.Staff,
                bag => {
                    bag.Cascade(Cascade.All);
                    bag.Lazy(CollectionLazy.Lazy);
                    bag.Inverse(true);
                },
                cr => cr.OneToMany());

            Bag(x => x.Products,
                bag => {
                    bag.Table("CProductStores");
                    bag.Cascade(Cascade.All);
                    bag.Lazy(CollectionLazy.Lazy);

                    bag.Key(km => {
                                km.Column("StoreId");
                                km.ForeignKey("FK_Store_Product");
                            });
                },
                cr => cr.ManyToMany(mm => mm.Column("ProductId")));
        }
    }
}