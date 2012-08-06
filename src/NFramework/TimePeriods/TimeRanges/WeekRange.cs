using System;

namespace NSoft.NFramework.TimePeriods.TimeRanges {
    /// <summary>
    /// 한 주(Week)를 기간으로 가지는 클래스입니다.
    /// </summary>
    [Serializable]
    public sealed class WeekRange : WeekTimeRange {
        public WeekRange() : this(new TimeCalendar()) {}
        public WeekRange(ITimeCalendar calendar) : this(ClockProxy.Clock.Now, calendar) {}
        public WeekRange(DateTime moment) : this(moment, new TimeCalendar()) {}
        public WeekRange(DateTime moment, ITimeCalendar calendar) : base(moment, 1, calendar) {}

        internal WeekRange(ITimePeriod period, ITimeCalendar calendar) : base(period, calendar) {}

        public WeekRange(int year, int startWeek) : this(year, startWeek, new TimeCalendar()) {}
        public WeekRange(int year, int startWeek, ITimeCalendar calendar) : base(year, startWeek, 1, calendar) {}

        public int WeekOfYear {
            get { return StartWeek; }
        }

        public string WeekOfYearName {
            get { return StartWeekOfYearName; }
        }

        public DateTime FirstDayOfWeek {
            get { return Start; }
        }

        public DateTime LastDayOfWeek {
            get { return Start.AddDays(TimeSpec.DaysPerWeek - 1); }
        }

        /// <summary>
        /// 한 주의 시작일과 종료일의 년도가 다른가?
        /// </summary>
        public bool MultipleCalendarYears {
            get { return TimeCalendar.GetYearOf(FirstDayOfWeek) != TimeCalendar.GetYearOf(LastDayOfWeek); }
        }

        public WeekRange GetPreviousWeek() {
            return AddWeeks(-1);
        }

        public WeekRange GetNextWeek() {
            return AddWeeks(1);
        }

        public WeekRange AddWeeks(int weeks) {
            var startOfWeek = TimeTool.GetStartOfYearWeek(Year, StartWeek, TimeCalendar.Culture, TimeCalendar.WeekOfYearRule);
            return new WeekRange(startOfWeek.AddDays(weeks * TimeSpec.DaysPerWeek), TimeCalendar);
        }

        protected override string Format(ITimeFormatter formatter) {
            var fmt = formatter ?? TimeFormatter.Instance;

            return formatter.GetCalendarPeriod(WeekOfYearName,
                                               fmt.GetShortDate(Start),
                                               fmt.GetShortDate(End),
                                               Duration);
        }
    }
}