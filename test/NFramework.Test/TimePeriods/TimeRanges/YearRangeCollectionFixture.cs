using System;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.TimePeriods.TimeRanges {
    [TestFixture]
    public class YearRangeCollectionFixture : TimePeriodFixtureBase {
        [Test]
        public void YearBaseMonthTest() {
            var moment = new DateTime(2009, 2, 15);
            var year = TimeTool.GetYearOf(April, moment.Year, moment.Month);
            var yearRanges = new YearRangeCollection(moment, 3, TimeCalendar.New(April));

            yearRanges.YearBaseMonth.Should().Be(April);
            yearRanges.Start.Should().Be(new DateTime(year, April, 1));
        }

        [Test]
        public void SingleYearsTest() {
            const int startYear = 2004;
            var yearRanges = new YearRangeCollection(startYear, 1);

            yearRanges.YearCount.Should().Be(1);
            yearRanges.StartYear.Should().Be(startYear);
            yearRanges.EndYear.Should().Be(startYear);

            var years = yearRanges.GetYears().ToTimePeriodCollection();

            years.Count.Should().Be(1);
            years[0].IsSamePeriod(new YearRange(startYear)).Should().Be.True();
        }

        [Test]
        public void DefaultCalendarYearsTest() {
            const int startYear = 2004;
            const int yearCount = 3;
            var years = new YearRangeCollection(startYear, yearCount);

            years.YearCount.Should().Be(yearCount);
            years.StartYear.Should().Be(startYear);
            years.EndYear.Should().Be(startYear + yearCount - 1);

            int index = 0;
            foreach(var year in years.GetYears()) {
                year.IsSamePeriod(new YearRange(startYear + index)).Should().Be.True();
                index++;
            }
        }

        [Test]
        public void CustomCalendarYearsTest() {
            const int startYear = 2004;
            const int yearCount = 3;
            const int startMonth = 4;

            var years = new YearRangeCollection(startYear, yearCount, TimeCalendar.New(startMonth));

            years.YearCount.Should().Be(yearCount);
            years.StartYear.Should().Be(startYear);
            years.EndYear.Should().Be(startYear + yearCount);

            var index = 0;

            foreach(var year in years.GetYears()) {
                year.Start.Should().Be(new DateTime(startYear + index, startMonth, 1));
                index++;
            }
        }
    }
}