using System;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Numerics {
    /// <summary>
    /// Spline 보간법
    /// </summary>
    public sealed class SplineInterpolator : InterpolatorBase {
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
            double dr;

            int ns;
            double den, dif, dift, ho, hp, w;

            unsafe {
                fixed(double* rp = &r[0])
                fixed(double* xp = &x[0])
                fixed(double* yp = &y[0])
                fixed(double* tp = &t[0]) {
                    for(var k = 0; k < M; k++) {
                        dif = Math.Abs(tp[k] - xp[0]);
                        ns = 0;

                        var c = ArrayTool.Copy(y, 0, y.Length);
                        var d = ArrayTool.Copy(y, 0, y.Length);

                        fixed(double* cp = &c[0])
                        fixed(double* dp = &d[0]) {
                            for(var m = 0; m < N; m++) {
                                if((dift = Math.Abs(tp[k] - xp[m])) < dif) {
                                    ns = m;
                                    dif = dift;
                                }
                            }

                            rp[k] = yp[ns--];

                            for(var m = 0; m < N - 1; m++) {
                                for(var i = 0; i < N - m - 1; i++) {
                                    ho = xp[i] - tp[k];
                                    hp = xp[i + m + 1] - tp[k];
                                    w = cp[i + 1] - dp[i];

                                    if(Math.Abs((den = ho - hp) - 0.0) < double.Epsilon)
                                        throw new InvalidOperationException("Error in routine point");

                                    den = w / den;
                                    dp[i] = hp * den;
                                    cp[i] = ho * den;
                                }

                                rp[k] += (dr = (2 * (ns + 1) < (N - m - 1) ? cp[ns + 1] : dp[ns--]));
                            }
                        }
                    }
                }
            }
            return r;
        }
    }
}