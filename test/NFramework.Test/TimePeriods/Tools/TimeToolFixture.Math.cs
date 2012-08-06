using System;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.TimePeriods.Tools {
    [Microsoft.Silverlight.Testing.Tag("TimePeriods")]
    [TestFixture]
    public class TimeToolFixture_Math : TimePeriodFixtureBase {
        private readonly DateTime min = new DateTime(2000, 10, 2, 13, 45, 53, 673);
        private readonly DateTime max = new DateTime(2002, 9, 3, 7, 14, 22, 234);

        [Test]
        public void MinTest() {
            TimeTool.Min(min, max).Should().Be(min);
            TimeTool.Min(min, min).Should().Be(min);
            TimeTool.Min(max, max).Should().Be(max);
        }

        [Test]
        public void MaxTest() {
            TimeTool.Max(min, max).Should().Be(max);
            TimeTool.Max(min, min).Should().Be(min);
            TimeTool.Max(max, max).Should().Be(max);
        }

        [Test]
        public void MinNullableTest() {
            TimeTool.Min(min, max).Should().Be(min);
            TimeTool.Min(min, null).Should().Be(min);
            TimeTool.Min(null, min).Should().Be(min);
            TimeTool.Min(null, null).Should().Not.Have.Value();
        }

        [Test]
        public void MaxNullableTest() {
            TimeTool.Max(min, max).Should().Be(max);
            TimeTool.Max(max, null).Should().Be(max);
            TimeTool.Max(null, max).Should().Be(max);
            TimeTool.Max(null, null).Should().Not.Have.Value();
        }

        [Test]
        public void AdjuestPeriodWithStartAndEnd() {
            var start = min;
            var end = max;

            TimeTool.AdjustPeriod(ref start, ref end);
            end.Should().Be.GreaterThan(start);

            TimeTool.AdjustPeriod(ref end, ref start);
            start.Should().Be.GreaterThan(end);
        }

        [Test]
        public void AdjuestPeriodWithStartAndEndNullable() {
            DateTime? start = min;
            DateTime? end = max;

            TimeTool.AdjustPeriod(ref start, ref end);
            end.Should().Be.GreaterThan(start);

            TimeTool.AdjustPeriod(ref end, ref start);
            start.Should().Be.GreaterThan(end);


            start = null;
            end = max;

            TimeTool.AdjustPeriod(ref start, ref end);
            Assert.IsNull(start);
            end.Should().Be(max);

            start = min;
            end = null;

            TimeTool.AdjustPeriod(ref start, ref end);
            Assert.IsNull(end);
            start.Should().Be(min);

            start = null;
            end = null;

            TimeTool.AdjustPeriod(ref start, ref end);
            Assert.IsNull(start);
            Assert.IsNull(end);
        }

        [Test]
        public void AdjustPeriodWithDuration() {
            var start = min;
            var duration = DurationUtil.Day;

            TimeTool.AdjustPeriod(ref start, ref duration);
            Assert.AreEqual(min, start);
            Assert.AreEqual(DurationUtil.Day, duration);

            duration = DurationUtil.Day.Negate();

            TimeTool.AdjustPeriod(ref start, ref duration);
            Assert.AreEqual(min.Add(DurationUtil.Day.Negate()), start);
            Assert.AreEqual(DurationUtil.Day, duration);
        }
    }
}