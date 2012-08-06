using System;
using System.Globalization;
using System.Threading.Tasks;
using NUnit.Framework;

namespace NSoft.NFramework.TimePeriods.Tools {
    [Microsoft.Silverlight.Testing.Tag("TimePeriods")]
    [TestFixture]
    public class TimeToolFixture_Compare : TimePeriodFixtureBase {
        private readonly DateTime testDate = new DateTime(2000, 10, 2, 13, 45, 53, 673);
        private readonly DateTime testDiffDate = new DateTime(2002, 9, 3, 7, 14, 22, 234);

        [Test]
        public void IsSameYearTest() {
            Assert.IsFalse(TimeTool.IsSameYear(testDate, testDiffDate));
            Assert.IsTrue(TimeTool.IsSameYear(new DateTime(2000, 1, 1), new DateTime(2000, 12, 31)));

            Assert.IsTrue(TimeTool.IsSameYear(new DateTime(2000, 4, 1), new DateTime(2001, 3, 31), April));
            Assert.IsFalse(TimeTool.IsSameYear(new DateTime(2000, 1, 1), new DateTime(2000, 4, 1), April));
        }

        [Test]
        public void IsSameHalfyearTest() {
            Assert.IsTrue(TimeTool.IsSameHalfyear(new DateTime(2000, 1, 1), new DateTime(2000, 6, 30)));
            Assert.IsTrue(TimeTool.IsSameHalfyear(new DateTime(2000, 7, 1), new DateTime(2000, 12, 31)));
            Assert.IsFalse(TimeTool.IsSameHalfyear(new DateTime(2000, 1, 1), new DateTime(2000, 7, 1)));
            Assert.IsFalse(TimeTool.IsSameHalfyear(new DateTime(2000, 7, 1), new DateTime(2001, 1, 1)));

            Assert.IsTrue(TimeTool.IsSameHalfyear(new DateTime(2000, 4, 1), new DateTime(2000, 9, 30), April));
            Assert.IsTrue(TimeTool.IsSameHalfyear(new DateTime(2000, 10, 1), new DateTime(2001, 3, 31), April));
            Assert.IsFalse(TimeTool.IsSameHalfyear(new DateTime(2000, 4, 1), new DateTime(2000, 10, 1), April));
            Assert.IsFalse(TimeTool.IsSameHalfyear(new DateTime(2000, 10, 1), new DateTime(2001, 4, 1), April));
        }

        [Test]
        public void IsSameQuarterTest() {
            Assert.IsTrue(TimeTool.IsSameQuarter(new DateTime(2000, 1, 1), new DateTime(2000, 3, 31)));
            Assert.IsTrue(TimeTool.IsSameQuarter(new DateTime(2000, 4, 1), new DateTime(2000, 6, 30)));
            Assert.IsTrue(TimeTool.IsSameQuarter(new DateTime(2000, 7, 1), new DateTime(2000, 9, 30)));
            Assert.IsTrue(TimeTool.IsSameQuarter(new DateTime(2000, 10, 1), new DateTime(2000, 12, 31)));

            Assert.IsFalse(TimeTool.IsSameQuarter(new DateTime(2000, 1, 1), new DateTime(2000, 4, 1)));
            Assert.IsFalse(TimeTool.IsSameQuarter(new DateTime(2000, 4, 1), new DateTime(2000, 7, 1)));
            Assert.IsFalse(TimeTool.IsSameQuarter(new DateTime(2000, 7, 1), new DateTime(2000, 10, 1)));
            Assert.IsFalse(TimeTool.IsSameQuarter(new DateTime(2000, 10, 1), new DateTime(2001, 1, 1)));

            Assert.IsTrue(TimeTool.IsSameQuarter(new DateTime(2000, 4, 1), new DateTime(2000, 6, 30), April));
            Assert.IsTrue(TimeTool.IsSameQuarter(new DateTime(2000, 7, 1), new DateTime(2000, 9, 30), April));
            Assert.IsTrue(TimeTool.IsSameQuarter(new DateTime(2000, 10, 1), new DateTime(2000, 12, 31), April));
            Assert.IsTrue(TimeTool.IsSameQuarter(new DateTime(2001, 1, 1), new DateTime(2001, 3, 30), April));

            Assert.IsFalse(TimeTool.IsSameQuarter(new DateTime(2000, 4, 1), new DateTime(2000, 7, 1), April));
            Assert.IsFalse(TimeTool.IsSameQuarter(new DateTime(2000, 7, 1), new DateTime(2000, 10, 1), April));
            Assert.IsFalse(TimeTool.IsSameQuarter(new DateTime(2000, 10, 1), new DateTime(2001, 1, 1), April));
            Assert.IsFalse(TimeTool.IsSameQuarter(new DateTime(2001, 1, 1), new DateTime(2001, 4, 1), April));
        }

