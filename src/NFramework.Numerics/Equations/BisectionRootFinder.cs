using System;

namespace NSoft.NFramework.Numerics {
    /// <summary>
    /// 이분법으로 특정 함수의 Root (근)을 찾는다 ( func(x) = 0 인 x 값 )
    /// </summary>
    public sealed class BisectionRootFinder : RootFinderBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 이분법으로 y = func(x) 함수의 [lower, upper] 구간에 대해, 근을 찾는다 (y=0 이되는 x 값)
        /// </summary>
        /// <param name="func">근을 찾을 함수</param>
        /// <param name="lower">근을 찾을 구간의 하한</param>
        /// <param name="upper">근을 찾을 구간의 상한</param>
        /// <param name="tryCount">시도 횟수</param>
        /// <param name="tolerance">근의 오차허용범위</param>
        /// <returns>근에 해당하는 x 값. 해를 못찾으면 <see cref="double.NaN"/>을 반환한다.</returns>
        public override double FindRoot(Func<double, double> func,
                                        double lower,
                                        double upper,
                                        int tryCount = MathTool.DefaultTryCount,
                                        double tolerance = MathTool.Epsilon) {
            func.ShouldNotBeNull("func");
            tolerance = Math.Abs(tolerance);

            if(IsDebugEnabled)
                log.Debug("Bisection을 이용하여, root 값을 찾습니다... func=[{0}], lower=[{1}], upper=[{2}], tryCount=[{3}], tolerance=[{4}]",
                          func, lower, upper, tryCount, tolerance);

            if(lower > upper)
                MathTool.Swap(ref lower, ref upper);

            var fa = func(lower);
            var fb = func(upper);

            if(Math.Abs(fa - RootY) < tolerance)
                return lower;
            if(Math.Abs(fb - RootY) < tolerance)
                return upper;

            Guard.Assert(Math.Abs(lower - upper) > tolerance, "상하한이 같은 값을 가지면 근을 구할 수 없습니다.");

            if(tryCount < DefaultTryCount)
                tryCount = DefaultTryCount;

            for(var k = 0; k < tryCount; k++) {
                var x = (lower + upper) / 2.0;

                if(Math.Abs(upper - x) < tolerance || Math.Abs(lower - x) < tolerance)
                    return x;

                var y = func(x);

                if(IsDebugEnabled)
                    log.Debug(@"Find root...  x=[{0}], y=[{1}]", x, y);

                // 해를 만족하던가, upper-lower의 변화가 매우 작던가..
                if(Math.Abs(y - RootY) < tolerance) {
                    if(IsDebugEnabled)
                        log.Debug("Iteration count=[{0}]", k);

                    return x;
                }

                if(fa * y > 0) {
                    lower = x;
                    fa = y;
                }
                else {
                    upper = x;
                    fb = y;
                }
            }

            return double.NaN;
        }
    }
}