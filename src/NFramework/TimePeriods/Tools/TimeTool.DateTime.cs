using System;
using System.Globalization;
using System.Linq;

namespace NSoft.NFramework.TimePeriods {
    public static partial class TimeTool {
        #region << Start / End TimeOfYear >>

        /// <summary>
        /// Calendar에 대해 <paramref name="dateTime"/>이 속한 년의 시작일을 반환합니다.
        /// </summary>
        /// <param name="dateTime">검사 일자</param>
        public static DateTime StartTimeOfYear(this DateTime dateTime) {
            return StartTimeOfYear(dateTime, TimeSpec.CalendarYearStartMonth);
        }

        /// <summary>
        /// 시작 월이 <paramref name="yearStartMonth"/>인 Calendar에 대해 <paramref name="dateTime"/>이 속한 년의 시작일을 반환합니다.
        /// </summary>
        /// <param name="dateTime">검사 일자</param>
        /// <param name="yearStartMonth">년도의 시작월(기본은 1)</param>
        public static DateTime StartTimeOfYear(this DateTime dateTime, int yearStartMonth) {
            var monthOffset = dateTime.Month - yearStartMonth;
            var year = monthOffset < 0 ? dateTime.Year - 1 : dateTime.Year;

            var result = new DateTime(year, yearStartMonth, 1);

            if(IsDebugEnabled)
                log.Debug("DateTime[{0}]이 속한 Year의 시작일은 [{1}] 입니다. yearStartMonth=[{2}]", dateTime, result, yearStartMonth);

            return result;
        }

        /// <summary>
        /// 지정한 년도의 시작 시각
        /// </summary>
        /// <param name="year"></param>
        /// <param name="yearStartMonth"></param>
        /// <returns></returns>
        public static DateTime StartTimeOfYear(this int year, int yearStartMonth = TimeSpec.CalendarYearStartMonth) {
            return new DateTime(year, yearStartMonth, 1);
        }

        /// <summary>
        /// 시작 월이 <paramref name="yearStartMonth"/>인 Calendar에 대해 <paramref name="dateTime"/>이 속한 년의 마지막 일을 반환합니다.
        /// </summary>
        /// <param name="dateTime">검사 일자</param>
        /// <param name="yearStartMonth">년도의 시작월(기본은 1)</param>
        public static DateTime EndTimeOfYear(this DateTime dateTime, int yearStartMonth = TimeSpec.CalendarYearStartMonth) {
            return
                dateTime
                    .StartTimeOfYear(yearStartMonth)
                    .AddYears(1)
                    .Add(TimeSpec.MinNegativeDuration);
        }

        /// <summary>
        /// 지졍된 년도의 완료 시각
        /// </summary>
        /// <param name="year"></param>
        /// <param name="yearStartMonth"></param>
        /// <returns></returns>
        public static DateTime EndTimeOfYear(this int year, int yearStartMonth = TimeSpec.CalendarYearStartMonth) {
            return EndTimeOfYear(new DateTime(year, yearStartMonth, 1), yearStartMonth);
        }

        /// <summary>
        /// 전년도 시작 시각
        /// </summary>
        /// <returns></returns>
        public static DateTime StartTimeOfLastYear(this DateTime current) {
            return StartTimeOfYear(current.Year - 1);
        }

        /// <summary>
        /// 전년도 마지막 시각
        /// </summary>
        /// <returns></returns>
        public static DateTime EndTimeOfLastYear(this DateTime current) {
            return EndTimeOfYear(current.Year - 1);
        }

        #endregion

        #region << Start / End TimeOfHalfyear >>

        /// <summary>
        /// 시작 월이 <paramref name="yearStartMonth"/>인 Calendar에 대해 <paramref name="dateTime"/>이 속한 반기의 시작일을 반환합니다.
        /// </summary>
        /// <param name="dateTime">검사 일자</param>
        /// <param name="yearStartMonth">년도의 시작월(기본은 1)</param>
        public static DateTime StartTimeOfHalfyear(this DateTime dateTime, int yearStartMonth = TimeSpec.CalendarYearStartMonth) {
            var halfyear = GetHalfyearOfMonth(yearStartMonth, dateTime.Month).GetHashCode();
            var months = (halfyear.GetHashCode() - 1) * TimeSpec.MonthsPerHalfyear;

            var result = StartTimeOfYear(dateTime, yearStartMonth).AddMonths(months);

            if(IsDebugEnabled)
                log.Debug("일자[{0}]이 속한 Halfyear의 시작일은 [{1}] 입니다. yearStartMonth=[{2}]", dateTime, result, yearStartMonth);

            return result;
        }

