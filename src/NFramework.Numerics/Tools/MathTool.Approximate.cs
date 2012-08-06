using System;
using System.Collections.Generic;
using System.Linq;

namespace NSoft.NFramework.Numerics {
    /// <summary>
    /// Mathematical Methods
    /// </summary>
    public static partial class MathTool {
        /// <summary>
        /// 근사치 비교
        /// </summary>
        /// <param name="a">비교 값</param>
        /// <param name="b">비교 대상 값</param>
        /// <param name="epsilon">허용오차</param>
        /// <returns>허용오차 내에서 같은 값인가?</returns>
        public static bool ApproximateEqual(this double a, double b, double epsilon = Epsilon) {
            return (Math.Abs(a - b) < Math.Abs(epsilon));
        }

        /// <summary>
        /// 근사치 비교
        /// </summary>
        /// <param name="a">비교 값</param>
        /// <param name="b">비교 대상 값</param>
        /// <param name="epsilon">허용오차</param>
        /// <returns>허용오차 내에서 같은 값인가?</returns>
        public static bool ApproximateEqual(this double? a, double? b, double epsilon = Epsilon) {
            if(a.HasValue && b.HasValue)
                return ApproximateEqual(a.Value, b.Value, epsilon);

            if(!a.HasValue && !b.HasValue)
                return true;

            return false;
        }

        /// <summary>
        /// 근사치 비교
        /// </summary>
        /// <param name="a">비교 값</param>
        /// <param name="b">비교 대상 값</param>
        /// <param name="epsilon">허용오차</param>
        /// <returns>허용오차 내에서 같은 값인가?</returns>
        public static bool ApproximateEqual(this float a, float b, float epsilon = (float)Epsilon) {
            return (Math.Abs(a - b) < Math.Abs(epsilon));
        }

        /// <summary>
        /// 근사치 비교
        /// </summary>
        /// <param name="a">비교 값</param>
        /// <param name="b">비교 대상 값</param>
        /// <param name="epsilon">허용오차</param>
        /// <returns>허용오차 내에서 같은 값인가?</returns>
        public static bool ApproximateEqual(this float? a, float? b, float epsilon = (float)Epsilon) {
            if(a.HasValue && b.HasValue)
                return ApproximateEqual(a.Value, b.Value, epsilon);
            if(!a.HasValue && !b.HasValue)
                return true;

            return false;
        }

        /// <summary>
        /// 근사치 비교
        /// </summary>
        /// <param name="a">비교 값</param>
        /// <param name="b">비교 대상 값</param>
        /// <param name="epsilon">허용오차</param>
        /// <returns>허용오차 내에서 같은 값인가?</returns>
        public static bool ApproximateEqual(this decimal a, decimal b, decimal epsilon = (decimal)Epsilon) {
            return (Math.Abs((double)a - (double)b) < Math.Abs((double)epsilon));
        }

        /// <summary>
        /// 근사치 비교
        /// </summary>
        /// <param name="a">비교 값</param>
        /// <param name="b">비교 대상 값</param>
        /// <param name="epsilon">허용오차</param>
        /// <returns>허용오차 내에서 같은 값인가?</returns>
        public static bool ApproximateEqual(this decimal? a, decimal? b, decimal epsilon = (decimal)Epsilon) {
            if(a.HasValue && b.HasValue)
                return ApproximateEqual(a.Value, b.Value, epsilon);
            if(!a.HasValue && !b.HasValue)
                return true;

            return false;
        }

        /// <summary>
        /// 시퀀스 요소중 selector와 허용오차 내의 근사치에 해당하는 요소만 필터링해서 반환합니다.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="check"></param>
        /// <param name="epsilon"></param>
        /// <returns></returns>
        public static IEnumerable<double> Approximate(this IEnumerable<double> source, double check, double epsilon = Epsilon) {
            return source.Where(x => ApproximateEqual(x, check, epsilon));
        }

        /// <summary>
        /// 시퀀스 요소중 검사값과 허용오차범위 내에 있는 근사치를 가진 요소만 반환한다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">시퀀스</param>
        /// <param name="check">검사값</param>
        /// <param name="selector">시퀀스에서 비교할 요소를 선택하는 함수</param>
        /// <returns></returns>
        public static IEnumerable<double> Approximate<T>(this IEnumerable<T> source, double check, Func<T, double> selector) {
            return source.Select(selector).Where(x => x.ApproximateEqual(check));
        }

        /// <summary>
        /// 시퀀스 요소중 검사값과 허용오차범위 내에 있는 근사치를 가진 요소만 반환한다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">시퀀스</param>
        /// <param name="check">검사값</param>
        /// <param name="epsilon">허용오차</param>
        /// <param name="selector">시퀀스에서 비교할 요소를 선택하는 함수</param>
        /// <returns></returns>
        public static IEnumerable<double> Approximate<T>(this IEnumerable<T> source, double check, double epsilon,
                                                         Func<T, double> selector) {
            return source.Select(selector).Where(x => x.ApproximateEqual(check, epsilon));
        }
    }
}