using System;
using System.Collections.Generic;

namespace NSoft.NFramework.Numerics.Distributions.Continuous {
    /// <summary>
    /// Weibull 분포. 자세한 내용은 <a href="http://en.wikipedia.org/wiki/Weibull_distribution">Wikipedia - Weibull distribution</a>을 참고하세요.
    /// </summary>
    /// <remarks>
    /// <para>The Weibull distribution is parametrized by a shape and scale parameter.</para>
    /// <para>The distribution will use the <see cref="System.Random"/> by default. 
    /// Users can get/set the random number generator by using the <see cref="RandomSource"/> property.</para>
    /// <para>The statistics classes will check all the incoming parameters whether they are in the allowed
    /// range. This might involve heavy computation. Optionally, by setting Control.CheckDistributionParameters
    /// to <c>false</c>, all parameter checks can be turned off.</para></remarks>
    [Serializable]
    public sealed class Weibull : IContinuousDistribution {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// Weibull shape parameter.
        /// </summary>
        private double _shape;

        /// <summary>
        /// Weibull inverse scale parameter.
        /// </summary>
        private double _scale;

        /// <summary>
        /// Reusable intermediate result 1 / (<see cref="_scale"/> ^ <see cref="_shape"/>)
        /// </summary>
        /// <remarks>
        /// By caching this parameter we can get slightly better numerics precision
        /// in certain constellations without any additional computations.
        /// </remarks>
        private double _scalePowShapeInv;

        private Random _random;

        /// <summary>
        /// Initializes a new instance of the Weibull class.
        /// </summary>
        /// <param name="shape">The shape of the Weibull distribution.</param>
        /// <param name="scale">The scale of the Weibull distribution.</param>
        /// <param name="randomFactory">난수발생기 Factory</param>
        public Weibull(double shape, double scale, Func<Random> randomFactory = null) {
            randomFactory.ShouldNotBeNull("randomFactory");

            if(IsDebugEnabled)
                log.Debug(@"Weibull 분포를 표현하는 인스턴스를 생성했습니다. shape=[{0}], scale=[{1}]", shape, scale);

            SetParameters(shape, scale);
            _random = (randomFactory != null) ? randomFactory() : MathTool.GetRandomFactory().Invoke();
        }

        /// <summary>
        /// Weibull 분포의 factor를 설정합니다.
        /// </summary>
        /// <param name="shape">The shape of the Weibull distribution.</param>
        /// <param name="scale">The scale of the Weibull distribution.</param>
        private void SetParameters(double shape, double scale) {
            AssertValidParameters(shape, scale);

            _shape = shape;
            _scale = scale;
            _scalePowShapeInv = Math.Pow(scale, -shape);
        }

        /// <summary>
        /// The shape of the Weibull distribution.
        /// </summary>
        public double Shape {
            get { return _shape; }
            set { SetParameters(value, _scale); }
        }

        /// <summary>
        /// The scale of the Weibull distribution.
        /// </summary>
        public double Scale {
            get { return _scale; }
            set { SetParameters(_shape, value); }
        }

        public override string ToString() {
            return string.Format(@"Weibull# Shape=[{0}], Scale=[{1}]", _shape, _scale);
        }

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
            get { return _scale * SpecialFunctions.Gamma(1.0 + (1.0 / _shape)); }
        }

        /// <summary>
        /// 분산
        /// </summary>
        public double Variance {
            get { return (_scale * _scale * SpecialFunctions.Gamma(1.0 + (2.0 / _shape))) - (Mean * Mean); }
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
            get { return (MathTool.EulerMascheroni * (1.0 - (1.0 / _shape))) + Math.Log(_scale / _shape) + 1.0; }
        }

        /// <summary>
        /// 기울기
        /// </summary>
        public double Skewness {
            get {
                double mu = Mean;
                double sigma = StDev;
                double sigma2 = sigma * sigma;
                double sigma3 = sigma2 * sigma;
                return ((_scale * _scale * _scale * SpecialFunctions.Gamma(1.0 + (3.0 / _shape))) - (3.0 * sigma2 * mu) - (mu * mu * mu)) /
                       sigma3;
            }
        }

