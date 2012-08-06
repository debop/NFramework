using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    /// <summary>
    /// TreeView의 TreeNode 형식의 엔티티의 추상 클래스입니다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TId"></typeparam>
    [Serializable]
    public class TreeNodeEntityBase<T, TId> : DataEntityBase<TId>, ITreeNodeEntity<T> where T : TreeNodeEntityBase<T, TId> {
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