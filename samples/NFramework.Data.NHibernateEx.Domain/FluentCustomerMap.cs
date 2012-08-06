using FluentNHibernate.Mapping;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    public sealed class FluentCustomerMap : ClassMap<FluentCustomer> {
        public FluentCustomerMap() {
            Table("FluentCustomer");

            Id(x => x.Id).GeneratedBy.Native();

            Map(x => x.Name).Not.Nullable().Index("IX_FluentCustomer_Name".AsNamingText());
            Map(x => x.Credit).Length(64);

            HasManyToMany(x => x.Products)
                .Table("CustomerToProduct".AsNamingText())
                .Cascade.AllDeleteOrphan()
                .LazyLoad()
                .AsBag();

            //
            //! BUG: FluentNHibernate Convention를 사용하면, Component의 ColumnPrefix 가 작동하지 않습니다.
            //! FluentNHibernate의 ComponentMap 을 이용하여 ComponentPrefix 를 사용하는 방법과 Convention 사용이 충돌합니다. Convention 을 사용하시면, Component와 관련된 부분은 직접 매핑을 수행해 주세요.
            //

            Component(x => x.WorkAddress, cp => {
                                              cp.Map(x => x.AddressLine1);
                                              cp.Map(x => x.AddressLine2);
                                              cp.Map(x => x.City);
                                              cp.Map(x => x.Country);
                                          });

            Component(x => x.HomeAddress, cp => {
                                              cp.Map(x => x.AddressLine1);
                                              cp.Map(x => x.AddressLine2);
                                              cp.Map(x => x.City);
                                              cp.Map(x => x.Country);
                                          });

            //Component(x => x.WorkAddress).ColumnPrefix("Work".AsNamingText());
            //Component(x => x.HomeAddress).ColumnPrefix("Home".AsNamingText());
        }
    }
}