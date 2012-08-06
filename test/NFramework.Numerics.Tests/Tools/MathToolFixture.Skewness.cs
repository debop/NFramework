using System.Linq;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Numerics.Utils {
    [TestFixture]
    public class MathToolFixture_Skewness : AbstractMathToolFixtureBase {
        [TestCase(-1000, 1, 1000)]
        [TestCase(0, 1, 1000)]
        [TestCase(1000, 1, 1000)]
        public void Skewness_Double_Test(double seed, double step, int count) {
            var source = GetSerialList(seed, step, count).ToList();

            var skewness = source.Skewness();

            if(IsDebugEnabled)
                log.Debug("seed=[{0}], step=[{1}], count=[{2}], skewness=[{3}]", seed, step, count, skewness);

            skewness.ApproximateEqual(0.0, 1e-5).Should().Be.True();
        }

        [TestCase(-1000, 1, 1000)]
        [TestCase(0, 1, 1000)]
        [TestCase(1000, 1, 1000)]
        public void Skewness_Nullable_Double_Test(double seed, double step, int count) {
            var source = GetSerialList(seed, step, count).Select(x => (double?)x).ToList();

            Assert.AreEqual(count, source.Count());
            Assert.AreEqual(count, source.Count(x => x.HasValue));

            for(int i = 0; i < count; i++)
                source.Add(null);

            var skewness = source.Skewness();

            if(IsDebugEnabled)
                log.Debug("seed=[{0}], step=[{1}], count=[{2}], skewness=[{3}]", seed, step, count, skewness);

            skewness.ApproximateEqual(0.0, 1e-5).Should().Be.True();
        }

        [TestCase(-1000, 1, 1000)]
        [TestCase(0, 1, 1000)]
        [TestCase(1000, 1, 1000)]
        public void Skewness_Float_Test(double seed, double step, int count) {
            var source = GetSerialList(seed, step, count).Select(x => (float)x).ToList();

            var skewness = source.Skewness();

            if(IsDebugEnabled)
                log.Debug("seed=[{0}], step=[{1}], count=[{2}], skewness=[{3}]", seed, step, count, skewness);

            skewness.ApproximateEqual(0.0f, 1e-5f).Should().Be.True();
        }

        [TestCase(-1000, 1, 1000)]
        [TestCase(0, 1, 1000)]
        [TestCase(1000, 1, 1000)]
        public void Skewness_Nullable_Float_Test(double seed, double step, int count) {
            var source = GetSerialList(seed, step, count).Select(x => (float?)x).ToList();

            Assert.AreEqual(count, source.Count());
            Assert.AreEqual(count, source.Count(x => x.HasValue));

            for(int i = 0; i < count; i++)
                source.Add(null);

            var skewness = source.Skewness();

            if(IsDebugEnabled)
                log.Debug("seed=[{0}], step=[{1}], count=[{2}], skewness=[{3}]", seed, step, count, skewness);

            skewness.ApproximateEqual(0.0f, 1e-5f).Should().Be.True();
        }

        [TestCase(-1000, 1, 1000)]
        [TestCase(0, 1, 1000)]
        [TestCase(1000, 1, 1000)]
        public void Skewness_Decimal_Test(double seed, double step, int count) {
            var source = GetSerialList(seed, step, count).Select(x => (decimal)x).ToList();

            var skewness = source.Skewness();

            if(IsDebugEnabled)
                log.Debug("seed=[{0}], step=[{1}], count=[{2}], skewness=[{3}]", seed, step, count, skewness);

            skewness.ApproximateEqual(0.0m, 1e-5m).Should().Be.True();
        }

        [TestCase(-1000, 1, 1000)]
        [TestCase(0, 1, 1000)]
        [TestCase(1000, 1, 1000)]
        public void Skewness_Nullable_Decimal_Test(double seed, double step, int count) {
            var source = GetSerialList(seed, step, count).Select(x => (decimal?)x).ToList();

            Assert.AreEqual(count, source.Count());
            Assert.AreEqual(count, source.Count(x => x.HasValue));

            for(int i = 0; i < count; i++)
                source.Add(null);

            var skewness = source.Skewness();

            if(IsDebugEnabled)
                log.Debug("seed=[{0}], step=[{1}], count=[{2}], skewness=[{3}]", seed, step, count, skewness);

            skewness.ApproximateEqual(0.0m, 1e-5m).Should().Be.True();
        }

        [TestCase(-1000, 1, 1000)]
        [TestCase(0, 1, 1000)]
        [TestCase(1000, 1, 1000)]
        public void Skewness_Long_Test(double seed, double step, int count) {
            var source = GetSerialList(seed, step, count).Select(x => (long)x).ToList();

            var skewness = source.Skewness();

            if(IsDebugEnabled)
                log.Debug("seed=[{0}], step=[{1}], count=[{2}], skewness=[{3}]", seed, step, count, skewness);

            skewness.ApproximateEqual(0.0, 1e-5).Should().Be.True();
        }

        [TestCase(-1000, 1, 1000)]
        [TestCase(0, 1, 1000)]
        [TestCase(1000, 1, 1000)]
        public void Skewness_Nullable_Long_Test(double seed, double step, int count) {
            var source = GetSerialList(seed, step, count).Select(x => (long?)x).ToList();

            Assert.AreEqual(count, source.Count());
            Assert.AreEqual(count, source.Count(x => x.HasValue));

            for(int i = 0; i < count; i++)
                source.Add(null);

            var skewness = source.Skewness();

            if(IsDebugEnabled)
                log.Debug("seed=[{0}], step=[{1}], count=[{2}], skewness=[{3}]", seed, step, count, skewness);

            skewness.ApproximateEqual(0.0, 1e-5).Should().Be.True();
        }

        [TestCase(-1000, 1, 1000)]
        [TestCase(0, 1, 1000)]
        [TestCase(1000, 1, 1000)]
        public void Skewness_Int_Test(double seed, double step, int count) {
            var source = GetSerialList(seed, step, count).Select(x => (int)x).ToList();

            var skewness = source.Skewness();

            if(IsDebugEnabled)
                log.Debug("seed=[{0}], step=[{1}], count=[{2}], skewness=[{3}]", seed, step, count, skewness);

            skewness.ApproximateEqual(0.0, 1e-5).Should().Be.True();
        }

        [TestCase(-1000, 1, 1000)]
        [TestCase(0, 1, 1000)]
        [TestCase(1000, 1, 1000)]
        public void Skewness_Nullable_Int_Test(double seed, double step, int count) {
            var source = GetSerialList(seed, step, count).Select(x => (int?)x).ToList();

            Assert.AreEqual(count, source.Count());
            Assert.AreEqual(count, source.Count(x => x.HasValue));

            for(int i = 0; i < count; i++)
                source.Add(null);

            var skewness = source.Skewness();

            if(IsDebugEnabled)
                log.Debug("seed=[{0}], step=[{1}], count=[{2}], skewness=[{3}]", seed, step, count, skewness);

            skewness.ApproximateEqual(0.0, 1e-5).Should().Be.True();
        }
    }
}