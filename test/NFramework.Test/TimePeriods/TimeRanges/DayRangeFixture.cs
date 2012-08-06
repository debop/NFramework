using System;
using System.Linq;
using NSoft.NFramework.LinqEx;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.TimePeriods.TimeRanges {
    [TestFixture]
    public class DayRangeFixture : TimePeriodFixtureBase {
        [Test]
        public void InitValuesTest() {
            var now = ClockProxy.Clock.Now;
            var firstDay = new DateTime(now.Year, now.Month, now.Day);
            var secondDay = firstDay.AddDays(1);

            var day = new DayRange(now, TimeCalendar.NewEmptyOffset());

            day.Start.Should().Be(firstDay);
            day.End.Should().Be(secondDay);
        }

        [Test]
        public void DefaultCalendarTest() {
            var yearStart = new DateTime(ClockProxy.Clock.Now.Year, January, 1);

            for(var m = 1; m <= TimeSpec.MonthsPerYear; m++) {
                var monthStart = new DateTime(yearStart.Year, m, 1);
                var monthEnd = monthStart.EndTimeOfMonth().StartTimeOfDay();

                for(var monthDay = monthStart.Day; monthDay < monthEnd.Day; monthDay++) {
                    var day = new DayRange(monthStart.AddDays(monthDay - monthStart.Day));

                    day.Year.Should().Be(yearStart.Year);
                    day.Month.Should().Be(monthStart.Month);
                    day.Month.Should().Be(monthEnd.Month);
                    day.Day.Should().Be(monthDay);

                    day.Start.Should().Be(monthStart.AddDays(monthDay - monthStart.Day).Add(day.TimeCalendar.StartOffset));
                    day.End.Should().Be(monthStart.AddDays(monthDay).Add(day.TimeCalendar.EndOffset));
                }
            }
        }

        [Test]
        public void ConstructorTest() {
            var now = ClockProxy.Clock.Now;

            var day = new DayRange(now);
            day.Start.Should().Be(now.Date);

            day = new DayRange(now.Year, now.Month, now.Day);
            day.Start.Should().Be(now.Date);
        }

        [Test]
        public void DayOfWeekTest() {
            var now = ClockProxy.Clock.Now;
            var timeCalendar = new TimeCalendar();

            var day = new DayRange(now, timeCalendar);
            day.DayOfWeek.Should().Be(timeCalendar.Culture.Calendar.GetDayOfWeek(now));
        }

        [Test]
        public void GetPreviousDayTest() {
            var now = ClockProxy.Clock.Now;
            var day = new DayRange(now);

            day.GetPreviousDay().Should().Be(day.AddDays(-1));
            day.GetPreviousDay().Start.Should().Be(now.Date.AddDays(-1));
        }

        [Test]
        public void GetNextDayTest() {
            var now = ClockProxy.Clock.Now;
            var day = new DayRange(now);

            day.GetNextDay().Should().Be(day.AddDays(1));
            day.GetNextDay().Start.Should().Be(now.Date.AddDays(1));
        }

        [Test]
        public void AddDaysTest() {
            var now = ClockProxy.Clock.Now;
            var nowDate = now.Date;

            var day = new DayRange(now, TimeCalendar.NewEmptyOffset());

            day.AddDays(0).Should().Be(day);

            Enumerable
                .Range(-60, 120)
                .RunEach(i => day.AddDays(i).Start.Should().Be(nowDate.AddDays(i)));
        }

        [Test]
        public void GetHoursTest() {
            var day = new DayRange();

            var hours = day.GetHours();
            Assert.IsNotNull(hours);

            var index = 0;
            foreach(var hour in hours) {
                hour.Start.Should().Be(day.Start.AddHours(index));
                hour.End.Should().Be(hour.TimeCalendar.MapEnd(hour.Start.AddHours(1)));
                index++;
            }
            index.Should().Be(TimeSpec.HoursPerDay);
        }
    }
}