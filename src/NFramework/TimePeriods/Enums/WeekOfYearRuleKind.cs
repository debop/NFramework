using System;
using System.Globalization;

namespace NSoft.NFramework.TimePeriods {
    /// <summary>
    /// 주차 (WeekOfYear) 계산을 위한 룰의 종류<br/><br/>
    /// 
    /// 해당일자의 주차를 구한다. 문화권(Culture) 및 새해 첫주차에 대한 정의에 따라 주차가 달라진다.
    /// ref : http://www.simpleisbest.net/archive/2005/10/27/279.aspx
    /// ref : http://en.wikipedia.org/wiki/ISO_8601#Week_dates
    /// </summary>
    /// <remarks>
    /// <see cref="CalendarWeekRule"/> 값에 따라 WeekOfYear 가 결정된다.
    /// 
    /// FirstDay : 1월1일이 포함된 주를 무조건 첫째 주로 삼는다. (우리나라, 미국 등의 기준) : .NET의 설정대로 하면 이렇게 된다.
    /// FirstFourDayWeek : 1월1일이 포함된 주가 4일 이상인 경우에만 그 해의 첫 번째 주로 삼는다. (ISO 8601)
    ///					   예) 한 주의 시작 요일이 일요일이고 1월1일이 일/월/화/수 중 하나이면 1월1일이 포함된 주는 해당 해의 첫 번째 주이다.
    ///					   예) 한 주의 시작 요일이 일요일이고 1월1일이 목/금/토 중 하나이면 1월1일이 포함된 주는 해당 해의 첫 번째 주로 간주하지 않는다.
    ///					   예) 2005년 1월 1일은 토요일이므로 1월1일이 포함된 주는 2005년의 첫 번째 주로 간주하지 않는다.
    /// FirstFullWeek : 1월의 첫 번째 주가 7일이 아니면 해당 해의 첫 번째 주로 삼지 않는다.
    ///				    예) 한 주의 시작 요일이 일요일인 경우, 1월1일이 일요일이 아니라면 1월1일이 포함된 주는 해당 해의 첫 번째 주로 간주하지 않는다.
    /// </remarks>
    /// <seealso cref="WeekTool"/>
    [Serializable]
    public enum WeekOfYearRuleKind {
        /// <summary>
        /// 문화권의 <see cref="DateTimeFormatInfo"/>의 
        /// <see cref="DateTimeFormatInfo.CalendarWeekRule"/>, 
        /// <see cref="DateTimeFormatInfo.FirstDayOfWeek"/>를 기준으로 주차를 결정한다.
        /// </summary>
        Calendar,

        /// <summary>
        /// ISO 8601 규칙 (FirstDayOfWeek는 DayOfWeek.Monday, CalendarWeekRule은 CalendarWeekRule.FourWeekDay)로 주차를 결정합니다.
        /// </summary>
        Iso8601
    }
}