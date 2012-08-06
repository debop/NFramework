using System;
using System.Collections.Generic;

namespace NSoft.NFramework.Numerics.Distributions.Continuous {
    /// <summary>
    /// This class implements functionality for the Chi distribution. <br/>
    /// This distribution is  a continuous probability distribution. <br/>
    /// The distribution usually arises when a k-dimensional vector's orthogonal 
    /// components are independent and each follow a standard normal distribution. The length of the vector will 
    /// then have a chi distribution.<br/>
    /// <a href="http://en.wikipedia.org/wiki/Chi_distribution">Wikipedia - Chi distribution</a>.
    /// </summary>
    /// <remarks><para>The distribution will use the <see cref="System.Random"/> by default. 
    /// Users can set the random number generator by using the <see cref="RandomSource"/> property.</para>
    /// <para>The statistics classes will check all the incoming parameters whether they are in the allowed
    /// range. This might involve heavy computation. Optionally, by setting Control.CheckDistributionParameters
    /// to <c>false</c>, all parameter checks can be turned off.</para></remarks>
    public sealed class Chi : IContinuousDistribution {
        private double _dof;
        private Random _random;

        public Chi(double dof) : this(dof, MathTool.GetRandomFactory()) {}

        public Chi(double dof, Func<Random> randomFactory) {
            SetParameters(dof);
            _random = (randomFactory != null) ? randomFactory() : MathTool.GetRandomFactory().Invoke();
        }

        private void SetParameters(double dof) {
            AssertParameters(dof);
            _dof = dof;
        }

        /// <summary>
        /// Chi 분포의 자유도
        /// </summary>
        public double DegreeOfFreedom {
            get { return _dof; }
            set { SetParameters(value); }
        }

        public override string ToString() {
            return string.Format("Chi(DoF=[{0}])", _dof);
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
            get { return Math.Sqrt(2) * (SpecialFunctions.Gamma((_dof + 1.0) / 2.0) / SpecialFunctions.Gamma(_dof / 2.0)); }
        }

        /// <summary>
        /// 분산
        /// </summary>
        public double Variance {
            get { return _dof - Mean.Square(); }
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
                return SpecialFunctions.GammaLn(_dof / 2.0) +
                       ((_dof - Math.Log(2) - ((_dof - 1.0) * SpecialFunctions.DiGamma(_dof / 2.0))) / 2.0);
            }
        }

        /// <summary>
        /// 기울기
        /// </summary>
        public double Skewness {
            get {
                var sigma = StDev;
                return (Mean * (1.0 - (2.0 * (sigma * sigma)))) / (sigma * sigma * sigma);
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
            return SpecialFunctions.GammaLowerIncomplete(_dof / 2.0, x * x / 2.0) / SpecialFunctions.Gamma(_dof / 2.0);
        }

        #endregion

        #region << IContinuousDistribution >>

        /// <summary>
        /// 최빈값 (빈도수가 최대인 값)
        /// </summary>
        public double Mode {
            get {
                if(_dof < 1)
                    throw new NotSupportedException();

                return Math.Sqrt(_dof - 1.0);
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
            return (Math.Pow(2.0, 1.0 - (_dof / 2.0)) * Math.Pow(x, _dof - 1.0) * Math.Exp(-x * x / 2.0)) /
                   SpecialFunctions.Gamma(_dof / 2.0);
        }

        /// <summary>
        /// 분포의 로그 확률 밀도 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double DensityLn(double x) {
            return ((1.0 - (_dof / 2.0)) * Math.Log(2.0)) + ((_dof - 1.0) * Math.Log(x)) - (x * x / 2.0) -
                   SpecialFunctions.GammaLn(_dof / 2.0);
        }

        /// <summary>
        /// 분포의 무작위 값을 제공합니다.
        /// </summary>
        /// <returns></returns>
        public double Sample() {
            return DoSample(_random, _dof);
        }

        /// <summary>
        /// 현 분포의 무작위 값을 열거합니다.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<double> Samples() {
            while(true)
                yield return DoSample(_random, _dof);
        }

        #endregion

        #region << Static Methods >>

        private static void AssertParameters(double dof) {
            dof.ShouldBePositive("dof");
        }

        private static double DoSample(Random rnd, double dof) {
            double sum = 0;
            var n = (int)dof;
            for(var i = 0; i < n; i++) {
                sum += Math.Pow(Normal.Sample(rnd, 0.0, 1.0), 2);
            }

            return Math.Sqrt(sum);
        }

        /// <summary>
        /// Chi 분포의 무작위 값을 반환합니다.
        /// </summary>
        /// <param name="rnd"></param>
        /// <param name="dof"></param>
        /// <returns></returns>
        public static double Sample(Random rnd, double dof) {
            AssertParameters(dof);
            return DoSample(rnd, dof);
        }

        /// <summary>
        ///  Chi 분포의 무작위 값을 열거합니다.
        /// </summary>
        /// <param name="rnd"></param>
        /// <param name="dof"></param>
        /// <returns></returns>
        public static IEnumerable<double> Samples(Random rnd, double dof) {
            AssertParameters(dof);

            while(true)
                yield return DoSample(rnd, dof);
        }

        #endregion
    }
}