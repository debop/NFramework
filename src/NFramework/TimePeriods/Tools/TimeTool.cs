using System;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.TimePeriods {
    /// <summary>
    /// Time, TimePeriod 관련한 Utility 메소드를 제공합니다.
    /// </summary>
    public static partial class TimeTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// <paramref name="period"/>를 문자열로 표현합니다.
        /// </summary>
        /// <param name="period"></param>
        /// <returns></returns>
        public static string AsString(this ITimePeriod period) {
            return (period == null) ? NullString : period.GetDescription(TimeFormatter.Instance);
        }

        /// <summary>
        /// DateTime 형식으로 변환한다. 실패시에는 new DateTime(0) 를 반환한다.
        /// </summary>
        /// <param name="value">날짜를 나타내는 문자열</param>
        /// <returns>변환된 DateTime 인스턴스 개체, 실패시에는 Ticks가 0인 DateTime을 반환한다.</returns>
        public static DateTime ToDateTime(this string value) {
            return ToDateTime(value, () => (DateTime)typeof(DateTime).GetTypeDefaultValue());
        }

        /// <summary>
        /// DateTime 형식으로 변환한다.
        /// </summary>
        /// <param name="value">날짜를 나타내는 문자열</param>
        /// <param name="defaultValue">변환 실패시의 기본값</param>
        /// <returns>변환된 DateTime 인스턴스 개체</returns>
        public static DateTime ToDateTime(this string value, DateTime defaultValue) {
            return ToDateTime(value, () => defaultValue);
        }

        /// <summary>
        /// <paramref name="value"/>를 DateTime 수형으로 변환합니다.
        /// </summary>
        /// <param name="value">변환할 문자열</param>
        /// <param name="defaultValueFactory">변환 실패 시 반환할 값을 제공하는 Factory</param>
        /// <returns>변환된 DateTime 인스턴스 개체</returns>
        public static DateTime ToDateTime(this string value, Func<DateTime> defaultValueFactory) {
            return ConvertTool.AsDateTime(value, defaultValueFactory);
        }
    }
}