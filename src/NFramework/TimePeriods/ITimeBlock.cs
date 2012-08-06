using System;

namespace NSoft.NFramework.TimePeriods {
    /// <summary>
    /// Start Time과 Duration이 지정될 수 있고, End Time 은 계산되는 자료구조를 나타냅니다.
    /// </summary>
    public interface ITimeBlock : ITimePeriod {
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
        /// 기간 설정
        /// </summary>
        /// <param name="newStart"></param>
        /// <param name="duration"></param>
        void Setup(DateTime newStart, TimeSpan duration);

        /// <summary>
        /// 시작시각(<see cref="Start"/>)은 고정, 기간(duration)으로 완료시각(<see cref="End"/>)를 재설정
        /// </summary>
        /// <param name="newDuration"></param>
        void DurationFromStart(TimeSpan newDuration);

        /// <summary>
        /// 완료시각(<see cref="End"/>)은 고정 이전 기간(duration)으로 시작시간을 계산하여, 기간으로 재설정
        /// </summary>
        /// <param name="newDuration"></param>
        void DurationFromEnd(TimeSpan newDuration);

        /// <summary>
        ///  지정된 Offset만큼 기간이 이전 시간으로 이동한 TimeBlock을 반환한다.
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        ITimeBlock GetPreviousBlock(TimeSpan offset);

        /// <summary>
        /// 지정된 Offset만큼 기간이 이후 시간으로 이동한 TimeBlock을 반환한다.
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        ITimeBlock GetNextBlock(TimeSpan offset);
    }
}