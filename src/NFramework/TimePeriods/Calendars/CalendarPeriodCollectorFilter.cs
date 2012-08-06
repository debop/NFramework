using System;
using System.Collections.Generic;

namespace NSoft.NFramework.TimePeriods.Calendars {
    [Serializable]
    public class CalendarPeriodCollectorFilter : CalendarVisitorFilter, ICalendarPeriodCollectionFilter {
        private readonly object _syncLock = new object();

        private readonly IList<MonthRangeInYear> _collectiongMonths = new List<MonthRangeInYear>();
        private readonly IList<DayRangeInMonth> _collectingDays = new List<DayRangeInMonth>();
        private readonly IList<HourRangeInDay> _collectingHours = new List<HourRangeInDay>();
        private readonly IList<DayHourRange> _collectingDayHours = new List<DayHourRange>();

        // private readonly IList<MinuteRangeInHour> _collectingMinutes = new List<MinuteRangeInHour>();

        public IList<MonthRangeInYear> CollectingMonths {
            get { return _collectiongMonths; }
        }

        public IList<DayRangeInMonth> CollectingDays {
            get { return _collectingDays; }
        }

        public IList<HourRangeInDay> CollectingHours {
            get { return _collectingHours; }
        }

        public IList<DayHourRange> CollectingDayHours {
            get { return _collectingDayHours; }
        }

        // public IList<MinuteRangeInHour> CollectingMinutes { get { return _collectingMinutes; } }

        public override void Clear() {
            lock(_syncLock) {
                base.Clear();
                _collectiongMonths.Clear();
                _collectingDays.Clear();
                _collectingHours.Clear();
                _collectingDayHours.Clear();
                // _collectingMinutes.Clear();
            }
        }
    }
}