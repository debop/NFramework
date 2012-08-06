using System;
using System.Diagnostics;

namespace NSoft.NFramework.TimePeriods {
    /// <summary>
    /// 기준 일자의 시간 간격을 이용하여 기간을 표현합니다.
    /// </summary>
    [Serializable]
    [DebuggerDisplay("Start={Start}, Duration={Duration}, End={End}, IsReadOnly={IsReadOnly}")]
    public class TimeBlock : TimePeriodBase, ITimeBlock {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 기간이 무한대인 TimeBlock
        /// </summary>
        public static readonly TimeBlock Anytime = new TimeBlock(true);

        #region << Operators >>

        /// <summary>
        /// <see cref="TimeRange"/> 인스턴스를 암묵적으로 <see cref="TimeBlock"/> 형식의 인스턴스로 Casting합니다.
        /// </summary>
        /// <param name="block">원본 TimeRange 인스턴스</param>
        /// <returns><see cref="TimeBlock"/> 인스턴스</returns>
        public static explicit operator TimeRange(TimeBlock block) {
            if(block == null)
                return null;

            if(IsDebugEnabled)
                log.Debug("TimeBlock[{0}]을 TimeRange 수형으로 변환합니다.", block);

            return block.IsAnytime
                       ? TimeRange.Anytime
                       : new TimeRange(block.Start, block.End, block.IsReadOnly);
        }

        /// <summary>
        /// <see cref="TimeBlock"/> 인스턴스를 명시적으로 <see cref="TimeInterval"/> 형식의 인스턴스로 Casting합니다.
        /// </summary>
        /// <param name="block">원본 TimeBlock 인스턴스</param>
        /// <returns><see cref="TimeBlock"/> 인스턴스</returns>
        public static explicit operator TimeInterval(TimeBlock block) {
            if(IsDebugEnabled)
                log.Debug("TimeBlock[{0}]를 TimeInterval으로 변환합니다.", block);

            if(block == null)
                return null;

            return block.IsAnytime
                       ? TimeInterval.Anytime
                       : new TimeInterval(block);
        }

        #endregion

        #region << Constructors >>

        public TimeBlock() : this(false) {}

        public TimeBlock(bool isReadonly) : base(isReadonly) {
            _duration = TimeSpec.MaxPeriodDuration;
        }

        public TimeBlock(DateTime moment) : base(moment) {
            _duration = base.Duration;
        }

        public TimeBlock(DateTime moment, bool isReadOnly) : base(moment, isReadOnly) {
            _duration = base.Duration;
        }

        public TimeBlock(DateTime? start, DateTime? end) : this(start, end, false) {}

        public TimeBlock(DateTime? start, DateTime? end, bool isReadOnly) : base(start, end, isReadOnly) {
            _duration = base.Duration;
        }

        public TimeBlock(DateTime start, TimeSpan duration) : this(start, duration, false) {}

        public TimeBlock(DateTime start, TimeSpan duration, bool isReadonly) : base(start, duration, isReadonly) {
            AssertValidDuration(duration);
            _duration = base.Duration;
        }

        public TimeBlock(TimeSpan duration, DateTime end) : this(duration, end, false) {}

        public TimeBlock(TimeSpan duration, DateTime end, bool isReadonly) : base(null, end, isReadonly) {
            AssertValidDuration(duration);

            _duration = duration;
            _start = end.Subtract(_duration);
        }

        public TimeBlock(ITimePeriod source) : base(source) {
            _duration = base.Duration;
        }

        public TimeBlock(ITimePeriod source, bool isReadonly) : base(source, isReadonly) {
            _duration = base.Duration;
        }

        #endregion

        /// <summary>
        /// 기간의 시작 시각. 시작 시각이 미정인 경우 <see cref="TimeSpec.MinPeriodTime"/>를 반환합니다.
        /// </summary>
        public new DateTime Start {
            get { return base.Start; }
            set {
                AssertMutable();
                //if(HasEnd)
                //	TimeTool.AssertValidPeriod(value, End);
                _start = value;
            }
        }

        /// <summary>
        /// 기간의 완료 시각. 완료 시각이 미정인 경우 <see cref="TimeSpec.MaxPeriodTime"/>를 반환합니다.
        /// </summary>
        public new DateTime End {
            get { return base.End; }
            set {
                AssertMutable();
                //if(HasStart)
                //	TimeTool.AssertValidPeriod(Start, value);
                _end = value;
            }
        }

        private TimeSpan _duration;

        /// <summary>
        /// 기간을 TimeSpan으료 표현, 기간이 정해지지 않았다면 <see cref="TimeSpec.MaxPeriodDuration"/> 을 반환합니다.
        /// </summary>
        public override TimeSpan Duration {
            get { return _duration; }
            set {
                AssertMutable();
                AssertValidDuration(value);
                DurationFromStart(value);
            }
        }

