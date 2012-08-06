using System;
using System.Linq;
using NSoft.NFramework.TimePeriods.TimeRanges;

namespace NSoft.NFramework.TimePeriods.Calendars {
    /// <summary>
    /// 칼렌다 기준으로 특정 기간(limits)에서 필터(filter)에 해당하는 기간을 추출합니다.
    /// </summary>
    [Serializable]
    public class CalendarPeriodCollector : CalendarVisitor<CalendarPeriodCollectorFilter, CalendarPeriodCollectorContext> {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public CalendarPeriodCollector(CalendarPeriodCollectorFilter filter, ITimePeriod limits) : base(filter, limits) {}

        public CalendarPeriodCollector(CalendarPeriodCollectorFilter filter, ITimePeriod limits, SeekDirection seekDirection)
            : base(filter, limits, seekDirection) {}

        public CalendarPeriodCollector(CalendarPeriodCollectorFilter filter, ITimePeriod limits, ITimeCalendar calendar)
            : base(filter, limits, calendar) {}

        public CalendarPeriodCollector(CalendarPeriodCollectorFilter filter, ITimePeriod limits, SeekDirection? seekDirection,
                                       ITimeCalendar calendar) : base(filter, limits, seekDirection, calendar) {}

        private readonly ITimePeriodCollection _periods = new TimePeriodCollection();

        public ITimePeriodCollection Periods {
            get { return _periods; }
        }

        /// <summary>
        /// 필터에 해당하는 Year 단위의 기간(<see cref="YearRange"/>)를 수집합니다.
        /// </summary>
        public void CollectYears() {
            StartPeriodVisit(new CalendarPeriodCollectorContext
                             {
                                 Scope = CalendarPeriodCollectorContext.CollectKind.Year
                             });
        }

        /// <summary>
        /// 필터에 해당하는 Month 단위의 기간(<see cref="MonthRange"/>)를 수집합니다.
        /// </summary>
        public void CollectMonths() {
            StartPeriodVisit(new CalendarPeriodCollectorContext
                             {
                                 Scope = CalendarPeriodCollectorContext.CollectKind.Month
                             });
        }

        /// <summary>
        /// 필터에 해당하는 Day 단위의 기간(<see cref="DayRange"/>)를 수집합니다.
        /// </summary>
        public void CollectDays() {
            StartPeriodVisit(new CalendarPeriodCollectorContext
                             {
                                 Scope = CalendarPeriodCollectorContext.CollectKind.Day
                             });
        }

        /// <summary>
        /// 필터에 해당하는 Hour 단위의 기간(<see cref="HourRange"/>)를 수집합니다.
        /// </summary>
        public void CollectHours() {
            StartPeriodVisit(new CalendarPeriodCollectorContext
                             {
                                 Scope = CalendarPeriodCollectorContext.CollectKind.Hour
                             });
        }

        protected override bool EnterYears(YearRangeCollection yearRangeCollection, CalendarPeriodCollectorContext context) {
            return (int)context.Scope > (int)CalendarPeriodCollectorContext.CollectKind.Year;
        }

        protected override bool EnterMonths(YearRange yearRange, CalendarPeriodCollectorContext context) {
            return (int)context.Scope > (int)CalendarPeriodCollectorContext.CollectKind.Month;
        }

        protected override bool EnterDays(MonthRange month, CalendarPeriodCollectorContext context) {
            return (int)context.Scope > (int)CalendarPeriodCollectorContext.CollectKind.Day;
        }

        protected override bool EnterHours(DayRange day, CalendarPeriodCollectorContext context) {
            return (int)context.Scope > (int)CalendarPeriodCollectorContext.CollectKind.Hour;
        }

        protected override bool OnVisitYears(YearRangeCollection years, CalendarPeriodCollectorContext context) {
            if(IsDebugEnabled)
                log.Debug("Visit Years... years=[{0}]", years);

            if(context.Scope != CalendarPeriodCollectorContext.CollectKind.Year)
                return true; // continue

            var query = years.GetYears().Where(year => IsMatchingYear(year, context) && CheckLimits(year));
            _periods.AddAll(query.Cast<ITimePeriod>().AsEnumerable());

            return false; // abort
        }

        protected override bool OnVisitYear(YearRange year, CalendarPeriodCollectorContext context) {
            if(IsDebugEnabled)
                log.Debug("Visit Year... year=[{0}]", year);

            if(context.Scope != CalendarPeriodCollectorContext.CollectKind.Month) {
                return true; // continue
            }

            // all month
            if(Filter.CollectingMonths.Count == 0) {
                var months = year.GetMonths().Where(m => IsMatchingMonth(m, context) && CheckLimits(m));

                _periods.AddAll(months.Cast<ITimePeriod>().AsEnumerable());
            }
            else {
                var months =
                    Filter.CollectingMonths
                        .Select(m => {
                                    if(m.IsSingleMonth) {
                                        var month = new MonthRange(year.YearValue, m.Min, year.TimeCalendar);
                                        if(IsMatchingMonth(month, context) && CheckLimits(month))
                                            return (ITimePeriod)month;
                                    }
                                    else {
                                        var monthRanges = new MonthRangeCollection(year.YearValue, m.Min, m.Max - m.Min,
                                                                                   year.TimeCalendar);
                                        var isMatching = monthRanges.GetMonths().All(month => IsMatchingMonth(month, context));

                                        if(isMatching && CheckLimits(monthRanges))
                                            return (ITimePeriod)monthRanges;
                                    }
                                    return (ITimePeriod)null;
                                })
                        .Where(m => m != null);

                _periods.AddAll(months.Cast<ITimePeriod>().AsEnumerable());
            }

            return false; // abort
        }

