using System;
using NUnit.Framework;

namespace NSoft.NFramework.TimePeriods {
    [Microsoft.Silverlight.Testing.Tag("Core")]
    [TestFixture]
    public class PeriodTimeFormatTestFixture {
        [Test]
        public void IsExpiredByMinutes() {
            // 매시 1분에
            var periodFormat = "1 * * * *";
            var getTime = DateTime.Today;

            Assert.IsFalse(PeriodTimeFormat.IsExpired(periodFormat, getTime, getTime.AddSeconds(30)));
            Assert.IsTrue(PeriodTimeFormat.IsExpired(periodFormat, getTime, getTime.AddMinutes(1)));

            // 매시 5분에
            periodFormat = "5 * * * *";
            Assert.IsFalse(PeriodTimeFormat.IsExpired(periodFormat, getTime, getTime.AddMinutes(4)));
            Assert.IsTrue(PeriodTimeFormat.IsExpired(periodFormat, getTime, getTime.AddMinutes(5)));

            // 5분마다 
            periodFormat = "0,5,10,15,20,25,30,35,40,45,50,55 * * * *";
            Assert.IsFalse(PeriodTimeFormat.IsExpired(periodFormat, getTime, getTime.AddMinutes(4)));
            Assert.IsTrue(PeriodTimeFormat.IsExpired(periodFormat, getTime, getTime.AddMinutes(5)));
        }

        [Test]
        public void IsExpiredByHour() {
            // 매일 1시 정각에
            var periodFormat = "0 1 * * *";
            var getTime = DateTime.Today;

            Assert.IsFalse(PeriodTimeFormat.IsExpired(periodFormat, getTime, getTime.AddMinutes(30)));
            Assert.IsTrue(PeriodTimeFormat.IsExpired(periodFormat, getTime, getTime.AddHours(1)));

            // 매일 6시 정각에 
            periodFormat = "0 6 * * *";
            Assert.IsFalse(PeriodTimeFormat.IsExpired(periodFormat, getTime, getTime.AddHours(5)));
            Assert.IsTrue(PeriodTimeFormat.IsExpired(periodFormat, getTime, getTime.AddHours(6)));

            // 매일 6시간마다
            periodFormat = "0 0,6,12,18 * * *";
            Assert.IsFalse(PeriodTimeFormat.IsExpired(periodFormat, getTime, getTime.AddHours(5)));
            Assert.IsTrue(PeriodTimeFormat.IsExpired(periodFormat, getTime, getTime.AddHours(6)));
        }

        [Test]
        public void MinutePeriodTest() {
            // 매시 5분에 작업을 수행한다.
            const string PeriodFmt = "5 * * * *";

            var lastTime = new DateTime(2007, 3, 28, 12, 4, 0);
            var currTime = new DateTime(2007, 3, 28, 12, 4, 55);

            Assert.IsFalse(PeriodTimeFormat.IsExpired(PeriodFmt, lastTime, currTime));
            Assert.IsFalse(PeriodTimeFormat.IsExpired(PeriodFmt, lastTime, currTime.AddSeconds(4)));
            Assert.IsTrue(PeriodTimeFormat.IsExpired(PeriodFmt, lastTime, currTime.AddMinutes(1)));
        }

        [Test]
        public void MinutePeriodTest2() {
            // 매 10분마다 수행한다.
            const string PeriodFmt = "0,10,20,30,40,50 * * * *";

            var lastTime = new DateTime(2007, 3, 28, 12, 5, 0);
            var currTime = new DateTime(2007, 3, 28, 12, 9, 55);

            Assert.IsFalse(PeriodTimeFormat.IsExpired(PeriodFmt, lastTime, currTime));
            Assert.IsFalse(PeriodTimeFormat.IsExpired(PeriodFmt, lastTime, currTime.AddSeconds(4)));
            Assert.IsFalse(PeriodTimeFormat.IsExpired(PeriodFmt, lastTime, currTime.AddMinutes(-5)));
            Assert.IsTrue(PeriodTimeFormat.IsExpired(PeriodFmt, lastTime, currTime.AddMinutes(1)));

            Assert.IsTrue(PeriodTimeFormat.IsExpired(PeriodFmt, lastTime.AddMinutes(10), currTime.AddMinutes(11)));
        }

