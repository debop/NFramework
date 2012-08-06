using System;
using System.Linq;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.TimePeriods.TimeRanges;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.TimePeriods.Calendars.Seekers {
    [TestFixture]
    public class DaySeekerFixture : TimePeriodFixtureBase {
        [Test]
        public void SimpleForwardTest() {
            var start = new DayRange();
            var daySeeker = new DaySeeker();

            var day1 = daySeeker.FindDay(start, 0);
            day1.IsSamePeriod(start).Should().Be.True();

            var day2 = daySeeker.FindDay(start, 1);
            day2.IsSamePeriod(start.GetNextDay()).Should().Be.True();

            Enumerable
                .Range(-10, 20)
                .RunEach(i => {
                             var offset = i * 5;
                             var day = daySeeker.FindDay(start, offset);
                             day.IsSamePeriod(start.AddDays(offset)).Should().Be.True();
                         });
        }

        [Test]
        public void SimpleBackwardTest() {
            var start = new DayRange();
            var daySeeker = new DaySeeker(SeekDirection.Backward);


            var day1 = daySeeker.FindDay(start, 0);
            day1.IsSamePeriod(start).Should().Be.True();


            var day2 = daySeeker.FindDay(start, 1);
            day2.IsSamePeriod(start.GetPreviousDay()).Should().Be.True();

            Enumerable
                .Range(-10, 20)
                .RunEach(i => {
                             var offset = i * 5;
                             var day = daySeeker.FindDay(start, offset);
                             day.IsSamePeriod(start.AddDays(-offset)).Should().Be.True();
                         });
        }

        [Test]
        public void SeekDirectionTest() {
            var start = new DayRange();

            var daySeeker = new DaySeeker();

            Enumerable
                .Range(-10, 20)
                .RunEach(i => {
                             var offset = i * 5;
                             var day = daySeeker.FindDay(start, offset);
                             day.IsSamePeriod(start.AddDays(offset)).Should().Be.True();
                         });


            var backwardSeeker = new DaySeeker(SeekDirection.Backward);

            Enumerable
                .Range(-10, 20)
                .RunEach(i => {
                             var offset = i * 5;
                             var day = backwardSeeker.FindDay(start, offset);
                             day.IsSamePeriod(start.AddDays(-offset)).Should().Be.True();
                         });
        }

        [Test]
        public void MinDateTest() {
            var daySeeker = new DaySeeker();
            var day = daySeeker.FindDay(new DayRange(DateTime.MinValue), -10);
            Assert.IsNull(day);
        }

        [Test]
        public void MaxDateTest() {
            var daySeeker = new DaySeeker();
            var day = daySeeker.FindDay(new DayRange(DateTime.MaxValue.AddDays(-1)), 10);
            Assert.IsNull(day);
        }

        [Test]
        public void SeekWeekendHolidayTest() {
            var start = new DayRange(new DateTime(2011, 2, 15));

            var filter = new CalendarVisitorFilter();
            filter.AddWorkingWeekDays();
            filter.ExcludePeriods.Add(new DayRangeCollection(2011, 2, 27, 14)); // 14 days -> week 9 and 10

            var daySeeker = new DaySeeker(filter);

            var day1 = daySeeker.FindDay(start, 3); // wtihtin the same working week
            day1.IsSamePeriod(new DayRange(2011, 2, 18)).Should().Be.True();

            var day2 = daySeeker.FindDay(start, 4); // 주말(19,20) 제외 21일 월요일
            day2.IsSamePeriod(new DayRange(2011, 2, 21)).Should().Be.True();

            var day3 = daySeeker.FindDay(start, 10); // 2월27일부터 14일간 휴가로 제외 기간을 설정했음.
            day3.IsSamePeriod(new DayRange(2011, 3, 15)).Should().Be.True();
        }
    }
}