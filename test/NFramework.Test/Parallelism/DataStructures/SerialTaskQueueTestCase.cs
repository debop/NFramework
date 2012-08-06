using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace NSoft.NFramework.Parallelism.DataStructures {
    [Microsoft.Silverlight.Testing.Tag("Parallel")]
    [TestFixture]
    public class SerialTaskQueueTestCase : ParallelismFixtureBase {
        private const int TaskCount = 100;

        /// <summary>
        /// Task 추가 순으로 Task를 실행합니다.
        /// </summary>
        [Test]
        public void RunTaskInSerial() {
            // 큐에 추가된 Task 순서대로 실행되는지 검사한다.
            var taskQueue = new SerialTaskQueue();

            int count = 0;

            for(int i = 0; i < TaskCount; i++) {
                var task = new Task(state => {
                                        Thread.Sleep(Rnd.Next(10, 50));

                                        if(IsDebugEnabled)
                                            log.Debug("index = " + state);

                                        Assert.AreEqual(count, (int)state);
                                        Interlocked.Increment(ref count);
                                    },
                                    i);

                taskQueue.Enqueue(task);
            }

            // 큐에 있는 모든 Task들이 실행될 때까지 기다립니다.
            //
            taskQueue.Completed().Wait();
        }
    }
}