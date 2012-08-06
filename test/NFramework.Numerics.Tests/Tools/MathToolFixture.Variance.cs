using System.Linq;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.Reflections;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Numerics.Utils {
    [TestFixture]
    public class MathToolFixture_Variance : AbstractMathToolFixtureBase {
        [TestCase(-1000, 1, 1000)]
        [TestCase(0, 1, 1000)]
        [TestCase(1000, 1, 1000)]
        public void Variance_Double_Test(double seed, double step, int count) {
            var source = GetSerialList(seed, step, count).ToList();

            var variance = source.Variance();

            if(IsDebugEnabled)
                log.Debug("seed=[{0}], step=[{1}], count=[{2}], variance=[{3}]", seed, step, count, variance);

            variance.ApproximateEqual(83416.66666, 1e-5).Should().Be.True();
        }

        [TestCase(-1000, 1, 1000)]
        [TestCase(0, 1, 1000)]
        [TestCase(1000, 1, 1000)]
        public void Variance_Nullable_Double_Test(double seed, double step, int count) {
            var source = GetSerialList(seed, step, count).Select(x => (double?)x).ToList();

            Assert.AreEqual(count, source.Count());
            Assert.AreEqual(count, source.Count(x => x.HasValue));

            for(int i = 0; i < count; i++)
                source.Add(null);

            var variance = source.Variance();

            if(IsDebugEnabled)
                log.Debug("seed=[{0}], step=[{1}], count=[{2}], variance=[{3}]", seed, step, count, variance);

            variance.ApproximateEqual(83416.66666, 1e-5).Should().Be.True();
        }

        [TestCase(-1000, 1, 1000)]
        [TestCase(0, 1, 1000)]
        [TestCase(1000, 1, 1000)]
        public void Variance_Float_Test(double seed, double step, int count) {
            var source = GetSerialList(seed, step, count).Select(x => (float)x).ToList();

            var variance = source.Variance();

            if(IsDebugEnabled)
                log.Debug("seed=[{0}], step=[{1}], count=[{2}], variance=[{3}]", seed, step, count, variance);

            variance.ApproximateEqual(83416.69f, 1e-2f).Should().Be.True();
        }

        [TestCase(-1000, 1, 1000)]
        [TestCase(0, 1, 1000)]
        [TestCase(1000, 1, 1000)]
        public void Variance_Nullable_Float_Test(double seed, double step, int count) {
            var source = GetSerialList(seed, step, count).Select(x => (float?)x).ToList();

            Assert.AreEqual(count, source.Count());
            Assert.AreEqual(count, source.Count(x => x.HasValue));

            for(int i = 0; i < count; i++)
                source.Add(null);

            var variance = source.Variance();

            if(IsDebugEnabled)
                log.Debug("seed=[{0}], step=[{1}], count=[{2}], variance=[{3}]", seed, step, count, variance);

            variance.ApproximateEqual(83416.69f, 1e-2f).Should().Be.True();
        }

        [TestCase(-1000, 1, 1000)]
        [TestCase(0, 1, 1000)]
        [TestCase(1000, 1, 1000)]
        public void Variance_Decimal_Test(double seed, double step, int count) {
            var source = GetSerialList(seed, step, count).Select(x => (decimal)x).ToList();

            var variance = source.Variance();

            if(IsDebugEnabled)
                log.Debug("seed=[{0}], step=[{1}], count=[{2}], variance=[{3}]", seed, step, count, variance);

            variance.ApproximateEqual(83416.66666m, 1e-5m).Should().Be.True();
        }

        [TestCase(-1000, 1, 1000)]
        [TestCase(0, 1, 1000)]
        [TestCase(1000, 1, 1000)]
        public void Variance_Nullable_Decimal_Test(double seed, double step, int count) {
            var source = GetSerialList(seed, step, count).Select(x => (decimal?)x).ToList();

            Assert.AreEqual(count, source.Count());
            Assert.AreEqual(count, source.Count(x => x.HasValue));

            for(int i = 0; i < count; i++)
                source.Add(null);

            var variance = source.Variance();

            if(IsDebugEnabled)
                log.Debug("seed=[{0}], step=[{1}], count=[{2}], variance=[{3}]", seed, step, count, variance);

            variance.ApproximateEqual(83416.66666m, 1e-5m).Should().Be.True();
        }

        [TestCase(-1000, 1, 1000)]
        [TestCase(0, 1, 1000)]
        [TestCase(1000, 1, 1000)]
        public void Variance_Long_Test(double seed, double step, int count) {
            var source = GetSerialList(seed, step, count).Select(x => (long)x).ToList();

            var variance = source.Variance();

            if(IsDebugEnabled)
                log.Debug("seed=[{0}], step=[{1}], count=[{2}], variance=[{3}]", seed, step, count, variance);

            variance.ApproximateEqual(83416.66666, 1e-5).Should().Be.True();
        }

        [TestCase(-1000, 1, 1000)]
        [TestCase(0, 1, 1000)]
        [TestCase(1000, 1, 1000)]
        public void Variance_Nullable_Long_Test(double seed, double step, int count) {
            var source = GetSerialList(seed, step, count).Select(x => (long?)x).ToList();

            Assert.AreEqual(count, source.Count());
            Assert.AreEqual(count, source.Count(x => x.HasValue));

            for(int i = 0; i < count; i++)
                source.Add(null);

            var variance = source.Variance();

            if(IsDebugEnabled)
                log.Debug("seed=[{0}], step=[{1}], count=[{2}], variance=[{3}]", seed, step, count, variance);

            variance.ApproximateEqual(83416.66666, 1e-5).Should().Be.True();
        }

        [TestCase(-1000, 1, 1000)]
        [TestCase(0, 1, 1000)]
        [TestCase(1000, 1, 1000)]
        public void Variance_Int_Test(double seed, double step, int count) {
            var source = GetSerialList(seed, step, count).Select(x => (int)x).ToList();

            var variance = source.Variance();

            if(IsDebugEnabled)
                log.Debug("seed=[{0}], step=[{1}], count=[{2}], variance=[{3}]", seed, step, count, variance);

            variance.ApproximateEqual(83416.66666, 1e-5).Should().Be.True();
        }

        [TestCase(-1000, 1, 1000)]
        [TestCase(0, 1, 1000)]
        [TestCase(1000, 1, 1000)]
        public void Variance_Nullable_Int_Test(double seed, double step, int count) {
            var source = GetSerialList(seed, step, count).Select(x => (int?)x).ToList();

            Assert.AreEqual(count, source.Count());
            Assert.AreEqual(count, source.Count(x => x.HasValue));

            for(int i = 0; i < count; i++)
                source.Add(null);

            var variance = source.Variance();

            if(IsDebugEnabled)
                log.Debug("seed=[{0}], step=[{1}], count=[{2}], variance=[{3}]", seed, step, count, variance);

            variance.ApproximateEqual(83416.66666, 1e-5).Should().Be.True();
        }

        public void VarianceWithBlock_Double_Test(double seed, double step, int count) {
            var source = LinqTool.Generate(5, x => Generator.GenerateDouble(x, 10));
            var expected = new double[] { 2.5 };

            var actual = source.Variance(5, x => x.Item2).Round(3);

            if(IsDebugEnabled)
                log.Debug("Variance with Block=[{0}]", actual.CollectionToString());

            actual.SequenceEqual(expected).Should().Be.True();

            //------------------------------------------------------

            source = LinqTool.Generate(7, x => Generator.GenerateDouble(x, 10));
            expected = new double[] { 2.5, 2.5, 2.5 };

            actual = source.Variance(5, x => x.Item2).Round(3);

            if(IsDebugEnabled)
                log.Debug("Variance with Block=[{0}]", actual.CollectionToString());

            actual.SequenceEqual(expected).Should().Be.True();
        }

        public void VarianceWithBlock_Float_Test(double seed, double step, int count) {
            var source = LinqTool.Generate(5, x => Generator.GenerateFloat(x, 10));
            var expected = new float[] { 2.5f };

            var actual = source.Variance(5, x => x.Item2).Round(3);

            if(IsDebugEnabled)
                log.Debug("Variance with Block=[{0}]", actual.CollectionToString());

            actual.SequenceEqual(expected).Should().Be.True();

            //------------------------------------------------------

            source = LinqTool.Generate(7, x => Generator.GenerateFloat(x, 10));
            expected = new float[] { 2.5f, 2.5f, 2.5f };

            actual = source.Variance(5, x => x.Item2).Round(3);

            if(IsDebugEnabled)
                log.Debug("Variance with Block=[{0}]", actual.CollectionToString());

            actual.SequenceEqual(expected).Should().Be.True();
        }

        public void VarianceWithBlock_Decimal_Test(double seed, double step, int count) {
            var source = LinqTool.Generate(5, x => Generator.GenerateDecimal(x));
            var expected = new decimal[] { 2.5m };

            var actual = source.Variance(5, x => x.Item2).Round(3);

            if(IsDebugEnabled)
                log.Debug("Variance with Block=[{0}]", actual.CollectionToString());

            actual.SequenceEqual(expected).Should().Be.True();

            //------------------------------------------------------

            source = LinqTool.Generate(7, x => Generator.GenerateDecimal(x));
            expected = new decimal[] { 2.5m, 2.5m, 2.5m };

            actual = source.Variance(5, x => x.Item2).Round(3);

            if(IsDebugEnabled)
                log.Debug("Variance with Block=[{0}]", actual.CollectionToString());

            actual.SequenceEqual(expected).Should().Be.True();
        }

        public void VarianceWithBlock_Long_Test(double seed, double step, int count) {
            var source = LinqTool.Generate(5, x => Generator.GenerateInt(x));
            var expected = new double[] { 2.5 };

            var actual = source.Variance(5, x => x.Item2).Round(3);

            if(IsDebugEnabled)
                log.Debug("Variance with Block=[{0}]", actual.CollectionToString());

            actual.SequenceEqual(expected).Should().Be.True();

            //------------------------------------------------------

            source = LinqTool.Generate(7, x => Generator.GenerateInt(x));
            expected = new double[] { 2.5, 2.5, 2.5 };

            actual = source.Variance(5, x => x.Item2).Round(3);

            if(IsDebugEnabled)
                log.Debug("Variance with Block=[{0}]", actual.CollectionToString());

            actual.SequenceEqual(expected).Should().Be.True();
        }

        public void VarianceWithBlock_Int_Test(double seed, double step, int count) {
            var source = LinqTool.Generate(1, 5, x => Generator.GenerateInt(x));
            var expected = new double[] { 2.5 };

            var actual = source.Variance(5, x => x.Item2).Round(3);

            if(IsDebugEnabled)
                log.Debug("Variance with Block=[{0}]", actual.CollectionToString());

            actual.SequenceEqual(expected).Should().Be.True();

            //------------------------------------------------------

            source = LinqTool.Generate(1, 7, x => Generator.GenerateInt(x));
            expected = new double[] { 2.5, 2.5, 2.5 };

            actual = source.Variance(5, x => x.Item2).Round(3);

            if(IsDebugEnabled)
                log.Debug("Variance with Block=[{0}]", actual.CollectionToString());

            actual.SequenceEqual(expected).Should().Be.True();
        }

        [Test]
        public void CumulativeVariance_Double_Test() {
            var source = LinqTool.Generate(1, 5, X => Generator.GenerateDouble(X, 10));
            var expected = (new double[] { .707, 1, 1.291, 1.581 }).Select(X => X * X).Round(3);
            var actual = source.CumulativeVariance(X => X.Item2).Round(3);

            if(IsDebugEnabled)
                log.Debug("CumulativeVariance source=[{0}], actual=[{1}], expected=[{2}]",
                          source.CollectionToString(), actual.CollectionToString(), expected.CollectionToString());

            actual.SequenceEqual(expected).Should().Be.True();
        }

        [Test]
        public void CumulativeVariance_NullableDouble_Test() {
            var source = LinqTool.Generate(1, 5, X => Generator.GenerateNullableDouble(X, 10, 2));
            var expected = new double?[] { double.NaN, 1.414, 1.528, 1.708 }.Select(X => X.Value * X.Value).Round(2).Cast<double?>();
            var actual = source.CumulativeVariance(X => X.Item2).Select(V => V.Value).Round(2).Cast<double?>();

            if(IsDebugEnabled)
                log.Debug("CumulativeVariance source=[{0}], actual=[{1}], expected=[{2}]",
                          source.CollectionToString(), actual.CollectionToString(), expected.CollectionToString());

            actual.SequenceEqual(expected).Should().Be.True();
        }

        [Test]
        public void CumulativeVariance_Float_Test() {
            var source = LinqTool.Generate(1, 5, X => Generator.GenerateFloat(X, 10));
            var expected = (new float[] { .707f, 1f, 1.291f, 1.581f }).Select(X => X * X).Round(3);
            var actual = source.CumulativeVariance(X => X.Item2).Round(3);

            if(IsDebugEnabled)
                log.Debug("CumulativeVariance source=[{0}], actual=[{1}], expected=[{2}]",
                          source.CollectionToString(), actual.CollectionToString(), expected.CollectionToString());

            actual.SequenceEqual(expected).Should().Be.True();
        }

        [Test]
        public void CumulativeVariance_NullableFloat_Test() {
            var source = LinqTool.Generate(1, 5, X => Generator.GenerateNullableFloat(X, 10, 2));
            var expected = new float?[] { float.NaN, 1.414f, 1.528f, 1.708f }.Select(X => X.Value * X.Value).Round(2).Cast<float?>();
            var actual = source.CumulativeVariance(X => X.Item2).Select(V => V.Value).Round(2).Cast<float?>();

            if(IsDebugEnabled)
                log.Debug("CumulativeVariance source=[{0}], actual=[{1}], expected=[{2}]",
                          source.CollectionToString(), actual.CollectionToString(), expected.CollectionToString());

            actual.SequenceEqual(expected).Should().Be.True();
        }

        [Test]
        public void CumulativeVariance_Decimal_Test() {
            var source = LinqTool.Generate(1, 5, X => Generator.GenerateDecimal(X));
            var expected = (new decimal[] { .707m, 1, 1.291m, 1.581m }).Select(X => X * X).Round(3);
            var actual = source.CumulativeVariance(X => X.Item2).Round(3);

            if(IsDebugEnabled)
                log.Debug("CumulativeVariance source=[{0}], actual=[{1}], expected=[{2}]",
                          source.CollectionToString(), actual.CollectionToString(), expected.CollectionToString());

            actual.SequenceEqual(expected).Should().Be.True();
        }

        [Test]
        public void CumulativeVariance_NullableDecimal_Test() {
            var source = LinqTool.Generate(1, 5, X => Generator.GenerateNullableDecimal(X, 20));
            var expected = new decimal?[] { 0.707m, 1.000m, 1.291m, 1.581m }.Select(X => X.Value * X.Value).Round(2).Cast<decimal?>();
            var actual = source.CumulativeVariance(X => X.Item2).Select(V => V.Value).Round(2).Cast<decimal?>();

            if(IsDebugEnabled)
                log.Debug("CumulativeVariance source=[{0}], actual=[{1}], expected=[{2}]",
                          source.CollectionToString(), actual.CollectionToString(), expected.CollectionToString());

            actual.SequenceEqual(expected).Should().Be.True();
        }

        [Test]
        public void CumulativeVariance_Long_Test() {
            var source = LinqTool.Generate(1, 5, X => Generator.GenerateLong(X));
            var expected = (new double[] { 0.707, 1.000, 1.291, 1.581 }).Select(X => X * X).Round(3);
            var actual = source.CumulativeVariance(X => X.Item2).Round(3);

            if(IsDebugEnabled)
                log.Debug("CumulativeVariance source=[{0}], actual=[{1}], expected=[{2}]",
                          source.CollectionToString(), actual.CollectionToString(), expected.CollectionToString());

            actual.SequenceEqual(expected).Should().Be.True();
        }

        [Test]
        public void CumulativeVariance_NullableLong_Test() {
            var source = LinqTool.Generate(1, 5, X => Generator.GenerateNullableLong(X, 20));
            var expected = new double?[] { 0.707, 1.000, 1.291, 1.581 }.Select(X => X.Value * X.Value).Round(2).Cast<double?>();
            var actual = source.CumulativeVariance(X => X.Item2).Select(V => V.Value).Round(2).Cast<double?>();

            if(IsDebugEnabled)
                log.Debug("CumulativeVariance source=[{0}], actual=[{1}], expected=[{2}]",
                          source.CollectionToString(), actual.CollectionToString(), expected.CollectionToString());

            actual.SequenceEqual(expected).Should().Be.True();
        }

        [Test]
        public void CumulativeVariance_Int_Test() {
            var source = LinqTool.Generate(1, 5, X => Generator.GenerateInt(X));
            var expected = (new double[] { 0.707, 1.000, 1.291, 1.581 }).Select(X => X * X).Round(3);
            var actual = source.CumulativeVariance(X => X.Item2).Round(3);

            if(IsDebugEnabled)
                log.Debug("CumulativeVariance source=[{0}], actual=[{1}], expected=[{2}]",
                          source.CollectionToString(), actual.CollectionToString(), expected.CollectionToString());

            actual.SequenceEqual(expected).Should().Be.True();
        }

        [Test]
        public void CumulativeVariance_NullableInt_Test() {
            var source = LinqTool.Generate(1, 5, X => Generator.GenerateNullableInt(X, 20));
            var expected = new double?[] { 0.707, 1.000, 1.291, 1.581 }.Select(X => X.Value * X.Value).Round(2).Cast<double?>();
            var actual = source.CumulativeVariance(X => X.Item2).Select(V => V.Value).Round(2).Cast<double?>();

            if(IsDebugEnabled)
                log.Debug("CumulativeVariance source=[{0}], actual=[{1}], expected=[{2}]",
                          source.CollectionToString(), actual.CollectionToString(), expected.CollectionToString());

            actual.SequenceEqual(expected).Should().Be.True();
        }
    }
}