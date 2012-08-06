using System;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.TimePeriods.Calendars {
    [TestFixture]
    public class DateAddFixture : TimePeriodFixtureBase {
        [Test]
        public void NoPeriodsTest() {
            var test = new DateTime(2011, 4, 12);

            var dateAdd = new DateAdd();

            dateAdd.Add(test, DurationUtil.Zero).Should().Be(test);

            dateAdd.Add(test, DurationUtil.Days(1)).Should().Be(test.AddDays(1));
            dateAdd.Add(test, DurationUtil.Days(-1)).Should().Be(test.AddDays(-1));

            dateAdd.Subtract(test, DurationUtil.Days(1)).Should().Be(test.AddDays(-1));
            dateAdd.Subtract(test, DurationUtil.Days(-1)).Should().Be(test.AddDays(1));
        }

        [Test]
        public void PeriodLimitsAddTest() {
            var test = new DateTime(2011, 4, 12);

            var timeRange1 = new TimeRange(new DateTime(2011, 4, 20), new DateTime(2011, 4, 25)); // 4월 20일~4월25일
            var timeRange2 = new TimeRange(new DateTime(2011, 4, 30), null); // 4월 30일 이후 

            var dateAdd = new DateAdd();

            // 예외 기간을 설정합니다. 4월20일 ~ 4월25일, 4월30일 이후
            //
            dateAdd.ExcludePeriods.Add(timeRange1);
            dateAdd.ExcludePeriods.Add(timeRange2);

            dateAdd.Add(test, DurationUtil.Day).Should().Be(test.Add(DurationUtil.Day));

            //! 4월 12일에 8일을 더하면 4월 20일이지만, 20~25일까지 제외되므로, 4월 25일이 된다.
            //
            dateAdd.Add(test, DurationUtil.Days(8)).Should().Be(timeRange1.End);

            //! 4월 20일에 20일을 더하면, 4월 20~25일 제외 후를 계산하면 4월 30일 이후가 된다. 하지만 4월 30일 이후는 제외가 되므로, 결과값은 null이 된다.
            //
            dateAdd.Add(test, DurationUtil.Days(20)).HasValue.Should().Be.False();

            dateAdd.Subtract(test, DurationUtil.Days(3)).Should().Be(test.Subtract(DurationUtil.Days(3)));
        }

        [Test]
        public void PeriodLimitsSubtractTest() {
            var test = new DateTime(2011, 4, 30);

            var timeRange1 = new TimeRange(new DateTime(2011, 4, 20), new DateTime(2011, 4, 25));
            var timeRange2 = new TimeRange(DateTime.MinValue, new DateTime(2011, 4, 6));

            var dateAdd = new DateAdd();

            // 예외 기간을 설정합니다. 4월 10일 이전, 4월20일 ~ 4월25일
            //
            dateAdd.ExcludePeriods.Add(timeRange1);
            dateAdd.ExcludePeriods.Add(timeRange2);

            dateAdd.Subtract(test, DurationUtil.Days(1)).Should().Be(test.Subtract(DurationUtil.Days(1)));

            //! 4월 30일로부터 5일전이면 4월25일이지만, 예외기간이 4월20일~4월25일이므로, 4월20일을 반환합니다.
            //
            dateAdd.Subtract(test, DurationUtil.Days(5)).Should().Be(timeRange1.Start);

            //! 4월 30일로부터 20일전이면, 4월10일 이지만 예외기간이 4월20일~4월25일이 있어 4월 5일이 되지만 4월 6일 이전은 예외기간이라 null을 반환합니다.
            //
            dateAdd.Subtract(test, DurationUtil.Days(20)).HasValue.Should().Be.False();

            dateAdd.Add(test, DurationUtil.Days(3)).Should().Be(test.Add(DurationUtil.Days(3)));
        }

        [Test]
        public void IncludeOutsideMaxTest() {
            var test = new DateTime(2011, 4, 12);

            var timeRange = new TimeRange(new DateTime(2011, 4, 20), DateTime.MaxValue);

            var dateAdd = new DateAdd();
            dateAdd.IncludePeriods.Add(timeRange);

            dateAdd.Add(test, DurationUtil.Zero).Should().Be(timeRange.Start);
            dateAdd.Add(test, DurationUtil.Days(1)).Should().Be(timeRange.Start.AddDays(1));

            dateAdd.Subtract(test, DurationUtil.Zero).Should().Not.Have.Value();
            dateAdd.Subtract(test, DurationUtil.Days(1)).Should().Not.Have.Value();
        }

        [Test]
        public void IncludeOutsideMinTest() {
            var test = new DateTime(2011, 4, 12);

            var timeRange = new TimeRange(DateTime.MinValue, new DateTime(2011, 4, 10));
            var dateAdd = new DateAdd();
            dateAdd.IncludePeriods.Add(timeRange);


            dateAdd.Add(test, DurationUtil.Zero).Should().Not.Have.Value();
            dateAdd.Add(test, DurationUtil.Days(1)).Should().Not.Have.Value();

            dateAdd.Subtract(test, DurationUtil.Zero).Should().Be(timeRange.End);
            dateAdd.Subtract(test, DurationUtil.Days(1)).Should().Be(timeRange.End.AddDays(-1));
        }

        [Test]
        public void AllExcludedTest() {
            var test = new DateTime(2011, 4, 12);

            var timeRange = new TimeRange(new DateTime(2011, 4, 10), new DateTime(2011, 4, 20));
            var dateAdd = new DateAdd();

            dateAdd.IncludePeriods.Add(timeRange);
            dateAdd.ExcludePeriods.Add(timeRange);

            dateAdd.Add(test, TimeSpan.Zero).Should().Not.Have.Value();
            dateAdd.Add(test, DurationUtil.Year(2)).Should().Not.Have.Value();
        }

        [Test]
        public void AllExcluded2Test() {
            var test = new DateTime(2011, 4, 12);

            var timeRange1 = new TimeRange(new DateTime(2011, 4, 10), new DateTime(2011, 4, 20));
            var timeRange2 = new TimeRange(new DateTime(2011, 4, 10), new DateTime(2011, 4, 15));
            var timeRange3 = new TimeRange(new DateTime(2011, 4, 15), new DateTime(2011, 4, 20));

            var dateAdd = new DateAdd();

            dateAdd.IncludePeriods.Add(timeRange1);
            dateAdd.ExcludePeriods.Add(timeRange2);
            dateAdd.ExcludePeriods.Add(timeRange3);

            dateAdd.Add(test, TimeSpan.Zero).Should().Not.Have.Value();
            dateAdd.Add(test, DurationUtil.Year(2)).Should().Not.Have.Value();
        }

        [Test]
        public void AllExcluded3Test() {
            var test = new DateTime(2011, 4, 12);

            var dateAdd = new DateAdd();
            dateAdd.IncludePeriods.Add(new TimeRange(new DateTime(2011, 4, 10), new DateTime(2011, 4, 20)));
            dateAdd.ExcludePeriods.Add(new TimeRange(new DateTime(2011, 4, 15), new DateTime(2011, 4, 20)));


            dateAdd.Add(test, TimeSpan.Zero).Should().Be(test);
            dateAdd.Add(test, DurationUtil.Days(1)).Should().Be(test.AddDays(1));
            dateAdd.Add(test, DurationUtil.Days(2)).Should().Be(test.AddDays(2));

            dateAdd.Add(test, DurationUtil.Days(3)).Should().Not.Have.Value();
        }

        [Test]
        public void PeriodMomentTest() {
            var test = new DateTime(2011, 4, 12);

            var timeRange = new TimeRange(test, test);

            var dateAdd = new DateAdd();

            dateAdd.Add(test, TimeSpan.Zero).Should().Be(test);

            dateAdd.IncludePeriods.Add(timeRange);
            dateAdd.Add(test, TimeSpan.Zero).Should().Be(test);

            dateAdd.ExcludePeriods.Add(timeRange);
            dateAdd.Add(test, TimeSpan.Zero).Should().Be(test);

            dateAdd.IncludePeriods.Clear();
            dateAdd.Add(test, TimeSpan.Zero).Should().Be(test);

            dateAdd.ExcludePeriods.Clear();
            dateAdd.Add(test, TimeSpan.Zero).Should().Be(test);
        }

        [Test]
        public void IncludeTest() {
            var test = new DateTime(2011, 4, 12);

            var timeRange = new TimeRange(new DateTime(2011, 4, 1), DateTime.MaxValue);

            var dateAdd = new DateAdd();
            dateAdd.IncludePeriods.Add(timeRange);

            dateAdd.Add(test, TimeSpan.Zero).Should().Be(test);
            dateAdd.Add(test, DurationUtil.Days(1)).Should().Be(test.AddDays(1));
            dateAdd.Add(test, DurationUtil.Days(365)).Should().Be(test.AddDays(365));
        }

        [Test]
        public void IncludeSplitTest() {
            DateTime test = new DateTime(2011, 4, 12);

            var timeRange1 = new TimeRange(new DateTime(2011, 4, 1), new DateTime(2011, 4, 15));
            var timeRange2 = new TimeRange(new DateTime(2011, 4, 20), new DateTime(2011, 4, 24));

            var dateAdd = new DateAdd();
            dateAdd.IncludePeriods.Add(timeRange1);
            dateAdd.IncludePeriods.Add(timeRange2);

            dateAdd.Add(test, TimeSpan.Zero).Should().Be(test);
            dateAdd.Add(test, DurationUtil.Days(1)).Should().Be(test.AddDays(1));
            dateAdd.Add(test, DurationUtil.Days(3)).Should().Be(timeRange2.Start);
            dateAdd.Add(test, DurationUtil.Days(5)).Should().Be(timeRange2.Start.AddDays(2));
            dateAdd.Add(test, DurationUtil.Days(6)).Should().Be(timeRange2.Start.AddDays(3));

            // NOTE: SeekBoundaryMode 에 따라 결과가 달라짐을 인식하세요!!!
            //
            dateAdd.Add(test, DurationUtil.Days(7), SeekBoundaryMode.Fill).Should().Be(timeRange2.End);
            dateAdd.Add(test, DurationUtil.Days(7), SeekBoundaryMode.Next).Should().Not.Have.Value();
        }

        [Test]
        public void ExcludeTest() {
            var test = new DateTime(2011, 4, 12);

            var timeRange = new TimeRange(new DateTime(2011, 4, 15), new DateTime(2011, 4, 20));

            var dateAdd = new DateAdd();
            dateAdd.ExcludePeriods.Add(timeRange);

            dateAdd.Add(test, TimeSpan.Zero).Should().Be(test);
            dateAdd.Add(test, DurationUtil.Days(1)).Should().Be(test.AddDays(1));
            dateAdd.Add(test, DurationUtil.Days(2)).Should().Be(test.AddDays(2));
            dateAdd.Add(test, DurationUtil.Days(3)).Should().Be(timeRange.End);
            dateAdd.Add(test, DurationUtil.Days(3, 0, 0, 0, 1)).Should().Be(timeRange.End.Add(DurationUtil.Millisecond));
            dateAdd.Add(test, DurationUtil.Days(5)).Should().Be(timeRange.End.AddDays(2));
        }

        [Test]
        public void ExcludeSplitTest() {
            var test = new DateTime(2011, 4, 12);

            var timeRange1 = new TimeRange(new DateTime(2011, 4, 15), new DateTime(2011, 4, 20));
            var timeRange2 = new TimeRange(new DateTime(2011, 4, 22), new DateTime(2011, 4, 25));

            var dateAdd = new DateAdd();
            dateAdd.ExcludePeriods.Add(timeRange1);
            dateAdd.ExcludePeriods.Add(timeRange2);

            dateAdd.Add(test, TimeSpan.Zero).Should().Be(test);
            dateAdd.Add(test, DurationUtil.Days(1)).Should().Be(test.AddDays(1));
            dateAdd.Add(test, DurationUtil.Days(2)).Should().Be(test.AddDays(2));
            dateAdd.Add(test, DurationUtil.Days(3)).Should().Be(timeRange1.End);
            dateAdd.Add(test, DurationUtil.Days(4)).Should().Be(timeRange1.End.AddDays(1));
            dateAdd.Add(test, DurationUtil.Days(5)).Should().Be(timeRange2.End);
            dateAdd.Add(test, DurationUtil.Days(6)).Should().Be(timeRange2.End.AddDays(1));
            dateAdd.Add(test, DurationUtil.Days(7)).Should().Be(timeRange2.End.AddDays(2));
        }

        [Test]
        public void IncludeEqualsExcludeTest() {
            var dateAdd = new DateAdd();

            dateAdd.IncludePeriods.Add(new TimeRange(new DateTime(2011, 3, 5), new DateTime(2011, 3, 10)));
            dateAdd.ExcludePeriods.Add(new TimeRange(new DateTime(2011, 3, 5), new DateTime(2011, 3, 10)));

            var test = new DateTime(2011, 3, 5);

            Assert.IsNull(dateAdd.Add(test, TimeSpan.Zero));
            Assert.IsNull(dateAdd.Add(test, new TimeSpan(1)));
            Assert.IsNull(dateAdd.Add(test, new TimeSpan(-1)));
            Assert.IsNull(dateAdd.Subtract(test, TimeSpan.Zero));
            Assert.IsNull(dateAdd.Subtract(test, new TimeSpan(1)));
            Assert.IsNull(dateAdd.Subtract(test, new TimeSpan(-1)));
        }

        [Test]
        public void IncludeExcludeTest() {
            var dateAdd = new DateAdd();

            dateAdd.IncludePeriods.Add(new TimeRange(new DateTime(2011, 3, 17), new DateTime(2011, 4, 20)));

            // setup some periods to exclude
            dateAdd.ExcludePeriods.Add(new TimeRange(new DateTime(2011, 3, 22), new DateTime(2011, 3, 25)));
            dateAdd.ExcludePeriods.Add(new TimeRange(new DateTime(2011, 4, 1), new DateTime(2011, 4, 7)));
            dateAdd.ExcludePeriods.Add(new TimeRange(new DateTime(2011, 4, 15), new DateTime(2011, 4, 16)));

            // positive
            var periodStart = new DateTime(2011, 3, 19);

            dateAdd.Add(periodStart, DurationUtil.Hours(1)).Should().Be(periodStart.AddHours(1));
            dateAdd.Add(periodStart, DurationUtil.Days(4)).Should().Be(new DateTime(2011, 3, 26));
            dateAdd.Add(periodStart, DurationUtil.Days(17)).Should().Be(new DateTime(2011, 4, 14));
            dateAdd.Add(periodStart, DurationUtil.Days(20)).Should().Be(new DateTime(2011, 4, 18));
            dateAdd.Add(periodStart, DurationUtil.Days(20)).Should().Be(new DateTime(2011, 4, 18));

            dateAdd.Add(periodStart, DurationUtil.Days(22), SeekBoundaryMode.Fill).Should().Be(new DateTime(2011, 4, 20));
            dateAdd.Add(periodStart, DurationUtil.Days(22), SeekBoundaryMode.Next).Should().Not.Have.Value();
            dateAdd.Add(periodStart, DurationUtil.Days(22)).Should().Not.Have.Value();


            // negative
            var periodEnd = new DateTime(2011, 4, 18);

            dateAdd.Add(periodEnd, DurationUtil.Hours(-1)).Should().Be(periodEnd.AddHours(-1));
            dateAdd.Add(periodEnd, DurationUtil.Days(-4)).Should().Be(new DateTime(2011, 4, 13));
            dateAdd.Add(periodEnd, DurationUtil.Days(-17)).Should().Be(new DateTime(2011, 3, 22));
            dateAdd.Add(periodEnd, DurationUtil.Days(-20)).Should().Be(new DateTime(2011, 3, 19));
            dateAdd.Add(periodEnd, DurationUtil.Days(-22), SeekBoundaryMode.Fill).Should().Be(new DateTime(2011, 3, 17));
            dateAdd.Add(periodEnd, DurationUtil.Days(-22), SeekBoundaryMode.Next).Should().Not.Have.Value();
            dateAdd.Add(periodEnd, DurationUtil.Days(-22)).Should().Not.Have.Value();
        }

        [Test]
        public void IncludeExclude2Test() {
            var dateAdd = new DateAdd();

            dateAdd.IncludePeriods.Add(new TimeRange(new DateTime(2011, 3, 1), new DateTime(2011, 3, 5)));
            dateAdd.ExcludePeriods.Add(new TimeRange(new DateTime(2011, 3, 5), new DateTime(2011, 3, 10)));
            dateAdd.IncludePeriods.Add(new TimeRange(new DateTime(2011, 3, 10), new DateTime(2011, 3, 15)));
            dateAdd.ExcludePeriods.Add(new TimeRange(new DateTime(2011, 3, 15), new DateTime(2011, 3, 20)));
            dateAdd.IncludePeriods.Add(new TimeRange(new DateTime(2011, 3, 20), new DateTime(2011, 3, 25)));

            var periodStart = new DateTime(2011, 3, 1);
            var periodEnd = new DateTime(2011, 3, 25);

            // add from start
            dateAdd.Add(periodStart, TimeSpan.Zero).Should().Be(periodStart);
            dateAdd.Add(periodStart, DurationUtil.Days(1)).Should().Be(new DateTime(2011, 3, 2));
            dateAdd.Add(periodStart, DurationUtil.Days(3)).Should().Be(new DateTime(2011, 3, 4));
            dateAdd.Add(periodStart, DurationUtil.Days(4)).Should().Be(new DateTime(2011, 3, 10));
            dateAdd.Add(periodStart, DurationUtil.Days(5)).Should().Be(new DateTime(2011, 3, 11));
            dateAdd.Add(periodStart, DurationUtil.Days(8)).Should().Be(new DateTime(2011, 3, 14));
            dateAdd.Add(periodStart, DurationUtil.Days(9)).Should().Be(new DateTime(2011, 3, 20));
            dateAdd.Add(periodStart, DurationUtil.Days(10)).Should().Be(new DateTime(2011, 3, 21));

            dateAdd.Add(periodStart, DurationUtil.Days(14), SeekBoundaryMode.Fill).Should().Be(new DateTime(2011, 3, 25));
            dateAdd.Add(periodStart, DurationUtil.Days(14), SeekBoundaryMode.Next).Should().Not.Have.Value();
            dateAdd.Add(periodStart, DurationUtil.Days(14)).Should().Not.Have.Value();


            // add from end
            dateAdd.Add(periodEnd, TimeSpan.Zero).Should().Be(periodEnd);
            dateAdd.Add(periodEnd, DurationUtil.Days(-1)).Should().Be(periodEnd.AddDays(-1));
            dateAdd.Add(periodEnd, DurationUtil.Days(-5)).Should().Be(new DateTime(2011, 3, 15));
            dateAdd.Add(periodEnd, DurationUtil.Days(-6)).Should().Be(new DateTime(2011, 3, 14));
            dateAdd.Add(periodEnd, DurationUtil.Days(-10)).Should().Be(new DateTime(2011, 3, 5));
            dateAdd.Add(periodEnd, DurationUtil.Days(-11)).Should().Be(new DateTime(2011, 3, 4));

            dateAdd.Add(periodEnd, DurationUtil.Days(-14), SeekBoundaryMode.Fill).Should().Be(new DateTime(2011, 3, 1));
            dateAdd.Add(periodEnd, DurationUtil.Days(-14), SeekBoundaryMode.Next).Should().Not.Have.Value();
            dateAdd.Add(periodEnd, DurationUtil.Days(-14)).Should().Not.Have.Value();

            // subtract form end

            dateAdd.Subtract(periodEnd, TimeSpan.Zero).Should().Be(periodEnd);
            dateAdd.Subtract(periodEnd, DurationUtil.Days(1)).Should().Be(periodEnd.AddDays(-1));
            dateAdd.Subtract(periodEnd, DurationUtil.Days(5)).Should().Be(new DateTime(2011, 3, 15));
            dateAdd.Subtract(periodEnd, DurationUtil.Days(6)).Should().Be(new DateTime(2011, 3, 14));
            dateAdd.Subtract(periodEnd, DurationUtil.Days(10)).Should().Be(new DateTime(2011, 3, 5));
            dateAdd.Subtract(periodEnd, DurationUtil.Days(11)).Should().Be(new DateTime(2011, 3, 4));

            dateAdd.Subtract(periodEnd, DurationUtil.Days(14), SeekBoundaryMode.Fill).Should().Be(new DateTime(2011, 3, 1));
            dateAdd.Subtract(periodEnd, DurationUtil.Days(14), SeekBoundaryMode.Next).Should().Not.Have.Value();
            dateAdd.Subtract(periodEnd, DurationUtil.Days(14)).Should().Not.Have.Value();


            // subtract form start
            dateAdd.Subtract(periodStart, TimeSpan.Zero).Should().Be(periodStart);
            dateAdd.Subtract(periodStart, DurationUtil.Days(-1)).Should().Be(new DateTime(2011, 3, 2));
            dateAdd.Subtract(periodStart, DurationUtil.Days(-3)).Should().Be(new DateTime(2011, 3, 4));
            dateAdd.Subtract(periodStart, DurationUtil.Days(-4)).Should().Be(new DateTime(2011, 3, 10));
            dateAdd.Subtract(periodStart, DurationUtil.Days(-5)).Should().Be(new DateTime(2011, 3, 11));
            dateAdd.Subtract(periodStart, DurationUtil.Days(-8)).Should().Be(new DateTime(2011, 3, 14));
            dateAdd.Subtract(periodStart, DurationUtil.Days(-9)).Should().Be(new DateTime(2011, 3, 20));
            dateAdd.Subtract(periodStart, DurationUtil.Days(-10)).Should().Be(new DateTime(2011, 3, 21));
            dateAdd.Subtract(periodStart, DurationUtil.Days(-14), SeekBoundaryMode.Fill).Should().Be(new DateTime(2011, 3, 25));
            dateAdd.Subtract(periodStart, DurationUtil.Days(-14), SeekBoundaryMode.Next).Should().Not.Have.Value();
            dateAdd.Subtract(periodStart, DurationUtil.Days(-14)).Should().Not.Have.Value();
        }

        [Test]
        public void IncludeExclude3Test() {
            var dateAdd = new DateAdd();
            dateAdd.ExcludePeriods.Add(new TimeRange(new DateTime(2011, 3, 5), new DateTime(2011, 3, 10)));
            dateAdd.IncludePeriods.Add(new TimeRange(new DateTime(2011, 3, 10), new DateTime(2011, 3, 15)));
            dateAdd.ExcludePeriods.Add(new TimeRange(new DateTime(2011, 3, 15), new DateTime(2011, 3, 20)));

            var test = new DateTime(2011, 3, 10);

            dateAdd.Subtract(test, TimeSpan.Zero).Should().Be(test);
            dateAdd.Add(test, DurationUtil.Days(1)).Should().Be(new DateTime(2011, 3, 11));
            dateAdd.Add(test, DurationUtil.Days(5), SeekBoundaryMode.Fill).Should().Be(test.AddDays(5));
            dateAdd.Add(test, DurationUtil.Days(5)).Should().Not.Have.Value();
        }

        [Test]
        public void IncludeExclude4Test() {
            var dateAdd = new DateAdd();
            dateAdd.IncludePeriods.Add(new TimeRange(new DateTime(2011, 3, 10), new DateTime(2011, 3, 20)));
            dateAdd.ExcludePeriods.Add(new TimeRange(new DateTime(2011, 3, 10), new DateTime(2011, 3, 15)));
            dateAdd.ExcludePeriods.Add(new TimeRange(new DateTime(2011, 3, 15), new DateTime(2011, 3, 20)));

            var test = new DateTime(2011, 3, 10);
            Assert.IsNull(dateAdd.Add(test, TimeSpan.Zero));
            Assert.IsNull(dateAdd.Add(test, DurationUtil.Days(1)));
            Assert.IsNull(dateAdd.Add(test, DurationUtil.Days(5)));
        }

        [Test]
        public void IncludeExclude5Test() {
            var dateAdd = new DateAdd();
            dateAdd.IncludePeriods.Add(new TimeRange(new DateTime(2011, 3, 10), new DateTime(2011, 3, 20)));
            dateAdd.ExcludePeriods.Add(new TimeRange(new DateTime(2011, 3, 5), new DateTime(2011, 3, 15)));
            dateAdd.ExcludePeriods.Add(new TimeRange(new DateTime(2011, 3, 15), new DateTime(2011, 3, 30)));

            var test = new DateTime(2011, 3, 10);
            Assert.IsNull(dateAdd.Add(test, TimeSpan.Zero));
            Assert.IsNull(dateAdd.Add(test, new TimeSpan(1)));
            Assert.IsNull(dateAdd.Add(test, new TimeSpan(-1)));
            Assert.IsNull(dateAdd.Subtract(test, TimeSpan.Zero));
            Assert.IsNull(dateAdd.Subtract(test, new TimeSpan(1)));
            Assert.IsNull(dateAdd.Subtract(test, new TimeSpan(-1)));
        }

        [Test]
        public void IncludeExclude6Test() {
            var dateAdd = new DateAdd();
            dateAdd.IncludePeriods.Add(new TimeRange(new DateTime(2011, 3, 10), new DateTime(2011, 3, 20)));
            dateAdd.ExcludePeriods.Add(new TimeRange(new DateTime(2011, 3, 5), new DateTime(2011, 3, 12)));
            dateAdd.ExcludePeriods.Add(new TimeRange(new DateTime(2011, 3, 18), new DateTime(2011, 3, 30)));

            var test = new DateTime(2011, 3, 10);

            dateAdd.Add(test, TimeSpan.Zero).Should().Be(new DateTime(2011, 3, 12));
            dateAdd.Add(test, DurationUtil.Days(1)).Should().Be(new DateTime(2011, 3, 13));
        }
    }
}