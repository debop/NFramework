using System;
using System.Collections.Generic;
using System.Linq;
using NSoft.NFramework.LinqEx;

namespace NSoft.NFramework.Numerics {
    /// <summary>
    /// Mathematical Methods
    /// </summary>
    public static partial class MathTool {
        /// <summary>
        /// Absoluete value
        /// </summary>
        /// <typeparam name="T">numeric type</typeparam>
        /// <param name="value">value to get absolute value</param>
        /// <returns>absolute value</returns>
        public static T Abs<T>(this T value) {
            // 반대부호의 값을 만듭니다.
            var negate = LinqTool.Operators<T>.Negate(value);

            // 큰 값을 반환합니다. (양의 수가 되겠지요?)
            return LinqTool.Operators<T>.GreaterThan(value, negate) ? value : negate;
        }

        /// <summary>
        /// Return enumerable of absolute value.
        /// </summary>
        /// <param name="source">sequence to get absolute value.</param>
        /// <returns>sequence of absolute value.</returns>
        public static IEnumerable<double> Abs(this IEnumerable<double> source) {
            return source.Select(x => Math.Abs(x));
        }

        /// <summary>
        /// Return enumerable of absolute value.
        /// </summary>
        /// <param name="source">sequence to get absolute value.</param>
        /// <returns>sequence of absolute value.</returns>
        public static IEnumerable<float> Abs(this IEnumerable<float> source) {
            return source.Select(x => Math.Abs(x));
        }

        /// <summary>
        /// Return enumerable of absolute value.
        /// </summary>
        /// <param name="source">sequence to get absolute value.</param>
        /// <returns>sequence of absolute value.</returns>
        public static IEnumerable<int> Abs(this IEnumerable<int> source) {
            return source.Select(x => Math.Abs(x));
        }

        /// <summary>
        /// Return enumerable of absolute value.
        /// </summary>
        /// <param name="source">sequence to get absolute value.</param>
        /// <returns>sequence of absolute value.</returns>
        public static IEnumerable<long> Abs(this IEnumerable<long> source) {
            return source.Select(x => Math.Abs(x));
        }

        /// <summary>
        /// Return enumerable of absolute value.
        /// </summary>
        /// <param name="source">sequence to get absolute value.</param>
        /// <returns>sequence of absolute value.</returns>
        public static IEnumerable<decimal> Abs(this IEnumerable<decimal> source) {
            return source.Select(x => Math.Abs(x));
        }

        /// <summary>
        /// Return enumerable of absolute value.
        /// </summary>
        /// <param name="source">sequence to get absolute value.</param>
        /// <param name="selector">selector for projection.</param>
        /// <returns>sequence of absolute value.</returns>
        public static IEnumerable<double> Abs<T>(this IEnumerable<T> source, Func<T, double> selector) {
            return source.Select(selector).Abs();
        }

        /// <summary>
        /// Return enumerable of absolute value.
        /// </summary>
        /// <param name="source">sequence to get absolute value.</param>
        /// <param name="selector">selector for projection.</param>
        /// <returns>sequence of absolute value.</returns>
        public static IEnumerable<float> Abs<T>(this IEnumerable<T> source, Func<T, float> selector) {
            return source.Select(selector).Abs();
        }

        /// <summary>
        /// Return enumerable of absolute value.
        /// </summary>
        /// <param name="source">sequence to get absolute value.</param>
        /// <param name="selector">selector for projection.</param>
        /// <returns>sequence of absolute value.</returns>
        public static IEnumerable<long> Abs<T>(this IEnumerable<T> source, Func<T, long> selector) {
            return source.Select(selector).Abs();
        }

        /// <summary>
        /// Return enumerable of absolute value.
        /// </summary>
        /// <param name="source">sequence to get absolute value.</param>
        /// <param name="selector">selector for projection.</param>
        /// <returns>sequence of absolute value.</returns>
        public static IEnumerable<int> Abs<T>(this IEnumerable<T> source, Func<T, int> selector) {
            return source.Select(selector).Abs();
        }

        /// <summary>
        /// Return enumerable of absolute value.
        /// </summary>
        /// <param name="source">sequence to get absolute value.</param>
        /// <param name="selector">selector for projection.</param>
        /// <returns>sequence of absolute value.</returns>
        public static IEnumerable<decimal> Abs<T>(this IEnumerable<T> source, Func<T, decimal> selector) {
            return source.Select(selector).Abs();
        }
    }
}