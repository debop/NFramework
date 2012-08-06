using System;
using System.Linq;
using NSoft.NFramework.LinqEx;
using NUnit.Framework;
using SharpTestsEx;

#if !SILVERLIGHT

#endif

namespace NSoft.NFramework.TimePeriods.TimeRanges {
    [TestFixture]
    public class MonthRangeFixture : TimePeriodFixtureBase {
        [Test]
        public void InitValuesTest() {
            var now = ClockProxy.Clock.Now;
            var firstMonth = new DateTime(now.Year, now.Month, 1);
            var secondMonth = firstMonth.AddMonths(1);

            var month = new MonthRange(now.Year, now.Month, TimeCalendar.NewEmptyOffset());

            month.Start.Should().Be(firstMonth);
            month.End.Should().Be(secondMonth);
        }

        [Test]
        public void DefaultCalendarTest() {
            var yearStart = ClockProxy.Clock.Now.TrimToMonth();

            Enumerable
                .Range(0, TimeSpec.MonthsPerYear)
                .RunEach(offset => {
                             var month = new MonthRange(yearStart.AddMonths(offset));

                             month.Year.Should().Be(yearStart.Year);
                             month.Month.Should().Be(offset + 1);
                             month.Start.Should().Be(yearStart.AddMonths(offset).Add(month.TimeCalendar.StartOffset));
                             month.End.Should().Be(yearStart.AddMonths(offset + 1).Add(month.TimeCalendar.EndOffset));
                         });
        }

        [Test]
        public void CurrentMonthTest() {
            var now = ClockProxy.Clock.Now;

            CultureTestData.Default
                .RunEach(culture => {
                             var currentYearMonthStart = now.StartTimeOfMonth();
                             var currentYearMonthEnd = currentYearMonthStart.AddMonths(1);

                             var month = new MonthRange(currentYearMonthStart, TimeCalendar.New(culture, TimeSpan.Zero, TimeSpan.Zero));

                             month.Year.Should().Be(now.Year);
                             month.Month.Should().Be(now.Month);
                             month.Start.Should().Be(currentYearMonthStart);
                             month.End.Should().Be(currentYearMonthEnd);
                         });
        }

        [Test]
        public void YearMonthTest() {
            var currentYear = ClockProxy.Clock.Now.Year;
            var calendar = TimeCalendar.New(TimeSpan.Zero, TimeSpan.Zero);

            var january = new MonthRange(currentYear, TimeSpec.CalendarYearStartMonth, calendar);

            january.Month.Should().Be(TimeSpec.CalendarYearStartMonth);
            january.Start.Should().Be(new DateTime(currentYear, 1, 1));
            january.End.Should().Be(new DateTime(currentYear, 2, 1));
        }

        [Test]
        public void MonthMomentTest() {
            var month = new MonthRange(new DateTime(2008, 1, 15), TimeCalendar.NewEmptyOffset());

            month.IsReadOnly.Should().Be.True();
            month.Year.Should().Be(2008);
            month.Month.Should().Be(TimeSpec.CalendarYearStartMonth);
            month.Start.Should().Be(new DateTime(2008, 1, 1));
            month.End.Should().Be(new DateTime(2008, 2, 1));

            var previous = month.GetPreviousMonth();

            previous.Year.Should().Be(2007);
            previous.Month.Should().Be(12);
            previous.Start.Should().Be(new DateTime(2007, 12, 1));
            previous.End.Should().Be(new DateTime(2008, 1, 1));

            var next = month.GetNextMonth();

            next.Year.Should().Be(2008);
            next.Month.Should().Be(2);
            next.Start.Should().Be(new DateTime(2008, 2, 1));
            next.End.Should().Be(new DateTime(2008, 3, 1));
        }

        [Test]
        public void LastMonthStartTest() {
            Enumerable
                .Range(2000, 20)
                .RunEach(year => {
                             for(var m = 1; m <= TimeSpec.MonthsPerYear; m++)
                                 new MonthRange(year, m).EndDayStart.Should().Be(new DateTime(year, m, 1).EndTimeOfMonth().Date);
                         });
        }

        [Test]
        public void GetDaysTest() {
            var month = new MonthRange();
            var days = month.GetDays().ToList();

            days.Count.Should().Be.GreaterThan(27);

            Enumerable
                .Range(0, days.Count)
                .RunEach(index => {
                             var day = days[index];
                             day.Start.Should().Be(month.Start.AddDays(index));
                             day.End.Should().Be(day.TimeCalendar.MapEnd(day.Start.AddDays(1)));

                             day.UnmappedStart.Should().Be(month.Start.AddDays(index));
                             day.UnmappedEnd.Should().Be(day.Start.AddDays(1));
                         });
        }

        [Test]
        public void GetPreviousMonthTest() {
            var month = new MonthRange();
            month.GetPreviousMonth().Should().Be(month.AddMonths(-1));
            month.GetPreviousMonth().Start.Should().Be(ClockProxy.Clock.Now.AddMonths(-1).StartTimeOfMonth());
        }

        [Test]
        public void GetNextMonthTest() {
            var month = new MonthRange();
            month.GetNextMonth().Should().Be(month.AddMonths(1));
            month.GetNextMonth().Start.Should().Be(ClockProxy.Clock.Now.AddMonths(1).StartTimeOfMonth());
        }

        [Test]
        public void AddMonthsTest() {
            var now = ClockProxy.Clock.Now;

            CultureTestData.Default
                .RunEach(culture => {
                             var currentYearMonthStart = now.StartTimeOfMonth();
                             var currentYearMonthEnd = currentYearMonthStart.AddMonths(1);

                             var currentYearMonth = new MonthRange(currentYearMonthStart, TimeCalendar.NewEmptyOffset(culture));

                             currentYearMonth.Start.Should().Be(currentYearMonthStart);
                             currentYearMonth.End.Should().Be(currentYearMonthEnd);
                             currentYearMonth.AddMonths(0).Should().Be(currentYearMonth);

                             var previousYearMonthStart = new DateTime(now.Year - 1, now.Month, 1);
                             var previousYearMonthEnd = previousYearMonthStart.AddMonths(1);
                             var previousYearMonth = currentYearMonth.AddMonths(TimeSpec.MonthsPerYear * -1);

                             previousYearMonth.Start.Should().Be(previousYearMonthStart);
                             previousYearMonth.End.Should().Be(previousYearMonthEnd);
                             previousYearMonth.Month.Should().Be(currentYearMonth.Month);
                             previousYearMonth.Start.Year.Should().Be(currentYearMonth.Start.Year - 1);
                             previousYearMonth.End.Year.Should().Be(currentYearMonth.End.Year - 1);

                             var nextYearMonthStart = now.AddYears(1).StartTimeOfMonth();
                             var nextYearMonthEnd = nextYearMonthStart.AddMonths(1);
                             var nextYearMonth = currentYearMonth.AddMonths(TimeSpec.MonthsPerYear);

                             nextYearMonth.Start.Should().Be(nextYearMonthStart);
                             nextYearMonth.End.Should().Be(nextYearMonthEnd);
                             nextYearMonth.Month.Should().Be(currentYearMonth.Month);
                             nextYearMonth.Start.Year.Should().Be(currentYearMonth.Start.Year + 1);
                             nextYearMonth.End.Year.Should().Be(currentYearMonth.End.Year + 1);
                         });
        }
    }
}