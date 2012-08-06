using System;
using System.Collections.Generic;

namespace NSoft.NFramework.Numerics.Distributions.Continuous {
    /// <summary>
    /// The Pareto distribution is a power law probability distribution that coincides with social, 
    /// scientific, geophysical, actuarial, and many other types of observable phenomena.
    /// For details about this distribution, see 
    /// <a href="http://en.wikipedia.org/wiki/Pareto_distribution">Wikipedia - Pareto distribution</a>.
    /// </summary>
    /// <remarks><para>The distribution will use the <see cref="System.Random"/> by default. 
    /// Users can get/set the random number generator by using the <see cref="RandomSource"/> property.</para>
    /// <para>The statistics classes will check all the incoming parameters whether they are in the allowed
    /// range. This might involve heavy computation. Optionally, by setting Control.CheckDistributionParameters
    /// to <c>false</c>, all parameter checks can be turned off.</para></remarks>
    public sealed class Pareto : IContinuousDistribution {
        private double _scale;
        private double _shape;
        private Random _random;

        public Pareto(double scale, double shape, Func<Random> randomFactory = null) {
            SetParameters(scale, shape);

            randomFactory.ShouldNotBeNull("randomFactory");
            _random = randomFactory();
        }

        private void SetParameters(double scale, double shape) {
            AssertParameters(scale, shape);

            _scale = scale;
            _shape = shape;
        }

        public double Scale {
            get { return _scale; }
            set { SetParameters(value, _shape); }
        }

        public double Shape {
            get { return _shape; }
            set { SetParameters(_scale, value); }
        }

        public override string ToString() {
            return string.Format("Pareto(Scale=[{0}], Shape=[{1}])", _scale, _shape);
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
            get {
                Guard.Assert<NotSupportedException>(_shape > 1.0, "Shape > 1.0 이어야 Mean 값을 계산할 수 있습니다.");
                return _shape * _scale / (_shape - 1.0);
            }
        }

        /// <summary>
        /// 분산
        /// </summary>
        public double Variance {
            get {
                Guard.Assert<NotSupportedException>(_shape > 2.0, "Shape > 2.0 이어야 Variance 값을 계산할 수 있습니다.");
                return _scale * _scale * _shape / ((_shape - 1.0) * (_shape - 1.0) * (_shape - 2.0));
            }
        }

        /// <summary>
        /// 표준편차
        /// </summary>
        public double StDev {
            get { return (_scale * Math.Sqrt(_shape)) / (Math.Abs(_shape - 1.0) * Math.Sqrt(_shape - 2.0)); }
        }

        /// <summary>
        /// 분포의 엔트로피 (불안정성) 
        /// </summary>
        public double Entropy {
            get { return Math.Log(_shape / _scale) - (1.0 / _shape) - 1.0; }
        }

        /// <summary>
        /// 기울기
        /// </summary>
        public double Skewness {
            get { return (2.0 * (_shape + 1.0) / (_shape - 3.0)) * Math.Sqrt((_shape - 2.0) / _shape); }
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
            return 1.0 - Math.Pow(_scale / x, _shape);
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
            get { return _scale * Math.Pow(2.0, 1.0 / _shape); }
        }

        /// <summary>
        /// 최소값
        /// </summary>
        public double Minumum {
            get { return _scale; }
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
            return _shape * Math.Pow(_scale, _shape) / Math.Pow(x, _shape + 1.0);
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
            return DoSample(_random, _scale, _shape);
        }

        /// <summary>
        /// 현 분포의 무작위 값을 열거합니다.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<double> Samples() {
            return DoSamples(_random, _scale, _shape);
        }

        #endregion

        //! ========================================================

        private static void AssertParameters(double scale, double shape) {
            scale.ShouldBePositive("scale");
            shape.ShouldBePositive("shape");
        }

        private static double DoSample(Random rnd, double scale, double shape) {
            return scale * Math.Pow(rnd.NextDouble(), -1.0 / shape);
        }

        private static IEnumerable<double> DoSamples(Random rnd, double scale, double shape) {
            while(true)
                yield return scale * Math.Pow(rnd.NextDouble(), -1.0 / shape);
        }

        public static double Sample(Random rnd, double scale, double shape) {
            rnd.ShouldNotBeNull("rnd");
            AssertParameters(scale, shape);
            return DoSample(rnd, scale, shape);
        }

        public static IEnumerable<double> Samples(Random rnd, double scale, double shape) {
            rnd.ShouldNotBeNull("rnd");
            AssertParameters(scale, shape);

            return DoSamples(rnd, scale, shape);
        }
    }
}