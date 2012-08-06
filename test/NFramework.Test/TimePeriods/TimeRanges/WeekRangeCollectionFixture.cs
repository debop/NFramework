using System.Globalization;
using System.Linq;
using NSoft.NFramework.LinqEx;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.TimePeriods.TimeRanges {
    [TestFixture]
    public class WeekRangeCollectionFixture : TimePeriodFixtureBase {
        [Test]
        public void SingleWeeksTest() {
            const int startYear = 2004;
            const int startWeek = 22;
            var weekRanges = new WeekRangeCollection(startYear, startWeek, 1);

            weekRanges.Year.Should().Be(startYear);
            weekRanges.WeekCount.Should().Be(1);
            weekRanges.StartWeek.Should().Be(startWeek);
            weekRanges.EndWeek.Should().Be(startWeek);

            var weeks = weekRanges.GetWeeks().ToTimePeriodCollection();

            weeks.Count.Should().Be(1);
            weeks[0].IsSamePeriod(new WeekRange(2004, 22)).Should().Be.True();
        }

        [Test]
        public void MultiWeekTest() {
            const int startYear = 2004;
            const int startWeek = 22;
            const int weekCount = 4;
            var weekRanges = new WeekRangeCollection(startYear, startWeek, weekCount);

            weekRanges.Year.Should().Be(startYear);
            weekRanges.WeekCount.Should().Be(weekCount);
            weekRanges.StartWeek.Should().Be(startWeek);
            weekRanges.EndWeek.Should().Be(startWeek + weekCount - 1);

            var weeks = weekRanges.GetWeeks().ToTimePeriodCollection();

            weeks.Count.Should().Be(weekCount);
            weeks[0].IsSamePeriod(new WeekRange(2004, 22)).Should().Be.True();
        }

        [Test]
        public void CalendarWeeksTest() {
            const int startYear = 2004;
            const int startWeek = 22;
            const int weekCount = 5;
            var weekRanges = new WeekRangeCollection(startYear, startWeek, weekCount);

            weekRanges.Year.Should().Be(startYear);
            weekRanges.WeekCount.Should().Be(weekCount);
            weekRanges.StartWeek.Should().Be(startWeek);
            weekRanges.EndWeek.Should().Be(startWeek + weekCount - 1);

            var weeks = weekRanges.GetWeeks().ToTimePeriodCollection();
            weeks.Count.Should().Be(weekCount);

            var week = new WeekRange(startYear, startWeek);

            for(var i = 0; i < weekCount; i++)
                weeks[i].IsSamePeriod(week.AddWeeks(i)).Should().Be.True();
        }

        [Test]
        [TestCase(1)]
        [TestCase(6)]
        [TestCase(24)]
        [TestCase(64)]
        [TestCase(108)]
        public void CalendarHoursTest(int weekCount) {
            var now = ClockProxy.Clock.Now;
            var weeks = new WeekRangeCollection(now, weekCount);

            var startTime = now.StartTimeOfWeek(CultureInfo.CurrentCulture).Add(weeks.TimeCalendar.StartOffset);
            var endTime = startTime.AddDays(weekCount * TimeSpec.DaysPerWeek).Add(weeks.TimeCalendar.EndOffset);

            weeks.Start.Should().Be(startTime);
            weeks.End.Should().Be(endTime);

            weeks.WeekCount.Should().Be(weekCount);

            var items = weeks.GetWeeks().ToTimePeriodCollection();
            items.Count.Should().Be(weekCount);

            Enumerable
                .Range(0, weekCount)
                .RunEach(i => {
                             items[i].Start.Should().Be(startTime.AddDays(i * TimeSpec.DaysPerWeek));
                             items[i].End.Should().Be(weeks.TimeCalendar.MapEnd(startTime.AddDays((i + 1) * TimeSpec.DaysPerWeek)));
                         });
        }
    }
}