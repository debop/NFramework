using System;
using System.Globalization;

namespace NSoft.NFramework.TimePeriods {
    /// <summary>
    /// 기간에 대한 계산 메소드를 제공합니다.
    /// </summary>
    public static class DurationUtil {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// 현 Thread Context의 Calendar
        /// </summary>
        public static Calendar CurrentCalendar {
            get { return DateTimeFormatInfo.CurrentInfo.Calendar; }
        }

        /// <summary>
        /// 0의 시간 간격
        /// </summary>
        public static TimeSpan Zero {
            get { return TimeSpan.Zero; }
        }

        /// <summary>
        /// 지정된 문화권의 달력 기준으로 1년이라는 기간 (윤달, 이슬람 달력 등등 다 다르지요)
        /// </summary>
        /// <param name="calendar"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public static TimeSpan Year(int year, Calendar calendar = null) {
            return Days((calendar ?? CurrentCalendar).GetDaysInYear(year));
        }

        /// <summary>
        /// 지정한 달력 기준으로 지정된 년도의 반기(전반기/후반기)의 기간
        /// </summary>
        public static TimeSpan Halfyear(int year, HalfyearKind halfyear = HalfyearKind.First, Calendar calendar = null) {
            calendar = calendar ?? CurrentCalendar;
            var halfyearMonths = TimeTool.GetMonthsOfHalfyear(halfyear);
            var duration = TimeSpec.NoDuration;

            foreach(var halfyearMonth in halfyearMonths)
                duration = duration.Add(Month(year, halfyearMonth, calendar));

            return duration;
        }

        /// <summary>
        /// 지정한 달력 기준으로 지정년도, 분기의 기간을 구합니다.
        /// </summary>
        /// <param name="calendar"></param>
        /// <param name="year"></param>
        /// <param name="quarter"></param>
        /// <returns></returns>
        public static TimeSpan Quarter(int year, QuarterKind quarter = QuarterKind.First, Calendar calendar = null) {
            calendar = calendar ?? CurrentCalendar;
            var quarterMonths = TimeTool.GetMonthsOfQuarter(quarter);
            var duration = TimeSpec.NoDuration;

            foreach(var quarterMonth in quarterMonths)
                duration = duration.Add(Month(year, quarterMonth, calendar));

            return duration;
        }

        /// <summary>
        /// 지정한 칼렌다에 의해 년, 월에 해당하는 일 수를 기간(TimeSpan)으로 반환합니다.
        /// </summary>
        /// <param name="calendar"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public static TimeSpan Month(int year, int month = TimeSpec.CalendarYearStartMonth, Calendar calendar = null) {
            calendar = calendar ?? CurrentCalendar;
            return Days(calendar.GetDaysInMonth(year, month));
        }

        /// <summary>
        /// 1주일의 시간 간격
        /// </summary>
        public static TimeSpan Week = Weeks(1);

        /// <summary>
        /// 지정된 Week 수를 TimeSpan으로 표현합니다.
        /// </summary>
        public static TimeSpan Weeks(int weeks = 0) {
            return Days(weeks * TimeSpec.DaysPerWeek);
        }

        /// <summary>
        /// 1일의 시간 간격
        /// </summary>
        public static TimeSpan Day = Days(1);

        /// <summary>
        /// 지정한 일, 시, 분, 초, 밀리초에 의한 기간 (TimeSpan)을 반환합니다.
        /// </summary>
        public static TimeSpan Days(int days = 0, int hours = 0, int minutes = 0, int seconds = 0, int milliseconds = 0) {
            return new TimeSpan(days, hours, minutes, seconds, milliseconds);
        }

        /// <summary>
        /// 1시간의 시간 간격
        /// </summary>
        public static TimeSpan Hour = Hours(1);

        /// <summary>
        /// 지정한 시간, 분, 초, 밀리초 만큼의 시간 간격을 생성합니다.
        /// </summary>
        /// <param name="hours">시</param>
        /// <param name="minutes">분</param>
        /// <param name="seconds">초</param>
        /// <param name="milliseconds">밀리초</param>
        /// <returns>시간 간격</returns>
        public static TimeSpan Hours(int hours = 0, int minutes = 0, int seconds = 0, int milliseconds = 0) {
            return new TimeSpan(0, hours, minutes, seconds, milliseconds);
        }

        /// <summary>
        /// 1분의 시간 간격
        /// </summary>
        public static TimeSpan Minute = Minutes(1);

        /// <summary>
        /// 지정한 분, 초, 밀리초 만큼의 시간 간격을 생성합니다.
        /// </summary>
        /// <param name="minutes">분</param>
        /// <param name="seconds">초</param>
        /// <param name="milliseconds">밀리초</param>
        /// <returns>시간 간격</returns>
        public static TimeSpan Minutes(int minutes = 0, int seconds = 0, int milliseconds = 0) {
            return new TimeSpan(0, 0, minutes, seconds, milliseconds);
        }

        public static TimeSpan Second = Seconds(1);

        /// <summary>
        /// 지정한 초, 밀리초 만큼의 시간 간격을 생성합니다.
        /// </summary>
        /// <param name="seconds">초</param>
        /// <param name="milliseconds">밀리초</param>
        /// <returns>시간 간격</returns>
        public static TimeSpan Seconds(int seconds = 0, int milliseconds = 0) {
            return new TimeSpan(0, 0, 0, seconds, milliseconds);
        }

        /// <summary>
        /// 1 밀리초의 시간 간격
        /// </summary>
        public static TimeSpan Millisecond = Milliseconds(1);

        /// <summary>
        /// 지정한 밀리초 만큼의 시간 간격을 생성합니다.
        /// </summary>
        /// <param name="milliseconds">밀리초</param>
        /// <returns>시간 간격</returns>
        public static TimeSpan Milliseconds(int milliseconds = 0) {
            return new TimeSpan(0, 0, 0, 0, milliseconds);
        }
    }
}