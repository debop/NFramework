using System;
using System.Collections.Generic;
using System.Linq;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.Reflections;
using NUnit.Framework;

namespace NSoft.NFramework.Numerics.Utils {
    [TestFixture]
    public class MathToolFixture_StandardMovingAverage : AbstractMathToolFixtureBase {
        [Test]
        public void StandardMovingAverageDoubleTest() {
            var source = new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, double.NaN, 11, 12, 13 };
            var expected = new double[] { 2, 3, 4, 5, 6, 7, 8, double.NaN, double.NaN, double.NaN, 12 };
            var blockSize = 3;

            var actual = MathTool.StandardMovingAverage(source, blockSize);
            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            source = new double[] { 1, 2, 3, 4, 5 };
            expected = new double[] { 3 };
            blockSize = 8;

            actual = MathTool.StandardMovingAverage(source, blockSize);
            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            source = new double[] { 1, double.NaN, 3, 4, 5 };
            expected = new double[] { double.NaN };
            blockSize = 8;

            actual = MathTool.StandardMovingAverage(source, blockSize);
            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            try {
                source = new double[] { 1, 2, 3, 4, 5 };
                blockSize = 1;

                int x = MathTool.StandardMovingAverage(source, blockSize).Count();
                Assert.Fail();
            }
            catch(ArgumentException e) {
                Assert.IsTrue(e.ParamName == "blockSize");
            }
            catch(InvalidOperationException) {}
            catch(Exception) {
                Assert.Fail();
            }
        }

