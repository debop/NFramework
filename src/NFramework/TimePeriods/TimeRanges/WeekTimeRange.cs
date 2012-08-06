using System;
using System.Collections.Generic;
using System.Linq;

namespace NSoft.NFramework.TimePeriods.TimeRanges {
    /// <summary>
    /// 주단위의 기간
    /// </summary>
    [Serializable]
    public abstract class WeekTimeRange : CalendarTimeRange, IEquatable<WeekTimeRange> {
        protected WeekTimeRange(ITimePeriod period, ITimeCalendar timeCalendar)
            : base(period, timeCalendar) {
            Year = period.Start.Year;
            StartWeek = WeekTool.GetYearAndWeek(period.Start, timeCalendar.Culture).Week ?? 1;
            WeekCount = 1;
        }

        protected WeekTimeRange(DateTime moment, int weekCount, ITimeCalendar timeTimeCalendar)
            : base(GetPeriodOf(moment, weekCount, timeTimeCalendar), timeTimeCalendar) {
            var yearAndWeek = WeekTool.GetYearAndWeek(moment, timeTimeCalendar);

            Year = yearAndWeek.Year ?? 0;
            StartWeek = yearAndWeek.Week ?? 1;
            WeekCount = weekCount;
        }

        protected WeekTimeRange(int year, int week, int weekCount) : this(year, week, weekCount, new TimeCalendar()) {}

        protected WeekTimeRange(int year, int week, int weekCount, ITimeCalendar timeCalendar)
            : base(GetPeriodOf(year, week, weekCount, timeCalendar), timeCalendar) {
            Year = year;
            StartWeek = week;
            WeekCount = weekCount;
        }

        /// <summary>
        /// 시작 주차의 년도
        /// </summary>
        public int Year { get; private set; }

        /// <summary>
        /// 시작 주차의 주
        /// </summary>
        public int StartWeek { get; private set; }

        /// <summary>
        /// 주차 수 (기간을 Week의 수로 표현)
        /// </summary>
        public int WeekCount { get; private set; }

        /// <summary>
        /// 마지막 주차 (년도가 바뀔 수 있으므로 같은 해의 주차로 볼 수는 없다)
        /// </summary>
        public int EndWeek {
            get { return StartWeek + WeekCount - 1; }
        }

        public string StartWeekOfYearName {
            get { return TimeCalendar.GetWeekOfYearName(Year, StartWeek); }
        }

        public string EndWeekOfYearName {
            get { return TimeCalendar.GetWeekOfYearName(Year, EndWeek); }
        }

        /// <summary>
        /// 주 단위의 기간에 포함된 Day의 컬렉션을 반환합니다.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DayRange> GetDays() {
            var startDay = TimeTool.GetStartOfYearWeek(Year, StartWeek, TimeCalendar.Culture, TimeCalendar.WeekOfYearRule);
            var dayCount = WeekCount * TimeSpec.DaysPerWeek;

            return
                Enumerable.Range(0, dayCount)
#if !SILVERLIGHT
                    .AsParallel()
                    .AsOrdered()
#endif
                    .Select(d => new DayRange(startDay.AddDays(d), TimeCalendar));
        }

        public override bool IsSamePeriod(ITimePeriod other) {
            return (other != null) && (other is WeekTimeRange) && Equals((WeekTimeRange)other);
        }

        public bool Equals(WeekTimeRange other) {
            return (other != null) && GetHashCode().Equals(other.GetHashCode());
        }

        public override bool Equals(object obj) {
            return (obj != null) && (obj is WeekTimeRange) && Equals((WeekTimeRange)obj);
        }

        public override int GetHashCode() {
            return HashTool.Compute(base.GetHashCode(), Year, StartWeek, WeekCount);
        }

        private static TimeRange GetPeriodOf(DateTime moment, int weekCount, ITimeCalendar timeCalendar) {
            var startOfWeek = TimeTool.StartTimeOfWeek(moment, (DayOfWeek?)timeCalendar.FirstDayOfWeek);
            return new TimeRange(startOfWeek, TimePeriods.DurationUtil.Weeks(weekCount));
        }

        private static TimeRange GetPeriodOf(int year, int weekOfYear, int weekCount, ITimeCalendar timeCalendar) {
            weekCount.ShouldBePositive("weekCount");

            var startWeek = WeekTool.GetWeekRange(new YearAndWeek(year, weekOfYear), timeCalendar.Culture, timeCalendar.WeekOfYearRule);

            var startDay = startWeek.Start;
            var endDay = startDay.AddDays(weekCount * TimeSpec.DaysPerWeek);

            return new TimeRange(startDay, endDay);
        }
    }
}