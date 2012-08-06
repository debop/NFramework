namespace NSoft.NFramework {
    /// <summary>
    /// 주차를 표현합니다.
    /// </summary>
    public struct YearWeek : IYearWeek {
        public YearWeek(int? year, int? week)
            : this() {
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

        public static implicit operator int(YearWeek yw) {
            return yw.GetHashCode();
        }

        public static implicit operator YearWeek(int value) {
            var year = value / 100;
            var week = value % 100;

            return new YearWeek(year, week);
        }

        //public static bool operator ==(YearWeek a, YearWeek b)
        //{
        //    return Equals(a, b);
        //}
        //public static bool operator !=(YearWeek a, YearWeek b)
        //{
        //    return !Equals(a, b);
        //}
        //public static bool operator >(YearWeek a, YearWeek b)
        //{
        //    return a.CompareTo(b) > 0;
        //}
        //public static bool operator <(YearWeek a, YearWeek b)
        //{
        //    return a.CompareTo(b) < 0;
        //}
        //public static bool operator >=(YearWeek a, YearWeek b)
        //{
        //    return a.CompareTo(b) >= 0;
        //}
        //public static bool operator <=(YearWeek a, YearWeek b)
        //{
        //    return a.CompareTo(b) <= 0;
        //}

        public int CompareTo(IYearWeek other) {
            return GetHashCode().CompareTo(other.GetHashCode());
        }

        public int CompareTo(object obj) {
            obj.ShouldNotBeNull("obj");
            Guard.Assert(() => obj is YearWeek);
            return CompareTo((YearWeek)obj);
        }

        public bool Equals(IYearWeek other) {
            return GetHashCode().Equals(other.GetHashCode());
        }

        public override bool Equals(object obj) {
            return (obj != null) && (obj is YearWeek) && Equals((YearWeek)obj);
        }

        public override int GetHashCode() {
            unchecked {
                return (Year ?? 0) * 100 + (Week ?? 0);
            }
        }

        public override string ToString() {
            return string.Format("YearWeek# Year=[{0}], Week=[{1}]", Year, Week);
        }
    }
}