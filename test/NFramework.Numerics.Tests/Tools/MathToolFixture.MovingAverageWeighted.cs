using System;
using System.Collections.Generic;
using System.Linq;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.Reflections;
using NUnit.Framework;

namespace NSoft.NFramework.Numerics.Utils {
    [TestFixture]
    public class MathToolFixture_WeightedMovingAverage : AbstractMathToolFixtureBase {
        [Test]
        public void WeightedMovingAverageDoubleTest() {
            var source = new double[] { 1, 2, 3, 6, 7, 9, 11, double.NaN, 10, 11, 12, 12, 13 };
            var expected = new double[] { 4.4, 6, 7.8, 9.7, double.NaN, double.NaN, double.NaN, double.NaN, 11.8, 12.5 };
            var blockSize = 4;

            var actual = MathTool.WeightedMovingAverage(source, blockSize, DoubleWeightGen);

            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());

            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            try {
                source = new double[] { 1, 2, 3, 4, 5 };
                blockSize = 1;
                int x = MathTool.WeightedMovingAverage(source, blockSize).Count();
                Assert.Fail();
            }
            catch(ArgumentException e) {
                Assert.IsTrue(e.ParamName == "blockSize");
            }
            catch(InvalidOperationException) {}
            catch(Exception) {
                Assert.Fail();
            }

            //------------------------------//

            try {
                source = new double[] { 1, 2, 3, 4, 5 };
                blockSize = 15;
                int x = MathTool.WeightedMovingAverage(source, blockSize).Count();
                Assert.Fail();
            }
            catch(ArgumentException e) {
                Assert.IsTrue(e.ParamName == "source");
            }
            catch(InvalidOperationException) {}
            catch(Exception) {
                Assert.Fail();
            }
        }

        [Test]
        public void WeightedMovingAverageNullableDoubleTest() {
            var source = new double?[] { 1, 2, 3, 6, 7, 9, 11, double.NaN, 10, 11, null, 15, 25 };
            var expected = new double?[] { 4.4, 6, 7.8, 9.7, double.NaN, double.NaN, double.NaN, double.NaN, 14, 21 };
            var blockSize = 4;


            IEnumerable<double?> actual = MathTool.WeightedMovingAverage(source, blockSize, DoubleWeightGen);

            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());

            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            try {
                source = new double?[] { 1, 2, 3, 4, 5 };
                blockSize = 1;
                int x = MathTool.WeightedMovingAverage(source, blockSize).Count();
                Assert.Fail();
            }
            catch(ArgumentException e) {
                Assert.IsTrue(e.ParamName == "blockSize");
            }
            catch(InvalidOperationException) {}
            catch(Exception) {
                Assert.Fail();
            }

            //------------------------------//

            try {
                source = new double?[] { 1, 2, 3, 4, 5 };
                blockSize = 15;
                int x = MathTool.WeightedMovingAverage(source, blockSize).Count();
                Assert.Fail();
            }
            catch(ArgumentException e) {
                Assert.IsTrue(e.ParamName == "source");
            }
            catch(InvalidOperationException) {}
            catch(Exception) {
                Assert.Fail();
            }

            //------------------------------//

            source = new double?[] { null, null, null, null, null };
            expected = new double?[] { null, null };
            blockSize = 4;

