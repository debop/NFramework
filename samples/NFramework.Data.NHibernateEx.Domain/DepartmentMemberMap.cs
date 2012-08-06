using FluentNHibernate.Mapping;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    public class DepartmentMemberMap : ClassMap<DepartmentMember> {
        public DepartmentMemberMap() {
            Table("DepartmentMember");

            Id(x => x.Id).GeneratedBy.Native();

            References(x => x.Department).Cascade.SaveUpdate().Fetch.Select().LazyLoad();
            References(x => x.User).Cascade.SaveUpdate().Fetch.Select().LazyLoad();

            Map(x => x.IsActive);
            Map(x => x.IsLeader);

            Map(x => x.UpdateTimestamp).CustomType("Timestamp");
        }
    }
}