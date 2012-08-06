using System;

namespace NSoft.NFramework.TimePeriods.TimeRanges {
    /// <summary>
    /// 일 단위의 기간을 표현합니다.
    /// </summary>
    [Serializable]
    public sealed class DayRange : DayTimeRange {
        public DayRange() : this(new TimeCalendar()) {}
        public DayRange(ITimeCalendar calendar) : this(ClockProxy.Clock.Today, calendar) {}
        public DayRange(DateTime moment) : this(moment, new TimeCalendar()) {}

        public DayRange(DateTime moment, ITimeCalendar calendar)
            : this(calendar.GetYear(moment), calendar.GetMonth(moment), calendar.GetDayOfMonth(moment), calendar) {}

        public DayRange(int year, int month) : this(year, month, 1, new TimeCalendar()) {}
        public DayRange(int year, int month, int day) : this(year, month, day, new TimeCalendar()) {}
        public DayRange(int year, int month, int day, ITimeCalendar calendar) : base(year, month, day, 1, calendar) {}

        public int Year {
            get { return StartYear; }
        }

        public int Month {
            get { return StartMonth; }
        }

        public int Day {
            get { return StartDay; }
        }

        public DayOfWeek DayOfWeek {
            get { return StartDayOfWeek; }
        }

        public string DayName {
            get { return StartDayName; }
        }

        public DayRange GetPreviousDay() {
            return AddDays(-1);
        }

        public DayRange GetNextDay() {
            return AddDays(1);
        }

        public DayRange AddDays(int days) {
            return new DayRange(Start.Date.AddDays(days), TimeCalendar);
        }

        protected override string Format(ITimeFormatter formatter) {
            var fmt = formatter ?? TimeFormatter.Instance;

            return fmt.GetCalendarPeriod(DayName,
                                         fmt.GetShortDate(Start),
                                         fmt.GetShortDate(End),
                                         Duration);
        }
    }
}