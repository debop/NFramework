using System;
using System.Collections.Generic;
using System.Linq;

namespace NSoft.NFramework.TimePeriods.TimeRanges {
    /// <summary>
    /// 반기(Half year) 단위의 기간의 컬렉션을 표현합니다.
    /// </summary>
    [Serializable]
    public sealed class HalfyearRangeCollection : HalfyearTimeRange {
        public HalfyearRangeCollection(DateTime moment, int halfyearCount) : this(moment, halfyearCount, new TimeCalendar()) {}

        public HalfyearRangeCollection(DateTime moment, int halfyearCount, ITimeCalendar calendar)
            : this(TimeTool.GetYearOf(calendar.YearBaseMonth, calendar.GetYear(moment), calendar.GetMonth(moment)),
                   calendar.GetMonth(moment).HalfyearOf(),
                   halfyearCount,
                   calendar) {}

        public HalfyearRangeCollection(int year, HalfyearKind halfyear, int halfyearCount)
            : this(year, halfyear, halfyearCount, new TimeCalendar()) {}

        public HalfyearRangeCollection(int year, HalfyearKind halfyear, int halfyearCount, ITimeCalendar calendar)
            : base(year, halfyear, halfyearCount, calendar) {}

        public IEnumerable<HalfyearRange> GetHalfyears() {
            return
                Enumerable
                    .Range(0, HalfyearCount)
#if !SILVERLIGHT
                    .AsParallel()
                    .AsOrdered()
#endif
                    .Select(index => {
                                var yhy = TimeTool.AddHalfyear(StartHalfyear, StartYear, index);
                                return new HalfyearRange(yhy.Year ?? 0, yhy.Halfyear ?? HalfyearKind.First, TimeCalendar);
                            });
        }

        protected override string Format(ITimeFormatter formatter) {
            var fmt = formatter ?? TimeFormatter.Instance;

            return fmt.GetCalendarPeriod(StartHalfyearOfYearName,
                                         EndHalfyearOfYearName,
                                         fmt.GetShortDate(Start),
                                         fmt.GetShortDate(End),
                                         Duration);
        }
    }
}