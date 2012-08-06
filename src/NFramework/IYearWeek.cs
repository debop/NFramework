using System;

namespace NSoft.NFramework {
    /// <summary>
    /// WeekOfYear ("Y년 W 번째 주") 를 의미하는 WeekOfYear 정보를 나타내는 인터페이스입니다.
    /// </summary>
    public interface IYearWeek : IEquatable<IYearWeek>, IComparable<IYearWeek>, IComparable {
        /// <summary>
        /// 년도
        /// </summary>
        int? Year { get; set; }

        /// <summary>
        /// 년도의 주 (Week)
        /// </summary>
        int? Week { get; set; }
    }
}