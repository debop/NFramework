using System;
using System.Linq;
using System.Threading.Tasks;
using NSoft.NFramework.LinqEx;
using NUnit.Framework;
using SharpTestsEx;

#if !SILVERLIGHT

#endif

namespace NSoft.NFramework.TimePeriods.TimeRanges {
    [TestFixture]
    public class QuarterRangeFixture : TimePeriodFixtureBase {
        [Test]
        public void InitValuesTest() {
            var now = ClockProxy.Clock.Now;
            var firstQuarter = now.TrimToMonth();
            var secondQuarter = firstQuarter.AddMonths(TimeSpec.MonthsPerQuarter);

            QuarterRange quarter = new QuarterRange(now.Year, QuarterKind.First, TimeCalendar.NewEmptyOffset());

            quarter.Start.Year.Should().Be(firstQuarter.Year);
            quarter.Start.Month.Should().Be(firstQuarter.Month);
            quarter.Start.Day.Should().Be(firstQuarter.Day);
            quarter.Start.Hour.Should().Be(0);
            quarter.Start.Minute.Should().Be(0);
            quarter.Start.Second.Should().Be(0);
            quarter.Start.Millisecond.Should().Be(0);

            quarter.End.Year.Should().Be(secondQuarter.Year);
            quarter.End.Month.Should().Be(secondQuarter.Month);
            quarter.End.Day.Should().Be(secondQuarter.Day);
            quarter.End.Hour.Should().Be(0);
            quarter.End.Minute.Should().Be(0);
            quarter.End.Second.Should().Be(0);
            quarter.End.Millisecond.Should().Be(0);
        }

#if !SILVERLIGHT
        [Test]
        public void DefaultCalendarTest() {
            var yearStart = new DateTime(ClockProxy.Clock.Now.Year, 1, 1);

            Parallel.ForEach(Enum.GetValues(typeof(QuarterKind)).Cast<QuarterKind>(),
                             yearQuarter => {
                                 var offset = (int)yearQuarter - 1;
                                 var quarter = new QuarterRange(yearStart.AddMonths(TimeSpec.MonthsPerQuarter * offset));

                                 quarter.YearBaseMonth.Should().Be(1);
                                 quarter.BaseYear.Should().Be(yearStart.Year);
                                 quarter.Start.Should().Be(
                                     yearStart.AddMonths(TimeSpec.MonthsPerQuarter * offset).Add(quarter.TimeCalendar.StartOffset));
                                 quarter.End.Should().Be(
                                     yearStart.AddMonths(TimeSpec.MonthsPerQuarter * (offset + 1)).Add(quarter.TimeCalendar.EndOffset));
                             });
        }
#endif

        [Test]
        public void MomentTest() {
            var now = ClockProxy.Clock.Now;
            var calendar = TimeCalendar.New(April);

            new QuarterRange(new DateTime(now.Year, 1, 1)).Quarter.Should().Be(QuarterKind.First);
            new QuarterRange(new DateTime(now.Year, 3, 30)).Quarter.Should().Be(QuarterKind.First);
            new QuarterRange(new DateTime(now.Year, 4, 1)).Quarter.Should().Be(QuarterKind.Second);
            new QuarterRange(new DateTime(now.Year, 6, 30)).Quarter.Should().Be(QuarterKind.Second);
            new QuarterRange(new DateTime(now.Year, 7, 1)).Quarter.Should().Be(QuarterKind.Third);
            new QuarterRange(new DateTime(now.Year, 9, 30)).Quarter.Should().Be(QuarterKind.Third);
            new QuarterRange(new DateTime(now.Year, 10, 1)).Quarter.Should().Be(QuarterKind.Fourth);
            new QuarterRange(new DateTime(now.Year, 12, 31)).Quarter.Should().Be(QuarterKind.Fourth);

            new QuarterRange(new DateTime(now.Year, 4, 1), calendar).Quarter.Should().Be(QuarterKind.First);
            new QuarterRange(new DateTime(now.Year, 6, 30), calendar).Quarter.Should().Be(QuarterKind.First);
            new QuarterRange(new DateTime(now.Year, 7, 1), calendar).Quarter.Should().Be(QuarterKind.Second);
            new QuarterRange(new DateTime(now.Year, 9, 30), calendar).Quarter.Should().Be(QuarterKind.Second);
            new QuarterRange(new DateTime(now.Year, 10, 1), calendar).Quarter.Should().Be(QuarterKind.Third);
            new QuarterRange(new DateTime(now.Year, 12, 31), calendar).Quarter.Should().Be(QuarterKind.Third);
            new QuarterRange(new DateTime(now.Year, 1, 1), calendar).Quarter.Should().Be(QuarterKind.Fourth);
            new QuarterRange(new DateTime(now.Year, 3, 30), calendar).Quarter.Should().Be(QuarterKind.Fourth);
        }

