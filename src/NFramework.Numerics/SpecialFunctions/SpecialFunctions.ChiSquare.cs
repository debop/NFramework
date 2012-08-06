using System;
using System.Collections.Generic;
using System.Linq;

namespace NSoft.NFramework.Numerics {
    public static partial class SpecialFunctions {
        /// <summary>
        /// ChiSquare 함수
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double ChiSquare(this double x) {
            return 2.0 * Gamma(x / 2.0);
        }

        /// <summary>
        /// ChiSquare 함수
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<double> ChiSquare(this IEnumerable<double> source) {
            source.ShouldNotBeNull("source");
            return source.Select(x => ChiSquare(x));
        }

        /// <summary>
        /// ChiSquare 함수
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static IEnumerable<double> ChiSquare<T>(this IEnumerable<T> source, Func<T, double> selector) {
            source.ShouldNotBeNull("source");
            return source.Select(selector).ChiSquare();
        }
    }
}