using System;
using System.Globalization;
using NSoft.NFramework.TimePeriods.TimeRanges;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.TimePeriods {
    /// <summary>
    /// WeekOfYear 에 대한 Extension Methods 입니다.
    /// ref : http://www.simpleisbest.net/archive/2005/10/27/279.aspx
    /// ref : http://en.wikipedia.org/wiki/ISO_8601#Week_dates
    /// </summary>
    /// <remarks>
    /// <see cref="CalendarWeekRule"/> 값에 따라 WeekOfYear 가 결정된다.
    /// 
    /// FirstDay : 1월1일이 포함된 주를 무조건 첫째 주로 삼는다. (우리나라, 미국 등의 기준) : .NET의 설정대로 하면 이렇게 된다.
    /// FirstForDayWeek : 1월1일이 포함된 주가 4일 이상인 경우에만 그 해의 첫 번째 주로 삼는다.	(ISO 8601)
    ///					   예) 한 주의 시작 요일이 일요일이고 1월1일이 일/월/화/수 중 하나이면 1월1일이 포함된 주는 해당 해의 첫 번째 주이다.
    ///					   예) 한 주의 시작 요일이 일요일이고 1월1일이 목/금/토 중 하나이면 1월1일이 포함된 주는 해당 해의 첫 번째 주로 간주하지 않는다.
    ///					   예) 2005년 1월 1일은 토요일이므로 1월1일이 포함된 주는 2005년의 첫 번째 주로 간주하지 않는다.
    /// FirstFullWeek : 1월의 첫 번째 주가 7일이 아니면 해당 해의 첫 번째 주로 삼지 않는다.
    ///				    예) 한 주의 시작 요일이 일요일인 경우, 1월1일이 일요일이 아니라면 1월1일이 포함된 주는 해당 해의 첫 번째 주로 간주하지 않는다.
    /// </remarks> 
    public static class WeekTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// <paramref name="weekOfYearRule"/> 에 따라 <paramref name="weekRule"/>과  <paramref name="firstDayOfWeek"/> 를 결정합니다.
        /// </summary>
        /// <param name="culture">문화권</param>
        /// <param name="weekOfYearRule">주차 계산을 위한 룰</param>
        /// <param name="weekRule">한해의 첫주를 산정하는 규칙</param>
        /// <param name="firstDayOfWeek">한주의 첫번째 요일</param>
        public static void GetCalendarWeekRuleAndFirstDayOfWeek(CultureInfo culture,
                                                                WeekOfYearRuleKind? weekOfYearRule,
                                                                out CalendarWeekRule weekRule,
                                                                out DayOfWeek firstDayOfWeek) {
            if(weekOfYearRule.GetValueOrDefault(WeekOfYearRuleKind.Calendar) == WeekOfYearRuleKind.Calendar) {
                weekRule = culture.GetOrCurrentCulture().DateTimeFormat.CalendarWeekRule;
                firstDayOfWeek = culture.GetOrCurrentCulture().DateTimeFormat.FirstDayOfWeek;
            }
            else {
                weekRule = CalendarWeekRule.FirstFourDayWeek;
                firstDayOfWeek = DayOfWeek.Monday;
            }

            if(IsDebugEnabled)
                log.Debug("WeekOfyearRuleKind에 따른 CalendarWeekRule, FirstDayOfWeek 정보를 결정했습니다!!! " +
                          @"culture=[{0}], weekOfYearRule=[{1}], weekRule=[{2}], firstDayOfWeek=[{3}]",
                          culture, weekOfYearRule, weekRule, firstDayOfWeek);
        }

        /// <summary>
        /// 주차 계산 룰과 문화권에 따른 주차 계산 룰을 구합니다.
        /// </summary>
        /// <param name="culture">문화권</param>
        /// <param name="weekOfYearRule">주차 계산 룰</param>
        /// <returns></returns>
        public static CalendarWeekRule GetCalendarWeekRule(CultureInfo culture = null,
                                                           WeekOfYearRuleKind? weekOfYearRule = null) {
            return weekOfYearRule.GetValueOrDefault(WeekOfYearRuleKind.Calendar) == WeekOfYearRuleKind.Iso8601
                       ? CalendarWeekRule.FirstFourDayWeek
                       : culture.GetOrCurrentCulture().DateTimeFormat.CalendarWeekRule;
        }

        /// <summary>
        /// 주차 계산 룰과 문화권에 따른 한주의 첫번째 요일을 구합니다.
        /// </summary>
        /// <param name="culture">문화권</param>
        /// <param name="weekOfYearRule">주차 계산 룰</param>
        /// <returns></returns>
        public static DayOfWeek GetFirstDayOfWeek(CultureInfo culture = null,
                                                  WeekOfYearRuleKind? weekOfYearRule = null) {
            return weekOfYearRule.GetValueOrDefault(WeekOfYearRuleKind.Calendar) == WeekOfYearRuleKind.Iso8601
                       ? DayOfWeek.Monday
                       : culture.GetOrCurrentCulture().DateTimeFormat.FirstDayOfWeek;
        }

        /// <summary>
        /// <paramref name="weekRule"/>, <paramref name="firstDayOfWeek"/>에 해당하는 <see cref="WeekOfYearRuleKind"/>를 판단합니다.
        /// </summary>
        /// <param name="weekRule">한해의 첫주를 산정하는 규칙</param>
        /// <param name="firstDayOfWeek">한주의 첫번째 요일</param>
        public static WeekOfYearRuleKind GetWeekOfYearRuleKind(CalendarWeekRule? weekRule = null,
                                                               DayOfWeek? firstDayOfWeek = null) {
            if(weekRule.GetValueOrDefault(DateTimeFormatInfo.CurrentInfo.CalendarWeekRule) == CalendarWeekRule.FirstFourDayWeek &&
               firstDayOfWeek.GetValueOrDefault(DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek) == DayOfWeek.Monday)
                return WeekOfYearRuleKind.Iso8601;

            return WeekOfYearRuleKind.Calendar;
        }

        /// <summary>
        /// 해당일자의 주차를 구한다. 문화권(Culture) 및 새해 첫주차에 대한 정의에 따라 주차가 달라진다.
        /// ref : http://www.simpleisbest.net/archive/2005/10/27/279.aspx
        /// ref : http://en.wikipedia.org/wiki/ISO_8601#Week_dates
        /// </summary>
        /// <remarks>
        /// <see cref="CalendarWeekRule"/> 값에 따라 WeekOfYear 가 결정된다.
        /// 
        /// FirstDay : 1월1일이 포함된 주를 무조건 첫째 주로 삼는다. (우리나라, 미국 등의 기준) : .NET의 설정대로 하면 이렇게 된다.
        /// FirstForDayWeek : 1월1일이 포함된 주가 4일 이상인 경우에만 그 해의 첫 번째 주로 삼는다.	(ISO 8601)
        ///					   예) 한 주의 시작 요일이 일요일이고 1월1일이 일/월/화/수 중 하나이면 1월1일이 포함된 주는 해당 해의 첫 번째 주이다.
        ///					   예) 한 주의 시작 요일이 일요일이고 1월1일이 목/금/토 중 하나이면 1월1일이 포함된 주는 해당 해의 첫 번째 주로 간주하지 않는다.
        ///					   예) 2005년 1월 1일은 토요일이므로 1월1일이 포함된 주는 2005년의 첫 번째 주로 간주하지 않는다.
        /// FirstFullWeek : 1월의 첫 번째 주가 7일이 아니면 해당 해의 첫 번째 주로 삼지 않는다.
        ///				    예) 한 주의 시작 요일이 일요일인 경우, 1월1일이 일요일이 아니라면 1월1일이 포함된 주는 해당 해의 첫 번째 주로 간주하지 않는다.
        /// </remarks>
        /// <param name="moment">주차(WeekOfYear)를 산정하기 위한 일자</param>
        /// <param name="yearStartMonth">한 해의 시작 월</param>
        /// <param name="culture">해당 Calendar를 얻기위한 문화권</param>
        /// <param name="weekOfYearRule">주차를 산정하는 방식</param>
        /// <returns>지정된 일자가 속한 Week Of Year를 반환</returns>
        public static YearAndWeek GetYearAndWeek(this DateTime moment,
                                                 CultureInfo culture = null,
                                                 WeekOfYearRuleKind weekOfYearRule = WeekOfYearRuleKind.Calendar,
                                                 int yearStartMonth = TimeSpec.CalendarYearStartMonth) {
            return GetYearAndWeek(moment, TimeCalendar.New(culture, yearStartMonth, weekOfYearRule));
        }

        /// <summary>
        /// 해당일자의 주차를 구한다. 문화권(Culture) 및 새해 첫주차에 대한 정의에 따라 주차가 달라진다.
        /// ref : http://www.simpleisbest.net/archive/2005/10/27/279.aspx
        /// ref : http://en.wikipedia.org/wiki/ISO_8601#Week_dates
        /// </summary>
        /// <remarks>
        /// <see cref="CalendarWeekRule"/> 값에 따라 WeekOfYear 가 결정된다.
        /// 
        /// FirstDay : 1월1일이 포함된 주를 무조건 첫째 주로 삼는다. (우리나라, 미국 등의 기준) : .NET의 설정대로 하면 이렇게 된다.
        /// FirstFourDayWeek : 1월1일이 포함된 주가 4일 이상인 경우에만 그 해의 첫 번째 주로 삼는다.	(ISO 8601)
        ///					   예) 한 주의 시작 요일이 일요일이고 1월1일이 일/월/화/수 중 하나이면 1월1일이 포함된 주는 해당 해의 첫 번째 주이다.
        ///					   예) 한 주의 시작 요일이 일요일이고 1월1일이 목/금/토 중 하나이면 1월1일이 포함된 주는 해당 해의 첫 번째 주로 간주하지 않는다.
        ///					   예) 2005년 1월 1일은 토요일이므로 1월1일이 포함된 주는 2005년의 첫 번째 주로 간주하지 않는다.
        /// FirstFullWeek : 1월의 첫 번째 주가 7일이 아니면 해당 해의 첫 번째 주로 삼지 않는다.
        ///				    예) 한 주의 시작 요일이 일요일인 경우, 1월1일이 일요일이 아니라면 1월1일이 포함된 주는 해당 해의 첫 번째 주로 간주하지 않는다.
        /// </remarks>
        /// <param name="moment">주차(WeekOfYear)를 산정하기 위한 일자</param>
        /// <param name="timeCalendar">주차 계산을 위한 규칙 정보를 가진 TimeCalendar 인스턴스</param>
        /// <returns>지정된 일자가 속한 Week Of Year를 반환</returns>
        public static YearAndWeek GetYearAndWeek(this DateTime moment, ITimeCalendar timeCalendar) {
            timeCalendar.ShouldNotBeNull("timeCalendar");

            var culture = timeCalendar.Culture.GetOrCurrentCulture();
            var weekRule = timeCalendar.CalendarWeekRule;
            var firstDayOfWeek = timeCalendar.FirstDayOfWeek;

            if(IsDebugEnabled)
                log.Debug("특정일[{0}] 의 주차를 계산합니다. culture=[{1}], weekRule=[{2}], firstDayOfWeek=[{3}]", moment, culture, weekRule,
                          firstDayOfWeek);

            var week = culture.Calendar.GetWeekOfYear(moment, weekRule, firstDayOfWeek);
            var year = moment.Year;

            //!+ NOTE: .NET 라이브러리가 1월1일 기준으로는 정상작동하지만, 12월 31로 계산하면, 무조건 FirstDay 형식으로 작업해버린다.
            //!+ FirstFourDayWeek Rule에 따르면 12월 31일이 다음해의 첫주차에 속할 경우도 있지만, .NET에서는 53주차로 반환해 버린다.
            //!+ 예 12월 31일이 월요일 경우 2001년 53주차가 아니라 2002년 1주차가 되어야 한다.
            //!+ 이를 해결하기 위해 부가적인 작업이 들어간다.
            //
            if(weekRule == CalendarWeekRule.FirstFourDayWeek && firstDayOfWeek == DayOfWeek.Monday) {
                var weekRange = new TimeRange(TimeTool.StartTimeOfWeek(moment, (DayOfWeek?)firstDayOfWeek), DurationUtil.Week);
                if(moment.Month == 12 && weekRange.HasInside(new DateTime(year + 1, 1, 1))) {
                    var startDate = moment.AddYears(1).StartTimeOfYear();
                    if((int)startDate.DayOfWeek > (int)firstDayOfWeek &&
                       (int)startDate.DayOfWeek - (int)firstDayOfWeek < 4) {
                        year++;
                        week = 1;
                    }
                }
            }
            // NOTE : 연도 보정 (1월인데, Week가 충분히 큰 숫자 이상이라면, 전년도의 주차를 따른다는 것이다. 그러므로 Year를 전년도로 설정해준다.
            if(moment.Month == 1 && week > 10)
                year--;

            var result = new YearAndWeek(year, week);

            if(IsDebugEnabled)
                log.Debug("일자[{0}] 의 주차는 [{4}]입니다. culture=[{1}], weekRule=[{2}], firstDayOfWeek=[{3}]", moment, culture, weekRule,
                          firstDayOfWeek, result);

            return result;
        }

        /// <summary>
        /// 해당 년도의 주차산정방식에 따라 마지막 주차를 산정합니다. 
        /// </summary>
        /// <param name="year">해당 년도</param>
        /// <param name="yearStartMonth">년의 시작 월</param>
        /// <param name="culture">문화권</param>
        /// <param name="weekOfYearRule">한해의 첫번째 주차를 산정하는 방식</param>
        /// <returns>해당 년도의 마지막 주차</returns>
        public static YearAndWeek GetEndYearAndWeek(this int year,
                                                    CultureInfo culture = null,
                                                    WeekOfYearRuleKind weekOfYearRule = WeekOfYearRuleKind.Calendar,
                                                    int yearStartMonth = TimeSpec.CalendarYearStartMonth) {
            return GetEndYearAndWeek(year, TimeCalendar.New(culture, yearStartMonth, weekOfYearRule));
        }

        /// <summary>
        /// 해당 년도의 주차산정방식에 따라 마지막 주차를 산정합니다. 
        /// </summary>
        /// <param name="year">해당 년도</param>
        /// <param name="timeCalendar">TimeCalendar</param>
        /// <returns>해당 년도의 마지막 주차</returns>
        public static YearAndWeek GetEndYearAndWeek(this int year, ITimeCalendar timeCalendar) {
            timeCalendar.ShouldNotBeNull("timeCalendar");
            var yearStartMonth = timeCalendar.YearBaseMonth;

            if(IsDebugEnabled)
                log.Debug("해당년도의 마지막 주차를 계산합니다... year=[{0}], yearStartMonth=[{1}], timeCalendar=[{2}]", year, yearStartMonth,
                          timeCalendar);

            var yw = GetYearAndWeek(TimeTool.EndTimeOfYear(year, yearStartMonth), timeCalendar);
            var endOfWeek = (yw.Year == year)
                                ? yw
                                : GetYearAndWeek(TimeTool.EndTimeOfYear(year, yearStartMonth).AddDays(-TimeSpec.DaysPerWeek));

            if(IsDebugEnabled)
                log.Debug("해당년도의 마지막 주차를 얻었습니다. " +
                          "year=[{0}], yearStartMonth=[{1}], timeCalendar=[{2}], endOfWeek=[{3}]",
                          year, yearStartMonth, timeCalendar, endOfWeek);

            return endOfWeek;
        }

        /// <summary>
        /// <paramref name="yearAndWeek"/> 주차에 해당하는 기간을 <see cref="WeekRange"/> 로 반환합니다. (예: 2011년 28주차의 실제 기간)
        /// </summary>
        /// <param name="yearAndWeek">년도와 주차 정보</param>
        /// <param name="yearStartMonth">한 해의 시작 월</param>
        /// <param name="culture">문하권</param>
        /// <param name="weekOfYearRule">한해의 첫주를 산정하는 규칙</param>
        /// <returns>주차에 해당하는 한주의 기간</returns>
        public static WeekRange GetWeekRange(this YearAndWeek yearAndWeek,
                                             CultureInfo culture = null,
                                             WeekOfYearRuleKind weekOfYearRule = WeekOfYearRuleKind.Calendar,
                                             int yearStartMonth = TimeSpec.CalendarYearStartMonth) {
            var timeCalendar = TimeCalendar.New(culture, yearStartMonth, weekOfYearRule, TimeSpan.Zero, TimeSpan.Zero);
            return GetWeekRange(yearAndWeek, timeCalendar);
        }

        /// <summary>
        /// <paramref name="yearAndWeek"/> 주차에 해당하는 기간을 <see cref="WeekRange"/> 로 반환합니다. (예: 2011년 28주차의 실제 기간)
        /// </summary>
        /// <param name="yearAndWeek">년도와 주차 정보</param>
        /// <param name="timeCalendar">TimeCalendar 옵션</param>
        /// <returns>주차에 해당하는 한주의 기간</returns>
        public static WeekRange GetWeekRange(this YearAndWeek yearAndWeek, ITimeCalendar timeCalendar) {
            //var timeCalendar = new TimeCalendar(new TimeCalendarConfig
            //                                    {
            //                                        Culture = culture,
            //                                        WeekOfYearRule = GetWeekOfYearRuleKind(weekRule, firstDayOfWeek),
            //                                        EndOffset = TimeSpan.Zero
            //                                    });


            yearAndWeek.Year = yearAndWeek.Year ?? 0;
            var endYearAndWeek = GetEndYearAndWeek(yearAndWeek.Year.Value, timeCalendar);
            yearAndWeek.Week = Math.Max(1, Math.Min(yearAndWeek.Week.Value, endYearAndWeek.Week.Value));

            // 년/주에 가장 가까운 월을 선택해서 2개월만 검사한다.
            // 1년 범위는 너무 느리다.
            // var searchDays = RwDate.GetStartOfYear(yearWeek.Year).GetYearRange();
            // 년/주에 가장 가까운 월을 선택해서 2개월만 검사한다.
            var nearMonth =
                yearAndWeek.Year.Value
                    .GetStartOfMonth(Math.Max(1, Math.Min(TimeSpec.MonthsPerYear, (yearAndWeek.Week.Value - 1) / 4 + 1)))
                    .GetMonthRange();

            var searchDays = new TimeRange(nearMonth.GetPreviousMonth().Start, nearMonth.GetNextMonth().End);

            var weekPeriod = new TimeRange();

            // 년도 경계 때문에 전년도 마지막 7일, 후년 첫 7일을 검색에 포함시킨다.
            //if (yearWeek.Week == 1)
            //    searchDays.Start = searchDays.Start.Value.AddDays(-7);
            //else if (yearWeek.Week > 50)
            //    searchDays.End = searchDays.End.Value.AddDays(7);

            // 지정 년도의 주차에 해당하는 날짜를 계산한다.
            var dayCount = 0;
            foreach(TimeRange dayRange in searchDays.ForEachDays()) {
                var startDay = dayRange.Start;
                var dayYearWeek = GetYearAndWeek(startDay, timeCalendar);

                if(dayYearWeek.Equals(yearAndWeek)) {
                    if(weekPeriod.HasStart == false) {
                        weekPeriod.Start = startDay;

                        if(timeCalendar.WeekOfYearRule == WeekOfYearRuleKind.Iso8601) {
                            weekPeriod.End = startDay.AddDays(TimeSpec.DaysPerWeek);
                            break;
                        }
                    }
                    else
                        weekPeriod.End = startDay;

                    dayCount++;
                }
                else if(dayCount > 0) {
                    // 연속된 날짜 이후가 같은 주차가 아니라면 더 찾을 필요가 없다.
                    break;
                }
            }
            if(weekPeriod.HasStart && weekPeriod.HasEnd == false) {
                // 다음해 새해 첫날이 첫주라면, 12월31로 끝나야 하고, 아니면 마지막 주의 끝은 다음해의 토요일까지이다.
                weekPeriod.End = (timeCalendar.CalendarWeekRule == CalendarWeekRule.FirstDay)
                                     ? weekPeriod.Start
                                     : weekPeriod.Start.AddDays(TimeSpec.DaysPerWeek);
            }

            Guard.Assert(weekPeriod.HasPeriod, "해당 주차의 범위를 찾지 못했습니다. YearAndWeek=[{0}]", yearAndWeek);

            return new WeekRange(weekPeriod, timeCalendar);
        }

        /// <summary>
        /// 해당 년도의 첫번째 주차에 해당하는 기간을 <see cref="WeekRange"/>로 반환합니다.
        /// </summary>
        /// <param name="year">년도</param>
        /// <param name="yearStartMonth">한 해의 시작 월</param>
        /// <param name="culture">문하권</param>
        /// <param name="weekOfYearRule">한 해의 첫주를 산정하는 룰</param>
        /// <returns>첫번째 주차에 해당하는 한주의 기간</returns>
        public static WeekRange GetStartWeekRangeOfYear(this int year,
                                                        CultureInfo culture = null,
                                                        WeekOfYearRuleKind weekOfYearRule = WeekOfYearRuleKind.Calendar,
                                                        int yearStartMonth = TimeSpec.CalendarYearStartMonth) {
            var timeCalendar = TimeCalendar.New(culture, yearStartMonth, weekOfYearRule);
            return GetStartWeekRangeOfYear(year, timeCalendar);
        }

        /// <summary>
        /// 해당 년도의 첫번째 주차에 해당하는 기간을 <see cref="WeekRange"/>로 반환합니다.
        /// </summary>
        /// <param name="year">년도</param>
        /// <param name="timeCalendar">TimeCalendar 옵션</param>
        /// <returns>첫번째 주차에 해당하는 한주의 기간</returns>
        public static WeekRange GetStartWeekRangeOfYear(this int year, ITimeCalendar timeCalendar) {
            return GetWeekRange(new YearAndWeek(year, 1), timeCalendar);
        }

        /// <summary>
        /// 해당 년도의 마지막 주차에 해당하는 기간을 <see cref="WeekRange"/>로 반환합니다.
        /// </summary>
        /// <param name="year">년도</param>
        /// <param name="yearStartMonth">한 해의 시작 월</param>
        /// <param name="culture">문하권</param>
        /// <param name="weekOfYearRule">한 해의 첫주를 산정하는 룰</param>
        /// <returns>마지막 주차에 해당하는 한주의 기간</returns>
        public static WeekRange GetEndWeekRangeOfYear(this int year,
                                                      CultureInfo culture = null,
                                                      WeekOfYearRuleKind weekOfYearRule = WeekOfYearRuleKind.Calendar,
                                                      int yearStartMonth = TimeSpec.CalendarYearStartMonth) {
            var timeCalendar = TimeCalendar.New(culture, yearStartMonth, weekOfYearRule);
            return GetEndWeekRangeOfYear(year, timeCalendar);
        }

        /// <summary>
        /// 해당 년도의 마지막 주차에 해당하는 기간을 <see cref="WeekRange"/>로 반환합니다.
        /// </summary>
        /// <param name="year">년도</param>
        /// <param name="timeCalendar">TimeCalendar 옵션</param>
        /// <returns>마지막 주차에 해당하는 한주의 기간</returns>
        public static WeekRange GetEndWeekRangeOfYear(this int year, ITimeCalendar timeCalendar) {
            var endYearAndWeek = GetEndYearAndWeek(year, timeCalendar);
            return GetWeekRange(endYearAndWeek, timeCalendar);
        }

        /// <summary>
        /// <paramref name="yearAndWeek"/>에 <paramref name="weeks"/>만큼의 주차를 더한 주차를 계산합니다.
        /// </summary>
        /// <param name="yearAndWeek">기준 주차</param>
        /// <param name="weeks">더할 주차 값</param>
        /// <param name="yearStartMonth">한해의 시작 월</param>
        /// <param name="culture">문화권</param>
        /// <param name="weekOfYearRule">주차 산정을 위한 룰</param>
        /// <returns>기준 주차에 주차를 더한 주차 정보</returns>
        public static YearAndWeek AddWeekOfYears(this YearAndWeek yearAndWeek,
                                                 int weeks,
                                                 CultureInfo culture = null,
                                                 WeekOfYearRuleKind weekOfYearRule = WeekOfYearRuleKind.Calendar,
                                                 int yearStartMonth = TimeSpec.CalendarYearStartMonth) {
            var timeCalendar = TimeCalendar.New(culture, yearStartMonth, weekOfYearRule);
            return AddWeekOfYears(yearAndWeek, weeks, timeCalendar);
        }

        /// <summary>
        /// <paramref name="yearAndWeek"/>에 <paramref name="weeks"/>만큼의 주차를 더한 주차를 계산합니다.
        /// </summary>
        /// <param name="yearAndWeek">기준 주차</param>
        /// <param name="weeks">더할 주차 값</param>
        /// <param name="timeCalendar">TimeCalendar</param>
        /// <returns>기준 주차에 주차를 더한 주차 정보</returns>
        public static YearAndWeek AddWeekOfYears(this YearAndWeek yearAndWeek, int weeks, ITimeCalendar timeCalendar) {
            timeCalendar.ShouldNotBeNull("timeCalendar");


            if(IsDebugEnabled)
                log.Debug("주차 연산을 수행합니다... yearAndWeek=[{0}], weeks=[{1}], timeCalendar=[{2}]",
                          yearAndWeek, weeks, timeCalendar);


            var result = new YearAndWeek(yearAndWeek.Year, yearAndWeek.Week);

            if(weeks == 0)
                return result;

            if(weeks > 0) {
                weeks += result.Week.Value;
                if(weeks < GetEndYearAndWeek(yearAndWeek.Year.Value, timeCalendar).Week) {
                    result.Week = weeks;
                    return result;
                }
                while(weeks >= 0) {
                    var endWeek = GetEndYearAndWeek(result.Year.Value, timeCalendar);
                    if(weeks <= endWeek.Week) {
                        result.Week = Math.Max(weeks, 1);
                        return result;
                    }
                    weeks -= endWeek.Week.Value;
                    result.Year++;
                }
                result.Week = Math.Max(weeks, 1);
            }
            else {
                weeks += result.Week.Value;

                if(weeks == 0) {
                    result.Year--;
                    result.Week = GetEndYearAndWeek(result.Year.Value, timeCalendar).Week.Value;
                    return result;
                }
                if(weeks > 0) {
                    result.Week = weeks;
                    return result;
                }
                while(weeks <= 0) {
                    result.Year--;
                    var endWeek = GetEndYearAndWeek(result.Year.Value, timeCalendar);
                    weeks += endWeek.Week.Value;

                    if(weeks > 0) {
                        result.Week = Math.Max(weeks, 1);
                        return result;
                    }
                }
                result.Week = Math.Max(weeks, 1);
            }

            if(IsDebugEnabled)
                log.Debug("주차 연산을 수행했습니다!!! yearAndWeek=[{0}], weeks=[{1}], result=[{2}]", yearAndWeek, weeks, result);

            return result;
        }
    }
}