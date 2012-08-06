using System;

namespace NSoft.NFramework.TimePeriods {
    /// <summary>
    /// 특정년도와 주차를 표현하는 인터페이스
    /// </summary>
    public interface IYearAndWeekOfYear : IEquatable<IYearAndWeekOfYear> {
        /// <summary>
        /// 년도
        /// </summary>
        int Year { get; set; }

        /// <summary>
        /// 주차
        /// </summary>
        int WeekOfYear { get; set; }
    }
}