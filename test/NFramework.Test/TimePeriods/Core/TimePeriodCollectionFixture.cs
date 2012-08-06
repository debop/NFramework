using System;
using System.Linq;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.TimePeriods.Core {
    [TestFixture]
    public class TimePeriodCollectionFixture : TimePeriodFixtureBase {
        private readonly TimeSpan _durationHour = DurationUtil.Hour;
        private readonly DateTime _startTestTime;
        private readonly DateTime _endTestTime;

        private readonly TimeSpan _offsetTestDuration = DurationUtil.Millisecond;
        private readonly TimeRangePeriodRelationTestData _timeRangeTestData;

        public TimePeriodCollectionFixture() {
            _startTestTime = ClockProxy.Clock.Now;
            _endTestTime = _startTestTime.Add(_durationHour);
            _timeRangeTestData = new TimeRangePeriodRelationTestData(_startTestTime, _endTestTime, _offsetTestDuration);
        }

        [Test]
        public void DefaultConstructorTest() {
            var timePeriods = new TimePeriodCollection();

            timePeriods.Count.Should().Be(0);
            timePeriods.HasStart.Should().Be.False();
            timePeriods.HasEnd.Should().Be.False();
            timePeriods.IsReadOnly.Should().Be.False();
        }

        [Test]
        public void CopyConstructorTest() {
            var timePeriods = new TimePeriodCollection(_timeRangeTestData.AllPeriods);

            timePeriods.Count.Should().Be(_timeRangeTestData.AllPeriods.Count);
            timePeriods.HasStart.Should().Be.True();
            timePeriods.HasEnd.Should().Be.True();
            timePeriods.IsReadOnly.Should().Be.False();
        }

        [Test]
        public void CountTest() {
            var timePeriods = new TimePeriodCollection();
            timePeriods.Count.Should().Be(0);

            timePeriods.AddAll(_timeRangeTestData.AllPeriods);
            timePeriods.Count.Should().Be(_timeRangeTestData.AllPeriods.Count);

            timePeriods.Clear();
            timePeriods.Count.Should().Be(0);
        }

        [Test]
        public void ItemIndexTest() {
            var timePeriods = new TimePeriodCollection();
            var schoolDay = new SchoolDay();
            timePeriods.AddAll(schoolDay);

            Assert.AreEqual(timePeriods[0], schoolDay.Lesson1);
            Assert.AreEqual(timePeriods[1], schoolDay.Break1);
            Assert.AreEqual(timePeriods[2], schoolDay.Lesson2);
            Assert.AreEqual(timePeriods[3], schoolDay.Break2);
            Assert.AreEqual(timePeriods[4], schoolDay.Lesson3);
            Assert.AreEqual(timePeriods[5], schoolDay.Break3);
            Assert.AreEqual(timePeriods[6], schoolDay.Lesson4);
        }

        [Test]
        public void IsAnytimeTest() {
            var now = ClockProxy.Clock.Now;
            var timePeriods = new TimePeriodCollection();
            Assert.IsTrue(timePeriods.IsAnytime);

            timePeriods.Add(TimeRange.Anytime);
            Assert.IsTrue(timePeriods.IsAnytime);

            timePeriods.Clear();
            Assert.IsTrue(timePeriods.IsAnytime);

            timePeriods.Add(new TimeRange(TimeSpec.MinPeriodTime, now));
            Assert.IsFalse(timePeriods.IsAnytime);

            timePeriods.Add(new TimeRange(now, TimeSpec.MaxPeriodTime));
            Assert.IsTrue(timePeriods.IsAnytime);

            timePeriods.Clear();
            Assert.IsTrue(timePeriods.IsAnytime);
        }

        [Test]
        public void IsMomentTest() {
            var now = ClockProxy.Clock.Now;
            var timePeriods = new TimePeriodCollection();
            Assert.IsFalse(timePeriods.IsMoment);

            timePeriods.Add(TimeRange.Anytime);
            Assert.IsFalse(timePeriods.IsMoment);

            timePeriods.Clear();
            Assert.IsFalse(timePeriods.IsMoment);

            timePeriods.Add(new TimeRange(now));
            Assert.IsTrue(timePeriods.IsMoment);

            timePeriods.Add(new TimeRange(now));
            Assert.IsTrue(timePeriods.IsMoment);

            timePeriods.Clear();
            Assert.IsTrue(timePeriods.IsAnytime);
        }

        [Test]
        public void HasStartTest() {
            TimePeriodCollection timePeriods = new TimePeriodCollection();
            Assert.IsFalse(timePeriods.HasStart);

            timePeriods.Add(new TimeBlock(TimeSpec.MinPeriodTime, DurationUtil.Hour));
            Assert.IsFalse(timePeriods.HasStart);

            timePeriods.Clear();
            timePeriods.Add(new TimeBlock(ClockProxy.Clock.Now, DurationUtil.Hour));
            Assert.IsTrue(timePeriods.HasStart);
        }

        [Test]
        public void StartTest() {
            var now = ClockProxy.Clock.Now;
            var timePeriods = new TimePeriodCollection();
            Assert.AreEqual(timePeriods.Start, TimeSpec.MinPeriodTime);

            timePeriods.Add(new TimeBlock(now, DurationUtil.Hour));
            Assert.AreEqual(timePeriods.Start, now);
            Assert.AreEqual(timePeriods.End, now.AddHours(1));

            timePeriods.Clear();
            Assert.AreEqual(timePeriods.Start, TimeSpec.MinPeriodTime);
        }

        [Test]
        public void StartMoveTest() {
            var now = ClockProxy.Clock.Now;
            var schoolDay = new SchoolDay(now);
            var timePeriods = new TimePeriodCollection(schoolDay);

            timePeriods.Start = now.AddHours(0);
            Assert.AreEqual(timePeriods.Start, now);
            timePeriods.Start = now.AddHours(1);
            Assert.AreEqual(timePeriods.Start, now.AddHours(1));
            timePeriods.Start = now.AddHours(-1);
            Assert.AreEqual(timePeriods.Start, now.AddHours(-1));
        }

        [Test]
        public void SortByStartTest() {
            var schoolDay = new SchoolDay();
            var timePeriods = new TimePeriodCollection
                              {
                                  schoolDay.Lesson4,
                                  schoolDay.Break3,
                                  schoolDay.Lesson3,
                                  schoolDay.Break2,
                                  schoolDay.Lesson2,
                                  schoolDay.Break1,
                                  schoolDay.Lesson1
                              };

            timePeriods.SortByStart();

            timePeriods[0].Should().Be(schoolDay.Lesson1);
            timePeriods[1].Should().Be(schoolDay.Break1);
            timePeriods[2].Should().Be(schoolDay.Lesson2);
            timePeriods[3].Should().Be(schoolDay.Break2);
            timePeriods[4].Should().Be(schoolDay.Lesson3);
            timePeriods[5].Should().Be(schoolDay.Break3);
            timePeriods[6].Should().Be(schoolDay.Lesson4);
        }

        [Test]
        public void SortByEndTest() {
            DateTime now = ClockProxy.Clock.Now;
            SchoolDay schoolDay = new SchoolDay(now);
            TimePeriodCollection timePeriods = new TimePeriodCollection();

            timePeriods.AddAll(schoolDay);

            timePeriods.SortByEnd();

            timePeriods[0].Should().Be(schoolDay.Lesson4);
            timePeriods[1].Should().Be(schoolDay.Break3);
            timePeriods[2].Should().Be(schoolDay.Lesson3);
            timePeriods[3].Should().Be(schoolDay.Break2);
            timePeriods[4].Should().Be(schoolDay.Lesson2);
            timePeriods[5].Should().Be(schoolDay.Break1);
            timePeriods[6].Should().Be(schoolDay.Lesson1);
        }

        [Test]
        public void InsidePeriodsTimePeriodTest() {
            var now = ClockProxy.Clock.Now;
            var timeRange1 = new TimeRange(new DateTime(now.Year, now.Month, 8), new DateTime(now.Year, now.Month, 18));
            var timeRange2 = new TimeRange(new DateTime(now.Year, now.Month, 10), new DateTime(now.Year, now.Month, 11));
            var timeRange3 = new TimeRange(new DateTime(now.Year, now.Month, 13), new DateTime(now.Year, now.Month, 15));
            var timeRange4 = new TimeRange(new DateTime(now.Year, now.Month, 9), new DateTime(now.Year, now.Month, 13));
            var timeRange5 = new TimeRange(new DateTime(now.Year, now.Month, 15), new DateTime(now.Year, now.Month, 17));

            TimePeriodCollection timePeriods = new TimePeriodCollection { timeRange1, timeRange2, timeRange3, timeRange4, timeRange5 };

            timePeriods.InsidePeriods(timeRange1).Count().Should().Be(5);
            timePeriods.InsidePeriods(timeRange2).Count().Should().Be(1);
            timePeriods.InsidePeriods(timeRange3).Count().Should().Be(1);
            timePeriods.InsidePeriods(timeRange4).Count().Should().Be(2);
            timePeriods.InsidePeriods(timeRange5).Count().Should().Be(1);

            var test1 = timeRange1.Copy(new TimeSpan(100, 0, 0, 0).Negate());
            var insidePeriods1 = timePeriods.InsidePeriods(test1).ToList();
            insidePeriods1.Count.Should().Be(0);

            var test2 = timeRange1.Copy(new TimeSpan(100, 0, 0, 0));
            var insidePeriods2 = timePeriods.InsidePeriods(test2).ToList();
            insidePeriods2.Count.Should().Be(0);

            var test3 = new TimeRange(new DateTime(now.Year, now.Month, 9), new DateTime(now.Year, now.Month, 11));
            var insidePeriods3 = timePeriods.InsidePeriods(test3).ToList();
            insidePeriods3.Count.Should().Be(1);

            var test4 = new TimeRange(new DateTime(now.Year, now.Month, 14), new DateTime(now.Year, now.Month, 17));
            var insidePeriods4 = timePeriods.InsidePeriods(test4).ToList();
            insidePeriods4.Count.Should().Be(1);
        }

        [Test]
        public void OverlapPeriodsTest() {
            var now = ClockProxy.Clock.Now;
            var timeRange1 = new TimeRange(new DateTime(now.Year, now.Month, 8), new DateTime(now.Year, now.Month, 18));
            var timeRange2 = new TimeRange(new DateTime(now.Year, now.Month, 10), new DateTime(now.Year, now.Month, 11));
            var timeRange3 = new TimeRange(new DateTime(now.Year, now.Month, 13), new DateTime(now.Year, now.Month, 15));
            var timeRange4 = new TimeRange(new DateTime(now.Year, now.Month, 9), new DateTime(now.Year, now.Month, 13));
            var timeRange5 = new TimeRange(new DateTime(now.Year, now.Month, 15), new DateTime(now.Year, now.Month, 17));

            var timePeriods = new TimePeriodCollection
                              {
                                  timeRange1,
                                  timeRange2,
                                  timeRange3,
                                  timeRange4,
                                  timeRange5
                              };

            timePeriods.OverlapPeriods(timeRange1).Count().Should().Be(5);
            timePeriods.OverlapPeriods(timeRange2).Count().Should().Be(3);
            timePeriods.OverlapPeriods(timeRange3).Count().Should().Be(2);
            timePeriods.OverlapPeriods(timeRange4).Count().Should().Be(3);
            timePeriods.OverlapPeriods(timeRange5).Count().Should().Be(2);

            var test1 = timeRange1.Copy(new TimeSpan(100, 0, 0, 0).Negate());
            var insidePeriods1 = timePeriods.OverlapPeriods(test1).ToList();
            insidePeriods1.Count.Should().Be(0);

            var test2 = timeRange1.Copy(new TimeSpan(100, 0, 0, 0));
            var insidePeriods2 = timePeriods.OverlapPeriods(test2).ToList();
            insidePeriods2.Count.Should().Be(0);

            var test3 = new TimeRange(new DateTime(now.Year, now.Month, 9), new DateTime(now.Year, now.Month, 11));
            var insidePeriods3 = timePeriods.OverlapPeriods(test3).ToList();
            insidePeriods3.Count.Should().Be(3);

            var test4 = new TimeRange(new DateTime(now.Year, now.Month, 14), new DateTime(now.Year, now.Month, 17));
            var insidePeriods4 = timePeriods.OverlapPeriods(test4).ToList();
            insidePeriods4.Count.Should().Be(3);
        }

        [Test]
        public void IntersectionPeriodsDateTimeTest() {
            var now = ClockProxy.Clock.Now;
            var timeRange1 = new TimeRange(new DateTime(now.Year, now.Month, 8), new DateTime(now.Year, now.Month, 18));
            var timeRange2 = new TimeRange(new DateTime(now.Year, now.Month, 10), new DateTime(now.Year, now.Month, 11));
            var timeRange3 = new TimeRange(new DateTime(now.Year, now.Month, 13), new DateTime(now.Year, now.Month, 15));
            var timeRange4 = new TimeRange(new DateTime(now.Year, now.Month, 9), new DateTime(now.Year, now.Month, 14));
            var timeRange5 = new TimeRange(new DateTime(now.Year, now.Month, 16), new DateTime(now.Year, now.Month, 17));

            TimePeriodCollection timePeriods = new TimePeriodCollection
                                               {
                                                   timeRange1,
                                                   timeRange2,
                                                   timeRange3,
                                                   timeRange4,
                                                   timeRange5
                                               };

            timePeriods.IntersectionPeriods(timeRange1.Start).Count().Should().Be(1);
            timePeriods.IntersectionPeriods(timeRange1.End).Count().Should().Be(1);

            timePeriods.IntersectionPeriods(timeRange2.Start).Count().Should().Be(3);
            timePeriods.IntersectionPeriods(timeRange2.End).Count().Should().Be(3);

            timePeriods.IntersectionPeriods(timeRange3.Start).Count().Should().Be(3);
            timePeriods.IntersectionPeriods(timeRange3.End).Count().Should().Be(2);

            timePeriods.IntersectionPeriods(timeRange4.Start).Count().Should().Be(2);
            timePeriods.IntersectionPeriods(timeRange4.End).Count().Should().Be(3);

            timePeriods.IntersectionPeriods(timeRange5.Start).Count().Should().Be(2);
            timePeriods.IntersectionPeriods(timeRange5.End).Count().Should().Be(2);

            var test1 = timeRange1.Start.AddMilliseconds(-1);
            var insidePeriods1 = timePeriods.IntersectionPeriods(test1).ToList();
            insidePeriods1.Count.Should().Be(0);

            DateTime test2 = timeRange1.End.AddMilliseconds(1);
            var insidePeriods2 = timePeriods.IntersectionPeriods(test2).ToList();
            insidePeriods2.Count.Should().Be(0);

            DateTime test3 = new DateTime(now.Year, now.Month, 12);
            var insidePeriods3 = timePeriods.IntersectionPeriods(test3).ToList();
            insidePeriods3.Count.Should().Be(2);

            DateTime test4 = new DateTime(now.Year, now.Month, 14);
            var insidePeriods4 = timePeriods.IntersectionPeriods(test4).ToList();
            insidePeriods4.Count.Should().Be(3);
        }

        [Test]
        public void RelationPeriodsTest() {
            var now = ClockProxy.Clock.Now;
            var timeRange1 = new TimeRange(new DateTime(now.Year, now.Month, 8), new DateTime(now.Year, now.Month, 18));
            var timeRange2 = new TimeRange(new DateTime(now.Year, now.Month, 10), new DateTime(now.Year, now.Month, 11));
            var timeRange3 = new TimeRange(new DateTime(now.Year, now.Month, 13), new DateTime(now.Year, now.Month, 15));
            var timeRange4 = new TimeRange(new DateTime(now.Year, now.Month, 9), new DateTime(now.Year, now.Month, 14));
            var timeRange5 = new TimeRange(new DateTime(now.Year, now.Month, 16), new DateTime(now.Year, now.Month, 17));

            TimePeriodCollection timePeriods = new TimePeriodCollection { timeRange1, timeRange2, timeRange3, timeRange4, timeRange5 };

            timePeriods.RelationPeriods(timeRange1, PeriodRelation.ExactMatch).Count().Should().Be(1);
            timePeriods.RelationPeriods(timeRange2, PeriodRelation.ExactMatch).Count().Should().Be(1);
            timePeriods.RelationPeriods(timeRange3, PeriodRelation.ExactMatch).Count().Should().Be(1);
            timePeriods.RelationPeriods(timeRange4, PeriodRelation.ExactMatch).Count().Should().Be(1);
            timePeriods.RelationPeriods(timeRange5, PeriodRelation.ExactMatch).Count().Should().Be(1);

            // all
            Assert.AreEqual(
                timePeriods.RelationPeriods(new TimeRange(new DateTime(now.Year, now.Month, 7), new DateTime(now.Year, now.Month, 19)),
                                            PeriodRelation.Enclosing).Count(), 5);

            // timerange3
            Assert.AreEqual(
                timePeriods.RelationPeriods(
                    new TimeRange(new DateTime(now.Year, now.Month, 11), new DateTime(now.Year, now.Month, 16)),
                    PeriodRelation.Enclosing).Count(), 1);
        }

        [Test]
        public void IntersectionPeriodsTimePeriodTest() {
            var now = ClockProxy.Clock.Now;
            var timeRange1 = new TimeRange(new DateTime(now.Year, now.Month, 8), new DateTime(now.Year, now.Month, 18));
            var timeRange2 = new TimeRange(new DateTime(now.Year, now.Month, 10), new DateTime(now.Year, now.Month, 11));
            var timeRange3 = new TimeRange(new DateTime(now.Year, now.Month, 13), new DateTime(now.Year, now.Month, 15));
            var timeRange4 = new TimeRange(new DateTime(now.Year, now.Month, 9), new DateTime(now.Year, now.Month, 13));
            var timeRange5 = new TimeRange(new DateTime(now.Year, now.Month, 15), new DateTime(now.Year, now.Month, 17));

            var timePeriods = new TimePeriodCollection
                              {
                                  timeRange1,
                                  timeRange2,
                                  timeRange3,
                                  timeRange4,
                                  timeRange5
                              };

            timePeriods.IntersectionPeriods(timeRange1).Count().Should().Be(5);
            timePeriods.IntersectionPeriods(timeRange2).Count().Should().Be(3);
            timePeriods.IntersectionPeriods(timeRange3).Count().Should().Be(4);
            timePeriods.IntersectionPeriods(timeRange4).Count().Should().Be(4);
            timePeriods.IntersectionPeriods(timeRange5).Count().Should().Be(3);

            var test1 = timeRange1.Copy(new TimeSpan(100, 0, 0, 0).Negate());
            var insidePeriods1 = timePeriods.IntersectionPeriods(test1).ToList();
            insidePeriods1.Count.Should().Be(0);

            var test2 = timeRange1.Copy(new TimeSpan(100, 0, 0, 0));
            var insidePeriods2 = timePeriods.IntersectionPeriods(test2).ToList();
            insidePeriods2.Count.Should().Be(0);

            var test3 = new TimeRange(new DateTime(now.Year, now.Month, 9), new DateTime(now.Year, now.Month, 11));
            var insidePeriods3 = timePeriods.IntersectionPeriods(test3).ToList();
            insidePeriods3.Count.Should().Be(3);

            var test4 = new TimeRange(new DateTime(now.Year, now.Month, 14), new DateTime(now.Year, now.Month, 17));
            var insidePeriods4 = timePeriods.IntersectionPeriods(test4).ToList();
            insidePeriods4.Count.Should().Be(3);
        }

        [Test]
        public void HasEndTest() {
            var now = ClockProxy.Clock.Now;
            var timePeriods = new TimePeriodCollection();
            Assert.IsFalse(timePeriods.HasEnd);

            timePeriods.Add(new TimeBlock(DurationUtil.Hour, TimeSpec.MaxPeriodTime));
            Assert.IsFalse(timePeriods.HasEnd);

            timePeriods.Clear();
            timePeriods.Add(new TimeBlock(now, DurationUtil.Hour));
            Assert.IsTrue(timePeriods.HasEnd);
        }

        [Test]
        public void EndTest() {
            var now = ClockProxy.Clock.Now;
            var timePeriods = new TimePeriodCollection();
            timePeriods.End.Should().Be(TimeSpec.MaxPeriodTime);

            timePeriods.Add(new TimeBlock(DurationUtil.Hour, now));
            timePeriods.End.Should().Be(now);

            timePeriods.Clear();
            timePeriods.End.Should().Be(TimeSpec.MaxPeriodTime);
        }

        [Test]
        public void EndMoveTest() {
            var schoolDay = new SchoolDay();
            var timePeriods = new TimePeriodCollection(schoolDay);

            var end = schoolDay.End;

            timePeriods.End = end.AddHours(0);
            timePeriods.End.Should().Be(end);

            timePeriods.End = end.AddHours(1);
            timePeriods.End.Should().Be(end.AddHours(1));

            timePeriods.End = end.AddHours(-1);
            timePeriods.End.Should().Be(end.AddHours(-1));
        }

        [Test]
        public void DurationTest() {
            var timePeriods = new TimePeriodCollection();
            timePeriods.Duration.Should().Be(TimeSpec.MaxPeriodDuration);

            var duration = DurationUtil.Hour;
            timePeriods.Add(new TimeBlock(ClockProxy.Clock.Now, duration));
            timePeriods.Duration.Should().Be(duration);
        }

        [Test]
        public void MoveTest() {
            var now = ClockProxy.Clock.Now;
            var schoolDay = new SchoolDay(now);
            var timePeriods = new TimePeriodCollection(schoolDay);

            var startDate = schoolDay.Start;
            var endDate = schoolDay.End;
            var startDuration = timePeriods.Duration;

            var duration = DurationUtil.Hour;
            timePeriods.Move(duration);

            timePeriods.Start.Should().Be(startDate.Add(duration));
            timePeriods.End.Should().Be(endDate.Add(duration));
            timePeriods.Duration.Should().Be(startDuration);
        }

        [Test]
        public void AddTest() {
            var timePeriods = new TimePeriodCollection();
            timePeriods.Count.Should().Be(0);

            timePeriods.Add(new TimeRange());
            timePeriods.Count.Should().Be(1);

            timePeriods.Add(new TimeRange());
            timePeriods.Count.Should().Be(2);

            timePeriods.Clear();
            timePeriods.Count.Should().Be(0);
        }

        [Test]
        public void ContainsPeriodTest() {
            var now = ClockProxy.Clock.Now;
            var schoolDay = new SchoolDay(now);
            var timePeriods = new TimePeriodCollection(schoolDay);

            var timeRange = new TimeRange(schoolDay.Lesson1.Start, schoolDay.Lesson1.End);
            Assert.IsFalse(timePeriods.Contains(timeRange));
            Assert.IsTrue(timePeriods.ContainsPeriod(timeRange));

            timePeriods.Add(timeRange);
            Assert.IsTrue(timePeriods.Contains(timeRange));
            Assert.IsTrue(timePeriods.ContainsPeriod(timeRange));
        }

        [Test]
        public void AddAllTest() {
            var now = ClockProxy.Clock.Now;
            var schoolDay = new SchoolDay(now);
            var timePeriods = new TimePeriodCollection();

            timePeriods.Count.Should().Be(0);

            timePeriods.AddAll(schoolDay);
            timePeriods.Count.Should().Be(schoolDay.Count);

            timePeriods.Clear();
            timePeriods.Count.Should().Be(0);
        }

        [Test]
        public void InsertTest() {
            var now = ClockProxy.Clock.Now;
            var schoolDay = new SchoolDay(now);
            var timePeriods = new TimePeriodCollection();
            timePeriods.Count.Should().Be(0);

            timePeriods.Add(schoolDay.Lesson1);
            timePeriods.Count.Should().Be(1);
            timePeriods.Add(schoolDay.Lesson3);
            timePeriods.Count.Should().Be(2);
            timePeriods.Add(schoolDay.Lesson4);
            timePeriods.Count.Should().Be(3);

            // between
            timePeriods[1].Should().Be(schoolDay.Lesson3);
            timePeriods.Insert(1, schoolDay.Lesson2);
            timePeriods[1].Should().Be(schoolDay.Lesson2);

            // first
            timePeriods[0].Should().Be(schoolDay.Lesson1);
            timePeriods.Insert(0, schoolDay.Break1);
            timePeriods[0].Should().Be(schoolDay.Break1);

            // last
            timePeriods[timePeriods.Count - 1].Should().Be(schoolDay.Lesson4);
            timePeriods.Insert(timePeriods.Count, schoolDay.Break3);
            timePeriods[timePeriods.Count - 1].Should().Be(schoolDay.Break3);
        }

        [Test]
        public void ContainsTest() {
            var now = ClockProxy.Clock.Now;
            var schoolDay = new SchoolDay(now);
            var timePeriods = new TimePeriodCollection();

            Assert.IsFalse(timePeriods.Contains(schoolDay.Lesson1));
            timePeriods.Add(schoolDay.Lesson1);
            Assert.IsTrue(timePeriods.Contains(schoolDay.Lesson1));
            timePeriods.Remove(schoolDay.Lesson1);
            Assert.IsFalse(timePeriods.Contains(schoolDay.Lesson1));
        }

        [Test]
        public void IndexOfTest() {
            var now = ClockProxy.Clock.Now;
            var schoolDay = new SchoolDay(now);
            var timePeriods = new TimePeriodCollection();

            timePeriods.IndexOf(new TimeRange()).Should().Be(-1);
            timePeriods.IndexOf(new TimeBlock()).Should().Be(-1);

            timePeriods.AddAll(schoolDay);

            timePeriods.IndexOf(schoolDay.Lesson1).Should().Be(0);
            timePeriods.IndexOf(schoolDay.Break1).Should().Be(1);
            timePeriods.IndexOf(schoolDay.Lesson2).Should().Be(2);
            timePeriods.IndexOf(schoolDay.Break2).Should().Be(3);
            timePeriods.IndexOf(schoolDay.Lesson3).Should().Be(4);
            timePeriods.IndexOf(schoolDay.Break3).Should().Be(5);
            timePeriods.IndexOf(schoolDay.Lesson4).Should().Be(6);

            timePeriods.Remove(schoolDay.Lesson1);
            timePeriods.IndexOf(schoolDay.Lesson1).Should().Be(-1);
            timePeriods.IndexOf(schoolDay.Break1).Should().Be(0);
        }

        [Test]
        public void CopyToTest() {
            var now = ClockProxy.Clock.Now;
            var schoolDay = new SchoolDay(now);
            var timePeriods = new TimePeriodCollection(schoolDay);

            var array = new ITimePeriod[schoolDay.Count];
            timePeriods.CopyTo(array, 0);

            array[0].Should().Be(schoolDay.Lesson1);
            array[1].Should().Be(schoolDay.Break1);
            array[2].Should().Be(schoolDay.Lesson2);
            array[3].Should().Be(schoolDay.Break2);
            array[4].Should().Be(schoolDay.Lesson3);
            array[5].Should().Be(schoolDay.Break3);
            array[6].Should().Be(schoolDay.Lesson4);
        }

        [Test]
        public void ClearTest() {
            var timePeriods = new TimePeriodCollection();
            timePeriods.Count.Should().Be(0);
            timePeriods.Clear();
            timePeriods.Count.Should().Be(0);

            timePeriods.AddAll(new SchoolDay());
            timePeriods.Count.Should().Be(7);
            timePeriods.Clear();
            timePeriods.Count.Should().Be(0);
        }

        [Test]
        public void RemoveTest() {
            var now = ClockProxy.Clock.Now;
            var schoolDay = new SchoolDay(now);
            var timePeriods = new TimePeriodCollection();

            Assert.IsFalse(timePeriods.Contains(schoolDay.Lesson1));
            timePeriods.Add(schoolDay.Lesson1);
            Assert.IsTrue(timePeriods.Contains(schoolDay.Lesson1));
            timePeriods.Remove(schoolDay.Lesson1);
            Assert.IsFalse(timePeriods.Contains(schoolDay.Lesson1));
        }

        [Test]
        public void RemoveAtTest() {
            var now = ClockProxy.Clock.Now;
            var schoolDay = new SchoolDay(now);
            var timePeriods = new TimePeriodCollection(schoolDay);

            // inside
            timePeriods[2].Should().Be(schoolDay.Lesson2);
            timePeriods.RemoveAt(2);
            timePeriods[2].Should().Be(schoolDay.Break2);

            // first
            timePeriods[0].Should().Be(schoolDay.Lesson1);
            timePeriods.RemoveAt(0);
            timePeriods[0].Should().Be(schoolDay.Break1);

            // last
            timePeriods[timePeriods.Count - 1].Should().Be(schoolDay.Lesson4);
            timePeriods.RemoveAt(timePeriods.Count - 1);
            timePeriods[timePeriods.Count - 1].Should().Be(schoolDay.Break3);
        }

        [Test]
        public void IsSamePeriodTest() {
            var now = ClockProxy.Clock.Now;
            var schoolDay = new SchoolDay(now);
            var timePeriods = new TimePeriodCollection(schoolDay);

            Assert.IsTrue(timePeriods.IsSamePeriod(timePeriods));
            Assert.IsTrue(timePeriods.IsSamePeriod(schoolDay));

            Assert.IsTrue(schoolDay.IsSamePeriod(schoolDay));
            Assert.IsTrue(schoolDay.IsSamePeriod(timePeriods));

            Assert.IsFalse(timePeriods.IsSamePeriod(TimeBlock.Anytime));
            Assert.IsFalse(schoolDay.IsSamePeriod(TimeBlock.Anytime));

            timePeriods.RemoveAt(0);
            Assert.IsFalse(timePeriods.IsSamePeriod(schoolDay));
        }

        [Test]
        public void HasInsideTest() {
            var now = ClockProxy.Clock.Now;
            var offset = DurationUtil.Second;
            var testData = new TimeRangePeriodRelationTestData(now, now.AddHours(1), offset);
            var timePeriods = new TimePeriodCollection
                              {
                                  testData.Reference
                              };

            Assert.IsFalse(timePeriods.HasInside(testData.Before));
            Assert.IsFalse(timePeriods.HasInside(testData.StartTouching));
            Assert.IsFalse(timePeriods.HasInside(testData.StartInside));
            Assert.IsFalse(timePeriods.HasInside(testData.InsideStartTouching));
            Assert.IsTrue(timePeriods.HasInside(testData.EnclosingStartTouching));
            Assert.IsTrue(timePeriods.HasInside(testData.Enclosing));
            Assert.IsTrue(timePeriods.HasInside(testData.EnclosingEndTouching));
            Assert.IsTrue(timePeriods.HasInside(testData.ExactMatch));
            Assert.IsFalse(timePeriods.HasInside(testData.Inside));
            Assert.IsFalse(timePeriods.HasInside(testData.InsideEndTouching));
            Assert.IsFalse(timePeriods.HasInside(testData.EndInside));
            Assert.IsFalse(timePeriods.HasInside(testData.EndTouching));
            Assert.IsFalse(timePeriods.HasInside(testData.After));
        }

        [Test]
        public void IntersectsWithTest() {
            var now = ClockProxy.Clock.Now;
            var offset = DurationUtil.Second;
            var testData = new TimeRangePeriodRelationTestData(now, now.AddHours(1), offset);
            var timePeriods = new TimePeriodCollection
                              {
                                  testData.Reference
                              };

            Assert.IsFalse(timePeriods.IntersectsWith(testData.Before));
            Assert.IsTrue(timePeriods.IntersectsWith(testData.StartTouching));
            Assert.IsTrue(timePeriods.IntersectsWith(testData.StartInside));
            Assert.IsTrue(timePeriods.IntersectsWith(testData.InsideStartTouching));
            Assert.IsTrue(timePeriods.IntersectsWith(testData.EnclosingStartTouching));
            Assert.IsTrue(timePeriods.IntersectsWith(testData.Enclosing));
            Assert.IsTrue(timePeriods.IntersectsWith(testData.EnclosingEndTouching));
            Assert.IsTrue(timePeriods.IntersectsWith(testData.ExactMatch));
            Assert.IsTrue(timePeriods.IntersectsWith(testData.Inside));
            Assert.IsTrue(timePeriods.IntersectsWith(testData.InsideEndTouching));
            Assert.IsTrue(timePeriods.IntersectsWith(testData.EndInside));
            Assert.IsTrue(timePeriods.IntersectsWith(testData.EndTouching));
            Assert.IsFalse(timePeriods.IntersectsWith(testData.After));
        }

        [Test]
        public void OverlapsWithTest() {
            var now = ClockProxy.Clock.Now;
            var offset = DurationUtil.Second;
            var testData = new TimeRangePeriodRelationTestData(now, now.AddHours(1), offset);
            var timePeriods = new TimePeriodCollection
                              {
                                  testData.Reference
                              };

            Assert.IsFalse(timePeriods.OverlapsWith(testData.Before));
            Assert.IsFalse(timePeriods.OverlapsWith(testData.StartTouching));
            Assert.IsTrue(timePeriods.OverlapsWith(testData.StartInside));
            Assert.IsTrue(timePeriods.OverlapsWith(testData.InsideStartTouching));
            Assert.IsTrue(timePeriods.OverlapsWith(testData.EnclosingStartTouching));
            Assert.IsTrue(timePeriods.OverlapsWith(testData.Enclosing));
            Assert.IsTrue(timePeriods.OverlapsWith(testData.EnclosingEndTouching));
            Assert.IsTrue(timePeriods.OverlapsWith(testData.ExactMatch));
            Assert.IsTrue(timePeriods.OverlapsWith(testData.Inside));
            Assert.IsTrue(timePeriods.OverlapsWith(testData.InsideEndTouching));
            Assert.IsTrue(timePeriods.OverlapsWith(testData.EndInside));
            Assert.IsFalse(timePeriods.OverlapsWith(testData.EndTouching));
            Assert.IsFalse(timePeriods.OverlapsWith(testData.After));
        }

        [Test]
        public void GetRelationTest() {
            var now = ClockProxy.Clock.Now;
            var offset = DurationUtil.Second;
            var testData = new TimeRangePeriodRelationTestData(now, now.AddHours(1), offset);
            var timePeriods = new TimePeriodCollection
                              {
                                  testData.Reference
                              };

            timePeriods.GetRelation(testData.Before).Should().Be(PeriodRelation.Before);
            timePeriods.GetRelation(testData.StartTouching).Should().Be(PeriodRelation.StartTouching);
            timePeriods.GetRelation(testData.StartInside).Should().Be(PeriodRelation.StartInside);
            timePeriods.GetRelation(testData.InsideStartTouching).Should().Be(PeriodRelation.InsideStartTouching);
            timePeriods.GetRelation(testData.EnclosingStartTouching).Should().Be(PeriodRelation.EnclosingStartTouching);
            timePeriods.GetRelation(testData.Enclosing).Should().Be(PeriodRelation.Enclosing);
            timePeriods.GetRelation(testData.EnclosingEndTouching).Should().Be(PeriodRelation.EnclosingEndTouching);
            timePeriods.GetRelation(testData.ExactMatch).Should().Be(PeriodRelation.ExactMatch);
            timePeriods.GetRelation(testData.Inside).Should().Be(PeriodRelation.Inside);
            timePeriods.GetRelation(testData.InsideEndTouching).Should().Be(PeriodRelation.InsideEndTouching);
            timePeriods.GetRelation(testData.EndInside).Should().Be(PeriodRelation.EndInside);
            timePeriods.GetRelation(testData.EndTouching).Should().Be(PeriodRelation.EndTouching);
            timePeriods.GetRelation(testData.After).Should().Be(PeriodRelation.After);
        }
    }
}