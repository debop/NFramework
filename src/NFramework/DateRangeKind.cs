namespace NSoft.NFramework {
    /// <summary>
    /// 날짜 구간의 단위 (초 단위, 시간 단위, 일 단위 등)
    /// </summary>
    public enum DateRangeKind {
        /// <summary>
        /// 초
        /// </summary>
        Second,

        /// <summary>
        /// 분
        /// </summary>
        Minute,

        /// <summary>
        /// 시
        /// </summary>
        Hour,

        /// <summary>
        /// 일
        /// </summary>
        Day,

        /// <summary>
        /// 주
        /// </summary>
        Week,

        /// <summary>
        /// 월
        /// </summary>
        Month,

        /// <summary>
        /// 분기
        /// </summary>
        Quarter,

        /// <summary>
        /// 반기
        /// </summary>
        HalfYear,

        /// <summary>
        /// 년
        /// </summary>
        Year
    }
}