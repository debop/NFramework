using System;

namespace NSoft.NFramework.TimePeriods {
    /// <summary>
    /// 수업 시간
    /// </summary>
    [Serializable]
    public class Lesson : TimeBlock {
        public static TimeSpan LessonDuration = TimePeriods.DurationUtil.Minutes(50);

        public Lesson(DateTime moment) : base(moment, LessonDuration) {}
    }

    /// <summary>
    /// 짧은 휴식시간
    /// </summary>
    [Serializable]
    public class ShortBreak : TimeBlock {
        public static TimeSpan ShortBreakDuration = TimePeriods.DurationUtil.Minutes(5);

        public ShortBreak(DateTime moment) : base(moment, ShortBreakDuration) {}
    }

    /// <summary>
    /// 긴 휴식시간
    /// </summary>
    [Serializable]
    public class LargeBreak : TimeBlock {
        public static TimeSpan LargeBreakDuration = TimePeriods.DurationUtil.Minutes(15);

        public LargeBreak(DateTime moment) : base(moment, LargeBreakDuration) {}
    }

    [Serializable]
    public class SchoolDay : TimePeriodChain {
        public SchoolDay() : this(GetDefaultStartDate(null)) {}
        public SchoolDay(IClock clock) : this(GetDefaultStartDate(clock)) {}

        public SchoolDay(DateTime moment) {
            Lesson1 = new Lesson(moment);
            Break1 = new ShortBreak(moment);
            Lesson2 = new Lesson(moment);
            Break2 = new LargeBreak(moment);
            Lesson3 = new Lesson(moment);
            Break3 = new ShortBreak(moment);
            Lesson4 = new Lesson(moment);

            base.Add(Lesson1);
            base.Add(Break1);
            base.Add(Lesson2);
            base.Add(Break2);
            base.Add(Lesson3);
            base.Add(Break3);
            base.Add(Lesson4);
        }

        public Lesson Lesson1 { get; private set; }
        public ShortBreak Break1 { get; private set; }
        public Lesson Lesson2 { get; private set; }
        public LargeBreak Break2 { get; private set; }
        public Lesson Lesson3 { get; private set; }
        public ShortBreak Break3 { get; private set; }
        public Lesson Lesson4 { get; private set; }

        private static DateTime GetDefaultStartDate(IClock clock) {
            clock = clock ?? ClockProxy.Clock;
            var now = clock.Now;

            return TimeTool.TrimToHour(now, 8);
        }
    }
}