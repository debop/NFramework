using System;
using System.Globalization;

namespace NSoft.NFramework.Tools {
    /// <summary>
    /// <see cref="CultureInfo"/>에 대한 Extension Methods
    /// </summary>
    public static class CultureTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public const string FileSortableDateTimePattern = @"yyyyMMdd'T'HHmmss";
        // public const string FileSortableDateTimeWithMillisecondPattern = @"yyyyMMdd'T'HHmmss.zzz";

        /// <summary>
        /// 현재 문화권의 <see cref="Calendar"/> 정보입니다.
        /// </summary>
        public static Calendar CurrentCalendar {
            get { return CultureInfo.CurrentCulture.Calendar; }
        }

        /// <summary>
        /// 현재 문화권의 <see cref="DateTimeFormatInfo"/> 입니다.
        /// </summary>
        public static DateTimeFormatInfo CurrentDateFormat {
            get { return CultureInfo.CurrentCulture.DateTimeFormat; }
        }

        /// <summary>
        /// 지정된 DateTime 값을 Pattern을 적용하여, 문자열로 반환한다.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="dateTimePattern"></param>
        /// <returns></returns>
        /// <seealso cref="DateTimeFormatInfo"/>
        public static string ToString(this DateTime date, string dateTimePattern) {
            return date.ToString(dateTimePattern);
        }

