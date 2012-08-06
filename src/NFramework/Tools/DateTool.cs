using System;
using NSoft.NFramework.TimePeriods;

namespace NSoft.NFramework.Tools {
    public static class DateTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// Timestamp 값이 1970-01-01 부터의 Tick 값을 표현한 것이다.
        /// </summary>
        public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        /// <summary>
        /// 해당 분기의 시작월
        /// </summary>
        /// <param name="quarter">해당 분기</param>
        /// <returns>해당분기의 시작월</returns>
        public static int GetStartMonthOfQuarter(this Quarters quarter) {
            return 3 * (quarter.GetHashCode() - 1) + 1;
        }

        /// <summary>
        /// 해당 분기의 마지막 월
        /// </summary>
        /// <param name="quarter">해당 분기</param>
        /// <returns>해당 분기의 마지막 월</returns>
        public static int GetEndMonthOfQuarter(this Quarters quarter) {
            return 3 * quarter.GetHashCode();
        }

        /// <summary>
        /// 현재 분기 첫번째 일자(<see cref="DateTime"/>
        /// </summary>
        /// <param name="quarter">해당 분기</param>
        /// <returns>특정 분기의 첫번째 DateTime <see cref="DateTime"/> (2007-04-01 00:00:00:000)</returns>
        public static DateTime GetStartOfQuarter(this Quarters quarter) {
            return quarter.GetStartOfQuarter(DateTime.Now.Year);
        }

        /// <summary>
        /// 해당 년도의 분기 첫번째 일자(<see cref="DateTime"/>
        /// </summary>
        /// <param name="year">해당 년도</param>
        /// <param name="quarter">해당 분기</param>
        /// <returns>특정 분기의 첫번째 DateTime <see cref="DateTime"/> (2007-04-01 00:00:00:000)</returns>
        public static DateTime GetStartOfQuarter(this Quarters quarter, int year) {
            return new DateTime(year, GetStartMonthOfQuarter(quarter), 1, 0, 0, 0, 0);
        }

        /// <summary>
        /// 현재 년도의 지정된 분기의 마지막 시각 (3월 31일 23:59:59:999)
        /// </summary>
        /// <param name="quarter">quarter</param>
        /// <returns>해당 분기의 마지막 <see cref="DateTime"/> (2007-03-31 23:59:59:999)</returns>
        public static DateTime GetEndOfQuarter(this Quarters quarter) {
            return quarter.GetEndOfQuarter(DateTime.Now.Year);
        }

        /// <summary>
        /// 해당년도의 해당 분기 마지막 시각 (3월 31일 23:59:59:999)
        /// </summary>
        /// <param name="quarter">quarter</param>
        /// <param name="year">year</param>
        /// <returns>해당 분기의 마지막 <see cref="DateTime"/> (2007-03-31 23:59:59:999)</returns>
        public static DateTime GetEndOfQuarter(this Quarters quarter, int year) {
            int endMonth = GetEndMonthOfQuarter(quarter);
            return new DateTime(year, endMonth, DateTime.DaysInMonth(year, endMonth), 23, 59, 59, 999);
        }

        /// <summary>
        /// 지정된 월이 속한 분기
        /// </summary>
        /// <param name="month">조회할 월</param>
        /// <returns>월이 속한 분기</returns>
        public static Quarters GetQuarter(this int month) {
            // Bug Fix at 2009-09-09

            int quarter = (month - 1) / 3 + 1;
            return ConvertTool.ConvertEnum(quarter, Quarters.First);
        }

        /// <summary>
        /// 지정된 날짜의 분기를 구한다.
        /// </summary>
        /// <param name="date">날짜</param>
        /// <returns>날짜가 속한 분기</returns>
        public static Quarters GetQuarter(this DateTime date) {
            return GetQuarter(date.Month);
        }

        /// <summary>
        /// 현재 날짜 기준의 분기 바로 전의 분기를 구한다.
        /// </summary>
        /// <returns></returns>
        public static Quarters GetLastQuarter() {
            return DateTime.Now.GetLastQuarter();
        }

