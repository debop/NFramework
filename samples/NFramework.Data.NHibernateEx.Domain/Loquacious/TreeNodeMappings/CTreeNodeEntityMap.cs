using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Loquacious.TreeNodeMappings {
    /// <summary>
    /// 부모, 자식 관계를 모두 표현 합니다. (many-to-one, set 을 매핑하는 예제입니다.)
    /// </summary>
    public class CTreeNodeEntityMap : ClassMapping<CTreeNodeEntity> {
        public CTreeNodeEntityMap() {
            Table("CTreeNodeEntity");
            Lazy(false);
            DynamicInsert(true);
            DynamicUpdate(true);

            Id(x => x.Id, c => {
                              c.Column("NodeId");
                              c.Generator(Generators.Native);
                          });

            Property(x => x.Title, c => c.Length(255));
            Property(x => x.Description, c => c.Length(9999));
            Property(x => x.Data, c => c.Length(9999));

            ManyToOne(x => x.Parent,
                      map => {
                          map.Column("ParentId");
                          map.Cascade(Cascade.Persist);
                          map.Fetch(FetchKind.Select);
                          map.Lazy(LazyRelation.Proxy);
                          map.ForeignKey("FK_CTreeNode_Parent");
                      });

            Set(x => x.Children,
                map => {
                    map.Cascade(Cascade.All.Include(Cascade.DeleteOrphans));
                    map.Inverse(true);
                    map.Lazy(CollectionLazy.Lazy);
                    map.Key(key => key.ForeignKey("FK_CTreeNode_Child"));
                },
                rel => rel.OneToMany());


            // CTreeNodePositionMap 을 사용하여 미리 정의된 Component Mapping을 재활용합니다.
            // Component(x => x.NodePosition);

            Component(x => x.NodePosition,
                      cm => {
                          cm.Class<TreeNodePosition>();
                          cm.Property(x => x.Order, c => c.Column("TreeOrder"));
                          cm.Property(x => x.Level, c => c.Column("TreeLevel"));
                      });
        }
    }
}