using System;

namespace NSoft.NFramework.Numerics {
    /// <summary>
    /// N 자유도를 가지는 T 분포를 가지는 Random Generator
    /// </summary>
    public sealed class TRandomizer : RandomizerBase {
        /// <summary>
        /// 생성자
        /// </summary>
        public TRandomizer() {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="n">자유도 (default : 1) (양수만 가능)</param>
        public TRandomizer(double n) {
            N = n;
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="n">자유도 (default : 1) (양수만 가능)</param>
        /// <param name="randomNumberFunc">사용자 정의 난수 발생 함수</param>
        public TRandomizer(int n, Func<double> randomNumberFunc = null)
            : base(randomNumberFunc) {
            N = n;
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="randomNumberFunc">사용자 정의 난수 발생 함수</param>
        public TRandomizer(Func<double> randomNumberFunc = null) : base(randomNumberFunc) {}

        private double _n = 1.0;

        /// <summary>
        /// 자유도 (T-Deviation parameter) (default : 1) (양수만 가능)
        /// </summary>
        public double N {
            get { return _n; }
            set {
                value.ShouldBePositive("N");
                _n = value;
            }
        }

        /// <summary>
        /// 난수 발생
        /// </summary>
        /// <returns>난수</returns>
        public override double Next() {
            double a, b, c;

            if(N <= 2.0)
                return RandomNumberFunc() / Math.Sqrt(N.ChiSquare() / N);

            do {
                a = RandomNumberFunc();
                b = a * a / (N - 2);
                c = Math.Log(1 - RandomNumberFunc()) / (1.0 - N / 2.0);
            } while(Math.Exp(-b - c) > 1.0 - b);

            return a / Math.Sqrt((1.0 - 2.0 / N) * (1.0 - b));
        }
    }
}