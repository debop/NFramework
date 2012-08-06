using System;
using System.Linq;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.TimePeriods.TimeRanges;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.TimePeriods.Calendars {
    [TestFixture]
    public class CalendarDateAddFixture : TimePeriodFixtureBase {
        [Test]
        public void NoPeriodTest() {
            var calendarDateAdd = new CalendarDateAdd();
            var now = ClockProxy.Clock.Now;

            Enumerable
                .Range(-10, 20)
                .RunEach(index => {
                             var offset = index * 5;

                             calendarDateAdd.Add(now, DurationUtil.Days(offset)).Should().Be(now.Add(DurationUtil.Days(offset)));
                             calendarDateAdd.Add(now, DurationUtil.Days(-offset)).Should().Be(now.Add(DurationUtil.Days(-offset)));

                             calendarDateAdd.Subtract(now, DurationUtil.Days(offset)).Should().Be(now.Subtract(DurationUtil.Days(offset)));
                             calendarDateAdd.Subtract(now, DurationUtil.Days(-offset)).Should().Be(
                                 now.Subtract(DurationUtil.Days(-offset)));
                         });
        }

        [Test]
        public void PeriodLimitsAddTest() {
            var test = new DateTime(2011, 4, 12);

            var timeRange1 = new TimeRange(new DateTime(2011, 4, 20), new DateTime(2011, 4, 25)); // 4월 20일~4월25일
            var timeRange2 = new TimeRange(new DateTime(2011, 4, 30), null); // 4월 30일 이후 

            var calendarDateAdd = new CalendarDateAdd();

            // 예외 기간을 설정합니다. 4월20일 ~ 4월25일, 4월30일 이후
            //
            calendarDateAdd.ExcludePeriods.Add(timeRange1);
            calendarDateAdd.ExcludePeriods.Add(timeRange2);

            calendarDateAdd.Add(test, DurationUtil.Day).Should().Be(test.Add(DurationUtil.Day));

            //! 4월 12일에 8일을 더하면 4월 20일이지만, 20~25일까지 제외되므로, 4월 25일이 된다.
            //
            calendarDateAdd.Add(test, DurationUtil.Days(8)).Should().Be(timeRange1.End);

            //! 4월 20일에 20일을 더하면, 4월 20~25일 제외 후를 계산하면 4월 30일 이후가 된다. 하지만 4월 30일 이후는 제외가 되므로, 결과값은 null이 된다.
            //
            calendarDateAdd.Add(test, DurationUtil.Days(20)).HasValue.Should().Be.False();

            calendarDateAdd.Subtract(test, DurationUtil.Days(3)).Should().Be(test.Subtract(DurationUtil.Days(3)));
        }

        [Test]
        public void PeriodLimitsSubtractTest() {
            var test = new DateTime(2011, 4, 30);

            var timeRange1 = new TimeRange(new DateTime(2011, 4, 20), new DateTime(2011, 4, 25));
            var timeRange2 = new TimeRange(DateTime.MinValue, new DateTime(2011, 4, 6));

            var calendarDateAdd = new CalendarDateAdd();

            // 예외 기간을 설정합니다. 4월 10일 이전, 4월20일 ~ 4월25일
            //
            calendarDateAdd.ExcludePeriods.Add(timeRange1);
            calendarDateAdd.ExcludePeriods.Add(timeRange2);

            calendarDateAdd.Subtract(test, DurationUtil.Days(1)).Should().Be(test.Subtract(DurationUtil.Days(1)));

            //! 4월 30일로부터 5일전이면 4월25일이지만, 예외기간이 4월20일~4월25일이므로, 4월20일을 반환합니다.
            //
            calendarDateAdd.Subtract(test, DurationUtil.Days(5)).Should().Be(timeRange1.Start);

            //! 4월 30일로부터 20일전이면, 4월10일 이지만 예외기간이 4월20일~4월25일이 있어 4월 5일이 되지만 4월 6일 이전은 예외기간이라 null을 반환합니다.
            //
            calendarDateAdd.Subtract(test, DurationUtil.Days(20)).HasValue.Should().Be.False();

            calendarDateAdd.Add(test, DurationUtil.Days(3)).Should().Be(test.Add(DurationUtil.Days(3)));
        }

        [Test]
        public void ExcludeTest() {
            DateTime test = new DateTime(2011, 4, 12);

            var timeRange = new TimeRange(new DateTime(2011, 4, 15), new DateTime(2011, 4, 20));
            var calendarDateAdd = new CalendarDateAdd();

            calendarDateAdd.ExcludePeriods.Add(timeRange);

            calendarDateAdd.Add(test, TimeSpan.Zero).Should().Be(test);
            calendarDateAdd.Add(test, DurationUtil.Days(2)).Should().Be(test.AddDays(2));
            calendarDateAdd.Add(test, DurationUtil.Days(3)).Should().Be(timeRange.End);
            calendarDateAdd.Add(test, DurationUtil.Days(3, 0, 0, 0, 1)).Should().Be(timeRange.End.Add(DurationUtil.Millisecond));
            calendarDateAdd.Add(test, DurationUtil.Days(5)).Should().Be(timeRange.End.AddDays(2));
        }

        [Test]
        public void ExcludeSplitTest() {
            var test = new DateTime(2011, 4, 12);
            var calendarDateAdd = new CalendarDateAdd();

            var timeRange1 = new TimeRange(new DateTime(2011, 4, 15), new DateTime(2011, 4, 20));
            var timeRange2 = new TimeRange(new DateTime(2011, 4, 22), new DateTime(2011, 4, 25));

            calendarDateAdd.ExcludePeriods.Add(timeRange1);
            calendarDateAdd.ExcludePeriods.Add(timeRange2);

            calendarDateAdd.Add(test, TimeSpan.Zero).Should().Be(test);
            calendarDateAdd.Add(test, DurationUtil.Days(2)).Should().Be(test.AddDays(2)); // 4월 14일
            calendarDateAdd.Add(test, DurationUtil.Days(3)).Should().Be(timeRange1.End); // 4월 15일인데 예외 기간이라서 4월 20일
            calendarDateAdd.Add(test, DurationUtil.Days(4)).Should().Be(timeRange1.End.AddDays(1)); // 4월 21일
            calendarDateAdd.Add(test, DurationUtil.Days(5)).Should().Be(timeRange2.End); // 4월 22일인데 예외 기간이라서 4월 25일
            calendarDateAdd.Add(test, DurationUtil.Days(7)).Should().Be(timeRange2.End.AddDays(2)); // 4월 27일
        }

        [Test]
        public void CalendarDateAddSeekBoundaryModeTest() {
            CultureTestData.Default
                .RunEach(culture => {
                             var timeCalendar = new TimeCalendar(new TimeCalendarConfig
                                                                 {
                                                                     Culture = culture,
                                                                     EndOffset = TimeSpan.Zero
                                                                 });

                             var calendarDateAdd = new CalendarDateAdd(timeCalendar);

                             //! 주중 8시~18시가 Working Time 이고, 4월 4일은 제외한다
                             //
                             calendarDateAdd.AddWorkingWeekDays();
                             calendarDateAdd.ExcludePeriods.Add(new DayRange(2011, 4, 4, calendarDateAdd.TimeCalendar));
                             calendarDateAdd.WorkingHours.Add(new HourRangeInDay(8, 18));

                             var start = new DateTime(2011, 4, 1, 9, 0, 0);

                             calendarDateAdd.Add(start, DurationUtil.Hours(29), SeekBoundaryMode.Fill).Should().Be(new DateTime(2011, 4,
                                                                                                                                6, 18, 0,
                                                                                                                                0));
                             calendarDateAdd.Add(start, DurationUtil.Hours(29), SeekBoundaryMode.Next).Should().Be(new DateTime(2011, 4,
                                                                                                                                7, 8, 0,
                                                                                                                                0));
                             calendarDateAdd.Add(start, DurationUtil.Hours(29)).Should().Be(new DateTime(2011, 4, 7, 8, 0, 0));
                         });
        }

        [Test]
        public void CalendarDateAdd1Test() {
            CultureTestData.Default
                .RunEach(culture => {
                             var timeCalendar = new TimeCalendar(new TimeCalendarConfig
                                                                 {
                                                                     Culture = culture,
                                                                     EndOffset = TimeSpan.Zero
                                                                 });

                             var calendarDateAdd = new CalendarDateAdd(timeCalendar);

                             //! 주중 8시~18시가 Working Time 이고, 4월 4일은 제외한다
                             //
                             calendarDateAdd.AddWorkingWeekDays();
                             calendarDateAdd.ExcludePeriods.Add(new DayRange(2011, 4, 4, calendarDateAdd.TimeCalendar));
                             calendarDateAdd.WorkingHours.Add(new HourRangeInDay(8, 18));

                             var start = new DateTime(2011, 4, 1, 9, 0, 0);

                             calendarDateAdd.Add(start, DurationUtil.Hours(22)).Should().Be(new DateTime(2011, 4, 6, 11, 0, 0));
                             calendarDateAdd.Add(start, DurationUtil.Hours(22), SeekBoundaryMode.Fill).Should().Be(new DateTime(2011, 4,
                                                                                                                                6, 11, 0,
                                                                                                                                0));

                             calendarDateAdd.Add(start, DurationUtil.Hours(29)).Should().Be(new DateTime(2011, 4, 7, 8, 0, 0));
                             calendarDateAdd.Add(start, DurationUtil.Hours(29), SeekBoundaryMode.Fill).Should().Be(new DateTime(2011, 4,
                                                                                                                                6, 18, 0,
                                                                                                                                0));
                         });
        }

        [Test]
        public void CalendarDateAdd2Test() {
            var calendarDateAdd = new CalendarDateAdd();

            //! 주중 8시~12시, 13시~18시가 Working Time 이고, 4월 4일은 제외한다
            //
            calendarDateAdd.AddWorkingWeekDays();
            calendarDateAdd.ExcludePeriods.Add(new DayRange(2011, 4, 4, calendarDateAdd.TimeCalendar));
            calendarDateAdd.WorkingHours.Add(new HourRangeInDay(8, 12));
            calendarDateAdd.WorkingHours.Add(new HourRangeInDay(13, 18));

            DateTime start = new DateTime(2011, 4, 1, 9, 0, 0);

            calendarDateAdd.Add(start, DurationUtil.Hours(3)).Should().Be(calendarDateAdd.Add(start, DurationUtil.Hours(3)));
            calendarDateAdd.Add(start, DurationUtil.Hours(4)).Should().Be(new DateTime(2011, 4, 1, 14, 0, 0));
            calendarDateAdd.Add(start, DurationUtil.Hours(8)).Should().Be(new DateTime(2011, 4, 5, 08, 0, 0));
        }

        [Test]
        public void CalendarDateAdd3Test() {
            var calendarDateAdd = new CalendarDateAdd();

            //! 주중 8:30~12:00, 13:30~18:00가 Working Time 이고, 4월 4일은 제외한다
            //
            calendarDateAdd.AddWorkingWeekDays();
            calendarDateAdd.ExcludePeriods.Add(new DayRange(2011, 4, 4, calendarDateAdd.TimeCalendar));
            calendarDateAdd.WorkingHours.Add(new HourRangeInDay(new TimeValue(8, 30), new TimeValue(12)));
            calendarDateAdd.WorkingHours.Add(new HourRangeInDay(new TimeValue(13, 30), new TimeValue(18)));

            var start = new DateTime(2011, 4, 1, 9, 0, 0);

            calendarDateAdd.Add(start, DurationUtil.Hours(3)).Should().Be(new DateTime(2011, 4, 1, 13, 30, 0));
            calendarDateAdd.Add(start, DurationUtil.Hours(4)).Should().Be(new DateTime(2011, 4, 1, 14, 30, 0));
            calendarDateAdd.Add(start, DurationUtil.Hours(8)).Should().Be(new DateTime(2011, 4, 5, 09, 0, 0));
        }

        [Test]
        public void EmptyStartWeekTest() {
            var calendarDateAdd = new CalendarDateAdd();

            //! 주중(월~금)만 Working Time으로 넣었습니다.
            calendarDateAdd.AddWorkingWeekDays();

            var start = new DateTime(2011, 4, 2, 13, 0, 0);
            var offset = DurationUtil.Hours(20); // 20 hours

            // 4월 2일(토), 4월 3일(일) 제외하면... 4월 4일 0시부터 20시간
            calendarDateAdd.Add(start, DurationUtil.Hours(20)).Should().Be(new DateTime(2011, 4, 4, 20, 0, 0));

            // 4월 2일(토), 4월 3일(일) 제외하면... 4월 4일 0시부터 24시간
            calendarDateAdd.Add(start, DurationUtil.Hours(24)).Should().Be(new DateTime(2011, 4, 5, 0, 0, 0));

            // 4월 2일(토), 4월 3일(일) 제외하면... 4월 4일 0시부터 5일이후 이므로 4월9일 (토), 4월 10일(일) 제외하면 4월 11일이 된다.
            calendarDateAdd.Add(start, DurationUtil.Days(5)).Should().Be(new DateTime(2011, 4, 11, 0, 0, 0));
        }
    }
}