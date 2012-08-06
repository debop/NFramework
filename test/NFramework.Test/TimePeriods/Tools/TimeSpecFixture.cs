using System;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.TimePeriods.Tools {
    [Microsoft.Silverlight.Testing.Tag("TimePeriods")]
    [TestFixture]
    public class TimeSpecFixture : TimePeriodFixtureBase {
        [Test]
        public void DateUnitTest() {
            // relations
            TimeSpec.MonthsPerYear.Should().Be(12);
            TimeSpec.HalfyearsPerYear.Should().Be(2);
            TimeSpec.QuartersPerYear.Should().Be(4);
            TimeSpec.QuartersPerHalfyear.Should().Be(TimeSpec.QuartersPerYear / TimeSpec.HalfyearsPerYear);
            TimeSpec.MaxWeeksPerYear.Should().Be(54);
            TimeSpec.MonthsPerHalfyear.Should().Be(TimeSpec.MonthsPerYear / TimeSpec.HalfyearsPerYear);
            TimeSpec.MonthsPerQuarter.Should().Be(TimeSpec.MonthsPerYear / TimeSpec.QuartersPerYear);
            TimeSpec.MaxDaysPerMonth.Should().Be(31);
            TimeSpec.DaysPerWeek.Should().Be(7);
            TimeSpec.HoursPerDay.Should().Be(24);
            TimeSpec.MinutesPerHour.Should().Be(60);
            TimeSpec.SecondsPerMinute.Should().Be(60);
            TimeSpec.MillisecondsPerSecond.Should().Be(1000);
        }

        [Test]
        public void HalfYearTest() {
            TimeSpec.FirstHalfyearMonths.Length.Should().Be(TimeSpec.MonthsPerHalfyear);

            for(int i = 0; i < TimeSpec.FirstHalfyearMonths.Length; i++) {
                TimeSpec.FirstHalfyearMonths[i].Should().Be(i + 1);
            }

            TimeSpec.SecondHalfyearMonths.Length.Should().Be(TimeSpec.MonthsPerHalfyear);

            for(int i = 0; i < TimeSpec.FirstHalfyearMonths.Length; i++) {
                TimeSpec.SecondHalfyearMonths[i].Should().Be(i + 7);
            }
        }

        [Test]
        public void QuarterTest() {
            TimeSpec.FirstQuarterMonth.Should().Be(1);
            TimeSpec.SecondQuarterMonth.Should().Be(TimeSpec.FirstQuarterMonth + TimeSpec.MonthsPerQuarter);
            TimeSpec.ThirdQuarterMonth.Should().Be(TimeSpec.SecondQuarterMonth + TimeSpec.MonthsPerQuarter);
            TimeSpec.FourthQuarterMonth.Should().Be(TimeSpec.ThirdQuarterMonth + TimeSpec.MonthsPerQuarter);


            TimeSpec.FirstQuarterMonths.Length.Should().Be(TimeSpec.MonthsPerQuarter);

            for(int i = 0; i < TimeSpec.FirstQuarterMonths.Length; i++) {
                TimeSpec.FirstQuarterMonths[i].Should().Be(i + 1);
            }

            TimeSpec.SecondQuarterMonths.Length.Should().Be(TimeSpec.MonthsPerQuarter);

            for(int i = 0; i < TimeSpec.SecondQuarterMonths.Length; i++) {
                TimeSpec.SecondQuarterMonths[i].Should().Be(i + 4);
            }

            TimeSpec.ThirdQuarterMonths.Length.Should().Be(TimeSpec.MonthsPerQuarter);

            for(int i = 0; i < TimeSpec.ThirdQuarterMonths.Length; i++) {
                TimeSpec.ThirdQuarterMonths[i].Should().Be(i + 7);
            }

            TimeSpec.FourthQuarterMonths.Length.Should().Be(TimeSpec.MonthsPerQuarter);

            for(int i = 0; i < TimeSpec.FourthQuarterMonths.Length; i++) {
                TimeSpec.FourthQuarterMonths[i].Should().Be(i + 10);
            }
        }

        [Test]
        public void DurationTest() {
            TimeSpec.NoDuration.Should().Be(TimeSpan.Zero);
            TimeSpec.EmptyDuration.Should().Be(TimeSpan.Zero);
            TimeSpec.MinPositiveDuration.Should().Be(new TimeSpan(1));
            TimeSpec.MinNegativeDuration.Should().Be(new TimeSpan(-1));
        }

        [Test]
        public void PeriodTest() {
            TimeSpec.MinPeriodTime.Should().Be(DateTime.MinValue);
            TimeSpec.MaxPeriodTime.Should().Be(DateTime.MaxValue);

            TimeSpec.MinPeriodDuration.Should().Be(TimeSpan.Zero);
            TimeSpec.MaxPeriodDuration.Should().Be(TimeSpec.MaxPeriodTime - TimeSpec.MinPeriodTime);
        }
    }
}