        /// <summary>
        /// DateTime 값을 long date, long time 이 모두 들어간 문자열로 변환합니다.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToFullDateTimeString(this DateTime date) {
            return date.ToString(CurrentDateFormat.FullDateTimePattern);
        }

        /// <summary>
        ///  DateTime 값을 정렬이 가능한 문자열로 변환합니다. (예: 2009-10-14T13:15:00, 2009-10-14 13:15:00) 
        /// </summary>
        /// <param name="date"></param>
        /// <param name="excludeDateTimeSeparator">true면 'T' 자를 없애줍니다.</param>
        /// <returns></returns>
        /// <see cref="DateTimeFormatInfo.SortableDateTimePattern"/>
        public static string ToSortableString(this DateTime date, bool excludeDateTimeSeparator = false) {
            var dateStr = date.ToString(CurrentDateFormat.SortableDateTimePattern);

            if(excludeDateTimeSeparator)
                return dateStr.Replace('T', ' ');

            return dateStr;
        }

        /// <summary>
        /// 정렬가능한 문자열로 변환된 DateTime을 파싱하여 DateTime으로 다시 변환합니다.
        /// </summary>
        /// <param name="dateStr"></param>
        /// <returns></returns>
        public static DateTime FromSortableString(this string dateStr) {
            string str = dateStr;
            var hasTimeSeparator = str.Contains("T");

            if(hasTimeSeparator == false)
                str = dateStr.Replace(" ", "T");

            return DateTime.ParseExact(str, CurrentDateFormat.SortableDateTimePattern, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// 지정한 DateTime 을 정렬가능한 문자열로 표현합니다. (형식: "yyyyMMdd'T'HHmmss")
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToFileSortableString(this DateTime date) {
            return date.ToString(FileSortableDateTimePattern, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// DateTime을 정렬 가능한 문자열로 표현된 정보를 역으로 DateTime 수형으로 파싱합니다. (형식: "yyyyMMdd'T'HHmmss")
        /// </summary>
        /// <param name="dateStr"></param>
        /// <returns></returns>
        public static DateTime FromFileSortableString(this string dateStr) {
            return DateTime.ParseExact(dateStr, FileSortableDateTimePattern, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// 날짜를 RFC1123 형식의 문자열로 반환합니다.	( "ddd, dd MMM yyyy HH':'mm':'ss 'GMT'" )
        /// </summary>
        /// <see cref="DateTimeFormatInfo.RFC1123Pattern"/>
        public static string ToRFC1123String(this DateTime date) {
            return date.ToString(CurrentDateFormat.RFC1123Pattern);
        }

        /// <summary>
        /// 지정된 문자열을 현재 문화권의 표시 방식으로 월, 일만으로 문자열을 만듭니다.
        /// </summary>
        public static string ToMonthDayString(this DateTime date) {
            return date.ToString(CurrentDateFormat.MonthDayPattern);
        }

        /// <summary>
        /// 지정된 문자열을 현재 문화권의 년/월 표시 방식으로 문자열을 만듭니다.
        /// </summary>
        public static string ToYearMonthString(this DateTime date) {
            return date.ToString(CurrentDateFormat.YearMonthPattern);
        }

        /// <summary>
        /// 지정된 문자열을 현재 문화권의 년/월 표시 방식으로 문자열을 만듭니다. (예: 2009-10-14 13:15:00Z) 마지막에 'Z' 가 붙는다
        /// </summary>
        /// <see cref="DateTimeFormatInfo.UniversalSortableDateTimePattern"/>
        public static string ToUniversalSortableDateTimeString(this DateTime date) {
            return date.ToString(CurrentDateFormat.UniversalSortableDateTimePattern);
        }

        /// <summary>
        /// 일자의 요일명의 축약어를 반환한다.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string GetAbbreviatedDayName(this DateTime date) {
            return CurrentDateFormat.GetAbbreviatedDayName(date.DayOfWeek);
        }

        /// <summary>
        /// 현재 문화권에 해당하는 요일명을 가져온다.
        /// </summary>
        /// <param name="date"></param>
        /// <returns>현재 문화권의 요일명 (예: 월, 화 .. Monday...)</returns>
        public static string GetDayName(this DateTime date) {
            return CurrentDateFormat.GetDayName(date.DayOfWeek);
        }

#if !SILVERLIGHT
        /// <summary>
        /// 지정된 일자의 최대 축약 요일명을 가져옵니다. 
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string GetShortestDayName(this DateTime date) {
            return CurrentDateFormat.GetShortestDayName(date.DayOfWeek);
        }
#endif

        /// <summary>
        /// 현재 문화권에서의 월의 이름의 축약어를 반환합니다.
        /// </summary>
        public static string GetAbbreivatedMonthName(this DateTime date) {
            return CurrentDateFormat.GetAbbreviatedMonthName(date.Month);
        }

        /// <summary>
        /// 현재 문화권에서의 월의 이름의 축약어를 반환합니다.
        /// </summary>
        public static string GetAbbreivatedMonthName(this int month) {
            return CurrentDateFormat.GetAbbreviatedMonthName(month);
        }

        /// <summary>
        /// 지정된 일자의 월의 이름을 반환한다.
        /// </summary>
        /// <param name="date">Month Name을 얻고자하는 일자</param>
        /// <returns>현재 문화권에서의 Month Name</returns>
        public static string GetMonthName(this DateTime date) {
            return date.Month.GetMonthName();
        }

        /// <summary>
        /// 지정된 일자의 월의 이름을 반환한다.
        /// </summary>
        /// <param name="month">월 (1 ~ 12)</param>
        /// <returns>현재 문화권에서의 Month Name</returns>
        public static string GetMonthName(this int month) {
            return CurrentDateFormat.GetMonthName(month);
        }

        /// <summary>
        /// 지정된 Culture 객체가 null 이거나 Name 속성 값이 빈문자열이라면 참을 반환한다.
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static bool IsNullCulture(this CultureInfo culture) {
            return (culture == null || culture.Name.IsWhiteSpace());
        }

        /// <summary>
        /// 지정된 Culture가 null이라면 <see cref="CultureInfo.CurrentUICulture"/>를 반환한다.
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static CultureInfo GetOrCurrentUICulture(this CultureInfo culture) {
            return GetCulture(culture, CultureInfo.CurrentUICulture);
        }

        /// <summary>
        /// 지정된 Culture가 null이라면 <see cref="CultureInfo.CurrentCulture"/>를 반환한다.
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static CultureInfo GetOrCurrentCulture(this CultureInfo culture) {
            return GetCulture(culture, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// 지정된 Culture가 null이라면 <see cref="CultureInfo.InvariantCulture"/>를 반환한다.
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static CultureInfo GetOrInvaliant(this CultureInfo culture) {
            return GetCulture(culture, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// 지정된 Culture가 null이라면, 지정된 <paramref name="defaultCulture"/>를 반환한다.
        /// </summary>
        /// <param name="culture"></param>
        /// <param name="defaultCulture"></param>
        /// <returns></returns>
        public static CultureInfo GetCulture(this CultureInfo culture, CultureInfo defaultCulture = null) {
            // if (culture == null)
            return culture.IsNullCulture()
                       ? (defaultCulture ?? CultureInfo.CurrentCulture)
                       : culture;
        }

        public static CultureInfo GetUICulture(this CultureInfo culture, CultureInfo uiCulture = null) {
            return culture.IsNullCulture()
                       ? (uiCulture ?? CultureInfo.CurrentUICulture)
                       : culture;
        }
    }
}