using System.Text;
using NSoft.NFramework.Reflections;

namespace NSoft.NFramework.TimePeriods.Calendars {
    /// <summary>
    /// Calendar 관련 Extensions
    /// </summary>
    public static class CalendarExtensions {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// <paramref name="filter"/> 정보를 문자열로 표현합니다.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static string AsString(this ICalendarVisitorFilter filter) {
            if(filter == null)
                return @"Filter is NULL";

            var builder = new StringBuilder();

            builder.AppendLine("CalendarVisitorFilter#");
            builder.AppendLine("---------------------------------");
            builder.AppendLine("\tYears=" + filter.Years.CollectionToString());
            builder.AppendLine("\tMonths=" + filter.Months.CollectionToString());
            builder.AppendLine("\tDays=" + filter.Days.CollectionToString());
            builder.AppendLine("\tWeekDays=" + filter.WeekDays.CollectionToString());
            builder.AppendLine("\tHours=" + filter.Hours.CollectionToString());
            builder.AppendLine("\tMinutes=" + filter.Minutes.CollectionToString());
            builder.AppendLine("\tExclude Periods=" + filter.ExcludePeriods.CollectionToString());
            builder.AppendLine("---------------------------------");

            return builder.ToString();
        }
    }
}