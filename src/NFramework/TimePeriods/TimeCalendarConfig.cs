using System;
using System.Globalization;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.TimePeriods {
    /// <summary>
    /// <see cref="TimeCalendar"/>의 설정 정보를 표현합니다.
    /// </summary>
    [Serializable]
    public struct TimeCalendarConfig : IEquatable<TimeCalendarConfig> {
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="culture">문화권</param>
        /// <param name="weekOfYearRule">주차 산정 룰</param>
        public TimeCalendarConfig(CultureInfo culture, WeekOfYearRuleKind weekOfYearRule = WeekOfYearRuleKind.Calendar)
            : this() {
            Culture = culture.GetOrCurrentCulture();
            WeekOfYearRule = weekOfYearRule;
        }

        /// <summary>
        /// 날짜를 표현하고, 산정하는데 기준이 되는 문화권 (NULL인 경우에는 Current Culture를 사용합니다)
        /// </summary>
        public CultureInfo Culture { get; set; }

        /// <summary>
        /// 년도의 종류 (시스템|회계|교육)
        /// </summary>
        public YearKind? YearKind { get; set; }

        /// <summary>
        /// Start Time에 대한 Offset (Offset 값에 따라 경계의 포함 여부를 결정짓는다.)
        /// </summary>
        public TimeSpan? StartOffset { get; set; }

        /// <summary>
        /// End Time에 대한 Offset (Offset 값에 따라 경계의 포함 여부를 결정짓는다.)
        /// </summary>
        public TimeSpan? EndOffset { get; set; }

        /// <summary>
        /// 한해의 시작 월 (기본 1월)
        /// </summary>
        public int? YearBaseMonth { get; set; }

        /// <summary>
        /// 주차 계산을 위한 룰 (Culture 기반| ISO8601 기반)
        /// </summary>
        public WeekOfYearRuleKind? WeekOfYearRule { get; set; }

        /// <summary>
        /// 주차 계산을 위한 룰
        /// </summary>
        public CalendarWeekRule CalendarWeekRule {
            get { return WeekTool.GetCalendarWeekRule(Culture, WeekOfYearRule); }
        }

        /// <summary>
        /// 주의 첫번째 요일 (예: 일요일|월요일)
        /// </summary>
        public DayOfWeek FirstDayOfWeek {
            get { return WeekTool.GetFirstDayOfWeek(Culture, WeekOfYearRule); }
        }

        public bool Equals(TimeCalendarConfig other) {
            return GetHashCode().Equals(other.GetHashCode());
        }

        public override bool Equals(object obj) {
            return (obj != null) && (obj is TimeCalendarConfig) && Equals((TimeCalendarConfig)obj);
        }

        public override int GetHashCode() {
            return HashTool.Compute(Culture,
                                    WeekOfYearRule,
                                    StartOffset,
                                    EndOffset);
        }

        public override string ToString() {
            return string.Format("TimeCalendarConfig# Culture=[{0}], WeekOfYearRule=[{1}], StartOffset=[{2}], EndOffset=[{3}]",
                                 Culture, WeekOfYearRule, StartOffset, EndOffset);
        }
    }
}