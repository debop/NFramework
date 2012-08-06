using System;

namespace NSoft.NFramework.Numerics {
    /// <summary>
    /// 특정 함수의 Root (근)을 찾는다 ( func(x) = 0 인 x 값 )
    /// </summary>
    public interface IRootFinder {
        /// <summary>
        /// y = func(x) 함수의 [lower, upper] 구간에 대해, 근을 찾는다 ( func(x) = 0 인 x 값 )
        /// </summary>
        /// <param name="func">근을 찾을 함수</param>
        /// <param name="lower">근을 찾을 구간의 하한</param>
        /// <param name="upper">근을 찾을 구간의 상한</param>
        /// <param name="tryCount">시도 횟수</param>
        /// <param name="tolerance">근의 오차허용범위</param>
        /// <returns>근에 해당하는 x 값. 해를 못찾으면 <see cref="double.NaN"/>을 반환한다.</returns>
        double FindRoot(Func<double, double> func,
                        double lower,
                        double upper,
                        int tryCount = MathTool.DefaultTryCount,
                        double tolerance = MathTool.Epsilon);
    }
}