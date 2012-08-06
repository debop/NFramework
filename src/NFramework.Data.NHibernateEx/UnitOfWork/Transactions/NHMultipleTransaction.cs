using System.Collections.Generic;

namespace NSoft.NFramework.Data.NHibernateEx {
    /// <summary>
    /// 복수의 UnitOfWork 에 대한 Transaction을 처리하는 Class
    /// </summary>
    public class NHMultipleTransaction : List<IUnitOfWorkTransaction>, IUnitOfWorkTransaction {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// commit transaction
        /// </summary>
        public void Commit() {
            if(IsDebugEnabled)
                log.Debug("Commit transactions in MultipleTransaction is starting...");

            ForEach(transaction => transaction.Commit());

            if(IsDebugEnabled)
                log.Debug("Commit transactions in MultipleTransaction is success!!!");
        }

        /// <summary>
        /// Rollback transaction
        /// </summary>
        public void Rollback() {
            if(IsDebugEnabled)
                log.Debug("Rollback transactions in MultipleTransaction is starting...");

            ForEach(transaction => transaction.Rollback());

            if(IsDebugEnabled)
                log.Debug("Rollback transactions in MultipleTransaction is success!!!");
        }

        /// <summary>
        /// 관리되지 않는 리소스의 확보, 해제 또는 다시 설정과 관련된 응용 프로그램 정의 작업을 수행합니다.
        /// </summary>
        public void Dispose() {
            if(IsDebugEnabled)
                log.Debug("NHibernate Multiple Transaction을 Dispose 합니다.");

            ForEach(delegate(IUnitOfWorkTransaction transaction) {
                        if(transaction != null)
                            transaction.Dispose();
                        transaction = null;
                    });
            Clear();

            if(IsDebugEnabled)
                log.Debug("Dispose MultipleTransaction is success.");
        }
    }
}