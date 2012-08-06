using System;

namespace NSoft.NFramework.Numerics.Integration {
    /// <summary>
    /// Romberg 적분법
    /// </summary>
    public sealed class RombergIntegrator : IntegratorBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private int _order;
        private double[,] _rom;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="order">Romberg approximation order (기본값은 5)</param>
        public RombergIntegrator(int order = DefaultOrder) {
            if(order < DefaultOrder)
                order = DefaultOrder;

            _order = order;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="integrator">원본 RombergIntegrator</param>
        public RombergIntegrator(RombergIntegrator integrator) {
            integrator.ShouldNotBeNull("integrator");
            _order = integrator.Order;
        }

        /// <summary>
        /// Romberg approximation order (기본값은 5)
        /// </summary>
        public int Order {
            get { return _order; }
            set {
                if(_order != value) {
                    _order = Math.Max(value, DefaultOrder);
                    _rom = null;
                }
            }
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
                log.Debug(@"Romberg 적분법을 이용하여 적분을 수행합니다. func=[{0}], a=[{1}], b=[{2}]", func, a, b);

            if(a > b)
                MathTool.Swap(ref a, ref b);

            if((_rom == null) || (_rom.GetLength(1) == _order))
                _rom = new double[2,_order];

            double h = (b - a);
            _rom[0, 0] = 0.5d * h * (func(a) + func(b));

            for(int i = 2, ipower = 1; i <= _order; i++, ipower *= 2, h /= 2) {
                // approximation using the trapezoid rule.
                double sum = 0;
                for(var j = 1; j <= ipower; j++)
                    sum += func(a + h * (j - 0.5));

                // Richardson extrapolation 
                _rom[1, 0] = 0.5 * (_rom[0, 0] + (h * sum));

                for(int k = 1, kpower = 4; k < i; k++, kpower *= 4)
                    _rom[1, k] = (kpower * _rom[1, k - 1] - _rom[0, k - 1]) / (kpower - 1);

                // save the extrapolated value for the next integration
                for(int j = 0; j < i; j++)
                    _rom[0, j] = _rom[1, j];
            }

            if(IsDebugEnabled)
                log.Debug(@"적분결과=[{0}]", _rom[0, _order - 1]);

            return _rom[0, _order - 1];
        }
    }
}