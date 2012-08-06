using System;
using System.Collections.Generic;
using System.Linq;
using NSoft.NFramework.Reflections;
using NSoft.NFramework.TimePeriods.TimeLines;

namespace NSoft.NFramework.TimePeriods.Calendars {
    /// <summary>
    /// 특정 시각과 기간(Duration)을 이용하여 상대 시각을 구합니다.
    /// </summary>
    [Serializable]
    public class DateAdd {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 생성자
        /// </summary>
        public DateAdd() {
            IncludePeriods = new TimePeriodCollection();
            ExcludePeriods = new TimePeriodCollection();
        }

        /// <summary>
        /// Date 기간 합산 연산 시에 포함되어야 할 기간
        /// </summary>
        public ITimePeriodCollection IncludePeriods { get; private set; }

        /// <summary>
        /// Date 기간 연산 (Add, Subtract) 시에 포함하지 말아야 할 기간
        /// </summary>
        public ITimePeriodCollection ExcludePeriods { get; private set; }

        /// <summary>
        /// <paramref name="start"/> 시각으로부터 <paramref name="offset"/> 기간이 지난 시각을 계산합니다.
        /// </summary>
        /// <param name="start">시작 시각</param>
        /// <param name="offset">기간(Duration)</param>
        /// <param name="seekBoundaryMode">검색시 경계에 대한 모드</param>
        /// <returns></returns>
        public virtual DateTime? Add(DateTime start, TimeSpan offset, SeekBoundaryMode seekBoundaryMode = SeekBoundaryMode.Next) {
            if(IsDebugEnabled)
                log.Debug("Start 시각[{0}] - Duration[{1}]의 시각을 계산합니다.... SeekBoundaryMode=[{2}]", start, offset, seekBoundaryMode);

            if(IncludePeriods.Count == 0 && ExcludePeriods.Count == 0)
                return start.Add(offset);

            TimeSpan? remaining;

            var end = offset < TimeSpan.Zero
                          ? CalculateEnd(start, offset.Negate(), SeekDirection.Backward, seekBoundaryMode, out remaining)
                          : CalculateEnd(start, offset, SeekDirection.Forward, seekBoundaryMode, out remaining);

            if(IsDebugEnabled)
                log.Debug("Start 시각[{0}] - Duration[{1}]의 시각 End=[{2}], remaining=[{3}] 입니다!!! SeekBoundaryMode=[{4}]",
                          start, offset, end, remaining, seekBoundaryMode);

            return end;
        }

        /// <summary>
        /// <paramref name="start"/> 시각으로부터 <paramref name="offset"/> 기간을 뺀 (즉 이전의) 시각을 계산합니다.
        /// </summary>
        /// <param name="start">시작 시각</param>
        /// <param name="offset">기간(Duration)</param>
        /// <param name="seekBoundaryMode">검색시 경계에 대한 모드</param>
        /// <returns></returns>
        public virtual DateTime? Subtract(DateTime start, TimeSpan offset, SeekBoundaryMode seekBoundaryMode = SeekBoundaryMode.Next) {
            if(IsDebugEnabled)
                log.Debug("Start 시각[{0}] + Duration[{1}]의 시각을 계산합니다.... SeekBoundaryMode=[{2}]", start, offset, seekBoundaryMode);

            if(IncludePeriods.Count == 0 && ExcludePeriods.Count == 0)
                return start.Subtract(offset);

            TimeSpan? remaining;

            var end = offset < TimeSpan.Zero
                          ? CalculateEnd(start, offset.Negate(), SeekDirection.Forward, seekBoundaryMode, out remaining)
                          : CalculateEnd(start, offset, SeekDirection.Backward, seekBoundaryMode, out remaining);

            if(IsDebugEnabled)
                log.Debug("Start 시각[{0}] + Duration[{1}]의 시각 End=[{2}], remaining=[{3}] 입니다!!! SeekBoundaryMode=[{4}]",
                          start, offset, end, remaining, seekBoundaryMode);

            return end;
        }