            actual = MathTool.WeightedMovingAverage(source, blockSize, DoubleWeightGen);

            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());

            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void WeightedMovingAverageFloatTest() {
            var source = new float[] { 1, 2, 3, 6, 7, 9, 11, float.NaN, 10, 11, 12, 12, 13 };
            var expected = new float[] { 4.4f, 6, 7.8f, 9.7f, float.NaN, float.NaN, float.NaN, float.NaN, 11.8f, 12.5f };
            var blockSize = 4;

            var actual = MathTool.WeightedMovingAverage(source, blockSize, FloatWeightGen);
            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            try {
                source = new float[] { 1, 2, 3, 4, 5 };
                blockSize = 1;
                int x = MathTool.WeightedMovingAverage(source, blockSize).Count();
                Assert.Fail();
            }
            catch(ArgumentException e) {
                Assert.IsTrue(e.ParamName == "blockSize");
            }
            catch(InvalidOperationException) {}
            catch(Exception) {
                Assert.Fail();
            }

            //------------------------------//

            try {
                source = new float[] { 1, 2, 3, 4, 5 };
                blockSize = 15;
                int x = MathTool.WeightedMovingAverage(source, blockSize).Count();
                Assert.Fail();
            }
            catch(ArgumentException e) {
                Assert.IsTrue(e.ParamName == "source");
            }
            catch(InvalidOperationException) {}
            catch(Exception) {
                Assert.Fail();
            }
        }

        [Test]
        public void WeightedMovingAverageNullableFloatTest() {
            var source = new float?[] { 1, 2, 3, 6, 7, 9, 11, float.NaN, 10, 11, null, 15, 25 };
            var expected = new float?[] { 4.4f, 6, 7.8f, 9.7f, float.NaN, float.NaN, float.NaN, float.NaN, 14, 21 };
            var blockSize = 4;
            var actual = MathTool.WeightedMovingAverage(source, blockSize, FloatWeightGen);
            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            try {
                source = new float?[] { 1, 2, 3, 4, 5 };
                blockSize = 1;
                int x = MathTool.WeightedMovingAverage(source, blockSize).Count();
                Assert.Fail();
            }
            catch(ArgumentException e) {
                Assert.IsTrue(e.ParamName == "blockSize");
            }
            catch(InvalidOperationException) {}
            catch(Exception) {
                Assert.Fail();
            }

            //------------------------------//

            try {
                source = new float?[] { 1, 2, 3, 4, 5 };
                blockSize = 15;
                int x = MathTool.WeightedMovingAverage(source, blockSize).Count();
                Assert.Fail();
            }
            catch(ArgumentException e) {
                Assert.IsTrue(e.ParamName == "source");
            }
            catch(InvalidOperationException) {}
            catch(Exception) {
                Assert.Fail();
            }

            //------------------------------//

            source = new float?[] { null, null, null, null, null };
            expected = new float?[] { null, null };
            blockSize = 4;


            actual = MathTool.WeightedMovingAverage(source, blockSize, FloatWeightGen);
            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void WeightedMovingAverageDecimalTest() {
            var source = new decimal[] { 1, 2, 3, 6, 7, 9, 11, 8, 10, 11, 12, 12, 13 };
            var expected = new decimal[] { 4.4m, 6, 7.8m, 9.7m, 9, 9.5m, 10.3m, 11.3m, 11.8m, 12.5m };
            var blockSize = 4;

            var actual = MathTool.WeightedMovingAverage(source, blockSize, DecimalWeightGen);
            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            try {
                source = new decimal[] { 1, 2, 3, 4, 5 };
                blockSize = 1;
                int x = MathTool.WeightedMovingAverage(source, blockSize).Count();
                Assert.Fail();
            }
            catch(ArgumentException e) {
                Assert.IsTrue(e.ParamName == "blockSize");
            }
            catch(InvalidOperationException) {}
            catch(Exception) {
                Assert.Fail();
            }

            //------------------------------//

            try {
                source = new decimal[] { 1, 2, 3, 4, 5 };
                blockSize = 15;
                int x = MathTool.WeightedMovingAverage(source, blockSize).Count();
                Assert.Fail();
            }
            catch(ArgumentException e) {
                Assert.IsTrue(e.ParamName == "source");
            }
            catch(InvalidOperationException) {}
            catch(Exception) {
                Assert.Fail();
            }
        }

        [Test]
        public void WeightedMovingAverageNullableDecimalTest() {
            var source = new decimal?[] { 1, 2, 3, 6, 7, 9, 11, 8, 10, 11, null, 15, 25 };
            var expected = new decimal?[] { 4.4m, 6, 7.8m, 9.7m, 9, 9.5m, 10.3m, 10.5m, 14, 21 };
            var blockSize = 4;

            var actual = MathTool.WeightedMovingAverage(source, blockSize, DecimalWeightGen);

            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());

            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            try {
                source = new decimal?[] { 1, 2, 3, 4, 5 };
                blockSize = 1;
                int x = MathTool.WeightedMovingAverage(source, blockSize).Count();
                Assert.Fail();
            }
            catch(ArgumentException e) {
                Assert.IsTrue(e.ParamName == "blockSize");
            }
            catch(InvalidOperationException) {}
            catch(Exception) {
                Assert.Fail();
            }

            //------------------------------//

            try {
                source = new decimal?[] { 1, 2, 3, 4, 5 };
                blockSize = 15;
                int x = MathTool.WeightedMovingAverage(source, blockSize).Count();
                Assert.Fail();
            }
            catch(ArgumentException e) {
                Assert.IsTrue(e.ParamName == "source");
            }
            catch(InvalidOperationException) {}
            catch(Exception) {
                Assert.Fail();
            }

            //------------------------------//

            source = new decimal?[] { null, null, null, null, null };
            expected = new decimal?[] { null, null };
            blockSize = 4;

            actual = MathTool.WeightedMovingAverage(source, blockSize, DecimalWeightGen);
            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void WeightedMovingAverageLongTest() {
            var source = new long[] { 1, 2, 3, 6, 7, 9, 11, 8, 10, 11, 12, 12, 13 };
            var expected = new double[] { 4.4, 6, 7.8, 9.7, 9, 9.5, 10.3, 11.3, 11.8, 12.5 };
            var blockSize = 4;


            var actual = MathTool.WeightedMovingAverage(source, blockSize, DoubleWeightGen);
            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            try {
                source = new long[] { 1, 2, 3, 4, 5 };
                blockSize = 1;
                int x = MathTool.WeightedMovingAverage(source, blockSize).Count();
                Assert.Fail();
            }
            catch(ArgumentException e) {
                Assert.IsTrue(e.ParamName == "blockSize");
            }
            catch(InvalidOperationException) {}
            catch(Exception) {
                Assert.Fail();
            }

            //------------------------------//

            try {
                source = new long[] { 1, 2, 3, 4, 5 };
                blockSize = 15;
                int x = MathTool.WeightedMovingAverage(source, blockSize).Count();
                Assert.Fail();
            }
            catch(ArgumentException e) {
                Assert.IsTrue(e.ParamName == "source");
            }
            catch(InvalidOperationException) {}
            catch(Exception) {
                Assert.Fail();
            }
        }

        [Test]
        public void WeightedMovingAverageNullableLongTest() {
            var source = new long?[] { 1, 2, 3, 6, 7, 9, 11, 8, 10, 11, null, 15, 25 };
            var expected = new double?[] { 4.4, 6, 7.8, 9.7, 9, 9.5, 10.3, 10.5, 14, 21 };
            var blockSize = 4;

            var actual = MathTool.WeightedMovingAverage(source, blockSize, DoubleWeightGen);
            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            try {
                source = new long?[] { 1, 2, 3, 4, 5 };
                blockSize = 1;
                int x = MathTool.WeightedMovingAverage(source, blockSize).Count();
                Assert.Fail();
            }
            catch(ArgumentException e) {
                Assert.IsTrue(e.ParamName == "blockSize");
            }
            catch(InvalidOperationException) {}
            catch(Exception) {
                Assert.Fail();
            }

            //------------------------------//

            try {
                source = new long?[] { 1, 2, 3, 4, 5 };
                blockSize = 15;
                int x = MathTool.WeightedMovingAverage(source, blockSize).Count();
                Assert.Fail();
            }
            catch(ArgumentException e) {
                Assert.IsTrue(e.ParamName == "source");
            }
            catch(InvalidOperationException) {}
            catch(Exception) {
                Assert.Fail();
            }

            //------------------------------//

            source = new long?[] { null, null, null, null, null };
            expected = new double?[] { null, null };
            blockSize = 4;


            actual = MathTool.WeightedMovingAverage(source, blockSize, DoubleWeightGen);
            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void WeightedMovingAverageIntTest() {
            var source = new int[] { 1, 2, 3, 6, 7, 9, 11, 8, 10, 11, 12, 12, 13 };
            var expected = new double[] { 4.4, 6, 7.8, 9.7, 9, 9.5, 10.3, 11.3, 11.8, 12.5 };
            var blockSize = 4;

            var actual = MathTool.WeightedMovingAverage(source, blockSize, DoubleWeightGen);
            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            try {
                source = new int[] { 1, 2, 3, 4, 5 };
                blockSize = 1;
                int x = MathTool.WeightedMovingAverage(source, blockSize).Count();
                Assert.Fail();
            }
            catch(ArgumentException e) {
                Assert.IsTrue(e.ParamName == "blockSize");
            }
            catch(InvalidOperationException) {}
            catch(Exception) {
                Assert.Fail();
            }

            //------------------------------//

            try {
                source = new int[] { 1, 2, 3, 4, 5 };
                blockSize = 15;
                int x = MathTool.WeightedMovingAverage(source, blockSize).Count();
                Assert.Fail();
            }
            catch(ArgumentException e) {
                Assert.IsTrue(e.ParamName == "source");
            }
            catch(InvalidOperationException) {}
            catch(Exception) {
                Assert.Fail();
            }
        }

        [Test]
        public void WeightedMovingAverageNullableIntTest() {
            var source = new int?[] { 1, 2, 3, 6, 7, 9, 11, 8, 10, 11, null, 15, 25 };
            var expected = new double?[] { 4.4, 6, 7.8, 9.7, 9, 9.5, 10.3, 10.5, 14, 21 };
            var blockSize = 4;


            var actual = MathTool.WeightedMovingAverage(source, blockSize, DoubleWeightGen);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            try {
                source = new int?[] { 1, 2, 3, 4, 5 };
                blockSize = 1;
                int x = MathTool.WeightedMovingAverage(source, blockSize).Count();
                Assert.Fail();
            }
            catch(ArgumentException e) {
                Assert.IsTrue(e.ParamName == "blockSize");
            }
            catch(InvalidOperationException) {}
            catch(Exception) {
                Assert.Fail();
            }

            //------------------------------//

            try {
                source = new int?[] { 1, 2, 3, 4, 5 };
                blockSize = 15;
                int x = MathTool.WeightedMovingAverage(source, blockSize).Count();
                Assert.Fail();
            }
            catch(ArgumentException e) {
                Assert.IsTrue(e.ParamName == "source");
            }
            catch(InvalidOperationException) {}
            catch(Exception) {
                Assert.Fail();
            }

            //------------------------------//

            source = new int?[] { null, null, null, null, null };
            expected = new double?[] { null, null };
            blockSize = 4;

            actual = MathTool.WeightedMovingAverage(source, blockSize, DoubleWeightGen);
            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void WeightedMovingAverageDoubleTest2() {
            var source = LinqTool.Generate(1, 7, X => Generator.GenerateDouble(X, -1));
            var expected = new double[] { 2.571, 3.571, 4.571, 5.571, 6.571 };
            const int blockSize = 3;

            var actual = source.WeightedMovingAverage(blockSize, DoubleWeightGen, I => I.Item2).Round(3);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void WeightedMovingAverageNullableDoubleTest2() {
            var source = LinqTool.Generate(1, 7, X => Generator.GenerateNullableDouble(X, -1, -1));
            var expected = new double?[] { 2.571, 3.571, 4.571, 5.571, 6.571 };
            const int blockSize = 3;

            IEnumerable<double?> actual =
                source.WeightedMovingAverage(blockSize, DoubleWeightGen, I => I.Item2).Select(S => S.Value).Round(3).Cast<double?>();
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void WeightedMovingAverageFloatTest2() {
            var source = LinqTool.Generate(1, 7, X => Generator.GenerateFloat(X, -1));
            var expected = new float[] { 2.571f, 3.571f, 4.571f, 5.571f, 6.571f };
            const int blockSize = 3;

            var actual = source.WeightedMovingAverage(blockSize, FloatWeightGen, I => I.Item2).Round(3);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void WeightedMovingAverageNullableFloatTest2() {
            var source = LinqTool.Generate(1, 7, X => Generator.GenerateNullableFloat(X, -1, -1));
            var expected = new float?[] { 2.571f, 3.571f, 4.571f, 5.571f, 6.571f };
            const int blockSize = 3;

            var actual =
                source.WeightedMovingAverage(blockSize, FloatWeightGen, I => I.Item2).Select(S => S.Value).Round(3).Cast<float?>();
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void WeightedMovingAverageDecimalTest2() {
            var source = LinqTool.Generate(1, 7, X => Generator.GenerateDecimal(X));
            var expected = new decimal[] { 2.571m, 3.571m, 4.571m, 5.571m, 6.571m };
            const int blockSize = 3;

            IEnumerable<decimal> actual = source.WeightedMovingAverage(blockSize, DecimalWeightGen, I => I.Item2).Round(3);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void WeightedMovingAverageNullableDecimalTest2() {
            var source = LinqTool.Generate(1, 7, X => Generator.GenerateNullableDecimal(X, -1));
            var expected = new decimal?[] { 2.571m, 3.571m, 4.571m, 5.571m, 6.571m };
            const int blockSize = 3;

            IEnumerable<decimal?> actual =
                source.WeightedMovingAverage(blockSize, DecimalWeightGen, I => I.Item2).Select(S => S.Value).Round(3).Cast<decimal?>();
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void WeightedMovingAverageLongTest2() {
            var source = LinqTool.Generate(1, 7, X => Generator.GenerateLong(X));
            var expected = new double[] { 2.571, 3.571, 4.571, 5.571, 6.571 };
            const int blockSize = 3;

            IEnumerable<double> actual = source.WeightedMovingAverage(blockSize, DoubleWeightGen, I => I.Item2).Round(3);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void WeightedMovingAverageNullableLongTest2() {
            var source = LinqTool.Generate(1, 7, X => Generator.GenerateNullableLong(X, -1));
            var expected = new double?[] { 2.571, 3.571, 4.571, 5.571, 6.571 };
            const int blockSize = 3;

            var actual =
                source.WeightedMovingAverage(blockSize, DoubleWeightGen, I => I.Item2).Select(S => S.Value).Round(3).Cast<double?>();
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void WeightedMovingAverageIntTest2() {
            var source = LinqTool.Generate(1, 7, X => Generator.GenerateInt(X));
            var expected = new double[] { 2.571, 3.571, 4.571, 5.571, 6.571 };
            const int blockSize = 3;

            var actual = source.WeightedMovingAverage(blockSize, DoubleWeightGen, I => I.Item2).Round(3);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void WeightedMovingAverageNullableIntTest2() {
            var source = LinqTool.Generate(1, 7, X => Generator.GenerateNullableInt(X, -1));
            var expected = new double?[] { 2.571, 3.571, 4.571, 5.571, 6.571 };
            const int blockSize = 3;

            var actual =
                source.WeightedMovingAverage(blockSize, DoubleWeightGen, I => I.Item2).Select(S => S.Value).Round(3).Cast<double?>();
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        private double DoubleWeightGen(int index) {
            return index * index;
        }

        private float FloatWeightGen(int index) {
            return index * index;
        }

        private decimal DecimalWeightGen(int index) {
            return index * index;
        }
    }
}