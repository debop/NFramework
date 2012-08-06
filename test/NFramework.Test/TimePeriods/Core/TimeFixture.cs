using System;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.TimePeriods.Core {
    [TestFixture]
    public class TimeFixture : TimePeriodBase {
        [Test]
        public void TimeConstructorTest() {
            var test = DateTime.Now;
            var time = new TimeValue(test);

            time.Duration.Should().Be(test.TimeOfDay);

            test.Hour.Should().Be(time.Hour);
            time.Minute.Should().Be(test.Minute);
            time.Second.Should().Be(test.Second);
            time.Millisecond.Should().Be(test.Millisecond);
        }

        [Test]
        public void EmptyDateTimeConstructorTest() {
            var test = DateTime.Today;
            var time = new TimeValue(test);

            time.Duration.Should().Be(TimeSpan.Zero);

            test.Hour.Should().Be(0);
            time.Minute.Should().Be(0);
            time.Second.Should().Be(0);
            time.Millisecond.Should().Be(0);
            time.Ticks.Should().Be(0);

            time.TotalHours.Should().Be(0);
            time.TotalMinutes.Should().Be(0);
            time.TotalSeconds.Should().Be(0);
            time.TotalMilliseconds.Should().Be(0);
        }

        [Test]
        public void ConstructorTest() {
            var time = new TimeValue(18, 23, 56, 344);

            time.Hour.Should().Be(18);
            time.Minute.Should().Be(23);
            time.Second.Should().Be(56);
            time.Millisecond.Should().Be(344);
        }

        [Test]
        public void EmptyConstructorTest() {
            var time = new TimeValue();

            time.Hour.Should().Be(0);
            time.Minute.Should().Be(0);
            time.Second.Should().Be(0);
            time.Millisecond.Should().Be(0);
            time.Ticks.Should().Be(0);
            time.Duration.Should().Be(TimeSpan.Zero);
            time.TotalHours.Should().Be(0);
            time.TotalMinutes.Should().Be(0);
            time.TotalSeconds.Should().Be(0);
            time.TotalMilliseconds.Should().Be(0);
        }

        [Test]
        public void DurationTest() {
            var test = new TimeSpan(0, 18, 23, 56, 344);
            var time = new TimeValue(test.Hours, test.Minutes, test.Seconds, test.Milliseconds);

            time.Hour.Should().Be(test.Hours);
            time.Minute.Should().Be(test.Minutes);
            time.Second.Should().Be(test.Seconds);
            time.Millisecond.Should().Be(test.Milliseconds);

            time.Duration.Ticks.Should().Be(test.Ticks);

            time.TotalHours.Should().Be(test.TotalHours);
            time.TotalMinutes.Should().Be(test.TotalMinutes);
            time.TotalSeconds.Should().Be(test.TotalSeconds);
            time.TotalMilliseconds.Should().Be(test.TotalMilliseconds);
        }

        [Test]
        public void GetDateTimeTest() {
            var dateTime = DateTime.Now;
            var timeSpan = new TimeSpan(0, 18, 23, 56, 344);
            var time = new TimeValue(timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);

            time.GetDateTime(dateTime).Should().Be(dateTime.Date.Add(timeSpan));
        }

        [Test]
        public void GetEmptyDateTimeTest() {
            var dateTime = DateTime.Today;
            var time = new TimeValue();

            time.GetDateTime(dateTime).Should().Be(dateTime);
        }
    }
}