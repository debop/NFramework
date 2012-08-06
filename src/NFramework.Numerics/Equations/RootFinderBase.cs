using System;

namespace NSoft.NFramework.Numerics {
    /// <summary>
    /// 근을 찾는 알고리즘을 구현한 Class의 기본 Class입니다.
    /// </summary>
    public abstract class RootFinderBase : IRootFinder {
        /// <summary>
        /// 해 찾기 위한 기본 시도 횟수
        /// </summary>
        public const int DefaultTryCount = 10000;

        /// <summary>
        /// Y축 원점
        /// </summary>
        public const double RootY = 0.0;

        /// <summary>
        /// y = func(x) 함수의 [lower, upper] 구간에 대해, 근을 찾는다 ( func(x) = 0 인 x 값 )
        /// </summary>
        /// <param name="func">근을 찾을 함수</param>
        /// <param name="lower">근을 찾을 구간의 하한</param>
        /// <param name="upper">근을 찾을 구간의 상한</param>
        /// <param name="tryCount">시도 횟수</param>
        /// <param name="tolerance">근의 오차허용범위</param>
        /// <returns>근에 해당하는 x 값. 해를 못찾으면 <see cref="double.NaN"/>을 반환한다.</returns>
        public abstract double FindRoot(Func<double, double> func,
                                        double lower,
                                        double upper,
                                        int tryCount = MathTool.DefaultTryCount,
                                        double tolerance = MathTool.Epsilon);
    }
}