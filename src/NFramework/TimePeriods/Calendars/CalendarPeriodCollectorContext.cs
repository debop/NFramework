using System;

namespace NSoft.NFramework.TimePeriods.Calendars {
    [Serializable]
    public class CalendarPeriodCollectorContext : ICalendarVisitorContext {
        public enum CollectKind {
            Year,
            Month,
            Day,
            Hour,
            Minute
        }

        public CollectKind Scope { get; set; }
    }
}