using System;
using System.Text;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Collections.Concurrent {
    [TestFixture]
    public class ConcurrentSkipListFixture : AbstractFixture {
        private ConcurrentSkipList<int> _skipList;

        protected override void OnSetUp() {
            base.OnSetUp();

            _skipList = new ConcurrentSkipList<int>();
        }

        private void AddStuff() {
            for(int i = 1; i < 5; i++)
                _skipList.TryAdd(i);
        }

        [Test]
        public void AddTest() {
            _skipList.TryAdd(1).Should().Be.True();
            _skipList.Count.Should().Be(1);
        }

        [Test]
        public void RemoveTest() {
            _skipList.Remove(2).Should().Be.False();
            _skipList.Remove(3).Should().Be.False();

            AddStuff();

            var count = _skipList.Count;

            _skipList.Remove(1).Should().Be.True();
            _skipList.Remove(1).Should().Be.False();
            _skipList.Remove(4).Should().Be.True();

            _skipList.Count.Should().Be(count - 2);
        }

        [Test]
        public void ContainsTest() {
            AddStuff();

            for(int i = 1; i < 5; i++)
                _skipList.Contains(i).Should().Be.True();

            _skipList.Contains(0).Should().Be.False();
            _skipList.Contains(10).Should().Be.False();
        }

        [Test]
        public void EnumerateTest() {
            AddStuff();

            var sb = new StringBuilder();

            foreach(var i in _skipList)
                sb.Append(i);

            sb.ToString().Should().Be("1234");
        }

        [Test]
        public void ToArrayTest() {
            var expected = new[] { 1, 2, 3, 4 };

            AddStuff();

            var array = _skipList.ToArray();
            CollectionAssert.AreEqual(expected, array);

            Array.Clear(array, 0, array.Length);

            _skipList.CopyTo(array, 0);
            CollectionAssert.AreEqual(expected, array);
        }
    }
}