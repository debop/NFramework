using System;
using System.Collections.Generic;
using System.Linq;

namespace NSoft.NFramework.Numerics.Distributions.Continuous {
    /// <summary>
    /// A random variable is said to be stable (or to have a stable distribution) if it has 
    /// the property that a linear combination of two independent copies of the variable has 
    /// the same distribution, up to location and scale parameters.
    /// For details about this distribution, see 
    /// <a href="http://en.wikipedia.org/wiki/Stable_distribution">Wikipedia - Stable distribution</a>.
    /// </summary>
    /// <remarks><para>The distribution will use the <see cref="System.Random"/> by default.`
    /// Users can get/set the random number generator by using the <see cref="RandomSource"/> property.</para>
    /// <para>The statistics classes will check all the incoming parameters whether they are in the allowed
    /// range. This might involve heavy computation. Optionally, by setting Control.CheckDistributionParameters
    /// to <c>false</c>, all parameter checks can be turned off.</para></remarks>
    public sealed class Stable : IContinuousDistribution {
        private double _alpha;
        private double _beta;
        private double _scale;
        private double _location;

        private Random _random;

        public Stable(double alpha, double beta, double scale, double location, Func<Random> randomFactory = null) {
            SetParameters(alpha, beta, scale, location);

            if(randomFactory != null)
                _random = randomFactory();
        }

        private void SetParameters(double alpha, double beta, double scale, double location) {
            AssertParameters(alpha, beta, scale, location);

            _alpha = alpha;
            _beta = beta;
            _scale = scale;
            _location = location;
        }

        /// <summary>
        /// Stable parameter
        /// </summary>
        public double Alpha {
            get { return _alpha; }
            set { SetParameters(value, _beta, _scale, _location); }
        }

        /// <summary>
        /// Skewness parameter
        /// </summary>
        public double Beta {
            get { return _beta; }
            set { SetParameters(_alpha, value, _scale, _location); }
        }

        public double Scale {
            get { return _scale; }
            set { SetParameters(_alpha, _beta, value, _location); }
        }

        public double Location {
            get { return _location; }
            set { SetParameters(_alpha, _beta, _scale, value); }
        }

        public override string ToString() {
            return string.Format("Stable(Stability=[{0}], Skewness=[{1}], Scale=[{2}], Location=[{3}])", Alpha, Beta, Scale, Location);
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
            get {
                Guard.Assert<NotSupportedException>(_alpha > 1.0, "Alpha 값이 1.0 초과이어야 Mean 값을 제공할 수 있습니다.");
                return _location;
            }
        }

        /// <summary>
        /// 분산
        /// </summary>
        public double Variance {
            get {
                return (_alpha.ApproximateEqual(2.0))
                           ? 2.0 * _scale.Square()
                           : double.PositiveInfinity;
            }
        }

        /// <summary>
        /// 표준편차
        /// </summary>
        public double StDev {
            get {
                return (_alpha.ApproximateEqual(2.0))
                           ? Math.Sqrt(2.0) * _scale
                           : double.PositiveInfinity;
            }
        }

