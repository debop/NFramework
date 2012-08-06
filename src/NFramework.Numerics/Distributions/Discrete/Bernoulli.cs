using System;
using System.Collections.Generic;
using System.Linq;

namespace NSoft.NFramework.Numerics.Distributions.Discrete {
    /// <summary>
    /// 베르누이 분포
    /// </summary>
    public sealed class Bernoulli : IDiscreteDistribution {
        private double _p;
        private Random _random;

        public Bernoulli(double p = 0.0, Func<Random> randomFactory = null) {
            SetParameters(p);

            if(randomFactory != null)
                _random = randomFactory();
        }

        private void SetParameters(double p) {
            AssertParameters(p);
            _p = p;
        }

        public double P {
            get { return _p; }
            set { SetParameters(value); }
        }

        public override string ToString() {
            return string.Format("Bernoulli(P=[{0}])", _p);
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
            get { return _p; }
        }

        /// <summary>
        /// 분산
        /// </summary>
        public double Variance {
            get { return _p * (1.0 - _p); }
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
            get { return -(_p * Math.Log(_p)) - ((1.0 - _p) * Math.Log(1.0 - _p)); }
        }

        /// <summary>
        /// 기울기
        /// </summary>
        public double Skewness {
            get { return (1.0 - (2.0 * _p)) / Math.Sqrt(_p * (1.0 - _p)); }
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

            if(x < 1.0)
                return 1.0 - _p;

            return 1.0;
        }

        #endregion

        #region Implementation of IDiscreteDistribution

        /// <summary>
        /// 최빈값 (빈도수가 최대인 값)
        /// </summary>
        public int Mode {
            get { return _p > 0.5 ? 1 : 0; }
        }

        /// <summary>
        /// 중앙값
        /// </summary>
        public int Median {
            get { throw new NotSupportedException("Bernoulli 분포에서는 Median이 정의되어 있지 않습니다."); }
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
            get { return 1; }
        }

        /// <summary>
        /// 분포의 확률
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public double Probability(int k) {
            if(k == 0)
                return 1.0 - _p;

            if(k == 1)
                return _p;

            return 0.0;
        }

        /// <summary>
        /// 분포의 Log 확률
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public double ProbabilityLn(int k) {
            if(k == 0)
                return Math.Log(1.0 - _p);

            return k == 1 ? Math.Log(_p) : Double.NegativeInfinity;
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

        //! ===================================

        private static void AssertParameters(double p) {
            Guard.Assert(p >= 0.0 && p <= 1.0, "p 는 [0, 1] 사이의 값이여야 합니다.");
        }

        /// <summary>
        /// Generates a sample sequence from the Bernoulli distribution.
        /// </summary>
        /// <param name="rnd">The random source to use.</param>
        /// <param name="p">The probability of generating a one.</param>
        /// <returns>A random sample from the Bernoulli distribution.</returns>
        private IEnumerable<int> DoSamples(Random rnd, double p) {
            while(true)
                yield return (rnd.NextDouble() < p) ? 1 : 0;
        }

        /// <summary>
        /// Samples a Bernoulli distributed random variable.
        /// </summary>
        /// <param name="rnd">The random number generator to use.</param>
        /// <param name="p">The probability of generating a 1.</param>
        /// <returns>A sample from the Bernoulli distribution.</returns>
        public int Sample(Random rnd, double p) {
            return Samples(rnd, p).First();
        }

        /// <summary>
        /// Samples a sequence of Bernoulli distributed random variables.
        /// </summary>
        /// <param name="rnd">The random number generator to use.</param>
        /// <param name="p">The probability of generating a 1.</param>
        /// <returns>a sequence of samples from the distribution.</returns>
        public IEnumerable<int> Samples(Random rnd, double p) {
            rnd.ShouldNotBeNull("rnd");
            AssertParameters(p);

            return DoSamples(rnd, p);
        }
    }
}