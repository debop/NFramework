using FluentNHibernate.Mapping;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    public class NHEmployeeMap : ClassMap<NHEmployee> {
        public NHEmployeeMap() {
            Id(x => x.Id).GeneratedBy.Native();

            Map(x => x.FirstName).Not.Nullable().Length(1024);
            Map(x => x.LastName).Not.Nullable().Length(255);

            //! NHEmployee : Picture는 1:1 매핑을 가진다.
            HasOne(x => x.Picture).LazyLoad(Laziness.Proxy).Fetch.Select().Cascade.All();

            // many-to-one
            References(x => x.Store)
                .Fetch.Select()
                .LazyLoad(Laziness.Proxy)
                .Nullable();
        }
    }
}