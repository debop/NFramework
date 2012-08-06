using System;
using NSoft.NFramework.Parallelism.Tools;

namespace NSoft.NFramework.Numerics.Integration {
    /// <summary>
    /// Simpson 적분법 (1-4-1 방식) 2차 곡선 근사 방식이다. 정확도가 <see cref="TrapizoidalIntegrator"/>보다 좋다.
    /// </summary>
    public sealed class SimpsonIntegrator : IntegratorBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private readonly object _syncLock = new object();

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="steps">적분 구간 갯수</param>
        public SimpsonIntegrator(int steps = DefaultSteps) {
            Steps = steps;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="integrator">source integrator</param>
        public SimpsonIntegrator(SimpsonIntegrator integrator) {
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
                log.Debug(@"Simpson 적분법을 이용하여 적분을 수행합니다. func=[{0}], a=[{1}], b=[{2}]", func, a, b);

            func.ShouldNotBeNull("unaryFunc");

            if(a > b)
                return Integrate(func, b, a);

            _steps = GetSteps(a, b, _steps);

            double h = (b - a) / (2.0 * _steps);
            double hdiv3 = h / 3;

            double fo = 0;
            double fe = 0;

            double N = 2 * _steps - 3;

            ParallelTool.ForWithStep(1, (int)N + 1, 2, () => 0.0, (i, loopState, local) => local + func(a + h * i),
                                     local => { lock(_syncLock) fo += local; });
            ParallelTool.ForWithStep(1, (int)N + 1, 2, () => 0.0, (i, loopState, local) => local + func(a + h * (i + 1)),
                                     local => { lock(_syncLock) fe += local; });

            //for(int i = 1; i <= N; i += 2)
            //{
            //    fo += func(a + h * i); // 홀수항 (odd)
            //    fe += func(a + h * (i + 1)); // 짝수항 (even)
            //}

            var result = (func(a) + func(b) + 4.0 * (fo + func(b - h)) + 2.0 * fe) * hdiv3;

            if(IsDebugEnabled)
                log.Debug(@"적분결과=[{0}]", result);

            return result;
        }
    }
}