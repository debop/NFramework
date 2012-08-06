using System;
using Quartz;

namespace NSoft.NFramework.JobScheduler.JobListeners {
    /// <summary>
    /// Job 실행 경과를 알아볼 수 있는 Listener의 추상화 클래스입니다.
    /// </summary>
    [CLSCompliant(false)]
    public abstract class AbstractJobListener : IJobListener {
        /// <summary>
        /// JobListener 이름
        /// </summary>
        public virtual string Name {
            get { return GetType().FullName; }
        }

        /// <summary>
        /// 작업 실행 시작 전에 호출됩니다.
        /// </summary>
        /// <param name="context"></param>
        public virtual void JobToBeExecuted(JobExecutionContext context) {}

        /// <summary>
        /// 작업이 취소되었을 때 호출됩니다.
        /// </summary>
        /// <param name="context"></param>
        public virtual void JobExecutionVetoed(JobExecutionContext context) {}

        /// <summary>
        /// 작업이 완료(성공이던 실패던)되었을 때, 호출됩니다.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="jobException"></param>
        public virtual void JobWasExecuted(JobExecutionContext context, JobExecutionException jobException) {}
    }
}