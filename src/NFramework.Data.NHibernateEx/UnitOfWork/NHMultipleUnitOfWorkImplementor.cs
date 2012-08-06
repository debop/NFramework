using System;
using System.Collections.Generic;
using System.Data;
using NHibernate;

namespace NSoft.NFramework.Data.NHibernateEx {
    /// <summary>
    /// Multiple UnitOfWork 
    /// </summary>
    public class NHMultipleUnitOfWorkImplementor : List<IUnitOfWorkImplementor>, IUnitOfWorkImplementor {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 사용횟수를 증가시킵니다.
        /// </summary>
        public void IncrementUsages() {
            if(IsDebugEnabled)
                log.Debug("Increment usage of Multiple UnitOfWork.");

            ForEach(implementor => implementor.IncrementUsages());
        }

        /// <summary>
        /// Unit Of Work 를 중첩 방식으로 사용할 때의 바로 전의 Unit Of Work
        /// </summary>
        public IUnitOfWorkImplementor Previous {
            get {
                var previousImplementors = new NHMultipleUnitOfWorkImplementor();
                ForEach(implementor => previousImplementors.Add(implementor.Previous));

                return previousImplementors;
            }
        }

        /// <summary>
        /// Current ISession of UnitOfWork
        /// </summary>
        public ISession Session {
            get { return this[0].Session; }
        }

        /// <summary>
        /// Current Session의 변경 내용을 Flush를 통해 Database에 적용한다.
        /// </summary>
        public void Flush() {
            if(log.IsDebugEnabled)
                log.Debug("모든 Unit Of Work에게 Flush 를 수행하도록 합니다...");

            ForEach(implementor => implementor.Flush());
        }

        /// <summary>
        /// Current Session의 내용을 모두 소거합니다.
        /// </summary>
        public new void Clear() {
            if(log.IsDebugEnabled)
                log.Debug("모든 Unit Of Work에게 Clear 를 수행하도록 합니다...");

            ForEach(implementor => implementor.Clear());
        }

        /// <summary>
        /// 현재 Unit Of Work에 Transaction이 활성화되어 있는지 나타낸다.
        /// </summary>
        public bool IsInActiveTransaction {
            get {
                if(Count == 0)
                    return false;

                return TrueForAll(implementor => implementor.IsInActiveTransaction);
            }
        }

        /// <summary>
        /// Transaction을 시작합니다.
        /// </summary>
        /// <returns></returns>
        public IUnitOfWorkTransaction BeginTransaction() {
            return BeginTransaction(IsolationLevel.ReadCommitted);
        }

        /// <summary>
        /// 지정된 <see cref="IsolationLevel"/>로 Transaction을 시작합니다.
        /// </summary>
        /// <param name="isolationLevel"></param>
        /// <returns></returns>
        public IUnitOfWorkTransaction BeginTransaction(IsolationLevel isolationLevel) {
            if(IsDebugEnabled)
                log.Debug("BeginTransaction for Multiple UnitOfWork... IsolationLevel=[{0}]", isolationLevel);

            var transaction = new NHMultipleTransaction();
            ForEach(implementor => transaction.Add(implementor.BeginTransaction(isolationLevel)));

            if(IsDebugEnabled)
                log.Debug("BeginTransaction for Multiple UnitOfWork is success!!!");

            return transaction;
        }

        /// <summary>
        /// Current Session의 변경 내용을 Transaction을 적용하여 Flush를 수행한다.
        /// </summary>
        public void TransactionalFlush() {
            if(IsDebugEnabled)
                log.Debug("TransactionalFlush for Multiple UnitOfWork is starting...");

            ForEach(implementor => implementor.TransactionalFlush());

            if(IsDebugEnabled)
                log.Debug("TransactionalFlush for Multiple UnitOfWork is success!!!");
        }

        /// <summary>
        /// Current Session의 변경 내용을 Transaction을 적용하여 Flush를 수행한다.
        /// </summary>
        /// <param name="isolationLevel"></param>
        public void TransactionalFlush(IsolationLevel isolationLevel) {
            if(IsDebugEnabled)
                log.Debug("TransactionalFlush for Multiple UnitOfWork is starting... isolationLevel:" + isolationLevel);

            ForEach(implementor => implementor.TransactionalFlush(isolationLevel));

            if(IsDebugEnabled)
                log.Debug("TransactionalFlush for Multiple UnitOfWork is success!!!");
        }

        #region << IDisposable >>

        /// <summary>
        /// 리소스 해제 여부
        /// </summary>
        public bool IsDisposed { get; protected set; }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~NHMultipleUnitOfWorkImplementor() {
            Dispose(false);
        }

        ///<summary>
        /// Release resources.
        ///</summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Release resources
        /// </summary>
        /// <param name="disposing">release managed resources.</param>
        protected virtual void Dispose(bool disposing) {
            if(IsDisposed)
                return;

            if(disposing && Count > 0) {
                var countOfUnitOfWork = Count;

                if(IsDebugEnabled)
                    log.Debug("Disposing multiple unit of work... Count of UnitOfWork=[{0}]", countOfUnitOfWork);

                With.TryAction(() => {
                                   ForEach(implementor => With.TryAction(implementor.Dispose));

                                   if(Count > 0 && TrueForAll(implementor => implementor.Previous != null))
                                       UnitOfWork.DisposeUnitOfWork(this);
                               });

                if(IsDebugEnabled)
                    log.Debug("MultipleUnitOfWorkImplementor의 내부 IUnitOfWork 들을 Dispose 했습니다!!! 갯수=[{0}]", countOfUnitOfWork);
            }
            IsDisposed = true;
        }

        #endregion
    }
}