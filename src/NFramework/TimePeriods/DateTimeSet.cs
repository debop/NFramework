using System;
using System.Collections.Generic;
using NSoft.NFramework.Collections.PowerCollections;

namespace NSoft.NFramework.TimePeriods {
    /// <summary>
    /// <see cref="DateTime"/>의 정렬을 수행하여 저장한 컬렉션입니다. (중복은 허용되지 않습니다)
    /// </summary>
    [Serializable]
    public class DateTimeSet : OrderedSet<DateTime>, IDateTimeSet {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 기본 생성자
        /// </summary>
        public DateTimeSet() {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="moments">추가할 요소들</param>
        public DateTimeSet(IEnumerable<DateTime> moments) : base(moments) {}

        /// <summary>
        /// Readonly indexer
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public new DateTime this[int index] {
            get { return base[index]; }
        }

        /// <summary>
        /// 최소값, 요소가 없으면 null을 반환한다.
        /// </summary>
        public virtual DateTime? Min {
            get { return IsEmpty ? (DateTime?)null : this[0]; }
        }

        /// <summary>
        /// 최대값, 요소가 없으면 null을 반환한다.
        /// </summary>
        public virtual DateTime? Max {
            get { return IsEmpty ? (DateTime?)null : this[Count - 1]; }
        }

        /// <summary>
        /// Min~Max의 기간을 나타낸다. 둘 중 하나라도 null이면 null을 반환한다.
        /// </summary>
        public virtual TimeSpan? Duration {
            get {
                if(IsEmpty)
                    return (TimeSpan?)null;

                var min = Min;
                var max = Max;
                return (min.HasValue && max.HasValue) ? max.Value.Subtract(min.Value) : (TimeSpan?)null;
            }
        }

        /// <summary>
        /// 요소가 없는 컬렉션인가?
        /// </summary>
        public virtual bool IsEmpty {
            get { return (Count == 0); }
        }

        /// <summary>
        /// 모든 요소가 같은 시각을 나타내는가?
        /// </summary>
        public virtual bool IsMoment {
            get {
                var duration = Duration;
                return (duration.HasValue) && (duration.Value == TimeSpan.Zero);
            }
        }

        /// <summary>
        /// 요소가 모든 시각을 나타내는가? <see cref="IDateTimeSet.Min"/>이  <see cref="DateTime.MinValue"/>이고, 
        /// <see cref="IDateTimeSet.Max"/>가 <see cref="DateTime.MaxValue"/>이다.
        /// </summary>
        public virtual bool IsAnytime {
            get {
                return Min.HasValue &&
                       Min.Value == TimeSpec.MinPeriodTime &&
                       Max.HasValue &&
                       Max.Value == TimeSpec.MaxPeriodTime;
            }
        }

        /// <summary>
        /// <paramref name="moment"/>를 요소로 추가합니다. 중복된 요소는 추가하지 않습니다.
        /// </summary>
        /// <param name="moment"></param>
        /// <returns></returns>
        public new bool Add(DateTime moment) {
            if(IsDebugEnabled)
                log.Debug("새로운 DateTime 요소를 추가합니다... moment=[{0}]", moment);

            return !Contains(moment) && base.Add(moment);
        }

        /// <summary>
        /// 지정된 컬렉션의 요소들을 모두 추가합니다.
        /// </summary>
        /// <param name="moments"></param>
        public void AddAll(IEnumerable<DateTime> moments) {
            AddMany(moments);
        }

        /// <summary>
        /// 순번에 해당하는 시각들의 Duration을 구합니다.
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public virtual IList<TimeSpan> GetDurations(int startIndex, int count) {
            startIndex.ShouldBePositiveOrZero("startIndex");
            count.ShouldBePositive("count");
            Guard.Assert(startIndex < Count,
                         "startIndex는 0 이상, List Count-1 미만이어야 합니다. startIndex=[{0}], Count=[{1}]", startIndex, Count);

            if(IsDebugEnabled)
                log.Debug("Duration들을 구합니다... startIndex=[{0}], count=[{1}]", startIndex, count);

            var endIndex = Math.Min(startIndex + count, Count - 1);
            var durations = new List<TimeSpan>();

            for(var i = startIndex; i < endIndex; i++) {
                durations.Add(this[i + 1] - this[i]);
            }

            return durations;
        }

        /// <summary>
        /// 지정된 시각의 바로 전의 시각을 찾습니다. 없으면 null을 반환합니다.
        /// </summary>
        /// <param name="moment"></param>
        /// <returns></returns>
        public virtual DateTime? FindPrevious(DateTime moment) {
            if(IsDebugEnabled)
                log.Debug("지정된 시각[{0}] 의 바로 전의 시각을 찾습니다. 없으면 null을 반환합니다.", moment);

            if(IsEmpty)
                return null;

            for(var i = Count - 1; i >= 0; i--)
                if(this[i] < moment)
                    return this[i];

            return null;
        }

        /// <summary>
        /// 지정된 시각의 바로 후의 시각을 찾습니다. 없으면 null을 반환합니다.
        /// </summary>
        /// <param name="moment"></param>
        /// <returns></returns>
        public virtual DateTime? FindNext(DateTime moment) {
            if(IsDebugEnabled)
                log.Debug("지정된 시각[{0}] 의 바로 후의 시각을 찾습니다. 없으면 null을 반환합니다.", moment);

            if(IsEmpty)
                return null;

            for(var i = 0; i < Count; i++)
                if(this[i] > moment)
                    return this[i];

            return null;
        }

        public override string ToString() {
            var min = Min;
            var max = Max;
            var duration = Duration;

            var hasRange = min.HasValue && max.HasValue && duration.HasValue;
            if(hasRange)
                return TimeFormatter.Instance.GetCollectionPeriod(Count, min.Value, max.Value, duration.Value);

            return TimeFormatter.Instance.GetCollection(Count);
        }
    }
}