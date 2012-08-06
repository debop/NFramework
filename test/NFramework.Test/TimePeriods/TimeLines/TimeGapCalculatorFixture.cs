using System;
using System.Linq;
using NSoft.NFramework.TimePeriods.TimeRanges;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.TimePeriods.TimeLines {
    [TestFixture]
    public class TimeGapCalculatorFixture : TimePeriodFixtureBase {
        [Test]
        public void NoPeriodsTest() {
            var limits = new TimeRange(new DateTime(2011, 3, 1), new DateTime(2011, 3, 5));
            var gapCalculator = new TimeGapCalculator<TimeRange>();

            var gaps = gapCalculator.GetGaps(new TimePeriodCollection(), limits);

            gaps.Count.Should().Be(1);
            gaps[0].IsSamePeriod(limits).Should().Be.True();
        }

        [Test]
        public void PeriodEqualsLimitsTest() {
            var limits = new TimeRange(new DateTime(2011, 3, 1), new DateTime(2011, 3, 5));
            var gapCalculator = new TimeGapCalculator<TimeRange>();

            var excludePeriods = new TimePeriodCollection { limits };

            var gaps = gapCalculator.GetGaps(excludePeriods, limits);

            gaps.Count.Should().Be(0);
        }

        [Test]
        public void PeriodLargerThanLimitsTest() {
            var limits = new TimeRange(new DateTime(2011, 3, 1), new DateTime(2011, 3, 5));
            var gapCalculator = new TimeGapCalculator<TimeRange>();

            var excludePeriods = new TimePeriodCollection
                                 {
                                     new TimeRange(new DateTime(2011, 2, 1), new DateTime(2011, 4, 1))
                                 };

            var gaps = gapCalculator.GetGaps(excludePeriods, limits);

            gaps.Count.Should().Be(0);
        }

        [Test]
        public void PeriodOutsideLimitsTest() {
            var limits = new TimeRange(new DateTime(2011, 3, 1), new DateTime(2011, 3, 5));
            var gapCalculator = new TimeGapCalculator<TimeRange>();

            var excludePeriods = new TimePeriodCollection
                                 {
                                     new TimeRange(new DateTime(2011, 2, 1), new DateTime(2011, 2, 15)),
                                     new TimeRange(new DateTime(2011, 4, 1), new DateTime(2011, 4, 15))
                                 };

            var gaps = gapCalculator.GetGaps(excludePeriods, limits);

            gaps.Count.Should().Be(1);
            gaps[0].IsSamePeriod(limits).Should().Be.True();
        }

        [Test]
        public void PeriodOutsideTouchingLimitsTest() {
            var limits = new MonthRange(2011, 3); // new TimeRange(new DateTime(2011, 3, 1), new DateTime(2011, 3, 31));
            var gapCalculator = new TimeGapCalculator<TimeRange>();

            var excludePeriods = new TimePeriodCollection
                                 {
                                     new TimeRange(new DateTime(2011, 2, 1), new DateTime(2011, 3, 5)),
                                     new TimeRange(new DateTime(2011, 3, 20), new DateTime(2011, 4, 15))
                                 };

            var gaps = gapCalculator.GetGaps(excludePeriods, limits);

            gaps.Count.Should().Be(1);
            gaps[0].IsSamePeriod(new TimeRange(new DateTime(2011, 3, 5), new DateTime(2011, 3, 20))).Should().Be.True();
        }

        [Test]
        public void SimleGapsTest() {
            var limits = new TimeRange(new DateTime(2011, 3, 1), new DateTime(2011, 3, 20));
            var gapCalculator = new TimeGapCalculator<TimeRange>();


            var excludeRange = new TimeRange(new DateTime(2011, 3, 10), new DateTime(2011, 3, 15));
            var excludePeriods = new TimePeriodCollection { excludeRange };

            var gaps = gapCalculator.GetGaps(excludePeriods, limits);

            gaps.Count.Should().Be(2);
            gaps[0].IsSamePeriod(new TimeRange(limits.Start, excludeRange.Start)).Should().Be.True();
            gaps[1].IsSamePeriod(new TimeRange(excludeRange.End, limits.End)).Should().Be.True();
        }

        [Test]
        public void PeriodTouchingLimitsStartTest() {
            var limits = new TimeRange(new DateTime(2011, 3, 1), new DateTime(2011, 3, 20));
            var gapCalculator = new TimeGapCalculator<TimeRange>();

            var excludePeriods = new TimePeriodCollection
                                 {
                                     new TimeRange(new DateTime(2011, 3, 1), new DateTime(2011, 3, 10))
                                 };

            var gaps = gapCalculator.GetGaps(excludePeriods, limits);

            gaps.Count.Should().Be(1);
            gaps[0].IsSamePeriod(new TimeRange(new DateTime(2011, 3, 10), new DateTime(2011, 3, 20))).Should().Be.True();
        }

        [Test]
        public void PeriodTouchingLimitsEndTest() {
            var limits = new TimeRange(new DateTime(2011, 3, 1), new DateTime(2011, 3, 20));
            var gapCalculator = new TimeGapCalculator<TimeRange>();

            var excludePeriods = new TimePeriodCollection
                                 {
                                     new TimeRange(new DateTime(2011, 3, 10), new DateTime(2011, 3, 20))
                                 };

            var gaps = gapCalculator.GetGaps(excludePeriods, limits);

            gaps.Count.Should().Be(1);
            gaps[0].IsSamePeriod(new TimeRange(new DateTime(2011, 3, 1), new DateTime(2011, 3, 10))).Should().Be.True();
        }

        [Test]
        public void MomentPeriodTest() {
            var limits = new TimeRange(new DateTime(2011, 3, 1), new DateTime(2011, 3, 20));
            var gapCalculator = new TimeGapCalculator<TimeRange>();

            var excludePeriods = new TimePeriodCollection
                                 {
                                     new TimeRange(new DateTime(2011, 3, 10), new DateTime(2011, 3, 10))
                                 };

            //! Gap 검사 시에 Moment는 제외된다!!!

            var gaps = gapCalculator.GetGaps(excludePeriods, limits);

            gaps.Count.Should().Be(1);
            gaps[0].IsSamePeriod(limits).Should().Be.True();
        }

        [Test]
        public void TouchingPeriodsTest() {
            var limits = new TimeRange(new DateTime(2011, 3, 29), new DateTime(2011, 4, 1));
            var gapCalculator = new TimeGapCalculator<TimeRange>();

            var excludePeriods = new TimePeriodCollection
                                 {
                                     new TimeRange(new DateTime(2011, 3, 30, 00, 00, 0), new DateTime(2011, 3, 30, 08, 30, 0)),
                                     new TimeRange(new DateTime(2011, 3, 30, 08, 30, 0), new DateTime(2011, 3, 30, 12, 00, 0)),
                                     new TimeRange(new DateTime(2011, 3, 30, 10, 00, 0), new DateTime(2011, 3, 31, 00, 00, 0))
                                 };

            ITimePeriodCollection gaps = gapCalculator.GetGaps(excludePeriods, limits);

            gaps.Count.Should().Be(2);
            Assert.IsTrue(gaps[0].IsSamePeriod(new TimeRange(new DateTime(2011, 3, 29), new DateTime(2011, 3, 30))));
            Assert.IsTrue(gaps[1].IsSamePeriod(new TimeRange(new DateTime(2011, 3, 31), new DateTime(2011, 4, 01))));
        }

        [Test]
        public void OverlappingPeriods1Test() {
            var limits = new TimeRange(new DateTime(2011, 3, 29), new DateTime(2011, 4, 1));
            var gapCalculator = new TimeGapCalculator<TimeRange>();

            // 어쨌든 3/30 ~ 3/31 
            var excludePeriods = new TimePeriodCollection
                                 {
                                     new TimeRange(new DateTime(2011, 3, 30, 00, 00, 0), new DateTime(2011, 3, 31, 00, 00, 0)),
                                     new TimeRange(new DateTime(2011, 3, 30, 00, 00, 0), new DateTime(2011, 3, 30, 12, 00, 0)),
                                     new TimeRange(new DateTime(2011, 3, 30, 12, 00, 0), new DateTime(2011, 3, 31, 00, 00, 0))
                                 };

            ITimePeriodCollection gaps = gapCalculator.GetGaps(excludePeriods, limits);

            gaps.Count.Should().Be(2);
            Assert.IsTrue(gaps[0].IsSamePeriod(new TimeRange(new DateTime(2011, 3, 29), new DateTime(2011, 3, 30))));
            Assert.IsTrue(gaps[1].IsSamePeriod(new TimeRange(new DateTime(2011, 3, 31), new DateTime(2011, 4, 01))));
        }

        [Test]
        public void OverlappingPeriods2Test() {
            var limits = new TimeRange(new DateTime(2011, 3, 29), new DateTime(2011, 4, 1));
            var gapCalculator = new TimeGapCalculator<TimeRange>();

            var excludePeriods = new TimePeriodCollection
                                 {
                                     new TimeRange(new DateTime(2011, 3, 30, 00, 00, 0), new DateTime(2011, 3, 31, 00, 00, 0)),
                                     new TimeRange(new DateTime(2011, 3, 30, 00, 00, 0), new DateTime(2011, 3, 30, 06, 30, 0)),
                                     new TimeRange(new DateTime(2011, 3, 30, 08, 30, 0), new DateTime(2011, 3, 30, 12, 00, 0)),
                                     new TimeRange(new DateTime(2011, 3, 30, 22, 30, 0), new DateTime(2011, 3, 31, 00, 00, 0))
                                 };

            var gaps = gapCalculator.GetGaps(excludePeriods, limits);

            gaps.Count.Should().Be(2);
            Assert.IsTrue(gaps[0].IsSamePeriod(new TimeRange(new DateTime(2011, 3, 29), new DateTime(2011, 3, 30))));
            Assert.IsTrue(gaps[1].IsSamePeriod(new TimeRange(new DateTime(2011, 3, 31), new DateTime(2011, 4, 01))));
        }

        [Test]
        public void OverlappingPeriods3Test() {
            var limits = new TimeRange(new DateTime(2011, 3, 29), new DateTime(2011, 4, 1));
            var gapCalculator = new TimeGapCalculator<TimeRange>();

            var excludePeriods = new TimePeriodCollection
                                 {
                                     new TimeRange(new DateTime(2011, 3, 30, 00, 00, 0), new DateTime(2011, 3, 31, 00, 00, 0)),
                                     new TimeRange(new DateTime(2011, 3, 30, 00, 00, 0), new DateTime(2011, 3, 31, 00, 00, 0))
                                 };

            ITimePeriodCollection gaps = gapCalculator.GetGaps(excludePeriods, limits);

            gaps.Count.Should().Be(2);
            Assert.IsTrue(gaps[0].IsSamePeriod(new TimeRange(new DateTime(2011, 3, 29), new DateTime(2011, 3, 30))));
            Assert.IsTrue(gaps[1].IsSamePeriod(new TimeRange(new DateTime(2011, 3, 31), new DateTime(2011, 4, 01))));
        }

        [Test]
        public void GetGapTest() {
            var now = ClockProxy.Clock.Now;
            var schoolDay = new SchoolDay(now);

            var gapCalculator = new TimeGapCalculator<TimeRange>();

            var excludePeriods = new TimePeriodCollection();
            excludePeriods.AddAll(schoolDay);

            gapCalculator.GetGaps(excludePeriods).Count.Should().Be(0);
            gapCalculator.GetGaps(excludePeriods, schoolDay).Count.Should().Be(0);

            excludePeriods.Clear();
            excludePeriods.Add(schoolDay.Lesson1);
            excludePeriods.Add(schoolDay.Lesson2);
            excludePeriods.Add(schoolDay.Lesson3);
            excludePeriods.Add(schoolDay.Lesson4);

            var gaps2 = gapCalculator.GetGaps(excludePeriods);
            gaps2.Count.Should().Be(3);
            gaps2[0].IsSamePeriod(schoolDay.Break1).Should().Be.True();
            gaps2[1].IsSamePeriod(schoolDay.Break2).Should().Be.True();
            gaps2[2].IsSamePeriod(schoolDay.Break3).Should().Be.True();

            var testRange3 = new TimeRange(schoolDay.Lesson1.Start, schoolDay.Lesson4.End);
            var gaps3 = gapCalculator.GetGaps(excludePeriods, testRange3);
            gaps3.Count.Should().Be(3);
            gaps3[0].IsSamePeriod(schoolDay.Break1).Should().Be.True();
            gaps3[1].IsSamePeriod(schoolDay.Break2).Should().Be.True();
            gaps3[2].IsSamePeriod(schoolDay.Break3).Should().Be.True();

            var testRange4 = new TimeRange(schoolDay.Start.AddHours(-1), schoolDay.End.AddHours(1));
            var gaps4 = gapCalculator.GetGaps(excludePeriods, testRange4);

            gaps4.Count.Should().Be(5);
            gaps4[0].IsSamePeriod(new TimeRange(testRange4.Start, schoolDay.Start)).Should().Be.True();
            gaps4[1].IsSamePeriod(schoolDay.Break1).Should().Be.True();
            gaps4[2].IsSamePeriod(schoolDay.Break2).Should().Be.True();
            gaps4[3].IsSamePeriod(schoolDay.Break3).Should().Be.True();
            gaps4[4].IsSamePeriod(new TimeRange(testRange4.End, schoolDay.End)).Should().Be.True();


            excludePeriods.Clear();
            excludePeriods.Add(schoolDay.Lesson1);
            var gaps8 = gapCalculator.GetGaps(excludePeriods, schoolDay.Lesson1);

            gaps8.Count.Should().Be(0);


            excludePeriods.Clear();
            excludePeriods.Add(schoolDay.Lesson1);
            var testRange9 = new TimeRange(schoolDay.Lesson1.Start.Subtract(new TimeSpan(1)), schoolDay.Lesson1.End.Add(new TimeSpan(1)));
            var gaps9 = gapCalculator.GetGaps(excludePeriods, testRange9);

            gaps9.Count.Should().Be(2);
            gaps9[0].Duration.Should().Be(TimeSpan.FromTicks(1));
            gaps9[1].Duration.Should().Be(TimeSpan.FromTicks(1));
        }

        [Test]
        public void CalendarGetGapTest() {
            // simmulation of some reservations
            var periods = new TimePeriodCollection
                          {
                              new DayRangeCollection(2011, 3, 7, 2),
                              new DayRangeCollection(2011, 3, 16, 2)
                          };

            // the overall search range
            var limits = new CalendarTimeRange(new DateTime(2011, 3, 4), new DateTime(2011, 3, 21));
            var days = new DayRangeCollection(limits.Start, limits.Duration.Days + 1);

            // limits의 내부이고, 주말인 DayRange를 추가합니다.
            var excludeDays = days.GetDays().Where(day => limits.HasInside(day) && day.DayOfWeek.IsWeekEnd());
            periods.AddAll(excludeDays.Cast<ITimePeriod>());

            var gapCalculator = new TimeGapCalculator<TimeRange>(new TimeCalendar());
            var gaps = gapCalculator.GetGaps(periods, limits);

            gaps.Count.Should().Be(4);
            gaps[0].IsSamePeriod(new TimeRange(new DateTime(2011, 3, 4), DurationUtil.Days(1))).Should().Be.True();
            gaps[1].IsSamePeriod(new TimeRange(new DateTime(2011, 3, 9), DurationUtil.Days(3))).Should().Be.True();
            gaps[2].IsSamePeriod(new TimeRange(new DateTime(2011, 3, 14), DurationUtil.Days(2))).Should().Be.True();
            gaps[3].IsSamePeriod(new TimeRange(new DateTime(2011, 3, 18), DurationUtil.Days(1))).Should().Be.True();
        }
    }
}