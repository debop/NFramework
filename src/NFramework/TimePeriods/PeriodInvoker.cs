using System;
using System.Threading;
using System.Threading.Tasks;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.TimePeriods {
    // TODO: PeriodRunner 로 개명

    /// <summary>
    /// 특정 반복 주기마다 메소드를 호출해주는 Class입니다.
    /// </summary>
    [Serializable]
    public class PeriodInvoker : IDisposable {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public static readonly TimeSpan DefaultIdleTimeSpan = TimeSpan.FromMinutes(1);
        public static readonly TimeSpan DefaultMinTimeSpan = TimeSpan.FromMilliseconds(50);

        private readonly object _syncLock = new object();

        private readonly Action<object> _periodAction;
        private CancellationTokenSource _cts;

        private readonly string _periodTimeFormat;
        private TimeSpan _periodTimeSpan = DefaultMinTimeSpan;

        public PeriodInvoker(string periodTimeFormat, Action<object> periodAction)
            : this(periodTimeFormat, DefaultIdleTimeSpan, periodAction, false) {}

        public PeriodInvoker(string periodTimeFormat, Action<object> periodAction, bool startNow)
            : this(periodTimeFormat, DefaultIdleTimeSpan, periodAction, startNow) {}

        public PeriodInvoker(string periodTimeFormat, TimeSpan idleTimeSpan, Action<object> periodAction, bool startNow = false) {
            periodAction.ShouldNotBeNull("periodAction");

            _periodAction = periodAction;
            _periodTimeFormat = periodTimeFormat;

            PeriodTimeSpan = idleTimeSpan;

            if(IsDebugEnabled)
                log.Debug("PeriodInvoke가 생성되었습니다. periodTimeFormat=[{0}], periodTimeSpan=[{1}], startNow=[{2}]", _periodTimeFormat,
                          _periodTimeFormat, startNow);

            if(startNow)
                Start();
        }

        /// <summary>
        /// 시작 여부
        /// </summary>
        public bool IsStarted { get; protected set; }

        /// <summary>
        /// 주기
        /// </summary>
        public TimeSpan PeriodTimeSpan {
            get { return _periodTimeSpan; }
            set {
                if(_periodTimeFormat.IsNotWhiteSpace())
                    _periodTimeSpan = (value > DefaultIdleTimeSpan) ? value : DefaultIdleTimeSpan;
                else
                    _periodTimeSpan = (value < DefaultMinTimeSpan) ? DefaultMinTimeSpan : value;
            }
        }

        /// <summary>
        /// 반복 호출 작업을 시작합니다.
        /// </summary>
        public void Start() {
            if(IsStarted)
                return;

            if(IsDebugEnabled)
                log.Debug("반복 주기에 지정된 Action을 호출하는 작업을 시작합니다...");

            lock(_syncLock) {
                _cts = new CancellationTokenSource();

                // Background Thread로 수행하기 위해 이렇게 하였습니다.
                //
                Task.Factory.StartNew(state => {
                                          var token = (CancellationToken)state;

                                          if(_periodTimeFormat.IsNotWhiteSpace())
                                              DoInvokeByPeriodTime(_periodTimeFormat, _periodTimeSpan, _periodAction, token);
                                          else
                                              DoInvokeByTimeSpan(_periodTimeSpan, _periodAction, token);
                                      },
                                      _cts.Token,
                                      _cts.Token,
                                      TaskCreationOptions.LongRunning,
                                      TaskScheduler.Default);

                IsStarted = true;
            }

            if(IsDebugEnabled)
                log.Debug("반복 주기에 지정된 Action을 호출하는 작업을 시작하였습니다!!!");
        }

        /// <summary>
        /// 반복 호출 작업을 취소합니다.
        /// </summary>
        public void Stop() {
            if(IsStarted == false)
                return;

            if(IsDebugEnabled)
                log.Debug("반복 주기 작업을 중단합니다...");

            lock(_syncLock) {
                With.TryActionAsync(() => {
                                        if(_cts != null)
                                            _cts.Cancel(false);
                                    },
                                    age => {
                                        if(IsDebugEnabled)
                                            log.Debug("작업 실행 중에 중단 요청에 의해 실행 취소되었습니다.");

                                        age.Handle(ex => true);
                                    },
                                    () => {
                                        IsStarted = false;

                                        if(_cts != null)
                                            With.TryAction(_cts.Dispose);
                                    });
            }

            if(IsDebugEnabled)
                log.Debug("반복 주기 작업을 중단했습니다!!!");
        }

        /// <summary>
        /// 반복 주기에 따라 지정된 Action을 비동기 방식으로 작업을 수행합니다.
        /// </summary>
        /// <param name="periodTimeFormat">주기 표현식</param>
        /// <param name="idleTimeSpan">유휴 시간 간격</param>
        /// <param name="periodAction">주기 도래시 수행할 델리게이트</param>
        /// <param name="token">취소시 필요한 토큰</param>
        protected virtual void DoInvokeByPeriodTime(string periodTimeFormat, TimeSpan idleTimeSpan, Action<object> periodAction,
                                                    CancellationToken token) {
            periodAction.ShouldNotBeNull("periodAction");

            if(IsDebugEnabled)
                log.Debug("반복 주기에 따라 지정된 Action을 비동기 방식으로 수행하는 작업을 시작합니다... periodTimeFormat=[{0}]", periodTimeFormat);

            var previousTime = DateTime.MinValue;

            while(token.IsCancellationRequested == false) {
                var currentTime = DateTime.Now;

                if(PeriodTimeFormat.IsExpired(periodTimeFormat, previousTime, currentTime)) {
                    if(IsDebugEnabled)
                        log.Debug("반복 주기에 도래하여, 지정된 Action을 수행을 시작합니다... " +
                                  "periodTimeFormat=[{0}], previousTime=[{1}], currentTime=[{2}]",
                                  periodTimeFormat, periodAction, currentTime);

                    With.TryActionAsync(
                        () => Task.Factory.StartNew(periodAction, token, TaskCreationOptions.PreferFairness).Wait(token),
                        age => {
                            if(IsDebugEnabled)
                                log.Debug("작업 실행 중에 중단 요청에 의해 실행 취소되었습니다...");

                            age.Handle(ex => token.IsCancellationRequested);
                        });

                    previousTime = currentTime;

                    if(IsDebugEnabled)
                        log.Debug("반복 주기에 도래하여, 지정된 Action을 수행을 완료하였습니다!!! " +
                                  "periodTimeFormat=[{0}], previousTime=[{1}], currentTime=[{2}]",
                                  periodTimeFormat, periodAction, DateTime.Now);
                }

                if(token.IsCancellationRequested == false)
                    token.WaitHandle.WaitOne(idleTimeSpan);
            }
        }

        /// <summary>
        /// 반복 주기에 따라 지정된 Action을 비동기 방식으로 수행하는 작업을 수행합니다.
        /// </summary>
        /// <param name="periodTimeSpan">반복 작업 간의 시간 간격</param>
        /// <param name="periodAction">주기 도래시 수행할 델리게이트</param>
        /// <param name="token">취소시 필요한 토큰</param>
        protected virtual void DoInvokeByTimeSpan(TimeSpan periodTimeSpan, Action<object> periodAction, CancellationToken token) {
            if(IsDebugEnabled)
                log.Debug("반복 주기에 따라 지정된 Action을 비동기 방식으로 수행하는 작업을 시작합니다... periodTimeSpan=[{0}]", periodTimeSpan);

            while(token.IsCancellationRequested == false) {
                if(IsDebugEnabled)
                    log.Debug("반복 주기에 도래하여, 지정된 Action을 수행을 시작합니다... periodTimeSpan=[{0}]", periodTimeSpan);

                With.TryActionAsync(() =>
                                    Task.Factory
                                        .StartNew(periodAction, token, TaskCreationOptions.PreferFairness)
                                        .ContinueWith(antecedent => {
                                                          if(antecedent.IsCompleted)
                                                              if(IsDebugEnabled)
                                                                  log.Debug(
                                                                      "반복 주기에 도래하여, 지정된 Action을 수행을 완료하였습니다!!! periodTimeSpan=[{0}]",
                                                                      periodTimeSpan);

                                                          if(token.IsCancellationRequested == false)
                                                              token.WaitHandle.WaitOne(periodTimeSpan);
                                                      })
                                        .Wait(token),
                                    age => {
                                        if(IsDebugEnabled)
                                            log.Debug("작업 실행 중에 중단 요청에 의해 실행 취소되었습니다.");

                                        age.Handle(ex => token.IsCancellationRequested);
                                    });
            }
        }

        #region << IDisposable >>

        public virtual bool IsDisposed { get; protected set; }

        ~PeriodInvoker() {
            Dispose(false);
        }

        public void Dispose() {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing) {
            if(IsDisposed)
                return;

            if(disposing) {
                if(IsStarted)
                    With.TryActionAsync(Stop);

                if(log.IsDebugEnabled)
                    log.Debug("반복 주기 작업을 중단하고, PeriodInvoker 인스턴스를 메모리에서 제거합니다.");
            }

            IsDisposed = true;
        }

        #endregion
    }
}