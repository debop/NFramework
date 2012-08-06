namespace NSoft.NFramework.TimePeriods.TimeLines {
    /// <summary>
    /// <see cref="ITimePeriod"/> 컬렉션의 항목 기간에 포함되지 않는 빈 기간 (TimeGap) 을 계산해 줍니다.
    /// </summary>
    /// <typeparam name="T">기간을 나타내는 수형 (<see cref="ITimePeriod"/>의 자식 수형)</typeparam>
    public class TimeGapCalculator<T> where T : ITimePeriod {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 생성자
        /// </summary>
        public TimeGapCalculator() : this(null) {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="mapper">시작/완료 시각에 대한 매퍼</param>
        public TimeGapCalculator(ITimePeriodMapper mapper) {
            PeriodMapper = mapper;
        }

        public ITimePeriodMapper PeriodMapper { get; private set; }

        /// <summary>
        /// <paramref name="limits"/> 기간내에서 <paramref name="excludedPeriods"/> 컬렉션의 기간들을 제외한 기간(Gap)을 계산해 낸다.
        /// </summary>
        /// <param name="excludedPeriods">제외할 기간들을 나타냅니다.</param>
        /// <param name="limits">전체 범위를 나타냅니다 (null이면 무한대의 범위를 나타냅니다)</param>
        /// <returns></returns>
        public virtual ITimePeriodCollection GetGaps(ITimePeriodContainer excludedPeriods, ITimePeriod limits = null) {
            excludedPeriods.ShouldNotBeNull("periods");

            if(IsDebugEnabled)
                log.Debug("Period들의 Gap들을 계산합니다... excludedPeriods=[{0}], limits=[{1}]", excludedPeriods, limits);

            var timeLine = new TimeLine<T>(excludedPeriods, limits, PeriodMapper);

            return timeLine.CalcuateGaps();
        }
    }
}