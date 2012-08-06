using System;

namespace NSoft.NFramework.Numerics {
    /// <summary>
    /// 베타 분포를 가지는 난수 발생기
    /// </summary>
    /// <remarks>
    /// <para>
    /// a, b가 둘다 1보다 클때,  f(0)=f(1)=0 이고 x = (a-1) / (a+b-2) 인 삼각형 산의 모양을 가진다.
    /// </para>
    /// <para>
    /// Gamma 분포가 하한을 결정하기 쉽지만, Beta 분포는 상,하한 분포를 결정하기 쉽다.
    /// a가 b보다 클 수록 1에  치우친 분포, b가 a보다 클 때에는 0에 치우친 분포를 나타낸다.
    /// </para>
    /// <para>
    /// 평균 = a / (a+b), 분산 = a*b/((a+b)^2 * (a + b + 1))<br/>
    /// a,b 의 값이 커질수록 표준편차가 작아져서 Narrow한 정규분포를 가지게 된다.
    /// </para>
    /// </remarks>
    public sealed class BetaRandomizer : RandomizerBase {
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="a">하한</param>
        /// <param name="b">상한</param>
        public BetaRandomizer(double a, double b) {
            A = a;
            B = b;
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="a">하한</param>
        /// <param name="b">상한</param>
        /// <param name="randomNumberFunc">사용자 난수 발생기</param>
        public BetaRandomizer(double a, double b, Func<double> randomNumberFunc = null)
            : base(randomNumberFunc) {
            A = a;
            B = b;
        }

        private double _a;
        private double _b;

        /// <summary>
        /// 하한
        /// </summary>
        public double A {
            get { return _a; }
            set {
                value.ShouldNotBeZero("A");
                _a = value;
            }
        }

        /// <summary>
        /// 상한
        /// </summary>
        public double B {
            get { return _b; }
            set {
                value.ShouldNotBeZero("B");
                _b = value;
            }
        }

        /// <summary>
        /// 평균
        /// </summary>
        public double Mean {
            get {
                Guard.Assert<DivideByZeroException>((A + B == 0.0), "A + B 가 0이면 안됩니다.");
                return A / (A + B);
            }
        }

        /// <summary>
        /// 분산
        /// </summary>
        public double Variance {
            get {
                double s = A + B;
                return (A * B) / (s * s * (s + 1));
            }
        }

        /// <summary>
        /// 표준편차
        /// </summary>
        public double StDev {
            get { return Math.Sqrt(Variance); }
        }

        /// <summary>
        /// 난수 발생
        /// </summary>
        /// <returns>난수</returns>
        public override double Next() {
            double x, y;

            do {
                x = Math.Pow(_randomNumberFunc(), 1.0 / A);
                y = Math.Pow(_randomNumberFunc(), 1.0 / B);
            } while(x + y > 1.0);

            return x / (x + y);
        }
    }
}