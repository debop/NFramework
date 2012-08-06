using NSoft.NFramework.Reflections;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.TimePeriods.TimeLines {
    /// <summary>
    /// <see cref="ITimeLine"/>, <see cref="ITimeLineMoment"/>, <see cref="ITimeLineMomentCollection"/>에 대한 확장 메소드를 제공합니다.
    /// </summary>
    public static class TimeLineTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// ITimeLineMoment 컬렉션의 모든 기간의 합집합을 구합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="timeLineMoments">결합할 기간들</param>
        /// <returns></returns>
        public static ITimePeriodCollection CombinePeriods<T>(this ITimeLineMomentCollection timeLineMoments) where T : ITimePeriod {
            if(IsDebugEnabled)
                log.Debug("ITimeLineMoment 컬렉션의 모든 기간의 합집합을 구합니다...");

            var periods = new TimePeriodCollection();
            if(timeLineMoments.IsEmpty) {
                return periods;
            }

            // search for periods
            var itemIndex = 0;
            while(itemIndex < timeLineMoments.Count) {
                var periodStart = timeLineMoments[itemIndex];
                Guard.Assert(periodStart.StartCount != 0, "StartCount 값은 0이 아니여야 합니다. periodStart.StartCount=[{0}]",
                             periodStart.StartCount);

                // search next period end
                // use balancing to handle overlapping periods
                var balance = periodStart.StartCount;
                ITimeLineMoment periodEnd = null;
                while(itemIndex < timeLineMoments.Count - 1 && balance > 0) {
                    itemIndex++;
                    periodEnd = timeLineMoments[itemIndex];
                    balance += periodEnd.StartCount;
                    balance -= periodEnd.EndCount;
                }

                periodEnd.ShouldNotBeNull("periodEnd");

                if(periodEnd.StartCount > 0) // touching
                {
                    itemIndex++;
                    continue;
                }

                // found a period
                if(itemIndex < timeLineMoments.Count) {
                    var period = ActivatorTool.CreateInstance<T>();
                    period.Setup(periodStart.Moment, periodEnd.Moment);

                    if(IsDebugEnabled)
                        log.Debug("Combine Period를 추가합니다. period=[{0}]" + period);

                    periods.Add(period);
                }

                itemIndex++;
            }

            if(IsDebugEnabled)
                log.Debug("기간들을 결합했습니다. periods=[{0}]", periods.CollectionToString());

            return periods;
        }

        /// <summary>
        /// ITimeLineMomentCollection으로부터 교집합에 해당하는 기간들을 구합니다
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="timeLineMoments"></param>
        /// <returns></returns>
        public static ITimePeriodCollection IntersectPeriods<T>(this ITimeLineMomentCollection timeLineMoments) where T : ITimePeriod {
            if(IsDebugEnabled)
                log.Debug("ITimeLineMomentCollection으로부터 교집합에 해당하는 기간들을 구합니다...");

            var periods = new TimePeriodCollection();

            if(timeLineMoments.IsEmpty)
                return periods;

            // search for periods
            var intersectionStart = -1;
            var balance = 0;
            for(var i = 0; i < timeLineMoments.Count; i++) {
                var moment = timeLineMoments[i];

                balance += moment.StartCount;
                balance -= moment.EndCount;

                // intersection is starting by a period start
                if(moment.StartCount > 0 && balance > 1 && intersectionStart < 0) {
                    intersectionStart = i;
                    continue;
                }

                // intersection is starting by a period end
                if(moment.EndCount > 0 && balance <= 1 && intersectionStart >= 0) {
                    var period = ActivatorTool.CreateInstance<T>();
                    period.Setup(timeLineMoments[intersectionStart].Moment, moment.Moment);

                    if(IsDebugEnabled)
                        log.Debug("Intersect Period를 추가합니다. period=[{0}]", period);

                    periods.Add(period);
                    intersectionStart = -1;
                }
            }

            if(IsDebugEnabled)
                log.Debug("ITimeLineMomentCollection으로부터 교집합에 해당하는 기간들을 구했습니다. periods=[{0}]", periods.CollectionToString());

            return periods;
        }

        /// <summary>
        /// <paramref name="timeLineMoments"/>가 가진 모든 ITimePeriod 들의 Gap을 계산합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="timeLineMoments"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public static ITimePeriodCollection CalculateGaps<T>(this ITimeLineMomentCollection timeLineMoments, ITimePeriod range)
            where T : ITimePeriod {
            if(IsDebugEnabled)
                log.Debug("ITimeLineMomentCollection의 Gap들을 계산합니다...");

            var gaps = new TimePeriodCollection();

            if(timeLineMoments.IsEmpty)
                return gaps;

            // find leading gap
            //
            var periodStart = timeLineMoments.Min;
            if(periodStart != null && range.Start < periodStart.Moment) {
                var startingGap = ActivatorTool.CreateInstance<T>();
                startingGap.Setup(range.Start, periodStart.Moment);

                if(IsDebugEnabled)
                    log.Debug("Starting Gap을 추가합니다... startingGap=[{0}]", startingGap);

                gaps.Add(startingGap);
            }

            // find intermediated gap
            //
            var itemIndex = 0;
            while(itemIndex < timeLineMoments.Count) {
                var moment = timeLineMoments[itemIndex];

                Guard.Assert(moment.StartCount != 0, "monent.StartCount 값은 0이 아니여야 합니다. moment=[{0}]", moment);

                // search next gap start
                // use balancing to handle overlapping periods
                var balance = moment.StartCount;
                ITimeLineMoment gapStart = null;

                while(itemIndex < timeLineMoments.Count - 1 && balance > 0) {
                    itemIndex++;
                    gapStart = timeLineMoments[itemIndex];
                    balance += gapStart.StartCount;
                    balance -= gapStart.EndCount;
                }

                gapStart.ShouldNotBeNull("gapStart");

                if(gapStart.StartCount > 0) // touching
                {
                    itemIndex++;
                    continue;
                }

                // found a gap
                if(itemIndex < timeLineMoments.Count - 1) {
                    var gap = ActivatorTool.CreateInstance<T>();
                    gap.Setup(gapStart.Moment, timeLineMoments[itemIndex + 1].Moment);

                    if(IsDebugEnabled)
                        log.Debug("Intermediated Gap을 추가합니다. gap=[{0}]", gap);

                    gaps.Add(gap);
                }

                itemIndex++;
            }

            // find ending gap
            //
            var periodEnd = timeLineMoments.Max;

            if(periodEnd != null && range.End > periodEnd.Moment) {
                var endingGap = ActivatorTool.CreateInstance<T>();
                endingGap.Setup(periodEnd.Moment, range.End);

                if(IsDebugEnabled)
                    log.Debug("Ending Gap을 추가합니다. endingGap=[{0}]", endingGap);

                gaps.Add(endingGap);
            }

            if(IsDebugEnabled)
                log.Debug("기간들의 Gap에 해당하는 부분을 계산했습니다!!! gaps=[{0}]", gaps.CollectionToString());

            return gaps;
        }
    }
}