        /// <summary>
        /// <paramref name="start"/>시각으로부터 <paramref name="offset"/> 만큼 떨어진 시각을 구합니다.
        /// </summary>
        /// <param name="start">기준 시각</param>
        /// <param name="offset">기간</param>
        /// <param name="seekDirection">검색 방향 (이전|이후)</param>
        /// <param name="seekBoundaryMode">검색 값 포함 여부</param>
        /// <param name="remaining">짜투리 기간</param>
        /// <returns>기준 시각으로터 오프셋만큼 떨어진 시각</returns>
        protected DateTime? CalculateEnd(DateTime start, TimeSpan offset, SeekDirection seekDirection, SeekBoundaryMode seekBoundaryMode,
                                         out TimeSpan? remaining) {
            if(IsDebugEnabled)
                log.Debug("기준시각으로부터 오프셋만큼 떨어진 시각을 구합니다... " +
                          @"start=[{0}], offset=[{1}], seekDirection=[{2}], seekBoundaryMode=[{3}]",
                          start, offset, seekDirection, seekBoundaryMode);

            Guard.Assert(offset >= TimeSpan.Zero, "offset 값은 항상 0 이상이어야 합니다. offset=[{0}]", offset);

            remaining = offset;

            // search periods
            ITimePeriodCollection searchPeriods = new TimePeriodCollection(IncludePeriods);
            if(searchPeriods.Count == 0)
                searchPeriods.Add(TimeRange.Anytime);

            // available periods
            ITimePeriodCollection availablePeriods = new TimePeriodCollection();
            if(ExcludePeriods.Count == 0) {
                availablePeriods.AddAll(searchPeriods);
            }
            else {
                // 예외 기간을 제외합니다.
                //
                var gapCalculator = new TimeGapCalculator<TimeRange>();

                var query
                    = searchPeriods
#if !SILVERLIGHT
                        .AsParallel()
                        .AsOrdered()
#endif
                        .SelectMany(searchPeriod =>
                                    ExcludePeriods.HasOverlapPeriods(searchPeriod)
                                        ? gapCalculator.GetGaps(ExcludePeriods, searchPeriod) // 예외 기간과 검색 기간에서 겹치지 않는 기간을 계산하여, 추려냅니다.
                                        : new TimePeriodCollection { searchPeriod } // 예외 기간과 겹쳐진 부분이 없다면, 기간 전체를 추가합니다.
                        );

                availablePeriods.AddAll(query);
            }

            // 유효한 Period가 없다면 중단합니다.
            if(availablePeriods.Count == 0)
                return null;

            // 기간중에 중복되는 부분의 없도록 유효한 기간을 결합합니다.
            if(availablePeriods.Count > 1) {
                var periodCombiner = new TimePeriodCombiner<TimeRange>();
                availablePeriods = periodCombiner.CombinePeriods(availablePeriods);
            }

            // 첫 시작 기간을 찾습니다.
            //
            DateTime seekMoment;
            var startPeriod = (seekDirection == SeekDirection.Forward)
                                  ? FindNextPeriod(start, availablePeriods, out seekMoment)
                                  : FindPreviousPeriod(start, availablePeriods, out seekMoment);

            // 첫 시작 기간이 없다면 중단합니다.
            if(startPeriod == null)
                return null;

            // 오프셋 값이 0 이라면, 바로 다음 값이므로 seekMoment를 반환합니다.
            if(offset == TimeSpan.Zero)
                return seekMoment;

            if(seekDirection == SeekDirection.Forward) {
                for(var i = availablePeriods.IndexOf(startPeriod); i < availablePeriods.Count; i++) {
                    var gap = availablePeriods[i];
                    var gapRemaining = gap.End - seekMoment;

                    if(IsDebugEnabled)
                        log.Debug("Seek Forward... gap=[{0}], gapRemaining=[{1}], remaining=[{2}], seekMoment=[{3}]", gap, gapRemaining,
                                  remaining, seekMoment);

                    var isTargetPeriod = (seekBoundaryMode == SeekBoundaryMode.Fill)
                                             ? gapRemaining >= remaining
                                             : gapRemaining > remaining;

                    if(isTargetPeriod) {
                        var end = seekMoment + remaining.Value;
                        remaining = null;
                        return end;
                    }

                    remaining = remaining - gapRemaining;

                    if(i == availablePeriods.Count - 1)
                        return null;

                    seekMoment = availablePeriods[i + 1].Start; // next period
                }
            }
            else {
                for(var i = availablePeriods.IndexOf(startPeriod); i >= 0; i--) {
                    var gap = availablePeriods[i];
                    var gapRemaining = seekMoment - gap.Start;

                    if(IsDebugEnabled)
                        log.Debug("Seek Backward... gap=[{0}], gapRemaining=[{1}], remaining=[{2}], seekMoment=[{3}]", gap, gapRemaining,
                                  remaining, seekMoment);

                    var isTargetPeriod = (seekBoundaryMode == SeekBoundaryMode.Fill)
                                             ? gapRemaining >= remaining
                                             : gapRemaining > remaining;

                    if(isTargetPeriod) {
                        var end = seekMoment - remaining.Value;
                        remaining = null;
                        return end;
                    }
                    remaining = remaining - gapRemaining;

                    if(i == 0)
                        return null;

                    seekMoment = availablePeriods[i - 1].End; // previous period
                }
            }
            return null;
        }

