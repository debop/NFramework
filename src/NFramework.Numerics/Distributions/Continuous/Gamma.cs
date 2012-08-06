using System;
using System.Collections.Generic;

namespace NSoft.NFramework.Numerics.Distributions.Continuous {
    /// <summary>
    /// Implements the univariate Gamma distribution. For details about this distribution, see 
    /// <a href="http://en.wikipedia.org/wiki/Gamma_distribution">Wikipedia - Gamma distribution</a>.
    /// </summary>
    /// <remarks>
    /// <para>The Gamma distribution is parametrized by a shape and inverse scale parameter. When we want
    /// to specify a Gamma distribution which is a point distribution we set the shape parameter to be the
    /// location of the point distribution and the inverse scale as positive infinity. The distribution
    /// with shape and inverse scale both zero is undefined.</para>
    /// <para> Random number generation for the Gamma distribution is based on the algorithm in:
    /// "A Simple Method for Generating Gamma Variables" - Marsaglia &amp; Tsang
    /// ACM Transactions on Mathematical Software, Vol. 26, No. 3, September 2000, Pages 363?72.</para>
    /// <para>The distribution will use the <see cref="System.Random"/> by default. 
    /// Users can get/set the random number generator by using the <see cref="RandomSource"/> property.</para>
    /// <para>The statistics classes will check all the incoming parameters whether they are in the allowed
    /// range. This might involve heavy computation. Optionally, by setting Control.CheckDistributionParameters
    /// to <c>false</c>, all parameter checks can be turned off.</para></remarks>
    public class Gamma : IContinuousDistribution {
        /// <summary>
        /// Constructs a Gamma distribution from a shape and scale parameter. The distribution will
        /// be initialized with the default <seealso cref="System.Random"/> random number generator.
        /// </summary>
        /// <param name="shape">The shape of the Gamma distribution.</param>
        /// <param name="scale">The scale of the Gamma distribution.</param>
        /// <returns>a normal distribution.</returns>
        public static Gamma WithShapeScale(double shape, double scale) {
            return new Gamma(shape, 1.0 / scale);
        }

        /// <summary>
        /// Constructs a Gamma distribution from a shape and inverse scale parameter. The distribution will
        /// be initialized with the default <seealso cref="System.Random"/> random number generator.
        /// </summary>
        /// <param name="shape">The shape of the Gamma distribution.</param>
        /// <param name="invScale">The inverse scale of the Gamma distribution.</param>
        /// <returns>a normal distribution.</returns>
        public static Gamma WithShapeInvScale(double shape, double invScale) {
            return new Gamma(shape, invScale);
        }

        //! ==================================================

        private double _shape;
        private double _invScale;
        private Random _random;

        public Gamma(double shape, double invScale, Func<Random> randomFactory = null) {
            SetParameters(shape, invScale);
            _random = (randomFactory != null) ? randomFactory() : MathTool.GetRandomFactory().Invoke();
        }

        private void SetParameters(double shape, double invScale) {
            AssertParameters(shape, invScale);
            _shape = shape;
            _invScale = invScale;
        }

        /// <summary>
        /// Gamma 분포의 Shape
        /// </summary>
        public double Shape {
            get { return _shape; }
            set { SetParameters(value, _invScale); }
        }

        /// <summary>
        /// Gamma 분포의 Scale
        /// </summary>
        public double Scale {
            get { return 1.0 / InvScale; }
            set {
                var invScale = 1.0 / value;
                if(double.IsNegativeInfinity(invScale))
                    invScale = -invScale;

                SetParameters(_shape, invScale);
            }
        }

        /// <summary>
        /// 감마분포의 역 Scale
        /// </summary>
        public double InvScale {
            get { return _invScale; }
            set { SetParameters(_shape, value); }
        }

        public override string ToString() {
            return string.Format("Gamma# Shape=[{0}], Inverse Scale=[{1}]", _shape, _invScale);
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
            get {
                if(double.IsPositiveInfinity(_invScale))
                    return _shape;

                if(Math.Abs(_invScale - 0.0) < double.Epsilon && Math.Abs(_shape - 0.0) < double.Epsilon)
                    return double.NaN;

                return _shape / _invScale;
            }
        }

