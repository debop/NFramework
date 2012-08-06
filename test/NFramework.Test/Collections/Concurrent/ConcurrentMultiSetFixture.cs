using System.Collections.Generic;
using System.Linq;
using NSoft.NFramework.Reflections;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework.Collections.Concurrent {
    [TestFixture]
    public class ConcurrentMultiSetFixture : AbstractFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private static ConcurrentMultiSet<int, string> CreateMultiSet() {
            return new ConcurrentMultiSet<int, string>
                   {
                       { 0, "zero" },
                       { 1, new[] { "one", "1", "first" } },
                       { 2, "two" },
                       { 3, "three" },
                       { 3, "3" },
                       { 4, "four" }
                   };
        }

        [Test]
        public void Can_IsEmpty() {
            var multiSet = new ConcurrentMultiSet<int, string>();

            Assert.IsTrue(multiSet.IsEmpty);
            Assert.IsTrue(multiSet.IsValueEmpty);

            var multiMap2 = CreateMultiSet();

            Assert.IsFalse(multiMap2.IsEmpty);
            Assert.IsFalse(multiMap2.IsValueEmpty);
        }

        [Test]
        public void AddTest() {
            var multiSet = CreateMultiSet();

            var keyCount = multiSet.KeyCount;
            var valueCount = multiSet.ValueCount;

            multiSet.Add(5, "five", "다섯");

            foreach(var value in multiSet.Values.SelectMany(value => value))
                log.Debug("Value=" + value);

            Assert.AreEqual(keyCount + 1, multiSet.KeyCount);
            Assert.AreEqual(valueCount + 2, multiSet.ValueCount);
        }

        [Test]
        public void RemoveTest() {
            var multiSet = CreateMultiSet();

            var keyCount = multiSet.KeyCount;
            var valueCount = multiSet.ValueCount;

            multiSet.Add(5, "five", "오", "다섯");
            multiSet.Add(5, "일이삼사오");

            log.Debug("Before Remove:" + multiSet.AsString());

            multiSet.Remove(5);

            log.Debug("After Remove:" + multiSet.AsString());

            Assert.AreEqual(keyCount, multiSet.KeyCount);
            Assert.AreEqual(valueCount, multiSet.ValueCount);
        }

        [Test]
        public void ContainsTest() {
            var multiSet = CreateMultiSet();

            Assert.IsTrue(multiSet.ContainsKey(4));
            Assert.IsTrue(multiSet.ContainsValue("four"));

            Assert.IsTrue(multiSet.ContainsValue("zero"));
            Assert.IsTrue(multiSet.ContainsValue("1"));
            Assert.IsTrue(multiSet.ContainsValue("first"));
        }

        [Test]
        public void ItemTest() {
            var multiSet = CreateMultiSet();

            var previousCount = multiSet[10].Count;

            multiSet[10].Add("십");
            multiSet[10].Add("ten");

            Assert.AreEqual(previousCount + 2, multiSet[10].Count);

            if(IsDebugEnabled)
                log.Debug("Item Test : " + multiSet);
        }

        [Test]
        public void CopyToTest() {
            var multiSet = CreateMultiSet();

            var items = new KeyValuePair<int, ISet<string>>[multiSet.Count];

            multiSet.CopyTo(items, 0);

            foreach(KeyValuePair<int, ISet<string>> item in items)
                log.Debug("{0}=[{1}]", item.Key, item.Value.CollectionToString());

            Assert.AreEqual(items.Length, multiSet.Count);
        }

        [Test]
        public void ThreadedTest() {
            TestTool.RunTasks(15,
                              Can_IsEmpty,
                              AddTest,
                              RemoveTest,
                              ContainsTest,
                              ItemTest,
                              CopyToTest);
        }
    }
}