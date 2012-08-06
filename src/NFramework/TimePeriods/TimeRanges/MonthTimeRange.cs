using System;
using System.Collections.Generic;
using System.Linq;

namespace NSoft.NFramework.TimePeriods.TimeRanges {
    /// <summary>
    /// 월 (Month) 단위의 기간
    /// </summary>
    [Serializable]
    public abstract class MonthTimeRange : CalendarTimeRange, IEquatable<MonthTimeRange> {
        protected MonthTimeRange(DateTime moment, int monthCount, ITimeCalendar calendar)
            : this(calendar.GetYear(moment), calendar.GetMonth(moment), monthCount, calendar) {}

        protected MonthTimeRange(int startYear, int startMonth, int monthCount)
            : this(startYear, startMonth, monthCount, new TimeCalendar()) {}

        protected MonthTimeRange(int startYear, int startMonth, int monthCount, ITimeCalendar calendar) :
            base(GetPeriodOf(startYear, startMonth, monthCount), calendar) {
            monthCount.ShouldBePositive("monthCount");
            MonthCount = monthCount;

            var yearAndMonth = TimeTool.AddMonth(startYear, startMonth, monthCount - 1);

            EndYear = yearAndMonth.Year ?? 0;
            EndMonth = yearAndMonth.Month ?? 1;
        }

        public int MonthCount { get; private set; }

        public new int EndYear { get; private set; }

        public new int EndMonth { get; private set; }

        public string StartMonthName {
            get { return TimeCalendar.GetMonthName(StartMonth); }
        }

        public string StartMonthOfYearName {
            get { return TimeCalendar.GetMonthOfYearName(StartYear, StartMonth); }
        }

        public string EndMonthName {
            get { return TimeCalendar.GetMonthName(EndMonth); }
        }

        public string EndMonthOfYearName {
            get { return TimeCalendar.GetMonthOfYearName(EndYear, EndMonth); }
        }

        /// <summary>
        /// 기간을 <see cref="DayRange"/>로 열거합니다.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DayRange> GetDays() {
            var startMonth = TimeTool.StartTimeOfMonth(Start);

            return
                Enumerable
                    .Range(0, MonthCount)
#if !SILVERLIGHT
                    .AsParallel()
                    .AsOrdered()
#endif
                    .SelectMany(m => {
                                    var monthStart = startMonth.AddMonths(m);
                                    var daysOfMonth = TimeTool.GetDaysInMonth(monthStart.Year, monthStart.Month);

                                    return
                                        Enumerable.Range(0, daysOfMonth)
                                            .Select(d => new DayRange(monthStart.AddDays(d), TimeCalendar));
                                });
        }

        public bool Equals(MonthTimeRange other) {
            return (other != null) && GetHashCode().Equals(other.GetHashCode());
        }

        public int GetHashCode(MonthTimeRange obj) {
            return HashTool.Compute(base.GetHashCode(), MonthCount);
        }

        private static ITimePeriod GetPeriodOf(int year, int month, int monthCount) {
            var start = new DateTime(year, month, 1);
            return new TimeRange(start, start.AddMonths(monthCount));
        }
    }
}