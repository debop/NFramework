using System;
using System.Collections.Generic;
using System.Linq;

namespace NSoft.NFramework.TimePeriods.TimeRanges {
    /// <summary>
    /// 일 단위로 기간을 나타냅니다.
    /// </summary>
    [Serializable]
    public abstract class DayTimeRange : CalendarTimeRange, IEquatable<DayTimeRange> {
        protected DayTimeRange(DateTime moment, int dayCount, ITimeCalendar calendar)
            : this(calendar.GetYear(moment), calendar.GetMonth(moment), calendar.GetDayOfMonth(moment), dayCount, calendar) {}

        protected DayTimeRange(int startYear, int startMonth, int startDay, int dayCount)
            : this(startYear, startMonth, startDay, dayCount, new TimeCalendar()) {}

        protected DayTimeRange(int startYear, int startMonth, int startDay, int dayCount, ITimeCalendar calendar) :
            base(GetPeriodOf(startYear, startMonth, startDay, dayCount), calendar) {
            DayCount = dayCount;
        }

        public int DayCount { get; private set; }

        public DayOfWeek StartDayOfWeek {
            get { return TimeCalendar.GetDayOfWeek(Start); }
        }

        public string StartDayName {
            get { return TimeCalendar.GetDayName(StartDayOfWeek); }
        }

        public DayOfWeek EndDayOfWeek {
            get { return TimeCalendar.GetDayOfWeek(End); }
        }

        public string EndDayName {
            get { return TimeCalendar.GetDayName(EndDayOfWeek); }
        }

        /// <summary>
        /// 일(Day) 단위의 기간에 속한 시간 단위의 기간 정보 (<see cref="HourRange"/>)의 컬렉션을 반환합니다.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<HourRange> GetHours() {
            var startDay = StartDayStart;

            return
                Enumerable
                    .Range(0, DayCount)
#if !SILVERLIGHT
                    .AsParallel()
                    .AsOrdered()
#endif
                    .SelectMany(d => Enumerable
                                         .Range(0, TimeSpec.HoursPerDay)
                                         .Select(h => new HourRange(startDay.AddHours(d * TimeSpec.HoursPerDay + h), TimeCalendar)));
        }

        public bool Equals(DayTimeRange other) {
            return (other != null) && GetHashCode().Equals(other.GetHashCode());
        }

        public override bool Equals(object obj) {
            return (obj != null) && (obj is DayTimeRange) && Equals((DayTimeRange)obj);
        }

        public override int GetHashCode() {
            return HashTool.Compute(base.GetHashCode(), DayCount);
        }

        private static TimeRange GetPeriodOf(int year, int month, int day, int dayCount) {
            dayCount.ShouldBePositive("dayCount");

            var start = new DateTime(year, month, day);
            var end = start.AddDays(dayCount);

            return new TimeRange(start, end);
        }
    }
}