using System;
using System.Linq;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.Numerics.Statistics;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Numerics.Distributions.Continuous {
    [TestFixture]
    public class NormalFixture : DistributionFixtureBase {
        [Test]
        public void CreateStandardNormalDistribution() {
            var normal = new Normal();

            normal.Mean.Should().Be(0.0);
            normal.StDev.Should().Be(1.0);
        }

        [Test]
        public void CreateNormal([Values(0.0, 10.0, -5.0)] double mean,
                                 [Values(0.0, 0.1, 1.0, 10.0, 100.0, double.PositiveInfinity)] double stdev) {
            var normal = new Normal(mean, stdev);
            normal.Mean.Should().Be(mean);
            normal.StDev.Should().Be(stdev);
        }

        [Test, Sequential]
        public void CreateNormalWithInvalidParameters([Values(double.NaN, 1.0, double.NaN, 1.0)] double mean,
                                                      [Values(1.0, double.NaN, double.NaN, -1.0)] double stdev) {
            Assert.Throws<InvalidOperationException>(() => new Normal(mean, stdev));
        }

        [Test, Combinatorial]
        public void CreateNormalFromMeanAndVariance([Values(0.0, 10.0, -5.0)] double mean,
                                                    [Values(0.0, 0.1, 1.0, 10.0, 100.0, Double.PositiveInfinity)] double var) {
            var normal = Normal.WithMeanVariance(mean, var);

            normal.Mean.Clamp(mean).Should().Be(mean);
            normal.Variance.Clamp(var).Should().Be(var);
        }

        [Test, Combinatorial]
        public void CreateNormalFromMeanAndPrecision([Values(0.0, 10.0, -5.0)] double mean,
                                                     [Values(0.0, 0.1, 1.0, 10.0, 100.0, Double.PositiveInfinity)] double precision) {
            var normal = Normal.WithMeanPrecision(mean, precision);

            normal.Mean.Clamp(mean).Should().Be(mean);
            normal.Precision.Clamp(precision).Should().Be(precision);
        }

        [Test]
        public void SetPrecisionTest([Values(-0.0, 0.0, 0.1, 1.0, 10.0, Double.PositiveInfinity)] double precision) {
            new Normal { Precision = precision }.Should().Not.Be.Null();
        }

        [Test]
        public void SetPrecisionFailsWithNegativePrecision() {
            var n = new Normal();
            Assert.Throws<InvalidOperationException>(() => n.Precision = -1.0);
        }

        [Test]
        public void SetVarianceTest([Values(-0.0, 0.0, 0.1, 1.0, 10.0, Double.PositiveInfinity)] double variance) {
            new Normal { Variance = variance }.Should().Not.Be.Null();
        }

        [Test]
        public void SetVarianceFailsWithNegativeVariance() {
            var n = new Normal();
            Assert.Throws<InvalidOperationException>(() => n.Variance = -1.0);
        }

        [Test]
        public void SetStDevTest([Values(-0.0, 0.0, 0.1, 1.0, 10.0, Double.PositiveInfinity)] double stDev) {
            new Normal { StDev = stDev }.Should().Not.Be.Null();
        }

        [Test]
        public void SetStDevFailsWithNegativeStDev() {
            var n = new Normal();
            Assert.Throws<InvalidOperationException>(() => n.StDev = -1.0);
        }

        [Test]
        public void SetMeanTest([Values(-0.0, 0.0, 0.1, 1.0, 10.0, Double.PositiveInfinity)] double mean) {
            new Normal { Mean = mean }.Should().Not.Be.Null();
        }

        [Test]
        public void ValidateDistributionPropertiesByStDev([Values(-0.0, 0.0, 0.1, 1.0, 10.0, Double.PositiveInfinity)] double stdev) {
            var normal = new Normal(1.0, stdev);

            normal.Entropy.Should().Be(Math.Log(normal.StDev) + MathTool.LnSqrtPi2E);
            normal.Skewness.Should().Be(0.0);
        }

        [Test]
        public void ValidateDistributionPropertiesByMean([Values(-0.0, 0.0, 0.1, 1.0, 10.0, Double.PositiveInfinity)] double mean) {
            var normal = new Normal(mean, 1.0);

            normal.Mean.Should().Be(mean);
            normal.Median.Should().Be(mean);

            normal.Minumum.Should().Be(double.NegativeInfinity);
            normal.Maximum.Should().Be(double.PositiveInfinity);
        }

        [Test, Combinatorial]
        public void DensityTest([Values(0.0, 10.0, -5.0)] double mean,
                                [Values(0.0, 0.1, 1.0, 10.0, 100.0, Double.PositiveInfinity)] double sdev) {
            var n = Normal.WithMeanStDev(mean, sdev);

            for(var i = 0; i < 11; i++) {
                var x = i - 5.0;
                var d = (mean - x) / sdev;
                var pdf = Math.Exp(-0.5 * d * d) / (sdev * MathTool.SqrtPi2);

                n.Density(x).Should().Be(pdf);
            }
        }

        [Test, Combinatorial]
        public void DensityLnTest([Values(0.0, 10.0, -5.0)] double mean,
                                  [Values(0.0, 0.1, 1.0, 10.0, 100.0, Double.PositiveInfinity)] double sdev) {
            var n = Normal.WithMeanStDev(mean, sdev);

            for(var i = 0; i < 11; i++) {
                var x = i - 5.0;
                var d = (mean - x) / sdev;
                var pdfln = (-0.5 * (d * d)) - Math.Log(sdev) - MathTool.LnSqrtPi2;

                n.DensityLn(x).Should().Be(pdfln);
            }
        }

        [Test]
        public void SampleTest() {
            new Normal().Should().Not.Be(double.NaN);

            Assert.Throws<InvalidOperationException>(() => new Normal(0.0, -1.0));
        }

        [Test]
        public void SamplesTest() {
            new Normal().Samples().Take(100).RunEach(x => x.Should().Not.Be(double.NaN));

            Assert.Throws<InvalidOperationException>(() => new Normal(0.0, -1.0).Samples().Take(100));
        }

        /// <summary>
        /// Validate cumulative distribution.
        /// </summary>
        /// <param name="x">Input X value.</param>
        /// <param name="f">Expected value.</param>
        [Test, Sequential]
        public void ValidateCumulativeDistribution(
            [Values(Double.NegativeInfinity, -5.0, -2.0, -0.0, 0.0, 4.0, 5.0, 6.0, 10.0, Double.PositiveInfinity)] double x,
            [Values(0.0, 0.00000028665157187919391167375233287464535385442301361187883,
                0.0002326290790355250363499258867279847735487493358890356, 0.0062096653257761351669781045741922211278977469230927036,
                0.0062096653257761351669781045741922211278977469230927036, 0.30853753872598689636229538939166226011639782444542207, 0.5,
                0.69146246127401310363770461060833773988360217555457859, 0.9937903346742238648330218954258077788721022530769078, 1.0)] double f) {
            var n = Normal.WithMeanStDev(5.0, 2.0);
            n.CumulativeDistribution(x).Clamp(f, 1e-10).Should().Be(f);
        }

        /// <summary>
        /// Validate inverse cumulative distribution.
        /// </summary>
        /// <param name="x">Input X value.</param>
        /// <param name="f">Expected value.</param>
        [Test, Sequential]
        public void ValidateInverseCumulativeDistribution(
            [Values(Double.NegativeInfinity, -5.0, -2.0, -0.0, 0.0, 4.0, 5.0, 6.0, 10.0, Double.PositiveInfinity)] double x,
            [Values(0.0, 0.00000028665157187919391167375233287464535385442301361187883,
                0.0002326290790355250363499258867279847735487493358890356, 0.0062096653257761351669781045741922211278977469230927036,
                .0062096653257761351669781045741922211278977469230927036, .30853753872598689636229538939166226011639782444542207, .5,
                .69146246127401310363770461060833773988360217555457859, 0.9937903346742238648330218954258077788721022530769078, 1.0)] double f) {
            var n = Normal.WithMeanStDev(5.0, 2.0);
            n.InverseCumulativeDistribution(f).Clamp(x, 1e-10).Should().Be(x);
        }

        [Test]
        public void StaticSampleTest() {
            Normal.Sample(new Random(), 0.0, 1.0).Should().Not.Be(double.NaN);

            Assert.Throws<InvalidOperationException>(() => Normal.Sample(new Random(), 0.0, -1.0));
        }

        [Test]
        public void StaticSamplesTest() {
            Normal.Samples(new Random(), 0.0, 1.0).Take(100).RunEach(x => x.Should().Not.Be(double.NaN));

            Assert.Throws<InvalidOperationException>(() => Normal.Samples(new Random(), 0.0, -1.0).First());
        }

        [Test]
        public void SamplesDistributionTest() {
            var source = Normal.Samples(MathTool.GetRandomFactory()(), 0.0, 1.0).AsParallel().Take(99999).ToList();
            var histogram = new Histogram(source, 50);

            DisplayDistribution("Normal", histogram);
        }
    }
}