using System;
using System.Collections.Generic;

namespace NSoft.NFramework.Numerics.Distributions.Continuous {
    /// <summary>
    /// 정규 분포.
    /// Implements the univariate Normal (or Gaussian) distribution. For details about this distribution, see 
    /// <a href="http://en.wikipedia.org/wiki/Normal_distribution">Wikipedia - Normal distribution</a>.
    /// </summary>
    /// <remarks><para>The distribution will use the <see cref="System.Random"/> by default. 
    /// Users can get/set the random number generator by using the <see cref="RandomSource"/> property.</para>
    /// <para>The statistics classes will check all the incoming parameters whether they are in the allowed
    /// range. This might involve heavy computation. Optionally, by setting Control.CheckDistributionParameters
    /// to <c>false</c>, all parameter checks can be turned off.</para></remarks>
    public class Normal : IContinuousDistribution {
        public static Normal WithMeanStDev(double mean, double stDev) {
            return new Normal(mean, stDev);
        }

        public static Normal WithMeanVariance(double mean, double variance) {
            return WithMeanStDev(mean, Math.Sqrt(variance));
        }

        public static Normal WithMeanPrecision(double mean, double precision) {
            return WithMeanStDev(mean, 1.0 / Math.Sqrt(precision));
        }

        /// <summary>
        /// 평균
        /// </summary>
        private double _mean;

        /// <summary>
        /// 표준편차
        /// </summary>
        private double _stDev;

        private Random _random;

        /// <summary>
        /// 생성자
        /// </summary>
        public Normal() : this(0.0, 1.0) {}

        /// <summary>
        /// 정규분포 생성자
        /// </summary>
        /// <param name="mean">평균</param>
        /// <param name="stDev">표준편차</param>
        /// <param name="randomFactory">난수발생기 Factory</param>
        public Normal(double mean = 0.0, double stDev = 1.0, Func<Random> randomFactory = null) {
            SetParameters(mean, stDev);
            _random = (randomFactory != null) ? randomFactory() : MathTool.GetRandomFactory().Invoke();
        }

        private void SetParameters(double mean, double stdDev) {
            AssertParameters(mean, stdDev);
            _mean = mean;
            _stDev = stdDev;
        }

        public override string ToString() {
            return string.Format("Normal(mean=[{0}], stDev=[{1}])", _mean, _stDev);
        }

        /// <summary>
        /// 정규분포의 정밀도
        /// </summary>
        public double Precision {
            get { return 1.0 / (_stDev * _stDev); }
            set {
                var sdev = 1.0 / Math.Sqrt(value);
                if(double.IsInfinity(sdev))
                    sdev = double.PositiveInfinity;
                SetParameters(_mean, sdev);
            }
        }

        #region Implementation of IDistribution

        /// <summary>
        /// 난수 발생기
        /// TODO: Randomizer로 명칭 변경 
        /// </summary>
        public Random RandomSource {
            get { return _random; }
            set {
                value.ShouldNotBeNull("RandomSource");
                _random = value;
            }
        }

        /// <summary>
        /// 평균
        /// </summary>
        public double Mean {
            get { return _mean; }
            set { SetParameters(value, _stDev); }
        }

        /// <summary>
        /// 분산
        /// </summary>
        public double Variance {
            get { return _stDev * _stDev; }
            set { SetParameters(_mean, Math.Sqrt(value)); }
        }

        /// <summary>
        /// 표준편차
        /// </summary>
        public double StDev {
            get { return _stDev; }
            set { SetParameters(_mean, value); }
        }

        /// <summary>
        /// 분포의 엔트로피 (불안정성) 
        /// </summary>
        public double Entropy {
            get { return Math.Log(_stDev) + MathTool.LnSqrtPi2E; }
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
            get { return 0.0; }
        }

        /// <summary>
        /// 확률 분포 계산을 위한 누적분포함수
        /// </summary>
        /// <param name="x">The location at which to compute the cumulative distribution function.</param>
        /// <returns>the cumulative distribution at location <paramref name="x"/>.</returns>
        public double CumulativeDistribution(double x) {
            return CumulativeDistribution(_mean, _stDev, x);
        }

        /// <summary>
        /// Computes the cumulative distribution function of the normal distribution.
        /// </summary>
        /// <param name="mean">The mean of the normal distribution.</param>
        /// <param name="sdev">The standard deviation of the normal distribution.</param>
        /// <param name="x">The location at which to compute the cumulative density.</param>
        /// <returns>the cumulative density at <paramref name="x"/>.</returns>
        internal static double CumulativeDistribution(double mean, double sdev, double x) {
            return 0.5 * (1.0 + SpecialFunctions.Erf((x - mean) / (sdev * MathTool.Sqrt2)));
        }

