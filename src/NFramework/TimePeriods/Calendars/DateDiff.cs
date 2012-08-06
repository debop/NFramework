using System;
using System.Globalization;

namespace NSoft.NFramework.TimePeriods.Calendars {
    /// <summary>
    /// 특정한 두 시각 사이의 Calendar, 예외 시각등을 고려한 기간을 계산합니다.
    /// </summary>
    /// <seealso cref="DateAdd"/>
    /// <seealso cref="CalendarDateAdd"/>
    [Serializable]
    public class DateDiff : ValueObjectBase, IEquatable<DateDiff> {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        #region << Constructors >>

        /// <summary>
        /// 현재 시각 - <paramref name="moment"/> 을 계산합니다.
        /// </summary>
        /// <param name="moment">선행시각</param>
        public DateDiff(DateTime moment) : this(moment, ClockProxy.Clock.Now, null, null, null) {}

        /// <summary>
        /// 현재 시각 - <paramref name="date1"/> 을 <paramref name="calendar"/> 기준으로 계산합니다.
        /// </summary>
        /// <param name="date1">선행 시각</param>
        /// <param name="calendar">기준 Calendar</param>
        /// <param name="firstDayOfWeek">한 주의 첫번째 요일</param>
        public DateDiff(DateTime date1, Calendar calendar, DayOfWeek firstDayOfWeek)
            : this(date1, ClockProxy.Clock.Now, calendar, firstDayOfWeek, null) {}

        /// <summary>
        /// <paramref name="date1"/>와 현재시각의 차이를 <paramref name="calendar"/> 기준으로 계산합니다.
        /// </summary>
        /// <param name="date1">선행 시각</param>
        /// <param name="calendar">기준 Calendar</param>
        /// <param name="firstDayOfWeek">한 주의 첫번째 요일</param>
        /// <param name="yearBaseMonth">한해의 시작 월</param>
        public DateDiff(DateTime date1, Calendar calendar, DayOfWeek firstDayOfWeek, int yearBaseMonth)
            : this(date1, ClockProxy.Clock.Now, calendar, firstDayOfWeek, yearBaseMonth) {}

        /// <summary>
        /// <paramref name="date2"/>-<paramref name="date1"/> 을 계산합니다.
        /// </summary>
        /// <param name="date1">선행 시각</param>
        /// <param name="date2">후행 시각</param>
        public DateDiff(DateTime date1, DateTime date2) : this(date1, date2, null, null, null) {}

        /// <summary>
        /// <paramref name="date2"/>-<paramref name="date1"/> 을 <paramref name="culture"/> 기준으로 계산합니다.
        /// </summary>
        /// <param name="date1">선행 시각</param>
        /// <param name="date2">후행 시각</param>
        /// <param name="culture">문화권</param>
        public DateDiff(DateTime date1, DateTime date2, CultureInfo culture)
            : this(date1, date2, culture.Calendar, culture.DateTimeFormat.FirstDayOfWeek, null) {}

        /// <summary>
        /// <paramref name="date2"/>-<paramref name="date1"/> 을 <paramref name="culture"/> 기준으로 계산합니다.
        /// </summary>
        /// <param name="date1">선행 시각</param>
        /// <param name="date2">후행 시각</param>
        /// <param name="culture">문화권</param>
        /// <param name="yearBaseMonth">한해의 시작 월</param>
        public DateDiff(DateTime date1, DateTime date2, CultureInfo culture, int yearBaseMonth)
            : this(date1, date2, culture.Calendar, culture.DateTimeFormat.FirstDayOfWeek, yearBaseMonth) {}

        /// <summary>
        ///  <paramref name="date2"/>-<paramref name="date1"/> 을 <paramref name="calendar"/> 기준으로 계산합니다.
        /// </summary>
        /// <param name="date1">선행 시각</param>
        /// <param name="date2">후행 시각</param>
        /// <param name="calendar">기준 Calendar</param>
        /// <param name="firstDayOfWeek">한 주의 첫번째 요일</param>
        public DateDiff(DateTime date1, DateTime date2, Calendar calendar, DayOfWeek firstDayOfWeek)
            : this(date1, date2, calendar, firstDayOfWeek, null) {}

