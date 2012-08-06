using System;

namespace NSoft.NFramework.TimePeriods {
    /// <summary>
    /// 기간의 단위를 나타냅니다.
    /// </summary>
    [Serializable]
    public enum PeriodKind {
        /// <summary>
        /// 알 수 없음 (수시)
        /// </summary>
        Unknown,

        /// <summary>
        /// 년
        /// </summary>
        Year,

        /// <summary>
        /// 반기
        /// </summary>
        Halfyear,

        /// <summary>
        /// 분기
        /// </summary>
        Quarter,

        /// <summary>
        /// 월
        /// </summary>
        Month,

        /// <summary>
        /// 주
        /// </summary>
        Week,

        /// <summary>
        /// 일
        /// </summary>
        Day,

        /// <summary>
        /// 시
        /// </summary>
        Hour,

        /// <summary>
        /// 분
        /// </summary>
        Minute,

        /// <summary>
        /// 초
        /// </summary>
        Second,

        /// <summary>
        /// 밀리초
        /// </summary>
        Millisecond,
    }
}