using System;
using System.Collections.Generic;
using System.Linq;

namespace NSoft.NFramework.TimePeriods.TimeRanges {
    /// <summary>
    /// 년(Year) 단위로 기간을 표현하는 클래스입니다.
    /// </summary>
    [Serializable]
    public abstract class YearTimeRange : YearCalendarTimeRange {
        protected YearTimeRange(DateTime moment, int yearCount, ITimeCalendar calendar)
            : this(TimeTool.GetYearOf(calendar, moment), yearCount, calendar) {}

        protected YearTimeRange(int startYear, int yearCount, ITimeCalendar calendar)
            : base(GetPeriodOf(calendar.YearBaseMonth, startYear, yearCount), calendar) {
            YearCount = yearCount;
        }

        /// <summary>
        /// 한 해의 시작 월
        /// </summary>
        public override int BaseYear {
            get { return StartYear; }
        }

        /// <summary>
        /// 현 기간이 가지는 년의 수
        /// </summary>
        public int YearCount { get; private set; }

        /// <summary>
        /// 시작 년의 명칭
        /// </summary>
        public string StartYearName {
            get { return TimeCalendar.GetYearName(StartYear); }
        }

        /// <summary>
        /// 완료 년의 명칭
        /// </summary>
        public string EndYearName {
            get { return TimeCalendar.GetYearName(EndYear); }
        }

        /// <summary>
        /// 현 기간에 속한 반기 단위의 기간들을 열거합니다.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<HalfyearRange> GetHalfyears() {
            var startYear = StartYear;

#if !SILVERLIGHT

            return
                Enumerable
                    .Range(0, YearCount)
                    .AsParallel()
                    .AsOrdered()
                    .SelectMany(y => Enum.GetValues(typeof(HalfyearKind))
                                         .Cast<HalfyearKind>()
                                         .Select(hy => new HalfyearRange(startYear + y, hy, TimeCalendar)));
#else
			return
				Enumerable
					.Range(0, YearCount)
					.SelectMany(y => Enumerable.Range(1, 2)
					                 	.Cast<HalfyearKind>()
					                 	.Select(hy => new HalfyearRange(startYear + y, hy, TimeCalendar)));
#endif
        }

        /// <summary>
        /// 현 기간에 속한 분기들을 열거합니다.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<QuarterRange> GetQuarters() {
            var startYear = StartYear;

#if !SILVERLIGHT
            return
                Enumerable
                    .Range(0, YearCount)
                    .AsParallel()
                    .AsOrdered()
                    .SelectMany(y => Enum.GetValues(typeof(QuarterKind))
                                         .Cast<QuarterKind>()
                                         .Select(q => new QuarterRange(startYear + y, q, TimeCalendar)));
#else
			return
				Enumerable
					.Range(0, YearCount)
					.SelectMany(y => Enumerable.Range(1, 4)
										.Cast<QuarterKind>()
										.Select(q => new QuarterRange(startYear + y, q, TimeCalendar)));
#endif
        }

        /// <summary>
        /// Calendar의 Year의 시작 Month부터 제공합니다.
        /// </summary>
        public virtual IEnumerable<MonthRange> GetMonths() {
            var startDate = new DateTime(StartYear, YearBaseMonth, 1);
            var monthCount = YearCount * TimeSpec.MonthsPerYear;

            return
                Enumerable
                    .Range(0, monthCount)
#if !SILVERLIGHT
                    .AsParallel()
                    .AsOrdered()
#endif
                    .Select(m => new MonthRange(startDate.AddMonths(m), TimeCalendar));
        }

        public override int GetHashCode() {
            return HashTool.Compute(base.GetHashCode(), YearCount);
        }

        private static TimeRange GetPeriodOf(int month, int year, int yearCount) {
            yearCount.ShouldBePositive("yearCount");

            var start = new DateTime(year, month, 1);
            return new TimeRange(start, start.AddYears(yearCount));
        }
    }
}