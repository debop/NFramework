using System;
using System.Threading;

namespace NSoft.NFramework {
    /// <summary>
    /// <see cref="IAsyncResult"/>를 표현하기 위한 기본 클래스입니다.
    /// </summary>
    [Serializable]
    public class AbstractAsyncResult : IAsyncResult {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private readonly ManualResetEventSlim _asyncWaitHandler;

        protected AbstractAsyncResult(object asyncState) {
            AsyncState = asyncState;
            _asyncWaitHandler = new ManualResetEventSlim(false);

            if(IsDebugEnabled)
                log.Debug("AsyncResult 인스턴스가 생성되었습니다. asyncState=[{0}]", asyncState);
        }

        ~AbstractAsyncResult() {
            if(_asyncWaitHandler != null)
                _asyncWaitHandler.Dispose();
        }

        #region << IAsyncResult >>

        /// <summary>
        /// 비동기 작업의 성공여부
        /// </summary>
        public virtual bool IsCompleted { get; protected set; }

        /// <summary>
        /// 비동기 작업의 대기 핸들
        /// </summary>
        public virtual WaitHandle AsyncWaitHandle {
            get { return _asyncWaitHandler.WaitHandle; }
        }

        public virtual object AsyncState { get; private set; }

        public virtual bool CompletedSynchronously {
            get { return false; }
        }

        #endregion

        /// <summary>
        /// 비동기 작업의 실패 여부
        /// </summary>
        public virtual bool IsFaulted { get; protected set; }

        /// <summary>
        /// 비동기 작업의 취소 여부
        /// </summary>
        public virtual bool IsCanceled { get; protected set; }

        /// <summary>
        /// 비동기 작업시 발생한 예외 정보
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// 비동기 작업이 성공적으로 완료되었다고 설정합니다.
        /// </summary>
        public void Success() {
            if(IsDebugEnabled)
                log.Debug("비동기 작업이 성공했습니다!!!");

            IsCompleted = true;
            Complete();
        }

        /// <summary>
        /// 비동기 작업을 실패로 설정한다.
        /// </summary>
        /// <param name="exception"></param>
        public void Fail(Exception exception) {
            if(IsDebugEnabled) {
                log.Debug("비동기 작업을 실패로 설정합니다.");
                log.Debug(exception);
            }

            Exception = exception;
            IsFaulted = true;

            Complete();
        }

        /// <summary>
        /// 비동기 작업이 취소되었다고 설정합니다.
        /// </summary>
        public virtual void Cancel() {
            if(IsDebugEnabled)
                log.Debug("비동기 작업이 취소되었습니다.");

            IsCanceled = true;
            Complete();
        }

        protected virtual void Complete() {
            _asyncWaitHandler.Set();
        }

        /// <summary>
        /// 예외가 발생하여, 실퍠로 설정되었다면, 그 예외를 rethrow 합니다.
        /// </summary>
        public void CheckResult() {
            if(Exception != null)
                throw Exception;
        }
    }
}