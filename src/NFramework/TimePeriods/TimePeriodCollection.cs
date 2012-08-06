using System;
using System.Collections.Generic;
using System.Linq;

namespace NSoft.NFramework.TimePeriods {
    /// <summary>
    /// <see cref="ITimePeriod"/>의 컬렉션입니다.
    /// </summary>
    [Serializable]
    public class TimePeriodCollection : TimePeriodContainer, ITimePeriodCollection {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 기본 생성자
        /// </summary>
        public TimePeriodCollection() {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="timePeriods">추가할 요소들</param>
        public TimePeriodCollection(IEnumerable<ITimePeriod> timePeriods) : base(timePeriods) {}

        /// <summary>
        /// 대상 TimePeriod 를 포함하는 TimePeriod 요소가 존재하는가?
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public virtual bool HasInsidePeriods(ITimePeriod target) {
            var result = _periods.Any(p => target.HasInside(p));

            if(IsDebugEnabled)
                log.Debug("target[{0}]을 포함하는 요소가 존재하는가? [{1}]", target, result);

            return result;
        }

        /// <summary>
        /// 대상 TimePeriod와 기간이 겹치는 TimePeriod 요소가 존재하는가?
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public virtual bool HasOverlapPeriods(ITimePeriod target) {
            var result = _periods.Any(p => target.OverlapsWith(p));

            if(IsDebugEnabled)
                log.Debug("target[{0}]과 기간이 겹치는 요소가 존재하는가? [{1}]", target, result);

            return result;
        }

        /// <summary>
        /// <paramref name="moment"/>와 기간이 교차하는 TimePeriod 요소가 존재하는가?
        /// </summary>
        /// <param name="moment">대상 일자</param>
        /// <returns></returns>
        public virtual bool HasIntersectionPeriods(DateTime moment) {
            var result = _periods.Any(p => p.HasInside(moment));

            if(IsDebugEnabled)
                log.Debug("moment[{0}]과 기간이 교차하는 요소가 존재하는가? [{1}]", moment, result);

            return result;
        }

        /// <summary>
        /// <paramref name="target"/> 기간과 기간이 교차하는 TimePeriod 요소가 존재하는가?
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public virtual bool HasIntersectionPeriods(ITimePeriod target) {
            var result = _periods.Any(p => target.IntersectsWith(p));

            if(IsDebugEnabled)
                log.Debug("target[{0}]과 기간이 교차하는 요소가 존재하는가? [{1}]", target, result);

            return result;
        }

        /// <summary>
        /// <paramref name="target"/> 기간을 포함하는 TimePeriod 들을 열거합니다.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public virtual IEnumerable<ITimePeriod> InsidePeriods(ITimePeriod target) {
            return _periods.Where(p => target.HasInside(p));
        }

        /// <summary>
        /// <paramref name="target"/> 기간과 겹치는 TimePeriod 들을 열거합니다.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public virtual IEnumerable<ITimePeriod> OverlapPeriods(ITimePeriod target) {
            return _periods.Where(p => target.OverlapsWith(p));
        }

        /// <summary>
        /// <paramref name="moment"/> 을 기간안에 포함하는 TimePeriod 들을 열거합니다.
        /// </summary>
        /// <param name="moment"></param>
        /// <returns></returns>
        public virtual IEnumerable<ITimePeriod> IntersectionPeriods(DateTime moment) {
            return _periods.Where(p => TimeTool.HasInside(p, moment));
        }

        /// <summary>
        /// 대상 TimePeriod 기간과 교차하는 TimePeriod 들을 열거합니다.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public virtual IEnumerable<ITimePeriod> IntersectionPeriods(ITimePeriod target) {
            return _periods.Where(p => TimeTool.IntersectsWith(target, p));
        }

        /// <summary>
        /// 대상 TimePeriod 와 특정 관계를 가지는 TimePeriod 요소들을 열거합니다.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="relation"></param>
        /// <returns></returns>
        public virtual IEnumerable<ITimePeriod> RelationPeriods(ITimePeriod target, PeriodRelation relation) {
            return _periods.Where(p => target.GetReleation(p) == relation);
        }

        /// <summary>
        /// 대상 TimePeriod 와 특정 관계를 가지는 TimePeriod 요소들을 열거합니다.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="relations"></param>
        /// <returns></returns>
        public virtual IEnumerable<ITimePeriod> RelationPeriods(ITimePeriod target, PeriodRelation[] relations) {
            return _periods.Where(p => relations.Contains(target.GetReleation(p)));
        }
    }
}