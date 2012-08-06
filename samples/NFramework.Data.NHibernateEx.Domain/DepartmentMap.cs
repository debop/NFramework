using FluentNHibernate.Mapping;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    public class DepartmentMap : ClassMap<Department> {
        public DepartmentMap() {
            Table("Department");

            Id(x => x.Id).GeneratedBy.Native();

            References(x => x.Company).Fetch.Select();

            Map(x => x.Code);
            Map(x => x.Name);


            References(x => x.Parent).Column("ParentId").Fetch.Select().LazyLoad();

            HasMany(x => x.Children)
                .KeyColumn("ParentId")
                .Cascade.AllDeleteOrphan()
                .Inverse()
                .LazyLoad()
                .AsSet();


            Component<TreeNodePosition>(x => x.NodePosition,
                                        p => {
                                            p.Map(x => x.Order).Column("TreeOrder".AsNamingText());
                                            p.Map(x => x.Level).Column("TreeLevel".AsNamingText());
                                        });

            HasMany(x => x.Members)
                .Cascade.AllDeleteOrphan()
                .Inverse()
                .LazyLoad()
                .AsSet();
        }
    }
}