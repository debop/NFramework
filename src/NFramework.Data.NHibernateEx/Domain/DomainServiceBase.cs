using System;
using NHibernate;
using NHibernate.Engine;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Services {
    /// <summary>
    /// Domain Service의 기본 Class입니다.
    /// </summary>
    [Serializable]
    public abstract class DomainServiceBase {
        /// <summary>
        /// Current UnitOfWork의 <see cref="ISessionFactoryImplementor"/> 입니다.
        /// </summary>
        protected virtual ISessionFactoryImplementor SessionFactoryImplementor {
            get { return (ISessionFactoryImplementor)UnitOfWork.CurrentSessionFactory; }
        }

        /// <summary>
        /// Current UnitOfWork의 <see cref="ISessionFactory"/> 입니다.
        /// </summary>
        protected virtual ISessionFactory SessionFactory {
            get { return UnitOfWork.CurrentSessionFactory; }
        }

        /// <summary>
        /// UnitOfWork에서 활성화된 NHibernate Session 
        /// </summary>
        protected virtual ISession Session {
            get { return UnitOfWork.CurrentSession; }
        }

        /// <summary>
        /// UnitOfWork에서 활성화된 NHibernate SessionImplementor
        /// </summary>
        protected virtual ISessionImplementor SessionImpl {
            get { return (ISessionImplementor)UnitOfWork.CurrentSession; }
        }
    }
}