using System;
using System.Threading.Tasks;

namespace NSoft.NFramework.Numerics.Integration {
    /// <summary>
    /// 사다리꼴 적분
    /// </summary>
    public sealed class TrapizoidalIntegrator : IntegratorBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private readonly object _syncLock = new object();

        /// <summary>
        /// 함수의 [a,b] 구간을 적분합니다.
        /// </summary>
        /// <param name="func">적분할 함수</param>
        /// <param name="a">적분 시작 위치</param>
        /// <param name="b">적분 끝 위치</param>
        /// <returns>적분 값</returns>
        public override double Integrate(Func<double, double> func, double a, double b) {
            func.ShouldNotBeNull("func");

            if(IsDebugEnabled)
                log.Debug(@"사다리꼴 적분법(Trapizoidal)을 이용하여 적분을 수행합니다. func=[{0}], a=[{1}], b=[{2}]", func, a, b);

            func.ShouldNotBeNull("func");

            if(a > b) MathTool.Swap<double>(ref a, ref b);

            double result = 0;

            int n = GetSteps(a, b);
            double h = (b - a) / n;
            double x = a;

            Parallel.For(1, n, () => 0.0, (i, loopState, local) => local + func(a + h * i),
                         local => { lock(_syncLock) result += local; });

            //for(int i = 1; i < n; i++)
            //{
            //    x += h;
            //    result += func(x);
            //}

            result = h * ((func(a) + func(b)) / 2 + result);

            if(IsDebugEnabled)
                log.Debug(@"적분결과=[{0}]", result);

            return result;
        }
    }
}