using System.Linq;
using NSoft.NFramework.LinqEx;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.TimePeriods.TimeRanges {
    [TestFixture]
    public class MinuteRangeCollectionFixture : TimePeriodFixtureBase {
        [Test]
        public void SingleMinutes() {
            var now = ClockProxy.Clock.Now;
            var minutes = new MinuteRangeCollection(now, 1);

            var startTime = now.TrimToSecond().Add(minutes.TimeCalendar.StartOffset);
            var endTime = startTime.AddMinutes(1).Add(minutes.TimeCalendar.EndOffset);

            minutes.Start.Should().Be(startTime);
            minutes.End.Should().Be(endTime);

            minutes.MinuteCount.Should().Be(1);

            var minList = minutes.GetMinutes().ToList();

            minList.Count.Should().Be(1);
            minList[0].Start.Should().Be(startTime);
            minList[0].End.Should().Be(endTime);
        }

        [Test]
        [TestCase(5)]
        [TestCase(10)]
        [TestCase(30)]
        [TestCase(60)]
        [TestCase(90)]
        public void CalendarMinutes(int? minuteCount) {
            var now = ClockProxy.Clock.Now;
            var minutes = new MinuteRangeCollection(now, minuteCount ?? 90);

            var startTime = now.TrimToSecond().Add(minutes.TimeCalendar.StartOffset);
            var endTime = startTime.AddMinutes(minuteCount ?? 90).Add(minutes.TimeCalendar.EndOffset);

            minutes.Start.Should().Be(startTime);
            minutes.End.Should().Be(endTime);
            minutes.MinuteCount.Should().Be(minuteCount);

            var items = minutes.GetMinutes().ToList();

            Enumerable
                .Range(0, minuteCount ?? 90)
                .RunEach(i => {
                             items[i].Start.Should().Be(startTime.AddMinutes(i));
                             items[i].UnmappedStart.Should().Be(startTime.AddMinutes(i));

                             items[i].End.Should().Be(minutes.TimeCalendar.MapEnd(startTime.AddMinutes(i + 1)));
                             items[i].UnmappedEnd.Should().Be(startTime.AddMinutes(i + 1));
                         });
        }

        [Test]
        public void CalendarMinutesTest() {
            const int startYear = 2004;
            const int startMonth = 2;
            const int startDay = 22;
            const int startHour = 23;
            const int startMinute = 59;
            const int minuteCount = 4;

            var minuteRanges = new MinuteRangeCollection(startYear, startMonth, startDay, startHour, startMinute, minuteCount);

            minuteRanges.MinuteCount.Should().Be(minuteCount);
            minuteRanges.StartYear.Should().Be(startYear);
            minuteRanges.StartMonth.Should().Be(startMonth);
            minuteRanges.StartDay.Should().Be(startDay);
            minuteRanges.StartHour.Should().Be(startHour);
            minuteRanges.StartMinute.Should().Be(startMinute);
            minuteRanges.EndYear.Should().Be(2004);
            minuteRanges.EndMonth.Should().Be(2);
            minuteRanges.EndDay.Should().Be(23);
            minuteRanges.EndHour.Should().Be(0);
            minuteRanges.EndMinute.Should().Be(3);

            var minutes = minuteRanges.GetMinutes().ToList();

            minuteCount.Should().Be(minutes.Count);
            minutes[0].IsSamePeriod(new MinuteRange(2004, 2, 22, 23, 59)).Should().Be.True();
            minutes[1].IsSamePeriod(new MinuteRange(2004, 2, 23, 0, 0)).Should().Be.True();
            minutes[2].IsSamePeriod(new MinuteRange(2004, 2, 23, 0, 1)).Should().Be.True();
            minutes[3].IsSamePeriod(new MinuteRange(2004, 2, 23, 0, 2)).Should().Be.True();
        }
    }
}