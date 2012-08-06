using System.Globalization;

namespace NSoft.NFramework.TimePeriods {
    /// <summary>
    ///Time 
    /// </summary>
    internal static class TimeStrings {
        #region << Year >>

        public const string SystemYearNameFormat = "{0}";
        public const string CalendarYearNameFormat = "CY{0}";
        public const string FiscalYearNameFormat = "FY{0}";
        public const string SchoolYearNameFormat = "SY{0}";

        public static string SystemYearName(int year) {
            return Format(SystemYearNameFormat, year);
        }

        public static string CalendarYearName(int year) {
            return Format(CalendarYearNameFormat, year);
        }

        public static string FiscalYearName(int year) {
            return Format(FiscalYearNameFormat, year);
        }

        public static string SchoolYearName(int year) {
            return Format(SchoolYearNameFormat, year);
        }

        #endregion

        #region << Halfyear >>

        public const string SystemHalfYearNameFormat = "HY{0}";
        public const string CalendarHalfYearNameFormat = "CHY{0}";
        public const string FiscalHalfYearNameFormat = "FHY{0}";
        public const string SchoolHalfYearNameFormat = "SHY{0}";

        public const string SystemHalfyearOfYearNameFormat = "HY{0} {1}";
        public const string CalendarHalfyearOfYearNameFormat = "CHY{0} {1}";
        public const string FiscalHalfyearOfYearNameFormat = "FHY{0} {1}";
        public const string SchoolHalfyearOfYearNameFormat = "SHY{0} {1}";

        public static string SystemHalfyearName(HalfyearKind halfyear) {
            return Format(SystemHalfYearNameFormat, (int)halfyear);
        }

        public static string CalendarHalfyearName(HalfyearKind halfyear) {
            return Format(CalendarHalfYearNameFormat, (int)halfyear);
        }

        public static string FiscalHalfyearName(HalfyearKind halfyear) {
            return Format(FiscalHalfYearNameFormat, (int)halfyear);
        }

        public static string SchoolHalfyearName(HalfyearKind halfyear) {
            return Format(SchoolHalfYearNameFormat, (int)halfyear);
        }

        public static string SystemHalfyearOfYearName(HalfyearKind halfyear, int year) {
            return Format(SystemHalfyearOfYearNameFormat, (int)halfyear, year);
        }

        public static string CalendarHalfyearOfYearName(HalfyearKind halfyear, int year) {
            return Format(CalendarHalfyearOfYearNameFormat, (int)halfyear, year);
        }

        public static string FiscalHalfyearOfYearName(HalfyearKind halfyear, int year) {
            return Format(FiscalHalfyearOfYearNameFormat, (int)halfyear, year);
        }

        public static string SchoolHalfyearOfYearName(HalfyearKind halfyear, int year) {
            return Format(SchoolHalfyearOfYearNameFormat, (int)halfyear, year);
        }

        #endregion

        #region << Quarter >>

        public const string SystemQuarterNameFormat = "Q{0}";
        public const string CalendarQuarterNameFormat = "CQ{0}";
        public const string FiscalQuarterNameFormat = "FQ{0}";
        public const string SchoolQuarterNameFormat = "SQ{0}";

        public const string SystemQuarterOfYearNameFormat = "Q{0} {1}";
        public const string CalendarQuarterOfYearNameFormat = "CQ{0} {1}";
        public const string FiscalQuarterOfYearNameFormat = "FQ{0} {1}";
        public const string SchoolQuarterOfYearNameFormat = "SQ{0} {1}";

        public static string SystemQuarterName(QuarterKind quarter) {
            return Format(SystemQuarterNameFormat, (int)quarter);
        }

        public static string CalendarQuarterName(QuarterKind quarter) {
            return Format(CalendarQuarterNameFormat, (int)quarter);
        }

        public static string FiscalQuarterName(QuarterKind quarter) {
            return Format(FiscalQuarterNameFormat, (int)quarter);
        }

        public static string SchoolQuarterName(QuarterKind quarter) {
            return Format(SchoolQuarterNameFormat, (int)quarter);
        }

        public static string SystemQuarterOfYearName(QuarterKind quarter, int year) {
            return Format(SystemQuarterOfYearNameFormat, (int)quarter, year);
        }

        public static string CalendarQuarterOfYearName(QuarterKind quarter, int year) {
            return Format(CalendarQuarterOfYearNameFormat, (int)quarter, year);
        }

        public static string FiscalQuarterOfYearName(QuarterKind quarter, int year) {
            return Format(FiscalQuarterOfYearNameFormat, (int)quarter, year);
        }

        public static string SchoolQuarterOfYearName(QuarterKind quarter, int year) {
            return Format(SchoolQuarterOfYearNameFormat, (int)quarter, year);
        }

        #endregion

        #region << Month >>

        public const string MonthOfYearNameFormat = "{0} {1}";

        public static string MonthOfYearName(string monthName, string yearName) {
            return Format(MonthOfYearNameFormat, monthName, yearName);
        }

        #endregion

        #region << Week >>

        public const string WeekOfYearNameFormat = "w/c {0} {1}";

        public static string WeekOfYearName(int weekOfYear, string yearName) {
            return Format(WeekOfYearNameFormat, weekOfYear, yearName);
        }

        #endregion

        #region Time Formatter

        public static string TimeSpanYears {
            get { return "Years"; }
        }

        public static string TimeSpanYear {
            get { return "Year"; }
        }

        public static string TimeSpanMonths {
            get { return "Months"; }
        }

        public static string TimeSpanMonth {
            get { return "Month"; }
        }

        public static string TimeSpanWeeks {
            get { return "Weeks"; }
        }

        public static string TimeSpanWeek {
            get { return "Week"; }
        }

        public static string TimeSpanDays {
            get { return "Days"; }
        }

        public static string TimeSpanDay {
            get { return "Day"; }
        }

        public static string TimeSpanHours {
            get { return "Hours"; }
        }

        public static string TimeSpanHour {
            get { return "Hour"; }
        }

        public static string TimeSpanMinutes {
            get { return "Minutes"; }
        }

        public static string TimeSpanMinute {
            get { return "Minute"; }
        }

        public static string TimeSpanSeconds {
            get { return "Seconds"; }
        }

        public static string TimeSpanSecond {
            get { return "Second"; }
        }

        #endregion

        internal static string Format(string format, params object[] args) {
            return string.Format(CultureInfo.InvariantCulture, format, args);
        }
    }
}