using System;

namespace NSoft.NFramework.TimePeriods {
    /// <summary>
    /// 시간 간격을 표현합니다.
    /// </summary>
    [Serializable]
    public class TimeInterval : TimePeriodBase, ITimeInterval {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public static TimeInterval Anytime = new TimeInterval(TimeSpec.MinPeriodTime,
                                                              TimeSpec.MaxPeriodTime,
                                                              IntervalEdge.Closed,
                                                              IntervalEdge.Closed,
                                                              false,
                                                              true);

        /// <summary>
        /// <see cref="TimeInterval"/> 인스턴스를 명시적으로 <see cref="TimeRange"/> 형식의 인스턴스로 Casting합니다.
        /// </summary>
        /// <param name="interval">원본 TimeInterval 인스턴스</param>
        /// <returns><see cref="TimeRange"/> 인스턴스</returns>
        public static explicit operator TimeRange(TimeInterval interval) {
            if(IsDebugEnabled)
                log.Debug("TimeInterval[{0}]를 TimeRange으로 변환합니다.", interval);

            if(interval == null)
                return null;

            return interval.IsAnytime
                       ? TimeRange.Anytime
                       : new TimeRange(interval.StartInterval, interval.EndInterval, interval.IsReadOnly);
        }

        /// <summary>
        /// <see cref="TimeInterval"/> 인스턴스를 명시적으로 <see cref="TimeBlock"/> 형식의 인스턴스로 Casting합니다.
        /// </summary>
        /// <param name="interval">원본 TimeRange 인스턴스</param>
        /// <returns><see cref="TimeBlock"/> 인스턴스</returns>
        public static explicit operator TimeBlock(TimeInterval interval) {
            if(IsDebugEnabled)
                log.Debug("TimeInterval[{0}]를 TimeBlock 으로 변환합니다.", interval);

            if(interval == null)
                return null;

            return interval.IsAnytime
                       ? TimeBlock.Anytime
                       : new TimeBlock(interval.StartInterval, interval.EndInterval, interval.IsReadOnly);
        }

        private bool _isIntervalEnabled;
        private IntervalEdge _startEdge;
        private IntervalEdge _endEdge;

        #region << Constructors >>

        public TimeInterval() : this(TimeSpec.MinPeriodTime, TimeSpec.MaxPeriodTime) {}
        public TimeInterval(DateTime moment) : this(moment, moment) {}

        public TimeInterval(DateTime moment, IntervalEdge startEdge, IntervalEdge endEdge, bool isIntervalEnabled, bool isReadOnly)
            : this(moment, moment, startEdge, endEdge, isIntervalEnabled, isReadOnly) {}

        public TimeInterval(DateTime startInterval, DateTime endInterval)
            : this(startInterval, endInterval, IntervalEdge.Closed, IntervalEdge.Closed, true, false) {}

        public TimeInterval(DateTime startInterval, DateTime endInterval, IntervalEdge startEdge, IntervalEdge endEdge)
            : this(startInterval, endInterval, startEdge, endEdge, true, false) {}

        public TimeInterval(DateTime startInterval, DateTime endInterval, IntervalEdge startEdge, IntervalEdge endEdge,
                            bool isIntervalEnabled, bool isReadOnly)
            : base(startInterval, endInterval, isReadOnly) {
            _startEdge = startEdge;
            _endEdge = endEdge;

            _isIntervalEnabled = isIntervalEnabled;
        }

        public TimeInterval(ITimePeriod src) : base(src) {
            src.ShouldNotBeNull("src");

            var interval = src as ITimeInterval;
            if(interval != null) {
                _start = interval.StartInterval;
                _end = interval.EndInterval;
                _startEdge = interval.StartEdge;
                _endEdge = interval.EndEdge;
                _isIntervalEnabled = interval.IsIntervalEnabled;
            }
        }

        public TimeInterval(ITimePeriod src, bool isReadOnly) : this(src) {
            IsReadOnly = isReadOnly;
        }

        #endregion

        /// <summary>
        /// 시작 시각이 열린 구간인가 (즉 StartTime이 지정되지 않았음)
        /// </summary>
        public bool IsStartOpen {
            get { return _startEdge == IntervalEdge.Open; }
        }

        /// <summary>
        /// 완료 시각이 열린 구간인가 (즉 EndTime이 지정되지 않았음)
        /// </summary>
        public bool IsEndOpen {
            get { return _endEdge == IntervalEdge.Open; }
        }

        /// <summary>
        /// 개구간인가? (StartTime, EndTime 모두 설정되어 있지 않을 때)
        /// </summary>
        public bool IsOpen {
            get { return IsStartOpen && IsEndOpen; }
        }

