using System;
using System.Linq;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.Numerics.Statistics;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Numerics.Distributions.Discrete {
    [TestFixture]
    public class DiscreteUniformFixture : DistributionFixtureBase {
        [Test]
        [Sequential]
        public void CreateDiscreteUniformTest([Values(-10, 0, 10, 20)] int lower,
                                              [Values(10, 4, 20, 20)] int upper) {
            var uniform = new DiscreteUniform(lower, upper);

            uniform.LowerBound.Should().Be(lower);
            uniform.UpperBound.Should().Be(upper);
        }

        [Test, Sequential]
        public void DiscreteUniformCreateFailWithBadParameters([Values(-1, 100)] int lower, [Values(-2, -200)] int upper) {
            Assert.Throws<InvalidOperationException>(() => new DiscreteUniform(lower, upper));
        }

        [Test]
        public void SetLowerBound([Values(0, 3, 10)] int lower) {
            new DiscreteUniform(0, 10)
            {
                LowerBound = lower
            };
        }

        [Test]
        public void SetLowerBoundWithInvalidParameters([Values(11, 20)] int lower) {
            var uniform = new DiscreteUniform(0, 10);
            Assert.Throws<InvalidOperationException>(() => uniform.LowerBound = lower);
        }

        [Test]
        public void SetUpperBound([Values(0, 3, 10)] int upper) {
            new DiscreteUniform(0, 100)
            {
                UpperBound = upper
            };
        }

        [Test]
        public void SetUpperBoundWithInvalidParameters([Values(-1, -2)] int upper) {
            var uniform = new DiscreteUniform(0, 10);
            Assert.Throws<InvalidOperationException>(() => uniform.UpperBound = upper);
        }

        [Test]
        public void ValidateDistributionProperties([Range(-10, 10, 2)] int lower, [Range(100, 110, 2)] int upper) {
            var uniform = new DiscreteUniform(lower, upper);

            uniform.Entropy.Should().Be(Math.Log(upper - lower + 1.0));
            uniform.Skewness.Should().Be(0);
            uniform.Kurtosis.Should().Be(0);
            uniform.Mode.Should().Be((lower + upper) / 2);
            uniform.Median.Should().Be(uniform.Mode);

            uniform.Minumum.Should().Be(lower);
            uniform.Maximum.Should().Be(upper);
        }

        [Test]
        [Sequential]
        public void ProbabilityTest([Values(-10, -10, -10, -10, -10)] int lower,
                                    [Values(10, 10, 10, -10, -10)] int upper,
                                    [Values(-5, 1, 10, 0, -10)] int x,
                                    [Values(1 / 21.0, 1 / 21.0, 1 / 21.0, 0.0, 1.0)] double p) {
            var uniform = new DiscreteUniform(lower, upper);

            uniform.Probability(x).Should().Be(p);
        }

        [Test]
        [Sequential]
        public void SampleTest([Values(-10, 0, 10, 20)] int lower,
                               [Values(10, 4, 20, 20)] int upper) {
            var uniform = new DiscreteUniform(lower, upper);

            uniform.Sample().Should()
                .Be.GreaterThanOrEqualTo(lower)
                .And
                .Be.LessThanOrEqualTo(upper);
        }

        [Test]
        [Sequential]
        public void SamplesTest([Values(-10, 0, 10, 20)] int lower,
                                [Values(10, 4, 20, 20)] int upper) {
            var uniform = new DiscreteUniform(lower, upper);

            uniform.Samples().Take(100).RunEach(x => x.Should()
                                                         .Be.GreaterThanOrEqualTo(lower)
                                                         .And
                                                         .Be.LessThanOrEqualTo(upper));
        }

        [Test]
        [Sequential]
        public void CumulativeDistributionTest([Values(-10, -10, -10, -10, -10, -10)] int lower,
                                               [Values(10, 10, 10, -10, -10, -10)] int upper,
                                               [Values(-5, 1, 10, 0, -10, -11)] double x,
                                               [Values(6.0 / 21.0, 12.0 / 21.0, 1.0, 1.0, 1.0, 0.0)] double cdf) {
            var b = new DiscreteUniform(lower, upper);
            Assert.AreEqual(cdf, b.CumulativeDistribution(x));
        }

        [Test]
        public void StaticSampleUniform() {
            DiscreteUniform.Sample(new Random(), 0, 10).Should().Be.GreaterThanOrEqualTo(0).And.Be.LessThanOrEqualTo(10);
            DiscreteUniform.Samples(new Random(), 0, 10).Take(10).Where(x => x >= 0 && x <= 10).Count().Should().Be(10);

            Assert.Throws<InvalidOperationException>(() => DiscreteUniform.Sample(new Random(), 10, 0));
            Assert.Throws<InvalidOperationException>(() => DiscreteUniform.Samples(new Random(), 10, 0).First());
        }

        [Test]
        public void SamplesDistributionTest() {
            var source =
                DiscreteUniform.Samples(MathTool.GetRandomFactory()(), 0, 10).AsParallel().Take(99999).Select(x => (double)x).ToList();
            var histogram = new Histogram(source, 11, 0, 11);

            DisplayDistribution("DiscreteUniform", histogram);
        }
    }
}