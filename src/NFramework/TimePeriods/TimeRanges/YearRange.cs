using System;

namespace NSoft.NFramework.TimePeriods.TimeRanges {
    /// <summary>
    /// 1년의 기간을 나타냅니다.
    /// </summary>
    [Serializable]
    public sealed class YearRange : YearTimeRange {
        public YearRange() : this(new TimeCalendar()) {}
        public YearRange(ITimeCalendar calendar) : this(ClockProxy.Clock.Now, calendar) {}
        public YearRange(DateTime moment) : this(moment, new TimeCalendar()) {}
        public YearRange(DateTime moment, ITimeCalendar calendar) : this(TimeTool.GetYearOf(calendar, moment), calendar) {}
        public YearRange(int year) : this(year, new TimeCalendar()) {}
        public YearRange(int year, ITimeCalendar calendar) : base(year, 1, calendar) {}

        public int YearValue {
            get { return StartYear; }
        }

        public string YearName {
            get { return StartYearName; }
        }

        public bool IsCalendarYear {
            get { return YearBaseMonth == TimeSpec.CalendarYearStartMonth; }
        }

        public YearRange GetPreviousYear() {
            return AddYears(-1);
        }

        public YearRange GetNextYear() {
            return AddYears(1);
        }

        public YearRange AddYears(int value) {
            var startTime = new DateTime(StartYear, YearBaseMonth, 1);
            return new YearRange(startTime.AddYears(value), TimeCalendar);
        }
    }
}