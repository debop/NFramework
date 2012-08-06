using System;
using System.Collections.Generic;
using System.Linq;

namespace NSoft.NFramework.TimePeriods.TimeRanges {
    /// <summary>
    /// 반기(Half Year) 단위의 기간을 표현하는 클래스입니다.
    /// </summary>
    [Serializable]
    public abstract class HalfyearTimeRange : YearCalendarTimeRange {
        protected HalfyearTimeRange(int startYear, HalfyearKind startHalfyearKind, int halfyearCount, ITimeCalendar calendar)
            : base(GetPeriodOf(calendar.YearBaseMonth, startYear, startHalfyearKind, halfyearCount), calendar) {
            StartYear = startYear;
            StartHalfyear = startHalfyearKind;

            HalfyearCount = halfyearCount;

            var endYearAndHalfyear = TimeTool.AddHalfyear(startHalfyearKind, startYear, halfyearCount - 1);
            EndYear = endYearAndHalfyear.Year ?? StartYear;
            EndHalfyear = endYearAndHalfyear.Halfyear ?? StartHalfyear;
        }

        public override int BaseYear {
            get { return StartYear; }
        }

        public new int StartYear { get; private set; }

        public HalfyearKind StartHalfyear { get; private set; }

        public new int EndYear { get; private set; }

        public HalfyearKind EndHalfyear { get; private set; }

        public int HalfyearCount { get; private set; }

        public string StartHalfyearName {
            get { return TimeCalendar.GetHalfYearName(StartHalfyear); }
        }

        public string StartHalfyearOfYearName {
            get { return TimeCalendar.GetHalfYearOfYearName(StartYear, StartHalfyear); }
        }

        public string EndHalfyearName {
            get { return TimeCalendar.GetHalfYearName(EndHalfyear); }
        }

        public string EndHalfyearOfYearName {
            get { return TimeCalendar.GetHalfYearOfYearName(EndYear, EndHalfyear); }
        }

        public bool IsCalendarHalfyear {
            get { return (YearBaseMonth - 1) % TimeSpec.MonthsPerHalfyear == 0; }
        }

        /// <summary>
        /// 반기(HalfYear) 기간의 시작 년도와 완료 년도가 다른가 여부
        /// </summary>
        public virtual bool MultipleCalendarYears {
            get {
                if(IsCalendarHalfyear)
                    return false;

                int startYear;
                int month;
                var monthCount = ((int)StartHalfyear - 1) * TimeSpec.MonthsPerHalfyear;
                TimeTool.AddMonth(StartYear, YearBaseMonth, monthCount, out startYear, out month);

                int endYear;
                monthCount += HalfyearCount * TimeSpec.MonthsPerHalfyear;
                TimeTool.AddMonth(StartYear, YearBaseMonth, monthCount, out endYear, out month);

                return (startYear != endYear);
            }
        }

        /// <summary>
        /// 기간에 속한 모든 분기를 컬렉션으로 반환합니다.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<QuarterRange> GetQuarters() {
            var quarterCount = HalfyearCount * TimeSpec.QuartersPerHalfyear;
            var startQuarter = ((int)StartHalfyear - 1) * TimeSpec.QuartersPerHalfyear;

            for(var q = 0; q < quarterCount; q++) {
                var targetQuarter = (startQuarter + q) % TimeSpec.QuartersPerYear;
                var year = BaseYear + (targetQuarter / TimeSpec.QuartersPerYear);

                yield return new QuarterRange(year, (QuarterKind)(targetQuarter + 1), TimeCalendar);
            }
        }

        /// <summary>
        /// 기간에 속한 모든 Month를 컬렉션으로 반환합니다.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MonthRange> GetMonths() {
            var startMonth = new DateTime(StartYear, YearBaseMonth, 1);
            var monthCount = HalfyearCount * TimeSpec.MonthsPerHalfyear;

            return
                Enumerable
                    .Range(0, monthCount)
#if !SILVERLIGHT
                    .AsParallel()
                    .AsOrdered()
#endif
                    .Select(m => new MonthRange(startMonth.AddMonths(m), TimeCalendar));
        }

        public override int GetHashCode() {
            return HashTool.Compute(base.GetHashCode(), StartHalfyear, EndHalfyear, HalfyearCount);
        }

        private static TimeRange GetPeriodOf(int baseMonth, int year, HalfyearKind halfyear, int halfyearCount) {
            halfyearCount.ShouldBePositive("halfyearCount");

            var yearStart = new DateTime(year, baseMonth, 1);
            var start = yearStart.AddMonths(((int)halfyear - 1) * TimeSpec.MonthsPerHalfyear);
            var end = start.AddMonths(halfyearCount * TimeSpec.MonthsPerHalfyear);

            return new TimeRange(start, end);
        }
    }
}