        /// <summary>
        /// 첨도 (뽀족한 정도) (+) 값이면 뾰족하고, (-) 값이면 뭉툭하다
        /// </summary>
        public double Kurtosis {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// 최빈값 (빈도수가 최대인 값)
        /// </summary>
        public double Mode {
            get { return (_shape <= 1.0) ? 0.0 : _scale * Math.Pow((_shape - 1.0) / _shape, 1.0 / _shape); }
        }

        /// <summary>
        /// 중앙값
        /// </summary>
        public double Median {
            get { return _scale * Math.Pow(MathTool.Ln2, 1.0 / _shape); }
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
                if(Math.Abs(x - 0.0) < double.Epsilon && Math.Abs(_shape - 1.0) < double.Epsilon) {
                    return _shape / _scale;
                }

                return _shape * Math.Pow(x / _scale, _shape - 1.0) * Math.Exp(-Math.Pow(x, _shape) * _scalePowShapeInv) / _scale;
            }

            return 0.0;
        }

        /// <summary>
        /// 분포의 로그 확률 밀도 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double DensityLn(double x) {
            if(x >= 0.0) {
                if(Math.Abs(x - 0.0) < double.Epsilon && Math.Abs(_shape - 1.0) < double.Epsilon) {
                    return Math.Log(_shape) - Math.Log(_scale);
                }

                return Math.Log(_shape) + ((_shape - 1.0) * Math.Log(x / _scale)) - (Math.Pow(x, _shape) * _scalePowShapeInv) -
                       Math.Log(_scale);
            }

            return double.NegativeInfinity;
        }

        /// <summary>
        /// 확률 분포 계산을 위한 누적분포함수
        /// </summary>
        /// <param name="x">The location at which to compute the cumulative distribution function.</param>
        /// <returns>the cumulative distribution at location <paramref name="x"/>.</returns>
        public double CumulativeDistribution(double x) {
            return (x < 0.0) ? 0.0 : -SpecialFunctions.ExponentialMinusOne(-Math.Pow(x, _shape) * _scalePowShapeInv);
        }

        /// <summary>
        /// 분포의 무작위 값을 제공합니다.
        /// </summary>
        /// <returns></returns>
        public double Sample() {
            return SampleWeibull(RandomSource, _shape, _scale);
        }

        /// <summary>
        /// 현 분포의 무작위 값을 열거합니다.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<double> Samples() {
            return SampleWeibulls(RandomSource, _shape, _scale);
        }

        //! ==================================================

        /// <summary>
        /// Weibull 분포의 인자 값을 검증합니다.
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="scale"></param>
        public static void AssertValidParameters(double shape, double scale) {
            shape.ShouldBePositive("shape");
            scale.ShouldBePositive("scale");
        }

        /// <summary>
        /// Generates a sample from the Weibull distribution.
        /// </summary>
        /// <param name="rnd">The random number generator to use.</param>
        /// <param name="shape">The shape of the Weibull distribution from which to generate samples.</param>
        /// <param name="scale">The scale of the Weibull distribution from which to generate samples.</param>
        /// <returns>a sample from the distribution.</returns>
        public static double Sample(Random rnd, double shape, double scale) {
            AssertValidParameters(shape, scale);

            return SampleWeibull(rnd, shape, scale);
        }

        /// <summary>
        /// Generates a sequence of samples from the Weibull distribution.
        /// </summary>
        /// <param name="rnd">The random number generator to use.</param>
        /// <param name="shape">The shape of the Weibull distribution from which to generate samples.</param>
        /// <param name="scale">The scale of the Weibull distribution from which to generate samples.</param>
        /// <returns>a sequence of samples from the distribution.</returns>
        public static IEnumerable<double> Samples(Random rnd, double shape, double scale) {
            AssertValidParameters(shape, scale);

            return SampleWeibulls(rnd, shape, scale);
        }

        /// <summary>
        /// Weibull 분포의 샘플 데이타를 제공합니다.
        /// </summary>
        /// <param name="rnd">The random number generator to use.</param>
        /// <param name="shape">The shape of the Weibull distribution.</param>
        /// <param name="scale">The scale of the Weibull distribution.</param>
        /// <returns>A sample from a Weibull distributed random variable.</returns>
        internal static double SampleWeibull(Random rnd, double shape, double scale) {
            var x = rnd.NextDouble();
            return scale * Math.Pow(-Math.Log(x), 1.0 / shape);
        }

        /// <summary>
        /// Weibull 분포의 샘플 데이타 시퀀스를 제공합니다.
        /// </summary>
        /// <param name="rnd">The random number generator to use.</param>
        /// <param name="shape">The shape of the Weibull distribution.</param>
        /// <param name="scale">The scale of the Weibull distribution.</param>
        /// <returns>A sample from a Weibull distributed random variable.</returns>
        internal static IEnumerable<double> SampleWeibulls(Random rnd, double shape, double scale) {
            while(true) {
                var x = rnd.NextDouble();
                yield return scale * Math.Pow(-Math.Log(x), 1.0 / shape);
            }
        }
    }
}