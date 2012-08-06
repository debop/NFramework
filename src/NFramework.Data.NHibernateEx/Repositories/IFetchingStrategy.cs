using NHibernate;

namespace NSoft.NFramework.Data.NHibernateEx {
    /// <summary>
    /// Database fetching strategy
    /// </summary>
    public interface IFetchingStrategy {
        /// <summary>
        /// 지정된 Criteria에 Fetching Strategy를 적용시킵니다.
        /// </summary>
        /// <param name="criteria">질의용 criteria</param>
        /// <returns>Fetching 전략이 적용된 Criteria</returns>
        ICriteria Apply(ICriteria criteria);
    }

    /// <summary>
    ///  Database fetching strategy for the specified entity type.
    /// </summary>
    /// <typeparam name="TEntity">Type of entity</typeparam>
    public interface IFetchingStrategy<TEntity> : IFetchingStrategy {}
}