        /// <summary>
        /// 분산
        /// </summary>
        public double Variance {
            get {
                if(double.IsPositiveInfinity(_invScale))
                    return 0.0;

                if(Math.Abs(_invScale - 0.0) < double.Epsilon && Math.Abs(_shape - 0.0) < double.Epsilon)
                    return double.NaN;

                return _shape / (_invScale * _invScale);
            }
        }

        /// <summary>
        /// 표준편차
        /// </summary>
        public double StDev {
            get {
                if(double.IsPositiveInfinity(_invScale))
                    return 0.0;

                if(Math.Abs(_invScale - 0.0) < double.Epsilon && Math.Abs(_shape - 0.0) < double.Epsilon)
                    return double.NaN;

                return Math.Sqrt(_shape / (_invScale * _invScale));
            }
        }

        /// <summary>
        /// 분포의 엔트로피 (불안정성) 
        /// </summary>
        public double Entropy {
            get {
                if(Double.IsPositiveInfinity(_invScale))
                    return 0.0;


                if(Math.Abs(_invScale - 0.0) < double.Epsilon && Math.Abs(_shape - 0.0) < double.Epsilon)
                    return double.NaN;

                return _shape - Math.Log(_invScale) + SpecialFunctions.GammaLn(_shape) +
                       ((1.0 - _shape) * SpecialFunctions.DiGamma(_shape));
            }
        }

        /// <summary>
        /// 기울기
        /// </summary>
        public double Skewness {
            get {
                if(Double.IsPositiveInfinity(_invScale))
                    return 0.0;

                if(Math.Abs(_invScale - 0.0) < double.Epsilon && Math.Abs(_shape - 0.0) < double.Epsilon)
                    return Double.NaN;

                return 2.0 / Math.Sqrt(_shape);
            }
        }

        /// <summary>
        /// 첨도 (뽀족한 정도) (+) 값이면 뾰족하고, (-) 값이면 뭉툭하다
        /// </summary>
        public double Kurtosis {
            get { return 6.0 / _shape; }
        }

        /// <summary>
        /// 확률 분포 계산을 위한 누적분포함수
        /// </summary>
        /// <param name="x">The location at which to compute the cumulative distribution function.</param>
        /// <returns>the cumulative distribution at location <paramref name="x"/>.</returns>
        public double CumulativeDistribution(double x) {
            if(double.IsPositiveInfinity(_invScale))

                return x >= _shape ? 1.0 : 0.0;


            if(Math.Abs(_shape - 0.0) < double.Epsilon && Math.Abs(_invScale - 0.0) < double.Epsilon) {
                return 0.0;
            }

            return SpecialFunctions.GammaLowerRegularized(_shape, x * _invScale);
        }

        #endregion

        #region Implementation of IContinuousDistribution

        /// <summary>
        /// 최빈값 (빈도수가 최대인 값)
        /// </summary>
        public double Mode {
            get {
                if(Double.IsPositiveInfinity(_invScale)) {
                    return _shape;
                }

                if(Math.Abs(_invScale - 0.0) < double.Epsilon && Math.Abs(_shape - 0.0) < double.Epsilon) {
                    return Double.NaN;
                }

                return (_shape - 1.0) / _invScale;
            }
        }

        /// <summary>
        /// 중앙값
        /// </summary>
        public double Median {
            get { throw new NotSupportedException(); }
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
            return Density(_shape, _invScale, x);
        }

        /// <summary>
        /// 분포의 로그 확률 밀도 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double DensityLn(double x) {
            return DensityLn(_shape, _invScale, x);
        }

        /// <summary>
        /// 분포의 무작위 값을 제공합니다.
        /// </summary>
        /// <returns></returns>
        public double Sample() {
            return SampleGamma(RandomSource, Shape, InvScale);
        }

