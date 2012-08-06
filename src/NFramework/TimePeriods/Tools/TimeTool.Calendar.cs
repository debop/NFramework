using System;
using System.Globalization;

namespace NSoft.NFramework.TimePeriods {
    public static partial class TimeTool {
        /// <summary>
        /// 지정된 Calendar 기준으로  <paramref name="moment"/>가 속한 년도를 구한다.
        /// </summary>
        public static int GetYearOf(this ITimeCalendar calendar, DateTime moment) {
            return GetYearOf(calendar.YearBaseMonth, calendar.GetYear(moment), calendar.GetMonth(moment));
        }

        /// <summary>
        /// <paramref name="moment"/> 가 속한 년도를 반환한다.
        /// </summary>
        /// <param name="moment"></param>
        /// <param name="yearBaseMonth"></param>
        /// <returns></returns>
        public static int GetYearOf(this DateTime moment, int yearBaseMonth) {
            return GetYearOf(yearBaseMonth, moment.Year, moment.Month);
        }

        public static int GetYearOf(int yearBaseMonth, int year, int month) {
            return month >= yearBaseMonth ? year : year - 1;
        }

        public static YearAndHalfyear NextHalfyear(HalfyearKind startHalfyearKind) {
            int year;
            HalfyearKind halfyear;

            NextHalfyear(startHalfyearKind, out year, out halfyear);

            return new YearAndHalfyear(year, halfyear);
        }

        public static void NextHalfyear(HalfyearKind startHalfyearKind, out int year, out HalfyearKind halfyear) {
            AddHalfyear(startHalfyearKind, 1, out year, out halfyear);
        }

        public static YearAndHalfyear PreviousHalfyear(HalfyearKind startHalfyearKind) {
            int year;
            HalfyearKind halfyear;

            PreviousHalfyear(startHalfyearKind, out year, out halfyear);

            return new YearAndHalfyear(year, halfyear);
        }

        public static void PreviousHalfyear(HalfyearKind startHalfyearKind, out int year, out HalfyearKind halfyear) {
            AddHalfyear(startHalfyearKind, -1, out year, out halfyear);
        }

        public static YearAndHalfyear AddHalfyear(this HalfyearKind startHalfyearKind, int count) {
            return AddHalfyear(startHalfyearKind, 0, count);
        }

        public static YearAndHalfyear AddHalfyear(this HalfyearKind startHalfyearKind, int startYear, int count) {
            int year;
            HalfyearKind halfyear;

            AddHalfyear(startYear, startHalfyearKind, count, out year, out halfyear);

            return new YearAndHalfyear(year, halfyear);
        }

        public static void AddHalfyear(HalfyearKind startHalfyearKind, int count, out int year, out HalfyearKind halfyear) {
            AddHalfyear(0, startHalfyearKind, count, out year, out halfyear);
        }

        public static void AddHalfyear(int startYear, HalfyearKind startHalfyearKind, int count, out int year, out HalfyearKind halfyear) {
            int offsetYear = (Math.Abs(count) / TimeSpec.HalfyearsPerYear) + 1;
            int startHalfyearCount = ((startYear + offsetYear) * TimeSpec.HalfyearsPerYear) + ((int)startHalfyearKind - 1);
            int targetHalfyearCount = startHalfyearCount + count;

            year = (targetHalfyearCount / TimeSpec.HalfyearsPerYear) - offsetYear;
            halfyear = (HalfyearKind)((targetHalfyearCount % TimeSpec.HalfyearsPerYear) + 1);
        }

        public static HalfyearKind GetHalfyearOfMonth(int yearMonth) {
            return GetHalfyearOfMonth(TimeSpec.CalendarYearStartMonth, yearMonth);
        }

        public static HalfyearKind GetHalfyearOfMonth(int yearBaseMonth, int yearMonth) {
            int yearMonthIndex = (int)yearMonth - 1;
            int yearStartMonthIndex = (int)yearBaseMonth - 1;
            if(yearMonthIndex < yearStartMonthIndex) {
                yearMonthIndex += TimeSpec.MonthsPerYear;
            }
            int deltaMonths = yearMonthIndex - yearStartMonthIndex;
            return (HalfyearKind)((deltaMonths / TimeSpec.MonthsPerHalfyear) + 1);
        }

