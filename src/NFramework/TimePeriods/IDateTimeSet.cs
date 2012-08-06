using System;
using System.Collections.Generic;

namespace NSoft.NFramework.TimePeriods {
    /// <summary>
    /// DateTime 을 요소로 가지는 컬렉션입니다.
    /// </summary>
    public interface IDateTimeSet : ICollection<DateTime> {
        /// <summary>
        /// 인덱서
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        DateTime this[int index] { get; }

        /// <summary>
        /// 최소값, 요소가 없으면 null을 반환한다.
        /// </summary>
        DateTime? Min { get; }

        /// <summary>
        /// 최대값, 요소가 없으면 null을 반환한다.
        /// </summary>
        DateTime? Max { get; }

        /// <summary>
        /// Min~Max의 기간을 나타낸다. 둘 중 하나라도 null이면 null을 반환한다.
        /// </summary>
        TimeSpan? Duration { get; }

        /// <summary>
        /// 요소가 없는 컬렉션인가?
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// 모든 요소가 같은 시각을 나타내는가?
        /// </summary>
        bool IsMoment { get; }

        /// <summary>
        /// 요소가 모든 시각을 나타내는가? <see cref="Min"/>이 null 이고, <see cref="Max"/>가 <see cref="DateTime.MaxValue"/>이다.
        /// </summary>
        bool IsAnytime { get; }

        /// <summary>
        /// 지정된 컬렉션의 요소들을 모두 추가합니다.
        /// </summary>
        /// <param name="moments"></param>
        void AddAll(IEnumerable<DateTime> moments);

        /// <summary>
        /// 순번에 해당하는 시각들의 Duration을 구합니다.
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        IList<TimeSpan> GetDurations(int startIndex, int count);

        /// <summary>
        /// 지정된 시각의 바로 전의 시각을 찾습니다. 없으면 null을 반환합니다.
        /// </summary>
        /// <param name="moment">기준 시각</param>
        /// <returns></returns>
        DateTime? FindPrevious(DateTime moment);

        /// <summary>
        /// 지정된 시각의 바로 후의 시각을 찾습니다. 없으면 null을 반환합니다.
        /// </summary>
        /// <param name="moment">기준시각</param>
        /// <returns></returns>
        DateTime? FindNext(DateTime moment);
    }
}