        #endregion

        #region Implementation of IContinuousDistribution

        /// <summary>
        /// 최빈값 (빈도수가 최대인 값)
        /// </summary>
        public double Mode {
            get { return _mean; }
        }

        /// <summary>
        /// 중앙값
        /// </summary>
        public double Median {
            get { return _mean; }
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
            return Density(_mean, _stDev, x);
        }

        /// <summary>
        /// 분포의 로그 확률 밀도 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double DensityLn(double x) {
            return DensityLn(_mean, _stDev, x);
        }

        /// <summary>
        /// 분포의 무작위 값을 제공합니다.
        /// </summary>
        /// <returns></returns>
        public double Sample() {
            return Sample(RandomSource, Mean, StDev);
        }

        /// <summary>
        /// 현 분포의 무작위 값을 열거합니다.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<double> Samples() {
            return Samples(RandomSource, Mean, StDev);
        }

        #endregion

        /// <summary>
        /// Computes the inverse cumulative distribution function of the normal distribution.
        /// </summary>
        /// <param name="p">The location at which to compute the inverse cumulative density.</param>
        /// <returns>the inverse cumulative density at <paramref name="p"/>.</returns>
        public double InverseCumulativeDistribution(double p) {
            return _mean - (_stDev * Math.Sqrt(2.0) * SpecialFunctions.ErfcInv(2.0 * p));
        }

        //! ===============================================

        private static void AssertParameters(double mean, double stddev) {
            mean.ShouldNotBeNaN("mean");
            stddev.ShouldNotBeNaN("stDev");
            stddev.ShouldBePositiveOrZero("stDev");
        }

        /// <summary>
        /// 정규분포의 밀도를 계산합니다.
        /// </summary>
        /// <param name="mean"></param>
        /// <param name="stDev"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double Density(double mean, double stDev, double x) {
            var d = (x - mean) / stDev;
            return Math.Exp(-0.5 * d * d) / (MathTool.SqrtPi2 * stDev);
        }

        /// <summary>
        /// 정규분포의 로그 밀도를 계산합니다.
        /// </summary>
        /// <param name="mean"></param>
        /// <param name="stDev"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double DensityLn(double mean, double stDev, double x) {
            var d = (x - mean) / stDev;
            return (-0.5 * d * d) - Math.Log(stDev) - MathTool.LnSqrtPi2;
        }

        /// <summary>
        /// Generates a sample from the normal distribution using the <i>Box-Muller</i> algorithm.
        /// </summary>
        /// <param name="rnd">The random number generator to use.</param>
        /// <param name="mean">The mean of the normal distribution from which to generate samples.</param>
        /// <param name="stDev">The standard deviation of the normal distribution from which to generate samples.</param>
        /// <returns>a sample from the distribution.</returns>
        public static double Sample(Random rnd, double mean, double stDev) {
            AssertParameters(mean, stDev);

            return mean + (stDev * SampleBoxMuller(rnd).Item1);
        }

        /// <summary>
        /// Generates a sequence of samples from the normal distribution using the <i>Box-Muller</i> algorithm.
        /// </summary>
        /// <param name="rnd">The random number generator to use.</param>
        /// <param name="mean">The mean of the normal distribution from which to generate samples.</param>
        /// <param name="stDev">The standard deviation of the normal distribution from which to generate samples.</param>
        /// <returns>a sequence of samples from the distribution.</returns>
        public static IEnumerable<double> Samples(Random rnd, double mean, double stDev) {
            AssertParameters(mean, stDev);

            while(true) {
                var sample = SampleBoxMuller(rnd);
                yield return mean + (stDev * sample.Item1);
                yield return mean + (stDev * sample.Item2);
            }
        }

        /// <summary>
        /// Samples a pair of standard normal distributed random variables using the <i>Box-Muller</i> algorithm.
        /// </summary>
        /// <param name="rnd">The random number generator to use.</param>
        /// <returns>a pair of random numbers from the standard normal distribution.</returns>
        internal static Tuple<double, double> SampleBoxMuller(Random rnd) {
            var v1 = (2.0 * rnd.NextDouble()) - 1.0;
            var v2 = (2.0 * rnd.NextDouble()) - 1.0;
            var r = (v1 * v1) + (v2 * v2);
            while(r >= 1.0 || Math.Abs(r - 0.0) < double.Epsilon) {
                v1 = (2.0 * rnd.NextDouble()) - 1.0;
                v2 = (2.0 * rnd.NextDouble()) - 1.0;
                r = (v1 * v1) + (v2 * v2);
            }

            var fac = Math.Sqrt(-2.0 * Math.Log(r) / r);
            return new Tuple<double, double>(v1 * fac, v2 * fac);
        }
    }
}