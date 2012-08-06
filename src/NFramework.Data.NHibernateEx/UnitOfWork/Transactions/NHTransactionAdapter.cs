using System;
using NHibernate;

namespace NSoft.NFramework.Data.NHibernateEx {
    /// <summary>
    /// NHibernate용 Transaction 처리를 구현한 Class
    /// </summary>
    public class NHTransactionAdapter : IUnitOfWorkTransaction {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private ITransaction _transaction;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="transaction">Transaction 인스턴스</param>
        public NHTransactionAdapter(ITransaction transaction) {
            transaction.ShouldNotBeNull("transaction");

            _transaction = transaction;
        }

        /// <summary>
        /// Commit transaction
        /// </summary>
        public void Commit() {
            if(IsDebugEnabled)
                log.Debug("Flush the associated ISession and end the unit of work...");

            _transaction.Commit();

            if(IsDebugEnabled)
                log.Debug("Transaction Commit is SUCCESS.");
        }

        /// <summary>
        /// Rollback transaction
        /// </summary>
        public void Rollback() {
            if(IsDebugEnabled)
                log.Debug("Force the underlying transaction to roll back.");

            _transaction.Rollback();

            if(log.IsWarnEnabled)
                log.Warn("Transaction Rollback!!!");
        }

        #region << IDisposable >>

        /// <summary>
        /// 현재 인스턴스 리소스 해제 여부
        /// </summary>
        public bool IsDisposed { get; protected set; }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~NHTransactionAdapter() {
            Dispose(false);
        }

        ///<summary>
        /// Release Transaction.
        ///</summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Release resource
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing) {
            if(IsDisposed)
                return;

            if(disposing) {
                With.TryAction(() => {
                                   if(_transaction != null) {
                                       _transaction.Dispose();
                                       _transaction = null;
                                   }
                               });

                if(IsDebugEnabled)
                    log.Debug("NHibernatNHTransactionAdapter가 Dispose 되었습니다.");
            }

            IsDisposed = true;
        }

        #endregion
    }
}