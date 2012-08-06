using System.Collections.Generic;
using NSoft.NFramework.Reflections;
using NUnit.Framework;

namespace NSoft.NFramework.Collections {
    [TestFixture]
    public class MultiSetFixture : AbstractFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;
        private static readonly bool IsInfoEnabled = log.IsInfoEnabled;

        #endregion

        private readonly MultiSet<int, string> _multiSet = new MultiSet<int, string>();

        protected override void OnSetUp() {
            base.OnSetUp();
            AddValues();
        }

        protected override void OnTearDown() {
            _multiSet.Clear();
            base.OnTearDown();
        }

        protected void AddValues() {
            _multiSet.Add(0, "zero");
            _multiSet.Add(1, new[] { "one", "1", "first", "one", "1", "first" }); // 중복된 것은 무시된다.
            _multiSet.Add(2, "two");
            _multiSet.Add(3, "three");
            _multiSet.Add(3, "3");
            _multiSet.Add(4, "four");
        }

        [Test]
        public void AddTest() {
            var keyCount = _multiSet.KeyCount;
            var valueCount = _multiSet.ValueCount;

            _multiSet.Add(5, new[] { "five", "다섯" });

            Assert.AreEqual(keyCount + 1, _multiSet.KeyCount);
            Assert.AreEqual(valueCount + 2, _multiSet.ValueCount);

            //Assert.AreEqual(6, _multiSet.KeyCount);
            //Assert.AreEqual(10, _multiSet.ValueCount);
        }

        [Test]
        public void RemoveTest() {
            var keyCount = _multiSet.KeyCount;
            var valueCount = _multiSet.ValueCount;

            _multiSet.Add(5, new[] { "five", "오", "다섯" });
            _multiSet.Add(5, "일이삼사오");

            if(IsDebugEnabled)
                log.Debug("Before Remove:" + _multiSet.AsString());

            _multiSet.Remove(5);

            if(IsDebugEnabled)
                log.Debug("After Remove:" + _multiSet.AsString());
            Assert.IsFalse(_multiSet.ContainsKey(5));

            Assert.AreEqual(keyCount, _multiSet.KeyCount);
            Assert.AreEqual(valueCount, _multiSet.ValueCount);
        }

        [Test]
        public void ContainsTest() {
            Assert.IsTrue(_multiSet.ContainsKey(4));
            Assert.IsTrue(_multiSet.ContainsValue("four"));
        }

        [Test]
        public void ItemTest() {
            var previousCount = _multiSet[10].Count;

            _multiSet[10].Add("십");
            _multiSet[10].Add("ten");

            Assert.AreEqual(previousCount + 2, _multiSet[10].Count);

            if(IsDebugEnabled)
                log.Debug("Item Test : " + _multiSet);
        }

        [Test]
        public void CopyToTest() {
            var items = new KeyValuePair<int, HashSet<string>>[_multiSet.Count];

            _multiSet.CopyTo(items, 0);

            if(IsDebugEnabled)
                foreach(KeyValuePair<int, HashSet<string>> item in items)
                    log.Debug("{0}=[{1}]", item.Key, item.Value.CollectionToString());

            Assert.AreEqual(items.Length, _multiSet.Count);
        }
    }
}