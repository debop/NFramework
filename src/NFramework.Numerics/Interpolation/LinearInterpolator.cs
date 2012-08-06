namespace NSoft.NFramework.Numerics {
    /// <summary>
    /// 선형 보간
    /// </summary>
    public sealed class LinearInterpolator : InterpolatorBase {
        /// <summary>
        /// 최대 반복 횟수
        /// </summary>
        public const int MaxIteration = 1000;

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

            var r = new double[M];

            unsafe {
                fixed(double* rp = &r[0])
                fixed(double* xp = &x[0])
                fixed(double* yp = &y[0])
                fixed(double* tp = &t[0]) {
                    int low, mid, hi;

                    for(var k = 0; k < M; k++) {
                        if(tp[k] <= xp[0]) {
                            low = 0;
                            hi = 1;
                        }
                        else if(tp[k] >= xp[N - 1]) {
                            low = N - 2;
                            hi = N - 1;
                        }
                        else {
                            low = 0;
                            hi = N - 1;
                            int j;

                            for(j = 0; j < MaxIteration && hi > low + 1; j++) {
                                mid = (hi + low) / 2;

                                if(tp[k] > xp[mid])
                                    low = mid;
                                else
                                    hi = mid;
                            }

                            Guard.Assert(j < MaxIteration, @"최대 반복 횟수를 초과했습니다.  maxIteration=[{0}]", j);
                        }

                        rp[k] = (tp[k] - xp[low]) / (xp[hi] - xp[low]) * (yp[hi] - yp[low]) + yp[low];
                    }
                }
            }
            return r;
        }
    }
}