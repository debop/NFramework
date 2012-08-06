using System;
using System.Threading;

namespace NSoft.NFramework.Threading {
    /// <summary>
    /// Worker Thread 의 기본 클래스
    /// </summary>
    [Serializable]
    public abstract class AbstractWorkerThread : AbstractThread {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 현재 Thread에 대해 중지 요청이 왔는지 여부
        /// </summary>
        private volatile bool _isShutdownRequested;

        private TimeSpan _workInterval = TimeSpan.FromMilliseconds(10);

        /// <summary>
        /// 중지 요청 여부
        /// </summary>
        public bool IsShutdownRequested {
            get {
                lock(this)
                    return _isShutdownRequested;
            }
        }

        public TimeSpan WorkInterval {
            get { return _workInterval; }
            set { _workInterval = value; }
        }

        /// <summary>
        /// 생성자
        /// </summary>
        protected AbstractWorkerThread() {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="name">스레드 이름</param>
        protected AbstractWorkerThread(string name) : base(name) {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="maxStackSize">스레드가 사용할 최대 Stack 크기</param>
        protected AbstractWorkerThread(int maxStackSize) : base(maxStackSize) {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="name">스레드 이름</param>
        /// <param name="maxStackSize">스레드가 사용할 최대 Stack 크기</param>
        protected AbstractWorkerThread(string name, int maxStackSize) : base(name, maxStackSize) {}

        /// <summary>
        /// InternalThread 중지 요청시
        /// </summary>
        public event EventHandler<WorkerThreadEventArgs> ShutDownRequested = delegate { };

        /// <summary>
        /// 스레드가 종료되었을 시 호출되는 함수. 내부적으로 <see cref="AbstractWorkerThread.ShutDownRequested"/> 이벤트를 호출한다.
        /// </summary>
        [Obsolete("OnShutDownRequested()를 사용하세요")]
        protected virtual void OnShutDowned() {
            ShutDownRequested(this, new WorkerThreadEventArgs(InternalThread));
        }

        /// <summary>
        /// 스레드가 종료되었을 시 호출되는 함수. 내부적으로 <see cref="AbstractWorkerThread.ShutDownRequested"/> 이벤트를 호출한다.
        /// </summary>
        protected virtual void OnShutDownRequested() {
            ShutDownRequested(this, new WorkerThreadEventArgs(InternalThread));
        }

        /// <summary>
        /// InternalThread 중지를 요청합니다.
        /// </summary>
        public void ShutDown() {
            if(IsDebugEnabled)
                log.Debug("Request shutdown current thread.");

            lock(this) {
                if(_isShutdownRequested == false) {
                    _isShutdownRequested = true;

                    // 현 Thread를 Interrupt 합니다.
                    // InternalThread 가 sleep 상태에 있다 하더라도 InternalThread 중지를 알립니다.
                    if(InternalThread != null)
                        InternalThread.Interrupt();

                    OnShutDownRequested();
                }
            }

            if(IsDebugEnabled)
                log.Debug("Shutdown current thread is SUCCESS!!!");
        }

        /// <summary>
        /// Two-phase termination이 가능하도록 내부에서 항상 shutdown요청을 감시하면서 단위작업을 수행한다.
        /// <see cref="ThreadInterruptedException"/>이 발생해도 내부적으로 무시하고 진행한다.
        /// </summary>
        public override void Run() {
            if(IsDebugEnabled)
                log.Debug("Run the worker thread.");

            OnStarted();

            try {
                // 중지가 요청될 때까지 단위 작업을 호출합니다.
                while(IsShutdownRequested == false) {
                    DoWork();
                    Thread.Sleep(WorkInterval);
                }
            }
            catch(ThreadInterruptedException tie) {
                if(log.IsInfoEnabled)
                    log.InfoException("Worker thread is interrupted. It's just information.", tie);
            }
            finally {
                DoShutDown();
            }

            OnFinished();

            if(IsDebugEnabled)
                log.Debug("Current worker thread is finished.");
        }

        /// <summary>
        /// InternalThread 내에서 작업하고자 하는 코드를 구현한다.
        /// <see cref="AbstractWorkerThread"/>.RunEach() 메소드에서 Shutdown 요청이 들어오기 전까지는 반복적으로 DoWork 메소드를 호출하므로,
        /// 실제 작업하고자 하는 단위 작업만 작성하면 됩니다.
        /// </summary>
        protected abstract void DoWork();

        /// <summary>
        /// 종료 요청시 뒷 정리를 담당하는 함수입니다.
        /// 뒷 정리에 관련된 내용을 구현해야 합니다.
        /// </summary>
        protected abstract void DoShutDown();

        /// <summary>
        /// 스레드를 중지시킵니다.
        /// </summary>
        public new void Interrupt() {
            if(IsDebugEnabled)
                log.Debug("Interrupt current worker therad...");

            ShutDown();
        }
    }
}