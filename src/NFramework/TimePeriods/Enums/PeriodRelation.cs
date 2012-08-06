using System;

namespace NSoft.NFramework.TimePeriods {
    /// <summary>
    /// 두 개의 <see cref="ITimePeriod"/>의 관계를 표현합니다.
    /// </summary>
    [Serializable]
    public enum PeriodRelation {
        /// <summary>
        /// 알 수 없음 (두개의 기간이 모두 AnyTime 일 경우
        /// </summary>
        NoRelation,

        /// <summary>
        /// 현 ITimeRange 이후에 대상 ITimeRange가 있을 때
        /// </summary>
        After,

        /// <summary>
        /// 현 ITimePeriod의 완료 시각이 대상 ITimePeriod의 시작 시각과 같습니다.
        /// </summary>
        StartTouching,

        /// <summary>
        /// 현 ITimePeriod 기간 안에 대상 ITimePeriod의 시작 시각만 포함될 때
        /// </summary>
        StartInside,

        /// <summary>
        /// 현 ITimePeriod의 시작 시각과 대상 ITimePeriod의 시작 시각이 일치하고, 대상 ITimePeriod 가 현 ITimePeriod에 포함될 때
        /// </summary>
        InsideStartTouching,

        /// <summary>
        /// 현 ITimePeriod의 시작 시각과 대상 ITimePeriod의 시작 시각이 일치하고, 현 ITimePeriod 가 대상 ITimePeriod에 포함될 때
        /// </summary>
        EnclosingStartTouching,

        /// <summary>
        /// 현 ITimePeriod가 대상 ITimePeriod 기간에 포함될 때
        /// </summary>
        Enclosing,

        /// <summary>
        /// 현 ITimePeriod의 완료 시각과 대상 ITimePeriod의 완료 시각이 일치하고, 현 ITimePeriod 가 대상 ITimePeriod에 포함될 때
        /// </summary>
        EnclosingEndTouching,

        /// <summary>
        /// 현 ITimePeriod 기간과 대상 ITimePeriod의 기간이 일치할 때, 둘 다 AnyTime이라도 ExactMath가 된다.
        /// </summary>
        ExactMatch,

        /// <summary>
        /// 현 기간안에 대상 기간이 내부에 포함될 때
        /// </summary>
        Inside,

        /// <summary>
        /// 현 기간 안에 대상 기간이 포함되는데, 완료시각만 같을 때
        /// </summary>
        InsideEndTouching,

        /// <summary>
        /// 현 기간 안에 대상 기간의 완료 시각만 포함될 때
        /// </summary>
        EndInside,

        /// <summary>
        /// 현 기간의 시작 시각이 대상 기간의 완료 시각과 일치할 때
        /// </summary>
        EndTouching,

        /// <summary>
        /// 대상 기간의 완료 시각이 현 기간의 시작시간 전에 있을 때
        /// </summary>
        Before
    }
}