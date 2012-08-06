using System;

namespace NSoft.NFramework.TimePeriods {
    /// <summary>
    /// 기간 (Duration)을 문자열로 표현하는 방식 (Compact|Detailed)
    /// </summary>
    [Serializable]
    public enum DurationFormatKind {
        /// <summary>
        /// 약식으로 표현
        /// </summary>
        Compact,

        /// <summary>
        /// 상세하게 표현
        /// </summary>
        Detailed
    }
}