        public static int[] GetMonthsOfHalfyear(this HalfyearKind halfyear) {
            switch(halfyear) {
                case HalfyearKind.First:
                    return TimeSpec.FirstHalfyearMonths;
                case HalfyearKind.Second:
                    return TimeSpec.SecondHalfyearMonths;
            }
            throw new InvalidOperationException("Invalid year halfyear " + halfyear);
        }

        public static YearAndQuarter NextQuarter(this QuarterKind startQuarterKind) {
            int year;
            QuarterKind quarter;

            NextQuarter(startQuarterKind, out year, out quarter);
            return new YearAndQuarter(year, quarter);
        }

        public static void NextQuarter(this QuarterKind startQuarterKind, out int year, out QuarterKind quarter) {
            AddQuarter(startQuarterKind, 1, out year, out quarter);
        }

        public static YearAndQuarter PreviousQuarter(this QuarterKind startQuarter) {
            int year;
            QuarterKind quarterKind;
            PreviousQuarter(startQuarter, out year, out quarterKind);

            return new YearAndQuarter(year, quarterKind);
        }

        public static void PreviousQuarter(this QuarterKind startQuarter, out int year, out QuarterKind quarter) {
            AddQuarter(startQuarter, -1, out year, out quarter);
        }

        public static YearAndQuarter AddQuarter(this QuarterKind startQuarter, int count) {
            return AddQuarter(startQuarter, 0, count);
        }

        public static YearAndQuarter AddQuarter(this QuarterKind startQuarter, int startYear, int count) {
            int year;
            QuarterKind quarterKind;
            AddQuarter(startQuarter, startYear, count, out year, out quarterKind);

            return new YearAndQuarter(year, quarterKind.GetHashCode());
        }

        public static void AddQuarter(this QuarterKind startQuarter, int count, out int year, out QuarterKind quarter) {
            AddQuarter(startQuarter, 0, count, out year, out quarter);
        }

        public static void AddQuarter(QuarterKind startQuarter, int startYear, int count, out int year, out QuarterKind quarter) {
            int offsetYear = (Math.Abs(count) / TimeSpec.QuartersPerYear) + 1;
            int startQuarterCount = ((startYear + offsetYear) * TimeSpec.QuartersPerYear) + ((int)startQuarter - 1);
            int targetQuarterCount = startQuarterCount + count;

            year = (targetQuarterCount / TimeSpec.QuartersPerYear) - offsetYear;
            quarter = (QuarterKind)((targetQuarterCount % TimeSpec.QuartersPerYear) + 1);
        }

        public static QuarterKind GetQuarterOfMonth(int yearMonth) {
            return GetQuarterOfMonth(TimeSpec.CalendarYearStartMonth, yearMonth);
        }

        public static QuarterKind GetQuarterOfMonth(int yearBaseMonth, int yearMonth) {
            int yearMonthIndex = (int)yearMonth - 1;
            int yearStartMonthIndex = (int)yearBaseMonth - 1;
            if(yearMonthIndex < yearStartMonthIndex) {
                yearMonthIndex += TimeSpec.MonthsPerYear;
            }
            int deltaMonths = yearMonthIndex - yearStartMonthIndex;
            return (QuarterKind)((deltaMonths / TimeSpec.MonthsPerQuarter) + 1);
        }

        public static int[] GetMonthsOfQuarter(this QuarterKind quarter) {
            switch(quarter) {
                case QuarterKind.First:
                    return TimeSpec.FirstQuarterMonths;
                case QuarterKind.Second:
                    return TimeSpec.SecondQuarterMonths;
                case QuarterKind.Third:
                    return TimeSpec.ThirdQuarterMonths;
                case QuarterKind.Fourth:
                    return TimeSpec.FourthQuarterMonths;
            }
            throw new InvalidOperationException("invalid year quarter " + quarter);
        }

        public static YearAndMonth NextMonth(this int startMonth) {
            int year, month;

            NextMonth(startMonth, out year, out month);
            return new YearAndMonth(year, month);
        }

        public static void NextMonth(this int startMonth, out int year, out int month) {
            AddMonth(startMonth, 1, out year, out month);
        }

