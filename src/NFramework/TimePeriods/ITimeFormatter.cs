using System;
using System.Globalization;

namespace NSoft.NFramework.TimePeriods {
    /// <summary>
    /// 시간 정보를 여러가지 문자 포맷으로 제공하기 위한 Formatter입니다.
    /// </summary>
    public interface ITimeFormatter {
        /// <summary>
        /// 문화권
        /// </summary>
        CultureInfo Culture { get; }

        /// <summary>
        /// 목록 구분자
        /// </summary>
        string ListSeparator { get; }

        /// <summary>
        /// 요소 구분자
        /// </summary>
        string ContextSeparator { get; }

        /// <summary>
        /// 기간의 시작과 완료 시각의 구분자
        /// </summary>
        string StartEndSeparator { get; }

        /// <summary>
        /// 기간(Duration) 구분자
        /// </summary>
        string DurationSeparator { get; }

        /// <summary>
        /// 시각 포맷
        /// </summary>
        string DateTimeFormat { get; }

        /// <summary>
        /// 단순 날짜 포맷
        /// </summary>
        string ShortDateFormat { get; }

        /// <summary>
        /// 긴 시간 포맷
        /// </summary>
        string LongTimeFormat { get; }

        /// <summary>
        /// 짧은 시간 포맷
        /// </summary>
        string ShortTimeFormat { get; }

        /// <summary>
        /// 기간을 문자열로 표현하는 방식
        /// </summary>
        DurationFormatKind DurationKind { get; }

        /// <summary>
        /// 기간을 초단위로 표시할 것인가?
        /// </summary>
        bool UseDurationSeconds { get; }

        /// <summary>
        /// 컬렉션의 index를 문자열로 표현합니다.
        /// </summary>
        string GetCollection(int count);

        /// <summary>
        /// 컬렉션의 기간 정보를 문자열로 표현합니다.
        /// </summary>
        string GetCollectionPeriod(int count, DateTime start, DateTime end, TimeSpan duration);

        /// <summary>
        /// 지정된 DateTime을 문자열로 표현합니다.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        string GetDateTime(DateTime dateTime);

        /// <summary>
        /// 지정된 DateTime을 Short Date 형식으로 문자열을 만듭니다.
        /// </summary>
        string GetShortDate(DateTime dateTime);

        /// <summary>
        /// <paramref name="dateTime"/>을 Long Time 형식의 문자열로 표현합니다.
        /// </summary>
        string GetLongTime(DateTime dateTime);

        /// <summary>
        /// <paramref name="dateTime"/>을 Short Time 형식의 문자열로 표현합니다.
        /// </summary>
        string GetShortTime(DateTime dateTime);

        /// <summary>
        /// <paramref name="timeSpan"/>을 문자열로 표현합니다.
        /// </summary>
        string GetDuration(TimeSpan timeSpan);

        /// <summary>
        /// <paramref name="timeSpan"/>을 문자열로 표현합니다.
        /// </summary>
        string GetDuration(TimeSpan timeSpan, DurationFormatKind? durationFormatKind);

        /// <summary>
        /// 지정된 기간을 문자열로 표현합니다.
        /// </summary>
        string GetDuration(int years, int months, int days, int hours, int minutes, int seconds);

        /// <summary>
        /// 시작-완료 시각을 문자열로 표현합니다.
        /// </summary>
        string GetPeriod(DateTime start, DateTime end);

        /// <summary>
        /// 시작-완료 | 기간 을 문자열로 표현합니다.
        /// </summary>
        string GetPeriod(DateTime start, DateTime end, TimeSpan duration);

        /// <summary>
        /// 시간 간격을 문자열로 표현합니다.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="startEdge"></param>
        /// <param name="endEdge"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        string GetInterval(DateTime start, DateTime end, IntervalEdge startEdge, IntervalEdge endEdge, TimeSpan duration);

        /// <summary>
        /// <see cref="Calendar"/>기준으로 시작-완료 | 기간을 문자열로 표현합니다.
        /// </summary>
        string GetCalendarPeriod(string start, string end, TimeSpan duration);

        /// <summary>
        /// <see cref="Calendar"/>기준으로 시작-완료 | 기간을 문자열로 표현합니다.
        /// </summary>
        string GetCalendarPeriod(string context, string start, string end, TimeSpan duration);

        /// <summary>
        /// <see cref="Calendar"/>기준으로 시작-완료 | 기간을 문자열로 표현합니다.
        /// </summary>
        string GetCalendarPeriod(string startContext, string endContext, string start, string end, TimeSpan duration);
    }
}