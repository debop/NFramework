using System;
using System.Linq;
using NSoft.NFramework.LinqEx;
using NUnit.Framework;
using SharpTestsEx;

#if !SILVERLIGHT

#endif

namespace NSoft.NFramework.TimePeriods.TimeRanges {
    [TestFixture]
    public class HalfyearRangeFixture : TimePeriodFixtureBase {
        [Test]
        public void InitValuesTest() {
            var now = ClockProxy.Clock.Now;
            var firstHalfyear = now.StartTimeOfYear();
            var secondHalfyear = new DateTime(now.Year, July, 1);

            var halfyear = new HalfyearRange(now.Year, HalfyearKind.First, TimeCalendar.NewEmptyOffset());

            halfyear.Start.Year.Should().Be(firstHalfyear.Year);
            halfyear.Start.Month.Should().Be(firstHalfyear.Month);
            halfyear.Start.Day.Should().Be(firstHalfyear.Day);
            halfyear.Start.Hour.Should().Be(0);
            halfyear.Start.Minute.Should().Be(0);
            halfyear.Start.Second.Should().Be(0);
            halfyear.Start.Millisecond.Should().Be(0);

            halfyear.End.Year.Should().Be(secondHalfyear.Year);
            halfyear.End.Month.Should().Be(secondHalfyear.Month);
            halfyear.End.Day.Should().Be(secondHalfyear.Day);
            halfyear.End.Hour.Should().Be(0);
            halfyear.End.Minute.Should().Be(0);
            halfyear.End.Second.Should().Be(0);
            halfyear.End.Millisecond.Should().Be(0);
        }

#if !SILVERLIGHT
        [Test]
        public void DefaultCalendarTest() {
            var yearStart = ClockProxy.Clock.Now.StartTimeOfYear();

            foreach(HalfyearKind yearHalfyear in Enum.GetValues(typeof(HalfyearKind))) {
                var offset = (int)yearHalfyear - 1;
                var halfyear = new HalfyearRange(yearStart.AddMonths(TimeSpec.MonthsPerHalfyear * offset));

                halfyear.YearBaseMonth.Should().Be(1);
                halfyear.BaseYear.Should().Be(yearStart.Year);
                halfyear.Start.Should().Be(
                    yearStart.AddMonths(TimeSpec.MonthsPerHalfyear * offset).Add(halfyear.TimeCalendar.StartOffset));
                halfyear.End.Should().Be(
                    yearStart.AddMonths(TimeSpec.MonthsPerHalfyear * (offset + 1)).Add(halfyear.TimeCalendar.EndOffset));
            }
        }
#endif

        [Test]
        public void MomentTest() {
            var now = ClockProxy.Clock.Now;

            new HalfyearRange().Halfyear.Should().Be(now.Month <= 6 ? HalfyearKind.First : HalfyearKind.Second);

            new HalfyearRange(new DateTime(now.Year, 1, 1)).Halfyear.Should().Be(HalfyearKind.First);
            new HalfyearRange(new DateTime(now.Year, 6, 30)).Halfyear.Should().Be(HalfyearKind.First);
            new HalfyearRange(new DateTime(now.Year, 7, 1)).Halfyear.Should().Be(HalfyearKind.Second);
            new HalfyearRange(new DateTime(now.Year, 12, 31)).Halfyear.Should().Be(HalfyearKind.Second);

            var timeCalendar = TimeCalendar.New(April);

            new HalfyearRange(new DateTime(now.Year, 4, 1), timeCalendar).Halfyear.Should().Be(HalfyearKind.First);
            new HalfyearRange(new DateTime(now.Year, 9, 30), timeCalendar).Halfyear.Should().Be(HalfyearKind.First);
            new HalfyearRange(new DateTime(now.Year, 10, 1), timeCalendar).Halfyear.Should().Be(HalfyearKind.Second);
            new HalfyearRange(new DateTime(now.Year, 3, 31), timeCalendar).Halfyear.Should().Be(HalfyearKind.Second);
        }

