using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework.Parallelism.DataStructures.Async {
    /// <summary>
    /// 비동기 방식으로 지종된 Action을 호출합니다.
    /// </summary>
    [TestFixture]
    public class AsyncCallFixture : ParallelismFixtureBase {
        private const int IterationCount = 100;

        [Test]
        public void AsyncCall_To_Action() {
            var processedCount = 0;

            Action<int> @action = x => {
                                      Interlocked.Increment(ref processedCount);
                                      if(IsDebugEnabled)
                                          log.Debug("Called with Item[{0}]. ProcessedCount={1}", x, processedCount);
                                  };

            // var asyncCall = AsyncCall.Create(action);
            var asyncCall = AsyncCall.Create(@action, Environment.ProcessorCount, int.MaxValue, null);

            Enumerable.Range(0, IterationCount).RunEach(asyncCall.Post);

            // 비동기 호출이므로, 걍 끝내면 모두 실행되지 않거든요...
            Thread.Sleep(IterationCount * 10);

            Assert.AreEqual(IterationCount, processedCount);
        }

        [Test]
        public void AsyncCall_To_Func() {
            var processedCount = 0;

            Func<int, Task> @function =
                x =>
                Task.Factory.StartNew(() => {
                                          Interlocked.Increment(ref processedCount);

                                          if(IsDebugEnabled)
                                              log.Debug("Called with Item[{0}]. ProcessedCount={1}", x, processedCount);
                                      },
                                      TaskCreationOptions.PreferFairness);


            // var asyncCall = AsyncCall.Create(@function);
            var asyncCall = AsyncCall.Create(@function, Environment.ProcessorCount, null);

            Enumerable.Range(0, IterationCount).RunEach(asyncCall.Post);

            // 비동기 호출이므로, 걍 끝내면 모두 실행되지 않거든요...
            Thread.Sleep(IterationCount * 10);

            Assert.AreEqual(IterationCount, processedCount);
        }

        [Test]
        public void ThreadTest() {
            TestTool.RunTasks(4,
                              AsyncCall_To_Action,
                              AsyncCall_To_Func);
        }
    }
}