        /// <summary>
        ///  <paramref name="date2"/>-<paramref name="date1"/> 을 <paramref name="calendar"/> 기준으로 계산합니다.
        /// </summary>
        /// <param name="date1">선행 시각</param>
        /// <param name="date2">후행 시각</param>
        /// <param name="calendar">기준 Calendar</param>
        /// <param name="firstDayOfWeek">한 주의 첫번째 요일</param>
        /// <param name="yearBaseMonth">한해의 시작 월</param>
        public DateDiff(DateTime date1, DateTime date2, Calendar calendar, DayOfWeek? firstDayOfWeek, int? yearBaseMonth) {
            Calendar = calendar ?? CultureInfo.CurrentCulture.Calendar;
            YearBaseMonth = yearBaseMonth ?? TimeSpec.CalendarYearStartMonth;
            FirstDayOfWeek = firstDayOfWeek ?? DayOfWeek.Sunday;

            Date1 = date1;
            Date2 = date2;
            Difference = date2.Subtract(date1);

            if(IsDebugEnabled)
                log.Debug("두 시각의 차이를 구합니다... date1=[{0}], date2=[{1}], Difference=[{2}", date1, date2, Difference);

            InitializeLazyVariables();
        }

        #endregion

        private void InitializeLazyVariables() {
            _years = new Lazy<int>(() => CalcYears(), true);
            _quarters = new Lazy<int>(() => CalcQuarters(), true);
            _months = new Lazy<int>(() => CalcMonths(), true);
            _weeks = new Lazy<int>(() => CalcWeeks(), true);

            _elapsedYears = new Lazy<int>(() => Years, true);
            _elapsedQuarters = new Lazy<int>(() => Quarters, true);
            _elapsedMonths = new Lazy<int>(() => Months - (ElapsedYears * TimeSpec.MonthsPerYear), true);
            _elapsedDays = new Lazy<int>(() => (int)Date2.Subtract(Date1.AddYears(ElapsedYears).AddMonths(ElapsedMonths)).TotalDays,
                                         true);
            _elapsedHours =
                new Lazy<int>(
                    () => (int)Date2.Subtract(Date1.AddYears(ElapsedYears).AddMonths(ElapsedMonths).AddDays(ElapsedDays)).TotalHours,
                    true);
            _elapsedMinutes =
                new Lazy<int>(
                    () =>
                    (int)
                    Date2.Subtract(Date1.AddYears(ElapsedYears).AddMonths(ElapsedMonths).AddDays(ElapsedDays).AddHours(ElapsedHours)).
                        TotalMinutes, true);
            _elapsedSeconds =
                new Lazy<int>(
                    () =>
                    (int)
                    Date2.Subtract(
                        Date1.AddYears(ElapsedYears).AddMonths(ElapsedMonths).AddDays(ElapsedDays).AddHours(ElapsedHours).AddMinutes(
                            ElapsedMinutes)).TotalSeconds, true);
        }

        private Lazy<int> _years,
                          _quarters,
                          _months,
                          _weeks;

        private Lazy<int> _elapsedYears,
                          _elapsedQuarters,
                          _elapsedMonths,
                          _elapsedDays,
                          _elapsedHours,
                          _elapsedMinutes,
                          _elapsedSeconds;

        /// <summary>
        /// 기준 Calendar
        /// </summary>
        public Calendar Calendar { get; private set; }

        /// <summary>
        /// 한 해의 첫번째 월 (보통 1월)
        /// </summary>
        public int YearBaseMonth { get; private set; }

        /// <summary>
        /// 한 주의 첫번째 요일 (한국, 미국은 일요일, ISO8601은 월요일)
        /// </summary>
        public DayOfWeek FirstDayOfWeek { get; private set; }

