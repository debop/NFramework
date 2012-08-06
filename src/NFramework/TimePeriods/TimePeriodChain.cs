using System;
using System.Collections.Generic;

namespace NSoft.NFramework.TimePeriods {
    /// <summary>
    /// ITimePeriod를 시간의 흐름 순으로 Chain (Linked List) 형태로 표현한 클래스입니다.
    /// </summary>
    [Serializable]
    public class TimePeriodChain : TimePeriodContainer, ITimePeriodChain {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 기본 생성자
        /// </summary>
        public TimePeriodChain() {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="periods">요소들</param>
        public TimePeriodChain(IEnumerable<ITimePeriod> periods) {
            if(periods != null)
                AddAll(periods);
        }

        /// <summary>
        /// Chain 범위의 시작 시각
        /// </summary>
        public new DateTime Start {
            get { return (First != null) ? First.Start : TimeSpec.MinPeriodTime; }
            set {
                if(First != null)
                    Move(value - Start);
            }
        }

        /// <summary>
        /// Chain 범위의 완료 시각
        /// </summary>
        public new DateTime End {
            get { return (Last != null) ? Last.End : TimeSpec.MaxPeriodTime; }
            set {
                if(Last != null)
                    Move(value - End);
            }
        }

        /// <summary>
        /// Period Chain의 첫번째 TimePeriod (없으면 null 반환)
        /// </summary>
        public ITimePeriod First {
            get { return (Count > 0) ? _periods[0] : null; }
        }

        /// <summary>
        /// Period Chain의 마지막 TimePeriod (없으면 null 반환)
        /// </summary>
        public ITimePeriod Last {
            get { return (_periods.Count > 0) ? _periods[_periods.Count - 1] : null; }
        }

        /// <summary>
        /// 인덱서
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public override ITimePeriod this[int index] {
            get { return base[index]; }
            set {
                Guard.Assert<ArgumentOutOfRangeException>(index >= 0 && index < Count, "인덱스 범위가 잘못되었습니다. Count=[{0}], index=[{1}]",
                                                          Count, index);
                RemoveAt(index);
                Insert(index, value);
            }
        }

        /// <summary>
        /// 새로운 <paramref name="item"/>을 Chain의 제일 끝에 붙여 넣습니다. <paramref name="item"/>의 기간이 변경됩니다.
        /// </summary>
        /// <param name="item"></param>
        public override void Add(ITimePeriod item) {
            item.ShouldNotBeNull("item");
            item.AssertMutable();

            ITimePeriod last = Last;

            if(last != null) {
                AssertSpaceAfter(last.End, item.Duration);
                item.Setup(last.End, last.End.Add(item.Duration));
            }

            if(IsDebugEnabled)
                log.Debug("Period를 Chain의 끝에 추가합니다. item=[{0}]", item);

            _periods.Add(item);
        }

        /// <summary>
        /// 기간이 존재하는 (HasPeriod가 true인) <see cref="ITimePeriod"/>들을 추가합니다.
        /// </summary>
        /// <param name="periods"></param>
        public override void AddAll(IEnumerable<ITimePeriod> periods) {
            periods.ShouldNotBeNull("periods");

            foreach(var period in periods)
                Add(period);
        }

        /// <summary>
        /// <see cref="ITimePeriod"/>의 Chain의 <paramref name="index"/>번째에 <paramref name="item"/>을 삽입합니다. 선행 Period와 후행 Period의 기간 값이 조정됩니다.
        /// </summary>
        /// <param name="index">추가할 위치</param>
        /// <param name="item">추가할 기간</param>
        public override void Insert(int index, ITimePeriod item) {
            item.ShouldNotBeNull("item");
            Guard.Assert<ArgumentOutOfRangeException>(index >= 0 && index <= Count, "인덱스 범위가 잘못되었습니다. Count=[{0}], index=[{1}]", Count,
                                                      index);

            item.AssertMutable();

            if(IsDebugEnabled)
                log.Debug("Chain의 인덱스[{0}]에 새로운 요소[{1}]를 삽입합니다...", index, item);

            var itemDuration = item.Duration;
            ITimePeriod previousItem = null;
            ITimePeriod nextItem = null;

            if(Count > 0) {
                if(index > 0) {
                    previousItem = this[index - 1];
                    AssertSpaceAfter(End, itemDuration);
                }
                if(index < Count - 1) {
                    nextItem = this[index];
                    AssertSpaceBefore(Start, itemDuration);
                }
            }

            _periods.Insert(index, item);

            if(previousItem != null) {
                if(IsDebugEnabled)
                    log.Debug("선행 Period에 기초하여 삽입한 Period와 후행 Period들의 시간을 조정합니다...");

                item.Setup(previousItem.End, previousItem.End.Add(itemDuration));

                for(int i = index + 1; i < Count; i++) {
                    var startTime = this[i].Start.Add(itemDuration);
                    this[i].Setup(startTime, startTime.Add(this[i].Duration));
                }
            }

            if(nextItem != null) {
                if(IsDebugEnabled)
                    log.Debug("후행 Period에 기초하여 삽입한 Period와 선행 Period들의 시간을 조정합니다...");

                var nextStart = nextItem.Start.Subtract(itemDuration);
                item.Setup(nextStart, nextStart.Add(itemDuration));

                for(var i = 0; i < index - 1; i++) {
                    nextStart = this[i].Start.Subtract(itemDuration);
                    this[i].Setup(nextStart, nextStart.Add(this[i].Duration));
                }
            }
        }

        /// <summary>
        /// TimePeriodChain에서 요소 <paramref name="item"/>을 제거합니다. (제거된 후의 후속 Period들의 시간이 조정됩니다)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public override bool Remove(ITimePeriod item) {
            item.ShouldNotBeNull("item");

            if(Count <= 0)
                return false;

            if(IsDebugEnabled)
                log.Debug("요소[{0}] 를 제거하려고 합니다...", item);

            var itemDuration = item.Duration;
            var index = IndexOf(item);

            ITimePeriod next = null;
            if(itemDuration > TimeSpan.Zero && index > 0 && index < Count - 1)
                next = this[index];

            var removed = _periods.Remove(item);

            if(removed && next != null) {
                if(IsDebugEnabled)
                    log.Debug("요소[{0}]를 제거하고, Chain의 후속 Period 들의 기간을 조정합니다...", item);

                for(int i = index; i < Count; i++) {
                    var start = this[i].Start.Subtract(itemDuration);
                    this[i].Setup(start, start.Add(this[i].Duration));
                }
            }

            if(IsDebugEnabled)
                log.Debug("요소[{0}] 를 제거 결과=[{1}]", item, removed);

            return removed;
        }