        protected override bool OnVisitMonth(MonthRange month, CalendarPeriodCollectorContext context) {
            if(IsDebugEnabled)
                log.Debug("Month[{0}]를 탐색합니다...", month);

            if(context.Scope != CalendarPeriodCollectorContext.CollectKind.Day) {
                return true; // continue
            }

            // all month
            if(Filter.CollectingDays.Count == 0) {
                var days = month.GetDays().Where(d => IsMatchingDay(d, context) && CheckLimits(d));
                _periods.AddAll(days.Cast<ITimePeriod>().AsEnumerable());
            }
            else {
                var days =
                    Filter.CollectingDays
                        .Select(day => {
                                    if(day.IsSingleDay) {
                                        var dayRange = new DayRange(month.Year, month.Month, day.Min, month.TimeCalendar);
                                        if(IsMatchingDay(dayRange, context) && CheckLimits(dayRange))
                                            return dayRange;
                                    }
                                    else {
                                        var dayRanges = new DayRangeCollection(month.Year, month.Month, day.Min, day.Max - day.Min,
                                                                               month.TimeCalendar);
                                        var isMatching = dayRanges.GetDays().All(d => IsMatchingDay(d, context));

                                        if(isMatching && CheckLimits(dayRanges))
                                            return (ITimePeriod)dayRanges;
                                    }
                                    return null;
                                })
                        .Where(d => d != null);

                _periods.AddAll(days.Cast<ITimePeriod>().AsEnumerable());
            }

            return false; // abort
        }

        protected override bool OnVisitDay(DayRange day, CalendarPeriodCollectorContext context) {
            if(IsDebugEnabled)
                log.Debug("Day[{0}]를 탐색합니다...", day);

            if(context.Scope != CalendarPeriodCollectorContext.CollectKind.Hour) {
                return true; // continue
            }

            // all month
            if(Filter.CollectingHours.Count == 0) {
                var hours = day.GetHours().Where(h => IsMatchingHour(h, context) && CheckLimits(h));
                _periods.AddAll(hours.Cast<ITimePeriod>().AsEnumerable());
            }
            else if(IsMatchingDay(day, context)) {
                var hours =
                    Filter.CollectingHours
                        .Select(hour => {
                                    var startTime = hour.Start.GetDateTime(day.Start);
                                    var endTime = hour.End.GetDateTime(day.Start);
                                    var hourRanges = new CalendarTimeRange(startTime, endTime, day.TimeCalendar);

                                    if(CheckExcludePeriods(hourRanges) && CheckLimits(hourRanges))
                                        return (ITimePeriod)hourRanges;

                                    return null;
                                })
                        .Where(h => h != null);

                _periods.AddAll(hours.AsEnumerable());
            }

            return false; // abort
        }

        //! 시간 미만 단위인 분단위까지 탐색하는 것은 Over입니다. DayRange 안에 TimeOfDay가 있으므로 굳이 할 필요 없습니다.
        //protected override bool OnVisitHour(HourRange hour, CalendarPeriodCollectorContext context)
        //{
        //    if(IsDebugEnabled)
        //        log.Debug("Visit Hour... hour=[{0}]", hour);

        //    if(context.Scope != CalendarPeriodCollectorContext.CollectKind.Minute)
        //    {
        //        return true; // continue
        //    }

        //    // all month
        //    if(Filter.CollectingMinutes.Count == 0)
        //    {
        //        var minutes =
        //            hour.GetMinutes()
        //                .AsParallel()
        //                .AsOrdered()
        //                .Where(n => IsMatchingMinute((MinuteRange) n, context) && CheckLimits(n));

        //        _periods.AddAll(minutes);
        //    }
        //    else if(IsMatchingHour(hour, context))
        //    {
        //        var minutes =
        //            Filter.CollectingMinutes
        //                .AsParallel()
        //                .AsOrdered()
        //                .Select(minute =>
        //                        {
        //                            var startTime = hour.Start.TrimToMinute(minute.Start.Minute);
        //                            var endTime = hour.Start.TrimToMinute(minute.End.Minute);

        //                            var minuteRanges = new CalendarTimeRange(startTime, endTime, hour.Calendar);
        //                            if(CheckExcludePeriods(minuteRanges) && CheckLimits(minuteRanges))
        //                                return (ITimePeriod) minuteRanges;

        //                            return null;
        //                        })
        //                .Where(n => n != null);

        //        _periods.AddAll(minutes);
        //    }

        //    return false; // abort
        //}

        public override string ToString() {
            return _periods.ToString();
        }
    }
}