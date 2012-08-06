using System;
using System.Linq;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.TimePeriods.TimeRanges;

namespace NSoft.NFramework.TimePeriods.Calendars {
    /// <summary>
    /// 특정 기간에 대한 필터링 정보를 기반으로 기간들을 필터링 할 수 있도록 특정 기간을 탐색하는 Visitor입니다.
    /// </summary>
    /// <typeparam name="TFilter"></typeparam>
    /// <typeparam name="TContext"></typeparam>
    [Serializable]
    public abstract class CalendarVisitor<TFilter, TContext>
        where TFilter : class, ICalendarVisitorFilter
        where TContext : class, ICalendarVisitorContext {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        protected CalendarVisitor(TFilter filter, ITimePeriod limits) : this(filter, limits, SeekDirection.Forward, null) {}

        protected CalendarVisitor(TFilter filter, ITimePeriod limits, SeekDirection seekDirection)
            : this(filter, limits, seekDirection, null) {}

        protected CalendarVisitor(TFilter filter, ITimePeriod limits, ITimeCalendar calendar)
            : this(filter, limits, SeekDirection.Forward, calendar) {}

        protected CalendarVisitor(TFilter filter, ITimePeriod limits, SeekDirection? seekDirection, ITimeCalendar calendar) {
            filter.ShouldNotBeNull("filter");
            limits.ShouldNotBeNull("limits");

            Filter = filter;
            Limits = limits;
            SeekDirection = seekDirection ?? SeekDirection.Forward;
            Calendar = calendar;
        }

        public TFilter Filter { get; private set; }

        public ITimePeriod Limits { get; private set; }

        public SeekDirection SeekDirection { get; private set; }

        public ITimeCalendar Calendar { get; private set; }

        protected void StartPeriodVisit(TContext context) {
            StartPeriodVisit(Limits, context);
        }

        protected void StartPeriodVisit(ITimePeriod period, TContext context) {
            if(IsDebugEnabled)
                log.Debug("기간에 대한 탐색을 시작합니다... period=[{0}], context=[{1}]", period, context);

            period.ShouldNotBeNull("period");

            if(period.IsMoment)
                return;

            OnVisitStart();

            var years = (Calendar != null)
                            ? new YearRangeCollection(period.Start.Year, period.End.Year - period.Start.Year + 1, Calendar)
                            : new YearRangeCollection(period.Start.Year, period.End.Year - period.Start.Year + 1);

            if(OnVisitYears(years, context) && EnterYears(years, context)) {
                var yearsToVisit = years.GetYears().ToTimePeriodCollection();

                if(SeekDirection == SeekDirection.Backward)
                    yearsToVisit.SortByEnd(OrderDirection.Asc);

                foreach(YearRange year in yearsToVisit) {
                    //if(IsDebugEnabled)
                    //    log.Debug("Year[{0}]를 탐색합니다...", year.YearValue);

                    if(year.OverlapsWith(period) == false || OnVisitYear(year, context) == false)
                        continue;

                    if(EnterMonths(year, context) == false)
                        continue;

                    var monthsToVisit = year.GetMonths().ToTimePeriodCollection();

                    if(SeekDirection == SeekDirection.Backward)
                        monthsToVisit.SortByEnd(OrderDirection.Asc);

                    monthsToVisit
                        .Cast<MonthRange>()
#if !SILVERLIGHT
                        .AsParallel()
                        .AsOrdered()
#endif
                        .RunEach(month =>
                                 // foreach(MonthRange month in monthsToVisit) 
                                 {
                                     //if(IsDebugEnabled)
                                     //    log.Debug("Month[{0}]를 탐색합니다...", month.Month);

                                     if(month.OverlapsWith(period) == false || OnVisitMonth(month, context) == false)
                                         return;

                                     if(EnterDays(month, context) == false)
                                         return;

                                     var daysToVisit = month.GetDays().ToTimePeriodCollection();

                                     if(SeekDirection == SeekDirection.Backward)
                                         daysToVisit.SortByEnd(OrderDirection.Asc);

                                     foreach(DayRange day in daysToVisit) {
                                         //if(IsDebugEnabled)
                                         //    log.Debug("Day[{0}]를 탐색합니다...", day.Day);

                                         if(day.OverlapsWith(period) == false || OnVisitDay(day, context) == false)
                                             continue;

                                         if(EnterHours(day, context) == false)
                                             continue;

                                         var hoursToVisit = day.GetHours().ToTimePeriodCollection();

                                         if(SeekDirection == SeekDirection.Backward)
                                             hoursToVisit.SortByEnd(OrderDirection.Asc);

                                         foreach(HourRange hour in hoursToVisit) {
                                             //if(IsDebugEnabled)
                                             //    log.Debug("Hour[{0}]를 탐색합니다...", hour.Hour);

                                             if(hour.OverlapsWith(period) == false || OnVisitHour(hour, context) == false)
                                                 continue;

                                             //if(EnterMinutes(hour, context) == false)
                                             //    continue;

                                             //var minutesToVisit = hour.GetMinutes();

                                             //if(SeekDirection == SeekDirection.Backward)
                                             //    minutesToVisit.SortByEnd(null);

                                             //foreach(MinuteRange minute in minutesToVisit)
                                             //{
                                             //    if(minute.OverlapsWith(period) == false || OnVisitMinute(minute, context) == false)
                                             //        continue;
                                             //}
                                         }
                                     }
                                 });
                }
            }

            OnVisitEnd();

            if(IsDebugEnabled)
                log.Debug("기간에 대한 탐색을 완료했습니다!!! period=[{0}], context=[{1}]", period, context);
        }

        protected YearRange StartYearVisit(YearRange year, TContext context, SeekDirection? visitDirection = null) {
            year.ShouldNotBeNull("year");
            var direction = visitDirection ?? SeekDirection;

            if(IsDebugEnabled)
                log.Debug("Year 단위로 탐색합니다. year=[{0}], context=[{1}], direction=[{2}]", year, context, direction);

            YearRange lastVisited = null;

            OnVisitStart();

            var minStart = DateTime.MinValue;
            var maxEnd = DateTime.MaxValue.AddYears(-1);
            var offset = (direction == SeekDirection.Forward) ? 1 : -1;

            while(year.Start > minStart && year.End < maxEnd) {
                if(OnVisitYear(year, context) == false) {
                    lastVisited = year;
                    break;
                }

                year = year.AddYears(offset);
            }

            OnVisitEnd();

            if(IsDebugEnabled)
                log.Debug("마지막 탐색 Year. lastVisited=[{0}]", lastVisited);

            return lastVisited;
        }

        protected MonthRange StartMonthVisit(MonthRange month, TContext context, SeekDirection? visitDirection = null) {
            month.ShouldNotBeNull("month");
            var direction = visitDirection ?? SeekDirection;

            if(IsDebugEnabled)
                log.Debug("Month 단위로 탐색합니다. month=[{0}], context=[{1}], direction=[{2}]", month, context, direction);

            MonthRange lastVisited = null;

            OnVisitStart();

            var minStart = DateTime.MinValue;
            var maxEnd = DateTime.MaxValue.AddYears(-1);
            var offset = (direction == SeekDirection.Forward) ? 1 : -1;

            while(month.Start > minStart && month.End < maxEnd) {
                if(OnVisitMonth(month, context) == false) {
                    lastVisited = month;
                    break;
                }

                month = month.AddMonths(offset);
            }

            OnVisitEnd();

            if(IsDebugEnabled)
                log.Debug("마지막 탐색 Month. lastVisited=[{0}]", lastVisited);

            return lastVisited;
        }

        protected DayRange StartDayVisit(DayRange day, TContext context, SeekDirection? visitDirection = null) {
            day.ShouldNotBeNull("day");
            var direction = visitDirection ?? SeekDirection;

            if(IsDebugEnabled)
                log.Debug("Day 단위로 탐색합니다. day=[{0}], context=[{1}], direction=[{2}]", day, context, direction);

            DayRange lastVisited = null;

            OnVisitStart();

            var minStart = DateTime.MinValue;
            var maxEnd = DateTime.MaxValue.AddYears(-1);
            var offset = (direction == SeekDirection.Forward) ? 1 : -1;

            while(day.Start > minStart && day.End < maxEnd) {
                if(OnVisitDay(day, context) == false) {
                    lastVisited = day;
                    break;
                }

                day = day.AddDays(offset);
            }

            OnVisitEnd();

            if(IsDebugEnabled)
                log.Debug("마지막 탐색 Day. lastVisited=[{0}]", lastVisited);

            return lastVisited;
        }

        protected HourRange StartHourhVisit(HourRange hour, TContext context, SeekDirection? visitDirection = null) {
            hour.ShouldNotBeNull("hour");
            var direction = visitDirection ?? SeekDirection;

            if(IsDebugEnabled)
                log.Debug("Hour 단위로 탐색합니다. hour=[{0}], context=[{1}], direction=[{2}]", hour, context, direction);

            HourRange lastVisited = null;

            OnVisitStart();

            var minStart = DateTime.MinValue;
            var maxEnd = DateTime.MaxValue.AddYears(-1);
            var offset = (direction == SeekDirection.Forward) ? 1 : -1;

            while(hour.Start > minStart && hour.End < maxEnd) {
                if(OnVisitHour(hour, context) == false) {
                    lastVisited = hour;
                    break;
                }
                hour = hour.AddHours(offset);
            }

            OnVisitEnd();

            if(IsDebugEnabled)
                log.Debug("마지막 탐색 Hour. lastVisited=[{0}]", lastVisited);

            return lastVisited;
        }

        protected virtual void OnVisitStart() {
            if(IsDebugEnabled)
                log.Debug("Calendar Visit를 시작합니다...");
        }

        protected virtual bool CheckLimits(ITimePeriod target) {
            return Limits.HasInside(target);
        }

        protected virtual bool CheckExcludePeriods(ITimePeriod target) {
            if(Filter.ExcludePeriods.Any() == false)
                return true;

            return (Filter.ExcludePeriods.OverlapPeriods(target).Any() == false);
        }

        protected virtual bool EnterYears(YearRangeCollection yearRangeCollection, TContext context) {
            return true;
        }

        protected virtual bool EnterMonths(YearRange yearRange, TContext context) {
            return true;
        }

        protected virtual bool EnterDays(MonthRange month, TContext context) {
            return true;
        }

        protected virtual bool EnterHours(DayRange day, TContext context) {
            return true;
        }

        //protected virtual bool EnterMinutes(HourRange hour, TContext context)
        //{
        //    return true;
        //}

        protected virtual bool OnVisitYears(YearRangeCollection years, TContext context) {
            return true;
        }

        protected virtual bool OnVisitYear(YearRange year, TContext context) {
            return true;
        }

        protected virtual bool OnVisitMonth(MonthRange month, TContext context) {
            return true;
        }

        protected virtual bool OnVisitDay(DayRange day, TContext context) {
            return true;
        }

        protected virtual bool OnVisitHour(HourRange hour, TContext context) {
            return true;
        }

        //protected virtual bool OnVisitMinute(MinuteRange minute, TContext context)
        //{
        //    return true;
        //}

        protected virtual bool IsMatchingYear(YearRange year, TContext context) {
            if(Filter.Years.Count > 0 && Filter.Years.Contains(year.YearValue) == false)
                return false;

            return CheckExcludePeriods(year);
        }

        protected virtual bool IsMatchingMonth(MonthRange month, TContext context) {
            if(Filter.Years.Count > 0 && Filter.Years.Contains(month.Year) == false)
                return false;

            if(Filter.Months.Count > 0 && Filter.Months.Contains(month.Month) == false)
                return false;

            return CheckExcludePeriods(month);
        }

        protected virtual bool IsMatchingDay(DayRange day, TContext context) {
            if(Filter.Years.Count > 0 && Filter.Years.Contains(day.Year) == false)
                return false;

            if(Filter.Months.Count > 0 && Filter.Months.Contains(day.Month) == false)
                return false;

            if(Filter.Days.Count > 0 && Filter.Days.Contains(day.Day) == false)
                return false;

            if(Filter.WeekDays.Count > 0 && Filter.WeekDays.Contains(day.DayOfWeek) == false)
                return false;

            return CheckExcludePeriods(day);
        }

        protected virtual bool IsMatchingHour(HourRange hour, TContext context) {
            if(Filter.Years.Count > 0 && Filter.Years.Contains(hour.Year) == false)
                return false;

            if(Filter.Months.Count > 0 && Filter.Months.Contains(hour.Month) == false)
                return false;

            if(Filter.Days.Count > 0 && Filter.Days.Contains(hour.Day) == false)
                return false;

            if(Filter.WeekDays.Count > 0 && Filter.WeekDays.Contains(hour.Start.DayOfWeek) == false)
                return false;

            if(Filter.Hours.Count > 0 && Filter.Hours.Contains(hour.Hour) == false)
                return false;

            return CheckExcludePeriods(hour);
        }

        //protected virtual bool IsMatchingMinute(MinuteRange minute, TContext context)
        //{
        //    if(Filter.Years.Count > 0 && Filter.Years.Contains(minute.Year) == false)
        //        return false;

        //    if(Filter.Months.Count > 0 && Filter.Months.Contains(minute.Month) == false)
        //        return false;

        //    if(Filter.Days.Count > 0 && Filter.Days.Contains(minute.Day) == false)
        //        return false;

        //    if(Filter.WeekDays.Count > 0 && Filter.WeekDays.Contains(minute.Start.DayOfWeek) == false)
        //        return false;

        //    if(Filter.Hours.Count > 0 && Filter.Hours.Contains(minute.Hour) == false)
        //        return false;

        //    if(Filter.Minutes.Count > 0 && Filter.Minutes.Contains(minute.Minute) == false)
        //        return false;

        //    return CheckExcludePeriods(minute);
        //}

        protected virtual void OnVisitEnd() {
            if(IsDebugEnabled)
                log.Debug("Calendar Visit를 종료합니다...");
        }
        }
}