        /// <summary>
        /// TimePeriodChain의 <paramref name="index"/> 번째 요소를 제거합니다.
        /// </summary>
        /// <param name="index"></param>
        public override void RemoveAt(int index) {
            if(IsDebugEnabled)
                log.Debug("Index=[{0}] 요소를 제거합니다.", index);

            index.ShouldBeInRange(0, Count, "index");

            Remove(_periods[index]);
        }

        /// <summary>
        /// <paramref name="moment"/> 이전에 <paramref name="duration"/> 만큼의 시간적 공간이 있는지 여부 (새로운 기간을 추가하기 위해서는 공간이 필요합니다)
        /// </summary>
        /// <param name="moment"></param>
        /// <param name="duration"></param>
        protected void AssertSpaceBefore(DateTime moment, TimeSpan duration) {
            var hasSpace = moment != TimeSpec.MinPeriodTime;

            if(hasSpace) {
                var remaining = moment - TimeSpec.MinPeriodTime;
                hasSpace = duration <= remaining;
            }
            Guard.Assert(hasSpace, "duration [{0}] is out of range.", duration);
        }

        /// <summary>
        /// <paramref name="moment"/> 이후에 <paramref name="duration"/> 만큼의 시간적 공간이 있는지 여부 (새로운 기간을 추가하기 위해서는 공간이 필요합니다)
        /// </summary>
        /// <param name="moment">검사할 시각</param>
        /// <param name="duration">필요한 시간 간격</param>
        protected void AssertSpaceAfter(DateTime moment, TimeSpan duration) {
            var hasSpace = moment != TimeSpec.MaxPeriodTime;

            if(hasSpace) {
                var remaining = TimeSpec.MaxPeriodTime - moment;
                hasSpace = duration <= remaining;
            }

            Guard.Assert(hasSpace, "duration [{0}] 은 범위를 벗어났습니다.", duration);
        }
    }
}