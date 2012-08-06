using System.Linq;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Numerics.Utils {
    [TestFixture]
    public class MathToolFixture_Kurtosis : AbstractMathToolFixtureBase {
        [TestCase(-1000, 1, 1000)]
        [TestCase(0, 1, 1000)]
        [TestCase(1000, 1, 1000)]
        public void Kurtosis_Double_Test(double seed, double step, int count) {
            var source = GetSerialList(seed, step, count).ToList();

            var kurtosis = source.Kurtosis();

            if(IsDebugEnabled)
                log.Debug("seed=[{0}], step=[{1}], count=[{2}], kurtosis=[{3}]", seed, step, count, kurtosis);

            kurtosis.ApproximateEqual(-1.2, 1e-5).Should().Be.True();
        }

        [TestCase(-1000, 1, 1000)]
        [TestCase(0, 1, 1000)]
        [TestCase(1000, 1, 1000)]
        public void Kurtosis_Nullable_Double_Test(double seed, double step, int count) {
            var source = GetSerialList(seed, step, count).Select(x => (double?)x).ToList();

            Assert.AreEqual(count, source.Count());
            Assert.AreEqual(count, source.Count(x => x.HasValue));

            for(int i = 0; i < count; i++)
                source.Add(null);

            var kurtosis = source.Kurtosis();

            if(IsDebugEnabled)
                log.Debug("seed=[{0}], step=[{1}], count=[{2}], kurtosis=[{3}]", seed, step, count, kurtosis);

            kurtosis.ApproximateEqual(-1.2, 1e-5).Should().Be.True();
        }

        [TestCase(-1000, 1, 1000)]
        [TestCase(0, 1, 1000)]
        [TestCase(1000, 1, 1000)]
        public void Kurtosis_Float_Test(double seed, double step, int count) {
            var source = GetSerialList(seed, step, count).Select(x => (float)x).ToList();

            var kurtosis = source.Kurtosis();

            if(IsDebugEnabled)
                log.Debug("seed=[{0}], step=[{1}], count=[{2}], kurtosis=[{3}]", seed, step, count, kurtosis);

            kurtosis.ApproximateEqual(-1.2f, 1e-5f).Should().Be.True();
        }

        [TestCase(-1000, 1, 1000)]
        [TestCase(0, 1, 1000)]
        [TestCase(1000, 1, 1000)]
        public void Kurtosis_Nullable_Float_Test(double seed, double step, int count) {
            var source = GetSerialList(seed, step, count).Select(x => (float?)x).ToList();

            Assert.AreEqual(count, source.Count());
            Assert.AreEqual(count, source.Count(x => x.HasValue));

            for(int i = 0; i < count; i++)
                source.Add(null);

            var kurtosis = source.Kurtosis();

            if(IsDebugEnabled)
                log.Debug("seed=[{0}], step=[{1}], count=[{2}], kurtosis=[{3}]", seed, step, count, kurtosis);

            kurtosis.ApproximateEqual(-1.2f, 1e-5f).Should().Be.True();
        }

        [TestCase(-1000, 1, 1000)]
        [TestCase(0, 1, 1000)]
        [TestCase(1000, 1, 1000)]
        public void Kurtosis_Decimal_Test(double seed, double step, int count) {
            var source = GetSerialList(seed, step, count).Select(x => (decimal)x).ToList();

            var kurtosis = source.Kurtosis();

            if(IsDebugEnabled)
                log.Debug("seed=[{0}], step=[{1}], count=[{2}], kurtosis=[{3}]", seed, step, count, kurtosis);

            kurtosis.ApproximateEqual(-1.2m, 1e-5m).Should().Be.True();
        }

        [TestCase(-1000, 1, 1000)]
        [TestCase(0, 1, 1000)]
        [TestCase(1000, 1, 1000)]
        public void Kurtosis_Nullable_Decimal_Test(double seed, double step, int count) {
            var source = GetSerialList(seed, step, count).Select(x => (decimal?)x).ToList();

            Assert.AreEqual(count, source.Count());
            Assert.AreEqual(count, source.Count(x => x.HasValue));

            for(int i = 0; i < count; i++)
                source.Add(null);

            var kurtosis = source.Kurtosis();

            if(IsDebugEnabled)
                log.Debug("seed=[{0}], step=[{1}], count=[{2}], kurtosis=[{3}]", seed, step, count, kurtosis);

            kurtosis.ApproximateEqual(-1.2m, 1e-5m).Should().Be.True();
        }

        [TestCase(-1000, 1, 1000)]
        [TestCase(0, 1, 1000)]
        [TestCase(1000, 1, 1000)]
        public void Kurtosis_Long_Test(double seed, double step, int count) {
            var source = GetSerialList(seed, step, count).Select(x => (long)x).ToList();

            var kurtosis = source.Kurtosis();

            if(IsDebugEnabled)
                log.Debug("seed=[{0}], step=[{1}], count=[{2}], kurtosis=[{3}]", seed, step, count, kurtosis);

            kurtosis.ApproximateEqual(-1.2, 1e-5).Should().Be.True();
        }

        [TestCase(-1000, 1, 1000)]
        [TestCase(0, 1, 1000)]
        [TestCase(1000, 1, 1000)]
        public void Kurtosis_Nullable_Long_Test(double seed, double step, int count) {
            var source = GetSerialList(seed, step, count).Select(x => (long?)x).ToList();

            Assert.AreEqual(count, source.Count());
            Assert.AreEqual(count, source.Count(x => x.HasValue));

            for(int i = 0; i < count; i++)
                source.Add(null);

            var kurtosis = source.Kurtosis();

            if(IsDebugEnabled)
                log.Debug("seed=[{0}], step=[{1}], count=[{2}], kurtosis=[{3}]", seed, step, count, kurtosis);

            kurtosis.ApproximateEqual(-1.2, 1e-5).Should().Be.True();
        }

        [TestCase(-1000, 1, 1000)]
        [TestCase(0, 1, 1000)]
        [TestCase(1000, 1, 1000)]
        public void Kurtosis_Int_Test(double seed, double step, int count) {
            var source = GetSerialList(seed, step, count).Select(x => (int)x).ToList();

            var kurtosis = source.Kurtosis();

            if(IsDebugEnabled)
                log.Debug("seed=[{0}], step=[{1}], count=[{2}], kurtosis=[{3}]", seed, step, count, kurtosis);

            kurtosis.ApproximateEqual(-1.2, 1e-5).Should().Be.True();
        }

        [TestCase(-1000, 1, 1000)]
        [TestCase(0, 1, 1000)]
        [TestCase(1000, 1, 1000)]
        public void Kurtosis_Nullable_Int_Test(double seed, double step, int count) {
            var source = GetSerialList(seed, step, count).Select(x => (int?)x).ToList();

            Assert.AreEqual(count, source.Count());
            Assert.AreEqual(count, source.Count(x => x.HasValue));

            for(int i = 0; i < count; i++)
                source.Add(null);

            var kurtosis = source.Kurtosis();

            if(IsDebugEnabled)
                log.Debug("seed=[{0}], step=[{1}], count=[{2}], kurtosis=[{3}]", seed, step, count, kurtosis);

            kurtosis.ApproximateEqual(-1.2, 1e-5).Should().Be.True();
        }
    }
}