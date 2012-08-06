namespace NSoft.NFramework.Numerics {
    /// <summary>
    /// Newton 보간법
    /// </summary>
    public class NewtonInterpolator : InterpolatorBase {
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

            var a = new double[N];
            var w = new double[N];

            unsafe {
                fixed(double* ap = &a[0])
                fixed(double* wp = &w[0])
                fixed(double* xp = &x[0])
                fixed(double* yp = &y[0]) {
                    // Newton 보간법으로 계수 행렬을 먼저 구해 놓는다.
                    for(int i = 0; i < N; i++) {
                        wp[i] = yp[i];

                        for(int j = i - 1; j >= 0; j--)
                            wp[j] = (wp[j + 1] - wp[j]) / (xp[i] - xp[j]);

                        ap[i] = wp[0];
                    }
                }
            }

            var r = new double[M];

            unsafe {
                fixed(double* rp = &r[0])
                fixed(double* ap = &a[0])
                fixed(double* tp = &t[0])
                fixed(double* xp = &x[0]) {
                    for(int k = 0; k < M; k++) {
                        rp[k] = ap[N - 1];
                        for(int i = N - 2; i >= 0; i--)
                            rp[k] = rp[k] * (tp[k] - xp[i]) + ap[i];
                    }
                }
            }
            return r;
        }
    }
}