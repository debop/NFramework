using System;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.TimePeriods.Core {
    [TestFixture]
    public class TimePeriodCombinerFixture : TimePeriodFixtureBase {
        [Test]
        public void NoPeriodsTest() {
            var periodCombiner = new TimePeriodCombiner<TimeRange>();

            var periods = periodCombiner.CombinePeriods(new TimePeriodCollection());
            periods.Count.Should().Be(0);
        }

        [Test]
        public void SinglePeriodAnytimeTest() {
            var periodCombiner = new TimePeriodCombiner<TimeRange>();
            var periods = periodCombiner.CombinePeriods(new TimePeriodCollection { TimeRange.Anytime });
            periods.Count.Should().Be(1);
            Assert.IsTrue(periods[0].IsSamePeriod(TimeRange.Anytime));
        }

        [Test]
        public void SinglePeriodTest() {
            var period = new TimeRange(new DateTime(2011, 3, 1), new DateTime(2011, 3, 5));
            var periodCombiner = new TimePeriodCombiner<TimeRange>();
            var periods = periodCombiner.CombinePeriods(new TimePeriodCollection { period });
            periods.Count.Should().Be(1);
            Assert.IsTrue(periods[0].IsSamePeriod(period));
        }

        [Test]
        public void MomentTest() {
            var period = new TimeRange(new DateTime(2011, 3, 1), new DateTime(2011, 3, 1));
            var periodCombiner = new TimePeriodCombiner<TimeRange>();
            var periods = periodCombiner.CombinePeriods(new TimePeriodCollection { period });
            periods.Count.Should().Be(1);
            Assert.IsTrue(periods[0].IsSamePeriod(period));
        }

        [Test]
        public void TwoPeriodsTest() {
            var period1 = new TimeRange(new DateTime(2011, 3, 1), new DateTime(2011, 3, 5));
            var period2 = new TimeRange(new DateTime(2011, 3, 10), new DateTime(2011, 3, 15));

            var periodCombiner = new TimePeriodCombiner<TimeRange>();
            var periods = periodCombiner.CombinePeriods(new TimePeriodCollection
                                                        {
                                                            period1,
                                                            period2
                                                        });
            periods.Count.Should().Be(2);
            Assert.IsTrue(periods[0].IsSamePeriod(period1));
            Assert.IsTrue(periods[1].IsSamePeriod(period2));
        }

        [Test]
        public void TwoPeriodsTouchingTest() {
            var period1 = new TimeRange(new DateTime(2011, 3, 1), new DateTime(2011, 3, 10));
            var period2 = new TimeRange(new DateTime(2011, 3, 10), new DateTime(2011, 3, 20));
            var periodCombiner = new TimePeriodCombiner<TimeRange>();
            var periods = periodCombiner.CombinePeriods(new TimePeriodCollection
                                                        {
                                                            period1,
                                                            period2
                                                        });
            periods.Count.Should().Be(1);
            Assert.IsTrue(periods[0].IsSamePeriod(new TimeRange(new DateTime(2011, 3, 1), new DateTime(2011, 3, 20))));
        }

        [Test]
        public void TwoPeriodsOverlap1Test() {
            var period1 = new TimeRange(new DateTime(2011, 3, 1), new DateTime(2011, 3, 15));
            var period2 = new TimeRange(new DateTime(2011, 3, 10), new DateTime(2011, 3, 20));
            var periodCombiner = new TimePeriodCombiner<TimeRange>();
            var periods = periodCombiner.CombinePeriods(period1, period2);
            periods.Count.Should().Be(1);
            Assert.IsTrue(periods[0].IsSamePeriod(new TimeRange(new DateTime(2011, 3, 1), new DateTime(2011, 3, 20))));
        }

        [Test]
        public void TwoPeriodsOverlap2Test() {
            var period1 = new TimeRange(new DateTime(2011, 3, 1), new DateTime(2011, 3, 10));
            var period2 = new TimeRange(new DateTime(2011, 3, 5), new DateTime(2011, 3, 20));
            var periodCombiner = new TimePeriodCombiner<TimeRange>();
            var periods = periodCombiner.CombinePeriods(period1, period2);

            periods.Count.Should().Be(1);
            Assert.IsTrue(periods[0].IsSamePeriod(new TimeRange(new DateTime(2011, 3, 1), new DateTime(2011, 3, 20))));
        }

        [Test]
        public void TwoPeriodsInside1Test() {
            var period1 = new TimeRange(new DateTime(2011, 3, 1), new DateTime(2011, 3, 20));
            var period2 = new TimeRange(new DateTime(2011, 3, 5), new DateTime(2011, 3, 15));
            var periodCombiner = new TimePeriodCombiner<TimeRange>();
            var periods = periodCombiner.CombinePeriods(period1, period2);

            periods.Count.Should().Be(1);
            Assert.IsTrue(periods[0].IsSamePeriod(new TimeRange(new DateTime(2011, 3, 1), new DateTime(2011, 3, 20))));
        }

        [Test]
        public void TwoPeriodsInside2Test() {
            var period1 = new TimeRange(new DateTime(2011, 3, 5), new DateTime(2011, 3, 15));
            var period2 = new TimeRange(new DateTime(2011, 3, 1), new DateTime(2011, 3, 20));

            var periodCombiner = new TimePeriodCombiner<TimeRange>();
            var periods = periodCombiner.CombinePeriods(period1, period2);

            periods.Count.Should().Be(1);
            Assert.IsTrue(periods[0].IsSamePeriod(new TimeRange(new DateTime(2011, 3, 1), new DateTime(2011, 3, 20))));
        }

        [Test]
        public void Pattern1Test() {
            var period1 = new TimeRange(new DateTime(2011, 3, 01), new DateTime(2011, 3, 05));
            var period2 = new TimeRange(new DateTime(2011, 3, 05), new DateTime(2011, 3, 10));
            var period3 = new TimeRange(new DateTime(2011, 3, 10), new DateTime(2011, 3, 15));
            var period4 = new TimeRange(new DateTime(2011, 3, 15), new DateTime(2011, 3, 20));
            var period5 = new TimeRange(new DateTime(2011, 3, 20), new DateTime(2011, 3, 25));

            var periodCombiner = new TimePeriodCombiner<TimeRange>();
            var periods = periodCombiner.CombinePeriods(period1, period2, period3, period4, period5);

            periods.Count.Should().Be(1);
            Assert.IsTrue(periods[0].IsSamePeriod(new TimeRange(new DateTime(2011, 3, 1), new DateTime(2011, 3, 25))));
        }

        [Test]
        public void Pattern2Test() {
            var period1 = new TimeRange(new DateTime(2011, 3, 05), new DateTime(2011, 3, 10));
            var period2 = new TimeRange(new DateTime(2011, 3, 01), new DateTime(2011, 3, 15));

            var period3 = new TimeRange(new DateTime(2011, 3, 20), new DateTime(2011, 3, 22));
            var period4 = new TimeRange(new DateTime(2011, 3, 22), new DateTime(2011, 3, 28));
            var period5 = new TimeRange(new DateTime(2011, 3, 25), new DateTime(2011, 3, 30));

            var periodCombiner = new TimePeriodCombiner<TimeRange>();
            var periods = periodCombiner.CombinePeriods(period1, period2, period3, period4, period5);

            periods.Count.Should().Be(2);
            Assert.IsTrue(periods[0].IsSamePeriod(new TimeRange(new DateTime(2011, 3, 1), new DateTime(2011, 3, 15))));
            Assert.IsTrue(periods[1].IsSamePeriod(new TimeRange(new DateTime(2011, 3, 20), new DateTime(2011, 3, 30))));
        }

        [Test]
        public void Pattern3Test() {
            var period1 = new TimeRange(new DateTime(2011, 3, 01), new DateTime(2011, 3, 05));
            var period2 = new TimeRange(new DateTime(2011, 3, 01), new DateTime(2011, 3, 10));
            var period3 = new TimeRange(new DateTime(2011, 3, 01), new DateTime(2011, 3, 15));
            var period4 = new TimeRange(new DateTime(2011, 3, 01), new DateTime(2011, 3, 20));
            var period5 = new TimeRange(new DateTime(2011, 3, 01), new DateTime(2011, 3, 25));

            var periodCombiner = new TimePeriodCombiner<TimeRange>();
            var periods = periodCombiner.CombinePeriods(period1, period2, period3, period4, period5);

            periods.Count.Should().Be(1);
            Assert.IsTrue(periods[0].IsSamePeriod(new TimeRange(new DateTime(2011, 3, 1), new DateTime(2011, 3, 25))));
        }

        [Test]
        public void Pattern4Test() {
            TimeRange period1 = new TimeRange(new DateTime(2011, 3, 20), new DateTime(2011, 3, 25));
            TimeRange period2 = new TimeRange(new DateTime(2011, 3, 15), new DateTime(2011, 3, 25));
            TimeRange period3 = new TimeRange(new DateTime(2011, 3, 10), new DateTime(2011, 3, 25));
            TimeRange period4 = new TimeRange(new DateTime(2011, 3, 05), new DateTime(2011, 3, 25));
            TimeRange period5 = new TimeRange(new DateTime(2011, 3, 01), new DateTime(2011, 3, 25));
            TimePeriodCombiner<TimeRange> periodCombiner = new TimePeriodCombiner<TimeRange>();
            ITimePeriodCollection periods =
                periodCombiner.CombinePeriods(new TimePeriodCollection { period1, period2, period3, period4, period5 });
            Assert.AreEqual(periods.Count, 1);
            Assert.IsTrue(periods[0].IsSamePeriod(new TimeRange(new DateTime(2011, 3, 1), new DateTime(2011, 3, 25))));
        }

        [Test]
        public void Pattern5Test() {
            var period1 = new TimeRange(new DateTime(2011, 3, 01), new DateTime(2011, 3, 05));
            var period2 = new TimeRange(new DateTime(2011, 3, 01), new DateTime(2011, 3, 10));
            var period3 = new TimeRange(new DateTime(2011, 3, 01), new DateTime(2011, 3, 15));
            var period4 = new TimeRange(new DateTime(2011, 3, 05), new DateTime(2011, 3, 25));
            var period5 = new TimeRange(new DateTime(2011, 3, 10), new DateTime(2011, 3, 25));
            var period6 = new TimeRange(new DateTime(2011, 3, 15), new DateTime(2011, 3, 25));

            var periodCombiner = new TimePeriodCombiner<TimeRange>();
            var periods = periodCombiner.CombinePeriods(period1, period2, period3, period4, period5, period6);

            periods.Count.Should().Be(1);
            Assert.IsTrue(periods[0].IsSamePeriod(new TimeRange(new DateTime(2011, 3, 1), new DateTime(2011, 3, 25))));
        }

        [Test]
        public void Pattern6Test() {
            var period1 = new TimeRange(new DateTime(2011, 3, 01), new DateTime(2011, 3, 10));
            var period2 = new TimeRange(new DateTime(2011, 3, 04), new DateTime(2011, 3, 08));

            var period3 = new TimeRange(new DateTime(2011, 3, 15), new DateTime(2011, 3, 18));
            var period4 = new TimeRange(new DateTime(2011, 3, 18), new DateTime(2011, 3, 22));
            var period5 = new TimeRange(new DateTime(2011, 3, 20), new DateTime(2011, 3, 24));

            var period6 = new TimeRange(new DateTime(2011, 3, 26), new DateTime(2011, 3, 30));
            var periodCombiner = new TimePeriodCombiner<TimeRange>();
            var periods = periodCombiner.CombinePeriods(period1, period2, period3, period4, period5, period6);

            periods.Count.Should().Be(3);
            Assert.IsTrue(periods[0].IsSamePeriod(new TimeRange(new DateTime(2011, 3, 01), new DateTime(2011, 3, 10))));
            Assert.IsTrue(periods[1].IsSamePeriod(new TimeRange(new DateTime(2011, 3, 15), new DateTime(2011, 3, 24))));
            Assert.IsTrue(periods[2].IsSamePeriod(new TimeRange(new DateTime(2011, 3, 26), new DateTime(2011, 3, 30))));
        }
    }
}