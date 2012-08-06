using NHibernate.Mapping.ByCode.Conformist;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Loquacious.TreeNodeMappings {
    public class CITreeNodePositionMap : ComponentMapping<ITreeNodePosition> {
        public CITreeNodePositionMap() {
            Lazy(false);

            Property(x => x.Order, c => c.Column("TreeOrder"));
            Property(x => x.Level, c => c.Column("TreeLevel"));
        }
    }
}