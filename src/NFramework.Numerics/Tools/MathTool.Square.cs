namespace NSoft.NFramework.Numerics {
    public static partial class MathTool {
        /// <summary>
        /// 제곱
        /// </summary>
        /// <param name="x">제곱할 수</param>
        /// <returns>제곱된 수</returns>
        public static double Square(this double x) {
            return x * x;
        }

        /// <summary>
        /// 제곱
        /// </summary>
        /// <param name="x">제곱할 수</param>
        /// <returns>제곱된 수</returns>
        public static double? Square(this double? x) {
            if(x == null)
                return null;
            return x * x;
        }

        /// <summary>
        /// 제곱
        /// </summary>
        /// <param name="x">제곱할 수</param>
        /// <returns>제곱된 수</returns>
        public static float Square(this float x) {
            return x * x;
        }

        /// <summary>
        /// 제곱
        /// </summary>
        /// <param name="x">제곱할 수</param>
        /// <returns>제곱된 수</returns>
        public static float? Square(this float? x) {
            if(x == null)
                return null;
            return x * x;
        }

        /// <summary>
        /// 제곱
        /// </summary>
        /// <param name="x">제곱할 수</param>
        /// <returns>제곱된 수</returns>
        public static decimal Square(this decimal x) {
            return x * x;
        }

        /// <summary>
        /// 제곱
        /// </summary>
        /// <param name="x">제곱할 수</param>
        /// <returns>제곱된 수</returns>
        public static decimal? Square(this decimal? x) {
            if(x == null)
                return null;
            return x * x;
        }

        /// <summary>
        /// 제곱
        /// </summary>
        /// <param name="x">제곱할 수</param>
        /// <returns>제곱된 수</returns>
        public static long Square(this long x) {
            return x * x;
        }

        /// <summary>
        /// 제곱
        /// </summary>
        /// <param name="x">제곱할 수</param>
        /// <returns>제곱된 수</returns>
        public static long? Square(this long? x) {
            if(x == null)
                return null;

            return x * x;
        }

        /// <summary>
        /// 제곱
        /// </summary>
        /// <param name="x">제곱할 수</param>
        /// <returns>제곱된 수</returns>
        public static int Square(this int x) {
            return x * x;
        }

        /// <summary>
        /// 제곱
        /// </summary>
        /// <param name="x">제곱할 수</param>
        /// <returns>제곱된 수</returns>
        public static int? Square(this int? x) {
            if(x == null)
                return null;
            return x * x;
        }
    }
}