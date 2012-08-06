using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Fluents.TreeNodeMappings {
    [Serializable]
    public class LocaledMetadataTreeNode :
        LocaledMetadataTreeNodeEntityBase<LocaledMetadataTreeNode, Int32, LocaledMetadataTreeNodeLocale>, IUpdateTimestampedEntity {
        public virtual string Code { get; set; }

        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public virtual string ExAttr { get; set; }

        public virtual DateTime? UpdateTimestamp { get; set; }

        public override int GetHashCode() {
            if(IsSaved)
                return base.GetHashCode();

            return HashTool.Compute(Code);
        }
        }

    [Serializable]
    public class LocaledMetadataTreeNodeLocale : DataObjectBase, ILocaleValue {
        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public virtual string ExAttr { get; set; }

        public override int GetHashCode() {
            return HashTool.Compute(Name, Description, ExAttr);
        }
    }
}