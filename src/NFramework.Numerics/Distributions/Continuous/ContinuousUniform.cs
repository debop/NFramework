using System;
using System.Collections.Generic;

namespace NSoft.NFramework.Numerics.Distributions.Continuous {
    /// <summary>
    /// 균일 연속 분포
    /// The continuous uniform distribution is a distribution over real numbers. For details about this distribution, see 
    /// <a href="http://en.wikipedia.org/wiki/Uniform_distribution_%28continuous%29">Wikipedia - Continuous uniform distribution</a>.
    /// </summary>
    /// <remarks><para>The distribution will use the <see cref="System.Random"/> by default. 
    /// Users can get/set the random number generator by using the <see cref="RandomSource"/> property.</para>
    /// <para>The statistics classes will check all the incoming parameters whether they are in the allowed
    /// range. This might involve heavy computation. Optionally, by setting Control.CheckDistributionParameters
    /// to <c>false</c>, all parameter checks can be turned off.</para></remarks>
    public sealed class ContinuousUniform : IContinuousDistribution {
        private double _lower;
        private double _upper;
        private Random _random;

        public ContinuousUniform(double lower, double upper, Func<Random> randomFactory = null) {
            SetParameters(lower, upper);

            _random = (randomFactory != null) ? randomFactory() : MathTool.GetRandomFactory().Invoke();
        }

        private void SetParameters(double lower, double upper) {
            AssertParameters(lower, upper);

            _lower = lower;
            _upper = upper;
        }

        /// <summary>
        /// 하한
        /// </summary>
        public double Lower {
            get { return _lower; }
            set { SetParameters(value, _upper); }
        }

        /// <summary>
        /// 상한
        /// </summary>
        public double Upper {
            get { return _upper; }
            set { SetParameters(_lower, value); }
        }

        public override string ToString() {
            return string.Format("ContinuousUniform(Lower=[{0}], Upper=[{1}])", Mean);
        }

        //! ===================================================================

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
            get { return (_lower + _upper) / 2.0; }
        }

        /// <summary>
        /// 분산
        /// </summary>
        public double Variance {
            get { return (_upper - _lower).Square() / 12.0; }
        }

        /// <summary>
        /// 표준편차
        /// </summary>
        public double StDev {
            get { return Math.Sqrt(Variance); }
        }

        /// <summary>
        /// 분포의 엔트로피 (불안정성) 
        /// </summary>
        public double Entropy {
            get { return Math.Log(_upper - _lower); }
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
            if(x <= _lower) {
                return 0.0;
            }

            if(x >= _upper) {
                return 1.0;
            }

            return (x - _lower) / (_upper - _lower);
        }

        #endregion

        #region Implementation of IContinuousDistribution

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
            get { return _lower; }
        }

        /// <summary>
        /// 최대값
        /// </summary>
        public double Maximum {
            get { return _upper; }
        }

        /// <summary>
        /// 분포의 확률 밀도
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double Density(double x) {
            if(x >= _lower && x <= _upper) {
                return 1.0 / (_upper - _lower);
            }

            return 0.0;
        }

        /// <summary>
        /// 분포의 로그 확률 밀도 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double DensityLn(double x) {
            if(x >= _lower && x <= _upper) {
                return -Math.Log(_upper - _lower);
            }

            return Double.NegativeInfinity;
        }

        /// <summary>
        /// 분포의 무작위 값을 제공합니다.
        /// </summary>
        /// <returns></returns>
        public double Sample() {
            return DoSample(_random, _lower, _upper);
        }

        /// <summary>
        /// 현 분포의 무작위 값을 열거합니다.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<double> Samples() {
            while(true)
                yield return DoSample(_random, _lower, _upper);
        }

        #endregion

        //! ===================================================================

        private static void AssertParameters(double lower, double upper) {
            Guard.Assert(lower <= upper, "lower[{0}] <= upper[{1}] 이어야 합니다.", lower, upper);
        }

        private static double DoSample(Random rnd, double lower, double upper) {
            return lower + (rnd.NextDouble() * (upper - lower));
        }

        /// <summary>
        /// 연속 균일 분포의 샘플을 생성합니다.
        /// </summary>
        public static double Sample(Random rnd, double lower, double upper) {
            rnd.ShouldNotBeNull("rnd");
            AssertParameters(lower, upper);
            return DoSample(rnd, lower, upper);
        }

        /// <summary>
        /// 연속 균일 분포의 샘플을 생성합니다.
        /// </summary>
        public static IEnumerable<double> Samples(Random rnd, double lower, double upper) {
            rnd.ShouldNotBeNull("rnd");
            AssertParameters(lower, upper);

            while(true)
                yield return DoSample(rnd, lower, upper);
        }
    }
}