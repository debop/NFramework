using System;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.TimePeriods.TimeRanges {
    [TestFixture]
    public class CalendarTimeRangeFixture : TimePeriodFixtureBase {
        [Test]
        public void CalendarTest() {
            var timeCalendar = new TimeCalendar();
            var timeRange = new CalendarTimeRange(TimeRange.Anytime, timeCalendar);

            timeRange.TimeCalendar.Should().Be(timeCalendar);
            timeRange.IsAnytime.Should().Be.True();
        }

        [Test]
        public void MomentTest() {
            var testDate = ClockProxy.Clock.Today;
            Assert.Throws<InvalidOperationException>(() => new CalendarTimeRange(testDate, testDate));
        }
    }
}