using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    /// <summary>
    /// <see cref="ITreeNodeEntity{T}"/>의 TreeView상에서의 위치를 나타냅니다.
    /// </summary>
    public interface ITreeNodePosition : IDataObject, IEquatable<ITreeNodePosition> {
        /// <summary>
        /// TreeView 상에서 Node의 레벨 (root node가 level 0이다)
        /// </summary>
        int? Level { get; set; }

        /// <summary>
        /// 형제 Node 간의 순서를 나타낸다
        /// </summary>
        int? Order { get; set; }
    }
}