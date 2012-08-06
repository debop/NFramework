namespace NSoft.NFramework.Numerics {
    /// <summary>
    /// Lagrange 보간법
    /// </summary>
    public sealed class LagrangeInterpolator : InterpolatorBase {
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

            int N = x.Length;
            int M = t.Length;

            var r = new double[M];

            unsafe {
                fixed(double* rp = &r[0])
                fixed(double* xp = &x[0])
                fixed(double* yp = &y[0])
                fixed(double* tp = &t[0]) {
                    for(int k = 0; k < M; k++) {
                        rp[k] = 0;
                        for(int i = 0; i < N; i++) {
                            double p = yp[i];

                            for(int j = 0; j < N; j++)
                                if(i != j)
                                    p *= (tp[k] - xp[j]) / (xp[i] - xp[j]);

                            rp[k] += p;
                        }
                    }
                }
            }


            return r;
        }
    }
}