        [Test]
        public void YearTest() {
            var currentYear = ClockProxy.Clock.Now.Year;

            new QuarterRange(currentYear, QuarterKind.Fourth, TimeCalendar.New(April)).BaseYear.Should().Be(currentYear);
            new QuarterRange(2006, QuarterKind.Fourth).BaseYear.Should().Be(2006);
        }

        [Test]
        public void YearQuarterTest() {
            var currentYear = ClockProxy.Clock.Now.Year;

            new QuarterRange(currentYear, QuarterKind.Third, TimeCalendar.New(April)).Quarter.Should().Be(QuarterKind.Third);
            new QuarterRange(currentYear, QuarterKind.Third).Quarter.Should().Be(QuarterKind.Third);
        }

        [Test]
        public void CurrentQuarterTest() {
            var currentYear = ClockProxy.Clock.Now.Year;
            var quarter = new QuarterRange(currentYear, QuarterKind.First);
            quarter.BaseYear.Should().Be(currentYear);
        }

        [Test]
        public void StartMonthTest() {
            int currentYear = ClockProxy.Clock.Now.Year;

            Enumerable.Range(1, TimeSpec.MonthsPerYear)
                .RunEach(m => {
                             new QuarterRange(currentYear, QuarterKind.First, TimeCalendar.New(m)).StartMonth.Should().Be(m);
                             new QuarterRange(currentYear, QuarterKind.Second, TimeCalendar.New(m)).StartMonth.Should().Be((m +
                                                                                                                            TimeSpec.
                                                                                                                                MonthsPerQuarter -
                                                                                                                            1) %
                                                                                                                           TimeSpec.
                                                                                                                               MonthsPerYear +
                                                                                                                           1);
                             new QuarterRange(currentYear, QuarterKind.Third, TimeCalendar.New(m)).StartMonth.Should().Be((m +
                                                                                                                           TimeSpec.
                                                                                                                               MonthsPerQuarter *
                                                                                                                           2 - 1) %
                                                                                                                          TimeSpec.
                                                                                                                              MonthsPerYear +
                                                                                                                          1);
                             new QuarterRange(currentYear, QuarterKind.Fourth, TimeCalendar.New(m)).StartMonth.Should().Be((m +
                                                                                                                            TimeSpec.
                                                                                                                                MonthsPerQuarter *
                                                                                                                            3 - 1) %
                                                                                                                           TimeSpec.
                                                                                                                               MonthsPerYear +
                                                                                                                           1);
                         });
        }

