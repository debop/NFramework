using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Fluents.TreeNodeMappings {
    /// <summary>
    /// TreeView의 Node를 나타냅니다.
    /// </summary>
    [Serializable]
    public class FTreeNode : TreeNodeEntityBase<FTreeNode, Int32> {
        public virtual string Title { get; set; }

        public virtual string Data { get; set; }

        public virtual string Description { get; set; }
    }
}