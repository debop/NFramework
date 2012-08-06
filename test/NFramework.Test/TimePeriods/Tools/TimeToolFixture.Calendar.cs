using System;
using System.Globalization;
using System.Linq;
using NSoft.NFramework.LinqEx;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.TimePeriods {
    [Microsoft.Silverlight.Testing.Tag("TimePeriods")]
    [TestFixture]
    public class TimeToolFixture_Calendar : TimePeriodFixtureBase {
        private static DateTime testTime = new DateTime(2000, 10, 2, 13, 45, 53, 673);
        private static DateTime nowTime = ClockProxy.Clock.Now;

        [Test]
        public void GetYearOfTest() {
            TimeTool.GetYearOf(new DateTime(2000, 1, 1), 1).Should().Be(2000);
            TimeTool.GetYearOf(new DateTime(2000, 4, 1), 4).Should().Be(2000);
            TimeTool.GetYearOf(new DateTime(2000, 3, 31), 4).Should().Be(1999);
            TimeTool.GetYearOf(new DateTime(2000, 1, 1), 4).Should().Be(1999);

            TimeTool.GetYearOf(nowTime, 1).Should().Be(nowTime.Year);
        }

        [Test]
        public void NextHalfyearTest() {
            TimeTool.NextHalfyear(HalfyearKind.First).Halfyear.Should().Be(HalfyearKind.Second);
            TimeTool.NextHalfyear(HalfyearKind.Second).Halfyear.Should().Be(HalfyearKind.First);
        }

        [Test]
        public void PreviousHalfyearTest() {
            TimeTool.PreviousHalfyear(HalfyearKind.First).Halfyear.Should().Be(HalfyearKind.Second);
            TimeTool.PreviousHalfyear(HalfyearKind.Second).Halfyear.Should().Be(HalfyearKind.First);
        }

        [Test]
        public void AddHalfyearTest() {
            TimeTool.AddHalfyear(HalfyearKind.First, 1).Halfyear.Should().Be(HalfyearKind.Second);
            TimeTool.AddHalfyear(HalfyearKind.Second, 1).Halfyear.Should().Be(HalfyearKind.First);
            TimeTool.AddHalfyear(HalfyearKind.First, -1).Halfyear.Should().Be(HalfyearKind.Second);
            TimeTool.AddHalfyear(HalfyearKind.Second, -1).Halfyear.Should().Be(HalfyearKind.First);

            TimeTool.AddHalfyear(HalfyearKind.First, 2).Halfyear.Should().Be(HalfyearKind.First);
            TimeTool.AddHalfyear(HalfyearKind.Second, 2).Halfyear.Should().Be(HalfyearKind.Second);
            TimeTool.AddHalfyear(HalfyearKind.First, -2).Halfyear.Should().Be(HalfyearKind.First);
            TimeTool.AddHalfyear(HalfyearKind.Second, -2).Halfyear.Should().Be(HalfyearKind.Second);

            TimeTool.AddHalfyear(HalfyearKind.First, 5).Halfyear.Should().Be(HalfyearKind.Second);
            TimeTool.AddHalfyear(HalfyearKind.Second, 5).Halfyear.Should().Be(HalfyearKind.First);
            TimeTool.AddHalfyear(HalfyearKind.First, -5).Halfyear.Should().Be(HalfyearKind.Second);
            TimeTool.AddHalfyear(HalfyearKind.Second, -5).Halfyear.Should().Be(HalfyearKind.First);

            TimeTool.AddHalfyear(HalfyearKind.First, 2008, 1).Year.Should().Be(2008);
            TimeTool.AddHalfyear(HalfyearKind.Second, 2008, 1).Year.Should().Be(2009);

            TimeTool.AddHalfyear(HalfyearKind.First, 2008, -1).Year.Should().Be(2007);
            TimeTool.AddHalfyear(HalfyearKind.Second, 2008, -1).Year.Should().Be(2008);

            TimeTool.AddHalfyear(HalfyearKind.First, 2008, 2).Year.Should().Be(2009);
            TimeTool.AddHalfyear(HalfyearKind.Second, 2008, 2).Year.Should().Be(2009);

            TimeTool.AddHalfyear(HalfyearKind.First, 2008, 3).Year.Should().Be(2009);
            TimeTool.AddHalfyear(HalfyearKind.Second, 2008, 3).Year.Should().Be(2010);
        }

        [Test]
        public void GetCalendarHalfyearOfMonthTest() {
            TimeSpec.FirstHalfyearMonths.RunEach(m => TimeTool.GetHalfyearOfMonth(m).Should().Be(HalfyearKind.First));
            TimeSpec.SecondHalfyearMonths.RunEach(m => TimeTool.GetHalfyearOfMonth(m).Should().Be(HalfyearKind.Second));
        }

        [Test]
        public void GetMonthsOfHalfyearTest() {
            Assert.AreEqual(TimeSpec.FirstHalfyearMonths, TimeTool.GetMonthsOfHalfyear(HalfyearKind.First));
            Assert.AreEqual(TimeSpec.SecondHalfyearMonths, TimeTool.GetMonthsOfHalfyear(HalfyearKind.Second));
        }

        [Test]
        public void NextQuarterTest() {
            TimeTool.NextQuarter(QuarterKind.First).Quarter.Should().Be(QuarterKind.Second);
            TimeTool.NextQuarter(QuarterKind.Second).Quarter.Should().Be(QuarterKind.Third);
            TimeTool.NextQuarter(QuarterKind.Third).Quarter.Should().Be(QuarterKind.Fourth);
            TimeTool.NextQuarter(QuarterKind.Fourth).Quarter.Should().Be(QuarterKind.First);
        }

        [Test]
        public void PreviousQuarterTest() {
            TimeTool.PreviousQuarter(QuarterKind.First).Quarter.Should().Be(QuarterKind.Fourth);
            TimeTool.PreviousQuarter(QuarterKind.Second).Quarter.Should().Be(QuarterKind.First);
            TimeTool.PreviousQuarter(QuarterKind.Third).Quarter.Should().Be(QuarterKind.Second);
            TimeTool.PreviousQuarter(QuarterKind.Fourth).Quarter.Should().Be(QuarterKind.Third);
        }

        [Test]
        public void AddQuarterTest() {
            TimeTool.AddQuarter(QuarterKind.First, 1).Quarter.Should().Be(QuarterKind.Second);
            TimeTool.AddQuarter(QuarterKind.Second, 1).Quarter.Should().Be(QuarterKind.Third);
            TimeTool.AddQuarter(QuarterKind.Third, 1).Quarter.Should().Be(QuarterKind.Fourth);
            TimeTool.AddQuarter(QuarterKind.Fourth, 1).Quarter.Should().Be(QuarterKind.First);

            TimeTool.AddQuarter(QuarterKind.First, -1).Quarter.Should().Be(QuarterKind.Fourth);
            TimeTool.AddQuarter(QuarterKind.Second, -1).Quarter.Should().Be(QuarterKind.First);
            TimeTool.AddQuarter(QuarterKind.Third, -1).Quarter.Should().Be(QuarterKind.Second);
            TimeTool.AddQuarter(QuarterKind.Fourth, -1).Quarter.Should().Be(QuarterKind.Third);

            TimeTool.AddQuarter(QuarterKind.First, 2).Quarter.Should().Be(QuarterKind.Third);
            TimeTool.AddQuarter(QuarterKind.Second, 2).Quarter.Should().Be(QuarterKind.Fourth);
            TimeTool.AddQuarter(QuarterKind.Third, 2).Quarter.Should().Be(QuarterKind.First);
            TimeTool.AddQuarter(QuarterKind.Fourth, 2).Quarter.Should().Be(QuarterKind.Second);

            TimeTool.AddQuarter(QuarterKind.First, -2).Quarter.Should().Be(QuarterKind.Third);
            TimeTool.AddQuarter(QuarterKind.Second, -2).Quarter.Should().Be(QuarterKind.Fourth);
            TimeTool.AddQuarter(QuarterKind.Third, -2).Quarter.Should().Be(QuarterKind.First);
            TimeTool.AddQuarter(QuarterKind.Fourth, -2).Quarter.Should().Be(QuarterKind.Second);

            TimeTool.AddQuarter(QuarterKind.First, 3).Quarter.Should().Be(QuarterKind.Fourth);
            TimeTool.AddQuarter(QuarterKind.Second, 3).Quarter.Should().Be(QuarterKind.First);
            TimeTool.AddQuarter(QuarterKind.Third, 3).Quarter.Should().Be(QuarterKind.Second);
            TimeTool.AddQuarter(QuarterKind.Fourth, 3).Quarter.Should().Be(QuarterKind.Third);

            TimeTool.AddQuarter(QuarterKind.First, -3).Quarter.Should().Be(QuarterKind.Second);
            TimeTool.AddQuarter(QuarterKind.Second, -3).Quarter.Should().Be(QuarterKind.Third);
            TimeTool.AddQuarter(QuarterKind.Third, -3).Quarter.Should().Be(QuarterKind.Fourth);
            TimeTool.AddQuarter(QuarterKind.Fourth, -3).Quarter.Should().Be(QuarterKind.First);

            TimeTool.AddQuarter(QuarterKind.First, 4).Quarter.Should().Be(QuarterKind.First);
            TimeTool.AddQuarter(QuarterKind.Second, 4).Quarter.Should().Be(QuarterKind.Second);
            TimeTool.AddQuarter(QuarterKind.Third, 4).Quarter.Should().Be(QuarterKind.Third);
            TimeTool.AddQuarter(QuarterKind.Fourth, 4).Quarter.Should().Be(QuarterKind.Fourth);

            TimeTool.AddQuarter(QuarterKind.First, -4).Quarter.Should().Be(QuarterKind.First);
            TimeTool.AddQuarter(QuarterKind.Second, -4).Quarter.Should().Be(QuarterKind.Second);
            TimeTool.AddQuarter(QuarterKind.Third, -4).Quarter.Should().Be(QuarterKind.Third);
            TimeTool.AddQuarter(QuarterKind.Fourth, -4).Quarter.Should().Be(QuarterKind.Fourth);

            TimeTool.AddQuarter(QuarterKind.First, 2008, 1).Year.Should().Be(2008);
            TimeTool.AddQuarter(QuarterKind.Second, 2008, 1).Year.Should().Be(2008);
            TimeTool.AddQuarter(QuarterKind.Third, 2008, 1).Year.Should().Be(2008);
            TimeTool.AddQuarter(QuarterKind.Fourth, 2008, 1).Year.Should().Be(2009);

            TimeTool.AddQuarter(QuarterKind.First, 2008, 2).Year.Should().Be(2008);
            TimeTool.AddQuarter(QuarterKind.Second, 2008, 2).Year.Should().Be(2008);
            TimeTool.AddQuarter(QuarterKind.Third, 2008, 2).Year.Should().Be(2009);
            TimeTool.AddQuarter(QuarterKind.Fourth, 2008, 2).Year.Should().Be(2009);

            TimeTool.AddQuarter(QuarterKind.First, 2008, 4).Year.Should().Be(2009);
            TimeTool.AddQuarter(QuarterKind.Second, 2008, 4).Year.Should().Be(2009);
            TimeTool.AddQuarter(QuarterKind.Third, 2008, 4).Year.Should().Be(2009);
            TimeTool.AddQuarter(QuarterKind.Fourth, 2008, 4).Year.Should().Be(2009);

            TimeTool.AddQuarter(QuarterKind.First, 2008, 5).Year.Should().Be(2009);
            TimeTool.AddQuarter(QuarterKind.Second, 2008, 5).Year.Should().Be(2009);
            TimeTool.AddQuarter(QuarterKind.Third, 2008, 5).Year.Should().Be(2009);
            TimeTool.AddQuarter(QuarterKind.Fourth, 2008, 5).Year.Should().Be(2010);
        }

        [Test]
        public void GetCalendarQuarterOfMonthTest() {
            TimeSpec.FirstQuarterMonths.RunEach(m => TimeTool.GetQuarterOfMonth(m).Should().Be(QuarterKind.First));
            TimeSpec.SecondQuarterMonths.RunEach(m => TimeTool.GetQuarterOfMonth(m).Should().Be(QuarterKind.Second));
            TimeSpec.ThirdQuarterMonths.RunEach(m => TimeTool.GetQuarterOfMonth(m).Should().Be(QuarterKind.Third));
            TimeSpec.FourthQuarterMonths.RunEach(m => TimeTool.GetQuarterOfMonth(m).Should().Be(QuarterKind.Fourth));
        }

        [Test]
        public void GetQuarterOfMonthTest() {
            TimeSpec.FirstQuarterMonths.RunEach(m => TimeTool.GetQuarterOfMonth(10, m).Should().Be(QuarterKind.Second));
            TimeSpec.SecondQuarterMonths.RunEach(m => TimeTool.GetQuarterOfMonth(10, m).Should().Be(QuarterKind.Third));
            TimeSpec.ThirdQuarterMonths.RunEach(m => TimeTool.GetQuarterOfMonth(10, m).Should().Be(QuarterKind.Fourth));
            TimeSpec.FourthQuarterMonths.RunEach(m => TimeTool.GetQuarterOfMonth(10, m).Should().Be(QuarterKind.First));
        }

        [Test]
        public void GetMonthsOfQuarterTest() {
            Assert.AreEqual(TimeSpec.FirstQuarterMonths, TimeTool.GetMonthsOfQuarter(QuarterKind.First));
            Assert.AreEqual(TimeSpec.SecondQuarterMonths, TimeTool.GetMonthsOfQuarter(QuarterKind.Second));
            Assert.AreEqual(TimeSpec.ThirdQuarterMonths, TimeTool.GetMonthsOfQuarter(QuarterKind.Third));
            Assert.AreEqual(TimeSpec.FourthQuarterMonths, TimeTool.GetMonthsOfQuarter(QuarterKind.Fourth));
        }

        [Test]
        public void NextMonthTest() {
            for(var i = 1; i <= TimeSpec.MonthsPerYear; i++)
                TimeTool.NextMonth(i).Month.Should().Be(i % TimeSpec.MonthsPerYear + 1);
        }

        [Test]
        public void PreviousMonthTest() {
            for(var i = 1; i <= TimeSpec.MonthsPerYear; i++)
                TimeTool.PreviousMonth(i).Month.Should().Be((i - 1) <= 0 ? (TimeSpec.MonthsPerYear + i - 1) : i - 1);
        }

        [Test]
        public void AddMonthTest() {
            for(int i = 1; i <= TimeSpec.MonthsPerYear; i++)
                TimeTool.AddMonth(1, i).Month.Should().Be(i % TimeSpec.MonthsPerYear + 1);

            for(var i = 1; i <= TimeSpec.MonthsPerYear; i++)
                TimeTool.AddMonth(1, -i).Month.Should().Be((1 - i) <= 0 ? (TimeSpec.MonthsPerYear + 1 - i) : 1 - i);

            const int ThreeYears = 3 * TimeSpec.MonthsPerYear;

            for(var i = 1; i < ThreeYears; i++) {
                var yearAndMonth = TimeTool.AddMonth(new YearAndMonth(2008, 1), i);

                yearAndMonth.Year.Should().Be(2008 + i / TimeSpec.MonthsPerYear);
                yearAndMonth.Month.Should().Be(i % TimeSpec.MonthsPerYear + 1);
            }
        }

        [Test]
        public void WeekOfYearCalendarTest() {
            var period = new TimeRange(new DateTime(2007, 12, 31), new DateTime(2009, 12, 31));

            var moments =
                TimeTool.ForEachDays(period)
#if !SILVERLIGHT
                    .AsParallel()
                    .AsOrdered()
#endif
                    .Select(p => p.Start);

            foreach(var culture in CultureTestData.Default) {
                var rule = WeekTool.GetWeekOfYearRuleKind(culture.DateTimeFormat.CalendarWeekRule, culture.DateTimeFormat.FirstDayOfWeek);

                if(rule == WeekOfYearRuleKind.Iso8601)
                    continue;

                foreach(var moment in moments) {
                    var calendarWeekOfYear = culture.Calendar.GetWeekOfYear(moment,
                                                                            culture.DateTimeFormat.CalendarWeekRule,
                                                                            culture.DateTimeFormat.FirstDayOfWeek);

                    var yearAndWeek = TimeTool.GetWeekOfYear(moment, culture, WeekOfYearRuleKind.Calendar);

                    Assert.AreEqual(calendarWeekOfYear, yearAndWeek.Week,
                                    "calendar WeekOfYear=[{0}], yearAndWeek=[{1}], culture=[{2}], momnent=[{3}], weekRule=[{4}], FirstDayOfWeek=[{5}]",
                                    calendarWeekOfYear, yearAndWeek, culture, moment, culture.DateTimeFormat.CalendarWeekRule,
                                    culture.DateTimeFormat.FirstDayOfWeek);
                }
            }
        }

        [Test]
        public void WeekOfYearIso8601Test() {
            var moment = new DateTime(2007, 12, 31);

            CultureTestData.Default
                .RunEach(culture => {
                             if(culture.DateTimeFormat.CalendarWeekRule != CalendarWeekRule.FirstFourDayWeek ||
                                culture.DateTimeFormat.FirstDayOfWeek != DayOfWeek.Monday)
                                 return;

                             TimeTool.GetWeekOfYear(moment, culture, WeekOfYearRuleKind.Iso8601).Week.Should().Be(1);
                         });
        }

        [Test]
        public void DayStartTest() {
            TimeTool.DayStart(testTime).Should().Be(testTime.Date);
            TimeTool.DayStart(testTime).TimeOfDay.Should().Be(TimeSpan.Zero);

            TimeTool.DayStart(nowTime).Should().Be(nowTime.Date);
            TimeTool.DayStart(nowTime).TimeOfDay.Should().Be(TimeSpan.Zero);
        }

        [Test]
        public void NextDayTest() {
            TimeTool.NextDay(DayOfWeek.Monday).Should().Be(DayOfWeek.Tuesday);
            TimeTool.NextDay(DayOfWeek.Tuesday).Should().Be(DayOfWeek.Wednesday);
            TimeTool.NextDay(DayOfWeek.Wednesday).Should().Be(DayOfWeek.Thursday);
            TimeTool.NextDay(DayOfWeek.Thursday).Should().Be(DayOfWeek.Friday);
            TimeTool.NextDay(DayOfWeek.Friday).Should().Be(DayOfWeek.Saturday);
            TimeTool.NextDay(DayOfWeek.Saturday).Should().Be(DayOfWeek.Sunday);
            TimeTool.NextDay(DayOfWeek.Sunday).Should().Be(DayOfWeek.Monday);
        }

        [Test]
        public void PreviousDayTest() {
            TimeTool.PreviousDay(DayOfWeek.Monday).Should().Be(DayOfWeek.Sunday);
            TimeTool.PreviousDay(DayOfWeek.Tuesday).Should().Be(DayOfWeek.Monday);
            TimeTool.PreviousDay(DayOfWeek.Wednesday).Should().Be(DayOfWeek.Tuesday);
            TimeTool.PreviousDay(DayOfWeek.Thursday).Should().Be(DayOfWeek.Wednesday);
            TimeTool.PreviousDay(DayOfWeek.Friday).Should().Be(DayOfWeek.Thursday);
            TimeTool.PreviousDay(DayOfWeek.Saturday).Should().Be(DayOfWeek.Friday);
            TimeTool.PreviousDay(DayOfWeek.Sunday).Should().Be(DayOfWeek.Saturday);
        }

        [Test]
        public void AddDayTest() {
            TimeTool.AddDays(DayOfWeek.Monday, 14).Should().Be(DayOfWeek.Monday);
            TimeTool.AddDays(DayOfWeek.Tuesday, 14).Should().Be(DayOfWeek.Tuesday);
            TimeTool.AddDays(DayOfWeek.Wednesday, 14).Should().Be(DayOfWeek.Wednesday);
            TimeTool.AddDays(DayOfWeek.Thursday, 14).Should().Be(DayOfWeek.Thursday);
            TimeTool.AddDays(DayOfWeek.Friday, 14).Should().Be(DayOfWeek.Friday);
            TimeTool.AddDays(DayOfWeek.Saturday, 14).Should().Be(DayOfWeek.Saturday);
            TimeTool.AddDays(DayOfWeek.Sunday, 14).Should().Be(DayOfWeek.Sunday);

            TimeTool.AddDays(DayOfWeek.Monday, -14).Should().Be(DayOfWeek.Monday);
            TimeTool.AddDays(DayOfWeek.Tuesday, -14).Should().Be(DayOfWeek.Tuesday);
            TimeTool.AddDays(DayOfWeek.Wednesday, -14).Should().Be(DayOfWeek.Wednesday);
            TimeTool.AddDays(DayOfWeek.Thursday, -14).Should().Be(DayOfWeek.Thursday);
            TimeTool.AddDays(DayOfWeek.Friday, -14).Should().Be(DayOfWeek.Friday);
            TimeTool.AddDays(DayOfWeek.Saturday, -14).Should().Be(DayOfWeek.Saturday);
            TimeTool.AddDays(DayOfWeek.Sunday, -14).Should().Be(DayOfWeek.Sunday);
        }
    }
}