        [Test]
        public void YearBaseMonthTest() {
            var currentYear = ClockProxy.Clock.Now.Year;
            var halfyear = new HalfyearRange(currentYear, HalfyearKind.First, TimeCalendar.New(April));

            halfyear.YearBaseMonth.Should().Be(April);
            new HalfyearRange(currentYear, HalfyearKind.Second).YearBaseMonth.Should().Be(1);
        }

        [Test]
        public void YearTest() {
            var currentYear = ClockProxy.Clock.Now.Year;
            new HalfyearRange(currentYear, HalfyearKind.First, TimeCalendar.New(4)).BaseYear.Should().Be(currentYear);
            new HalfyearRange(2006, HalfyearKind.First).BaseYear.Should().Be(2006);
        }

        [Test]
        public void YearHalfyearTest() {
            var currentYear = ClockProxy.Clock.Now.Year;
            new HalfyearRange(currentYear, HalfyearKind.First, TimeCalendar.New(4)).Halfyear.Should().Be(HalfyearKind.First);
            new HalfyearRange(currentYear, HalfyearKind.Second, TimeCalendar.New(4)).Halfyear.Should().Be(HalfyearKind.Second);
        }

        [Test]
        public void StartMonthTest() {
            var currentYear = ClockProxy.Clock.Now.Year;

            Enumerable.Range(0, TimeSpec.MonthsPerYear)
                .RunEach(m => {
                             var month = m + 1;
                             new HalfyearRange(currentYear, HalfyearKind.First, TimeCalendar.New(month)).StartMonth.Should().Be(month);
                             new HalfyearRange(currentYear, HalfyearKind.Second, TimeCalendar.New(month)).StartMonth.Should().Be((m +
                                                                                                                                  TimeSpec
                                                                                                                                      .
                                                                                                                                      MonthsPerHalfyear) %
                                                                                                                                 TimeSpec
                                                                                                                                     .
                                                                                                                                     MonthsPerYear +
                                                                                                                                 1);
                         });
        }

#if !SILVERLIGHT
        [Test]
        public void IsCalendarHalfyearTest() {
            DateTime now = ClockProxy.Clock.Now;

            foreach(HalfyearKind yearHalfyear in Enum.GetValues(typeof(HalfyearKind))) {
                Assert.IsTrue(new HalfyearRange(now.Year, yearHalfyear, TimeCalendar.New(1)).IsCalendarHalfyear);
                Assert.IsFalse(new HalfyearRange(now.Year, yearHalfyear, TimeCalendar.New(2)).IsCalendarHalfyear);
                Assert.IsFalse(new HalfyearRange(now.Year, yearHalfyear, TimeCalendar.New(3)).IsCalendarHalfyear);
                Assert.IsFalse(new HalfyearRange(now.Year, yearHalfyear, TimeCalendar.New(4)).IsCalendarHalfyear);
                Assert.IsFalse(new HalfyearRange(now.Year, yearHalfyear, TimeCalendar.New(5)).IsCalendarHalfyear);
                Assert.IsFalse(new HalfyearRange(now.Year, yearHalfyear, TimeCalendar.New(6)).IsCalendarHalfyear);
                Assert.IsTrue(new HalfyearRange(now.Year, yearHalfyear, TimeCalendar.New(7)).IsCalendarHalfyear);
                Assert.IsFalse(new HalfyearRange(now.Year, yearHalfyear, TimeCalendar.New(8)).IsCalendarHalfyear);
                Assert.IsFalse(new HalfyearRange(now.Year, yearHalfyear, TimeCalendar.New(9)).IsCalendarHalfyear);
                Assert.IsFalse(new HalfyearRange(now.Year, yearHalfyear, TimeCalendar.New(10)).IsCalendarHalfyear);
                Assert.IsFalse(new HalfyearRange(now.Year, yearHalfyear, TimeCalendar.New(11)).IsCalendarHalfyear);
                Assert.IsFalse(new HalfyearRange(now.Year, yearHalfyear, TimeCalendar.New(12)).IsCalendarHalfyear);
            }
        }
#endif

