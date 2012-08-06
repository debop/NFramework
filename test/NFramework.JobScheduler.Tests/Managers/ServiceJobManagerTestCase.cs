using System.Threading;
using NSoft.NFramework.InversionOfControl;
using NUnit.Framework;

namespace NSoft.NFramework.JobScheduler.Managers {
    [TestFixture]
    public class ServiceJobManagerTestCase : JobSchedulerTestCaseBase {
        [Test]
        public void ResolveServiceJobManager() {
            using(var jobManager = IoC.Resolve<IServiceJobManager>()) {
                Assert.IsNotNull(jobManager);

                jobManager.Start();
                Assert.IsTrue(jobManager.IsStarted);

                Thread.Sleep(3000);
                jobManager.Shutdown();
                Assert.IsFalse(jobManager.IsStarted);

                jobManager.Start();
                Assert.IsTrue(jobManager.IsStarted);

                Thread.Sleep(3000);
                jobManager.Shutdown();
            }
        }

        [Test]
        public void StdServiceJobManager_Disposable() {
            using(var jobManager = new StdServiceJobManager()) {
                jobManager.Start();

                Assert.IsTrue(jobManager.IsStarted);

                Thread.Sleep(5 * 1000);
            }
        }

        [Test]
        public void StdServiceJobManager_Repeat_Start_Stop() {
            using(var jobManager = new StdServiceJobManager()) {
                jobManager.Start();
                Assert.IsTrue(jobManager.IsStarted);

                Thread.Sleep(5 * 1000);

                jobManager.Shutdown();
                Assert.IsFalse(jobManager.IsStarted);

                Thread.Sleep(10);

                jobManager.Start();
                Assert.IsTrue(jobManager.IsStarted);

                Thread.Sleep(3 * 1000);
            }
        }
    }
}