        /// <summary>
        /// 시작시각이 폐구간인지 여부 (시작 시각이 지정되어 있다)
        /// </summary>
        public bool IsStartClosed {
            get { return _startEdge == IntervalEdge.Closed; }
        }

        /// <summary>
        /// 완료 시각이 폐구간인지 여부 (완료 시각이 지정되어 있다)
        /// </summary>
        public bool IsEndClosed {
            get { return _endEdge == IntervalEdge.Closed; }
        }

        /// <summary>
        /// 폐구간인가? (StartTime, EndTime 모두 의미있는 값으로 설정되어 있다)
        /// </summary>
        public bool IsClosed {
            get { return IsStartClosed && IsEndClosed; }
        }

        /// <summary>
        /// 빈 간격인가?
        /// </summary>
        public bool IsEmpty {
            get { return IsMoment && !IsClosed; }
        }

        /// <summary>
        /// 인터발로 쓸 수 없는 경우 (IsMoment 이면서, IsClosed 인 경우)
        /// </summary>
        public bool IsDegenerate {
            get { return IsMoment && IsClosed; }
        }

        /// <summary>
        /// 사용가능한 시간간격인가?
        /// </summary>
        public bool IsIntervalEnabled {
            get { return _isIntervalEnabled; }
            set {
                AssertMutable();
                _isIntervalEnabled = value;
            }
        }

        /// <summary>
        /// 시작 시각이 지정되었는가?
        /// </summary>
        public override bool HasStart {
            get { return _start != TimeSpec.MinPeriodTime || _startEdge != IntervalEdge.Closed; }
        }

        /// <summary>
        /// 시작 시각
        /// </summary>
        public DateTime StartInterval {
            get { return _start; }
            set {
                AssertMutable();
                value.ShouldBeLessOrEqual(_end, "StartInterval");
                _start = value;
            }
        }

        /// <summary>
        /// 기간의 시작 시각. 시작 시각이 미정인 경우 <see cref="TimeSpec.MinPeriodTime"/>를 반환합니다.
        /// </summary>
        public override DateTime Start {
            get {
                if(_isIntervalEnabled && _startEdge == IntervalEdge.Open)
                    return _start.AddTicks(1);
                return _start;
            }
        }

        /// <summary>
        /// 시작 시각의 걔/폐구간 종류
        /// </summary>
        public IntervalEdge StartEdge {
            get { return _startEdge; }
            set {
                AssertMutable();
                _startEdge = value;
            }
        }

        /// <summary>
        /// 시작 시각이 지정되었는가?
        /// </summary>
        public override bool HasEnd {
            get { return _end != TimeSpec.MaxPeriodTime || _endEdge != IntervalEdge.Closed; }
        }

        /// <summary>
        /// 완료 시각
        /// </summary>
        public DateTime EndInterval {
            get { return _end; }
            set {
                AssertMutable();
                value.ShouldBeGreaterOrEqual(_start, "EndInterval");
                _end = value;
            }
        }

        /// <summary>
        /// 기간의 완료 시각. 완료 시각이 미정인 경우 <see cref="TimeSpec.MaxPeriodTime"/>를 반환합니다.
        /// </summary>
        public override DateTime End {
            get {
                if(_isIntervalEnabled && _endEdge == IntervalEdge.Open)
                    return _end.AddTicks(-1);
                return _end;
            }
        }

        /// <summary>
        /// 완료 시각의 개/폐구간 종류
        /// </summary>
        public IntervalEdge EndEdge {
            get { return _endEdge; }
            set {
                AssertMutable();
                _endEdge = value;
            }
        }

        /// <summary>
        /// 기간을 TimeSpan으료 표현, 기간이 정해지지 않았다면 <see cref="TimeSpec.MaxPeriodDuration"/> 을 반환합니다.
        /// </summary>
        public override TimeSpan Duration {
            get { return _end.Subtract(_start); }
        }

        /// <summary>
        /// 기간을 설정합니다.
        /// </summary>
        /// <param name="newStart">설정할 시작 시각</param>
        /// <param name="newEnd">설정할 완료 시각</param>
        public override void Setup(DateTime? newStart, DateTime? newEnd) {
            base.Setup(newStart, newEnd);

            if(IsDebugEnabled)
                log.Debug("기간을 새로 설정합니다. newStart=[{0}], newEnd=[{1}]", newStart, newEnd);

            AssertMutable();

            TimeTool.AdjustPeriod(ref newStart, ref newEnd);

            _start = newStart.GetValueOrDefault(TimeSpec.MinPeriodTime);
            _end = newEnd.GetValueOrDefault(TimeSpec.MaxPeriodTime);
        }

        /// <summary>
        /// 시작 시각을 지정된 시각으로 변경합니다. 시작 시각 이후가 되면 안됩니다.
        /// </summary>
        /// <param name="moment"></param>
        public virtual void ExpandStartTo(DateTime moment) {
            AssertMutable();

            if(_start > moment)
                _start = moment;
        }

