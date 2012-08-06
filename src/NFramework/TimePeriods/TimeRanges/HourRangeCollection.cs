using System;
using System.Collections.Generic;
using System.Linq;

namespace NSoft.NFramework.TimePeriods.TimeRanges {
    /// <summary>
    /// <see cref="HourRange"/>의 컬렉션
    /// </summary>
    [Serializable]
    public sealed class HourRangeCollection : HourTimeRange {
        public HourRangeCollection(DateTime moment, int hourCount) : this(moment, hourCount, new TimeCalendar()) {}
        public HourRangeCollection(DateTime moment, int hourCount, ITimeCalendar calendar) : base(moment, hourCount, calendar) {}

        public HourRangeCollection(int year, int month, int day, int hour, int hourCount)
            : this(year, month, day, hour, hourCount, new TimeCalendar()) {}

        public HourRangeCollection(int year, int month, int day, int hour, int hourCount, ITimeCalendar calendar)
            : base(year, month, day, hour, hourCount, calendar) {}

        public IEnumerable<HourRange> GetHours() {
            var startHour = TimeTool.TrimToHour(Start, StartHour);

#if !SILVERLIGHT
            return
                ParallelEnumerable
                    .Range(0, HourCount)
                    .AsOrdered()
                    .Select(h => new HourRange(startHour.AddHours(h), TimeCalendar));
#else
			return
				Enumerable
					.Range(0, HourCount)
					.Select(h => new HourRange(startHour.AddHours(h), TimeCalendar));
#endif
        }

        protected override string Format(ITimeFormatter formatter) {
            var fmt = formatter ?? TimeFormatter.Instance;

            return fmt.GetCalendarPeriod(fmt.GetShortDate(Start),
                                         fmt.GetShortDate(End),
                                         fmt.GetShortTime(Start),
                                         fmt.GetShortTime(End),
                                         Duration);
        }
    }
}