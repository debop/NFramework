using System;
using System.Collections;
using Quartz;

namespace NSoft.NFramework.JobScheduler.Jobs {
    /// <summary>
    /// Quartz 작업 관리자가 수행할 Background Service 작업을 나타내는 기본 Interface입니다.
    /// </summary>
    [CLSCompliant(false)]
    public interface IServiceJob : IStatefulJob, IInterruptableJob {
        /// <summary>
        /// Job Name
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Job 그룹
        /// </summary>
        string Group { get; set; }

        /// <summary>
        /// 실행 여부
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// 실패 시 최대 수행 횟수 (주기 작업이 실패했을 경우 바로 다시 재실행하는 횟수를 말한다. 0이면 다음 주기에 실행한다)
        /// </summary>
        int MaxRetryCount { get; set; }

        /// <summary>
        /// 재시도 간격 (msec)
        /// </summary>
        int RetryInterval { get; set; }

        /// <summary>
        /// 작업 주기를 설정한 Trigger
        /// </summary>
        Quartz.Trigger Trigger { get; set; }

        /// <summary>
        /// Job 인스턴스의 상태 정보 (처음 Job 실행 시의 초기값만을 설정할 수 있습니다. 내부적으로 <see cref="JobDetail.JobDataMap"/>에 값이 설정됩니다)
        /// </summary>
        IDictionary StateMap { get; set; }
    }
}