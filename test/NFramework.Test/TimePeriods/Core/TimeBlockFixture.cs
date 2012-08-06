using System;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.TimePeriods.Core {
    [TestFixture]
    public class TimeBlockFixture : TimePeriodFixtureBase {
        private readonly TimeSpan _duration = DurationUtil.Hour;
        private readonly TimeSpan _offset = DurationUtil.Millisecond;

        private readonly DateTime _start;
        private readonly DateTime _end;

        private readonly TimeBlockPeriodRelationTestData _testData;

        public TimeBlockFixture() {
            _start = ClockProxy.Clock.Now;
            _end = _start.Add(_duration);
            _testData = new TimeBlockPeriodRelationTestData(_start, _duration, _offset);
        }

        [Test]
        public void AnytimeTest() {
            TimeBlock.Anytime.HasStart.Should().Be.False();
            TimeBlock.Anytime.Start.Should().Be(TimeSpec.MinPeriodTime);
            TimeBlock.Anytime.HasEnd.Should().Be.False();
            TimeBlock.Anytime.End.Should().Be(TimeSpec.MaxPeriodTime);
            TimeBlock.Anytime.Duration.Should().Be(TimeSpec.MaxPeriodDuration);

            TimeBlock.Anytime.IsAnytime.Should().Be.True();
            TimeBlock.Anytime.IsMoment.Should().Be.False();
            TimeBlock.Anytime.IsReadOnly.Should().Be.True();

            TimeBlock.Anytime.HasStart.Should().Be.False();
            TimeBlock.Anytime.HasEnd.Should().Be.False();
            TimeBlock.Anytime.HasPeriod.Should().Be.False();
        }

        [Test]
        public void DefaultTest() {
            var timeBlock = new TimeBlock();

            timeBlock.Should().Not.Be.EqualTo(TimeBlock.Anytime); // not readonly .vs. readonly
            timeBlock.GetRelation(TimeBlock.Anytime).Should().Be(PeriodRelation.ExactMatch);

            TimeBlock.Anytime.IsAnytime.Should().Be.True();
            TimeBlock.Anytime.IsMoment.Should().Be.False();
            TimeBlock.Anytime.IsReadOnly.Should().Be.True();

            TimeBlock.Anytime.HasStart.Should().Be.False();
            TimeBlock.Anytime.HasEnd.Should().Be.False();
            TimeBlock.Anytime.HasPeriod.Should().Be.False();
        }

        [Test]
        public void MomentTest() {
            var moment = ClockProxy.Clock.Now;
            var timeBlock = new TimeBlock(moment);

            timeBlock.Start.Should().Be(moment);
            timeBlock.End.Should().Be(moment);
            timeBlock.Duration.Should().Be(TimeSpec.MinPeriodDuration);

            timeBlock.IsAnytime.Should().Be.False();
            timeBlock.IsMoment.Should().Be.True();
            timeBlock.HasPeriod.Should().Be.True();
        }

        [Test]
        public void MomentByPeriodTest() {
            var timeBlock = new TimeBlock(ClockProxy.Clock.Now, TimeSpan.Zero);
            timeBlock.IsMoment.Should().Be.True();
        }

        [Test]
        public void NonMomentTest() {
            var timeBlock = new TimeBlock(ClockProxy.Clock.Now, TimeSpec.MinPositiveDuration);
            timeBlock.IsMoment.Should().Be.False();
            timeBlock.Duration.Should().Be(TimeSpec.MinPositiveDuration);
        }

        [Test]
        public void HasStartTest() {
            // 현재부터 ~ 쭉
            //
            var timeRange = new TimeBlock(ClockProxy.Clock.Now, null);

            timeRange.HasStart.Should().Be.True();
            timeRange.HasEnd.Should().Be.False();
        }

        [Test]
        public void HasEndTest() {
            // 현재까지
            var timeBlock = new TimeBlock(null, ClockProxy.Clock.Now);

            timeBlock.HasStart.Should().Be.False();
            timeBlock.HasEnd.Should().Be.True();
        }

        [Test]
        public void StartEndTest() {
            var timeBlock = new TimeBlock(_start, _end);

            timeBlock.Start.Should().Be(_start);
            timeBlock.End.Should().Be(_end);
            timeBlock.Duration.Should().Be(_duration);

            timeBlock.HasPeriod.Should().Be.True();
            timeBlock.IsAnytime.Should().Be.False();
            timeBlock.IsMoment.Should().Be.False();
            timeBlock.IsReadOnly.Should().Be.False();
        }

        [Test]
        public void StartEndSwapTest() {
            var timeBlock = new TimeBlock(_end, _start);

            timeBlock.Start.Should().Be(_start);
            timeBlock.End.Should().Be(_end);
            timeBlock.Duration.Should().Be(_duration);

            timeBlock.HasPeriod.Should().Be.True();
            timeBlock.IsAnytime.Should().Be.False();
            timeBlock.IsMoment.Should().Be.False();
            timeBlock.IsReadOnly.Should().Be.False();
        }

        [Test]
        public void StartTimeSpanTest() {
            var timeBlock = new TimeBlock(_start, _duration);

            timeBlock.Start.Should().Be(_start);
            timeBlock.Duration.Should().Be(_duration);
            timeBlock.End.Should().Be(_end);

            timeBlock.HasPeriod.Should().Be.True();
            timeBlock.IsAnytime.Should().Be.False();
            timeBlock.IsMoment.Should().Be.False();
            timeBlock.IsReadOnly.Should().Be.False();
        }

        [Test]
        public void StartNegativeTimeSpanTest() {
            Assert.Throws<InvalidOperationException>(() => new TimeBlock(_start, TimeSpan.FromMilliseconds(1).Negate()));
        }

        [Test]
        public void CopyConstructorTest() {
            var source = new TimeBlock(_start, _start.AddHours(1), true);
            var copy = new TimeBlock(source);

            copy.Start.Should().Be(source.Start);
            copy.Duration.Should().Be(source.Duration);
            copy.End.Should().Be(source.End);

            copy.IsReadOnly.Should().Be.True();

            copy.HasPeriod.Should().Be.True();
            copy.IsAnytime.Should().Be.False();
            copy.IsMoment.Should().Be.False();
        }

        [Test]
        public void StartTest() {
            var timeBlock = new TimeBlock(_start, _duration);

            timeBlock.Start.Should().Be(_start);
            timeBlock.Duration.Should().Be(_duration);

            var changedStart = _start.AddHours(-1);
            timeBlock.Start = changedStart;

            timeBlock.Start.Should().Be(changedStart);
            timeBlock.Duration.Should().Be(_duration);
        }

        [Test]
        public void StartReadOnlyTest() {
            var timeBlock = new TimeBlock(ClockProxy.Clock.Now, ClockProxy.Clock.Now.AddHours(1), true);
            Assert.Throws<InvalidOperationException>(() => timeBlock.Start = timeBlock.Start.AddHours(-1));
        }

        [Test]
        public void EndTest() {
            var timeBlock = new TimeBlock(_duration, _end);
            Assert.AreEqual(timeBlock.End, _end);
            Assert.AreEqual(timeBlock.Duration, _duration);
            DateTime changedEnd = _end.AddHours(1);
            timeBlock.End = changedEnd;
            Assert.AreEqual(timeBlock.End, changedEnd);
            Assert.AreEqual(timeBlock.Duration, _duration);
        }

        [Test]
        public void EndReadOnlyTest() {
            var timeBlock = new TimeBlock(ClockProxy.Clock.Now.AddHours(-1), ClockProxy.Clock.Now, true);
            Assert.Throws<InvalidOperationException>(() => timeBlock.End = timeBlock.End.AddHours(1));
        }

        [Test]
        public void DurationTest() {
            TimeBlock timeBlock = new TimeBlock(_start, _duration);
            Assert.AreEqual(timeBlock.Start, _start);
            Assert.AreEqual(timeBlock.End, _end);
            Assert.AreEqual(timeBlock.Duration, _duration);

            TimeSpan delta = TimeSpan.FromHours(1);
            TimeSpan newDuration = timeBlock.Duration + delta;
            timeBlock.Duration = newDuration;
            Assert.AreEqual(timeBlock.Start, _start);
            Assert.AreEqual(timeBlock.End, _end.Add(delta));
            Assert.AreEqual(timeBlock.Duration, newDuration);

            timeBlock.Duration = TimeSpec.MinPeriodDuration;
            Assert.AreEqual(timeBlock.Duration, TimeSpec.MinPeriodDuration);
        }

        [Test]
        public void MaxDurationOutOfRangeTest() {
            TimeBlock timeBlock = new TimeBlock(_start, _duration);
            Assert.Throws<ArgumentOutOfRangeException>(() => timeBlock.Duration = TimeSpec.MaxPeriodDuration);
        }

        [Test]
        public void DurationOutOfRangeTest() {
            TimeBlock timeBlock = new TimeBlock(_start, _duration);
            Assert.Throws<InvalidOperationException>(() => timeBlock.Duration = TimeSpan.FromMilliseconds(1).Negate());
        }

        [Test]
        public void DurationFromStartTest() {
            TimeBlock timeBlock = new TimeBlock(_start, _duration);
            Assert.AreEqual(timeBlock.Start, _start);
            Assert.AreEqual(timeBlock.End, _end);
            Assert.AreEqual(timeBlock.Duration, _duration);

            TimeSpan delta = TimeSpan.FromHours(1);
            TimeSpan newDuration = timeBlock.Duration + delta;
            timeBlock.DurationFromStart(newDuration);
            Assert.AreEqual(timeBlock.Start, _start);
            Assert.AreEqual(timeBlock.End, _end.Add(delta));
            Assert.AreEqual(timeBlock.Duration, newDuration);

            timeBlock.DurationFromStart(TimeSpec.MinPeriodDuration);
            Assert.AreEqual(timeBlock.Duration, TimeSpec.MinPeriodDuration);
        }

        [Test]
        public void DurationFromEndTest() {
            TimeBlock timeBlock = new TimeBlock(_start, _duration);
            Assert.AreEqual(timeBlock.Start, _start);
            Assert.AreEqual(timeBlock.End, _end);
            Assert.AreEqual(timeBlock.Duration, _duration);

            TimeSpan delta = TimeSpan.FromHours(1);
            TimeSpan newDuration = timeBlock.Duration + delta;
            timeBlock.DurationFromEnd(newDuration);
            Assert.AreEqual(timeBlock.Start, _start.Subtract(delta));
            Assert.AreEqual(timeBlock.End, _end);
            Assert.AreEqual(timeBlock.Duration, newDuration);

            timeBlock.DurationFromEnd(TimeSpec.MinPeriodDuration);
            Assert.AreEqual(timeBlock.Duration, TimeSpec.MinPeriodDuration);
        }

        [Test]
        public void HasInsideDateTimeTest() {
            TimeBlock timeBlock = new TimeBlock(_start, _duration);
            Assert.AreEqual(timeBlock.Duration, _duration);

            // start
            Assert.IsFalse(timeBlock.HasInside(_start.AddMilliseconds(-1)));
            Assert.IsTrue(timeBlock.HasInside(_start));
            Assert.IsTrue(timeBlock.HasInside(_start.AddMilliseconds(1)));

            // end
            Assert.IsTrue(timeBlock.HasInside(_end.AddMilliseconds(-1)));
            Assert.IsTrue(timeBlock.HasInside(_end));
            Assert.IsFalse(timeBlock.HasInside(_end.AddMilliseconds(1)));
        }

        [Test]
        public void HasInsidePeriodTest() {
            TimeBlock timeBlock = new TimeBlock(_start, _duration);
            Assert.AreEqual(timeBlock.Duration, _duration);

            // before
            TimeBlock before1 = new TimeBlock(_start.AddHours(-2), _start.AddHours(-1));
            Assert.IsFalse(timeBlock.HasInside(before1));
            TimeBlock before2 = new TimeBlock(_start.AddMilliseconds(-1), _end);
            Assert.IsFalse(timeBlock.HasInside(before2));
            TimeBlock before3 = new TimeBlock(_start.AddMilliseconds(-1), _start);
            Assert.IsFalse(timeBlock.HasInside(before3));

            // after
            TimeBlock after1 = new TimeBlock(_end.AddHours(1), _end.AddHours(2));
            Assert.IsFalse(timeBlock.HasInside(after1));
            TimeBlock after2 = new TimeBlock(_start, _end.AddMilliseconds(1));
            Assert.IsFalse(timeBlock.HasInside(after2));
            TimeBlock after3 = new TimeBlock(_end, _end.AddMilliseconds(1));
            Assert.IsFalse(timeBlock.HasInside(after3));

            // inside
            Assert.IsTrue(timeBlock.HasInside(timeBlock));
            TimeBlock inside1 = new TimeBlock(_start.AddMilliseconds(1), _end);
            Assert.IsTrue(timeBlock.HasInside(inside1));
            TimeBlock inside2 = new TimeBlock(_start.AddMilliseconds(1), _end.AddMilliseconds(-1));
            Assert.IsTrue(timeBlock.HasInside(inside2));
            TimeBlock inside3 = new TimeBlock(_start, _end.AddMilliseconds(-1));
            Assert.IsTrue(timeBlock.HasInside(inside3));
        }

        [Test]
        public void CopyTest() {
            TimeBlock readOnlyTimeBlock = new TimeBlock(_start, _duration);
            Assert.AreEqual(readOnlyTimeBlock.Copy(TimeSpan.Zero), readOnlyTimeBlock);

            TimeBlock timeBlock = new TimeBlock(_start, _duration);
            Assert.AreEqual(timeBlock.Start, _start);
            Assert.AreEqual(timeBlock.End, _end);
            Assert.AreEqual(timeBlock.Duration, _duration);

            ITimeBlock noMoveTimeBlock = timeBlock.Copy(TimeSpan.Zero);
            Assert.AreEqual(noMoveTimeBlock.Start, _start);
            Assert.AreEqual(noMoveTimeBlock.End, _end);
            Assert.AreEqual(noMoveTimeBlock.Duration, _duration);

            TimeSpan forwardOffset = new TimeSpan(2, 30, 15);
            ITimeBlock forwardTimeBlock = timeBlock.Copy(forwardOffset);
            Assert.AreEqual(forwardTimeBlock.Start, _start.Add(forwardOffset));
            Assert.AreEqual(forwardTimeBlock.End, _end.Add(forwardOffset));
            Assert.AreEqual(forwardTimeBlock.Duration, _duration);

            TimeSpan backwardOffset = new TimeSpan(-1, 10, 30);
            ITimeBlock backwardTimeBlock = timeBlock.Copy(backwardOffset);
            Assert.AreEqual(backwardTimeBlock.Start, _start.Add(backwardOffset));
            Assert.AreEqual(backwardTimeBlock.End, _end.Add(backwardOffset));
            Assert.AreEqual(backwardTimeBlock.Duration, _duration);
        }

        [Test]
        public void MoveTest() {
            TimeBlock timeBlockMoveZero = new TimeBlock(_start, _duration);
            timeBlockMoveZero.Move(TimeSpan.Zero);
            Assert.AreEqual(timeBlockMoveZero.Start, _start);
            Assert.AreEqual(timeBlockMoveZero.End, _end);
            Assert.AreEqual(timeBlockMoveZero.Duration, _duration);

            TimeBlock timeBlockMoveForward = new TimeBlock(_start, _duration);
            TimeSpan forwardOffset = new TimeSpan(2, 30, 15);
            timeBlockMoveForward.Move(forwardOffset);
            Assert.AreEqual(timeBlockMoveForward.Start, _start.Add(forwardOffset));
            Assert.AreEqual(timeBlockMoveForward.End, _end.Add(forwardOffset));

            TimeBlock timeBlockMoveBackward = new TimeBlock(_start, _duration);
            TimeSpan backwardOffset = new TimeSpan(-1, 10, 30);
            timeBlockMoveBackward.Move(backwardOffset);
            Assert.AreEqual(timeBlockMoveBackward.Start, _start.Add(backwardOffset));
            Assert.AreEqual(timeBlockMoveBackward.End, _end.Add(backwardOffset));
        }

        [Test]
        public void GetPreviousPeriodTest() {
            var readOnlyTimeBlock = new TimeBlock(_start, _duration, true);
            Assert.IsTrue(readOnlyTimeBlock.GetPreviousBlock().IsReadOnly);

            var timeBlock = new TimeBlock(_start, _duration);

            Assert.AreEqual(timeBlock.Start, _start);
            Assert.AreEqual(timeBlock.End, _end);
            Assert.AreEqual(timeBlock.Duration, _duration);

            var previousTimeBlock = timeBlock.GetPreviousBlock();
            Assert.AreEqual(previousTimeBlock.Start, _start.Subtract(_duration));
            Assert.AreEqual(previousTimeBlock.End, _start);
            Assert.AreEqual(previousTimeBlock.Duration, _duration);

            var previousOffset = TimeSpan.FromHours(1).Negate();
            var previousOffsetTimeBlock = timeBlock.GetPreviousBlock(previousOffset);

            previousOffsetTimeBlock.Start.Should().Be(_start.Subtract(_duration).Add(previousOffset));
            previousOffsetTimeBlock.End.Should().Be(_end.Subtract(_duration).Add(previousOffset));
            previousOffsetTimeBlock.Duration.Should().Be(_duration);
        }

        [Test]
        public void GetNextPeriodTest() {
            TimeBlock readOnlyTimeBlock = new TimeBlock(_start, _duration, true);
            Assert.IsTrue(readOnlyTimeBlock.GetNextBlock().IsReadOnly);

            TimeBlock timeBlock = new TimeBlock(_start, _duration);
            Assert.AreEqual(timeBlock.Start, _start);
            Assert.AreEqual(timeBlock.End, _end);
            Assert.AreEqual(timeBlock.Duration, _duration);

            ITimeBlock nextTimeBlock = timeBlock.GetNextBlock();
            Assert.AreEqual(nextTimeBlock.Start, _end);
            Assert.AreEqual(nextTimeBlock.End, _end.Add(_duration));
            Assert.AreEqual(nextTimeBlock.Duration, _duration);

            TimeSpan nextOffset = TimeSpan.FromHours(1);
            ITimeBlock nextOffsetTimeBlock = timeBlock.GetNextBlock(nextOffset);
            Assert.AreEqual(nextOffsetTimeBlock.Start, _end.Add(nextOffset));
            Assert.AreEqual(nextOffsetTimeBlock.End, _end.Add(_duration + nextOffset));
            Assert.AreEqual(nextOffsetTimeBlock.Duration, _duration);
        }

        [Test]
        public void IntersectsWithDurationTest() {
            TimeBlock timeBlock = new TimeBlock(_start, _duration);

            // before
            TimeBlock before1 = new TimeBlock(_start.AddHours(-2), _start.AddHours(-1));
            Assert.IsFalse(timeBlock.IntersectsWith(before1));
            TimeBlock before2 = new TimeBlock(_start.AddMilliseconds(-1), _start);
            Assert.IsTrue(timeBlock.IntersectsWith(before2));
            TimeBlock before3 = new TimeBlock(_start.AddMilliseconds(-1), _start.AddMilliseconds(1));
            Assert.IsTrue(timeBlock.IntersectsWith(before3));

            // after
            TimeBlock after1 = new TimeBlock(_end.AddHours(1), _end.AddHours(2));
            Assert.IsFalse(timeBlock.IntersectsWith(after1));
            TimeBlock after2 = new TimeBlock(_end, _end.AddMilliseconds(1));
            Assert.IsTrue(timeBlock.IntersectsWith(after2));
            TimeBlock after3 = new TimeBlock(_end.AddMilliseconds(-1), _end.AddMilliseconds(1));
            Assert.IsTrue(timeBlock.IntersectsWith(after3));

            // intersect
            Assert.IsTrue(timeBlock.IntersectsWith(timeBlock));
            TimeBlock itersect1 = new TimeBlock(_start.AddMilliseconds(-1), _end.AddMilliseconds(1));
            Assert.IsTrue(timeBlock.IntersectsWith(itersect1));
            TimeBlock itersect2 = new TimeBlock(_start.AddMilliseconds(-1), _start.AddMilliseconds(1));
            Assert.IsTrue(timeBlock.IntersectsWith(itersect2));
            TimeBlock itersect3 = new TimeBlock(_end.AddMilliseconds(-1), _end.AddMilliseconds(1));
            Assert.IsTrue(timeBlock.IntersectsWith(itersect3));
        }

        [Test]
        public void GetIntersectionTest() {
            TimeBlock readOnlyTimeBlock = new TimeBlock(_start, _duration);
            Assert.AreEqual(readOnlyTimeBlock.GetIntersectionBlock(readOnlyTimeBlock), new TimeBlock(readOnlyTimeBlock));

            TimeBlock timeBlock = new TimeBlock(_start, _duration);

            // before
            ITimeBlock before1 = timeBlock.GetIntersectionBlock(new TimeBlock(_start.AddHours(-2), _start.AddHours(-1)));
            Assert.AreEqual(before1, null);
            ITimeBlock before2 = timeBlock.GetIntersectionBlock(new TimeBlock(_start.AddMilliseconds(-1), _start));
            Assert.AreEqual(before2, new TimeBlock(_start));
            ITimeBlock before3 = timeBlock.GetIntersectionBlock(new TimeBlock(_start.AddMilliseconds(-1), _start.AddMilliseconds(1)));
            Assert.AreEqual(before3, new TimeBlock(_start, _start.AddMilliseconds(1)));

            // after
            ITimeBlock after1 = timeBlock.GetIntersectionBlock(new TimeBlock(_end.AddHours(1), _end.AddHours(2)));
            Assert.AreEqual(after1, null);
            ITimeBlock after2 = timeBlock.GetIntersectionBlock(new TimeBlock(_end, _end.AddMilliseconds(1)));
            Assert.AreEqual(after2, new TimeBlock(_end));
            ITimeBlock after3 = timeBlock.GetIntersectionBlock(new TimeBlock(_end.AddMilliseconds(-1), _end.AddMilliseconds(1)));
            Assert.AreEqual(after3, new TimeBlock(_end.AddMilliseconds(-1), _end));

            // intersect
            Assert.AreEqual(timeBlock.GetIntersectionBlock(timeBlock), timeBlock);
            ITimeBlock itersect1 = timeBlock.GetIntersectionBlock(new TimeBlock(_start.AddMilliseconds(-1), _end.AddMilliseconds(1)));
            Assert.AreEqual(itersect1, timeBlock);
            ITimeBlock itersect2 = timeBlock.GetIntersectionBlock(new TimeBlock(_start.AddMilliseconds(1), _end.AddMilliseconds(-1)));
            Assert.AreEqual(itersect2, new TimeBlock(_start.AddMilliseconds(1), _end.AddMilliseconds(-1)));
        }

        [Test]
        public void IsSamePeriodTest() {
            TimeBlock timeRange1 = new TimeBlock(_start, _duration);
            TimeBlock timeRange2 = new TimeBlock(_start, _duration);

            Assert.IsTrue(timeRange1.IsSamePeriod(timeRange1));
            Assert.IsTrue(timeRange2.IsSamePeriod(timeRange2));

            Assert.IsTrue(timeRange1.IsSamePeriod(timeRange2));
            Assert.IsTrue(timeRange2.IsSamePeriod(timeRange1));

            Assert.IsFalse(timeRange1.IsSamePeriod(TimeBlock.Anytime));
            Assert.IsFalse(timeRange2.IsSamePeriod(TimeBlock.Anytime));

            timeRange1.Move(new TimeSpan(1));
            Assert.IsFalse(timeRange1.IsSamePeriod(timeRange2));
            Assert.IsFalse(timeRange2.IsSamePeriod(timeRange1));

            timeRange1.Move(new TimeSpan(-1));
            Assert.IsTrue(timeRange1.IsSamePeriod(timeRange2));
            Assert.IsTrue(timeRange2.IsSamePeriod(timeRange1));
        }

        [Test]
        public void HasInsideTest() {
            Assert.IsFalse(_testData.Reference.HasInside(_testData.Before));
            Assert.IsFalse(_testData.Reference.HasInside(_testData.StartTouching));
            Assert.IsFalse(_testData.Reference.HasInside(_testData.StartInside));
            Assert.IsFalse(_testData.Reference.HasInside(_testData.InsideStartTouching));
            Assert.IsTrue(_testData.Reference.HasInside(_testData.EnclosingStartTouching));
            Assert.IsTrue(_testData.Reference.HasInside(_testData.Enclosing));
            Assert.IsTrue(_testData.Reference.HasInside(_testData.EnclosingEndTouching));
            Assert.IsTrue(_testData.Reference.HasInside(_testData.ExactMatch));
            Assert.IsFalse(_testData.Reference.HasInside(_testData.Inside));
            Assert.IsFalse(_testData.Reference.HasInside(_testData.InsideEndTouching));
            Assert.IsFalse(_testData.Reference.HasInside(_testData.EndInside));
            Assert.IsFalse(_testData.Reference.HasInside(_testData.EndTouching));
            Assert.IsFalse(_testData.Reference.HasInside(_testData.After));
        }

        [Test]
        public void IntersectsWithTest() {
            Assert.IsFalse(_testData.Reference.IntersectsWith(_testData.Before));
            Assert.IsTrue(_testData.Reference.IntersectsWith(_testData.StartTouching));
            Assert.IsTrue(_testData.Reference.IntersectsWith(_testData.StartInside));
            Assert.IsTrue(_testData.Reference.IntersectsWith(_testData.InsideStartTouching));
            Assert.IsTrue(_testData.Reference.IntersectsWith(_testData.EnclosingStartTouching));
            Assert.IsTrue(_testData.Reference.IntersectsWith(_testData.Enclosing));
            Assert.IsTrue(_testData.Reference.IntersectsWith(_testData.EnclosingEndTouching));
            Assert.IsTrue(_testData.Reference.IntersectsWith(_testData.ExactMatch));
            Assert.IsTrue(_testData.Reference.IntersectsWith(_testData.Inside));
            Assert.IsTrue(_testData.Reference.IntersectsWith(_testData.InsideEndTouching));
            Assert.IsTrue(_testData.Reference.IntersectsWith(_testData.EndInside));
            Assert.IsTrue(_testData.Reference.IntersectsWith(_testData.EndTouching));
            Assert.IsFalse(_testData.Reference.IntersectsWith(_testData.After));
        }

        [Test]
        public void OverlapsWithTest() {
            Assert.IsFalse(_testData.Reference.OverlapsWith(_testData.Before));
            Assert.IsFalse(_testData.Reference.OverlapsWith(_testData.StartTouching));
            Assert.IsTrue(_testData.Reference.OverlapsWith(_testData.StartInside));
            Assert.IsTrue(_testData.Reference.OverlapsWith(_testData.InsideStartTouching));
            Assert.IsTrue(_testData.Reference.OverlapsWith(_testData.EnclosingStartTouching));
            Assert.IsTrue(_testData.Reference.OverlapsWith(_testData.Enclosing));
            Assert.IsTrue(_testData.Reference.OverlapsWith(_testData.EnclosingEndTouching));
            Assert.IsTrue(_testData.Reference.OverlapsWith(_testData.ExactMatch));
            Assert.IsTrue(_testData.Reference.OverlapsWith(_testData.Inside));
            Assert.IsTrue(_testData.Reference.OverlapsWith(_testData.InsideEndTouching));
            Assert.IsTrue(_testData.Reference.OverlapsWith(_testData.EndInside));
            Assert.IsFalse(_testData.Reference.OverlapsWith(_testData.EndTouching));
            Assert.IsFalse(_testData.Reference.OverlapsWith(_testData.After));
        }

        [Test]
        public void GetRelationTest() {
            Assert.AreEqual(_testData.Reference.GetRelation(_testData.Before), PeriodRelation.Before);
            Assert.AreEqual(_testData.Reference.GetRelation(_testData.StartTouching), PeriodRelation.StartTouching);
            Assert.AreEqual(_testData.Reference.GetRelation(_testData.StartInside), PeriodRelation.StartInside);
            Assert.AreEqual(_testData.Reference.GetRelation(_testData.InsideStartTouching), PeriodRelation.InsideStartTouching);
            Assert.AreEqual(_testData.Reference.GetRelation(_testData.EnclosingStartTouching), PeriodRelation.EnclosingStartTouching);
            Assert.AreEqual(_testData.Reference.GetRelation(_testData.Enclosing), PeriodRelation.Enclosing);
            Assert.AreEqual(_testData.Reference.GetRelation(_testData.EnclosingEndTouching), PeriodRelation.EnclosingEndTouching);
            Assert.AreEqual(_testData.Reference.GetRelation(_testData.ExactMatch), PeriodRelation.ExactMatch);
            Assert.AreEqual(_testData.Reference.GetRelation(_testData.Inside), PeriodRelation.Inside);
            Assert.AreEqual(_testData.Reference.GetRelation(_testData.InsideEndTouching), PeriodRelation.InsideEndTouching);
            Assert.AreEqual(_testData.Reference.GetRelation(_testData.EndInside), PeriodRelation.EndInside);
            Assert.AreEqual(_testData.Reference.GetRelation(_testData.EndTouching), PeriodRelation.EndTouching);
            Assert.AreEqual(_testData.Reference.GetRelation(_testData.After), PeriodRelation.After);

            // reference
            Assert.AreEqual(_testData.Reference.Start, _start);
            Assert.AreEqual(_testData.Reference.End, _end);
            Assert.IsTrue(_testData.Reference.IsReadOnly);

            // after
            Assert.IsTrue(_testData.After.IsReadOnly);
            Assert.Less(_testData.After.Start, _start);
            Assert.Less(_testData.After.End, _start);
            Assert.IsFalse(_testData.Reference.HasInside(((ITimePeriod)_testData.After).Start));
            Assert.IsFalse(_testData.Reference.HasInside(((ITimePeriod)_testData.After).End));
            Assert.AreEqual(_testData.Reference.GetRelation(_testData.After), PeriodRelation.After);

            // start touching
            Assert.IsTrue(_testData.StartTouching.IsReadOnly);
            Assert.Less(_testData.StartTouching.Start, _start);
            Assert.AreEqual(_testData.StartTouching.End, _start);
            Assert.IsFalse(_testData.Reference.HasInside(((ITimePeriod)_testData.StartTouching).Start));
            Assert.IsTrue(_testData.Reference.HasInside(((ITimePeriod)_testData.StartTouching).End));
            Assert.AreEqual(_testData.Reference.GetRelation(_testData.StartTouching), PeriodRelation.StartTouching);

            // start inside
            Assert.IsTrue(_testData.StartInside.IsReadOnly);
            Assert.Less(_testData.StartInside.Start, _start);
            Assert.Less(_testData.StartInside.End, _end);
            Assert.Greater(_testData.StartInside.End, _start);
            Assert.IsFalse(_testData.Reference.HasInside(((ITimePeriod)_testData.StartInside).Start));
            Assert.IsTrue(_testData.Reference.HasInside(((ITimePeriod)_testData.StartInside).End));
            Assert.AreEqual(_testData.Reference.GetRelation(_testData.StartInside), PeriodRelation.StartInside);

            // inside start touching
            Assert.IsTrue(_testData.InsideStartTouching.IsReadOnly);
            Assert.AreEqual(_testData.InsideStartTouching.Start, _start);
            Assert.Greater(_testData.InsideStartTouching.End, _end);
            Assert.IsTrue(_testData.Reference.HasInside(((ITimePeriod)_testData.InsideStartTouching).Start));
            Assert.IsFalse(_testData.Reference.HasInside(((ITimePeriod)_testData.InsideStartTouching).End));
            Assert.AreEqual(_testData.Reference.GetRelation(_testData.InsideStartTouching), PeriodRelation.InsideStartTouching);

            // enclosing start touching
            Assert.IsTrue(_testData.EnclosingStartTouching.IsReadOnly);
            Assert.AreEqual(_testData.EnclosingStartTouching.Start, _start);
            Assert.Less(_testData.EnclosingStartTouching.End, _end);
            Assert.IsTrue(_testData.Reference.HasInside(((ITimePeriod)_testData.EnclosingStartTouching).Start));
            Assert.IsTrue(_testData.Reference.HasInside(((ITimePeriod)_testData.EnclosingStartTouching).End));
            Assert.AreEqual(_testData.Reference.GetRelation(_testData.EnclosingStartTouching), PeriodRelation.EnclosingStartTouching);

            // enclosing
            Assert.IsTrue(_testData.Enclosing.IsReadOnly);
            Assert.Greater(_testData.Enclosing.Start, _start);
            Assert.Less(_testData.Enclosing.End, _end);
            Assert.IsTrue(_testData.Reference.HasInside(((ITimePeriod)_testData.Enclosing).Start));
            Assert.IsTrue(_testData.Reference.HasInside(((ITimePeriod)_testData.Enclosing).End));
            Assert.AreEqual(_testData.Reference.GetRelation(_testData.Enclosing), PeriodRelation.Enclosing);

            // enclosing end touching
            Assert.IsTrue(_testData.EnclosingEndTouching.IsReadOnly);
            Assert.Greater(_testData.EnclosingEndTouching.Start, _start);
            Assert.AreEqual(_testData.EnclosingEndTouching.End, _end);
            Assert.IsTrue(_testData.Reference.HasInside(((ITimePeriod)_testData.EnclosingEndTouching).Start));
            Assert.IsTrue(_testData.Reference.HasInside(((ITimePeriod)_testData.EnclosingEndTouching).End));
            Assert.AreEqual(_testData.Reference.GetRelation(_testData.EnclosingEndTouching), PeriodRelation.EnclosingEndTouching);

            // exact match
            Assert.IsTrue(_testData.ExactMatch.IsReadOnly);
            Assert.AreEqual(_testData.ExactMatch.Start, _start);
            Assert.AreEqual(_testData.ExactMatch.End, _end);
            Assert.IsTrue(_testData.Reference.Equals(_testData.ExactMatch));
            Assert.IsTrue(_testData.Reference.HasInside(((ITimePeriod)_testData.ExactMatch).Start));
            Assert.IsTrue(_testData.Reference.HasInside(((ITimePeriod)_testData.ExactMatch).End));
            Assert.AreEqual(_testData.Reference.GetRelation(_testData.ExactMatch), PeriodRelation.ExactMatch);

            // inside
            Assert.IsTrue(_testData.Inside.IsReadOnly);
            Assert.Less(_testData.Inside.Start, _start);
            Assert.Greater(_testData.Inside.End, _end);
            Assert.IsFalse(_testData.Reference.HasInside(((ITimePeriod)_testData.Inside).Start));
            Assert.IsFalse(_testData.Reference.HasInside(((ITimePeriod)_testData.Inside).End));
            Assert.AreEqual(_testData.Reference.GetRelation(_testData.Inside), PeriodRelation.Inside);

            // inside end touching
            Assert.IsTrue(_testData.InsideEndTouching.IsReadOnly);
            Assert.Less(_testData.InsideEndTouching.Start, _start);
            Assert.AreEqual(_testData.InsideEndTouching.End, _end);
            Assert.IsFalse(_testData.Reference.HasInside(((ITimePeriod)_testData.InsideEndTouching).Start));
            Assert.IsTrue(_testData.Reference.HasInside(((ITimePeriod)_testData.InsideEndTouching).End));
            Assert.AreEqual(_testData.Reference.GetRelation(_testData.InsideEndTouching), PeriodRelation.InsideEndTouching);

            // end inside
            Assert.IsTrue(_testData.EndInside.IsReadOnly);
            Assert.Greater(_testData.EndInside.Start, _start);
            Assert.Less(_testData.EndInside.Start, _end);
            Assert.Greater(_testData.EndInside.End, _end);
            Assert.IsTrue(_testData.Reference.HasInside(((ITimePeriod)_testData.EndInside).Start));
            Assert.IsFalse(_testData.Reference.HasInside(((ITimePeriod)_testData.EndInside).End));
            Assert.AreEqual(_testData.Reference.GetRelation(_testData.EndInside), PeriodRelation.EndInside);

            // end touching
            Assert.IsTrue(_testData.EndTouching.IsReadOnly);
            Assert.AreEqual(_testData.EndTouching.Start, _end);
            Assert.Greater(_testData.EndTouching.End, _end);
            Assert.IsTrue(_testData.Reference.HasInside(((ITimePeriod)_testData.EndTouching).Start));
            Assert.IsFalse(_testData.Reference.HasInside(((ITimePeriod)_testData.EndTouching).End));
            Assert.AreEqual(_testData.Reference.GetRelation(_testData.EndTouching), PeriodRelation.EndTouching);

            // before
            Assert.IsTrue(_testData.Before.IsReadOnly);
            Assert.Greater(_testData.Before.Start, _testData.Reference.End);
            Assert.Greater(_testData.Before.End, _testData.Reference.End);
            Assert.IsFalse(_testData.Reference.HasInside(((ITimePeriod)_testData.Before).Start));
            Assert.IsFalse(_testData.Reference.HasInside(((ITimePeriod)_testData.Before).End));
            Assert.AreEqual(_testData.Reference.GetRelation(_testData.Before), PeriodRelation.Before);
        }

        [Test]
        public void ResetTest() {
            TimeBlock timeBlock = new TimeBlock(_start, _duration);
            Assert.AreEqual(timeBlock.Start, _start);
            Assert.IsTrue(timeBlock.HasStart);
            Assert.AreEqual(timeBlock.End, _end);
            Assert.IsTrue(timeBlock.HasEnd);
            Assert.AreEqual(timeBlock.Duration, _duration);

            timeBlock.Reset();
            Assert.AreEqual(timeBlock.Start, TimeSpec.MinPeriodTime);
            Assert.IsFalse(timeBlock.HasStart);
            Assert.AreEqual(timeBlock.End, TimeSpec.MaxPeriodTime);
            Assert.IsFalse(timeBlock.HasEnd);
        }

        [Test]
        public void EqualsTest() {
            var timeBlock1 = new TimeBlock(_start, _duration);
            var timeBlock2 = new TimeBlock(_start, _duration);

            timeBlock1.Should().Be(timeBlock2);

            var timeBlock3 = new TimeBlock(_start.AddMilliseconds(-1), _end.AddMilliseconds(1));

            timeBlock1.Should().Not.Be(timeBlock3);
        }
    }
}