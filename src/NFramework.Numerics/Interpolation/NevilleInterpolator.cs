namespace NSoft.NFramework.Numerics {
    /// <summary>
    /// Neville 보간법
    /// </summary>
    public sealed class NevilleInterpolator : InterpolatorBase {
        /// <summary>
        /// (x,y) 값으로 (t, ?) 값을 찾는다
        /// </summary>
        /// <param name="x">x 값</param>
        /// <param name="y">f(x)의 값</param>
        /// <param name="t">보간을 수행할 위치</param>
        /// <returns>f(t)의 값</returns>
        public override double[] Interpolate(double[] x, double[] y, double[] t) {
            CheckSameLength(x, y);
            t.ShouldNotBeEmpty("t");

            var N = x.Length;
            var M = t.Length;

            var w = new double[N];
            var r = new double[M];

            unsafe {
                fixed(double* wp = &w[0])
                fixed(double* rp = &r[0])
                fixed(double* xp = &x[0])
                fixed(double* yp = &y[0])
                fixed(double* tp = &t[0]) {
                    for(var k = 0; k < M; k++) {
                        for(var i = 0; i < N; i++) {
                            wp[i] = yp[i];
                            for(var j = i - 1; j >= 0; j--)
                                wp[j] = wp[j + 1] + (wp[j + 1] - wp[j]) * (tp[k] - xp[i]) / (xp[i] - xp[j]);
                        }
                        rp[k] = wp[0];
                    }
                }
            }

            return r;
        }
    }
}