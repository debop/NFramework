using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace NSoft.NFramework.LinqEx.LinqTools {
    [Microsoft.Silverlight.Testing.Tag("Linq")]
    [TestFixture]
    public class LinqToolFixture_Arithmetic {
        [Test]
        public void AddDoubleTest() {
            var leftSequence = new double[] { 1, 2, 3, 4, 5 };
            var rightSequence = new double[] { 1, 2, 3, 4, 5 };
            var expected = new double[] { 2, 4, 6, 8, 10 };

            var actual = LinqTool.Add(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            leftSequence = new double[] { 1, 2, 3, double.NaN, double.PositiveInfinity };
            rightSequence = new double[] { 1, 2, 3, 4, 5 };
            expected = new double[] { 2, 4, 6, double.NaN, double.PositiveInfinity };

            actual = LinqTool.Add(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void AddNullableDoubleTest() {
            var leftSequence = new double?[] { 1, 2, 3, 4, 5 };
            var rightSequence = new double?[] { 1, 2, 3, 4, 5 };
            var expected = new double?[] { 2, 4, 6, 8, 10 };

            var actual = LinqTool.Add(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            leftSequence = new double?[] { 1, 2, 3, double.NaN, double.PositiveInfinity };
            rightSequence = new double?[] { 1, 2, 3, 4, 5 };
            expected = new double?[] { 2, 4, 6, double.NaN, double.PositiveInfinity };

            actual = LinqTool.Add(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            leftSequence = new double?[] { 1, null, 3, double.NaN, double.PositiveInfinity };
            rightSequence = new double?[] { 1, 2, 3, 4, 5 };
            expected = new double?[] { 2, null, 6, double.NaN, double.PositiveInfinity };

            actual = LinqTool.Add(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void AddFloatTest() {
            var leftSequence = new float[] { 1, 2, 3, 4, 5 };
            var rightSequence = new float[] { 1, 2, 3, 4, 5 };
            var expected = new float[] { 2, 4, 6, 8, 10 };

            var actual = LinqTool.Add(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            leftSequence = new float[] { 1, 2, 3, float.NaN, float.PositiveInfinity };
            rightSequence = new float[] { 1, 2, 3, 4, 5 };
            expected = new float[] { 2, 4, 6, float.NaN, float.PositiveInfinity };

            actual = LinqTool.Add(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void AddNullableFloatTest() {
            var leftSequence = new float?[] { 1, 2, 3, 4, 5 };
            var rightSequence = new float?[] { 1, 2, 3, 4, 5 };
            var expected = new float?[] { 2, 4, 6, 8, 10 };

            var actual = LinqTool.Add(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            leftSequence = new float?[] { 1, 2, 3, float.NaN, float.PositiveInfinity };
            rightSequence = new float?[] { 1, 2, 3, 4, 5 };
            expected = new float?[] { 2, 4, 6, float.NaN, float.PositiveInfinity };

            actual = LinqTool.Add(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            leftSequence = new float?[] { 1, null, 3, float.NaN, float.PositiveInfinity };
            rightSequence = new float?[] { 1, 2, 3, 4, 5 };
            expected = new float?[] { 2, null, 6, float.NaN, float.PositiveInfinity };

            actual = LinqTool.Add(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void AddDecimalTest() {
            var leftSequence = new decimal[] { 1, 2, 3, 4, 5 };
            var rightSequence = new decimal[] { 1, 2, 3, 4, 5 };
            var expected = new decimal[] { 2, 4, 6, 8, 10 };

            var actual = LinqTool.Add(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void AddNullableDecimaTest() {
            var leftSequence = new decimal?[] { 1, 2, 3, 4, 5 };
            var rightSequence = new decimal?[] { 1, 2, 3, 4, 5 };
            var expected = new decimal?[] { 2, 4, 6, 8, 10 };

            var actual = LinqTool.Add(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            leftSequence = new decimal?[] { 1, null, 3, 4, 5 };
            rightSequence = new decimal?[] { 1, 2, 3, null, 5 };
            expected = new decimal?[] { 2, null, 6, null, 10 };

            actual = LinqTool.Add(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void AddLongTest() {
            IEnumerable<long> leftSequence = new long[] { 1, 2, 3, 4, 5 };
            IEnumerable<long> rightSequence = new long[] { 1, 2, 3, 4, 5 };
            IEnumerable<long> expected = new long[] { 2, 4, 6, 8, 10 };

            IEnumerable<long> actual = LinqTool.Add(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void AddNullableLongTest() {
            var leftSequence = new long?[] { 1, 2, 3, 4, 5 };
            var rightSequence = new long?[] { 1, 2, 3, 4, 5 };
            var expected = new long?[] { 2, 4, 6, 8, 10 };

            var actual = LinqTool.Add(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            leftSequence = new long?[] { 1, null, 3, 4, 5 };
            rightSequence = new long?[] { 1, 2, 3, null, 5 };
            expected = new long?[] { 2, null, 6, null, 10 };

            actual = LinqTool.Add(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void AddIntTest() {
            var leftSequence = new int[] { 1, 2, 3, 4, 5 };
            var rightSequence = new int[] { 1, 2, 3, 4, 5 };
            var expected = new int[] { 2, 4, 6, 8, 10 };

            var actual = LinqTool.Add(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void AddNullableIntTest() {
            var leftSequence = new int?[] { 1, 2, 3, 4, 5 };
            var rightSequence = new int?[] { 1, 2, 3, 4, 5 };
            var expected = new int?[] { 2, 4, 6, 8, 10 };

            var actual = LinqTool.Add(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            leftSequence = new int?[] { 1, null, 3, 4, 5 };
            rightSequence = new int?[] { 1, 2, 3, null, 5 };
            expected = new int?[] { 2, null, 6, null, 10 };

            actual = LinqTool.Add(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void AddErrTest() {
            try {
                var leftSequence = new bool[] { true, true, true, true, true };
                var rightSequence = new bool[] { true, true, true, true, true };

                bool[] res = leftSequence.Add(rightSequence).ToArray();
                Assert.Fail();
            }
            catch(InvalidOperationException) {}
            catch(Exception) {
                Assert.Fail();
            }
        }

        [Test]
        public void SubtractDoubleTest() {
            var leftSequence = new double[] { 1, 2, 3, 4, 5 };
            var rightSequence = new double[] { 1, 2, 3, 4, 5 };
            var expected = new double[] { 0, 0, 0, 0, 0 };

            var actual = LinqTool.Subtract(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            leftSequence = new double[] { 2, 4, 6, double.NaN, double.PositiveInfinity };
            rightSequence = new double[] { 1, 2, 3, 4, 5 };
            expected = new double[] { 1, 2, 3, double.NaN, double.PositiveInfinity };

            actual = LinqTool.Subtract(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void SubtractNullableDoubleTest() {
            var leftSequence = new double?[] { 2, 4, 6, 8, 10 };
            var rightSequence = new double?[] { 1, 2, 3, 4, 5 };
            var expected = new double?[] { 1, 2, 3, 4, 5 };

            var actual = LinqTool.Subtract(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            leftSequence = new double?[] { 2, 4, 6, double.NaN, double.PositiveInfinity };
            rightSequence = new double?[] { 1, 2, 3, 4, 5 };
            expected = new double?[] { 1, 2, 3, double.NaN, double.PositiveInfinity };

            actual = LinqTool.Subtract(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            leftSequence = new double?[] { 2, 4, 6, double.NaN, double.PositiveInfinity };
            rightSequence = new double?[] { 1, null, 3, 4, 5 };
            expected = new double?[] { 1, null, 3, double.NaN, double.PositiveInfinity };

            actual = LinqTool.Subtract(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void SubtractFloatTest() {
            var leftSequence = new float[] { 1, 2, 3, 4, 5 };
            var rightSequence = new float[] { 1, 2, 3, 4, 5 };
            var expected = new float[] { 0, 0, 0, 0, 0 };

            var actual = LinqTool.Subtract(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            leftSequence = new float[] { 2, 4, 6, float.NaN, float.PositiveInfinity };
            rightSequence = new float[] { 1, 2, 3, 4, 5 };
            expected = new float[] { 1, 2, 3, float.NaN, float.PositiveInfinity };

            actual = LinqTool.Subtract(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void SubtractNullableFloatTest() {
            var leftSequence = new float?[] { 2, 4, 6, 8, 10 };
            var rightSequence = new float?[] { 1, 2, 3, 4, 5 };
            var expected = new float?[] { 1, 2, 3, 4, 5 };

            var actual = LinqTool.Subtract(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            leftSequence = new float?[] { 2, 4, 6, float.NaN, float.PositiveInfinity };
            rightSequence = new float?[] { 1, 2, 3, 4, 5 };
            expected = new float?[] { 1, 2, 3, float.NaN, float.PositiveInfinity };

            actual = LinqTool.Subtract(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            leftSequence = new float?[] { 2, 4, 6, float.NaN, float.PositiveInfinity };
            rightSequence = new float?[] { 1, null, 3, 4, 5 };
            expected = new float?[] { 1, null, 3, float.NaN, float.PositiveInfinity };

            actual = LinqTool.Subtract(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void SubtractDecimalTest() {
            var leftSequence = new decimal[] { 2, 4, 6, 8, 10 };
            var rightSequence = new decimal[] { 1, 2, 3, 4, 5 };
            var expected = new decimal[] { 1, 2, 3, 4, 5 };

            var actual = LinqTool.Subtract(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void SubtractNullableDecimalTest() {
            var leftSequence = new decimal?[] { 2, 4, 6, 8, 10 };
            var rightSequence = new decimal?[] { 1, 2, 3, 4, 5 };
            var expected = new decimal?[] { 1, 2, 3, 4, 5 };

            IEnumerable<decimal?> actual = LinqTool.Subtract(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            leftSequence = new decimal?[] { 2, null, 6, null, 10 };
            rightSequence = new decimal?[] { 1, 2, 3, null, 5 };
            expected = new decimal?[] { 1, null, 3, null, 5 };

            actual = LinqTool.Subtract(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void SubtractLongTest() {
            var leftSequence = new long[] { 2, 4, 6, 8, 10 };
            var rightSequence = new long[] { 1, 2, 3, 4, 5 };
            var expected = new long[] { 1, 2, 3, 4, 5 };

            var actual = LinqTool.Subtract(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void SubtractNullableLongTest() {
            var leftSequence = new long?[] { 2, 4, 6, 8, 10 };
            var rightSequence = new long?[] { 1, 2, 3, 4, 5 };
            var expected = new long?[] { 1, 2, 3, 4, 5 };

            var actual = LinqTool.Subtract(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            leftSequence = new long?[] { 2, null, 6, 8, 10 };
            rightSequence = new long?[] { 1, 2, 3, null, 5 };
            expected = new long?[] { 1, null, 3, null, 5 };

            actual = LinqTool.Subtract(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void SubtractIntTest() {
            var leftSequence = new int[] { 2, 4, 6, 8, 10 };
            var rightSequence = new int[] { 1, 2, 3, 4, 5 };
            var expected = new int[] { 1, 2, 3, 4, 5 };

            var actual = LinqTool.Subtract(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void SubtractNullableIntTest() {
            var leftSequence = new int?[] { 2, 4, 6, 8, 10 };
            var rightSequence = new int?[] { 1, 2, 3, 4, 5 };
            var expected = new int?[] { 1, 2, 3, 4, 5 };

            var actual = LinqTool.Subtract(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            leftSequence = new int?[] { 2, null, 6, 8, 10 };
            rightSequence = new int?[] { 1, 2, 3, null, 5 };
            expected = new int?[] { 1, null, 3, null, 5 };

            actual = LinqTool.Subtract(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void SubtractErrTest() {
            try {
                var leftSequence = new bool[] { true, true, true, true, true };
                var rightSequence = new bool[] { true, true, true, true, true };

                var res = leftSequence.Subtract(rightSequence).ToArray();
                Assert.Fail();
            }
            catch(InvalidOperationException) {}
            catch(Exception) {
                Assert.Fail();
            }
        }

        [Test]
        public void MultiplyDoubleTest() {
            var leftSequence = new double[] { 1, 2, 3, 4, 5 };
            var rightSequence = new double[] { 1, 2, 3, 4, 5 };
            var expected = new double[] { 1, 4, 9, 16, 25 };

            var actual = LinqTool.Multiply(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            leftSequence = new double[] { 1, 2, 3, double.NaN, double.PositiveInfinity };
            rightSequence = new double[] { 1, 2, 3, 4, 5 };
            expected = new double[] { 1, 4, 9, double.NaN, double.PositiveInfinity };

            actual = LinqTool.Multiply(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void MultiplyNullableDoubleTest() {
            var leftSequence = new double?[] { 1, 2, 3, 4, 5 };
            var rightSequence = new double?[] { 1, 2, 3, 4, 5 };
            var expected = new double?[] { 1, 4, 9, 16, 25 };

            var actual = LinqTool.Multiply(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            leftSequence = new double?[] { 1, 2, 3, double.NaN, double.PositiveInfinity };
            rightSequence = new double?[] { 1, 2, 3, 4, 5 };
            expected = new double?[] { 1, 4, 9, double.NaN, double.PositiveInfinity };

            actual = LinqTool.Multiply(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            leftSequence = new double?[] { 1, null, 3, double.NaN, double.PositiveInfinity };
            rightSequence = new double?[] { 1, 2, 3, 4, 5 };
            expected = new double?[] { 1, null, 9, double.NaN, double.PositiveInfinity };

            actual = LinqTool.Multiply(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void MultiplyFloatTest() {
            var leftSequence = new float[] { 1, 2, 3, 4, 5 };
            var rightSequence = new float[] { 1, 2, 3, 4, 5 };
            var expected = new float[] { 1, 4, 9, 16, 25 };

            var actual = LinqTool.Multiply(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            leftSequence = new float[] { 1, 2, 3, float.NaN, float.PositiveInfinity };
            rightSequence = new float[] { 1, 2, 3, 4, 5 };
            expected = new float[] { 1, 4, 9, float.NaN, float.PositiveInfinity };

            actual = LinqTool.Multiply(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void MultiplyNullableFloatTest() {
            var leftSequence = new float?[] { 1, 2, 3, 4, 5 };
            var rightSequence = new float?[] { 1, 2, 3, 4, 5 };
            var expected = new float?[] { 1, 4, 9, 16, 25 };

            var actual = LinqTool.Multiply(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            leftSequence = new float?[] { 1, 2, 3, float.NaN, float.PositiveInfinity };
            rightSequence = new float?[] { 1, 2, 3, 4, 5 };
            expected = new float?[] { 1, 4, 9, float.NaN, float.PositiveInfinity };

            actual = LinqTool.Multiply(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            leftSequence = new float?[] { 1, null, 3, float.NaN, float.PositiveInfinity };
            rightSequence = new float?[] { 1, 2, 3, 4, 5 };
            expected = new float?[] { 1, null, 9, float.NaN, float.PositiveInfinity };

            actual = LinqTool.Multiply(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void MultiplyDecimalTest() {
            var leftSequence = new decimal[] { 1, 2, 3, 4, 5 };
            var rightSequence = new decimal[] { 1, 2, 3, 4, 5 };
            var expected = new decimal[] { 1, 4, 9, 16, 25 };

            var actual = LinqTool.Multiply(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void MultiplyNullableDecimalTest() {
            var leftSequence = new decimal?[] { 1, 2, 3, 4, 5 };
            var rightSequence = new decimal?[] { 1, 2, 3, 4, 5 };
            var expected = new decimal?[] { 1, 4, 9, 16, 25 };

            var actual = LinqTool.Multiply(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            leftSequence = new decimal?[] { 1, null, 3, 4, 5 };
            rightSequence = new decimal?[] { 1, 2, 3, null, 5 };
            expected = new decimal?[] { 1, null, 9, null, 25 };

            actual = LinqTool.Multiply(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void MultiplyLongTest() {
            var leftSequence = new long[] { 1, 2, 3, 4, 5 };
            var rightSequence = new long[] { 1, 2, 3, 4, 5 };
            var expected = new long[] { 1, 4, 9, 16, 25 };

            IEnumerable<long> actual = LinqTool.Multiply(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void MultiplyNullableLongTest() {
            var leftSequence = new long?[] { 1, 2, 3, 4, 5 };
            var rightSequence = new long?[] { 1, 2, 3, 4, 5 };
            var expected = new long?[] { 1, 4, 9, 16, 25 };

            var actual = LinqTool.Multiply(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            leftSequence = new long?[] { 1, null, 3, 4, 5 };
            rightSequence = new long?[] { 1, 2, 3, null, 5 };
            expected = new long?[] { 1, null, 9, null, 25 };

            actual = LinqTool.Multiply(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void MultiplyIntTest() {
            var leftSequence = new int[] { 1, 2, 3, 4, 5 };
            var rightSequence = new int[] { 1, 2, 3, 4, 5 };
            var expected = new int[] { 1, 4, 9, 16, 25 };

            var actual = LinqTool.Multiply(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void MultiplyNullableIntTest() {
            var leftSequence = new int?[] { 1, 2, 3, 4, 5 };
            var rightSequence = new int?[] { 1, 2, 3, 4, 5 };
            var expected = new int?[] { 1, 4, 9, 16, 25 };

            var actual = LinqTool.Multiply(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            leftSequence = new int?[] { 1, null, 3, 4, 5 };
            rightSequence = new int?[] { 1, 2, 3, null, 5 };
            expected = new int?[] { 1, null, 9, null, 25 };

            actual = LinqTool.Multiply(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void MultiplyErrTest() {
            try {
                var leftSequence = new bool[] { true, true, true, true, true };
                var rightSequence = new bool[] { true, true, true, true, true };

                bool[] res = leftSequence.Multiply(rightSequence).ToArray();
                Assert.Fail();
            }
            catch(InvalidOperationException) {}
            catch(Exception) {
                Assert.Fail();
            }
        }

        [Test]
        public void DivideDoubleTest() {
            var leftSequence = new double[] { 1, 2, 3, 4, 5 };
            var rightSequence = new double[] { 1, 2, 3, 4, 5 };
            var expected = new double[] { 1, 1, 1, 1, 1 };

            var actual = LinqTool.Divide(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            leftSequence = new double[] { 1, 2, 3, double.NaN, double.PositiveInfinity };
            rightSequence = new double[] { 1, 2, 3, 4, 5 };
            expected = new double[] { 1, 1, 1, double.NaN, double.PositiveInfinity };

            actual = LinqTool.Divide(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            leftSequence = new double[] { 1, 2, 3, double.NaN, double.PositiveInfinity };
            rightSequence = new double[] { 1, 0, 3, 4, 5 };
            expected = new double[] { 1, double.PositiveInfinity, 1, double.NaN, double.PositiveInfinity };

            actual = LinqTool.Divide(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void DivideNullableDoubleTest() {
            var leftSequence = new double?[] { 1, 2, 3, 4, 5 };
            var rightSequence = new double?[] { 1, 2, 3, 4, 5 };
            var expected = new double?[] { 1, 1, 1, 1, 1 };

            IEnumerable<double?> actual = LinqTool.Divide(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            leftSequence = new double?[] { 1, 2, 3, double.NaN, double.PositiveInfinity };
            rightSequence = new double?[] { 1, 2, 3, 4, 5 };
            expected = new double?[] { 1, 1, 1, double.NaN, double.PositiveInfinity };

            actual = LinqTool.Divide(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            leftSequence = new double?[] { 1, null, 3, double.NaN, double.PositiveInfinity };
            rightSequence = new double?[] { 1, 2, 3, 4, 5 };
            expected = new double?[] { 1, null, 1, double.NaN, double.PositiveInfinity };

            actual = LinqTool.Divide(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            leftSequence = new double?[] { 1, 2, 3, double.NaN, double.PositiveInfinity };
            rightSequence = new double?[] { 1, 0, 3, 4, 5 };
            expected = new double?[] { 1, double.PositiveInfinity, 1, double.NaN, double.PositiveInfinity };

            actual = LinqTool.Divide(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void DivideFloatTest() {
            var leftSequence = new float[] { 1, 2, 3, 4, 5 };
            var rightSequence = new float[] { 1, 2, 3, 4, 5 };
            var expected = new float[] { 1, 1, 1, 1, 1 };

            var actual = LinqTool.Divide(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            leftSequence = new float[] { 1, 2, 3, float.NaN, float.PositiveInfinity };
            rightSequence = new float[] { 1, 2, 3, 4, 5 };
            expected = new float[] { 1, 1, 1, float.NaN, float.PositiveInfinity };

            actual = LinqTool.Divide(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            leftSequence = new float[] { 1, 2, 3, float.NaN, float.PositiveInfinity };
            rightSequence = new float[] { 1, 0, 3, 4, 5 };
            expected = new float[] { 1, float.PositiveInfinity, 1, float.NaN, float.PositiveInfinity };

            actual = LinqTool.Divide(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void DivideNullableFloatTest() {
            var leftSequence = new float?[] { 1, 2, 3, 4, 5 };
            var rightSequence = new float?[] { 1, 2, 3, 4, 5 };
            var expected = new float?[] { 1, 1, 1, 1, 1 };

            var actual = LinqTool.Divide(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            leftSequence = new float?[] { 1, 2, 3, float.NaN, float.PositiveInfinity };
            rightSequence = new float?[] { 1, 2, 3, 4, 5 };
            expected = new float?[] { 1, 1, 1, float.NaN, float.PositiveInfinity };

            actual = LinqTool.Divide(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            leftSequence = new float?[] { 1, null, 3, float.NaN, float.PositiveInfinity };
            rightSequence = new float?[] { 1, 2, 3, 4, 5 };
            expected = new float?[] { 1, null, 1, float.NaN, float.PositiveInfinity };

            actual = LinqTool.Divide(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            leftSequence = new float?[] { 1, 2, 3, float.NaN, float.PositiveInfinity };
            rightSequence = new float?[] { 1, 0, 3, 4, 5 };
            expected = new float?[] { 1, float.PositiveInfinity, 1, float.NaN, float.PositiveInfinity };

            actual = LinqTool.Divide(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void DivideDecimalTest() {
            var leftSequence = new decimal[] { 1, 2, 3, 4, 5 };
            var rightSequence = new decimal[] { 1, 2, 3, 4, 5 };
            var expected = new decimal[] { 1, 1, 1, 1, 1 };

            var actual = LinqTool.Divide(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            try {
                leftSequence = new decimal[] { 1, 2, 3, 4, 5 };
                rightSequence = new decimal[] { 1, 0, 3, 4, 5 };
                expected = new decimal[] { 1, 1, 1, 1, 1 };

                actual = LinqTool.Divide(leftSequence, rightSequence);
                Assert.IsTrue(expected.SequenceEqual(actual));
                int x = actual.Count();
                Assert.Fail();
            }
            catch(DivideByZeroException) {}
            catch(Exception) {
                Assert.Fail();
            }
        }

        [Test]
        public void DivideNullableDecimalTest() {
            var leftSequence = new decimal?[] { 1, 2, 3, 4, 5 };
            var rightSequence = new decimal?[] { 1, 2, 3, 4, 5 };
            var expected = new decimal?[] { 1, 1, 1, 1, 1 };

            var actual = LinqTool.Divide(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            leftSequence = new decimal?[] { 1, null, 3, 4, 5 };
            rightSequence = new decimal?[] { 1, 2, 3, null, 5 };
            expected = new decimal?[] { 1, null, 1, null, 1 };

            actual = LinqTool.Divide(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            try {
                leftSequence = new decimal?[] { 1, 2, 3, 4, 5 };
                rightSequence = new decimal?[] { 1, 0, 3, null, 5 };
                expected = new decimal?[] { 1, null, 1, null, 1 };

                actual = LinqTool.Divide(leftSequence, rightSequence);
                Assert.IsTrue(expected.SequenceEqual(actual));
                int x = actual.Count();
                Assert.Fail();
            }
            catch(DivideByZeroException) {}
            catch(Exception) {
                Assert.Fail();
            }
        }

        [Test]
        public void DivideLongTest() {
            var leftSequence = new long[] { 1, 2, 3, 4, 5 };
            var rightSequence = new long[] { 1, 2, 3, 4, 5 };
            var expected = new long[] { 1, 1, 1, 1, 1 };

            var actual = LinqTool.Divide(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            try {
                leftSequence = new long[] { 1, 2, 3, 4, 5 };
                rightSequence = new long[] { 1, 0, 3, 4, 5 };
                expected = new long[] { 1, 1, 1, 1, 1 };

                actual = LinqTool.Divide(leftSequence, rightSequence);
                int x = actual.Count();
                Assert.Fail();
            }
            catch(DivideByZeroException) {}
            catch(Exception) {
                Assert.Fail();
            }
        }

        [Test]
        public void DivideNullableLongTest() {
            var leftSequence = new long?[] { 1, null, 3, 4, 5 };
            var rightSequence = new long?[] { 1, 2, 3, null, 5 };
            var expected = new long?[] { 1, null, 1, null, 1 };

            var actual = LinqTool.Divide(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            try {
                leftSequence = new long?[] { 1, 2, 3, 4, 5 };
                rightSequence = new long?[] { 1, 0, 3, 4, 5 };
                expected = new long?[] { 1, 1, 1, 1, 1 };

                actual = LinqTool.Divide(leftSequence, rightSequence);
                Assert.IsTrue(expected.SequenceEqual(actual));
                int x = actual.Count();
                Assert.Fail();
            }
            catch(DivideByZeroException) {}
            catch(Exception) {
                Assert.Fail();
            }

            //------------------------------//
            try {
                leftSequence = new long?[] { 1, 2, 3, 4, 5 };
                rightSequence = new long?[] { 1, 0, 3, null, 5 };
                expected = new long?[] { 1, null, 1, null, 1 };

                actual = LinqTool.Divide(leftSequence, rightSequence);
                Assert.IsTrue(expected.SequenceEqual(actual));
                int x = actual.Count();
                Assert.Fail();
            }
            catch(DivideByZeroException) {}
            catch(Exception) {
                Assert.Fail();
            }
        }

        [Test]
        public void DivideIntTest() {
            var leftSequence = new int[] { 1, 2, 3, 4, 5 };
            var rightSequence = new int[] { 1, 2, 3, 4, 5 };
            var expected = new int[] { 1, 1, 1, 1, 1 };

            var actual = LinqTool.Divide(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            try {
                leftSequence = new int[] { 1, 2, 3, 4, 5 };
                rightSequence = new int[] { 1, 0, 3, 4, 5 };
                expected = new int[] { 1, 1, 1, 1, 1 };

                actual = LinqTool.Divide(leftSequence, rightSequence);
                int x = actual.Count();
                Assert.Fail();
            }
            catch(DivideByZeroException) {}
            catch(Exception) {
                Assert.Fail();
            }
        }

        [Test]
        public void DivideNullableIntTest() {
            var leftSequence = new int?[] { 1, 2, 3, 4, 5 };
            var rightSequence = new int?[] { 1, 2, 3, 4, 5 };
            var expected = new int?[] { 1, 1, 1, 1, 1 };

            var actual = LinqTool.Divide(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            leftSequence = new int?[] { 1, null, 3, 4, 5 };
            rightSequence = new int?[] { 1, 2, 3, null, 5 };
            expected = new int?[] { 1, null, 1, null, 1 };

            actual = LinqTool.Divide(leftSequence, rightSequence);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            try {
                leftSequence = new int?[] { 1, 2, 3, 4, 5 };
                rightSequence = new int?[] { 1, 0, 3, null, 5 };
                expected = new int?[] { 1, null, 1, null, 1 };

                actual = LinqTool.Divide(leftSequence, rightSequence);
                Assert.IsTrue(expected.SequenceEqual(actual));
                int x = actual.Count();
                Assert.Fail();
            }
            catch(DivideByZeroException) {}
            catch(Exception) {
                Assert.Fail();
            }
        }

        [Test]
        public void DivideErrTest() {
            try {
                var leftSequence = new bool[] { true, true, true, true, true };
                var rightSequence = new bool[] { true, true, true, true, true };

                bool[] res = leftSequence.Divide(rightSequence).ToArray();
                Assert.Fail();
            }
            catch(InvalidOperationException) {}
            catch(Exception) {
                Assert.Fail();
            }
        }
    }
}