        [Test]
        public void CalendarYearQuarterTest() {
            var currentYear = ClockProxy.Clock.Now.Year;
            var timeCalendar = TimeCalendar.NewEmptyOffset();

            var current = new QuarterRange(currentYear, QuarterKind.First);
            current.GetNextQuarter().GetNextQuarter().GetNextQuarter().GetNextQuarter().Quarter.Should().Be(current.Quarter);
            current.GetPreviousQuarter().GetPreviousQuarter().GetPreviousQuarter().GetPreviousQuarter().Quarter.Should().Be(
                current.Quarter);

            var q1 = new QuarterRange(currentYear, QuarterKind.First, timeCalendar);

            Assert.IsTrue(q1.IsReadOnly);
            q1.Quarter.Should().Be(QuarterKind.First);
            q1.Start.Should().Be(new DateTime(currentYear, TimeSpec.FirstQuarterMonth, 1));
            q1.End.Should().Be(new DateTime(currentYear, TimeSpec.SecondQuarterMonth, 1));

            var q2 = new QuarterRange(currentYear, QuarterKind.Second, timeCalendar);

            Assert.IsTrue(q2.IsReadOnly);
            q2.Quarter.Should().Be(QuarterKind.Second);
            q2.Start.Should().Be(new DateTime(currentYear, TimeSpec.SecondQuarterMonth, 1));
            q2.End.Should().Be(new DateTime(currentYear, TimeSpec.ThirdQuarterMonth, 1));

            var q3 = new QuarterRange(currentYear, QuarterKind.Third, timeCalendar);

            Assert.IsTrue(q3.IsReadOnly);
            q3.Quarter.Should().Be(QuarterKind.Third);
            q3.Start.Should().Be(new DateTime(currentYear, TimeSpec.ThirdQuarterMonth, 1));
            q3.End.Should().Be(new DateTime(currentYear, TimeSpec.FourthQuarterMonth, 1));

            var q4 = new QuarterRange(currentYear, QuarterKind.Fourth, timeCalendar);

            Assert.IsTrue(q4.IsReadOnly);
            q4.Quarter.Should().Be(QuarterKind.Fourth);
            q4.Start.Should().Be(new DateTime(currentYear, TimeSpec.FourthQuarterMonth, 1));
            q4.End.Should().Be(new DateTime(currentYear + 1, TimeSpec.FirstQuarterMonth, 1));
        }

        [Test]
        public void QuarterMomentTest() {
            var quarter = new QuarterRange(2008, QuarterKind.First, TimeCalendar.NewEmptyOffset());

            quarter.IsReadOnly.Should().Be.True();
            quarter.BaseYear.Should().Be(2008);
            quarter.Quarter.Should().Be(QuarterKind.First);
            quarter.Start.Should().Be(new DateTime(2008, TimeSpec.FirstQuarterMonth, 1));
            quarter.End.Should().Be(new DateTime(2008, TimeSpec.SecondQuarterMonth, 1));

            var previous = quarter.GetPreviousQuarter();

            previous.BaseYear.Should().Be(2007);
            previous.Quarter.Should().Be(QuarterKind.Fourth);
            previous.Start.Should().Be(new DateTime(2007, TimeSpec.FourthQuarterMonth, 1));
            previous.End.Should().Be(new DateTime(2008, TimeSpec.FirstQuarterMonth, 1));

            var next = quarter.GetNextQuarter();
            next.BaseYear.Should().Be(2008);
            next.Quarter.Should().Be(QuarterKind.Second);
            next.Start.Should().Be(new DateTime(2008, TimeSpec.SecondQuarterMonth, 1));
            next.End.Should().Be(new DateTime(2008, TimeSpec.ThirdQuarterMonth, 1));
        }

#if !SILVERLIGHT
        [Test]
        public void IsCalendarQuarterTest() {
            int currentYear = ClockProxy.Clock.Now.Year;

            foreach(QuarterKind yearQuarter in Enum.GetValues(typeof(QuarterKind))) {
                Assert.IsTrue(new QuarterRange(currentYear, yearQuarter, TimeCalendar.New(1)).IsCalendarQuarter);
                Assert.IsFalse(new QuarterRange(currentYear, yearQuarter, TimeCalendar.New(2)).IsCalendarQuarter);
                Assert.IsFalse(new QuarterRange(currentYear, yearQuarter, TimeCalendar.New(3)).IsCalendarQuarter);
                Assert.IsTrue(new QuarterRange(currentYear, yearQuarter, TimeCalendar.New(4)).IsCalendarQuarter);
                Assert.IsFalse(new QuarterRange(currentYear, yearQuarter, TimeCalendar.New(5)).IsCalendarQuarter);
                Assert.IsFalse(new QuarterRange(currentYear, yearQuarter, TimeCalendar.New(6)).IsCalendarQuarter);
                Assert.IsTrue(new QuarterRange(currentYear, yearQuarter, TimeCalendar.New(7)).IsCalendarQuarter);
                Assert.IsFalse(new QuarterRange(currentYear, yearQuarter, TimeCalendar.New(8)).IsCalendarQuarter);
                Assert.IsFalse(new QuarterRange(currentYear, yearQuarter, TimeCalendar.New(9)).IsCalendarQuarter);
                Assert.IsTrue(new QuarterRange(currentYear, yearQuarter, TimeCalendar.New(10)).IsCalendarQuarter);
                Assert.IsFalse(new QuarterRange(currentYear, yearQuarter, TimeCalendar.New(11)).IsCalendarQuarter);
                Assert.IsFalse(new QuarterRange(currentYear, yearQuarter, TimeCalendar.New(12)).IsCalendarQuarter);
            }
        }
#endif

