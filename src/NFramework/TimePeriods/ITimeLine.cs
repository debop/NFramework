namespace NSoft.NFramework.TimePeriods {
    /// <summary>
    /// <see cref="ITimePeriod"/>의 컬렉션인 <see cref="Periods"/>를 가지며, 이를 통해 여러 기간에 대한 Union, Intersection, Gap 등을 구할 수 있도록 합니다.
    /// </summary>
    public interface ITimeLine {
        ITimePeriodContainer Periods { get; }

        ITimePeriod Limits { get; }

        ITimePeriodMapper PeriodMapper { get; }

        ITimePeriodCollection CombinePeriods();

        ITimePeriodCollection IntersectPeriods();

        ITimePeriodCollection CalcuateGaps();
    }
}