using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using NSoft.NFramework.LinqEx;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.TimePeriods.Tools {
    [Microsoft.Silverlight.Testing.Tag("TimePeriods")]
    [TestFixture]
    public class TimeToolFixture_Linq : TimePeriodFixtureBase {
        private DateTime? _startTime;
        private DateTime? _endTime;

        protected virtual DateTime? StartTime {
            get { return _startTime ?? (_startTime = new DateTime(2008, 4, 10, 5, 33, 24, 345)); }
        }

        protected virtual DateTime? EndTime {
            get { return _endTime ?? (_endTime = new DateTime(2012, 10, 20, 13, 43, 12, 599)); }
        }

        private TimeRange GetPeriod() {
            return new TimeRange(StartTime, EndTime, false);
        }

        [Test]
        public void ForEachYearsTest() {
            var count = 0;
            var period = GetPeriod();

            if(period.HasPeriod) {
                TimeTool
                    .ForEachYears(period)
                    .RunEach(year => {
                                 if(IsDebugEnabled)
                                     log.Debug("Year [{0}] = {1}", count, year);
                                 count++;
                             });

                Assert.AreEqual(period.ForEachYears().Count(), count);
            }
        }

        [Test]
        public void ForEachMonthsTest() {
            var count = 0;
            var period = GetPeriod();

            if(period.HasPeriod) {
                TimeTool
                    .ForEachMonths(period)
                    .RunEach(month => {
                                 if(IsDebugEnabled)
                                     log.Debug("Month [{0}] = {1}", count, month);

                                 count++;
                             });

                Assert.AreEqual(period.ForEachMonths().Count(), count);
            }
        }

        [Test]
        public void ForEachWeeksTest() {
            var count = 0;
            var period = GetPeriod();

            if(period.HasPeriod) {
                var weeks = TimeTool.ForEachWeeks(period).Take(100);
                weeks.RunEach(week => {
                                  count++;

                                  if(IsDebugEnabled)
                                      log.Debug("Week[{0}] : {1} ~ {2}, WeekOfYear={3}",
                                                count,
                                                week.Start.ToShortDateString(),
                                                week.End.ToShortDateString(),
                                                week.End.GetWeekOfYear(CultureInfo.CurrentCulture, WeekOfYearRuleKind.Iso8601));
                              });
                weeks.Count().Should().Be(count);
            }
        }

        [Test]
        public void ForEachDaysTest() {
            var count = 0;
            var period = GetPeriod();

            if(period.HasPeriod) {
                var days = TimeTool.ForEachDays(period).Take(100);
                days.RunEach(day => count++);

                Assert.AreEqual(days.Count(), count);

                days.ElementAt(0).Start.Should().Be(StartTime);
            }
        }

        [Test]
        public void ForEachHoursTest() {
            var count = 0;
            var period = GetPeriod();

            if(period.HasPeriod) {
                var hours = TimeTool.ForEachHours(period).Take(100);
                hours.RunEach(hour => count++);

                Assert.AreEqual(hours.Count(), count);
            }
        }

        [Test]
        public void ForEachMinutesTest() {
            var count = 0;
            var period = GetPeriod();

            if(period.HasPeriod) {
                var minutes = TimeTool.ForEachMinutes(period).Take(100);
                minutes.RunEach(m => count++);

                Assert.AreEqual(minutes.Count(), count);
            }
        }

        [TestCase(PeriodKind.Year)]
        [TestCase(PeriodKind.Halfyear)]
        [TestCase(PeriodKind.Quarter)]
        [TestCase(PeriodKind.Month)]
        [TestCase(PeriodKind.Week)]
        [TestCase(PeriodKind.Day)]
        [TestCase(PeriodKind.Hour)]
        [TestCase(PeriodKind.Minute)]
        public void ForEachPeriodsTest(PeriodKind periodKind) {
            var count = 0;
            var period = GetPeriod();

            if(period.HasPeriod) {
                var periods = TimeTool.ForEachPeriods(period, periodKind).Take(1000);
                periods.RunEach(item => { count++; });

                Assert.AreEqual(periods.Count(), count);
            }
        }

        [TestCase(PeriodKind.Year)]
        [TestCase(PeriodKind.Halfyear)]
        [TestCase(PeriodKind.Quarter)]
        [TestCase(PeriodKind.Month)]
        [TestCase(PeriodKind.Week)]
        [TestCase(PeriodKind.Day)]
        [TestCase(PeriodKind.Hour)]
        [TestCase(PeriodKind.Minute)]
        public void RunPeriodTest(PeriodKind periodKind) {
            var period = GetPeriod();

            if(period.HasPeriod) {
                var periods = TimeTool.ForEachPeriods(period, periodKind).Take(1000);

                var count = 0;
                var max = TimeTool.RunPeriod(period, periodKind, p => ++count).Take(1000).Last();

                Assert.AreEqual(periods.Count(), max);
            }
        }

        [TestCase(PeriodKind.Year)]
        [TestCase(PeriodKind.Halfyear)]
        [TestCase(PeriodKind.Quarter)]
        [TestCase(PeriodKind.Month)]
        [TestCase(PeriodKind.Week)]
        [TestCase(PeriodKind.Day)]
        [TestCase(PeriodKind.Hour)]
        [TestCase(PeriodKind.Minute)]
        public void RunPeriodAsParallelTest(PeriodKind periodKind) {
            var period = GetPeriod();

            if(period.HasPeriod) {
                var periods = TimeTool.ForEachPeriods(period, periodKind).Take(1000);

                //! 이건 좋은 예가 아닙니다. 병렬 작업 중에 locking을 수행하는 것은 좋은 코드가 아닙니다.
                //! 단지, 병렬로 작업이 처리될 수 있음을 테스트 하기 위한 코드입니다.
                //
                var count = 0;
                var lastCount =
                    TimeTool.RunPeriodAsParallel(period, periodKind, p => Interlocked.Increment(ref count)).OrderBy(x => x).Take(1000).
                        Max();

                Assert.AreEqual(periods.Count(), lastCount);
            }
        }

#if !SILVERLIGHT
        [Test]
        [TestCase(PeriodKind.Year)]
        [TestCase(PeriodKind.Halfyear)]
        [TestCase(PeriodKind.Quarter)]
        [TestCase(PeriodKind.Month)]
        [TestCase(PeriodKind.Week)]
        [TestCase(PeriodKind.Day)]
        [TestCase(PeriodKind.Hour)]
        [TestCase(PeriodKind.Minute)]
        public void RunPeriodAsyncTest(PeriodKind periodKind) {
            var period = GetPeriod();

            if(period.HasPeriod) {
                if(periodKind == PeriodKind.Day) {
                    period.ShrinkStartTo(period.End.AddYears(-1));
                }
                if(periodKind == PeriodKind.Hour || periodKind == PeriodKind.Minute) {
                    period.ShrinkStartTo(period.End.AddMonths(-1));
                }

                var periods = TimeTool.ForEachPeriods(period, periodKind).Take(1000);

                var count = 0;
                var max = TimeTool.RunPeriodAsync(period, periodKind, p => Interlocked.Increment(ref count)).Take(1000).Last();

                Assert.AreEqual(periods.Count(), max);
            }
        }
#endif
    }

    [TestFixture]
    public class TimeToolFixture_IsEmpty : TimeToolFixture_Linq {
        protected override DateTime? StartTime {
            get { return null; }
        }

        protected override DateTime? EndTime {
            get { return null; }
        }
    }

    [TestFixture]
    public class TimeToolFixture_IsStartOpenRange : TimeToolFixture_Linq {
        protected override DateTime? StartTime {
            get { return null; }
        }
    }

    [TestFixture]
    public class TimeToolFixture_IsEndOpenRange : TimeToolFixture_Linq {
        protected override DateTime? StartTime {
            get { return null; }
        }
    }
}