        public static YearAndMonth PreviousMonth(this int startMonth) {
            int year, month;

            PreviousMonth(startMonth, out year, out month);
            return new YearAndMonth(year, month);
        }

        public static void PreviousMonth(int startMonth, out int year, out int month) {
            AddMonth(startMonth, -1, out year, out month);
        }

        public static YearAndMonth AddMonth(this int startMonth, int count) {
            int year, month;

            AddMonth(startMonth, count, out year, out month);
            return new YearAndMonth(year, month);
        }

        public static void AddMonth(int startMonth, int count, out int year, out int month) {
            AddMonth(0, startMonth, count, out year, out month);
        }

        public static YearAndMonth AddMonth(this YearAndMonth startYearMonth, int count) {
            int year, month;
            AddMonth(startYearMonth.Year ?? 0, startYearMonth.Month ?? 1, count, out year, out month);
            return new YearAndMonth(year, month);
        }

        public static YearAndMonth AddMonth(int startYear, int startMonth, int count) {
            int year, month;
            AddMonth(startYear, startMonth, count, out year, out month);
            return new YearAndMonth(year, month);
        }

        public static void AddMonth(int startYear, int startMonth, int count, out int year, out int month) {
            int offsetYear = (Math.Abs(count) / TimeSpec.MonthsPerYear) + 1;
            int startMonthCount = ((startYear + offsetYear) * TimeSpec.MonthsPerYear) + ((int)startMonth - 1);
            int targetMonthCount = startMonthCount + count;

            year = (targetMonthCount / TimeSpec.MonthsPerYear) - offsetYear;
            month = (int)((targetMonthCount % TimeSpec.MonthsPerYear) + 1);
        }

        public static int GetDaysInMonth(this int year, int month) {
            DateTime firstDay = new DateTime(year, month, 1);
            return firstDay.AddMonths(1).AddDays(-1).Day;
        }

        public static DateTime GetStartOfWeek(this DateTime time, DayOfWeek firstDayOfWeek) {
            var currentDay = new DateTime(time.Year, time.Month, time.Day);

            while(currentDay.DayOfWeek != firstDayOfWeek) {
                currentDay = currentDay.AddDays(-1);
            }
            return currentDay;
        }

        public static YearAndWeek GetWeekOfYear(this DateTime moment, CultureInfo culture, WeekOfYearRuleKind weekOfYearRule) {
            return WeekTool.GetYearAndWeek(moment, culture, weekOfYearRule);
        }

        public static YearAndWeek GetWeekOfYear(this DateTime moment, CultureInfo culture, CalendarWeekRule weekRule,
                                                DayOfWeek firstDayOfWeek, WeekOfYearRuleKind weekOfYearRule) {
            return WeekTool.GetYearAndWeek(moment, culture, weekOfYearRule);
        }

        public static void GetWeekOfYear(this DateTime moment, CultureInfo culture, WeekOfYearRuleKind weekOfYearRule, out int year,
                                         out int weekOfYear) {
            GetWeekOfYear(moment, culture, culture.DateTimeFormat.CalendarWeekRule, culture.DateTimeFormat.FirstDayOfWeek,
                          weekOfYearRule, out year, out weekOfYear);
        }

        public static void GetWeekOfYear(this DateTime moment, CultureInfo culture, CalendarWeekRule weekRule, DayOfWeek firstDayOfWeek,
                                         WeekOfYearRuleKind weekOfYearRule, out int year, out int weekOfYear) {
            var yearAndWeek = WeekTool.GetYearAndWeek(moment, culture, weekOfYearRule);

            year = yearAndWeek.Year ?? 0;
            weekOfYear = yearAndWeek.Week ?? 0;

            //culture.ShouldNotBeNull("culture");


            //// NOTE: ISO 8601이 FirstFourDayWeek, Monday를 기준으로 하는데 왜 이렇게 하는지 모르겠네?
            ////
            //if(weekOfYearRule == WeekOfYearRuleKind.Iso8601 && weekRule == CalendarWeekRule.FirstFourDayWeek && firstDayOfWeek == DayOfWeek.Monday)
            //{
            //    //
            //    // NOTE: see http://blogs.msdn.com/b/shawnste/archive/2006/01/24/517178.aspx
            //    //
            //    var day = culture.Calendar.GetDayOfWeek(moment);
            //    if(day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            //    {
            //        moment = moment.AddDays(3);
            //    }
            //}

            //weekOfYear = culture.Calendar.GetWeekOfYear(moment, weekRule, firstDayOfWeek);
            //year = moment.Year;
            //if(weekOfYear >= 52 && moment.Month < 12)
            //{
            //    year--;
            //}
        }

