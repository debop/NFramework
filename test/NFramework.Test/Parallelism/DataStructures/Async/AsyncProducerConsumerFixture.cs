using System.Collections.Concurrent;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Parallelism.DataStructures.Async {
    [Microsoft.Silverlight.Testing.Tag("Parallel")]
    [TestFixture]
    public class AsyncProducerConsumerFixture : ParallelismFixtureBase {
        [Test]
        public void DefaultConstructorTest() {
            var asyncColl = new AsyncProducerConsumerCollection<int>();
            asyncColl.Should().Not.Be.Null();
            asyncColl.Count.Should().Be(0);

            asyncColl.Add(100);
            asyncColl.Count.Should().Be(1);
        }

        [Test]
        public void ConstructorTest() {
            var coll = new ConcurrentQueue<int>();
            for(int i = 0; i < 100; i++)
                coll.Enqueue(i);

            var asyncColl = new AsyncProducerConsumerCollection<int>(coll);
            asyncColl.Should().Not.Be.Null();
            asyncColl.Count.Should().Be(coll.Count);
        }

        [Test]
        public void AddTest() {
            var asyncColl = new AsyncProducerConsumerCollection<int>();

            for(var i = 0; i < 100; i++)
                asyncColl.Add(i);

            asyncColl.Count.Should().Be(100);
        }

        [Test]
        public void TakeTest() {
            var asyncColl = new AsyncProducerConsumerCollection<int>();

            for(var i = 0; i < 100; i++)
                asyncColl.Add(i);

            asyncColl.Count.Should().Be(100);

            for(var i = 100; i > 0; i--)
                asyncColl.Take();

            asyncColl.Count.Should().Be(0);
        }

        [Test]
        public void CopyToTest() {
            var asyncColl = new AsyncProducerConsumerCollection<int>();

            for(var i = 0; i < 100; i++)
                asyncColl.Add(i);

            asyncColl.Count.Should().Be(100);

            var array = new int[100];
            asyncColl.CopyTo(array, 0);

            array.Length.Should().Be(100);
        }
    }
}