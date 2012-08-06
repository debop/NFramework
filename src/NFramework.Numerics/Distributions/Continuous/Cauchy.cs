using System;
using System.Collections.Generic;

namespace NSoft.NFramework.Numerics.Distributions.Continuous {
    /// <summary>
    /// The Cauchy distribution is a symmetric continuous probability distribution. For details about this distribution, see 
    /// <a href="http://en.wikipedia.org/wiki/cauchy_distribution">Wikipedia - Cauchy distribution</a>.
    /// </summary>
    /// <remarks><para>The distribution will use the <see cref="System.Random"/> by default. 
    /// Users can get/set the random number generator by using the <see cref="RandomSource"/> property.</para>
    /// <para>The statistics classes will check all the incoming parameters whether they are in the allowed
    /// range. This might involve heavy computation. Optionally, by setting Control.CheckDistributionParameters
    /// to <c>false</c>, all parameter checks can be turned off.</para></remarks>
    public sealed class Cauchy : IContinuousDistribution {
        private double _scale;
        private Random _random;

        public Cauchy() : this(0, 1, null) {}

        public Cauchy(double location = 0, double scale = 1, Func<Random> randomFactory = null) {
            SetParameters(location, scale);
            _random = (randomFactory != null) ? randomFactory() : MathTool.GetRandomFactory().Invoke();
        }

        private void SetParameters(double location, double scale) {
            AssertParameters(location, scale);

            Median = location;
            _scale = scale;
        }

        /// <summary>
        /// 분포의 위치 파라미터 (기본 0), <see cref="Median"/> 값과 같다.
        /// </summary>
        public double Location {
            get { return Median; }
            set { SetParameters(value, _scale); }
        }

        /// <summary>
        /// 분포 Scale (기본 1)
        /// </summary>
        public double Scale {
            get { return _scale; }
            set { SetParameters(Median, value); }
        }

        public override string ToString() {
            return string.Format("Cauchy(Location=[{0}], Scale=[{1}])", Location, Scale);
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
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// 분산
        /// </summary>
        public double Variance {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// 표준편차
        /// </summary>
        public double StDev {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// 분포의 엔트로피 (불안정성) 
        /// </summary>
        public double Entropy {
            get { return Math.Log(4.0 * MathTool.Pi * _scale); }
        }

        /// <summary>
        /// 기울기
        /// </summary>
        public double Skewness {
            get { throw new NotSupportedException(); }
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
            return ((1.0 / MathTool.Pi) * Math.Atan((x - Median) / _scale)) + 0.5;
        }

        #endregion

        #region << IContinuousDistribution >>

        /// <summary>
        /// 최빈값 (빈도수가 최대인 값)
        /// </summary>
        public double Mode {
            get { return Median; }
        }

        /// <summary>
        /// 중앙값
        /// </summary>
        public double Median { get; private set; }

        /// <summary>
        /// 최소값
        /// </summary>
        public double Minumum {
            get { return double.NegativeInfinity; }
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
            return 1.0 / (MathTool.Pi * _scale * (1.0 + (((x - Median) / _scale) * ((x - Median) / _scale))));
        }

        /// <summary>
        /// 분포의 로그 확률 밀도 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double DensityLn(double x) {
            return -Math.Log(MathTool.Pi * _scale * (1.0 + (((x - Median) / _scale) * ((x - Median) / _scale))));
        }

        /// <summary>
        /// 분포의 무작위 값을 제공합니다.
        /// </summary>
        /// <returns></returns>
        public double Sample() {
            return DoSample(RandomSource, Median, _scale);
        }

        /// <summary>
        /// 현 분포의 무작위 값을 열거합니다.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<double> Samples() {
            while(true)
                yield return DoSample(RandomSource, Median, _scale);
        }

        #endregion

        //! ==============================================================

        private static void AssertParameters(double location, double scale) {
            scale.ShouldBePositive("scale");
            location.ShouldNotBeNaN("location");
        }

        /// <summary>
        /// 분포의 변량을 생성합니다.
        /// </summary>
        /// <param name="rnd"></param>
        /// <param name="location"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        private static double DoSample(Random rnd, double location, double scale) {
            return location + (scale * Math.Tan(MathTool.Pi * (rnd.NextDouble() - 0.5)));
        }
    }
}