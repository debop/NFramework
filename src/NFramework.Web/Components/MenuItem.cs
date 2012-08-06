using System;
using NSoft.NFramework.Data;
using NSoft.NFramework.Data.NHibernateEx.Domain;

namespace NSoft.NFramework.Web
{
    /// <summary>
    /// 메뉴 정보
    /// </summary>
    [Serializable]
    public class MenuItem : DataObjectBase, ITreeNodeEntity<MenuItem>, IEquatable<MenuItem>
    {
        private Iesi.Collections.Generic.ISet<MenuItem> _children;
        private ITreeNodePosition _nodePosition;

        /// <summary>
        /// 생성자
        /// </summary>
        public MenuItem() {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="name">메뉴명</param>
        /// <param name="menuUrl">Url</param>
        public MenuItem(string id, string name, string menuUrl)
        {
            Id.ShouldNotBeWhiteSpace("Id");

            Id = id;
            Name = name;
            MenuUrl = menuUrl;
        }

        /// <summary>
        /// Id
        /// </summary>
        public virtual string Id { get; set; }

        /// <summary>
        /// 메뉴명
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Url
        /// </summary>
        public virtual string MenuUrl { get; set; }

        /// <summary>
        /// 부모 노드
        /// </summary>
        public MenuItem Parent { get; set; }

        /// <summary>
        /// 자식들
        /// </summary>
        public Iesi.Collections.Generic.ISet<MenuItem> Children
        {
            get { return _children ?? (_children = new Iesi.Collections.Generic.HashedSet<MenuItem>()); }
            protected set { _children = value; }
        }

        /// <summary>
        /// TreeNode의 위치
        /// </summary>
        public ITreeNodePosition NodePosition
        {
            get { return _nodePosition ?? (_nodePosition = new TreeNodePosition(0, 0)); }
            set { _nodePosition = value; }
        }

        public bool Equals(MenuItem other)
        {
            return (other != null) && GetHashCode().Equals(other.GetHashCode());
        }

        public override int GetHashCode()
        {
            return HashTool.Compute(Id);
        }

        /// <summary>
        /// 객체를 표현한 문자열을 반환합니다.
        /// </summary>
        /// <returns>문자열</returns>
        public override string ToString()
        {
            return string.Format("MenuItem# Id=[{0}],Name=[{1}], MenuUrl=[{2}]", Id, Name, MenuUrl);
        }
    }
}