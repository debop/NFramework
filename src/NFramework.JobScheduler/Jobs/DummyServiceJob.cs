using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Quartz;

namespace NSoft.NFramework.JobScheduler.Jobs {
    /// <summary>
    /// 테스트용 서비스 작업입니다. 작업 내용은 로그 쓰고, 작업을 흉내내기 위한 작업 시간 지연을 합니다.
    /// </summary>
    [CLSCompliant(false)]
    public class DummyServiceJob : AbstractServiceJob {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public DummyServiceJob() {}
        public DummyServiceJob(string name) : base(name) {}
        public DummyServiceJob(IDictionary stateMap) : base(stateMap) {}

        /// <summary>
        /// 실제 작업을 정의합니다.
        /// </summary>
        /// <param name="context">Quartz <see cref="JobExecutionContext"/></param>
        /// <param name="token">작업중 중단를 할 수 있도록 하는 Token</param>
        public override void DoExecute(JobExecutionContext context, CancellationToken token) {
            var jobName = context.JobDetail.FullName;

            if(IsDebugEnabled)
                log.Debug(@"Service Job[{0}]을 시작합니다...", jobName);

            try {
                var jobDataMap = context.JobDetail.JobDataMap;

                if(IsDebugEnabled)
                    foreach(var key in jobDataMap.Keys)
                        log.Debug(@"Service Job[{0}] has state data. key=[{1}], value=[{2}]", jobName, key, jobDataMap.Get(key));

                if(token.IsCancellationRequested) {
                    if(IsDebugEnabled)
                        log.Debug(@"Service Job[{0}]의 작업 취소 요청을 받았습니다. 작업을 중단합니다!!!", jobName);

                    return;
                }

                var dummyTask = Task.Factory.StartNew(() => Thread.Sleep(500));

                dummyTask.Wait(token);
                // Task.WaitAll(new Task[] { dummyTask }, token);

                if(IsDebugEnabled)
                    log.Debug(@"Service Job[{0}]의 작업을 완료했습니다!!!", jobName);
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled) {
                    log.Error("Service Job[{0}] 실행 중 예외가 발생했습니다.", jobName);
                    log.Error(ex);
                }

                throw;
            }
        }
    }
}