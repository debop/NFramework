using System;
using System.Collections.Generic;
using System.Linq;

namespace NSoft.NFramework.Numerics.Distributions.Discrete {
    /// <summary>
    /// <para>The Conway-Maxwell-Poisson distribution is a generalization of the Poisson, Geometric and Bernoulli
    /// distributions. It is parameterized by two real numbers "lambda" and "nu". For
    /// <list>    
    ///     <item>nu = 0 the distribution reverts to a Geometric distribution</item>
    ///     <item>nu = 1 the distribution reverts to the Poisson distribution</item>
    ///     <item>nu -> infinity the distribution converges to a Bernoulli distribution</item>
    /// </list></para>
    /// This implementation will cache the value of the normalization constant.
    /// <a href="http://en.wikipedia.org/wiki/Conway%E2%80%93Maxwell%E2%80%93Poisson_distribution">Wikipedia - ConwayMaxwellPoisson distribution</a>.
    /// </summary>
    /// <remarks><para>The distribution will use the <see cref="System.Random"/> by default. 
    /// Users can set the random number generator by using the <see cref="RandomSource"/> property.</para>
    /// <para>The statistics classes will check all the incoming parameters whether they are in the allowed
    /// range. This might involve heavy computation. Optionally, by setting Control.CheckDistributionParameters
    /// to <c>false</c>, all parameter checks can be turned off.</para></remarks>
    public class ConwayMaxwellPoisson : IDiscreteDistribution {
        private const double Tolerance = 1e-12;

        private double _mean = double.MinValue;
        private double _variance = double.MinValue;

        /// <summary>
        /// Caches the value of the normalization constant.
        /// </summary>
        private double _z = double.MinValue;

        /// <summary>
        /// The lambda parameter.
        /// </summary>
        private double _lambda;

        /// <summary>
        /// The nu parameter.
        /// </summary>
        private double _nu;

        /// <summary>
        /// The distribution's random number generator.
        /// </summary>
        private Random _random;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConwayMaxwellPoisson"/> class. 
        /// </summary>
        public ConwayMaxwellPoisson(double lambda, double nu, Func<Random> randomFactory = null) {
            SetParameters(lambda, nu);

            if(randomFactory != null)
                _random = randomFactory();
        }

        /// <summary>
        /// Sets the parameters of the distribution after checking their validity.
        /// </summary>
        /// <param name="lambda">The lambda parameter.</param>
        /// <param name="nu">The nu parameter.</param>
        /// <exception cref="InvalidOperationException">When the parameters don't pass the <see cref="AssertParameters"/> function.</exception>
        private void SetParameters(double lambda, double nu) {
            AssertParameters(lambda, nu);

            _lambda = lambda;
            _nu = nu;
        }

        public double Lambda {
            get { return _lambda; }
            set { SetParameters(value, _nu); }
        }

        public double Nu {
            get { return _nu; }
            set { SetParameters(_lambda, value); }
        }

        public override string ToString() {
            return string.Format("ConwayMaxwellPoisson(Lambda=[{0}], Nu=[{1}])", _lambda, _nu);
        }

        #region Implementation of IDistribution

        /// <summary>
        /// 난수 발생기
        /// TODO: Randomizer로 명칭 변경 
        /// </summary>
        public Random RandomSource {
            get { return _random ?? (_random = MathTool.GetRandomFactory()()); }
            set { _random = value; }
        }

        /// <summary>
        /// 평균
        /// </summary>
        public double Mean {
            get {
                // Special case requiring no computation.
                if(_lambda.ApproximateEqual(0.0))
                    return 0.0;

                if(!_mean.ApproximateEqual(double.MinValue))
                    return _mean;

                return (_mean = CalcMean(_lambda, _nu));
            }
        }

