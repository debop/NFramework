using System;
using System.Collections.Generic;
using System.Linq;

namespace NSoft.NFramework.TimePeriods.TimeRanges {
    /// <summary>
    /// 분기 단위의 기간에 대한 컬렉션
    /// </summary>
    [Serializable]
    public class QuarterRangeCollection : QuarterTimeRange {
        public QuarterRangeCollection(DateTime moment, int quarterCount) : this(moment, quarterCount, new TimeCalendar()) {}

        public QuarterRangeCollection(DateTime moment, int quarterCount, ITimeCalendar calendar)
            : this(TimeTool.GetYearOf(calendar.YearBaseMonth, calendar.GetYear(moment), calendar.GetMonth(moment)),
                   calendar.GetMonth(moment).QuarterOf(),
                   quarterCount,
                   calendar) {}

        public QuarterRangeCollection(int year, QuarterKind quarter, int quarterCount)
            : this(year, quarter, quarterCount, new TimeCalendar()) {}

        public QuarterRangeCollection(int year, QuarterKind quarter, int quarterCount, ITimeCalendar calendar)
            : base(year, quarter, quarterCount, calendar) {}

        public IEnumerable<QuarterRange> GetQuarters() {
            var startYear = StartYear;
            var startQuarter = StartQuarter;

            return
                Enumerable
                    .Range(0, QuarterCount)
#if !SILVERLIGHT
                    .AsParallel()
                    .AsOrdered()
#endif
                    .Select(q => {
                                var yq = TimeTool.AddQuarter(startQuarter, startYear, q);
                                return new QuarterRange(yq.Year ?? 0, yq.Quarter ?? QuarterKind.First, TimeCalendar);
                            });
        }

        protected override string Format(ITimeFormatter formatter) {
            var fmt = formatter ?? TimeFormatter.Instance;

            return fmt.GetCalendarPeriod(StartQuarterOfYearName,
                                         EndQuarterOfYearName,
                                         fmt.GetShortDate(Start),
                                         fmt.GetShortDate(End),
                                         Duration);
        }
    }
}