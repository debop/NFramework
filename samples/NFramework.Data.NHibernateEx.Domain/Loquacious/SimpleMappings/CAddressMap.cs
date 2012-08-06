using NHibernate.Mapping.ByCode.Conformist;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Loquacious.SimpleMappings {
    public class CAddressMap : ComponentMapping<CAddress> {
        public CAddressMap() {
            Lazy(false);

            Property(x => x.AddressLine1, c => c.Length(1024));
            Property(x => x.AddressLine2, c => c.Length(1024));
            Property(x => x.City, c => c.Length(64));
            Property(x => x.Country, c => c.Length(64));
        }
    }
}