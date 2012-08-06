using System;

namespace NSoft.NFramework.TimePeriods {
    /// <summary>
    /// 기본 Clock
    /// </summary>
    [Serializable]
    public abstract class AbstractClock : IClock {
        private DateTime? _now;

        protected AbstractClock() {}

        protected AbstractClock(DateTime now) {
            _now = now;
        }

        /// <summary>
        /// 현재 시각
        /// </summary>
        public virtual DateTime Now {
            get { return (_now ?? (_now = DateTime.Now)).Value; }
            protected set { _now = value; }
        }

        /// <summary>
        /// 오늘 (현재 시각의 날짜부분만)
        /// </summary>
        public DateTime Today {
            get { return Now.Date; }
        }

        /// <summary>
        /// 현재 시각의 시간부분만
        /// </summary>
        public TimeSpan TimeOfDay {
            get { return Now.TimeOfDay; }
        }
    }
}