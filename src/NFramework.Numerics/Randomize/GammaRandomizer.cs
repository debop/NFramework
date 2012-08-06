using System;

namespace NSoft.NFramework.Numerics {
    /// <summary>
    /// Gamma 분포를 가지는 Random Generator
    /// </summary>
    /// <remarks>
    /// <para>
    /// 평균하여 단위시간에 1회 일어나는 독립 사상을 어떤 시각부터 관측했을 때 정확히 a 개째의 사상이 일어나기 까지의 시간은 감마분포에 따른다.
    /// </para>
    /// </remarks>
    public sealed class GammaRandomizer : RandomizerBase {
        /// <summary>
        /// 생성자
        /// </summary>
        public GammaRandomizer() {
            N = 1.0;
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="n"></param>
        public GammaRandomizer(double n) {
            N = n;
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="n"></param>
        /// <param name="randomNumberFunc"></param>
        public GammaRandomizer(double n, Func<double> randomNumberFunc = null)
            : base(randomNumberFunc) {
            N = n;
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="randomNumberFunc"></param>
        public GammaRandomizer(Func<double> randomNumberFunc = null)
            : base(randomNumberFunc) {
            N = 1.0;
        }

        /// <summary>
        /// 단위시간당 발생하는 횟수
        /// </summary>
        public double N { get; set; }

        #region Overrides of RandomizerBase

        /// <summary>
        /// 난수 발생
        /// </summary>
        /// <returns>난수</returns>
        public override double Next() {
            double t, u, x, y;

            if(N > 1.0) {
                t = Math.Sqrt(2 * N - 1);
                do {
                    do {
                        do {
                            x = 1.0 - RandomNumberFunc();
                            y = 2.0 * RandomNumberFunc() - 1.0;
                        } while(x * x + y * y > 1.0);

                        y /= x;
                        x = t * y + N - 1;
                    } while(x <= 0);

                    u = (N - 1) * Math.Log(x / (N - 1)) - t * y;
                } while(u < -50 || RandomNumberFunc() > (1 + y * y) * Math.Exp(u));
            }
            else {
                t = MathTool.E / (N + MathTool.E);
                do {
                    if(RandomNumberFunc() < t) {
                        x = Math.Pow(RandomNumberFunc(), 1 / N);
                        y = Math.Exp(-x);
                    }
                    else {
                        x = 1 - Math.Log(RandomNumberFunc());
                        y = Math.Pow(x, N - 1);
                    }
                } while(RandomNumberFunc() >= y);
            }

            return x;
        }

        #endregion
    }
}