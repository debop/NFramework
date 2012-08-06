namespace NSoft.NFramework.Data.NHibernateEx {
    /// <summary>
    /// Unit Of Work의 중첩 사용을 가능토록 한 Interface
    /// </summary>
    public interface IUnitOfWorkImplementor : IUnitOfWork {
        /// <summary>
        /// 사용 Count를 증가시킨다.
        /// </summary>
        void IncrementUsages();

        /// <summary>
        /// Unit Of Work 를 중첩 방식으로 사용할 때의 바로 전의 Unit Of Work
        /// </summary>
        IUnitOfWorkImplementor Previous { get; }

        /// <summary>
        /// 현재 활성화한 ISession
        /// </summary>
        NHibernate.ISession Session { get; }
    }
}