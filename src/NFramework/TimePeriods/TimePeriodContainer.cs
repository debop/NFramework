using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NSoft.NFramework.LinqEx;

namespace NSoft.NFramework.TimePeriods {
    /// <summary>
    /// <see cref="ITimePeriod"/>를 항목로 가지는 컨테이너를 표현합니다.
    /// </summary>
    [Serializable]
    public abstract class TimePeriodContainer : ITimePeriodContainer {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        [CLSCompliant(false)] protected readonly List<ITimePeriod> _periods = new List<ITimePeriod>();

        /// <summary>
        /// 생성자
        /// </summary>
        protected TimePeriodContainer() {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="periods"></param>
        protected TimePeriodContainer(IEnumerable<ITimePeriod> periods) {
            if(periods != null)
                _periods.AddRange(periods);
        }

        /// <summary>
        /// 컬렉션을 반복하는 열거자를 반환합니다.
        /// </summary>
        /// <returns>
        /// 컬렉션을 반복하는 데 사용할 수 있는 <see cref="T:System.Collections.Generic.IEnumerator`1"/>입니다.
        /// </returns>
        public IEnumerator<ITimePeriod> GetEnumerator() {
            return _periods.GetEnumerator();
        }

        /// <summary>
        /// 컬렉션을 반복하는 열거자를 반환합니다.
        /// </summary>
        /// <returns>
        /// 컬렉션을 반복하는 데 사용할 수 있는 <see cref="T:System.Collections.IEnumerator"/> 개체입니다.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        /// <summary>
        /// <paramref name="item"/>을 컬렉션의 항목으로 추가합니다.
        /// </summary>
        /// <param name="item"></param>
        public virtual void Add(ITimePeriod item) {
            _periods.Add(item);
        }

        /// <summary>
        /// 모든 항목을 제거합니다.
        /// </summary>
        public virtual void Clear() {
            _periods.Clear();
        }

        /// <summary>
        /// <paramref name="item"/>에 해당하는 항목이 들어있는지 검사한다.
        /// </summary>
        public virtual bool Contains(ITimePeriod item) {
            return _periods.Contains(item);
        }

        /// <summary>
        /// 특정 인덱스 (<paramref name="arrayIndex"/>) 부터 시작하여 항목들을 <paramref name="array"/> 배열에 복사합니다.
        /// </summary>
        public virtual void CopyTo(ITimePeriod[] array, int arrayIndex) {
            _periods.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// <paramref name="item"/> 항목을 컬렉션에서 제거합니다.
        /// </summary>
        /// <param name="item">제거할 항목</param>
        /// <returns>제거 결과</returns>
        public virtual bool Remove(ITimePeriod item) {
            return _periods.Remove(item);
        }

        /// <summary>
        /// 항목의 수
        /// </summary>
        public virtual int Count {
            get { return _periods.Count; }
        }

        /// <summary>
        /// 읽기 전용
        /// </summary>
        public virtual bool IsReadOnly {
            get { return false; }
        }

        /// <summary>
        /// 대상 기간을 포함하고 있는지 검사합니다.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public virtual bool ContainsPeriod(ITimePeriod target) {
            target.ShouldNotBeNull("target");
            return _periods.Any(p => p.IsSamePeriod(target));
        }

        /// <summary>
        /// 기간이 존재하는 (HasPeriod가 true인) <see cref="ITimePeriod"/>들을 추가합니다.
        /// </summary>
        /// <param name="periods"></param>
        public virtual void AddAll(IEnumerable<ITimePeriod> periods) {
            if(periods != null) {
                _periods.AddRange(periods);
            }
        }

        /// <summary>
        /// ITimePeriod 항목들의 Start 속성 값을 기준으로 정렬을 수행합니다.
        /// </summary>
        /// <param name="sortDir"></param>
        public virtual void SortByStart(OrderDirection sortDir = OrderDirection.Asc) {
            var isAsc = (sortDir == OrderDirection.Asc);

            if(isAsc)
                _periods.Sort((lhs, rhs) => lhs.Start.CompareTo(rhs.Start));
            else
                _periods.Sort((lhs, rhs) => rhs.Start.CompareTo(lhs.Start));
        }

        /// <summary>
        /// ITimePeriod 항목들의 End 속성 값을 기준으로 정렬을 수행합니다.
        /// </summary>
        /// <param name="sortDir"></param>
        public virtual void SortByEnd(OrderDirection sortDir = OrderDirection.Asc) {
            var isAsc = sortDir == OrderDirection.Asc;

            if(isAsc)
                _periods.Sort((lhs, rhs) => rhs.End.CompareTo(lhs.End));
            else
                _periods.Sort((lhs, rhs) => lhs.End.CompareTo(rhs.End));
        }

        /// <summary>
        /// ITimePeriod 항목들의 Duration 속성 값을 기준으로 정렬을 수행합니다.
        /// </summary>
        /// <param name="sortDir"></param>
        public virtual void SortByDuration(OrderDirection sortDir = OrderDirection.Asc) {
            var isAsc = (sortDir == OrderDirection.Asc);

            if(isAsc)
                _periods.Sort((lhs, rhs) => rhs.Duration.CompareTo(lhs.Duration));
            else
                _periods.Sort((lhs, rhs) => lhs.Duration.CompareTo(rhs.Duration));
        }

        /// <summary>
        /// 기간의 시작 시각. 시작 시각이 미정인 경우 <see cref="DateTime.MinValue"/>를 반환합니다.
        /// </summary>
        public virtual DateTime Start {
            get { return (Count > 0) ? _periods.Min(p => p.Start) : TimeSpec.MinPeriodTime; }
            set {
                if(Count > 0)
                    Move(value - Start);
            }
        }

        /// <summary>
        /// 기간의 완료 시각. 완료 시각이 미정인 경우 <see cref="DateTime.MaxValue"/>를 반환합니다.
        /// </summary>
        public virtual DateTime End {
            get { return (Count > 0) ? _periods.Max(p => p.End) : TimeSpec.MaxPeriodTime; }
            set {
                if(Count > 0)
                    Move(value - End);
            }
        }

        /// <summary>
        /// 기간을 TimeSpan으료 표현
        /// </summary>
        public virtual TimeSpan Duration {
            get { return HasPeriod ? (End - Start) : TimeSpec.MaxPeriodDuration; }
        }

        public virtual string DurationDescription {
            get { return TimeFormatter.Instance.GetDuration(Duration, DurationFormatKind.Detailed); }
        }

        /// <summary>
        /// 기간의 시작 시각. null 인 경우 시작 시각을 정하지 않은 것입니다.
        /// </summary>
        public virtual DateTime? StartAsNullable {
            get { return HasStart ? _periods.Min(p => p.Start) : (DateTime?)null; }
        }

        /// <summary>
        /// 기간의 완료 시각. null인 경우 완료 시각을 정하지 않은 것입니다.
        /// </summary>
        public virtual DateTime? EndAsNullable {
            get { return HasEnd ? _periods.Max(p => p.End) : (DateTime?)null; }
        }

        /// <summary>
        /// 시작 시각이 지정되었는가?
        /// </summary>
        public virtual bool HasStart {
            get { return Start != TimeSpec.MinPeriodTime; }
        }

        /// <summary>
        /// 완료 시각이 지정되었는가?
        /// </summary>
        public virtual bool HasEnd {
            get { return End != TimeSpec.MaxPeriodTime; }
        }

        /// <summary>
        /// 정해진 기간이 있는지 표시합니다.
        /// </summary>
        public virtual bool HasPeriod {
            get { return HasStart && HasEnd; }
        }

        /// <summary>
        /// 시작 시각과 완료 시각의 값이 같은가? 
        /// </summary>
        public virtual bool IsMoment {
            get { return HasPeriod && Equals(Start, End); }
        }

        /// <summary>
        /// 시작 시각도 없고, 완료 시각도 없는 구간 (전체 구간)
        /// </summary>
        public virtual bool IsAnytime {
            get { return !HasStart && !HasEnd; }
        }

        /// <summary>
        /// 기간을 설정합니다.
        /// </summary>
        /// <param name="newStart"></param>
        /// <param name="newEnd"></param>
        public virtual void Setup(DateTime? newStart, DateTime? newEnd) {
            throw new NotSupportedException("TimePeriodContainer에서는 Setup() 메소드를 지원하지 않습니다.");
        }

        /// <summary>
        /// 현재 기간에서 오프셋만큼 Shift 한 <see cref="ITimeRange"/>정보를 반환합니다.
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public virtual ITimePeriod Copy(TimeSpan offset) {
            throw new NotSupportedException("TimePeriodContainer에서는 Copy() 메소드를 지원하지 않습니다.");
        }

        /// <summary>
        /// 기간을 오프셋만큼 이동
        /// </summary>
        /// <param name="offset"></param>
        public virtual void Move(TimeSpan offset) {
            if(offset == TimeSpan.Zero)
                return;

            _periods.RunEach(p => p.Move(offset));
        }

        /// <summary>
        /// 두 기간이 같은 기간을 나타내는지 검사합니다
        /// </summary>
        /// <param name="other">비교할 대상</param>
        /// <returns></returns>
        public virtual bool IsSamePeriod(ITimePeriod other) {
            other.ShouldNotBeNull("other");
            return Equals(Start, other.Start) && Equals(End, other.End);
        }

        /// <summary>
        /// 지정된 시각이 기간에 속하는지 검사합니다.
        /// </summary>
        /// <param name="moment">검사할 일자</param>
        /// <returns></returns>
        public virtual bool HasInside(DateTime moment) {
            return TimeTool.HasInside(this, moment);
        }

        /// <summary>
        /// 지정한 기간이 현 기간 내에 속하는지 검사합니다.
        /// </summary>
        /// <param name="other">대상 기간</param>
        /// <returns></returns>
        public virtual bool HasInside(ITimePeriod other) {
            return TimeTool.HasInside(this, other);
        }

        /// <summary>
        /// 지정한 기간이 현 기간과 겹치는 부분이 있는지 검사합니다.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public virtual bool IntersectsWith(ITimePeriod other) {
            return TimeTool.IntersectsWith(this, other);
        }

