using System;
using System.Collections.Generic;

namespace NSoft.NFramework.TimePeriods {
    /// <summary>
    /// TimeRange 간의 관계를 테스트하기 위한 클래스입니다.
    /// </summary>
    [Serializable]
    public class TimeRangePeriodRelationTestData {
        private readonly List<ITimePeriod> _allPeriods = new List<ITimePeriod>();

        public TimeRangePeriodRelationTestData(DateTime start, DateTime end, TimeSpan offset) {
            Guard.Assert<ArgumentOutOfRangeException>(offset >= TimeSpan.Zero, "offset은 0 이상의 기간을 가져야 합니다.");

            Reference = new TimeRange(start, end, true);

            var beforeEnd = start.Subtract(offset);
            var beforeStart = beforeEnd.Subtract(Reference.Duration);
            var insideStart = start.Add(offset);
            var insideEnd = end.Subtract(offset);
            var afterStart = end.Add(offset);
            var afterEnd = afterStart.Add(Reference.Duration);

            After = new TimeRange(beforeStart, beforeEnd, true);
            StartTouching = new TimeRange(beforeStart, start, true);
            StartInside = new TimeRange(beforeStart, insideStart, true);
            InsideStartTouching = new TimeRange(start, afterStart, true);
            EnclosingStartTouching = new TimeRange(start, insideEnd, true);
            Enclosing = new TimeRange(insideStart, insideEnd, true);
            EnclosingEndTouching = new TimeRange(insideStart, end, true);
            ExactMatch = new TimeRange(start, end, true);
            Inside = new TimeRange(beforeStart, afterEnd, true);
            InsideEndTouching = new TimeRange(beforeStart, end, true);
            EndInside = new TimeRange(insideEnd, afterEnd, true);
            EndTouching = new TimeRange(end, afterEnd, true);
            Before = new TimeRange(afterStart, afterEnd, true);

            _allPeriods.Add(Reference);
            _allPeriods.Add(After);
            _allPeriods.Add(StartTouching);
            _allPeriods.Add(StartInside);
            _allPeriods.Add(InsideStartTouching);
            _allPeriods.Add(EnclosingStartTouching);
            _allPeriods.Add(Enclosing);
            _allPeriods.Add(EnclosingEndTouching);
            _allPeriods.Add(ExactMatch);
            _allPeriods.Add(Inside);
            _allPeriods.Add(InsideEndTouching);
            _allPeriods.Add(EndInside);
            _allPeriods.Add(EndTouching);
            _allPeriods.Add(Before);
        }

        public ICollection<ITimePeriod> AllPeriods {
            get { return _allPeriods; }
        }

        public ITimeRange Reference { get; private set; }

        public ITimeRange Before { get; private set; }

        public ITimeRange StartTouching { get; private set; }

        public ITimeRange StartInside { get; private set; }

        public ITimeRange InsideStartTouching { get; private set; }

        public ITimeRange EnclosingStartTouching { get; private set; }

        public ITimeRange Inside { get; private set; }

        public ITimeRange EnclosingEndTouching { get; private set; }

        public ITimeRange ExactMatch { get; private set; }

        public ITimeRange Enclosing { get; private set; }

        public ITimeRange InsideEndTouching { get; private set; }

        public ITimeRange EndInside { get; private set; }

        public ITimeRange EndTouching { get; private set; }

        public ITimeRange After { get; private set; }
    }
}