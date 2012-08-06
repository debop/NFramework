using System.Collections.Generic;
using System.Diagnostics;

namespace NSoft.NFramework.Diagnostics {
    /// <summary>
    /// PerformanceCounter 제공자입니다.
    /// </summary>
    public interface IPerformanceCounterProvider {
        /// <summary>
        /// PerformanceCounter가 속한 Category 명
        /// </summary>
        string CategoryName { get; }

        /// <summary>
        /// 제공되는 PerformanceCounter의 컬렉션입니다.
        /// </summary>
        IList<PerformanceCounter> PerformanceCounters { get; }

        /// <summary>
        /// 제공하는 PerformanceCounter 들의 CountName을 반환합니다.
        /// </summary>
        /// <returns></returns>
        string[] GetCounterNames();
    }
}