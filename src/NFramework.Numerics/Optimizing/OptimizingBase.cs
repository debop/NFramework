using System;

namespace NSoft.NFramework.Numerics.Optimizing {
    public abstract class OptimizingBase : IMinimumFinder {
        /// <summary>
        /// y = func(x) 함수의 [lower, upper] 구간에서 f(x)의 최소 값이 되는 x를 구합니다.
        /// </summary>
        /// <param name="func">함수</param>
        /// <param name="lower">구간의 하한</param>
        /// <param name="upper">구간의 상한</param>
        /// <param name="tryCount">시도횟수</param>
        /// <param name="tolerance">허용 오차</param>
        /// <returns>f(x)가 최소값이 되는 x 값, 검색 실패시에는 double.NaN을 반환한다</returns>
        public abstract double FindMiminum(Func<double, double> func,
                                           double lower,
                                           double upper,
                                           int tryCount = MathTool.DefaultTryCount,
                                           double tolerance = MathTool.Epsilon);
    }
}