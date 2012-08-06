using System;

namespace NSoft.NFramework.TimePeriods {
    /// <summary>
    /// 년도와 주차를 나타냅니다.
    /// </summary>
    [Serializable]
    public struct YearAndWeek : IEquatable<YearAndWeek>, IComparable<YearAndWeek>, IComparable {
        public YearAndWeek(int? year, int? week) : this() {
            Year = year;
            Week = week;
        }

        /// <summary>
        /// 년도
        /// </summary>
        public int? Year { get; set; }

        /// <summary>
        /// 주차
        /// </summary>
        public int? Week { get; set; }

        public static bool operator ==(YearAndWeek lhs, YearAndWeek rhs) {
            return Equals(lhs, rhs);
        }

        public static bool operator !=(YearAndWeek lhs, YearAndWeek rhs) {
            return Equals(lhs, rhs);
        }

        public static bool operator >(YearAndWeek lhs, YearAndWeek rhs) {
            return lhs.CompareTo(rhs) > 0;
        }

        public static bool operator <(YearAndWeek lhs, YearAndWeek rhs) {
            return lhs.CompareTo(rhs) < 0;
        }

        public static bool operator >=(YearAndWeek lhs, YearAndWeek rhs) {
            return lhs.CompareTo(rhs) >= 0;
        }

        public static bool operator <=(YearAndWeek lhs, YearAndWeek rhs) {
            return lhs.CompareTo(rhs) <= 0;
        }

        public int CompareTo(YearAndWeek other) {
            return GetHashCode().CompareTo(other.GetHashCode());
        }

        public int CompareTo(object obj) {
            obj.ShouldNotBeNull("obj");
            Guard.Assert(obj is YearAndWeek, "형식이 맞지 않습니다.");

            return CompareTo((YearAndWeek)obj);
        }

        public bool Equals(YearAndWeek other) {
            return GetHashCode().Equals(other.GetHashCode());
        }

        public override bool Equals(object obj) {
            return (obj != null) && (obj is YearAndWeek) && Equals((YearAndWeek)obj);
        }

        public override int GetHashCode() {
            return (Year ?? 0) * 100 + (Week ?? 0);
        }

        public override string ToString() {
            return string.Format("YearAndWeek# Year=[{0}], Week=[{1}]", Year, Week);
        }
    }
}