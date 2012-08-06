using System;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.TimePeriods.Core {
    [TestFixture]
    public class TimePeriodChainFixture : TimePeriodFixtureBase {
        [Test]
        public void DefaultConstructorTest() {
            var periodChain = new TimePeriodChain();

            periodChain.Count.Should().Be(0);
            periodChain.HasStart.Should().Be.False();
            periodChain.HasEnd.Should().Be.False();
            periodChain.HasPeriod.Should().Be.False();
            periodChain.IsReadOnly.Should().Be.False();
            periodChain.IsMoment.Should().Be.False();

            periodChain.IsAnytime.Should().Be.True();
        }

        [Test]
        public void CopyConstructorTest() {
            var schoolDay = new SchoolDay();
            var copyChain = new TimePeriodChain(schoolDay);

            copyChain.Count.Should().Be(schoolDay.Count);
            copyChain.HasStart.Should().Be(schoolDay.HasStart);
            copyChain.HasEnd.Should().Be(schoolDay.HasEnd);
            copyChain.IsReadOnly.Should().Be(schoolDay.IsReadOnly);

            copyChain.Start.Should().Be(schoolDay.Start);
            copyChain.End.Should().Be(schoolDay.End);
            copyChain.Duration.Should().Be(schoolDay.Duration);
        }

        [Test]
        public void FirstTest() {
            var schoolDay = new SchoolDay();
            schoolDay.First.Should().Be(schoolDay.Lesson1);
        }

        [Test]
        public void LastTest() {
            var schoolDay = new SchoolDay();
            schoolDay.Last.Should().Be(schoolDay.Lesson4);
        }

        [Test]
        public void CountTest() {
            new TimePeriodChain().Count.Should().Be(0);
            new SchoolDay().Count.Should().Be(7);
        }

        [Test]
        public void IndexTest() {
            var schoolDay = new SchoolDay();

            schoolDay[0].Should().Be(schoolDay.Lesson1);
            schoolDay[1].Should().Be(schoolDay.Break1);
            schoolDay[2].Should().Be(schoolDay.Lesson2);
            schoolDay[3].Should().Be(schoolDay.Break2);
            schoolDay[4].Should().Be(schoolDay.Lesson3);
            schoolDay[5].Should().Be(schoolDay.Break3);
            schoolDay[6].Should().Be(schoolDay.Lesson4);
        }

        [Test]
        public void IsAnyTimeTest() {
            var now = ClockProxy.Clock.Now;
            var chain = new TimePeriodChain();

            chain.IsAnytime.Should().Be.True();

            chain.Add(new TimeBlock(TimeSpec.MinPeriodTime, now));

            chain.IsAnytime.Should().Be.False();

            chain.Add(new TimeBlock(now, TimeSpec.MaxPeriodTime));

            chain.IsAnytime.Should().Be.True();
        }

        [Test]
        public void IsMomentTest() {
            var now = ClockProxy.Clock.Now;
            var chain = new TimePeriodChain();

            chain.IsMoment.Should().Be.False();

            chain.Add(new TimeBlock(now));

            chain.Count.Should().Be(1);
            chain.HasStart.Should().Be.True();
            chain.HasEnd.Should().Be.True();
            chain.HasPeriod.Should().Be.True();
            chain.IsMoment.Should().Be.True();
            chain.IsAnytime.Should().Be.False();

            chain.Add(new TimeBlock(now.AddDays(1)));

            chain.Count.Should().Be(2);
            chain.HasStart.Should().Be.True();
            chain.HasEnd.Should().Be.True();
            chain.HasPeriod.Should().Be.True();
            chain.IsMoment.Should().Be.True();
            chain.IsAnytime.Should().Be.False();

            chain.Add(new TimeBlock(now, TimeSpan.FromDays(1)));

            chain.Count.Should().Be(3);
            chain.HasStart.Should().Be.True();
            chain.HasEnd.Should().Be.True();
            chain.HasPeriod.Should().Be.True();
            chain.IsMoment.Should().Be.False();
            chain.IsAnytime.Should().Be.False();
        }

        [Test]
        public void StartTest() {
            var now = ClockProxy.Clock.Now;
            var chain = new TimePeriodChain();

            chain.Start.Should().Be(TimeSpec.MinPeriodTime);

            chain.Add(new TimeBlock(now, DurationUtil.Hour));
            chain.Start.Should().Be(now);

            chain.Clear();
            chain.Start.Should().Be(TimeSpec.MinPeriodTime);
        }

        [Test]
        public void StartMoveTest() {
            var now = ClockProxy.Clock.Now;
            var schoolDay = new SchoolDay(now);
            schoolDay.Start.Should().Be(now);

            schoolDay.Start = now.AddHours(0);
            schoolDay.Start.Should().Be(now);

            schoolDay.Start = now.AddHours(1);
            schoolDay.Start.Should().Be(now.AddHours(1));

            schoolDay.Start = now.AddHours(-1);
            schoolDay.Start.Should().Be(now.AddHours(-1));
        }

        [Test]
        public void EndTest() {
            var now = ClockProxy.Clock.Now;
            var chain = new TimePeriodChain();

            chain.End.Should().Be(TimeSpec.MaxPeriodTime);

            chain.Add(new TimeBlock(DurationUtil.Hour, now));
            chain.End.Should().Be(now);

            chain.Clear();
            chain.End.Should().Be(TimeSpec.MaxPeriodTime);
        }

        [Test]
        public void EndMoveTest() {
            var now = ClockProxy.Clock.Now;
            var schoolDay = new SchoolDay(now);

            var end = schoolDay.End;

            schoolDay.End.Should().Be(end);

            schoolDay.End = end.AddHours(0);
            schoolDay.End.Should().Be(end);

            schoolDay.End = end.AddHours(1);
            schoolDay.End.Should().Be(end.AddHours(1));

            schoolDay.End = end.AddHours(-1);
            schoolDay.End.Should().Be(end.AddHours(-1));
        }

        [Test]
        public void DurationTest() {
            var chain = new TimePeriodChain();
            chain.Duration.Should().Be(TimeSpec.MaxPeriodDuration);

            chain.Add(new TimeBlock(ClockProxy.Clock.Now, DurationUtil.Hour));
            chain.Duration.Should().Be(DurationUtil.Hour);

            chain.HasPeriod.Should().Be.True();
            chain.IsMoment.Should().Be.False();
            chain.IsAnytime.Should().Be.False();
        }

        [Test]
        public void MoveTest() {
            var schoolDay = new SchoolDay();

            var start = schoolDay.Start;
            var end = schoolDay.End;
            var duration = schoolDay.Duration;

            var offset = DurationUtil.Hour;
            schoolDay.Move(offset);

            schoolDay.Start.Should().Be(start.Add(offset));
            schoolDay.End.Should().Be(end.Add(offset));
            schoolDay.Duration.Should().Be(duration);

            schoolDay.Move(-offset);

            schoolDay.Start.Should().Be(start);
            schoolDay.End.Should().Be(end);
            schoolDay.Duration.Should().Be(duration);
        }

        [Test]
        public void AddTest() {
            var schoolDay = new SchoolDay();

            var count = schoolDay.Count;
            var end = schoolDay.End;
            var shortBreak = new ShortBreak(schoolDay.Start);
            schoolDay.Add(shortBreak);

            schoolDay.Count.Should().Be(count + 1);
            schoolDay.Last.Should().Be(shortBreak);
            schoolDay.End.Should().Be(end.Add(shortBreak.Duration));

            shortBreak.Start.Should().Be(end);
            shortBreak.End.Should().Be(schoolDay.End);
            shortBreak.Duration.Should().Be(ShortBreak.ShortBreakDuration);
        }

        [Test]
        public void AddTimeRangeTest() {
            var schoolDay = new SchoolDay();
            var chain = new TimePeriodChain(schoolDay);

            var range = new TimeRange(schoolDay.Lesson1.Start, schoolDay.Lesson1.End);

            chain.Add(range);
            chain.Last.Should().Be(range);
        }

        [Test]
        public void ContainsPeriodTest() {
            var schoolDay = new SchoolDay();
            var chain = new TimePeriodChain(schoolDay);

            var timeRange = new TimeRange(schoolDay.Lesson1.Start, schoolDay.Lesson1.End);

            chain.Contains(timeRange).Should().Be.False();
            chain.ContainsPeriod(timeRange).Should().Be.True();
        }

        [Test]
        public void AddAllTest() {
            SchoolDay schoolDay = new SchoolDay();

            int count = schoolDay.Count;
            TimeSpan duration = schoolDay.Duration;
            DateTime start = schoolDay.Start;

            schoolDay.AddAll(new SchoolDay());
            Assert.AreEqual(schoolDay.Count, count + count);
            Assert.AreEqual(schoolDay.Start, start);
            Assert.AreEqual(schoolDay.Duration, duration + duration);
        }

        [Test]
        public void AddReadOnlyTest() {
            var timePeriods = new TimePeriodChain();
            Assert.Throws<InvalidOperationException>(
                () => timePeriods.Add(new TimeRange(TimeSpec.MinPeriodTime, TimeSpec.MaxPeriodTime, true)));
        }

        [Test]
        public void InsertReadOnlyTest() {
            var timePeriods = new TimePeriodChain();
            Assert.Throws<InvalidOperationException>(
                () => timePeriods.Insert(0, new TimeRange(TimeSpec.MinPeriodTime, TimeSpec.MaxPeriodTime, true)));
        }

        [Test]
        public void InsertTest() {
            var schoolDay = new SchoolDay();

            // first
            int count = schoolDay.Count;
            DateTime start = schoolDay.Start;
            Lesson lesson1 = new Lesson(schoolDay.Start);
            schoolDay.Insert(0, lesson1);
            Assert.AreEqual(schoolDay.Count, count + 1);
            Assert.AreEqual(schoolDay[0], lesson1);
            Assert.AreEqual(schoolDay.First, lesson1);
            Assert.AreEqual(schoolDay.Start, start.Subtract(lesson1.Duration));
            Assert.AreEqual(lesson1.Start, schoolDay.Start);
            Assert.AreEqual(lesson1.End, start);
            Assert.AreEqual(lesson1.Duration, Lesson.LessonDuration);

            // inside
            count = schoolDay.Count;
            start = schoolDay.Start;
            ShortBreak shortBreak1 = new ShortBreak(schoolDay.Start);
            schoolDay.Insert(1, shortBreak1);
            Assert.AreEqual(schoolDay.Count, count + 1);
            Assert.AreEqual(schoolDay[1], shortBreak1);
            Assert.AreEqual(schoolDay.First, lesson1);
            Assert.AreEqual(schoolDay.Start, start);
            Assert.AreEqual(shortBreak1.Start, schoolDay.Start.Add(lesson1.Duration));
            Assert.AreEqual(shortBreak1.Duration, ShortBreak.ShortBreakDuration);

            // last
            count = schoolDay.Count;
            DateTime end = schoolDay.End;
            ShortBreak shortBreak2 = new ShortBreak(schoolDay.Start);
            schoolDay.Insert(schoolDay.Count, shortBreak2);
            Assert.AreEqual(schoolDay.Count, count + 1);
            Assert.AreEqual(schoolDay[count], shortBreak2);
            Assert.AreEqual(schoolDay.Last, shortBreak2);
            Assert.AreEqual(schoolDay.End, shortBreak2.End);
            Assert.AreEqual(shortBreak2.Start, end);
            Assert.AreEqual(shortBreak2.Duration, ShortBreak.ShortBreakDuration);
        }

        [Test]
        public void InsertTimeRangeTest() {
            DateTime now = ClockProxy.Clock.Now;
            SchoolDay schoolDay = new SchoolDay(now);
            TimePeriodChain timePeriods = new TimePeriodChain(schoolDay);

            TimeRange timeRange = new TimeRange(schoolDay.Lesson1.Start, schoolDay.Lesson1.End);

            timePeriods.Add(timeRange);
            Assert.AreEqual(timePeriods.Last, timeRange);
        }

        [Test]
        public void ContainsTest() {
            SchoolDay schoolDay = new SchoolDay();

            Assert.IsFalse(schoolDay.Contains(new TimeRange()));
            Assert.IsFalse(schoolDay.Contains(new TimeBlock()));

            Assert.IsTrue(schoolDay.Contains(schoolDay.Lesson1));
            Assert.IsTrue(schoolDay.Contains(schoolDay.Break1));
            Assert.IsTrue(schoolDay.Contains(schoolDay.Lesson2));
            Assert.IsTrue(schoolDay.Contains(schoolDay.Break2));
            Assert.IsTrue(schoolDay.Contains(schoolDay.Lesson3));
            Assert.IsTrue(schoolDay.Contains(schoolDay.Break3));
            Assert.IsTrue(schoolDay.Contains(schoolDay.Lesson4));

            schoolDay.Remove(schoolDay.Lesson1);
            Assert.IsFalse(schoolDay.Contains(schoolDay.Lesson1));
        }

        [Test]
        public void IndexOfTest() {
            SchoolDay schoolDay = new SchoolDay();

            Assert.AreEqual(schoolDay.IndexOf(new TimeRange()), -1);
            Assert.AreEqual(schoolDay.IndexOf(new TimeBlock()), -1);

            Assert.AreEqual(schoolDay.IndexOf(schoolDay.Lesson1), 0);
            Assert.AreEqual(schoolDay.IndexOf(schoolDay.Break1), 1);
            Assert.AreEqual(schoolDay.IndexOf(schoolDay.Lesson2), 2);
            Assert.AreEqual(schoolDay.IndexOf(schoolDay.Break2), 3);
            Assert.AreEqual(schoolDay.IndexOf(schoolDay.Lesson3), 4);
            Assert.AreEqual(schoolDay.IndexOf(schoolDay.Break3), 5);
            Assert.AreEqual(schoolDay.IndexOf(schoolDay.Lesson4), 6);

            schoolDay.Remove(schoolDay.Lesson1);
            Assert.AreEqual(schoolDay.IndexOf(schoolDay.Lesson1), -1);
        }

        [Test]
        public void CopyToTest() {
            TimePeriodChain timePeriods = new TimePeriodChain();
            ITimePeriod[] array1 = new ITimePeriod[0];
            timePeriods.CopyTo(array1, 0);

            SchoolDay schoolDay = new SchoolDay();
            ITimePeriod[] array2 = new ITimePeriod[schoolDay.Count];
            schoolDay.CopyTo(array2, 0);
            Assert.AreEqual(array2[0], schoolDay.Lesson1);
            Assert.AreEqual(array2[1], schoolDay.Break1);
            Assert.AreEqual(array2[2], schoolDay.Lesson2);
            Assert.AreEqual(array2[3], schoolDay.Break2);
            Assert.AreEqual(array2[4], schoolDay.Lesson3);
            Assert.AreEqual(array2[5], schoolDay.Break3);
            Assert.AreEqual(array2[6], schoolDay.Lesson4);

            ITimePeriod[] array3 = new ITimePeriod[schoolDay.Count + 3];
            schoolDay.CopyTo(array3, 3);
            Assert.AreEqual(array3[3], schoolDay.Lesson1);
            Assert.AreEqual(array3[4], schoolDay.Break1);
            Assert.AreEqual(array3[5], schoolDay.Lesson2);
            Assert.AreEqual(array3[6], schoolDay.Break2);
            Assert.AreEqual(array3[7], schoolDay.Lesson3);
            Assert.AreEqual(array3[8], schoolDay.Break3);
            Assert.AreEqual(array3[9], schoolDay.Lesson4);
        }

        [Test]
        public void ClearTest() {
            var timePeriods = new TimePeriodChain();
            Assert.AreEqual(timePeriods.Count, 0);
            timePeriods.Clear();
            Assert.AreEqual(timePeriods.Count, 0);

            SchoolDay schoolDay = new SchoolDay();
            Assert.AreEqual(schoolDay.Count, 7);
            schoolDay.Clear();
            Assert.AreEqual(schoolDay.Count, 0);
        }

        [Test]
        public void RemoveTest() {
            var schoolDay = new SchoolDay();

            // first
            int count = schoolDay.Count;
            DateTime end = schoolDay.End;
            ITimePeriod removeItem = schoolDay.First;
            TimeSpan duration = schoolDay.Duration;
            schoolDay.Remove(removeItem);
            Assert.AreEqual(schoolDay.Count, count - 1);
            Assert.AreNotEqual(schoolDay.First, removeItem);
            Assert.AreEqual(schoolDay.End, end);
            Assert.AreEqual(schoolDay.Duration, duration.Subtract(removeItem.Duration));

            // inside
            count = schoolDay.Count;
            duration = schoolDay.Duration;
            DateTime start = schoolDay.Start;
            ITimePeriod first = schoolDay.First;
            ITimePeriod last = schoolDay.Last;
            removeItem = schoolDay[1];
            schoolDay.Remove(removeItem);
            Assert.AreEqual(schoolDay.Count, count - 1);
            Assert.AreNotEqual(schoolDay[1], removeItem);
            Assert.AreEqual(schoolDay.First, first);
            Assert.AreEqual(schoolDay.Start, start);
            Assert.AreEqual(schoolDay.Last, last);
            Assert.AreEqual(schoolDay.Duration, duration.Subtract(removeItem.Duration));

            // last
            count = schoolDay.Count;
            start = schoolDay.Start;
            duration = schoolDay.Duration;
            removeItem = schoolDay.Last;
            schoolDay.Remove(removeItem);
            Assert.AreEqual(schoolDay.Count, count - 1);
            Assert.AreNotEqual(schoolDay.Last, removeItem);
            Assert.AreEqual(schoolDay.Start, start);
            Assert.AreEqual(schoolDay.Duration, duration.Subtract(removeItem.Duration));
        }

        [Test]
        public void RemoveAtTest() {
            var schoolDay = new SchoolDay();

            // first
            int count = schoolDay.Count;
            DateTime end = schoolDay.End;
            ITimePeriod removeItem = schoolDay[0];
            TimeSpan duration = schoolDay.Duration;
            schoolDay.RemoveAt(0);
            Assert.AreEqual(schoolDay.Count, count - 1);
            Assert.AreNotEqual(schoolDay[0], removeItem);
            Assert.AreEqual(schoolDay.End, end);
            Assert.AreEqual(schoolDay.Duration, duration.Subtract(removeItem.Duration));

            // inside
            count = schoolDay.Count;
            duration = schoolDay.Duration;
            DateTime start = schoolDay.Start;
            ITimePeriod first = schoolDay.First;
            ITimePeriod last = schoolDay.Last;
            removeItem = schoolDay[1];
            schoolDay.RemoveAt(1);
            Assert.AreEqual(schoolDay.Count, count - 1);
            Assert.AreNotEqual(schoolDay[1], removeItem);
            Assert.AreEqual(schoolDay.First, first);
            Assert.AreEqual(schoolDay.Start, start);
            Assert.AreEqual(schoolDay.Last, last);
            Assert.AreEqual(schoolDay.Duration, duration.Subtract(removeItem.Duration));

            // last
            count = schoolDay.Count;
            start = schoolDay.Start;
            duration = schoolDay.Duration;
            removeItem = schoolDay[schoolDay.Count - 1];
            schoolDay.RemoveAt(schoolDay.Count - 1);
            Assert.AreEqual(schoolDay.Count, count - 1);
            Assert.AreNotEqual(schoolDay[schoolDay.Count - 1], removeItem);
            Assert.AreEqual(schoolDay.Start, start);
            Assert.AreEqual(schoolDay.Duration, duration.Subtract(removeItem.Duration));
        }

        [Test]
        public void IsSamePeriodTest() {
            var schoolDay = new SchoolDay();
            TimeRange manualRange = new TimeRange(schoolDay.Start, schoolDay.End);

            Assert.IsTrue(schoolDay.IsSamePeriod(schoolDay));
            Assert.IsTrue(schoolDay.IsSamePeriod(manualRange));
            Assert.IsTrue(manualRange.IsSamePeriod(schoolDay));

            Assert.IsFalse(schoolDay.IsSamePeriod(TimeBlock.Anytime));
            Assert.IsFalse(manualRange.IsSamePeriod(TimeBlock.Anytime));

            schoolDay.RemoveAt(0);
            Assert.IsFalse(schoolDay.IsSamePeriod(manualRange));
            Assert.IsFalse(manualRange.IsSamePeriod(schoolDay));
        }

        [Test]
        public void HasInsideTest() {
            SchoolDay schoolDay = new SchoolDay();
            TimeSpan offset = DurationUtil.Second;
            TimeRangePeriodRelationTestData testData = new TimeRangePeriodRelationTestData(schoolDay.Start, schoolDay.End, offset);

            Assert.IsFalse(schoolDay.HasInside(testData.Before));
            Assert.IsFalse(schoolDay.HasInside(testData.StartTouching));
            Assert.IsFalse(schoolDay.HasInside(testData.StartInside));
            Assert.IsFalse(schoolDay.HasInside(testData.InsideStartTouching));
            Assert.IsTrue(schoolDay.HasInside(testData.EnclosingStartTouching));
            Assert.IsTrue(schoolDay.HasInside(testData.Enclosing));
            Assert.IsTrue(schoolDay.HasInside(testData.ExactMatch));
            Assert.IsTrue(schoolDay.HasInside(testData.EnclosingEndTouching));
            Assert.IsFalse(schoolDay.HasInside(testData.Inside));
            Assert.IsFalse(schoolDay.HasInside(testData.InsideEndTouching));
            Assert.IsFalse(schoolDay.HasInside(testData.EndInside));
            Assert.IsFalse(schoolDay.HasInside(testData.EndTouching));
            Assert.IsFalse(schoolDay.HasInside(testData.After));
        }

        [Test]
        public void IntersectsWithTest() {
            SchoolDay schoolDay = new SchoolDay();
            TimeSpan offset = DurationUtil.Second;
            TimeRangePeriodRelationTestData testData = new TimeRangePeriodRelationTestData(schoolDay.Start, schoolDay.End, offset);

            Assert.IsFalse(schoolDay.IntersectsWith(testData.Before));
            Assert.IsTrue(schoolDay.IntersectsWith(testData.StartTouching));
            Assert.IsTrue(schoolDay.IntersectsWith(testData.StartInside));
            Assert.IsTrue(schoolDay.IntersectsWith(testData.InsideStartTouching));
            Assert.IsTrue(schoolDay.IntersectsWith(testData.EnclosingStartTouching));
            Assert.IsTrue(schoolDay.IntersectsWith(testData.Enclosing));
            Assert.IsTrue(schoolDay.IntersectsWith(testData.EnclosingEndTouching));
            Assert.IsTrue(schoolDay.IntersectsWith(testData.ExactMatch));
            Assert.IsTrue(schoolDay.IntersectsWith(testData.Inside));
            Assert.IsTrue(schoolDay.IntersectsWith(testData.InsideEndTouching));
            Assert.IsTrue(schoolDay.IntersectsWith(testData.EndInside));
            Assert.IsTrue(schoolDay.IntersectsWith(testData.EndTouching));
            Assert.IsFalse(schoolDay.IntersectsWith(testData.After));
        }

        [Test]
        public void OverlapsWithTest() {
            SchoolDay schoolDay = new SchoolDay();
            TimeSpan offset = DurationUtil.Second;
            TimeRangePeriodRelationTestData testData = new TimeRangePeriodRelationTestData(schoolDay.Start, schoolDay.End, offset);

            Assert.IsFalse(schoolDay.OverlapsWith(testData.Before));
            Assert.IsFalse(schoolDay.OverlapsWith(testData.StartTouching));
            Assert.IsTrue(schoolDay.OverlapsWith(testData.StartInside));
            Assert.IsTrue(schoolDay.OverlapsWith(testData.InsideStartTouching));
            Assert.IsTrue(schoolDay.OverlapsWith(testData.EnclosingStartTouching));
            Assert.IsTrue(schoolDay.OverlapsWith(testData.Enclosing));
            Assert.IsTrue(schoolDay.OverlapsWith(testData.EnclosingEndTouching));
            Assert.IsTrue(schoolDay.OverlapsWith(testData.ExactMatch));
            Assert.IsTrue(schoolDay.OverlapsWith(testData.Inside));
            Assert.IsTrue(schoolDay.OverlapsWith(testData.InsideEndTouching));
            Assert.IsTrue(schoolDay.OverlapsWith(testData.EndInside));
            Assert.IsFalse(schoolDay.OverlapsWith(testData.EndTouching));
            Assert.IsFalse(schoolDay.OverlapsWith(testData.After));
        }

        [Test]
        public void GetRelationTest() {
            var schoolDay = new SchoolDay();
            var offset = DurationUtil.Second;
            var testData = new TimeRangePeriodRelationTestData(schoolDay.Start, schoolDay.End, offset);

            Assert.AreEqual(schoolDay.GetRelation(testData.Before), PeriodRelation.Before);
            Assert.AreEqual(schoolDay.GetRelation(testData.StartTouching), PeriodRelation.StartTouching);
            Assert.AreEqual(schoolDay.GetRelation(testData.StartInside), PeriodRelation.StartInside);
            Assert.AreEqual(schoolDay.GetRelation(testData.InsideStartTouching), PeriodRelation.InsideStartTouching);
            Assert.AreEqual(schoolDay.GetRelation(testData.EnclosingStartTouching), PeriodRelation.EnclosingStartTouching);
            Assert.AreEqual(schoolDay.GetRelation(testData.Enclosing), PeriodRelation.Enclosing);
            Assert.AreEqual(schoolDay.GetRelation(testData.EnclosingEndTouching), PeriodRelation.EnclosingEndTouching);
            Assert.AreEqual(schoolDay.GetRelation(testData.ExactMatch), PeriodRelation.ExactMatch);
            Assert.AreEqual(schoolDay.GetRelation(testData.Inside), PeriodRelation.Inside);
            Assert.AreEqual(schoolDay.GetRelation(testData.InsideEndTouching), PeriodRelation.InsideEndTouching);
            Assert.AreEqual(schoolDay.GetRelation(testData.EndInside), PeriodRelation.EndInside);
            Assert.AreEqual(schoolDay.GetRelation(testData.EndTouching), PeriodRelation.EndTouching);
            Assert.AreEqual(schoolDay.GetRelation(testData.After), PeriodRelation.After);
        }
    }
}