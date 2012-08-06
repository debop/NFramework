using System;
using System.Diagnostics;

namespace NSoft.NFramework.TimePeriods {
    /// <summary>
    /// 기간을 표현하는 클래스입니다.
    /// </summary>
    [Serializable]
    [DebuggerDisplay("Start={Start}, End={End}, Duration={Duration}, IsReadOnly={IsReadOnly}")]
    public class TimeRange : TimePeriodBase, ITimeRange {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 시작 시각이 NULL, 완료 시각이 NULL인 OPEN 구간을 가지는 기간을 뜻한다. (그래서 AnyTime 이다)
        /// </summary>
        public static readonly TimeRange Anytime = new TimeRange(true);

        #region << Operators >>

        /// <summary>
        /// <see cref="TimeRange"/> 인스턴스를 암묵적으로 <see cref="TimeBlock"/> 형식의 인스턴스로 Casting합니다.
        /// </summary>
        /// <param name="range">원본 TimeRange 인스턴스</param>
        /// <returns><see cref="TimeBlock"/> 인스턴스</returns>
        public static implicit operator TimeBlock(TimeRange range) {
            if(IsDebugEnabled)
                log.Debug("TimeRange[{0}]를 TimeBlock으로 변환합니다.", range);

            if(range == null)
                return null;

            return range.IsAnytime
                       ? TimeBlock.Anytime
                       : new TimeBlock(range.Start, range.Duration, range.IsReadOnly);
        }

        /// <summary>
        /// <see cref="TimeRange"/> 인스턴스를 명시적으로 <see cref="TimeInterval"/> 형식의 인스턴스로 Casting합니다.
        /// </summary>
        /// <param name="range">원본 TimeRange 인스턴스</param>
        /// <returns><see cref="TimeBlock"/> 인스턴스</returns>
        public static implicit operator TimeInterval(TimeRange range) {
            if(IsDebugEnabled)
                log.Debug("TimeRange[{0}]를 TimeInterval으로 변환합니다.", range);

            if(range == null)
                return null;

            return range.IsAnytime
                       ? TimeInterval.Anytime
                       : new TimeInterval(range);
        }

        #endregion

        #region << Constructors >>

        public TimeRange() {}
        public TimeRange(bool isReadonly) : base(isReadonly) {}

        public TimeRange(DateTime? start, DateTime? end) : base(start, end) {}
        public TimeRange(DateTime? start, DateTime? end, bool isReadonly) : base(start, end, isReadonly) {}

        public TimeRange(DateTime moment) : base(moment) {}
        public TimeRange(DateTime moment, bool isReadOnly) : base(moment, isReadOnly) {}

        public TimeRange(DateTime start, TimeSpan duration) : base(start, duration) {}
        public TimeRange(DateTime start, TimeSpan duration, bool isReadonly) : base(start, duration, isReadonly) {}

        public TimeRange(ITimePeriod source) : base(source) {}
        public TimeRange(ITimePeriod source, bool isReadonly) : base(source, isReadonly) {}

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
        /// 완료 시각을 지정된 시각으로 확장합니다. 완료 시각 이전이 되면 안됩니다.
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
            AssertMutable();

            ExpandStartTo(moment);
            ExpandEndTo(moment);
        }

        /// <summary>
        /// 시작시각과 완료시각을 지정된 기간 정보를 기준으로 변경합니다.
        /// </summary>
        /// <param name="period"></param>
        public void ExpandTo(ITimePeriod period) {
            period.ShouldNotBeNull("period");

            AssertMutable();

            if(period.HasStart)
                ExpandStartTo(period.Start);

            if(period.HasEnd)
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

            if(HasInside(moment) && moment < _end)
                _end = moment;
        }

        /// <summary>
        /// 기간을 지정한 기간으로 축소시킵니다.
        /// </summary>
        /// <param name="period"></param>
        public virtual void ShrinkTo(ITimePeriod period) {
            period.ShouldNotBeNull("period");

            if(period.HasStart)
                ShrinkStartTo(period.Start);

            if(period.HasEnd)
                ShrinkEndTo(period.End);
        }

        /// <summary>
        /// 두 기간의 겹치는 기간을 반환합니다.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public new ITimeRange GetIntersection(ITimePeriod other) {
            other.ShouldNotBeNull("other");
            return TimeTool.GetIntersectionRange(this, other);
        }

        /// <summary>
        /// 두 기간의 합집합 기간을 반환합니다.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public new ITimeRange GetUnion(ITimePeriod other) {
            other.ShouldNotBeNull("other");
            return TimeTool.GetUnionRange(this, other);
        }
    }
}