using System;
using System.Collections.Generic;
using System.Linq;

namespace NSoft.NFramework.Numerics.Distributions.Discrete {
    /// <summary>
    /// Implements the binomial distribution. For details about this distribution, see 
    /// <a href="http://en.wikipedia.org/wiki/Binomial_distribution">Wikipedia - Binomial distribution</a>.
    /// </summary>
    /// <remarks><para>The distribution is parameterized by a probability (between 0.0 and 1.0).</para>
    /// <para>The distribution will use the <see cref="System.Random"/> by default. 
    /// Users can set the random number generator by using the <see cref="RandomSource"/> property.</para>
    /// <para>The statistics classes will check all the incoming parameters whether they are in the allowed
    /// range. This might involve heavy computation. Optionally, by setting Control.CheckDistributionParameters
    /// to <c>false</c>, all parameter checks can be turned off.</para></remarks>
    public sealed class Binomial : IDiscreteDistribution {
        /// <summary>
        /// 확률
        /// </summary>
        private double _p;

        /// <summary>
        /// 시도 횟수
        /// </summary>
        private int _n;

        private Random _random;

        public Binomial(double p, int n, Func<Random> randomFactory = null) {
            SetParameters(p, n);
        }

        private void SetParameters(double p, int n) {
            AssertParameters(p, n);
            _p = p;
            _n = n;
        }

        /// <summary>
        /// 성공 확률 (Success Probability)
        /// </summary>
        public double P {
            get { return _p; }
            set { SetParameters(value, _n); }
        }

        /// <summary>
        /// 시도 횟수 (Number of Trials)
        /// </summary>
        public int N {
            get { return _n; }
            set { SetParameters(_p, value); }
        }

        public override string ToString() {
            return string.Format("Binomial(Success Probability=[{0}], Number of Trials=[{1}]", _p, _n);
        }

        #region Implementation of IDistribution

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
            get { return _p * _n; }
        }

        /// <summary>
        /// 분산
        /// </summary>
        public double Variance {
            get { return _p * (1.0 - _p) * _n; }
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
            get {
                if(_p.ApproximateEqual(0.0) || _p.ApproximateEqual(1.0))
                    return 0.0;

                var e = 0.0;
                for(var i = 0; i <= _n; i++) {
                    var p = Probability(i);
                    e -= p * Math.Log(p);
                }

                return e;
            }
        }

        /// <summary>
        /// 기울기
        /// </summary>
        public double Skewness {
            get { return (1.0 - (2.0 * _p)) / Math.Sqrt(_n * _p * (1.0 - _p)); }
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
            if(x < 0.0) {
                return 0.0;
            }

            if(x > _n) {
                return 1.0;
            }

            var cdf = 0.0;
            for(var i = 0; i <= (int)Math.Floor(x); i++) {
                cdf += MathTool.Combinations(_n, i) * Math.Pow(_p, i) * Math.Pow(1.0 - _p, _n - i);
            }

            return cdf;
        }

        #endregion

        #region Implementation of IDiscreteDistribution

        /// <summary>
        /// 최빈값 (빈도수가 최대인 값)
        /// </summary>
        public int Mode {
            get {
                if(_p.ApproximateEqual(1.0))
                    return _n;

                if(_p.ApproximateEqual(0.0))
                    return 0;

                return (int)Math.Floor((_n + 1) * _p);
            }
        }

        /// <summary>
        /// 중앙값
        /// </summary>
        public int Median {
            get { return (int)Math.Floor(_p * _n); }
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
            get { return _n; }
        }

        /// <summary>
        /// 분포의 확률
        /// </summary>
        /// <param name="k">시도 횟수</param>
        /// <returns></returns>
        public double Probability(int k) {
            if(k < 0)
                return 0.0;

            if(k > _n)
                return 0.0;

            if(_p.ApproximateEqual(0.0) && k == 0)
                return 1.0;

            if(_p.ApproximateEqual(0.0))
                return 0.0;

            if(_p.ApproximateEqual(1.0) && k == _n)
                return 1.0;


            if(_p.ApproximateEqual(1.0))
                return 0.0;

            return SpecialFunctions.Binomial(_n, k) * Math.Pow(_p, k) * Math.Pow(1.0 - _p, _n - k);
        }

        /// <summary>
        /// 분포의 Log 확률
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public double ProbabilityLn(int k) {
            if(k < 0)
                return Double.NegativeInfinity;

            if(k > _n)
                return Double.NegativeInfinity;

            if(_p.ApproximateEqual(0.0) && k == 0)
                return 0.0;

            if(_p.ApproximateEqual(0.0))
                return Double.NegativeInfinity;

            if(_p.ApproximateEqual(1.0) && k == _n)
                return 0.0;

            if(_p.ApproximateEqual(1.0))
                return Double.NegativeInfinity;

            return SpecialFunctions.BinomialLn(_n, k) + (k * Math.Log(_p)) + ((_n - k) * Math.Log(1.0 - _p));
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
            return DoSamples(RandomSource, _p, _n);
        }

        #endregion

        //! =========================================

        private static void AssertParameters(double p, int n) {
            p.ShouldBeBetween(0.0, 1.0, "p");
            n.ShouldBePositiveOrZero("n");
        }

        private static IEnumerable<int> DoSamples(Random rnd, double p, int n) {
            while(true) {
                var k = 0;
                for(var i = 0; i < n; i++) {
                    k += rnd.NextDouble() < p ? 1 : 0;
                }

                yield return k;
            }
        }

        public static int Sample(Random rnd, double p, int n) {
            return Samples(rnd, p, n).First();
        }

        public static IEnumerable<int> Samples(Random rnd, double p, int n) {
            rnd.ShouldNotBeNull("rnd");
            AssertParameters(p, n);

            return DoSamples(rnd, p, n);
        }
    }
}