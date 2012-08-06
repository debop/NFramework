using System;

namespace NSoft.NFramework.TimePeriods {
    /// <summary>
    ///  참고 : http://stackoverflow.com/questions/43711/whats-a-good-way-to-overwrite-datetime-now-during-testing
    /// </summary>
    public interface IClock {
        /// <summary>
        /// 현재 시각
        /// </summary>
        DateTime Now { get; }

        /// <summary>
        /// 오늘 (현재 시각의 날짜부분만)
        /// </summary>
        DateTime Today { get; }

        /// <summary>
        /// 현재 시각의 시간부분만
        /// </summary>
        TimeSpan TimeOfDay { get; }
    }
}