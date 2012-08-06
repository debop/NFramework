namespace NSoft.NFramework.Numerics {
    public static partial class MathTool {
        /// <summary>
        /// 지정된 두 변수의 값을 바꾼다.
        /// </summary>
        /// <typeparam name="T">변수의 수형</typeparam>
        /// <param name="a">첫번째 변수</param>
        /// <param name="b">두번째 변수</param>
        public static void Swap<T>(ref T a, ref T b) {
            T temp = a;
            a = b;
            b = temp;
        }

        /// <summary>
        /// 지정된 두 변수의 값을 바꾼다.
        /// </summary>
        /// <param name="a">첫번째 변수</param>
        /// <param name="b">두번째 변수</param>
        public static void Swap(ref double a, ref double b) {
            double t = a;
            a = b;
            b = t;
        }

        /// <summary>
        /// 지정된 두 변수의 값을 바꾼다.
        /// </summary>
        /// <param name="a">첫번째 변수</param>
        /// <param name="b">두번째 변수</param>
        public static void Swap(ref float a, ref float b) {
            float t = a;
            a = b;
            b = t;
        }

        /// <summary>
        /// 지정된 두 변수의 값을 바꾼다.
        /// </summary>
        /// <param name="a">첫번째 변수</param>
        /// <param name="b">두번째 변수</param>
        public static void Swap(ref decimal a, ref decimal b) {
            decimal t = a;
            a = b;
            b = t;
        }

        /// <summary>
        /// 지정된 두 변수의 값을 바꾼다.
        /// </summary>
        /// <param name="a">첫번째 변수</param>
        /// <param name="b">두번째 변수</param>
        public static void Swap(ref byte a, ref byte b) {
            byte t = a;
            a = b;
            b = t;
        }

        /// <summary>
        /// 지정된 두 변수의 값을 바꾼다.
        /// </summary>
        /// <param name="a">첫번째 변수</param>
        /// <param name="b">두번째 변수</param>
        public static void Swap(ref char a, ref char b) {
            char t = a;
            a = b;
            b = t;
        }

        /// <summary>
        /// 지정된 두 변수의 값을 바꾼다.
        /// </summary>
        /// <param name="a">첫번째 변수</param>
        /// <param name="b">두번째 변수</param>
        public static void Swap(ref short a, ref short b) {
            short t = a;
            a = b;
            b = t;
        }

        /// <summary>
        /// 지정된 두 변수의 값을 바꾼다.
        /// </summary>
        /// <param name="a">첫번째 변수</param>
        /// <param name="b">두번째 변수</param>
        public static void Swap(ref int a, ref int b) {
            int t = a;
            a = b;
            b = t;
        }

        /// <summary>
        /// 지정된 두 변수의 값을 바꾼다.
        /// </summary>
        /// <param name="a">첫번째 변수</param>
        /// <param name="b">두번째 변수</param>
        public static void Swap(ref long a, ref long b) {
            long t = a;
            a = b;
            b = t;
        }
    }
}