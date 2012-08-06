using System;
using System.Globalization;

namespace NSoft.NFramework.TimePeriods {
    public static partial class TimeTool {
        /// <summary>
        /// 현재 날짜가 속한 년의 시작 일자를 나타냅니다. (올해 1월 1일)
        /// </summary>
        public static DateTime CurrentCalendarYear {
            get { return CurrentYear(TimeSpec.CalendarYearStartMonth); }
        }

        /// <summary>
        /// <paramref name="yearStartMonth"/>를 한 해의 시작월로하는 한해의 시작일자를 표현합니다.
        /// </summary>
        /// <param name="yearStartMonth"></param>
        /// <returns></returns>
        public static DateTime CurrentYear(this int yearStartMonth) {
            var now = ClockProxy.Clock.Now;
            var startMonth = yearStartMonth;
            var monthOffset = now.Month - startMonth;
            var year = monthOffset < 0 ? now.Year - 1 : now.Year;

            return new DateTime(year, startMonth, 1);
        }

        /// <summary>
        /// 현재 날짜가 속한 반기의 시작 일자를 나타냅니다.
        /// </summary>
        public static DateTime CurrentCalendarHalfyear {
            get { return CurrentHalfyear(TimeSpec.CalendarYearStartMonth); }
        }

        /// <summary>
        /// 현재 날짜가 속한 반기의 시작 일자를 나타냅니다. 단 한해의 시작 월이 <paramref name="yearStartMonth"/>을 기준으로 합니다.
        /// </summary>
        /// <param name="yearStartMonth"></param>
        /// <returns></returns>
        public static DateTime CurrentHalfyear(this int yearStartMonth) {
            var now = ClockProxy.Clock.Now;
            var year = now.Year;
            if(now.Month < yearStartMonth)
                year--;

            var halfyear = GetHalfyearOfMonth(yearStartMonth, now.Month);
            var months = ((int)halfyear - 1) * TimeSpec.MonthsPerHalfyear;

            return new DateTime(year, yearStartMonth, 1).AddMonths(months);
        }

        /// <summary>
        /// 현재 날짜가 속한 분기의 시작 일자를 나타냅니다.
        /// </summary>
        public static DateTime CurrentCalendarQuarter {
            get { return CurrentQuarter(TimeSpec.CalendarYearStartMonth); }
        }

        /// <summary>
        /// 현재 날짜가 속한 분기의 시작 일자를 나타냅니다. 단 한해의 시작 월이 <paramref name="yearStartMonth"/>을 기준으로 합니다.
        /// </summary>
        /// <param name="yearStartMonth"></param>
        /// <returns></returns>
        public static DateTime CurrentQuarter(this int yearStartMonth) {
            var now = ClockProxy.Clock.Now;

            var year = now.Year;
            if(now.Month < yearStartMonth)
                year--;

            var quarter = GetQuarterOfMonth(yearStartMonth, now.Month);
            var months = ((int)quarter - 1) * TimeSpec.MonthsPerQuarter;

            return new DateTime(year, yearStartMonth, 1).AddMonths(months);
        }

        /// <summary>
        /// 현재 날짜가 속한 월의 시작 일자를 나타냅니다.
        /// </summary>
        public static DateTime CurrentMonth {
            get { return TrimToDay(ClockProxy.Clock.Now); }
        }

        /// <summary>
        /// 현재 날짜가 속한 주(Week)의 시작 일자를 나타냅니다.
        /// </summary>
        /// <returns></returns>
        public static DateTime CurrentWeek() {
            return CurrentWeek(DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek);
        }

        /// <summary>
        /// 현재 날짜가 속한 주(Week)의 시작 일자를 나타냅니다. 지정된 문화권에 따른 한주의 시작 요일을 기준으로 한다.
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static DateTime CurrentWeek(this CultureInfo culture) {
            return ClockProxy.Clock.Now.StartTimeOfWeek(culture);
        }

        /// <summary>
        /// <paramref name="firstDayOfWeek"/>가 한주의 시작 요일일 때, 현재 날짜가 속한 주(Week)의 시작 일자를 나타냅니다. 
        /// </summary>
        /// <param name="firstDayOfWeek"></param>
        /// <returns></returns>
        public static DateTime CurrentWeek(this DayOfWeek firstDayOfWeek) {
            return StartTimeOfWeek(ClockProxy.Clock.Now, (DayOfWeek?)firstDayOfWeek);
        }

        /// <summary>
        /// 오늘
        /// </summary>
        public static DateTime CurrentDay {
            get { return ClockProxy.Clock.Today; }
        }

        /// <summary>
        /// 현재 시각의 시간
        /// </summary>
        public static DateTime CurrentHour {
            get { return TrimToMinute(ClockProxy.Clock.Now); }
        }

        /// <summary>
        /// 현재 시각의 분단위
        /// </summary>
        public static DateTime CurrentMinute {
            get { return TrimToSecond(ClockProxy.Clock.Now); }
        }

        /// <summary>
        /// 현재 시각의 초단위까지만
        /// </summary>
        public static DateTime CurrentSecond {
            get { return TrimToMillisecond(ClockProxy.Clock.Now); }
        }
    }
}