        [Test]
        public void MultipleCalendarYearsTest() {
            DateTime now = ClockProxy.Clock.Now;

            Assert.IsFalse(new QuarterRange(now.Year, QuarterKind.First, TimeCalendar.New(1)).MultipleCalendarYears);
            Assert.IsFalse(new QuarterRange(now.Year, QuarterKind.First, TimeCalendar.New(2)).MultipleCalendarYears);
            Assert.IsFalse(new QuarterRange(now.Year, QuarterKind.First, TimeCalendar.New(3)).MultipleCalendarYears);
            Assert.IsFalse(new QuarterRange(now.Year, QuarterKind.First, TimeCalendar.New(4)).MultipleCalendarYears);
            Assert.IsFalse(new QuarterRange(now.Year, QuarterKind.First, TimeCalendar.New(5)).MultipleCalendarYears);
            Assert.IsFalse(new QuarterRange(now.Year, QuarterKind.First, TimeCalendar.New(6)).MultipleCalendarYears);
            Assert.IsFalse(new QuarterRange(now.Year, QuarterKind.First, TimeCalendar.New(7)).MultipleCalendarYears);
            Assert.IsFalse(new QuarterRange(now.Year, QuarterKind.First, TimeCalendar.New(8)).MultipleCalendarYears);
            Assert.IsFalse(new QuarterRange(now.Year, QuarterKind.First, TimeCalendar.New(9)).MultipleCalendarYears);
            Assert.IsFalse(new QuarterRange(now.Year, QuarterKind.First, TimeCalendar.New(10)).MultipleCalendarYears);
            Assert.IsTrue(new QuarterRange(now.Year, QuarterKind.First, TimeCalendar.New(11)).MultipleCalendarYears);
            Assert.IsTrue(new QuarterRange(now.Year, QuarterKind.First, TimeCalendar.New(12)).MultipleCalendarYears);

            Assert.IsFalse(new QuarterRange(now.Year, QuarterKind.Second, TimeCalendar.New(1)).MultipleCalendarYears);
            Assert.IsFalse(new QuarterRange(now.Year, QuarterKind.Second, TimeCalendar.New(2)).MultipleCalendarYears);
            Assert.IsFalse(new QuarterRange(now.Year, QuarterKind.Second, TimeCalendar.New(3)).MultipleCalendarYears);
            Assert.IsFalse(new QuarterRange(now.Year, QuarterKind.Second, TimeCalendar.New(4)).MultipleCalendarYears);
            Assert.IsFalse(new QuarterRange(now.Year, QuarterKind.Second, TimeCalendar.New(5)).MultipleCalendarYears);
            Assert.IsFalse(new QuarterRange(now.Year, QuarterKind.Second, TimeCalendar.New(6)).MultipleCalendarYears);
            Assert.IsFalse(new QuarterRange(now.Year, QuarterKind.Second, TimeCalendar.New(7)).MultipleCalendarYears);
            Assert.IsTrue(new QuarterRange(now.Year, QuarterKind.Second, TimeCalendar.New(8)).MultipleCalendarYears);
            Assert.IsTrue(new QuarterRange(now.Year, QuarterKind.Second, TimeCalendar.New(9)).MultipleCalendarYears);
            Assert.IsFalse(new QuarterRange(now.Year, QuarterKind.Second, TimeCalendar.New(10)).MultipleCalendarYears);
            Assert.IsFalse(new QuarterRange(now.Year, QuarterKind.Second, TimeCalendar.New(11)).MultipleCalendarYears);
            Assert.IsFalse(new QuarterRange(now.Year, QuarterKind.Second, TimeCalendar.New(12)).MultipleCalendarYears);

            Assert.IsFalse(new QuarterRange(now.Year, QuarterKind.Third, TimeCalendar.New(1)).MultipleCalendarYears);
            Assert.IsFalse(new QuarterRange(now.Year, QuarterKind.Third, TimeCalendar.New(2)).MultipleCalendarYears);
            Assert.IsFalse(new QuarterRange(now.Year, QuarterKind.Third, TimeCalendar.New(3)).MultipleCalendarYears);
            Assert.IsFalse(new QuarterRange(now.Year, QuarterKind.Third, TimeCalendar.New(4)).MultipleCalendarYears);
            Assert.IsTrue(new QuarterRange(now.Year, QuarterKind.Third, TimeCalendar.New(5)).MultipleCalendarYears);
            Assert.IsTrue(new QuarterRange(now.Year, QuarterKind.Third, TimeCalendar.New(6)).MultipleCalendarYears);
            Assert.IsFalse(new QuarterRange(now.Year, QuarterKind.Third, TimeCalendar.New(7)).MultipleCalendarYears);
            Assert.IsFalse(new QuarterRange(now.Year, QuarterKind.Third, TimeCalendar.New(8)).MultipleCalendarYears);
            Assert.IsFalse(new QuarterRange(now.Year, QuarterKind.Third, TimeCalendar.New(9)).MultipleCalendarYears);
            Assert.IsFalse(new QuarterRange(now.Year, QuarterKind.Third, TimeCalendar.New(10)).MultipleCalendarYears);
            Assert.IsFalse(new QuarterRange(now.Year, QuarterKind.Third, TimeCalendar.New(11)).MultipleCalendarYears);
            Assert.IsFalse(new QuarterRange(now.Year, QuarterKind.Third, TimeCalendar.New(12)).MultipleCalendarYears);

            Assert.IsFalse(new QuarterRange(now.Year, QuarterKind.Fourth, TimeCalendar.New(1)).MultipleCalendarYears);
            Assert.IsTrue(new QuarterRange(now.Year, QuarterKind.Fourth, TimeCalendar.New(2)).MultipleCalendarYears);
            Assert.IsTrue(new QuarterRange(now.Year, QuarterKind.Fourth, TimeCalendar.New(3)).MultipleCalendarYears);
            Assert.IsFalse(new QuarterRange(now.Year, QuarterKind.Fourth, TimeCalendar.New(4)).MultipleCalendarYears);
            Assert.IsFalse(new QuarterRange(now.Year, QuarterKind.Fourth, TimeCalendar.New(5)).MultipleCalendarYears);
            Assert.IsFalse(new QuarterRange(now.Year, QuarterKind.Fourth, TimeCalendar.New(6)).MultipleCalendarYears);
            Assert.IsFalse(new QuarterRange(now.Year, QuarterKind.Fourth, TimeCalendar.New(7)).MultipleCalendarYears);
            Assert.IsFalse(new QuarterRange(now.Year, QuarterKind.Fourth, TimeCalendar.New(8)).MultipleCalendarYears);
            Assert.IsFalse(new QuarterRange(now.Year, QuarterKind.Fourth, TimeCalendar.New(9)).MultipleCalendarYears);
            Assert.IsFalse(new QuarterRange(now.Year, QuarterKind.Fourth, TimeCalendar.New(10)).MultipleCalendarYears);
            Assert.IsFalse(new QuarterRange(now.Year, QuarterKind.Fourth, TimeCalendar.New(11)).MultipleCalendarYears);
            Assert.IsFalse(new QuarterRange(now.Year, QuarterKind.Fourth, TimeCalendar.New(12)).MultipleCalendarYears);
        }