        [Test]
        public void MinutePeriodTest3() {
            // 매 10분마다 수행한다.
            const string PeriodFmt = "0,10,20,30,40,50 12 * * *";

            var lastTime = new DateTime(2007, 3, 28, 12, 5, 0);
            var currTime = new DateTime(2007, 3, 28, 12, 9, 55);

            Assert.IsFalse(PeriodTimeFormat.IsExpired(PeriodFmt, lastTime, currTime));
            Assert.IsFalse(PeriodTimeFormat.IsExpired(PeriodFmt, lastTime, currTime.AddSeconds(4)));
            Assert.IsFalse(PeriodTimeFormat.IsExpired(PeriodFmt, lastTime, currTime.AddMinutes(-5)));
            Assert.IsTrue(PeriodTimeFormat.IsExpired(PeriodFmt, lastTime, currTime.AddMinutes(1)));

            Assert.IsTrue(PeriodTimeFormat.IsExpired(PeriodFmt, lastTime.AddMinutes(10), currTime.AddMinutes(11)));
        }

        [Test]
        public void HourPeriodTest() {
            // 12시에 작업을 수행한다.
            const string PeriodFormat = "0 12 * * *";

            var lastTime = new DateTime(2007, 3, 28, 11, 55, 0);
            var currTime = new DateTime(2007, 3, 28, 11, 55, 0);

            Assert.IsFalse(PeriodTimeFormat.IsExpired(PeriodFormat, lastTime, currTime));
            Assert.IsFalse(PeriodTimeFormat.IsExpired(PeriodFormat, lastTime, currTime.AddMinutes(4)));
            Assert.IsTrue(PeriodTimeFormat.IsExpired(PeriodFormat, lastTime, currTime.AddMinutes(10)));
        }

        [Test]
        public void HourPeriodTest2() {
            // 매 12시간마다 수행한다.
            const string PeriodFmt = "0 0,12 * * *";

            var lastTime = new DateTime(2007, 3, 28, 11, 55, 0);
            var currTime = new DateTime(2007, 3, 28, 11, 55, 0);

            Assert.IsFalse(PeriodTimeFormat.IsExpired(PeriodFmt, lastTime, currTime));
            Assert.IsFalse(PeriodTimeFormat.IsExpired(PeriodFmt, lastTime, currTime.AddMinutes(4)));
            Assert.IsTrue(PeriodTimeFormat.IsExpired(PeriodFmt, lastTime, currTime.AddMinutes(10)));

            Assert.IsFalse(PeriodTimeFormat.IsExpired(PeriodFmt, lastTime, currTime.AddMinutes(-5)));
            Assert.IsTrue(PeriodTimeFormat.IsExpired(PeriodFmt, lastTime, currTime.AddMinutes(5)));

            Assert.IsTrue(PeriodTimeFormat.IsExpired(PeriodFmt, lastTime.AddHours(5), currTime.AddHours(13)));
        }

        [Test]
        public void WeekPeriodTest2() {
            const string PeriodFmt = "7 4 * * 6";

            var lastTime = new DateTime(2007, 3, 28, 11, 55, 0);
            var currTime = new DateTime(2007, 3, 31, 11, 55, 0);

            Assert.IsFalse(PeriodTimeFormat.IsExpired(PeriodFmt, lastTime, currTime));
            Assert.IsTrue(PeriodTimeFormat.IsExpired(PeriodFmt, lastTime, currTime.AddDays(4)));
            Assert.IsTrue(PeriodTimeFormat.IsExpired(PeriodFmt, lastTime, currTime.AddDays(10)));

            Assert.IsFalse(PeriodTimeFormat.IsExpired(PeriodFmt, lastTime, currTime.AddDays(-5)));
            Assert.IsTrue(PeriodTimeFormat.IsExpired(PeriodFmt, lastTime, currTime.AddDays(5)));

            Assert.IsTrue(PeriodTimeFormat.IsExpired(PeriodFmt, lastTime.AddDays(5), currTime.AddDays(13)));
        }

        [Test]
        public void InvalidTimeFormat() {
            const string PeriodFmt = "a b c 0,12 * * *";

            var lastTime = new DateTime(2007, 3, 28, 11, 55, 0);
            var currTime = new DateTime(2007, 3, 28, 11, 55, 0);

            Assert.Throws<InvalidOperationException>(() => PeriodTimeFormat.IsExpired(PeriodFmt, lastTime, currTime));
        }
    }
}