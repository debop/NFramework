using System;

namespace NSoft.NFramework.TimePeriods {
    public static partial class TimeTool {
        /// <summary>
        ///  두 일자 중 최소 일자를 구한다.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static DateTime Min(DateTime a, DateTime b) {
            return (a > b) ? b : a;
        }

        /// <summary>
        /// 두 일자 중 최대 일자를 구한다.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static DateTime Max(DateTime a, DateTime b) {
            return (a > b) ? a : b;
        }

        /// <summary>
        /// 두 일자 중 최소 일자를 구한다.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static DateTime? Min(DateTime? a, DateTime? b) {
            if(a.HasValue && b.HasValue)
                return Min(a.Value, b.Value);

            if(a.HasValue)
                return a;

            if(b.HasValue)
                return b;

            return null;
        }

        /// <summary>
        /// 두 일자 중 최대 일자를 구한다.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static DateTime? Max(DateTime? a, DateTime? b) {
            if(a.HasValue && b.HasValue)
                return Max(a.Value, b.Value);

            if(a.HasValue)
                return a;

            if(b.HasValue)
                return b;

            return null;
        }

        /// <summary>
        /// 두 시간 간격 중 작은 값을 반환한다.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static TimeSpan Min(TimeSpan a, TimeSpan b) {
            return (a < b) ? a : b;
        }

        /// <summary>
        /// 두 시간 간격 중 큰 값을 반환한다.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static TimeSpan Max(TimeSpan a, TimeSpan b) {
            return (a < b) ? b : a;
        }

        /// <summary>
        /// 기간의 시작 시각과 완료 시각을 순서대로 정렬합니다.
        /// </summary>
        /// <param name="start">시작 시각</param>
        /// <param name="end">완료 시각</param>
        public static void AdjustPeriod(ref DateTime start, ref DateTime end) {
            if(start > end) {
                DateTime temp = start;
                start = end;
                end = temp;
            }
        }

        /// <summary>
        /// 기간의 시작 시각과 완료 시각을 순서대로 정렬합니다.
        /// </summary>
        /// <param name="start">시작 시각</param>
        /// <param name="end">완료 시각</param>
        public static void AdjustPeriod(ref DateTime? start, ref DateTime? end) {
            if(start.HasValue && end.HasValue) {
                if(start.Value > end.Value) {
                    DateTime temp = start.Value;
                    start = end.Value;
                    end = temp;
                }
            }
        }

        /// <summary>
        /// <paramref name="duration"/>의 부호가 Minus(-) 이면, 
        /// <paramref name="start"/> 시간을 <paramref name="duration"/>만큼 앞으로 이동하고, <paramref name="duration"/> 값을 양수로 변경한다.
        /// </summary>
        /// <param name="start">시작 시각</param>
        /// <param name="duration">시간 간격</param>
        public static void AdjustPeriod(ref DateTime start, ref TimeSpan duration) {
            if(duration < TimeSpan.Zero) {
                start = start.Add(duration);
                duration = duration.Negate();
            }
        }
    }
}