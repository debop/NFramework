using System;
using System.Collections.Generic;

namespace NSoft.NFramework.Numerics.Distributions.Continuous {
    /// <summary>
    /// The Laplace distribution is a distribution over the real numbers parameterized by a mean and
    /// scale parameter. The PDF is:
    ///     p(x) = \frac{1}{2 * scale} \exp{- |x - mean| / scale}.
    /// <a href="http://en.wikipedia.org/wiki/Laplace_distribution">Wikipedia - Laplace distribution</a>.
    /// </summary>
    /// <remarks>The distribution will use the <see cref="System.Random"/> by default. 
    /// <para>Users can set the random number generator by using the <see cref="RandomSource"/> property.</para>
    /// <para>The statistics classes will check all the incoming parameters whether they are in the allowed
    /// range. This might involve heavy computation. Optionally, by setting Control.CheckDistributionParameters
    /// to <c>false</c>, all parameter checks can be turned off.</para></remarks>
    public sealed class Laplace : IContinuousDistribution {
        private double _scale;
        private Random _random;

        public Laplace() : this(0.0, 1.0) {}

        public Laplace(double location = 0.0, double scale = 1.0, Func<Random> randomFactory = null) {
            SetParameters(location, scale);
            RandomSource = (randomFactory != null) ? randomFactory() : MathTool.GetRandomFactory()();
        }

        private void SetParameters(double location, double scale) {
            AssertParameters(location, scale);

            _scale = scale;
            Mean = location;
        }

        public double Scale {
            get { return _scale; }
            set { SetParameters(Mean, value); }
        }

        public double Location {
            get { return Mean; }
            set { SetParameters(value, _scale); }
        }

        public override string ToString() {
            return string.Format("Laplace(Location=[{0}], Scale=[{1}])", Location, Scale);
        }

        #region << IDistribution >>

        /// <summary>
        /// 난수 발생기
        /// TODO: Randomizer로 명칭 변경 
        /// </summary>
        public Random RandomSource {
            get { return _random; }
            set {
                value.ShouldNotBeNull("value");
                _random = value;
            }
        }

        /// <summary>
        /// 평균
        /// </summary>
        public double Mean { get; private set; }

        /// <summary>
        /// 분산
        /// </summary>
        public double Variance {
            get { return 2.0 * _scale * _scale; }
        }

        /// <summary>
        /// 표준편차
        /// </summary>
        public double StDev {
            get { return Math.Sqrt(2.0) * _scale; }
        }

        /// <summary>
        /// 분포의 엔트로피 (불안정성) 
        /// </summary>
        public double Entropy {
            get { return Math.Log(2.0 * MathTool.E * _scale); }
        }

        /// <summary>
        /// 기울기
        /// </summary>
        public double Skewness {
            get { return 0.0; }
        }

        /// <summary>
        /// 첨도 (뽀족한 정도) (+) 값이면 뾰족하고, (-) 값이면 뭉툭하다
        /// </summary>
        public double Kurtosis {
            get { return 3.0; }
        }

        /// <summary>
        /// 확률 분포 계산을 위한 누적분포함수
        /// </summary>
        /// <param name="x">The location at which to compute the cumulative distribution function.</param>
        /// <returns>the cumulative distribution at location <paramref name="x"/>.</returns>
        public double CumulativeDistribution(double x) {
            return 0.5 * (1.0 + (Math.Sign(x - Mean) * (1.0 - Math.Exp(-Math.Abs(x - Mean) / _scale))));
        }

        #endregion

        #region << IContinuousDistribution >>

        /// <summary>
        /// 최빈값 (빈도수가 최대인 값)
        /// </summary>
        public double Mode {
            get { return Mean; }
        }

        /// <summary>
        /// 중앙값
        /// </summary>
        public double Median {
            get { return Mean; }
        }

        /// <summary>
        /// 최소값
        /// </summary>
        public double Minumum {
            get { return double.NegativeInfinity; }
        }

        /// <summary>
        /// 최대값
        /// </summary>
        public double Maximum {
            get { return double.PositiveInfinity; }
        }

        /// <summary>
        /// 분포의 확률 밀도
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double Density(double x) {
            return Math.Exp(-Math.Abs(x - Mean) / _scale) / (2.0 * _scale);
        }

        /// <summary>
        /// 분포의 로그 확률 밀도 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double DensityLn(double x) {
            return Math.Log(Density(x));
        }

        /// <summary>
        /// 분포의 무작위 값을 제공합니다.
        /// </summary>
        /// <returns></returns>
        public double Sample() {
            return DoSample(_random, Mean, _scale);
        }

        /// <summary>
        /// 현 분포의 무작위 값을 열거합니다.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<double> Samples() {
            while(true)
                yield return DoSample(_random, Mean, _scale);
        }

        #endregion

        //! ================================================

        private static void AssertParameters(double location, double scale) {
            location.ShouldNotBeNaN("location");
            scale.ShouldBePositive("scale");
        }

        private static double DoSample(Random rnd, double location, double scale) {
            var u = rnd.NextDouble() - 0.5;
            return location - (scale * Math.Sign(u) * Math.Log(1.0 - (2.0 * Math.Abs(u))));
        }

        public static double Sample(Random rnd, double location, double scale) {
            rnd.ShouldNotBeNull("rnd");
            AssertParameters(location, scale);

            return DoSample(rnd, location, scale);
        }

        public static IEnumerable<double> Samples(Random rnd, double location, double scale) {
            rnd.ShouldNotBeNull("rnd");
            AssertParameters(location, scale);

            while(true)
                yield return DoSample(rnd, location, scale);
        }
    }
}