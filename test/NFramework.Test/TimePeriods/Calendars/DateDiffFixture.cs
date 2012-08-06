using System;
using System.Globalization;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.TimePeriods.Calendars {
    [TestFixture]
    public class DateDiffFixture : TimePeriodFixtureBase {
        [Test]
        public void DefaultsTest() {
            var now = ClockProxy.Clock.Now;
            var dateDiff = new DateDiff(now, now);

            dateDiff.YearBaseMonth.Should().Be(TimeSpec.CalendarYearStartMonth);
            dateDiff.FirstDayOfWeek.Should().Be(DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek);
        }

        [Test]
        public void EmptyDateDiffTest() {
            var now = ClockProxy.Clock.Now;

            var dateDiff = new DateDiff(now, now);

            dateDiff.IsEmpty.Should().Be.True();
            dateDiff.Difference.Should().Be(TimeSpan.Zero);

            Assert.AreEqual(0, dateDiff.Years);
            Assert.AreEqual(0, dateDiff.Quarters);
            Assert.AreEqual(0, dateDiff.Months);
            Assert.AreEqual(0, dateDiff.Weeks);
            Assert.AreEqual(0, dateDiff.Days);
            Assert.AreEqual(0, dateDiff.Hours);
            Assert.AreEqual(0, dateDiff.Minutes);
            Assert.AreEqual(0, dateDiff.Seconds);

            Assert.AreEqual(0, dateDiff.ElapsedYears);
            Assert.AreEqual(0, dateDiff.ElapsedMonths);
            Assert.AreEqual(0, dateDiff.ElapsedDays);
            Assert.AreEqual(0, dateDiff.ElapsedHours);
            Assert.AreEqual(0, dateDiff.ElapsedMinutes);
            Assert.AreEqual(0, dateDiff.ElapsedSeconds);
        }

        [Test]
        public void DifferenceTest() {
            var date1 = new DateTime(2008, 10, 12, 15, 32, 44, 243);
            var date2 = new DateTime(2010, 1, 3, 23, 22, 9, 345);

            var dateDiff = new DateDiff(date1, date2);

            Assert.AreEqual(date2.Subtract(date1), dateDiff.Difference);
        }

        [Test]
        [TestCase(1)]
        [TestCase(3)]
        [TestCase(15)]
        public void YearsTest(int? yearDiff) {
            var year = yearDiff.GetValueOrDefault(1);

            var date1 = ClockProxy.Clock.Now;
            var date2 = date1.AddYears(year);
            var date3 = date1.AddYears(-year);

            if(IsDebugEnabled)
                log.Debug("date1=[{0}], date2=[{1}], date3=[{2}]", date1, date2, date3);

            var dateDiff12 = new DateDiff(date1, date2);

            Assert.AreEqual(year, dateDiff12.ElapsedYears);
            Assert.AreEqual(0, dateDiff12.ElapsedMonths);
            Assert.AreEqual(0, dateDiff12.ElapsedDays);
            Assert.AreEqual(0, dateDiff12.ElapsedHours);
            Assert.AreEqual(0, dateDiff12.ElapsedMinutes);
            Assert.AreEqual(0, dateDiff12.ElapsedSeconds);

            Assert.AreEqual(year, dateDiff12.Years);
            Assert.AreEqual(year * TimeSpec.QuartersPerYear, dateDiff12.Quarters);
            Assert.AreEqual(year * TimeSpec.MonthsPerYear, dateDiff12.Months);
            // Assert.AreEqual(, dateDiff12.Weeks);

            var date12Days = date2.Subtract(date1).TotalDays.AsInt();

            Assert.AreEqual(date12Days, dateDiff12.Days);
            Assert.AreEqual(date12Days * TimeSpec.HoursPerDay, dateDiff12.Hours);
            Assert.AreEqual(date12Days * TimeSpec.HoursPerDay * TimeSpec.MinutesPerHour, dateDiff12.Minutes);
            Assert.AreEqual(date12Days * TimeSpec.HoursPerDay * TimeSpec.MinutesPerHour * TimeSpec.SecondsPerMinute, dateDiff12.Seconds);


            var dateDiff13 = new DateDiff(date1, date3);

            Assert.AreEqual(-year, dateDiff13.ElapsedYears);
            Assert.AreEqual(0, dateDiff13.ElapsedMonths);
            Assert.AreEqual(0, dateDiff13.ElapsedDays);
            Assert.AreEqual(0, dateDiff13.ElapsedHours);
            Assert.AreEqual(0, dateDiff13.ElapsedMinutes);
            Assert.AreEqual(0, dateDiff13.ElapsedSeconds);

            Assert.AreEqual(-year, dateDiff13.Years);
            Assert.AreEqual(-year * TimeSpec.QuartersPerYear, dateDiff13.Quarters);
            Assert.AreEqual(-year * TimeSpec.MonthsPerYear, dateDiff13.Months);

            var date13Days = date1.Subtract(date3).TotalDays.AsInt();

            Assert.AreEqual(-date13Days, dateDiff13.Days);
            Assert.AreEqual(-date13Days * TimeSpec.HoursPerDay, dateDiff13.Hours);
            Assert.AreEqual(-date13Days * TimeSpec.HoursPerDay * TimeSpec.MinutesPerHour, dateDiff13.Minutes);
            Assert.AreEqual(-date13Days * TimeSpec.HoursPerDay * TimeSpec.MinutesPerHour * TimeSpec.SecondsPerMinute, dateDiff13.Seconds);
        }

        [Test]
        public void QuartersTest() {
            var date1 = new DateTime(2011, 5, 14, 15, 32, 44, 243);
            var date2 = date1.AddMonths(TimeSpec.MonthsPerQuarter);
            var date3 = date1.AddMonths(-TimeSpec.MonthsPerQuarter);

            var dateDiff12 = new DateDiff(date1, date2);

            Assert.AreEqual(0, dateDiff12.ElapsedYears);
            Assert.AreEqual(3, dateDiff12.ElapsedMonths);
            Assert.AreEqual(0, dateDiff12.ElapsedDays);
            Assert.AreEqual(0, dateDiff12.ElapsedHours);
            Assert.AreEqual(0, dateDiff12.ElapsedMinutes);
            Assert.AreEqual(0, dateDiff12.ElapsedSeconds);

            Assert.AreEqual(0, dateDiff12.Years);
            Assert.AreEqual(1, dateDiff12.Quarters);
            Assert.AreEqual(TimeSpec.MonthsPerQuarter, dateDiff12.Months);
            Assert.AreEqual(14, dateDiff12.Weeks);
            Assert.AreEqual(92, dateDiff12.Days);
            Assert.AreEqual(92 * TimeSpec.HoursPerDay, dateDiff12.Hours);
            Assert.AreEqual(92 * TimeSpec.HoursPerDay * TimeSpec.MinutesPerHour, dateDiff12.Minutes);
            Assert.AreEqual(92 * TimeSpec.HoursPerDay * TimeSpec.MinutesPerHour * TimeSpec.SecondsPerMinute, dateDiff12.Seconds);


            var dateDiff13 = new DateDiff(date1, date3);

            Assert.AreEqual(0, dateDiff13.ElapsedYears);
            Assert.AreEqual(-3, dateDiff13.ElapsedMonths);
            Assert.AreEqual(0, dateDiff13.ElapsedDays);
            Assert.AreEqual(0, dateDiff13.ElapsedHours);
            Assert.AreEqual(0, dateDiff13.ElapsedMinutes);
            Assert.AreEqual(0, dateDiff13.ElapsedSeconds);

            Assert.AreEqual(0, dateDiff13.Years);
            Assert.AreEqual(-1, dateDiff13.Quarters);
            Assert.AreEqual(-TimeSpec.MonthsPerQuarter, dateDiff13.Months);
            Assert.AreEqual(-12, dateDiff13.Weeks);
            Assert.AreEqual(-89, dateDiff13.Days);
            Assert.AreEqual(-89 * TimeSpec.HoursPerDay, dateDiff13.Hours);
            Assert.AreEqual(-89 * TimeSpec.HoursPerDay * TimeSpec.MinutesPerHour, dateDiff13.Minutes);
            Assert.AreEqual(-89 * TimeSpec.HoursPerDay * TimeSpec.MinutesPerHour * TimeSpec.SecondsPerMinute, dateDiff13.Seconds);
        }

        [Test]
        public void MonthsTest() {
            var date1 = new DateTime(2011, 5, 14, 15, 32, 44, 243);
            var date2 = date1.AddMonths(1);
            var date3 = date1.AddMonths(-1);

            var dateDiff12 = new DateDiff(date1, date2);

            Assert.AreEqual(0, dateDiff12.ElapsedYears);
            Assert.AreEqual(1, dateDiff12.ElapsedMonths);
            Assert.AreEqual(0, dateDiff12.ElapsedDays);
            Assert.AreEqual(0, dateDiff12.ElapsedHours);
            Assert.AreEqual(0, dateDiff12.ElapsedMinutes);
            Assert.AreEqual(0, dateDiff12.ElapsedSeconds);

            Assert.AreEqual(0, dateDiff12.Years);
            Assert.AreEqual(0, dateDiff12.Quarters);
            Assert.AreEqual(1, dateDiff12.Months);
            Assert.AreEqual(5, dateDiff12.Weeks);
            Assert.AreEqual(31, dateDiff12.Days);
            Assert.AreEqual(31 * TimeSpec.HoursPerDay, dateDiff12.Hours);
            Assert.AreEqual(31 * TimeSpec.HoursPerDay * TimeSpec.MinutesPerHour, dateDiff12.Minutes);
            Assert.AreEqual(31 * TimeSpec.HoursPerDay * TimeSpec.MinutesPerHour * TimeSpec.SecondsPerMinute, dateDiff12.Seconds);


            var dateDiff13 = new DateDiff(date1, date3);

            Assert.AreEqual(0, dateDiff13.ElapsedYears);
            Assert.AreEqual(-1, dateDiff13.ElapsedMonths);
            Assert.AreEqual(0, dateDiff13.ElapsedDays);
            Assert.AreEqual(0, dateDiff13.ElapsedHours);
            Assert.AreEqual(0, dateDiff13.ElapsedMinutes);
            Assert.AreEqual(0, dateDiff13.ElapsedSeconds);

            Assert.AreEqual(0, dateDiff13.Years);
            Assert.AreEqual(0, dateDiff13.Quarters);
            Assert.AreEqual(-1, dateDiff13.Months);
            Assert.AreEqual(-4, dateDiff13.Weeks);
            Assert.AreEqual(-30, dateDiff13.Days);
            Assert.AreEqual(-30 * TimeSpec.HoursPerDay, dateDiff13.Hours);
            Assert.AreEqual(-30 * TimeSpec.HoursPerDay * TimeSpec.MinutesPerHour, dateDiff13.Minutes);
            Assert.AreEqual(-30 * TimeSpec.HoursPerDay * TimeSpec.MinutesPerHour * TimeSpec.SecondsPerMinute, dateDiff13.Seconds);
        }

        [Test]
        public void WeeksTest() {
            var date1 = new DateTime(2011, 5, 14, 15, 32, 44, 243);
            var date2 = date1.AddDays(TimeSpec.DaysPerWeek);
            var date3 = date1.AddDays(-TimeSpec.DaysPerWeek);

            var dateDiff12 = new DateDiff(date1, date2);

            Assert.AreEqual(0, dateDiff12.Years);
            Assert.AreEqual(0, dateDiff12.Quarters);
            Assert.AreEqual(0, dateDiff12.Months);
            Assert.AreEqual(1, dateDiff12.Weeks);
            Assert.AreEqual(TimeSpec.DaysPerWeek, dateDiff12.Days);
            Assert.AreEqual(TimeSpec.DaysPerWeek * TimeSpec.HoursPerDay, dateDiff12.Hours);
            Assert.AreEqual(TimeSpec.DaysPerWeek * TimeSpec.HoursPerDay * TimeSpec.MinutesPerHour, dateDiff12.Minutes);
            Assert.AreEqual(TimeSpec.DaysPerWeek * TimeSpec.HoursPerDay * TimeSpec.MinutesPerHour * TimeSpec.SecondsPerMinute,
                            dateDiff12.Seconds);


            DateDiff dateDiff13 = new DateDiff(date1, date3);

            Assert.AreEqual(0, dateDiff13.Years);
            Assert.AreEqual(0, dateDiff13.Quarters);
            Assert.AreEqual(0, dateDiff13.Months);
            Assert.AreEqual(-1, dateDiff13.Weeks);
            Assert.AreEqual(-TimeSpec.DaysPerWeek, dateDiff13.Days);
            Assert.AreEqual(-TimeSpec.DaysPerWeek * TimeSpec.HoursPerDay, dateDiff13.Hours);
            Assert.AreEqual(-TimeSpec.DaysPerWeek * TimeSpec.HoursPerDay * TimeSpec.MinutesPerHour, dateDiff13.Minutes);
            Assert.AreEqual(-TimeSpec.DaysPerWeek * TimeSpec.HoursPerDay * TimeSpec.MinutesPerHour * TimeSpec.SecondsPerMinute,
                            dateDiff13.Seconds);
        }

        [Test]
        [TestCase(1)]
        [TestCase(3)]
        public void DaysTest(int? dayDiff) {
            var days = dayDiff.GetValueOrDefault(1);

            var date1 = new DateTime(2011, 5, 18, 15, 32, 44, 243);
            var date2 = date1.AddDays(days);
            var date3 = date1.AddDays(-days);

            var dateDiff12 = new DateDiff(date1, date2);

            Assert.AreEqual(0, dateDiff12.ElapsedYears);
            Assert.AreEqual(0, dateDiff12.ElapsedMonths);
            Assert.AreEqual(days, dateDiff12.ElapsedDays);
            Assert.AreEqual(0, dateDiff12.ElapsedHours);
            Assert.AreEqual(0, dateDiff12.ElapsedMinutes);
            Assert.AreEqual(0, dateDiff12.ElapsedSeconds);

            Assert.AreEqual(0, dateDiff12.Years);
            Assert.AreEqual(0, dateDiff12.Quarters);
            Assert.AreEqual(0, dateDiff12.Months);
            Assert.AreEqual(0, dateDiff12.Weeks);
            Assert.AreEqual(days, dateDiff12.Days);
            Assert.AreEqual(days * TimeSpec.HoursPerDay, dateDiff12.Hours);
            Assert.AreEqual(days * TimeSpec.HoursPerDay * TimeSpec.MinutesPerHour, dateDiff12.Minutes);
            Assert.AreEqual(days * TimeSpec.HoursPerDay * TimeSpec.MinutesPerHour * TimeSpec.SecondsPerMinute, dateDiff12.Seconds);


            var dateDiff13 = new DateDiff(date1, date3);

            Assert.AreEqual(0, dateDiff13.ElapsedYears);
            Assert.AreEqual(0, dateDiff13.ElapsedMonths);
            Assert.AreEqual(-days, dateDiff13.ElapsedDays);
            Assert.AreEqual(0, dateDiff13.ElapsedHours);
            Assert.AreEqual(0, dateDiff13.ElapsedMinutes);
            Assert.AreEqual(0, dateDiff13.ElapsedSeconds);

            Assert.AreEqual(0, dateDiff13.Years);
            Assert.AreEqual(0, dateDiff13.Quarters);
            Assert.AreEqual(0, dateDiff13.Months);
            Assert.AreEqual(0, dateDiff13.Weeks);
            Assert.AreEqual(-days, dateDiff13.Days);
            Assert.AreEqual(-days * TimeSpec.HoursPerDay, dateDiff13.Hours);
            Assert.AreEqual(-days * TimeSpec.HoursPerDay * TimeSpec.MinutesPerHour, dateDiff13.Minutes);
            Assert.AreEqual(-days * TimeSpec.HoursPerDay * TimeSpec.MinutesPerHour * TimeSpec.SecondsPerMinute, dateDiff13.Seconds);
        }

        [Test]
        [TestCase(1)]
        [TestCase(3)]
        [TestCase(5)]
        public void HoursTest(int? hourDiff) {
            var hours = hourDiff.GetValueOrDefault(1);

            var date1 = new DateTime(2011, 5, 14, 15, 32, 44, 243);
            var date2 = date1.AddHours(hours);
            var date3 = date1.AddHours(-hours);

            var dateDiff12 = new DateDiff(date1, date2);

            Assert.AreEqual(0, dateDiff12.ElapsedYears);
            Assert.AreEqual(0, dateDiff12.ElapsedMonths);
            Assert.AreEqual(0, dateDiff12.ElapsedDays);
            Assert.AreEqual(hours, dateDiff12.ElapsedHours);
            Assert.AreEqual(0, dateDiff12.ElapsedMinutes);
            Assert.AreEqual(0, dateDiff12.ElapsedSeconds);

            Assert.AreEqual(0, dateDiff12.Years);
            Assert.AreEqual(0, dateDiff12.Quarters);
            Assert.AreEqual(0, dateDiff12.Months);
            Assert.AreEqual(0, dateDiff12.Weeks);
            Assert.AreEqual(0, dateDiff12.Days);
            Assert.AreEqual(hours, dateDiff12.Hours);
            Assert.AreEqual(hours * TimeSpec.MinutesPerHour, dateDiff12.Minutes);
            Assert.AreEqual(hours * TimeSpec.MinutesPerHour * TimeSpec.SecondsPerMinute, dateDiff12.Seconds);


            var dateDiff13 = new DateDiff(date1, date3);

            Assert.AreEqual(0, dateDiff13.ElapsedYears);
            Assert.AreEqual(0, dateDiff13.ElapsedMonths);
            Assert.AreEqual(0, dateDiff13.ElapsedDays);
            Assert.AreEqual(-hours, dateDiff13.ElapsedHours);
            Assert.AreEqual(0, dateDiff13.ElapsedMinutes);
            Assert.AreEqual(0, dateDiff13.ElapsedSeconds);

            Assert.AreEqual(0, dateDiff13.Years);
            Assert.AreEqual(0, dateDiff13.Quarters);
            Assert.AreEqual(0, dateDiff13.Months);
            Assert.AreEqual(0, dateDiff13.Weeks);
            Assert.AreEqual(0, dateDiff13.Days);
            Assert.AreEqual(-hours, dateDiff13.Hours);
            Assert.AreEqual(-hours * TimeSpec.MinutesPerHour, dateDiff13.Minutes);
            Assert.AreEqual(-hours * TimeSpec.MinutesPerHour * TimeSpec.SecondsPerMinute, dateDiff13.Seconds);
        }

        [Test]
        [TestCase(1)]
        [TestCase(3)]
        [TestCase(5)]
        public void MinutesTest(int? minuteDiff) {
            var minutes = minuteDiff.GetValueOrDefault(1);

            var date1 = new DateTime(2011, 5, 14, 15, 32, 44, 243);
            var date2 = date1.AddMinutes(minutes);
            var date3 = date1.AddMinutes(-minutes);

            var dateDiff12 = new DateDiff(date1, date2);

            Assert.AreEqual(0, dateDiff12.ElapsedYears);
            Assert.AreEqual(0, dateDiff12.ElapsedMonths);
            Assert.AreEqual(0, dateDiff12.ElapsedDays);
            Assert.AreEqual(0, dateDiff12.ElapsedHours);
            Assert.AreEqual(minutes, dateDiff12.ElapsedMinutes);
            Assert.AreEqual(0, dateDiff12.ElapsedSeconds);

            Assert.AreEqual(0, dateDiff12.Years);
            Assert.AreEqual(0, dateDiff12.Quarters);
            Assert.AreEqual(0, dateDiff12.Months);
            Assert.AreEqual(0, dateDiff12.Weeks);
            Assert.AreEqual(0, dateDiff12.Days);
            Assert.AreEqual(0, dateDiff12.Hours);
            Assert.AreEqual(minutes, dateDiff12.Minutes);
            Assert.AreEqual(minutes * TimeSpec.SecondsPerMinute, dateDiff12.Seconds);


            var dateDiff13 = new DateDiff(date1, date3);

            Assert.AreEqual(0, dateDiff13.ElapsedYears);
            Assert.AreEqual(0, dateDiff13.ElapsedMonths);
            Assert.AreEqual(0, dateDiff13.ElapsedDays);
            Assert.AreEqual(0, dateDiff13.ElapsedHours);
            Assert.AreEqual(-minutes, dateDiff13.ElapsedMinutes);
            Assert.AreEqual(0, dateDiff13.ElapsedSeconds);

            Assert.AreEqual(0, dateDiff13.Years);
            Assert.AreEqual(0, dateDiff13.Quarters);
            Assert.AreEqual(0, dateDiff13.Months);
            Assert.AreEqual(0, dateDiff13.Weeks);
            Assert.AreEqual(0, dateDiff13.Days);
            Assert.AreEqual(0, dateDiff13.Hours);
            Assert.AreEqual(-minutes, dateDiff13.Minutes);
            Assert.AreEqual(-minutes * TimeSpec.SecondsPerMinute, dateDiff13.Seconds);
        }

        [Test]
        [TestCase(1)]
        [TestCase(3)]
        [TestCase(5)]
        public void SecondsTest(int? secondDiff) {
            var seconds = secondDiff.GetValueOrDefault(1);

            DateTime date1 = new DateTime(2011, 5, 14, 15, 32, 44, 243);
            DateTime date2 = date1.AddSeconds(seconds);
            DateTime date3 = date1.AddSeconds(-seconds);

            var dateDiff12 = new DateDiff(date1, date2);

            Assert.AreEqual(0, dateDiff12.ElapsedYears);
            Assert.AreEqual(0, dateDiff12.ElapsedMonths);
            Assert.AreEqual(0, dateDiff12.ElapsedDays);
            Assert.AreEqual(0, dateDiff12.ElapsedHours);
            Assert.AreEqual(0, dateDiff12.ElapsedMinutes);
            Assert.AreEqual(seconds, dateDiff12.ElapsedSeconds);

            Assert.AreEqual(0, dateDiff12.Years);
            Assert.AreEqual(0, dateDiff12.Quarters);
            Assert.AreEqual(0, dateDiff12.Months);
            Assert.AreEqual(0, dateDiff12.Weeks);
            Assert.AreEqual(0, dateDiff12.Days);
            Assert.AreEqual(0, dateDiff12.Hours);
            Assert.AreEqual(0, dateDiff12.Minutes);
            Assert.AreEqual(seconds, dateDiff12.Seconds);


            var dateDiff13 = new DateDiff(date1, date3);

            Assert.AreEqual(0, dateDiff13.ElapsedYears);
            Assert.AreEqual(0, dateDiff13.ElapsedMonths);
            Assert.AreEqual(0, dateDiff13.ElapsedDays);
            Assert.AreEqual(0, dateDiff13.ElapsedHours);
            Assert.AreEqual(0, dateDiff13.ElapsedMinutes);
            Assert.AreEqual(-seconds, dateDiff13.ElapsedSeconds);

            Assert.AreEqual(0, dateDiff13.Years);
            Assert.AreEqual(0, dateDiff13.Quarters);
            Assert.AreEqual(0, dateDiff13.Months);
            Assert.AreEqual(0, dateDiff13.Weeks);
            Assert.AreEqual(0, dateDiff13.Days);
            Assert.AreEqual(0, dateDiff13.Hours);
            Assert.AreEqual(0, dateDiff13.Minutes);
            Assert.AreEqual(-seconds, dateDiff13.Seconds);
        }

        [Test]
        [TestCase(1)]
        [TestCase(3)]
        [TestCase(5)]
        public void PositiveDurationTest(int? diff) {
            var added = diff.GetValueOrDefault(1);

            var date1 = ClockProxy.Clock.Now; //new DateTime(2011, 5, 14, 15, 32, 44, 243);
            var date2 = date1.AddYears(added).AddMonths(added).AddDays(added).AddHours(added).AddMinutes(added).AddSeconds(added);

            DateDiff dateDiff = new DateDiff(date1, date2);

            Assert.AreEqual(added, dateDiff.ElapsedYears);
            Assert.AreEqual(added, dateDiff.ElapsedMonths);
            Assert.AreEqual(added, dateDiff.ElapsedDays);
            Assert.AreEqual(added, dateDiff.ElapsedHours);
            Assert.AreEqual(added, dateDiff.ElapsedMinutes);
            Assert.AreEqual(added, dateDiff.ElapsedSeconds);
        }

        [Test]
        [TestCase(1)]
        [TestCase(3)]
        [TestCase(5)]
        public void NegativeDurationTest(int? diff) {
            var subtracted = -diff.GetValueOrDefault(1);

            var date1 = ClockProxy.Clock.Now; //new DateTime(2011, 5, 14, 15, 32, 44, 243);
            var date2 =
                date1.AddYears(subtracted).AddMonths(subtracted).AddDays(subtracted).AddHours(subtracted).AddMinutes(subtracted).
                    AddSeconds(subtracted);

            DateDiff dateDiff = new DateDiff(date1, date2);

            Assert.AreEqual(subtracted, dateDiff.ElapsedYears);
            Assert.AreEqual(subtracted, dateDiff.ElapsedMonths);
            Assert.AreEqual(subtracted, dateDiff.ElapsedDays);
            Assert.AreEqual(subtracted, dateDiff.ElapsedHours);
            Assert.AreEqual(subtracted, dateDiff.ElapsedMinutes);
            Assert.AreEqual(subtracted, dateDiff.ElapsedSeconds);
        }
    }
}