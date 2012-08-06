using System;
using System.Collections;
using NSoft.NFramework.JobScheduler.Jobs;
using Quartz;

namespace NSoft.NFramework.JobScheduler {
    /// <summary>
    /// Job 관련 Utility 메소드를 제공합니다.
    /// </summary>
    [CLSCompliant(false)]
    public static class JobTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public const string RetryCountKey = @"RetryCount";

        /// <summary>
        /// Job 상태정보를 담은 <see cref="JobDataMap"/>에서 해당 키의 값을 반환합니다. 없으면, null을 반환합니다.
        /// </summary>
        public static object GetJobData(this JobExecutionContext context, object key) {
            return context.JobDetail.JobDataMap.GetJobData(key);
        }

        /// <summary>
        /// Job 상태정보를 담은 <see cref="JobDataMap"/>에서 해당 키의 값을 반환합니다. 없으면, null을 반환합니다.
        /// </summary>
        public static object GetJobData(this IServiceJob serviceJob, object key) {
            serviceJob.ShouldNotBeNull("serviceJob");

            if(serviceJob is JobDetail)
                return ((JobDetail)serviceJob).GetJobData(key);

            return null;
        }

        /// <summary>
        /// Job 상태정보를 담은 <see cref="JobDataMap"/>에서 해당 키의 값을 반환합니다. 없으면, null을 반환합니다.
        /// </summary>
        public static object GetJobData(this JobDetail jobDetail, object key) {
            jobDetail.ShouldNotBeNull("jobDetail");
            return jobDetail.JobDataMap.GetJobData(key);
        }

        /// <summary>
        /// Job 상태정보를 담은 <see cref="JobDataMap"/>에서 해당 키의 값을 반환합니다. 없으면, null을 반환합니다.
        /// </summary>
        public static object GetJobData(this JobDataMap map, object key) {
            map.ShouldNotBeNull("map");

            return map.Contains(key) ? map.Get(key) : null;
        }

        /// <summary>
        ///  Job 상태정보를 담은 <see cref="JobDataMap"/>에서 해당 키에 값을 설정합니다.
        /// </summary>
        /// <param name="serviceJob"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetJobData(this JobDetail serviceJob, object key, object value) {
            serviceJob.ShouldNotBeNull("serviceJob");
            serviceJob.JobDataMap.SetJobData(key, value);
        }

        /// <summary>
        /// Job 상태정보를 담은 <see cref="JobDataMap"/>에서 해당 키에 값을 설정합니다.
        /// </summary>
        public static void SetJobData(this JobDataMap map, object key, object value) {
            map.ShouldNotBeNull("map");

            if(IsDebugEnabled)
                log.Debug(@"JobDataMap에 다음 값을 설정합니다. key=[{0}], value=[{1}]", key, value);

            map[key] = value;
        }

        /// <summary>
        /// 특정 Job의 상태 정보를 설정합니다.
        /// </summary>
        public static void SetJobData(this JobDetail serviceJob, IDictionary dictionary) {
            serviceJob.ShouldNotBeNull("serviceJob");
            serviceJob.JobDataMap.SetJobData(dictionary);
        }

        /// <summary>
        /// 특정 Job의 상태 정보를 설정합니다.
        /// </summary>
        public static void SetJobData(this JobDataMap map, IDictionary dictionary) {
            map.ShouldNotBeNull("map");

            if(dictionary != null)
                foreach(DictionaryEntry entry in dictionary)
                    map.SetJobData(entry.Key.ToString(), entry.Value);
        }

        /// <summary>
        /// Scheduler에 <see cref="IServiceJob"/>들을 스케쥴링합니다.
        /// </summary>
        /// <param name="scheduler"></param>
        /// <param name="jobs"></param>
        public static void ScheduleServiceJob(this IScheduler scheduler, params IServiceJob[] jobs) {
            scheduler.ShouldNotBeNull("scheduler");

            foreach(var job in jobs) {
                job.Trigger.ShouldNotBeNull("Trigger");

                if(IsDebugEnabled)
                    log.Debug(@"작업[{0}]을 Trigger[{1}]로 스케쥴링힙니다.", job.Name, job.Trigger.Name);

                scheduler.ScheduleJob((JobDetail)job, job.Trigger);
            }
        }
    }
}