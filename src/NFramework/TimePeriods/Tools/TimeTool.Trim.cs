using System;

namespace NSoft.NFramework.TimePeriods {
    public static partial class TimeTool {
        /// <summary>
        /// Year 하위 단위를 시작값으로 설정합니다. 즉 지정한 날짜가 속한 년도의 첫번째 날짜 (<see cref="StartTimeOfYear(System.DateTime)"/> 와 같음)
        /// </summary>
        /// <param name="moment"></param>
        /// <returns></returns>
        public static DateTime TrimToYear(this DateTime moment) {
            return new DateTime(moment.Year, 1, 1);
        }

        /// <summary>
        /// <paramref name="moment"/>가 속한 년도에 <paramref name="month"/>월의 첫번째 날을 반환합니다.
        /// </summary>
        /// <param name="moment"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public static DateTime TrimToMonth(this DateTime moment, int month = TimeSpec.CalendarYearStartMonth) {
            // month.ShouldBeInRange(1, TimeSpec.MonthsPerYear + 1, "month");
            return new DateTime(moment.Year, month, 1);
        }

        /// <summary>
        /// <paramref name="moment"/>가 속한 년, 월에 <paramref name="day"/> 일의 날짜를 반환합니다.
        /// </summary>
        /// <param name="moment">기준 일자</param>
        /// <param name="day">1 이상 해당 년/월의 최대일 이하여야 합니다.</param>
        public static DateTime TrimToDay(this DateTime moment, int day = 1) {
            var maxDay = GetDaysInMonth(moment.Year, moment.Month);
            // day.ShouldBeInRange(1, maxDay + 1, "day");
            return new DateTime(moment.Year, moment.Month, day);
        }

        /// <summary>
        /// <paramref name="moment"/>가 속한 날짜에서 <paramref name="hour"/> 시간을 정하고, 그 이하는 0 으로 한다.
        /// </summary>
        public static DateTime TrimToHour(this DateTime moment, int hour = 0) {
            // hour.ShouldBeInRange(0, TimeSpec.HoursPerDay, "hour");
            return moment.Date.AddHours(hour);
        }

        /// <summary>
        /// <paramref name="moment"/>가 속한 날짜에서 <paramref name="minute"/> 분을 정하고, 그 이하는 0 으로 한다.
        /// </summary>
        public static DateTime TrimToMinute(this DateTime moment, int minute = 0) {
            // minute.ShouldBeInRange(0, TimeSpec.MinutesPerHour, "minute");
            return new DateTime(moment.Year, moment.Month, moment.Day, moment.Hour, minute, 0);
        }

        /// <summary>
        /// <paramref name="moment"/>가 속한 날짜에서 <paramref name="second"/> 초을 정하고, 그 이하는 0 으로 한다.
        /// </summary>
        public static DateTime TrimToSecond(this DateTime moment, int second = 0) {
            // second.ShouldBeInRange(0, TimeSpec.SecondsPerMinute, "second");
            return new DateTime(moment.Year, moment.Month, moment.Day, moment.Hour, moment.Minute, second);
        }

        /// <summary>
        /// <paramref name="moment"/>가 속한 날짜에서 <paramref name="millisecond"/> 초을 정한다.
        /// </summary>
        public static DateTime TrimToMillisecond(this DateTime moment, int millisecond = 0) {
            // millisecond.ShouldBeInRange(0, TimeSpec.MillisecondsPerSecond, "millisecond");
            return new DateTime(moment.Year, moment.Month, moment.Day, moment.Hour, moment.Minute, moment.Second, millisecond);
        }
    }
}