        /// <summary>
        /// 현 분포의 무작위 값을 열거합니다.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<double> Samples() {
            while(true)
                yield return SampleGamma(RandomSource, Shape, InvScale);
        }

        #endregion

        //! ==================================================

        private static void AssertParameters(double shape, double invScale) {
            shape.ShouldBePositiveOrZero("shape");
            invScale.ShouldBePositiveOrZero("invScale");
        }

        /// <summary>
        /// 감마분포의 확률밀도를 계산합니다.
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="invScale"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double Density(double shape, double invScale, double x) {
            if(Double.IsPositiveInfinity(invScale))
                return Math.Abs(x - shape) < double.Epsilon ? Double.PositiveInfinity : 0.0;

            if(Math.Abs(shape - 0.0) < double.Epsilon && Math.Abs(invScale - 0.0) < double.Epsilon)
                return 0.0;

            if(Math.Abs(shape - 1.0) < double.Epsilon)
                return invScale * Math.Exp(-invScale * x);

            return Math.Pow(invScale, shape) * Math.Pow(x, shape - 1.0) * Math.Exp(-invScale * x) / SpecialFunctions.Gamma(shape);
        }

        /// <summary>
        /// 감마분포의 로그 확률밀도를 계산합니다.
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="invScale"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double DensityLn(double shape, double invScale, double x) {
            if(double.IsPositiveInfinity(invScale))
                return Math.Abs(x - shape) < double.Epsilon ? Double.PositiveInfinity : Double.NegativeInfinity;

            if(Math.Abs(shape - 0.0) < double.Epsilon && Math.Abs(invScale - 0.0) < double.Epsilon)
                return Double.NegativeInfinity;

            if(Math.Abs(shape - 1.0) < double.Epsilon)
                return Math.Log(invScale) - (invScale * x);

            return (shape * Math.Log(invScale)) + ((shape - 1.0) * Math.Log(x)) - (invScale * x) - SpecialFunctions.GammaLn(shape);
        }

        /// <summary>
        /// 감마 분포의 샘플 데이타를 생성합니다.
        /// </summary>
        /// <param name="rnd"></param>
        /// <param name="shape"></param>
        /// <param name="invScale"></param>
        /// <returns></returns>
        public static double Sample(Random rnd, double shape, double invScale) {
            AssertParameters(shape, invScale);
            return SampleGamma(rnd, shape, invScale);
        }

        /// <summary>
        /// 감마 분포의 샘플 데이타를 생성합니다.
        /// </summary>
        /// <param name="rnd"></param>
        /// <param name="shape"></param>
        /// <param name="invScale"></param>
        /// <returns></returns>
        public static IEnumerable<double> Samples(Random rnd, double shape, double invScale) {
            AssertParameters(shape, invScale);
            while(true)
                yield return SampleGamma(rnd, shape, invScale);
        }

        /// <summary>
        /// <para>Sampling implementation based on:
        /// "A Simple Method for Generating Gamma Variables" - Marsaglia &amp; Tsang
        /// ACM Transactions on Mathematical Software, Vol. 26, No. 3, September 2000, Pages 363?72.</para>
        /// <para>This method performs no parameter checks.</para>
        /// </summary>
        /// <param name="rnd">The random number generator to use.</param>
        /// <param name="shape">The shape of the Gamma distribution.</param>
        /// <param name="invScale">The inverse scale of the Gamma distribution.</param>
        /// <returns>A sample from a Gamma distributed random variable.</returns>
        internal static double SampleGamma(Random rnd, double shape, double invScale) {
            if(Double.IsPositiveInfinity(invScale)) {
                return shape;
            }

            var a = shape;
            var alphafix = 1.0;

            // Fix when alpha is less than one.
            if(shape < 1.0) {
                a = shape + 1.0;
                alphafix = Math.Pow(rnd.NextDouble(), 1.0 / shape);
            }

            var d = a - (1.0 / 3.0);
            var c = 1.0 / Math.Sqrt(9.0 * d);
            while(true) {
                var x = Normal.Sample(rnd, 0.0, 1.0);
                var v = 1.0 + (c * x);
                while(v <= 0.0) {
                    x = Normal.Sample(rnd, 0.0, 1.0);
                    v = 1.0 + (c * x);
                }

                v = v * v * v;
                var u = rnd.NextDouble();
                x = x * x;
                if(u < 1.0 - (0.0331 * x * x)) {
                    return alphafix * d * v / invScale;
                }

                if(Math.Log(u) < (0.5 * x) + (d * (1.0 - v + Math.Log(v)))) {
                    return alphafix * d * v / invScale;
                }
            }
        }
    }
}