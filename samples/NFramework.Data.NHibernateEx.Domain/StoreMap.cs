using FluentNHibernate.Mapping;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    public class StoreMap : ClassMap<Store> {
        public StoreMap() {
            Table("Store");

            Id(x => x.Id).GeneratedBy.Native().Column("StoreId").UnsavedValue(0);

            Map(x => x.Name);

            HasMany(x => x.Staff)
                .Access.CamelCaseField(Prefix.Underscore)
                .KeyColumn("StoreId")
                .Inverse()
                .LazyLoad()
                .Cascade.All();

            HasManyToMany(x => x.Products)
                .Access.CamelCaseField(Prefix.Underscore)
                .LazyLoad()
                .Cascade.All()
                .Table("ProductInStores")
                .ParentKeyColumn("StoreId")
                .ChildKeyColumn("ProductId");
        }
    }
}