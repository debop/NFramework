using System;
using System.Collections.Generic;
using System.Linq;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.Reflections;
using NUnit.Framework;

namespace NSoft.NFramework.Numerics.Utils {
    [TestFixture]
    public class MathToolFixture_StDevBlock : AbstractMathToolFixtureBase {
        [Test]
        public void StdevDoubleTest() {
            IEnumerable<double> source;
            IEnumerable<double> expected;
            IEnumerable<double> actual;
            int blockSize;

            source = new double[] { 1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384 };
            expected = new double[]
                       { 3.096, 6.191, 12.383, 24.766, 49.531, 99.062, 198.125, 396.249, 792.498, 1584.996, 3169.993, 6339.985 };
            blockSize = 4;

            actual = source.StDev(blockSize).Round(3);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            try {
                source = new double[] { 9 };
                blockSize = 4;
                int x = source.StDev(blockSize).Count();
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

            source = new double[] { 1, 2, 4, 8, 16, 32, 64, 128, 256, double.NaN, 1024, 2048, 4096, 8192, 16384 };
            blockSize = 4;
            expected = new double[]
                       {
                           3.096, 6.191, 12.383, 24.766, 49.531, 99.062, double.NaN, double.NaN, double.NaN, double.NaN, 3169.993,
                           6339.985
                       };

            actual = source.StDev(blockSize).Round(3);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            try {
                source = new double[] { 1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384 };
                blockSize = 1;
                int x = source.StDev(blockSize).Count();
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

            source = new double[] { 1, double.NaN, 4, 8, 16, 32 };
            blockSize = 4;
            expected = new double[] { double.NaN, double.NaN, 12.383 };

            actual = source.StDev(blockSize).Round(3);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            source = new double[] { 1, double.NaN };
            blockSize = 4;
            expected = new double[] { double.NaN };

            actual = source.StDev(blockSize).Round(3);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            source = new double[] { 1, 2 };
            blockSize = 4;
            expected = new double[] { 0.707 };

            actual = source.StDev(blockSize).Round(3);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void StdevNullableDoubleTest() {
            IEnumerable<double?> source;
            IEnumerable<double?> expected;
            IEnumerable<double?> actual;
            int blockSize;

            source = new double?[] { 1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384 };
            expected = new double?[]
                       { 3.096, 6.191, 12.383, 24.766, 49.531, 99.062, 198.125, 396.249, 792.498, 1584.996, 3169.993, 6339.985 };
            blockSize = 4;

            actual = source.StDev(blockSize).Select(V => V.Value).Round(3).Cast<double?>();
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            try {
                source = new double?[] { 9 };
                blockSize = 4;
                int x = source.StDev(blockSize).Count();
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

            source = new double?[] { 1, 2, 4, 8, 16, 32, 64, 128, 256, double.NaN, 1024, 2048, 4096, 8192, 16384 };
            blockSize = 4;
            expected = new double?[]
                       {
                           3.096, 6.191, 12.383, 24.766, 49.531, 99.062, double.NaN, double.NaN, double.NaN, double.NaN, 3169.993,
                           6339.985
                       };

            actual = source.StDev(blockSize).Select(V => V.Value).Round(3).Cast<double?>();
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            source = new double?[] { 1, 2, 4, 8, 16, 32, 64, 128, null, 512, 1024, 2048, 4096, 8192, 16384 };
            blockSize = 4;
            expected = new double?[]
                       { 3.096, 6.191, 12.383, 24.766, 49.531, 48.881, 242.300, 449.521, 782.093, 1584.996, 3169.993, 6339.985 };

            actual = source.StDev(blockSize).Select(V => V.Value).Round(3).Cast<double?>();
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            source = new double?[] { 1, 2, 4, 8, 16, 32, double.NaN, 128, null, 512, 1024, 2048, 4096, 8192, 16384 };
            blockSize = 4;
            expected = new double?[]
                       {
                           3.096, 6.191, 12.383, double.NaN, double.NaN, double.NaN, double.NaN, 449.521, 782.093, 1584.996, 3169.993,
                           6339.985
                       };

            actual = source.StDev(blockSize).Select(V => V.Value).Round(3).Cast<double?>();
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            try {
                source = new double?[] { 1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384 };
                blockSize = 1;
                int x = source.StDev(blockSize).Count();
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

            source = new double?[] { 1, double.NaN, 4, 8, 16, 32 };
            blockSize = 4;
            expected = new double?[] { double.NaN, double.NaN, 12.383 };

            actual = source.StDev(blockSize).Select(V => V.Value).Round(3).Cast<double?>();
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            source = new double?[] { 1, double.NaN };
            blockSize = 4;
            expected = new double?[] { double.NaN };

            actual = source.StDev(blockSize).Select(V => V.Value).Round(3).Cast<double?>();
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            source = new double?[] { 1, 2 };
            blockSize = 4;
            expected = new double?[] { 0.707 };

            actual = source.StDev(blockSize).Select(V => V.Value).Round(3).Cast<double?>();
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            source = new double?[] { 1, null, null, null, null, null };
            blockSize = 4;
            expected = new double?[] { null, null, null };

            actual = source.StDev(blockSize);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void StdevFloatTest() {
            IEnumerable<float> source;
            IEnumerable<float> expected;
            IEnumerable<float> actual;
            int blockSize;

            source = new float[] { 1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384 };
            expected = new float[]
                       {
                           3.096f, 6.191f, 12.383f, 24.766f, 49.531f, 99.062f, 198.125f, 396.249f, 792.498f, 1584.996f, 3169.993f,
                           6339.985f
                       };
            blockSize = 4;

            actual = source.StDev(blockSize).Round(3);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            try {
                source = new float[] { 9 };
                blockSize = 4;
                int x = source.StDev(blockSize).Count();
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

            source = new float[] { 1, 2, 4, 8, 16, 32, 64, 128, 256, float.NaN, 1024, 2048, 4096, 8192, 16384 };
            blockSize = 4;
            expected = new float[]
                       {
                           3.096f, 6.191f, 12.383f, 24.766f, 49.531f, 99.062f, float.NaN, float.NaN, float.NaN, float.NaN, 3169.993f,
                           6339.985f
                       };

            actual = source.StDev(blockSize).Round(3);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            try {
                source = new float[] { 1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384 };
                blockSize = 1;
                int x = source.StDev(blockSize).Count();
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

            source = new float[] { 1, float.NaN, 4, 8, 16, 32 };
            blockSize = 4;
            expected = new float[] { float.NaN, float.NaN, 12.383f };

            actual = source.StDev(blockSize).Round(3);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            source = new float[] { 1, float.NaN };
            blockSize = 4;
            expected = new float[] { float.NaN };

            actual = source.StDev(blockSize).Round(3);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            source = new float[] { 1, 2 };
            blockSize = 4;
            expected = new float[] { 0.707f };

            actual = source.StDev(blockSize).Round(3);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void StdevNullableFloatTest() {
            IEnumerable<float?> source;
            IEnumerable<float?> expected;
            IEnumerable<float?> actual;
            int blockSize;

            source = new float?[] { 1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384 };
            expected = new float?[]
                       {
                           3.096f, 6.191f, 12.383f, 24.766f, 49.531f, 99.062f, 198.125f, 396.249f, 792.498f, 1584.996f, 3169.993f,
                           6339.985f
                       };
            blockSize = 4;

            actual = source.StDev(blockSize).Select(V => V.Value).Round(3).Cast<float?>();
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            try {
                source = new float?[] { 9 };
                blockSize = 4;
                int x = source.StDev(blockSize).Count();
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

            source = new float?[] { 1, 2, 4, 8, 16, 32, 64, 128, 256, float.NaN, 1024, 2048, 4096, 8192, 16384 };
            blockSize = 4;
            expected = new float?[]
                       {
                           3.096f, 6.191f, 12.383f, 24.766f, 49.531f, 99.062f, float.NaN, float.NaN, float.NaN, float.NaN, 3169.993f,
                           6339.985f
                       };

            actual = source.StDev(blockSize).Select(V => V.Value).Round(3).Cast<float?>();
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            source = new float?[] { 1, 2, 4, 8, 16, 32, 64, 128, null, 512, 1024, 2048, 4096, 8192, 16384 };
            blockSize = 4;
            expected = new float?[]
                       {
                           3.096f, 6.191f, 12.383f, 24.766f, 49.531f, 48.881f, 242.300f, 449.521f, 782.093f, 1584.996f, 3169.993f,
                           6339.985f
                       };

            actual = source.StDev(blockSize).Select(V => V.Value).Round(3).Cast<float?>();
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            source = new float?[] { 1, 2, 4, 8, 16, 32, float.NaN, 128, null, 512, 1024, 2048, 4096, 8192, 16384 };
            blockSize = 4;
            expected = new float?[]
                       {
                           3.096f, 6.191f, 12.383f, float.NaN, float.NaN, float.NaN, float.NaN, 449.521f, 782.093f, 1584.996f, 3169.993f,
                           6339.985f
                       };

            actual = source.StDev(blockSize).Select(V => V.Value).Round(3).Cast<float?>();
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            try {
                source = new float?[] { 1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384 };
                blockSize = 1;
                int x = source.StDev(blockSize).Count();
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

            source = new float?[] { 1, float.NaN, 4, 8, 16, 32 };
            blockSize = 4;
            expected = new float?[] { float.NaN, float.NaN, 12.383f };

            actual = source.StDev(blockSize).Select(V => V.Value).Round(3).Cast<float?>();
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            source = new float?[] { 1, float.NaN };

            expected = new float?[] { float.NaN };

            actual = source.StDev(blockSize).Select(V => V.Value).Round(3).Cast<float?>();
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            source = new float?[] { 1, 2 };
            blockSize = 4;
            expected = new float?[] { 0.707f };

            actual = source.StDev(blockSize).Select(V => V.Value).Round(3).Cast<float?>();
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            source = new float?[] { 1, null, null, null, null, null };
            blockSize = 4;
            expected = new float?[] { null, null, null };

            actual = source.StDev(blockSize);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void StdevDecimalTest() {
            IEnumerable<decimal> source;
            IEnumerable<decimal> expected;
            IEnumerable<decimal> actual;
            int blockSize;

            source = new decimal[] { 1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384 };
            expected = new decimal[]
                       {
                           3.096m, 6.191m, 12.383m, 24.766m, 49.531m, 99.062m, 198.125m, 396.249m, 792.498m, 1584.996m, 3169.993m,
                           6339.985m
                       };
            blockSize = 4;

            actual = source.StDev(blockSize).Round(3);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            try {
                source = new decimal[] { 9 };
                blockSize = 4;
                int x = source.StDev(blockSize).Count();
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

            try {
                source = new decimal[] { 1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384 };
                blockSize = 1;
                int x = source.StDev(blockSize).Count();
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

            source = new decimal[] { 1, 2 };
            blockSize = 4;
            expected = new decimal[] { 0.707m };

            actual = source.StDev(blockSize).Round(3);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void StdevNullableDecimalTest() {
            IEnumerable<decimal?> source;
            IEnumerable<decimal?> expected;
            IEnumerable<decimal?> actual;
            int blockSize;

            source = new decimal?[] { 1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384 };
            expected = new decimal?[]
                       {
                           3.096m, 6.191m, 12.383m, 24.766m, 49.531m, 99.062m, 198.125m, 396.249m, 792.498m, 1584.996m, 3169.993m,
                           6339.985m
                       };
            blockSize = 4;

            actual = source.StDev(blockSize).Select(V => V.Value).Round(3).Cast<decimal?>();
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            try {
                source = new decimal?[] { 9 };
                blockSize = 4;
                int x = source.StDev(blockSize).Count();
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

            source = new decimal?[] { 1, 2, 4, 8, 16, 32, 64, 128, null, 512, 1024, 2048, 4096, 8192, 16384 };
            blockSize = 4;
            expected = new decimal?[]
                       {
                           3.096m, 6.191m, 12.383m, 24.766m, 49.531m, 48.881m, 242.300m, 449.521m, 782.093m, 1584.996m, 3169.993m,
                           6339.985m
                       };

            actual = source.StDev(blockSize).Select(V => V.Value).Round(3).Cast<decimal?>();
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            try {
                source = new decimal?[] { 1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384 };
                blockSize = 1;
                int x = source.StDev(blockSize).Count();
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

            source = new decimal?[] { 1, 2 };
            blockSize = 4;
            expected = new decimal?[] { 0.707m };

            actual = source.StDev(blockSize).Select(V => V.Value).Round(3).Cast<decimal?>();
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            source = new decimal?[] { 1, null, null, null, null, null };
            blockSize = 4;
            expected = new decimal?[] { null, null, null };

            actual = source.StDev(blockSize);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void StdevLongTest() {
            IEnumerable<long> source;
            IEnumerable<double> expected;
            IEnumerable<double> actual;
            int blockSize;

            source = new long[] { 1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384 };
            expected = new double[]
                       { 3.096, 6.191, 12.383, 24.766, 49.531, 99.062, 198.125, 396.249, 792.498, 1584.996, 3169.993, 6339.985 };
            blockSize = 4;

            actual = source.StDev(blockSize).Round(3);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            try {
                source = new long[] { 9 };
                blockSize = 4;
                int x = source.StDev(blockSize).Count();
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

            try {
                source = new long[] { 1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384 };
                blockSize = 1;
                int x = source.StDev(blockSize).Count();
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

            source = new long[] { 1, 2 };
            blockSize = 4;
            expected = new double[] { 0.707 };

            actual = source.StDev(blockSize).Round(3);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void StdevNullableLongTest() {
            IEnumerable<long?> source;
            IEnumerable<double?> expected;
            IEnumerable<double?> actual;
            int blockSize;

            source = new long?[] { 1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384 };
            expected = new double?[]
                       { 3.096, 6.191, 12.383, 24.766, 49.531, 99.062, 198.125, 396.249, 792.498, 1584.996, 3169.993, 6339.985 };
            blockSize = 4;

            actual = source.StDev(blockSize).Select(V => V.Value).Round(3).Cast<double?>();
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            try {
                source = new long?[] { 9 };
                blockSize = 4;
                int x = source.StDev(blockSize).Count();
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

            source = new long?[] { 1, 2, 4, 8, 16, 32, 64, 128, null, 512, 1024, 2048, 4096, 8192, 16384 };
            blockSize = 4;
            expected = new double?[]
                       { 3.096, 6.191, 12.383, 24.766, 49.531, 48.881, 242.300, 449.521, 782.093, 1584.996, 3169.993, 6339.985 };

            actual = source.StDev(blockSize).Select(V => V.Value).Round(3).Cast<double?>();
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            try {
                source = new long?[] { 1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384 };
                blockSize = 1;
                int x = source.StDev(blockSize).Count();
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

            source = new long?[] { 1, 2 };
            blockSize = 4;
            expected = new double?[] { 0.707 };

            actual = source.StDev(blockSize).Select(V => V.Value).Round(3).Cast<double?>();
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            source = new long?[] { 1, null, null, null, null, null };
            blockSize = 4;
            expected = new double?[] { null, null, null };

            actual = source.StDev(blockSize);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void StdevIntTest() {
            IEnumerable<int> source;
            IEnumerable<double> expected;
            IEnumerable<double> actual;
            int blockSize;

            source = new int[] { 1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384 };
            expected = new double[]
                       { 3.096, 6.191, 12.383, 24.766, 49.531, 99.062, 198.125, 396.249, 792.498, 1584.996, 3169.993, 6339.985 };
            blockSize = 4;

            actual = source.StDev(blockSize).Round(3);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            try {
                source = new int[] { 9 };
                blockSize = 4;
                int x = source.StDev(blockSize).Count();
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

            try {
                source = new int[] { 1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384 };
                blockSize = 1;
                int x = source.StDev(blockSize).Count();
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

            source = new int[] { 1, 2 };
            blockSize = 4;
            expected = new double[] { 0.707 };

            actual = source.StDev(blockSize).Round(3);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void StdevNullableIntTest() {
            IEnumerable<int?> source;
            IEnumerable<double?> expected;
            IEnumerable<double?> actual;
            int blockSize;

            source = new int?[] { 1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384 };
            expected = new double?[]
                       { 3.096, 6.191, 12.383, 24.766, 49.531, 99.062, 198.125, 396.249, 792.498, 1584.996, 3169.993, 6339.985 };
            blockSize = 4;

            actual = source.StDev(blockSize).Select(V => V.Value).Round(3).Cast<double?>();
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            try {
                source = new int?[] { 9 };
                blockSize = 4;
                int x = source.StDev(blockSize).Count();
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

            source = new int?[] { 1, 2, 4, 8, 16, 32, 64, 128, null, 512, 1024, 2048, 4096, 8192, 16384 };
            blockSize = 4;
            expected = new double?[]
                       { 3.096, 6.191, 12.383, 24.766, 49.531, 48.881, 242.300, 449.521, 782.093, 1584.996, 3169.993, 6339.985 };

            actual = source.StDev(blockSize).Select(V => V.Value).Round(3).Cast<double?>();
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            try {
                source = new int?[] { 1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384 };
                blockSize = 1;
                int x = source.StDev(blockSize).Count();
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

            source = new int?[] { 1, 2 };
            blockSize = 4;
            expected = new double?[] { 0.707 };

            actual = source.StDev(blockSize).Select(V => V.Value).Round(3).Cast<double?>();
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            source = new int?[] { 1, null, null, null, null, null };
            blockSize = 4;
            expected = new double?[] { null, null, null };

            actual = source.StDev(blockSize);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        //--------------------------------------------------------------

        [Test]
        public void StdevDoubleTest1() {
            IEnumerable<Tuple<string, double>> source = LinqTool.Generate(1, 7,
                                                                          X => Generator.GenerateDouble((int)Math.Pow(2, X - 1), 10));
            IEnumerable<double> expected = new double[] { 3.096, 6.191, 12.383, 24.766 };

            IEnumerable<double> actual = source.StDev(4, X => X.Item2).Round(3);

            if(IsDebugEnabled)
                log.Debug("expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());

            Assert.IsTrue(expected.SequenceEqual(actual), "expected=[{0}], actual=[{1}]", expected.CollectionToString(),
                          actual.CollectionToString());
        }

        [Test]
        public void StdevNullableDoubleTest1() {
            IEnumerable<Tuple<string, double?>> source = LinqTool.Generate(1, 7,
                                                                           X =>
                                                                           Generator.GenerateNullableDouble((int)Math.Pow(2, X - 1), 10,
                                                                                                            2));
            IEnumerable<double?> expected = new double?[] { 3.512, 6.110, 12.383, 24.766 };

            IEnumerable<double?> actual = source.StDev(4, X => X.Item2).Select(V => V.Value).Round(3).Cast<double?>();

            if(IsDebugEnabled)
                log.Debug("expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());

            Assert.IsTrue(expected.SequenceEqual(actual), "expected=[{0}], actual=[{1}]", expected.CollectionToString(),
                          actual.CollectionToString());
        }

        [Test]
        public void StdevFloatTest1() {
            IEnumerable<Tuple<string, float>> source = LinqTool.Generate(1, 7, X => Generator.GenerateFloat((int)Math.Pow(2, X - 1), 10));
            IEnumerable<float> expected = new float[] { 3.096f, 6.191f, 12.383f, 24.766f };

            IEnumerable<float> actual = source.StDev(4, X => X.Item2).Round(3);

            if(IsDebugEnabled)
                log.Debug("expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());

            Assert.IsTrue(expected.SequenceEqual(actual), "expected=[{0}], actual=[{1}]", expected.CollectionToString(),
                          actual.CollectionToString());
        }

        [Test]
        public void StdevNullableFloatTest1() {
            IEnumerable<Tuple<string, float?>> source = LinqTool.Generate(1, 7,
                                                                          X =>
                                                                          Generator.GenerateNullableFloat((int)Math.Pow(2, X - 1), 10, 2));
            IEnumerable<float?> expected = new float?[] { 3.512f, 6.110f, 12.383f, 24.766f };

            IEnumerable<float?> actual = source.StDev(4, X => X.Item2).Select(V => V.Value).Round(3).Cast<float?>();

            if(IsDebugEnabled)
                log.Debug("expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());

            Assert.IsTrue(expected.SequenceEqual(actual), "expected=[{0}], actual=[{1}]", expected.CollectionToString(),
                          actual.CollectionToString());
        }

        [Test]
        public void StdevDecimalTest1() {
            IEnumerable<Tuple<string, decimal>> source = LinqTool.Generate(1, 7, X => Generator.GenerateDecimal((int)Math.Pow(2, X - 1)));
            IEnumerable<decimal> expected = new decimal[] { 3.096m, 6.191m, 12.383m, 24.766m };

            IEnumerable<decimal> actual = source.StDev(4, X => X.Item2).Round(3);

            if(IsDebugEnabled)
                log.Debug("expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());

            Assert.IsTrue(expected.SequenceEqual(actual), "expected=[{0}], actual=[{1}]", expected.CollectionToString(),
                          actual.CollectionToString());
        }

        [Test]
        public void StdevNullableDecimalTest1() {
            IEnumerable<Tuple<string, decimal?>> source = LinqTool.Generate(1, 7,
                                                                            X =>
                                                                            Generator.GenerateNullableDecimal((int)Math.Pow(2, X - 1), 2));
            IEnumerable<decimal?> expected = new decimal?[] { 3.512m, 6.110m, 12.383m, 24.766m };

            IEnumerable<decimal?> actual = source.StDev(4, X => X.Item2).Select(V => V.Value).Round(3).Cast<decimal?>();

            if(IsDebugEnabled)
                log.Debug("expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());

            Assert.IsTrue(expected.SequenceEqual(actual), "expected=[{0}], actual=[{1}]", expected.CollectionToString(),
                          actual.CollectionToString());
        }

        [Test]
        public void StdevLongTest1() {
            IEnumerable<Tuple<string, long>> source = LinqTool.Generate(1, 7, X => Generator.GenerateLong((int)Math.Pow(2, X - 1)));
            IEnumerable<double> expected = new double[] { 3.096, 6.191, 12.383, 24.766 };

            IEnumerable<double> actual = source.StDev(4, X => X.Item2).Round(3);

            if(IsDebugEnabled)
                log.Debug("expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());

            Assert.IsTrue(expected.SequenceEqual(actual), "expected=[{0}], actual=[{1}]", expected.CollectionToString(),
                          actual.CollectionToString());
        }

        [Test]
        public void StdevNullableLongTest1() {
            IEnumerable<Tuple<string, long?>> source = LinqTool.Generate(1, 7,
                                                                         X => Generator.GenerateNullableLong((int)Math.Pow(2, X - 1), 2));
            IEnumerable<double?> expected = new double?[] { 3.512, 6.110, 12.383, 24.766 };

            IEnumerable<double?> actual = source.StDev(4, X => X.Item2).Select(V => V.Value).Round(3).Cast<double?>();

            if(IsDebugEnabled)
                log.Debug("expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());

            Assert.IsTrue(expected.SequenceEqual(actual), "expected=[{0}], actual=[{1}]", expected.CollectionToString(),
                          actual.CollectionToString());
        }

        [Test]
        public void StdevIntTest1() {
            IEnumerable<Tuple<string, int>> source = LinqTool.Generate(1, 7, X => Generator.GenerateInt((int)Math.Pow(2, X - 1)));
            IEnumerable<double> expected = new double[] { 3.096, 6.191, 12.383, 24.766 };

            IEnumerable<double> actual = source.StDev(4, X => X.Item2).Round(3);

            if(IsDebugEnabled)
                log.Debug("expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());

            Assert.IsTrue(expected.SequenceEqual(actual), "expected=[{0}], actual=[{1}]", expected.CollectionToString(),
                          actual.CollectionToString());
        }

        [Test]
        public void StdevNullableIntTest1() {
            IEnumerable<Tuple<string, int?>> source = LinqTool.Generate(1, 7,
                                                                        X => Generator.GenerateNullableInt((int)Math.Pow(2, X - 1), 2));
            IEnumerable<double?> expected = new double?[] { 3.512, 6.110, 12.383, 24.766 };

            IEnumerable<double?> actual = source.StDev(4, X => X.Item2).Select(V => V.Value).Round(3).Cast<double?>();

            if(IsDebugEnabled)
                log.Debug("expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());

            Assert.IsTrue(expected.SequenceEqual(actual), "expected=[{0}], actual=[{1}]", expected.CollectionToString(),
                          actual.CollectionToString());
        }
    }
}