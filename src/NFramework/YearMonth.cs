using System;

namespace NSoft.NFramework {
    /// <summary>
    /// 년/월을 표현합니다.
    /// </summary>
    public struct YearMonth : IEquatable<YearMonth>, IComparable<YearMonth>, IComparable {
        public YearMonth(int? year, int? month) : this() {
            Year = year;
            Month = month;
        }

        /// <summary>
        /// 년
        /// </summary>
        public int? Year { get; set; }

        /// <summary>
        /// 월 ( 1 ~ 12 의 값을 가진다 )
        /// </summary>
        public int? Month { get; set; }

        public int CompareTo(YearMonth other) {
            return GetHashCode().CompareTo(other.GetHashCode());
        }

        public int CompareTo(object obj) {
            obj.ShouldNotBeNull("obj");
            Guard.Assert(() => obj is YearMonth);

            return CompareTo((YearMonth)obj);
        }

        public bool Equals(YearMonth other) {
            return GetHashCode().Equals(other.GetHashCode());
        }

        public override bool Equals(object obj) {
            return (obj != null) && (obj is YearMonth) && Equals((YearMonth)obj);
        }

        public override int GetHashCode() {
            unchecked {
                return (Year ?? 0) * 100 + Month ?? 0;
            }
        }

        public override string ToString() {
            return string.Format("YearMonth# Year=[{0}], Month=[{1}]", Year, Month);
        }
    }
}