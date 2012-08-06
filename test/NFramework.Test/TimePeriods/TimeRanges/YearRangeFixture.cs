using System;
using System.Linq;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.TimePeriods.TimeRanges {
    [Microsoft.Silverlight.Testing.Tag("TimePeriods")]
    [TestFixture]
    public class YearRangeFixture : TimePeriodFixtureBase {
        [Test]
        public void InitValuesTest() {
            var now = ClockProxy.Clock.Now;
            var thisYear = new DateTime(now.Year, 1, 1);
            var nextYear = thisYear.AddYears(1);

            var year = new YearRange(now, TimeCalendar.NewEmptyOffset());

            year.Start.Year.Should().Be(thisYear.Year);
            year.Start.Month.Should().Be(thisYear.Month);
            year.Start.Day.Should().Be(thisYear.Day);
            year.Start.Hour.Should().Be(0);
            year.Start.Minute.Should().Be(0);
            year.Start.Second.Should().Be(0);
            year.Start.Millisecond.Should().Be(0);

            year.End.Year.Should().Be(nextYear.Year);
            year.End.Month.Should().Be(nextYear.Month);
            year.End.Day.Should().Be(nextYear.Day);
            year.End.Hour.Should().Be(0);
            year.End.Minute.Should().Be(0);
            year.End.Second.Should().Be(0);
            year.End.Millisecond.Should().Be(0);
        }

        [Test]
        public void DefaultCalendarTest() {
            var yearStart = new DateTime(ClockProxy.Clock.Now.Year, 1, 1);

            var year = new YearRange(yearStart);

            year.YearBaseMonth.Should().Be(1);
            year.BaseYear.Should().Be(yearStart.Year);
            year.Start.Should().Be(yearStart.Add(year.TimeCalendar.StartOffset));
            year.End.Should().Be(yearStart.AddYears(1).Add(year.TimeCalendar.EndOffset));
        }

        [Test]
        public void YearBaseMonthTest() {
            var year = new YearRange(TimeCalendar.New(April));
            year.YearBaseMonth.Should().Be(April);
            new YearRange().YearBaseMonth.Should().Be(January);
        }

        [Test]
        public void IsCalendarYearTest() {
            Assert.IsFalse(new YearRange(TimeCalendar.New(April)).IsCalendarYear);
            Assert.IsTrue(new YearRange(TimeCalendar.New(January)).IsCalendarYear);
            Assert.IsFalse(new YearRange(2008, TimeCalendar.New(April)).IsCalendarYear);
            Assert.IsTrue(new YearRange(2008).IsCalendarYear);
            Assert.IsTrue(new YearRange().IsCalendarYear);
            Assert.IsTrue(new YearRange(2008).IsCalendarYear);
        }

        [Test]
        public void StartYearTest() {
            var currentYear = ClockProxy.Clock.Now.Year;
            new YearRange(2008, TimeCalendar.New(April)).BaseYear.Should().Be(2008);
            new YearRange(currentYear).BaseYear.Should().Be(currentYear);
            new YearRange(2008).BaseYear.Should().Be(2008);

            new YearRange(new DateTime(2008, 7, 20), TimeCalendar.New(October)).BaseYear.Should().Be(2007);
            new YearRange(new DateTime(2008, 10, 1), TimeCalendar.New(October)).BaseYear.Should().Be(2008);
        }

        [Test]
        public void CurrentYearTest() {
            var currentYear = ClockProxy.Clock.Now.Year;
            var year = new YearRange(currentYear);

            year.IsReadOnly.Should().Be.True();
            year.BaseYear.Should().Be(currentYear);
            year.Start.Should().Be(new DateTime(currentYear, 1, 1));
            year.End.Should().Be.LessThan(new DateTime(currentYear + 1, 1, 1));
        }

        [Test]
        public void YearIndexTest() {
            const int yearIndex = 1994;
            var year = new YearRange(yearIndex);

            year.IsReadOnly.Should().Be.True();
            year.BaseYear.Should().Be(yearIndex);
            year.Start.Should().Be(new DateTime(yearIndex, 1, 1));
            year.End.Should().Be.LessThan(new DateTime(yearIndex + 1, 1, 1));
        }

        [Test]
        public void YearMomentTest() {
            const int yearIndex = 2002;
            var year = new YearRange(new DateTime(yearIndex, 3, 15));

            year.IsReadOnly.Should().Be.True();
            year.BaseYear.Should().Be(yearIndex);
            year.Start.Should().Be(new DateTime(yearIndex, 1, 1));
            year.End.Should().Be.LessThan(new DateTime(yearIndex + 1, 1, 1));
        }

        [Test]
        public void YearPeriodTest() {
            var currentYear = ClockProxy.Clock.Now.Year;
            var yearStart = new DateTime(currentYear, 4, 1);
            var yearEnd = yearStart.AddYears(1);

            var year = new YearRange(currentYear, TimeCalendar.New(TimeSpan.Zero, TimeSpan.Zero, April));

            year.IsReadOnly.Should().Be.True();
            year.YearBaseMonth.Should().Be(April);
            year.BaseYear.Should().Be(yearStart.Year);
            year.Start.Should().Be(yearStart);
            year.End.Should().Be(yearEnd);
        }

        [Test]
        public void YearCompareTest() {
            var moment = new DateTime(2008, 2, 18);
            var calendarYearSweden = new YearRange(moment, TimeCalendar.New(January));
            calendarYearSweden.YearBaseMonth.Should().Be(January);

            var calendarYearGermany = new YearRange(moment, TimeCalendar.New(April));
            calendarYearGermany.YearBaseMonth.Should().Be(April);

            var calendarYearUnitedStates = new YearRange(moment, TimeCalendar.New(October));
            calendarYearUnitedStates.YearBaseMonth.Should().Be(October);

            calendarYearSweden.Should().Not.Be.EqualTo(calendarYearGermany);
            calendarYearSweden.Should().Not.Be.EqualTo(calendarYearUnitedStates);
            calendarYearGermany.Should().Not.Be.EqualTo(calendarYearUnitedStates);

            calendarYearSweden.BaseYear.Should().Be(calendarYearGermany.BaseYear + 1);
            calendarYearSweden.BaseYear.Should().Be(calendarYearUnitedStates.BaseYear + 1);

            calendarYearSweden.GetPreviousYear().BaseYear.Should().Be(calendarYearGermany.GetPreviousYear().BaseYear + 1);
            calendarYearSweden.GetPreviousYear().BaseYear.Should().Be(calendarYearUnitedStates.GetPreviousYear().BaseYear + 1);

            calendarYearSweden.GetNextYear().BaseYear.Should().Be(calendarYearGermany.GetNextYear().BaseYear + 1);
            calendarYearSweden.GetNextYear().BaseYear.Should().Be(calendarYearUnitedStates.GetNextYear().BaseYear + 1);

            calendarYearSweden.IntersectsWith(calendarYearGermany).Should().Be.True();
            calendarYearSweden.IntersectsWith(calendarYearUnitedStates).Should().Be.True();
            calendarYearGermany.IntersectsWith(calendarYearUnitedStates).Should().Be.True();
        }

        [Test]
        public void GetPreviousYearTest() {
            var currentYearStart = new DateTime(ClockProxy.Clock.Now.Year, 4, 1);

            var currentYear = new YearRange(currentYearStart.Year, TimeCalendar.New(TimeSpan.Zero, TimeSpan.Zero, April));
            var previousYear = currentYear.GetPreviousYear();

            previousYear.IsReadOnly.Should().Be.True();
            previousYear.YearBaseMonth.Should().Be(April);
            previousYear.BaseYear.Should().Be(currentYearStart.Year - 1);
            previousYear.Start.Should().Be(currentYearStart.AddYears(-1));
            previousYear.End.Should().Be(currentYearStart);
        }

        [Test]
        public void GetNextYearTest() {
            DateTime currentYearStart = new DateTime(ClockProxy.Clock.Now.Year, 4, 1);

            var currentYear = new YearRange(currentYearStart.Year, TimeCalendar.New(TimeSpan.Zero, TimeSpan.Zero, April));
            var nextYear = currentYear.GetNextYear();

            nextYear.IsReadOnly.Should().Be.True();
            nextYear.YearBaseMonth.Should().Be(April);
            nextYear.BaseYear.Should().Be(currentYearStart.Year + 1);
            nextYear.Start.Should().Be(currentYearStart.AddYears(1));
            nextYear.End.Should().Be(currentYearStart.AddYears(2));
        }

        [Test]
        public void AddYearsTest() {
            var currentYear = new YearRange(TimeCalendar.New(April));

            currentYear.AddYears(0).Should().Be(currentYear);

            var pastYear = currentYear.AddYears(-10);
            pastYear.Start.Should().Be(currentYear.Start.AddYears(-10));
            pastYear.End.Should().Be(currentYear.End.AddYears(-10));

            var futureYear = currentYear.AddYears(10);
            futureYear.Start.Should().Be(currentYear.Start.AddYears(10));
            futureYear.End.Should().Be(currentYear.End.AddYears(10));
        }

        [Test]
        public void GetHalfyearsTest() {
            var year = new YearRange(TimeCalendar.New(October));

            var index = 0;
            foreach(var halfyear in year.GetHalfyears()) {
                halfyear.BaseYear.Should().Be(year.BaseYear);
                halfyear.Start.Should().Be(year.Start.AddMonths(index * TimeSpec.MonthsPerHalfyear));
                halfyear.End.Should().Be(halfyear.TimeCalendar.MapEnd(halfyear.Start.AddMonths(TimeSpec.MonthsPerHalfyear)));

                index++;
            }

            Assert.AreEqual(TimeSpec.HalfyearsPerYear, index);
        }

        [Test]
        public void GetQuartersTest() {
            var year = new YearRange(TimeCalendar.New(October));

            var index = 0;
            foreach(var quarter in year.GetQuarters()) {
                quarter.BaseYear.Should().Be(year.BaseYear);
                quarter.Start.Should().Be(year.Start.AddMonths(index * TimeSpec.MonthsPerQuarter));
                quarter.End.Should().Be(quarter.TimeCalendar.MapEnd(quarter.Start.AddMonths(TimeSpec.MonthsPerQuarter)));
                index++;
            }
            index.Should().Be(TimeSpec.QuartersPerYear);
        }

        [Test]
        public void GetMonthsTest() {
            var year = new YearRange(TimeCalendar.New(October));

            var months = year.GetMonths().ToList();
            months.Count.Should().Be(TimeSpec.MonthsPerYear);

            for(var m = 0; m < months.Count; m++) {
                var month = months[m];
                month.Start.Should().Be(year.Start.AddMonths(m));
                month.End.Should().Be(month.TimeCalendar.MapEnd(month.Start.AddMonths(1)));
            }
        }
    }
}