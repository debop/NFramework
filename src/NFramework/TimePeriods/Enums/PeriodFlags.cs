using System;

namespace NSoft.NFramework.TimePeriods {
    /// <summary>
    /// 기간에 대한 여러가지 복합체를 만들어 냅니다. <see cref="PeriodKind"/>와는 달리 Bit 연산을 수행합니다.
    /// Gantt Chart 등에서 기간을 축 (Axis) 로 나타낼때, 여러 단위로 지정할 때 사용합니다.
    /// </summary>
    [Flags]
    [Serializable]
    public enum PeriodFlags {
        /// <summary>
        /// 없음
        /// </summary>
        None = 0x00,

        /// <summary>
        /// 년 (Year)
        /// </summary>
        Year = 0x01,

        /// <summary>
        /// 반기
        /// </summary>
        HalfYear = 0x02,

        /// <summary>
        /// 분기
        /// </summary>
        Quarter = 0x04,

        /// <summary>
        /// 월
        /// </summary>
        Month = 0x08,

        /// <summary>
        /// 주
        /// </summary>
        Week = 0x10,

        /// <summary>
        /// 일
        /// </summary>
        Day = 0x20,

        /// <summary>
        /// 시간
        /// </summary>
        Hour = 0x40,

        /// <summary>
        /// 년/월
        /// </summary>
        YearMonth = Year | Month,

        /// <summary>
        /// 년/월/일
        /// </summary>
        YearMonthDay = YearMonth | Day,

        /// <summary>
        /// 년/월/일/시
        /// </summary>
        YearMonthDayHour = YearMonthDay | Hour,

        /// <summary>
        /// 년/분기
        /// </summary>
        YearQuarter = Year | Quarter,

        /// <summary>
        /// 년/분기/월
        /// </summary>
        YearQuarterMonth = YearQuarter | Month,

        /// <summary>
        /// 년/분기/월/일
        /// </summary>
        YearQuarterMonthDay = YearQuarterMonth | Day,

        /// <summary>
        /// 년/분기/월/일/시
        /// </summary>
        YearQuarterMonthDayHour = YearQuarterMonthDay | Hour,

        /// <summary>
        /// 년/주
        /// </summary>
        YearWeek = Year | Week,

        /// <summary>
        /// 년/주/일
        /// </summary>
        YearWeekDay = YearWeek | Day,

        /// <summary>
        /// 년/주/일/시
        /// </summary>
        YearWeekDayHour = YearWeekDay | Hour,

        /// <summary>
        /// 월/일
        /// </summary>
        MonthDay = Month | Day,

        /// <summary>
        /// 월/일/시
        /// </summary>
        MonthDayHour = MonthDay | Hour,
    }
}