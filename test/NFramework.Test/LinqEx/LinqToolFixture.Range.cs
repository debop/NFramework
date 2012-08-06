using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace NSoft.NFramework.LinqEx.LinqTools {
    [Microsoft.Silverlight.Testing.Tag("Linq")]
    [TestFixture]
    public class LinqToolFixture_Ragne {
        [Test]
        public void RangeTest1() {
            IEnumerable<int> expected = new int[] { 1, 3, 5, 7, 9, 11 };
            IEnumerable<int> actual = LinqTool.Range(1, 12, 2);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void RangeTest2() {
            IEnumerable<double> expected = new double[] { 1.25, 2.5, 3.75, 5, 6.25, 7.5 };
            IEnumerable<double> actual = LinqTool.Range(1.25, 7.555, 1.25);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void RangeTest3() {
            try {
                IEnumerable<bool> source = new bool[] { true, true, true, true };
                bool[] actual = LinqTool.Range(false, true, true).ToArray();
                Assert.Fail();
            }
            catch(InvalidOperationException) {
                //Assert.IsTrue(e.Message == "Range<T> cannot be invoked using 'Boolean' type, only numeric values are valid.");
            }
            catch(Exception ex) {
                Assert.Fail(ex.Message);
            }
        }
    }
}