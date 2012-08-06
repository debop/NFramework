using System;
using NSoft.NFramework.TimePeriods.TimeRanges;

namespace NSoft.NFramework.TimePeriods.Calendars.Seekers {
    [Serializable]
    public sealed class DaySeeker : CalendarVisitor<CalendarVisitorFilter, DaySeekerContext> {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private readonly ITimePeriodCollection _periods = new TimePeriodCollection();

        public DaySeeker() : this(SeekDirection.Forward, new TimeCalendar()) {}
        public DaySeeker(CalendarVisitorFilter filter) : this(filter, null, new TimeCalendar()) {}
        public DaySeeker(SeekDirection? seekDirection) : this(new CalendarVisitorFilter(), seekDirection, new TimeCalendar()) {}

        public DaySeeker(SeekDirection? seekDirection, ITimeCalendar calendar)
            : this(new CalendarVisitorFilter(), seekDirection, calendar) {}

        public DaySeeker(CalendarVisitorFilter filter, SeekDirection? seekDirection, ITimeCalendar calendar)
            : base(filter, TimeRange.Anytime, seekDirection ?? SeekDirection.Forward, calendar) {}

        public ITimePeriodCollection Periods {
            get { return _periods; }
        }

        /// <summary>
        /// <paramref name="start"/> 일부터 <paramref name="offset"/> 만큼의 일수가 지난 후의 날짜를 구합니다.
        /// </summary>
        /// <param name="start">기준 일자</param>
        /// <param name="offset">오프셋</param>
        /// <returns>기준일자로부터 오프셋만큼 떨어진 일자</returns>
        public DayRange FindDay(DayRange start, int offset = 1) {
            if(IsDebugEnabled)
                log.Debug("Day 찾기... 시작일=[{0}], offset=[{1}]", start, offset);

            if(offset == 0)
                return start;

            DaySeekerContext context = new DaySeekerContext(start, offset);

            var visitDir = SeekDirection;

            if(offset < 0)
                visitDir = (visitDir == SeekDirection.Forward) ? SeekDirection.Backward : SeekDirection.Forward;

            StartDayVisit(start, context, visitDir);

            if(IsDebugEnabled)
                log.Debug("Day 찾기... 시작일=[{0}], offset=[{1}], foundDay=[{2}]", start, offset, context.FoundDay);

            return context.FoundDay;
        }

        protected override bool EnterYears(YearRangeCollection yearRangeCollection, DaySeekerContext context) {
            return !context.IsFinished;
        }

        protected override bool EnterMonths(YearRange yearRange, DaySeekerContext context) {
            return !context.IsFinished;
        }

        protected override bool EnterDays(MonthRange month, DaySeekerContext context) {
            return !context.IsFinished;
        }

        protected override bool EnterHours(DayRange day, DaySeekerContext context) {
            return false; // !context.IsFinished;
        }

        protected override bool OnVisitDay(DayRange day, DaySeekerContext context) {
            day.ShouldNotBeNull("dayRange");

            if(context.IsFinished)
                return false;

            if(day.IsSamePeriod(context.StartDay))
                return true;

            if(IsMatchingDay(day, context) == false)
                return true;

            if(CheckLimits(day) == false)
                return true;

            context.ProcessDay(day);

            // context가 찾기를 완료하면, Visit를 중단하도록 합니다.
            return !context.IsFinished;
        }
    }
}