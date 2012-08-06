using System;
using System.Collections.Generic;

namespace NSoft.NFramework.Numerics.Distributions.Continuous {
    /// <summary>
    /// The inverse Gamma distribution is a distribution over the positive real numbers parameterized by
    /// two positive parameters.
    /// <a href="http://en.wikipedia.org/wiki/inverse-gamma_distribution">Wikipedia - InverseGamma distribution</a>.
    /// </summary>
    /// <remarks><para>The distribution will use the <see cref="System.Random"/> by default. 
    /// Users can set the random number generator by using the <see cref="RandomSource"/> property.</para>
    /// <para>The statistics classes will check all the incoming parameters whether they are in the allowed
    /// range. This might involve heavy computation. Optionally, by setting Control.CheckDistributionParameters
    /// to <c>false</c>, all parameter checks can be turned off.</para></remarks>
    public sealed class InverseGamma : IContinuousDistribution {
        private double _shape;
        private double _scale;
        private Random _random;

        public InverseGamma(double shape, double scale, Func<Random> randomFactory = null) {
            SetParameters(shape, scale);
            RandomSource = (randomFactory != null) ? randomFactory() : MathTool.GetRandomFactory()();
        }

        private void SetParameters(double shape, double scale) {
            AssertParameters(shape, scale);
            _shape = shape;
            _scale = scale;
        }

        /// <summary>
        /// the shape (alpha) parameter.
        /// </summary>
        public double Shape {
            get { return _shape; }
            set { SetParameters(value, _scale); }
        }

        /// <summary>
        /// The scale (beta) parameter.
        /// </summary>
        public double Scale {
            get { return _scale; }
            set { SetParameters(_shape, value); }
        }

        public override string ToString() {
            return string.Format("InverseGamma(Shape=[{0}], Scale=[{1}])", _shape, _scale);
        }

        #region << IDistribution >>

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
                if(_shape <= 1)
                    throw new NotSupportedException("_shape이 1 이하라면 Mean 값을 계산할 수 없습니다.");

                return _scale / (_shape - 1.0);
            }
        }

        /// <summary>
        /// 분산
        /// </summary>
        public double Variance {
            get {
                if(_shape <= 2)
                    throw new NotSupportedException("_shape이 2 이하라면 Variance 값을 계산할 수 없습니다.");

                return _scale * _scale / ((_shape - 1.0) * (_shape - 1.0) * (_shape - 2.0));
            }
        }

        /// <summary>
        /// 표준편차
        /// </summary>
        public double StDev {
            get { return _scale / (Math.Abs(_shape - 1.0) * Math.Sqrt(_shape - 2.0)); }
        }

        /// <summary>
        /// 분포의 엔트로피 (불안정성) 
        /// </summary>
        public double Entropy {
            get { return _shape + Math.Log(_scale) + SpecialFunctions.GammaLn(_shape) - ((1 + _shape) * SpecialFunctions.DiGamma(_shape)); }
        }

        /// <summary>
        /// 기울기
        /// </summary>
        public double Skewness {
            get {
                if(_shape <= 3)
                    throw new NotSupportedException("_shape이 3 이하라면 Variance 값을 계산할 수 없습니다.");

                return (4 * Math.Sqrt(_shape - 2.0)) / (_shape - 3.0);
            }
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
            return SpecialFunctions.GammaUpperRegularized(_shape, _scale / x);
        }

        #endregion

        #region << IContinuousDistribution >>

        /// <summary>
        /// 최빈값 (빈도수가 최대인 값)
        /// </summary>
        public double Mode {
            get { return _scale / (_shape + 1.0); }
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
            if(x >= 0.0) {
                return Math.Pow(_scale, _shape) * Math.Pow(x, -_shape - 1.0) * Math.Exp(-_scale / x) / SpecialFunctions.Gamma(_shape);
            }

            return 0.0;
        }

        /// <summary>
        /// 분포의 로그 확률 밀도 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double DensityLn(double x) {
            return Math.Log(Density(x));
        }

        /// <summary>
        /// 분포의 무작위 값을 제공합니다.
        /// </summary>
        /// <returns></returns>
        public double Sample() {
            return DoSample(RandomSource, _shape, _scale);
        }

        /// <summary>
        /// 현 분포의 무작위 값을 열거합니다.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<double> Samples() {
            while(true)
                yield return DoSample(RandomSource, _shape, _scale);
        }

        #endregion

        //! =====================================================

        private static void AssertParameters(double shape, double scale) {
            shape.ShouldBePositive("shape");
            scale.ShouldBePositive("scale");
        }

        private static double DoSample(Random rnd, double shape, double scale) {
            return 1.0 / Gamma.Sample(rnd, shape, scale);
        }

        public static double Sample(Random rnd, double shape, double scale) {
            rnd.ShouldNotBeNull("rnd");
            AssertParameters(shape, scale);

            return DoSample(rnd, shape, scale);
        }

        public static IEnumerable<double> Samples(Random rnd, double shape, double scale) {
            rnd.ShouldNotBeNull("rnd");
            AssertParameters(shape, scale);

            while(true)
                yield return DoSample(rnd, shape, scale);
        }
    }
}