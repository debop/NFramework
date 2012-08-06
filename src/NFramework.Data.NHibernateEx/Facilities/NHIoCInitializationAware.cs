using System;
using NHibernate;
using NHibernate.Cfg;
using NSoft.NFramework.InversionOfControl;

namespace NSoft.NFramework.Data.NHibernateEx.Facilities {
    /// <summary>
    /// NHibernate용 Mapped Class (Entity) 에 대해 IoC를 통해 자동으로 Repository{TEntity}에 매핑되게끔 하는 초기화 모듈
    /// </summary>
    public class NHIoCInitializationAware : INHInitializationAware {
        private readonly Predicate<Type> _isCandidateForRepository;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="isCandidateRepository"></param>
        public NHIoCInitializationAware(Predicate<Type> isCandidateRepository) {
            _isCandidateForRepository = isCandidateRepository;
        }

        #region Implementation of INHInitializationAware

        /// <summary>
        /// 초기화 전에 수행해야 할 작업
        /// </summary>
        public void BeforeInitialzation() {
            // nothing to do.
        }

        /// <summary>
        /// NHibernate Configuration 작업에 추가할 내용들을 정의한다.
        /// </summary>
        /// <param name="cfg"></param>
        public void Configured(Configuration cfg) {
            // nothing to do.
        }

        /// <summary>
        /// NHibernate Session Factory 초기화 작업에 추가할 내용들을 정의한다.
        /// </summary>
        /// <param name="cfg"></param>
        /// <param name="sessionFactory"></param>
        public void Initialized(Configuration cfg, ISessionFactory sessionFactory) {
            NHIoC.Register(IoC.Container,
                           sessionFactory,
                           typeof(NHRepository<>),
                           _isCandidateForRepository);
        }

        #endregion
    }
}