using System;
using System.Collections.Generic;
using System.Linq;

namespace NSoft.NFramework.TimePeriods.TimeRanges {
    /// <summary>
    /// <see cref="MinuteRange"/>의 컬렉션을 제공합니다.
    /// </summary>
    [Serializable]
    public sealed class MinuteRangeCollection : MinuteTimeRange {
        public MinuteRangeCollection(DateTime moment, int minuteCount) : this(moment, minuteCount, new TimeCalendar()) {}
        public MinuteRangeCollection(DateTime moment, int minuteCount, ITimeCalendar calendar) : base(moment, minuteCount, calendar) {}

        public MinuteRangeCollection(int year, int month, int day, int hour, int minute, int minuteCount)
            : this(year, month, day, hour, minute, minuteCount, new TimeCalendar()) {}

        public MinuteRangeCollection(int year, int month, int day, int hour, int minute, int minuteCount, ITimeCalendar calendar)
            : base(year, month, day, hour, minute, minuteCount, calendar) {}

        /// <summary>
        /// 기간에 속한 <see cref="MinuteRange"/>를 열거합니다.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MinuteRange> GetMinutes() {
            var startMinute = Start.Date.SetTimePart(StartHour, StartMinute, 0, 0);

            return
                Enumerable
                    .Range(0, MinuteCount)
#if !SILVERLIGHT
                    .AsParallel()
                    .AsOrdered()
#endif
                    .Select(m => new MinuteRange(startMinute.AddMinutes(m), TimeCalendar));
        }

        protected override string Format(ITimeFormatter formatter) {
            var fmt = formatter ?? TimeFormatter.Instance;

            return fmt.GetCalendarPeriod(fmt.GetShortDate(Start),
                                         fmt.GetShortDate(End),
                                         fmt.GetShortTime(Start),
                                         fmt.GetShortTime(End),
                                         Duration);
        }
    }
}