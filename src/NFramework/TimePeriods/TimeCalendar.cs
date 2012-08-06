using System;
using System.Globalization;

namespace NSoft.NFramework.TimePeriods {
    /// <summary>
    ///  문화권에 따른 날짜 표현, 날짜 계산 등을 제공하는 Calendar 입니다. (ISO 8601, Korean 등)
    /// </summary>
    [Serializable]
    public class TimeCalendar : ITimeCalendar, IEquatable<ITimeCalendar> {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// 시작 시각에 대한 기본 Offset (<see cref="TimeSpec.NoDuration"/>)
        /// </summary>
        public static readonly TimeSpan DefaultStartOffset = TimeSpec.NoDuration;

        /// <summary>
        /// 완료 시각에 대한 기본 Offset (<see cref="TimeSpec.MinNegativeDuration"/>)
        /// </summary>
        public static readonly TimeSpan DefaultEndOffset = TimeSpec.MinNegativeDuration;

        #region << Static Methods - New, NewEmptyOffset >>

        /// <summary>
        /// 기본 환경설정으로 <see cref="TimeCalendar"/>를 생성합니다.
        /// </summary>
        /// <returns>TimeCalendar 인스턴스</returns>
        public static TimeCalendar New() {
            return new TimeCalendar(new TimeCalendarConfig { YearBaseMonth = TimeSpec.CalendarYearStartMonth });
        }

        /// <summary>
        /// <paramref name="culture"/>를 문화권으로 하는 TimeCalendar를 생성합니다.
        /// </summary>
        /// <param name="culture">문화권</param>
        /// <returns>TimeCalendar 인스턴스</returns>
        public static TimeCalendar New(CultureInfo culture) {
            return new TimeCalendar(new TimeCalendarConfig { Culture = culture });
        }

        /// <summary>
        /// 한해의 시작 월이 <paramref name="yearBaseMonth"/>인 TimeCalendar를 생성합니다.
        /// </summary>
        /// <param name="yearBaseMonth">한 해의 시작 월</param>
        /// <returns>TimeCalendar 인스턴스</returns>
        public static TimeCalendar New(int yearBaseMonth) {
            return new TimeCalendar(new TimeCalendarConfig { YearBaseMonth = yearBaseMonth });
        }

        /// <summary>
        /// 시작 오프셋(<paramref name="startOffset"/>), 완료 오프셋(<paramref name="endOffset"/>)을 가지는 TimeCalendar를 생성합니다.
        /// </summary>
        /// <param name="startOffset">시작 오프셋</param>
        /// <param name="endOffset">완료 오프셋</param>
        /// <returns>TimeCalendar 인스턴스</returns>
        public static TimeCalendar New(TimeSpan? startOffset, TimeSpan? endOffset) {
            return New(null, startOffset, endOffset, null);
        }

        /// <summary>
        /// TimeCalendar를 생성합니다.
        /// </summary>
        /// <param name="startOffset">시작 오프셋</param>
        /// <param name="endOffset">완료 오프셋</param>
        /// <param name="yearBaseMonth">한 해의 시작 월</param>
        /// <returns>TimeCalendar 인스턴스</returns>
        public static TimeCalendar New(TimeSpan? startOffset, TimeSpan? endOffset, int? yearBaseMonth) {
            return New(null, startOffset, endOffset, yearBaseMonth);
        }

        /// <summary>
        /// TimeCalendar를 생성합니다.
        /// </summary>
        /// <param name="culture">문화권</param>
        /// <param name="startOffset">시작 오프셋</param>
        /// <param name="endOffset">완료 오프셋</param>
        /// <returns>TimeCalendar</returns>
        public static TimeCalendar New(CultureInfo culture, TimeSpan? startOffset, TimeSpan? endOffset) {
            return New(culture, startOffset, endOffset, null);
        }

