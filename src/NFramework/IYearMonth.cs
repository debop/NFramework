namespace NSoft.NFramework {
    /// <summary>
    /// 년도와 월만 표현
    /// </summary>
    public interface IYearMonth {
        /// <summary>
        /// 년도
        /// </summary>
        int? Year { get; set; }

        /// <summary>
        /// 월
        /// </summary>
        int? Month { get; set; }
    }
}