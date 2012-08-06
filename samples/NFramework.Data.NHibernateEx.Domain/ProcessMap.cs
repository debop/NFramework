using FluentNHibernate.Mapping;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    public class ProcessMap : ClassMap<Process> {
        public ProcessMap() {
            Table("Process");

            Id(x => x.Id).GeneratedBy.Native();

            Map(x => x.Code).Unique().Not.Nullable();
            Map(x => x.Name).Not.Nullable();
        }
    }
}