        /// <summary>
        /// TimeCalendar를 생성합니다.
        /// </summary>
        /// <param name="culture">문화권</param>
        /// <param name="startOffset">시작 오프셋</param>
        /// <param name="endOffset">완료 오프셋</param>
        /// <param name="yearBaseMonth">한 해의 시작 월</param>
        /// <returns>TimeCalendar 인스턴스</returns>
        public static TimeCalendar New(CultureInfo culture, TimeSpan? startOffset, TimeSpan? endOffset, int? yearBaseMonth) {
            if(startOffset.HasValue)
                startOffset.Value.ShouldBeGreaterOrEqual(TimeSpan.Zero, "startOffset");

            if(endOffset.HasValue)
                endOffset.Value.ShouldBeLessOrEqual(TimeSpan.Zero, "endOffset");

            return new TimeCalendar(new TimeCalendarConfig(culture)
                                    {
                                        StartOffset = startOffset,
                                        EndOffset = endOffset,
                                        YearBaseMonth = yearBaseMonth
                                    });
        }

        /// <summary>
        /// TimeCalendar를 생성합니다.
        /// </summary>
        /// <param name="culture">문화권</param>
        /// <param name="yearBaseMonth">한 해의 시작 월</param>
        /// <param name="weekOfYearRule">주차 계산 규칙</param>
        /// <returns>TimeCalendar 인스턴스</returns>
        public static TimeCalendar New(CultureInfo culture, int? yearBaseMonth,
                                       WeekOfYearRuleKind weekOfYearRule = WeekOfYearRuleKind.Calendar) {
            return new TimeCalendar(new TimeCalendarConfig(culture, weekOfYearRule)
                                    {
                                        YearBaseMonth = yearBaseMonth
                                    });
        }

        /// <summary>
        /// TimeCalendar를 생성합니다.
        /// </summary>
        /// <param name="culture">문화권</param>
        /// <param name="yearBaseMonth">한 해의 시작 월</param>
        /// <param name="weekOfYearRule">주차 계산 규칙</param>
        /// <param name="startOffset">시작 오프셋</param>
        /// <param name="endOffset">완료 오프셋</param>
        /// <returns>TimeCalendar 인스턴스</returns>
        public static TimeCalendar New(CultureInfo culture, int? yearBaseMonth, WeekOfYearRuleKind weekOfYearRule, TimeSpan? startOffset,
                                       TimeSpan? endOffset) {
            return new TimeCalendar(new TimeCalendarConfig(culture, weekOfYearRule)
                                    {
                                        YearBaseMonth = yearBaseMonth,
                                        StartOffset = startOffset,
                                        EndOffset = endOffset
                                    });
        }

        /// <summary>
        /// 시작, 완료 Offset 모두 Zero인 TimeCalendar를 생성합니다.
        /// </summary>
        /// <param name="yearBaseMonth">한해의 시작 월</param>
        /// <returns></returns>
        public static TimeCalendar NewEmptyOffset(int yearBaseMonth = TimeSpec.CalendarYearStartMonth) {
            return new TimeCalendar(new TimeCalendarConfig
                                    {
                                        StartOffset = TimeSpan.Zero,
                                        EndOffset = TimeSpan.Zero,
                                        YearBaseMonth = yearBaseMonth
                                    });
        }

        /// <summary>
        /// 시작, 완료 Offset 모두 Zero인 TimeCalendar를 생성합니다.
        /// </summary>
        /// <param name="culture">문화권</param>
        /// <param name="yearBaseMonth">한해의 시작 월</param>
        /// <returns></returns>
        public static TimeCalendar NewEmptyOffset(CultureInfo culture, int yearBaseMonth = TimeSpec.CalendarYearStartMonth) {
            return new TimeCalendar(new TimeCalendarConfig
                                    {
                                        Culture = culture,
                                        StartOffset = TimeSpan.Zero,
                                        EndOffset = TimeSpan.Zero,
                                        YearBaseMonth = yearBaseMonth
                                    });
        }

        #endregion

