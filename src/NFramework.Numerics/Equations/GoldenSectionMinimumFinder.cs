using System;

namespace NSoft.NFramework.Numerics {
    /// <summary>
    /// 황금비-분할법 (Golden Section ) 알고리즘을 이용하여 특정 함수의 [lower, upper]구간에서 func(x)의 최소값의 위치를 찾는다.
    /// </summary>
    public class GoldenSectionMinimumFinder : MinimumFinderBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 황금 비율 (2 / (3 + Sqrt(5))
        /// </summary>
        public static readonly double GodenRatio = 2.0 / (3.0 + Math.Sqrt(5.0));

        /// <summary>
        /// y = func(x) 함수의 [lower, upper] 구간에서 f(x)의 최소 값이 되는 x를 구합니다.
        /// </summary>
        /// <param name="func">함수</param>
        /// <param name="lower">구간의 하한</param>
        /// <param name="upper">구간의 상한</param>
        /// <param name="tryCount">시도횟수</param>
        /// <param name="tolerance">허용 오차</param>
        /// <returns>f(x)가 최소값이 되는 x 값, 검색 실패시에는 double.NaN을 반환한다</returns>
        public override double FindMiminum(Func<double, double> @func,
                                           double lower,
                                           double upper,
                                           int tryCount = MathTool.DefaultTryCount,
                                           double tolerance = MathTool.Epsilon) {
            @func.ShouldNotBeNull("func");
            tolerance = Math.Abs(tolerance);

            if(IsDebugEnabled)
                log.Debug("Find root by GoldenSectionMinimumFinder... " +
                          "func=[{0}], lower=[{1}], upper=[{2}], tryCount=[{3}], tolerance=[{4}]",
                          func, lower, upper, tryCount, tolerance);

            if(tryCount < MathTool.DefaultTryCount)
                tryCount = MathTool.DefaultTryCount;

            if(lower > upper)
                MathTool.Swap(ref lower, ref upper);

            var t = GodenRatio * (upper - lower);
            var c = lower + t;
            var d = upper - t;

            var fc = @func(c);
            var fd = @func(d);

            for(var i = 0; i < tryCount; i++) {
                if(fc > fd) {
                    lower = c;
                    c = d;
                    fc = fd;
                    d = upper - GodenRatio * (upper - lower);

                    if(Math.Abs(d - c) <= tolerance)
                        return c;

                    fd = @func(d);
                }
                else {
                    upper = d;
                    d = c;
                    fd = fc;
                    c = lower + GodenRatio * (upper - lower);

                    if(Math.Abs(d - c) <= tolerance)
                        return d;

                    fc = @func(c);
                }
            }

            return double.NaN;
        }
    }
}