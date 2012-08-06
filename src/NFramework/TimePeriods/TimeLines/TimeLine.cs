using System;
using System.Collections.Generic;
using System.Linq;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.TimePeriods.TimeLines {
    /// <summary>
    /// 여러 <see cref="ITimePeriod"/>들을 시간의 흐름별로 펼쳐서 표현합니다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class TimeLine<T> : ITimeLine where T : ITimePeriod {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public TimeLine(ITimePeriodContainer periods) : this(periods, null, null) {}
        public TimeLine(ITimePeriodContainer periods, ITimePeriodMapper periodMapper) : this(periods, null, periodMapper) {}

        public TimeLine(ITimePeriodContainer periods, ITimePeriod limits, ITimePeriodMapper periodMapper) {
            periods.ShouldNotBeNull("periods");

            Periods = periods;
            Limits = (limits != null) ? new TimeRange(limits) : new TimeRange(periods);
            PeriodMapper = periodMapper;
        }

        /// <summary>
        /// TimeLine 연산을 위한 Period 의 컬렉션입니다.
        /// </summary>
        public ITimePeriodContainer Periods { get; private set; }

        /// <summary>
        /// Period 컬렉션의 전체 경계를 나타냅니다.
        /// </summary>
        public ITimePeriod Limits { get; private set; }

        /// <summary>
        /// Period 연산 결과에 대한 Start, End에 대한 Mapping을 수행할 Mapper (Start, End의 Offset을 적용한다)
        /// </summary>
        public ITimePeriodMapper PeriodMapper { get; private set; }

        /// <summary>
        /// <see cref="Periods"/>의 기간들의 합집합에 해당하는 기간들을 반환합니다.
        /// </summary>
        /// <returns></returns>
        public ITimePeriodCollection CombinePeriods() {
            if(Periods.Count == 0)
                return new TimePeriodCollection();

            var timeLineMoments = GetTimeLineMoments();

            if(timeLineMoments.Count == 0)
                return new TimePeriodCollection { new TimeRange(Periods) };

            return timeLineMoments.CombinePeriods<T>();
        }

        /// <summary>
        /// <see cref="Periods"/>의 기간들의 교집합에 해당하는 기간들을 반환합니다.
        /// </summary>
        /// <returns></returns>
        public ITimePeriodCollection IntersectPeriods() {
            if(Periods.Count == 0)
                return new TimePeriodCollection();

            var timeLineMoments = GetTimeLineMoments();

            if(timeLineMoments.Count == 0)
                return new TimePeriodCollection();

            return timeLineMoments.IntersectPeriods<T>();
        }

        /// <summary>
        /// <see cref="Periods"/>들의 Gap 부분들만을 기간 컬렉션으로 반환합니다.
        /// </summary>
        /// <returns>Gap 기간들의 컬렉션</returns>
        public ITimePeriodCollection CalcuateGaps() {
            if(IsDebugEnabled)
                log.Debug("ITimePeriod 사이의 Gap들을 계산하여 ITimePeriod 컬렉션으로 반환합니다...");

            // exclude periods
            //
            var gapPeriods = new TimePeriodCollection();

            foreach(var period in Periods.Where(p => Limits.IntersectsWith(p)))
                gapPeriods.Add(new TimeRange(period));

            var timeLineMoments = GetTimeLineMoments(gapPeriods);

            if(timeLineMoments.Count == 0)
                return new TimePeriodCollection { Limits };

            var range = ActivatorTool.CreateInstance<T>();
            range.Setup(MapPeriodStart(Limits.Start), MapPeriodEnd(Limits.End));

            return timeLineMoments.CalculateGaps<T>(range);
        }

        /// <summary>
        /// 기간 컬렉션으로부터 ITimeLineMoment 컬렉션을 빌드합니다
        /// </summary>
        /// <returns></returns>
        private ITimeLineMomentCollection GetTimeLineMoments() {
            return GetTimeLineMoments(Periods);
        }

        /// <summary>
        /// 기간 컬렉션으로부터 ITimeLineMoment 컬렉션을 빌드합니다
        /// </summary>
        /// <param name="momentPeriods"></param>
        /// <returns></returns>
        private ITimeLineMomentCollection GetTimeLineMoments(ICollection<ITimePeriod> momentPeriods) {
            if(IsDebugEnabled)
                log.Debug("기간 컬렉션으로부터 ITimeLineMoment 컬렉션을 빌드합니다...");

            var timeLineMoments = new TimeLineMomentCollection();

            if(momentPeriods.Count == 0)
                return timeLineMoments;

            // setup gap set with all start/end points
            var intersections = new TimePeriodCollection();

            foreach(var momentPeriod in momentPeriods.Where(mp => !mp.IsMoment)) {
                var intersection = Limits.GetIntersection(momentPeriod);
                if(intersection == null || intersection.IsMoment)
                    continue;

                if(PeriodMapper != null)
                    intersection = new TimeRange(MapPeriodStart(intersection.Start),
                                                 MapPeriodEnd(intersection.End));

                intersections.Add(intersection);
            }

            timeLineMoments.AddAll(intersections);

            if(IsDebugEnabled)
                log.Debug("기간 컬렉션으로부터 ITimeLineMoment 컬렉션을 빌드했습니다. timeLineMoments=[{0}]", timeLineMoments);

            return timeLineMoments;
        }

        /// <summary>
        /// <paramref name="start"/>를 <see cref="PeriodMapper"/>의 StartOffset을 제거합니다.
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        private DateTime MapPeriodStart(DateTime start) {
            return (PeriodMapper != null) ? PeriodMapper.UnmapStart(start) : start;
        }

        /// <summary>
        /// <paramref name="end"/>를 <see cref="PeriodMapper"/>의 EndOffset을 제거합니다.
        /// </summary>
        /// <param name="end"></param>
        /// <returns></returns>
        private DateTime MapPeriodEnd(DateTime end) {
            return (PeriodMapper != null) ? PeriodMapper.UnmapEnd(end) : end;
        }
    }
}