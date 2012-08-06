using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework.Parallelism.DataStructures {
    [Microsoft.Silverlight.Testing.Tag("Parallel")]
    [TestFixture]
    public class ConcurrentDataStructureTestCase : ParallelismFixtureBase {
        [Test]
        public void ConcurrentDictionary_GetOrAdd_Not_ThreadSafe_In_Threads() {
            TestTool.RunTasks(5,
                              ConcurrentDictionary_GetOrAdd_Not_ThreadSafe,
                              ConcurrentDictionary_GetOrAdd_Not_ThreadSafe,
                              ConcurrentDictionary_GetOrAdd_Not_ThreadSafe);
        }

        [Test]
        public void ConcurrentDictionary_GetOrAdd_Not_ThreadSafe() {
            var dc = new ConcurrentDictionary<int, ThreadLocal<int>>();

            Task.Factory
                .StartNew(() => {
                              dc.GetOrAdd(1,
                                          i =>
                                          new ThreadLocal<int>(() => {
                                                                   var currentId = Thread.CurrentThread.ManagedThreadId;

                                                                   if(IsDebugEnabled)
                                                                       log.Debug("Current Thread Id=" + currentId);

                                                                   return currentId;
                                                               }));
                          })
                .Wait();

            ThreadPool.QueueUserWorkItem(_ => Assert.AreEqual(Thread.CurrentThread.ManagedThreadId, dc[1].Value));

            Thread.Sleep(10);

            Assert.AreEqual(Thread.CurrentThread.ManagedThreadId, dc[1].Value);
        }
    }
}