        /// <summary>
        /// 기본 생성자
        /// </summary>
        public TimeCalendar() : this(new TimeCalendarConfig()) {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="config">TimeCalendar 설정 정보</param>
        public TimeCalendar(TimeCalendarConfig config) {
            if(config.StartOffset.HasValue)
                config.StartOffset.Value.ShouldBeGreaterOrEqual(TimeSpan.Zero, "StartOffset");

            if(config.EndOffset.HasValue)
                config.EndOffset.Value.ShouldBeLessOrEqual(TimeSpan.Zero, "EndOffset");

            Culture = config.Culture ?? CultureInfo.CurrentCulture;
            YearKind = config.YearKind ?? YearKind.SystemYear;
            StartOffset = config.StartOffset ?? DefaultStartOffset;
            EndOffset = config.EndOffset ?? DefaultEndOffset;
            YearBaseMonth = config.YearBaseMonth ?? TimeSpec.CalendarYearStartMonth;
            WeekOfYearRule = config.WeekOfYearRule ?? WeekOfYearRuleKind.Calendar;
            CalendarWeekRule = config.CalendarWeekRule;
            FirstDayOfWeek = config.FirstDayOfWeek;
        }

        /// <summary>
        /// <paramref name="moment"/>를 <see cref="StartOffset"/>을 적용하여 매핑합니다.
        /// </summary>
        /// <param name="moment">대상 일자</param>
        /// <returns>매핑된 일자</returns>
        public DateTime MapStart(DateTime moment) {
            return (moment > TimeSpec.MinPeriodTime) ? moment.Add(StartOffset) : moment;
        }

        /// <summary>
        /// <paramref name="moment"/>를 <see cref="EndOffset"/>을 적용하여 매핑합니다.
        /// </summary>
        /// <param name="moment">대상 일자</param>
        /// <returns>매핑된 일자</returns>
        public DateTime MapEnd(DateTime moment) {
            return (moment < TimeSpec.MaxPeriodTime) ? moment.Add(EndOffset) : moment;
        }

        /// <summary>
        /// <paramref name="moment"/>를 <see cref="StartOffset"/> 적용을 해제합니다.
        /// </summary>
        /// <param name="moment">Offset이 적용된 일자</param>
        /// <returns>Offset 적용을 제거한 일자</returns>
        public DateTime UnmapStart(DateTime moment) {
            return (moment > TimeSpec.MinPeriodTime) ? moment.Subtract(StartOffset) : moment;
        }

        /// <summary>
        /// <paramref name="moment"/>를 <see cref="EndOffset"/> 적용을 해제합니다.
        /// </summary>
        /// <param name="moment">Offset이 적용된 일자</param>
        /// <returns>Offset 적용을 제거한 일자</returns>
        public DateTime UnmapEnd(DateTime moment) {
            return (moment < TimeSpec.MaxPeriodTime) ? moment.Subtract(EndOffset) : moment;
        }

        /// <summary>
        /// 문화권 정보 (문화권에 따라 달력에 대한 규칙 및 명칭이 달라집니다.)
        /// </summary>
        public CultureInfo Culture { get; private set; }

        /// <summary>
        /// 년 종류
        /// </summary>
        public YearKind YearKind { get; private set; }

        /// <summary>
        /// 시작 오프셋 (시작일자가 1월 1일이 아닌 경우) 
        /// </summary>
        public TimeSpan StartOffset { get; private set; }

        /// <summary>
        /// 종료 오프셋
        /// </summary>
        public TimeSpan EndOffset { get; private set; }

        /// <summary>
        /// 년의 기준 월 (한국 교육년은 3월, 미국은 9월, 회계년은 2월 등 다양하다)
        /// </summary>
        public int YearBaseMonth { get; private set; }

        /// <summary>
        /// 년의 주차 계산 방식
        /// </summary>
        public WeekOfYearRuleKind WeekOfYearRule { get; private set; }

        /// <summary>
        /// 한해의 첫번째 주를 산정하는 방식
        /// </summary>
        public CalendarWeekRule CalendarWeekRule { get; private set; }

        /// <summary>
        /// 한 주의 시작 요일 (한국, 미국: Sunday, ISO-8601: Monday)
        /// </summary>
        public DayOfWeek FirstDayOfWeek { get; private set; }

        /// <summary>
        /// 지정된 일자의 년
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public int GetYear(DateTime time) {
            return Culture.Calendar.GetYear(time);
        }

        /// <summary>
        /// 지정된 일자의 월
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public int GetMonth(DateTime time) {
            return Culture.Calendar.GetMonth(time);
        }

        /// <summary>
        /// 지정된 시각의 시간
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public int GetHour(DateTime time) {
            return Culture.Calendar.GetHour(time);
        }

        /// <summary>
        /// 지정된 시각의 분
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public int GetMinute(DateTime time) {
            return Culture.Calendar.GetMinute(time);
        }

        /// <summary>
        /// 지정된 날짜의 월 몇번째 일인지
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public int GetDayOfMonth(DateTime time) {
            return Culture.Calendar.GetDayOfMonth(time);
        }

        /// <summary>
        /// 지정된 날짜의 요일
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public DayOfWeek GetDayOfWeek(DateTime time) {
            return Culture.Calendar.GetDayOfWeek(time);
        }

        /// <summary>
        /// 지정된 년,월의 날짜수
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public int GetDaysInMonth(int year, int month) {
            return Culture.Calendar.GetDaysInMonth(year, month);
        }

        /// <summary>
        /// 년도 이름
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public string GetYearName(int year) {
            switch(YearKind) {
                case YearKind.CalendarYear:
                    return TimeStrings.CalendarYearName(year);

                case YearKind.FiscalYear:
                    return TimeStrings.FiscalYearName(year);

                case YearKind.SchoolYear:
                    return TimeStrings.SchoolYearName(year);

                default:
                    return TimeStrings.SystemYearName(year);
            }
        }

        /// <summary>
        /// 반기를 표현하는 문자열을 반환합니다.
        /// </summary>
        /// <param name="halfyear"></param>
        /// <returns></returns>
        public string GetHalfYearName(HalfyearKind halfyear) {
            switch(YearKind) {
                case YearKind.CalendarYear:
                    return TimeStrings.CalendarHalfyearName(halfyear);

                case YearKind.FiscalYear:
                    return TimeStrings.FiscalHalfyearName(halfyear);

                case YearKind.SchoolYear:
                    return TimeStrings.SchoolHalfyearName(halfyear);

                default:
                    return TimeStrings.SystemHalfyearName(halfyear);
            }
        }

        /// <summary>
        /// 지정한 년도의 반기를 표현하는 문자열을 반환합니다.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="halfyear"></param>
        /// <returns></returns>
        public string GetHalfYearOfYearName(int year, HalfyearKind halfyear) {
            switch(YearKind) {
                case YearKind.CalendarYear:
                    return TimeStrings.CalendarHalfyearOfYearName(halfyear, year);

                case YearKind.FiscalYear:
                    return TimeStrings.FiscalHalfyearOfYearName(halfyear, year);

                case YearKind.SchoolYear:
                    return TimeStrings.SchoolHalfyearOfYearName(halfyear, year);

                default:
                    return TimeStrings.SystemHalfyearOfYearName(halfyear, year);
            }
        }

        /// <summary>
        /// 분기를 표현하는 문자열을 반환합니다. (2사분기)
        /// </summary>
        /// <param name="quarter"></param>
        /// <returns></returns>
        public string GetQuarterName(QuarterKind quarter) {
            switch(YearKind) {
                case YearKind.CalendarYear:
                    return TimeStrings.CalendarQuarterName(quarter);

                case YearKind.FiscalYear:
                    return TimeStrings.FiscalQuarterName(quarter);

                case YearKind.SchoolYear:
                    return TimeStrings.SchoolQuarterName(quarter);

                default:
                    return TimeStrings.SystemQuarterName(quarter);
            }
        }

        /// <summary>
        /// 특정년도의 분기를 표현하는 문자열을 반환합니다. (2011년 2사분기)
        /// </summary>
        /// <param name="year"></param>
        /// <param name="quarter"></param>
        /// <returns></returns>
        public string GetQuarterOfYearName(int year, QuarterKind quarter) {
            switch(YearKind) {
                case YearKind.CalendarYear:
                    return TimeStrings.CalendarQuarterOfYearName(quarter, year);

                case YearKind.FiscalYear:
                    return TimeStrings.FiscalQuarterOfYearName(quarter, year);

                case YearKind.SchoolYear:
                    return TimeStrings.SchoolQuarterOfYearName(quarter, year);

                default:
                    return TimeStrings.SystemQuarterOfYearName(quarter, year);
            }
        }

        /// <summary>
        /// 지정한 월을 표현하는 문자열을 반환합니다. (예: 1월 | January)
        /// </summary>
        /// <param name="month"></param>
        /// <returns></returns>
        public string GetMonthName(int month) {
            return Culture.DateTimeFormat.GetMonthName(month);
        }

        /// <summary>
        /// 월을 표현하는 문자열을 반환합니다. (예: 1월 | January)
        /// </summary>
        /// <param name="month"></param>
        /// <returns></returns>
        public string GetMonthName(MonthKind month) {
            return Culture.DateTimeFormat.GetMonthName((int)month);
        }

        /// <summary>
        /// 특정 년, 월을 표현하는 문자열을 반환합니다.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public string GetMonthOfYearName(int year, int month) {
            return TimeStrings.MonthOfYearName(GetMonthName(month), GetYearName(year));
        }

