using System;

namespace NSoft.NFramework.TimePeriods {
    /// <summary>
    /// 시간 단위에 대한 상수
    /// </summary>
    public static class TimeSpec {
        /// <summary>
        /// 1년의 개월 수 (12)
        /// </summary>
        public const int MonthsPerYear = 12;

        /// <summary>
        /// 1년의 반기 수 (2)
        /// </summary>
        public const int HalfyearsPerYear = 2;

        /// <summary>
        /// 1년의 분기 수 (4)
        /// </summary>
        public const int QuartersPerYear = 4;

        /// <summary>
        /// 반기의 분기 수 (2)
        /// </summary>
        public const int QuartersPerHalfyear = QuartersPerYear / HalfyearsPerYear;

        /// <summary>
        /// 반기의 개월 수 (6)
        /// </summary>
        public const int MonthsPerHalfyear = MonthsPerYear / HalfyearsPerYear;

        /// <summary>
        /// 분기의 개월 수 (3)
        /// </summary>
        public const int MonthsPerQuarter = MonthsPerYear / QuartersPerYear;

        /// <summary>
        /// 1년의 최대 주차 (53주)
        /// </summary>
        public const int MaxWeeksPerYear = 54;

        /// <summary>
        /// 한달의 최대 일수 (31)
        /// </summary>
        public const int MaxDaysPerMonth = 31;

        /// <summary>
        /// 한 주의 일 수 (7)
        /// </summary>
        public const int DaysPerWeek = 7;

        /// <summary>
        /// 일일의 시간 (24)
        /// </summary>
        public const int HoursPerDay = 24;

        /// <summary>
        /// 단위 시간의 분 (60)
        /// </summary>
        public const int MinutesPerHour = 60;

        /// <summary>
        /// 단위 분의 초 (60)
        /// </summary>
        public const int SecondsPerMinute = 60;

        /// <summary>
        /// 단위 초의 밀리 초 (1000)
        /// </summary>
        public const int MillisecondsPerSecond = 1000;

        /// <summary>
        /// 1년의 시작 월 (1)
        /// </summary>
        public const int CalendarYearStartMonth = 1;

        /// <summary>
        /// 한 주의 주중 일 수 (5)
        /// </summary>
        public const int WeekDaysPerWeek = 5;

        /// <summary>
        /// 한 주의 주말 일 수 (2)
        /// </summary>
        public const int WeekEndsPerWeek = 2;

        /// <summary>
        /// 한 주의 첫번째 주중 요일 (월요일)
        /// </summary>
        public const DayOfWeek FirstWorkingDayOfWeek = DayOfWeek.Monday;

        /// <summary>
        /// 전반기에 속하는 월 (1월~6월)
        /// </summary>
        public static readonly int[] FirstHalfyearMonths = new[] { 1, 2, 3, 4, 5, 6 };

        /// <summary>
        /// 후반기에 속하는 월 (7월~12월)
        /// </summary>
        public static readonly int[] SecondHalfyearMonths = new[] { 7, 8, 9, 10, 11, 12 };

        /// <summary>
        /// 1분기 시작 월 (1월)
        /// </summary>
        public const int FirstQuarterMonth = 1;

        /// <summary>
        /// 2분기 시작 월 (4월)
        /// </summary>
        public const int SecondQuarterMonth = FirstQuarterMonth + MonthsPerQuarter;

        /// <summary>
        /// 3분기 시작 월 (7월)
        /// </summary>
        public const int ThirdQuarterMonth = SecondQuarterMonth + MonthsPerQuarter;

        /// <summary>
        /// 4분기 시작 월 (10월)
        /// </summary>
        public const int FourthQuarterMonth = ThirdQuarterMonth + MonthsPerQuarter;

        /// <summary>
        /// 1분기에 속하는 월 (1월~3월)
        /// </summary>
        public static readonly int[] FirstQuarterMonths = new[] { 1, 2, 3 };

        /// <summary>
        /// 2분기에 속하는 월 (4월~6월)
        /// </summary>
        public static readonly int[] SecondQuarterMonths = new[] { 4, 5, 6 };

        /// <summary>
        /// 3분기에 속하는 월 (7월~9월)
        /// </summary>
        public static readonly int[] ThirdQuarterMonths = new[] { 7, 8, 9 };

        /// <summary>
        /// 4분기에 속하는 월 (10월~12월)
        /// </summary>
        public static readonly int[] FourthQuarterMonths = new[] { 10, 11, 12 };

        /// <summary>
        /// 기간 없음 (TimeSpan.Zero)
        /// </summary>
        public static readonly TimeSpan NoDuration = TimeSpan.Zero;

        /// <summary>
        /// 기간 없음 (TimeSpan.Zero)
        /// </summary>
        public static readonly TimeSpan EmptyDuration = TimeSpan.Zero;

        /// <summary>
        /// 양(Positive)의 최소 기간 (TimeSpan(1))
        /// </summary>
        public static readonly TimeSpan MinPositiveDuration = new TimeSpan(1);

        /// <summary>
        /// 음(Negative)의 최소 기간 (TimeSpan(-1))
        /// </summary>
        public static readonly TimeSpan MinNegativeDuration = new TimeSpan(-1);

        /// <summary>
        /// 최소 기간에 해당하는 일자 (<see cref="DateTime.MinValue"/>)
        /// </summary>
        public static readonly DateTime MinPeriodTime = DateTime.MinValue;

        /// <summary>
        /// 최대 기간에 해당하는 일자 (<see cref="DateTime.MaxValue"/>)
        /// </summary>
        public static readonly DateTime MaxPeriodTime = DateTime.MaxValue;

        /// <summary>
        /// 최소 기간 (0입니다. 즉 <see cref="TimeSpan.Zero"/>)
        /// </summary>
        public static readonly TimeSpan MinPeriodDuration = TimeSpan.Zero;

        /// <summary>
        /// 최대 기간 (<see cref="MaxPeriodTime"/>-<see cref="MinPeriodTime"/>)
        /// </summary>
        public static readonly TimeSpan MaxPeriodDuration = MaxPeriodTime - MinPeriodTime;
    }
}