        public static DateTime StartTimeOfHalfyear(this int year, HalfyearKind halfyearKind,
                                                   int yearStartMonth = TimeSpec.CalendarYearStartMonth) {
            return StartTimeOfHalfyear(new DateTime(year, TimeSpec.MonthsPerHalfyear * (int)halfyearKind + 1, 1), yearStartMonth);
        }

        /// <summary>
        /// 시작 월이 <paramref name="yearStartMonth"/>인 Calendar에 대해 <paramref name="dateTime"/>이 속한 반기의 종료일을 반환합니다.
        /// </summary>
        /// <param name="dateTime">검사 일자</param>
        /// <param name="yearStartMonth">년도의 시작월(기본은 1)</param>
        public static DateTime EndTimeOfHalfyear(this DateTime dateTime, int yearStartMonth = TimeSpec.CalendarYearStartMonth) {
            return
                dateTime
                    .StartTimeOfHalfyear(yearStartMonth)
                    .AddMonths(TimeSpec.MonthsPerHalfyear)
                    .Add(TimeSpec.MinNegativeDuration);
        }

        public static DateTime EndTimeOfHalfyear(this int year, HalfyearKind halfyearKind,
                                                 int yearStartMonth = TimeSpec.CalendarYearStartMonth) {
            return
                StartTimeOfHalfyear(new DateTime(year, TimeSpec.MonthsPerHalfyear * (int)halfyearKind + 1, 1), yearStartMonth)
                    .AddMonths(TimeSpec.MonthsPerYear)
                    .Add(TimeSpec.MinNegativeDuration);
        }

        #endregion

        #region << Start / End TimeOfQuarter >>

        /// <summary>
        /// 시작 월이 <paramref name="yearStartMonth"/>인 Calendar에 대해 <paramref name="dateTime"/>이 속한 분기의 시작일을 반환합니다.
        /// </summary>
        /// <param name="dateTime">검사 일자</param>
        /// <param name="yearStartMonth">년도의 시작월(기본은 1)</param>
        public static DateTime StartTimeOfQuarter(this DateTime dateTime, int yearStartMonth = TimeSpec.CalendarYearStartMonth) {
            var quarter = GetQuarterOfMonth(yearStartMonth, dateTime.Month).GetHashCode();
            var months = (quarter.GetHashCode() - 1) * TimeSpec.MonthsPerQuarter;

            var result = StartTimeOfYear(dateTime, yearStartMonth).AddMonths(months);

            if(IsDebugEnabled)
                log.Debug("일자[{0}]이 속한 Halfyear의 시작일은 [{1}] 입니다. yearStartMonth=[{2}]", dateTime, result, yearStartMonth);

            return result;
        }

        /// <summary>
        /// 지정한 년도, 분기의 시작 시각
        /// </summary>
        public static DateTime StartTimeOfQuarter(this int year, QuarterKind quarter,
                                                  int yearStartMonth = TimeSpec.CalendarYearStartMonth) {
            var months = (quarter.GetHashCode() - 1) * TimeSpec.MonthsPerQuarter;
            return new DateTime(year, yearStartMonth, 1).AddMonths(months);
        }

        /// <summary>
        /// 시작 월이 <paramref name="yearStartMonth"/>인 Calendar에 대해 <paramref name="dateTime"/>이 속한 분기의 종료일을 반환합니다.
        /// </summary>
        /// <param name="dateTime">검사 일자</param>
        /// <param name="yearStartMonth">년도의 시작월(기본은 1)</param>
        public static DateTime EndTimeOfQuarter(this DateTime dateTime, int yearStartMonth = TimeSpec.CalendarYearStartMonth) {
            return
                dateTime
                    .StartTimeOfQuarter(yearStartMonth)
                    .AddMonths(TimeSpec.MonthsPerQuarter)
                    .Add(TimeSpec.MinNegativeDuration);
        }

