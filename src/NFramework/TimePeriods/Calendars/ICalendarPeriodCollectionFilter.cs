using System.Collections.Generic;

namespace NSoft.NFramework.TimePeriods.Calendars {
    public interface ICalendarPeriodCollectionFilter : ICalendarVisitorFilter {
        IList<MonthRangeInYear> CollectingMonths { get; }

        IList<DayRangeInMonth> CollectingDays { get; }
    }
}