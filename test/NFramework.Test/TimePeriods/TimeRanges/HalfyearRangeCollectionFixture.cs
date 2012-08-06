using System;
using System.Linq;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.TimePeriods.TimeRanges {
    [TestFixture]
    public class HalfyearRangeCollectionFixture : TimePeriodFixtureBase {
        [Test]
        public void YearBaseMonthTest() {
            var moment = new DateTime(2009, 2, 15);
            var year = TimeTool.GetYearOf(4, moment.Year, moment.Month);
            var halfyears = new HalfyearRangeCollection(moment, 3, TimeCalendar.New(4));

            halfyears.YearBaseMonth.Should().Be(4);
            halfyears.Start.Should().Be(new DateTime(year, 4, 1));
        }

        [Test]
        public void SingleHalfyearsTest() {
            const int startYear = 2004;
            const HalfyearKind startHalfyear = HalfyearKind.Second;
            var halfyears = new HalfyearRangeCollection(startYear, startHalfyear, 1);

            halfyears.YearBaseMonth.Should().Be(1);
            halfyears.HalfyearCount.Should().Be(1);
            halfyears.StartHalfyear.Should().Be(startHalfyear);
            halfyears.StartYear.Should().Be(startYear);
            halfyears.EndYear.Should().Be(startYear);
            halfyears.EndHalfyear.Should().Be(HalfyearKind.Second);

            var halfyearList = halfyears.GetHalfyears().ToList();

            halfyearList.Count.Should().Be(1);
            halfyearList[0].IsSamePeriod(new HalfyearRange(2004, HalfyearKind.Second)).Should().Be.True();
        }

        [Test]
        public void FirstCalendarHalfyearsTest() {
            const int startYear = 2004;
            const HalfyearKind startHalfyear = HalfyearKind.First;
            const int halfyearCount = 3;
            var halfyears = new HalfyearRangeCollection(startYear, startHalfyear, halfyearCount);

            halfyears.YearBaseMonth.Should().Be(1);
            halfyears.HalfyearCount.Should().Be(halfyearCount);
            halfyears.StartHalfyear.Should().Be(startHalfyear);
            halfyears.StartYear.Should().Be(startYear);
            halfyears.EndYear.Should().Be(2005);
            halfyears.EndHalfyear.Should().Be(HalfyearKind.First);

            var halfyearList = halfyears.GetHalfyears().ToList();

            halfyearList.Count.Should().Be(halfyearCount);
            Assert.IsTrue(halfyearList[0].IsSamePeriod(new HalfyearRange(2004, HalfyearKind.First)));
            Assert.IsTrue(halfyearList[1].IsSamePeriod(new HalfyearRange(2004, HalfyearKind.Second)));
            Assert.IsTrue(halfyearList[2].IsSamePeriod(new HalfyearRange(2005, HalfyearKind.First)));
        }

        [Test]
        public void SecondCalendarHalfyearsTest() {
            const int startYear = 2004;
            const HalfyearKind startHalfyear = HalfyearKind.Second;
            const int halfyearCount = 3;
            var halfyears = new HalfyearRangeCollection(startYear, startHalfyear, halfyearCount);

            halfyears.YearBaseMonth.Should().Be(1);
            halfyears.HalfyearCount.Should().Be(halfyearCount);
            halfyears.StartHalfyear.Should().Be(startHalfyear);
            halfyears.StartYear.Should().Be(startYear);
            halfyears.EndYear.Should().Be(2005);
            halfyears.EndHalfyear.Should().Be(HalfyearKind.Second);

            var halfyearList = halfyears.GetHalfyears().ToList();

            halfyearList.Count.Should().Be(halfyearCount);
            Assert.IsTrue(halfyearList[0].IsSamePeriod(new HalfyearRange(2004, HalfyearKind.Second)));
            Assert.IsTrue(halfyearList[1].IsSamePeriod(new HalfyearRange(2005, HalfyearKind.First)));
            Assert.IsTrue(halfyearList[2].IsSamePeriod(new HalfyearRange(2005, HalfyearKind.Second)));
        }

        [Test]
        public void FirstCustomCalendarHalfyearsTest() {
            var timeCalendar = TimeCalendar.New(October);
            const int startYear = 2004;
            const HalfyearKind startHalfyear = HalfyearKind.First;
            const int halfyearCount = 3;
            var halfyears = new HalfyearRangeCollection(startYear, startHalfyear, halfyearCount, timeCalendar);

            halfyears.YearBaseMonth.Should().Be(October);
            halfyears.HalfyearCount.Should().Be(halfyearCount);
            halfyears.StartHalfyear.Should().Be(startHalfyear);
            halfyears.StartYear.Should().Be(startYear);
            halfyears.EndYear.Should().Be(2005);
            halfyears.EndHalfyear.Should().Be(HalfyearKind.First);

            var halfyearList = halfyears.GetHalfyears().ToList();

            halfyearList.Count.Should().Be(halfyearCount);
            Assert.IsTrue(halfyearList[0].IsSamePeriod(new HalfyearRange(2004, HalfyearKind.First, timeCalendar)));
            Assert.IsTrue(halfyearList[1].IsSamePeriod(new HalfyearRange(2004, HalfyearKind.Second, timeCalendar)));
            Assert.IsTrue(halfyearList[2].IsSamePeriod(new HalfyearRange(2005, HalfyearKind.First, timeCalendar)));
        }

        [Test]
        public void SecondCustomCalendarHalfyearsTest() {
            var timeCalendar = TimeCalendar.New(October);
            const int startYear = 2004;
            const HalfyearKind startHalfyear = HalfyearKind.Second;
            const int halfyearCount = 3;
            HalfyearRangeCollection halfyears = new HalfyearRangeCollection(startYear, startHalfyear, halfyearCount, timeCalendar);

            halfyears.YearBaseMonth.Should().Be(October);
            halfyears.HalfyearCount.Should().Be(halfyearCount);
            halfyears.StartHalfyear.Should().Be(startHalfyear);
            halfyears.StartYear.Should().Be(startYear);
            halfyears.EndYear.Should().Be(2005);
            halfyears.EndHalfyear.Should().Be(HalfyearKind.Second);

            var halfyearList = halfyears.GetHalfyears().ToList();

            halfyearList.Count.Should().Be(halfyearCount);
            Assert.IsTrue(halfyearList[0].IsSamePeriod(new HalfyearRange(2004, HalfyearKind.Second, timeCalendar)));
            Assert.IsTrue(halfyearList[1].IsSamePeriod(new HalfyearRange(2005, HalfyearKind.First, timeCalendar)));
            Assert.IsTrue(halfyearList[2].IsSamePeriod(new HalfyearRange(2005, HalfyearKind.Second, timeCalendar)));
        }
    }
}