using System;
using System.Collections.Generic;
using NSoft.NFramework.JobScheduler.Jobs;

namespace NSoft.NFramework.JobScheduler.Managers {
    /// <summary>
    /// 서비스 작업 (<see cref="IServiceJob"/>)을 IoC를 통해 얻은 후, Quartz 작업 스케쥴러에게 작업을 수행하도록 합니다.
    /// </summary>
    public interface IServiceJobManager : IDisposable {
        /// <summary>
        /// 작업 관리 시작 여부
        /// </summary>
        bool IsStarted { get; }

        /// <summary>
        /// 관리할 서비스 작업 (<see cref="IServiceJob"/>)의 인스턴스들
        /// </summary>
        IEnumerable<IServiceJob> ServiceJobs { get; }

        /// <summary>
        /// 서비스 작업 관리자를 시작합니다. 모든 서비스 작업에 대한 작업을 스케쥴에 맞게 수행되도록 합니다.
        /// </summary>
        void Start();

        /// <summary>
        /// 서비스 작업들을 모두 중단하도록 합니다.
        /// </summary>
        void Shutdown();
    }
}