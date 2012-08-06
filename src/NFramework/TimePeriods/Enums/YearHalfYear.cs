using System;

namespace NSoft.NFramework.TimePeriods {
    /// <summary>
    /// 반기 (Halfyear) 종류
    /// </summary>
    [Serializable]
    public enum HalfyearKind {
        /// <summary>
        /// 상반기
        /// </summary>
        First = 1,

        /// <summary>
        /// 하반기
        /// </summary>
        Second = 2
    }
}