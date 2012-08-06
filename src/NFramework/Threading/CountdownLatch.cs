using System;
using System.Threading;

namespace NSoft.NFramework.Threading {
    /// <summary>
    /// Master Thread에 소속된 Sub Thread들의 모두 작업이 완료될 때까지 Master Thread를 기다리게 하는 작업을 수행한다.
    /// </summary>
    /// <remarks>
    /// 참고: http://msdn.microsoft.com/ko-kr/magazine/cc163427.aspx
    /// </remarks>
    /// <example>
    /// <code>
    /// // wait 5 events with 5 millisecond timeout.
    /// using(CountdownLatch countdown = new CountdownLatch(5))
    /// {
    ///		bool result = countdown.WaitOne(TimeSpan.FromMilliseconds(5));
    ///		Assert.IsFalse(result);
    /// }
    /// </code>
    /// </example>
    [Serializable]
    public class CountdownLatch : IDisposable {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private int _numberOfCustomers;
        // private readonly EventWaitHandle _doneWaitingEvent;
#if !SILVERLIGHT
        private ManualResetEventSlim _doneWaitingEvent;
#else
		private ManualResetEvent _doneWaitingEvent;
#endif

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="numberOfCustomers">Sub thread의 수</param>
        public CountdownLatch(int numberOfCustomers = 1) {
            numberOfCustomers.ShouldBePositiveOrZero("numberOfCustomers");

            bool initalState = SetCustomers(numberOfCustomers);

#if !SILVERLIGHT
            _doneWaitingEvent = new ManualResetEventSlim(initalState);
#else
			_doneWaitingEvent = new ManualResetEvent(initalState);
#endif
        }

        /// <summary>
        /// 하나의 작업이 끝나기를 기다린다.
        /// </summary>
        /// <returns></returns>
        public bool WaitOne() {
            if(IsDebugEnabled)
                log.Debug("하나의 작업이 끝나기를 기다립니다...");

#if !SILVERLIGHT
            _doneWaitingEvent.Wait();
            return true;
#else
			return _doneWaitingEvent.WaitOne();
#endif
        }

        /// <summary>
        /// Timeout 기간동안만 하나의 작업이 끝나기를 기다린다. timeout이 되면 false를 반환한다.
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public bool WaitOne(TimeSpan timeout) {
            if(IsDebugEnabled)
                log.Debug("하나의 작업이 끝나기를 기다립니다... timeout=[{0}]", timeout);


#if !SILVERLIGHT
            return _doneWaitingEvent.Wait(timeout, CancellationToken.None);
#else
			return _doneWaitingEvent.WaitOne(timeout);
#endif
        }

        /// <summary>
        /// CountdownLatch가 대기상태이면 계속 진행할 수있도록 Event 상태를 신호 받음으로 설정한다.
        /// </summary>
        /// <returns>대기중인 자식 Thread 수</returns>
        public int Set() {
            var val = Interlocked.Decrement(ref _numberOfCustomers);

            if(val <= 0)
                _doneWaitingEvent.Set();

            return val;
        }

        /// <summary>
        /// 자식 Thread수가 0이면 스레드가 차단되도록 이벤트 상태를 신호 없음으로 설정합니다.
        /// </summary>
        /// <param name="numberOfCustomers">자식 Thread 수</param>
        public void Reset(int numberOfCustomers) {
            if(SetCustomers(numberOfCustomers) == false)
                _doneWaitingEvent.Reset();
        }

        /// <summary>
        /// 기다려야 할 갯수를 설정합니다. 
        /// </summary>
        /// <param name="numberOfCustomers">기다려야할 갯수. 0보다 크거나 같아야합니다.</param>
        /// <returns>기다려야할 갯수가 0이면 True, 아니면 False</returns>
        private bool SetCustomers(int numberOfCustomers) {
            numberOfCustomers.ShouldBePositiveOrZero("numberOfCustomers");

            if(IsDebugEnabled)
                log.Debug("기다려야 할 갯수를 설정합니다. numberOfCustomers=[{0}]", numberOfCustomers);

            _numberOfCustomers = numberOfCustomers;
            return (_numberOfCustomers == 0);
        }

        #region << IDisposable >>

        /// <summary>
        /// 리소스 해제 여부
        /// </summary>
        public bool IsDisposed { get; protected set; }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~CountdownLatch() {
            Dispose(false);
        }

        /// <summary>
        /// Release resources. 
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Release resources.  
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing) {
            if(IsDisposed)
                return;

            if(disposing) {
                if(_doneWaitingEvent != null) {
                    With.TryAction(_doneWaitingEvent.Dispose);
                    _doneWaitingEvent = null;
                }
            }

            IsDisposed = true;
        }

        #endregion
    }
}