        [Test]
        public void IsSameMonthTest() {
            Assert.IsFalse(TimeTool.IsSameMonth(testDate, testDiffDate));
            Assert.IsTrue(TimeTool.IsSameMonth(new DateTime(2000, 10, 1), new DateTime(2000, 10, 31)));
            Assert.IsTrue(TimeTool.IsSameMonth(new DateTime(2000, 10, 1), new DateTime(2000, 10, 1)));
            Assert.IsTrue(TimeTool.IsSameMonth(new DateTime(2000, 10, 31), new DateTime(2000, 10, 1)));
            Assert.IsFalse(TimeTool.IsSameMonth(new DateTime(2000, 10, 1), new DateTime(2000, 11, 1)));
            Assert.IsFalse(TimeTool.IsSameMonth(new DateTime(2000, 10, 1), new DateTime(2000, 9, 30)));
        }

#if !SILVERLIGHT
        [Test]
        public void IsSameWeekTest() {
            var previousWeek = testDate.AddDays(TimeSpec.DaysPerWeek + 1);
            var nextWeek = testDate.AddDays(TimeSpec.DaysPerWeek + 1);

            Parallel.ForEach(CultureTestData.Default,
                             culture => {
                                 foreach(CalendarWeekRule weekRule in Enum.GetValues(typeof(CalendarWeekRule))) {
                                     culture.DateTimeFormat.CalendarWeekRule = weekRule;

                                     foreach(WeekOfYearRuleKind yearWeekType in Enum.GetValues(typeof(WeekOfYearRuleKind))) {
                                         int year;
                                         int weekOfYear;

                                         TimeTool.GetWeekOfYear(testDate, culture, yearWeekType, out year, out weekOfYear);
                                         var startOfWeek = TimeTool.GetStartOfYearWeek(year, weekOfYear, culture, yearWeekType);

                                         if(IsDebugEnabled)
                                             log.Debug("testDate=[{0}], startOfWeek=[{1}]", testDate, startOfWeek);

                                         Assert.IsTrue(TimeTool.IsSameWeek(testDate, startOfWeek, culture, yearWeekType));
                                         Assert.IsTrue(TimeTool.IsSameWeek(testDate, testDate, culture, yearWeekType));
                                         Assert.IsTrue(TimeTool.IsSameWeek(testDiffDate, testDiffDate, culture, yearWeekType));
                                         Assert.IsFalse(TimeTool.IsSameWeek(testDate, testDiffDate, culture, yearWeekType));
                                         Assert.IsFalse(TimeTool.IsSameWeek(testDate, previousWeek, culture, yearWeekType));
                                         Assert.IsFalse(TimeTool.IsSameWeek(testDate, nextWeek, culture, yearWeekType));

                                         foreach(DayOfWeek dayOfWeek in Enum.GetValues(typeof(DayOfWeek))) {
                                             culture.DateTimeFormat.FirstDayOfWeek = dayOfWeek;
                                             TimeTool.GetWeekOfYear(testDate, culture, weekRule, dayOfWeek, yearWeekType, out year,
                                                                    out weekOfYear);
                                             startOfWeek = TimeTool.GetStartOfYearWeek(year, weekOfYear, culture, weekRule, dayOfWeek,
                                                                                       yearWeekType);

                                             Assert.IsTrue(TimeTool.IsSameWeek(testDate, startOfWeek, culture, yearWeekType));
                                             Assert.IsTrue(TimeTool.IsSameWeek(testDate, testDate, culture, yearWeekType));
                                             Assert.IsTrue(TimeTool.IsSameWeek(testDiffDate, testDiffDate, culture, yearWeekType));
                                             Assert.IsFalse(TimeTool.IsSameWeek(testDate, testDiffDate, culture, yearWeekType));
                                             Assert.IsFalse(TimeTool.IsSameWeek(testDate, previousWeek, culture, yearWeekType));
                                             Assert.IsFalse(TimeTool.IsSameWeek(testDate, nextWeek, culture, yearWeekType));
                                         }
                                     }
                                 }
                             });
        }
#endif

