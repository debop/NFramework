using System;

namespace NSoft.NFramework.Data.NHibernateEx {
    /// <summary>
    /// IUnitOfWork를 구현한 기본 클래스
    /// </summary>
    public abstract class NHUnitOfWorkAdapterBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 지정된 격리수준으로 Transaction을 생성하고, 이 Transaction하에서 현재 Session을 Flushing 한다.
        /// Transaction이 실패하면, Flush한 작업은 모두 취소된다.
        /// </summary>
        /// <param name="isolationLevel">격리수준</param>
        public virtual void TransactionalFlush(System.Data.IsolationLevel isolationLevel) {
            if(IsDebugEnabled)
                log.Debug("Transactional Session Flushing is starting... IsolationLevel: " + isolationLevel);

            var tx = UnitOfWork.Current.BeginTransaction(isolationLevel);

            try {
                // forces a flush of the current unit of work
                tx.Commit();

                if(IsDebugEnabled)
                    log.Debug("Transaction is committed!!!");
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.ErrorException("Transactional Flush failed!!! transaction rollbacked.", ex);

                if(tx != null)
                    tx.Rollback();

                throw;
            }
            finally {
                if(tx != null)
                    tx.Dispose();
            }
        }

        /// <summary>
        /// Transaction을 생성하고, 이 Transaction하에서 현재 Session을 Flushing 한다.
        /// Transaction이 실패하면, Flush한 작업은 모두 취소된다.
        /// </summary>
        /// <remarks>
        /// 격리수준은 <see cref="System.Data.IsolationLevel.ReadCommitted"/>를 사용한다.
        /// </remarks>
        public void TransactionalFlush() {
            TransactionalFlush(System.Data.IsolationLevel.ReadCommitted);
        }
    }
}