        public DateTime Date1 { get; private set; }

        public DateTime Date2 { get; private set; }

        /// <summary>
        /// <see cref="Date2"/>-<see cref="Date1"/> 을 계산한 값
        /// </summary>
        public TimeSpan Difference { get; protected set; }

        /// <summary>
        /// 시간 차가 없는지 여부?
        /// </summary>
        public bool IsEmpty {
            get { return Difference == TimeSpan.Zero; }
        }

        /// <summary>
        /// 두 시각의 년 단위의 차이
        /// </summary>
        public int Years {
            get { return _years.Value; }
        }

        /// <summary>
        /// 두 시각의 분기 단위의 차이
        /// </summary>
        public int Quarters {
            get { return _quarters.Value; }
        }

        /// <summary>
        /// 두 시각의 월 단위의 차이
        /// </summary>
        public int Months {
            get { return _months.Value; }
        }

        /// <summary>
        /// 두 시각의 주 단위의 차이
        /// </summary>
        public int Weeks {
            get { return _weeks.Value; }
        }

        /// <summary>
        /// 두 시각의 일 단위의 차이
        /// </summary>
        public int Days {
            get { return (int)Math.Round(RoundEx((Difference.TotalDays))); }
        }

        /// <summary>
        /// 두 시각의 요일 단위의 차이
        /// </summary>
        public int WeekDays {
            get { return ((int)Math.Round(RoundEx(Difference.TotalDays)) / TimeSpec.DaysPerWeek); }
        }

        /// <summary>
        /// 두 시각의 시간 단위의 차이
        /// </summary>
        public int Hours {
            get { return (int)Math.Round(RoundEx(Difference.TotalHours)); }
        }

        /// <summary>
        /// 두 시각의 분 단위의 차이
        /// </summary>
        public int Minutes {
            get { return (int)Math.Round(RoundEx(Difference.TotalMinutes)); }
        }

        /// <summary>
        /// 두 시각의 초 단위의 차이
        /// </summary>
        public int Seconds {
            get { return (int)Math.Round(RoundEx(Difference.TotalSeconds)); }
        }

        /// <summary>
        /// 두 시각의 시간 간격의 총 년 수
        /// </summary>
        public int ElapsedYears {
            get { return _elapsedYears.Value; }
        }

        /// <summary>
        /// 두 시각의 시간 간격의 총 분기 수
        /// </summary>
        public int ElapsedQuarter {
            get { return _elapsedQuarters.Value; }
        }

        /// <summary>
        /// 두 시각의 시간 간격의 총 월 수
        /// </summary>
        public int ElapsedMonths {
            get { return _elapsedMonths.Value; }
        }

        /// <summary>
        /// 두 시각의 시간 간격의 총 일 수
        /// </summary>
        public int ElapsedDays {
            get { return _elapsedDays.Value; }
        }

        /// <summary>
        /// 두 시각의 시간 간격의 총 시간 수
        /// </summary>
        public int ElapsedHours {
            get { return _elapsedHours.Value; }
        }

        /// <summary>
        /// 두 시각의 시간 간격의 총 분수
        /// </summary>
        public int ElapsedMinutes {
            get { return _elapsedMinutes.Value; }
        }

        /// <summary>
        /// 두 시각의 시간 간격의 총 초수
        /// </summary>
        public int ElapsedSeconds {
            get { return _elapsedSeconds.Value; }
        }

        private int Year1 {
            get { return Calendar.GetYear(Date1); }
        }

        private int Year2 {
            get { return Calendar.GetYear(Date2); }
        }

        private int Month1 {
            get { return Calendar.GetMonth(Date1); }
        }

        private int Month2 {
            get { return Calendar.GetMonth(Date2); }
        }

