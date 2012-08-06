using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    /// <summary>
    /// TreeNode의 TreeView 상에서의 위치를 나타내는 레벨, 정렬 순서를 나타냅니다.
    /// </summary>
    [Serializable]
    public class TreeNodePosition : DataObjectBase, ITreeNodePosition {
        /// <summary>
        /// 생성자
        /// </summary>
        public TreeNodePosition() : this(0, 0) {}

        /// <summary>
        /// 생성자
        /// </summary>
        public TreeNodePosition(int? level, int? order) {
            Level = level ?? 0;
            Order = order ?? 0;
        }

        /// <summary>
        /// Copy 생성자
        /// </summary>
        /// <param name="src"></param>
        public TreeNodePosition(ITreeNodePosition src)
            : base(src) {
            //if(src != null)
            //{
            //	Level = src.Level;
            //	Order = src.Order;
            //}
        }

        /// <summary>
        /// TreeView 상에서 Node의 레벨 (root node가 level 0이다)
        /// </summary>
        public virtual int? Level { get; set; }

        /// <summary>
        /// 형제 Node 간의 순서를 나타낸다
        /// </summary>
        public virtual int? Order { get; set; }

        /// <summary>
        /// HashCode를 반환합니다.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() {
            return HashTool.Compute(Level, Order);
        }

        /// <summary>
        /// 현재 개체가 동일한 형식의 다른 개체와 같은지 여부를 나타냅니다.
        /// </summary>
        /// <returns>
        /// 현재 개체가 <paramref name="other"/> 매개 변수와 같으면 true이고, 그렇지 않으면 false입니다.
        /// </returns>
        /// <param name="other">이 개체와 비교할 개체입니다.</param>
        public bool Equals(ITreeNodePosition other) {
            return (other != null) && GetHashCode().Equals(other);
        }

        /// <summary>
        /// 현재 TreeNodePosition 인스턴스를 문자열로 표현합니다.
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return string.Format("TreeNodePosition# Level=[{0}], Order=[{1}]", Level, Order);
        }
    }
}