using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    /// <summary>
    /// 다국어 정보를 가지는 TreeNode 엔티티의 기본 클래스입니다.
    /// </summary>
    /// <typeparam name="T">엔티티의 수형</typeparam>
    /// <typeparam name="TId">엔티티 Identifier의 수형</typeparam>
    /// <typeparam name="TLocale">엔티티의 지역화 정보 수형</typeparam>
    [Serializable]
    public class LocaledTreeNodeEntityBase<T, TId, TLocale> : LocaledEntityBase<TId, TLocale>, ITreeNodeEntity<T>
        where T : LocaledTreeNodeEntityBase<T, TId, TLocale>
        where TLocale : ILocaleValue {
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