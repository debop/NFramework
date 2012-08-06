using System;
using NSoft.NFramework.TimePeriods.TimeLines;

namespace NSoft.NFramework.TimePeriods {
    /// <summary>
    /// <see cref="ITimePeriod"/> 기간들을 결합하는 클래스입니다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class TimePeriodCombiner<T> where T : ITimePeriod {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// 기본생성자
        /// </summary>
        public TimePeriodCombiner() : this(null) {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="mapper">시각 Mapper</param>
        public TimePeriodCombiner(ITimePeriodMapper mapper) {
            PeriodMapper = mapper;
        }

        /// <summary>
        /// 시각을 Offset을 이용하여 Map, UnMap을 수행합니다. (<see cref="TimeCalendar"/>)
        /// </summary>
        public ITimePeriodMapper PeriodMapper { get; private set; }

        /// <summary>
        /// <paramref name="periods"/>들을 모아 <see cref="ITimePeriodCollection"/>으로 만듭니다.
        /// </summary>
        /// <param name="periods"></param>
        /// <returns></returns>
        public virtual ITimePeriodCollection CombinePeriods(params ITimePeriod[] periods) {
            return new TimeLine<T>(new TimePeriodCollection(periods), PeriodMapper).CombinePeriods();
        }

        /// <summary>
        /// <paramref name="periods"/>의 기간들을 결합하여, <see cref="ITimePeriodCollection"/>을 생성합니다.
        /// </summary>
        /// <param name="periods"></param>
        /// <returns></returns>
        public virtual ITimePeriodCollection CombinePeriods(ITimePeriodContainer periods) {
            periods.ShouldNotBeNull("periods");

            return new TimeLine<T>(periods, PeriodMapper).CombinePeriods();
        }
    }
}