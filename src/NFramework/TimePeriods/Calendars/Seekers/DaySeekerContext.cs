using System;
using NSoft.NFramework.TimePeriods.TimeRanges;

namespace NSoft.NFramework.TimePeriods.Calendars.Seekers {
    [Serializable]
    public sealed class DaySeekerContext : ValueObjectBase, ICalendarVisitorContext {
        public DaySeekerContext(DayRange startDay, int dayCount) {
            startDay.ShouldNotBeNull("startDay");

            StartDay = startDay;
            DayCount = Math.Abs(dayCount);
            RemaingDays = DayCount;
        }

        public int DayCount { get; private set; }

        public int RemaingDays { get; private set; }

        public DayRange StartDay { get; private set; }

        public DayRange FoundDay { get; private set; }

        public bool IsFinished {
            get { return RemaingDays == 0; }
        }

        public void ProcessDay(DayRange day) {
            if(IsFinished)
                return;

            RemaingDays -= 1;

            if(IsFinished)
                FoundDay = day;
        }

        public override int GetHashCode() {
            return HashTool.Compute(StartDay, DayCount);
        }
    }
}