using System;

namespace NSoft.NFramework.TimePeriods.TimeRanges {
    /// <summary>
    /// <see cref="TimeCalendar"/> 기준의 기간을 나타냅니다.
    /// </summary>
    [Serializable]
    public class CalendarTimeRange : TimeRange, ICalendarTimeRange, IEquatable<ICalendarTimeRange>, IComparable<CalendarTimeRange> {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 지정된 기간을 <paramref name="mapper"/>를 통해 매핑한 기간으로 반환합니다.
        /// </summary>
        /// <param name="period"></param>
        /// <param name="mapper"></param>
        /// <returns></returns>
        private static TimeRange ToCalendarTimeRange(ITimePeriod period, ITimePeriodMapper mapper) {
            period.ShouldNotBeNull("period");

            mapper = mapper ?? new TimeCalendar();
            var mappedStart = mapper.MapStart(period.Start);
            var mappedEnd = mapper.MapEnd(period.End);

            TimeTool.AssertValidPeriod(mappedStart, mappedEnd);
            var mapped = new TimeRange(mappedStart, mappedEnd);

            if(IsDebugEnabled)
                log.Debug("TimeCalendar 기준의 기간으로 매핑했습니다. period=[{0}], mapped=[{1}]", period, mapped);

            return mapped;
        }

        #region << Constructors >>

        public CalendarTimeRange(DateTime start, DateTime end) : this(start, end, new TimeCalendar()) {}

        public CalendarTimeRange(DateTime start, DateTime end, ITimeCalendar timeCalendar)
            : this(new TimeRange(start, end), timeCalendar) {}

        public CalendarTimeRange(DateTime start, TimeSpan duration) : this(new TimeRange(start, duration), new TimeCalendar()) {}

        public CalendarTimeRange(DateTime start, TimeSpan duration, ITimeCalendar timeCalendar)
            : this(new TimeRange(start, duration), timeCalendar) {}

        public CalendarTimeRange(ITimePeriod period) : this(period, new TimeCalendar()) {}

        public CalendarTimeRange(ITimePeriod period, ITimeCalendar timeCalendar)
            : base(ToCalendarTimeRange(period, timeCalendar), true) {
            TimeCalendar = timeCalendar;
        }

        #endregion

        /// <summary>
        /// 기간의 기준이 되는 <see cref="ITimeCalendar"/>입니다.
        /// </summary>
        public ITimeCalendar TimeCalendar { get; private set; }

        /// <summary>
        /// 시작 년도
        /// </summary>
        public virtual int StartYear {
            get { return Start.Year; }
        }

        /// <summary>
        /// 시작 월
        /// </summary>
        public virtual int StartMonth {
            get { return Start.Month; }
        }

        /// <summary>
        /// 시작 일
        /// </summary>
        public virtual int StartDay {
            get { return Start.Day; }
        }

        /// <summary>
        /// 시작 시
        /// </summary>
        public virtual int StartHour {
            get { return Start.Hour; }
        }

        /// <summary>
        /// 시작 분
        /// </summary>
        public virtual int StartMinute {
            get { return Start.Minute; }
        }

        /// <summary>
        /// 완료 년도
        /// </summary>
        public virtual int EndYear {
            get { return End.Year; }
        }

        /// <summary>
        /// 완료 월
        /// </summary>
        public virtual int EndMonth {
            get { return End.Month; }
        }

        /// <summary>
        /// 완료 일
        /// </summary>
        public virtual int EndDay {
            get { return End.Day; }
        }

        /// <summary>
        /// 완료 시
        /// </summary>
        public virtual int EndHour {
            get { return End.Hour; }
        }

        /// <summary>
        /// 완료 분
        /// </summary>
        public virtual int EndMinute {
            get { return End.Minute; }
        }

        /// <summary>
        /// <see cref="TimeCalendar"/>를 기준으로 Mapping된 시작 시각
        /// </summary>
        public DateTime MappedStart {
            get { return TimeCalendar.MapStart(Start); }
        }

        /// <summary>
        /// <see cref="TimeCalendar"/>를 기준으로 Mapping된 완료 시각
        /// </summary>
        public DateTime MappedEnd {
            get { return TimeCalendar.MapEnd(End); }
        }

