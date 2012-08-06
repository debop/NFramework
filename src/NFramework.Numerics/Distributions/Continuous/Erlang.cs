using System;
using System.Collections.Generic;

namespace NSoft.NFramework.Numerics.Distributions.Continuous {
    /// <summary>
    /// Erlang 연속 분포
    /// This class implements functionality for the Erlang distribution.<br/>
    /// This distribution is a continuous probability distribution with wide applicability primarily due to its
    /// relation to the exponential and Gamma distributions.
    /// <a href="http://en.wikipedia.org/wiki/Erlang_distribution">Wikipedia - Erlang distribution</a>.
    /// </summary>
    /// <remarks><para>The distribution will use the <see cref="System.Random"/> by default. 
    /// Users can set the random number generator by using the <see cref="RandomSource"/> property.</para>
    /// <para>The statistics classes will check all the incoming parameters whether they are in the allowed
    /// range. This might involve heavy computation. Optionally, by setting Control.CheckDistributionParameters
    /// to <c>false</c>, all parameter checks can be turned off.</para></remarks>
    public class Erlang : IContinuousDistribution {
        #region << Static Constructors >>

        public static Erlang WithShapeScale(int shape, double scale) {
            return new Erlang(shape, 1.0 / scale);
        }

        public static Erlang WithShapeInvScale(int shape, double invScale) {
            return new Erlang(shape, invScale);
        }

        #endregion

        private double _shape;
        private double _invScale;
        private Random _random;

        public Erlang(int shape, double invScale, Func<Random> randomFactory = null) {
            SetParameters(shape, invScale);
            _random = (randomFactory != null) ? randomFactory() : MathTool.GetRandomFactory().Invoke();
        }

        private void SetParameters(double shape, double invScale) {
            AssertParameters(shape, invScale);
            _shape = shape;
            _invScale = invScale;
        }

        public int Shape {
            get { return (int)_shape; }
            set { SetParameters(value, _invScale); }
        }

        public double InvScale {
            get { return _invScale; }
            set { SetParameters(_shape, value); }
        }

        public double Scale {
            get { return 1.0 / _invScale; }

            set {
                var invScale = 1.0 / value;

                if(Double.IsNegativeInfinity(invScale)) {
                    invScale = -invScale;
                }

                SetParameters(_shape, invScale);
            }
        }

        public override string ToString() {
            return string.Format("Erlang(Shape=[{0}], Inverse Scale=[{1}]", _shape, _invScale);
        }

        //! ====================================================

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
                if(double.IsPositiveInfinity(_invScale))
                    return _shape;

                if(_invScale.ApproximateEqual(0.0) && _shape.ApproximateEqual(0.0))
                    return Double.NaN;

                return _shape / _invScale;
            }
        }

        /// <summary>
        /// 분산
        /// </summary>
        public double Variance {
            get {
                if(Double.IsPositiveInfinity(_invScale))
                    return 0.0;

                if(_invScale.ApproximateEqual(0.0) && _shape.ApproximateEqual(0.0))
                    return Double.NaN;

                return _shape / (_invScale * _invScale);
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
            get {
                if(Double.IsPositiveInfinity(_invScale))
                    return 0.0;

                if(_invScale.ApproximateEqual(0.0) && _shape.ApproximateEqual(0.0))
                    return Double.NaN;

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

                if(_invScale.ApproximateEqual(0.0) && _shape.ApproximateEqual(0.0))
                    return Double.NaN;

                return 2.0 / Math.Sqrt(_shape);
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
            if(Double.IsPositiveInfinity(_invScale))
                return x >= _shape ? 1.0 : 0.0;

            if(_shape.ApproximateEqual(0.0) && _invScale.ApproximateEqual(0.0))
                return 0.0;

            return SpecialFunctions.GammaLowerRegularized(_shape, x * _invScale);
        }

        #endregion

        #region << IContinuousDistribution >>

        /// <summary>
        /// 최빈값 (빈도수가 최대인 값)
        /// </summary>
        public double Mode {
            get {
                if(_shape < 1)
                    throw new NotSupportedException();

                if(Double.IsPositiveInfinity(_invScale))
                    return _shape;

                if(_invScale.ApproximateEqual(0.0) && _shape.ApproximateEqual(0.0))
                    return Double.NaN;

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
            if(double.IsPositiveInfinity(_invScale))
                return x.ApproximateEqual(_shape) ? Double.PositiveInfinity : 0.0;


            if(_shape.ApproximateEqual(0.0) && _invScale.ApproximateEqual(0.0))
                return 0.0;

            if(_shape.ApproximateEqual(1.0))
                return _invScale * Math.Exp(-_invScale * x);

            return Math.Pow(_invScale, _shape) * Math.Pow(x, _shape - 1.0) * Math.Exp(-_invScale * x) / SpecialFunctions.Gamma(_shape);
        }

        /// <summary>
        /// 분포의 로그 확률 밀도 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double DensityLn(double x) {
            if(Double.IsPositiveInfinity(_invScale))
                return x.ApproximateEqual(_shape) ? Double.PositiveInfinity : Double.NegativeInfinity;


            if(_shape.ApproximateEqual(0.0) && _invScale.ApproximateEqual(0.0))
                return Double.NegativeInfinity;

            if(_shape.ApproximateEqual(1.0))
                return Math.Log(_invScale) - (_invScale * x);

            return (_shape * Math.Log(_invScale)) + ((_shape - 1.0) * Math.Log(x)) - (_invScale * x) - SpecialFunctions.GammaLn(_shape);
        }

        /// <summary>
        /// 분포의 무작위 값을 제공합니다.
        /// </summary>
        /// <returns></returns>
        public double Sample() {
            return DoSample(_random, _shape, _invScale);
        }

        /// <summary>
        /// 현 분포의 무작위 값을 열거합니다.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<double> Samples() {
            while(true)
                yield return DoSample(_random, _shape, _invScale);
        }

        #endregion

        //! ====================================================

        private static void AssertParameters(double shape, double invScale) {
            shape.ShouldBePositiveOrZero("shape");
            invScale.ShouldBePositiveOrZero("invScale");
        }

        private static double DoSample(Random rnd, double shape, double invScale) {
            if(double.IsPositiveInfinity(invScale))
                return shape;

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

        public static double Sample(Random rnd, double shape, double invScale) {
            rnd.ShouldNotBeNull("rnd");
            AssertParameters(shape, invScale);
            return DoSample(rnd, shape, invScale);
        }

        public static IEnumerable<double> Samples(Random rnd, double shape, double invScale) {
            rnd.ShouldNotBeNull("rnd");
            AssertParameters(shape, invScale);

            while(true)
                yield return DoSample(rnd, shape, invScale);
        }
    }
}