using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Threading {
    /// <summary>
    /// <see cref="InternalThread"/>를 Wrapping 한 Abstract InternalThread Class 입니다.
    /// </summary>
    /// <remarks>
    /// 쓰레드 진행과 관련된 event 들을 추가하여, 스레드 내부 상황을 알 수 있도록 했다.
    /// </remarks>
    [Serializable]
    public abstract class AbstractThread {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// <see cref="System.Threading.Monitor"/>.Wait 를 이용하여 Sleep 함수를 구현한 것이다. 앞으로는 <see cref="ThreadTool.Sleep"/> 함수를 이용하세요
        /// </summary>
        /// <param name="millisecondsTimeout">timeout</param>
        /// <seealso cref="ThreadTool.Sleep"/>
        public static void Sleep(int millisecondsTimeout) {
            if(IsDebugEnabled)
                log.Debug("Sleep current thread with millisecondsTimeout=[{0}](msec)", millisecondsTimeout);

            var obj = new object();

            lock(obj) {
                Monitor.Wait(obj, millisecondsTimeout);
            }
        }

        /// <summary>
        /// 생성자
        /// </summary>
        protected AbstractThread() {
            InternalThread = new Thread(Run);
        }

        /// <summary>
        /// Initialize a new instance of AbstractThread with thread name.
        /// </summary>
        /// <param name="name">쓰레드 이름</param>
        protected AbstractThread(string name) : this() {
            InternalThread.Name = name;
        }

        /// <summary>
        /// Initialize a new instance of AbstractThread with max stack size.
        /// </summary>
        /// <param name="maxStackSize">최대 스택 크기</param>
        protected AbstractThread(int maxStackSize) {
            InternalThread = new Thread(Run, maxStackSize);
        }

        /// <summary>
        /// Initialize a new instance of AbstractThread with thread name and max stack size
        /// </summary>
        /// <param name="name">쓰레드 명</param>
        /// <param name="maxStackSize">최대 스택 크기</param>
        protected AbstractThread(string name, int maxStackSize) : this(maxStackSize) {
            InternalThread.Name = name;
        }

        /// <summary>
        /// Wrapping 한 <c>InternalThread</c> 인스턴스
        /// </summary>
        public Thread InternalThread { get; private set; }

        /// <summary>
        /// InternalThread 시작시 발생하는 이벤트
        /// </summary>
        public event EventHandler<WorkerThreadEventArgs> ThreadStarted = delegate { };

        /// <summary>
        /// InternalThread 완료시 발생하는 이벤트
        /// </summary>
        public event EventHandler<WorkerThreadEventArgs> ThreadFinished = delegate { };

        /// <summary>
        ///  InternalThread 가 수행될 때, 진행상황을 알 수있도록 하는 이벤트
        /// </summary>
        public event EventHandler<WorkerProgressChangedEventArgs> ThreadProgressChanged = delegate { };

        /// <summary>
        /// 쓰레드 시작시 호출된다.
        /// </summary>
        protected virtual void OnStarted() {
            if(IsDebugEnabled)
                log.Debug("Raise ThreadStarted event...");

            ThreadStarted(this, new WorkerThreadEventArgs(InternalThread));
        }

        /// <summary>
        /// 쓰레드의 진행 상황이 변경되었을 시에 발생한다.
        /// </summary>
        /// <param name="progressPercentage">진행 정보</param>
        protected virtual void OnProgressChanged(int progressPercentage) {
            if(IsDebugEnabled)
                log.Debug("Raise ThreadProgressChanged event...");

            ThreadProgressChanged(this, new WorkerProgressChangedEventArgs(InternalThread, progressPercentage));
        }

        /// <summary>
        /// 쓰레드가 완료되었을 시에 발생한다.
        /// </summary>
        protected virtual void OnFinished() {
            if(IsDebugEnabled)
                log.Debug("Raise ThreadFinished event...");

            ThreadFinished(this, new WorkerThreadEventArgs(InternalThread));
        }

        /// <summary>
        /// ThreadStart Delegate용 함수
        /// 이 함수를 재정의하여 사용한다.
        /// </summary>
        public abstract void Run();

        /// <summary>
        /// Wapping 한 <c>InternalThread</c> 인스턴스로 캐스팅하여 가져오게 하기 위해 사용
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static explicit operator Thread(AbstractThread m) {
            return m.InternalThread;
        }

        #region Wrapped Properties

        /// <summary>
        ///  이 스레드의 아파트 상태를 가져오거나 설정합니다.
        /// </summary>
        /// <value><c>System.Threading.ApartmentState</c> 값 중 하나입니다. 초기 값은 Unknown입니다.</value>
        /// <exception cref="System.ArgumentException">이 속성을 유효하지 않은 아파트 상태, 즉 단일 스레드 아파트(STA)나 다중 스레드 아파트(MTA) 이외의 아파트 상태로 설정하려고 시도한 경우</exception>
        public ApartmentState ApartmentState {
            get { return InternalThread.GetApartmentState(); }
            set { InternalThread.SetApartmentState(value); }
        }

        /// <summary>
        /// 현재 스레드에 대한 culture를 가져오거나 설정합니다.
        /// </summary>
        /// <value>현재 스레드에 대한 culture를 나타내는 System.Globalization.CultureInfo입니다.</value>
        public CultureInfo CurrentCulture {
            get { return InternalThread.CurrentCulture; }
            set { InternalThread.CurrentCulture = value; }
        }

        /// <summary>
        /// 리소스 관리자가 런타임에 culture 관련 리소스를 찾기 위해 사용하는 현재 culture를 가져오거나 설정합니다.
        /// </summary>
        /// <value>현재 culture를 나타내는 System.Globalization.CultureInfo입니다.</value>
        /// <exception cref="ArgumentNullException">속성 값이 null인 경우</exception>
        public CultureInfo CurrentUICulture {
            get { return InternalThread.CurrentUICulture; }
            set { InternalThread.CurrentUICulture = value; }
        }

        /// <summary>
        /// 현재 스레드의 다양한 컨텍스트 정보를 포함하는 System.Threading.ExecutionContext 개체를 가져옵니다.
        /// </summary>
        /// <value>
        /// 현재 스레드의 컨텍스트 정보를 통합하는 System.Threading.ExecutionContext 개체입니다.
        /// </value>
        public ExecutionContext ExecutionContext {
            get { return InternalThread.ExecutionContext; }
        }

        /// <summary>
        /// 현재 스레드의 실행 상태를 나타내는 값을 가져옵니다.
        /// </summary>
        /// <value>이 스레드가 시작되었으며 정상적으로 종료 또는 중단되지 않았으면 true이고, 그렇지 않으면 false입니다.</value>
        public bool IsAlive {
            get { return InternalThread.IsAlive; }
        }

        /// <summary>
        /// 스레드가 배경 스레드인지 여부를 나타내는 값을 가져오거나 설정합니다.
        /// </summary>
        /// <value>이 스레드가 배경 스레드이거나 배경 스레드가 될 예정이면 true이고, 그렇지 않으면 false입니다.</value>
        /// <exception cref="System.Threading.ThreadStateException">스레드가 비활성화된 경우</exception>
        public bool IsBackground {
            get { return InternalThread.IsBackground; }
            set { InternalThread.IsBackground = value; }
        }

        /// <summary>
        /// 스레드가 관리되는 스레드 풀에 속하는지 여부를 나타내는 값을 가져옵니다.
        /// </summary>
        /// <value>이 스레드가 관리되는 스레드 풀에 속하면 true이고, 그렇지 않으면 false입니다.</value>
        public bool IsThreadPoolThread {
            get { return InternalThread.IsThreadPoolThread; }
        }

        /// <summary>
        /// 현재 관리되는 스레드의 고유 식별자를 가져옵니다.
        /// </summary>
        /// <value>이 스레드의 고유 식별자를 나타내는 정수입니다.</value>
        public int ManagedThreadId {
            get { return InternalThread.ManagedThreadId; }
        }

        /// <summary>
        /// 스레드의 이름을 가져오거나 설정합니다.
        /// </summary>
        /// <value>스레드의 이름을 포함하는 문자열이며 설정된 이름이 없으면 null입니다.</value>
        public string Name {
            get { return InternalThread.Name; }
            set {
                if(InternalThread.Name.IsEmpty() && value.IsNotEmpty())
                    InternalThread.Name = value;
            }
        }

        /// <summary>
        /// 스레드의 예약 우선 순위를 나타내는 값을 가져오거나 설정합니다.
        /// </summary>
        /// <value>
        ///	<c>System.Threading.ThreadPriority</c> 값 중 하나입니다. 기본값은 Normal입니다.
        /// </value>
        /// <exception cref="System.Threading.ThreadStateException">
        /// 스레드가 System.Threading.ThreadState.Aborted와 같은 최종 상태에 도달한 경우
        /// </exception>
        /// <exception cref="ArgumentException">
        /// set 작업에 대해 지정된 값이 유효한 ThreadPriority 값이 아닌 경우
        /// </exception>
        public ThreadPriority Priority {
            get { return InternalThread.Priority; }
            set { InternalThread.Priority = value; }
        }

        /// <summary>
        /// 현재 스레드의 상태를 포함하는 값을 가져옵니다.
        /// </summary>
        /// <value>
        /// 현재 스레드의 상태를 나타내는 System.Threading.ThreadState 값 중 하나입니다. 초기 값은 Unstarted입니다.
        /// </value>
        public ThreadState ThreadState {
            get { return InternalThread.ThreadState; }
        }

        #endregion

        #region Wrapped Methods

        /// <summary>
        ///  이 메서드가 호출되는 스레드에서 System.Threading.ThreadAbortException을 발생시켜
        ///  스레드 종료 프로세스를 시작합니다. 이 메서드를 호출하면 대개 스레드가 종료됩니다.
        /// </summary>
        /// <exception cref="System.Security.SecurityException">호출자에게 필요한 권한이 없는 경우</exception>
        /// <exception cref="ThreadStateException">중단 중인 스레드가 현재 일시 중단된 경우</exception>
        public void Abort() {
            if(IsDebugEnabled)
                log.Debug("Abort thread...");

            InternalThread.Abort();
        }

        /// <summary>
        /// 이 메서드가 호출되는 스레드에서 System.Threading.ThreadAbortException을 발생시켜 스레드 종료 프로세스를
        /// 시작하고, 스레드 종료에 대한 예외 정보를 제공합니다. 이 메서드를 호출하면 대개 스레드가 종료됩니다.
        /// </summary>
        /// 
        /// <param name="stateInfo">상태와 같이 중단 중인 스레드에서 사용할 수 있는 응용 프로그램 관련 정보를 포함하는 개체입니다.</param>
        /// <exception cref="System.Security.SecurityException">
        /// 호출자에게 필요한 권한이 없는 경우
        /// </exception>
        /// <exception cref="ThreadStateException">
        /// 중단 중인 스레드가 현재 일시 중단된 경우
        /// </exception>
        public void Abort(object stateInfo) {
            if(IsDebugEnabled)
                log.Debug("Abort thread with stateInfo=[{0}]", stateInfo);

            InternalThread.Abort(stateInfo);
        }

        /// <summary>
        /// 아파트 상태를 나타내는 <see cref="System.Threading.ApartmentState"/> 값을 반환합니다.
        /// </summary>
        /// <returns>
        /// 관리되는 스레드의 아파트 상태를 나타내는 System.Threading.ApartmentState 값 중 하나입니다.
        /// 기본값은 System.Threading.ApartmentState.Unknown입니다.
        /// </returns>
        public ApartmentState GetApartmentState() {
            return InternalThread.GetApartmentState();
        }

        /// <summary>
        /// 현재 스레드의 해시 코드를 반환합니다.
        /// </summary>
        /// <returns>정수 해시 코드 값입니다.</returns>
        [ComVisible(false)]
        public override int GetHashCode() {
            return InternalThread.GetHashCode();
        }

        /// <summary>
        /// WaitSleepJoin 스레드 상태에 있는 스레드를 중단합니다.
        /// </summary>
        /// <exception cref="System.Security.SecurityException">
        /// 호출자에게 해당 System.Security.Permissions.SecurityPermission이 없는 경우
        /// </exception>
        public virtual void Interrupt() {
            if(IsDebugEnabled)
                log.Debug("Interrupt thread...");

            InternalThread.Interrupt();
        }

        /// <summary>
        /// 표준 COM 및 SendMessage 펌프를 계속 수행하면서 스레드가 종료될 때까지 호출 스레드를 차단합니다.
        /// </summary>
        /// <exception cref="ThreadStateException">호출자가 System.Threading.ThreadState.Unstarted 상태의 스레드에 참여하려고 시도한 경우</exception>
        /// <exception cref="ThreadInterruptedException">대기 중에 스레드가 중단된 경우</exception>
        public void Join() {
            if(IsDebugEnabled)
                log.Debug("Join thread...");

            InternalThread.Join();
        }

        /// <summary>
        /// 표준 COM 및 SendMessage 펌프를 계속 수행하면서 스레드가 종료되거나 지정된 시간이 경과할 때까지 호출 스레드를 차단합니다.
        /// </summary>
        /// <param name="millisecondsTimeout">스레드가 종료되기를 기다릴 밀리초 수입니다.</param>
        /// <returns>
        /// 스레드가 종료되면 true이고, 
        /// millisecondsTimeout 매개 변수에서 지정한 기간이 경과된 후에도 스레드가 종료되지 않으면 false입니다.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///  millisecondsTimeout 값이 음수이고 System.Threading.Timeout.Infinite(밀리초)와 같지 않은 경우
        /// </exception>
        /// <exception cref="ThreadStateException">스레드가 시작되지 않은 경우</exception>
        public bool Join(int millisecondsTimeout) {
            if(IsDebugEnabled)
                log.Debug("Join internal thread with timeout. millisecondsTimeout=[{0}](msec)", millisecondsTimeout);

            return InternalThread.Join(millisecondsTimeout);
        }

        /// <summary>
        /// 표준 COM 및 SendMessage 펌프를 계속 수행하면서 스레드가 종료되거나 지정된 시간이 경과할 때까지 호출 스레드를 차단합니다.
        /// </summary>
        /// <param name="timeout">스레드가 종료되기를 기다릴 시간으로 설정된 <see cref="System.TimeSpan"/></param>
        /// <returns>
        /// 스레드가 종료되면 true이고, 
        /// millisecondsTimeout 매개 변수에서 지정한 기간이 경과된 후에도 스레드가 종료되지 않으면 false입니다.
        /// </returns>
        /// <exception cref="ThreadStateException">
        /// 호출자가 System.Threading.ThreadState.Unstarted 상태의 스레드에 참여하려고 시도한 경우
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///  timeout 값이 음수이고 System.Threading.Timeout.Infinite(밀리초)와 같지 않거나 System.Int32.MaxValue(밀리초)보다 큰 경우
        /// </exception>
        public bool Join(TimeSpan timeout) {
            if(IsDebugEnabled)
                log.Debug("Join internal thread with timeout. timeout=[{0}]", timeout);

            return InternalThread.Join(timeout);
        }

        /// <summary>
        /// 스레드를 시작하기 전에 스레드의 아파트 상태를 설정합니다.
        /// </summary>
        /// <param name="state"> 새 아파트 상태입니다.</param>
        /// <exception cref="InvalidOperationException">
        /// 아파트의 상태가 이미 초기화된 경우
        /// </exception>
        /// <exception cref="ThreadStateException">스레드가 이미 시작된 경우</exception>
        /// <exception cref="ArgumentException">state가 잘못된 아파트 상태인 경우</exception>
        public void SetApartmentState(ApartmentState state) {
            InternalThread.SetApartmentState(state);
        }

        /// <summary>
        /// 운영 체제에서 현재 인스턴스의 상태를 System.Threading.ThreadState.Running으로 변경하도록 합니다.
        /// </summary>
        /// <exception cref="ThreadStateException">스레드가 이미 시작된 경우</exception>
        /// <exception cref="System.OutOfMemoryException"> 이 스레드를 시작할 충분한 메모리가 없는 경우</exception>
        /// <exception cref="System.Security.SecurityException">호출자에게 해당 System.Security.Permissions.SecurityPermission이 없는 경우</exception>
        public void Start() {
            if(IsDebugEnabled)
                log.Debug("Start the internal thread...");

            InternalThread.Start();
        }

        /// <summary>
        /// 운영 체제에서 현재 인스턴스의 상태를 System.Threading.ThreadState.Running으로 변경하도록 하며 경우에
        /// 따라 스레드가 실행하는 메서드에 사용될 데이터가 들어 있는 개체를 제공합니다.
        /// </summary>
        /// <param name="parameter">스레드가 실행하는 메서드에 사용될 데이터가 들어 있는 개체입니다.</param>
        /// <exception cref="ThreadStateException">스레드가 이미 시작된 경우</exception>
        /// <exception cref="System.OutOfMemoryException"> 이 스레드를 시작할 충분한 메모리가 없는 경우</exception>
        /// <exception cref="System.Security.SecurityException">호출자에게 해당 System.Security.Permissions.SecurityPermission이 없는 경우</exception>
        /// <exception cref="System.InvalidOperationException">
        /// System.Threading.ParameterizedThreadStart 대리자 대신 System.Threading.ThreadStart
        ///     대리자를 사용하여 이 메서드를 만든 경우
        /// </exception>
        public void Start(object parameter) {
            if(IsDebugEnabled)
                log.Debug("Start the internal thread with parameter. parameter=[{0}]", parameter);

            InternalThread.Start(parameter);
        }

        /// <summary>
        /// 스레드를 시작하기 전에 스레드의 아파트 상태를 설정합니다.
        /// </summary>
        /// <param name="state">새 아파트 상태입니다.</param>
        /// <returns>아파트 상태가 설정되어 있으면 true이고, 그렇지 않으면 false입니다.</returns>
        /// <exception cref="ThreadStateException">트랜잭션이 이미 시작된 경우</exception>
        /// <exception cref="ArgumentException">state가 잘못된 아파트 상태인 경우</exception>
        public bool TrySetApartmentState(ApartmentState state) {
            return InternalThread.TrySetApartmentState(state);
        }

        #endregion
    }
}