        private int CalcYears() {
            if(IsDebugEnabled)
                log.Debug("CalcYears...");

            if(Equals(Date1, Date2))
                return 0;

            var compareDay = Math.Min(Date2.Day, Calendar.GetDaysInMonth(Year1, Month2));
            var compareDate = new DateTime(Year1, Month2, compareDay).Add(Date2.TimeOfDay);

            if(Date2 > Date1) {
                if(compareDate < Date1)
                    compareDate = compareDate.AddYears(1);
            }
            else {
                if(compareDate > Date1)
                    compareDate = compareDate.AddYears(-1);
            }
            return Year2 - Calendar.GetYear(compareDate);
        }

        private int CalcQuarters() {
            if(IsDebugEnabled)
                log.Debug("Calc Quarters...");

            if(Equals(Date1, Date2))
                return 0;

            var year1 = TimeTool.GetYearOf(YearBaseMonth, Year1, Month1);
            var quarter1 = TimeTool.GetQuarterOfMonth(YearBaseMonth, Month1);

            var year2 = TimeTool.GetYearOf(YearBaseMonth, Year2, Month2);
            var quarter2 = TimeTool.GetQuarterOfMonth(YearBaseMonth, Month2);

            return (year2 * TimeSpec.QuartersPerYear + quarter2) -
                   (year1 * TimeSpec.QuartersPerYear + quarter1);
        }

        private int CalcMonths() {
            if(IsDebugEnabled)
                log.Debug("Calc Months...");

            if(Equals(Date1, Date2))
                return 0;

            var compareDay = Math.Min(Date2.Day, Calendar.GetDaysInMonth(Year1, Month1));
            var compareDate = new DateTime(Year1, Month1, compareDay).Add(Date2.TimeOfDay);

            if(Date2 > Date1) {
                if(compareDate < Date1)
                    compareDate = compareDate.AddMonths(1);
            }
            else {
                if(compareDate > Date1)
                    compareDate = compareDate.AddMonths(-1);
            }

            return (Year2 * TimeSpec.MonthsPerYear + Month2) -
                   (Calendar.GetYear(compareDate) * TimeSpec.MonthsPerYear + Calendar.GetMonth(compareDate));
        }

        private int CalcWeeks() {
            if(IsDebugEnabled)
                log.Debug("Calc Weeks...");

            if(Equals(Date1, Date2))
                return 0;

            var week1 = TimeTool.GetStartOfWeek(Date1, FirstDayOfWeek);
            var week2 = TimeTool.GetStartOfWeek(Date2, FirstDayOfWeek);

            if(Equals(week1, week2))
                return 0;

            return (int)(week2.Subtract(week1).TotalDays / TimeSpec.DaysPerWeek);
        }

        private static double RoundEx(double number = 0.0d) {
            if(number >= 0.0d)
                return Math.Round(number);

            return -Math.Round(-number);
        }

        public string GetDiscription(int? precision = null, ITimeFormatter formatter = null) {
            precision = precision ?? int.MaxValue;
            formatter = formatter ?? TimeFormatter.Instance;

            precision.Value.ShouldBePositive("precision");

            var elapsedTimes = new[]
                               {
                                   ElapsedYears,
                                   ElapsedMonths,
                                   ElapsedDays,
                                   ElapsedHours,
                                   ElapsedMinutes,
                                   ElapsedSeconds
                               };

            if(precision <= elapsedTimes.Length - 1) {
                for(var i = precision.Value; i < elapsedTimes.Length; i++)
                    elapsedTimes[i] = 0;
            }

            return formatter.GetDuration(elapsedTimes[0],
                                         elapsedTimes[1],
                                         elapsedTimes[2],
                                         elapsedTimes[3],
                                         elapsedTimes[4],
                                         elapsedTimes[5]);
        }

        public bool Equals(DateDiff other) {
            return (other != null) && GetHashCode().Equals(other.GetHashCode());
        }

        public override int GetHashCode() {
            return HashTool.Compute(Calendar,
                                    YearBaseMonth,
                                    FirstDayOfWeek,
                                    Date1,
                                    Date2,
                                    Difference);
        }

        public override string ToString() {
            return GetDiscription();
        }
    }
}