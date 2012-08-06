using System;

namespace NSoft.NFramework.Numerics {
    /// <summary>
    /// Newton-Rapson 알고리즘을 이용하여 특정 함수의 Root (근)을 찾는다 ( func(x) = 0 인 x 값 )
    /// </summary>
    public class NewtonRapsonRootFinder : RootFinderBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 미분 함수
        /// </summary>
        /// <param name="func">원함수</param>
        /// <param name="x">미분 함수를 얻기 위한 X 좌표</param>
        /// <returns>미분함수에의한 Y 좌표</returns>
        protected static double gfunc(Func<double, double> func, double x) {
            double dx = 1.0E-5;

            return (func(x + dx) - func(x)) / dx;
        }

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
                log.Debug(@"Find root by NewtonRapson... func=[{0}], lower=[{1}], upper=[{2}], tryCount=[{3}], tolerance=[{4}]",
                          func, lower, upper, tryCount, tolerance);

            if(tryCount < DefaultTryCount)
                tryCount = DefaultTryCount;

            double x = (lower + upper) / 2;

            if(Math.Abs(func(x).Clamp(RootY, tolerance) - RootY) < double.Epsilon)
                return x;

            for(int i = 0; i < tryCount; i++) {
                double prevX = x;
                x -= func(x) / gfunc(func, x);

                if(IsDebugEnabled)
                    log.Debug(@"root value=[{0}]", x);

                if(Math.Abs(x - prevX) < Math.Abs(prevX) * tolerance)
                    return x;
            }
            return double.NaN;
        }
    }
}