namespace NSoft.NFramework.Tools {
    /// <summary>
    /// Boolean 값에 대한 Extension Methods 입니다.
    /// </summary>
    public static class BooleanTool {
        /// <summary>
        /// Yes
        /// </summary>
        public const string YES = @"Yes";

        /// <summary>
        /// No
        /// </summary>
        public const string NO = @"No";

        /// <summary>
        /// True
        /// </summary>
        public const string True = "True";

        /// <summary>
        /// False
        /// </summary>
        public const string False = "False";

        /// <summary>
        /// Boolean 값을 Yes / No 문자열로 변환한다.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToYesNo(this bool value) {
            return (value) ? YES : NO;
        }

        /// <summary>
        /// Boolean 값을 True / False 문자열로 변환한다.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToTrueFalse(this bool value) {
            return (value) ? True : False;
        }

        /// <summary>
        /// Boolean 값을 True : 1, False : 0 값으로 반환한다.
        /// </summary>
        public static int ToInt32(this bool value) {
            return value.GetHashCode();
        }

        public static bool AsBoolean(this string str) {
            if(str == null)
                return false;

            if(str == False || str == NO)
                return false;

            var s = str.ToUpper();
            if(s == "F" || s == "N" || s == "0")
                return false;

            return true;
        }

        public static bool AsBoolean(this short value) {
            return (value != 0);
        }

        public static bool AsBoolean(this int value) {
            return (value != 0);
        }

        public static bool AsBoolean(this long value) {
            return (value != 0L);
        }
    }
}