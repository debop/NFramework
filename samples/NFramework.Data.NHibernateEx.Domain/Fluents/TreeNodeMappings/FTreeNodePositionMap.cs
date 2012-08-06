using FluentNHibernate.Mapping;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Fluents.TreeNodeMappings {
    public class FTreeNodePositionMap : ComponentMap<TreeNodePosition> {
        public FTreeNodePositionMap() {
            Access.CamelCaseField(Prefix.Underscore);

            Map(x => x.Order);
            Map(x => x.Level);
        }
    }
}