        /// <summary>
        /// 지정한 년도, 분기의 완료 시각
        /// </summary>
        public static DateTime EndTimeOfQuarter(this int year, QuarterKind quarter, int yearStartMonth = TimeSpec.CalendarYearStartMonth) {
            return
                StartTimeOfQuarter(year, quarter, yearStartMonth).AddMonths(TimeSpec.MonthsPerQuarter).Add(TimeSpec.MinNegativeDuration);
        }

        /// <summary>
        /// 전(前) 분기의 시작 시각
        /// </summary>
        /// <returns></returns>
        public static DateTime StartOfLastQuarter(this DateTime current) {
            return StartTimeOfQuarter(current.Year, LastQuarterOf(current));
        }

        /// <summary>
        /// 전(前) 분기의 마지막 시각
        /// </summary>
        /// <returns></returns>
        public static DateTime EndOfLastQuarter(this DateTime current) {
            return EndTimeOfQuarter(current.Year, LastQuarterOf(current));
        }

        #endregion

        #region << Start / End TimeOfMonth >>

        /// <summary>
        /// <paramref name="moment"/>이 속한 월의 시작일을 구합니다.
        /// </summary>
        /// <param name="moment"></param>
        /// <returns></returns>
        public static DateTime StartTimeOfMonth(this DateTime moment) {
            return new DateTime(moment.Year, moment.Month, 1);
        }

        /// <summary>
        /// 해당 년, 월의 시작 시각
        /// </summary>
        public static DateTime StartTimeOfMonth(this int year, MonthKind month) {
            return new DateTime(year, (int)month, 1);
        }

        /// <summary>
        /// 해당 년, 월의 시작 시각
        /// </summary>
        public static DateTime StartTimeOfMonth(this int year, int month) {
            return new DateTime(year, month, 1);
        }

        /// <summary>
        /// <paramref name="moment"/>이 속한 월의 완료일을 구합니다.
        /// </summary>
        public static DateTime EndTimeOfMonth(this DateTime moment) {
            return moment.StartTimeOfMonth().AddMonths(1).Add(TimeSpec.MinNegativeDuration);
        }

        /// <summary>
        /// 해당 년, 월의 마지막 시각
        /// </summary>
        public static DateTime EndTimeOfMonth(this int year, MonthKind month) {
            return EndTimeOfMonth(StartTimeOfMonth(year, month));
        }

        /// <summary>
        /// 해당 년, 월의 마지막 시각
        /// </summary>
        public static DateTime EndTimeOfMonth(this int year, int month) {
            return StartTimeOfMonth(year, month).AddMonths(1).AddMilliseconds(-1);
        }

        /// <summary>
        /// 전(前)월의 시작 시각
        /// </summary>
        public static DateTime StartTimeOfLastMonth(this DateTime current) {
            var lastMonth = current.AddMonths(-1);
            return StartTimeOfMonth(lastMonth.Year, lastMonth.Month);
        }

        /// <summary>
        /// 전(前)월의 마지막 시각
        /// </summary>
        public static DateTime EndTimeOfLastMonth(this DateTime current) {
            var lastMonth = current.AddMonths(-1);
            return EndTimeOfMonth(lastMonth.Year, lastMonth.Month);
        }

        #endregion

        #region << Start / End TimeOfWeek >>

        /// <summary>
        /// <paramref name="dateTime"/>이 속한 주(Week)의 첫째 날을 반환합니다.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime StartTimeOfWeek(this DateTime dateTime) {
            return StartTimeOfWeek(dateTime, PeriodContext.Current.FirstDayOfWeek);
        }

        /// <summary>
        ///  지정된 날짜가 속한 주(week)의 첫번째 요일 (예:일요일)의 날짜
        /// </summary>
        public static DateTime StartTimeOfWeek(this DateTime time, CultureInfo culture) {
            return StartTimeOfWeek(time, culture.DateTimeFormat.FirstDayOfWeek);
        }