        public static int GetWeeksOfYear(int year, CultureInfo culture, WeekOfYearRuleKind weekOfYearRuleKind) {
            return GetWeeksOfYear(year, culture, culture.DateTimeFormat.CalendarWeekRule, culture.DateTimeFormat.FirstDayOfWeek,
                                  weekOfYearRuleKind);
        }

        public static int GetWeeksOfYear(int year, CultureInfo culture, CalendarWeekRule weekRule, DayOfWeek firstDayOfWeek,
                                         WeekOfYearRuleKind weekOfYearRuleKind) {
            if(culture == null) {
                throw new ArgumentNullException("culture");
            }

            int currentYear;
            int currentWeek;
            DateTime currentDay = new DateTime(year, 12, 31);
            GetWeekOfYear(currentDay, culture, weekRule, firstDayOfWeek, weekOfYearRuleKind, out currentYear, out currentWeek);
            while(currentYear != year) {
                currentDay = currentDay.AddDays(-1);
                GetWeekOfYear(currentDay, culture, weekRule, firstDayOfWeek, weekOfYearRuleKind, out currentYear, out currentWeek);
            }
            return currentWeek;
        }

        public static DateTime GetStartOfYearWeek(int year, int weekOfYear, CultureInfo culture, WeekOfYearRuleKind weekOfYearRuleKind) {
            return GetStartOfYearWeek(year, weekOfYear, culture,
                                      culture.DateTimeFormat.CalendarWeekRule, culture.DateTimeFormat.FirstDayOfWeek, weekOfYearRuleKind);
        }

        public static DateTime GetStartOfYearWeek(int year, int weekOfYear, CultureInfo culture, CalendarWeekRule weekRule,
                                                  DayOfWeek firstDayOfWeek, WeekOfYearRuleKind weekOfYearRuleKind) {
            culture.ShouldNotBeNull("culture");
            weekOfYear.ShouldBePositive("weekOfYear");

            DateTime dateTime = new DateTime(year, 1, 1).AddDays(weekOfYear * TimeSpec.DaysPerWeek);
            int currentYear;
            int currentWeek;

            GetWeekOfYear(dateTime, culture, weekRule, firstDayOfWeek, weekOfYearRuleKind, out currentYear, out currentWeek);


            // end date of week
            while(currentWeek != weekOfYear) {
                dateTime = dateTime.AddDays(-1);
                GetWeekOfYear(dateTime, culture, weekRule, firstDayOfWeek, weekOfYearRuleKind, out currentYear, out currentWeek);
            }

            // end of previous week
            while(currentWeek == weekOfYear) {
                dateTime = dateTime.AddDays(-1);
                GetWeekOfYear(dateTime, culture, weekRule, firstDayOfWeek, weekOfYearRuleKind, out currentYear, out currentWeek);
            }

            return dateTime.AddDays(1);
        }

        public static DateTime DayStart(this DateTime dateTime) {
            return dateTime.Date;
        }

        /// <summary>
        /// 다음 요일
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        public static DayOfWeek NextDay(this DayOfWeek day) {
            return AddDays(day, 1);
        }

        /// <summary>
        /// 이전 요일
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        public static DayOfWeek PreviousDay(this DayOfWeek day) {
            return AddDays(day, -1);
        }

        /// <summary>
        /// 요일 계산을 수행한다.
        /// </summary>
        /// <param name="day"></param>
        /// <param name="days"></param>
        /// <returns></returns>
        public static DayOfWeek AddDays(this DayOfWeek day, int days) {
            if(days == 0) {
                return day;
            }

            var weeks = (Math.Abs(days) / TimeSpec.DaysPerWeek) + 1;

            var offset = weeks * TimeSpec.DaysPerWeek + (int)day;
            var targetOffset = offset + days;

            return (DayOfWeek)(targetOffset % TimeSpec.DaysPerWeek);
        }
    }
}