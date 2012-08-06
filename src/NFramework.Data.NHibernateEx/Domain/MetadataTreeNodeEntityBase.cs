using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    /// <summary>
    /// Tree 형태의 자료 구조를 가지면서, Metadata를 가지는 엔티티
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TId"></typeparam>
    [Serializable]
    public class MetadataTreeNodeEntityBase<T, TId> : MetadataEntityBase<TId>,
                                                      ITreeNodeEntity<T>
        where T : ITreeNodeEntity<T> {
        private Iesi.Collections.Generic.ISet<T> _children;
        private ITreeNodePosition _nodePosition;

        /// <summary>
        /// 부모 노드
        /// </summary>
        public virtual T Parent { get; set; }

        /// <summary>
        /// 자식 노드들
        /// </summary>
        public virtual Iesi.Collections.Generic.ISet<T> Children {
            get { return _children ?? (_children = new Iesi.Collections.Generic.HashedSet<T>()); }
            protected set { _children = value; }
        }

        /// <summary>
        /// TreeNode의 위치
        /// </summary>
        public virtual ITreeNodePosition NodePosition {
            get { return _nodePosition ?? (_nodePosition = new TreeNodePosition(0, 0)); }
            set { _nodePosition = value; }
        }
        }
}