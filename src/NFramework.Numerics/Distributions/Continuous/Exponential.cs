using System;
using System.Collections.Generic;

namespace NSoft.NFramework.Numerics.Distributions.Continuous {
    /// <summary>
    /// 지수 분포
    /// The exponential distribution is a distribution over the real numbers parameterized by one non-negative parameter.
    /// <a href="http://en.wikipedia.org/wiki/exponential_distribution">Wikipedia - exponential distribution</a>.
    /// </summary>
    /// <remarks>The distribution will use the <see cref="System.Random"/> by default. 
    /// <para>Users can set the random number generator by using the <see cref="RandomSource"/> property.</para>
    /// <para>The statistics classes will check all the incoming parameters whether they are in the allowed
    /// range. This might involve heavy computation. Optionally, by setting Control.CheckDistributionParameters
    /// to <c>false</c>, all parameter checks can be turned off.</para></remarks>
    public sealed class Exponential : IContinuousDistribution {
        private double _lambda;
        private Random _random;

        public Exponential() : this(1.0) {}

        public Exponential(double lambda = 1.0, Func<Random> randomFactory = null) {
            SetParameters(lambda);
            _random = (randomFactory != null) ? randomFactory() : MathTool.GetRandomFactory().Invoke();
        }

        private void SetParameters(double lambda) {
            AssertParameters(lambda);
            _lambda = lambda;
        }

        /// <summary>
        /// 람다 값 (기본 1.0)
        /// </summary>
        public double Lambda {
            get { return _lambda; }
            set { SetParameters(value); }
        }

        public override string ToString() {
            return string.Format("Exponential(Lambda=[{0}])", _lambda);
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
            get { return 1.0 / _lambda; }
        }

        /// <summary>
        /// 분산
        /// </summary>
        public double Variance {
            get { return 1.0 / (_lambda * _lambda); }
        }

        /// <summary>
        /// 표준편차
        /// </summary>
        public double StDev {
            get { return 1.0 / _lambda; }
        }

        /// <summary>
        /// 분포의 엔트로피 (불안정성) 
        /// </summary>
        public double Entropy {
            get { return 1.0 - Math.Log(_lambda); }
        }

        /// <summary>
        /// 기울기
        /// </summary>
        public double Skewness {
            get { return 2.0; }
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
            if(x >= 0.0) {
                return 1.0 - Math.Exp(-_lambda * x);
            }

            return 0.0;
        }

        #endregion

        #region Implementation of IContinuousDistribution

        /// <summary>
        /// 최빈값 (빈도수가 최대인 값)
        /// </summary>
        public double Mode {
            get { return 0.0; }
        }

        /// <summary>
        /// 중앙값
        /// </summary>
        public double Median {
            get { return Math.Log(2.0) / _lambda; }
        }

        /// <summary>
        /// 최소값
        /// </summary>
        public double Minumum {
            get { return 0.0; }
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
            if(x >= 0.0) {
                return _lambda * Math.Exp(-_lambda * x);
            }

            return 0.0;
        }

        /// <summary>
        /// 분포의 로그 확률 밀도 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double DensityLn(double x) {
            return Math.Log(_lambda) - (_lambda * x);
        }

        /// <summary>
        /// 분포의 무작위 값을 제공합니다.
        /// </summary>
        /// <returns></returns>
        public double Sample() {
            return DoSample(_random, _lambda);
        }

        /// <summary>
        /// 현 분포의 무작위 값을 열거합니다.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<double> Samples() {
            while(true)
                yield return DoSample(_random, _lambda);
        }

        #endregion

        //! =========================================================

        private static void AssertParameters(double lambda) {
            lambda.ShouldBePositive("lambda");
        }

        private static double DoSample(Random rnd, double lambda) {
            var r = rnd.NextDouble();

            while(r.ApproximateEqual(0.0))
                r = rnd.NextDouble();

            return -Math.Log(r) / lambda;
        }

        /// <summary>
        /// 지수 분포의 샘플을 제공합니다.
        /// </summary>
        /// <param name="rnd"></param>
        /// <param name="lambda"></param>
        /// <returns></returns>
        public static double Sample(Random rnd, double lambda) {
            rnd.ShouldNotBeNull("rnd");
            AssertParameters(lambda);

            return DoSample(rnd, lambda);
        }

        /// <summary>
        /// 지수 분포의 샘플 시퀀스를 제공합니다.
        /// </summary>
        /// <param name="rnd"></param>
        /// <param name="lambda"></param>
        /// <returns></returns>
        public static IEnumerable<double> Samples(Random rnd, double lambda) {
            rnd.ShouldNotBeNull("rnd");
            AssertParameters(lambda);

            while(true)
                yield return DoSample(rnd, lambda);
        }
    }
}