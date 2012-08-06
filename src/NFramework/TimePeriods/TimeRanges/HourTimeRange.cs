using System;
using System.Collections.Generic;
using System.Linq;

namespace NSoft.NFramework.TimePeriods.TimeRanges {
    /// <summary>
    /// 시간(Hour) 단위로 기간을 표현하는 클래스입니다.
    /// </summary>
    [Serializable]
    public abstract class HourTimeRange : CalendarTimeRange, IEquatable<HourTimeRange> {
        protected HourTimeRange(DateTime moment, int hourCount, ITimeCalendar calendar)
            : this(
                calendar.GetYear(moment), calendar.GetMonth(moment), calendar.GetDayOfMonth(moment), calendar.GetHour(moment), hourCount,
                calendar) {}

        protected HourTimeRange(int startYear, int startMonth, int startDay, int startHour, int hourCount)
            : this(startYear, startMonth, startDay, startHour, hourCount, new TimeCalendar()) {}

        protected HourTimeRange(int startYear, int startMonth, int startDay, int startHour, int hourCount, ITimeCalendar calendar)
            : base(GetPeriodOf(startYear, startMonth, startDay, startHour, hourCount), calendar) {
            HourCount = hourCount;
            EndHour = Start.AddHours(HourCount).Hour;
        }

        public int HourCount { get; private set; }

        public new int EndHour { get; private set; }

        /// <summary>
        /// 시작 시각 ~ 완료 시각 사이의 모든 분단위의 기간을 컬렉션으로 반환합니다.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MinuteRange> GetMinutes() {
            var start = Start;
            return
                Enumerable
                    .Range(0, HourCount)
#if !SILVERLIGHT
                    .AsParallel()
                    .AsOrdered()
#endif
                    .SelectMany(h => Enumerable
                                         .Range(0, TimeSpec.MinutesPerHour)
                                         .Select(m => new MinuteRange(start.AddHours(h).AddMinutes(m), TimeCalendar)));
        }

        public bool Equals(HourTimeRange other) {
            return (other != null) && GetHashCode().Equals(other.GetHashCode());
        }

        public override bool Equals(object obj) {
            return (obj != null) && (obj is HourTimeRange) && Equals((HourTimeRange)obj);
        }

        public override int GetHashCode() {
            return HashTool.Compute(base.GetHashCode(), HourCount);
        }

        private static TimeRange GetPeriodOf(int year, int month, int day, int hour, int hourCount) {
            hourCount.ShouldBePositive("hourCount");

            var start = new DateTime(year, month, day, hour, 0, 0);
            var end = start.AddHours(hourCount);

            return new TimeRange(start, end);
        }
    }
}