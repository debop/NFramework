using System;
using System.Collections.Generic;

namespace NSoft.NFramework.Numerics.Distributions.Continuous {
    /// <summary>
    /// Implements the Beta distribution. For details about this distribution, see 
    /// <a href="http://en.wikipedia.org/wiki/Beta_distribution">Wikipedia - Beta distribution</a>.
    /// </summary>
    /// <remarks>
    /// <para>There are a few special cases for the parameterization of the Beta distribution. When both
    /// shape parameters are positive infinity, the Beta distribution degenerates to a point distribution
    /// at 0.5. When one of the shape parameters is positive infinity, the distribution degenerates to a point
    /// distribution at the positive infinity. When both shape parameters are 0.0, the Beta distribution 
    /// degenerates to a Bernoulli distribution with parameter 0.5. When one shape parameter is 0.0, the
    /// distribution degenerates to a point distribution at the non-zero shape parameter.</para>
    /// <para>The distribution will use the <see cref="System.Random"/> by default. 
    /// Users can get/set the random number generator by using the <see cref="RandomSource"/> property.</para>
    /// <para>The statistics classes will check all the incoming parameters whether they are in the allowed
    /// range. This might involve heavy computation. Optionally, by setting Control.CheckDistributionParameters
    /// to <c>false</c>, all parameter checks can be turned off.</para></remarks>
    public sealed class Beta : IContinuousDistribution {
        /// <summary>
        /// Beta shape parameter A
        /// </summary>
        private double _a;

        /// <summary>
        /// Beta shape parameter B
        /// </summary>
        private double _b;

        private Random _random;

        /// <summary>
        /// 베타분포 생성자
        /// </summary>
        /// <param name="a">Beta 함수 계수 a</param>
        /// <param name="b">Beta 함수 계수 b</param>
        /// <param name="randomFactory">Random 인스턴스 생성자</param>
        public Beta(double a, double b, Func<Random> randomFactory = null) {
            SetParameters(a, b);
            _random = (randomFactory != null) ? randomFactory() : MathTool.GetRandomFactory().Invoke();
        }

        private void SetParameters(double a, double b) {
            AssertParameters(a, b);
            _a = a;
            _b = b;
        }

        /// <summary>
        /// Beta 함수 Factor A
        /// </summary>
        public double A {
            get { return _a; }
            set { SetParameters(value, _b); }
        }

        /// <summary>
        /// Beta 함수 Factor B
        /// </summary>
        public double B {
            get { return _b; }
            set { SetParameters(_a, value); }
        }

        public override string ToString() {
            return string.Format("Beta# A=[{0}], B=[{1}])", _a, _b);
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
                if(Math.Abs(_a - 0.0) < double.Epsilon && Math.Abs(_b - 0.0) < double.Epsilon)
                    return 0.5;

                if(Math.Abs(_a - 0.0) < double.Epsilon)
                    return 0.0;

                if(Math.Abs(_b - 0.0) < double.Epsilon)
                    return 1.0;

                if(double.IsPositiveInfinity(_a) && double.IsPositiveInfinity(_b))
                    return 0.5;

                if(double.IsPositiveInfinity(_a))
                    return 1.0;
                if(double.IsPositiveInfinity(_b))
                    return 0.0;

                return _a / (_a + _b);
            }
        }

