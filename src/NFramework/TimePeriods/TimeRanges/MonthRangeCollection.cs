using System;
using System.Collections.Generic;
using System.Linq;

namespace NSoft.NFramework.TimePeriods.TimeRanges {
    /// <summary>
    /// 월 단위의 컬렉션
    /// </summary>
    [Serializable]
    public sealed class MonthRangeCollection : MonthTimeRange {
        public MonthRangeCollection(DateTime moment, int monthCount) : this(moment, monthCount, new TimeCalendar()) {}
        public MonthRangeCollection(DateTime moment, int monthCount, ITimeCalendar calendar) : base(moment, monthCount, calendar) {}

        public MonthRangeCollection(int year, int month, int monthCount) : this(year, month, monthCount, new TimeCalendar()) {}

        public MonthRangeCollection(int year, int month, int monthCount, ITimeCalendar calendar)
            : base(year, month, monthCount, calendar) {}

        public IEnumerable<MonthRange> GetMonths() {
            var startTime = Start;
            return
                Enumerable
                    .Range(0, MonthCount)
#if !SILVERLIGHT
                    .AsParallel()
                    .AsOrdered()
#endif
                    .Select(m => new MonthRange(startTime.AddMonths(m), TimeCalendar));
        }

        protected override string Format(ITimeFormatter formatter) {
            var fmt = formatter ?? TimeFormatter.Instance;

            return fmt.GetCalendarPeriod(StartMonthName,
                                         EndMonthName,
                                         fmt.GetShortDate(Start),
                                         fmt.GetShortDate(End),
                                         Duration);
        }
    }
}