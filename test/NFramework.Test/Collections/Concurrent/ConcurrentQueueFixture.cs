using System.Collections.Concurrent;
using NUnit.Framework;

namespace NSoft.NFramework.Collections.Concurrent {
    [TestFixture]
    public class ConcurrentQueueFixture : AbstractFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private ConcurrentQueue<int> queue;

        [SetUp]
        public void Setup() {
            queue = new ConcurrentQueue<int>();
            for(int i = 0; i < 10; i++) {
                queue.Enqueue(i);
            }
        }

        [Test]
        public void StressEnqueueTest() {
            new ConcurrentQueue<int>().AddStress();
        }

        [Test]
        public void StressDenqueueTest() {
            new ConcurrentQueue<int>().RemoveStress(CheckOrderingType.InOrder);
        }

        [Test]
        public void CountTest() {
            Assert.AreEqual(10, queue.Count, "#1");
            int value;
            queue.TryPeek(out value);
            queue.TryDequeue(out value);
            queue.TryDequeue(out value);
            Assert.AreEqual(8, queue.Count, "#2");
            queue.Clear();
            Assert.AreEqual(0, queue.Count, "#3");
            Assert.IsTrue(queue.IsEmpty, "#4");
        }

        //[Ignore]
        [Test]
        public void EnumerateTest() {
            string s = string.Empty;
            foreach(var i in queue) {
                s += i;
            }
            Assert.AreEqual("0123456789", s, "#1 : " + s);
        }

        [Test()]
        public void TryPeekTest() {
            int value;
            queue.TryPeek(out value);
            Assert.AreEqual(0, value, "#1 : " + value);
            queue.TryDequeue(out value);
            Assert.AreEqual(0, value, "#2 : " + value);
            queue.TryDequeue(out value);
            Assert.AreEqual(1, value, "#3 : " + value);
            queue.TryPeek(out value);
            Assert.AreEqual(2, value, "#4 : " + value);
            queue.TryPeek(out value);
            Assert.AreEqual(2, value, "#5 : " + value);
        }

        [Test()]
        public void TryDequeueTest() {
            int value;
            queue.TryPeek(out value);
            Assert.AreEqual(0, value, "#1");
            Assert.IsTrue(queue.TryDequeue(out value), "#2");
            Assert.IsTrue(queue.TryDequeue(out value), "#3");
            Assert.AreEqual(1, value, "#4");
        }

        [Test()]
        public void TryDequeueEmptyTest() {
            int value;
            queue.Clear();
            queue.Enqueue(1);
            Assert.IsTrue(queue.TryDequeue(out value), "#1");
            Assert.IsFalse(queue.TryDequeue(out value), "#2");
            Assert.IsTrue(queue.IsEmpty, "#3");
        }

        [Test]
        public void ToArrayTest() {
            var array = queue.ToArray();
            var s = string.Empty;

            foreach(int i in array) {
                s += i;
            }
            Assert.AreEqual("0123456789", s, "#1 : " + s);

            queue.CopyTo(array, 0);
            s = string.Empty;

            foreach(int i in array) {
                s += i;
            }
            Assert.AreEqual("0123456789", s, "#2 : " + s);
        }
    }
}