        /// <summary>
        /// 분산
        /// </summary>
        public double Variance {
            get { return (_a * _b) / ((_a + _b) * (_a + _b) * (_a + _b + 1.0)); }
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
                if(double.IsPositiveInfinity(_a) || double.IsPositiveInfinity(_b))
                    return 0.0;

                if(Math.Abs(_a - 0.0) < double.Epsilon && Math.Abs(_b - 0.0) < double.Epsilon)
                    return -Math.Log(0.5);

                if(Math.Abs(_a - 0.0) < double.Epsilon || Math.Abs(_b - 0.0) < double.Epsilon)
                    return 0.0;

                return SpecialFunctions.BetaLn(_a, _b)
                       - ((_a - 1.0) * SpecialFunctions.DiGamma(_a))
                       - ((-B - 1.0) * SpecialFunctions.DiGamma(_b))
                       + ((_a + _b - 2.0) * SpecialFunctions.DiGamma(_a + _b));
            }
        }

        /// <summary>
        /// 기울기
        /// </summary>
        public double Skewness {
            get {
                if(Double.IsPositiveInfinity(_a) && Double.IsPositiveInfinity(_b)) {
                    return 0.0;
                }

                if(Double.IsPositiveInfinity(_a)) {
                    return -2.0;
                }

                if(Double.IsPositiveInfinity(_b)) {
                    return 2.0;
                }

                if(Math.Abs(_a - 0.0) < double.Epsilon && Math.Abs(_b - 0.0) < double.Epsilon) {
                    return 0.0;
                }

                if(Math.Abs(_a - 0.0) < double.Epsilon) {
                    return 2.0;
                }

                if(Math.Abs(_b - 0.0) < double.Epsilon) {
                    return -2.0;
                }

                return 2.0 * (_b - _a) * Math.Sqrt(_a + _b + 1.0) / ((_a + _b + 2.0) * Math.Sqrt(_a * _b));
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
            if(x < 0.0) {
                return 0.0;
            }

            if(x >= 1.0) {
                return 1.0;
            }

            if(Double.IsPositiveInfinity(_a) && Double.IsPositiveInfinity(_b)) {
                return x < 0.5 ? 0.0 : 1.0;
            }

            if(Double.IsPositiveInfinity(_a)) {
                return x < 1.0 ? 0.0 : 1.0;
            }

            if(Double.IsPositiveInfinity(_b)) {
                return x >= 0.0 ? 1.0 : 0.0;
            }

            if(Math.Abs(_a - 0.0) < double.Epsilon && Math.Abs(_b - 0.0) < double.Epsilon) {
                if(x >= 0.0 && x < 1.0) {
                    return 0.5;
                }

                return 1.0;
            }

            if(Math.Abs(_a - 0.0) < double.Epsilon) {
                return 1.0;
            }

            if(Math.Abs(_b - 0.0) < double.Epsilon) {
                return x >= 1.0 ? 1.0 : 0.0;
            }

            if(Math.Abs(_a - 1.0) < double.Epsilon && Math.Abs(_b - 1.0) < double.Epsilon) {
                return x;
            }

            return SpecialFunctions.BetaRegularized(_a, _b, x);
        }

        #endregion

        #region Implementation of IContinuousDistribution

        /// <summary>
        /// 최빈값 (빈도수가 최대인 값)
        /// </summary>
        public double Mode {
            get {
                if(Math.Abs(_a - 0.0) < double.Epsilon && Math.Abs(_b - 0.0) < double.Epsilon) {
                    return 0.5;
                }

                if(Math.Abs(_a - 0.0) < double.Epsilon) {
                    return 0.0;
                }

                if(Math.Abs(_b - 0.0) < double.Epsilon) {
                    return 1.0;
                }

                if(Double.IsPositiveInfinity(_a) && Double.IsPositiveInfinity(_b)) {
                    return 0.5;
                }

                if(Double.IsPositiveInfinity(_a)) {
                    return 1.0;
                }

                if(Double.IsPositiveInfinity(_b)) {
                    return 0.0;
                }

                if(Math.Abs(_a - 1.0) < double.Epsilon && Math.Abs(_b - 1.0) < double.Epsilon) {
                    return 0.5;
                }

                return (_a - 1.0) / (_a + _b - 2.0);
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
            get { return 1.0; }
        }

        /// <summary>
        /// 분포의 확률 밀도
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double Density(double x) {
            if(x < 0.0 || x > 1.0) {
                return 0.0;
            }

            if(Double.IsPositiveInfinity(_a) && Double.IsPositiveInfinity(_b)) {
                return (Math.Abs(x - 0.5) < double.Epsilon) ? Double.PositiveInfinity : 0.0;
            }

            if(Double.IsPositiveInfinity(_a)) {
                return (Math.Abs(x - 1.0) < double.Epsilon) ? Double.PositiveInfinity : 0.0;
            }

            if(Double.IsPositiveInfinity(_b)) {
                return (Math.Abs(x - 0.0) < double.Epsilon) ? Double.PositiveInfinity : 0.0;
            }

            if(Math.Abs(_a - 0.0) < double.Epsilon && Math.Abs(_b - 0.0) < double.Epsilon) {
                if(Math.Abs(x - 0.0) < double.Epsilon || Math.Abs(x - 1.0) < double.Epsilon) {
                    return Double.PositiveInfinity;
                }

                return 0.0;
            }

            if(Math.Abs(_a - 0.0) < double.Epsilon) {
                return Math.Abs(x - 0.0) < double.Epsilon ? Double.PositiveInfinity : 0.0;
            }

            if(Math.Abs(_b - 0.0) < double.Epsilon) {
                return Math.Abs(x - 1.0) < double.Epsilon ? Double.PositiveInfinity : 0.0;
            }

            if(Math.Abs(_a - 1.0) < double.Epsilon && Math.Abs(_b - 1.0) < double.Epsilon) {
                return 1.0;
            }

            var b = SpecialFunctions.Gamma(_a + _b) / (SpecialFunctions.Gamma(_a) * SpecialFunctions.Gamma(_b));
            return b * Math.Pow(x, _a - 1.0) * Math.Pow(1.0 - x, _b - 1.0);
        }

        /// <summary>
        /// 분포의 로그 확률 밀도 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double DensityLn(double x) {
            if(x < 0.0 || x > 1.0) {
                return Double.NegativeInfinity;
            }

            if(Double.IsPositiveInfinity(_a) && Double.IsPositiveInfinity(_b)) {
                return (Math.Abs(x - 0.5) < double.Epsilon) ? Double.PositiveInfinity : Double.NegativeInfinity;
            }

            if(Double.IsPositiveInfinity(_a)) {
                return (Math.Abs(x - 1.0) < double.Epsilon) ? Double.PositiveInfinity : Double.NegativeInfinity;
            }

            if(Double.IsPositiveInfinity(_b)) {
                return (Math.Abs(x - 0.0) < double.Epsilon) ? Double.PositiveInfinity : Double.NegativeInfinity;
            }

            if(Math.Abs(_a - 0.0) < double.Epsilon && Math.Abs(_b - 0.0) < double.Epsilon) {
                if(Math.Abs(x - 0.0) < double.Epsilon || Math.Abs(x - 1.0) < double.Epsilon) {
                    return Double.PositiveInfinity;
                }

                return Double.NegativeInfinity;
            }

            if(Math.Abs(_a - 0.0) < double.Epsilon) {
                return Math.Abs(x - 0.0) < double.Epsilon ? Double.PositiveInfinity : Double.NegativeInfinity;
            }

            if(Math.Abs(_b - 0.0) < double.Epsilon) {
                return Math.Abs(x - 1.0) < double.Epsilon ? Double.PositiveInfinity : Double.NegativeInfinity;
            }

            if(Math.Abs(_a - 1.0) < double.Epsilon && Math.Abs(_b - 1.0) < double.Epsilon) {
                return 0.0;
            }

            var a = SpecialFunctions.GammaLn(_a + _b) - SpecialFunctions.GammaLn(_a) - SpecialFunctions.GammaLn(_b);
            var b = Math.Abs(x - 0.0) < double.Epsilon
                        ? (Math.Abs(_a - 1.0) < double.Epsilon ? 0.0 : Double.NegativeInfinity)
                        : (_a - 1.0) * Math.Log(x);
            var c = Math.Abs(x - 1.0) < double.Epsilon
                        ? (Math.Abs(_b - 1.0) < double.Epsilon ? 0.0 : Double.NegativeInfinity)
                        : (_b - 1.0) * Math.Log(1.0 - x);

            return a + b + c;
        }

        /// <summary>
        /// 분포의 무작위 값을 제공합니다.
        /// </summary>
        /// <returns></returns>
        public double Sample() {
            return Sample(RandomSource, A, B);
        }

        /// <summary>
        /// 현 분포의 무작위 값을 열거합니다.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<double> Samples() {
            return Samples(RandomSource, A, B);
        }

        #endregion

        //! ======================================================================

        /// <summary>
        /// 베타 분포에 맞는 계수인지 확인합니다. 둘다 0보다 큰 양수여야 합니다.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        private static void AssertParameters(double a, double b) {
            a.ShouldBePositiveOrZero("a");
            b.ShouldBePositiveOrZero("b");
        }

        public static double Sample(Random rnd, double a, double b) {
            AssertParameters(a, b);
            return SampleBeta(rnd, a, b);
        }

        public static IEnumerable<double> Samples(Random rnd, double a, double b) {
            AssertParameters(a, b);
            while(true)
                yield return SampleBeta(rnd, a, b);
        }

        internal static double SampleBeta(Random rnd, double a, double b) {
            var x = Gamma.SampleGamma(rnd, a, 1.0);
            var y = Gamma.SampleGamma(rnd, b, 1.0);

            return x / (x + y);
        }
    }
}