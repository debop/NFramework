using System;
using NSoft.NFramework.Reflections;
using NUnit.Framework;

namespace NSoft.NFramework.Collections {
    [TestFixture]
    public class DoubleQueueFixture : AbstractFixture {
        private readonly DoubleQueue<string> _doubleQueue = new DoubleQueue<string>();

        [TestFixtureSetUp]
        public void ClassSetUp() {
            _doubleQueue.EnqueueHead("나");
            _doubleQueue.EnqueueHead("가");
            _doubleQueue.EnqueueTail("다");
        }

        [Test]
        public void EnqueueTest() {
            _doubleQueue.EnqueueHead("나");
            _doubleQueue.EnqueueHead("가");
            _doubleQueue.EnqueueTail("다");

            Console.WriteLine("Queue contents: " + _doubleQueue);
        }

        [Test]
        public void CtorTest() {
            var intDq = new DoubleQueue<int>(new[] { 1, 2, 3, 4, 5 });

            Assert.AreEqual(5, intDq.Count);
            Console.WriteLine("int queue: " + intDq);

            var strDq = new DoubleQueue<string>(new string[0]);

            Assert.AreEqual(0, strDq.Count);
            Console.WriteLine("str queue: " + strDq);
        }

        [Test]
        public void DequeueTest() {
            _doubleQueue.EnqueueHead("나");
            _doubleQueue.EnqueueHead("가");
            _doubleQueue.EnqueueTail("다");

            Assert.AreEqual("가", _doubleQueue.DequeueHead());
            Assert.AreEqual("다", _doubleQueue.DequeueTail());
            Console.WriteLine("Queue contents: " + _doubleQueue);
        }

        [Test]
        public void PeekTest() {
            _doubleQueue.EnqueueHead("나");
            _doubleQueue.EnqueueHead("가");
            _doubleQueue.EnqueueTail("다");

            Assert.AreEqual("가", _doubleQueue.PeekHead());
            Assert.AreEqual("다", _doubleQueue.PeekTail());
            Console.WriteLine("Queue contents: " + _doubleQueue);
        }

        [Test]
        public void CopyToTest() {
            _doubleQueue.Clear();

            _doubleQueue.EnqueueHead("나");
            _doubleQueue.EnqueueHead("가");
            _doubleQueue.EnqueueTail("다");

            var array = new string[_doubleQueue.Count];

            _doubleQueue.CopyTo(array);
            foreach(string s in array)
                Console.WriteLine(s + ",");

            Assert.AreEqual(3, array.Length);
        }

        [Test]
        public void CloneTest() {
            var cloned = (DoubleQueue<string>)_doubleQueue.Clone();

            Console.Write(cloned.ToString());
            Assert.IsTrue(cloned.Contains("가"));
        }

        [Test]
        public void ReverseTest() {
            //Console.Write("InOrder: ");
            //foreach (string s in _doubleQueue.InOrderEnumerable)
            //    Console.Write(s + ",");

            //Console.WriteLine("");
            //Console.Write("ReverseOrder: ");
            //foreach (string s in _doubleQueue.ReverseOrderEnumerable)
            //    Console.Write(s + ",");

            //_doubleQueue.Reverse();
            //Console.WriteLine("");
            //Console.WriteLine("Reverse : " + _doubleQueue);

            Console.WriteLine("InOrder: " + _doubleQueue.CollectionToString());
            Console.WriteLine("ReverseOrder: " + _doubleQueue.CollectionToString());

            _doubleQueue.Reverse();
            Console.WriteLine("Reversed: " + _doubleQueue);
        }
    }
}