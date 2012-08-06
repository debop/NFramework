using System;
using System.Collections.Concurrent;
using System.Threading;

namespace NSoft.NFramework.UnitTesting {
    public static class ParallelTestTool {
        private const int RunCount = 100;
        private const int UnitCount = 10;

        public static void Repeat(Action action, int runCount = RunCount) {
            for(var i = 0; i < runCount; i++)
                action();
        }

        public static void ParallelStressTest<TSource>(TSource obj, Action<TSource> action, int threadCount = 0) {
            action.ShouldNotBeNull("action");

            if(threadCount <= 0)
                threadCount = Environment.ProcessorCount * 2;

            Thread[] threads = new Thread[threadCount];

            for(var i = 0; i < threadCount; i++) {
                threads[i] = new Thread(() => action(obj));
                threads[i].Start();
            }

            // Wait for the completion
            for(var i = 0; i < threadCount; i++)
                threads[i].Join();
        }

        public static void AddAsParallel(IProducerConsumerCollection<int> collection, int threadCount) {
            var startIndex = -UnitCount;

            ParallelStressTest(collection,
                               coll => {
                                   var start = Interlocked.Add(ref startIndex, UnitCount);
                                   for(var i = start; i < start + UnitCount; i++) {
                                       coll.TryAdd(i);
                                   }
                               },
                               threadCount);
        }

        public static void RemoveAsParallel(IProducerConsumerCollection<int> collection, int threadCount, int times) {
            var index = -1;

            ParallelStressTest(collection,
                               coll => {
                                   var num = Interlocked.Increment(ref index);

                                   int value;
                                   if(num < times)
                                       coll.TryTake(out value);
                               },
                               threadCount);
        }
    }
}