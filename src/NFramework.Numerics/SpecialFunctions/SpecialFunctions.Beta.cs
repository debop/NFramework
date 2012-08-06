using System;
using System.Collections.Generic;

namespace NSoft.NFramework.Numerics {
    public static partial class SpecialFunctions {
        internal const int BetaRegulizeIterations = 1000;

        /// <summary>
        /// Log Beta Function
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static double BetaLn(double x, double y) {
            x.ShouldBePositive("x");
            y.ShouldBePositive("y");

            return GammaLn(x) + GammaLn(y) - (GammaLn(x + y));
        }

        /// <summary>
        /// Log Beta Function
        /// </summary>
        /// <param name="xs"></param>
        /// <param name="ys"></param>
        /// <returns></returns>
        public static IEnumerable<double> BetaLn(this IEnumerable<double> xs, IEnumerable<double> ys) {
            xs.ShouldNotBeNull("xs");
            ys.ShouldNotBeNull("ys");

            using(var xe = xs.GetEnumerator())
            using(var ye = ys.GetEnumerator()) {
                while(xe.MoveNext() && ye.MoveNext()) {
                    yield return BetaLn(xe.Current, ye.Current);
                }
            }
        }

        /// <summary>
        /// Beta Function
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static double Beta(double x, double y) {
            return Math.Exp(BetaLn(x, y));
        }

        /// <summary>
        /// Beta Function
        /// </summary>
        /// <param name="xs"></param>
        /// <param name="ys"></param>
        /// <returns></returns>
        public static IEnumerable<double> Beta(this IEnumerable<double> xs, IEnumerable<double> ys) {
            xs.ShouldNotBeNull("xs");
            ys.ShouldNotBeNull("ys");

            using(var xe = xs.GetEnumerator())
            using(var ye = ys.GetEnumerator()) {
                while(xe.MoveNext() && ye.MoveNext()) {
                    yield return Beta(xe.Current, ye.Current);
                }
            }
        }

        /// <summary>
        /// Returns the lower incomplete (unregularized) beta function
        /// I_x(a,b) = int(t^(a-1)*(1-t)^(b-1),t=0..x) for real a &gt; 0, b &gt; 0, 1 &gt;= x &gt;= 0.
        /// </summary>
        /// <param name="a">The first Beta parameter, a positive real number.</param>
        /// <param name="b">The second Beta parameter, a positive real number.</param>
        /// <param name="x">The upper limit of the integral.</param>
        /// <returns>The lower incomplete (unregularized) beta function.</returns>
        public static double BetaIncomplete(double a, double b, double x) {
            return BetaRegularized(a, b, x) * Beta(a, b);
        }

        /// <summary>
        /// Returns the regularized lower incomplete beta function
        /// I_x(a,b) = 1/Beta(a,b) * int(t^(a-1)*(1-t)^(b-1),t=0..x) for real a &gt; 0, b &gt; 0, 1 &gt;= x &gt;= 0.
        /// </summary>
        /// <param name="a">The first Beta parameter, a positive real number.</param>
        /// <param name="b">The second Beta parameter, a positive real number.</param>
        /// <param name="x">The upper limit of the integral.</param>
        /// <returns>The regularized lower incomplete beta function.</returns>
        public static double BetaRegularized(double a, double b, double x) {
            return BetaRegularized(a, b, x, BetaRegulizeIterations);
        }

        /// <summary>
        /// Returns the regularized lower incomplete beta function
        /// I_x(a,b) = 1/Beta(a,b) * int(t^(a-1)*(1-t)^(b-1),t=0..x) for real a &gt; 0, b &gt; 0, 1 &gt;= x &gt;= 0.
        /// </summary>
        /// <param name="a">The first Beta parameter, a positive real number.</param>
        /// <param name="b">The second Beta parameter, a positive real number.</param>
        /// <param name="x">The upper limit of the integral.</param>
        /// <param name="maxIteration">최대 반복 횟수</param>
        /// <returns>The regularized lower incomplete beta function.</returns>
        public static double BetaRegularized(double a, double b, double x, int maxIteration) {
            a.ShouldBePositiveOrZero("a");
            b.ShouldBePositiveOrZero("b");
            x.ShouldBeGreaterOrEqual(0.0d, "x");
            x.ShouldBeLessOrEqual(1.0d, "x");

            var bt = (Math.Abs(x - 0.0) < double.Epsilon || Math.Abs(x - 1.0) < double.Epsilon)
                         ? 0.0
                         : Math.Exp(GammaLn(a + b) - GammaLn(a) - GammaLn(b) + (a * Math.Log(x)) + (b * Math.Log(1.0 - x)));

            var symmetryTransformation = x >= (a + 1.0) / (a + b + 2.0);

            /* Continued fraction representation */
            maxIteration = Math.Max(maxIteration, BetaRegulizeIterations);

            const double eps = double.Epsilon;
            const double fpmin = 1.0 / eps;

            if(symmetryTransformation) {
                x = 1.0 - x;
                var swap = a;
                a = b;
                b = swap;
            }

            var qab = a + b;
            var qap = a + 1.0;
            var qam = a - 1.0;
            var c = 1.0;
            var d = 1.0 - (qab * x / qap);

            if(Math.Abs(d) < fpmin) {
                d = fpmin;
            }

            d = 1.0 / d;
            var h = d;

            for(int m = 1, m2 = 2; m <= maxIteration; m++, m2 += 2) {
                var aa = m * (b - m) * x / ((qam + m2) * (a + m2));
                d = 1.0 + (aa * d);

                if(Math.Abs(d) < fpmin) {
                    d = fpmin;
                }

                c = 1.0 + (aa / c);
                if(Math.Abs(c) < fpmin) {
                    c = fpmin;
                }

                d = 1.0 / d;
                h *= d * c;
                aa = -(a + m) * (qab + m) * x / ((a + m2) * (qap + m2));
                d = 1.0 + (aa * d);

                if(Math.Abs(d) < fpmin) {
                    d = fpmin;
                }

                c = 1.0 + (aa / c);

                if(Math.Abs(c) < fpmin) {
                    c = fpmin;
                }

                d = 1.0 / d;
                var del = d * c;
                h *= del;

                if(Math.Abs(del - 1.0) <= eps) {
                    if(symmetryTransformation) {
                        return 1.0 - (bt * h / a);
                    }

                    return bt * h / a;
                }
            }

            throw new InvalidOperationException("최대 반복 횟수를 넘어어서, 값을 계산하지 못했습니다. maxIteration=" + maxIteration);
        }
    }
}