        [Test]
        public void StandardMovingAverageNullableDoubleTest() {
            var source = new double?[] { 1, null, 3, 4, 5, 6, 7, 8, 9, double.NaN, 11, 12, 13 };
            var expected = new double?[] { 2, 3.5, 4, 5, 6, 7, 8, double.NaN, double.NaN, double.NaN, 12 };
            var blockSize = 3;

            var actual = MathTool.StandardMovingAverage(source, blockSize);
            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            source = new double?[] { 1, 2, null, 6 };
            expected = new double?[] { 3 };
            blockSize = 8;

            actual = MathTool.StandardMovingAverage(source, blockSize);
            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            source = new double?[] { 1, double.NaN, null, 6 };
            expected = new double?[] { double.NaN };
            blockSize = 8;
            actual = MathTool.StandardMovingAverage(source, blockSize);
            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//


            try {
                source = new double?[] { 1, 2, 3, 4, 5 };
                blockSize = 1;
                int x = MathTool.StandardMovingAverage(source, blockSize).Count();
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

            source = new double?[] { null, null, null, null };
            expected = new double?[] { null };
            blockSize = 8;

            actual = MathTool.StandardMovingAverage(source, blockSize);
            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            source = new double?[] { 1, null, null, null };
            expected = new double?[] { 1, null };
            blockSize = 3;

            actual = MathTool.StandardMovingAverage(source, blockSize);
            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void StandardMovingAverageFloatTest() {
            var source = new float[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, float.NaN, 11, 12, 13 };
            var expected = new float[] { 2, 3, 4, 5, 6, 7, 8, float.NaN, float.NaN, float.NaN, 12 };
            var blockSize = 3;

            var actual = MathTool.StandardMovingAverage(source, blockSize);
            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            source = new float[] { 1, 2, 3, 4, 5 };
            expected = new float[] { 3 };
            blockSize = 8;

            actual = MathTool.StandardMovingAverage(source, blockSize);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            source = new float[] { 1, float.NaN, 3, 4, 5 };
            expected = new float[] { float.NaN };
            blockSize = 8;

            actual = MathTool.StandardMovingAverage(source, blockSize);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            try {
                source = new float[] { 1, 2, 3, 4, 5 };
                blockSize = 1;
                int x = MathTool.StandardMovingAverage(source, blockSize).Count();
                Assert.Fail();
            }
            catch(ArgumentException e) {
                Assert.IsTrue(e.ParamName == "blockSize");
            }
            catch(InvalidOperationException) {}
            catch(Exception) {
                Assert.Fail();
            }
        }

        [Test]
        public void StandardMovingAverageNullableFloatTest() {
            var source = new float?[] { 1, null, 3, 4, 5, 6, 7, 8, 9, float.NaN, 11, 12, 13 };
            var expected = new float?[] { 2, 3.5f, 4, 5, 6, 7, 8, float.NaN, float.NaN, float.NaN, 12 };
            var blockSize = 3;

            var actual = MathTool.StandardMovingAverage(source, blockSize);
            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            source = new float?[] { 1, 2, null, 6 };
            expected = new float?[] { 3 };
            blockSize = 8;

            actual = MathTool.StandardMovingAverage(source, blockSize);
            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            source = new float?[] { 1, float.NaN, null, 6 };
            expected = new float?[] { float.NaN };
            blockSize = 8;

            actual = MathTool.StandardMovingAverage(source, blockSize);
            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            try {
                source = new float?[] { 1, 2, 3, 4, 5 };
                blockSize = 1;
                int x = MathTool.StandardMovingAverage(source, blockSize).Count();
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

            source = new float?[] { null, null, null, null };
            expected = new float?[] { null };
            blockSize = 8;

            actual = MathTool.StandardMovingAverage(source, blockSize);
            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            source = new float?[] { 1, null, null, null };
            expected = new float?[] { 1, null };
            blockSize = 3;

            actual = MathTool.StandardMovingAverage(source, blockSize);
            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void StandardMovingAverageDecimalTest() {
            var source = new decimal[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
            var blockSize = 3;
            var expected = new decimal[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

            var actual = MathTool.StandardMovingAverage(source, blockSize);
            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            source = new decimal[] { 1, 2, 3, 4, 5 };
            expected = new decimal[] { 3 };
            blockSize = 8;

            actual = MathTool.StandardMovingAverage(source, blockSize);
            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            try {
                source = new decimal[] { 1, 2, 3, 4, 5 };
                blockSize = 1;
                int x = MathTool.StandardMovingAverage(source, blockSize).Count();
                Assert.Fail();
            }
            catch(ArgumentException e) {
                Assert.IsTrue(e.ParamName == "blockSize");
            }
            catch(InvalidOperationException) {}

            catch(Exception) {
                Assert.Fail();
            }
        }

        [Test]
        public void StandardMovingAverageNullableDecimalTest() {
            var source = new decimal?[] { 1, null, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
            var expected = new decimal?[] { 2, 3.5m, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var blockSize = 3;

            var actual = MathTool.StandardMovingAverage(source, blockSize);
            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            source = new decimal?[] { 1, 2, null, 6 };
            expected = new decimal?[] { 3 };
            blockSize = 8;

            actual = MathTool.StandardMovingAverage(source, blockSize);
            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());

            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            try {
                source = new decimal?[] { 1, 2, 3, 4, 5 };
                blockSize = 1;
                int x = MathTool.StandardMovingAverage(source, blockSize).Count();
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

            source = new decimal?[] { null, null, null, null };
            expected = new decimal?[] { null };
            blockSize = 8;

            actual = MathTool.StandardMovingAverage(source, blockSize);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            source = new decimal?[] { 1, null, null, null };
            expected = new decimal?[] { 1, null, null };
            blockSize = 2;

            actual = MathTool.StandardMovingAverage(source, blockSize);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void StandardMovingAverageLongTest() {
            var source = new long[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
            var expected = new double[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var blockSize = 3;

            var actual = MathTool.StandardMovingAverage(source, blockSize);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            source = new long[] { 1, 2, 3, 4, 5 };
            expected = new double[] { 3 };
            blockSize = 8;

            actual = MathTool.StandardMovingAverage(source, blockSize);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            try {
                source = new long[] { 1, 2, 3, 4, 5 };
                blockSize = 1;
                int x = MathTool.StandardMovingAverage(source, blockSize).Count();
                Assert.Fail();
            }
            catch(ArgumentException e) {
                Assert.IsTrue(e.ParamName == "blockSize");
            }
            catch(InvalidOperationException) {}
            catch(Exception) {
                Assert.Fail();
            }
        }

        [Test]
        public void StandardMovingAverageNullableLongTest() {
            var source = new long?[] { 1, null, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
            var expected = new double?[] { 2, 3.5, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var blockSize = 3;

            var actual = MathTool.StandardMovingAverage(source, blockSize);
            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            source = new long?[] { 1, 2, null, 9 };
            expected = new double?[] { 4 };
            blockSize = 8;

            actual = MathTool.StandardMovingAverage(source, blockSize);
            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            try {
                source = new long?[] { 1, 2, 3, 4, 5 };
                blockSize = 1;
                int x = MathTool.StandardMovingAverage(source, blockSize).Count();
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

            source = new long?[] { null, null, null, null };
            expected = new double?[] { null };
            blockSize = 8;

            actual = MathTool.StandardMovingAverage(source, blockSize);
            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            source = new long?[] { 1, null, null, null };
            expected = new double?[] { 1, null, null };
            blockSize = 2;

            actual = MathTool.StandardMovingAverage(source, blockSize);
            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void StandardMovingAverageIntTest() {
            var source = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
            var expected = new double[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var blockSize = 3;

            var actual = MathTool.StandardMovingAverage(source, blockSize);

            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());

            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            source = new int[] { 1, 2, 3, 4, 5 };
            expected = new double[] { 3 };
            blockSize = 8;

            actual = MathTool.StandardMovingAverage(source, blockSize);

            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());

            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            try {
                source = new int[] { 1, 2, 3, 4, 5 };
                blockSize = 1;
                int x = MathTool.StandardMovingAverage(source, blockSize).Count();
                Assert.Fail();
            }
            catch(ArgumentException e) {
                Assert.IsTrue(e.ParamName == "blockSize");
            }
            catch(InvalidOperationException) {}
            catch(Exception) {
                Assert.Fail();
            }
        }

        [Test]
        public void StandardMovingAverageNullableIntTest() {
            var source = new int?[] { 1, null, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
            var expected = new double?[] { 2, 3.5, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var blockSize = 3;

            var actual = MathTool.StandardMovingAverage(source, blockSize);

            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            source = new int?[] { 1, 2, null, 6 };
            expected = new double?[] { 3 };
            blockSize = 8;

            actual = MathTool.StandardMovingAverage(source, blockSize);
            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            try {
                source = new int?[] { 1, 2, 3, 4, 5 };
                blockSize = 1;
                int x = MathTool.StandardMovingAverage(source, blockSize).Count();
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

            source = new int?[] { null, null, null, null };
            expected = new double?[] { null };
            blockSize = 8;

            actual = MathTool.StandardMovingAverage(source, blockSize);

            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());


            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            source = new int?[] { 1, null, null, null };
            expected = new double?[] { 1, null, null };
            blockSize = 2;

            actual = MathTool.StandardMovingAverage(source, blockSize);

            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());

            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void StandardMovingAverageDoubleTest1() {
            var source = LinqTool.Generate(1, 13, X => Generator.GenerateDouble(X, 10));
            const int blockSize = 3;
            var expected = new double[] { 2, 3, 4, 5, 6, 7, 8, double.NaN, double.NaN, double.NaN, 12 };
            var actual = MathTool.StandardMovingAverage(source, blockSize, S => S.Item2);

            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());

            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void StandardMovingAverageNullableDoubleTest1() {
            var source = LinqTool.Generate(1, 13, X => Generator.GenerateNullableDouble(X, 10, 2));
            const int blockSize = 3;
            var expected = new double?[] { 2, 3.5, 4, 5, 6, 7, 8, double.NaN, double.NaN, double.NaN, 12 };
            IEnumerable<double?> actual = MathTool.StandardMovingAverage(source, blockSize, S => S.Item2);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void StandardMovingAverageFloatTest1() {
            IEnumerable<Tuple<string, float>> source = LinqTool.Generate(1, 13, X => Generator.GenerateFloat(X, 10));
            const int blockSize = 3;
            var expected = new float[] { 2, 3, 4, 5, 6, 7, 8, float.NaN, float.NaN, float.NaN, 12 };
            var actual = MathTool.StandardMovingAverage(source, blockSize, S => S.Item2);

            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());

            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void StandardMovingAverageNullableFloatTest1() {
            IEnumerable<Tuple<string, float?>> source = LinqTool.Generate(1, 13, X => Generator.GenerateNullableFloat(X, 10, 2));
            const int blockSize = 3;
            var expected = new float?[] { 2, 3.5f, 4, 5, 6, 7, 8, float.NaN, float.NaN, float.NaN, 12 };
            var actual = MathTool.StandardMovingAverage(source, blockSize, S => S.Item2);

            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());

            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void StandardMovingAverageDecimalTest1() {
            var source = LinqTool.Generate(1, 13, X => Generator.GenerateDecimal(X));
            const int blockSize = 3;
            var expected = new decimal[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var actual = MathTool.StandardMovingAverage(source, blockSize, S => S.Item2);

            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());

            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void StandardMovingAverageNullableDecimalTest1() {
            var source = LinqTool.Generate(1, 13, X => Generator.GenerateNullableDecimal(X, 2));
            const int blockSize = 3;
            var expected = new decimal?[] { 2, 3.5m, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var actual = MathTool.StandardMovingAverage(source, blockSize, S => S.Item2);

            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());

            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void StandardMovingAverageLongTest1() {
            var source = LinqTool.Generate(1, 13, X => Generator.GenerateLong(X));
            const int blockSize = 3;
            var expected = new double[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var actual = MathTool.StandardMovingAverage(source, blockSize, S => S.Item2);

            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());

            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void StandardMovingAverageNullableLongTest1() {
            var source = LinqTool.Generate(1, 13, X => Generator.GenerateNullableLong(X, 2));
            const int blockSize = 3;
            var expected = new double?[] { 2, 3.5, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var actual = MathTool.StandardMovingAverage(source, blockSize, S => S.Item2);

            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());

            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void StandardMovingAverageIntTest1() {
            var source = LinqTool.Generate(1, 13, X => Generator.GenerateInt(X));
            const int blockSize = 3;
            var expected = new double[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var actual = MathTool.StandardMovingAverage(source, blockSize, S => S.Item2);

            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());

            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void StandardMovingAverageNullableIntTest1() {
            var source = LinqTool.Generate(1, 13, X => Generator.GenerateNullableInt(X, 2));
            const int blockSize = 3;
            var expected = new double?[] { 2, 3.5, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var actual = MathTool.StandardMovingAverage(source, blockSize, S => S.Item2);

            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());

            Assert.IsTrue(expected.SequenceEqual(actual));
        }
    }
}