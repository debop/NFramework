using System;
using System.Collections.Generic;

namespace NSoft.NFramework.Numerics.Distributions.Continuous {
    /// <summary>
    /// Implements the FisherSnedecor distribution. For details about this distribution, see 
    /// <a href="http://en.wikipedia.org/wiki/F-distribution">Wikipedia - FisherSnedecor distribution</a>.
    /// </summary>
    /// <remarks><para>The distribution will use the <see cref="System.Random"/> by default. 
    /// Users can set the random number generator by using the <see cref="RandomSource"/> property.</para>
    /// <para>The statistics classes will check all the incoming parameters whether they are in the allowed
    /// range. This might involve heavy computation. Optionally, by setting Control.CheckDistributionParameters
    /// to <c>false</c>, all parameter checks can be turned off.</para></remarks>
    public class FisherSnedecor : IContinuousDistribution {
        private double _d1;
        private double _d2;
        private Random _random;

        public FisherSnedecor(double d1, double d2, Func<Random> randomFactory = null) {
            SetParameters(d1, d2);
            RandomSource = (randomFactory != null) ? randomFactory() : MathTool.GetRandomFactory()();
        }

        private void SetParameters(double d1, double d2) {
            AssertParameters(d1, d2);
            _d1 = d1;
            _d2 = d2;
        }

        public double DegreeOfFreedom1 {
            get { return _d1; }
            set { SetParameters(value, _d2); }
        }

        public double DegreeOfFreedom2 {
            get { return _d2; }
            set { SetParameters(_d1, value); }
        }

        public override string ToString() {
            return string.Format("FisherSnedecor(DegreeOfFreedom1=[{0}], DegreeOfFreedom2=[{1}]", _d1, _d2);
        }

        #region << IDistribution >>

        /// <summary>
        /// 난수 발생기
        /// TODO: Randomizer로 명칭 변경 
        /// </summary>
        public Random RandomSource {
            get { return _random; }
            set {
                value.ShouldNotBeNull("value");
                _random = value;
            }
        }

        /// <summary>
        /// 평균
        /// </summary>
        public double Mean {
            get {
                Guard.Assert<NotSupportedException>(_d2 > 2, "DegreeOfFreedom2 값이 2 이상이어야 Mean 값을 제공할 수 있습니다.");

                return _d2 / (_d2 - 2.0);
            }
        }

        /// <summary>
        /// 분산
        /// </summary>
        public double Variance {
            get {
                Guard.Assert<NotSupportedException>(_d2 > 4, "DegreeOfFreedom2 값이 4 이상이어야 Variance 값을 제공할 수 있습니다.");

                return (2.0 * _d2 * _d2 * (_d1 + _d2 - 2.0)) / (_d1 * (_d2 - 2.0) * (_d2 - 2.0) * (_d2 - 4.0));
            }
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
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// 기울기
        /// </summary>
        public double Skewness {
            get {
                Guard.Assert<NotSupportedException>(_d2 > 6, "DegreeOfFreedom2 값이 6 이상이어야 Skewness 값을 제공할 수 있습니다.");

                return (((2.0 * _d1) + _d2 - 2.0) * Math.Sqrt(8.0 * (_d2 - 4.0))) / ((_d2 - 6.0) * Math.Sqrt(_d1 * (_d1 + _d2 - 2.0)));
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
            return SpecialFunctions.BetaRegularized(_d1 / 2.0, _d2 / 2.0, _d1 * x / ((_d1 * x) + _d2));
        }

        #endregion

        #region << IContinuousDistribution >>

        /// <summary>
        /// 최빈값 (빈도수가 최대인 값)
        /// </summary>
        public double Mode {
            get {
                Guard.Assert<NotSupportedException>(_d2 > 2, "DegreeOfFreedom2 값이 2 이상이어야 Mode 값을 제공할 수 있습니다.");

                return (_d2 * (_d1 - 2.0)) / (_d1 * (_d2 + 2.0));
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
            return Math.Sqrt(Math.Pow(_d1 * x, _d1) * Math.Pow(_d2, _d2) / Math.Pow((_d1 * x) + _d2, _d1 + _d2)) /
                   (x * SpecialFunctions.Beta(_d1 / 2.0, _d2 / 2.0));
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
            return DoSample(_random, _d1, _d2);
        }

        /// <summary>
        /// 현 분포의 무작위 값을 열거합니다.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<double> Samples() {
            while(true)
                yield return DoSample(_random, _d1, _d2);
        }

        #endregion

        //! ===============================================

        private static void AssertParameters(double d1, double d2) {
            d1.ShouldBePositive("d1");
            d2.ShouldBePositive("d2");
        }

        private static double DoSample(Random rnd, double d1, double d2) {
            return (ChiSquare.Sample(rnd, d1) / d1) / (ChiSquare.Sample(rnd, d2) / d2);
        }

        public double Sample(Random rnd, double d1, double d2) {
            rnd.ShouldNotBeNull("rnd");
            AssertParameters(d1, d2);

            return DoSample(rnd, d1, d2);
        }

        public IEnumerable<double> Samples(Random rnd, double d1, double d2) {
            rnd.ShouldNotBeNull("rnd");
            AssertParameters(d1, d2);
            while(true)
                yield return DoSample(rnd, d1, d2);
        }
    }
}