using System;
using System.Collections.Generic;
using System.Linq;

namespace NSoft.NFramework.TimePeriods.TimeRanges {
    /// <summary>
    /// 1년 (Year) 단위를 복수 개를 가진 범위를 나타냅니다.
    /// </summary>
    [Serializable]
    public sealed class YearRangeCollection : YearTimeRange {
        public YearRangeCollection(DateTime moment, int yearCount) : this(moment, yearCount, new TimeCalendar()) {}

        public YearRangeCollection(DateTime moment, int yearCount, ITimeCalendar calendar)
            : this(TimeTool.GetYearOf(calendar, moment), yearCount, calendar) {}

        public YearRangeCollection(int year, int yearCount) : this(year, yearCount, new TimeCalendar()) {}
        public YearRangeCollection(int year, int yearCount, ITimeCalendar calendar) : base(year, yearCount, calendar) {}

        public IEnumerable<YearRange> GetYears() {
            var startYear = StartYear;

            return
                Enumerable
                    .Range(0, YearCount)
#if !SILVERLIGHT
                    .AsParallel()
                    .AsOrdered()
#endif
                    .Select(y => new YearRange(startYear + y, TimeCalendar));
        }

        protected override string Format(ITimeFormatter formatter) {
            return formatter.GetCalendarPeriod(StartYearName,
                                               EndYearName,
                                               formatter.GetShortDate(Start),
                                               formatter.GetShortDate(End),
                                               Duration);
        }
    }
}