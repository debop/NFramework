using System.Linq;
using NSoft.NFramework.LinqEx;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.TimePeriods.TimeRanges {
    [TestFixture]
    public class MinuteRangeFixture : TimePeriodFixtureBase {
        [Test]
        public void MinuteTest() {
            var now = ClockProxy.Clock.Now;
            var firstMinute = now.TrimToSecond();
            var secondMinute = firstMinute.AddMinutes(1);

            var m = new MinuteRange(now, TimeCalendar.NewEmptyOffset());

            m.Start.Should().Be(firstMinute);
            m.End.Should().Be(secondMinute);
        }

        [Test]
        public void DefaultCalendarTest() {
            var now = ClockProxy.Clock.Now;
            var currentHour = now.TrimToMinute();

            Enumerable
                .Range(0, TimeSpec.MinutesPerHour)
                .RunEach(m => {
                             var minute = new MinuteRange(currentHour.AddMinutes(m));

                             minute.Start.Should().Be(currentHour.AddMinutes(m).Add(minute.TimeCalendar.StartOffset));
                             minute.End.Should().Be(currentHour.AddMinutes(m + 1).Add(minute.TimeCalendar.EndOffset));

                             minute.UnmappedStart.Should().Be(currentHour.AddMinutes(m));
                             minute.UnmappedEnd.Should().Be(currentHour.AddMinutes(m + 1));
                         });
        }

        [Test]
        public void ConstructorTest() {
            var now = ClockProxy.Clock.Now;

            new MinuteRange(now).Start.Should().Be(now.TrimToSecond());
            new MinuteRange(now.Year, now.Month, now.Day, now.Hour, now.Minute).Start.Should().Be(now.TrimToSecond());
        }

        [Test]
        public void PreviousMinute() {
            var minute = new MinuteRange();
            minute.GetPreviousMinute().Should().Be(minute.AddMinutes(-1));
        }

        [Test]
        public void MextMinute() {
            var minute = new MinuteRange();
            minute.GetNextMinute().Should().Be(minute.AddMinutes(1));
        }

        [Test]
        public void AddMinutes() {
            var now = ClockProxy.Clock.Now;

            var m = new MinuteRange(now, TimeCalendar.NewEmptyOffset());
            var minuteTime = now.TrimToSecond();

            m.AddMinutes(-1).Start.Should().Be(minuteTime.AddMinutes(-1));
            m.AddMinutes(1).Start.Should().Be(minuteTime.AddMinutes(1));

            Enumerable
                .Range(-TimeSpec.MinutesPerHour * 2, TimeSpec.MinutesPerHour * 2)
                .RunEach(i => m.AddMinutes(i).Start.Should().Be(minuteTime.AddMinutes(i)));
        }
    }
}