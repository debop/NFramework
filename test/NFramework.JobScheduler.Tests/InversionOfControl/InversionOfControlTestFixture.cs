using System;
using System.Linq;
using NSoft.NFramework.InversionOfControl;
using NSoft.NFramework.JobScheduler.Jobs;
using NSoft.NFramework.JobScheduler.Managers;
using NUnit.Framework;
using Quartz;

namespace NSoft.NFramework.JobScheduler.InversionOfControl {
    [TestFixture]
    public class InversionOfControlTestFixture {
        [TestFixtureSetUp]
        public void ClassSetUp() {
            if(IoC.IsNotInitialized)
                IoC.Initialize();
        }

        [Test]
        public void CanInitialize() {
            if(IoC.IsNotInitialized)
                IoC.Initialize();

            Assert.IsTrue(IoC.IsInitialized);
        }

        [Test]
        public void CanResolveServiceJob() {
            var dummyJob1 = ServiceJobContainer.ResolveJob("DummyServiceJob1");
            Assert.IsNotNull(dummyJob1);

            Assert.AreEqual(1000, dummyJob1.RetryInterval);
            Assert.AreEqual("DummyGroup", dummyJob1.Group);
            Assert.AreEqual(1, dummyJob1.StateMap["RunCount"].AsInt());

            // BUG: IDictionary 정보를 설정하지 못한다!!!  NSoft.NFramework.IoC.config에 보면 잘 되는데...
            Assert.AreEqual(1, dummyJob1.GetJobData("RunCount").AsInt());

            Assert.IsNotNull(dummyJob1.Trigger);
        }

        [Test]
        public void CanTryResolveServiceJob() {
            IServiceJob dummyJob1;
            var resolved = ServiceJobContainer.TryResolveJob("DummyServiceJob1", out dummyJob1);

            Assert.IsTrue(resolved);
            Assert.IsNotNull(dummyJob1);

            Assert.AreEqual(1000, dummyJob1.RetryInterval);
            Assert.AreEqual("DummyGroup", dummyJob1.Group);
            Assert.AreEqual(1, dummyJob1.StateMap["RunCount"].AsInt());

            // BUG: IDictionary 정보를 설정하지 못한다!!!  NSoft.NFramework.IoC.config에 보면 잘 되는데...
            Assert.AreEqual(1, dummyJob1.GetJobData("RunCount").AsInt());

            Assert.IsNotNull(dummyJob1.Trigger);
        }

        [Test]
        public void CanResolveAllServiceJob() {
            var jobs = ServiceJobContainer.ResolveAllServiceJob().ToList();

            Assert.Greater(jobs.Count, 0);

            foreach(var job in jobs) {
                Assert.IsNotNull(job);
                Assert.IsNotNull(job.Trigger);
            }
        }

        [Test]
        public void CanResolveAllServiceTriggers() {
            var triggers = ServiceJobContainer.ResolveAllTrigger().ToList();

            Assert.Greater(triggers.Count, 0);

            foreach(var trigger in triggers) {
                Assert.IsNotNull(trigger);

                if(trigger is SimpleTrigger) {
                    var simpleTrigger = (SimpleTrigger)trigger;
                    Assert.IsNotEmpty(simpleTrigger.Name);

                    // Assert.AreEqual(-1, simpleTrigger.RepeatCount);

                    Console.WriteLine("SimpleTrigger = " + trigger);
                }
                else if(trigger is CronTrigger) {
                    var cronTrigger = (CronTrigger)trigger;
                    Assert.IsNotEmpty(cronTrigger.CronExpressionString);

                    Console.WriteLine("CronTrigger = " + trigger);
                }
            }
        }

        [Test]
        public void CanResolveQuartzTrigger() {
            var trigger = IoC.Resolve<CronTrigger>("CronTrigger");
            Assert.IsNotNull(trigger);
            Assert.IsNotEmpty(trigger.CronExpressionString);
        }
    }
}