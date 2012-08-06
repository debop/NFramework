using System;

namespace NSoft.NFramework.Numerics.Integration {
    /// <summary>
    /// 적분을 수행하는 기본 Class
    /// </summary>
    public abstract class IntegratorBase : IIntegrator {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 기본 적분 구간 갯수
        /// </summary>
        public const int DefaultSteps = 9999;

        /// <summary>
        /// 기본 차수
        /// </summary>
        public const int DefaultOrder = 5;

        /// <summary>
        /// 구간에 따라 Step 수를 조정한다.
        /// </summary>
        /// <param name="a">하한</param>
        /// <param name="b">상한</param>
        /// <param name="steps">초기 구간 수</param>
        /// <returns>조정된 구간 수</returns>
        protected static int GetSteps(double a, double b, int steps = DefaultSteps) {
            if(a > b) MathTool.Swap(ref a, ref b);

            if((b - a) > steps * 10.0)
                steps = (int)((b - a) * 1000);

            if(IsDebugEnabled)
                log.Debug(@"Get steps for integration. lower bound=[{0}], upper bound=[{1}], steps=[{2}]", a, b, steps);

            return steps;
        }

        /// <summary>
        /// 함수의 [a,b] 구간을 적분합니다.
        /// </summary>
        /// <param name="func">적분할 함수</param>
        /// <param name="a">적분 시작 위치</param>
        /// <param name="b">적분 끝 위치</param>
        /// <returns>적분 값</returns>
        public abstract double Integrate(Func<double, double> func, double a, double b);
    }
}