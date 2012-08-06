using System;

namespace NSoft.NFramework.TimePeriods {
    // TODO: LinkedList 로 표현하는 것이 좋을 듯 한데...

    /// <summary>
    /// ITimePeriod 요소들을 LinkedList 방식으로 연속해서 나열하여 관리하는 컬렉션입니다.
    /// </summary>
    public interface ITimePeriodChain : ITimePeriodContainer {
        /// <summary>
        /// Chain의 시작 시각
        /// </summary>
        new DateTime Start { get; set; }

        /// <summary>
        /// Chain의 완료 시각
        /// </summary>
        new DateTime End { get; set; }

        /// <summary>
        /// 첫번째  TimePeriod
        /// </summary>
        ITimePeriod First { get; }

        /// <summary>
        /// 마지막 TimePeriod
        /// </summary>
        ITimePeriod Last { get; }
    }
}