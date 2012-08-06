using System;
using System.Collections.Generic;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.Reflections;
using NSoft.NFramework.TimePeriods.TimeLines;
using NSoft.NFramework.TimePeriods.TimeRanges;

namespace NSoft.NFramework.TimePeriods.Calendars {
    /// <summary>
    /// 특정 Calendar 기준으로 특정 시각과 기간(Duration)을 이용하여 상대 시각을 구합니다.
    /// </summary>
    [Serializable]
    public class CalendarDateAdd : DateAdd {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private readonly List<DayOfWeek> _weekDays = new List<DayOfWeek>();
        private readonly List<HourRangeInDay> _workingHours = new List<HourRangeInDay>();
        private readonly List<DayHourRange> _workingDayHours = new List<DayHourRange>();

        public CalendarDateAdd() : this(TimePeriods.TimeCalendar.NewEmptyOffset()) {}

        public CalendarDateAdd(ITimeCalendar timeCalendar) {
            timeCalendar.ShouldNotBeNull("timeCalendar");
            Guard.Assert(timeCalendar.StartOffset == TimeSpan.Zero, "Calendar의 StartOffset은 TimeSpan.Zero이어야 합니다.");
            Guard.Assert(timeCalendar.EndOffset == TimeSpan.Zero, "Calendar의 StartOffset은 TimeSpan.Zero이어야 합니다.");

            TimeCalendar = timeCalendar;
        }

        /// <summary>
        /// 기준 <see cref="ITimeCalendar"/>
        /// </summary>
        public ITimeCalendar TimeCalendar { get; private set; }

        /// <summary>
        /// 작업 요일들
        /// </summary>
        public List<DayOfWeek> WeekDays {
            get { return _weekDays; }
        }

        /// <summary>
        /// 하루중 작업시간 기간들 (오전/오후/야간 등으로 나뉠 수 있으므로)
        /// </summary>
        public List<HourRangeInDay> WorkingHours {
            get { return _workingHours; }
        }

        public IList<DayHourRange> WorkingDayHours {
            get { return _workingDayHours; }
        }

