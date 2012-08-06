using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace NSoft.NFramework.Collections.PowerCollections {
    [TestFixture]
    public class UtilFixture {
#pragma warning disable 649
        private struct StructType {
            public int i;
        }

        private class ClassType {
            public int i;
        }

        public struct CloneableStruct : ICloneable {
            public int value;
            public int tweak;

            public CloneableStruct(int v) {
                value = v;
                tweak = 1;
            }

            public object Clone() {
                CloneableStruct newStruct;
                newStruct.value = value;
                newStruct.tweak = tweak + 1;
                return newStruct;
            }

            public bool Identical(CloneableStruct other) {
                return value == other.value && tweak == other.tweak;
            }

            public override bool Equals(object other) {
                if(!(other is CloneableStruct))
                    return false;

                CloneableStruct o = (CloneableStruct)other;

                return (o.value == value);
            }

            public override int GetHashCode() {
                return value.GetHashCode();
            }
        }

#pragma warning restore 649

#if !SILVERLIGHT
        [Test]
        public void IsCloneableType() {
            bool isCloneable, isValue;

            isCloneable = Util.IsCloneableType(typeof(int), out isValue);
            Assert.IsTrue(isCloneable);
            Assert.IsTrue(isValue);

            isCloneable = Util.IsCloneableType(typeof(ICloneable), out isValue);
            Assert.IsTrue(isCloneable);
            Assert.IsFalse(isValue);

            isCloneable = Util.IsCloneableType(typeof(StructType), out isValue);
            Assert.IsTrue(isCloneable);
            Assert.IsTrue(isValue);

            isCloneable = Util.IsCloneableType(typeof(ClassType), out isValue);
            Assert.IsFalse(isCloneable);
            Assert.IsFalse(isValue);

            isCloneable = Util.IsCloneableType(typeof(ArrayList), out isValue);
            Assert.IsTrue(isCloneable);
            Assert.IsFalse(isValue);

            isCloneable = Util.IsCloneableType(typeof(CloneableStruct), out isValue);
            Assert.IsTrue(isCloneable);
            Assert.IsFalse(isValue);

            // isCloneable = Util.IsCloneableType(typeof(OrderedDictionary<int, double>), out isValue);
            // Assert.IsTrue(isCloneable);
            // Assert.IsFalse(isValue);
        }
#endif

        [Test]
        public void WrapEnumerable() {
            IEnumerable<int> enum1 = new List<int>(new int[] { 1, 4, 5, 6, 9, 1 });
            IEnumerable<int> enum2 = Util.CreateEnumerableWrapper(enum1);
            InterfaceTests.TestEnumerableElements(enum2, new int[] { 1, 4, 5, 6, 9, 1 });
        }

        [Test]
        public void TestGetHashCode() {
            int result = Util.GetHashCode("foo", EqualityComparer<string>.Default);
            Assert.AreEqual(result, "foo".GetHashCode());

            result = Util.GetHashCode(null, EqualityComparer<string>.Default);
            Assert.AreEqual(result, 0x1786E23C);

            var r1 = Util.GetHashCode("Banana", StringComparer.InvariantCultureIgnoreCase);
            var r2 = Util.GetHashCode("banANA", StringComparer.InvariantCultureIgnoreCase);
            Assert.AreEqual(r1, r2);
        }
    }
}