        /// <summary>
        /// 분포의 엔트로피 (불안정성) 
        /// </summary>
        public double Entropy {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// 기울기
        /// </summary>
        public double Skewness {
            get {
                Guard.Assert(Alpha.ApproximateEqual(2.0), "Alpha 가 2.0이 아니면 Skewness 값을 제공할 수 없습니다.");
                return 0.0;
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
            if(Alpha.ApproximateEqual(2.0))
                return (new Normal(_location, StDev)).CumulativeDistribution(x);

            if(Alpha.ApproximateEqual(1.0) && Beta.ApproximateEqual(0.0))
                return (new Cauchy(_location, _scale)).CumulativeDistribution(x);

            if(Alpha.ApproximateEqual(0.5) && Beta.ApproximateEqual(1.0))
                return LevyCumulativeDistribution(_scale, _location, x);

            throw new NotSupportedException();
        }

        /// <summary>
        /// Computes the cumulative distribution function of the Levy distribution.
        /// </summary>
        /// <param name="scale">The scale parameter.</param>
        /// <param name="location">The location parameter.</param>
        /// <param name="x">The location at which to compute the cumulative density.</param>
        /// <returns>
        /// the cumulative density at <paramref name="x"/>.
        /// </returns>
        private static double LevyCumulativeDistribution(double scale, double location, double x) {
            // The parameters scale and location must be correct
            return SpecialFunctions.Erfc(Math.Sqrt(scale / (2 * (x - location))));
        }

        #endregion

        #region Implementation of IContinuousDistribution

        /// <summary>
        /// 최빈값 (빈도수가 최대인 값)
        /// </summary>
        public double Mode {
            get {
                Guard.Assert<NotSupportedException>(Beta.ApproximateEqual(0.0), "Beta가 0.0이 아니면 Mode 값은 지원하지 않습니다.");
                return _location;
            }
        }

        /// <summary>
        /// 중앙값
        /// </summary>
        public double Median {
            get {
                Guard.Assert<NotSupportedException>(Beta.ApproximateEqual(0.0), "Beta가 0.0이 아니면 Median 값은 지원하지 않습니다.");
                return _location;
            }
        }

        /// <summary>
        /// 최소값
        /// </summary>
        public double Minumum {
            get { return Math.Abs(Beta).ApproximateEqual(1.0) ? 0.0 : Double.NegativeInfinity; }
        }

        /// <summary>
        /// 최대값
        /// </summary>
        public double Maximum {
            get { return Double.PositiveInfinity; }
        }

        /// <summary>
        /// 분포의 확률 밀도
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double Density(double x) {
            if(Alpha.ApproximateEqual(2.0)) {
                return (new Normal(_location, StDev)).Density(x);
            }

            if(Alpha.ApproximateEqual(1.0) && Beta.ApproximateEqual(0.0)) {
                return (new Cauchy(_location, _scale)).Density(x);
            }

            if(Alpha.ApproximateEqual(0.5) && Beta.ApproximateEqual(1.0)) {
                return LevyDensity(_scale, _location, x);
            }

            throw new NotSupportedException();
        }

        /// <summary>
        /// Computes the density of the Levy distribution.
        /// </summary>
        /// <param name="scale">The scale parameter of the distribution.</param>
        /// <param name="location">The location parameter of the distribution.</param>
        /// <param name="x">The location at which to compute the density.</param>
        /// <returns>the density at <paramref name="x"/>.</returns>
        private static double LevyDensity(double scale, double location, double x) {
            // The parameters scale and location must be correct
            if(x < location) {
                throw new NotSupportedException();
            }

            return (Math.Sqrt(scale / MathTool.Pi2) * Math.Exp(-scale / (2 * (x - location)))) / Math.Pow(x - location, 1.5);
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
            return DoSamples(RandomSource, Alpha, Beta).First();
        }

        /// <summary>
        /// 현 분포의 무작위 값을 열거합니다.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<double> Samples() {
            return DoSamples(RandomSource, Alpha, Beta);
        }

        #endregion

        //! =====================================================

        private static void AssertParameters(double alpha, double beta, double scale, double location) {
            Guard.Assert(alpha > 0.0 && alpha <= 2.0, "Alpha[{0}] 는 (0, 2] 구간의 값을 가져야 합니다.", alpha);
            Guard.Assert(beta >= -1.0 && beta <= 1.0, "Beta[{0}] 는 [-1.0, 1.0] 구간의 값을 가져야 합니다.", beta);
            scale.ShouldBePositive("scale");
            location.ShouldNotBeNaN("location");
        }

        private static IEnumerable<double> DoSamples(Random rnd, double alpha, double beta) {
            while(true) {
                var randTheta = ContinuousUniform.Sample(rnd, -MathTool.PiOver2, MathTool.PiOver2);
                var randW = Exponential.Sample(rnd, 1.0);

                if(!1.0.AlmostEqual(alpha)) {
                    var theta = (1.0 / alpha) * Math.Atan(beta * Math.Tan(MathTool.PiOver2 * alpha));
                    var angle = alpha * (randTheta + theta);
                    var part1 = beta * Math.Tan(MathTool.PiOver2 * alpha);

                    var factor = Math.Pow(1.0 + (part1 * part1), 1.0 / (2.0 * alpha));
                    var factor1 = Math.Sin(angle) / Math.Pow(Math.Cos(randTheta), (1.0 / alpha));
                    var factor2 = Math.Pow(Math.Cos(randTheta - angle) / randW, (1 - alpha) / alpha);

                    yield return factor * factor1 * factor2;
                }
                else {
                    var part1 = MathTool.PiOver2 + (beta * randTheta);
                    var summand = part1 * Math.Tan(randTheta);
                    var subtrahend = beta * Math.Log(MathTool.PiOver2 * randW * Math.Cos(randTheta) / part1);

                    yield return (2.0 / Math.PI) * (summand - subtrahend);
                }
            }
        }

        public static double Sample(Random rnd, double alpha, double beta) {
            return Samples(rnd, alpha, beta).First();
        }

        public static IEnumerable<double> Samples(Random rnd, double alpha, double beta) {
            rnd.ShouldNotBeNull("rnd");
            AssertParameters(alpha, beta, 1, 0);

            return DoSamples(rnd, alpha, beta);
        }
    }
}