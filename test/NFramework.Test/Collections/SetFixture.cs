using NUnit.Framework;

namespace NSoft.NFramework.Collections {
    [TestFixture]
    public class SetFixture : AbstractFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;
        private static readonly bool IsInfoEnabled = log.IsInfoEnabled;

        #endregion

        private readonly Set<int> _set = new Set<int>(1, 2, 3, 4, 5);
        private readonly Set<int> _even = new Set<int>(0, 2, 4, 6, 8);
        private readonly Set<int> _odd = new Set<int>(1, 3, 5, 7, 9);

        [Test]
        public void UnionTest() {
            var set = _even.Union(_odd);

            if(IsDebugEnabled)
                log.Debug("Union:" + set.ToString(true));

            Assert.AreEqual(10, set.Count);
        }

        [Test]
        public void IntersectTest() {
            var set = _even.Intersection(_set);

            if(IsDebugEnabled)
                log.Debug("Intersection:" + set.ToString(true));

            Assert.AreEqual(2, set.Count);
        }

        [Test]
        public void DifferenceTest() {
            var set = _even.Difference(_odd);

            if(IsDebugEnabled)
                log.Debug("Diffence:" + set.ToString(true));

            Assert.AreEqual(5, set.Count);
        }

        [Test]
        public void ExclusiveOrSet() {
            var set = _even.ExclusiveOr(_odd);

            if(IsDebugEnabled)
                log.Debug("Complentary:" + set.ToString(true));

            Assert.AreEqual(10, set.Count);
        }

        [Test]
        public void IsSubsetTest() {
            var set = _even.Clone();

            Assert.IsTrue(set.IsSubset(_even));
            Assert.IsFalse(set.IsProperSubset(_even));

            set.Remove(0);

            Assert.IsTrue(set.IsSubset(_even));
            Assert.IsTrue(set.IsProperSubset(_even));
        }

        [Test]
        public void IsSupersetTest() {
            var set = _even.Clone();

            Assert.IsTrue(set.IsSuperset(_even));
            Assert.IsFalse(set.IsProperSuperset(_even));

            set.Add(10);

            Assert.IsTrue(set.IsSuperset(_even));
            Assert.IsTrue(set.IsProperSuperset(_even));
        }

        [Test]
        public void EqualsTeset() {
            Assert.IsFalse(_even.Equals(_odd));
            Assert.IsTrue(_even.Equals(_even.Clone()));

            Assert.IsFalse(_even == _odd);
            Assert.IsTrue(_even == _even.Clone());
        }
    }
}