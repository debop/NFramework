using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using NSoft.NFramework.LinqEx;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.TimePeriods.TimeRanges {
    [Microsoft.Silverlight.Testing.Tag("TimePeriods")]
    [TestFixture]
    public class WeekRangeFixture : TimePeriodFixtureBase {
        [Test]
        public void DefaultCalendarTest() {
            const int startWeek = 1;
            var currentTime = ClockProxy.Clock.Now;
            var currentYear = ClockProxy.Clock.Now.Year;

            Action<CultureInfo> @weekRangeTestAction =
                (culture) => {
                    foreach(WeekOfYearRuleKind yearWeekType in WeekOfYearRules) {
                        var yearWeek = WeekTool.GetYearAndWeek(currentTime, culture, yearWeekType);

                        for(int weekOfYear = startWeek; weekOfYear < yearWeek.Week; weekOfYear++) {
                            var week = new WeekRange(currentYear, weekOfYear, TimeCalendar.New(culture, 1, yearWeekType));
                            week.Year.Should().Be(currentYear);

                            var weekStart = WeekTool.GetWeekRange(new YearAndWeek(currentYear, weekOfYear), culture, yearWeekType).Start;
                            var weekEnd = weekStart.AddDays(TimeSpec.DaysPerWeek);

                            week.UnmappedStart.Should().Be(weekStart);
                            week.UnmappedEnd.Should().Be(weekEnd);
                        }
                    }
                };

            Parallel.For(0, CultureTestData.Default.Count(), i => @weekRangeTestAction(CultureTestData.Default[i]));

            //!++ BUG:Silverlight 용 Paralell.ForEach 에 버그가 있습니다.
            // Parallel.ForEach(CultureTestData.Default.Cultures, @weekRangeTestAction);
        }

        [Test]
        public void EnAuCultureTest() {
            var cultureInfo = new CultureInfo("en-AU");
            //	cultureInfo.DateTimeFormat.CalendarWeekRule = CalendarWeekRule.FirstFourDayWeek;
            var timeCalendar = new TimeCalendar(new TimeCalendarConfig { Culture = cultureInfo });
            var week = new WeekRange(new DateTime(2011, 4, 1, 9, 0, 0), timeCalendar);
            week.Start.Should().Be(new DateTime(2011, 3, 28));
        }

        [Test]
        public void WeekByCultures() {
            var now = ClockProxy.Clock.Now;

            CultureTestData.Default
                .RunEach(culture => {
                             if(IsDebugEnabled)
                                 log.Debug("culture = " + culture);

                             var timeCalendar = new TimeCalendar(new TimeCalendarConfig() { Culture = culture });
                             var week = new WeekRange(now, timeCalendar);

                             week.StartDayStart.Should().Be(now.StartTimeOfWeek(culture).Date);
                             week.EndDayStart.Should().Be(now.EndTimeOfWeek(culture).Date);
                         });
        }
    }
}