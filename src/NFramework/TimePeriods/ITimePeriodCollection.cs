using System;
using System.Collections.Generic;

namespace NSoft.NFramework.TimePeriods {
    /// <summary>
    /// <see cref="ITimePeriod"/> 요소들을 컬렉션
    /// </summary>
    public interface ITimePeriodCollection : ITimePeriodContainer {
        /// <summary>
        /// 대상 TimePeriod 를 포함하는 TimePeriod 요소가 존재하는가?
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        bool HasInsidePeriods(ITimePeriod target);

        /// <summary>
        /// 대상 TimePeriod와 기간이 겹치는 TimePeriod 요소가 존재하는가?
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        bool HasOverlapPeriods(ITimePeriod target);

        /// <summary>
        /// <paramref name="moment"/> 을 기간안에 포함하는 TimePeriod 가 존재하는가?
        /// </summary>
        /// <param name="moment"></param>
        /// <returns></returns>
        bool HasIntersectionPeriods(DateTime moment);

        /// <summary>
        /// 대상 TimePeriod와 기간이 교차하는 TimePeriod 요소가 존재하는가?
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        bool HasIntersectionPeriods(ITimePeriod target);

        /// <summary>
        /// 대상 TimePeriod 기간을 포함하는 TimePeriod 들을 열거합니다.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        IEnumerable<ITimePeriod> InsidePeriods(ITimePeriod target);

        /// <summary>
        /// 대상 TimePeriod 기간과 겹치는 TimePeriod 들을 열거합니다.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        IEnumerable<ITimePeriod> OverlapPeriods(ITimePeriod target);

        /// <summary>
        /// <paramref name="moment"/> 을 기간안에 포함하는 TimePeriod 들을 열거합니다.
        /// </summary>
        /// <param name="moment"></param>
        /// <returns></returns>
        IEnumerable<ITimePeriod> IntersectionPeriods(DateTime moment);

        /// <summary>
        /// <paramref name="target"/> 기간과 교차하는 TimePeriod 들을 열거합니다.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        IEnumerable<ITimePeriod> IntersectionPeriods(ITimePeriod target);

        /// <summary>
        /// 대상 TimePeriod 와 특정 관계를 가지는 TimePeriod 요소들을 열거합니다.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="relation"></param>
        /// <returns></returns>
        IEnumerable<ITimePeriod> RelationPeriods(ITimePeriod target, PeriodRelation relation);

        /// <summary>
        /// 대상 TimePeriod 와 특정 관계를 가지는 TimePeriod 요소들을 열거합니다.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="relations"></param>
        /// <returns></returns>
        IEnumerable<ITimePeriod> RelationPeriods(ITimePeriod target, PeriodRelation[] relations);
    }
}