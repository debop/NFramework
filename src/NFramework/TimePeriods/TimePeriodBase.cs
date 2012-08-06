using System;
using System.Diagnostics;

namespace NSoft.NFramework.TimePeriods {
    /// <summary>
    /// 시작일자와 완료일자로 기간을 표현하는 기본 클래스입니다.
    /// </summary>
    [Serializable]
    [DebuggerDisplay("Start={Start}, End={End}, Duration={Duration}, IsReadOnly={IsReadOnly}")]
    public abstract class TimePeriodBase : ITimePeriod {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        #region << Constructors >>

        /// <summary>
        /// 생성자
        /// </summary>
        protected TimePeriodBase() : this(null, null, false) {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="isReadOnly">일기 전용 여부</param>
        protected TimePeriodBase(bool isReadOnly) : this(null, null, isReadOnly) {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="moment">기준 일자 (시작일자와 완료일자에 해당)</param>
        protected TimePeriodBase(DateTime moment) : this(moment, moment, false) {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="moment">기준 일자 (시작일자와 완료일자에 해당)</param>
        /// <param name="isReadOnly">일기 전용 여부</param>
        protected TimePeriodBase(DateTime moment, bool isReadOnly) : this(moment, moment, isReadOnly) {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="start">시작 일자</param>
        /// <param name="end">완료 일자</param>
        protected TimePeriodBase(DateTime? start, DateTime? end) : this(start, end, false) {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="start">시작 일자</param>
        /// <param name="end">완료 일자</param>
        /// <param name="isReadOnly">읽기 전용 여부</param>
        protected TimePeriodBase(DateTime? start, DateTime? end, bool isReadOnly) {
            TimeTool.AdjustPeriod(ref start, ref end);

            _start = start ?? TimeSpec.MinPeriodTime;
            _end = end ?? TimeSpec.MaxPeriodTime;
            IsReadOnly = isReadOnly;
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="start">시작일자</param>
        /// <param name="duration">시간 간격</param>
        protected TimePeriodBase(DateTime start, TimeSpan duration) : this(start, duration, false) {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="start">시작일자</param>
        /// <param name="duration">시간 간격</param>
        /// <param name="isReadOnly">읽기 전용 여부</param>
        protected TimePeriodBase(DateTime start, TimeSpan duration, bool isReadOnly) {
            TimeTool.AdjustPeriod(ref start, ref duration);
            _start = start;
            Duration = duration;
            IsReadOnly = isReadOnly;
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="source">복사할 원본 ITimePeriod</param>
        protected TimePeriodBase(ITimePeriod source) {
            source.ShouldNotBeNull("source");

            _start = source.Start;
            _end = source.End;
            IsReadOnly = source.IsReadOnly;
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="source">복사할 원본 ITimePeriod</param>
        /// <param name="isReadOnly">읽기 전용 여부</param>
        protected TimePeriodBase(ITimePeriod source, bool isReadOnly) : this(source) {
            IsReadOnly = isReadOnly;
        }

        #endregion

        [CLSCompliant(false)] protected DateTime _start;

        [CLSCompliant(false)] protected DateTime _end;

        /// <summary>
        /// 기간의 시작 시각. 시작 시각이 미정인 경우 <see cref="TimeSpec.MinPeriodTime"/>를 반환합니다.
        /// </summary>
        public virtual DateTime Start {
            get { return _start; }
        }

        /// <summary>
        /// 기간의 완료 시각. 완료 시각이 미정인 경우 <see cref="TimeSpec.MaxPeriodTime"/>를 반환합니다.
        /// </summary>
        public virtual DateTime End {
            get { return _end; }
        }

        /// <summary>
        /// 기간의 시작 시각. null 인 경우 시작 시각을 정하지 않은 것입니다.
        /// </summary>
        public DateTime? StartAsNullable {
            get { return (HasStart) ? _start : (DateTime?)null; }
        }

        /// <summary>
        /// 기간의 완료 시각. null인 경우 완료 시각을 정하지 않은 것입니다.
        /// </summary>
        public DateTime? EndAsNullable {
            get { return (HasEnd) ? _end : (DateTime?)null; }
        }

        /// <summary>
        /// 기간을 TimeSpan으료 표현, 기간이 정해지지 않았다면 <see cref="TimeSpec.MaxPeriodDuration"/> 을 반환합니다.
        /// </summary>
        public virtual TimeSpan Duration {
            get { return _end.Subtract(_start); }
            set {
                AssertMutable();
                value.ShouldBeGreaterOrEqual(TimeSpan.Zero, "Duration");

                if(HasStart)
                    _end = _start.Add(value);
            }
        }

        public virtual string DurationDescription {
            get { return TimeFormatter.Instance.GetDuration(Duration, DurationFormatKind.Detailed); }
        }

        /// <summary>
        /// 시작 시각이 지정되었는가?
        /// </summary>
        public virtual bool HasStart {
            get { return _start != TimeSpec.MinPeriodTime; }
        }

        /// <summary>
        /// 완료 시각이 지정되었는가?
        /// </summary>
        public virtual bool HasEnd {
            get { return _end != TimeSpec.MaxPeriodTime; }
        }

        /// <summary>
        /// 정해진 기간이 있는지 표시합니다. 즉 의미있는 기간으로 설정되어 있는가?
        /// </summary>
        public bool HasPeriod {
            get { return HasStart && HasEnd; }
        }

        /// <summary>
        /// 시작 시각과 완료 시각의 값이 같은가? 
        /// </summary>
        public virtual bool IsMoment {
            get { return Equals(_start, _end); }
        }

        /// <summary>
        /// 시작 시각도 없고, 완료 시각도 없는 기간인가? (즉 AnyTime)
        /// </summary>
        public virtual bool IsAnytime {
            get { return !HasStart && !HasEnd; }
        }

        /// <summary>
        /// 읽기 전용
        /// </summary>
        public virtual bool IsReadOnly { get; protected set; }

        /// <summary>
        /// 기간을 설정합니다.
        /// </summary>
        /// <param name="newStart">설정할 시작 시각</param>
        /// <param name="newEnd">설정할 완료 시각</param>
        public virtual void Setup(DateTime? newStart, DateTime? newEnd) {
            if(IsDebugEnabled)
                log.Debug("기간을 새로 설정합니다. newStart=[{0}], newEnd=[{1}]", newStart, newEnd);

            AssertMutable();

            TimeTool.AdjustPeriod(ref newStart, ref newEnd);

            _start = newStart.GetValueOrDefault(TimeSpec.MinPeriodTime);
            _end = newEnd.GetValueOrDefault(TimeSpec.MaxPeriodTime);
        }

        /// <summary>
        /// 현 TimePeriod 의 속성을 복사하여, 새로운 TimePeriod를 생성하여 반환합니다.
        /// </summary>
        public ITimePeriod Copy() {
            return Copy(TimeSpan.Zero);
        }

        /// <summary>
        /// 현 ITimePeriod 에서 <paramref name="offset"/> 만큼 Shift 한 <see cref="ITimeRange"/>정보를 반환합니다.
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public virtual ITimePeriod Copy(TimeSpan offset) {
            if(IsDebugEnabled)
                log.Debug("[{0}] 를 offset[{1}] 을 주어 복사한 객체를 반환합니다...", GetType().Name, offset);

            if(offset == TimeSpan.Zero)
                return new TimeRange(this);

            return new TimeRange(HasStart ? Start.Add(offset) : _start,
                                 HasEnd ? End.Add(offset) : _end,
                                 IsReadOnly);
        }

        /// <summary>
        /// 현재 기간에서 오프셋만큼 Shift 한 <see cref="ITimeRange"/>정보를 반환합니다.
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        ITimePeriod ITimePeriod.Copy(TimeSpan offset) {
            return Copy(offset);
        }

        /// <summary>
        /// 기간을 오프셋만큼 이동
        /// </summary>
        /// <param name="offset"></param>
        public virtual void Move(TimeSpan offset) {
            if(offset == TimeSpan.Zero)
                return;

            AssertMutable();

            if(IsDebugEnabled)
                log.Debug("[{0}] 를 offset[{1}]만큼 이동합니다...", GetType().Name, offset);

            if(HasStart)
                _start = _start.Add(offset);

            if(HasEnd)
                _end = _end.Add(offset);
        }

        /// <summary>
        /// 두 기간이 같은 기간을 나타내는지 검사합니다
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public virtual bool IsSamePeriod(ITimePeriod other) {
            return (other != null) && Equals(Start, other.Start) && Equals(End, other.End);
        }

        /// <summary>
        /// 지정된 시각이 기간에 속하는지 검사합니다.
        /// </summary>
        /// <param name="moment"></param>
        /// <returns></returns>
        public virtual bool HasInside(DateTime moment) {
            return TimeTool.HasInside(this, moment);
        }

        /// <summary>
        /// 지정한 기간이 현 기간 내에 속하는지 검사합니다.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public virtual bool HasInside(ITimePeriod other) {
            other.ShouldNotBeNull("other");
            return TimeTool.HasInside(this, other);
        }

        /// <summary>
        /// 지정한 기간이 현 기간과 겹치는 부분이 있는지 검사합니다.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public virtual bool IntersectsWith(ITimePeriod other) {
            other.ShouldNotBeNull("other");
            return TimeTool.IntersectsWith(this, other);
        }

        /// <summary>
        /// 지정한 기간이 현 기간과 겹치는 부분이 있는지 검사합니다.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public virtual bool OverlapsWith(ITimePeriod other) {
            other.ShouldNotBeNull("other");
            return TimeTool.OverlapsWith(this, other);
        }

        /// <summary>
        /// 기간을 미정으로 초기화합니다.
        /// </summary>
        public virtual void Reset() {
            AssertMutable();

            _start = TimeSpec.MinPeriodTime;
            _end = TimeSpec.MaxPeriodTime;
        }

        /// <summary>
        /// 다른 TimePeriod와의 관계를 나타냅니다.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public virtual PeriodRelation GetRelation(ITimePeriod other) {
            other.ShouldNotBeNull("other");
            return TimeTool.GetReleation(this, other);
        }

        /// <summary>
        /// TimePeriod의 설명을 표현합니다.
        /// </summary>
        /// <returns></returns>
        public virtual string GetDescription(ITimeFormatter formatter) {
            return Format(formatter ?? TimeFormatter.Instance);
        }

        /// <summary>
        /// 두 기간의 겹치는 기간을 반환합니다.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public virtual TimePeriodBase GetIntersection(ITimePeriod other) {
            other.ShouldNotBeNull("other");
            return TimeTool.GetIntersectionRange(this, other);
        }

        ITimePeriod ITimePeriod.GetIntersection(ITimePeriod other) {
            return GetIntersection(other);
        }

        /// <summary>
        /// 두 기간의 합집합 기간을 반환합니다.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public virtual TimePeriodBase GetUnion(ITimePeriod other) {
            other.ShouldNotBeNull("other");
            return TimeTool.GetUnionRange(this, other);
        }

        ITimePeriod ITimePeriod.GetUnion(ITimePeriod other) {
            return GetUnion(other);
        }

        /// <summary>
        /// <paramref name="formatter"/>를 이용하여 기간 내용을 문자열로 표현합니다.
        /// </summary>
        /// <param name="formatter"></param>
        /// <returns></returns>
        protected virtual string Format(ITimeFormatter formatter) {
            return formatter.GetPeriod(Start, End, Duration);
        }

        /// <summary>
        /// 변경가능한 (읽기전용이 아닌) 객체인지 확인합니다. 읽기 전용일 경우 예외를 발생시킵니다.
        /// </summary>
        protected virtual void AssertMutable() {
            if(IsReadOnly)
                throw new InvalidOperationException("Current object is Readonly.");
        }

        public virtual int CompareTo(object obj) {
            obj.ShouldNotBeNull("obj");

            return CompareTo((ITimePeriod)obj);
        }

        public int CompareTo(ITimePeriod other) {
            return Start.CompareTo(other.Start);
        }

        /// <summary>
        /// 현재 개체가 동일한 형식의 다른 개체와 같은지 여부를 나타냅니다. (StartTime, EndTime, IsReadOnly가 같아야 True를 반환합니다)
        /// </summary>
        /// <returns>
        /// 현재 개체가 <paramref name="other"/> 매개 변수와 같으면 true이고, 그렇지 않으면 false입니다.
        /// </returns>
        /// <param name="other">이 개체와 비교할 개체입니다.</param>
        public bool Equals(ITimePeriod other) {
            return (other != null) && GetHashCode().Equals(other.GetHashCode());
        }

        /// <summary>
        /// 현재 개체가 동일한 형식의 다른 개체와 같은지 여부를 나타냅니다. (StartTime, EndTime, IsReadOnly가 같아야 True를 반환합니다)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) {
            return (obj != null) && (obj is ITimePeriod) && Equals((ITimePeriod)obj);
        }

        public override int GetHashCode() {
            return HashTool.Compute(GetType(), Start, End, IsReadOnly);
        }

        public override string ToString() {
            return string.Concat(GetType().Name, "# ", GetDescription(null));
        }
    }
}