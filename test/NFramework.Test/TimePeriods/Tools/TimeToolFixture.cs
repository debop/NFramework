using System;
using NSoft.NFramework.Tools;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.TimePeriods.Tools {
    [Microsoft.Silverlight.Testing.Tag("TimePeriods")]
    [TestFixture]
    public class TimeToolFixture : TimePeriodFixtureBase {
        private static DateTime testTime = new DateTime(2000, 10, 2, 13, 45, 53, 673);
        private static readonly DateTime nowTime = ClockProxy.Clock.Now;

        [Test]
        public void AsStringTest() {
            var period = new TimeRange(testTime, nowTime);
            var periodString = TimeTool.AsString(period);

            Console.WriteLine("periodString=" + periodString);
            periodString.Should().Not.Be.Empty();
        }

        [Test]
        public void ToDateTimeTest() {
            var testTimeString = testTime.ToSortableString();
            var parsedTime = TimeTool.ToDateTime(testTimeString);

            Assert.AreEqual(testTime.TrimToMillisecond(), parsedTime);

            parsedTime = TimeTool.ToDateTime(string.Empty, nowTime);
            Assert.AreEqual(nowTime, parsedTime);
        }
    }
}