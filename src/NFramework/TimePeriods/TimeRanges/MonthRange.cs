using System;

namespace NSoft.NFramework.TimePeriods.TimeRanges {
    /// <summary>
    /// 월(Month) 단위로 기간을 표현하는 클래스입니다.
    /// </summary>
    [Serializable]
    public sealed class MonthRange : MonthTimeRange {
        public MonthRange() : this(new TimeCalendar()) {}
        public MonthRange(ITimeCalendar calendar) : this(ClockProxy.Clock.Now.Date, calendar) {}
        public MonthRange(DateTime moment) : this(moment, new TimeCalendar()) {}
        public MonthRange(DateTime moment, ITimeCalendar calendar) : this(calendar.GetYear(moment), calendar.GetMonth(moment), calendar) {}
        public MonthRange(int year, int month) : this(year, month, new TimeCalendar()) {}
        public MonthRange(int year, int month, ITimeCalendar calendar) : base(year, month, 1, calendar) {}

        public int Year {
            get { return StartYear; }
        }

        public int Month {
            get { return StartMonth; }
        }

        public string MonthName {
            get { return StartMonthName; }
        }

        public string MonthOfYearName {
            get { return StartMonthOfYearName; }
        }

        public int DaysInMonth {
            get { return TimeTool.GetDaysInMonth(StartYear, StartMonth); }
        }

        public MonthRange GetPreviousMonth() {
            return AddMonths(-1);
        }

        public MonthRange GetNextMonth() {
            return AddMonths(1);
        }

        public MonthRange AddMonths(int months) {
            return new MonthRange(TimeTool.StartTimeOfMonth(Start).AddMonths(months), TimeCalendar);
        }

        protected override string Format(ITimeFormatter formatter) {
            var fmt = formatter ?? TimeFormatter.Instance;

            return fmt.GetCalendarPeriod(MonthOfYearName,
                                         fmt.GetShortDate(Start),
                                         fmt.GetShortDate(End),
                                         Duration);
        }
    }
}