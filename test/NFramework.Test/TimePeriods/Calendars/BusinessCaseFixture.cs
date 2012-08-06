using System;
using System.Globalization;
using NSoft.NFramework.TimePeriods.TimeRanges;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.TimePeriods.Calendars {
    [TestFixture]
    public class BusinessCaseFixture : TimePeriodFixtureBase {
        [Test]
        public void TimeRangeCalendarTimeRangeTest() {
            var now = ClockProxy.Clock.Now;

            for(var i = 0; i < 500; i += 10) {
                //var fiveSeconds = new TimeRange(new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 15, 0),
                //                                      new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 20, 0));

                var current = now.AddDays(i);
                var currentFiveSeconds = new TimeRange(current.TrimToSecond(15), current.TrimToSecond(20));

                new YearRange(current).HasInside(currentFiveSeconds).Should().Be.True();
                new HalfyearRange(current).HasInside(currentFiveSeconds).Should().Be.True();
                new QuarterRange(current).HasInside(currentFiveSeconds).Should().Be.True();
                new MonthRange(current).HasInside(currentFiveSeconds).Should().Be.True();
                new WeekRange(current).HasInside(currentFiveSeconds).Should().Be.True();
                new DayRange(current).HasInside(currentFiveSeconds).Should().Be.True();
                new HourRange(current).HasInside(currentFiveSeconds).Should().Be.True();
                new MinuteRange(current).HasInside(currentFiveSeconds).Should().Be.True();
            }


            TimeRange anytime = new TimeRange();

            new YearRange().HasInside(anytime).Should().Be.False();
            new HalfyearRange().HasInside(anytime).Should().Be.False();
            new QuarterRange().HasInside(anytime).Should().Be.False();
            new MonthRange().HasInside(anytime).Should().Be.False();
            new WeekRange().HasInside(anytime).Should().Be.False();
            new DayRange().HasInside(anytime).Should().Be.False();
            new HourRange().HasInside(anytime).Should().Be.False();
            new MinuteRange().HasInside(anytime).Should().Be.False();
        }

        [Test]
        public void FiscalYearTest() {
            var testDate = new DateTime(2008, 11, 18);
            var year = new YearRange(testDate, TimeCalendar.New(October));

            year.YearBaseMonth.Should().Be(October);
            year.BaseYear.Should().Be(testDate.Year);

            // Start & End
            year.Start.Year.Should().Be(testDate.Year);
            year.Start.Month.Should().Be(October);
            year.Start.Day.Should().Be(1);
            year.End.Year.Should().Be(testDate.Year + 1);
            year.End.Month.Should().Be(September);
            year.End.Day.Should().Be(CultureInfo.CurrentCulture.Calendar.GetDaysInMonth(year.End.Year, year.End.Month));

            // Half Years

            var halfyears = year.GetHalfyears();

            foreach(var halfyear in year.GetHalfyears()) {
                if(halfyear.Halfyear == HalfyearKind.First) {
                    halfyear.Start.Should().Be(year.Start);
                    halfyear.Start.Year.Should().Be(testDate.Year);
                    halfyear.Start.Month.Should().Be(October);
                    halfyear.Start.Day.Should().Be(1);

                    halfyear.End.Year.Should().Be(testDate.Year + 1);
                    halfyear.End.Month.Should().Be(March);
                    halfyear.End.Day.Should().Be(CultureInfo.CurrentCulture.Calendar.GetDaysInMonth(halfyear.End.Year,
                                                                                                    halfyear.End.Month));
                }
                else {
                    halfyear.Start.Should().Be(year.Start.AddMonths(TimeSpec.MonthsPerHalfyear));
                    halfyear.Start.Year.Should().Be(testDate.Year + 1);
                    halfyear.Start.Month.Should().Be(April);
                    halfyear.Start.Day.Should().Be(1);

                    halfyear.End.Year.Should().Be(testDate.Year + 1);
                    halfyear.End.Month.Should().Be(September);
                    halfyear.End.Day.Should().Be(CultureInfo.CurrentCulture.Calendar.GetDaysInMonth(halfyear.End.Year,
                                                                                                    halfyear.End.Month));
                }
            }

            // Quarters
            foreach(var quarter in year.GetQuarters()) {
                switch(quarter.Quarter) {
                    case QuarterKind.First:
                        quarter.Start.Should().Be(year.Start);
                        quarter.Start.Year.Should().Be(testDate.Year);
                        quarter.Start.Month.Should().Be(October);
                        quarter.Start.Day.Should().Be(1);
                        quarter.End.Year.Should().Be(testDate.Year);
                        quarter.End.Month.Should().Be(December);
                        quarter.End.Day.Should().Be(quarter.TimeCalendar.GetDaysInMonth(quarter.End.Year, quarter.End.Month));
                        break;

                    case QuarterKind.Second:
                        quarter.Start.Year.Should().Be(testDate.Year + 1);
                        quarter.Start.Month.Should().Be(January);
                        quarter.Start.Day.Should().Be(1);
                        quarter.End.Year.Should().Be(testDate.Year + 1);
                        quarter.End.Month.Should().Be(March);
                        quarter.End.Day.Should().Be(quarter.TimeCalendar.GetDaysInMonth(quarter.End.Year, quarter.End.Month));
                        break;

                    case QuarterKind.Third:
                        quarter.Start.Year.Should().Be(testDate.Year + 1);
                        quarter.Start.Month.Should().Be(April);
                        quarter.Start.Day.Should().Be(1);
                        quarter.End.Year.Should().Be(testDate.Year + 1);
                        quarter.End.Month.Should().Be(June);
                        quarter.End.Day.Should().Be(quarter.TimeCalendar.GetDaysInMonth(quarter.End.Year, quarter.End.Month));
                        break;

                    case QuarterKind.Fourth:
                        quarter.End.Should().Be(year.End);
                        quarter.Start.Year.Should().Be(testDate.Year + 1);
                        quarter.Start.Month.Should().Be(July);
                        quarter.Start.Day.Should().Be(1);
                        quarter.End.Year.Should().Be(testDate.Year + 1);
                        quarter.End.Month.Should().Be(September);
                        quarter.End.Day.Should().Be(quarter.TimeCalendar.GetDaysInMonth(quarter.End.Year, quarter.End.Month));
                        break;
                }
            }

            // months
            //
            var monthIndex = 0;
            foreach(var month in year.GetMonths()) {
                if(monthIndex == 0)
                    Assert.AreEqual(month.Start, year.Start);
                else if(monthIndex == TimeSpec.MonthsPerYear - 1)
                    Assert.AreEqual(month.End, year.End);

                var startDate = new DateTime(year.BaseYear, year.Start.Month, 1).AddMonths(monthIndex);

                month.Start.Year.Should().Be(startDate.Year);
                month.Start.Month.Should().Be(startDate.Month);
                month.Start.Day.Should().Be(startDate.Day);
                month.End.Year.Should().Be(startDate.Year);
                month.End.Month.Should().Be(startDate.Month);

                monthIndex++;
            }
        }

        [Test]
        public void CalendarQuarterOfYearTest() {
            var currentYear = ClockProxy.Clock.Now.Year;

            var timeCalendar = TimeCalendar.New(October);
            var calendarYear = new YearRange(currentYear, timeCalendar);

            calendarYear.YearBaseMonth.Should().Be(October);
            calendarYear.BaseYear.Should().Be(currentYear);
            calendarYear.Start.Should().Be(new DateTime(currentYear, October, 1));
            calendarYear.End.Should().Be(calendarYear.TimeCalendar.MapEnd(calendarYear.Start.AddYears(1)));
            calendarYear.UnmappedEnd.Should().Be(calendarYear.Start.AddYears(1));

            // Q1
            var q1 = new QuarterRange(calendarYear.BaseYear, QuarterKind.First, timeCalendar);
            q1.YearBaseMonth.Should().Be(calendarYear.YearBaseMonth);
            q1.BaseYear.Should().Be(calendarYear.BaseYear);
            q1.Start.Should().Be(new DateTime(currentYear, October, 1));
            q1.End.Should().Be(q1.TimeCalendar.MapEnd(q1.Start.AddMonths(TimeSpec.MonthsPerQuarter)));
            q1.UnmappedEnd.Should().Be(q1.Start.AddMonths(TimeSpec.MonthsPerQuarter));

            // Q2
            var q2 = new QuarterRange(calendarYear.BaseYear, QuarterKind.Second, timeCalendar);
            q2.YearBaseMonth.Should().Be(calendarYear.YearBaseMonth);
            q2.BaseYear.Should().Be(calendarYear.BaseYear);
            q2.Start.Should().Be(new DateTime(currentYear + 1, 1, 1));
            q2.End.Should().Be(q2.TimeCalendar.MapEnd(q2.Start.AddMonths(TimeSpec.MonthsPerQuarter)));
            q2.UnmappedEnd.Should().Be(q2.Start.AddMonths(TimeSpec.MonthsPerQuarter));

            // Q3
            var q3 = new QuarterRange(calendarYear.BaseYear, QuarterKind.Third, timeCalendar);
            q3.YearBaseMonth.Should().Be(calendarYear.YearBaseMonth);
            q3.BaseYear.Should().Be(calendarYear.BaseYear);
            q3.Start.Should().Be(new DateTime(currentYear + 1, 4, 1));
            q3.End.Should().Be(q3.TimeCalendar.MapEnd(q3.Start.AddMonths(TimeSpec.MonthsPerQuarter)));
            q3.UnmappedEnd.Should().Be(q3.Start.AddMonths(TimeSpec.MonthsPerQuarter));

            // Q4
            var q4 = new QuarterRange(calendarYear.BaseYear, QuarterKind.Fourth, timeCalendar);
            q4.YearBaseMonth.Should().Be(calendarYear.YearBaseMonth);
            q4.BaseYear.Should().Be(calendarYear.BaseYear);
            q4.Start.Should().Be(new DateTime(currentYear + 1, 7, 1));
            q4.End.Should().Be(q4.TimeCalendar.MapEnd(q4.Start.AddMonths(TimeSpec.MonthsPerQuarter)));
            q4.UnmappedEnd.Should().Be(q4.Start.AddMonths(TimeSpec.MonthsPerQuarter));
        }
    }
}