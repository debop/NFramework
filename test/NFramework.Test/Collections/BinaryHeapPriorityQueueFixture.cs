using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Collections {
    [TestFixture]
    public class BinaryHeapPriorityQueueFixture : AbstractFixture {
        private BinaryHeapPriorityQueue<int> _priorityQueue;

        private const int ItemCount = 1000;

        protected override void OnSetUp() {
            base.OnSetUp();

            _priorityQueue = new BinaryHeapPriorityQueue<int>();

            for(int i = 0; i < ItemCount; i++)
                _priorityQueue.Enqueue(Rnd.Next(ItemCount));
        }

        [Test]
        public void EnqueueTest() {
            for(int i = 0; i < ItemCount; i++) {
                _priorityQueue.Enqueue(Rnd.Next(ItemCount));
            }
            Assert.AreEqual(ItemCount * 2, _priorityQueue.Size);
        }

        [Test]
        public void DequeueTest() {
            int root = _priorityQueue.Dequeue();
            int next = _priorityQueue.Dequeue();

            Assert.IsTrue(root >= next);
        }

        [Test]
        public void PeekTest() {
            int top = _priorityQueue.Peek();
            _priorityQueue.Contains(top).Should().Be.True();
        }
    }
}