        [Test]
        public void IsSameDayTest() {
            Assert.IsFalse(TimeTool.IsSameDay(testDate, testDiffDate));
            Assert.IsTrue(TimeTool.IsSameDay(new DateTime(2000, 10, 19), new DateTime(2000, 10, 19)));
            Assert.IsTrue(TimeTool.IsSameDay(new DateTime(2000, 10, 19), new DateTime(2000, 10, 19).AddDays(1).AddTicks(-1)));
            Assert.IsFalse(TimeTool.IsSameDay(new DateTime(1978, 10, 19), new DateTime(2000, 10, 19)));
            Assert.IsFalse(TimeTool.IsSameDay(new DateTime(2000, 10, 18), new DateTime(2000, 10, 17)));
            Assert.IsFalse(TimeTool.IsSameDay(new DateTime(2000, 10, 18), new DateTime(2000, 10, 19)));
        }

        [Test]
        public void IsSameHourTest() {
            Assert.IsFalse(TimeTool.IsSameHour(testDate, testDiffDate));
            Assert.IsTrue(TimeTool.IsSameHour(new DateTime(2000, 10, 19, 18, 0, 0),
                                              new DateTime(2000, 10, 19, 18, 0, 0).AddHours(1).AddTicks(-1)));
            Assert.IsFalse(TimeTool.IsSameHour(new DateTime(1978, 10, 19, 18, 0, 0), new DateTime(2000, 10, 19, 17, 0, 0)));
            Assert.IsFalse(TimeTool.IsSameHour(new DateTime(1978, 10, 19, 18, 0, 0), new DateTime(2000, 10, 19, 19, 0, 0)));
        }

        [Test]
        public void IsSameMinuteTest() {
            Assert.IsFalse(TimeTool.IsSameMinute(testDate, testDiffDate));
            Assert.IsTrue(TimeTool.IsSameMinute(new DateTime(2000, 10, 19, 18, 20, 0),
                                                new DateTime(2000, 10, 19, 18, 20, 0).AddMinutes(1).AddTicks(-1)));
            Assert.IsFalse(TimeTool.IsSameMinute(new DateTime(1978, 10, 19, 18, 20, 0), new DateTime(2000, 10, 19, 18, 19, 0)));
            Assert.IsFalse(TimeTool.IsSameMinute(new DateTime(1978, 10, 19, 18, 20, 0), new DateTime(2000, 10, 19, 18, 21, 0)));
        }

        [Test]
        public void IsSameSecondTest() {
            Assert.IsFalse(TimeTool.IsSameSecond(testDate, testDiffDate));
            Assert.IsTrue(TimeTool.IsSameSecond(new DateTime(2000, 10, 19, 18, 20, 30),
                                                new DateTime(2000, 10, 19, 18, 20, 30).AddSeconds(1).AddTicks(-1)));
            Assert.IsFalse(TimeTool.IsSameSecond(new DateTime(1978, 10, 19, 18, 20, 30), new DateTime(2000, 10, 19, 18, 20, 29)));
            Assert.IsFalse(TimeTool.IsSameSecond(new DateTime(1978, 10, 19, 18, 20, 30), new DateTime(2000, 10, 19, 18, 20, 31)));
        }
    }
}