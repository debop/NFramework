using System;
using Quartz;

namespace NSoft.NFramework.JobScheduler.JobListeners {
    /// <summary>
    /// 단순하게 Job 실행과 관련된 기록을 Logging을 수행하는 Listener 입니다.
    /// </summary>
    [CLSCompliant(false)]
    public class LoggingJobListener : AbstractJobListener {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 작업 실행 시작 전에 호출됩니다.
        /// </summary>
        /// <param name="context"></param>
        public override void JobToBeExecuted(JobExecutionContext context) {
            base.JobToBeExecuted(context);

            if(IsDebugEnabled)
                log.Debug(@"Job[{0}]이 실행 예정되었습니다.", context.JobDetail.FullName);
        }

        /// <summary>
        /// 작업이 취소되었을 때 호출됩니다.
        /// </summary>
        /// <param name="context"></param>
        public override void JobExecutionVetoed(JobExecutionContext context) {
            base.JobExecutionVetoed(context);

            if(IsDebugEnabled)
                log.Debug(@"Job[{0}] 실행이 거부되었습니다.", context.JobDetail.FullName);
        }

        /// <summary>
        /// 작업이 완료(성공이던 실패던)되었을 때, 호출됩니다.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="jobException"></param>
        public override void JobWasExecuted(JobExecutionContext context, JobExecutionException jobException) {
            base.JobWasExecuted(context, jobException);

            if(jobException != null) {
                if(log.IsWarnEnabled)
                    log.WarnException(string.Format("Job[{0}] 실행에 예외가 발생했습니다!!!", context.JobDetail.FullName), jobException);
            }
            else if(IsDebugEnabled)
                log.Debug(@"Job[{0}] 이 수행되었습니다.", context.JobDetail.FullName);
        }
    }
}