namespace NSoft.NFramework.Numerics {
    /// <summary>
    /// 보간 (Interpolation)
    /// </summary>
    public interface IInterpolator {
        /// <summary>
        /// (x,y) 값으로 (t, ?) 값을 찾는다
        /// </summary>
        /// <param name="x">x 값</param>
        /// <param name="y">f(x)의 값</param>
        /// <param name="t">보간을 수행할 위치</param>
        /// <returns>f(t)의 값</returns>
        double Interpolate(double[] x, double[] y, double t);

        /// <summary>
        /// (x,y) 값으로 (t, ?) 값을 찾는다
        /// </summary>
        /// <param name="x">x 값</param>
        /// <param name="y">f(x)의 값</param>
        /// <param name="t">보간을 수행할 위치</param>
        /// <returns>f(t)의 값</returns>
        double[] Interpolate(double[] x, double[] y, double[] t);
    }
}