        [Test]
        public void MultipleCalendarYearsTest() {
            DateTime now = ClockProxy.Clock.Now;

            Assert.IsFalse(new HalfyearRange(now.Year, HalfyearKind.First, TimeCalendar.New(1)).MultipleCalendarYears);
            Assert.IsFalse(new HalfyearRange(now.Year, HalfyearKind.First, TimeCalendar.New(2)).MultipleCalendarYears);
            Assert.IsFalse(new HalfyearRange(now.Year, HalfyearKind.First, TimeCalendar.New(3)).MultipleCalendarYears);
            Assert.IsFalse(new HalfyearRange(now.Year, HalfyearKind.First, TimeCalendar.New(4)).MultipleCalendarYears);
            Assert.IsFalse(new HalfyearRange(now.Year, HalfyearKind.First, TimeCalendar.New(5)).MultipleCalendarYears);
            Assert.IsFalse(new HalfyearRange(now.Year, HalfyearKind.First, TimeCalendar.New(6)).MultipleCalendarYears);
            Assert.IsFalse(new HalfyearRange(now.Year, HalfyearKind.First, TimeCalendar.New(7)).MultipleCalendarYears);
            Assert.IsTrue(new HalfyearRange(now.Year, HalfyearKind.First, TimeCalendar.New(8)).MultipleCalendarYears);
            Assert.IsTrue(new HalfyearRange(now.Year, HalfyearKind.First, TimeCalendar.New(9)).MultipleCalendarYears);
            Assert.IsTrue(new HalfyearRange(now.Year, HalfyearKind.First, TimeCalendar.New(10)).MultipleCalendarYears);
            Assert.IsTrue(new HalfyearRange(now.Year, HalfyearKind.First, TimeCalendar.New(11)).MultipleCalendarYears);
            Assert.IsTrue(new HalfyearRange(now.Year, HalfyearKind.First, TimeCalendar.New(12)).MultipleCalendarYears);

            Assert.IsFalse(new HalfyearRange(now.Year, HalfyearKind.Second, TimeCalendar.New(1)).MultipleCalendarYears);
            Assert.IsTrue(new HalfyearRange(now.Year, HalfyearKind.Second, TimeCalendar.New(2)).MultipleCalendarYears);
            Assert.IsTrue(new HalfyearRange(now.Year, HalfyearKind.Second, TimeCalendar.New(3)).MultipleCalendarYears);
            Assert.IsTrue(new HalfyearRange(now.Year, HalfyearKind.Second, TimeCalendar.New(4)).MultipleCalendarYears);
            Assert.IsTrue(new HalfyearRange(now.Year, HalfyearKind.Second, TimeCalendar.New(5)).MultipleCalendarYears);
            Assert.IsTrue(new HalfyearRange(now.Year, HalfyearKind.Second, TimeCalendar.New(6)).MultipleCalendarYears);
            Assert.IsFalse(new HalfyearRange(now.Year, HalfyearKind.Second, TimeCalendar.New(7)).MultipleCalendarYears);
            Assert.IsFalse(new HalfyearRange(now.Year, HalfyearKind.Second, TimeCalendar.New(8)).MultipleCalendarYears);
            Assert.IsFalse(new HalfyearRange(now.Year, HalfyearKind.Second, TimeCalendar.New(9)).MultipleCalendarYears);
            Assert.IsFalse(new HalfyearRange(now.Year, HalfyearKind.Second, TimeCalendar.New(10)).MultipleCalendarYears);
            Assert.IsFalse(new HalfyearRange(now.Year, HalfyearKind.Second, TimeCalendar.New(11)).MultipleCalendarYears);
            Assert.IsFalse(new HalfyearRange(now.Year, HalfyearKind.Second, TimeCalendar.New(12)).MultipleCalendarYears);
        }

