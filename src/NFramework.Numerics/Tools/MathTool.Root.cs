using System;

namespace NSoft.NFramework.Numerics {
    public static partial class MathTool {
        /// <summary>
        /// 3제곱근 계산
        /// </summary>
        /// <param name="x">3 제곱근을 계산할 값</param>
        /// <returns>3 제곱근 값</returns>
        public static double CubeRoot(this double x) {
            if(Math.Abs(x - 0.0d) < double.Epsilon)
                return 0d;

            double prev;
            var positive = (x > 0);
            if(!positive)
                x = -x;

            var s = (x > 1) ? x : 1d;
            do {
                prev = s;
                s = (x / (s * s) + 2 * s) / 3;
            } while(s < prev);

            return (positive) ? prev : -prev;
        }

        /// <summary>
        /// 3제곱근 계산
        /// </summary>
        /// <param name="x">3 제곱근을 계산할 값</param>
        /// <returns>3 제곱근 값</returns>
        public static float CubeRoot(this float x) {
            if(Math.Abs(x - 0.0f) < float.Epsilon)
                return 0.0f;

            float prev;
            var positive = (x > 0);
            if(!positive)
                x = -x;

            float s = (x > 1) ? x : 1f;
            do {
                prev = s;
                s = (x / (s * s) + 2 * s) / 3;
            } while(s < prev);

            return (positive) ? prev : -prev;
        }

        /// <summary>
        /// 3제곱근 계산
        /// </summary>
        /// <param name="x">3 제곱근을 계산할 값</param>
        /// <returns>3 제곱근 값</returns>
        public static decimal CubeRoot(this decimal x) {
            if(Math.Abs(x - 0.0m) < (decimal)Epsilon)
                return 0.0m;

            decimal prev;
            var positive = (x > 0);
            if(!positive)
                x = -x;

            decimal s = (x > 1) ? x : 1m;
            do {
                prev = s;
                s = (x / (s * s) + 2 * s) / 3;
            } while(s < prev);

            return (positive) ? prev : -prev;
        }
    }
}