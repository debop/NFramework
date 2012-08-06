using System;

namespace NSoft.NFramework.Numerics {
    /// <summary>
    /// 활선법(Secant) 알고리즘을 이용하여 특정 함수의 Root (근)을 찾는다 ( func(x) = 0 인 x 값 )
    /// </summary>
    public class SecantRootFinder : RootFinderBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// y = func(x) 함수의 [lower, upper] 구간에 대해, 근을 찾는다 ( func(x) = 0 인 x 값 )
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
                log.Debug(@"Find root by Secant... func=[{0}], lower=[{1}], upper=[{2}], tryCount=[{3}], tolerance=[{4}]",
                          func, lower, upper, tryCount, tolerance);

            if(lower > upper)
                MathTool.Swap(ref lower, ref upper);

            if(Math.Abs(func(lower).Clamp(RootY, tolerance) - RootY) < double.Epsilon)
                return lower;
            if(Math.Abs(func(upper).Clamp(RootY, tolerance) - RootY) < double.Epsilon)
                return upper;

            if(tryCount < DefaultTryCount)
                tryCount = DefaultTryCount;

            double root, xt;
            var y1 = func(lower);
            var y2 = func(upper);

            if(Math.Abs(y1) < Math.Abs(y2)) {
                root = lower;
                xt = upper;

                MathTool.Swap(ref y1, ref y2);
            }
            else {
                xt = lower;
                root = upper;
            }

            for(var i = 0; i < tryCount; i++) {
                var dx = (xt - root) * y2 / (y2 - y1);
                xt = root;
                y1 = y2;
                root += dx;
                y2 = func(root);

                if(IsDebugEnabled)
                    log.Debug(@"Secant root=[{0}]", root);

                if((Math.Abs(dx - RootY) < double.Epsilon) || (Math.Abs((y2 - y1) - RootY) < double.Epsilon))
                    return root;
                // if(Math.Abs(Math.Abs(dx).Clamp(RootY, tolerance) - RootY) < double.Epsilon || Math.Abs(Math.Abs(y2 - y1).Clamp(RootY, tolerance) - RootY) < double.Epsilon)
                //		return root;
            }

            return double.NaN;
        }
    }
}