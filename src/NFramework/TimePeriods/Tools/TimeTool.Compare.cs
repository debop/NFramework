using System;
using System.Globalization;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.TimePeriods {
    public static partial class TimeTool {
        /// <summary>
        /// 두 날짜 값이 <paramref name="periodKind"/> 단위까지 같은지 판단합니다.
        /// </summary>
        public static bool IsSameTime(this DateTime left, DateTime right, PeriodKind periodKind) {
            switch(periodKind) {
                case PeriodKind.Year:
                    return IsSameYear(left, right);

                case PeriodKind.Halfyear:
                    return IsSameHalfyear(left, right);

                case PeriodKind.Quarter:
                    return IsSameQuarter(left, right);

                case PeriodKind.Month:
                    return IsSameMonth(left, right);

                case PeriodKind.Week:
                    return IsSameWeek(left, right, null, WeekOfYearRuleKind.Calendar);

                case PeriodKind.Day:
                    return IsSameDay(left, right);

                case PeriodKind.Hour:
                    return IsSameHour(left, right);

                case PeriodKind.Minute:
                    return IsSameMinute(left, right);

                case PeriodKind.Second:
                    return IsSameSecond(left, right);

                case PeriodKind.Millisecond:
                default:
                    return IsSameDateTime(left, right);
            }
        }

        /// <summary>
        /// 두 DateTime 값이 년(Year) 단위까지 같은지 판단합니다.
        /// </summary>
        public static bool IsSameYear(this DateTime left, DateTime right, int yearBaseMonth = TimeSpec.CalendarYearStartMonth) {
            if(IsDebugEnabled)
                log.Debug("같은 년(Year)인지 판단합니다. left=[{0}], right=[{1}], yearBaseMonth=[{2}], ",
                          left.ToShortDateString(), right.ToShortDateString(), yearBaseMonth);

            return GetYearOf(left, yearBaseMonth) == GetYearOf(right, yearBaseMonth);
        }

        /// <summary>
        /// 두 DateTime 값이 반기(Half Year) 단위까지 같은지 판단합니다.
        /// </summary>
        public static bool IsSameHalfyear(this DateTime left, DateTime right, int yearBaseMonth = TimeSpec.CalendarYearStartMonth) {
            if(IsDebugEnabled)
                log.Debug("같은 반기(Halfyear)인지 판단합니다. yearBaseMonth=[{0}], left=[{1}], right=[{2}]",
                          yearBaseMonth, left.ToShortDateString(), right.ToShortDateString());

            var leftYear = left.GetYearOf(yearBaseMonth);
            var rightYear = right.GetYearOf(yearBaseMonth);

            if(leftYear != rightYear)
                return false;

            return GetHalfyearOfMonth(yearBaseMonth, left.Month) == GetHalfyearOfMonth(yearBaseMonth, right.Month);
        }

        /// <summary>
        /// 두 DateTime 값이 분기(Quarter) 단위까지 같은지 판단합니다.
        /// </summary>
        public static bool IsSameQuarter(this DateTime left, DateTime right, int yearBaseMonth = TimeSpec.CalendarYearStartMonth) {
            if(IsDebugEnabled)
                log.Debug("같은 분기(Quarter)인지 판단합니다. yearBaseMonth=[{0}], left=[{1}], right=[{2}]",
                          yearBaseMonth, left.ToShortDateString(), right.ToShortDateString());

            var leftYear = left.GetYearOf(yearBaseMonth);
            var rightYear = right.GetYearOf(yearBaseMonth);

            if(leftYear != rightYear)
                return false;

            return GetQuarterOfMonth(yearBaseMonth, left.Month) == GetQuarterOfMonth(yearBaseMonth, right.Month);
        }

        /// <summary>
        /// 두 DateTime 값이 월(Month) 단위까지 같은지 판단합니다.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool IsSameMonth(this DateTime left, DateTime right) {
            return Equals(left.TrimToDay(), right.TrimToDay());
            // return IsSameYear(left, right) && Equals(left.Month, right.Month);
        }

        /// <summary>
        /// 두 DateTime 값이 주(Week) 단위까지 같은지 판단합니다.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="culture"></param>
        /// <param name="weekOfYearRuleKind"></param>
        /// <returns></returns>
        public static bool IsSameWeek(this DateTime left, DateTime right, CultureInfo culture, WeekOfYearRuleKind weekOfYearRuleKind) {
            culture = culture.GetOrCurrentCulture();
            return IsSameWeek(left, right, culture, culture.DateTimeFormat.CalendarWeekRule, culture.DateTimeFormat.FirstDayOfWeek,
                              weekOfYearRuleKind);
        }

        /// <summary>
        /// 두 DateTime 값이 주(Week) 단위까지 같은지 판단합니다.
        /// </summary>
        public static bool IsSameWeek(this DateTime left, DateTime right, CultureInfo culture, CalendarWeekRule weekRule,
                                      DayOfWeek firstDayOfWeek, WeekOfYearRuleKind weekOfYearRuleKind) {
            culture = culture.GetOrCurrentCulture();

            var leftYearWeek = GetWeekOfYear(left, culture, weekRule, firstDayOfWeek, weekOfYearRuleKind);
            var rightYearWeek = GetWeekOfYear(right, culture, weekRule, firstDayOfWeek, weekOfYearRuleKind);

            return Equals(leftYearWeek, rightYearWeek);
        }

        /// <summary>
        /// 두 DateTime 값이 일(Day) 단위까지 같은지 판단합니다.
        /// </summary>
        public static bool IsSameDay(this DateTime left, DateTime right) {
            return Equals(left.TrimToHour(), right.TrimToHour());
        }

        /// <summary>
        /// 두 DateTime 값이 시간(Hour) 단위까지 같은지 판단합니다.
        /// </summary>
        public static bool IsSameHour(this DateTime left, DateTime right) {
            return Equals(left.TrimToMinute(), right.TrimToMinute());
        }

        /// <summary>
        /// 두 DateTime 값이 분(Minute) 단위까지 같은지 판단합니다.
        /// </summary>
        public static bool IsSameMinute(this DateTime left, DateTime right) {
            return Equals(left.TrimToSecond(), right.TrimToSecond());
        }

        /// <summary>
        /// 두 DateTime 값이 초(Second) 단위까지 같은지 판단합니다.
        /// </summary>
        public static bool IsSameSecond(this DateTime left, DateTime right) {
            return Equals(left.TrimToMillisecond(), right.TrimToMillisecond());
        }

        /// <summary>
        /// 두 일자가 같은 값을 가지는지 판단합니다.
        /// </summary>
        public static bool IsSameDateTime(this DateTime left, DateTime right) {
            return Equals(left, right);
        }
    }
}