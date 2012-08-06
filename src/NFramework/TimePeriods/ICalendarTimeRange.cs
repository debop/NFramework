namespace NSoft.NFramework.TimePeriods {
    /// <summary>
    /// <see cref="ITimeCalendar"/>의 설정정보를 바탕으로 하는 <see cref="ITimeRange"/>를 표현합니다.
    /// </summary>
    public interface ICalendarTimeRange : ITimeRange {
        /// <summary>
        /// 기간 설정에 사용될 <see cref="ITimeCalendar"/>입니다.
        /// </summary>
        ITimeCalendar TimeCalendar { get; }
    }
}