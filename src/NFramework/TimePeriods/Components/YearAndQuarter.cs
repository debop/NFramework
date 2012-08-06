using System;

namespace NSoft.NFramework.TimePeriods {
    /// <summary>
    /// 년도와 분기를 나타내는 구조체입니다.
    /// </summary>
    [Serializable]
    public struct YearAndQuarter : IEquatable<YearAndQuarter>, IComparable<YearAndQuarter>, IComparable {
        public YearAndQuarter(int? year, int? quarter) : this() {
            Year = year;

            if(quarter.HasValue)
                Quarter = quarter.Value.AsEnum<QuarterKind>(QuarterKind.First);
        }

        public YearAndQuarter(int? year, QuarterKind? quarter)
            : this() {
            Year = year;
            Quarter = quarter;
        }

        /// <summary>
        /// 년도
        /// </summary>
        public int? Year { get; set; }

        /// <summary>
        /// 분기
        /// </summary>
        public QuarterKind? Quarter { get; set; }

        public int CompareTo(YearAndQuarter other) {
            return GetHashCode().CompareTo(other.GetHashCode());
        }

        public int CompareTo(object obj) {
            obj.ShouldNotBeNull("obj");
            Guard.Assert(obj is YearAndQuarter, "대상 인스턴스의 수형이 YearAndQuarter가 아닙니다.");

            return CompareTo((YearAndQuarter)obj);
        }

        public bool Equals(YearAndQuarter other) {
            return GetHashCode().Equals(other.GetHashCode());
        }

        public override bool Equals(object obj) {
            return (obj != null) && (obj is YearAndQuarter) && Equals((YearAndQuarter)obj);
        }

        public override int GetHashCode() {
            return (Year ?? 0) * 100 + (Quarter.HasValue ? (Quarter.Value.GetHashCode() - 1) * 25 : 0);
        }

        public override string ToString() {
            return string.Format("YearAndQuarter# Year=[{0}], Quarter=[{1}]", Year, Quarter);
        }
    }
}