using System;
using System.Drawing;

namespace NSoft.NFramework.Numerics {
    public static partial class MathTool {
        /// <summary>
        /// 직각삼각형의 빗변의 길이를 구하는 식이다.  Sqrt(a^2 + b^2)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double Hypot(this double a, double b) {
            double r;

            if(a.ApproximateEqual(0))
                return Math.Abs(b);

            if(b.ApproximateEqual(0))
                return Math.Abs(a);


            if(Math.Abs(a) > Math.Abs(b)) {
                r = b / a;
                r = Math.Abs(a) * Math.Sqrt(1.0 + r * r);
            }
            else if(b.ApproximateEqual(0) == false) {
                r = a / b;
                r = Math.Abs(b) * Math.Sqrt(1 + r * r);
            }
            else
                r = 0.0;

            return r;
        }

        /// <summary>
        /// Moler-Morrison 법을 이용하여 직각 삼각형의 빗변을 구한다.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double Hypot2(this double a, double b) {
            a = Math.Abs(a);
            b = Math.Abs(b);

            if(b > a)
                Swap(ref a, ref b);

            if(b.ApproximateEqual(0))
                return a;

            for(var i = 0; i < 3; i++) {
                var t = b / a;
                t *= t;
                t /= (4 + t);
                a += 2 * a * t;
                b *= t;
            }
            return a;
        }

        /// <summary>
        /// 2차원 두 점 사이의 최단 거리를 구한다.
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        public static double Distance(double x1, double y1, double x2, double y2) {
            return Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
        }

        /// <summary>
        ///  2차원 두 점 사이의 최단 거리를 구한다.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static double Distance(this Point p1, Point p2) {
            return Distance(p1.X, p1.Y, p2.X, p2.Y);
        }

        /// <summary>
        ///  2차원 두 점 사이의 최단 거리를 구한다.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static double Distance(this PointF p1, PointF p2) {
            return Distance(p1.X, p1.Y, p2.X, p2.Y);
        }
    }
}