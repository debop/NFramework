using System;
using NSoft.NFramework.Serializations;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Collections {
    [TestFixture]
    public class WeakReferenceMapFixture : AbstractFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;
        private static readonly bool IsInfoEnabled = log.IsInfoEnabled;

        #endregion

        private static WeakReferenceMap CreateWeakReferenceMap() {
            return new WeakReferenceMap();
        }

        [Test]
        public void Basic() {
            var key = new object();
            var value = new object();

            var map = CreateWeakReferenceMap();
            map[key] = value;

            map[key].Should().Be(value);
        }

        [Test]
        public void WeakReferenceGetsFreedButHashCodeRemainsConstant() {
            var obj = new object();
            var wrap = new WeakReferenceWrapper(obj);
            int hashcode = wrap.GetHashCode();

            obj = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();

            wrap.IsAlive.Should().Be.False();
            wrap.Target.Should().Be.Null();
            wrap.GetHashCode().Should().Be(hashcode);
        }

        [Test]
        public void Scavenging() {
            var map = CreateWeakReferenceMap();

            map[new object()] = new object();
            map[new object()] = new object();

            GC.Collect();

            map.Scavenge();
            map.Count.Should().Be(0);
        }

        [Test]
        public void IterationAfterGC() {
            var map = CreateWeakReferenceMap();

            map[new object()] = new object();
            map[new object()] = new object();

            GC.Collect();

            map.Count.Should().Be(2);
            map.GetEnumerator().MoveNext().Should().Be.False();
        }

        [Test]
        public void Iteration() {
            var key = new object();
            var value = new object();

            var map = CreateWeakReferenceMap();
            map[key] = value;

            foreach(var pair in map) {
                Assert.AreSame(key, pair.Key);
                Assert.AreSame(value, pair.Value);
            }
        }

        [Test]
        public void RetrieveNonExistentItem() {
            var map = CreateWeakReferenceMap();
            var obj = map[new object()];

            obj.Should().Be.Null();
        }

        [Test]
        public void WeakRefWrapperEquals() {
            var obj = new object();
            Assert.AreEqual(new WeakReferenceWrapper(obj), new WeakReferenceWrapper(obj));
            Assert.IsFalse(new WeakReferenceWrapper(obj).Equals(null));
            Assert.IsFalse(new WeakReferenceWrapper(obj).Equals(10));
        }

#if !SILVERLIGHT
        [Test]
        public void IsSerializable() {
            var map = CreateWeakReferenceMap();
            map.Add("key", new object());

            map.IsSafelySerializable().Should().Be.True();
        }
#endif
    }
}