using System;
using System.Collections.Generic;
using System.Linq;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.TimePeriods.TimeLines;

namespace NSoft.NFramework.TimePeriods.Calendars {
    /// <summary>
    /// 특정 Calendar 기준으로 특정 기간의 기간 (TimeSpan)을 구합니다.
    /// </summary>
    public class CalendarDateDiff {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private readonly CalendarPeriodCollectorFilter _collectorFilter = new CalendarPeriodCollectorFilter();
        private readonly ITimeCalendar _calendar;

        public CalendarDateDiff() : this(TimePeriods.TimeCalendar.NewEmptyOffset()) {}

        public CalendarDateDiff(ITimeCalendar timeCalendar) {
            timeCalendar.ShouldNotBeNull("timeCalendar");
            Guard.Assert(timeCalendar.StartOffset == TimeSpan.Zero, "Calendar의 StartOffset은 TimeSpan.Zero이어야 합니다.");
            Guard.Assert(timeCalendar.EndOffset == TimeSpan.Zero, "Calendar의 StartOffset은 TimeSpan.Zero이어야 합니다.");

            _calendar = timeCalendar;
        }

        public IList<DayOfWeek> WeekDays {
            get { return _collectorFilter.WeekDays; }
        }

        public IList<HourRangeInDay> WorkingHours {
            get { return _collectorFilter.CollectingHours; }
        }

        public IList<DayHourRange> WorkingDayHours {
            get { return _collectorFilter.CollectingDayHours; }
        }

        public ITimeCalendar Calendar {
            get { return _calendar; }
        }

        /// <summary>
        /// 주중 (월~금) 을 Working Day로 추가합니다.
        /// </summary>
        public virtual void AddWorkingWeekDays() {
            AddWeekDays(DayOfWeek.Monday,
                        DayOfWeek.Tuesday,
                        DayOfWeek.Wednesday,
                        DayOfWeek.Thursday,
                        DayOfWeek.Friday);
        }

        /// <summary>
        /// 주말 (토,일) 을 Working Day로 추가합니다.
        /// </summary>
        public virtual void AddWeekendWeekDays() {
            AddWeekDays(DayOfWeek.Saturday,
                        DayOfWeek.Sunday);
        }

        /// <summary>
        /// <paramref name="dayOfWeeks"/>를 WorkingDay에 추가합니다.
        /// </summary>
        /// <param name="dayOfWeeks"></param>
        protected virtual void AddWeekDays(params DayOfWeek[] dayOfWeeks) {
            if(dayOfWeeks != null)
                dayOfWeeks.RunEach(dow => WeekDays.Add(dow));
        }

        /// <summary>
        /// <paramref name="moment"/> 부터 현재 시각까지의 WorkingTime의 기간을 구합니다.
        /// </summary>
        /// <param name="moment"></param>
        /// <returns></returns>
        public TimeSpan Difference(DateTime moment) {
            return Difference(moment, ClockProxy.Clock.Now);
        }

        /// <summary>
        /// <paramref name="fromTime"/> ~ <paramref name="toTime"/> 기간의 WorkingTime 의 기간을 구합니다.
        /// </summary>
        /// <param name="fromTime"></param>
        /// <param name="toTime"></param>
        /// <returns></returns>
        public TimeSpan Difference(DateTime fromTime, DateTime toTime) {
            if(IsDebugEnabled)
                log.Debug("fromTime[{0}] ~ toTime[{1}] 의 WorkingTime 기간을 구합니다.", fromTime, toTime);

            if(Equals(fromTime, toTime))
                return TimeSpan.Zero;

            var filterIsEmpty =
                _collectorFilter.WeekDays.Count == 0 &&
                _collectorFilter.CollectingHours.Count == 0 &&
                _collectorFilter.CollectingDayHours.Count == 0;

            if(filterIsEmpty)
                return
                    new DateDiff(fromTime,
                                 toTime,
                                 _calendar.Culture.Calendar,
                                 _calendar.FirstDayOfWeek,
                                 _calendar.YearBaseMonth)
                        .Difference;

            var differenceRange = new TimeRange(fromTime, toTime);

            var collector = new CalendarPeriodCollector(_collectorFilter,
                                                        new TimeRange(differenceRange.Start.Date,
                                                                      differenceRange.End.Date.AddDays(1)),
                                                        SeekDirection.Forward,
                                                        _calendar);

            // Gap을 계산합니다.
            var gapCalculator = new TimeGapCalculator<TimeRange>(_calendar);
            var gaps = gapCalculator.GetGaps(collector.Periods, differenceRange);

            var difference = gaps.Aggregate(TimeSpan.Zero, (current, gap) => current.Add(gap.Duration));

            if(IsDebugEnabled)
                log.Debug("fromTime[{0}] ~ toTime[{1}] 의 WorkingTime 기간은 [{2}] 입니다!!!", fromTime, toTime, difference);

            return (fromTime < toTime) ? difference : difference.Negate();
        }
    }
}