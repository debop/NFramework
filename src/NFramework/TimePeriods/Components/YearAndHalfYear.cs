using System;

namespace NSoft.NFramework.TimePeriods {
    /// <summary>
    /// 년도와 반기를 표현합니다.
    /// </summary>
    [Serializable]
    public struct YearAndHalfyear : IEquatable<YearAndHalfyear>, IComparable<YearAndHalfyear>, IComparable {
        public YearAndHalfyear(int? year, HalfyearKind? halfyear)
            : this() {
            Year = year;
            Halfyear = halfyear;
        }

        public YearAndHalfyear(int? year, int? halfyear)
            : this() {
            Year = year;
            if(halfyear.HasValue)
                Halfyear = halfyear.AsEnum<HalfyearKind>(HalfyearKind.First);
        }

        /// <summary>
        /// 년도
        /// </summary>
        public int? Year { get; set; }

        /// <summary>
        /// 반기 (전반기|후반기)
        /// </summary>
        public HalfyearKind? Halfyear { get; set; }

        public int CompareTo(YearAndHalfyear other) {
            return GetHashCode().CompareTo(other.GetHashCode());
        }

        public int CompareTo(object obj) {
            obj.ShouldNotBeNull("obj");
            Guard.Assert(obj is YearAndHalfyear, "대상 인스턴스의 수형이 YearAndHalfyear가 아닙니다.");

            return CompareTo((YearAndHalfyear)obj);
        }

        public bool Equals(YearAndHalfyear other) {
            return GetHashCode().Equals(other.GetHashCode());
        }

        public override bool Equals(object obj) {
            return (obj is YearAndHalfyear) && Equals((YearAndHalfyear)obj);
        }

        public override int GetHashCode() {
            return (Year ?? 0) * 100 + (Halfyear.HasValue ? Halfyear.Value.GetHashCode() : 0);
        }

        public override string ToString() {
            return string.Format("YearAndHalfyear# Year=[{0}], Halfyear=[{1}]", Year, Halfyear);
        }
    }
}