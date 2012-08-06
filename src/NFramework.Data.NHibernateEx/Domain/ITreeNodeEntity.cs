using Iesi.Collections.Generic;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    /// <summary>
    /// Tree 형식의 부모와 자식들을 가지는 엔티티에 대한 Interface
    /// </summary>
    /// <typeparam name="T">TreeNodeEntity 수형</typeparam>
    public interface ITreeNodeEntity<T> : IDataObject where T : ITreeNodeEntity<T> {
        /// <summary>
        /// 부모 노드
        /// </summary>
        T Parent { get; set; }

        /// <summary>
        /// 자식 노드들
        /// </summary>
        ISet<T> Children { get; }

        /// <summary>
        /// TreeNode의 위치
        /// </summary>
        ITreeNodePosition NodePosition { get; }
    }
}