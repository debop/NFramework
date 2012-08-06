using System;
using Iesi.Collections.Generic;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    /// <summary>
    /// 계층구조를 가지고, 조상, 자손 정보를 가지는 엔티티의 추상 클래스입니다.
    /// </summary>
    /// <typeparam name="T">엔티티 수형</typeparam>
    /// <typeparam name="TId">엔티티 Identity의 수형</typeparam>
    [Serializable]
    public abstract class HierarchyEntityBase<T, TId> : TreeNodeEntityBase<T, TId>, IHierarchyEntity<T>
        where T : HierarchyEntityBase<T, TId> {
        private ISet<T> _ancestors;
        private ISet<T> _descendents;

        /// <summary>
        /// 조상 엔티티의 집합
        /// </summary>
        public virtual ISet<T> Ancestors {
            get { return _ancestors ?? (_ancestors = new HashedSet<T>()); }
            protected set { _ancestors = value; }
        }

        /// <summary>
        /// 자손 엔티티의 집합
        /// </summary>
        public virtual ISet<T> Descendents {
            get { return _descendents ?? (_descendents = new HashedSet<T>()); }
            protected set { _descendents = value; }
        }
        }
}