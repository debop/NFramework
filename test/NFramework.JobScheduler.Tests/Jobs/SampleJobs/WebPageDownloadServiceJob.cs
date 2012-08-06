using System;
using System.Collections;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using NSoft.NFramework.Parallelism.Tools;
using Quartz;

namespace NSoft.NFramework.JobScheduler.Jobs.SampleJobs {
    public class WebPageDownloadServiceJob : AbstractServiceJob {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public WebPageDownloadServiceJob() {}
        public WebPageDownloadServiceJob(string name) : base(name) {}
        public WebPageDownloadServiceJob(IDictionary stateMap) : base(stateMap) {}

        /// <summary>
        /// 실제 작업을 정의합니다.
        /// </summary>
        /// <param name="context">Quartz <see cref="JobExecutionContext"/></param>
        /// <param name="token">작업중 중단를 할 수 있도록 하는 Token</param>
        public override void DoExecute(JobExecutionContext context, CancellationToken token) {
            var jobName = context.JobDetail.FullName;

            if(IsDebugEnabled)
                log.Debug(@"Job[{0}]을 시작합니다...", jobName);

            var targetUri = context.GetJobData("TargetUri") as Uri;

            using(var client = new WebClient()) {
                var task = client.DownloadStringTask(targetUri ?? new Uri(@"http://debop.egloos.com"));

                Task.WaitAll(new[] { task }, token);

                if(IsDebugEnabled) {
                    log.Debug("다운로드를 받았습니다.");
                    log.Debug(task.Result);
                }
            }

            if(IsDebugEnabled)
                log.Debug(@"Job[{0}]을 완료했습니다.", jobName);
        }
    }
}