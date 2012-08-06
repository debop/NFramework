using System;
using System.Collections.Generic;
using System.Linq;

namespace NSoft.NFramework.TimePeriods.TimeRanges {
    /// <summary>
    /// 주(Week) 단위의 컬렉션입니다.
    /// </summary>
    [Serializable]
    public sealed class WeekRangeCollection : WeekTimeRange {
        public WeekRangeCollection(DateTime moment, int weekCount) : this(moment, weekCount, new TimeCalendar()) {}
        public WeekRangeCollection(DateTime moment, int weekCount, ITimeCalendar timeCalendar) : base(moment, weekCount, timeCalendar) {}

        public WeekRangeCollection(int year, int startWeek, int weekCount) : this(year, startWeek, weekCount, new TimeCalendar()) {}

        public WeekRangeCollection(int year, int startWeek, int weekCount, ITimeCalendar timeCalendar)
            : base(year, startWeek, weekCount, timeCalendar) {}

        /// <summary>
        /// 기간 중의 모든 <see cref="WeekRange"/>를 열거합니다.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<WeekRange> GetWeeks() {
            var startTime = Start;

            return
                Enumerable
                    .Range(0, WeekCount)
#if !SILVERLIGHT
                    .AsParallel()
                    .AsOrdered()
#endif
                    .Select(w => new WeekRange(startTime.AddDays(w * TimeSpec.DaysPerWeek), TimeCalendar));
        }
    }
}