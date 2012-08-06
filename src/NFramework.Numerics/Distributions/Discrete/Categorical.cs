using System;
using System.Collections.Generic;
using System.Linq;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.Numerics.Statistics;

namespace NSoft.NFramework.Numerics.Distributions.Discrete {
    /// <summary>
    /// Implements the categorical distribution. For details about this distribution, see 
    /// <a href="http://en.wikipedia.org/wiki/Categorical_distribution">Wikipedia - Categorical distribution</a>. This
    /// distribution is sometimes called the Discrete distribution.
    /// </summary>
    /// <remarks><para>The distribution is parameterized by a vector of ratios: in other words, the parameter
    /// does not have to be normalized and sum to 1. The reason is that some vectors can't be exactly normalized
    /// to sum to 1 in floating point representation.</para>
    /// <para>The distribution will use the <see cref="System.Random"/> by default. 
    /// Users can set the random number generator by using the <see cref="RandomSource"/> property.</para>
    /// <para>The statistics classes will check all the incoming parameters whether they are in the allowed
    /// range. This might involve heavy computation. Optionally, by setting Control.CheckDistributionParameters
    /// to <c>false</c>, all parameter checks can be turned off.</para></remarks>
    public sealed class Categorical : IDiscreteDistribution {
        private double[] _p;
        private Random _random;

        public Categorical(double[] p, Func<Random> randomFactory = null) {
            SetParameters(p);
            _random = (randomFactory != null) ? randomFactory() : MathTool.GetRandomFactory().Invoke();
        }

        public Categorical(Histogram histogram, Func<Random> randomFactory = null) {
            histogram.ShouldNotBeNull("histogram");
            var p = histogram.GetCounts();

            SetParameters(p);
            _random = (randomFactory != null) ? randomFactory() : MathTool.GetRandomFactory().Invoke();
        }

        private void SetParameters(double[] p) {
            AssertParameters(p);
            _p = (double[])p.Clone();
        }

        /// <summary>
        /// he normalized probability vector of the multinomial.
        /// </summary>
        public double[] P {
            get { return _p.Normalize().ToArray(); }
            set { SetParameters(value); }
        }

        public override string ToString() {
            return string.Format("Categorical(Dimension=[{0}])", _p.Length);
        }

        #region << IDistribution >>

        /// <summary>
        /// 난수 발생기
        /// TODO: Randomizer로 명칭 변경 
        /// </summary>
        public Random RandomSource {
            get { return _random ?? (_random = MathTool.GetRandomFactory().Invoke()); }
            set { _random = value; }
        }

        /// <summary>
        /// 평균
        /// </summary>
        public double Mean {
            get { return _p.Mean(); }
        }

        /// <summary>
        /// 분산
        /// </summary>
        public double Variance {
            get { return _p.Variance(); }
        }

        /// <summary>
        /// 표준편차
        /// </summary>
        public double StDev {
            get { return _p.StDev(); }
        }

        /// <summary>
        /// 분포의 엔트로피 (불안정성) 
        /// </summary>
        public double Entropy {
            get { return _p.Sum(p => p * Math.Log(p)); }
        }

        /// <summary>
        /// 기울기
        /// </summary>
        public double Skewness {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// 첨도 (뽀족한 정도) (+) 값이면 뾰족하고, (-) 값이면 뭉툭하다
        /// </summary>
        public double Kurtosis {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// 확률 분포 계산을 위한 누적분포함수
        /// </summary>
        /// <param name="x">The location at which to compute the cumulative distribution function.</param>
        /// <returns>the cumulative distribution at location <paramref name="x"/>.</returns>
        public double CumulativeDistribution(double x) {
            if(x < 0.0)
                return 0.0;

            if(x >= _p.Length)
                return 1.0;

            var cdf = UnnormalizedCdf(_p);
            return cdf[(int)Math.Floor(x)] / cdf[_p.Length - 1];
        }

        #endregion

        #region Implementation of IDiscreteDistribution

        /// <summary>
        /// 최빈값 (빈도수가 최대인 값)
        /// </summary>
        public int Mode {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// 중앙값
        /// </summary>
        public int Median {
            get { return (int)_p.Median(); }
        }

        /// <summary>
        /// 최소값
        /// </summary>
        public int Minumum {
            get { return 0; }
        }

        /// <summary>
        /// 최대값
        /// </summary>
        public int Maximum {
            get { return _p.Length - 1; }
        }

        /// <summary>
        /// 분포의 확률
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public double Probability(int k) {
            if(k < 0 || k >= _p.Length)
                return 0.0;

            return _p[k];
        }

        /// <summary>
        /// 분포의 Log 확률
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public double ProbabilityLn(int k) {
            if(k < 0 || k >= _p.Length)
                return 0.0;

            return Math.Log(_p[k]);
        }

        /// <summary>
        /// 분포의 데이타를 반환합니다.
        /// </summary>
        /// <returns></returns>
        public int Sample() {
            return Samples().First();
        }

        /// <summary>
        /// 분포의 무작위 데이타를 열거합니다.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<int> Samples() {
            return DoSamples(RandomSource, _p);
        }

        #endregion

        private static void AssertParameters(IEnumerable<double> p) {
            p.ShouldNotBeEmpty("p");
            Guard.Assert(p.Any(x => x < 0.0 || double.IsNaN(x)) == false, "요소 중에 음수나 double.NaN 값이 있으면 안됩니다.");
            Guard.Assert(p.Sum().ApproximateEqual(0.0) == false, "전체 합이 0이면 안됩니다.");
        }

        /// <summary>
        /// Computes the unnormalized cumulative distribution function. This method performs no
        /// parameter checking.
        /// </summary>
        /// <param name="p">An array of nonnegative ratios: this array does not need to be normalized 
        /// as this is often impossible using floating point arithmetic.</param>
        /// <returns>An array representing the unnormalized cumulative distribution function.</returns>
        internal static double[] UnnormalizedCdf(double[] p) {
            var cp = (double[])p.Clone();

            for(var i = 1; i < p.Length; i++) {
                cp[i] += cp[i - 1];
            }

            return cp;
        }

        /// <summary>
        /// Returns one trials from the categorical distribution.
        /// </summary>
        /// <param name="rnd">The random number generator to use.</param>
        /// <param name="cdf">The cumulative distribution of the probability distribution.</param>
        /// <returns>sequence of sample from the categorical distribution implied by <paramref name="cdf"/>.</returns>
        internal static IEnumerable<int> DoSamples(Random rnd, double[] cdf) {
            while(true) {
                var u = rnd.NextDouble() * cdf[cdf.Length - 1];
                var index = EnumerableTool.IndexOf(cdf, cdf.FirstOrDefault(x => u > x));

                yield return Math.Max(0, index);
            }
        }
    }
}