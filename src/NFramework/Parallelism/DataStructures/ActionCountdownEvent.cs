using System;
using System.Threading;

namespace NSoft.NFramework.Parallelism.DataStructures {
    /// <summary>
    /// <see cref="DisposableAction"/>과 유사하게, CountdownEvent가 완료되면 (Countdown 수가 0이 되면), 지정한 Action을 수행합니다.
    /// </summary>
    [Serializable]
    public class ActionCountdownEvent : IDisposable {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private CountdownEvent _event;
        private readonly Action _action;
        private readonly ExecutionContext _context;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="initialCount">초기 countdown 수 (0보다 크거나 같아야 합니다)</param>
        /// <param name="action">countdown이 끝나면 (countdown 수가 0가 되면) 수행할 action</param>
        public ActionCountdownEvent(int initialCount, Action action) {
            initialCount.ShouldBePositiveOrZero("initialCount");
            action.ShouldNotBeNull("action");

            _action = action;
            _event = new CountdownEvent(initialCount);

            if(initialCount == 0)
                action();
            else
                _context = ExecutionContext.Capture();
        }

        /// <summary>
        /// Countdown 수를 하나 늘린다.
        /// </summary>
        public void AddCount() {
            _event.AddCount();
        }

        /// <summary>
        /// 현재 count 수
        /// </summary>
        public int CurrentCount {
            get { return _event.CurrentCount; }
        }

        /// <summary>
        /// Countdown 수를 하나 감소시키도록, 신호를 보냅니다. Countdown이 완료되면, 지정한 action을 수행합니다.
        /// </summary>
        public void Signal() {
            if(IsDebugEnabled)
                log.Debug("CountdownEvent에 Signal을 보냅니다.");

            // Countdown에 Signal을 등록하고, countdown 수가 0이 되었다면!!!);
            if(_event.Signal()) {
                if(IsDebugEnabled)
                    log.Debug("Countdown을 모두 마쳤으므로, 후속 Action을 수행합니다...");

                // NOTE: 멀티스레드에서는 현재 인스턴스가 생성된 스레드 컨텍스트에서 수행되도록 한다.
                //
                if(_context != null)
                    ExecutionContext.Run(_context, _ => _action.Invoke(), null);
                else
                    _action();
            }
        }

        #region << IDisposable >>

        /// <summary>
        /// 리소스 해제 여부
        /// </summary>
        protected bool IsDisposed { get; set; }

        ~ActionCountdownEvent() {
            Dispose(false);
        }

        /// <summary>
        /// 리소스 해제
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 리소스 해제
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing) {
            if(IsDisposed)
                return;

            if(disposing) {
                if(_event != null) {
                    _event.Dispose();
                    _event = null;
                }
            }

            IsDisposed = true;
        }

        #endregion
    }
}