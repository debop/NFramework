using System;

namespace NSoft.NFramework.Numerics {
    /// <summary>
    /// logistic 분포를 따르는 난수 발생기
    /// </summary>
    /// <remarks>
    /// <para>
    /// 분포 함수 F(x) = 1 / ( 1 + e ^-x), 밀도 함수 f(x) = e^-x / ( 1 + e^-x )^2 의 분포
    /// </para>
    /// <para>평균 0, 분산 pi^2/3 이다. 분포는 정규분포에 가깝지만 끝은 정규 분포보다 길다.</para>
    /// <para>역함수 F-1(x) = log((1-x)/x) 를 이용한다.</para>
    /// </remarks>
    public sealed class LogisticsRandomizer : RandomizerBase {
        /// <summary>
        /// 생성자
        /// </summary>
        public LogisticsRandomizer() {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="randomNumberFunc">사용자 정의 난수 발생 함수</param>
        public LogisticsRandomizer(Func<double> randomNumberFunc = null) : base(randomNumberFunc) {}

        /// <summary>
        /// 난수 발생
        /// </summary>
        /// <returns>난수</returns>
        public override double Next() {
            var x = RandomNumberFunc();
            return Math.Log((1.0 - x) / x);
        }

        /// <summary>
        /// 일양분포의 난수값을 정규분포의 난수값으로 변환한다.
        /// </summary>
        /// <param name="v">Uniform 분포에서의 변수</param>
        /// <param name="mean">정규분포의 평균</param>
        /// <param name="stdev">정규분포의 표준편차</param>
        /// <returns>난수 발생 분포(밀도 함수)를 정규분포로 변환했을 때의 난수 값</returns>
        protected override double Normalize(double v, double mean, double stdev) {
            return stdev * (v / 1.81) + mean;
        }
    }
}