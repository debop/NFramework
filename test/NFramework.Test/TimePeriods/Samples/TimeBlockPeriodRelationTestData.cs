using System;
using System.Collections.Generic;

namespace NSoft.NFramework.TimePeriods {
    [Serializable]
    public class TimeBlockPeriodRelationTestData {
        private readonly List<ITimePeriod> allPeriods = new List<ITimePeriod>();

        public TimeBlockPeriodRelationTestData(DateTime start, TimeSpan duration, TimeSpan offset) {
            Guard.Assert(offset > TimeSpan.Zero, "offset은 양의 수여야 합니다.");

            var end = start.Add(duration);
            Reference = new TimeBlock(start, duration, true);

            var beforeEnd = start.Subtract(offset);
            var beforeStart = beforeEnd.Subtract(Reference.Duration);
            var insideStart = start.Add(offset);
            var insideEnd = end.Subtract(offset);
            var afterStart = end.Add(offset);
            var afterEnd = afterStart.Add(Reference.Duration);

            After = new TimeBlock(beforeStart, beforeEnd, true);
            StartTouching = new TimeBlock(beforeStart, start, true);
            StartInside = new TimeBlock(beforeStart, insideStart, true);
            InsideStartTouching = new TimeBlock(start, afterStart, true);
            EnclosingStartTouching = new TimeBlock(start, insideEnd, true);
            Enclosing = new TimeBlock(insideStart, insideEnd, true);
            EnclosingEndTouching = new TimeBlock(insideStart, end, true);
            ExactMatch = new TimeBlock(start, end, true);
            Inside = new TimeBlock(beforeStart, afterEnd, true);
            InsideEndTouching = new TimeBlock(beforeStart, end, true);
            EndInside = new TimeBlock(insideEnd, afterEnd, true);
            EndTouching = new TimeBlock(end, afterEnd, true);
            Before = new TimeBlock(afterStart, afterEnd, true);

            allPeriods.Add(Reference);
            allPeriods.Add(After);
            allPeriods.Add(StartTouching);
            allPeriods.Add(StartInside);
            allPeriods.Add(InsideStartTouching);
            allPeriods.Add(EnclosingStartTouching);
            allPeriods.Add(Enclosing);
            allPeriods.Add(EnclosingEndTouching);
            allPeriods.Add(ExactMatch);
            allPeriods.Add(Inside);
            allPeriods.Add(InsideEndTouching);
            allPeriods.Add(EndInside);
            allPeriods.Add(EndTouching);
            allPeriods.Add(Before);
        }

        public ICollection<ITimePeriod> AllPeriods {
            get { return allPeriods; }
        }

        public ITimeBlock Reference { get; private set; }

        public ITimeBlock Before { get; private set; }

        public ITimeBlock StartTouching { get; private set; }

        public ITimeBlock StartInside { get; private set; }

        public ITimeBlock InsideStartTouching { get; private set; }

        public ITimeBlock EnclosingStartTouching { get; private set; }

        public ITimeBlock Inside { get; private set; }

        public ITimeBlock EnclosingEndTouching { get; private set; }

        public ITimeBlock ExactMatch { get; private set; }

        public ITimeBlock Enclosing { get; private set; }

        public ITimeBlock InsideEndTouching { get; private set; }

        public ITimeBlock EndInside { get; private set; }

        public ITimeBlock EndTouching { get; private set; }

        public ITimeBlock After { get; private set; }
    }
}