        /// <summary>
        /// 특정 년, 월을 표현하는 문자열을 반환합니다.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public string GetMonthOfYearName(int year, MonthKind month) {
            return TimeStrings.MonthOfYearName(GetMonthName(month), GetYearName(year));
        }

        /// <summary>
        /// 년,주차를 문자열로 표현합니다.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="weekOfYear"></param>
        /// <returns></returns>
        public string GetWeekOfYearName(int year, int weekOfYear) {
            return TimeStrings.WeekOfYearName(weekOfYear, GetYearName(year));
        }

        /// <summary>
        /// 지정한 요일을 문자열로 표현합니다.
        /// </summary>
        /// <param name="dayOfWeek"></param>
        /// <returns></returns>
        public string GetDayName(DayOfWeek dayOfWeek) {
            return Culture.DateTimeFormat.GetDayName(dayOfWeek);
        }

        /// <summary>
        /// 지정된 일자의 주차(Week of Year)를 반환합니다. (<see cref="CalendarWeekRule"/>, <see cref="ITimeCalendar.FirstDayOfWeek"/>에 따라 달라집니다)
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public int GetWeekOfYear(DateTime time) {
            int year, weekOfYear;
            TimeTool.GetWeekOfYear(time, Culture, WeekOfYearRule, out year, out weekOfYear);

            return weekOfYear;
        }

