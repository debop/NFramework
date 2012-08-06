using System;
using System.Threading;
using System.Threading.Tasks;
using NSoft.NFramework.Parallelism.Tools;
using NUnit.Framework;

namespace NSoft.NFramework.Parallelism.Extensions {
    [Microsoft.Silverlight.Testing.Tag("Parallel")]
    [TestFixture]
    public class TaskExtensionsTestCase : ParallelismFixtureBase {
        [Test]
        public void Task_ToAsync() {
            const int Number = 9999;
            int asyncValue = 0;

            var task = new Task(() => {
                                    Thread.Sleep(1000);
                                    Console.WriteLine("Task is completed.");
                                },
                                TaskCreationOptions.LongRunning);

            task.Start();

            task.ToAsync(ar => {
                             var runTask = (Task)ar;
                             asyncValue = (int)runTask.AsyncState;
                             Console.WriteLine("AsyncCallback is called. runTask is completed=" + runTask.IsCompleted);
                         },
                         Number);


            task.Wait();

            // 비동기 callback 함수가 실행될 시간을 벌어줍니다.
            //
            Thread.Sleep(100);

            Assert.AreEqual(Number, asyncValue);
        }

        [Test]
        public void Task_WithTimeout() {
            var task = new Task(() => {
                                    Thread.Sleep(100);
                                    if(IsDebugEnabled)
                                        log.Debug("Task is completed.");
                                });

            // 지정된 시간이 지났지만, 주어진 작업이 완료가 안되었다면, 작업을 취소하도록 합니다.
            var timeoutTask = task.WithTimeout(TimeSpan.FromMilliseconds(200));

            // 작업을 동기적으로 수행합니다.
            task.RunSynchronously();
            timeoutTask.Wait();

            Assert.IsTrue(timeoutTask.IsCompleted);
        }

        [Test]
        public void Task_AttachToParent() {
            var task = Task.Factory.StartNew(() => {
                                                 Thread.Sleep(Rnd.Next(10, 20));
                                                 if(IsDebugEnabled)
                                                     log.Debug("Task is completed");
                                             });

            task.AttachToParent();

            Console.WriteLine("Task attached to parent.");
        }
    }
}