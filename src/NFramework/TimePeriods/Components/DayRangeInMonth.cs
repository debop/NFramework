using System;

namespace NSoft.NFramework.TimePeriods {
    /// <summary>
    /// 같은 Month 내에서의 Day 만의 기간을 나타냅니다. (예: 5일에서 21일까지)
    /// </summary>
    [Serializable]
    public struct DayRangeInMonth : IEquatable<DayRangeInMonth>, IComparable<DayRangeInMonth>, IComparable {
        public DayRangeInMonth(int startDay, int endDay)
            : this() {
            AssertValidDayRange(startDay);
            AssertValidDayRange(endDay);

            if(startDay <= endDay) {
                Min = startDay;
                Max = endDay;
            }
            else {
                Min = endDay;
                Max = startDay;
            }
        }

        private static void AssertValidDayRange(int day) {
            Guard.Assert(day > 0 && day <= TimeSpec.MaxDaysPerMonth,
                         "일(Day)를 표현하는 숫자는 1~31 사이여야 합니다. day=[{0}]", day);
        }

        /// <summary>
        /// 시작 <see cref="TimeValue"/>
        /// </summary>
        public int Min { get; private set; }

        /// <summary>
        /// 완료 <see cref="TimeValue"/>
        /// </summary>
        public int Max { get; private set; }

        public bool IsSingleDay {
            get { return Min == Max; }
        }

        public bool HasInside(int day) {
            return Min <= day && day <= Max;
        }

        public int CompareTo(object obj) {
            obj.ShouldBeInstanceOf<DayRangeInMonth>("obj");
            //obj.ShouldNotBeNull("obj");
            //Guard.Assert(obj is DayRangeInMonth, "대상 인스턴스의 수형이 DayRangeInMonth가 아닙니다.");

            return CompareTo((DayRangeInMonth)obj);
        }

        public int CompareTo(DayRangeInMonth other) {
            return Min.CompareTo(other.Min);
        }

        public bool Equals(DayRangeInMonth other) {
            return GetHashCode().Equals(other.GetHashCode());
        }

        public override bool Equals(object obj) {
            return (obj != null) && (obj is DayRangeInMonth) && Equals((DayRangeInMonth)obj);
        }

        public override int GetHashCode() {
            return HashTool.Compute(Max, Min);
        }

        public override string ToString() {
            return string.Format("DayRangeInMonth# Min=[{0}], Max=[{1}]", Min, Max);
        }
    }
}