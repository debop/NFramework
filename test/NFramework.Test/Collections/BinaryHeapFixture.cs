using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Collections {
    [TestFixture]
    public class BinaryHeapFixture : AbstractFixture {
        private const int ItemCount = 100;

        [Test]
        public void InsertSerial() {
            var heap = new BinaryHeap<int>();

            for(int i = 0; i < ItemCount; i++)
                heap.Add(i);

            int root = int.MaxValue;
            for(int i = 0; i < ItemCount; i++) {
                var item = heap.PopRoot();
                root.Should().Be.GreaterThanOrEqualTo(item);
                root = item;
            }
        }

        [Test]
        public void InsertRandom() {
            var heap = new BinaryHeap<int>();

            const int maxValue = ItemCount * 100;

            for(int i = 0; i < ItemCount; i++)
                heap.Add(Rnd.Next(1, maxValue));

            int root = int.MaxValue;
            for(int i = 0; i < ItemCount; i++) {
                var item = heap.PopRoot();
                root.Should().Be.GreaterThanOrEqualTo(item);
                root = item;
            }
        }

        [Test]
        public void HeapSort_Serial() {
            var list = new List<int>(Enumerable.Range(0, ItemCount));

            var sortedList = BinaryHeap<int>.Sort(list, Comparer<int>.Default);

            AssertSortedList<int>(sortedList);
        }

        [Test]
        public void HeapSort_Random() {
            var list = new List<int>(Enumerable.Range(0, ItemCount).Select(i => Rnd.Next(1, ItemCount * 10)).ToList());

            var sortedList = BinaryHeap<int>.Sort(list, Comparer<int>.Default);

            AssertSortedList<int>(sortedList);
        }

        private static void AssertSortedList<T>(IList<T> list) {
            //list.RunEach(item => Console.WriteLine(item));

            T minValue = list[0];

            list.All<T>(item => {
                            var result = Comparer<T>.Default.Compare(minValue, item) <= 0;
                            minValue = item;
                            return result;
                        })
                .Should().Be.True();
        }
    }
}