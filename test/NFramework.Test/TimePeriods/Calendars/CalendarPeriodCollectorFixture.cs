using System;
using System.Linq;
using NSoft.NFramework.Reflections;
using NSoft.NFramework.TimePeriods.TimeRanges;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.TimePeriods.Calendars {
    [TestFixture]
    public class CalendarPeriodCollectorFixture : TimePeriodFixtureBase {
        [Test]
        public void CollectYearsTest() {
            var filter = new CalendarPeriodCollectorFilter();

            filter.Years.Add(2006);
            filter.Years.Add(2007);
            filter.Years.Add(2012);

            var testPeriod = new CalendarTimeRange(new DateTime(2001, 1, 1), new DateTime(2019, 12, 31));

            var collector = new CalendarPeriodCollector(filter, testPeriod);
            collector.CollectYears();

            if(IsDebugEnabled)
                log.Debug("CollectYears... Period=" + collector.Periods.CollectionToString());

            collector.Periods.Count.Should().Be(filter.Years.Count);

            for(var i = 0; i < collector.Periods.Count; i++)
                collector.Periods[i].IsSamePeriod(new YearRange(filter.Years.ElementAt(i)));
        }

        [Test]
        public void CollectMonthsTest() {
            var filter = new CalendarPeriodCollectorFilter();
            filter.Months.Add(January);

            var testPeriod = new CalendarTimeRange(new DateTime(2010, 1, 1), new DateTime(2011, 12, 31));

            var collector = new CalendarPeriodCollector(filter, testPeriod);
            collector.CollectMonths();

            if(IsDebugEnabled)
                log.Debug("CollectMonths... Period=" + collector.Periods.CollectionToString());

            collector.Periods.Count.Should().Be(2);
            collector.Periods[0].IsSamePeriod(new MonthRange(2010, January)).Should().Be.True();
            collector.Periods[1].IsSamePeriod(new MonthRange(2011, January)).Should().Be.True();
        }

        [Test]
        public void CollectDaysTest() {
            var filter = new CalendarPeriodCollectorFilter();

            //! 1월의 금요일만 추출
            filter.Months.Add(January);
            filter.WeekDays.Add(DayOfWeek.Friday);

            var testPeriod = new CalendarTimeRange(new DateTime(2010, 1, 1), new DateTime(2011, 12, 31));

            var collector = new CalendarPeriodCollector(filter, testPeriod);
            collector.CollectDays();

            if(IsDebugEnabled) {
                foreach(var period in collector.Periods)
                    log.Debug("CollectDays... Period=" + period);
            }

            collector.Periods.Count.Should().Be(9);

            collector.Periods[0].IsSamePeriod(new DayRange(new DateTime(2010, 1, 1))).Should().Be.True();
            collector.Periods[1].IsSamePeriod(new DayRange(new DateTime(2010, 1, 8))).Should().Be.True();
            collector.Periods[2].IsSamePeriod(new DayRange(new DateTime(2010, 1, 15))).Should().Be.True();
            collector.Periods[3].IsSamePeriod(new DayRange(new DateTime(2010, 1, 22))).Should().Be.True();
            collector.Periods[4].IsSamePeriod(new DayRange(new DateTime(2010, 1, 29))).Should().Be.True();

            collector.Periods[5].IsSamePeriod(new DayRange(new DateTime(2011, 1, 7))).Should().Be.True();
            collector.Periods[6].IsSamePeriod(new DayRange(new DateTime(2011, 1, 14))).Should().Be.True();
            collector.Periods[7].IsSamePeriod(new DayRange(new DateTime(2011, 1, 21))).Should().Be.True();
            collector.Periods[8].IsSamePeriod(new DayRange(new DateTime(2011, 1, 28))).Should().Be.True();
        }

        [Test]
        public void CollectHoursTest() {
            //! 1월의 금요일의 8:00~18:00 까지 기간만을 계산한다.
            var filter = new CalendarPeriodCollectorFilter();

            filter.Months.Add(January);
            filter.WeekDays.Add(DayOfWeek.Friday);
            filter.CollectingHours.Add(new HourRangeInDay(8, 18));

            var testPeriod = new CalendarTimeRange(new DateTime(2010, 1, 1), new DateTime(2011, 12, 31));

            var collector = new CalendarPeriodCollector(filter, testPeriod);
            collector.CollectHours();

            Assert.AreEqual(collector.Periods.Count, 9);
            Assert.IsTrue(
                collector.Periods[0].IsSamePeriod(new CalendarTimeRange(new DateTime(2010, 1, 01, 8, 0, 0),
                                                                        new DateTime(2010, 1, 01, 18, 0, 0))),
                collector.Periods[0].ToString());
            Assert.IsTrue(
                collector.Periods[1].IsSamePeriod(new CalendarTimeRange(new DateTime(2010, 1, 08, 8, 0, 0),
                                                                        new DateTime(2010, 1, 08, 18, 0, 0))),
                collector.Periods[1].ToString());
            Assert.IsTrue(
                collector.Periods[2].IsSamePeriod(new CalendarTimeRange(new DateTime(2010, 1, 15, 8, 0, 0),
                                                                        new DateTime(2010, 1, 15, 18, 0, 0))),
                collector.Periods[2].ToString());
            Assert.IsTrue(
                collector.Periods[3].IsSamePeriod(new CalendarTimeRange(new DateTime(2010, 1, 22, 8, 0, 0),
                                                                        new DateTime(2010, 1, 22, 18, 0, 0))),
                collector.Periods[3].ToString());
            Assert.IsTrue(
                collector.Periods[4].IsSamePeriod(new CalendarTimeRange(new DateTime(2010, 1, 29, 8, 0, 0),
                                                                        new DateTime(2010, 1, 29, 18, 0, 0))),
                collector.Periods[4].ToString());
            Assert.IsTrue(
                collector.Periods[5].IsSamePeriod(new CalendarTimeRange(new DateTime(2011, 1, 07, 8, 0, 0),
                                                                        new DateTime(2011, 1, 07, 18, 0, 0))),
                collector.Periods[5].ToString());
            Assert.IsTrue(
                collector.Periods[6].IsSamePeriod(new CalendarTimeRange(new DateTime(2011, 1, 14, 8, 0, 0),
                                                                        new DateTime(2011, 1, 14, 18, 0, 0))),
                collector.Periods[6].ToString());
            Assert.IsTrue(
                collector.Periods[7].IsSamePeriod(new CalendarTimeRange(new DateTime(2011, 1, 21, 8, 0, 0),
                                                                        new DateTime(2011, 1, 21, 18, 0, 0))),
                collector.Periods[7].ToString());
            Assert.IsTrue(
                collector.Periods[8].IsSamePeriod(new CalendarTimeRange(new DateTime(2011, 1, 28, 8, 0, 0),
                                                                        new DateTime(2011, 1, 28, 18, 0, 0))),
                collector.Periods[8].ToString());
        }

        [Test]
        public void CollectHoursWithMinutesTest() {
            //! 1월의 금요일의 8:30~18:30 까지 기간만을 계산한다.
            var filter = new CalendarPeriodCollectorFilter();

            filter.Months.Add(January);
            filter.WeekDays.Add(DayOfWeek.Friday);
            filter.CollectingHours.Add(new HourRangeInDay(new TimeValue(8, 30), new TimeValue(18, 50)));

            var testPeriod = new CalendarTimeRange(new DateTime(2010, 1, 1), new DateTime(2011, 12, 31));

            var collector = new CalendarPeriodCollector(filter, testPeriod);
            collector.CollectHours();

            Assert.AreEqual(collector.Periods.Count, 9);
            Assert.IsTrue(
                collector.Periods[0].IsSamePeriod(new CalendarTimeRange(new DateTime(2010, 1, 01, 8, 30, 0),
                                                                        new DateTime(2010, 1, 01, 18, 50, 0))),
                collector.Periods[0].ToString());
            Assert.IsTrue(
                collector.Periods[1].IsSamePeriod(new CalendarTimeRange(new DateTime(2010, 1, 08, 8, 30, 0),
                                                                        new DateTime(2010, 1, 08, 18, 50, 0))),
                collector.Periods[1].ToString());
            Assert.IsTrue(
                collector.Periods[2].IsSamePeriod(new CalendarTimeRange(new DateTime(2010, 1, 15, 8, 30, 0),
                                                                        new DateTime(2010, 1, 15, 18, 50, 0))),
                collector.Periods[2].ToString());
            Assert.IsTrue(
                collector.Periods[3].IsSamePeriod(new CalendarTimeRange(new DateTime(2010, 1, 22, 8, 30, 0),
                                                                        new DateTime(2010, 1, 22, 18, 50, 0))),
                collector.Periods[3].ToString());
            Assert.IsTrue(
                collector.Periods[4].IsSamePeriod(new CalendarTimeRange(new DateTime(2010, 1, 29, 8, 30, 0),
                                                                        new DateTime(2010, 1, 29, 18, 50, 0))),
                collector.Periods[4].ToString());
            Assert.IsTrue(
                collector.Periods[5].IsSamePeriod(new CalendarTimeRange(new DateTime(2011, 1, 07, 8, 30, 0),
                                                                        new DateTime(2011, 1, 07, 18, 50, 0))),
                collector.Periods[5].ToString());
            Assert.IsTrue(
                collector.Periods[6].IsSamePeriod(new CalendarTimeRange(new DateTime(2011, 1, 14, 8, 30, 0),
                                                                        new DateTime(2011, 1, 14, 18, 50, 0))),
                collector.Periods[6].ToString());
            Assert.IsTrue(
                collector.Periods[7].IsSamePeriod(new CalendarTimeRange(new DateTime(2011, 1, 21, 8, 30, 0),
                                                                        new DateTime(2011, 1, 21, 18, 50, 0))),
                collector.Periods[7].ToString());
            Assert.IsTrue(
                collector.Periods[8].IsSamePeriod(new CalendarTimeRange(new DateTime(2011, 1, 28, 8, 30, 0),
                                                                        new DateTime(2011, 1, 28, 18, 50, 0))),
                collector.Periods[8].ToString());
        }

        [Test]
        public void CollectExcludePeriodTest() {
            const int workingDays2011 = 365 - 2 - (51 * 2) - 1;
            const int workingDaysMarch2011 = 31 - 8; // total days - weekend days

            var year2011 = new YearRange(2011);

            var filter1 = new CalendarPeriodCollectorFilter();
            filter1.AddWorkingWeekDays();

            var collector1 = new CalendarPeriodCollector(filter1, year2011);
            collector1.CollectDays();
            collector1.Periods.Count.Should().Be(workingDays2011);

            // 3월 제외
            var filter2 = new CalendarPeriodCollectorFilter();
            filter2.AddWorkingWeekDays();
            filter2.ExcludePeriods.Add(new MonthRange(2011, March));

            var collector2 = new CalendarPeriodCollector(filter2, year2011);
            collector2.CollectDays();
            collector2.Periods.Count.Should().Be(workingDays2011 - workingDaysMarch2011);


            // 2011년 26주차~27주차 (여름휴가가 2주야!!!)
            //
            var filter3 = new CalendarPeriodCollectorFilter();
            filter3.AddWorkingWeekDays();
            filter3.ExcludePeriods.Add(new MonthRange(2011, March));
            filter3.ExcludePeriods.Add(new WeekRangeCollection(2011, 26, 2));

            var collector3 = new CalendarPeriodCollector(filter3, year2011);
            collector3.CollectDays();
            collector3.Periods.Count.Should().Be(workingDays2011 - workingDaysMarch2011 - 2 * TimeSpec.WeekDaysPerWeek);
        }
    }
}