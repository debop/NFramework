using System;
using System.Collections.Generic;
using System.Linq;

namespace NSoft.NFramework.Numerics.Distributions.Discrete {
    /// <summary>
    /// 정수값을 변량으로 균일하게 이산 분포를 합니다.. 이 분포는 상하한 값을 포함합니다.
    /// <a href="http://en.wikipedia.org/wiki/Uniform_distribution_%28discrete%29">Wikipedia - Discrete uniform distribution</a>.
    /// </summary>
    /// <remarks><para>The distribution will use the <see cref="System.Random"/> by default. 
    /// Users can set the random number generator by using the <see cref="RandomSource"/> property.</para>
    /// <para>The statistics classes will check all the incoming parameters whether they are in the allowed
    /// range. This might involve heavy computation. Optionally, by setting Control.CheckDistributionParameters
    /// to <c>false</c>, all parameter checks can be turned off.</para></remarks>
    public class DiscreteUniform : IDiscreteDistribution {
        private int _lower;
        private int _upper;
        private Random _random;

        public DiscreteUniform(int lower, int upper, Func<Random> randomFactory = null) {
            SetParameters(lower, upper);

            _random = (randomFactory != null) ? randomFactory() : MathTool.GetRandomFactory().Invoke();
        }

        private void SetParameters(int lower, int upper) {
            AssertParameters(lower, upper);
            _lower = lower;
            _upper = upper;
        }

        /// <summary>
        /// 하한
        /// </summary>
        public int LowerBound {
            get { return _lower; }
            set { SetParameters(value, _upper); }
        }

        /// <summary>
        /// 상한
        /// </summary>
        public int UpperBound {
            get { return _upper; }
            set { SetParameters(_lower, value); }
        }

        #region Implementation of IDistribution

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
        public double Mean {
            get { return (_lower + _upper) / 2.0; }
        }

        /// <summary>
        /// 분산
        /// </summary>
        public double Variance {
            get { return (((_upper - _lower + 1.0) * (_upper - _lower + 1.0)) - 1.0) / 12.0; }
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
            get { return Math.Log(_upper - _lower + 1.0); }
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
            if(x < _lower)
                return 0.0;

            if(x >= _upper)
                return 1.0;

            return Math.Min(1.0, (Math.Floor(x) - _lower + 1) / (_upper - _lower + 1));
        }

        #endregion

        #region Implementation of IDiscreteDistribution

        /// <summary>
        /// 최빈값 (빈도수가 최대인 값)
        /// </summary>
        public int Mode {
            get { return (int)Math.Floor((_lower + _upper) / 2.0); }
        }

        /// <summary>
        /// 중앙값
        /// </summary>
        public int Median {
            get { return Mode; }
        }

        /// <summary>
        /// 최소값
        /// </summary>
        public int Minumum {
            get { return _lower; }
        }

        /// <summary>
        /// 최대값
        /// </summary>
        public int Maximum {
            get { return _upper; }
        }

        /// <summary>
        /// 분포의 확률
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public double Probability(int k) {
            if(k >= _lower && k <= _upper)
                return 1.0 / (_upper - _lower + 1.0);

            return 0.0;
        }

        /// <summary>
        /// 분포의 Log 확률
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public double ProbabilityLn(int k) {
            if(k >= _lower && k <= _upper) {
                return -Math.Log(_upper - _lower + 1.0);
            }

            return Double.NegativeInfinity;
        }

        /// <summary>
        /// 분포의 데이타를 반환합니다.
        /// </summary>
        /// <returns></returns>
        public int Sample() {
            return Sample(_random, _lower, _upper);
        }

        /// <summary>
        /// 분포의 무작위 데이타를 열거합니다.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<int> Samples() {
            return Samples(_random, _lower, _upper);
        }

        #endregion

        //! ====================================================

        private static void AssertParameters(int lower, int upper) {
            Guard.Assert(upper >= lower, "상한[{0}] 값이 하한[{1}] 보다 크거나 같아야 합니다.", upper, lower);
        }

        public static int Sample(Random rnd, int lower, int upper) {
            return Samples(rnd, lower, upper).First();
        }

        public static IEnumerable<int> Samples(Random rnd, int lower, int upper) {
            AssertParameters(lower, upper);
            while(true)
                yield return (rnd.Next() % (upper - lower + 1)) + lower;
        }

        /// <summary>
        /// 상하한을 포함한 균일한 분포를 형성하는 데이타를 생성합니다.
        /// </summary>
        /// <param name="rnd"></param>
        /// <param name="lower">하한</param>
        /// <param name="upper">샹한</param>
        /// <returns></returns>
        internal int SampleUniform(Random rnd, int lower, int upper) {
            return (rnd.Next() % (upper - lower + 1)) + lower;
        }
    }
}