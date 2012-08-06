using FluentNHibernate.Mapping;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    public class ChildMap : ClassMap<Child> {
        public ChildMap() {
            Table("`Child`");

            Cache.Region("NSoft.NFramework").ReadWrite().IncludeAll();

            Id(x => x.Id).GeneratedBy.Assigned();

            Version(x => x.Version).UnsavedValue("-1");

            Map(x => x.Name);

            References(x => x.Parent).Column("ParentId").Cascade.SaveUpdate().Fetch.Select().Not.Nullable();
        }
    }
}