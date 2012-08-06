using System;

namespace NSoft.NFramework.TimePeriods {
    /// <summary>
    /// 시작 시각 ~ 완료시각이라는 시간의 범위를 나타내는 자료구조이고, 기간(Duration) 값은 계산됩니다.
    /// </summary>
    public interface ITimeRange : ITimePeriod {
        /// <summary>
        /// 기간의 시작 시각. 시작 시각이 미정인 경우 <see cref="TimeSpec.MinPeriodTime"/>를 반환합니다.
        /// </summary>
        new DateTime Start { get; set; }

        /// <summary>
        /// 기간의 완료 시각. 완료 시각이 미정인 경우 <see cref="TimeSpec.MaxPeriodTime"/>를 반환합니다.
        /// </summary>
        new DateTime End { get; set; }

        /// <summary>
        /// 기간을 TimeSpan으료 표현, 기간이 정해지지 않았다면 <see cref="TimeSpec.MaxPeriodDuration"/> 을 반환합니다.
        /// </summary>
        new TimeSpan Duration { get; set; }

        /// <summary>
        /// 시작 시각을 지정된 시각으로 변경합니다. 시작 시각 이후가 되면 안됩니다.
        /// </summary>
        /// <param name="moment"></param>
        void ExpandStartTo(DateTime moment);

        /// <summary>
        /// 완료 시각을 지정된 시각으로 변경합니다. 완료 시각 이전이 되면 안됩니다.
        /// </summary>
        /// <param name="moment"></param>
        void ExpandEndTo(DateTime moment);

        /// <summary>
        /// 시작 시각과 완료시각을 지정된 시각으로 설정합니다.
        /// </summary>
        /// <param name="moment"></param>
        void ExpandTo(DateTime moment);

        /// <summary>
        /// 시작시각과 완료시각을 지정된 기간 정보를 기준으로 변경합니다.
        /// </summary>
        /// <param name="period"></param>
        void ExpandTo(ITimePeriod period);

        /// <summary>
        /// 시작 시각을 지정된 시각으로 변경합니다. 시작시각보다 이후 시각이여야 합니다.
        /// </summary>
        /// <param name="moment"></param>
        void ShrinkStartTo(DateTime moment);

        /// <summary>
        /// 완료 시각을 지정된 시각으로 당깁니다. 완료시각보다 이전 시각이여야 합니다.
        /// </summary>
        /// <param name="moment"></param>
        void ShrinkEndTo(DateTime moment);

        /// <summary>
        /// 기간을 지정한 기간으로 축소시킵니다.
        /// </summary>
        /// <param name="period"></param>
        void ShrinkTo(ITimePeriod period);
    }
}