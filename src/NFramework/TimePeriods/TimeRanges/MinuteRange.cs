using System;

namespace NSoft.NFramework.TimePeriods.TimeRanges {
    /// <summary>
    /// 분(Minute)단위의 기간을 나타냅니다.
    /// </summary>
    [Serializable]
    public sealed class MinuteRange : MinuteTimeRange {
        public MinuteRange() : this(new TimeCalendar()) {}
        public MinuteRange(ITimeCalendar calendar) : this(ClockProxy.Clock.Now, calendar) {}
        public MinuteRange(DateTime moment) : this(moment, new TimeCalendar()) {}
        public MinuteRange(DateTime moment, ITimeCalendar calendar) : base(moment, 1, calendar) {}

        public MinuteRange(int year, int month, int day, int hour, int minute)
            : this(year, month, day, hour, minute, new TimeCalendar()) {}

        public MinuteRange(int year, int month, int day, int hour, int minute, ITimeCalendar calendar)
            : base(year, month, day, hour, minute, 1, calendar) {}

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

        public int Minute {
            get { return StartMinute; }
        }

        public MinuteRange GetPreviousMinute() {
            return AddMinutes(-1);
        }

        public MinuteRange GetNextMinute() {
            return AddMinutes(1);
        }

        public MinuteRange AddMinutes(int minutes) {
            var start = Start.Date.SetTimePart(StartHour, StartMinute, 0, 0);
            return new MinuteRange(start.AddMinutes(minutes), TimeCalendar);
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