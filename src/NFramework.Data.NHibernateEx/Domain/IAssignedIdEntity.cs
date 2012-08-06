namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    /// <summary>
    /// Identity 값을 Business Layer에서 Assign해주어야 하는 Entity를 표현한 Interface
    /// </summary>
    /// <typeparam name="TId"></typeparam>
    public interface IAssignedIdEntity<TId> {
        /// <summary>
        /// Set new identity value.
        /// </summary>
        /// <param name="newId">new identity value</param>
        void SetIdentity(TId newId);
    }
}