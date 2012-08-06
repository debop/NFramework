using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using NSoft.NFramework.JobScheduler.Jobs;
using Quartz;
using Quartz.Impl;

namespace NSoft.NFramework.JobScheduler.Managers {
    /// <summary>
    /// Quartz 작업 관리자를 이용한 Job Manager입니다.
    /// </summary>
    [CLSCompliant(false)]
    public abstract class AbstractServiceJobManager : IServiceJobManager {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private readonly object _syncLock = new object();

        /// <summary>
        /// Job Scheduler의 Factory
        /// </summary>
        private readonly ISchedulerFactory _factory = new StdSchedulerFactory();

        /// <summary>
        /// Scheduler
        /// </summary>
        private IScheduler _scheduler;

        /// <summary>
        /// Service Job의 리스트
        /// </summary>
        [CLSCompliant(false)] protected IList<IServiceJob> _serviceJobs;

        /// <summary>
        /// 생성자
        /// </summary>
        protected AbstractServiceJobManager() : this(null) {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="props"></param>
        protected AbstractServiceJobManager(NameValueCollection props) {
            _factory = (props != null) ? new StdSchedulerFactory(props) : new StdSchedulerFactory();
            _scheduler = _factory.GetScheduler();
        }

        /// <summary>
        /// Factory of Scheduler
        /// </summary>
        [CLSCompliant(false)]
        public virtual ISchedulerFactory Factory {
            get { return _factory; }
        }

        /// <summary>
        /// Quartz Job Scheduler
        /// </summary>
        [CLSCompliant(false)]
        protected virtual IScheduler Scheduler {
            get { return _scheduler ?? (_scheduler = Factory.GetScheduler()); }
            set { _scheduler = value; }
        }

        /// <summary>
        /// 작업 관리 시작 여부
        /// </summary>
        public bool IsStarted {
            get { return Scheduler.IsStarted; }
        }

        /// <summary>
        /// 관리할 서비스 작업 (<see cref="IServiceJob"/>)의 인스턴스들
        /// </summary>
        public IEnumerable<IServiceJob> ServiceJobs {
            get { return _serviceJobs ?? Enumerable.Empty<IServiceJob>(); }
        }

        /// <summary>
        /// 서비스 작업 관리자를 시작합니다. 모든 서비스 작업에 대한 작업을 스케쥴에 맞게 수행되도록 합니다.
        /// </summary>
        public virtual void Start() {
            if(IsStarted)
                return;

            if(log.IsInfoEnabled)
                log.Info(@"서비스 작업 관리자 [{0}]를 시작합니다...", GetType().FullName);

            lock(_syncLock) {
                _serviceJobs = ServiceJobContainer.ResolveAllServiceJob().Where(job => job.Enabled).ToList();
                Scheduler.ScheduleServiceJob(_serviceJobs.ToArray());
                Scheduler.Start();
            }

            if(log.IsInfoEnabled)
                log.Info(@"서비스 작업 관리자 [{0}]를 시작했습니다!!!", GetType().FullName);
        }

        /// <summary>
        /// 서비스 작업들을 모두 중단하도록 합니다.
        /// </summary>
        public virtual void Shutdown() {
            if(IsStarted == false)
                return;

            if(log.IsInfoEnabled)
                log.Info(@"서비스 작업 관리자 [{0}]를 종료합니다...", GetType().FullName);

            With.TryAction(Scheduler.Shutdown,
                           ex => {
                               if(log.IsErrorEnabled)
                                   log.ErrorException(@"서비스 작업 관리자를 종료하는데 실패했습니다.", ex);
                           },
                           () => { _scheduler = null; });

            if(log.IsInfoEnabled)
                log.Info(@"서비스 작업 관리자 [{0}]를 종료했습니다!!!", GetType().FullName);
        }

        #region << IDisposable >>

        /// <summary>
        /// 리소스 해제 여부
        /// </summary>
        public bool IsDisposed { get; protected set; }

        ~AbstractServiceJobManager() {
            Dispose(false);
        }

        /// <summary>
        /// 관리되지 않는 리소스를 해제합니다. Quartz Scheduler를 Shutdown시킵니다.
        /// </summary>
        public void Dispose() {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        /// <summary>
        /// JobScheduler를 멈추고, 리소스를 해제합니다.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing) {
            if(IsDisposed)
                return;

            if(disposing) {
                With.TryAction(Shutdown);

                if(IsDebugEnabled)
                    log.Debug(@"ServiceJobManager를 Dispose 했습니다!!!");
            }

            IsDisposed = true;
        }

        #endregion
    }
}