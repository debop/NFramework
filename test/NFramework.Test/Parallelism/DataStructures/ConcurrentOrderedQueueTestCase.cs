using System;
using System.Linq;
using System.Threading.Tasks;
using NSoft.NFramework.LinqEx;
using NUnit.Framework;

namespace NSoft.NFramework.Parallelism.DataStructures {
    [TestFixture]
    public class ConcurrentOrderedQueueTestCase : ParallelismFixtureBase {
        private const int DataCount = 100;

        [Test]
        public void Is_Properly_Sorted_Queue() {
            var sortedQueue = new ConcurrentSortedQueue<int, double>();

            // 병렬로 값을 추가하므로, 순서에 상관없이 추가될 것이다.
            Parallel.For(0,
                         DataCount,
                         i => {
                             var value = Rnd.NextDouble();
                             sortedQueue.Enqueue(i, value);
                         });


            //KeyValuePair<int, double> item;
            //while (sortedQueue.TryDequeue(out item))
            //{
            //    Console.WriteLine("Item key={0}, value={1}", item.Key, item.Value);
            //}

            var items = sortedQueue.ToArray();

            items.RunEach(pair => Console.WriteLine("key=" + pair.Key));

            // 이미 정렬되어 있다면, Key로 정렬한 값과 같아야 한다.
            Assert.IsTrue(items.SequenceEqual(items.OrderBy(pair => pair.Key)));
        }
    }
}