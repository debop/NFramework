using System.Linq;
using NSoft.NFramework.LinqEx;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.TimePeriods.TimeRanges {
    [TestFixture]
    public class MonthRangeCollectionFixture : TimePeriodFixtureBase {
        [Test]
        public void SingleMonthsTest() {
            const int startYear = 2004;
            const int startMonth = 6;

            var monthRanges = new MonthRangeCollection(startYear, startMonth, 1);
            monthRanges.MonthCount.Should().Be(1);

            var months = monthRanges.GetMonths().ToList();

            months.Count.Should().Be(1);
            months[0].IsSamePeriod(new MonthRange(startYear, startMonth));

            monthRanges.StartYear.Should().Be(startYear);
            monthRanges.EndYear.Should().Be(startYear);

            monthRanges.StartMonth.Should().Be(startMonth);
            monthRanges.EndMonth.Should().Be(startMonth);
        }

        [Test]
        public void CalendarMonthsTest() {
            const int startYear = 2004;
            const int startMonth = 11;
            const int monthCount = 5;

            var monthRanges = new MonthRangeCollection(startYear, startMonth, monthCount);

            Assert.AreEqual(monthRanges.MonthCount, monthCount);
            Assert.AreEqual(monthRanges.StartMonth, startMonth);
            Assert.AreEqual(monthRanges.StartYear, startYear);
            Assert.AreEqual(monthRanges.EndYear, 2005);
            Assert.AreEqual(monthRanges.EndMonth, 3);

            var months = monthRanges.GetMonths().ToList();

            Assert.AreEqual(monthCount, months.Count);
            Assert.IsTrue(months[0].IsSamePeriod(new MonthRange(2004, 11)));
            Assert.IsTrue(months[1].IsSamePeriod(new MonthRange(2004, 12)));
            Assert.IsTrue(months[2].IsSamePeriod(new MonthRange(2005, 1)));
            Assert.IsTrue(months[3].IsSamePeriod(new MonthRange(2005, 2)));
            Assert.IsTrue(months[4].IsSamePeriod(new MonthRange(2005, 3)));
        }

        [TestCase(1)]
        [TestCase(6)]
        [TestCase(48)]
        [TestCase(180)]
        [TestCase(365)]
        public void CalendarHoursTest(int? monthCount) {
            var now = ClockProxy.Clock.Now;
            var months = new MonthRangeCollection(now, monthCount ?? 12);

            var startTime = now.TrimToDay().Add(months.TimeCalendar.StartOffset);
            var endTime = startTime.AddMonths(monthCount ?? 12).Add(months.TimeCalendar.EndOffset);

            months.Start.Should().Be(startTime);
            months.End.Should().Be(endTime);

            months.MonthCount.Should().Be(monthCount);

            var items = months.GetMonths().ToList();
            items.Count.Should().Be(monthCount);

            Enumerable
                .Range(0, monthCount ?? 12)
                .RunEach(i => {
                             items[i].Start.Should().Be(startTime.AddMonths(i));
                             items[i].End.Should().Be(months.TimeCalendar.MapEnd(startTime.AddMonths(i + 1)));

                             items[i].UnmappedStart.Should().Be(startTime.AddMonths(i));
                             items[i].UnmappedEnd.Should().Be(startTime.AddMonths(i + 1));

                             items[i].IsSamePeriod(new MonthRange(months.Start.AddMonths(i))).Should().Be.True();

                             var ym = TimeTool.AddMonth(now.Year, now.Month, i);
                             items[i].IsSamePeriod(new MonthRange(ym.Year ?? 0, ym.Month ?? 1)).Should().Be.True();
                         });
        }
    }
}