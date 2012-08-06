using System;
using System.Collections.Generic;

namespace NSoft.NFramework.TimePeriods {
    /// <summary>
    /// 기간이 존재하는 <see cref="ITimePeriod"/> 들을 소유할 수 있는 컨테이너 역할을 수행합니다.
    /// </summary>
    public interface ITimePeriodContainer : IList<ITimePeriod>, ITimePeriod {
        /// <summary>
        /// 요소 기간 중의 첫 시작 시각
        /// </summary>
        new DateTime Start { get; set; }

        /// <summary>
        /// 요소 기간 중의 마지막 완료 시각
        /// </summary>
        new DateTime End { get; set; }

        /// <summary>
        /// 읽기 전용 여부
        /// </summary>
        new bool IsReadOnly { get; }

        /// <summary>
        /// 대상 기간을 포함하고 있는지 검사합니다.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        bool ContainsPeriod(ITimePeriod target);

        /// <summary>
        /// 기간이 존재하는 (HasPeriod가 true인) <see cref="ITimePeriod"/>들을 추가합니다.
        /// </summary>
        /// <param name="periods"></param>
        void AddAll(IEnumerable<ITimePeriod> periods);

        /// <summary>
        /// ITimePeriod 요소들의 Start 속성 값을 기준으로 정렬을 수행합니다.
        /// </summary>
        /// <param name="sortDir"></param>
        void SortByStart(OrderDirection sortDir);

        /// <summary>
        /// ITimePeriod 요소들의 End 속성 값을 기준으로 정렬을 수행합니다.
        /// </summary>
        /// <param name="sortDir"></param>
        void SortByEnd(OrderDirection sortDir);

        /// <summary>
        /// ITimePeriod 요소들의 Duration 속성 값을 기준으로 정렬을 수행합니다.
        /// </summary>
        /// <param name="sortDir"></param>
        void SortByDuration(OrderDirection sortDir);
    }
}