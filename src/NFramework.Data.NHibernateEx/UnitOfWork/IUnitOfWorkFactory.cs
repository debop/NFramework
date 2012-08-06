using System;
using System.Collections;
using System.Data;
using NHibernate;

namespace NSoft.NFramework.Data.NHibernateEx {
    /// <summary>
    /// Factory Interface for IUnitOfWork
    /// </summary>
    public interface IUnitOfWorkFactory {
        /// <summary>
        /// NHibernate configuration filename
        /// </summary>
        string ConfigurationFileName { get; }

        /// <summary>
        /// NHibernate configuration
        /// </summary>
        NHibernate.Cfg.Configuration Configuration { get; }

        /// <summary>
        /// NHibernate SessionFactory
        /// </summary>
        ISessionFactory SessionFactory { get; }

        /// <summary>
        /// Current session	in current thread context
        /// </summary>
        ISession CurrentSession { get; set; }

        /// <summary>
        /// NHibernate HQL 문장을 제공하는 Provider
        /// </summary>
        IIniQueryProvider QueryProvider { get; set; }

        /// <summary>
        /// initialize unit of work factory.
        /// </summary>
        void Init();

        /// <summary>
        /// Create a new unit of work implementation.
        /// </summary>
        /// <param name="maybeUserProvidedConnection">instance of IDbConnection.</param>
        /// <param name="previous">현재 사용중인 IUnitOfWorkImplementor의 인스턴스</param>
        /// <returns>새로 생성한 IUnitOfWorkImplementor의 인스턴스</returns>
        IUnitOfWorkImplementor Create(IDbConnection maybeUserProvidedConnection, IUnitOfWorkImplementor previous);

        /// <summary>
        /// <paramref name="typeOfEntity"/>이 매핑된 Current Session을 반환한다.
        /// </summary>
        /// <param name="typeOfEntity">Type of entity</param>
        /// <returns>지정된 Entity형식이 매핑된 ISession</returns>
        ISession GetCurrentSessionFor(Type typeOfEntity);

        /// <summary>
        /// <typeparamref name="TEntity"/> 수형이 매핑된 Current Session을 반환한다.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        ISession GetCurrentSessionFor<TEntity>();

        /// <summary>
        /// 지정된 factory name을 가진 session factory의 current session을 가져 온다.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        ISession GetCurrentSessionFor(string name);

        /// <summary>
        /// 지정된 Entity 형식을 해당 Session에 매핑시킨다.
        /// </summary>
        /// <param name="typeOfEntity">Type of entity</param>
        /// <param name="session">Instance of ISession to set</param>
        void SetCurrentSession(Type typeOfEntity, ISession session);

        /// <summary>
        /// 지정된 이름의 Sessio을 현재 사용할 Session으로 설정합니다. Dispose() 시에 원래 Session으로 복귀시킵니다.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IDisposable SetCurrentSessionName(string name);

        /// <summary>
        /// Long Conversation 시에 ASP.NET Session에 보관된 IUnitOfWork 인스턴스를 가져와 HttpRequest의 Context에 전달한다.
        /// </summary>
        /// <param name="hashtable">the Hashtable to load the unit of work from</param>
        /// <param name="unitOfWork">retrieved instance of IUnitOfWork</param>
        /// <param name="longConversationId">long conversation id to identifying convesations</param>
        void LoadUnitOfWorkFromHashtable(Hashtable hashtable, out IUnitOfWork unitOfWork, out Guid? longConversationId);

        /// <summary>
        /// Long Conversation 시에 현재 활성화 된 IUnitOfWork 인스턴스를 Hashtable 에 저장한다.
        /// </summary>
        /// <param name="hashtable">the Hashtable to save the unit of work to</param>
        void SaveUnitOfWorkToHashtable(Hashtable hashtable);

        /// <summary>
        /// 지정된 <see cref="IUnitOfWorkImplementor"/>의 인스턴스를 Dispose합니다.
        /// </summary>
        /// <param name="adapter"></param>
        void DisposeUnitOfWork(IUnitOfWorkImplementor adapter);
    }
}