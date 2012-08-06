using FluentNHibernate.Mapping;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    public class AddressMap : ComponentMap<Address> {
        public AddressMap() {
            Map(x => x.AddressLine1).Length(255);
            Map(x => x.AddressLine2).Length(255);
            Map(x => x.City).Length(64);
            Map(x => x.Country).Length(64);
        }
    }
}