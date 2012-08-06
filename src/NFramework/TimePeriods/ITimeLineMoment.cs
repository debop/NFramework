using System;

namespace NSoft.NFramework.TimePeriods {
    /// <summary>
    /// <see cref="ITimePeriod"/>의 컬렉션인 <see cref="Periods"/>를 가지며, <see cref="Moment"/>를 기준으로 선행 기간의 수와 후행 기간의 수를 파악합니다.
    /// </summary>
    public interface ITimeLineMoment {
        DateTime Moment { get; }

        ITimePeriodCollection Periods { get; }

        int StartCount { get; }

        int EndCount { get; }
    }
}