using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.TimePeriods.Core {
    [TestFixture]
    public class TimeCalendarFixture : TimePeriodFixtureBase {
        private readonly DateTime _testDate = ClockProxy.Clock.Now; //new DateTime(2011, 3, 18, 14, 9, 34, 234);

        [Test]
        public void StartOffsetTest() {
            Assert.AreEqual(TimeCalendar.DefaultStartOffset, new TimeCalendar().StartOffset);

            var offset = DurationUtil.Second;
            var timeCalendar = new TimeCalendar(new TimeCalendarConfig
                                                {
                                                    StartOffset = offset,
                                                    EndOffset = TimeSpan.Zero
                                                });
            Assert.AreEqual(offset, timeCalendar.StartOffset);
        }

        [Test]
        public void EndOffsetTest() {
            Assert.AreEqual(TimeCalendar.DefaultEndOffset, new TimeCalendar().EndOffset);

            var offset = DurationUtil.Second.Negate();
            var timeCalendar = new TimeCalendar(new TimeCalendarConfig
                                                {
                                                    StartOffset = TimeSpan.Zero,
                                                    EndOffset = offset
                                                });
            Assert.AreEqual(offset, timeCalendar.EndOffset);
        }

        [Test]
        public void InvalidOffset1Test() {
            Assert.Throws<InvalidOperationException>(() =>
                                                     new TimeCalendar(new TimeCalendarConfig
                                                                      {
                                                                          StartOffset = DurationUtil.Second.Negate(),
                                                                          EndOffset = TimeSpan.Zero
                                                                      }));
        }

        [Test]
        public void InvalidOffset2Test() {
            Assert.Throws<InvalidOperationException>(() =>
                                                     new TimeCalendar(new TimeCalendarConfig
                                                                      {
                                                                          StartOffset = TimeSpan.Zero,
                                                                          EndOffset = DurationUtil.Second
                                                                      }));
        }

        [Test]
        public void CultureTest() {
            Assert.AreEqual(Thread.CurrentThread.CurrentCulture, new TimeCalendar().Culture);
            CultureTestData.Default.All(culture => TimeCalendar.New(culture).Culture == culture).Should().Be.True();
        }

        [Test]
        public void FirstDayOfWeekTest() {
            Assert.AreEqual(Thread.CurrentThread.CurrentCulture.DateTimeFormat.FirstDayOfWeek, new TimeCalendar().FirstDayOfWeek);
            CultureTestData.Default.All(culture => TimeCalendar.New(culture).FirstDayOfWeek == culture.DateTimeFormat.FirstDayOfWeek).
                Should().Be.True();
        }

        [Test]
        public void MapStartTest() {
            var now = ClockProxy.Clock.Now;
            var offset = DurationUtil.Second;

            foreach(var culture in CultureTestData.Default) {
                Assert.AreEqual(TimeCalendar.New(culture, TimeSpan.Zero, TimeSpan.Zero).MapStart(now), now);
                Assert.AreEqual(TimeCalendar.New(culture, offset, TimeSpan.Zero).MapStart(now), now.Add(offset));

                Assert.AreEqual(TimeCalendar.New(culture, TimeSpan.Zero, TimeSpan.Zero).MapStart(_testDate), _testDate);
                Assert.AreEqual(TimeCalendar.New(culture, offset, TimeSpan.Zero).MapStart(_testDate), _testDate.Add(offset));
            }
        }

        [Test]
        public void UnmapStartTest() {
            var now = ClockProxy.Clock.Now;
            var offset = DurationUtil.Second;

            foreach(CultureInfo culture in CultureTestData.Default) {
                Assert.AreEqual(now, TimeCalendar.New(culture, TimeSpan.Zero, TimeSpan.Zero).UnmapStart(now));
                Assert.AreEqual(now.Subtract(offset), TimeCalendar.New(culture, offset, TimeSpan.Zero).UnmapStart(now));

                Assert.AreEqual(_testDate, TimeCalendar.New(culture, TimeSpan.Zero, TimeSpan.Zero).UnmapStart(_testDate));
                Assert.AreEqual(_testDate.Subtract(offset), TimeCalendar.New(culture, offset, TimeSpan.Zero).UnmapStart(_testDate));
            }
        }

        [Test]
        public void MapEndTest() {
            var now = ClockProxy.Clock.Now;
            var offset = DurationUtil.Second.Negate();

            foreach(var culture in CultureTestData.Default) {
                Assert.AreEqual(now, TimeCalendar.New(culture, TimeSpan.Zero, TimeSpan.Zero).MapEnd(now));
                Assert.AreEqual(now.Add(offset), TimeCalendar.New(culture, TimeSpan.Zero, offset).MapEnd(now));

                Assert.AreEqual(_testDate, TimeCalendar.New(culture, TimeSpan.Zero, TimeSpan.Zero).MapEnd(_testDate));
                Assert.AreEqual(_testDate.Add(offset), TimeCalendar.New(culture, TimeSpan.Zero, offset).MapEnd(_testDate));
            }
        }

        [Test]
        public void UnmapEndTest() {
            var now = ClockProxy.Clock.Now;
            var offset = DurationUtil.Second.Negate();

            foreach(var culture in CultureTestData.Default) {
                Assert.AreEqual(now, TimeCalendar.New(culture, TimeSpan.Zero, TimeSpan.Zero).UnmapEnd(now));
                Assert.AreEqual(now.Subtract(offset), TimeCalendar.New(culture, TimeSpan.Zero, offset).UnmapEnd(now));

                Assert.AreEqual(_testDate, TimeCalendar.New(culture, TimeSpan.Zero, TimeSpan.Zero).UnmapEnd(_testDate));
                Assert.AreEqual(_testDate.Subtract(offset), TimeCalendar.New(culture, TimeSpan.Zero, offset).UnmapEnd(_testDate));
            }
        }

        [Test]
        public void GetYearTest() {
            var now = ClockProxy.Clock.Now;

            foreach(var culture in CultureTestData.Default) {
                Assert.AreEqual(culture.Calendar.GetYear(now), TimeCalendar.New(culture).GetYear(now));
                Assert.AreEqual(culture.Calendar.GetYear(_testDate), TimeCalendar.New(culture).GetYear(_testDate));
            }
        }

        [Test]
        public void GetMonthTest() {
            var now = ClockProxy.Clock.Now;

            foreach(var culture in CultureTestData.Default) {
                Assert.AreEqual(culture.Calendar.GetMonth(now), TimeCalendar.New(culture).GetMonth(now));
                Assert.AreEqual(culture.Calendar.GetMonth(_testDate), TimeCalendar.New(culture).GetMonth(_testDate));
            }
        }

        [Test]
        public void GetHourTest() {
            var now = ClockProxy.Clock.Now;

            foreach(var culture in CultureTestData.Default) {
                Assert.AreEqual(culture.Calendar.GetHour(now), TimeCalendar.New(culture).GetHour(now));
                Assert.AreEqual(culture.Calendar.GetHour(_testDate), TimeCalendar.New(culture).GetHour(_testDate));
            }
        }

        [Test]
        public void GetMinuteTest() {
            var now = ClockProxy.Clock.Now;

            foreach(var culture in CultureTestData.Default) {
                Assert.AreEqual(culture.Calendar.GetMinute(now), TimeCalendar.New(culture).GetMinute(now));
                Assert.AreEqual(culture.Calendar.GetMinute(_testDate), TimeCalendar.New(culture).GetMinute(_testDate));
            }
        }

        [Test]
        public void GetDayOfMonthTest() {
            var now = ClockProxy.Clock.Now;

            foreach(var culture in CultureTestData.Default) {
                Assert.AreEqual(culture.Calendar.GetDayOfMonth(now), TimeCalendar.New(culture).GetDayOfMonth(now));
                Assert.AreEqual(culture.Calendar.GetDayOfMonth(_testDate), TimeCalendar.New(culture).GetDayOfMonth(_testDate));
            }
        }

        [Test]
        public void GetDayOfWeekTest() {
            var now = ClockProxy.Clock.Now;

            foreach(var culture in CultureTestData.Default) {
                Assert.AreEqual(culture.Calendar.GetDayOfWeek(now), TimeCalendar.New(culture).GetDayOfWeek(now));
                Assert.AreEqual(culture.Calendar.GetDayOfWeek(_testDate), TimeCalendar.New(culture).GetDayOfWeek(_testDate));
            }
        }

        [Test]
        public void GetDaysInMonthTest() {
            var now = ClockProxy.Clock.Now;

            foreach(var culture in CultureTestData.Default) {
                TimeCalendar.New(culture).GetDaysInMonth(now.Year, now.Month).Should().Be(culture.Calendar.GetDaysInMonth(now.Year,
                                                                                                                          now.Month));
                TimeCalendar.New(culture).GetDaysInMonth(_testDate.Year, _testDate.Month).Should().Be(
                    culture.Calendar.GetDaysInMonth(_testDate.Year, _testDate.Month));
            }
        }

        [Test]
        public void GetMonthNameTest() {
            var now = ClockProxy.Clock.Now;

            foreach(var culture in CultureTestData.Default) {
                TimeCalendar.New(culture).GetMonthName(now.Month).Should().Be(culture.DateTimeFormat.GetMonthName(now.Month));
                TimeCalendar.New(culture).GetMonthName(_testDate.Month).Should().Be(culture.DateTimeFormat.GetMonthName(_testDate.Month));
            }
        }

        [Test]
        public void GetDayNameTest() {
            var now = ClockProxy.Clock.Now;

            foreach(var culture in CultureTestData.Default) {
                TimeCalendar.New(culture).GetDayName(now.DayOfWeek).Should().Be(culture.DateTimeFormat.GetDayName(now.DayOfWeek));
                TimeCalendar.New(culture).GetDayName(_testDate.DayOfWeek).Should().Be(
                    culture.DateTimeFormat.GetDayName(_testDate.DayOfWeek));
            }
        }

        [Test]
        public void GetWeekOfYearTest() {
            var now = ClockProxy.Clock.Now;

            foreach(var culture in CultureTestData.Default) {
#if !SILVERLIGHT
                foreach(CalendarWeekRule weekRule in Enum.GetValues(typeof(CalendarWeekRule)))
#else
				foreach(CalendarWeekRule weekRule in Enumerable.Range(0,2))
#endif
                {
                    culture.DateTimeFormat.CalendarWeekRule = weekRule;

                    // calendar week
                    //
                    var timeCalendar = TimeCalendar.New(culture);
                    var yearAndWeek = TimeTool.GetWeekOfYear(now, culture, weekRule, culture.DateTimeFormat.FirstDayOfWeek,
                                                             WeekOfYearRuleKind.Calendar);

                    timeCalendar.GetWeekOfYear(now).Should().Be(yearAndWeek.Week ?? 1);


                    // iso 8601 calendar week
                    //
                    var timeCalendarIso8601 = TimeCalendar.New(culture, January, WeekOfYearRuleKind.Iso8601);
                    yearAndWeek = TimeTool.GetWeekOfYear(now, culture, weekRule, culture.DateTimeFormat.FirstDayOfWeek,
                                                         WeekOfYearRuleKind.Iso8601);

                    timeCalendarIso8601.GetWeekOfYear(now).Should().Be(yearAndWeek.Week ?? 1);
                }
            }
        }

        [Test]
        public void GetStartOfWeekTest() {
            var now = ClockProxy.Clock.Now;

            foreach(CultureInfo culture in CultureTestData.Default) {
#if !SILVERLIGHT
                foreach(CalendarWeekRule weekRule in Enum.GetValues(typeof(CalendarWeekRule)))
#else
				foreach(CalendarWeekRule weekRule in Enumerable.Range(0, 2))
#endif
                {
                    culture.DateTimeFormat.CalendarWeekRule = weekRule;

                    // calendar week
                    var yearWeek = TimeTool.GetWeekOfYear(now, culture, WeekOfYearRuleKind.Calendar);
                    var weekStartCalendar = TimeTool.GetStartOfYearWeek(yearWeek.Year ?? 0, yearWeek.Week ?? 1, culture,
                                                                        WeekOfYearRuleKind.Calendar);
                    var timeCalendar = TimeCalendar.New(culture);

                    timeCalendar.GetStartOfYearWeek(yearWeek.Year ?? 0, yearWeek.Week ?? 1).Should().Be(weekStartCalendar);

                    // iso 8601 calendar week
                    yearWeek = TimeTool.GetWeekOfYear(now, culture, WeekOfYearRuleKind.Iso8601);
                    var weekStartCalendarIso8601 = TimeTool.GetStartOfYearWeek(yearWeek.Year ?? 0, yearWeek.Week ?? 1, culture,
                                                                               WeekOfYearRuleKind.Iso8601);
                    var timeCalendarIso8601 = TimeCalendar.New(culture, January, WeekOfYearRuleKind.Iso8601);

                    timeCalendarIso8601.GetStartOfYearWeek(yearWeek.Year ?? 0, yearWeek.Week ?? 1).Should().Be(weekStartCalendarIso8601);
                }
            }
        }
    }
}