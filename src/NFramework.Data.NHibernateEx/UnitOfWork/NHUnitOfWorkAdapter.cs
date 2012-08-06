using System;
using System.Threading;
using NHibernate;

namespace NSoft.NFramework.Data.NHibernateEx {
    /// <summary>
    /// Concrete class for IUnitOfWorkImplementor
    /// </summary>
    public class NHUnitOfWorkAdapter : NHUnitOfWorkAdapterBase, IUnitOfWorkImplementor {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="uowFactory">Factory of UnitOfWork</param>
        /// <param name="session">NHibernate session</param>
        /// <param name="previous">Previos UnitOfWork</param>
        public NHUnitOfWorkAdapter(IUnitOfWorkFactory uowFactory, ISession session, NHUnitOfWorkAdapter previous) {
            uowFactory.ShouldNotBeNull("uowFactory");
            session.ShouldNotBeNull("session");

            _factory = uowFactory;
            _session = session;
            _previous = previous;

            if(IsDebugEnabled)
                log.Debug("IUnitOfWork의 기본 Class인 NHUnitOfWorkAdapter의 인스턴스가 생성되었습니다.");
        }

        private readonly IUnitOfWorkFactory _factory;
        private readonly ISession _session;
        private readonly NHUnitOfWorkAdapter _previous;
        private int _usageCount = 1;

        /// <summary>
        /// Factory for <see cref="NHUnitOfWorkFactory"/>
        /// </summary>
        public IUnitOfWorkFactory Factory {
            get { return _factory; }
        }

        /// <summary>
        /// 현재 활성화한 ISession
        /// </summary>
        public NHibernate.ISession Session {
            get { return _session; }
        }

        /// <summary>
        /// Unit Of Work 를 중첩 방식으로 사용할 때의 바로 전의 Unit Of Work
        /// </summary>
        public NHUnitOfWorkAdapter Previous {
            get { return _previous; }
        }

        /// <summary>
        /// UnitOfWork 사용 횟수를 증가시킨다.
        /// </summary>
        public void IncrementUsages() {
            Interlocked.Increment(ref _usageCount);

            if(IsDebugEnabled)
                log.Debug("Increment usages of NHUnitOfWorkAdapter. usage count: " + _usageCount);
        }

        /// <summary>
        /// Unit Of Work 를 중첩 방식으로 사용할 때의 바로 전의 Unit Of Work
        /// </summary>
        IUnitOfWorkImplementor IUnitOfWorkImplementor.Previous {
            get { return Previous; }
        }

        /// <summary>
        /// Current Session의 변경 내용을 Flush를 통해 Database에 적용한다.
        /// </summary>
        public void Flush() {
            if(IsDebugEnabled)
                log.Debug("Flush session is starting...");

            _session.Flush();

            if(IsDebugEnabled)
                log.Debug("Flush session is success.");
        }

        /// <summary>
        /// Current Session의 내용을 모두 소거합니다.
        /// </summary>
        public void Clear() {
            _session.Clear();
        }

        /// <summary>
        /// 현재 Unit Of Work에 Transaction이 활성화되어 있는지 나타낸다.
        /// </summary>
        public bool IsInActiveTransaction {
            get { return _session.Transaction.IsActive; }
        }

        /// <summary>
        /// Transaction을 시작합니다.
        /// </summary>
        /// <returns></returns>
        public IUnitOfWorkTransaction BeginTransaction() {
            return new NHTransactionAdapter(_session.BeginTransaction());
        }

        /// <summary>
        /// 지정된 <see cref="System.Data.IsolationLevel"/>로 Transaction을 시작합니다.
        /// </summary>
        /// <param name="isolationLevel">격리 수준</param>
        /// <returns>Transactio 객체</returns>
        public IUnitOfWorkTransaction BeginTransaction(System.Data.IsolationLevel isolationLevel) {
            return new NHTransactionAdapter(_session.BeginTransaction(isolationLevel));
        }

        #region << IDisposable >>

        /// <summary>
        /// 리소스 해제 여부
        /// </summary>
        public bool IsDisposed { get; protected set; }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~NHUnitOfWorkAdapter() {
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

            if(disposing) {
                if(IsDebugEnabled)
                    log.Debug("Disposing NHUnitOfWorkAdapter instance...");

                Interlocked.Decrement(ref _usageCount);

                if(IsDebugEnabled)
                    log.Debug("Usage count of NHUnitOfWork : " + _usageCount);

                if(_usageCount != 0) {
                    if(IsDebugEnabled)
                        log.Debug("NHUnitOfWorkAdapter 사용 수가 0이 아니므로 Dispose 하지 않습니다.");

                    return;
                }

                if(_factory != null)
                    With.TryAction(() => _factory.DisposeUnitOfWork(this));

                if(_session != null)
                    With.TryAction(() => _session.Dispose());

                if(IsDebugEnabled)
                    log.Debug("Disposed NHUnitOfWorkAdapter instance!!!");
            }

            IsDisposed = true;
        }

        #endregion
    }
}