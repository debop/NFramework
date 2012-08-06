using System;
using System.Threading;
using NSoft.NFramework.JobScheduler.Jobs.SampleJobs;
using NSoft.NFramework.JobScheduler.Managers;
using NUnit.Framework;
using Quartz;
using Quartz.Impl;

namespace NSoft.NFramework.JobScheduler.Jobs {
    [TestFixture]
    public class JobTestCase : JobSchedulerTestCaseBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private ISchedulerFactory _factory;
        private IScheduler _scheduler;

        protected override void DoTestFixtureSetUp() {
            base.DoTestFixtureSetUp();

            _factory = new StdSchedulerFactory();
        }

        protected override void DoTestFixtureTearDown() {
            base.DoTestFixtureTearDown();
            _factory = null;
        }

        [Test]
        public void ServiceJob_Scheduling() {
            var job = new DummyServiceJob("DummyJob")
                      {
                          RetryInterval = 100,
                          MaxRetryCount = 5,
                          Trigger = new SimpleTrigger("SimpleTrigger", null, -1, TimeSpan.FromMilliseconds(500))
                      };

            RunServiceJob(job, TimeSpan.FromSeconds(3));
        }

        [TestCase("DummyServiceJob1")]
        [TestCase("DummyServiceJob2")]
        public void ServiceJob_Resolve_And_Scheduling(string jobId) {
            var job = ServiceJobContainer.ResolveJob(jobId);
            Assert.IsNotNull(job);
            Assert.IsNotNull(job.Trigger);

            RunServiceJob(job, TimeSpan.FromSeconds(3));
        }

        [TestCase("DummyServiceJob1")]
        [TestCase("DummyServiceJob2")]
        public void ServiceJob_TryResolve_And_Scheduling(string jobId) {
            IServiceJob job;

            var resolved = ServiceJobContainer.TryResolveJob(jobId, out job);

            Assert.IsTrue(resolved);
            Assert.IsNotNull(job);
            Assert.IsNotNull(job.Trigger);

            RunServiceJob(job, TimeSpan.FromSeconds(3));
        }

        [TestCase("http://www.daum.net")]
        [TestCase("http://www.naver.com")]
        public void WebPageDownloadServiceJob_Schedule(string targetUriString) {
            var job = new WebPageDownloadServiceJob("WebClientJob")
                      {
                          Trigger = new SimpleTrigger("SimpleWebClientTrigger", -1, TimeSpan.FromSeconds(3))
                      };

            // job 자체의 정보를 추가해봤자 Quartz Scheduler가 매번 새로운 인스턴스를 생성합니다.
            // 그러므로 꼭 상태유지를 해야 할 값은 JobDataMap을 이용하여 저장해야 합니다.
            //
            job.SetJobData("TargetUri", new Uri(targetUriString));

            RunServiceJob(job, TimeSpan.FromSeconds(10));
        }

        private void RunServiceJob(IServiceJob job, TimeSpan timeout) {
            job.ShouldNotBeNull("job");

            if(IsDebugEnabled)
                log.Debug(@"서비스 작업[{0}]을 실행합니다...", job.Name);

            try {
                _scheduler = _factory.GetScheduler();
                _scheduler.Start();

                _scheduler.ScheduleServiceJob(job);

                Thread.Sleep(timeout);
            }
            finally {
                _scheduler.Shutdown(true);

                if(IsDebugEnabled)
                    log.Debug(@"서비스 작업[{0}] 실행을 완료했습니다.", job.Name);
            }
        }
    }
}