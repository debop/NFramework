using System.Linq;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.TimePeriods.TimeRanges {
    [TestFixture]
    public class DayRangeCollectionFixture : TimePeriodFixtureBase {
        [Test]
        public void SingleDaysTest() {
            const int startYear = 2004;
            const int startMonth = 2;
            const int startDay = 22;
            var days = new DayRangeCollection(startYear, startMonth, startDay, 1);

            days.DayCount.Should().Be(1);
            days.StartYear.Should().Be(startYear);
            days.StartMonth.Should().Be(startMonth);
            days.StartDay.Should().Be(startDay);
            days.EndYear.Should().Be(2004);
            days.EndMonth.Should().Be(2);
            days.EndDay.Should().Be(startDay);

            var dayList = days.GetDays().ToList();

            dayList.Count.Should().Be(1);
            dayList[0].IsSamePeriod(new DayRange(2004, 2, 22)).Should().Be.True();
        }

        [Test]
        public void CalendarDaysTest() {
            const int startYear = 2004;
            const int startMonth = 2;
            const int startDay = 27;
            const int dayCount = 5;
            var days = new DayRangeCollection(startYear, startMonth, startDay, dayCount);

            days.DayCount.Should().Be(dayCount);
            days.StartYear.Should().Be(startYear);
            days.StartMonth.Should().Be(startMonth);
            days.StartDay.Should().Be(startDay);
            days.EndYear.Should().Be(2004);
            days.EndMonth.Should().Be(3);
            days.EndDay.Should().Be(2);

            var dayList = days.GetDays().ToList();
            dayList.Count.Should().Be(dayCount);

            dayList[0].IsSamePeriod(new DayRange(2004, 2, 27)).Should().Be.True();
            dayList[1].IsSamePeriod(new DayRange(2004, 2, 28)).Should().Be.True();
            dayList[2].IsSamePeriod(new DayRange(2004, 2, 29)).Should().Be.True();
            dayList[3].IsSamePeriod(new DayRange(2004, 3, 1)).Should().Be.True();
            dayList[4].IsSamePeriod(new DayRange(2004, 3, 2)).Should().Be.True();

            var dayRanges = days.GetDays().ToList();

            for(var i = 0; i < dayCount; i++)
                dayRanges[i].IsSamePeriod(new DayRange(days.Start.AddDays(i))).Should().Be.True();
        }

        [TestCase(1)]
        [TestCase(6)]
        [TestCase(48)]
        [TestCase(180)]
        [TestCase(365)]
        public void CalendarHoursTest(int dayCount) {
            var now = ClockProxy.Clock.Now;
            var days = new DayRangeCollection(now, dayCount);

            var startTime = now.Date.Add(days.TimeCalendar.StartOffset);
            var endTime = startTime.AddDays(dayCount).Add(days.TimeCalendar.EndOffset);

            days.Start.Should().Be(startTime);
            days.End.Should().Be(endTime);

            days.DayCount.Should().Be(dayCount);

            var items = days.GetDays().ToList();
            items.Count.Should().Be(dayCount);

            for(var i = 0; i < dayCount; i++) {
                items[i].Start.Should().Be(startTime.AddDays(i));
                items[i].End.Should().Be(days.TimeCalendar.MapEnd(startTime.AddDays(i + 1)));
                items[i].IsSamePeriod(new DayRange(days.Start.AddDays(i))).Should().Be.True();
            }
        }
    }
}