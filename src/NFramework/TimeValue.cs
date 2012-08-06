using System;

namespace NSoft.NFramework {
    /// <summary>
    /// <see cref="DateTime"/>의 시간 부분을 제외한 날짜 부분만을 표현합니다.
    /// </summary>
    [Serializable]
    public struct TimeValue : IEquatable<TimeValue>, IComparable<TimeValue>, IComparable {
        /// <summary>
        /// 현재 시각을 나타내는 <see cref="TimeValue"/>를 반환합니다.
        /// </summary>
        public static TimeValue Now {
            get { return new TimeValue(DateTime.Now); }
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="time"></param>
        public TimeValue(DateTime time)
            : this() {
            Duration = time.TimeOfDay;
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="duration"></param>
        public TimeValue(TimeSpan duration)
            : this() {
            Duration = duration;
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="hours"></param>
        /// <param name="minutes"></param>
        /// <param name="seconds"></param>
        /// <param name="milliseconds"></param>
        public TimeValue(int hours, int minutes = 0, int seconds = 0, int milliseconds = 0)
            : this() {
            Duration = new TimeSpan(0, hours, minutes, seconds, milliseconds);
        }

        /// <summary>
        /// 하루중의 시간을 나타냅니다. <see cref="DateTime.TimeOfDay"/> 와 같다.
        /// </summary>
        public TimeSpan Duration { get; private set; }

        /// <summary>
        /// Duration 중의 시간
        /// </summary>
        public int Hour {
            get { return Duration.Hours; }
        }

        /// <summary>
        /// Duration 중의 분
        /// </summary>
        public int Minute {
            get { return Duration.Minutes; }
        }

        /// <summary>
        /// Duration 중의 초
        /// </summary>
        public int Second {
            get { return Duration.Seconds; }
        }

        /// <summary>
        /// Duration 중의 밀리초
        /// </summary>
        public int Millisecond {
            get { return Duration.Milliseconds; }
        }

        /// <summary>
        /// Duration을 시간 단위로 표현
        /// </summary>
        public double TotalHours {
            get { return Duration.TotalHours; }
        }

        /// <summary>
        /// Duration을 분 단위로 표현
        /// </summary>
        public double TotalMinutes {
            get { return Duration.TotalMinutes; }
        }

        /// <summary>
        /// Duration을 초 단위로 표현
        /// </summary>
        public double TotalSeconds {
            get { return Duration.TotalSeconds; }
        }

        /// <summary>
        /// Duration을 밀리초 단위로 표현
        /// </summary>
        public double TotalMilliseconds {
            get { return Duration.TotalMilliseconds; }
        }

        /// <summary>
        /// Ticks
        /// </summary>
        public long Ticks {
            get { return Duration.Ticks; }
        }

        /// <summary>
        /// 지정된 날짜에 시간 부분을 무시하고, 현재 시간부분으로 대체한 날짜를 반환합니다.
        /// </summary>
        /// <param name="moment">날짜</param>
        /// <returns></returns>
        public DateTime GetDateTime(DateTime moment) {
            return moment.Date.Add(Duration);
        }

        public DateTime GetDateTime(DateValue date) {
            return date.GetDateTime(this);
        }

        public int CompareTo(object obj) {
            obj.ShouldNotBeNull("obj");
            Guard.Assert(() => obj is TimeValue);

            return CompareTo((TimeValue)obj);
        }

        public int CompareTo(TimeValue other) {
            return Duration.CompareTo(other.Duration);
        }

        public bool Equals(TimeValue other) {
            return GetHashCode().Equals(other.GetHashCode());
        }

        public override bool Equals(object obj) {
            return (obj != null) && (obj is TimeValue) && Equals((TimeValue)obj);
        }

        public override int GetHashCode() {
            return HashTool.Compute(Duration);
        }

        public override string ToString() {
            return "TimeValue# " + Duration;
        }
    }
}