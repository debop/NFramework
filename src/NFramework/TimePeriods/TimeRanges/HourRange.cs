using System;

namespace NSoft.NFramework.TimePeriods.TimeRanges {
    /// <summary>
    /// 시간(Hour) 단위의 기간을 표현합니다.
    /// </summary>
    [Serializable]
    public class HourRange : HourTimeRange {
        public HourRange() : this(new TimeCalendar()) {}
        public HourRange(ITimeCalendar calendar) : this(ClockProxy.Clock.Now, calendar) {}
        public HourRange(DateTime moment) : this(moment, new TimeCalendar()) {}
        public HourRange(DateTime moment, ITimeCalendar calendar) : base(moment, 1, calendar) {}

        public HourRange(int year, int month, int day, int hour) : this(year, month, day, hour, new TimeCalendar()) {}
        public HourRange(int year, int month, int day, int hour, ITimeCalendar calendar) : base(year, month, day, hour, 1, calendar) {}

        public int Year {
            get { return StartYear; }
        }

        public int Month {
            get { return StartMonth; }
        }

        public int Day {
            get { return StartDay; }
        }

        public int Hour {
            get { return StartHour; }
        }

        public HourRange GetPreviousHour() {
            return AddHours(-1);
        }

        public HourRange GetNextHour() {
            return AddHours(1);
        }

        public HourRange AddHours(int hours) {
            var startHour = Start.Date.SetTimePart(Hour);
            return new HourRange(startHour.AddHours(hours), TimeCalendar);
        }

        protected override string Format(ITimeFormatter formatter) {
            var fmt = formatter ?? TimeFormatter.Instance;

            return fmt.GetCalendarPeriod(fmt.GetShortDate(Start),
                                         fmt.GetShortTime(Start),
                                         fmt.GetShortTime(End),
                                         Duration);
        }
    }
}