using System;

namespace NSoft.NFramework.TimePeriods.TimeRanges {
    /// <summary>
    /// 분(Minute) 단위로 기간을 표현하는 클래스입니다.
    /// </summary>
    [Serializable]
    public abstract class MinuteTimeRange : CalendarTimeRange {
        protected MinuteTimeRange(DateTime moment, int minuteCount, ITimeCalendar calendar) :
            this(
            calendar.GetYear(moment), calendar.GetMonth(moment), calendar.GetDayOfMonth(moment), calendar.GetHour(moment),
            calendar.GetMinute(moment), minuteCount, calendar) {}

        protected MinuteTimeRange(int year, int month, int day, int hour, int minute, int minuteCount)
            : this(year, month, day, hour, minute, minuteCount, new TimeCalendar()) {}

        protected MinuteTimeRange(int year, int month, int day, int hour, int minute, int minuteCount, ITimeCalendar calendar)
            : base(GetPeriodOf(year, month, day, hour, minute, minuteCount), calendar) {
            MinuteCount = minuteCount;
            EndMinute = Start.AddMinutes(MinuteCount).Minute;
        }

        public int MinuteCount { get; private set; }

        public new int EndMinute { get; private set; }

        public override int GetHashCode() {
            return HashTool.Compute(base.GetHashCode(), MinuteCount);
        }

        private static ITimePeriod GetPeriodOf(int year, int month, int day, int hour, int minute, int minuteCount) {
            var start = new DateTime(year, month, day, hour, minute, 0);
            return new TimeRange(start, TimeSpan.FromMinutes(minuteCount));
        }
    }
}