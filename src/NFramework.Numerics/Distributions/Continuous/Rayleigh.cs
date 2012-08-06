using System;
using System.Collections.Generic;
using System.Linq;

namespace NSoft.NFramework.Numerics.Distributions.Continuous {
    /// <summary>
    /// The Rayleigh distribution (pronounced /ˈreɪli/) is a continuous probability distribution. As an 
    /// example of how it arises, the wind speed will have a Rayleigh distribution if the components of 
    /// the two-dimensional wind velocity vector are uncorrelated and normally distributed with equal variance.
    /// For details about this distribution, see 
    /// <a href="http://en.wikipedia.org/wiki/Rayleigh_distribution">Wikipedia - Rayleigh distribution</a>.
    /// </summary>
    /// <remarks><para>The distribution will use the <see cref="System.Random"/> by default. 
    /// Users can get/set the random number generator by using the <see cref="RandomSource"/> property.</para>
    /// <para>The statistics classes will check all the incoming parameters whether they are in the allowed
    /// range. This might involve heavy computation. Optionally, by setting Control.CheckDistributionParameters
    /// to <c>false</c>, all parameter checks can be turned off.</para></remarks>
    public sealed class Rayleigh : IContinuousDistribution {
        private double _scale;
        private Random _random;

        public Rayleigh(double scale, Func<Random> randomFactory) {
            SetParameters(scale);
            randomFactory.ShouldNotBeNull("randomFactory");
            _random = randomFactory();
        }

        private void SetParameters(double scale) {
            AssertParameters(scale);
            _scale = scale;
        }

        public double Scale {
            get { return _scale; }
            set { SetParameters(value); }
        }

        public override string ToString() {
            return string.Format("Rayleigh(Scale=[{0}])", _scale);
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
            get { return _scale * Math.Sqrt(MathTool.PiOver2); }
        }

        /// <summary>
        /// 분산
        /// </summary>
        public double Variance {
            get { return (2.0 - MathTool.PiOver2) * _scale * _scale; }
        }

        /// <summary>
        /// 표준편차
        /// </summary>
        public double StDev {
            get { return Math.Sqrt(2.0 - MathTool.PiOver2) * _scale; }
        }

        /// <summary>
        /// 분포의 엔트로피 (불안정성) 
        /// </summary>
        public double Entropy {
            get { return 1.0 + Math.Log(_scale / Math.Sqrt(2)) + (MathTool.EulerMascheroni / 2.0); }
        }

        /// <summary>
        /// 기울기
        /// </summary>
        public double Skewness {
            get { return (2.0 * Math.Sqrt(MathTool.Pi) * (MathTool.Pi - 3.0)) / Math.Pow(4.0 - MathTool.Pi, 1.5); }
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
            return 1.0 - Math.Exp(-x * x / (2.0 * _scale * _scale));
        }

        #endregion

        #region Implementation of IContinuousDistribution

        /// <summary>
        /// 최빈값 (빈도수가 최대인 값)
        /// </summary>
        public double Mode {
            get { return _scale; }
        }

        /// <summary>
        /// 중앙값
        /// </summary>
        public double Median {
            get { return _scale * Math.Sqrt(Math.Log(4.0)); }
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
            return (x / (_scale * _scale)) * Math.Exp(-x * x / (2.0 * _scale * _scale));
        }

        /// <summary>
        /// 분포의 로그 확률 밀도 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double DensityLn(double x) {
            return Math.Log(x / (_scale * _scale)) - (x * x / (2.0 * _scale * _scale));
        }

        /// <summary>
        /// 분포의 무작위 값을 제공합니다.
        /// </summary>
        /// <returns></returns>
        public double Sample() {
            return DoSamples(RandomSource, Scale).First();
        }

        /// <summary>
        /// 현 분포의 무작위 값을 열거합니다.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<double> Samples() {
            return DoSamples(RandomSource, Scale);
        }

        #endregion

        //! =====================================================

        private static void AssertParameters(double scale) {
            scale.ShouldBePositive("scale");
        }

        private static IEnumerable<double> DoSamples(Random rnd, double scale) {
            while(true)
                yield return scale * Math.Sqrt(-2.0 * Math.Log(rnd.NextDouble()));
        }

        public static double Sample(Random rnd, double scale) {
            rnd.ShouldNotBeNull("rnd");
            AssertParameters(scale);

            return DoSamples(rnd, scale).First();
        }

        public static IEnumerable<double> Samples(Random rnd, double scale) {
            rnd.ShouldNotBeNull("rnd");
            AssertParameters(scale);

            return DoSamples(rnd, scale);
        }
    }
}