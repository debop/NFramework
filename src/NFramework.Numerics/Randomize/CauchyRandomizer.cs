using System;

namespace NSoft.NFramework.Numerics {
    /// <summary>
    /// Cauchy 분포를 가지는 Random 함수
    /// </summary>
    /// <remarks>
    /// <para>( 밀도 함수 f(x) = 1 / ( 1 + x * x) * PI)의 분포 )</para>
    /// <para>정규분포와 비슷하지만, 끝이 퍼져있고, 적분은 발산하므로, 확률이나 분산은 구할 수 없다.</para>
    /// </remarks>
    public sealed class CauchyRandomizer : RandomizerBase {
        /// <summary>
        /// 생성자
        /// </summary>
        public CauchyRandomizer() {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="randomNumberFunc">사용자 정의 함수</param>
        public CauchyRandomizer(Func<double> randomNumberFunc = null) : base(randomNumberFunc) {}

        /// <summary>
        /// 난수 발생
        /// </summary>
        /// <returns>난수</returns>
        public override double Next() {
            double x, y;

            do {
                x = 1.0 - RandomNumberFunc();
                y = 2.0 * RandomNumberFunc() - 1.0;
            } while(x * x + y * y > 1.0);

            return y / x;
        }

        /// <summary>
        /// 일양분포를 가지도록 한 난수 발생 함수
        /// </summary>
        /// <returns></returns>
        public static double NextUniform() {
            double x, y;

            do {
                x = 1.0 - RandomGenerator.NextDouble();
                y = 2.0 * RandomGenerator.NextDouble() - 1.0;
            } while(x * x + y * y > 1.0);

            return y / x;
        }
    }
}