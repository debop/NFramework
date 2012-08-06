namespace NSoft.NFramework.Numerics {
    /// <summary>
    /// 보간을 수행하는 기본 Class
    /// </summary>
    public abstract class InterpolatorBase : IInterpolator {
        #region << logger >>

        protected static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        protected static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 두 배열이 같은 길이를 가졌는지 검사한다.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void CheckSameLength(double[] x, double[] y) {
            x.ShouldNotBeEmpty("x");
            y.ShouldNotBeEmpty("y");

            Guard.Assert(x.Length == y.Length, "(x, y) is not same length.");
            Guard.Assert(x.Length > 1, "Not enough values for interpolation. value length greater than 1.");
        }

        /// <summary>
        /// (x,y) 값으로 (t, ?) 값을 찾는다
        /// </summary>
        /// <param name="x">x 값</param>
        /// <param name="y">f(x)의 값</param>
        /// <param name="t">보간을 수행할 위치</param>
        /// <returns>f(t)의 값</returns>
        public virtual double Interpolate(double[] x, double[] y, double t) {
            var r = Interpolate(x, y, new[] { t });
            return r[0];
        }

        /// <summary>
        /// (x,y) 값으로 (t, ?) 값을 찾는다
        /// </summary>
        /// <param name="x">x 값</param>
        /// <param name="y">f(x)의 값</param>
        /// <param name="t">보간을 수행할 위치</param>
        /// <returns>f(t)의 값</returns>
        public abstract double[] Interpolate(double[] x, double[] y, double[] t);
    }
}