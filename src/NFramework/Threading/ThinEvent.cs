using System;
using System.Threading;

namespace NSoft.NFramework.Threading {
    /// <summary>
    /// 경량의 Event 객체를 표현합니다. Set, Reset, Wait를 사용합니다.
    /// </summary>
    /// <remarks>
    /// 참고: http://msdn.microsoft.com/ko-kr/magazine/cc163427.aspx
    /// </remarks>
    [Serializable]
    public struct ThinEvent {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private const int SET = 1;
        private const int UN_SET = 0;

        private int _state;

        private EventWaitHandle _eventObj;

        private const int SpinCount = 4000;

        /// <summary>
        /// Set Event State
        /// </summary>
        public void Set() {
            if(IsDebugEnabled)
                log.Debug("Set event...");

            _state = SET;
            Thread.MemoryBarrier();

            if(_eventObj != null)
                _eventObj.Set();
        }

        /// <summary>
        /// Reset Event	State
        /// </summary>
        public void Reset() {
            if(IsDebugEnabled)
                log.Debug("Reset event...");

            _state = UN_SET;

            if(_eventObj != null)
                _eventObj.Reset();
        }

        /// <summary>
        /// Wait Event State to Set 
        /// </summary>
        public void Wait() {
            if(IsDebugEnabled)
                log.Debug("Wait event state to SET...");

            var spinWait = new System.Threading.SpinWait();
            var spinCount = 0;

            while(_state == UN_SET) {
                if(spinCount++ < SpinCount) {
                    spinWait.SpinOnce();

                    if(_eventObj == null) {
                        var mre = new ManualResetEvent(_state == SET);

                        if(Interlocked.CompareExchange(ref _eventObj, mre, null) != null) {
                            // If someone set the flag before seeing the new event object, we must ensure it's been set.
                            if(_state == SET)
                                _eventObj.Set();
                                // Lost the race w/ another thread. Just use its event.
                            else
                                mre.Close();
                        }
                        if(_eventObj != null)
                            _eventObj.WaitOne();
                    }
                }
            }
        }
    }
}