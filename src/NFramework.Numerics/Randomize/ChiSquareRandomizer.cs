using System;

namespace NSoft.NFramework.Numerics {
    /// <summary>
    /// Chi-Square distribution을 따르는 Random 함수
    /// </summary>
    /// <remarks>
    /// <para>
    /// 확률변수 z1,z2,...zn이 독립적으로 표준정규분포에 따를 때 
    /// x^2 = z1^2 + z2^2 + ... + zn^2 의 분포를 자유도 chi의 카이제곱(chi^2) 분포라고 한다.
    /// </para>
    /// </remarks>
    public sealed class ChiSquareRandomizer : RandomizerBase {
        /// <summary>
        /// 생성자
        /// </summary>
        public ChiSquareRandomizer() {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="n">단위시간당 발생하는 횟수</param>
        public ChiSquareRandomizer(double n) {
            N = n;
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="n">단위시간당 발생하는 횟수</param>
        /// <param name="randomNumberFunc">사용자 정의 난수 발생 함수</param>
        public ChiSquareRandomizer(double n, Func<double> randomNumberFunc = null)
            : base(randomNumberFunc) {
            N = n;
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="randomNumberFunc">사용자 정의 난수 발생 함수</param>
        public ChiSquareRandomizer(Func<double> randomNumberFunc) : base(randomNumberFunc) {}

        private double _n = 1.0;

        /// <summary>
        /// 단위시간당 발생하는 횟수 (Chi-Square parameter number)
        /// </summary>
        public double N {
            get { return _n; }
            set {
                if(Math.Abs(_n - value) > double.Epsilon) {
                    value.ShouldBePositive("N");
                    _n = value;
                    _gamma = null;
                }
            }
        }

        private GammaRandomizer _gamma;

        /// <summary>
        /// Gamma 분포를 가지는 Random Generator
        /// </summary>
        public GammaRandomizer Gamma {
            get {
                if(Equals(_gamma == null))
                    _gamma = new GammaRandomizer(N / 2.0);
                return _gamma;
            }
        }

        /// <summary>
        /// 난수 발생
        /// </summary>
        /// <returns>난수</returns>
        public override double Next() {
            double s = 0.0;
            var n = (int)N;

            for(int i = 0; i < n; i++) {
                double t = RandomNumberFunc();
                s += t * t;
            }

            return s;
        }

        /// <summary>
        /// 감마 함수 분포를 가지는 난수발생기로 난수 발생
        /// </summary>
        /// <returns></returns>
        public double NextGamma() {
            return 2.0 * Gamma.Next();
        }
    }
}