using System;
using System.Collections.Generic;
using System.Linq;

namespace NSoft.NFramework.Numerics.Distributions.Continuous {
    public sealed class StudentT : IContinuousDistribution {
        private double _location;
        private double _scale;
        private double _dof;
        private Random _random;

        public StudentT() : this(0.0, 1.0, 1.0, null) {}

        public StudentT(double location = 0.0, double scale = 1.0, double dof = 1.0, Func<Random> randomFactory = null) {
            SetParameters(location, scale, dof);

            if(randomFactory != null)
                _random = randomFactory();
        }

        private void SetParameters(double location, double scale, double dof) {
            AssertParameters(location, scale, dof);
            _location = location;
            _scale = scale;
            _dof = dof;
        }

        public double Location {
            get { return _location; }
            set { SetParameters(value, _scale, _dof); }
        }

        public double Scale {
            get { return _scale; }
            set { SetParameters(_location, value, _dof); }
        }

        public double DoF {
            get { return _dof; }
            set { SetParameters(_location, _scale, value); }
        }

        public override string ToString() {
            return string.Format("StudentT(Location=[{0}], Scale=[{1}], DoF=[{2}])", Location, Scale, DoF);
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
            get { return _dof > 1.0 ? _location : double.NaN; }
        }

        /// <summary>
        /// 분산
        /// </summary>
        public double Variance {
            get {
                if(double.IsPositiveInfinity(_dof))
                    return _scale * _scale;

                if(_dof > 2.0)
                    return _dof * _scale * _scale / (_dof - 2.0);

                return _dof > 1.0 ? double.PositiveInfinity : double.NaN;
            }
        }

        /// <summary>
        /// 표준편차
        /// </summary>
        public double StDev {
            get {
                if(double.IsPositiveInfinity(_dof))
                    return _scale;

                if(_dof > 2.0)
                    return Math.Sqrt(_dof * _scale * _scale / (_dof - 2.0));

                return _dof > 1.0 ? double.PositiveInfinity : double.NaN;
            }
        }

        /// <summary>
        /// 분포의 엔트로피 (불안정성) 
        /// </summary>
        public double Entropy {
            get {
                if(_location.ApproximateEqual(0.0) == false || _scale.ApproximateEqual(1.0) == false)
                    throw new NotSupportedException("Location이 0.0 이거나 Scale이 1.0이면 지원할 수 없습니다.");

                return (((_dof + 1.0) / 2.0) * (SpecialFunctions.DiGamma((1.0 + _dof) / 2.0) - SpecialFunctions.DiGamma(_dof / 2.0)))
                       + Math.Log(Math.Sqrt(_dof) * SpecialFunctions.Beta(_dof / 2.0, 1.0 / 2.0));
            }
        }

        /// <summary>
        /// 기울기
        /// </summary>
        public double Skewness {
            get {
                Guard.Assert<NotSupportedException>(_dof > 3, "DoF > 3 이어야 지원합니다.");
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
            // TODO JVG we can probably do a better job for Cauchy special case
            if(Double.IsPositiveInfinity(_dof)) {
                return Normal.CumulativeDistribution(_location, _scale, x);
            }

            var k = (x - _location) / _scale;
            var h = _dof / (_dof + (k * k));
            var ib = 0.5 * SpecialFunctions.BetaRegularized(_dof / 2.0, 0.5, h);
            return x <= _location ? ib : 1.0 - ib;
        }

        #endregion

        #region Implementation of IContinuousDistribution

        /// <summary>
        /// 최빈값 (빈도수가 최대인 값)
        /// </summary>
        public double Mode {
            get { return _location; }
        }

        /// <summary>
        /// 중앙값
        /// </summary>
        public double Median {
            get { return _location; }
        }

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
            // TODO JVG we can probably do a better job for Cauchy special case
            if(double.IsPositiveInfinity(_dof)) {
                return Normal.Density(_location, _scale, x);
            }

            var d = (x - _location) / _scale;

            return SpecialFunctions.Gamma((_dof + 1.0) / 2.0)
                   * Math.Pow(1.0 + (d * d / _dof), -0.5 * (_dof + 1.0))
                   / SpecialFunctions.Gamma(_dof / 2.0)
                   / Math.Sqrt(_dof * Math.PI)
                   / _scale;
        }

        /// <summary>
        /// 분포의 로그 확률 밀도 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double DensityLn(double x) {
            // TODO JVG we can probably do a better job for Cauchy special case
            if(double.IsPositiveInfinity(_dof)) {
                return Normal.DensityLn(_location, _scale, x);
            }

            var d = (x - _location) / _scale;

            return SpecialFunctions.GammaLn((_dof + 1.0) / 2.0)
                   - (0.5 * ((_dof + 1.0) * Math.Log(1.0 + (d * d / _dof))))
                   - SpecialFunctions.GammaLn(_dof / 2.0)
                   - (0.5 * Math.Log(_dof * Math.PI)) - Math.Log(_scale);
        }

        /// <summary>
        /// 분포의 무작위 값을 제공합니다.
        /// </summary>
        /// <returns></returns>
        public double Sample() {
            return Samples().First();
        }

        /// <summary>
        /// 현 분포의 무작위 값을 열거합니다.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<double> Samples() {
            return DoSamples(RandomSource, _dof).Select(x => _location + (_scale * x));
        }

        #endregion

        //! =====================================================

        private static void AssertParameters(double location, double scale, double dof) {
            scale.ShouldBePositive("scale");
            dof.ShouldBePositive("dof");
            location.ShouldNotBeNaN("location");
        }

        /// <summary>
        /// Samples standard student-t distributed random variables.
        /// </summary>
        /// <remarks>The algorithm is method 2 in section 5, chapter 9 
        /// in L. Devroye's "Non-Uniform Random Variate Generation"</remarks>
        /// <param name="rnd">The random number generator to use.</param>
        /// <param name="dof">The degrees of freedom for the standard student-t distribution.</param>
        /// <returns>a random number from the standard student-t distribution.</returns>
        private static IEnumerable<double> DoSamples(Random rnd, double dof) {
            while(true) {
                var n = Normal.SampleBoxMuller(rnd).Item1;
                var g = Gamma.Sample(rnd, 0.5 * dof, 0.5);
                yield return Math.Sqrt(dof / g) * n;
            }
        }

        public double Sample(Random rnd, double location, double scale, double dof) {
            return Samples(rnd, location, scale, dof).First();
        }

        public IEnumerable<double> Samples(Random rnd, double location, double scale, double dof) {
            rnd.ShouldNotBeNull("rnd");
            AssertParameters(location, scale, dof);

            return DoSamples(RandomSource, dof).Select(x => location + (scale * x));
        }
    }
}