        /// <summary>
        /// 지정된 날짜의 분기의 앞의 분기를 구한다.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static Quarters GetLastQuarter(this DateTime date) {
            return GetQuarter(date.AddMonths(-3).Month);
        }

        /// <summary>
        /// 전(前) 분기의 시작 시각
        /// </summary>
        /// <returns></returns>
        public static DateTime GetStartOfLastquarter() {
            return GetLastQuarter().GetStartOfQuarter();
        }

        /// <summary>
        /// 전(前) 분기의 마지막 시각
        /// </summary>
        /// <returns></returns>
        public static DateTime GetEndOfLastquarter() {
            return GetLastQuarter().GetEndOfQuarter(DateTime.Now.Year);
        }

        /// <summary>
        /// 현재 분기의 시작 시각
        /// </summary>
        /// <returns></returns>
        public static DateTime GetStartOfCurrentQuarter() {
            return DateTime.Now.GetQuarter().GetStartOfQuarter();
        }

        /// <summary>
        /// 현재 분기의 마지막 시각
        /// </summary>
        /// <returns></returns>
        public static DateTime GetEndOfCurrentQuarter() {
            return GetQuarter(DateTime.Now.Month).GetEndOfQuarter(DateTime.Now.Year);
        }

        /// <summary>
        /// 전(前) 주(week)의 시작 시각
        /// </summary>
        /// <returns></returns>
        public static DateTime GetStartOfLastWeek() {
            var current = DateTime.Now;
            int daysToSubtract = (int)current.DayOfWeek + 7;
            var dt = current.Subtract(TimeSpan.FromDays(daysToSubtract));

            return GetStartOfDay(dt);
        }

        /// <summary>
        /// 전(前) 주의 마지막 시각
        /// </summary>
        /// <returns></returns>
        public static DateTime GetEndOfLastWeek() {
            var dt = GetStartOfLastWeek().AddDays(7);
            return dt.AddMilliseconds(-1);
        }

        /// <summary>
        /// 금주의 시작 시각
        /// </summary>
        /// <returns></returns>
        public static DateTime GetStartOfCurrentWeek() {
            var current = DateTime.Now;
            var dt = current.Subtract(TimeSpan.FromDays((double)current.DayOfWeek));

            return GetStartOfDay(dt);
        }

        /// <summary>
        /// 금주의 마지막 시각
        /// </summary>
        /// <returns></returns>
        public static DateTime GetEndOfCurrentWeek() {
            return DateTime.Now.GetEndOfWeek();
            //DateTime dt = GetStartOfCurrentWeek().AddDays(7);
            //return dt.AddMilliseconds(-1);
        }

        /// <summary>
        /// 지정된 날짜가 속한 주(week)의 첫번째 요일 (한국:일요일, ISO8601:월요일)의 날짜
        /// </summary>
        /// <param name="time">일자</param>
        /// <returns>지정된 일자가 속한 주의 첫번째 요일의 일자를 반환한다. 문화권에 따라 한주의 첫번째 요일은 다르다. 한국은 Sunday, ISO 9601로는 Monday이다.</returns>
        public static DateTime GetStartOfWeek(this DateTime time) {
            // return time.Date.AddDays(DayOfWeek.Sunday - time.DayOfWeek).Date;
            var firstDayOfWeek = WeekTool.GetFirstDayOfWeek();
            return time.GetStartOfWeek(firstDayOfWeek);
        }

        /// <summary>
        /// 지정된 날짜가 속한 주(week)의 첫번째 요일 (일요일)의 날짜
        /// </summary>
        /// <param name="time">일자</param>
        /// <param name="firstDayOfWeek">한 주의 첫번째 요일</param>
        /// <returns>지정된 일자가 속한 주의 첫번째 요일의 일자를 반환한다. 문화권에 따라 한주의 첫번째 요일은 다르다. 한국은 Sunday, ISO 9601로는 Monday이다.</returns>
        public static DateTime GetStartOfWeek(this DateTime time, DayOfWeek firstDayOfWeek) {
            var add = firstDayOfWeek.GetHashCode() - time.DayOfWeek.GetHashCode();
            if(add > 0)
                add -= 7;
            return time.Date.AddDays(add).Date;
            // return time.Date.AddDays(firstDayOfWeek.GetHashCode() - time.DayOfWeek.GetHashCode()).Date;
        }

        /// <summary>
        /// 지정된 날짜가 속한 주(week)의 마지막 날짜 (한국:토요일, ISO8601:일요일)
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime GetEndOfWeek(this DateTime time) {
            // return time.Date.AddDays(DayOfWeek.Saturday - time.DayOfWeek).Date.AddDays(1).AddMilliseconds(-1);
            return time.GetStartOfWeek().AddDays(6).GetEndOfDay();
        }

        /// <summary>
        /// 지정된 날짜가 속한 주(week)의 마지막 날짜 (한국:토요일, ISO8601:일요일)
        /// </summary>
        /// <param name="time"></param>
        /// <param name="firstDayOfWeek">한 주의 첫번째 요일</param>
        /// <returns></returns>
        public static DateTime GetEndOfWeek(this DateTime time, DayOfWeek firstDayOfWeek) {
            return time.GetStartOfWeek(firstDayOfWeek).AddDays(6).GetEndOfDay();
        }

        /// <summary>
        /// 지정된 일자가 속한 월의 시작일자
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetStartOfMonth(this DateTime date) {
            return GetStartOfMonth(date.Year, date.Month);
        }

        /// <summary>
        /// 해당 년, 월의 시작 시각
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public static DateTime GetStartOfMonth(this int year, int month) {
            return new DateTime(year, month, 1);
        }

        /// <summary>
        /// 지정된 날짜가 속한 월의 마지막 시각
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetEndOfMonth(this DateTime date) {
            return GetEndOfMonth(date.Year, date.Month);
        }

        /// <summary>
        /// 해당 년, 월의 마지막 시각
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public static DateTime GetEndOfMonth(this int year, int month) {
            return GetStartOfMonth(year, month).AddMonths(1).AddMilliseconds(-1);
        }

        /// <summary>
        /// 전(前)월의 시작 시각
        /// </summary>
        /// <returns></returns>
        public static DateTime GetStartOfLastMonth() {
            var lastMonth = DateTime.Now.AddMonths(-1);
            return GetStartOfMonth(lastMonth.Year, lastMonth.Month);
        }

        /// <summary>
        /// 전(前)월의 마지막 시각
        /// </summary>
        /// <returns></returns>
        public static DateTime GetEndOfLastMonth() {
            var lastMonth = DateTime.Now.AddMonths(-1);
            return GetEndOfMonth(lastMonth.Year, lastMonth.Month);
        }

        /// <summary>
        /// 금월의 시작 시각
        /// </summary>
        /// <returns></returns>
        public static DateTime GetStartOfCurrentMonth() {
            return DateTime.Now.GetStartOfMonth();
        }

        /// <summary>
        /// 금월의 마지막 시각
        /// </summary>
        /// <returns></returns>
        public static DateTime GetEndOfCurrentMonth() {
            return DateTime.Now.GetEndOfMonth();
        }

        /// <summary>
        /// 일자의 반기를 반환합니다. (전반기 : 1, 후반기 : 2)
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static int GetHalfYear(this DateTime date) {
            return ((date.Month - 1) / 6) + 1;
        }

        /// <summary>
        /// 반기의 시작일자를 구한다. N년 1월1일, N년 7월 1일
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetStartOfHalfYear(this DateTime date) {
            return (date.GetHalfYear() == 1) ? date.GetStartOfYear() : date.GetStartOfYear().AddMonths(7);
        }

        /// <summary>
        /// 반기의 마지막 일자를 구한다. N년 6월30일, N년 12월 31일
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetEndOfHalfYear(this DateTime date) {
            return (date.GetHalfYear() == 1) ? date.GetStartOfYear().AddMonths(6) : date.GetEndOfYear();
        }

        /// <summary>
        /// 지정된 년도의 시작 시각
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public static DateTime GetStartOfYear(this int year) {
            return new DateTime(year, 1, 1, 0, 0, 0, 0);
        }

        /// <summary>
        /// 지정된 년도의 마지막 시각
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public static DateTime GetEndOfYear(this int year) {
            return new DateTime(year, 12, DateTime.DaysInMonth(year, 12), 23, 59, 59, 999);
        }

        /// <summary>
        /// 전년도 시작 시각
        /// </summary>
        /// <returns></returns>
        public static DateTime GetStartOfLastYear() {
            return GetStartOfYear(DateTime.Now.Year - 1);
        }

        /// <summary>
        /// 전년도 마지막 시각
        /// </summary>
        /// <returns></returns>
        public static DateTime GetEndOfLastYear() {
            return GetEndOfYear(DateTime.Now.Year - 1);
        }

        /// <summary>
        /// 지정된 날짜에 해당하는 년도의 첫번째 일자
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetStartOfYear(this DateTime date) {
            return GetStartOfYear(date.Year);
        }

        /// <summary>
        /// 지정된 날짜에 해당하는 년도의 마지막 일자
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetEndOfYear(this DateTime date) {
            return GetEndOfYear(date.Year);
        }

        /// <summary>
        /// 이번 년도의 첫번째 일자
        /// </summary>
        /// <returns></returns>
        public static DateTime GetStartOfCurrentYear() {
            return DateTime.Now.GetStartOfYear();
        }

        /// <summary>
        /// 이변년도의 마지막 일자
        /// </summary>
        /// <returns></returns>
        public static DateTime GetEndOfCurrentYear() {
            return DateTime.Now.GetEndOfYear();
        }

        /// <summary>
        /// 지정된 날짜의 시작 시각 (시간 파트가 0 이다.)
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetStartOfDay(this DateTime date) {
            return date.SetTimePart(0, 0, 0, 0);
        }

        /// <summary>
        /// 지정된 날짜의 마지막 시각
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetEndOfDay(this DateTime date) {
            return date.Date.AddDays(1).Add(TimeSpec.MinNegativeDuration);
            // return date.SetTime(0, 0, 0, 0).AddDays(1).AddMilliseconds(-1);
        }

        /// <summary>
        /// 현재날짜로부터 가장 가까운 지정된 요일을 반환한다.
        /// </summary>
        /// <param name="current"></param>
        /// <param name="dow"></param>
        /// <returns></returns>
        public static DateTime NextDayOfWeek(this DateTime current, DayOfWeek dow) {
            var next = current;
            do {
                next = next.AddDays(1);
            } while(next.DayOfWeek != dow);

            return next;
        }

        /// <summary>
        /// 현재날짜로 지난 날짜 중에 가장 가까운 지정된 요일을 반환한다.
        /// </summary>
        /// <param name="current"></param>
        /// <param name="dow"></param>
        /// <returns></returns>
        public static DateTime PrevDayOfWeek(this DateTime current, DayOfWeek dow) {
            var prev = current;
            do {
                prev = prev.AddDays(-1);
            } while(prev.DayOfWeek != dow);

            return prev;
        }

        /// <summary>
        /// 지정된 날짜의 년, 월, 일을 새로 설정한다. Time Part는 그대로
        /// </summary>
        /// <param name="date"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        [Obsolete("Use NSoft.NFramework.TimePeriods.TimeTool.SetDatePart instead")]
        public static DateTime SetDate(this DateTime date, int year, int month, int day) {
            return new DateTime(year, month, day).Add(date.TimeOfDay);
        }

        /// <summary>
        /// 지정된 날짜의 년, 월을 새로 설정한다. 일, Time Part는 그대로
        /// </summary>
        /// <param name="date"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        [Obsolete("Use NSoft.NFramework.TimePeriods.TimeTool.SetDatePart instead")]
        public static DateTime SetDate(this DateTime date, int year, int month) {
            return date.SetDate(year, month, date.Day);
        }

        /// <summary>
        /// DateTime의 년도만 지정된 년도로 설정한다.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        [Obsolete("Use NSoft.NFramework.TimePeriods.TimeTool.SetDatePart instead")]
        public static DateTime SetDate(this DateTime date, int year) {
            return date.SetDate(year, date.Month);
        }

        /// <summary>
        /// DateTime의 년도를 지정된 값으로 설정한다.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public static DateTime SetYear(this DateTime date, int year) {
            return date.SetDatePart(year);
        }

        /// <summary>
        /// DateTime의 월만 지정된 월로 바꾼다.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public static DateTime SetMonth(this DateTime date, int month) {
            return date.SetDatePart(date.Year, month);
        }

        /// <summary>
        /// DateTime의 일만 지정된 일로 바꾼다.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        public static DateTime SetDay(this DateTime date, int day) {
            return date.SetDatePart(date.Year, date.Month, day);
        }

        /// <summary>
        /// Date Part와 TimePart를 합한다.
        /// </summary>
        /// <param name="datePart">Date 정보만</param>
        /// <param name="timePart">Time 정보만</param>
        /// <returns></returns>
        public static DateTime Combine(this DateTime datePart, DateTime timePart) {
            return new DateTime(datePart.Date.Ticks).Add(timePart.TimeOfDay);
        }

        /// <summary>
        /// 지정된 날짜의 시간 파트를 설정한다.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="hour"></param>
        /// <returns></returns>
        [Obsolete("Use NSoft.NFramework.TimePeriods.TimeTool.SetTimePart instead")]
        public static DateTime SetTime(this DateTime date, int hour) {
            return date.SetTimePart(hour);
            // return date.SetTime(hour, date.Minute);
        }

        /// <summary>
        /// 지정된 날짜의 시간 파트를 설정한다.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <returns></returns>
        [Obsolete("Use NSoft.NFramework.TimePeriods.TimeTool.SetTimePart instead")]
        public static DateTime SetTime(this DateTime date, int hour, int minute) {
            return date.SetTimePart(hour, minute);
            // return date.SetTime(hour, minute, date.Second);
        }

        /// <summary>
        /// 지정된 날짜의 시간 파트를 설정한다.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        [Obsolete("Use NSoft.NFramework.TimePeriods.TimeTool.SetTimePart instead")]
        public static DateTime SetTime(this DateTime date, int hour, int minute, int second) {
            return date.SetTimePart(hour, minute, second);
            // return date.SetTime(hour, minute, second, date.Millisecond);
        }

        /// <summary>
        /// 날짜의 시간 파트를 설정한다.
        /// </summary>
        /// <param name="date">설정할 DateTime</param>
        /// <param name="hour">시</param>
        /// <param name="minute">분</param>
        /// <param name="second">초</param>
        /// <param name="millisecond">밀리초</param>
        /// <returns>설정된 DateTime</returns>
        [Obsolete("Use NSoft.NFramework.TimePeriods.TimeTool.SetTimePart instead")]
        public static DateTime SetTime(this DateTime date, int hour, int minute, int second, int millisecond) {
            return date.SetTimePart(hour, minute, second, millisecond);
            // return new DateTime(date.Year, date.Month, date.Day, hour, minute, second, millisecond);
        }

        /// <summary>
        /// 날짜의 시간을 설정한다.
        /// </summary>
        /// <param name="date">대상 DateTime</param>
        /// <param name="hour">시</param>
        /// <returns>설정된 DateTime</returns>
        public static DateTime SetHour(this DateTime date, int hour) {
            return date.SetTimePart(hour);
        }

        /// <summary>
        /// 날짜의 분을 설정한다.
        /// </summary>
        /// <param name="date">대상 DateTime</param>
        /// <param name="minute">분</param>
        /// <returns>설정된 DateTime</returns>
        public static DateTime SetMinute(this DateTime date, int minute) {
            return date.SetTimePart(date.Hour, minute);
        }

        /// <summary>
        ///	 날짜의 초을 설정한다.
        /// </summary>
        /// <param name="date">대상 DateTime</param>
        /// <param name="second">초</param>
        /// <returns>설정된 DateTime</returns>
        public static DateTime SetSecond(this DateTime date, int second) {
            return date.SetTimePart(date.Hour, date.Minute, second);
        }

        /// <summary>
        /// 날짜의 밀리초을 설정한다.
        /// </summary>
        /// <param name="date">대상 DateTime</param>
        /// <param name="millisecond">밀리초</param>
        /// <returns>설정된 DateTime</returns>
        public static DateTime SetMillisecond(this DateTime date, int millisecond) {
            return date.SetTimePart(date.Hour, date.Minute, date.Second, millisecond);
        }

        /// <summary>
        /// 지정된 날짜의 정오를 가르키는 시간을 만든다.
        /// </summary>
        /// <param name="date">기준 DateTime</param>
        /// <returns>지정된 날짜의 정오를 가르키는 DateTime</returns>
        public static DateTime Noon(this DateTime date) {
            return date.SetTimePart(12);
        }

        /// <summary>
        /// DateTime 형식으로 변환한다.
        /// </summary>
        /// <param name="value">날짜를 나타내는 문자열</param>
        /// <param name="defaultValue">변환 실패시의 기본값</param>
        /// <returns>변환된 DateTime 인스턴스 개체</returns>
        public static DateTime ToDateTime(this string value, DateTime defaultValue) {
            DateTime result;
            try {
                result = (value != null) ? DateTime.Parse(value) : defaultValue;
            }
            catch {
                result = defaultValue;
            }
            return result;
        }

        /// <summary>
        /// DateTime 형식으로 변환한다. 실패시에는 new DateTime(0) 를 반환한다.
        /// </summary>
        /// <param name="value">날짜를 나타내는 문자열</param>
        /// <returns>변환된 DateTime 인스턴스 개체, 실패시에는 Ticks가 0인 DateTime을 반환한다.</returns>
        public static DateTime ToDateTime(this string value) {
            return ToDateTime(value, new DateTime(0));
        }

        /// <summary>
        /// 지정된 week 수만큼의 TimeSpan을 구한다.
        /// </summary>
        /// <param name="weeks">TimeSpan값을 구할 week 수</param>
        /// <returns>지정된 week 만큼의 TimeSpan</returns>
        /// <example>
        ///	<code>
        ///		// 1주일 (7일 = 7 * 24 Hour) 기간의 TimeSpan
        ///		TimeSpan ts = 1.Weeks();
        /// </code>
        /// </example>
        public static TimeSpan Weeks(this int weeks) {
            return Days(weeks * 7);
        }

        /// <summary>
        ///	 지정된 Day 만큼의 TimeSpan을 구한다.
        /// </summary>
        /// <param name="days">TimeSpan을 구할 Day값</param>
        /// <returns>지정된 Day만큼의 TimeSpan</returns>
        public static TimeSpan Days(this int days) {
            return new TimeSpan(days, 0, 0, 0, 0);
        }

        /// <summary>
        /// 지정된 시간에 대한 TimeSpan
        /// </summary>
        /// <param name="hours"></param>
        /// <returns></returns>
        public static TimeSpan Hours(this int hours) {
            return new TimeSpan(0, hours, 0, 0, 0);
        }

        /// <summary>
        /// 지정된 분에 대한 TimeSpan
        /// </summary>
        /// <param name="minutes"></param>
        /// <returns></returns>
        public static TimeSpan Minutes(this int minutes) {
            return new TimeSpan(0, 0, minutes, 0, 0);
        }

        /// <summary>
        /// 지정된 초에 대한 TimeSpan
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static TimeSpan Seconds(this int seconds) {
            return new TimeSpan(0, 0, 0, seconds, 0);
        }

        /// <summary>
        /// 지정된 Millisecond 에 대한 TimeSpan
        /// </summary>
        /// <param name="milliseconds"></param>
        /// <returns></returns>
        public static TimeSpan Milliseconds(this int milliseconds) {
            return new TimeSpan(0, 0, 0, 0, milliseconds);
        }

        /// <summary>
        /// 지정된 일자에 TimeSpan을 뺀 날자를 반환한다.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime Ago(this TimeSpan from, DateTime date) {
            return date.Subtract(from);
        }

        /// <summary>
        /// 지정된 일자에 TimeSpan을 더한 날짜를 반환한다.
        /// </summary>
        /// <param name="from">더할 TimeSpan</param>
        /// <param name="date">대상 DateTime</param>
        /// <returns>날짜에 TimeSpan을 더한 날짜</returns>
        public static DateTime From(this TimeSpan from, DateTime date) {
            return date.Add(from);
        }

        /// <summary>
        /// 현재 날짜에 지정된 TimeSpan을 더한 날짜를 반환한다.
        /// </summary>
        /// <param name="from">더할 TimeSpan</param>
        /// <returns>현재 날짜에 TimeSpan을 더한 날짜</returns>
        public static DateTime FromNow(this TimeSpan from) {
            return from.From(DateTime.Now);
        }

        /// <summary>
        /// 지정된 일자에 TimeSpan을 더한 날짜를 반환한다. <see cref="From"/>과 같다.
        /// </summary>
        /// <param name="from">더할 TimeSpan</param>
        /// <param name="date">대상 DateTime</param>
        /// <returns>날짜에 TimeSpan을 더한 날짜</returns>
        public static DateTime Since(this TimeSpan from, DateTime date) {
            return date.Add(from);
        }

#if !SILVERLIGHT
        /// <summary>
        /// .NET DateTime 을 SQL Server DateTime 수형의 정밀도로 표현합니다. (SQL Server의 DateTime의 정밀도가 좀 낮다)
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>													
        public static DateTime ToSqlPrecision(this DateTime dt) {
            return new System.Data.SqlTypes.SqlDateTime(dt).Value;
        }
#endif
    }
}