using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    /// <summary>
    /// Entity의 Identity값이 Database System에 의해서 설정되는 것이 아니라, 사용자가 설정하는 Entity의 기본 Class입니다.
    /// (id의 generator class="assigned" 인 entity)
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    [Serializable]
    public class AssignedIdEntityBase<TId> : DataEntityBase<TId>, IAssignedIdEntity<TId> {
        /// <summary>
        /// Entity의 Identity값을 설정합니다.
        /// </summary>
        /// <param name="newId"></param>
        public virtual void SetIdentity(TId newId) {
            Id = newId;
        }
    }
}