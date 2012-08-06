using System;
using NSoft.NFramework.TimePeriods.TimeRanges;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.TimePeriods {
    public static partial class TimeTool {
        /// <summary>
        /// <paramref name="start"/>를 시작 시각으로 <paramref name="duration"/> 만큼의 기간을 가지는 시간 범위를 빌드합니다.
        /// </summary>
        /// <param name="start">시작시각</param>
        /// <param name="duration">기간</param>
        /// <returns>TimeBlock</returns>
        public static TimeBlock GetTimeBlock(this DateTime start, TimeSpan duration) {
            return new TimeBlock(start, duration, false);
        }

        /// <summary>
        /// <paramref name="start"/> ~ <paramref name="end"/> 시각을 기간으로 하는 시간 범위를 빌드합니다.
        /// </summary>
        /// <param name="start">시작시각</param>
        /// <param name="end">완료시각</param>
        /// <returns>TimeBlock</returns>
        public static TimeBlock GetTimeBlock(this DateTime start, DateTime end) {
            return new TimeBlock(start, end, false);
        }

        /// <summary>
        /// <paramref name="start"/>를 시작 시각으로 <paramref name="duration"/> 만큼의 기간을 가지는 시간 범위를 빌드합니다.
        /// </summary>
        /// <param name="start">시작시각</param>
        /// <param name="duration">기간</param>
        /// <returns>TimeRange</returns>
        public static TimeRange GetTimeRange(this DateTime start, TimeSpan duration) {
            return new TimeRange(start, duration, false);
        }

        /// <summary>
        /// <paramref name="start"/> ~ <paramref name="end"/> 시각을 기간으로 하는 시간 범위를 빌드합니다.
        /// </summary>
        /// <param name="start">시작시각</param>
        /// <param name="end">완료시각</param>
        /// <returns>TimeRange</returns>
        public static TimeRange GetTimeRange(this DateTime start, DateTime end) {
            return new TimeRange(start, end, false);
        }

        /// <summary>
        /// <paramref name="start"/>부터 상대적으로 <paramref name="years"/> 년수 만큼의 기간
        /// </summary>
        /// <param name="start">시작 시각</param>
        /// <param name="years">시간 간격 (년수)</param>
        public static TimeRange GetRelativeYearPeriod(this DateTime start, int years) {
            return new TimeRange(start, start.AddYears(years));
        }

        /// <summary>
        /// <paramref name="start"/>부터 상대적으로 <paramref name="months"/> 개월수 만큼의 기간
        /// </summary>
        /// <param name="start">시작 시각</param>
        /// <param name="months">시간 간격 (월수)</param>
        public static TimeRange GetRelativeMonthPeriod(this DateTime start, int months) {
            return new TimeRange(start, start.AddMonths(months));
        }

        /// <summary>
        /// <paramref name="start"/>부터 상대적으로 <paramref name="weeks"/> 주수 만큼의 기간
        /// </summary>
        /// <param name="start">시작 시각</param>
        /// <param name="weeks">시간 간격 (주수)</param>
        public static TimeRange GetRelativeWeekPeriod(this DateTime start, int weeks) {
            return new TimeRange(start, DurationUtil.Weeks(weeks));
        }

        /// <summary>
        /// <paramref name="start"/>부터 상대적으로 <paramref name="days"/> 일수 만큼의 기간
        /// </summary>
        /// <param name="start">시작 시각</param>
        /// <param name="days">시간 간격 (일수)</param>
        public static TimeRange GetRelativeDayPeriod(this DateTime start, int days) {
            return new TimeRange(start, DurationUtil.Days(days));
        }

        /// <summary>
        /// <paramref name="start"/>부터 상대적으로 <paramref name="hours"/> 시간수 만큼의 기간
        /// </summary>
        /// <param name="start">시작 시각</param>
        /// <param name="hours">시간 간격 (시간수)</param>
        public static TimeRange GetRelativeHourPeriod(this DateTime start, int hours) {
            return new TimeRange(start, DurationUtil.Hours(hours));
        }

        /// <summary>
        /// <paramref name="start"/>부터 상대적으로 <paramref name="minutes"/> 분수 만큼의 기간
        /// </summary>
        /// <param name="start">시작 시각</param>
        /// <param name="minutes">시간 간격 (분수)</param>
        public static TimeRange GetRelativeMinutePeriod(this DateTime start, int minutes) {
            return new TimeRange(start, DurationUtil.Minutes(minutes));
        }

        /// <summary>
        /// <paramref name="start"/>부터 상대적으로 <paramref name="seconds"/> 초수 만큼의 기간
        /// </summary>
        /// <param name="start">시작 시각</param>
        /// <param name="seconds">시간 간격 (초수)</param>
        public static TimeRange GetRelativeSecondPeriod(this DateTime start, int seconds) {
            return new TimeRange(start, DurationUtil.Seconds(seconds));
        }

        /// <summary>
        /// <paramref name="moment"/>이 속하면서, <paramref name="periodKind"/>에 해당하는 <see cref="ITimePeriod"/>을 구합니다.
        /// </summary>
        /// <param name="moment"></param>
        /// <param name="periodKind"></param>
        /// <returns></returns>
        public static ITimePeriod GetPeriodOf(this DateTime moment, PeriodKind periodKind) {
            return GetPeriodOf(moment, periodKind, TimeCalendar.New());
        }

        /// <summary>
        /// <paramref name="moment"/>이 속하면서, <paramref name="periodKind"/>에 해당하는 <see cref="ITimePeriod"/>을 반환합니다.
        /// </summary>
        /// <param name="moment"></param>
        /// <param name="periodKind"></param>
        /// <param name="timeCalendar"></param>
        /// <returns></returns>
        /// <seealso cref="GetPeriodsOf(System.DateTime,NFramework.TimePeriods.PeriodKind,int,NFramework.TimePeriods.ITimeCalendar)"/>
        public static ITimePeriod GetPeriodOf(this DateTime moment, PeriodKind periodKind, ITimeCalendar timeCalendar) {
            if(IsDebugEnabled)
                log.Debug("날짜[{0}]가 속한 기간종류[{1}]의 기간을 구합니다. timeCalendar=[{2}]", moment.ToSortableString(), periodKind, timeCalendar);

            timeCalendar = timeCalendar ?? TimeCalendar.New();

            switch(periodKind) {
                case PeriodKind.Year:
                    return GetYearRange(moment, timeCalendar);

                case PeriodKind.Halfyear:
                    return GetHalfyearRange(moment, timeCalendar);

                case PeriodKind.Quarter:
                    return GetQuarterRange(moment, timeCalendar);
                    // return new QuarterRange(moment, timeCalendar);

                case PeriodKind.Month:
                    return GetMonthRange(moment, timeCalendar);

                case PeriodKind.Week:
                    return GetWeekRange(moment, timeCalendar);

                case PeriodKind.Day:
                    return GetDayRange(moment, timeCalendar);

                case PeriodKind.Hour:
                    return GetHourRange(moment, timeCalendar);

                case PeriodKind.Minute:
                    return GetMinuteRange(moment, timeCalendar);

                case PeriodKind.Second:
                    return new TimeRange(moment.TrimToMillisecond(), DurationUtil.Second);

                default:
                    throw new NotSupportedException("지원하지 않는 TimePeriod 종류입니다. periodKind=" + periodKind);
            }
        }

        /// <summary>
        /// <paramref name="moment"/>이 속하면서, <paramref name="periodKind"/>에 해당하는 <see cref="ICalendarTimeRange"/>을 구합니다.
        /// </summary>
        /// <param name="moment"></param>
        /// <param name="periodKind"></param>
        /// <param name="periodCount"></param>
        /// <returns></returns>
        public static ICalendarTimeRange GetPeriodsOf(this DateTime moment, PeriodKind periodKind, int periodCount) {
            return GetPeriodsOf(moment, periodKind, periodCount, TimeCalendar.New());
        }

        /// <summary>
        ///  <paramref name="moment"/>이 속하면서, <paramref name="periodKind"/>에 해당하는 <see cref="ICalendarTimeRange"/>을 구합니다.
        /// </summary>
        /// <param name="moment"></param>
        /// <param name="periodKind"></param>
        /// <param name="periodCount"></param>
        /// <param name="timeCalendar"></param>
        /// <returns></returns>
        public static ICalendarTimeRange GetPeriodsOf(this DateTime moment, PeriodKind periodKind, int periodCount,
                                                      ITimeCalendar timeCalendar) {
            if(IsDebugEnabled)
                log.Debug("날짜[{0}]가 속한 기간종류[{1}]의 기간을 구합니다. periodCount=[{2}], timeCalendar=[{3}]",
                          moment.ToSortableString(), periodKind, periodCount, timeCalendar);

            switch(periodKind) {
                case PeriodKind.Year:
                    return GetYearRanges(moment, periodCount, timeCalendar);

                case PeriodKind.Halfyear:
                    return GetHalfyearRanges(moment, periodCount, timeCalendar);

                case PeriodKind.Quarter:
                    return GetQuarterRanges(moment, periodCount, timeCalendar);

                case PeriodKind.Month:
                    return GetMonthRanges(moment, periodCount, timeCalendar);

                case PeriodKind.Week:
                    return GetWeekRanges(moment, periodCount, timeCalendar);

                case PeriodKind.Day:
                    return GetDayRanges(moment, periodCount, timeCalendar);

                case PeriodKind.Hour:
                    return GetHourRanges(moment, periodCount, timeCalendar);

                case PeriodKind.Minute:
                    return GetMinuteRanges(moment, periodCount, timeCalendar);

                case PeriodKind.Second:
                    return new CalendarTimeRange(moment.TrimToMillisecond(), DurationUtil.Seconds(periodCount), timeCalendar);

                default:
                    throw new NotSupportedException("지원하지 않는 TimePeriod 종류입니다. periodKind=" + periodKind);
            }
        }

        #region << YearRange >>

        /// <summary>
        /// <paramref name="moment"/>가 속한 년의 전체 기간
        /// </summary>
        public static YearRange GetYearRange(this DateTime moment, ITimeCalendar timeCalendar = null) {
            return new YearRange(moment, timeCalendar ?? TimeCalendar.New());
        }

        /// <summary>
        /// <paramref name="moment"/>가 속한 년과 <paramref name="yearCount"/> 만큼의 기간
        /// </summary>
        public static YearRangeCollection GetYearRanges(this DateTime moment, int yearCount, ITimeCalendar timeCalendar = null) {
            return new YearRangeCollection(moment, yearCount, timeCalendar ?? TimeCalendar.New());
        }

        #endregion

        #region << HalfyearRange >>

        /// <summary>
        /// <paramref name="moment"/>가 속한 분기의 전체 기간 (5월이면, 1월1일 ~ 6월30일)
        /// </summary>
        public static HalfyearRange GetHalfyearRange(this DateTime moment, ITimeCalendar timeCalendar = null) {
            return new HalfyearRange(moment, timeCalendar ?? TimeCalendar.New());
        }

        /// <summary>
        /// <paramref name="moment"/>가 속한 분기부터 <paramref name="halfyearCount"/>만큼의 분기
        /// </summary>
        public static HalfyearRangeCollection GetHalfyearRanges(this DateTime moment, int halfyearCount,
                                                                ITimeCalendar timeCalendar = null) {
            return new HalfyearRangeCollection(moment, halfyearCount, timeCalendar ?? TimeCalendar.New());
        }

        #endregion

        #region << QuarterRange >>

        /// <summary>
        /// <paramref name="moment"/>가 속한 분기의 전체 기간 (5월이면 4월1일부터 6월30일까지)
        /// </summary>
        public static QuarterRange GetQuarterRange(this DateTime moment, ITimeCalendar timeCalendar = null) {
            return new QuarterRange(moment, timeCalendar ?? TimeCalendar.New());
        }

        /// <summary>
        /// <paramref name="moment"/>가 속한 분기부터 <paramref name="quarterCount"/> 갯수의 분기까지의 기간
        /// </summary>
        /// <param name="moment">기준 일자</param>
        /// <param name="quarterCount">분기 수</param>
        /// <param name="timeCalendar">기준 TimeCalendar</param>
        public static QuarterRangeCollection GetQuarterRanges(this DateTime moment, int quarterCount, ITimeCalendar timeCalendar = null) {
            return new QuarterRangeCollection(moment, quarterCount, timeCalendar ?? TimeCalendar.New());
        }

        #endregion

        #region << MonthRange >>

        /// <summary>
        /// <paramref name="moment"/>가 속한 월의 전체 기간 (5월이면 5월1일~5월31일)
        /// </summary>
        public static MonthRange GetMonthRange(this DateTime moment, ITimeCalendar timeCalendar = null) {
            return new MonthRange(moment, timeCalendar ?? TimeCalendar.New());
        }

        /// <summary>
        /// <paramref name="moment"/>가 속한 월로부터 <paramref name="monthCount"/> 만큼의 기간
        /// </summary>
        public static MonthRangeCollection GetMonthRanges(this DateTime moment, int monthCount, ITimeCalendar timeCalendar = null) {
            return new MonthRangeCollection(moment, monthCount, timeCalendar ?? TimeCalendar.New());
        }

        #endregion

        #region << WeekRange >>

        /// <summary>
        /// <paramref name="moment"/>가 속한 한주의 기간을 반환합니다.
        /// </summary>
        public static WeekRange GetWeekRange(this DateTime moment, ITimeCalendar timeCalendar = null) {
            return new WeekRange(moment, timeCalendar ?? TimeCalendar.New());
        }

        /// <summary>
        /// <paramref name="moment"/>가 속한 주에서 <paramref name="weekCount"/>만큼의 주의 기간
        /// </summary>
        public static WeekRangeCollection GetWeekRanges(this DateTime moment, int weekCount, ITimeCalendar timeCalendar = null) {
            return new WeekRangeCollection(moment, weekCount, timeCalendar ?? TimeCalendar.New());
        }

        #endregion

        #region << DayRange >>

        /// <summary>
        /// <paramref name="moment"/>가 속한 하루의 기간을 반환합니다 (1일)
        /// </summary>
        public static DayRange GetDayRange(this DateTime moment, ITimeCalendar timeCalendar = null) {
            return new DayRange(moment, timeCalendar ?? TimeCalendar.New());
        }

        /// <summary>
        /// <paramref name="moment"/>가 속한 하루서부터 <paramref name="dayCount"/>만큼의 일자의 기간
        /// </summary>
        public static DayRangeCollection GetDayRanges(this DateTime moment, int dayCount, ITimeCalendar timeCalendar = null) {
            return new DayRangeCollection(moment, dayCount, timeCalendar ?? TimeCalendar.New());
        }

        #endregion

        #region << HourRange >>

        /// <summary>
        /// <paramref name="moment"/>가 속한 시간의 기간을 반환합니다 (1시간)
        /// </summary>
        public static HourRange GetHourRange(this DateTime moment, ITimeCalendar timeCalendar = null) {
            return new HourRange(moment, timeCalendar ?? TimeCalendar.New());
        }

        /// <summary>
        /// <paramref name="moment"/>가 속한 하루서부터 <paramref name="hourCount"/>만큼의 시간의 기간
        /// </summary>
        public static HourRangeCollection GetHourRanges(this DateTime moment, int hourCount, ITimeCalendar timeCalendar = null) {
            return new HourRangeCollection(moment, hourCount, timeCalendar ?? TimeCalendar.New());
        }

        #endregion

        #region << MinuteRange >>

        /// <summary>
        /// <paramref name="moment"/>가 속한 분의 기간을 반환합니다 (1분)
        /// </summary>
        public static MinuteRange GetMinuteRange(this DateTime moment, ITimeCalendar timeCalendar = null) {
            return new MinuteRange(moment, timeCalendar ?? TimeCalendar.New());
        }

        /// <summary>
        /// <paramref name="moment"/>가 속한 하루서부터 <paramref name="minuteCount"/>만큼의 분의 기간
        /// </summary>
        public static MinuteRangeCollection GetMinuteRanges(this DateTime moment, int minuteCount, ITimeCalendar timeCalendar = null) {
            return new MinuteRangeCollection(moment, minuteCount, timeCalendar ?? TimeCalendar.New());
        }

        #endregion
    }
}