using System;
using System.Collections.Generic;
using System.Linq;

namespace NSoft.NFramework.TimePeriods.TimeRanges {
    /// <summary>
    /// 일 단위 기간(<see cref="DayRange"/>)의 컬렉션에 해당합니다.
    /// </summary>
    [Serializable]
    public sealed class DayRangeCollection : DayTimeRange {
        public DayRangeCollection(DateTime moment, int dayCount) : this(moment, dayCount, new TimeCalendar()) {}
        public DayRangeCollection(DateTime moment, int dayCount, ITimeCalendar calendar) : base(moment, dayCount, calendar) {}

        public DayRangeCollection(int year, int month, int day, int dayCount) : this(year, month, day, dayCount, new TimeCalendar()) {}

        public DayRangeCollection(int year, int month, int day, int dayCount, ITimeCalendar calendar)
            : base(year, month, day, dayCount, calendar) {}

        public IEnumerable<DayRange> GetDays() {
            var startDay = Start;

            return
                Enumerable
                    .Range(0, DayCount)
#if !SILVERLIGHT
                    .AsParallel()
                    .AsOrdered()
#endif
                    .Select(d => new DayRange(startDay.AddDays(d), TimeCalendar));
        }

        protected override string Format(ITimeFormatter formatter) {
            var fmt = formatter ?? TimeFormatter.Instance;

            return fmt.GetCalendarPeriod(StartDayName,
                                         EndDayName,
                                         fmt.GetShortDate(Start),
                                         fmt.GetShortDate(End),
                                         Duration);
        }
    }
}