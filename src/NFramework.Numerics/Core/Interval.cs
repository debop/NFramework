using System;
using System.Runtime.Serialization;
using NSoft.NFramework.LinqEx;

namespace NSoft.NFramework.Numerics {
    /// <summary>
    ///상하한 구간을 표현한다.
    /// </summary>
    /// <remarks>
    /// 0 ~ 1 사이 구간 중에 상하한을 포함하는 경우를 Closed 라 표현하고 [0,1] 이라 하고
    /// 상하한을 포함하지 않으면 Opened라 표현하고 (0,1) 이라 쓴다.  상/하한 별개로 Open/Close를 혼용할 수도 있다.
    /// </remarks>
    [Serializable]
    public class Interval<T> : ISerializable, ICloneable, IEquatable<Interval<T>> where T : IComparable<T> {
        /// <summary>
        /// 하한
        /// </summary>
        public const string MinName = "Min";

        /// <summary>
        /// 상한
        /// </summary>
        public const string MaxName = "Max";

        /// <summary>
        /// 종류명
        /// </summary>
        public const string KindName = "Kind";

        /// <summary>
        /// 개방 구간을 나타내는 Character
        /// </summary>
        public static readonly char[] OpenedChars = new[] { '(', ')' };

        /// <summary>
        /// 폐쇄 구간을 나타내는 Character
        /// </summary>
        public static readonly char[] ClosedChars = new[] { '[', ']' };

        /// <summary>
        /// <see cref="Interval{TOther}"/>를 생성합니다.
        /// </summary>
        /// <typeparam name="TOther"></typeparam>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="kind"></param>
        /// <returns></returns>
        public static Interval<TOther> Create<TOther>(TOther min, TOther max, IntervalKind kind = IntervalKind.ClosedOpen)
            where TOther : struct, IComparable<TOther> {
            return new Interval<TOther>(min, max, kind);
        }

        /// <summary>
        /// 생성자
        /// </summary>
        public Interval() {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="type"></param>
        public Interval(T min, T max, IntervalKind type = IntervalKind.ClosedOpen) {
            if(min.CompareTo(max) < 0) {
                Min = min;
                Max = max;
            }
            else {
                Min = max;
                Max = min;
            }

            Kind = type;
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="source"></param>
        public Interval(Interval<T> source) {
            source.ShouldNotBeNull("source");

            Min = source.Min;
            Max = source.Max;
            Kind = source.Kind;
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected Interval(SerializationInfo info, StreamingContext context) {
            Min = (T)info.GetValue(MinName, typeof(T));
            Max = (T)info.GetValue(MaxName, typeof(T));
            Kind = (IntervalKind)info.GetValue(KindName, typeof(IntervalKind));
        }

        /// <summary>
        /// 하한
        /// </summary>
        public T Min { get; set; }

        /// <summary>
        /// 상한
        /// </summary>
        public T Max { get; set; }

        /// <summary>
        /// 구간 경계 종류
        /// </summary>
        public virtual IntervalKind Kind { get; set; }

        /// <summary>
        /// 간격안에 존재하는가?
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public bool IsIn(T x) {
            switch(Kind) {
                case IntervalKind.Open:
                    return !(x.CompareTo(Min) <= 0) && (x.CompareTo(Max) < 0);
                case IntervalKind.Closed:
                    return !(x.CompareTo(Min) < 0) && (x.CompareTo(Max) <= 0);
                case IntervalKind.OpenClosed:
                    return !(x.CompareTo(Min) <= 0) && (x.CompareTo(Max) <= 0);
                case IntervalKind.ClosedOpen:
                    return !(x.CompareTo(Min) < 0) && (x.CompareTo(Max) < 0);
                default:
                    return false;
            }
        }

        /// <summary>
        /// 간격 길이
        /// </summary>
        /// <returns></returns>
        public T GetLength() {
            return LinqTool.Operators<T>.Subtract(Max, Min);
        }

        private static char GetRangeChar(IntervalKind type, bool min) {
            switch(type) {
                case IntervalKind.Open:
                    return (min) ? OpenedChars[0] : OpenedChars[1];
                case IntervalKind.Closed:
                    return (min) ? ClosedChars[0] : ClosedChars[1];
                case IntervalKind.ClosedOpen:
                    return (min) ? ClosedChars[0] : OpenedChars[1];
                case IntervalKind.OpenClosed:
                    return (min) ? OpenedChars[0] : ClosedChars[1];
                default:
                    throw new ArgumentException("Unknown IntervalKind");
            }
        }

        /// <summary>
        /// 현재 개체가 동일한 형식의 다른 개체와 같은지 여부를 나타냅니다.
        /// </summary>
        /// <returns>
        ///             현재 개체가 <paramref name="other" /> 매개 변수와 같으면 true이고, 그렇지 않으면 false입니다.
        /// </returns>
        /// <param name="other">이 개체와 비교할 개체입니다.</param>
        public bool Equals(Interval<T> other) {
            return (other != null) && Equals(Min, other.Min) && Equals(Max, other.Max) && (Kind == other.Kind);
        }

        /// <summary>
        /// 현재 인스턴스와 같은지 검사합니다.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) {
            return (obj != null) && (Equals(obj as Interval<T>));
        }

        /// <summary>
        /// HashCode를 반환합니다.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() {
            return HashTool.Compute(Min, Max, Kind);
        }

        /// <summary>
        /// 현재 인스턴스를 문자열로 표현합니다.
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            char r0 = GetRangeChar(Kind, true);
            char r1 = GetRangeChar(Kind, false);

            return string.Format("{0}{1}, {2}{3}", r0, Min, Max, r1);
        }

        /// <summary>
        /// 대상 개체를 serialize하는 데 필요한 데이터로 <see cref="T:System.Runtime.Serialization.SerializationInfo" />를 채웁니다.
        /// </summary>
        /// <param name="info">데이터로 채울 <see cref="T:System.Runtime.Serialization.SerializationInfo" />입니다. </param>
        /// <param name="context">이 serialization에 대한 대상입니다(<see cref="T:System.Runtime.Serialization.StreamingContext" /> 참조). </param>
        /// <exception cref="T:System.Security.SecurityException">호출자에게 필요한 권한이 없는 경우 </exception>
        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue(MinName, Min);
            info.AddValue(MaxName, Max);
            info.AddValue(KindName, Kind);
        }

        /// <summary>
        /// 현재 인스턴스의 복사본인 새 개체를 만듭니다.
        /// </summary>
        /// <returns>
        /// 이 인스턴스의 복사본인 새 개체입니다.
        /// </returns>
        public virtual Interval<T> Clone() {
            return new Interval<T>(this);
        }

        object ICloneable.Clone() {
            return Clone();
        }
    }
}