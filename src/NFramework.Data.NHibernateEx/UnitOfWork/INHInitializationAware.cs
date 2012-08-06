namespace NSoft.NFramework.Data.NHibernateEx {
    /// <summary>
    /// used to handle extra initialization for configuration and session factory.
    /// </summary>
    public interface INHInitializationAware {
        /// <summary>
        /// 초기화 전에 수행해야 할 작업
        /// </summary>
        void BeforeInitialzation();

        /// <summary>
        /// NHibernate Configuration 작업에 추가할 내용들을 정의한다.
        /// </summary>
        /// <param name="cfg"></param>
        void Configured(NHibernate.Cfg.Configuration cfg);

        /// <summary>
        /// NHibernate Session Factory 초기화 작업에 추가할 내용들을 정의한다.
        /// </summary>
        /// <param name="cfg"></param>
        /// <param name="sessionFactory"></param>
        void Initialized(NHibernate.Cfg.Configuration cfg, NHibernate.ISessionFactory sessionFactory);
    }
}