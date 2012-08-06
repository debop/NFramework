using System;

namespace NSoft.NFramework.TimePeriods.TimeRanges {
    /// <summary>
    /// 년(Year) 단위의 기간을 표현합니다.
    /// </summary>
    [Serializable]
    public abstract class YearCalendarTimeRange : CalendarTimeRange {
        protected YearCalendarTimeRange(ITimePeriod period, ITimeCalendar calendar) : base(period, calendar) {}

        /// <summary>
        /// Calendar의 시작 월
        /// </summary>
        public int YearBaseMonth {
            get { return TimeCalendar.YearBaseMonth; }
        }

        /// <summary>
        /// Calendar의 시작 년
        /// </summary>
        public virtual int BaseYear {
            get { return StartYear; }
        }
    }
}