        /// <summary>
        /// 지정된 날짜가 속한 주(week)의 첫번째 요일 (예: 일요일)의 날짜
        /// </summary>
        /// <param name="time">일자</param>
        /// <param name="firstDayOfWeek">한 주의 첫번째 요일</param>
        /// <returns>지정된 일자가 속한 주의 첫번째 요일의 일자를 반환한다. 문화권에 따라 한주의 첫번째 요일은 다르다. 한국은 Sunday, ISO 9601로는 Monday이다.</returns>
        public static DateTime StartTimeOfWeek(this DateTime time, DayOfWeek? firstDayOfWeek) {
            var currentFirstDow = firstDayOfWeek ?? PeriodContext.Current.FirstDayOfWeek;
            var day = time;

            while(day.DayOfWeek != currentFirstDow)
                day = day.AddDays(-1);


            return day.Date;
        }

        /// <summary>
        /// <paramref name="dateTime"/>이 속한 주(Week)의 첫째 날을 반환합니다.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime EndTimeOfWeek(this DateTime dateTime) {
            return EndTimeOfWeek(dateTime, PeriodContext.Current.FirstDayOfWeek);
        }

        /// <summary>
        ///  지정된 날짜가 속한 주(week)의 첫번째 요일 (예:일요일)의 날짜
        /// </summary>
        public static DateTime EndTimeOfWeek(this DateTime time, CultureInfo culture) {
            return EndTimeOfWeek(time, culture.DateTimeFormat.FirstDayOfWeek);
        }

        /// <summary>
        /// 지정된 날짜가 속한 주(week)의 마지막 날짜 (한국:토요일, ISO8601:일요일)
        /// </summary>
        /// <param name="time"></param>
        /// <param name="firstDayOfWeek">한 주의 첫번째 요일</param>
        /// <returns></returns>
        public static DateTime EndTimeOfWeek(this DateTime time, DayOfWeek? firstDayOfWeek) {
            return
                StartTimeOfWeek(time, firstDayOfWeek)
                    .AddDays(TimeSpec.DaysPerWeek)
                    .Add(TimeSpec.MinNegativeDuration);
        }

        /// <summary>
        /// 전(前) 주(week)의 시작 시각
        /// </summary>
        /// <returns></returns>
        public static DateTime StartTimeOfLastWeek(this DateTime current) {
            int daysToSubtract = (int)current.DayOfWeek + TimeSpec.DaysPerWeek;

            return
                current
                    .Subtract(TimeSpan.FromDays(daysToSubtract))
                    .StartTimeOfDay();
        }

        /// <summary>
        /// 전(前) 주의 마지막 시각
        /// </summary>
        /// <returns></returns>
        public static DateTime EndTimeOfLastWeek(this DateTime current) {
            return
                current
                    .StartTimeOfLastWeek()
                    .AddDays(TimeSpec.DaysPerWeek)
                    .Add(TimeSpec.MinNegativeDuration);
        }

        #endregion

        #region << Start / End TimeOfDay >>

        /// <summary>
        /// 현재 일의 시작 시각
        /// </summary>
        /// <param name="moment"></param>
        /// <returns></returns>
        public static DateTime StartTimeOfDay(this DateTime moment) {
            return moment.Date;
        }

        /// <summary>
        /// 현재 일의 마지막 시각
        /// </summary>
        /// <param name="moment"></param>
        /// <returns></returns>
        public static DateTime EndTimeOfDay(this DateTime moment) {
            return moment.Date.AddDays(1).Add(TimeSpec.MinNegativeDuration);
        }

        #endregion

        #region << Start / End TimeOfHour >>

        public static DateTime StartTimeOfHour(this DateTime dateTime) {
            return dateTime.TrimToMinute();
        }

        public static DateTime EndTimeOfHour(this DateTime dateTime) {
            return dateTime.TrimToMinute().AddHours(1).Add(TimeSpec.MinNegativeDuration);
        }

        #endregion

        #region << Start / End TimeOfMinute >>

        public static DateTime StartTimeOfMinute(this DateTime dateTime) {
            return dateTime.TrimToSecond();
        }

        public static DateTime EndTimeOfMinute(this DateTime dateTime) {
            return dateTime.TrimToSecond().AddMinutes(1).Add(TimeSpec.MinNegativeDuration);
        }

        #endregion

        #region << Start / End TimeOfSecond >>

        public static DateTime StartTimeOfSecond(this DateTime dateTime) {
            return dateTime.TrimToMillisecond();
        }

        public static DateTime EndTimeOfSecond(this DateTime dateTime) {
            return dateTime.TrimToMillisecond().AddSeconds(1).Add(TimeSpec.MinNegativeDuration);
        }

        #endregion

        #region << HalfyearKind >>

        public static HalfyearKind HalfyearOf(this int month) {
            return TimeSpec.FirstHalfyearMonths.Any(m => m == month)
                       ? HalfyearKind.First
                       : HalfyearKind.Second;
        }

        public static HalfyearKind HalfyearOf(this DateTime moment) {
            return moment.Month.HalfyearOf();
        }

        public static int StartMonthOfHalfyear(this HalfyearKind half) {
            return ((int)half - 1) * TimeSpec.MonthsPerHalfyear + 1;
        }

        public static int EndMonthOfHalfyear(this HalfyearKind half) {
            return half.StartMonthOfHalfyear() + TimeSpec.MonthsPerHalfyear - 1;
        }

        #endregion

        #region << QuarterKind >>

        /// <summary>
        /// 해당 분기의 시작월
        /// </summary>
        /// <param name="quarter">해당 분기</param>
        /// <returns>해당분기의 시작월</returns>
        public static int StartMonthOfQuarter(this QuarterKind quarter) {
            return TimeSpec.MonthsPerQuarter * ((int)quarter - 1) + 1;
        }

        /// <summary>
        /// 해당 분기의 마지막 월
        /// </summary>
        /// <param name="quarter">해당 분기</param>
        /// <returns>해당 분기의 마지막 월</returns>
        public static int EndMonthOfQuarter(this QuarterKind quarter) {
            return TimeSpec.MonthsPerQuarter * (int)quarter;
        }

        /// <summary>
        /// 지정된 월이 속한 분기
        /// </summary>
        /// <param name="month">조회할 월</param>
        /// <returns>월이 속한 분기</returns>
        public static QuarterKind QuarterOf(this int month) {
            var quarter = (month - 1) / TimeSpec.MonthsPerQuarter + 1;

            return ConvertTool.ConvertEnum(quarter, QuarterKind.First);
        }

        /// <summary>
        /// 지정된 날짜의 분기를 구한다.
        /// </summary>
        /// <param name="date">날짜</param>
        /// <returns>날짜가 속한 분기</returns>
        public static QuarterKind QuarterOf(this DateTime date) {
            return QuarterOf(date.Month);
        }

        /// <summary>
        /// 지정된 날짜의 분기의 앞의 분기를 구한다.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static QuarterKind LastQuarterOf(this DateTime date) {
            return date.AddMonths(-TimeSpec.MonthsPerQuarter).Month.QuarterOf();
        }

        #endregion

        #region << DayOfWeek >>

        /// <summary>
        /// 현재 날짜로부터 가장 가까운 지정된 요일을 반환한다.
        /// </summary>
        public static DateTime NextDayOfWeek(this DateTime current) {
            return NextDayOfWeek(current, current.DayOfWeek);
        }

        /// <summary>
        /// 현재 날짜로부터 가장 가까운 지정된 요일을 반환한다.
        /// </summary>
        public static DateTime NextDayOfWeek(this DateTime current, DayOfWeek dow) {
            var next = current;
            do {
                next = next.AddDays(1);
            } while(next.DayOfWeek != dow);

            return next;
        }

        /// <summary>
        /// 현재 날짜로 지난 날짜 중에 가장 가까운 지정된 요일을 반환한다.
        /// </summary>
        public static DateTime PrevDayOfWeek(this DateTime current) {
            return PrevDayOfWeek(current, current.DayOfWeek);
        }

        /// <summary>
        /// 현재 날짜로 지난 날짜 중에 가장 가까운 지정된 요일을 반환한다.
        /// </summary>
        public static DateTime PrevDayOfWeek(this DateTime current, DayOfWeek dow) {
            var prev = current;
            do {
                prev = prev.AddDays(-1);
            } while(prev.DayOfWeek != dow);

            return prev;
        }

        #endregion

        #region << Get / Set DatePart >>

        /// <summary>
        /// 지정한 DateTime 에서 시각부분을 뺀 일자만을 반환합니다.
        /// </summary>
        /// <param name="moment"></param>
        /// <returns></returns>
        public static DateTime GetDatePart(this DateTime moment) {
            return moment.Date;
        }

        /// <summary>
        /// 날짜에 DatePart의 값을 가지는가?
        /// </summary>
        /// <param name="moment"></param>
        /// <returns></returns>
        public static bool HasDatePart(this DateTime moment) {
            return moment.Date.Ticks > 0;
        }

        /// <summary>
        /// <param name="moment"/>의 날짜부분을 <param name="datePart"/> 의 날짜부분으로 설정합니다.
        /// </summary>
        public static DateTime SetDatePart(this DateTime moment, DateTime datePart) {
            return SetTimePart(datePart, moment.TimeOfDay);
        }

        /// <summary>
        /// 지정된 날짜의 년, 월, 일을 새로 설정한다. Time Part는 그대로
        /// </summary>
        public static DateTime SetDatePart(this DateTime moment, int year, int month = 1, int day = 1) {
            return new DateTime(year, month, day).SetTimePart(moment.TimeOfDay);
        }

        /// <summary>
        /// DateTime의 년도를 지정된 값으로 설정한다.
        /// </summary>
        /// <param name="moment"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public static DateTime SetYear(this DateTime moment, int year) {
            return moment.SetDatePart(year);
        }

        /// <summary>
        /// DateTime의 월만 지정된 월로 바꾼다.
        /// </summary>
        /// <param name="moment"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public static DateTime SetMonth(this DateTime moment, int month) {
            return moment.SetDatePart(moment.Year, month);
        }

        /// <summary>
        /// DateTime의 일만 지정된 일로 바꾼다.
        /// </summary>
        /// <param name="moment"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        public static DateTime SetDay(this DateTime moment, int day) {
            return SetDatePart(moment, moment.Year, moment.Month, day);
        }

        /// <summary>
        /// <paramref name="datePart"/>의 날짜부분과, <paramref name="timePart"/>의 시간부분을 합친다.
        /// </summary>
        /// <param name="datePart">Date 정보만</param>
        /// <param name="timePart">Time 정보만</param>
        /// <seealso cref="SetTimePart(System.DateTime,System.DateTime)"/>
        public static DateTime Combine(this DateTime datePart, DateTime timePart) {
            return SetTimePart(datePart, timePart.TimeOfDay);
        }

        #endregion

        #region << Get / Set TimePart >>

        /// <summary>
        /// 지정한 DateTime 에서 시각부분만을 반환합니다.
        /// </summary>
        public static TimeSpan GetTimePart(this DateTime moment) {
            return moment.TimeOfDay;
        }

        /// <summary>
        /// 하루중 시간 부분이 존재하면 참을 반환합니다.
        /// </summary>
        public static bool HasTimePart(this DateTime dateTime) {
            return dateTime.TimeOfDay > TimeSpan.Zero;
        }

        /// <summary>
        /// <paramref name="moment"/>의 시간 부분을 <paramref name="timepart"/>의 시간부분으로 설정합니다.
        /// </summary>
        public static DateTime SetTimePart(this DateTime moment, DateTime timepart) {
            return SetTimePart(moment, timepart.TimeOfDay);
        }

        /// <summary>
        ///<paramref name="moment"/>의 시간 부분을 <paramref name="timepart"/> 값으로 설정합니다.
        /// </summary>
        public static DateTime SetTimePart(this DateTime moment, TimeSpan timepart) {
            return moment.Date.Add(timepart);
        }

        /// <summary>
        /// 날짜의 시간 파트를 설정한다.
        /// </summary>
        /// <param name="moment">설정할 DateTime</param>
        /// <param name="hour">시</param>
        /// <param name="minute">분</param>
        /// <param name="second">초</param>
        /// <param name="millisecond">밀리초</param>
        /// <returns>설정된 DateTime</returns>
        public static DateTime SetTimePart(this DateTime moment, int hour, int minute = 0, int second = 0, int millisecond = 0) {
            return new DateTime(moment.Year, moment.Month, moment.Day, hour, minute, second, millisecond);
        }

        /// <summary>
        /// 날짜의 시간(Hour)을 설정한다. 나머지 부분은 그대로 둡니다.
        /// </summary>
        /// <param name="moment">대상 DateTime</param>
        /// <param name="hour">시</param>
        /// <returns>설정된 DateTime</returns>
        public static DateTime SetHour(this DateTime moment, int hour) {
            return SetTimePart(moment, hour);
        }

        /// <summary>
        /// 날짜의 분(Minute)을 설정한다. 나머지 부분은 그대로 둡니다.
        /// </summary>
        /// <param name="moment">대상 DateTime</param>
        /// <param name="minute">분</param>
        /// <returns>설정된 DateTime</returns>
        public static DateTime SetMinute(this DateTime moment, int minute) {
            return moment.SetTimePart(moment.Hour, minute);
        }

        /// <summary>
        ///	 날짜의 초(Second)을 설정한다. 나머지 부분은 그대로 둡니다.
        /// </summary>
        /// <param name="date">대상 DateTime</param>
        /// <param name="second">초</param>
        /// <returns>설정된 DateTime</returns>
        public static DateTime SetSecond(this DateTime date, int second) {
            return date.SetTimePart(date.Hour, date.Minute, second);
        }

        /// <summary>
        /// 날짜의 밀리초(Millisecond)을 설정한다. 나머지 부분은 그대로 둡니다.
        /// </summary>
        /// <param name="date">대상 DateTime</param>
        /// <param name="millisecond">밀리초</param>
        /// <returns>설정된 DateTime</returns>
        public static DateTime SetMillisecond(this DateTime date, int millisecond) {
            return date.SetTimePart(date.Hour, date.Minute, date.Second, millisecond);
        }

        #endregion

        #region << Fluent Methods >>

        /// <summary>
        /// 지정된 날짜의 정오를 가르키는 시간을 만든다.
        /// </summary>
        /// <param name="moment">기준 DateTime</param>
        /// <returns>지정된 날짜의 정오를 가르키는 DateTime</returns>
        public static DateTime Noon(this DateTime moment) {
            return moment.Date.SetHour(12);
        }

        /// <summary>
        /// <paramref name="targetMoment"/>에 <paramref name="from"/>을 뺀 DateTime를 반환한다.
        /// </summary>
        /// <param name="from">뺄 TimeSpan</param>
        /// <param name="targetMoment">대상 일자</param>
        /// <returns>targetMoment에서 from 값을 뺀 값</returns>
        public static DateTime Ago(this TimeSpan from, DateTime targetMoment) {
            return targetMoment.Subtract(from);
        }

        /// <summary>
        /// 지정된 일자에 TimeSpan을 더한 날짜를 반환한다.
        /// </summary>
        /// <param name="from">더할 TimeSpan</param>
        /// <param name="targetMoment">대상 DateTime</param>
        /// <returns>날짜에 TimeSpan을 더한 날짜</returns>
        public static DateTime From(this TimeSpan from, DateTime targetMoment) {
            return targetMoment.Add(from);
        }

        /// <summary>
        /// 현재 날짜에 지정된 TimeSpan을 더한 날짜를 반환한다.
        /// </summary>
        /// <param name="from">더할 TimeSpan</param>
        /// <returns>현재 날짜에 TimeSpan을 더한 날짜</returns>
        public static DateTime FromNow(this TimeSpan from) {
            return From(from, DateTime.Now);
        }

        /// <summary>
        /// <paramref name="targetMoment"/>에 <paramref name="from"/>을 더한 DateTime를 반환한다. <see cref="From"/>과 같다.
        /// </summary>
        /// <param name="from">더할 TimeSpan</param>
        /// <param name="targetMoment">대상 DateTime</param>
        /// <returns>날짜에 TimeSpan을 더한 날짜</returns>
        public static DateTime Since(this TimeSpan from, DateTime targetMoment) {
            return targetMoment.Add(from);
        }

        #endregion

#if !SILVERLIGHT
        /// <summary>
        /// .NET DateTime 을 SQL Server DateTime 수형의 정밀도로 표현합니다. (SQL Server의 DateTime의 정밀도가 좀 낮다)
        /// </summary>
        /// <param name="moment"></param>
        /// <returns></returns>													
        public static DateTime ToSqlPrecision(this DateTime moment) {
            return new System.Data.SqlTypes.SqlDateTime(moment).Value;
        }
#endif
    }
}