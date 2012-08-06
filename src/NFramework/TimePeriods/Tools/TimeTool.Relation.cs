using System;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.TimePeriods {
    public static partial class TimeTool {
        /// <summary>
        /// 기간안에 대상 시각이 포함되는지 여부
        /// </summary>
        /// <param name="period">기간</param>
        /// <param name="target">대상 일자</param>
        /// <returns>기간에 포함 여부</returns>
        public static bool HasInside(this ITimePeriod period, DateTime target) {
            var hasInside = target >= period.Start && target <= period.End;

            if(IsDebugEnabled)
                log.Debug("기간[{0}] 에 일자[{1}]가 포함되는지 여부를 알아봅니다... HasInside=[{2}]", period.AsString(), target.ToSortableString(),
                          hasInside);

            return hasInside;
        }

        /// <summary>
        /// 현 기간 안에 대상 기간이 포함되는지 여부
        /// </summary>
        /// <param name="period"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool HasInside(this ITimePeriod period, ITimePeriod target) {
            return HasInside(period, target.Start) && HasInside(period, target.End);
        }

        /// <summary>
        /// 기간이 정해지지 않은 TimePeriod인지 파악합니다. (Start와 End가 모두 Null)
        /// </summary>
        /// <param name="period"></param>
        /// <returns></returns>
        public static bool IsAnytime(this ITimePeriod period) {
            return period != null && period.IsAnytime;
        }

        /// <summary>
        /// 기간이 정해지지 않은 TimePeriod가 아닌지 파악합니다. (시작시각 또는 완료시각이 지정되어 있으면 됩니다)
        /// </summary>
        /// <param name="period"></param>
        /// <returns></returns>
        public static bool IsNotAnytime(this ITimePeriod period) {
            return !IsAnytime(period);
        }

        /// <summary>
        /// <paramref name="period"/>가 <paramref name="target"/>과의 시간 축으로 선 후행 관계를 판단합니다.
        /// </summary>
        /// <param name="period"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static PeriodRelation GetReleation(this ITimePeriod period, ITimePeriod target) {
            period.ShouldNotBeNull("period");
            target.ShouldNotBeNull("target");

            var relation = PeriodRelation.NoRelation;

            if(period.Start > target.End) {
                relation = PeriodRelation.After;
            }
            else if(period.End < target.Start) {
                relation = PeriodRelation.Before;
            }
            else if(period.Start == target.Start && period.End == target.End) {
                relation = PeriodRelation.ExactMatch;
            }
            else if(period.Start == target.End) {
                relation = PeriodRelation.StartTouching;
            }
            else if(period.End == target.Start) {
                relation = PeriodRelation.EndTouching;
            }
            else if(HasInside(period, target)) {
                if(period.Start == target.Start)
                    relation = PeriodRelation.EnclosingStartTouching;
                else
                    relation = (period.End == target.End) ? PeriodRelation.EnclosingEndTouching : PeriodRelation.Enclosing;
            }
                // 기간이 대상 기간 내부에 속할 때
            else {
                var insideStart = HasInside(target, period.Start);
                var insideEnd = HasInside(target, period.End);

                if(insideStart && insideEnd) {
                    relation = Equals(period.Start, target.Start)
                                   ? PeriodRelation.InsideStartTouching
                                   : period.End == target.End
                                         ? PeriodRelation.InsideEndTouching
                                         : PeriodRelation.Inside;
                }
                else if(insideStart)
                    relation = PeriodRelation.StartInside;

                else if(insideEnd)
                    relation = PeriodRelation.EndInside;
            }

            if(IsDebugEnabled)
                log.Debug("period[{0}]와 target[{1}] 간의 Relation은 [{2}] 입니다.", period.AsString(), target.AsString(), relation);

            return relation;
        }

        /// <summary>
        /// 두 기간 교차하거나, <paramref name="period"/>가 <paramref name="target"/> 의 내부 구간이면 true를 반환합니다.
        /// </summary>
        /// <param name="period"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool IntersectsWith(this ITimePeriod period, ITimePeriod target) {
            target.ShouldNotBeNull("target");

            var isIntersected = period.HasInside(target.Start) ||
                                period.HasInside(target.End) ||
                                (target.Start < period.Start && target.End > period.End);

            if(IsDebugEnabled)
                log.Debug("period[{0}]와 target[{1}]이 교차 구간이 있는지 확인합니다. isIntersected=[{2}]", period.AsString(), target.AsString(),
                          isIntersected);

            return isIntersected;
        }

        /// <summary>
        /// 두 기간이 겹치는 구간이 있는지 파악합니다.
        /// </summary>
        /// <param name="period"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool OverlapsWith(this ITimePeriod period, ITimePeriod target) {
            target.ShouldNotBeNull("target");


            var relation = GetReleation(period, target); //period.GetRelation(target);

            var isOverlaps = relation != PeriodRelation.After &&
                             relation != PeriodRelation.StartTouching &&
                             relation != PeriodRelation.EndTouching &&
                             relation != PeriodRelation.Before;

            if(IsDebugEnabled)
                log.Debug("period[{0}]와 target[{1}]이 Overlap되는지 있는지 확인합니다. isOverlaps=[{2}]", period.AsString(), target.AsString(),
                          isOverlaps);

            return isOverlaps;
        }

        /// <summary>
        /// 두 기간의 공통되는 기간을 반환한다. (교집합)
        /// </summary>
        /// <param name="period"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static TimeBlock GetIntersectionBlock(this ITimePeriod period, ITimePeriod target) {
            target.ShouldNotBeNull("target");

            TimeBlock intersectionBlock = null;

            if(IntersectsWith(period, target)) {
                var start = Max(period.Start, target.Start);
                var end = Min(period.End, target.End);

                intersectionBlock = new TimeBlock(start, end, period.IsReadOnly);
            }

            if(IsDebugEnabled)
                log.Debug("period[{0}]와 target[{1}] 의 교집합 TimeBlock [{2}]을 구했습니다!!!", period.AsString(), target.AsString(),
                          intersectionBlock.AsString());

            return intersectionBlock;
        }

        /// <summary>
        /// 두 기간의 합집합 기간을 반환한다.
        /// </summary>
        /// <param name="period"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static TimeBlock GetUnionBlock(this ITimePeriod period, ITimePeriod target) {
            target.ShouldNotBeNull("target");

            TimeBlock unionBlock = null;

            if(period.HasPeriod && target.HasPeriod) {
                unionBlock = new TimeBlock(period.StartAsNullable < target.StartAsNullable
                                               ? period.StartAsNullable
                                               : target.StartAsNullable,
                                           period.EndAsNullable > target.EndAsNullable
                                               ? period.EndAsNullable
                                               : target.EndAsNullable,
                                           period.IsReadOnly);
            }
            else {
                var start = (period.StartAsNullable.HasValue && target.StartAsNullable.HasValue)
                                ? Min(period.Start, target.Start)
                                : (period.StartAsNullable ?? target.StartAsNullable);
                var end = (period.EndAsNullable.HasValue && target.EndAsNullable.HasValue)
                              ? Max(period.End, target.End)
                              : (period.EndAsNullable ?? target.EndAsNullable);

                unionBlock = new TimeBlock(start, end, period.IsReadOnly);
            }

            if(IsDebugEnabled)
                log.Debug("period[{0}]와 target[{1}] 의 합집합 TimeBlock [{2}]을 구했습니다!!!", period.AsString(), target.AsString(),
                          unionBlock.AsString());

            return unionBlock;
        }

        /// <summary>
        /// 두 기간의 공통되는 기간을 반환한다. (교집합)
        /// </summary>
        /// <param name="period"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static TimeRange GetIntersectionRange(this ITimePeriod period, ITimePeriod target) {
            target.ShouldNotBeNull("target");

            TimeRange intersectionPeriod = null;

            if(IntersectsWith(period, target)) {
                var start = Max(period.Start, target.Start);
                var end = Min(period.End, target.End);

                intersectionPeriod = new TimeRange(start, end, period.IsReadOnly);
            }

            if(IsDebugEnabled)
                log.Debug("period[{0}]와 target[{1}] 의 교집합 TimeRange [{2}]를 구했습니다!!!", period.AsString(), target.AsString(),
                          intersectionPeriod.AsString());

            return intersectionPeriod;
        }

        /// <summary>
        /// 두 기간의 합집합 기간을 반환한다.
        /// </summary>
        /// <param name="period"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static TimeRange GetUnionRange(this ITimePeriod period, ITimePeriod target) {
            target.ShouldNotBeNull("target");

            TimeRange unionPeriod = null;

            if(period.HasPeriod && target.HasPeriod) {
                unionPeriod = new TimeRange(period.StartAsNullable < target.StartAsNullable
                                                ? period.StartAsNullable
                                                : target.StartAsNullable,
                                            period.EndAsNullable > target.EndAsNullable
                                                ? period.EndAsNullable
                                                : target.EndAsNullable,
                                            period.IsReadOnly);
            }
            else {
                var start = (period.StartAsNullable.HasValue && target.StartAsNullable.HasValue)
                                ? Min(period.Start, target.Start)
                                : (period.StartAsNullable ?? target.StartAsNullable);
                var end = (period.EndAsNullable.HasValue && target.EndAsNullable.HasValue)
                              ? Max(period.End, target.End)
                              : (period.EndAsNullable ?? target.EndAsNullable);

                unionPeriod = new TimeRange(start, end, period.IsReadOnly);
            }

            if(IsDebugEnabled)
                log.Debug("period[{0}]와 target[{1}] 의 합집합 TimeRange [{2}]를 구했습니다!!!", period.AsString(), target.AsString(),
                          unionPeriod.AsString());

            return unionPeriod;
        }
    }
}