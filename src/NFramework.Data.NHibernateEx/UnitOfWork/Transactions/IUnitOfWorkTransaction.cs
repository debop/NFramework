using System;

namespace NSoft.NFramework.Data.NHibernateEx {
    /// <summary>
    /// Unit Of Work Pattern에서 Transaction 처리를 표현한 Interface
    /// </summary>
    public interface IUnitOfWorkTransaction : IDisposable {
        /// <summary>
        /// Commit transaction
        /// </summary>
        void Commit();

        /// <summary>
        /// Rollback transaction
        /// </summary>
        void Rollback();
    }
}