        [Test]
        public void CalendarHalfyearTest() {
            var currentYear = ClockProxy.Clock.Now.Year;
            var calendar = TimeCalendar.NewEmptyOffset();

            var h1 = new HalfyearRange(currentYear, HalfyearKind.First, calendar);

            Assert.IsTrue(h1.IsReadOnly);
            Assert.IsTrue(h1.IsCalendarHalfyear);
            h1.YearBaseMonth.Should().Be(TimeSpec.CalendarYearStartMonth);
            h1.Halfyear.Should().Be(HalfyearKind.First);
            h1.BaseYear.Should().Be(currentYear);
            h1.Start.Should().Be(new DateTime(currentYear, 1, 1));
            h1.End.Should().Be(new DateTime(currentYear, 7, 1));

            var h2 = new HalfyearRange(currentYear, HalfyearKind.Second, calendar);
            Assert.IsTrue(h2.IsReadOnly);
            Assert.IsTrue(h2.IsCalendarHalfyear);
            h2.YearBaseMonth.Should().Be(TimeSpec.CalendarYearStartMonth);
            h2.Halfyear.Should().Be(HalfyearKind.Second);
            h2.BaseYear.Should().Be(currentYear);
            h2.Start.Should().Be(new DateTime(currentYear, 7, 1));
            h2.End.Should().Be(new DateTime(currentYear + 1, 1, 1));
        }

        [Test]
        public void DefaultHalfyearTest() {
            var currentYear = ClockProxy.Clock.Now.Year;
            const int yearStartMonth = 4;
            var calendar = TimeCalendar.New(TimeSpan.Zero, TimeSpan.Zero, yearStartMonth);

            var h1 = new HalfyearRange(currentYear, HalfyearKind.First, calendar);

            h1.IsReadOnly.Should().Be.True();
            h1.IsCalendarHalfyear.Should().Be.False();
            h1.YearBaseMonth.Should().Be(yearStartMonth);
            h1.Halfyear.Should().Be(HalfyearKind.First);
            h1.BaseYear.Should().Be(currentYear);
            h1.Start.Should().Be(new DateTime(currentYear, 4, 1));
            h1.End.Should().Be(new DateTime(currentYear, 10, 1));

            var h2 = new HalfyearRange(currentYear, HalfyearKind.Second, calendar);

            h2.IsReadOnly.Should().Be.True();
            h2.IsCalendarHalfyear.Should().Be.False();
            h2.YearBaseMonth.Should().Be(yearStartMonth);
            h2.Halfyear.Should().Be(HalfyearKind.Second);
            h2.BaseYear.Should().Be(currentYear);
            h2.Start.Should().Be(new DateTime(currentYear, 10, 1));
            h2.End.Should().Be(new DateTime(currentYear + 1, 4, 1));
        }

        [Test]
        public void GetQuartersTest() {
            var currentYear = ClockProxy.Clock.Now.Year;
            var timeCalendar = TimeCalendar.New(10);
            var h1 = new HalfyearRange(currentYear, HalfyearKind.First, timeCalendar);

            var h1Quarters = h1.GetQuarters();

            Assert.IsNotNull(h1Quarters);
            h1Quarters.Count().Should().Be(TimeSpec.QuartersPerHalfyear);

            var h1Index = 0;
            foreach(var h1Quarter in h1Quarters) {
                h1Quarter.BaseYear.Should().Be(h1.BaseYear);
                h1Quarter.Quarter.Should().Be(h1Index == 0 ? QuarterKind.First : QuarterKind.Second);
                h1Quarter.Start.Should().Be(h1.Start.AddMonths(h1Index * TimeSpec.MonthsPerQuarter));
                h1Quarter.End.Should().Be(h1Quarter.TimeCalendar.MapEnd(h1Quarter.Start.AddMonths(TimeSpec.MonthsPerQuarter)));

                h1Index++;
            }

            var h2 = new HalfyearRange(currentYear, HalfyearKind.Second, timeCalendar);

            var h2Quarters = h2.GetQuarters();

            Assert.IsNotNull(h2Quarters);
            h2Quarters.Count().Should().Be(TimeSpec.QuartersPerHalfyear);

            var h2Index = 0;
            foreach(var h2Quarter in h2Quarters) {
                h2Quarter.BaseYear.Should().Be(h2.BaseYear);
                h2Quarter.Quarter.Should().Be(h2Index == 0 ? QuarterKind.Third : QuarterKind.Fourth);
                h2Quarter.Start.Should().Be(h2.Start.AddMonths(h2Index * TimeSpec.MonthsPerQuarter));
                h2Quarter.End.Should().Be(h2Quarter.TimeCalendar.MapEnd(h2Quarter.Start.AddMonths(TimeSpec.MonthsPerQuarter)));
                h2Index++;
            }
        }

