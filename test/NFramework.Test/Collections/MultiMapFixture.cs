using System.Collections.Generic;
using NSoft.NFramework.Reflections;
using NUnit.Framework;

namespace NSoft.NFramework.Collections {
    [TestFixture]
    public class MultiMapFixture : AbstractFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;
        private static readonly bool IsInfoEnabled = log.IsInfoEnabled;

        #endregion

        private readonly MultiMap<int, string> _multiMap = new MultiMap<int, string>();

        protected override void OnSetUp() {
            base.OnSetUp();

            AddValues();
        }

        protected override void OnTearDown() {
            _multiMap.Clear();
            base.OnTearDown();
        }

        protected void AddValues() {
            _multiMap.Add(0, "zero");
            _multiMap.Add(1, new[] { "one", "1", "first" });
            _multiMap.Add(2, "two");
            _multiMap.Add(3, "three");
            _multiMap.Add(3, "3");
            _multiMap.Add(4, "four");
        }

        [Test]
        public void Can_IsEmpty() {
            Assert.IsFalse(_multiMap.IsEmpty);
            Assert.IsFalse(_multiMap.IsValueEmpty);
        }

        [Test]
        public void AddTest() {
            var keyCount = _multiMap.KeyCount;
            var valueCount = _multiMap.ValueCount;

            _multiMap.Add(5, new[] { "five", "다섯" });

            Assert.AreEqual(keyCount + 1, _multiMap.KeyCount);
            Assert.AreEqual(valueCount + 2, _multiMap.ValueCount);

            //Assert.AreEqual(6, MultiMap.KeyCount);
            //Assert.AreEqual(10, MultiMap.ValueCount);
        }

        [Test]
        public void RemoveTest() {
            var keyCount = _multiMap.KeyCount;
            var valueCount = _multiMap.ValueCount;

            _multiMap.Add(5, new[] { "five", "오", "다섯" });
            _multiMap.Add(5, "일이삼사오");

            if(IsDebugEnabled)
                log.Debug("Before Remove:" + _multiMap.AsString());

            _multiMap.Remove(5);

            if(IsDebugEnabled)
                log.Debug("After Remove:" + _multiMap.AsString());
            Assert.IsFalse(_multiMap.ContainsKey(5));

            Assert.AreEqual(keyCount, _multiMap.KeyCount);
            Assert.AreEqual(valueCount, _multiMap.ValueCount);
        }

        [Test]
        public void ContainsTest() {
            Assert.IsTrue(_multiMap.ContainsKey(4));
            Assert.IsTrue(_multiMap.ContainsValue("four"));
        }

        [Test]
        public void ItemTest() {
            var previousCount = _multiMap[10].Count;

            _multiMap[10].Add("십");
            _multiMap[10].Add("ten");

            Assert.AreEqual(previousCount + 2, _multiMap[10].Count);

            if(IsDebugEnabled)
                log.Debug("Item Test : " + _multiMap);
        }

        [Test]
        public void CopyToTest() {
            var items = new KeyValuePair<int, IList<string>>[_multiMap.Count];

            _multiMap.CopyTo(items, 0);

            if(IsDebugEnabled)
                foreach(KeyValuePair<int, IList<string>> item in items)
                    log.Debug("{0}=[{1}]", item.Key, item.Value.CollectionToString());

            Assert.AreEqual(items.Length, _multiMap.Count);
        }
    }
}