        /// <summary>
        /// 지정한 기간이 현 기간과 겹치는 부분이 있는지 검사합니다.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public virtual bool OverlapsWith(ITimePeriod other) {
            return TimeTool.OverlapsWith(this, other);
        }

        /// <summary>
        /// Container의 모든 항목을 삭제합니다.
        /// </summary>
        public virtual void Reset() {
            _periods.Clear();
        }

        /// <summary>
        /// 다른 TimePeriod와의 관계를 판단합니다.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public virtual PeriodRelation GetRelation(ITimePeriod other) {
            return TimeTool.GetReleation(this, other);
        }

        /// <summary>
        /// TimePeriod의 설명을 문자열로 반환합니다.
        /// </summary>
        /// <returns></returns>
        public string GetDescription(ITimeFormatter formatter = null) {
            return Format(formatter);
        }

        /// <summary>
        /// <paramref name="formatter"/>로 포맷한 문자열을 반환합니다.
        /// </summary>
        /// <param name="formatter"></param>
        /// <returns></returns>
        public virtual string Format(ITimeFormatter formatter = null) {
            return (formatter ?? TimeFormatter.Instance).GetCollectionPeriod(Count, Start, End, Duration);
        }

        /// <summary>
        /// 두 기간의 겹치는 기간을 반환합니다.
        /// </summary>
        /// <param name="other">대상 기간</param>
        /// <returns>겹치는 기간</returns>
        public ITimePeriod GetIntersection(ITimePeriod other) {
            return TimeTool.GetIntersectionRange(this, other);
        }

        /// <summary>
        /// 두 기간의 합집합 기간을 반환합니다.
        /// </summary>
        /// <param name="other">대상기간</param>
        /// <returns>합집합 기간</returns>
        public ITimePeriod GetUnion(ITimePeriod other) {
            return TimeTool.GetUnionRange(this, other);
        }

        /// <summary>
        /// <paramref name="item"/>과 같은 항목의 인덱스를 반환합니다. 없으면 -1 을 반환합니다.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual int IndexOf(ITimePeriod item) {
            return _periods.IndexOf(item);
        }

        /// <summary>
        /// <paramref name="item"/>을 <paramref name="index"/> 순서에 삽입합니다.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public virtual void Insert(int index, ITimePeriod item) {
            item.ShouldNotBeNull("item");
            _periods.Insert(index, item);
        }

        /// <summary>
        /// 지정한 인덱스에 해당하는 항목을 제거합니다.
        /// </summary>
        public virtual void RemoveAt(int index) {
            _periods.RemoveAt(index);
        }

        /// <summary>
        /// 지정한 인덱스에 있는 항목을 가져오거나 설정합니다.
        /// </summary>
        /// <returns>
        /// 지정한 인덱스의 항목입니다.
        /// </returns>
        /// <param name="index">가져오거나 설정할 항목의 인덱스(0부터 시작)입니다.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/>가 <see cref="T:System.Collections.Generic.IList`1"/>의 유효한 인덱스가 아닌 경우</exception>
        /// <exception cref="T:System.NotSupportedException">속성이 설정되어 있으며 <see cref="T:System.Collections.Generic.IList`1"/>가 읽기 전용인 경우</exception>
        public virtual ITimePeriod this[int index] {
            get { return _periods[index]; }
            set {
                value.ShouldNotBeNull("value");
                _periods[index] = value;
            }
        }

        public virtual bool Equals(ITimePeriod other) {
            return (other != null) && IsSamePeriod(other);
        }

        public override bool Equals(object obj) {
            return (obj != null) && (obj is ITimePeriod) && Equals((ITimePeriod)obj);
        }

        public int CompareTo(ITimePeriod other) {
            return Start.CompareTo(other.Start);
        }

        public int CompareTo(object obj) {
            return CompareTo((ITimePeriod)obj);
        }

        public override int GetHashCode() {
            return HashTool.Compute(Start, End, IsReadOnly);
        }

        public override string ToString() {
            return GetDescription();
        }
    }
}