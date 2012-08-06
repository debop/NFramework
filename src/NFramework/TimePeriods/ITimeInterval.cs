using System;

namespace NSoft.NFramework.TimePeriods {
    /// <summary>
    /// 시간 간격을 나타냅니다.
    /// </summary>
    public interface ITimeInterval : ITimePeriod, IEquatable<ITimeInterval> {
        /// <summary>
        /// 시작 시각이 열린 구간인가 (즉 StartTime이 지정되지 않았음)
        /// </summary>
        bool IsStartOpen { get; }

        /// <summary>
        /// 완료 시각이 열린 구간인가 (즉 EndTime이 지정되지 않았음)
        /// </summary>
        bool IsEndOpen { get; }

        /// <summary>
        /// 개구간인가? (StartTime, EndTime 모두 설정되어 있지 않을 때)
        /// </summary>
        bool IsOpen { get; }

        /// <summary>
        /// 시작시각이 폐구간인지 여부 (시작 시각이 지정되어 있다)
        /// </summary>
        bool IsStartClosed { get; }

        /// <summary>
        /// 완료 시각이 폐구간인지 여부 (완료 시각이 지정되어 있다)
        /// </summary>
        bool IsEndClosed { get; }

        /// <summary>
        /// 폐구간인가? (StartTime, EndTime 모두 의미있는 값으로 설정되어 있다)
        /// </summary>
        bool IsClosed { get; }

        /// <summary>
        /// 빈 간격인가?
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// 인터발로 쓸 수 없는 경우 (IsMoment 이면서, IsClosed 인 경우)
        /// </summary>
        bool IsDegenerate { get; }

        /// <summary>
        /// 사용가능한 시간간격인가?
        /// </summary>
        bool IsIntervalEnabled { get; }

        /// <summary>
        /// 시작 시각
        /// </summary>
        DateTime StartInterval { get; set; }

        /// <summary>
        /// 완료 시각
        /// </summary>
        DateTime EndInterval { get; set; }

        /// <summary>
        /// 시작 시각의 걔/폐구간 종류
        /// </summary>
        IntervalEdge StartEdge { get; set; }

        /// <summary>
        /// 완료 시각의 개/폐구간 종류
        /// </summary>
        IntervalEdge EndEdge { get; set; }

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
        /// 시작 시각과 완료시각을 지정된 시각으로 확장 합니다.
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
        /// 시작 시각과 완료시각을 지정된 시각으로 축소 합니다.
        /// </summary>
        /// <param name="moment"></param>
        void ShrinkTo(DateTime moment);

        /// <summary>
        /// 기간을 지정한 기간으로 축소시킵니다.
        /// </summary>
        /// <param name="period"></param>
        void ShrinkTo(ITimePeriod period);

        /// <summary>
        /// 현재 IInterval에서 오프셋만큼 이동한 <see cref="ITimeInterval"/>정보를 반환합니다.
        /// </summary>
        /// <param name="offset">이동할 오프셋</param>
        /// <returns></returns>
        new ITimeInterval Copy(TimeSpan offset);
    }
}