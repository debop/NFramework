using System.Collections.Generic;
using System.Linq;
using NSoft.NFramework.Reflections;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework.Collections.Concurrent {
    [TestFixture]
    public class ConcurrentMultiMapFixture : AbstractFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private static ConcurrentMultiMap<int, string> CreateMultiMap() {
            var multiMap = new ConcurrentMultiMap<int, string>
                           {
                               { 0, "zero" },
                               { 1, new[] { "one", "1", "first" } },
                               { 2, "two" },
                               { 3, "three" },
                               { 3, "3" },
                               { 4, "four" }
                           };

            return multiMap;
        }

        [Test]
        public void Can_IsEmpty() {
            var multiMap = new ConcurrentMultiMap<int, string>();

            Assert.IsTrue(multiMap.IsEmpty);
            Assert.IsTrue(multiMap.IsValueEmpty);

            var multiMap2 = CreateMultiMap();

            Assert.IsFalse(multiMap2.IsEmpty);
            Assert.IsFalse(multiMap2.IsValueEmpty);
        }

        [Test]
        public void AddTest() {
            var multiMap = CreateMultiMap();

            var keyCount = multiMap.KeyCount;
            var valueCount = multiMap.ValueCount;

            multiMap.Add(5, new[] { "five", "다섯" });

            foreach(var value in multiMap.Values.SelectMany(value => value))
                log.Debug("Value=" + value);

            Assert.AreEqual(keyCount + 1, multiMap.KeyCount);
            Assert.AreEqual(valueCount + 2, multiMap.ValueCount);
        }

        [Test]
        public void RemoveTest() {
            var multiMap = CreateMultiMap();

            var keyCount = multiMap.KeyCount;
            var valueCount = multiMap.ValueCount;

            multiMap.Add(5, new[] { "five", "오", "다섯" });
            multiMap.Add(5, "일이삼사오");

            log.Debug("Before Remove:" + multiMap.AsString());

            multiMap.Remove(5);

            log.Debug("After Remove:" + multiMap.AsString());

            Assert.AreEqual(keyCount, multiMap.KeyCount);
            Assert.AreEqual(valueCount, multiMap.ValueCount);
        }

        [Test]
        public void ContainsTest() {
            var multiMap = CreateMultiMap();

            Assert.IsTrue(multiMap.ContainsKey(4));
            Assert.IsTrue(multiMap.ContainsValue("four"));

            Assert.IsTrue(multiMap.ContainsValue("zero"));
            Assert.IsTrue(multiMap.ContainsValue("1"));
            Assert.IsTrue(multiMap.ContainsValue("first"));
        }

        [Test]
        public void ItemTest() {
            var multiMap = CreateMultiMap();

            var previousCount = multiMap[10].Count;

            multiMap[10].Add("십");
            multiMap[10].Add("ten");

            Assert.AreEqual(previousCount + 2, multiMap[10].Count);

            if(IsDebugEnabled)
                log.Debug("Item Test : " + multiMap);
        }

        [Test]
        public void CopyToTest() {
            var multiMap = CreateMultiMap();

            var items = new KeyValuePair<int, ICollection<string>>[multiMap.Count];

            multiMap.CopyTo(items, 0);

            foreach(KeyValuePair<int, ICollection<string>> item in items)
                log.Debug("{0}=[{1}]", item.Key, item.Value.CollectionToString());

            Assert.AreEqual(items.Length, multiMap.Count);
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