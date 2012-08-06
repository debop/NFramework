using System;

namespace NSoft.NFramework.Data.NHibernateEx {
    /// <summary>
    /// Unit Of Work Pattern을 구현한 Interface
    /// </summary>
    public interface IUnitOfWork : IDisposable {
        /// <summary>
        /// Current Session의 변경 내용을 Flush를 통해 Database에 적용한다.
        /// </summary>
        void Flush();

        /// <summary>
        /// Current Session의 내용을 모두 소거합니다.
        /// </summary>
        void Clear();

        /// <summary>
        /// 현재 Unit Of Work에 Transaction이 활성화되어 있는지 나타낸다.
        /// </summary>
        bool IsInActiveTransaction { get; }

        /// <summary>
        /// Transaction을 시작합니다.
        /// </summary>
        /// <returns></returns>
        IUnitOfWorkTransaction BeginTransaction();

        /// <summary>
        /// 지정된 <see cref="System.Data.IsolationLevel"/>로 Transaction을 시작합니다.
        /// </summary>
        /// <param name="isolationLevel">격리 수준</param>
        /// <returns>Transactio 객체</returns>
        IUnitOfWorkTransaction BeginTransaction(System.Data.IsolationLevel isolationLevel);

        /// <summary>
        /// Current Session의 변경 내용을 Transaction을 적용하여 Flush를 수행한다.
        /// </summary>
        void TransactionalFlush();

        /// <summary>
        /// Current Session의 변경 내용을 Transaction을 적용하여 Flush를 수행한다.
        /// </summary>
        /// <param name="isolationLevel">격리수준</param>
        void TransactionalFlush(System.Data.IsolationLevel isolationLevel);
    }
}