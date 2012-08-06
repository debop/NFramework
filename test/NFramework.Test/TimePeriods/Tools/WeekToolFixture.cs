using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.Parallelism.Tools;
using NSoft.NFramework.Tools;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.TimePeriods.Tools {
    //!++ BUG:Silverlight 용 Paralell.ForEach 에 버그가 있습니다.

    [Microsoft.Silverlight.Testing.Tag("TimePeriods")]
    [TestFixture]
    public class WeekToolFixture : TimePeriodFixtureBase {
        //private static readonly WeekOfYearRuleKind[] _weekOfYearRuleKinds = EnumExtensions.GetEnumValues<WeekOfYearRuleKind>();

        [TestCase(2003, 12, 28, 2003, 52)]
        [TestCase(2003, 12, 29, 2004, 1)]
        [TestCase(2010, 12, 29, 2010, 52)]
        [TestCase(2010, 1, 3, 2009, 53)]
        [TestCase(2010, 12, 31, 2010, 52)]
        [TestCase(2011, 1, 1, 2010, 52)]
        [TestCase(2011, 1, 3, 2011, 1)]
        [TestCase(2012, 1, 1, 2011, 52)]
        public void GetYearAndWeekByIso8601(int year, int month, int day, int weekYear, int week) {
            var moment = new DateTime(year, month, day);
            var yearAndWeek = WeekTool.GetYearAndWeek(moment, null, WeekOfYearRuleKind.Iso8601);

            Assert.AreEqual(weekYear, yearAndWeek.Year);
            Assert.AreEqual(week, yearAndWeek.Week);
        }

        [Test]
        public void GetStartWeekRangeOfYear_ISO8601() {
            Parallel.For(2000, 2100,
                         year => {
                             var startWeekRange = WeekTool.GetStartWeekRangeOfYear(year, null, WeekOfYearRuleKind.Iso8601);

                             if(IsDebugEnabled)
                                 log.Debug("Year=[{0}], StartWeekRange=[{1}]", year, startWeekRange);

                             startWeekRange.Start.Date.Subtract(new DateTime(year - 1, 12, 28)).TotalDays.Should().Be.GreaterThan(0);
                             startWeekRange.End.Date.Subtract(new DateTime(year, 1, 3)).TotalDays.Should().Be.GreaterThan(0);
                         });
        }

        /// <summary>
        /// 모든 CalendarWeekRule에 따라 전년도 마지막날과 새해 첫날의 WeekOfYear를 비교한다.
        /// </summary>
        [Test]
        public void GetYearAndWeekTest() {
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("ko-KR");
            var rule = WeekOfYearRuleKind.Calendar;
            CalendarWeekRule weekRule;
            DayOfWeek firstDayOfWeek;

            foreach(var culture in CultureTestData.Default.Cultures) {
                var currentCulture = culture;
                WeekTool.GetCalendarWeekRuleAndFirstDayOfWeek(currentCulture, rule, out weekRule, out firstDayOfWeek);

                if(IsDebugEnabled)
                    log.Debug("WeekOfYearRule=[{0}], CalendarWeekRule=[{1}], FirstDayOfWeek=[{2}]", rule, weekRule, firstDayOfWeek);


                CalendarWeekRule weekRule1 = weekRule;
                DayOfWeek firstDayOfWeek1 = firstDayOfWeek;

                Parallel.For(2000, 2100,
                             year => {
                                 var startDay = TimeTool.StartTimeOfYear(year);
                                 var endDay = startDay.Add(TimeSpec.MinNegativeDuration);

                                 var startDayYearAndWeek = WeekTool.GetYearAndWeek(startDay, currentCulture);
                                 var endDayYearAndWeek = WeekTool.GetYearAndWeek(endDay, currentCulture);

                                 if(IsDebugEnabled)
                                     log.Debug("{0},{1} ==> End:{2} Start:{3}", endDay, startDay, endDayYearAndWeek, startDayYearAndWeek);


                                 // 새해 첫날은 무조건 첫 주가 된다.
                                 if(weekRule1 == CalendarWeekRule.FirstDay) {
                                     Assert.AreNotEqual(endDayYearAndWeek, startDayYearAndWeek);
                                 }
                                 else if(weekRule1 == CalendarWeekRule.FirstFullWeek) {
                                     if(startDay.DayOfWeek == firstDayOfWeek1)
                                         Assert.AreNotEqual(endDayYearAndWeek, startDayYearAndWeek);
                                     else
                                         Assert.AreEqual(endDayYearAndWeek, startDayYearAndWeek);
                                 }
                                 else if(weekRule1 == CalendarWeekRule.FirstFourDayWeek) {
                                     if(startDay.DayOfWeek == firstDayOfWeek1)
                                         Assert.AreNotEqual(endDayYearAndWeek, startDayYearAndWeek);
                                     //else
                                     //    Assert.AreEqual(endDayYearAndWeek, startDayYearAndWeek,
                                     //                    "endDay=[{0}], startDay=[{1}], culture=[{2}]",
                                     //                    endDayYearAndWeek, startDayYearAndWeek, currentCulture);
                                 }
                             });
            }

            rule = WeekOfYearRuleKind.Iso8601;
            WeekTool.GetCalendarWeekRuleAndFirstDayOfWeek(CultureInfo.CurrentCulture, rule, out weekRule, out firstDayOfWeek);


            Parallel.For(2000, 2100,
                         year => {
                             var startDay = TimeTool.StartTimeOfYear(year);
                             var endDay = startDay.Add(TimeSpec.MinNegativeDuration);

                             var startDayYearAndWeek = WeekTool.GetYearAndWeek(startDay, null, rule);
                             var endDayYearAndWeek = WeekTool.GetYearAndWeek(endDay, null, rule);

                             if(IsDebugEnabled)
                                 log.Debug("{0},{1} ==> End:{2} Start:{3}", endDay, startDay, endDayYearAndWeek, startDayYearAndWeek);


                             if(startDay.DayOfWeek == firstDayOfWeek)
                                 Assert.AreNotEqual(endDayYearAndWeek, startDayYearAndWeek);
                             else
                                 Assert.AreEqual(endDayYearAndWeek, startDayYearAndWeek);
                         });
        }

        /// <summary>
        /// 한해의 마지막 주차를 산정합니다.
        /// </summary>
        [Test]
        public void GetEndYearAndWeekTest() {
            Parallel.For(0, CultureTestData.Default.Count(),
                         i => {
                             var currentCulture = CultureTestData.Default[i];

                             Enumerable.Range(2000, 100)
                                 .RunEach(year => {
                                              var yearWeek = WeekTool.GetEndYearAndWeek(year, currentCulture);

                                              Assert.AreEqual(year, yearWeek.Year);
                                              Assert.IsTrue(yearWeek.Week >= 52);
                                          });
                         });

            Parallel.For(1980, 2200,
                         year => {
                             var yearWeek = WeekTool.GetEndYearAndWeek(year, null, WeekOfYearRuleKind.Iso8601);

                             Assert.AreEqual(year, yearWeek.Year);
                             Assert.IsTrue(yearWeek.Week >= 52);
                         });
        }

        [Test]
        public void GetWeekRangeTest() {
            Parallel.For(0, CultureTestData.Default.Count(),
                         i => {
                             var currentCulture = CultureTestData.Default[i];

                             Enumerable
                                 .Range(2000, 20)
                                 .RunEach(year => {
                                              var endDay = (year - 1).GetEndOfYear();
                                              var startDay = year.GetStartOfYear();

                                              var endDayYearWeek = WeekTool.GetYearAndWeek(endDay, currentCulture);
                                              Assert.IsTrue(endDayYearWeek.Year >= year - 1);

                                              var startDayYearWeek = WeekTool.GetYearAndWeek(startDay, currentCulture);
                                              Assert.IsTrue(startDayYearWeek.Year <= year);

                                              // 해당일자가 속한 주차의 일자들을 구한다. 년말/년초 구간은 꼭 7일이 아닐 수 있다.
                                              //
                                              var endDayWeekRange = WeekTool.GetWeekRange(endDayYearWeek, currentCulture);
                                              var startDayWeekRange = WeekTool.GetWeekRange(startDayYearWeek, currentCulture);

                                              Assert.IsTrue(endDayWeekRange.HasPeriod);
                                              Assert.IsTrue(startDayWeekRange.HasPeriod);

                                              if(IsDebugEnabled)
                                                  log.Debug("Start Day Weeks = " + startDayWeekRange);

                                              if(endDayYearWeek == startDayYearWeek)
                                                  Assert.AreEqual(endDayWeekRange,
                                                                  startDayWeekRange,
                                                                  @"culture={0}, End={1}, Start={2}, EndOfWeek={3}, StartOfWeek={4}",
                                                                  currentCulture, endDayWeekRange, startDayWeekRange, endDayYearWeek,
                                                                  startDayYearWeek);
                                              else
                                                  Assert.AreNotEqual(endDayWeekRange,
                                                                     startDayWeekRange,
                                                                     @"culture={0}, End={1}, Start={2}, EndOfWeek={3}, StartOfWeek={4}",
                                                                     currentCulture, endDayWeekRange, startDayWeekRange, endDayYearWeek,
                                                                     startDayYearWeek);
                                          });
                         });

            var rule = WeekOfYearRuleKind.Iso8601;

            Parallel.For(2000, 2020,
                         year => {
                             var endDay = (year - 1).GetEndOfYear();
                             var startDay = year.GetStartOfYear();

                             var endDayYearWeek = WeekTool.GetYearAndWeek(endDay, null, rule);
                             Assert.IsTrue(endDayYearWeek.Year >= year - 1);

                             var startDayYearWeek = WeekTool.GetYearAndWeek(startDay, null, rule);
                             Assert.IsTrue(startDayYearWeek.Year <= year);

                             // 해당일자가 속한 주차의 일자들을 구한다. 년말/년초 구간은 꼭 7일이 아닐 수 있다.
                             //
                             var endDayWeekRange = WeekTool.GetWeekRange(endDayYearWeek, null, rule);
                             var startDayWeekRange = WeekTool.GetWeekRange(startDayYearWeek, null, rule);

                             Assert.IsTrue(endDayWeekRange.HasPeriod);
                             Assert.IsTrue(startDayWeekRange.HasPeriod);

                             if(IsDebugEnabled)
                                 log.Debug("Start Day Weeks = " + startDayWeekRange);

                             if(endDayYearWeek == startDayYearWeek)
                                 Assert.AreEqual(endDayWeekRange,
                                                 startDayWeekRange,
                                                 @"rule={0}, End={1}, Start={2}, EndOfWeek={3}, StartOfWeek={4}",
                                                 rule, endDayWeekRange, startDayWeekRange, endDayYearWeek, startDayYearWeek);
                             else
                                 Assert.AreNotEqual(endDayWeekRange,
                                                    startDayWeekRange,
                                                    @"rule={0}, End={1}, Start={2}, EndOfWeek={3}, StartOfWeek={4}",
                                                    rule, endDayWeekRange, startDayWeekRange, endDayYearWeek, startDayYearWeek);
                         });
        }

        [Test]
        public void AddWeekOfYearsTest() {
            const int maxAddWeeks = 40;

            Action<CultureInfo> @AddWeekOfYearsAction =
                (culture) => {
                    var currentCulture = culture;

                    Enumerable
                        .Range(2000, 20)
                        .RunEach(year => {
                                     const int step = 10;
                                     YearAndWeek prevResult = new YearAndWeek();

                                     var maxWeek = WeekTool.GetEndYearAndWeek(year, currentCulture);

                                     for(var week = 1; week < maxWeek.Week; week += step) {
                                         for(var addWeeks = -maxAddWeeks; addWeeks <= maxAddWeeks; addWeeks += step) {
                                             var current = new YearAndWeek(year, week);
                                             var result = WeekTool.AddWeekOfYears(current, addWeeks, currentCulture);

                                             if(addWeeks != 0 && prevResult.Week.HasValue) {
                                                 if(result.Year == prevResult.Year)
                                                     Assert.AreEqual(prevResult.Week + step, result.Week,
                                                                     @"Prev=[{0}], Result=[{1}], addWeeks=[{2}]", prevResult, result,
                                                                     addWeeks);
                                             }

                                             result.Week.Should().Have.Value();
                                             result.Week.Value.Should().Be.GreaterThan(0);
                                             result.Week.Value.Should().Be.LessThanOrEqualTo(TimeSpec.MaxWeeksPerYear);

                                             prevResult = result;

                                             if(IsDebugEnabled)
                                                 log.Debug("Current=[{0}], Added=[{1}], AddWeeks=[{2}]", current, result, addWeeks);
                                         }
                                     }
                                 });
                };


            Parallel.For(0, CultureTestData.Default.Count(), i => @AddWeekOfYearsAction(CultureTestData.Default[i]));

            var rule = WeekOfYearRuleKind.Iso8601;

            ParallelTool.ForWithStep(2000, 2020, 2,
                                     year => {
                                         const int step = 10;
                                         var prevResult = new YearAndWeek();

                                         var maxWeek = WeekTool.GetEndYearAndWeek(year, null, rule);

                                         for(var week = 1; week < maxWeek.Week; week += step) {
                                             for(var addWeeks = -maxAddWeeks; addWeeks <= maxAddWeeks; addWeeks += step) {
                                                 var current = new YearAndWeek(year, week);
                                                 var result = WeekTool.AddWeekOfYears(current, addWeeks, null, rule);

                                                 if(addWeeks != 0 && prevResult.Week.HasValue) {
                                                     if(result.Year == prevResult.Year)
                                                         Assert.AreEqual(prevResult.Week + step, result.Week,
                                                                         @"Prev={0}, Result={1}, addWeeks={2}", prevResult, result,
                                                                         addWeeks);
                                                 }

                                                 result.Week.Should().Have.Value();
                                                 result.Week.Value.Should().Be.GreaterThan(0);
                                                 result.Week.Value.Should().Be.LessThanOrEqualTo(TimeSpec.MaxWeeksPerYear);

                                                 prevResult = result;

                                                 if(IsDebugEnabled)
                                                     log.Debug("Current=[{0}], Added=[{1}], AddWeeks=[{2}]", current, result, addWeeks);
                                             }
                                         }
                                     });
        }
    }
}