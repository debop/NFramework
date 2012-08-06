using System;
using System.Collections.Generic;
using System.Linq;

namespace NSoft.NFramework.TimePeriods {
    public static partial class TimeTool {
        /// <summary>
        /// 기간의 시작 일자가 완료 일자보다 선행되어야 합니다.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public static void AssertValidPeriod(DateTime? start, DateTime? end) {
            if(start.HasValue && end.HasValue)
                Guard.Assert(start.Value <= end.Value, "시작 시각이 완료 시각보다 이전 시각이어야 합니다. Start=[{0}], End=[{1}]", start, end);
        }

        /// <summary>
        /// <paramref name="period"/>가 수정 가능한가 를 검사합니다. (읽기 전용이면 <see cref="InvalidOperationException"/> 예외를 발생시킵니다)
        /// </summary>
        /// <param name="period"></param>
        public static void AssertMutable(this ITimePeriod period) {
            Guard.Assert(period.IsReadOnly == false, "TimePeriod가 읽기 전용이면 안됩니다. period=[{0}]", period);
        }

        /// <summary>
        /// <paramref name="left"/>와 <paramref name="right"/> 시퀀스의 요소들이 모두 같은 지를 판단합니다.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool AllItemsAreEqual(this IEnumerable<ITimePeriod> left, IEnumerable<ITimePeriod> right) {
            if(left.Count() != right.Count())
                return false;

            var itemCount = left.Count();

            for(var i = 0; i < itemCount; i++) {
                if(Equals(left.ElementAt(i), right.ElementAt(i)) == false)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 요일이 주중인가?
        /// </summary>
        /// <param name="dayOfWeek"></param>
        /// <returns></returns>
        public static bool IsWeekDay(this DayOfWeek dayOfWeek) {
            return (IsWeekEnd(dayOfWeek) == false);
        }

        /// <summary>
        /// 요일이 주말인가?
        /// </summary>
        /// <param name="dayOfWeek"></param>
        /// <returns></returns>
        public static bool IsWeekEnd(this DayOfWeek dayOfWeek) {
            return (dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday);
        }
    }
}