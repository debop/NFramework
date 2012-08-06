using System;
using System.Globalization;
using System.Text;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.TimePeriods {
    /// <summary>
    /// <see cref="ITimePeriod"/>를 문자열로 표현하기 위한 Formatter입니다.
    /// </summary>
    [Serializable]
    public class TimeFormatter : ITimeFormatter {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// 객체의 표현식의 구분자(;)
        /// </summary>
        public const string DefaultContextSeparator = @"; ";

        /// <summary>
        /// 기간의 시작시각과 완료시각의 분리자(~)
        /// </summary>
        public const string DefaultStartEndSeparator = @" ~ ";

        /// <summary>
        /// 기간(Duration)의 구분자 (|)
        /// </summary>
        public const string DefaultDurationSeparator = @" | ";

        private static TimeFormatter _instance = new TimeFormatter();
        private static readonly object _syncLock = new object();

        #region << Constructors >>

        public TimeFormatter() : this(CultureInfo.CurrentCulture) {}

        public TimeFormatter(CultureInfo culture,
                             string dateTimeFormat = null,
                             string shortDateFormat = null,
                             string longTimeFormat = null,
                             string shortTimeFormat = null,
                             string contextSeparator = DefaultContextSeparator,
                             string startEndSeparator = DefaultStartEndSeparator,
                             string durationSeparator = DefaultDurationSeparator,
                             DurationFormatKind durationFormatKind = DurationFormatKind.Compact,
                             bool useDurationSeconds = false,
                             bool useIsoIntervalNotation = false) {
            Culture = culture.GetOrCurrentCulture();

            DateTimeFormat = dateTimeFormat;
            ShortDateFormat = shortDateFormat;
            LongTimeFormat = longTimeFormat;
            ShortTimeFormat = shortTimeFormat;

            ContextSeparator = contextSeparator ?? DefaultContextSeparator;
            StartEndSeparator = startEndSeparator ?? DefaultStartEndSeparator;
            DurationSeparator = durationSeparator ?? DefaultDurationSeparator;

            DurationKind = durationFormatKind;
            UseDurationSeconds = useDurationSeconds;
            UseIsoIntervalNotation = useIsoIntervalNotation;
        }

        #endregion

        /// <summary>
        /// Singleton Instance
        /// </summary>
        public static TimeFormatter Instance {
            get {
                if(_instance == null)
                    lock(_syncLock)
                        if(_instance == null) {
                            var instance = new TimeFormatter();
                            System.Threading.Thread.MemoryBarrier();
                            _instance = instance;
                        }

                return _instance;
            }
            set {
                value.ShouldNotBeNull("Instance");
                lock(_syncLock)
                    _instance = value;
            }
        }

        /// <summary>
        /// 문화권
        /// </summary>
        public CultureInfo Culture { get; private set; }

        /// <summary>
        /// 목록 구분자
        /// </summary>
        public string ListSeparator {
            get { return Culture.TextInfo.ListSeparator; }
        }

        /// <summary>
        /// 요소 구분자
        /// </summary>
        public string ContextSeparator { get; private set; }

        /// <summary>
        /// 기간의 시작과 완료 시각의 구분자
        /// </summary>
        public string StartEndSeparator { get; private set; }

        /// <summary>
        /// 기간(Duration) 구분자
        /// </summary>
        public string DurationSeparator { get; private set; }

        /// <summary>
        /// 시각 포맷
        /// </summary>
        public string DateTimeFormat { get; private set; }

        /// <summary>
        /// 단순 날짜 포맷
        /// </summary>
        public string ShortDateFormat { get; private set; }

        /// <summary>
        /// 긴 시간 포맷
        /// </summary>
        public string LongTimeFormat { get; private set; }

        /// <summary>
        /// 짧은 시간 포맷
        /// </summary>
        public string ShortTimeFormat { get; private set; }

        /// <summary>
        /// 기간을 문자열로 표현하는 방식
        /// </summary>
        public DurationFormatKind DurationKind { get; private set; }

        /// <summary>
        /// 기간을 초단위로 표시할 것인가?
        /// </summary>
        public bool UseDurationSeconds { get; private set; }

        /// <summary>
        /// 구간에 대한 경계 포함 여부에 대한 표기법을 ISO 표준 표기법을 사용할 것인가?
        /// </summary>
        public bool UseIsoIntervalNotation { get; private set; }

        /// <summary>
        /// 컬렉션의 index를 문자열로 표현합니다.
        /// </summary>
        public string GetCollection(int count) {
            return @"Count = " + count;
        }

        /// <summary>
        /// 컬렉션의 기간 정보를 문자열로 표현합니다.
        /// </summary>
        public string GetCollectionPeriod(int count, DateTime start, DateTime end, TimeSpan duration) {
            return string.Format("{0}{1} {2}", GetCollection(count), ListSeparator, GetPeriod(start, end, duration));
        }

        /// <summary>
        /// 지정된 DateTime을 문자열로 표현합니다.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public string GetDateTime(DateTime dateTime) {
            return DateTimeFormat.IsNotWhiteSpace()
                       ? dateTime.ToString(DateTimeFormat)
                       : dateTime.ToString(Culture.DateTimeFormat);
        }

        /// <summary>
        /// 지정된 DateTime을 Short Date 형식으로 문자열을 만듭니다.
        /// </summary>
        public string GetShortDate(DateTime dateTime) {
            return ShortDateFormat.IsNotWhiteSpace()
                       ? dateTime.ToString(ShortDateFormat)
                       : dateTime.ToShortDateString();
        }

        /// <summary>
        /// <paramref name="dateTime"/>을 Long Time 형식의 문자열로 표현합니다.
        /// </summary>
        public string GetLongTime(DateTime dateTime) {
            return LongTimeFormat.IsNotWhiteSpace()
                       ? dateTime.ToString(LongTimeFormat)
                       : dateTime.ToLongTimeString();
        }

        /// <summary>
        /// <paramref name="dateTime"/>을 Short Time 형식의 문자열로 표현합니다.
        /// </summary>
        public string GetShortTime(DateTime dateTime) {
            return ShortTimeFormat.IsNotWhiteSpace()
                       ? dateTime.ToString(ShortTimeFormat)
                       : dateTime.ToShortTimeString();
        }

        /// <summary>
        /// <paramref name="timeSpan"/>을 문자열로 표현합니다.
        /// </summary>
        public string GetDuration(TimeSpan timeSpan) {
            return GetDuration(timeSpan, DurationKind);
        }

        /// <summary>
        /// <paramref name="timeSpan"/>을 문자열로 표현합니다.
        /// </summary>
        public string GetDuration(TimeSpan timeSpan, DurationFormatKind? durationFormatKind) {
            switch(durationFormatKind.GetValueOrDefault(DurationFormatKind.Compact)) {
                case DurationFormatKind.Detailed:
                    var days = timeSpan.TotalDays.AsInt(0);
                    var hours = timeSpan.Hours;
                    var minutes = timeSpan.Minutes;
                    var seconds = UseDurationSeconds ? timeSpan.Seconds : 0;
                    return GetDuration(0, 0, days, hours, minutes, seconds);

                case DurationFormatKind.Compact:
                default:
                    return UseDurationSeconds
                               ? string.Format("{0}.{1:00}:{2:00}:{3:00}", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes,
                                               timeSpan.Seconds)
                               : string.Format("{0}.{1:00}:{2:00}", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes);
            }
        }

        /// <summary>
        /// 지정된 기간을 문자열로 표현합니다.
        /// </summary>
        public string GetDuration(int years, int months, int days, int hours, int minutes, int seconds) {
            var builder = new StringBuilder(200);

            if(years != 0) {
                builder.Append(years).Append(StringTool.WhiteSpace).Append(years == 1
                                                                               ? TimeStrings.TimeSpanYear
                                                                               : TimeStrings.TimeSpanYears);
            }

            if(months != 0) {
                if(builder.Length > 0)
                    builder.Append(StringTool.WhiteSpace);

                builder.Append(months).Append(StringTool.WhiteSpace).Append(months == 1
                                                                                ? TimeStrings.TimeSpanMonth
                                                                                : TimeStrings.TimeSpanMonths);
            }
            if(days != 0) {
                if(builder.Length > 0)
                    builder.Append(StringTool.WhiteSpace);

                builder.Append(days).Append(StringTool.WhiteSpace).Append(days == 1 ? TimeStrings.TimeSpanDay : TimeStrings.TimeSpanDays);
            }
            if(hours != 0) {
                if(builder.Length > 0)
                    builder.Append(StringTool.WhiteSpace);

                builder.Append(hours).Append(StringTool.WhiteSpace).Append(hours == 1
                                                                               ? TimeStrings.TimeSpanHour
                                                                               : TimeStrings.TimeSpanHours);
            }
            if(minutes != 0) {
                if(builder.Length > 0)
                    builder.Append(StringTool.WhiteSpace);

                builder.Append(minutes).Append(StringTool.WhiteSpace).Append(minutes == 1
                                                                                 ? TimeStrings.TimeSpanMinute
                                                                                 : TimeStrings.TimeSpanMinutes);
            }
            if(seconds != 0) {
                if(builder.Length > 0)
                    builder.Append(StringTool.WhiteSpace);

                builder.Append(seconds).Append(StringTool.WhiteSpace).Append(seconds == 1
                                                                                 ? TimeStrings.TimeSpanSecond
                                                                                 : TimeStrings.TimeSpanSeconds);
            }

            return builder.ToString();
        }

        /// <summary>
        /// 시작-완료 시각을 문자열로 표현합니다.
        /// </summary>
        public string GetPeriod(DateTime start, DateTime end) {
            return GetPeriod(start, end, end - start);
        }

        /// <summary>
        /// 시간 간격을 문자열로 표현합니다.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="startEdge"></param>
        /// <param name="endEdge"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public string GetInterval(DateTime start, DateTime end, IntervalEdge startEdge, IntervalEdge endEdge, TimeSpan duration) {
            if(end < start) {
                throw new ArgumentOutOfRangeException("end");
            }

            StringBuilder sb = new StringBuilder();

            // interval start
            switch(startEdge) {
                case IntervalEdge.Closed:
                    sb.Append("[");
                    break;
                case IntervalEdge.Open:
                    sb.Append(UseIsoIntervalNotation ? "]" : "(");
                    break;
            }

            bool addDuration = true;
            bool startHasTimeOfDay = TimeTool.HasTimePart(start);

            // no duration - schow start date (optionally with the time part)
            if(duration == TimeSpec.MinPeriodDuration) {
                sb.Append(startHasTimeOfDay ? GetDateTime(start) : GetShortDate(start));
                addDuration = false;
            }
                // within one day: show full start, end time and suration
            else if(TimeTool.IsSameDay(start, end)) {
                sb.Append(GetDateTime(start));
                sb.Append(StartEndSeparator);
                sb.Append(GetLongTime(end));
                sb.Append(GetShortDate(start));
            }
            else {
                bool endHasTimeOfDay = TimeTool.HasTimePart(start);
                bool hasTimeOfDays = startHasTimeOfDay || endHasTimeOfDay;
                if(hasTimeOfDays) {
                    sb.Append(GetDateTime(start));
                    sb.Append(StartEndSeparator);
                    sb.Append(GetDateTime(start));
                }
                else {
                    sb.Append(GetShortDate(start));
                    sb.Append(StartEndSeparator);
                    sb.Append(GetShortDate(end));
                }
            }

            // interval end
            switch(endEdge) {
                case IntervalEdge.Closed:
                    sb.Append("]");
                    break;
                case IntervalEdge.Open:
                    sb.Append(UseIsoIntervalNotation ? "[" : ")");
                    break;
            }

            // duration
            if(addDuration) {
                sb.Append(DurationSeparator);
                sb.Append(GetDuration(duration));
            }

            return sb.ToString();
        }

        /// <summary>
        /// 시작-완료 | 기간 을 문자열로 표현합니다.
        /// </summary>
        public string GetPeriod(DateTime start, DateTime end, TimeSpan duration) {
            TimeTool.AssertValidPeriod(start, end);

            // no duration - start equals end
            if(duration == TimeSpec.MinPeriodDuration) {
                return start.HasTimePart() ? GetDateTime(start) : GetShortDate(start);
            }

            // 날짜까지 같으니까, 중복된 부분을 뺀 완료 날짜를 제외한 시간만 표시한다.
            //
            if(TimeTool.IsSameDay(start, end)) {
                return string.Concat(GetDateTime(start), StartEndSeparator, GetLongTime(end), DurationSeparator, GetDuration(duration));
            }

            // 시작 일자, 완료 일자, 기간(Duration)을 표현한다.
            //
            var hasTimeOfDays = start.HasTimePart() || end.HasTimePart();
            var startPart = hasTimeOfDays ? GetDateTime(start) : GetShortDate(start);
            var endPart = hasTimeOfDays ? GetDateTime(end) : GetShortDate(end);

            return string.Concat(startPart, StartEndSeparator, endPart, DurationSeparator, GetDuration(duration));
        }

        /// <summary>
        /// <see cref="Calendar"/>기준으로 시작-완료 | 기간을 문자열로 표현합니다.
        /// </summary>
        public string GetCalendarPeriod(string start, string end, TimeSpan duration) {
            return GetCalendarPeriod(string.Empty, string.Empty, start, end, duration);
        }

        /// <summary>
        /// <see cref="Calendar"/>기준으로 시작-완료 | 기간을 문자열로 표현합니다.
        /// </summary>
        public string GetCalendarPeriod(string context, string start, string end, TimeSpan duration) {
            return GetCalendarPeriod(context, context, start, end, duration);
        }

        /// <summary>
        /// <see cref="Calendar"/>기준으로 시작-완료 | 기간을 문자열로 표현합니다.
        /// </summary>
        public string GetCalendarPeriod(string startContext, string endContext, string start, string end, TimeSpan duration) {
            var contextPeriod = Equals(startContext, endContext)
                                    ? startContext
                                    : string.Concat(startContext, StartEndSeparator, endContext);
            var timePeriod = Equals(start, end) ? start : string.Concat(start, StartEndSeparator, end);

            return (contextPeriod.IsWhiteSpace())
                       ? string.Concat(timePeriod, DurationSeparator, GetDuration(duration))
                       : string.Concat(contextPeriod, ContextSeparator, timePeriod, DurationSeparator, GetDuration(duration));
        }
    }
}