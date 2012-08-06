using System;

namespace NSoft.NFramework.Numerics {
    public static partial class MathTool {
        /// <summary>
        /// 지정된 수가 소수인지 검사한다.
        /// </summary>
        /// <param name="value">검사할 값</param>
        /// <returns>소수인지 여부</returns>
        public static bool IsPrimes(this long value) {
            var sqrtValue = (long)Math.Sqrt(value);

            for(long i = 2; i < sqrtValue; i++)
                if((value % i) == 0)
                    return false;

            return true;
        }

        /// <summary>
        /// 지정된 수가 소수인지 검사한다.
        /// </summary>
        /// <param name="value">검사할 값</param>
        /// <returns>소수인지 여부</returns>
        public static bool IsPrimes(this int value) {
            var sqrtValue = (int)Math.Sqrt(value);

            for(var i = 2; i < sqrtValue; i++)
                if((value % i) == 0)
                    return false;

            return true;
        }
    }
}