        /// <summary>
        /// 현 TimeBlock 를 복사합니다.
        /// </summary>
        public new TimeBlock Copy() {
            return Copy(TimeSpan.Zero);
        }

        /// <summary>
        /// 현 TimeBlock에서 오프셋만큼 Shift 한 <see cref="TimeBlock"/> 반환합니다.
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public new TimeBlock Copy(TimeSpan offset) {
            if(IsDebugEnabled)
                log.Debug("[{0}] 를 offset[{1}] 을 주어 복사한 객체를 반환합니다...", GetType().Name, offset);

            if(offset == TimeSpan.Zero)
                return new TimeBlock(this);

            return new TimeBlock(HasStart ? Start.Add(offset) : _start,
                                 HasEnd ? End.Add(offset) : _end,
                                 IsReadOnly);
        }

        /// <summary>
        /// 기간 설정
        /// </summary>
        /// <param name="newStart"></param>
        /// <param name="duration"></param>
        public virtual void Setup(DateTime newStart, TimeSpan duration) {
            AssertValidDuration(duration);

            if(IsDebugEnabled)
                log.Debug("새로운 값으로 TimeBlock을 설정합니다. newStart=[{0}], duration=[{1}]", newStart, duration);

            _start = newStart;
            _duration = duration;
            _end = _start.Add(duration);
        }

        /// <summary>
        /// 시작시각(<see cref="ITimeBlock.Start"/>)은 고정, 기간(duration)으로 완료시각(<see cref="ITimeBlock.End"/>)를 재설정
        /// </summary>
        /// <param name="newDuration"></param>
        public virtual void DurationFromStart(TimeSpan newDuration) {
            AssertMutable();
            AssertValidDuration(newDuration);

            _duration = newDuration;
            _end = Start.Add(_duration);
        }

        /// <summary>
        /// 완료시각(<see cref="ITimeBlock.End"/>)은 고정 이전 기간(duration)으로 시작시간을 계산하여, 기간으로 재설정
        /// </summary>
        /// <param name="newDuration"></param>
        public virtual void DurationFromEnd(TimeSpan newDuration) {
            AssertMutable();
            AssertValidDuration(newDuration);

            _duration = newDuration;
            _start = End.Subtract(newDuration);
        }

        /// <summary>
        /// 현 TimeBlock의 이전 TimeBlock을 반환합니다.
        /// </summary>
        /// <returns></returns>
        public ITimeBlock GetPreviousBlock() {
            return GetPreviousBlock(TimeSpan.Zero);
        }

        /// <summary>
        /// 지정된 Offset만큼 기간이 이전 시간으로 이동한 TimeBlock을 반환한다.
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public virtual ITimeBlock GetPreviousBlock(TimeSpan offset) {
            var endOff = (offset > TimeSpan.Zero) ? offset.Negate() : offset;
            var result = new TimeBlock(Duration, Start.Add(endOff), IsReadOnly);

            if(IsDebugEnabled)
                log.Debug("이전 Block을 구합니다. offset=[{0}], PreviousBlock=[{1}]", offset, result);

            return result;
        }

        /// <summary>
        /// 현재 TimeBlock 이후 TimeBlock을 반환합니다
        /// </summary>
        /// <returns></returns>
        public ITimeBlock GetNextBlock() {
            return GetNextBlock(TimeSpan.Zero);
        }

        /// <summary>
        /// 지정된 Offset만큼 기간이 이후 시간으로 이동한 TimeBlock을 반환한다.
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public virtual ITimeBlock GetNextBlock(TimeSpan offset) {
            var startOff = (offset > TimeSpan.Zero) ? offset : offset.Negate();
            var result = new TimeBlock(End.Add(startOff), Duration, IsReadOnly);

            if(IsDebugEnabled)
                log.Debug("다음 Block을 구합니다. offset=[{0}], NextBlock=[{1}]", offset, result);

            return result;
        }

        /// <summary>
        /// 두 기간의 겹치는 기간을 반환합니다.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public new TimeBlock GetIntersection(ITimePeriod other) {
            other.ShouldNotBeNull("other");
            return TimeTool.GetIntersectionBlock(this, other);
        }

        /// <summary>
        /// 두 기간의 합집합 기간을 반환합니다.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public new TimeBlock GetUnion(ITimePeriod other) {
            other.ShouldNotBeNull("other");
            return TimeTool.GetUnionBlock(this, other);
        }

        /// <summary>
        /// <paramref name="duration"/>이 유효한 값인지 검사합니다.
        /// </summary>
        /// <param name="duration"></param>
        protected virtual void AssertValidDuration(TimeSpan duration) {
            duration.ShouldBeGreaterOrEqual(TimeSpec.MinPeriodDuration, "duration");
        }
    }
}