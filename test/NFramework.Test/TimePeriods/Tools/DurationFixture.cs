using System;
using System.Globalization;
using System.Threading.Tasks;
using NSoft.NFramework.LinqEx;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.TimePeriods.Tools {
    [Microsoft.Silverlight.Testing.Tag("TimePeriods")]
    [TestFixture]
    public class DurationFixture : TimePeriodFixtureBase {
        private static readonly int currentYear = ClockProxy.Clock.Now.Year;
        private static readonly Calendar currentCalendar = DateTimeFormatInfo.CurrentInfo.Calendar;

        [Test]
        public void YearTest() {
            DurationUtil.Year(currentYear).Should().Be(TimeSpan.FromDays(currentCalendar.GetDaysInYear(currentYear)));
            DurationUtil.Year(currentYear + 1).Should().Be(TimeSpan.FromDays(currentCalendar.GetDaysInYear(currentYear + 1)));
            DurationUtil.Year(currentYear - 1).Should().Be(TimeSpan.FromDays(currentCalendar.GetDaysInYear(currentYear - 1)));

            DurationUtil.Year(currentYear, currentCalendar).Should().Be(TimeSpan.FromDays(currentCalendar.GetDaysInYear(currentYear)));
            DurationUtil.Year(currentYear + 1, currentCalendar).Should().Be(
                TimeSpan.FromDays(currentCalendar.GetDaysInYear(currentYear + 1)));
            DurationUtil.Year(currentYear - 1, currentCalendar).Should().Be(
                TimeSpan.FromDays(currentCalendar.GetDaysInYear(currentYear - 1)));

            CultureTestData.Default
                .RunEach(culture => {
                             var calendar = culture.Calendar;
                             DurationUtil.Year(currentYear, calendar).Should().Be(TimeSpan.FromDays(calendar.GetDaysInYear(currentYear)));
                             DurationUtil.Year(currentYear + 1, calendar).Should().Be(
                                 TimeSpan.FromDays(calendar.GetDaysInYear(currentYear + 1)));
                             DurationUtil.Year(currentYear - 1, calendar).Should().Be(
                                 TimeSpan.FromDays(calendar.GetDaysInYear(currentYear - 1)));
                         });
        }

#if !SILVERLIGHT
        [Test]
        public void HalfyearTest() {
            Parallel.ForEach(CultureTestData.Default,
                             culture => {
                                 var calendar = culture.Calendar;
                                 foreach(HalfyearKind halfyear in Enum.GetValues(typeof(HalfyearKind))) {
                                     var months = TimeTool.GetMonthsOfHalfyear(halfyear);
                                     var duration = TimeSpan.Zero;

                                     foreach(var month in months) {
                                         var monthDays = calendar.GetDaysInMonth(currentYear, month);
                                         duration += TimeSpan.FromDays(monthDays);
                                     }

                                     DurationUtil.Halfyear(currentYear, halfyear).Should().Be(duration);
                                     DurationUtil.Halfyear(currentYear, halfyear, calendar).Should().Be(duration);
                                 }
                             });
        }

        [Test]
        public void QuarterTest() {
            Parallel.ForEach(CultureTestData.Default,
                             culture => {
                                 var calendar = culture.Calendar;
                                 foreach(QuarterKind yearQuarter in Enum.GetValues(typeof(QuarterKind))) {
                                     var months = TimeTool.GetMonthsOfQuarter(yearQuarter);
                                     var duration = TimeSpan.Zero;

                                     foreach(var month in months) {
                                         int monthDays = calendar.GetDaysInMonth(currentYear, month);
                                         duration += TimeSpan.FromDays(monthDays);
                                     }

                                     DurationUtil.Quarter(currentYear, yearQuarter).Should().Be(duration);
                                     DurationUtil.Quarter(currentYear, yearQuarter, calendar).Should().Be(duration);
                                 }
                             });
        }
#endif

        [Test]
        public void MonthTest() {
            CultureTestData.Default
                .RunEach(culture => {
                             var calendar = culture.Calendar;
                             for(int i = 1; i <= TimeSpec.MonthsPerYear; i++) {
                                 DurationUtil.Month(currentYear, i).Should().Be(
                                     TimeSpan.FromDays(calendar.GetDaysInMonth(currentYear, i)));
                                 DurationUtil.Month(currentYear, i, calendar).Should().Be(
                                     TimeSpan.FromDays(calendar.GetDaysInMonth(currentYear, i)));
                             }
                         });
        }

        [Test]
        public void WeekTest() {
            DurationUtil.Week.Should().Be(TimeSpan.FromDays(TimeSpec.DaysPerWeek));

            DurationUtil.Weeks(0).Should().Be(TimeSpan.Zero);
            DurationUtil.Weeks(1).Should().Be(TimeSpan.FromDays(TimeSpec.DaysPerWeek));
            DurationUtil.Weeks(2).Should().Be(TimeSpan.FromDays(TimeSpec.DaysPerWeek * 2));

            DurationUtil.Weeks(-1).Should().Be(TimeSpan.FromDays(TimeSpec.DaysPerWeek * -1));
            DurationUtil.Weeks(-2).Should().Be(TimeSpan.FromDays(TimeSpec.DaysPerWeek * -2));
        }

        [Test]
        public void DayTest() {
            DurationUtil.Day.Should().Be(TimeSpan.FromDays(1));

            DurationUtil.Days(0).Should().Be(TimeSpan.Zero);
            DurationUtil.Days(1).Should().Be(TimeSpan.FromDays(1));
            DurationUtil.Days(2).Should().Be(TimeSpan.FromDays(2));
            DurationUtil.Days(-1).Should().Be(TimeSpan.FromDays(-1));
            DurationUtil.Days(-2).Should().Be(TimeSpan.FromDays(-2));

            DurationUtil.Days(1, 23).Should().Be(new TimeSpan(1, 23, 0, 0));
            DurationUtil.Days(1, 23, 22).Should().Be(new TimeSpan(1, 23, 22, 0));
            DurationUtil.Days(1, 23, 22, 55).Should().Be(new TimeSpan(1, 23, 22, 55));
            DurationUtil.Days(1, 23, 22, 55, 496).Should().Be(new TimeSpan(1, 23, 22, 55, 496));
        }

        [Test]
        public void HourTest() {
            DurationUtil.Hour.Should().Be(TimeSpan.FromHours(1));

            DurationUtil.Hours(0).Should().Be(TimeSpan.Zero);
            DurationUtil.Hours(1).Should().Be(TimeSpan.FromHours(1));
            DurationUtil.Hours(2).Should().Be(TimeSpan.FromHours(2));
            DurationUtil.Hours(-1).Should().Be(TimeSpan.FromHours(-1));
            DurationUtil.Hours(-2).Should().Be(TimeSpan.FromHours(-2));

            DurationUtil.Hours(23).Should().Be(new TimeSpan(0, 23, 0, 0));
            DurationUtil.Hours(23, 22).Should().Be(new TimeSpan(0, 23, 22, 0));
            DurationUtil.Hours(23, 22, 55).Should().Be(new TimeSpan(0, 23, 22, 55));
            DurationUtil.Hours(23, 22, 55, 496).Should().Be(new TimeSpan(0, 23, 22, 55, 496));
        }

        [Test]
        public void MinuteTest() {
            DurationUtil.Minute.Should().Be(TimeSpan.FromMinutes(1));

            DurationUtil.Minutes(0).Should().Be(TimeSpan.Zero);
            DurationUtil.Minutes(1).Should().Be(TimeSpan.FromMinutes(1));
            DurationUtil.Minutes(2).Should().Be(TimeSpan.FromMinutes(2));
            DurationUtil.Minutes(-1).Should().Be(TimeSpan.FromMinutes(-1));
            DurationUtil.Minutes(-2).Should().Be(TimeSpan.FromMinutes(-2));

            DurationUtil.Minutes(22).Should().Be(new TimeSpan(0, 0, 22, 0));
            DurationUtil.Minutes(22, 55).Should().Be(new TimeSpan(0, 0, 22, 55));
            DurationUtil.Minutes(22, 55, 496).Should().Be(new TimeSpan(0, 0, 22, 55, 496));
        }

        [Test]
        public void SecondTest() {
            DurationUtil.Second.Should().Be(TimeSpan.FromSeconds(1));

            DurationUtil.Seconds(0).Should().Be(TimeSpan.Zero);
            DurationUtil.Seconds(1).Should().Be(TimeSpan.FromSeconds(1));
            DurationUtil.Seconds(2).Should().Be(TimeSpan.FromSeconds(2));
            DurationUtil.Seconds(-1).Should().Be(TimeSpan.FromSeconds(-1));
            DurationUtil.Seconds(-2).Should().Be(TimeSpan.FromSeconds(-2));

            DurationUtil.Seconds(18).Should().Be(new TimeSpan(0, 0, 0, 18));
            DurationUtil.Seconds(18, 496).Should().Be(new TimeSpan(0, 0, 0, 18, 496));
        }

        [Test]
        public void MillisecondTest() {
            DurationUtil.Millisecond.Should().Be(TimeSpan.FromMilliseconds(1));

            DurationUtil.Milliseconds(0).Should().Be(TimeSpan.Zero);
            DurationUtil.Milliseconds(1).Should().Be(TimeSpan.FromMilliseconds(1));
            DurationUtil.Milliseconds(2).Should().Be(TimeSpan.FromMilliseconds(2));

            DurationUtil.Milliseconds(-1).Should().Be(TimeSpan.FromMilliseconds(-1));
            DurationUtil.Milliseconds(-2).Should().Be(TimeSpan.FromMilliseconds(-2));
        }
    }
}