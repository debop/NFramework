using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Loquacious.TreeNodeMappings {
    /// <summary>
    /// 부모, 자식 관계를 모두 표현 합니다. (many-to-one, set 을 매핑하는 예제입니다.)
    /// </summary>
    [Serializable]
    public class CTreeNodeEntity : TreeNodeEntityBase<CTreeNodeEntity, Int32> {
        public virtual string Title { get; set; }

        public virtual string Data { get; set; }

        public virtual string Description { get; set; }
    }
}