        /// <summary>
        /// 완료 시각을 지정된 시각으로 변경합니다. 완료 시각 이전이 되면 안됩니다.
        /// </summary>
        /// <param name="moment"></param>
        public virtual void ExpandEndTo(DateTime moment) {
            AssertMutable();

            if(_end < moment)
                _end = moment;
        }

        /// <summary>
        /// 시작 시각과 완료시각을 지정된 시각으로 설정합니다.
        /// </summary>
        /// <param name="moment"></param>
        public virtual void ExpandTo(DateTime moment) {
            ExpandStartTo(moment);
            ExpandEndTo(moment);
        }

        /// <summary>
        /// 시작시각과 완료시각을 지정된 기간 정보를 기준으로 변경합니다.
        /// </summary>
        /// <param name="period"></param>
        public virtual void ExpandTo(ITimePeriod period) {
            period.ShouldNotBeNull("period");

            ExpandStartTo(period.Start);
            ExpandEndTo(period.End);
        }

        /// <summary>
        /// 시작 시각을 지정된 시각으로 변경합니다. 시작시각보다 이후 시각이여야 합니다.
        /// </summary>
        /// <param name="moment"></param>
        public virtual void ShrinkStartTo(DateTime moment) {
            AssertMutable();
            if(HasInside(moment) && _start < moment)
                _start = moment;
        }

        /// <summary>
        /// 완료 시각을 지정된 시각으로 당깁니다. 완료시각보다 이전 시각이여야 합니다.
        /// </summary>
        /// <param name="moment"></param>
        public virtual void ShrinkEndTo(DateTime moment) {
            AssertMutable();
            if(HasInside(moment) && _end > moment)
                _end = moment;
        }

        /// <summary>
        /// 시작 시각과 완료시각을 지정된 시각으로 축소 합니다.
        /// </summary>
        /// <param name="moment"></param>
        public virtual void ShrinkTo(DateTime moment) {
            ShrinkStartTo(moment);
            ShrinkEndTo(moment);
        }

        /// <summary>
        /// 기간을 지정한 기간으로 축소시킵니다.
        /// </summary>
        /// <param name="period"></param>
        public virtual void ShrinkTo(ITimePeriod period) {
            period.ShouldNotBeNull("period");

            ShrinkStartTo(period.Start);
            ShrinkEndTo(period.End);
        }

        /// <summary>
        /// 현 인스턴스의 기간을 가진 인스턴스를 새로 생성하여 반환합니다.
        /// </summary>
        /// <returns></returns>
        public new ITimeInterval Copy() {
            return Copy(TimeSpan.Zero);
        }

        /// <summary>
        /// 현재 IInterval에서 오프셋만큼 이동한 <see cref="ITimeInterval"/>정보를 반환합니다.
        /// </summary>
        /// <param name="offset">이동할 오프셋</param>
        /// <returns></returns>
        public new ITimeInterval Copy(TimeSpan offset) {
            return new TimeInterval(StartInterval.Add(offset),
                                    EndInterval.Add(offset),
                                    StartEdge,
                                    EndEdge,
                                    IsIntervalEnabled,
                                    IsReadOnly);
        }

        /// <summary>
        /// Interval을 재설정합니다.
        /// </summary>
        public override void Reset() {
            base.Reset();

            _isIntervalEnabled = true;
            _startEdge = IntervalEdge.Closed;
            _endEdge = IntervalEdge.Closed;
        }

        /// <summary>
        /// 두 기간의 겹치는 기간을 반환합니다.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public new ITimeInterval GetIntersection(ITimePeriod other) {
            var range = base.GetIntersection(other);

            if(range == null)
                return null;

            return new TimeInterval(range.Start,
                                    range.End,
                                    IntervalEdge.Closed,
                                    IntervalEdge.Closed,
                                    IsIntervalEnabled,
                                    IsReadOnly);
        }

        /// <summary>
        /// 두 기간의 합집합 기간을 반환합니다.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public new ITimeInterval GetUnion(ITimePeriod other) {
            other.ShouldNotBeNull("other");
            return (TimeInterval)TimeTool.GetUnionRange(this, other);
        }

        //---------------------------------------------------------------------

        protected override string Format(ITimeFormatter formatter) {
            return formatter.GetInterval(_start, _end, _startEdge, _endEdge, Duration);
        }

        public bool Equals(ITimeInterval other) {
            return (other != null) && (GetHashCode().Equals(other.GetHashCode()));
        }

        public override int GetHashCode() {
            var baseHash = base.GetHashCode();
            return HashTool.Compute(baseHash, IsIntervalEnabled, StartEdge, EndEdge);
        }
    }
}