using System;
using System.Linq;
using NSoft.NFramework.LinqEx;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.TimePeriods.Tools {
    [Microsoft.Silverlight.Testing.Tag("TimePeriods")]
    [TestFixture]
    public class TimeToolFixture_Trim : TimePeriodFixtureBase {
        private static DateTime testTime = new DateTime(2000, 10, 2, 13, 45, 53, 673);
        private static DateTime nowTime = ClockProxy.Clock.Now;

        [Test]
        public void TrimMonthTest() {
            TimeTool.TrimToMonth(testTime).Should().Be(new DateTime(testTime.Year, 1, 1));

            Enumerable
                .Range(0, TimeSpec.MonthsPerYear)
                .RunEach(m => { TimeTool.TrimToMonth(testTime, m + 1).Should().Be(new DateTime(testTime.Year, m + 1, 1)); });
        }

        [Test]
        public void TrimDayTest() {
            TimeTool.TrimToDay(testTime).Should().Be(new DateTime(testTime.Year, testTime.Month, 1));

            Enumerable
                .Range(0, TimeTool.GetDaysInMonth(testTime.Year, testTime.Month))
                .RunEach(d => { TimeTool.TrimToDay(testTime, d + 1).Should().Be(new DateTime(testTime.Year, testTime.Month, d + 1)); });
        }

        [Test]
        public void TrimHourTest() {
            TimeTool.TrimToHour(testTime).Should().Be(testTime.Date);

            Enumerable
                .Range(0, TimeSpec.HoursPerDay)
                .RunEach(h => { TimeTool.TrimToHour(testTime, h).Should().Be(testTime.Date.AddHours(h)); });
        }

        [Test]
        public void TrimMinuteTest() {
            TimeTool.TrimToMinute(testTime).Should().Be(testTime.Date.AddHours(testTime.Hour));

            Enumerable
                .Range(0, TimeSpec.MinutesPerHour)
                .RunEach(m => { TimeTool.TrimToMinute(testTime, m).Should().Be(testTime.Date.AddHours(testTime.Hour).AddMinutes(m)); });
        }

        [Test]
        public void TrimSecondTest() {
            TimeTool.TrimToSecond(testTime).Should().Be(testTime.Date.AddHours(testTime.Hour).AddMinutes(testTime.Minute));

            Enumerable
                .Range(0, TimeSpec.SecondsPerMinute)
                .RunEach(
                    s => {
                        TimeTool.TrimToSecond(testTime, s).Should().Be(
                            testTime.Date.AddHours(testTime.Hour).AddMinutes(testTime.Minute).AddSeconds(s));
                    });
        }

        [Test]
        public void TrimMilliSecondTest() {
            TimeTool.TrimToMillisecond(testTime).Should().Be(
                testTime.Date.AddHours(testTime.Hour).AddMinutes(testTime.Minute).AddSeconds(testTime.Second));

            Enumerable
                .Range(0, TimeSpec.MillisecondsPerSecond)
                .RunEach(ms => {
                             var trimToMS =
                                 testTime.Date.AddHours(testTime.Hour).AddMinutes(testTime.Minute).AddSeconds(testTime.Second).
                                     AddMilliseconds(ms);
                             TimeTool.TrimToMillisecond(testTime, ms).Should().Be(trimToMS);
                         });
        }
    }
}