        /// <summary>
        /// 분산
        /// </summary>
        public double Variance {
            get {
                // Special case requiring no computation.
                if(_lambda.ApproximateEqual(0.0))
                    return 0.0;

                if(!_variance.ApproximateEqual(double.MinValue))
                    return _variance;

                return (_variance = CalcVariance(_lambda, _nu, Mean));
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
            double sum = 0;
            for(var i = 0; i < x + 1; i++) {
                sum += Probability(i);
            }

            return sum;
        }

        #endregion

        #region Implementation of IDiscreteDistribution

        /// <summary>
        /// 최빈값 (빈도수가 최대인 값)
        /// </summary>
        public int Mode {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// 중앙값
        /// </summary>
        public int Median {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// 최소값
        /// </summary>
        public int Minumum {
            get { return 0; }
        }

        /// <summary>
        /// 최대값
        /// </summary>
        public int Maximum {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// 분포의 확률
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public double Probability(int k) {
            return Math.Pow(_lambda, k) / Math.Pow(SpecialFunctions.Factorial(k), _nu) / Z;
        }

        /// <summary>
        /// 분포의 Log 확률
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public double ProbabilityLn(int k) {
            return Math.Log(Probability(k));
        }

        /// <summary>
        /// 분포의 데이타를 반환합니다.
        /// </summary>
        /// <returns></returns>
        public int Sample() {
            return Samples().First();
        }

        /// <summary>
        /// 분포의 무작위 데이타를 열거합니다.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<int> Samples() {
            return DoSamples(RandomSource, Lambda, Nu, Z);
        }

        #endregion

        //? ====================================================

        /// <summary>
        /// Gets the normalization constant of the Conway-Maxwell-Poisson distribution.
        /// </summary>
        private double Z {
            get {
                if(!_z.ApproximateEqual(double.MinValue))
                    return _z;

                _z = Normalization(_lambda, _nu);
                return _z;
            }
        }

        private static void AssertParameters(double lambda, double nu) {
            lambda.ShouldBePositive("lambda");
            nu.ShouldBePositiveOrZero("nu");
        }

        private static double CalcMean(double lambda, double nu) {
            // The normalization constant for the distribution.
            var z = 1 + lambda;

            // The probability of the next term.
            var a1 = lambda * lambda / Math.Pow(2, nu);

            // The unnormalized mean.
            var zx = lambda;

            // The contribution of the next term to the mean.
            var ax1 = 2 * a1;

            for(var i = 3; i < 1000; i++) {
                var e = lambda / Math.Pow(i, nu);
                var ex = lambda / Math.Pow(i, nu - 1) / (i - 1);
                var a2 = a1 * e;
                var ax2 = ax1 * ex;

                var m = zx / z;
                var upper = (zx + (ax1 / (1 - (ax2 / ax1)))) / z;
                var lower = zx / (z + (a1 / (1 - (a2 / a1))));

                if((ax2 < ax1) && (a2 < a1)) {
                    var r = (upper - lower) / m;
                    if(r < Tolerance) {
                        break;
                    }
                }

                z = z + a1;
                zx = zx + ax1;
                a1 = a2;
                ax1 = ax2;
            }

            return zx / z;
        }

        private static double CalcVariance(double lambda, double nu, double mean) {
            // The normalization constant for the distribution.
            var z = 1 + lambda;

            // The probability of the next term.
            var a1 = lambda * lambda / Math.Pow(2, nu);

            // The unnormalized second moment.
            var zxx = lambda;

            // The contribution of the next term to the second moment.
            var axx1 = 4 * a1;

            for(var i = 3; i < 1000; i++) {
                var e = lambda / Math.Pow(i, nu);
                var exx = lambda / Math.Pow(i, nu - 2) / (i - 1) / (i - 1);
                var a2 = a1 * e;
                var axx2 = axx1 * exx;

                var m = zxx / z;
                var upper = (zxx + (axx1 / (1 - (axx2 / axx1)))) / z;
                var lower = zxx / (z + (a1 / (1 - (a2 / a1))));

                if((axx2 < axx1) && (a2 < a1)) {
                    var r = (upper - lower) / m;
                    if(r < Tolerance) {
                        break;
                    }
                }

                z = z + a1;
                zxx = zxx + axx1;
                a1 = a2;
                axx1 = axx2;
            }

            return (zxx / z) - (mean * mean);
        }

        /// <summary>
        /// Computes an approximate normalization constant for the CMP distribution.
        /// </summary>
        /// <param name="lambda">The lambda parameter for the CMP distribution.</param>
        /// <param name="nu">The nu parameter for the CMP distribution.</param>
        /// <returns>
        /// an approximate normalization constant for the CMP distribution.
        /// </returns>
        private static double Normalization(double lambda, double nu) {
            // Initialize Z with the first two terms.
            var z = 1.0 + lambda;

            // Remembers the last term added.
            var t = lambda;

            // Start adding more terms until convergence.
            for(var i = 2; i < 1000; i++) {
                // The new addition for term i.
                var e = lambda / Math.Pow(i, nu);

                // The new term.
                t = t * e;

                // The updated normalization constant.
                z = z + t;

                // The stopping criterion.
                if(e < 1) {
                    if(t / (1 - e) / z < Tolerance) {
                        break;
                    }
                }
            }

            return z;
        }

        /// <summary>
        /// Returns one trials from the distribution.
        /// </summary>
        /// <param name="rnd">The random number generator to use.</param>
        /// <param name="lambda">The lambda parameter</param>
        /// <param name="nu">The nu parameter.</param>
        /// <param name="z">The z parameter.</param>
        /// <returns>
        /// One sample from the distribution implied by <paramref name="lambda"/>, <paramref name="nu"/>, and <paramref name="z"/>.
        /// </returns>
        private static IEnumerable<int> DoSamples(Random rnd, double lambda, double nu, double z) {
            while(true) {
                var u = rnd.NextDouble();
                var p = 1.0 / z;
                var cdf = p;
                var i = 0;

                while(u > cdf) {
                    i++;
                    p = p * lambda / Math.Pow(i, nu);
                    cdf += p;
                }

                yield return i;
            }
        }
    }
}