using System;
using System.Linq;
using System.Threading.Tasks;
using NSoft.NFramework.LinqEx;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.TimePeriods.Tools {
    [Microsoft.Silverlight.Testing.Tag("TimePeriods")]
    [TestFixture]
    public class TimeToolFixture_Period : TimePeriodFixtureBase {
        private const int PeriodCount = 24;

        private DateTime? _startTime;
        private DateTime? _endTime;

        protected virtual DateTime startTime {
            get { return (_startTime ?? (_startTime = new DateTime(2008, 4, 10, 5, 33, 24, 345))).Value; }
        }

        protected virtual DateTime endTime {
            get { return (_endTime ?? (_endTime = new DateTime(2018, 10, 20, 13, 43, 12, 599))).Value; }
        }

        protected virtual TimeSpan duration {
            get { return endTime.Subtract(startTime); }
        }

        [Test]
        public void GetTimeBlockByDuration() {
            var timeBlock = TimeTool.GetTimeBlock(startTime, duration);

            timeBlock.Start.Should().Be(startTime);
            timeBlock.End.Should().Be(endTime);
            timeBlock.Duration.Should().Be(duration);
        }

        [Test]
        public void GetTimeBlockByStartAndEnd() {
            var timeBlock = TimeTool.GetTimeBlock(startTime, endTime);
            timeBlock.Start.Should().Be(startTime);
            timeBlock.End.Should().Be(endTime);
            timeBlock.Duration.Should().Be(duration);
        }

        [Test]
        public void GetTimeRangeByDuration() {
            var timeRange = TimeTool.GetTimeRange(startTime, duration);

            timeRange.Start.Should().Be(startTime);
            timeRange.End.Should().Be(endTime);
            timeRange.Duration.Should().Be(duration);
        }

        [Test]
        public void GetTimeRangeByNegateDuration() {
            var timeRange = TimeTool.GetTimeRange(startTime, duration.Negate());

            timeRange.Start.Should().Be(startTime.Subtract(duration));
            timeRange.End.Should().Be(endTime.Subtract(duration));
            timeRange.Duration.Should().Be(duration);
        }

        [Test]
        public void GetTimeRangeByStartAndEnd() {
            var timeRange = TimeTool.GetTimeRange(startTime, endTime);
            timeRange.Start.Should().Be(startTime);
            timeRange.End.Should().Be(endTime);
            timeRange.Duration.Should().Be(duration);
        }

#if !SILVERLIGHT
        [Test]
        public void GetPeriodOfTest() {
            foreach(PeriodKind periodKind in Enum.GetValues(typeof(PeriodKind))) {
                if(periodKind == PeriodKind.Unknown)
                    continue;
                if(periodKind == PeriodKind.Millisecond)
                    continue;

                var moment = startTime;

                var period = TimeTool.GetPeriodOf(moment, periodKind);

                period.HasInside(moment).Should().Be.True();
                period.HasInside(endTime).Should().Be.False();

                if(IsDebugEnabled)
                    log.Debug("[{0}] : Period[{1}] HasInside([{2}])", periodKind, period, moment);
            }
        }

        [Test]
        public void GetPeriodOfWithCalendarTest() {
            foreach(PeriodKind periodKind in Enum.GetValues(typeof(PeriodKind))) {
                if(periodKind == PeriodKind.Unknown)
                    continue;

                if(periodKind == PeriodKind.Millisecond)
                    continue;

                var moment = startTime;
                var timeCalendar = TimeCalendar.NewEmptyOffset();

                var period = TimeTool.GetPeriodOf(moment, periodKind, timeCalendar);

                period.HasInside(moment).Should().Be.True();
                period.HasInside(endTime).Should().Be.False();

                if(IsDebugEnabled)
                    log.Debug("[{0}] : Period[{1}] HasInside([{2}])", periodKind, period, moment);
            }
        }

        [Test]
        public void GetPeriodsOfTest() {
            Parallel.ForEach(Enum.GetValues(typeof(PeriodKind)).Cast<PeriodKind>(),
                             periodKind => {
                                 if(periodKind == PeriodKind.Unknown)
                                     return;

                                 if(periodKind == PeriodKind.Millisecond)
                                     return;

                                 for(int count = 1; count < 5; count++) {
                                     var moment = startTime;
                                     var timeCalendar = TimeCalendar.NewEmptyOffset();

                                     var period = TimeTool.GetPeriodsOf(moment, periodKind, count, timeCalendar);

                                     period.HasPeriod.Should().Be.True();
                                     period.HasInside(moment).Should().Be.True();
                                     period.HasInside(endTime).Should().Be.False();

                                     if(IsDebugEnabled)
                                         log.Debug("[{0}] : Periods [{1}] HasInside([{2}])", periodKind, period, moment);
                                 }
                             });
        }
#endif

        [Test]
        public void GetYearRangeTest() {
            var yearRange = TimeTool.GetYearRange(startTime, TimeCalendar.NewEmptyOffset());

            var start = startTime.StartTimeOfYear();
            yearRange.Start.Should().Be(start);
            yearRange.StartYear.Should().Be(start.Year);
            yearRange.End.Should().Be(start.AddYears(1));
            yearRange.EndYear.Should().Be(start.AddYears(1).Year);
        }

        [Test]
        public void GetYearRangesTest() {
            Enumerable
                .Range(1, PeriodCount)
                .RunEach(i => {
                             var yearRanges = TimeTool.GetYearRanges(startTime, i, TimeCalendar.NewEmptyOffset());

                             var start = startTime.StartTimeOfYear();
                             yearRanges.Start.Should().Be(start);
                             yearRanges.StartYear.Should().Be(start.Year);
                             yearRanges.End.Should().Be(start.AddYears(i));
                             yearRanges.EndYear.Should().Be(start.AddYears(i).Year);

                             yearRanges.YearCount.Should().Be(i);
                         });
        }

        [Test]
        public void GetHalfyearRangeTest() {
            var halfyearRange = TimeTool.GetHalfyearRange(startTime, TimeCalendar.NewEmptyOffset());

            var start = startTime.StartTimeOfHalfyear();

            halfyearRange.Start.Should().Be(start);
            halfyearRange.End.Should().Be(halfyearRange.GetNextHalfyear().Start);
        }

        [Test]
        public void GetHalfyearRangesTest() {
            Enumerable
                .Range(1, PeriodCount)
                .RunEach(i => {
                             var halfyearRanges = TimeTool.GetHalfyearRanges(startTime, i, TimeCalendar.NewEmptyOffset());

                             var start = startTime.StartTimeOfHalfyear();

                             halfyearRanges.Start.Should().Be(start);
                             halfyearRanges.End.Should().Be(start.AddMonths(i * TimeSpec.MonthsPerHalfyear));
                             halfyearRanges.HalfyearCount.Should().Be(i);
                         });
        }

        [Test]
        public void GetQuarterRangeTest() {
            var quarterRange = TimeTool.GetQuarterRange(startTime, TimeCalendar.NewEmptyOffset());

            var start = startTime.StartTimeOfQuarter();

            quarterRange.Start.Should().Be(start);
            quarterRange.End.Should().Be(quarterRange.GetNextQuarter().Start);
        }

        [Test]
        public void GetQuarterRangesTest() {
            Enumerable
                .Range(1, PeriodCount)
                .RunEach(i => {
                             var quarterRanges = TimeTool.GetQuarterRanges(startTime, i, TimeCalendar.NewEmptyOffset());

                             var start = startTime.StartTimeOfQuarter();

                             quarterRanges.Start.Should().Be(start);
                             quarterRanges.End.Should().Be(start.AddMonths(i * TimeSpec.MonthsPerQuarter));
                             quarterRanges.QuarterCount.Should().Be(i);
                         });
        }

        [Test]
        public void GetMonthRangeTest() {
            var monthRange = TimeTool.GetMonthRange(startTime, TimeCalendar.NewEmptyOffset());

            var start = startTime.StartTimeOfMonth();

            monthRange.Start.Should().Be(start);
            monthRange.End.Should().Be(monthRange.GetNextMonth().Start);
        }

        [Test]
        public void GetMonthRangesTest() {
            Enumerable
                .Range(1, PeriodCount)
                .RunEach(i => {
                             var monthRanges = TimeTool.GetMonthRanges(startTime, i, TimeCalendar.NewEmptyOffset());

                             var start = startTime.StartTimeOfMonth();

                             monthRanges.Start.Should().Be(start);
                             monthRanges.End.Should().Be(start.AddMonths(i));
                             monthRanges.MonthCount.Should().Be(i);
                         });
        }

        [Test]
        public void GetWeekRangeTest() {
            var weekRange = TimeTool.GetWeekRange(startTime, TimeCalendar.NewEmptyOffset());

            var start = startTime.StartTimeOfWeek();

            weekRange.Start.Should().Be(start);
            weekRange.End.Should().Be(weekRange.GetNextWeek().Start);
        }

        [Test]
        public void GetWeekRangesTest() {
            Enumerable
                .Range(1, PeriodCount)
                .RunEach(i => {
                             var weekRanges = TimeTool.GetWeekRanges(startTime, i, TimeCalendar.NewEmptyOffset());

                             var start = TimeTool.StartTimeOfWeek(startTime);

                             weekRanges.Start.Should().Be(start);
                             weekRanges.End.Should().Be(start.AddDays(i * TimeSpec.DaysPerWeek));
                             weekRanges.WeekCount.Should().Be(i);
                         });
        }

        [Test]
        public void GetDayRangeTest() {
            var dayRange = TimeTool.GetDayRange(startTime, TimeCalendar.NewEmptyOffset());

            var start = startTime.StartTimeOfDay();

            dayRange.Start.Should().Be(start);
            dayRange.End.Should().Be(dayRange.GetNextDay().Start);
        }

        [Test]
        public void GetDayRangesTest() {
            Enumerable
                .Range(1, PeriodCount)
                .RunEach(i => {
                             var dayRanges = TimeTool.GetDayRanges(startTime, i, TimeCalendar.NewEmptyOffset());

                             var start = startTime.StartTimeOfDay();

                             dayRanges.Start.Should().Be(start);
                             dayRanges.End.Should().Be(start.AddDays(i));
                             dayRanges.DayCount.Should().Be(i);
                         });
        }

        [Test]
        public void GetHourRangeTest() {
            var hourRange = TimeTool.GetHourRange(startTime, TimeCalendar.NewEmptyOffset());

            var start = startTime.StartTimeOfHour();

            hourRange.Start.Should().Be(start);
            hourRange.End.Should().Be(hourRange.GetNextHour().Start);
        }

        [Test]
        public void GetHourRangesTest() {
            Enumerable
                .Range(1, PeriodCount)
                .RunEach(i => {
                             var hourRanges = TimeTool.GetHourRanges(startTime, i, TimeCalendar.NewEmptyOffset());

                             var start = startTime.StartTimeOfHour();

                             hourRanges.Start.Should().Be(start);
                             hourRanges.End.Should().Be(start.AddHours(i));
                             hourRanges.HourCount.Should().Be(i);
                         });
        }

        [Test]
        public void GetMinuteRangeTest() {
            var minuteRange = TimeTool.GetMinuteRange(startTime, TimeCalendar.NewEmptyOffset());

            var start = startTime.StartTimeOfMinute();

            minuteRange.Start.Should().Be(start);
            minuteRange.End.Should().Be(minuteRange.GetNextMinute().Start);
        }

        [Test]
        public void GetMinuteRangesTest() {
            Enumerable
                .Range(1, PeriodCount)
                .RunEach(i => {
                             var hourRanges = TimeTool.GetMinuteRanges(startTime, i, TimeCalendar.NewEmptyOffset());

                             var start = startTime.StartTimeOfMinute();

                             hourRanges.Start.Should().Be(start);
                             hourRanges.End.Should().Be(start.AddMinutes(i));
                             hourRanges.MinuteCount.Should().Be(i);
                         });
        }
    }
}