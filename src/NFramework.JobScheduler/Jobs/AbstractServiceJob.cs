using System;
using System.Collections;
using System.Threading;
using Quartz;

namespace NSoft.NFramework.JobScheduler.Jobs {
    /// <summary>
    /// 서비스 작업의 최상위 추상화 클래스입니다.
    /// </summary>
    [CLSCompliant(false)]
    public abstract class AbstractServiceJob : JobDetail, IServiceJob {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        protected AbstractServiceJob() {
            JobType = GetType();
            MaxRetryCount = 3;
        }

        protected AbstractServiceJob(string name) : this() {
            name.ShouldNotBeEmpty("name");
            Name = name;
        }

        protected AbstractServiceJob(IDictionary stateMap) : this() {
            if(log.IsInfoEnabled)
                log.Info(@"ServiceJob을 위한 StateMap 정보를 제공받았습니다!!!");

            if(stateMap != null) {
                foreach(DictionaryEntry de in stateMap)
                    StateMap.Add(de.Key, de.Value);

                JobDataMap.SetJobData(stateMap);
            }
        }

        /// <summary>
        /// 작업 취소 시 <see cref="CancellationToken"/>에 신호를 보냅니다.
        /// </summary>
        protected CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

        private IDictionary _stateMap;

        private bool _enabled = true;

        /// <summary>
        /// 실행 여부
        /// </summary>
        public bool Enabled {
            get { return _enabled; }
            set { _enabled = value; }
        }

        /// <summary>
        /// 실패 시 최대 수행 횟수 (주기 작업이 실패했을 경우 바로 다시 재실행하는 횟수를 말한다. 0이면 다음 주기에 실행한다)
        /// </summary>
        public int MaxRetryCount { get; set; }

        /// <summary>
        /// 재시도 간격 (msec)
        /// </summary>
        public int RetryInterval { get; set; }

        /// <summary>
        /// 작업 주기를 설정한 Trigger
        /// </summary>
        public Trigger Trigger { get; set; }

        /// <summary>
        /// Job 인스턴스의 상태 정보 (처음 Job 실행 시의 초기값만을 설정할 수 있습니다. 내부적으로 <see cref="JobDetail.JobDataMap"/>에 값이 설정됩니다)
        /// </summary>
        public IDictionary StateMap {
            get { return _stateMap ?? (_stateMap = new Hashtable()); }
            set {
                _stateMap = value;

                if(_stateMap != null)
                    JobDataMap.SetJobData(_stateMap);
                else
                    JobDataMap.Clear();
            }
        }

        /// <summary>
        /// 스케쥴러에 의해 주기적으로 호출되는 작업의 본체
        /// </summary>
        /// <param name="context"></param>
        public virtual void Execute(JobExecutionContext context) {
            var jobName = context.JobDetail.FullName;

            if(IsDebugEnabled)
                log.Debug(@"Job[{0}]을 실행합니다...", jobName);

            if(Enabled == false) {
                if(IsDebugEnabled)
                    log.Debug(@"작업[{0}]이 사용가능 상태가 아니기 때문에, 작업을 수행하지 않습니다. Enabled=[{1}]", jobName, Enabled);
                return;
            }

            var dataMap = context.JobDetail.JobDataMap;
            var retryCount = dataMap.GetJobData(JobTool.RetryCountKey).AsInt(0);

            try {
                DoExecute(context, CancellationTokenSource.Token);

                // 작업이 성공했으면, 재실행횟수를 0으로 리셋합니다.
                dataMap.SetJobData(JobTool.RetryCountKey, 0);

                if(IsDebugEnabled)
                    log.Debug(@"Job [{0}]을 완료했습니다!!!", jobName);
            }
            catch(Exception ex) {
                retryCount++;

                if(log.IsWarnEnabled) {
                    log.Warn(@"작업[{0}] 실행 중 예외가 발생했습니다. 재실행 횟수=[{1}]", jobName, retryCount);
                    log.Warn(ex);
                }

                if(CancellationTokenSource.Token.IsCancellationRequested)
                    return;

                //! 최대 재실행 횟수가 존재한다면, 재실행을 즉시 실행하도록 하고, JobExecutionException을 발생시키면, Quartz가 Job을 즉시 재실행시킵니다.
                //
                var maxRetryCount = Math.Max(0, MaxRetryCount.AsInt(0));
                var canRetry = (maxRetryCount > 0 && retryCount < maxRetryCount);

                if(canRetry) {
                    Thread.Sleep(Math.Max(10, RetryInterval.AsInt(10)));
                    dataMap.SetJobData(JobTool.RetryCountKey, retryCount);
                    throw new JobExecutionException(ex) { RefireImmediately = true };
                }
            }
        }

        /// <summary>
        /// 작업이 취소되었을 시에 호출되는 메소드입니다.
        /// </summary>
        public virtual void Interrupt() {
            if(IsDebugEnabled)
                log.Debug(@"작업 취소 요청을 받았습니다. 작업 취소를 시작합니다...");

            try {
                CancellationTokenSource.Cancel();
            }
            catch(Exception ex) {
                if(log.IsWarnEnabled)
                    log.WarnException(@"작업 취소를 요청했습니다만, 예외가 발생했습니다. 예외는 무시됩니다.", ex);
            }

            if(IsDebugEnabled)
                log.Debug(@"작업을 취소했습니다!!!");
        }

        /// <summary>
        /// 실제 작업을 정의합니다.
        /// </summary>
        /// <param name="context">Quartz <see cref="JobExecutionContext"/></param>
        /// <param name="token">작업 중 작업 취소를 할 수 있도록 하는 Token</param>
        public abstract void DoExecute(JobExecutionContext context, CancellationToken token);
    }
}