        [Test]
        public void DefaultQuarterTest() {
            const int yearStartMonth = April;
            var currentYear = ClockProxy.Clock.Now.Year;

            var timeCalendar = TimeCalendar.New(TimeSpan.Zero, TimeSpan.Zero, yearStartMonth);

            var q1 = new QuarterRange(currentYear, QuarterKind.First, timeCalendar);

            q1.IsReadOnly.Should().Be.True();
            q1.YearBaseMonth.Should().Be(yearStartMonth);
            q1.Quarter.Should().Be(QuarterKind.First);
            q1.BaseYear.Should().Be(currentYear);
            q1.Start.Should().Be(new DateTime(currentYear, 4, 1));
            q1.End.Should().Be(new DateTime(currentYear, 7, 1));

            var q2 = new QuarterRange(currentYear, QuarterKind.Second, timeCalendar);

            q2.IsReadOnly.Should().Be.True();
            q2.YearBaseMonth.Should().Be(yearStartMonth);
            q2.Quarter.Should().Be(QuarterKind.Second);
            q2.BaseYear.Should().Be(currentYear);
            q2.Start.Should().Be(new DateTime(currentYear, 7, 1));
            q2.End.Should().Be(new DateTime(currentYear, 10, 1));

            var q3 = new QuarterRange(currentYear, QuarterKind.Third, timeCalendar);

            q3.IsReadOnly.Should().Be.True();
            q3.YearBaseMonth.Should().Be(yearStartMonth);
            q3.Quarter.Should().Be(QuarterKind.Third);
            q3.BaseYear.Should().Be(currentYear);
            q3.Start.Should().Be(new DateTime(currentYear, 10, 1));
            q3.End.Should().Be(new DateTime(currentYear + 1, 1, 1));

            var q4 = new QuarterRange(currentYear, QuarterKind.Fourth, timeCalendar);

            q4.IsReadOnly.Should().Be.True();
            q4.YearBaseMonth.Should().Be(yearStartMonth);
            q4.Quarter.Should().Be(QuarterKind.Fourth);
            q4.BaseYear.Should().Be(currentYear);
            q4.Start.Should().Be(new DateTime(currentYear + 1, 1, 1));
            q4.End.Should().Be(new DateTime(currentYear + 1, 4, 1));
        }

