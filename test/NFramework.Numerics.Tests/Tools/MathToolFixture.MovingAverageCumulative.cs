using System.Collections.Generic;
using System.Linq;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.Reflections;
using NUnit.Framework;

namespace NSoft.NFramework.Numerics.Utils {
    [TestFixture]
    public class MathToolFixture_CumulativeMovingAverage : AbstractMathToolFixtureBase {
        [Test]
        public void CumulativeMovingAverageDoubleTest1() {
            var source = new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, double.NaN, 11, 12, 13 };
            var expected = new double[] { 1, 1.5, 2, 2.5, 3, 3.5, 4, 4.5, 5, double.NaN, double.NaN, double.NaN, double.NaN };
            var actual = source.CumulativeMovingAverage().ToList();
            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void CumulativeMovingAverageNullableDoubleTest() {
            var source = new double?[] { 1, 2, null, null, 3, 4, 5, 6, 7, 8, 9, double.NaN, 11, 12, 13 };
            var expected = new double?[] { 1, 1.5, 2, 2.5, 3, 3.5, 4, 4.5, 5, double.NaN, double.NaN, double.NaN, double.NaN };
            var actual = source.CumulativeMovingAverage().ToList();
            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void CumulativeMovingAverageFloatTest1() {
            var source = new float[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, float.NaN, 11, 12, 13 };
            var expected = new float[] { 1, 1.5f, 2, 2.5f, 3, 3.5f, 4, 4.5f, 5, float.NaN, float.NaN, float.NaN, float.NaN };
            var actual = source.CumulativeMovingAverage().ToList();
            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void CumulativeMovingAverageNullableFloatTest() {
            var source = new float?[] { 1, 2, null, null, 3, 4, 5, 6, 7, 8, 9, float.NaN, 11, 12, 13 };
            var expected = new float?[] { 1, 1.5f, 2, 2.5f, 3, 3.5f, 4, 4.5f, 5, float.NaN, float.NaN, float.NaN, float.NaN };
            var actual = source.CumulativeMovingAverage().ToList();
            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void CumulativeMovingAverageDecimalTest1() {
            var source = new decimal[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
            var expected = new decimal[] { 1m, 1.5m, 2m, 2.5m, 3m, 3.5m, 4m, 4.5m, 5m, 5.5m, 6m, 6.5m, 7m };
            var actual = source.CumulativeMovingAverage().ToList();
            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void CumulativeMovingAverageNullableDecimalTest() {
            var source = new decimal?[] { 1, 2, 3, 4, null, null, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
            var expected = new decimal?[] { 1, 1.5m, 2, 2.5m, 3, 3.5m, 4, 4.5m, 5, 5.5m, 6, 6.5m, 7 };
            var actual = source.CumulativeMovingAverage().ToList();
            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void CumulativeMovingAverageLongTest1() {
            var source = new long[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
            var expected = new double[] { 1, 1.5, 2, 2.5, 3, 3.5, 4, 4.5, 5, 5.5, 6, 6.5, 7 };
            var actual = source.CumulativeMovingAverage().ToList();
            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void CumulativeMovingAverageNullableLongTest() {
            var source = new long?[] { 1, 2, null, null, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
            var expected = new double?[] { 1, 1.5, 2, 2.5, 3, 3.5, 4, 4.5, 5, 5.5, 6, 6.5, 7 };
            var actual = source.CumulativeMovingAverage().ToList();
            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void CumulativeMovingAverageIntTest1() {
            var source = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
            var expected = new double[] { 1, 1.5, 2, 2.5, 3, 3.5, 4, 4.5, 5, 5.5, 6, 6.5, 7 };
            var actual = source.CumulativeMovingAverage().ToList();
            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void CumulativeMovingAverageNullableIntTest() {
            var source = new int?[] { 1, 2, null, null, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
            var expected = new double?[] { 1, 1.5, 2, 2.5, 3, 3.5, 4, 4.5, 5, 5.5, 6, 6.5, 7 };
            var actual = source.CumulativeMovingAverage().ToList();

            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void CumulativeMovingAverageDoubleTest2() {
            var source = LinqTool.Generate(1, 14, X => Generator.GenerateDouble(X, -1));
            var expected = LinqTool.Generate(1, 14, X => ((double)X + 1) / 2);
            var actual = source.CumulativeMovingAverage(X => X.Item2).ToList();
            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void CumulativeMovingAverageDoubleTest3() {
            var source = LinqTool.Generate(1, 14, X => Generator.GenerateNullableDouble(X, -1, -1));
            var expected = LinqTool.Generate(1, 14, X => ((double?)X + 1) / 2);
            var actual = source.CumulativeMovingAverage(X => X.Item2).ToList();
            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void CumulativeMovingAverageFloatTest2() {
            var source = LinqTool.Generate(1, 14, X => Generator.GenerateFloat(X, -1));
            var expected = LinqTool.Generate(1, 14, X => ((float)X + 1) / 2);
            var actual = source.CumulativeMovingAverage(X => X.Item2).ToList();
            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void CumulativeMovingAverageFloatTest3() {
            var source = LinqTool.Generate(1, 14, X => Generator.GenerateNullableFloat(X, -1, -1));
            var expected = LinqTool.Generate(1, 14, X => ((float?)X + 1) / 2);
            var actual = source.CumulativeMovingAverage(X => X.Item2).ToList();
            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void CumulativeMovingAverageDecimalTest2() {
            var source = LinqTool.Generate(1, 14, X => Generator.GenerateDecimal(X));
            var expected = LinqTool.Generate(1, 14, X => ((decimal)X + 1) / 2);
            IEnumerable<decimal> actual = source.CumulativeMovingAverage(X => X.Item2).ToList();

            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void CumulativeMovingAverageDecimalTest3() {
            var source = LinqTool.Generate(1, 14, X => Generator.GenerateNullableDecimal(X, -1));
            var expected = LinqTool.Generate<decimal?>(1, 14, X => ((decimal)X + 1) / 2);
            var actual = source.CumulativeMovingAverage(X => X.Item2).ToList();

            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void CumulativeMovingAverageLongTest2() {
            var source = LinqTool.Generate(1, 14, X => Generator.GenerateLong(X));
            var expected = LinqTool.Generate(1, 14, X => ((double)X + 1) / 2);
            var actual = source.CumulativeMovingAverage(X => X.Item2).ToList();

            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void CumulativeMovingAverageLongTest3() {
            var source = LinqTool.Generate(1, 14, X => Generator.GenerateNullableLong(X, -1));
            var expected = LinqTool.Generate(1, 14, X => ((double?)X + 1) / 2);
            var actual = source.CumulativeMovingAverage(X => X.Item2).ToList();

            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void CumulativeMovingAverageIntTest2() {
            var source = LinqTool.Generate(1, 14, X => Generator.GenerateInt(X));
            var expected = LinqTool.Generate(1, 14, X => ((double)X + 1) / 2);
            var actual = source.CumulativeMovingAverage(X => X.Item2).ToList();

            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void CumulativeMovingAverageIntTest3() {
            var source = LinqTool.Generate(1, 14, X => Generator.GenerateNullableInt(X, -1));
            var expected = LinqTool.Generate(1, 14, X => (double?)((X + 1.0) / 2.0));
            var actual = source.CumulativeMovingAverage(X => X.Item2).ToList();

            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());

            Assert.IsTrue(expected.SequenceEqual(actual));
        }
    }
}