using System;

namespace NSoft.NFramework.TimePeriods {
    /// <summary>
    /// 같은 년도 내에서의 최대 최소 기간
    /// </summary>
    [Serializable]
    public struct MonthRangeInYear : IEquatable<MonthRangeInYear>, IComparable<MonthRangeInYear>, IComparable {
        public MonthRangeInYear(int minMonth, int maxMonth) : this() {
            minMonth.ShouldBeBetween(1, TimeSpec.MonthsPerYear, "minMonth");
            maxMonth.ShouldBeBetween(1, TimeSpec.MonthsPerYear, "maxMonth");
            Guard.Assert(minMonth <= maxMonth, "minMonth <= maxMonth 여야 합니다. min=[{0}], max=[{1}]", minMonth, maxMonth);

            Min = minMonth;
            Max = maxMonth;
        }

        /// <summary>
        /// 하한 값
        /// </summary>
        public int Min { get; private set; }

        /// <summary>
        /// 상한 값
        /// </summary>
        public int Max { get; private set; }

        public bool IsSingleMonth {
            get { return Min == Max; }
        }

        public bool HasInside(int month) {
            return Min <= month && month <= Max;
        }

        public int CompareTo(MonthRangeInYear other) {
            return GetHashCode().CompareTo(other.GetHashCode());
        }

        public int CompareTo(object obj) {
            obj.ShouldNotBeNull("obj");
            Guard.Assert(obj is MonthRangeInYear, "대상 인스턴스의 수형이 MonthRangeInYear가 아닙니다.");

            return CompareTo((MonthRangeInYear)obj);
        }

        public bool Equals(MonthRangeInYear other) {
            return GetHashCode().Equals(other.GetHashCode());
        }

        public override bool Equals(object obj) {
            return (obj != null) && (obj is MonthRangeInYear) && Equals((MonthRangeInYear)obj);
        }

        public override int GetHashCode() {
            return Max * 100 + Min;
        }

        public override string ToString() {
            return string.Format("MonthRangeInYear# Min=[{0}], Max=[{1}]", Min, Max);
        }
    }
}