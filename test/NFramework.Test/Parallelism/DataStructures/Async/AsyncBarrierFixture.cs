using System;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework.Parallelism.DataStructures.Async {
    [Microsoft.Silverlight.Testing.Tag("Parallel")]
    [TestFixture]
    public class AsyncBarrierFixture : ParallelismFixtureBase {
        private static readonly int ThreadCount = Environment.ProcessorCount;
        private const int InitialParticipantCount = 10;

        [Test]
        public void Can_SingnalAndWait_To_Complete_Task() {
            TestTool.RunTasks(ThreadCount, Run_AsyncBarrier, Run_AsyncBarrier);
        }

        [Test]
        public void Run_AsyncBarrier() {
            var asyncBarrier = new AsyncBarrier(InitialParticipantCount);
            Assert.AreEqual(InitialParticipantCount, asyncBarrier.ParticipantCount);

            var task = asyncBarrier.SignalAndWait();

            while(task.IsCompleted == false) {
                if(IsDebugEnabled)
                    log.Debug("AsyncBarrier# RemainingCount={0}, ParticipantCount={1}, IsCompleted={2}",
                              asyncBarrier.RemainingCount, asyncBarrier.ParticipantCount, task.IsCompleted);

                Assert.IsFalse(task.IsCanceled);
                Assert.IsFalse(task.IsFaulted);

                task = asyncBarrier.SignalAndWait();
            }

            // Participant가 모두 없어졌으면 Task는 완료되었다고 본다.
            //
            Assert.IsTrue(task.IsCompleted);

            if(IsDebugEnabled)
                log.Debug("AsyncBarrier# RemainingCount={0}, ParticipantCount={1}, IsCompleted={2}",
                          asyncBarrier.RemainingCount, asyncBarrier.ParticipantCount, task.IsCompleted);
        }
    }
}