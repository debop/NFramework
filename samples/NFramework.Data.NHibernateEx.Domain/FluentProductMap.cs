using FluentNHibernate.Mapping;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    public class FluentProductMap : ClassMap<FluentProduct> {
        public FluentProductMap() {
            Table("FluentProduct");

            Id(x => x.Id).GeneratedBy.Native();

            Map(x => x.Name).Not.Nullable();
            Map(x => x.ModelNo).Not.Nullable();

            Map(x => x.Price).Nullable();
        }
    }
}