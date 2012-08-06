using FluentNHibernate.Mapping;
using FluentNHibernate.MappingModel.Collections;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Fluents.TreeNodeMappings {
    /// <summary>
    /// TreeView 의 Node를 매핑합니다.
    /// </summary>
    public class FTreeNodeMap : ClassMap<FTreeNode> {
        public FTreeNodeMap() {
            Id(x => x.Id).UnsavedValue(0).GeneratedBy.Native();

            Map(x => x.Title).Length(255);
            Map(x => x.Description).Length(MappingContext.MaxStringLength);
            Map(x => x.Data).Length(MappingContext.MaxStringLength);

            References(x => x.Parent)
                .Access.Property()
                .Cascade.SaveUpdate()
                .Fetch.Select()
                .LazyLoad(Laziness.Proxy);

            HasMany(x => x.Children)
                .Access.CamelCaseField(Prefix.Underscore)
                .Cascade.AllDeleteOrphan()
                .ForeignKeyConstraintName("FK_FTreeNodeMap_Child")
                .Inverse()
                .LazyLoad()
                .AsSet(SortType.Natural);


            //! NOTE: NodePosition 수형이 ITreeNodePosition이므로, TreeNodePosition으로 Casting을 수행해야 합니다.
            //! NOTE: 이 것때문에 ComponentMap<ITreeNodePosition> 을 사용하지 못합니다.
            //
            Component<TreeNodePosition>(x => x.NodePosition,
                                        p => {
                                            p.Map(x => x.Order).Column("TreeOrder".AsNamingText());
                                            p.Map(x => x.Level).Column("TreeLevel".AsNamingText());
                                        });

            //Component(x => x.NodePosition).ColumnPrefix("Tree");
        }
    }
}