using System;
using System.Globalization;

namespace NSoft.NFramework.TimePeriods {
    /// <summary>
    /// 문화권에 따른 날짜 표현, 날짜 계산 등을 제공하는 Calendar 입니다. (ISO 8601, Korean 등)
    /// </summary>
    public interface ITimeCalendar : ITimePeriodMapper {
        /// <summary>
        /// 문화권 정보 (문화권에 따라 달력에 대한 규칙 및 명칭이 달라집니다.)
        /// </summary>
        CultureInfo Culture { get; }

        /// <summary>
        /// 년 종류
        /// </summary>
        YearKind YearKind { get; }

        /// <summary>
        /// 시작 오프셋 (시작일자가 1월 1일이 아닌 경우) 
        /// </summary>
        TimeSpan StartOffset { get; }

        /// <summary>
        /// 종료 오프셋
        /// </summary>
        TimeSpan EndOffset { get; }

        /// <summary>
        /// 년의 기준 월 (한국 교육년은 3월, 미국은 9월, 회계년은 2월 등 다양하다)
        /// </summary>
        int YearBaseMonth { get; }

        /// <summary>
        /// 년의 주차 계산 방식
        /// </summary>
        WeekOfYearRuleKind WeekOfYearRule { get; }

        /// <summary>
        /// Calendar Week Rule
        /// </summary>
        CalendarWeekRule CalendarWeekRule { get; }

        /// <summary>
        /// 한 주의 시작 요일 (한국, 미국: Sunday, ISO-8601: Monday)
        /// </summary>
        DayOfWeek FirstDayOfWeek { get; }

        /// <summary>
        /// 지정된 일자의 년
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        int GetYear(DateTime time);

        /// <summary>
        /// 지정된 일자의 월
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        int GetMonth(DateTime time);

        /// <summary>
        /// 지정된 시각의 시간
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        int GetHour(DateTime time);

        /// <summary>
        /// 지정된 시각의 분
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        int GetMinute(DateTime time);

        /// <summary>
        /// 지정된 날짜의 월 몇번째 일인지
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        int GetDayOfMonth(DateTime time);

        /// <summary>
        /// 지정된 날짜의 요일
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        DayOfWeek GetDayOfWeek(DateTime time);

        /// <summary>
        /// 지정된 년,월의 날짜수
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        int GetDaysInMonth(int year, int month);

        /// <summary>
        /// 년도 이름
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        string GetYearName(int year);

        /// <summary>
        /// 반기를 표현하는 문자열을 반환합니다.
        /// </summary>
        /// <param name="halfyear"></param>
        /// <returns></returns>
        string GetHalfYearName(HalfyearKind halfyear);

        /// <summary>
        /// 지정한 년도의 반기를 표현하는 문자열을 반환합니다.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="halfyear"></param>
        /// <returns></returns>
        string GetHalfYearOfYearName(int year, HalfyearKind halfyear);

        /// <summary>
        /// 분기를 표현하는 문자열을 반환합니다. (2사분기)
        /// </summary>
        /// <param name="quarter"></param>
        /// <returns></returns>
        string GetQuarterName(QuarterKind quarter);

        /// <summary>
        /// 특정년도의 분기를 표현하는 문자열을 반환합니다. (2011년 2사분기)
        /// </summary>
        /// <param name="year"></param>
        /// <param name="quarter"></param>
        /// <returns></returns>
        string GetQuarterOfYearName(int year, QuarterKind quarter);

        /// <summary>
        /// 월을 표현하는 문자열을 반환합니다.
        /// </summary>
        /// <param name="month"></param>
        /// <returns></returns>
        string GetMonthName(int month);

        /// <summary>
        /// 특정 년, 월을 표현하는 문자열을 반환합니다.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        string GetMonthOfYearName(int year, int month);

        /// <summary>
        /// 년,주차를 문자열로 표현합니다.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="weekOfYear"></param>
        /// <returns></returns>
        string GetWeekOfYearName(int year, int weekOfYear);

        /// <summary>
        /// 지정한 요일을 문자열로 표현합니다.
        /// </summary>
        /// <param name="dayOfWeek"></param>
        /// <returns></returns>
        string GetDayName(DayOfWeek dayOfWeek);

        /// <summary>
        /// 지정된 일자의 주차(Week of Year)를 반환합니다. (<see cref="CalendarWeekRule"/>, <see cref="FirstDayOfWeek"/>에 따라 달라집니다)
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        int GetWeekOfYear(DateTime time);

        /// <summary>
        /// 지정된 년, 주차에 해당하는 주의 첫번째 일자를 반환한다. (예: 2011년 3주차의 첫번째 일자는?) (<see cref="CalendarWeekRule"/>, <see cref="FirstDayOfWeek"/>에 따라 달라집니다)
        /// </summary>
        /// <param name="year"></param>
        /// <param name="weekOfYear"></param>
        /// <returns></returns>
        DateTime GetStartOfYearWeek(int year, int weekOfYear);
    }
}