using System;
using System.Linq;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.TimePeriods.TimeRanges {
    [TestFixture]
    public class QuarterRangeCollectionFixture : TimePeriodFixtureBase {
        [Test]
        public void YearBaseMonthTest() {
            var moment = new DateTime(2009, 2, 15);
            var year = TimeTool.GetYearOf(April, moment.Year, moment.Month);
            var quarters = new QuarterRangeCollection(moment, 3, TimeCalendar.New(April));

            quarters.YearBaseMonth.Should().Be(April);
            quarters.Start.Should().Be(new DateTime(year, April, 1));
        }

        [Test]
        public void SingleQuartersTest() {
            const int startYear = 2004;
            const QuarterKind startQuarter = QuarterKind.Second;

            var quarterRanges = new QuarterRangeCollection(startYear, startQuarter, 1);

            quarterRanges.YearBaseMonth.Should().Be(1);
            quarterRanges.QuarterCount.Should().Be(1);
            quarterRanges.StartQuarter.Should().Be(startQuarter);
            quarterRanges.StartYear.Should().Be(startYear);
            quarterRanges.EndYear.Should().Be(startYear);
            quarterRanges.EndQuarter.Should().Be(QuarterKind.Second);

            var quarters = quarterRanges.GetQuarters().ToTimePeriodCollection();

            quarters.Count.Should().Be(1);
            quarters[0].IsSamePeriod(new QuarterRange(2004, QuarterKind.Second)).Should().Be.True();
        }

        [Test]
        public void FirstCalendarQuartersTest() {
            const int startYear = 2004;
            const QuarterKind startQuarter = QuarterKind.First;
            const int quarterCount = 5;

            var quarterRanges = new QuarterRangeCollection(startYear, startQuarter, quarterCount);

            quarterRanges.YearBaseMonth.Should().Be(1);
            quarterRanges.QuarterCount.Should().Be(quarterCount);
            quarterRanges.StartQuarter.Should().Be(startQuarter);
            quarterRanges.StartYear.Should().Be(startYear);
            quarterRanges.EndYear.Should().Be(2005);
            quarterRanges.EndQuarter.Should().Be(QuarterKind.First);

            var quarters = quarterRanges.GetQuarters().ToTimePeriodCollection();

            quarters.Count.Should().Be(quarterCount);
            quarters[0].IsSamePeriod(new QuarterRange(2004, QuarterKind.First)).Should().Be.True();
            quarters[1].IsSamePeriod(new QuarterRange(2004, QuarterKind.Second)).Should().Be.True();
            quarters[2].IsSamePeriod(new QuarterRange(2004, QuarterKind.Third)).Should().Be.True();
            quarters[3].IsSamePeriod(new QuarterRange(2004, QuarterKind.Fourth)).Should().Be.True();
            quarters[4].IsSamePeriod(new QuarterRange(2005, QuarterKind.First)).Should().Be.True();
        }

        [Test]
        public void SecondCalendarQuartersTest() {
            const int startYear = 2004;
            const QuarterKind startQuarter = QuarterKind.Second;
            const int quarterCount = 5;

            var quarterRanges = new QuarterRangeCollection(startYear, startQuarter, quarterCount);

            quarterRanges.YearBaseMonth.Should().Be(1);
            quarterRanges.QuarterCount.Should().Be(quarterCount);
            quarterRanges.StartQuarter.Should().Be(startQuarter);
            quarterRanges.StartYear.Should().Be(startYear);
            quarterRanges.EndYear.Should().Be(2005);
            quarterRanges.EndQuarter.Should().Be(QuarterKind.Second);

            var quarters = quarterRanges.GetQuarters().ToTimePeriodCollection();

            quarterCount.Should().Be(quarters.Count);
            quarters[0].IsSamePeriod(new QuarterRange(2004, QuarterKind.Second)).Should().Be.True();
            quarters[1].IsSamePeriod(new QuarterRange(2004, QuarterKind.Third)).Should().Be.True();
            quarters[2].IsSamePeriod(new QuarterRange(2004, QuarterKind.Fourth)).Should().Be.True();
            quarters[3].IsSamePeriod(new QuarterRange(2005, QuarterKind.First)).Should().Be.True();
            quarters[4].IsSamePeriod(new QuarterRange(2005, QuarterKind.Second)).Should().Be.True();
        }

        [Test]
        public void FirstCustomCalendarQuartersTest() {
            var calendar = TimeCalendar.New(October);
            const int startYear = 2004;
            const QuarterKind startQuarter = QuarterKind.First;
            const int quarterCount = 5;

            var quarterRanges = new QuarterRangeCollection(startYear, startQuarter, quarterCount, calendar);

            quarterRanges.YearBaseMonth.Should().Be(October);
            quarterRanges.QuarterCount.Should().Be(quarterCount);
            quarterRanges.StartQuarter.Should().Be(startQuarter);
            quarterRanges.StartYear.Should().Be(startYear);
            quarterRanges.EndYear.Should().Be(2005);
            quarterRanges.EndQuarter.Should().Be(QuarterKind.First);

            var quarters = quarterRanges.GetQuarters().ToTimePeriodCollection();

            quarters.Count.Should().Be(quarterCount);
            quarters[0].IsSamePeriod(new QuarterRange(2004, QuarterKind.First, calendar)).Should().Be.True();
            quarters[1].IsSamePeriod(new QuarterRange(2004, QuarterKind.Second, calendar)).Should().Be.True();
            quarters[2].IsSamePeriod(new QuarterRange(2004, QuarterKind.Third, calendar)).Should().Be.True();
            quarters[3].IsSamePeriod(new QuarterRange(2004, QuarterKind.Fourth, calendar)).Should().Be.True();
            quarters[4].IsSamePeriod(new QuarterRange(2005, QuarterKind.First, calendar)).Should().Be.True();
        }

        [Test]
        public void SecondCustomCalendarQuartersTest() {
            var calendar = TimeCalendar.New(October);
            const int startYear = 2004;
            const QuarterKind startQuarter = QuarterKind.Second;
            const int quarterCount = 5;

            var quarterRanges = new QuarterRangeCollection(startYear, startQuarter, quarterCount, calendar);

            quarterRanges.YearBaseMonth.Should().Be(October);
            quarterRanges.QuarterCount.Should().Be(quarterCount);
            quarterRanges.StartQuarter.Should().Be(startQuarter);
            quarterRanges.StartYear.Should().Be(startYear);
            quarterRanges.EndYear.Should().Be(2005);
            quarterRanges.EndQuarter.Should().Be(QuarterKind.Second);

            var quarters = quarterRanges.GetQuarters().ToTimePeriodCollection();

            quarters.Count.Should().Be(quarterCount);
            quarters[0].IsSamePeriod(new QuarterRange(2004, QuarterKind.Second, calendar)).Should().Be.True();
            quarters[1].IsSamePeriod(new QuarterRange(2004, QuarterKind.Third, calendar)).Should().Be.True();
            quarters[2].IsSamePeriod(new QuarterRange(2004, QuarterKind.Fourth, calendar)).Should().Be.True();
            quarters[3].IsSamePeriod(new QuarterRange(2005, QuarterKind.First, calendar)).Should().Be.True();
            quarters[4].IsSamePeriod(new QuarterRange(2005, QuarterKind.Second, calendar)).Should().Be.True();
        }

        [Test]
        public void ThirdCustomCalendarQuartersTest() {
            var calendar = TimeCalendar.New(October);
            const int startYear = 2004;
            const QuarterKind startQuarter = QuarterKind.Third;
            const int quarterCount = 5;

            var quarterRanges = new QuarterRangeCollection(startYear, startQuarter, quarterCount, calendar);

            quarterRanges.YearBaseMonth.Should().Be(October);
            quarterRanges.QuarterCount.Should().Be(quarterCount);
            quarterRanges.StartQuarter.Should().Be(startQuarter);
            quarterRanges.StartYear.Should().Be(startYear);
            quarterRanges.EndYear.Should().Be(2005);
            quarterRanges.EndQuarter.Should().Be(QuarterKind.Third);

            var quarters = quarterRanges.GetQuarters().ToTimePeriodCollection();

            quarters.Count.Should().Be(quarterCount);
            quarters[0].IsSamePeriod(new QuarterRange(2004, QuarterKind.Third, calendar)).Should().Be.True();
            quarters[1].IsSamePeriod(new QuarterRange(2004, QuarterKind.Fourth, calendar)).Should().Be.True();
            quarters[2].IsSamePeriod(new QuarterRange(2005, QuarterKind.First, calendar)).Should().Be.True();
            quarters[3].IsSamePeriod(new QuarterRange(2005, QuarterKind.Second, calendar)).Should().Be.True();
            quarters[4].IsSamePeriod(new QuarterRange(2005, QuarterKind.Third, calendar)).Should().Be.True();
        }

        [Test]
        public void FourthCustomCalendarQuartersTest() {
            var calendar = TimeCalendar.New(October);
            const int startYear = 2004;
            const QuarterKind startQuarter = QuarterKind.Fourth;
            const int quarterCount = 5;

            var quarterRanges = new QuarterRangeCollection(startYear, startQuarter, quarterCount, calendar);

            quarterRanges.YearBaseMonth.Should().Be(October);
            quarterRanges.QuarterCount.Should().Be(quarterCount);
            quarterRanges.StartQuarter.Should().Be(startQuarter);
            quarterRanges.StartYear.Should().Be(startYear);
            quarterRanges.EndYear.Should().Be(2005);
            quarterRanges.EndQuarter.Should().Be(QuarterKind.Fourth);

            var quarters = quarterRanges.GetQuarters().ToList();

            quarters.Count.Should().Be(quarterCount);
            quarters[0].IsSamePeriod(new QuarterRange(2004, QuarterKind.Fourth, calendar)).Should().Be.True();
            quarters[1].IsSamePeriod(new QuarterRange(2005, QuarterKind.First, calendar)).Should().Be.True();
            quarters[2].IsSamePeriod(new QuarterRange(2005, QuarterKind.Second, calendar)).Should().Be.True();
            quarters[3].IsSamePeriod(new QuarterRange(2005, QuarterKind.Third, calendar)).Should().Be.True();
            quarters[4].IsSamePeriod(new QuarterRange(2005, QuarterKind.Fourth, calendar)).Should().Be.True();
        }
    }
}