        [Test]
        public void GetMonthsTest() {
            var currentYear = ClockProxy.Clock.Now.Year;
            var timeCalendar = TimeCalendar.New(10);
            var halfyear = new HalfyearRange(currentYear, HalfyearKind.First, timeCalendar);

            var months = halfyear.GetMonths();

            Assert.IsNotNull(months);
            months.Count().Should().Be(TimeSpec.MonthsPerHalfyear);

            int index = 0;
            foreach(var month in months) {
                Assert.AreEqual(month.Start, halfyear.Start.AddMonths(index));
                Assert.AreEqual(month.End, month.TimeCalendar.MapEnd(month.Start.AddMonths(1)));
                index++;
            }
            Assert.AreEqual(index, TimeSpec.MonthsPerHalfyear);
        }

        [Test]
        public void AddHalfyearsTest() {
            const int yearStartMonth = April;

            var currentYear = ClockProxy.Clock.Now.Year;
            var timeCalendar = TimeCalendar.NewEmptyOffset(yearStartMonth);

            var calendarStartDate = new DateTime(currentYear, 4, 1);
            var calendarHalfyear = new HalfyearRange(currentYear, HalfyearKind.First, timeCalendar);

            calendarHalfyear.AddHalfyears(0).Should().Be(calendarHalfyear);

            var prevH1 = calendarHalfyear.AddHalfyears(-1);
            prevH1.Halfyear.Should().Be(HalfyearKind.Second);
            prevH1.BaseYear.Should().Be(currentYear - 1);
            prevH1.Start.Should().Be(calendarStartDate.AddMonths(-6));
            prevH1.End.Should().Be(calendarStartDate);

            var prevH2 = calendarHalfyear.AddHalfyears(-2);
            prevH2.Halfyear.Should().Be(HalfyearKind.First);
            prevH2.BaseYear.Should().Be(currentYear - 1);
            prevH2.Start.Should().Be(calendarStartDate.AddMonths(-12));
            prevH2.End.Should().Be(calendarStartDate.AddMonths(-6));

            var prevH3 = calendarHalfyear.AddHalfyears(-3);
            prevH3.Halfyear.Should().Be(HalfyearKind.Second);
            prevH3.BaseYear.Should().Be(currentYear - 2);
            prevH3.Start.Should().Be(calendarStartDate.AddMonths(-18));
            prevH3.End.Should().Be(calendarStartDate.AddMonths(-12));

            var futureH1 = calendarHalfyear.AddHalfyears(1);
            futureH1.Halfyear.Should().Be(HalfyearKind.Second);
            futureH1.BaseYear.Should().Be(currentYear);
            futureH1.Start.Should().Be(calendarStartDate.AddMonths(6));
            futureH1.End.Should().Be(calendarStartDate.AddMonths(12));

            var futureH2 = calendarHalfyear.AddHalfyears(2);
            futureH2.Halfyear.Should().Be(HalfyearKind.First);
            futureH2.BaseYear.Should().Be(currentYear + 1);
            futureH2.Start.Should().Be(calendarStartDate.AddMonths(12));
            futureH2.End.Should().Be(calendarStartDate.AddMonths(18));

            var futureH3 = calendarHalfyear.AddHalfyears(3);
            futureH3.Halfyear.Should().Be(HalfyearKind.Second);
            futureH3.BaseYear.Should().Be(currentYear + 1);
            futureH3.Start.Should().Be(calendarStartDate.AddMonths(18));
            futureH3.End.Should().Be(calendarStartDate.AddMonths(24));
        }
    }
}