        [Test]
        public void GetMonthsTest() {
            var quarter = new QuarterRange(ClockProxy.Clock.Now.Year, QuarterKind.First, TimeCalendar.New(10));

            var months = quarter.GetMonths().ToTimePeriodCollection();
            months.Should().Not.Be.Null();
            months.Count.Should().Be.GreaterThan(0);

            int index = 0;
            foreach(MonthRange month in months) {
                month.Start.Should().Be(quarter.Start.AddMonths(index));
                month.UnmappedEnd.Should().Be(month.Start.AddMonths(1));

                index++;
            }

            Assert.AreEqual(TimeSpec.MonthsPerQuarter, index);
        }

        [Test]
        public void AddQuartersTest() {
            const int yearStartMonth = April;

            var currentYear = ClockProxy.Clock.Now.Year;
            var timeCalendar = TimeCalendar.New(TimeSpan.Zero, TimeSpan.Zero, yearStartMonth);

            var calendarStartDate = new DateTime(currentYear, 4, 1);
            var calendarQuarter = new QuarterRange(currentYear, QuarterKind.First, timeCalendar);

            Assert.AreEqual(calendarQuarter.AddQuarters(0), calendarQuarter);

            var prevQ1 = calendarQuarter.AddQuarters(-1);
            prevQ1.Quarter.Should().Be(QuarterKind.Fourth);
            prevQ1.BaseYear.Should().Be(currentYear - 1);
            prevQ1.Start.Should().Be(calendarStartDate.AddMonths(-3));
            prevQ1.End.Should().Be(calendarStartDate);

            var prevQ2 = calendarQuarter.AddQuarters(-2);
            prevQ2.Quarter.Should().Be(QuarterKind.Third);
            prevQ2.BaseYear.Should().Be(currentYear - 1);
            prevQ2.Start.Should().Be(calendarStartDate.AddMonths(-6));
            prevQ2.End.Should().Be(calendarStartDate.AddMonths(-3));

            var prevQ3 = calendarQuarter.AddQuarters(-3);
            prevQ3.Quarter.Should().Be(QuarterKind.Second);
            prevQ3.BaseYear.Should().Be(currentYear - 1);
            prevQ3.Start.Should().Be(calendarStartDate.AddMonths(-9));
            prevQ3.End.Should().Be(calendarStartDate.AddMonths(-6));

            var prevQ4 = calendarQuarter.AddQuarters(-4);
            prevQ4.Quarter.Should().Be(QuarterKind.First);
            prevQ4.BaseYear.Should().Be(currentYear - 1);
            prevQ4.Start.Should().Be(calendarStartDate.AddMonths(-12));
            prevQ4.End.Should().Be(calendarStartDate.AddMonths(-9));

            var prevQ5 = calendarQuarter.AddQuarters(-5);
            prevQ5.Quarter.Should().Be(QuarterKind.Fourth);
            prevQ5.BaseYear.Should().Be(currentYear - 2);
            prevQ5.Start.Should().Be(calendarStartDate.AddMonths(-15));
            prevQ5.End.Should().Be(calendarStartDate.AddMonths(-12));

            var futureQ1 = calendarQuarter.AddQuarters(1);
            futureQ1.Quarter.Should().Be(QuarterKind.Second);
            futureQ1.BaseYear.Should().Be(currentYear);
            futureQ1.Start.Should().Be(calendarStartDate.AddMonths(3));
            futureQ1.End.Should().Be(calendarStartDate.AddMonths(6));

            var futureQ2 = calendarQuarter.AddQuarters(2);
            futureQ2.Quarter.Should().Be(QuarterKind.Third);
            futureQ2.BaseYear.Should().Be(currentYear);
            futureQ2.Start.Should().Be(calendarStartDate.AddMonths(6));
            futureQ2.End.Should().Be(calendarStartDate.AddMonths(9));

            var futureQ3 = calendarQuarter.AddQuarters(3);
            futureQ3.Quarter.Should().Be(QuarterKind.Fourth);
            futureQ3.BaseYear.Should().Be(currentYear);
            futureQ3.Start.Should().Be(calendarStartDate.AddMonths(9));
            futureQ3.End.Should().Be(calendarStartDate.AddMonths(12));

            var futureQ4 = calendarQuarter.AddQuarters(4);
            futureQ4.Quarter.Should().Be(QuarterKind.First);
            futureQ4.BaseYear.Should().Be(currentYear + 1);
            futureQ4.Start.Should().Be(calendarStartDate.AddMonths(12));
            futureQ4.End.Should().Be(calendarStartDate.AddMonths(15));

            var futureQ5 = calendarQuarter.AddQuarters(5);
            futureQ5.Quarter.Should().Be(QuarterKind.Second);
            futureQ5.BaseYear.Should().Be(currentYear + 1);
            futureQ5.Start.Should().Be(calendarStartDate.AddMonths(15));
            futureQ5.End.Should().Be(calendarStartDate.AddMonths(18));
        }
    }
}