        /// <summary>
        /// <paramref name="start"/>가 <paramref name="periods"/>의 기간 중에 가장 가까운 기간에 속해 있으면 그 값을 반환하고,
        /// 아니면 <paramref name="start"/>와 가장 근접한 후행 <see cref="ITimePeriod"/>를 찾는다. 
        /// <paramref name="moment"/>에는 가장 가까운 TimePeriod의 Start 속성 값을 지정합니다.
        /// </summary>
        private static ITimePeriod FindNextPeriod(DateTime start, IEnumerable<ITimePeriod> periods, out DateTime moment) {
            //! assumes no no overlapping periods in parameter periods (HashSet<ITimePeriod> 겠지?)	

            if(IsDebugEnabled)
                log.Debug("시작시각의 이후 기간을 찾습니다... start=[{0}], periods=[{1}]", start, periods.CollectionToString());

            ITimePeriod nearestPeriod = null;
            var diffence = TimeSpan.MaxValue;
            moment = start;

            foreach(var period in periods) {
                // start가 기간에 속한다면...
                if(period.HasInside(start)) {
                    nearestPeriod = period;
                    moment = start;
                    break;
                }

                // 기간이 start 이전이라면 (Before)
                if(period.End < start)
                    continue;

                // 근처 값이 아니라면 포기 
                var periodToMement = period.Start - start;
                if(periodToMement >= diffence)
                    continue;

                diffence = periodToMement;
                nearestPeriod = period;
                moment = nearestPeriod.Start;
            }

            if(IsDebugEnabled)
                log.Debug("시작시각의 다음 기간을 찾았습니다!!! start=[{0}], moment=[{1}], nearestPeriod=[{2}]", start, moment, nearestPeriod);

            return nearestPeriod;
        }

        /// <summary>
        /// <paramref name="start"/>가 <paramref name="periods"/>의 기간 중에 가장 가까운 기간에 속해 있으면 그 값을 반환하고,
        /// 아니면 <paramref name="start"/>와 가장 근접하고, 선행되는 <see cref="ITimePeriod"/>를 찾는다. 
        /// <paramref name="moment"/>에는 가장 가까운 TimePeriod의 Start 속성 값을 지정합니다.
        /// </summary>
        private static ITimePeriod FindPreviousPeriod(DateTime start, IEnumerable<ITimePeriod> periods, out DateTime moment) {
            //! assumes no no overlapping periods in parameter periods (HashSet<ITimePeriod> 겠지?)

            if(IsDebugEnabled)
                log.Debug("시작시각의 이전 기간을 찾습니다... start=[{0}], periods=[{1}]", start, periods.CollectionToString());

            ITimePeriod nearestPeriod = null;
            var diffence = TimeSpan.MaxValue;
            moment = start;

            foreach(ITimePeriod period in periods) {
                // start가 기간에 속한다면...
                if(period.HasInside(start)) {
                    nearestPeriod = period;
                    moment = start;
                    break;
                }
                // 기간이 이미 start 이후라면 (After)
                if(period.Start > start)
                    continue;

                // 근처 값이 아니라면 포기
                //
                var periodToMement = start - period.End;
                if(periodToMement >= diffence)
                    continue;

                diffence = periodToMement;
                nearestPeriod = period;
                moment = nearestPeriod.End;
            }

            if(IsDebugEnabled)
                log.Debug("시작시각의 이전 기간을 찾았습니다!!! start=[{0}], moment=[{1}], nearestPeriod=[{2}]", start, moment, nearestPeriod);

            return nearestPeriod;
        }
    }
}