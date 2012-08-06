using System.Collections.Generic;
using System.Linq;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.Reflections;
using NUnit.Framework;

namespace NSoft.NFramework.Numerics.Utils {
    [TestFixture]
    public class MathToolFixture_MovingSum : AbstractMathToolFixtureBase {
        [Test]
        public void MovingSumDoubleTest() {
            var source = new double[] { 1, double.NaN, 3, 4, 5, 6, 7, 8, 9, double.NaN, 11, 12, 13 };
            var expected = new double[] { double.NaN, double.NaN, 12, 15, 18, 21, 24, double.NaN, double.NaN, double.NaN, 36 };
            var blockSize = 3;

            var actual = MathTool.MovingSum(source, blockSize);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            source = new double[] { 1, 2, 3, 4 };
            expected = new double[] { 10 };
            blockSize = 8;

            actual = MathTool.MovingSum(source, blockSize);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            source = new double[] { 1, double.NaN, 3, 4 };
            expected = new double[] { double.NaN, };
            blockSize = 8;

            actual = MathTool.MovingSum(source, blockSize);

            if(IsDebugEnabled)
                log.Debug("MovingSum: expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());

            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void MovingSumNullableDoubleTest() {
            var source = new double?[] { 1, null, 3, 4, 5, 6, 7, 8, 9, double.NaN, 11, 12, 13 };
            var expected = new double?[] { 4, 7, 12, 15, 18, 21, 24, double.NaN, double.NaN, double.NaN, 36 };
            var blockSize = 3;

            var actual = MathTool.MovingSum(source, blockSize);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            source = new double?[] { 1, 2, null, 4 };
            expected = new double?[] { 7 };
            blockSize = 8;

            actual = MathTool.MovingSum(source, blockSize);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            source = new double?[] { 1, double.NaN, null, 4 };
            expected = new double?[] { double.NaN };
            blockSize = 8;

            actual = MathTool.MovingSum(source, blockSize);

            if(IsDebugEnabled)
                log.Debug("MovingSum: expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());

            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void MovingSumFloatTest() {
            var source = new float[] { 1, float.NaN, 3, 4, 5, 6, 7, 8, 9, float.NaN, 11, 12, 13 };
            var expected = new float[] { float.NaN, float.NaN, 12, 15, 18, 21, 24, float.NaN, float.NaN, float.NaN, 36 };
            var blockSize = 3;

            var actual = MathTool.MovingSum(source, blockSize);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            source = new float[] { 1, 2, 3, 4 };
            expected = new float[] { 10 };
            blockSize = 8;

            actual = MathTool.MovingSum(source, blockSize);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            source = new float[] { 1, 2, float.NaN, 4 };
            expected = new float[] { float.NaN };
            blockSize = 8;

            actual = MathTool.MovingSum(source, blockSize);

            if(IsDebugEnabled)
                log.Debug("MovingSum: expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());

            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void MovingSumNullableFloatTest() {
            var source = new float?[] { 1, null, 3, 4, 5, 6, 7, 8, 9, float.NaN, 11, 12, 13 };
            var expected = new float?[] { 4, 7, 12, 15, 18, 21, 24, float.NaN, float.NaN, float.NaN, 36 };
            var blockSize = 3;

            var actual = MathTool.MovingSum(source, blockSize);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            source = new float?[] { 1, 2, null, 4 };
            expected = new float?[] { 7 };
            blockSize = 8;

            actual = MathTool.MovingSum(source, blockSize);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            source = new float?[] { 1, float.NaN, null, 4 };
            blockSize = 8;
            expected = new float?[] { float.NaN };

            actual = MathTool.MovingSum(source, blockSize);

            if(IsDebugEnabled)
                log.Debug("MovingSum: expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());

            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void MovingSumDecimalTest() {
            var source = new decimal[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
            var expected = new decimal[] { 6, 9, 12, 15, 18, 21, 24, 27, 30, 33, 36 };
            var blockSize = 3;

            var actual = MathTool.MovingSum(source, blockSize);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            source = new decimal[] { 1, 2, 3, 4 };
            expected = new decimal[] { 10 };
            blockSize = 8;

            actual = MathTool.MovingSum(source, blockSize);

            if(IsDebugEnabled)
                log.Debug("MovingSum: expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());

            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void MovingSumNullableDecimalTest() {
            var source = new decimal?[] { 1, null, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
            var expected = new decimal?[] { 4, 7, 12, 15, 18, 21, 24, 27, 30, 33, 36 };
            var blockSize = 3;

            var actual = MathTool.MovingSum(source, blockSize);

            if(IsDebugEnabled)
                log.Debug("MovingSum: expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());

            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            source = new decimal?[] { 1, 2, null, 4 };
            expected = new decimal?[] { 7 };
            blockSize = 8;

            actual = MathTool.MovingSum(source, blockSize);

            if(IsDebugEnabled)
                log.Debug("MovingSum: expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void MovingSumLongTest() {
            var source = new long[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
            var expected = new long[] { 6, 9, 12, 15, 18, 21, 24, 27, 30, 33, 36 };
            var blockSize = 3;

            var actual = MathTool.MovingSum(source, blockSize);

            if(IsDebugEnabled)
                log.Debug("MovingSum: expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());

            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            source = new long[] { 1, 2, 3, 4 };
            expected = new long[] { 10 };
            blockSize = 8;

            actual = MathTool.MovingSum(source, blockSize);

            if(IsDebugEnabled)
                log.Debug("MovingSum: expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());

            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void MovingSumNullableLongTest() {
            var source = new long?[] { 1, null, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
            var expected = new long?[] { 4, 7, 12, 15, 18, 21, 24, 27, 30, 33, 36 };
            var blockSize = 3;

            var actual = MathTool.MovingSum(source, blockSize);

            if(IsDebugEnabled)
                log.Debug("MovingSum: expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());

            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            source = new long?[] { 1, 2, null, 4 };
            expected = new long?[] { 7 };
            blockSize = 8;

            actual = MathTool.MovingSum(source, blockSize);

            if(IsDebugEnabled)
                log.Debug("MovingSum: expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());

            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void MovingSumIntTest() {
            var source = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
            var expected = new int[] { 6, 9, 12, 15, 18, 21, 24, 27, 30, 33, 36 };
            var blockSize = 3;

            var actual = MathTool.MovingSum(source, blockSize);

            if(IsDebugEnabled)
                log.Debug("MovingSum: expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());

            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            source = new int[] { 1, 2, 3, 4 };
            expected = new int[] { 10 };
            blockSize = 8;

            actual = MathTool.MovingSum(source, blockSize);

            if(IsDebugEnabled)
                log.Debug("MovingSum: expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());

            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void MovingSumNullableIntTest() {
            var source = new int?[] { 1, null, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
            var expected = new int?[] { 4, 7, 12, 15, 18, 21, 24, 27, 30, 33, 36 };
            var blockSize = 3;

            var actual = MathTool.MovingSum(source, blockSize);

            if(IsDebugEnabled)
                log.Debug("MovingSum: expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());

            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            source = new int?[] { 1, 2, null, 4 };
            expected = new int?[] { 7 };
            blockSize = 8;

            actual = MathTool.MovingSum(source, blockSize);

            if(IsDebugEnabled)
                log.Debug("MovingSum: expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());

            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void MovingSumDoubleTest2() {
            var source = LinqTool.Generate(1, 13, X => Generator.GenerateDouble(X, 10));
            var expected = new double[] { 6, 9, 12, 15, 18, 21, 24, double.NaN, double.NaN, double.NaN, 36 };
            const int blockSize = 3;

            var actual = MathTool.MovingSum(source, blockSize, S => S.Item2);

            if(IsDebugEnabled)
                log.Debug("MovingSum: expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());

            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void MovingSumDoubleTest3() {
            var source = LinqTool.Generate(1, 13, X => Generator.GenerateNullableDouble(X, 10, 2));
            var expected = new double?[] { 4, 7, 12, 15, 18, 21, 24, double.NaN, double.NaN, double.NaN, 36 };
            const int blockSize = 3;

            var actual = MathTool.MovingSum(source, blockSize, S => S.Item2);

            if(IsDebugEnabled)
                log.Debug("MovingSum: expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());

            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void MovingSumFloatTest2() {
            var source = LinqTool.Generate(1, 13, X => Generator.GenerateFloat(X, 10));
            const int blockSize = 3;
            var expected = new float[] { 6, 9, 12, 15, 18, 21, 24, float.NaN, float.NaN, float.NaN, 36 };
            var actual = MathTool.MovingSum(source, blockSize, S => S.Item2);

            if(IsDebugEnabled)
                log.Debug("MovingSum: expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());

            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void MovingSumFloatTest3() {
            var source = LinqTool.Generate(1, 13, X => Generator.GenerateNullableFloat(X, 10, 2));
            const int blockSize = 3;
            var expected = new float?[] { 4, 7, 12, 15, 18, 21, 24, float.NaN, float.NaN, float.NaN, 36 };
            var actual = MathTool.MovingSum(source, blockSize, S => S.Item2);

            if(IsDebugEnabled)
                log.Debug("MovingSum: expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());

            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void MovingSumDecimalTest2() {
            var source = LinqTool.Generate(1, 13, X => Generator.GenerateDecimal(X));
            const int blockSize = 3;
            var expected = new decimal[] { 6, 9, 12, 15, 18, 21, 24, 27, 30, 33, 36 };
            IEnumerable<decimal> actual = MathTool.MovingSum(source, blockSize, S => S.Item2);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void MovingSumDecimalTest3() {
            var source = LinqTool.Generate(1, 13, X => Generator.GenerateNullableDecimal(X, 2));
            const int blockSize = 3;
            var expected = new decimal?[] { 4, 7, 12, 15, 18, 21, 24, 27, 30, 33, 36 };
            var actual = MathTool.MovingSum(source, blockSize, S => S.Item2);

            if(IsDebugEnabled)
                log.Debug("MovingSum: expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void MovingSumLongTest2() {
            var source = LinqTool.Generate(1, 13, X => Generator.GenerateLong(X));
            const int blockSize = 3;
            var expected = new long[] { 6, 9, 12, 15, 18, 21, 24, 27, 30, 33, 36 };
            var actual = MathTool.MovingSum(source, blockSize, S => S.Item2);

            if(IsDebugEnabled)
                log.Debug("MovingSum: expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());

            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void MovingSumLongTest3() {
            var source = LinqTool.Generate(1, 13, X => Generator.GenerateNullableLong(X, 2));
            const int blockSize = 3;
            var expected = new long?[] { 4, 7, 12, 15, 18, 21, 24, 27, 30, 33, 36 };
            var actual = MathTool.MovingSum(source, blockSize, S => S.Item2);

            if(IsDebugEnabled)
                log.Debug("MovingSum: expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());

            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void MovingSumIntTest2() {
            var source = LinqTool.Generate(1, 13, X => Generator.GenerateInt(X));
            const int blockSize = 3;
            var expected = new int[] { 6, 9, 12, 15, 18, 21, 24, 27, 30, 33, 36 };
            var actual = MathTool.MovingSum(source, blockSize, S => S.Item2);

            if(IsDebugEnabled)
                log.Debug("MovingSum: expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());

            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void MovingSumIntTest3() {
            var source = LinqTool.Generate(1, 13, X => Generator.GenerateNullableInt(X, 2));
            const int blockSize = 3;
            var expected = new int?[] { 4, 7, 12, 15, 18, 21, 24, 27, 30, 33, 36 };
            var actual = MathTool.MovingSum(source, blockSize, S => S.Item2);

            if(IsDebugEnabled)
                log.Debug("MovingSum: expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());

            Assert.IsTrue(expected.SequenceEqual(actual));
        }
    }
}