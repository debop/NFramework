using System;
using System.Diagnostics;

namespace NSoft.NFramework.TimePeriods {
    /// <summary>
    /// 하루 24시간 내의 시간단위의 기간을 나타냅니다. ( 예: 09시~16시 )
    /// </summary>
    [DebuggerDisplay("Start={Start}, End={End}")]
    [Serializable]
    public class HourRangeInDay : ValueObjectBase, IEquatable<HourRangeInDay>, IComparable<HourRangeInDay>, IComparable {
        public HourRangeInDay(int hour) : this(hour, hour) {}

        public HourRangeInDay(int startHour, int endHour) : this(new TimeValue(startHour), new TimeValue(endHour)) {}

        public HourRangeInDay(TimeValue start, TimeValue end) {
            if(start.Ticks <= end.Ticks) {
                Start = start;
                End = end;
            }
            else {
                Start = end;
                End = start;
            }
        }

        /// <summary>
        /// 시작 <see cref="TimeValue"/>
        /// </summary>
        public TimeValue Start { get; private set; }

        /// <summary>
        /// 완료 <see cref="TimeValue"/>
        /// </summary>
        public TimeValue End { get; private set; }

        /// <summary>
        /// 시작 시각과 완료 시각이 같은 값이면, 순간이라 표현한다.
        /// </summary>
        public bool IsMoment {
            get { return Equals(Start, End); }
        }

        public bool HasInside(int hour) {
            return HasInside(new TimeValue(hour));
        }

        public bool HasInside(TimeValue target) {
            return target.CompareTo(Start) >= 0 && target.CompareTo(End) <= 0;
        }

        public int CompareTo(object obj) {
            obj.ShouldNotBeNull("obj");
            Guard.Assert(obj is HourRangeInDay, "대상 인스턴스의 수형이 HourRangeInDay가 아닙니다.");

            return CompareTo((HourRangeInDay)obj);
        }

        public int CompareTo(HourRangeInDay other) {
            return Start.CompareTo(other.Start);
        }

        public bool Equals(HourRangeInDay other) {
            return (other != null) && GetHashCode().Equals(other.GetHashCode());
        }

        public override int GetHashCode() {
            return HashTool.Compute(Start, End);
        }

        public override string ToString() {
            return string.Concat("HourRangeInDay# ", Start, TimeFormatter.DefaultStartEndSeparator, End);
        }
    }
}