using System;
using System.Threading;

namespace NSoft.NFramework.Parallelism.DataStructures {
    /// <summary>
    /// <see cref="SpinLock"/> struct를 단순히 Wrapping한 클래스입니다.
    /// </summary>
    [Serializable]
    public class SpinLockClass {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private SpinLock _spinLock;

        /// <summary>
        /// 기본 생성자
        /// </summary>
        public SpinLockClass() {
            _spinLock = new SpinLock();
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="enableThreadOwnerTracking">디버깅을 위해 threadID를 Tracking 할 것인지 여부</param>
        public SpinLockClass(bool enableThreadOwnerTracking) {
            _spinLock = new SpinLock(enableThreadOwnerTracking);
        }

        /// <summary>
        /// 지정된 delegate을 lock을 건 상태에서 실행합니다.
        /// </summary>
        /// <param name="runUnderLock">실행할 delegate</param>
        public void Execute(Action runUnderLock) {
            runUnderLock.ShouldNotBeNull("runUnderLock");

            var lockTaken = false;

            try {
                Enter(ref lockTaken);
                runUnderLock();
            }
            finally {
                if(lockTaken)
                    Exit();
            }
        }

        /// <summary>
        /// Enters the lock. <paramref name="lockTaken"/> 은 입력 시에는 항상 False 값을 가져야 합니다.
        /// </summary>
        /// <param name="lockTaken">
        /// Upon exit of the Enter method, specifies whether the lock was acquired. 
        /// The variable passed by reference must be initialized to false.
        /// </param>
        public void Enter(ref bool lockTaken) {
            _spinLock.Enter(ref lockTaken);

            if(IsDebugEnabled)
                log.Debug("SpinLock Entered... lockTaken=[{0}]", lockTaken);
        }

        /// <summary>
        /// Exit wrapped SpinLock
        /// </summary>
        public void Exit() {
            if(IsDebugEnabled)
                log.Debug("SpinLock Exit...");

            _spinLock.Exit();
        }

        /// <summary>
        /// Exit wrapped SpinLock
        /// </summary>
        /// <param name="useMemoryBarrier">
        /// A Boolean value that indicates whether a memory fence should be issued in
        /// order to immediately publish the exit operation to other threads.
        /// </param>
        public void Exit(bool useMemoryBarrier) {
            _spinLock.Exit(useMemoryBarrier);
        }
    }
}