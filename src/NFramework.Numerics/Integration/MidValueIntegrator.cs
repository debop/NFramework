using System;
using NSoft.NFramework.Parallelism.Tools;

namespace NSoft.NFramework.Numerics.Integration {
    /// <summary>
    /// 중점법
    /// </summary>
    public sealed class MidValueIntegrator : IntegratorBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private readonly object _syncLock = new object();

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="steps">적분 구간 갯수</param>
        public MidValueIntegrator(int steps = DefaultSteps) {
            Steps = steps;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="integrator">원본</param>
        public MidValueIntegrator(MidValueIntegrator integrator) {
            integrator.ShouldNotBeNull("integrator");
            Steps = integrator.Steps;
        }

        private int _steps;

        /// <summary>
        /// 적분할 구간 갯수
        /// </summary>
        public int Steps {
            get { return _steps; }
            set { _steps = Math.Max(4, value); }
        }

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
                log.Debug(@"중점접(MidValue)를 이용하여 적분을 수행합니다. func=[{0}], a=[{1}], b=[{2}]", func, a, b);

            func.ShouldNotBeNull("f");

            if(a > b)
                MathTool.Swap(ref a, ref b);

            int n = GetSteps(a, b, Steps);
            double h = (b - a) / n;
            double hdiv2 = h / 2;
            double n2 = n * 2;
            double result = 0;

            ParallelTool.ForWithStep(1,
                                     (int)(n2 + 1),
                                     2,
                                     () => 0.0,
                                     (i, loopState, local) => local + func(a + i * hdiv2),
                                     local => {
                                         lock(_syncLock)
                                             result += local;
                                     });

            //for(int i = 1; i < n2; i += 2)
            //    result += func(a + i * hdiv2);

            result *= h;
            if(IsDebugEnabled)
                log.Debug(@"적분 결과=[{0}]", result);

            return result;
        }
    }
}