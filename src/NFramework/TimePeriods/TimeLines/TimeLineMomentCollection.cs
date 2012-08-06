using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NSoft.NFramework.Collections.PowerCollections;

namespace NSoft.NFramework.TimePeriods.TimeLines {
    /// <summary>
    /// <see cref="ITimePeriod"/>를 시작 시각, 완료 시각을 키로 가지고, 
    /// <see cref="ITimePeriod"/>를 Value로 가지는 MultiMap{DateTime, ITimePeriod} 을 생성합니다. <b>단 시각(DateTime)으로 정렬됩니다.</b>
    /// </summary>
    [Serializable]
    public class TimeLineMomentCollection : ITimeLineMomentCollection {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 시각으로 정렬을 수행하는 Delegate
        /// </summary>
        private static readonly Comparison<ITimeLineMoment> DefaultComparison = (left, right) => left.Moment.CompareTo(right.Moment);

        private readonly object _syncLock = new object();

        private readonly OrderedBag<ITimeLineMoment> _timeLineMoments = new OrderedBag<ITimeLineMoment>(DefaultComparison);

        public int Count {
            get { return _timeLineMoments.Count; }
        }

        public bool IsEmpty {
            get { return Count == 0; }
        }

        public ITimeLineMoment Min {
            get { return (IsEmpty == false) ? _timeLineMoments[0] : null; }
        }

        public ITimeLineMoment Max {
            get { return (IsEmpty == false) ? _timeLineMoments[Count - 1] : null; }
        }

        public ITimeLineMoment this[int index] {
            get { return _timeLineMoments[index]; }
        }

        public void Add(ITimePeriod period) {
            period.ShouldNotBeNull("period");

            AddPeriod(period.Start, period);
            AddPeriod(period.End, period);
        }

        public void AddAll(IEnumerable<ITimePeriod> periods) {
            foreach(var period in periods.Where(p => p != null)) {
                AddPeriod(period.Start, period);
                AddPeriod(period.End, period);
            }
        }

        public void Remove(ITimePeriod period) {
            if(period != null) {
                RemovePeriod(period.Start, period);
                RemovePeriod(period.End, period);
            }
        }

        /// <summary>
        /// <paramref name="moment"/>와 같은 값을 가진 <see cref="TimeLineMoment"/>를 찾습니다.
        /// </summary>
        /// <param name="moment">찾고자하는 moment</param>
        /// <returns></returns>
        public ITimeLineMoment Find(DateTime moment) {
            return _timeLineMoments.FirstOrDefault(tlm => Equals(tlm.Moment, moment));
        }

        /// <summary>
        /// <paramref name="moment"/>와 같은 값을 가진 <see cref="TimeLineMoment"/>가 존재하는지 파악합니다.
        /// </summary>
        /// <param name="moment">찾고자하는 moment</param>
        /// <returns></returns>
        public bool Contains(DateTime moment) {
            return Find(moment) != null;
        }

        public IEnumerator<ITimeLineMoment> GetEnumerator() {
            return _timeLineMoments.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        /// <summary>
        /// <paramref name="moment"/>를 나타내는 TimeLineMoment를 키로 하고, Value로 <paramref name="period"/> 를 추가합니다.
        /// </summary>
        protected void AddPeriod(DateTime moment, ITimePeriod period) {
            if(IsDebugEnabled)
                log.Debug("TimeLineMoment를 추가합니다... moment=[{0}], period=[{1}]", moment, period);

            lock(_syncLock) {
                var timeLineMoment = Find(moment);

                if(timeLineMoment == null) {
                    timeLineMoment = new TimeLineMoment(moment);
                    _timeLineMoments.Add(timeLineMoment);
                }
                timeLineMoment.Periods.Add(period);

                if(IsDebugEnabled)
                    log.Debug("TimeLineMoment를 추가했습니다!!! timeLineMoment=[{0}]", timeLineMoment);
            }
        }

        /// <summary>
        /// <paramref name="moment"/>를 나타내는 TimeLineMoment를 키로 하고, Value로 <paramref name="period"/> 를 삭제합니다.
        /// </summary>
        protected void RemovePeriod(DateTime moment, ITimePeriod period) {
            if(IsDebugEnabled)
                log.Debug("TimeLineMoment를 제거합니다... moment=[{0}], period=[{1}]", moment, period);

            lock(_syncLock) {
                var timeLineMoment = Find(moment);

                if(timeLineMoment != null && timeLineMoment.Periods.Contains(period)) {
                    timeLineMoment.Periods.Remove(period);

                    if(timeLineMoment.Periods.Count == 0)
                        _timeLineMoments.Remove(timeLineMoment);

                    if(IsDebugEnabled)
                        log.Debug("TimeLineMoment를 제거했습니다!!! timeLineMoment=[{0}]", timeLineMoment);
                }
            }
        }
    }
}