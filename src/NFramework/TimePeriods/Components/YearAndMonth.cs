using System;

namespace NSoft.NFramework.TimePeriods {
    /// <summary>
    /// 년도와 월을 나타냅니다.
    /// </summary>
    [Serializable]
    public struct YearAndMonth : IEquatable<YearAndMonth>, IComparable<YearAndMonth>, IComparable {
        public YearAndMonth(int? year, int? month) : this() {
            Year = year;
            Month = month;
        }

        /// <summary>
        /// 년도
        /// </summary>
        public int? Year { get; set; }

        /// <summary>
        /// 월
        /// </summary>
        public int? Month { get; set; }

        public int CompareTo(YearAndMonth other) {
            return GetHashCode().CompareTo(other.GetHashCode());
        }

        public int CompareTo(object obj) {
            obj.ShouldNotBeNull("obj");
            Guard.Assert(obj is YearAndMonth, "대상 인스턴스의 수형이 YearAndMonth가 아닙니다.");

            return CompareTo((YearAndMonth)obj);
        }

        public bool Equals(YearAndMonth other) {
            return GetHashCode().Equals(other.GetHashCode());
        }

        public override bool Equals(object obj) {
            return (obj != null) && (obj is YearAndMonth) && Equals((YearAndMonth)obj);
        }

        public override int GetHashCode() {
            return (Year ?? 0) * 100 + (Month ?? 0);
        }

        public override string ToString() {
            return string.Format("YearAndMonth# Year=[{0}], Month=[{1}]", Year, Month);
        }
    }
}