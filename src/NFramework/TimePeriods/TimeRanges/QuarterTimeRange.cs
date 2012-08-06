using System;
using System.Collections.Generic;
using System.Linq;

namespace NSoft.NFramework.TimePeriods.TimeRanges {
    /// <summary>
    /// 분기(Quarter) 단위의 기간을 표현합니다.
    /// </summary>
    [Serializable]
    public abstract class QuarterTimeRange : YearCalendarTimeRange, IEquatable<QuarterTimeRange> {
        protected QuarterTimeRange(int startYear, QuarterKind startQuarter, int quarterCount)
            : this(startYear, startQuarter, quarterCount, new TimeCalendar()) {}

        protected QuarterTimeRange(int startYear, QuarterKind startQuarter, int quarterCount, ITimeCalendar calendar)
            : base(GetPeriodOf(calendar, startYear, startQuarter.GetHashCode(), quarterCount), calendar) {
            StartYear = startYear;

            StartQuarter = startQuarter;
            QuarterCount = quarterCount;

            var endYearQuarter = TimeTool.AddQuarter(startQuarter, startYear, quarterCount - 1);

            EndYear = endYearQuarter.Year ?? StartYear;
            EndQuarter = endYearQuarter.Quarter ?? StartQuarter;
        }

        public override int BaseYear {
            get { return StartYear; }
        }

        public new int StartYear { get; private set; }

        public QuarterKind StartQuarter { get; private set; }

        public new int EndYear { get; private set; }

        public QuarterKind EndQuarter { get; private set; }

        public int QuarterCount { get; private set; }

        public string StartQuarterName {
            get { return TimeCalendar.GetQuarterName(StartQuarter); }
        }

        public string StartQuarterOfYearName {
            get { return TimeCalendar.GetQuarterOfYearName(StartYear, StartQuarter); }
        }

        public string EndQuarterName {
            get { return TimeCalendar.GetQuarterName(EndQuarter); }
        }

        public string EndQuarterOfYearName {
            get { return TimeCalendar.GetQuarterOfYearName(EndYear, EndQuarter); }
        }

        public new int StartMonth {
            get {
                var monthCount = ((int)StartQuarter - 1) * TimeSpec.MonthsPerQuarter;

                int year;
                int startMonth;
                TimeTool.AddMonth(StartYear, YearBaseMonth, monthCount, out year, out startMonth);

                return startMonth;
            }
        }

        public new int EndMonth {
            get {
                var monthCount = ((int)StartQuarter - 1) * TimeSpec.MonthsPerQuarter + QuarterCount * TimeSpec.MonthsPerQuarter;

                int year;
                int endMonth;

                TimeTool.AddMonth(StartYear, YearBaseMonth, monthCount, out year, out endMonth);
                return endMonth;
            }
        }

        public virtual bool IsCalendarQuarter {
            get { return ((int)YearBaseMonth - 1) % TimeSpec.MonthsPerQuarter == 0; }
        }

        /// <summary>
        /// 시작 분기와 완료 분기가 다른 년도인지 판단합니다.
        /// </summary>
        public virtual bool MultipleCalendarYears {
            get {
                if(IsCalendarQuarter) {
                    return false;
                }
                int startYear;
                int month;
                var monthCount = ((int)StartQuarter - 1) * TimeSpec.MonthsPerQuarter;
                TimeTool.AddMonth(StartYear, YearBaseMonth, monthCount, out startYear, out month);

                int endYear;
                monthCount += QuarterCount * TimeSpec.MonthsPerQuarter;
                TimeTool.AddMonth(StartYear, YearBaseMonth, monthCount, out endYear, out month);
                return startYear != endYear;
            }
        }

        /// <summary>
        /// 내부의 모든 Month를 열거합니다.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MonthRange> GetMonths() {
            var startMonth = new DateTime(StartYear, YearBaseMonth, 1);
            var monthCount = QuarterCount * TimeSpec.MonthsPerQuarter;

            return
                Enumerable
                    .Range(0, monthCount)
#if !SILVERLIGHT
                    .AsParallel()
                    .AsOrdered()
#endif
                    .Select(m => new MonthRange(startMonth.AddMonths(m), TimeCalendar));
        }

        public bool Equals(QuarterTimeRange other) {
            return (other != null) && GetHashCode().Equals(other.GetHashCode());
        }

        public override int GetHashCode() {
            return HashTool.Compute(base.GetHashCode(), StartQuarter, QuarterCount, EndQuarter);
        }

        private static ITimePeriod GetPeriodOf(ITimeCalendar calendar, int year, int quarter, int quarterCount) {
            quarterCount.ShouldBePositive("quarterCount");

            var yearStart = new DateTime(year, calendar.YearBaseMonth, 1);
            var start = yearStart.AddMonths((quarter - 1) * TimeSpec.MonthsPerQuarter);
            var end = start.AddMonths(quarterCount * TimeSpec.MonthsPerQuarter);

            return new TimeRange(start, end);
        }
    }
}