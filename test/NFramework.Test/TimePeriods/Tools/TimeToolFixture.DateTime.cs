using System;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.TimePeriods.Tools {
    [Microsoft.Silverlight.Testing.Tag("TimePeriods")]
    [TestFixture]
    public class TimeToolFixture_DateTime : TimePeriodFixtureBase {
        private static DateTime testTime = new DateTime(2000, 10, 2, 13, 45, 53, 673);
        private static DateTime nowTime = ClockProxy.Clock.Now;

        [Test]
        public void GetDateTest() {
            TimeTool.GetDatePart(testTime).Should().Be(testTime.Date);
            TimeTool.GetDatePart(nowTime).Should().Be(nowTime.Date);
        }

        [Test]
        public void SetDateTest() {
            TimeTool.SetDatePart(testTime, nowTime).Date.Should().Be(nowTime.Date);
            TimeTool.SetDatePart(nowTime, testTime).Date.Should().Be(testTime.Date);
        }

        [Test]
        public void HasTimeOfDayTest() {
            TimeTool.HasTimePart(testTime).Should().Be.True();
            TimeTool.HasTimePart(nowTime).Should().Be.True();

            TimeTool.HasTimePart(nowTime.SetTimePart(1)).Should().Be.True();
            TimeTool.HasTimePart(nowTime.SetTimePart(0, 1)).Should().Be.True();
            TimeTool.HasTimePart(nowTime.SetTimePart(0, 0, 1)).Should().Be.True();
            TimeTool.HasTimePart(nowTime.SetTimePart(0, 0, 0, 1)).Should().Be.True();

            TimeTool.HasTimePart(nowTime.SetTimePart(0, 0, 0, 0)).Should().Be.False();
        }

        [Test]
        public void SetTimeOfDayTest() {
            TimeTool.SetTimePart(nowTime, testTime).HasTimePart().Should().Be.True();
            TimeTool.SetTimePart(nowTime, testTime).TimeOfDay.Should().Be(testTime.TimeOfDay);
            TimeTool.SetTimePart(nowTime, testTime).TimeOfDay.Should().Not.Be.EqualTo(nowTime.TimeOfDay);
        }
    }
}