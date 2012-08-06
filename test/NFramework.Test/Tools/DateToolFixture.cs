using System;
using System.Globalization;
using NUnit.Framework;

namespace NSoft.NFramework.Tools {
    [TestFixture]
    public class DateToolFixture : AbstractFixture {
        [Test]
        public void Days() {
            var date = new DateTime(2000, 1, 1);
            //Console.WriteLine("Current Date: " + date);
            //Console.WriteLine("EndOfDay: " + date.GetEndOfDay());
            Assert.AreEqual(date.AddDays(1).AddTicks(-1), date.GetEndOfDay());
        }

        [Test]
        public void DaysOfWeekTest() {
            var date = DateTime.Now;

            var nextFriday = date.NextDayOfWeek(DayOfWeek.Friday);
            var prevFriday = date.PrevDayOfWeek(DayOfWeek.Friday);

            Assert.AreEqual(nextFriday.DayOfWeek, DayOfWeek.Friday);
            Assert.AreEqual(prevFriday.DayOfWeek, DayOfWeek.Friday);

            if(date.DayOfWeek != DayOfWeek.Friday)
                Assert.AreEqual(nextFriday, prevFriday.AddDays(7));
            else
                Assert.AreEqual(nextFriday, prevFriday.AddDays(7 * 2));
        }

        [Test]
        public void Week() {
            var date = new DateTime(2010, 1, 1);
            Console.WriteLine("Current Date: " + date);

            var koreanFirstDayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
            var startOfWeek = date.GetStartOfWeek(koreanFirstDayOfWeek);
            var endOfWeek = date.GetEndOfWeek(koreanFirstDayOfWeek);

            Assert.AreEqual(new DateTime(2009, 12, 27), startOfWeek.Date);
            Assert.AreEqual(new DateTime(2010, 1, 2), endOfWeek.Date);

            Console.WriteLine("StartOfWeek: " + date.GetStartOfWeek());
            Console.WriteLine("EndOfWeek: " + date.GetEndOfWeek());
        }

        [Test]
        public void TimeTest() {
            var datePart = new DateTime(2000, 1, 1);
            var timePart = new DateTime().AddDays(1).AddMilliseconds(-1);

            var date = datePart.Combine(timePart);
            Assert.AreEqual(date, datePart.AddDays(1).AddMilliseconds(-1));

            Console.WriteLine("Combine({0}, {1}) = {2}", datePart, timePart, date);
        }

        [Test]
        [TestCase(1)]
        [TestCase(32)]
        [TestCase(100)]
        [TestCase(1000)]
        [TestCase(-1)]
        [TestCase(-100)]
        [TestCase(0)]
        public void Ago_Date(int agoValue) {
            var date = new DateTime(1968, 10, 14, 10, 0, 0, 0);

            Assert.AreEqual(agoValue.Days().Ago(date), date - new TimeSpan(agoValue, 0, 0, 0));
            Assert.AreEqual(agoValue.Hours().Ago(date), date - new TimeSpan(0, agoValue, 0, 0, 0));
            Assert.AreEqual(agoValue.Minutes().Ago(date), date - new TimeSpan(0, 0, agoValue, 0, 0));
            Assert.AreEqual(agoValue.Seconds().Ago(date), date - new TimeSpan(0, 0, 0, agoValue, 0));
            Assert.AreEqual(agoValue.Milliseconds().Ago(date), date - new TimeSpan(0, 0, 0, 0, agoValue));
        }

        [Test]
        [TestCase(1)]
        [TestCase(32)]
        [TestCase(100)]
        [TestCase(1000)]
        [TestCase(-1)]
        [TestCase(-100)]
        [TestCase(0)]
        public void From_Date(int fromValue) {
            var date = new DateTime(1968, 10, 14, 10, 0, 0, 0);

            Assert.AreEqual(fromValue.Days().From(date), date + new TimeSpan(fromValue, 0, 0, 0));
            Assert.AreEqual(fromValue.Hours().From(date), date + new TimeSpan(0, fromValue, 0, 0, 0));
            Assert.AreEqual(fromValue.Minutes().From(date), date + new TimeSpan(0, 0, fromValue, 0, 0));
            Assert.AreEqual(fromValue.Seconds().From(date), date + new TimeSpan(0, 0, 0, fromValue, 0));
            Assert.AreEqual(fromValue.Milliseconds().From(date), date + new TimeSpan(0, 0, 0, 0, fromValue));
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(23)]
        [TestCase(24, ExpectedException = typeof(ArgumentOutOfRangeException))]
        [TestCase(-1, ExpectedException = typeof(ArgumentOutOfRangeException))]
        public void SetTime_Hours(int value) {
            var date = DateTime.Today;

            var result = DateTool.SetTime(date, value);
            var expected = date.AddHours(value);
            Assert.AreEqual(expected, result);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(59)]
        [TestCase(60, ExpectedException = typeof(ArgumentOutOfRangeException))]
        [TestCase(-1, ExpectedException = typeof(ArgumentOutOfRangeException))]
        public void SetTime_Minutes(int value) {
            var date = DateTime.Today;

            var result = date.SetMinute(value);
            var expected = date.AddMinutes(value);
            Assert.AreEqual(expected, result);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(59)]
        [TestCase(60, ExpectedException = typeof(ArgumentOutOfRangeException))]
        [TestCase(-1, ExpectedException = typeof(ArgumentOutOfRangeException))]
        public void SetTime_Seconds(int value) {
            var date = DateTime.Today;

            var result = date.SetSecond(value);
            var expected = date.AddSeconds(value);

            Assert.AreEqual(expected, result);
        }
    }
}