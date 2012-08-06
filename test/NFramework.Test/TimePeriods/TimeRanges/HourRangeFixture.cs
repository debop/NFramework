using System.Linq;
using NSoft.NFramework.LinqEx;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.TimePeriods.TimeRanges {
    [TestFixture]
    public class HourRangeFixture : TimePeriodFixtureBase {
        [Test]
        public void InitValuesTest() {
            var now = ClockProxy.Clock.Now;
            var firstHour = now.TrimToMinute();
            var secondHour = firstHour.AddHours(1);

            var hour = new HourRange(now, TimeCalendar.NewEmptyOffset());

            hour.Start.Year.Should().Be(firstHour.Year);
            hour.Start.Month.Should().Be(firstHour.Month);
            hour.Start.Day.Should().Be(firstHour.Day);
            hour.Start.Hour.Should().Be(firstHour.Hour);
            hour.Start.Minute.Should().Be(0);
            hour.Start.Second.Should().Be(0);
            hour.Start.Millisecond.Should().Be(0);

            hour.End.Year.Should().Be(secondHour.Year);
            hour.End.Month.Should().Be(secondHour.Month);
            hour.End.Day.Should().Be(secondHour.Day);
            hour.End.Hour.Should().Be(secondHour.Hour);
            hour.End.Minute.Should().Be(0);
            hour.End.Second.Should().Be(0);
            hour.End.Millisecond.Should().Be(0);
        }

        [Test]
        public void DefaultCalendarTest() {
            var now = ClockProxy.Clock.Now;
            var todayStart = now.Date;

            Enumerable
                .Range(0, TimeSpec.HoursPerDay)
                .RunEach(dayHour => {
                             var hourRange = new HourRange(todayStart.AddHours(dayHour));

                             hourRange.Year.Should().Be(todayStart.Year);
                             hourRange.Month.Should().Be(todayStart.Month);
                             hourRange.Month.Should().Be(todayStart.Month);
                             hourRange.Day.Should().Be(todayStart.Day);
                             hourRange.Hour.Should().Be(dayHour);
                             hourRange.Start.Should().Be(todayStart.AddHours(dayHour).Add(hourRange.TimeCalendar.StartOffset));
                             hourRange.End.Should().Be(todayStart.AddHours(dayHour + 1).Add(hourRange.TimeCalendar.EndOffset));
                         });
        }

        [Test]
        public void ConstructorTest() {
            var now = ClockProxy.Clock.Now;

            new HourRange(now).Year.Should().Be(now.Year);
            new HourRange(now).Month.Should().Be(now.Month);
            new HourRange(now).Day.Should().Be(now.Day);
            new HourRange(now).Hour.Should().Be(now.Hour);

            new HourRange(now.Year, now.Month, now.Day, now.Hour).Year.Should().Be(now.Year);
            new HourRange(now.Year, now.Month, now.Day, now.Hour).Month.Should().Be(now.Month);
            new HourRange(now.Year, now.Month, now.Day, now.Hour).Day.Should().Be(now.Day);
            new HourRange(now.Year, now.Month, now.Day, now.Hour).Hour.Should().Be(now.Hour);
        }

        [Test]
        public void GetPreviousHourTest() {
            HourRange hourRange = new HourRange();
            Assert.AreEqual(hourRange.GetPreviousHour(), hourRange.AddHours(-1));
        }

        [Test]
        public void GetNextHourTest() {
            var hourRange = new HourRange();
            hourRange.GetNextHour().Should().Be(hourRange.AddHours(1));
        }

        [Test]
        public void AddHoursTest() {
            var now = ClockProxy.Clock.Now;
            var nowHour = now.TrimToMinute();
            var hourRange = new HourRange(now, TimeCalendar.NewEmptyOffset());

            hourRange.AddHours(0).Should().Be(hourRange);

            var previousHour = nowHour.AddHours(-1);

            hourRange.AddHours(-1).Year.Should().Be(previousHour.Year);
            hourRange.AddHours(-1).Month.Should().Be(previousHour.Month);
            hourRange.AddHours(-1).Day.Should().Be(previousHour.Day);
            hourRange.AddHours(-1).Hour.Should().Be(previousHour.Hour);

            var nextHour = nowHour.AddHours(1);

            hourRange.AddHours(1).Year.Should().Be(nextHour.Year);
            hourRange.AddHours(1).Month.Should().Be(nextHour.Month);
            hourRange.AddHours(1).Day.Should().Be(nextHour.Day);
            hourRange.AddHours(1).Hour.Should().Be(nextHour.Hour);
        }

        [Test]
        public void GetMinutesTest() {
            var hourRange = new HourRange();

            var minutes = hourRange.GetMinutes().ToTimePeriodCollection();

            minutes.Should().Not.Be.Null();
            minutes.Count.Should().Be(TimeSpec.MinutesPerHour);

            Enumerable
                .Range(0, minutes.Count)
                .RunEach(index => {
                             var minute = (MinuteRange)minutes[index];

                             minute.Start.Should().Be(hourRange.Start.AddMinutes(index));
                             minute.UnmappedStart.Should().Be(hourRange.Start.AddMinutes(index));

                             minute.End.Should().Be(minute.TimeCalendar.MapEnd(minute.Start.AddMinutes(1)));
                             minute.UnmappedEnd.Should().Be(minute.Start.AddMinutes(1));
                         });
        }
    }
}