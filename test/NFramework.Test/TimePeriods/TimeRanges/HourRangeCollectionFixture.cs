using System.Linq;
using NSoft.NFramework.LinqEx;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.TimePeriods.TimeRanges {
    [TestFixture]
    public class HourRangeCollectionFixture : TimePeriodFixtureBase {
        [Test]
        public void SingleHoursTest() {
            const int startYear = 2004;
            const int startMonth = 2;
            const int startDay = 22;
            const int startHour = 17;

            var hourRanges = new HourRangeCollection(startYear, startMonth, startDay, startHour, 1);

            hourRanges.HourCount.Should().Be(1);
            hourRanges.StartYear.Should().Be(startYear);
            hourRanges.StartMonth.Should().Be(startMonth);
            hourRanges.StartDay.Should().Be(startDay);
            hourRanges.StartHour.Should().Be(startHour);
            hourRanges.EndYear.Should().Be(2004);
            hourRanges.EndMonth.Should().Be(2);
            hourRanges.EndDay.Should().Be(startDay);
            hourRanges.EndHour.Should().Be(startHour + 1);

            var hours = hourRanges.GetHours().ToList();

            hours.Count.Should().Be(1);
            hours[0].IsSamePeriod(new HourRange(2004, 2, 22, 17)).Should().Be.True();
        }

        [Test]
        public void CalendarHoursTest() {
            const int startYear = 2004;
            const int startMonth = 2;
            const int startDay = 11;
            const int startHour = 22;
            const int hourCount = 4;

            var hourRanges = new HourRangeCollection(startYear, startMonth, startDay, startHour, hourCount);

            hourRanges.HourCount.Should().Be(hourCount);
            hourRanges.StartYear.Should().Be(startYear);
            hourRanges.StartMonth.Should().Be(startMonth);
            hourRanges.StartDay.Should().Be(startDay);
            hourRanges.StartHour.Should().Be(startHour);
            hourRanges.EndYear.Should().Be(2004);
            hourRanges.EndMonth.Should().Be(2);
            hourRanges.EndDay.Should().Be(startDay + 1);
            hourRanges.EndHour.Should().Be(2);

            var hours = hourRanges.GetHours().ToList();

            hours.Count.Should().Be(hourCount);
            hours[0].IsSamePeriod(new HourRange(2004, 2, 11, 22)).Should().Be.True();
            hours[1].IsSamePeriod(new HourRange(2004, 2, 11, 23)).Should().Be.True();
            hours[2].IsSamePeriod(new HourRange(2004, 2, 12, 0)).Should().Be.True();
            hours[3].IsSamePeriod(new HourRange(2004, 2, 12, 1)).Should().Be.True();
        }

        [Test]
        [TestCase(1)]
        [TestCase(24)]
        [TestCase(48)]
        [TestCase(64)]
        [TestCase(124)]
        public void CalendarHoursTest(int? hourCount) {
            var now = ClockProxy.Clock.Now;
            var hourRanges = new HourRangeCollection(now, hourCount ?? 1);

            var startTime = now.TrimToMinute().Add(hourRanges.TimeCalendar.StartOffset);
            var endTime = startTime.AddHours(hourCount ?? 1).Add(hourRanges.TimeCalendar.EndOffset);

            hourRanges.Start.Should().Be(startTime);
            hourRanges.End.Should().Be(endTime);

            hourRanges.HourCount.Should().Be(hourCount);

            var items = hourRanges.GetHours().ToList();
            items.Count.Should().Be(hourCount);

            Enumerable
                .Range(0, hourCount ?? 1)
                .RunEach(i => {
                             items[i].Start.Should().Be(startTime.AddHours(i));
                             items[i].End.Should().Be(hourRanges.TimeCalendar.MapEnd(startTime.AddHours(i + 1)));
                             items[i].UnmappedEnd.Should().Be(startTime.AddHours(i + 1));
                         });
        }
    }
}