        /// <summary>
        /// 지정된 년, 주차에 해당하는 주의 첫번째 일자를 반환한다. (예: 2011년 3주차의 첫번째 일자는?) (<see cref="CalendarWeekRule"/>, <see cref="ITimeCalendar.FirstDayOfWeek"/>에 따라 달라집니다)
        /// </summary>
        /// <param name="year"></param>
        /// <param name="weekOfYear"></param>
        /// <returns></returns>
        public DateTime GetStartOfYearWeek(int year, int weekOfYear) {
            return TimeTool.GetStartOfYearWeek(year, weekOfYear, Culture, WeekOfYearRule);
        }

        public virtual bool Equals(ITimeCalendar other) {
            return (other != null) && GetHashCode().Equals(other.GetHashCode());
        }

        public override bool Equals(object obj) {
            return (obj != null) && (obj is ITimeCalendar) && Equals((ITimeCalendar)obj);
        }

        public override int GetHashCode() {
            return HashTool.Compute(Culture, StartOffset, EndOffset, YearBaseMonth, WeekOfYearRule);
        }

        public override string ToString() {
            return string.Format("TimeCalendar# Culture=[{0}], YearKind=[{1}], StartOffset=[{2}], EndOffset=[{3}], "
                                 + @"YearBaseMonth=[{4}],WeekOfYearRule=[{5}], CalendarWeekRule=[{6}], FirstDayOfWeek=[{7}]",
                                 Culture, YearKind, StartOffset, EndOffset,
                                 YearBaseMonth, WeekOfYearRule, CalendarWeekRule, FirstDayOfWeek);
        }
    }
}