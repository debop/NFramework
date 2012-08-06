using System;

namespace NSoft.NFramework.TimePeriods.TimeRanges {
    /// <summary>
    /// 한 분기를 나타내는 클래스입니다.
    /// </summary>
    [Serializable]
    public sealed class QuarterRange : QuarterTimeRange {
        public QuarterRange() : this(new TimeCalendar()) {}
        public QuarterRange(ITimeCalendar calendar) : this(ClockProxy.Clock.Now.Date, calendar) {}
        public QuarterRange(DateTime moment) : this(moment, new TimeCalendar()) {}

        public QuarterRange(DateTime moment, ITimeCalendar calendar) :
            this(TimeTool.GetYearOf(calendar.YearBaseMonth, calendar.GetYear(moment), calendar.GetMonth(moment)),
                 TimeTool.GetQuarterOfMonth(calendar.YearBaseMonth, moment.Month),
                 calendar) {}

        public QuarterRange(int year, QuarterKind quarter) : this(year, quarter, new TimeCalendar()) {}
        public QuarterRange(int year, QuarterKind quarter, ITimeCalendar calendar) : base(year, quarter, 1, calendar) {}

        /// <summary>
        /// 해당 년도
        /// </summary>
        public int Year {
            get { return StartYear; }
        }

        /// <summary>
        /// 분기
        /// </summary>
        public QuarterKind Quarter {
            get { return StartQuarter; }
        }

        /// <summary>
        /// 분기명
        /// </summary>
        public string QuarterName {
            get { return StartQuarterName; }
        }

        /// <summary>
        /// 쿼터 명
        /// </summary>
        public string QuarterOfYearName {
            get { return StartQuarterOfYearName; }
        }

        /// <summary>
        /// 전분기
        /// </summary>
        /// <returns></returns>
        public QuarterRange GetPreviousQuarter() {
            return AddQuarters(-1);
        }

        /// <summary>
        /// 다음 분기
        /// </summary>
        /// <returns></returns>
        public QuarterRange GetNextQuarter() {
            return AddQuarters(1);
        }

        /// <summary>
        /// 현 분기에서 <paramref name="count"/> 수만큼 분기를 더한다.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public QuarterRange AddQuarters(int count) {
            var yearQuarter = TimeTool.AddQuarter(StartQuarter, StartYear, count);
            return new QuarterRange(yearQuarter.Year ?? 0, yearQuarter.Quarter ?? QuarterKind.First, TimeCalendar);
        }

        protected override string Format(ITimeFormatter formatter) {
            var fmt = formatter ?? TimeFormatter.Instance;

            return fmt.GetCalendarPeriod(QuarterOfYearName,
                                         fmt.GetShortDate(Start),
                                         fmt.GetShortDate(End),
                                         Duration);
        }
    }
}