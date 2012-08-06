using System;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.TimePeriods.Core {
    [TestFixture]
    public class TimeRangeFixture : TimePeriodFixtureBase {
        private static readonly TimeSpan duration = new TimeSpan(1, 0, 0);
        private static readonly TimeSpan offset = new TimeSpan(0, 0, 1);

        private DateTime _start;
        private DateTime _end;
        private TimeRangePeriodRelationTestData _testData;

        [TestFixtureSetUp]
        public void ClassSetUp() {
            _start = DateTime.Now;
            _end = _start.Add(duration);

            _testData = new TimeRangePeriodRelationTestData(_start, _end, offset);
        }

        [Test]
        public void AnytimeTest() {
            TimeRange.Anytime.Start.Should().Be(TimeSpec.MinPeriodTime);
            TimeRange.Anytime.End.Should().Be(TimeSpec.MaxPeriodTime);

            TimeRange.Anytime.IsAnytime.Should().Be.True();
            TimeRange.Anytime.IsReadOnly.Should().Be.True();

            TimeRange.Anytime.HasPeriod.Should().Be.False();
            TimeRange.Anytime.HasStart.Should().Be.False();
            TimeRange.Anytime.HasEnd.Should().Be.False();
            TimeRange.Anytime.IsMoment.Should().Be.False();
        }

        [Test]
        public void DefaultConstructorTest() {
            var timeRange = new TimeRange();

            timeRange.Should().Not.Be.EqualTo(TimeRange.Anytime); // IsReadOnly 때문에
            timeRange.GetRelation(TimeRange.Anytime).Should().Be(PeriodRelation.ExactMatch);

            timeRange.IsAnytime.Should().Be.True();
            timeRange.IsReadOnly.Should().Be.False();

            timeRange.HasPeriod.Should().Be.False();
            timeRange.HasStart.Should().Be.False();
            timeRange.HasEnd.Should().Be.False();
            timeRange.IsMoment.Should().Be.False();
        }

        [Test]
        public void MomentTest() {
            var moment = ClockProxy.Clock.Now;
            var timeRange = new TimeRange(moment);

            timeRange.Start.Should().Be(moment);
            timeRange.End.Should().Be(moment);
            timeRange.Duration.Should().Be(TimeSpec.MinPeriodDuration);

            timeRange.IsAnytime.Should().Be.False();
            timeRange.IsMoment.Should().Be.True();
            timeRange.HasPeriod.Should().Be.True();
        }

        [Test]
        public void MomentByPeriodTest() {
            var timeRange = new TimeRange(ClockProxy.Clock.Now, TimeSpan.Zero);
            timeRange.IsMoment.Should().Be.True();
        }

        [Test]
        public void NonMomentTest() {
            var timeRange = new TimeRange(ClockProxy.Clock.Now, TimeSpec.MinPositiveDuration);
            timeRange.IsMoment.Should().Be.False();
            timeRange.Duration.Should().Be(TimeSpec.MinPositiveDuration);
        }

        [Test]
        public void HasStartTest() {
            //현재부터 ~ 쭉
            var timeRange = new TimeRange(ClockProxy.Clock.Now, null);

            timeRange.HasStart.Should().Be.True();
            timeRange.HasEnd.Should().Be.False();
        }

        [Test]
        public void HasEndTest() {
            // 현재까지
            var timeRange = new TimeRange(null, ClockProxy.Clock.Now);

            timeRange.HasStart.Should().Be.False();
            timeRange.HasEnd.Should().Be.True();
        }

        [Test]
        public void StartEndTest() {
            var range = new TimeRange(_start, _end);

            range.Start.Should().Be(_start);
            range.End.Should().Be(_end);
            range.Duration.Should().Be(duration);

            range.HasPeriod.Should().Be.True();
            range.IsAnytime.Should().Be.False();
            range.IsMoment.Should().Be.False();
            range.IsReadOnly.Should().Be.False();
        }

        [Test]
        public void StartEndSwapTest() {
            var range = new TimeRange(_end, _start);

            range.Start.Should().Be(_start);
            range.End.Should().Be(_end);
            range.Duration.Should().Be(duration);

            range.HasPeriod.Should().Be.True();
            range.IsAnytime.Should().Be.False();
            range.IsMoment.Should().Be.False();
            range.IsReadOnly.Should().Be.False();
        }

        [Test]
        public void StartTimeSpanTest() {
            var range = new TimeRange(_start, duration);

            range.Start.Should().Be(_start);
            range.Duration.Should().Be(duration);
            range.End.Should().Be(_end);

            range.HasPeriod.Should().Be.True();
            range.IsAnytime.Should().Be.False();
            range.IsMoment.Should().Be.False();
            range.IsReadOnly.Should().Be.False();
        }

        [Test]
        public void StartNegateTimeSpanTest() {
            var negateDuration = TimeSpec.MinNegativeDuration;

            var range = new TimeRange(_start, negateDuration);

            range.Start.Should().Be(_start.Add(negateDuration));
            range.Duration.Should().Be(negateDuration.Negate());
            range.End.Should().Be(_start);

            range.HasPeriod.Should().Be.True();
            range.IsAnytime.Should().Be.False();
            range.IsMoment.Should().Be.False();
            range.IsReadOnly.Should().Be.False();
        }

        [Test]
        public void CopyConstructorTest() {
            var source = new TimeRange(_start, _start.AddHours(1), true);
            var copy = new TimeRange(source);

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
            var range = new TimeRange(_start, _start.AddHours(1));

            range.Start.Should().Be(_start);

            var changedStart = _start.AddHours(-1);
            range.Start = changedStart;

            range.Start.Should().Be(changedStart);
        }

        [Test]
        public void StartReadOnlyTest() {
            var range = new TimeRange(ClockProxy.Clock.Now, ClockProxy.Clock.Now.AddHours(1), true);
            Assert.Throws<InvalidOperationException>(() => range.Start = range.Start.AddHours(-1));
        }

        [Test]
        public void StartOutOfRangeTest() {
            var range = new TimeRange(ClockProxy.Clock.Now, ClockProxy.Clock.Now.AddHours(1), true);
            Assert.Throws<InvalidOperationException>(() => range.Start = range.Start.AddHours(2));
        }

        [Test]
        public void EndTest() {
            var range = new TimeRange(_end.AddHours(-1), _end);

            range.End.Should().Be(_end);

            var changedEnd = _end.AddHours(1);
            range.End = changedEnd;

            range.End.Should().Be(changedEnd);
        }

        [Test]
        public void EndReadOnlyTest() {
            var range = new TimeRange(ClockProxy.Clock.Now, ClockProxy.Clock.Now.AddHours(1), true);
            Assert.Throws<InvalidOperationException>(() => range.End = range.End.AddHours(1));
        }

        [Test]
        public void EndOutOfRangeTest() {
            var range = new TimeRange(ClockProxy.Clock.Now, ClockProxy.Clock.Now.AddHours(1), true);
            Assert.Throws<InvalidOperationException>(() => range.End = range.End.AddHours(-2));
        }

        [Test]
        public void HasInsideDateTimeTest() {
            var range = new TimeRange(_start, _end);

            range.End.Should().Be(_end);

            range.HasInside(_start.Add(duration.Negate())).Should().Be.False();
            range.HasInside(_start).Should().Be.True();
            range.HasInside(_start.Add(duration)).Should().Be.True();

            range.HasInside(_end.Add(duration.Negate())).Should().Be.True();
            range.HasInside(_end).Should().Be.True();
            range.HasInside(_end.Add(duration)).Should().Be.False();
        }

        [Test]
        public void HasInsidePeriodTest() {
            var range = new TimeRange(_start, _end);
            range.End.Should().Be(_end);

            // before
            var before1 = new TimeRange(_start.AddHours(-2), _start.AddHours(-1));
            var before2 = new TimeRange(_start.AddMilliseconds(-1), _end);
            var before3 = new TimeRange(_start.AddMilliseconds(-1), _start);

            range.HasInside(before1).Should().Be.False();
            range.HasInside(before2).Should().Be.False();
            range.HasInside(before3).Should().Be.False();

            // after
            var after1 = new TimeRange(_end.AddHours(1), _end.AddHours(2));
            var after2 = new TimeRange(_start, _end.AddMilliseconds(1));
            var after3 = new TimeRange(_end, _end.AddMilliseconds(1));

            range.HasInside(after2).Should().Be.False();
            range.HasInside(after1).Should().Be.False();
            range.HasInside(after3).Should().Be.False();

            // inside
            range.HasInside(range).Should().Be.True();

            var inside1 = new TimeRange(_start.AddMilliseconds(1), _end);
            var inside2 = new TimeRange(_start.AddMilliseconds(1), _end.AddMilliseconds(-1));
            var inside3 = new TimeRange(_start, _end.AddMilliseconds(-1));

            range.HasInside(inside1).Should().Be.True();
            range.HasInside(inside2).Should().Be.True();
            range.HasInside(inside3).Should().Be.True();
        }

        [Test]
        public void CopyTest() {
            var readOnlyTimeRange = new TimeRange(_start, _end);
            readOnlyTimeRange.Copy().Should().Be(readOnlyTimeRange);
            readOnlyTimeRange.Copy(TimeSpan.Zero).Should().Be(readOnlyTimeRange);

            var timeRange = new TimeRange(_start, _end);
            timeRange.Start.Should().Be(_start);
            timeRange.End.Should().Be(_end);

            var noMoveTimeRange = timeRange.Copy(TimeSpan.Zero);

            noMoveTimeRange.Start.Should().Be(timeRange.Start);
            noMoveTimeRange.End.Should().Be(timeRange.End);
            noMoveTimeRange.Duration.Should().Be(timeRange.Duration);
            noMoveTimeRange.Should().Be(timeRange);

            var forwardOffset = new TimeSpan(2, 30, 15);
            var forwardTimeRange = timeRange.Copy(forwardOffset);

            forwardTimeRange.Start.Should().Be(_start.Add(forwardOffset));
            forwardTimeRange.End.Should().Be(_end.Add(forwardOffset));
            forwardTimeRange.Duration.Should().Be(duration);

            var backwardOffset = new TimeSpan(-1, 10, 30);
            var backwardTimeRange = timeRange.Copy(backwardOffset);

            backwardTimeRange.Start.Should().Be(_start.Add(backwardOffset));
            backwardTimeRange.End.Should().Be(_end.Add(backwardOffset));
            backwardTimeRange.Duration.Should().Be(duration);
        }

        [Test]
        public void MoveTest() {
            var timeRangeMoveZero = new TimeRange(_start, _end);
            timeRangeMoveZero.Move(TimeSpan.Zero);

            timeRangeMoveZero.Start.Should().Be(_start);
            timeRangeMoveZero.End.Should().Be(_end);
            timeRangeMoveZero.Duration.Should().Be(duration);

            var timeRangeMoveForward = new TimeRange(_start, _end);
            var forwardOffset = new TimeSpan(2, 30, 15);
            timeRangeMoveForward.Move(forwardOffset);

            timeRangeMoveForward.Start.Should().Be(_start.Add(forwardOffset));
            timeRangeMoveForward.End.Should().Be(_end.Add(forwardOffset));
            timeRangeMoveForward.Duration.Should().Be(duration);


            var timeRangeMoveBackward = new TimeRange(_start, _end);
            var backwardOffset = new TimeSpan(-1, 10, 30);
            timeRangeMoveBackward.Move(backwardOffset);

            timeRangeMoveBackward.Start.Should().Be(_start.Add(backwardOffset));
            timeRangeMoveBackward.End.Should().Be(_end.Add(backwardOffset));
            timeRangeMoveBackward.Duration.Should().Be(duration);
        }

        [Test]
        public void ExpandStartToTest() {
            var timeRange = new TimeRange(_start, _end);

            timeRange.ExpandStartTo(_start.AddMilliseconds(1));
            timeRange.Start.Should().Be(_start);

            timeRange.ExpandStartTo(_start.AddMinutes(-1));
            timeRange.Start.Should().Be(_start.AddMinutes(-1));
        }

        [Test]
        public void ExpandEndToTest() {
            var timeRange = new TimeRange(_start, _end);

            timeRange.ExpandEndTo(_end.AddMilliseconds(-1));
            timeRange.End.Should().Be(_end);

            timeRange.ExpandEndTo(_end.AddMinutes(1));
            timeRange.End.Should().Be(_end.AddMinutes(1));
        }

        [Test]
        public void ExpandToDateTimeTest() {
            var timeRange = new TimeRange(_start, _end);

            // start
            timeRange.ExpandTo(_start.AddMilliseconds(1));
            timeRange.Start.Should().Be(_start);

            timeRange.ExpandTo(_start.AddMinutes(-1));
            timeRange.Start.Should().Be(_start.AddMinutes(-1));

            // end
            timeRange.ExpandTo(_end.AddMilliseconds(-1));
            timeRange.End.Should().Be(_end);

            timeRange.ExpandTo(_end.AddMinutes(1));
            timeRange.End.Should().Be(_end.AddMinutes(1));
        }

        [Test]
        public void ExpandToPeriodTest() {
            TimeRange timeRange = new TimeRange(_start, _end);

            // no expansion
            timeRange.ExpandTo(new TimeRange(_start.AddMilliseconds(1), _end.AddMilliseconds(-1)));
            timeRange.Start.Should().Be(_start);
            timeRange.End.Should().Be(_end);

            // start
            var changedStart = _start.AddMinutes(-1);
            timeRange.ExpandTo(new TimeRange(changedStart, _end));
            timeRange.Start.Should().Be(changedStart);
            timeRange.End.Should().Be(_end);

            // end
            var changedEnd = _end.AddMinutes(1);
            timeRange.ExpandTo(new TimeRange(changedStart, changedEnd));
            timeRange.Start.Should().Be(changedStart);
            timeRange.End.Should().Be(changedEnd);

            // start/end
            changedStart = changedStart.AddMinutes(-1);
            changedEnd = changedEnd.AddMinutes(1);
            timeRange.ExpandTo(new TimeRange(changedStart, changedEnd));
            timeRange.Start.Should().Be(changedStart);
            timeRange.End.Should().Be(changedEnd);
        }

        [Test]
        public void ShrinkStartToTest() {
            var timeRange = new TimeRange(_start, _end);

            timeRange.ShrinkStartTo(_start.AddMilliseconds(-1));
            timeRange.Start.Should().Be(_start);

            timeRange.ShrinkStartTo(_start.AddMinutes(1));
            timeRange.Start.Should().Be(_start.AddMinutes(1));
        }

        [Test]
        public void ShrinkEndToTest() {
            var timeRange = new TimeRange(_start, _end);

            timeRange.ShrinkEndTo(_end.AddMilliseconds(1));
            timeRange.End.Should().Be(_end);

            timeRange.ShrinkEndTo(_end.AddMinutes(-1));
            timeRange.End.Should().Be(_end.AddMinutes(-1));
        }

        [Test]
        public void ShrinkToTest() {
            var timeRange = new TimeRange(_start, _end);

            // no shrink
            timeRange.ShrinkTo(new TimeRange(_start.AddMilliseconds(-1), _end.AddMilliseconds(1)));
            timeRange.Start.Should().Be(_start);
            timeRange.End.Should().Be(_end);

            // start
            var changedStart = _start.AddMinutes(1);
            timeRange.ShrinkTo(new TimeRange(changedStart, _end));
            timeRange.Start.Should().Be(changedStart);
            timeRange.End.Should().Be(_end);

            // end
            var changedEnd = _end.AddMinutes(-1);
            timeRange.ShrinkTo(new TimeRange(changedStart, changedEnd));
            timeRange.Start.Should().Be(changedStart);
            timeRange.End.Should().Be(changedEnd);

            // start/end
            changedStart = changedStart.AddMinutes(1);
            changedEnd = changedEnd.AddMinutes(-1);
            timeRange.ShrinkTo(new TimeRange(changedStart, changedEnd));
            timeRange.Start.Should().Be(changedStart);
            timeRange.End.Should().Be(changedEnd);
        }

        [Test]
        public void IsSamePeriodTest() {
            var timeRange1 = new TimeRange(_start, _end);
            var timeRange2 = new TimeRange(_start, _end);

            Assert.IsTrue(timeRange1.IsSamePeriod(timeRange1));
            Assert.IsTrue(timeRange2.IsSamePeriod(timeRange2));

            Assert.IsTrue(timeRange1.IsSamePeriod(timeRange2));
            Assert.IsTrue(timeRange2.IsSamePeriod(timeRange1));

            Assert.IsFalse(timeRange1.IsSamePeriod(TimeRange.Anytime));
            Assert.IsFalse(timeRange2.IsSamePeriod(TimeRange.Anytime));

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
        public void IntersectsWithDateTimeTest() {
            TimeRange timeRange = new TimeRange(_start, _end);

            // before
            var before1 = new TimeRange(_start.AddHours(-2), _start.AddHours(-1));
            Assert.IsFalse(timeRange.IntersectsWith(before1));

            var before2 = new TimeRange(_start.AddMilliseconds(-1), _start);
            Assert.IsTrue(timeRange.IntersectsWith(before2));

            var before3 = new TimeRange(_start.AddMilliseconds(-1), _start.AddMilliseconds(1));
            Assert.IsTrue(timeRange.IntersectsWith(before3));

            // after
            var after1 = new TimeRange(_end.AddHours(1), _end.AddHours(2));
            Assert.IsFalse(timeRange.IntersectsWith(after1));

            var after2 = new TimeRange(_end, _end.AddMilliseconds(1));
            Assert.IsTrue(timeRange.IntersectsWith(after2));

            var after3 = new TimeRange(_end.AddMilliseconds(-1), _end.AddMilliseconds(1));
            Assert.IsTrue(timeRange.IntersectsWith(after3));

            // intersect
            Assert.IsTrue(timeRange.IntersectsWith(timeRange));

            var itersect1 = new TimeRange(_start.AddMilliseconds(-1), _end.AddMilliseconds(1));
            Assert.IsTrue(timeRange.IntersectsWith(itersect1));

            var itersect2 = new TimeRange(_start.AddMilliseconds(-1), _start.AddMilliseconds(1));
            Assert.IsTrue(timeRange.IntersectsWith(itersect2));

            var itersect3 = new TimeRange(_end.AddMilliseconds(-1), _end.AddMilliseconds(1));
            Assert.IsTrue(timeRange.IntersectsWith(itersect3));
        }

        [Test]
        public void GetIntersectionTest() {
            var readOnlyTimeRange = new TimeRange(_start, _end);
            readOnlyTimeRange.GetIntersection(readOnlyTimeRange).Should().Be(new TimeRange(readOnlyTimeRange));

            var timeRange = new TimeRange(_start, _end);

            // before
            var before1 = timeRange.GetIntersection(new TimeRange(_start.AddHours(-2), _start.AddHours(-1)));
            before1.Should().Be.Null();

            var before2 = timeRange.GetIntersection(new TimeRange(_start.AddMilliseconds(-1), _start));
            before2.Should().Be(new TimeRange(_start));

            var before3 = timeRange.GetIntersection(new TimeRange(_start.AddMilliseconds(-1), _start.AddMilliseconds(1)));
            before3.Should().Be(new TimeRange(_start, _start.AddMilliseconds(1)));

            // after
            var after1 = timeRange.GetIntersection(new TimeRange(_end.AddHours(1), _end.AddHours(2)));
            after1.Should().Be.Null();

            var after2 = timeRange.GetIntersection(new TimeRange(_end, _end.AddMilliseconds(1)));
            after2.Should().Be(new TimeRange(_end));

            var after3 = timeRange.GetIntersection(new TimeRange(_end.AddMilliseconds(-1), _end.AddMilliseconds(1)));
            after3.Should().Be(new TimeRange(_end.AddMilliseconds(-1), _end));

            // intersect
            timeRange.GetIntersection(timeRange).Should().Be(timeRange);

            var itersect1 = timeRange.GetIntersection(new TimeRange(_start.AddMilliseconds(-1), _end.AddMilliseconds(1)));
            itersect1.Should().Be(timeRange);

            var itersect2 = timeRange.GetIntersection(new TimeRange(_start.AddMilliseconds(1), _end.AddMilliseconds(-1)));
            itersect2.Should().Be(new TimeRange(_start.AddMilliseconds(1), _end.AddMilliseconds(-1)));
        }

        [Test]
        public void GetRelationTest() {
            _testData.Reference.GetRelation(_testData.Before).Should().Be(PeriodRelation.Before);
            _testData.Reference.GetRelation(_testData.StartTouching).Should().Be(PeriodRelation.StartTouching);
            _testData.Reference.GetRelation(_testData.StartInside).Should().Be(PeriodRelation.StartInside);
            _testData.Reference.GetRelation(_testData.InsideStartTouching).Should().Be(PeriodRelation.InsideStartTouching);
            _testData.Reference.GetRelation(_testData.Enclosing).Should().Be(PeriodRelation.Enclosing);
            _testData.Reference.GetRelation(_testData.ExactMatch).Should().Be(PeriodRelation.ExactMatch);
            _testData.Reference.GetRelation(_testData.Inside).Should().Be(PeriodRelation.Inside);
            _testData.Reference.GetRelation(_testData.InsideEndTouching).Should().Be(PeriodRelation.InsideEndTouching);
            _testData.Reference.GetRelation(_testData.EndInside).Should().Be(PeriodRelation.EndInside);
            _testData.Reference.GetRelation(_testData.EndTouching).Should().Be(PeriodRelation.EndTouching);
            _testData.Reference.GetRelation(_testData.After).Should().Be(PeriodRelation.After);

            // reference
            _testData.Reference.Start.Should().Be(_start);
            _testData.Reference.End.Should().Be(_end);
            Assert.IsTrue(_testData.Reference.IsReadOnly);

            // after
            Assert.IsTrue(_testData.After.IsReadOnly);
            Assert.Less(_testData.After.Start, _start);
            Assert.Less(_testData.After.End, _start);
            Assert.IsFalse(_testData.Reference.HasInside(((ITimePeriod)_testData.After).Start));
            Assert.IsFalse(_testData.Reference.HasInside(((ITimePeriod)_testData.After).End));
            _testData.Reference.GetRelation(_testData.After).Should().Be(PeriodRelation.After);

            // start touching
            Assert.IsTrue(_testData.StartTouching.IsReadOnly);
            Assert.Less(_testData.StartTouching.Start, _start);
            _testData.StartTouching.End.Should().Be(_start);

            Assert.IsFalse(_testData.Reference.HasInside(((ITimePeriod)_testData.StartTouching).Start));
            Assert.IsTrue(_testData.Reference.HasInside(((ITimePeriod)_testData.StartTouching).End));
            _testData.Reference.GetRelation(_testData.StartTouching).Should().Be(PeriodRelation.StartTouching);

            // start inside
            Assert.IsTrue(_testData.StartInside.IsReadOnly);
            Assert.Less(_testData.StartInside.Start, _start);
            Assert.Less(_testData.StartInside.End, _end);
            Assert.Greater(_testData.StartInside.End, _start);
            Assert.IsFalse(_testData.Reference.HasInside(((ITimePeriod)_testData.StartInside).Start));
            Assert.IsTrue(_testData.Reference.HasInside(((ITimePeriod)_testData.StartInside).End));
            _testData.Reference.GetRelation(_testData.StartInside).Should().Be(PeriodRelation.StartInside);

            // inside start touching
            Assert.IsTrue(_testData.InsideStartTouching.IsReadOnly);
            _testData.InsideStartTouching.Start.Should().Be(_start);
            Assert.Greater(_testData.InsideStartTouching.End, _end);
            Assert.IsTrue(_testData.Reference.HasInside(((ITimePeriod)_testData.InsideStartTouching).Start));
            Assert.IsFalse(_testData.Reference.HasInside(((ITimePeriod)_testData.InsideStartTouching).End));
            _testData.Reference.GetRelation(_testData.InsideStartTouching).Should().Be(PeriodRelation.InsideStartTouching);

            // enclosing start touching
            Assert.IsTrue(_testData.EnclosingStartTouching.IsReadOnly);
            _testData.EnclosingStartTouching.Start.Should().Be(_start);
            Assert.Less(_testData.EnclosingStartTouching.End, _end);
            Assert.IsTrue(_testData.Reference.HasInside(((ITimePeriod)_testData.EnclosingStartTouching).Start));
            Assert.IsTrue(_testData.Reference.HasInside(((ITimePeriod)_testData.EnclosingStartTouching).End));
            _testData.Reference.GetRelation(_testData.EnclosingStartTouching).Should().Be(PeriodRelation.EnclosingStartTouching);

            // enclosing
            Assert.IsTrue(_testData.Enclosing.IsReadOnly);
            Assert.Greater(_testData.Enclosing.Start, _start);
            Assert.Less(_testData.Enclosing.End, _end);
            Assert.IsTrue(_testData.Reference.HasInside(((ITimePeriod)_testData.Enclosing).Start));
            Assert.IsTrue(_testData.Reference.HasInside(((ITimePeriod)_testData.Enclosing).End));
            _testData.Reference.GetRelation(_testData.Enclosing).Should().Be(PeriodRelation.Enclosing);

            // enclosing end touching
            Assert.IsTrue(_testData.EnclosingEndTouching.IsReadOnly);
            Assert.Greater(_testData.EnclosingEndTouching.Start, _start);
            Assert.AreEqual(_testData.EnclosingEndTouching.End, _end);
            Assert.IsTrue(_testData.Reference.HasInside(((ITimePeriod)_testData.EnclosingEndTouching).Start));
            Assert.IsTrue(_testData.Reference.HasInside(((ITimePeriod)_testData.EnclosingEndTouching).End));
            _testData.Reference.GetRelation(_testData.EnclosingEndTouching).Should().Be(PeriodRelation.EnclosingEndTouching);

            // exact match
            Assert.IsTrue(_testData.ExactMatch.IsReadOnly);
            _testData.ExactMatch.Start.Should().Be(_start);
            _testData.ExactMatch.End.Should().Be(_end);
            Assert.IsTrue(_testData.Reference.Equals(_testData.ExactMatch));
            Assert.IsTrue(_testData.Reference.HasInside(((ITimePeriod)_testData.ExactMatch).Start));
            Assert.IsTrue(_testData.Reference.HasInside(((ITimePeriod)_testData.ExactMatch).End));
            _testData.Reference.GetRelation(_testData.ExactMatch).Should().Be(PeriodRelation.ExactMatch);

            // inside
            Assert.IsTrue(_testData.Inside.IsReadOnly);
            Assert.Less(_testData.Inside.Start, _start);
            Assert.Greater(_testData.Inside.End, _end);
            Assert.IsFalse(_testData.Reference.HasInside(((ITimePeriod)_testData.Inside).Start));
            Assert.IsFalse(_testData.Reference.HasInside(((ITimePeriod)_testData.Inside).End));
            _testData.Reference.GetRelation(_testData.Inside).Should().Be(PeriodRelation.Inside);

            // inside end touching
            Assert.IsTrue(_testData.InsideEndTouching.IsReadOnly);
            Assert.Less(_testData.InsideEndTouching.Start, _start);
            Assert.AreEqual(_testData.InsideEndTouching.End, _end);
            Assert.IsFalse(_testData.Reference.HasInside(((ITimePeriod)_testData.InsideEndTouching).Start));
            Assert.IsTrue(_testData.Reference.HasInside(((ITimePeriod)_testData.InsideEndTouching).End));
            _testData.Reference.GetRelation(_testData.InsideEndTouching).Should().Be(PeriodRelation.InsideEndTouching);

            // end inside
            Assert.IsTrue(_testData.EndInside.IsReadOnly);
            Assert.Greater(_testData.EndInside.Start, _start);
            Assert.Less(_testData.EndInside.Start, _end);
            Assert.Greater(_testData.EndInside.End, _end);
            Assert.IsTrue(_testData.Reference.HasInside(((ITimePeriod)_testData.EndInside).Start));
            Assert.IsFalse(_testData.Reference.HasInside(((ITimePeriod)_testData.EndInside).End));
            _testData.Reference.GetRelation(_testData.EndInside).Should().Be(PeriodRelation.EndInside);

            // end touching
            Assert.IsTrue(_testData.EndTouching.IsReadOnly);
            Assert.AreEqual(_testData.EndTouching.Start, _end);
            Assert.Greater(_testData.EndTouching.End, _end);
            Assert.IsTrue(_testData.Reference.HasInside(((ITimePeriod)_testData.EndTouching).Start));
            Assert.IsFalse(_testData.Reference.HasInside(((ITimePeriod)_testData.EndTouching).End));
            _testData.Reference.GetRelation(_testData.EndTouching).Should().Be(PeriodRelation.EndTouching);

            // before
            Assert.IsTrue(_testData.Before.IsReadOnly);
            Assert.Greater(_testData.Before.Start, _testData.Reference.End);
            Assert.Greater(_testData.Before.End, _testData.Reference.End);
            Assert.IsFalse(_testData.Reference.HasInside(((ITimePeriod)_testData.Before).Start));
            Assert.IsFalse(_testData.Reference.HasInside(((ITimePeriod)_testData.Before).End));
            _testData.Reference.GetRelation(_testData.Before).Should().Be(PeriodRelation.Before);
        }

        [Test]
        public void ResetTest() {
            var timeRange = new TimeRange(_start, _end);

            timeRange.Start.Should().Be(_start);
            timeRange.HasStart.Should().Be.True();
            timeRange.End.Should().Be(_end);
            timeRange.HasEnd.Should().Be.True();

            timeRange.StartAsNullable.Should().Have.Value();
            timeRange.EndAsNullable.Should().Have.Value();

            timeRange.Reset();

            timeRange.Start.Should().Be(TimeSpec.MinPeriodTime);
            timeRange.HasStart.Should().Be.False();
            timeRange.End.Should().Be(TimeSpec.MaxPeriodTime);
            timeRange.HasEnd.Should().Be.False();

            timeRange.StartAsNullable.Should().Not.Have.Value();
            timeRange.EndAsNullable.Should().Not.Have.Value();
        }

        [Test]
        public void EqualsTest() {
            var timeRange1 = new TimeRange(_start, _end);
            var timeRange2 = new TimeRange(_start, _end);
            var timeRange3 = new TimeRange(_start.AddMilliseconds(-1), _end.AddMilliseconds(1));

            timeRange1.Equals(timeRange2).Should().Be.True();
            timeRange1.Equals(timeRange3).Should().Be.False();
            timeRange2.Equals(timeRange1).Should().Be.True();
            timeRange2.Equals(timeRange3).Should().Be.False();
        }
    }
}