        /// <summary>
        /// <see cref="TimeCalendar"/>를 기준으로 Unmap 된 시작 시각
        /// </summary>
        public DateTime UnmappedStart {
            get { return TimeCalendar.UnmapStart(Start); }
        }

        /// <summary>
        /// <see cref="TimeCalendar"/>를 기준으로 Unmap 된 완료 시각
        /// </summary>
        public DateTime UnmappedEnd {
            get { return TimeCalendar.UnmapEnd(End); }
        }

        /// <summary>
        /// 시작 시각이 속한 월의 시작일 (예: 2011-05-01)
        /// </summary>
        public DateTime StartMonthStart {
            get { return TimeTool.TrimToDay(Start); }
        }

        /// <summary>
        /// 완료 시각이 속한 월의 시작일 (예: 2011-06-01)
        /// </summary>
        public DateTime EndMonthStart {
            get { return TimeTool.TrimToDay(End); }
        }

        /// <summary>
        /// 시작 시각의 시작일 (예: 2011-05-18T00:00:00.000)
        /// </summary>
        public DateTime StartDayStart {
            get { return TimeTool.TrimToHour(Start); }
        }

        /// <summary>
        /// 완료 시각의 시작일 (예: 2011-05-25T00:00:00.000)
        /// </summary>
        public DateTime EndDayStart {
            get { return TimeTool.TrimToHour(End); }
        }

        /// <summary>
        /// 시작 시각의 시(Hour) 단위까지 표현 (분단위 버름)
        /// </summary>
        public DateTime StartHourStart {
            get { return TimeTool.TrimToMinute(Start); }
        }

        /// <summary>
        /// 완료 시각의 시(Hour) 단위까지 표현 (분단위 버림)
        /// </summary>
        public DateTime EndHourStart {
            get { return TimeTool.TrimToMinute(End); }
        }

        /// <summary>
        /// 시작 시각의 분(Minute) 단위까지 표현 (초단위는 버림)
        /// </summary>
        public DateTime StartMinuteStart {
            get { return TimeTool.TrimToSecond(Start); }
        }

        /// <summary>
        /// 완료 시각의 분(Minute) 단위까지 표현 (초단위는 버림)
        /// </summary>
        public DateTime EndMinuteStart {
            get { return TimeTool.TrimToSecond(End); }
        }

        /// <summary>
        /// 시작 시각의 초(Second) 단위까지의 표현
        /// </summary>
        public DateTime StartSecondStart {
            get { return TimeTool.TrimToMillisecond(Start); }
        }

        /// <summary>
        /// 완료 시각의 초(Second) 단위까지의 표현
        /// </summary>
        public DateTime EndSecondStart {
            get { return TimeTool.TrimToMillisecond(End); }
        }

        /// <summary>
        /// 현재 기간에서 오프셋만큼 Shift 한 <see cref="ITimePeriod"/>정보를 반환합니다.
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public override ITimePeriod Copy(TimeSpan offset) {
            return ToCalendarTimeRange(base.Copy(offset), TimeCalendar);
        }

        /// <summary>
        /// <paramref name="formatter"/>를 이용하여 기간 내용을 문자열로 표현합니다.
        /// </summary>
        /// <param name="formatter"></param>
        /// <returns></returns>
        protected override string Format(ITimeFormatter formatter) {
            return formatter.GetCalendarPeriod(formatter.GetDateTime(Start), formatter.GetDateTime(End), Duration);
        }

        public int CompareTo(CalendarTimeRange other) {
            return Start.CompareTo(other.Start);
        }

        public override int CompareTo(object obj) {
            obj.ShouldNotBeNull("obj");

            if(obj is CalendarTimeRange)
                return CompareTo((CalendarTimeRange)obj);

            return 1;
        }

        public bool Equals(ICalendarTimeRange other) {
            return (other != null) && GetHashCode().Equals(other.GetHashCode());
        }

        public override bool Equals(object obj) {
            return (obj != null) && (obj is ICalendarTimeRange) && Equals((ICalendarTimeRange)obj);
        }

        public override int GetHashCode() {
            return HashTool.Compute(base.GetHashCode(), TimeCalendar.GetHashCode());
        }

        public override string ToString() {
            return string.Format("CalendarTimeRange# [{0}], TimeCalendar=[{1}]", GetDescription(null), TimeCalendar);
        }
    }
}