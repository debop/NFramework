namespace NSoft.NFramework.Numerics {
    public static partial class MathTool {
        /// <summary>
        /// 각도의 Degree 단위를 Radian 단위로 변경 ( 180 degree => PI )
        /// </summary>
        /// <param name="degree">degree 단위의 각도</param>
        /// <returns>radian 단위의 각도</returns>
        public static double ToRadian(this double degree) {
            return degree * MathTool.Pi / 180.0;
        }

        /// <summary>
        /// 각도의 Degree 단위를 Radian 단위로 변경 ( 180 degree => PI )
        /// </summary>
        /// <param name="degree">degree 단위의 각도</param>
        /// <returns>radian 단위의 각도</returns>
        public static float ToRadian(this float degree) {
            return degree * (float)MathTool.Pi / 180.0f;
        }

        /// <summary>
        /// 각도의 Degree 단위를 Radian 단위로 변경 ( 180 degree => PI )
        /// </summary>
        /// <param name="degree">degree 단위의 각도</param>
        /// <returns>radian 단위의 각도</returns>
        public static decimal ToRadian(this decimal degree) {
            return degree * (decimal)MathTool.Pi / 180.0m;
        }

        /// <summary>
        /// 각도의 Radian 단위를 Degree 단위로 변경 ( PI => 180 )
        /// </summary>
        /// <param name="radian">radian 단위의 각도</param>
        /// <returns>degree 단위의 각도</returns>
        public static double ToDegree(this double radian) {
            return radian * 180.0 / MathTool.Pi;
        }

        /// <summary>
        /// 각도의 Radian 단위를 Degree 단위로 변경 ( PI => 180 )
        /// </summary>
        /// <param name="radian">radian 단위의 각도</param>
        /// <returns>degree 단위의 각도</returns>
        public static float ToDegree(this float radian) {
            return radian * 180.0f / (float)MathTool.Pi;
        }

        /// <summary>
        /// 각도의 Radian 단위를 Degree 단위로 변경 ( PI => 180 )
        /// </summary>
        /// <param name="radian">radian 단위의 각도</param>
        /// <returns>degree 단위의 각도</returns>
        public static decimal ToDegree(this decimal radian) {
            return radian * 180.0m / (decimal)MathTool.Pi;
        }
    }
}