        /// <summary>
        /// 포함 기간은 지원하지 않습니다.
        /// </summary>
        public new ITimePeriodCollection IncludePeriods {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// 주중 (월~금) 을 Working Day로 추가합니다.
        /// </summary>
        public virtual void AddWorkingWeekDays() {
            AddWeekDays(DayOfWeek.Monday,
                        DayOfWeek.Tuesday,
                        DayOfWeek.Wednesday,
                        DayOfWeek.Thursday,
                        DayOfWeek.Friday);
        }

        /// <summary>
        /// 주말 (토,일) 을 Working Day로 추가합니다.
        /// </summary>
        public virtual void AddWeekendWeekDays() {
            AddWeekDays(DayOfWeek.Saturday, DayOfWeek.Sunday);
        }

        /// <summary>
        /// <paramref name="dayOfWeeks"/>를 WorkingDay에 추가합니다.
        /// </summary>
        /// <param name="dayOfWeeks"></param>
        private void AddWeekDays(params DayOfWeek[] dayOfWeeks) {
            if(dayOfWeeks != null)
                dayOfWeeks.RunEach(dow => _weekDays.Add(dow));
        }

        /// <summary>
        /// <paramref name="start"/> 시각으로부터 <paramref name="offset"/> 기간이 지난 시각을 계산합니다.
        /// </summary>
        /// <param name="start">시작 시각</param>
        /// <param name="offset">기간(Duration)</param>
        /// <param name="seekBoundaryMode">검색시 경계에 대한 모드</param>
        /// <returns></returns>
        public override DateTime? Add(DateTime start, TimeSpan offset, SeekBoundaryMode seekBoundaryMode = SeekBoundaryMode.Next) {
            if(IsDebugEnabled)
                log.Debug("Start 시각[{0}] + Duration[{1}]의 시각을 계산합니다.... SeekBoundaryMode=[{2}]", start, offset, seekBoundaryMode);

            if(WeekDays.Count == 0 && ExcludePeriods.Count == 0 && WorkingHours.Count == 0)
                return start.Add(offset);

            var end = offset < TimeSpan.Zero
                          ? CalculateEnd(start, offset.Negate(), SeekDirection.Backward, seekBoundaryMode)
                          : CalculateEnd(start, offset, SeekDirection.Forward, seekBoundaryMode);

            if(IsDebugEnabled)
                log.Debug("Start 시각[{0}] - Duration[{1}]의 시각 End=[{2}] 입니다!!! SeekBoundaryMode=[{3}]",
                          start, offset, end, seekBoundaryMode);

            return end;
        }

        /// <summary>
        /// <paramref name="start"/> 시각으로부터 <paramref name="offset"/> 기간을 뺀 (즉 이전의) 시각을 계산합니다.
        /// </summary>
        /// <param name="start">시작 시각</param>
        /// <param name="offset">기간(Duration)</param>
        /// <param name="seekBoundaryMode">검색시 경계에 대한 모드</param>
        /// <returns></returns>
        public override DateTime? Subtract(DateTime start, TimeSpan offset, SeekBoundaryMode seekBoundaryMode = SeekBoundaryMode.Next) {
            if(IsDebugEnabled)
                log.Debug("Start 시각[{0}] - Duration[{1}]의 시각을 계산합니다.... SeekBoundaryMode=[{2}]", start, offset, seekBoundaryMode);

            if(WeekDays.Count == 0 && ExcludePeriods.Count == 0 && WorkingHours.Count == 0)
                return start.Subtract(offset);

            var end = offset < TimeSpan.Zero
                          ? CalculateEnd(start, offset.Negate(), SeekDirection.Forward, seekBoundaryMode)
                          : CalculateEnd(start, offset, SeekDirection.Backward, seekBoundaryMode);

            if(IsDebugEnabled)
                log.Debug("Start 시각[{0}] - Duration[{1}]의 시각 End=[{2}] 입니다!!! SeekBoundaryMode=[{3}]",
                          start, offset, end, seekBoundaryMode);

            return end;
        }

        /// <summary>
        /// <paramref name="start"/>시각으로부터 <paramref name="offset"/> 만큼 떨어진 시각을 구합니다.
        /// </summary>
        /// <param name="start">기준 시각</param>
        /// <param name="offset">기간</param>
        /// <param name="seekDirection">검색 방향 (이전|이후)</param>
        /// <param name="seekBoundaryMode">검색 값 포함 여부</param>
        /// <returns>기준 시각으로터 오프셋만큼 떨어진 시각</returns>
        protected DateTime? CalculateEnd(DateTime start, TimeSpan offset, SeekDirection seekDirection, SeekBoundaryMode seekBoundaryMode) {
            if(IsDebugEnabled)
                log.Debug("기준시각으로부터 오프셋만큼 떨어진 시각을 구합니다... " +
                          @"start=[{0}], offset=[{1}], seekDirection=[{2}], seekBoundaryMode=[{3}], Calendar=[{4}]",
                          start, offset, seekDirection, seekBoundaryMode, TimeCalendar);

            Guard.Assert(offset >= TimeSpan.Zero, "offset 값은 TimeSpan.Zero 이상이어야 합니다. offset=[{0}]", offset);

            DateTime? end = null;
            DateTime moment = start;
            TimeSpan? remaining = offset;

            var week = new WeekRange(start, TimeCalendar);

            while(week != null) {
                base.IncludePeriods.Clear();
                base.IncludePeriods.AddAll(GetAvailableWeekPeriods(week));

                if(IsDebugEnabled)
                    log.Debug("가능한 기간은=[{0}]", base.IncludePeriods.CollectionToString());

                end = CalculateEnd(moment, remaining.Value, seekDirection, seekBoundaryMode, out remaining);

                if(end != null || remaining.HasValue == false)
                    break;

                if(seekDirection == SeekDirection.Forward) {
                    week = FindNextWeek(week);
                    if(week != null)
                        moment = week.Start;
                }
                else {
                    week = FindPreviousWeek(week);
                    if(week != null)
                        moment = week.End;
                }
            }

            if(IsDebugEnabled)
                log.Debug("기준시각으로부터 오프셋만큼 떨어진 시각을 구했습니다!!! " +
                          @"start=[{0}], offset=[{1}], seekDirection=[{2}], seekBoundaryMode=[{3}], Calendar=[{4}], end=[{5}], remaining=[{6}]",
                          start, offset, seekDirection, seekBoundaryMode, TimeCalendar, end, remaining);

            return end;
        }

        /// <summary>
        /// <paramref name="current"/> 기준으로 예외 기간 등을 고려한 후행의 가장 근접한 WeekRange를 구합니다.
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        private WeekRange FindNextWeek(WeekRange current) {
            if(IsDebugEnabled)
                log.Debug("현 Week [{0}]의 다음 Week 기간을  구합니다...", current);

            WeekRange result;
            if(ExcludePeriods.Count == 0) {
                result = current.GetNextWeek();
            }
            else {
                var limits = new TimeRange(current.End.AddTicks(1), DateTime.MaxValue);
                var gapCalculator = new TimeGapCalculator<TimeRange>(TimeCalendar);
                var remainingPeriods = gapCalculator.GetGaps(ExcludePeriods, limits);

                result = (remainingPeriods.Count > 0) ? new WeekRange(remainingPeriods[0].Start, TimeCalendar) : null;
            }

            if(IsDebugEnabled)
                log.Debug("현 Week의 다음 Week 기간을 구했습니다. current=[{0}], next=[{1}]", current, result);

            return result;
        }

        /// <summary>
        /// <paramref name="current"/> 기준으로 예외기간 등을 고려한 선행의 WeekRange를 구합니다.
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        private WeekRange FindPreviousWeek(WeekRange current) {
            if(IsDebugEnabled)
                log.Debug("현 Week [{0}]의 이전 Week 기간을 구합니다...", current);

            WeekRange result;

            if(ExcludePeriods.Count == 0) {
                result = current.GetPreviousWeek();
            }
            else {
                var limits = new TimeRange(DateTime.MinValue, current.Start.AddTicks(-1));
                var gapCalculator = new TimeGapCalculator<TimeRange>(TimeCalendar);
                var remainingPeriods = gapCalculator.GetGaps(ExcludePeriods, limits);

                result = (remainingPeriods.Count > 0)
                             ? new WeekRange(remainingPeriods[remainingPeriods.Count - 1].End, TimeCalendar)
                             : null;
            }

            if(IsDebugEnabled)
                log.Debug("현 Week의 이전 Week 기간을 구했습니다. current=[{0}], next=[{1}]", current, result);

            return result;
        }

        /// <summary>
        /// <paramref name="period"/> 기간 내에서 예외 기간등을 제외한 기간들을 HourRange 컬렉션으로 단위로 반환합니다.
        /// </summary>
        /// <param name="period"></param>
        /// <returns></returns>
        private IEnumerable<ITimePeriod> GetAvailableWeekPeriods(ITimePeriod period) {
            period.ShouldNotBeNull("period");

            if(WeekDays.Count == 0 && WorkingHours.Count == 0 && WorkingDayHours.Count == 0)
                return new TimePeriodCollection { period };

            // 필터에 필터링할 정보를 추가합니다.
            //
            var filter = new CalendarPeriodCollectorFilter();
            WeekDays.RunEach(weekDay => filter.WeekDays.Add(weekDay));
            WorkingHours.RunEach(workingHour => filter.CollectingHours.Add(workingHour));
            WorkingDayHours.RunEach(workingDayHour => filter.CollectingDayHours.Add(workingDayHour));

            var weekCollector = new CalendarPeriodCollector(filter, period, SeekDirection.Forward, TimeCalendar);
            weekCollector.CollectHours();

            return weekCollector.Periods;
        }
    }
}