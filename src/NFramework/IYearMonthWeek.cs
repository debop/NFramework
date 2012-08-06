namespace NSoft.NFramework {
    /// <summary>
    /// 년 (Year), 월 (Month), 주 (Week)를 표현합니다.
    /// </summary>
    public interface IYearMonthWeek : IYearMonth {
        int? Week { get; set; }
    }
}