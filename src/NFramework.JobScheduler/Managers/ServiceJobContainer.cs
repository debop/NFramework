using System;
using System.Collections.Generic;
using System.Linq;
using NSoft.NFramework.InversionOfControl;
using NSoft.NFramework.JobScheduler.Jobs;
using Quartz;

namespace NSoft.NFramework.JobScheduler.Managers {
    /// <summary>
    ///  Quartz 관련 Components (Job, Trigger) 들을 IoC Resolve 하여 제공합니다.
    /// </summary>
    [CLSCompliant(false)]
    public static class ServiceJobContainer {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 지정된 Component Id를 가진 <see cref="IServiceJob"/> 컴포넌트를 Resolve합니다.
        /// </summary>
        /// <param name="componentId">Service Job 의 Id</param>
        /// <returns></returns>
        public static IServiceJob ResolveJob(string componentId) {
            componentId.ShouldNotBeWhiteSpace("componentId");

            if(IsDebugEnabled)
                log.Debug(@"ComponentId[{0}] 에 해당하는 IServiceJob Component를 Resolve합니다...", componentId);

            var job = IoC.Resolve<IServiceJob>(componentId);
            job.ShouldNotBeNull("job");

            BuildJobDataMap(job);

            if(IsDebugEnabled)
                log.Debug(@"ComponentId[{0}] 에 해당하는 IServiceJob Component를 Resolve 했습니다. ServiceJob=[{1}]", componentId, job);

            return job;
        }

        /// <summary>
        /// 지정된 Component Id를 가진 <see cref="IServiceJob"/> 컴포넌트를 Resolve합니다.
        /// </summary>
        /// <param name="componentId"></param>
        /// <param name="serviceJob"></param>
        /// <returns></returns>
        public static bool TryResolveJob(string componentId, out IServiceJob serviceJob) {
            serviceJob =
                With.TryFunction<IServiceJob>(() => ResolveJob(componentId),
                                              null,
                                              ex => {
                                                  if(log.IsErrorEnabled) {
                                                      log.Error("ServiceJob Component [{0}] 를 Resolve하는데 예외가 발생했습니다!!!", componentId);
                                                      log.Error(ex);
                                                  }
                                              });

            return (serviceJob != null);
        }

        /// <summary>
        /// IoC 환경설정에 설정된 모든 <see cref="IServiceJob"/> Component를 생성합니다.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<IServiceJob> ResolveAllServiceJob() {
            if(IsDebugEnabled)
                log.Debug(@"모든 IServiceJob 의 Components 들을 Resolve 합니다...");

            var serviceJobs = IoC.ResolveAll(typeof(IServiceJob)).Cast<IServiceJob>().ToList();

            foreach(var job in serviceJobs)
                BuildJobDataMap(job);

            return serviceJobs;
        }

        /// <summary>
        /// 초기 상태 정보를 설정합니다.
        /// </summary>
        /// <param name="serviceJob"></param>
        private static void BuildJobDataMap(IServiceJob serviceJob) {
            if(serviceJob != null && serviceJob is JobDetail) {
                ((JobDetail)serviceJob).SetJobData(serviceJob.StateMap);
            }
        }

        /// <summary>
        /// 지정된 Component Id를 가진 <see cref="Trigger"/>  컴포넌트를 Resolve합니다.
        /// </summary>
        /// <param name="componentId"></param>
        /// <returns></returns>
        public static Trigger ResolveTrigger(string componentId) {
            componentId.ShouldNotBeWhiteSpace("componentId");

            if(IsDebugEnabled)
                log.Debug(@"ComponentId[{0}] 에 해당하는 Quartz.Trigger component를 Resolve합니다...", componentId);

            var trigger = IoC.Resolve<Trigger>(componentId);

            if(IsDebugEnabled)
                log.Debug(@"ComponentId[{0}] 에 해당하는 Quartz.Trigger component를 Resolve했습니다!!! Trigger=[{1}]", componentId, trigger);

            return trigger;
        }

        /// <summary>
        /// <paramref name="triggerType"/>의 <see cref="Trigger"/> 컴포넌트를 Resolve 합니다.
        /// </summary>
        /// <param name="triggerType"></param>
        /// <returns></returns>
        public static Trigger ResolveTrigger(Type triggerType) {
            triggerType.ShouldNotBeNull("compoentType");

            if(IsDebugEnabled)
                log.Debug(@"TriggerType [{0}] 에 해당하는 Trigger Component를 Resolve합니다...", triggerType);

            var trigger = IoC.Resolve<Trigger>(triggerType);

            if(IsDebugEnabled)
                log.Debug(@"TriggerType [{0}] 에 해당하는 Trigger Component를 Resolve했습니다!!! Trigger=[{1}]", triggerType, trigger);


            return trigger;
        }

        /// <summary>
        /// 지정된 Component Id를 가진 <see cref="Trigger"/>  컴포넌트를 Resolve합니다.
        /// </summary>
        /// <param name="componentId"></param>
        /// <param name="trigger"></param>
        /// <returns></returns>
        public static bool TryResolveTrigger(string componentId, out Trigger trigger) {
            trigger =
                With.TryFunction<Trigger>(() => ResolveTrigger(componentId),
                                          null,
                                          ex => {
                                              if(log.IsErrorEnabled)
                                                  log.Error("ServiceJob Component [{0}] 를 Resolve하는데 예외가 발생했습니다.\n{1}", componentId, ex);
                                          });
            return (trigger != null);
        }

        /// <summary>
        /// <paramref name="triggerType"/>의 <see cref="Trigger"/> 컴포넌트를 Resolve 합니다.
        /// </summary>
        /// <param name="triggerType"></param>
        /// <param name="trigger"></param>
        /// <returns></returns>
        public static bool TryResolveTrigger(Type triggerType, out Trigger trigger) {
            trigger =
                With.TryFunction<Trigger>(() => ResolveTrigger(triggerType),
                                          null,
                                          ex => {
                                              if(log.IsErrorEnabled)
                                                  log.Error("ServiceJob Component TriggerType=[{0}] 를 Resolve하는데 예외가 발생했습니다.\n{1}",
                                                            triggerType, ex);
                                          });
            return (trigger != null);
        }

        /// <summary>
        /// IoC 환경설정에 정의된 <see cref="SimpleTrigger"/> 의 모든 컴포넌트를 Resolve합니다.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<SimpleTrigger> ResolveAllSimpleTrigger() {
            if(IsDebugEnabled)
                log.Debug(@"IoC 환경설정에 정의된 SimpleTrigger의 모든 컴포넌트를 Resolve합니다...");

            return IoC.ResolveAll(typeof(SimpleTrigger)).Cast<SimpleTrigger>();
        }

        /// <summary>
        /// IoC 환경설정에 정의된 <see cref="CronTrigger"/> 의 모든 컴포넌트를 Resolve합니다.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<CronTrigger> ResolveAllCronTrigger() {
            if(IsDebugEnabled)
                log.Debug(@"IoC 환경설정에 정의된 CronTrigger의 모든 컴포넌트를 Resolve합니다...");

            return IoC.ResolveAll(typeof(CronTrigger)).Cast<CronTrigger>();
        }

        /// <summary>
        /// IoC 환경설정에 정의된 <see cref="Trigger"/>의 모든 컴포넌트를 Resolve합니다.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Trigger> ResolveAllTrigger() {
            if(IsDebugEnabled)
                log.Debug(@"IoC 환경설정에 정의된 Trigger의 모든 컴포넌트를 Resolve합니다...");

            return IoC.ResolveAll(typeof(Trigger)).Cast<Trigger>();
        }
    }
}