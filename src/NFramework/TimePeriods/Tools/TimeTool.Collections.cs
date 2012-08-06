using System.Collections.Generic;
using System.Linq;

namespace NSoft.NFramework.TimePeriods {
    public static partial class TimeTool {
        /// <summary>
        /// <paramref name="sequence"/>를 <see cref="TimePeriodCollection"/>으로 변환합니다.
        /// </summary>
        public static ITimePeriodCollection ToTimePeriodCollection<T>(this IEnumerable<T> sequence) where T : ITimePeriod {
            return new TimePeriodCollection(sequence.Cast<ITimePeriod>());
        }
    }
}