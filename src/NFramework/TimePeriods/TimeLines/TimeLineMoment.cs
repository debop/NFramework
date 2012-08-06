using System;
using System.Linq;

namespace NSoft.NFramework.TimePeriods.TimeLines {
    /// <summary>
    /// 특정 기준 시각에 대한 필터링을 수행합니다.
    /// </summary>
    [Serializable]
    public class TimeLineMoment : ITimeLineMoment {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        public TimeLineMoment(DateTime moment) {
            Moment = moment;
        }

        /// <summary>
        /// 기준 시각
        /// </summary>
        public DateTime Moment { get; private set; }

        private ITimePeriodCollection _periods;

        /// <summary>
        /// 기간 컬렉션
        /// </summary>
        public ITimePeriodCollection Periods {
            get { return _periods ?? (_periods = new TimePeriodCollection()); }
            protected set { _periods = value; }
        }

        /// <summary>
        /// <see cref="Periods"/>중에 기간의 시작 시각과 <see cref="Moment"/>가 같은 기간의 수
        /// </summary>
        public int StartCount {
            get { return Periods.Count(p => p.Start == Moment); }
        }

        /// <summary>
        /// <see cref="Periods"/>중에 기간 완료 시각과 <see cref="Moment"/>가 같은 기간의 수
        /// </summary>
        public int EndCount {
            get { return Periods.Count(p => p.End == Moment); }
        }

        public override string ToString() {
            return string.Format("TimeLineMoment# Moment=[{0}], [{1}/{2}]", Moment, StartCount, EndCount);
        }
    }
}