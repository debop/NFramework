using System;

namespace NSoft.NFramework.TimePeriods.TimeRanges {
    /// <summary>
    /// 반기를 기간으로 표현하는 클래스입니다.
    /// </summary>
    [Serializable]
    public sealed class HalfyearRange : HalfyearTimeRange {
        public HalfyearRange() : this(new TimeCalendar()) {}
        public HalfyearRange(ITimeCalendar calendar) : this(ClockProxy.Clock.Now.Date, calendar) {}
        public HalfyearRange(DateTime moment) : this(moment, new TimeCalendar()) {}

        public HalfyearRange(DateTime moment, ITimeCalendar calendar) :
            this(TimeTool.GetYearOf(calendar.YearBaseMonth, calendar.GetYear(moment), calendar.GetMonth(moment)),
                 TimeTool.GetHalfyearOfMonth(calendar.YearBaseMonth, moment.Month),
                 calendar) {}

        public HalfyearRange(int year, HalfyearKind halfyear) : this(year, halfyear, new TimeCalendar()) {}
        public HalfyearRange(int year, HalfyearKind halfyear, ITimeCalendar calendar) : base(year, halfyear, 1, calendar) {}

        public int Year {
            get { return StartYear; }
        }

        public HalfyearKind Halfyear {
            get { return StartHalfyear; }
        }

        public string HalfyearName {
            get { return TimeCalendar.GetHalfYearName(Halfyear); }
        }

        public string HalfyearOfYearName {
            get { return TimeCalendar.GetHalfYearOfYearName(Year, Halfyear); }
        }

        public HalfyearRange GetPreviousHalfyear() {
            return AddHalfyears(-1);
        }

        public HalfyearRange GetNextHalfyear() {
            return AddHalfyears(1);
        }

        public HalfyearRange AddHalfyears(int count) {
            var halfyear = TimeTool.AddHalfyear(Halfyear, Year, count);
            return new HalfyearRange(halfyear.Year ?? 0, halfyear.Halfyear ?? HalfyearKind.First, TimeCalendar);
        }

        protected override string Format(ITimeFormatter formatter) {
            var fmt = formatter ?? TimeFormatter.Instance;

            return fmt.GetCalendarPeriod(HalfyearOfYearName,
                                         fmt.GetShortDate(Start),
                                         fmt.GetShortDate(End),
                                         Duration);
        }
    }
}