using System;

namespace NSoft.NFramework.TimePeriods {
    /// <summary>
    /// 기간 (시작시각~완료시각) 을 나타내는 Interface입니다.
    /// </summary>
    public interface ITimePeriod : IEquatable<ITimePeriod>, IComparable<ITimePeriod>, IComparable {
        /// <summary>
        /// 기간의 시작 시각. 시작 시각이 미정인 경우 <see cref="TimeSpec.MinPeriodTime"/>를 반환합니다.
        /// </summary>
        DateTime Start { get; }

        /// <summary>
        /// 기간의 완료 시각. 완료 시각이 미정인 경우 <see cref="TimeSpec.MaxPeriodTime"/>를 반환합니다.
        /// </summary>
        DateTime End { get; }

        /// <summary>
        /// 기간의 시작 시각. 시작 시각을 정해지지 않은 경우 null을 반환합니다.
        /// </summary>
        DateTime? StartAsNullable { get; }

        /// <summary>
        /// 기간의 완료 시각. 미정인 경우 null을 반환합니다.
        /// </summary>
        DateTime? EndAsNullable { get; }

        /// <summary>
        /// 기간을 TimeSpan으료 표현, 기간이 정해지지 않았다면 <see cref="TimeSpec.MaxPeriodDuration"/> 을 반환합니다.
        /// </summary>
        TimeSpan Duration { get; }

        /// <summary>
        /// 기간에 대한 문자열 표현
        /// </summary>
        string DurationDescription { get; }

        /// <summary>
        /// 시작 시각이 지정되었는가?
        /// </summary>
        bool HasStart { get; }

        /// <summary>
        /// 완료 시각이 지정되었는가?
        /// </summary>
        bool HasEnd { get; }

        /// <summary>
        /// 정해진 기간이 있는지 표시합니다.
        /// </summary>
        bool HasPeriod { get; }

        /// <summary>
        /// 시작 시각과 완료 시각의 값이 같은가? 
        /// </summary>
        bool IsMoment { get; }

        /// <summary>
        /// 시작 시각도 없고, 완료 시각도 없는 구간 (전체 구간)
        /// </summary>
        bool IsAnytime { get; }

        /// <summary>
        /// 읽기 전용
        /// </summary>
        bool IsReadOnly { get; }

        /// <summary>
        /// 기간을 설정합니다.
        /// </summary>
        /// <param name="newStart"></param>
        /// <param name="newEnd"></param>
        void Setup(DateTime? newStart, DateTime? newEnd);

        /// <summary>
        /// 현재 기간에서 오프셋만큼 Shift 한 <see cref="ITimePeriod"/>정보를 반환합니다.
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        ITimePeriod Copy(TimeSpan offset);

        /// <summary>
        /// 기간을 오프셋만큼 이동
        /// </summary>
        /// <param name="offset"></param>
        void Move(TimeSpan offset);

        /// <summary>
        /// 두 기간이 같은 기간을 나타내는지 검사합니다
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        bool IsSamePeriod(ITimePeriod other);

        /// <summary>
        /// 지정된 시각이 기간에 속하는지 검사합니다.
        /// </summary>
        /// <param name="moment"></param>
        /// <returns></returns>
        bool HasInside(DateTime moment);

        /// <summary>
        /// 지정한 기간이 현 기간 내에 속하는지 검사합니다.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        bool HasInside(ITimePeriod other);

        /// <summary>
        /// 지정한 기간이 현 기간과 겹치는 부분이 있는지 검사합니다.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        bool IntersectsWith(ITimePeriod other);

        /// <summary>
        /// 지정한 기간이 현 기간과 겹치는 부분이 있는지 검사합니다.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        bool OverlapsWith(ITimePeriod other);

        /// <summary>
        /// 기간을 미정으로 초기화합니다.
        /// </summary>
        void Reset();

        /// <summary>
        /// 다른 TimePeriod와의 관계를 나타냅니다.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        PeriodRelation GetRelation(ITimePeriod other);

        /// <summary>
        /// TimePeriod의 설명을 표현합니다.
        /// </summary>
        /// <returns></returns>
        string GetDescription(ITimeFormatter formatter);

        /// <summary>
        /// 두 기간의 겹치는 기간을 반환합니다.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        ITimePeriod GetIntersection(ITimePeriod other);

        /// <summary>
        /// 두 기간의 합집합 기간을 반환합니다.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        ITimePeriod GetUnion(ITimePeriod other);
    }
}