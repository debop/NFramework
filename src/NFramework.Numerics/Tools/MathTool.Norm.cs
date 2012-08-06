using System;
using System.Collections.Generic;
using System.Linq;

namespace NSoft.NFramework.Numerics {
    public static partial class MathTool {
        /// <summary>
        /// Norm ( 요소의 제곱의 합 )을 구한다. (벡터의 길이, 차원의 가장 짧은 거리)
        /// </summary>
        /// <param name="source">변량의 시퀀스</param>
        /// <returns>Norm</returns>
        public static double Norm(this IEnumerable<double> source) {
            source.ShouldNotBeNull("source");
            return source.Sum(x => x.Square());
        }

        /// <summary>
        /// Norm ( 요소의 제곱의 합 )을 구한다. (벡터의 길이, 차원의 가장 짧은 거리)
        /// </summary>
        /// <param name="source">변량의 시퀀스</param>
        /// <returns>Norm</returns>
        public static double Norm(params double[] source) {
            if(source == null || source.Length == 0)
                return 0.0d;

            return source.Norm();
        }

        /// <summary>
        /// Norm ( 요소의 제곱의 합 )을 구한다. (벡터의 길이, 차원의 가장 짧은 거리)
        /// </summary>
        /// <param name="source">변량의 시퀀스</param>
        /// <returns>Norm</returns>
        public static double? Norm(this IEnumerable<double?> source) {
            return source.SumOfSquares();
        }

        /// <summary>
        /// Norm ( 요소의 제곱의 합 )을 구한다. (벡터의 길이, 차원의 가장 짧은 거리)
        /// </summary>
        /// <param name="source">변량의 시퀀스</param>
        /// <returns>Norm</returns>
        public static float Norm(this IEnumerable<float> source) {
            source.ShouldNotBeNull("source");
            return source.Sum(x => x.Square());
        }

        /// <summary>
        /// Norm ( 요소의 제곱의 합 )을 구한다. (벡터의 길이, 차원의 가장 짧은 거리)
        /// </summary>
        /// <param name="source">변량의 시퀀스</param>
        /// <returns>Norm</returns>
        public static float Norm(params float[] source) {
            if(source == null || source.Length == 0)
                return 0.0f;

            return source.Norm();
        }

        /// <summary>
        /// Norm ( 요소의 제곱의 합 )을 구한다. (벡터의 길이, 차원의 가장 짧은 거리)
        /// </summary>
        /// <param name="source">변량의 시퀀스</param>
        /// <returns>Norm</returns>
        public static float? Norm(this IEnumerable<float?> source) {
            return source.SumOfSquares();
        }

        /// <summary>
        /// Norm ( 요소의 제곱의 합 )을 구한다. (벡터의 길이, 차원의 가장 짧은 거리)
        /// </summary>
        /// <param name="source">변량의 시퀀스</param>
        /// <returns>Norm</returns>
        public static decimal Norm(this IEnumerable<decimal> source) {
            source.ShouldNotBeNull("source");
            return source.Sum(x => x.Square());
        }

        /// <summary>
        /// Norm ( 요소의 제곱의 합 )을 구한다. (벡터의 길이, 차원의 가장 짧은 거리)
        /// </summary>
        /// <param name="source">변량의 시퀀스</param>
        /// <returns>Norm</returns>
        public static decimal Norm(params decimal[] source) {
            if(source == null || source.Length == 0)
                return 0.0m;

            return source.Norm();
        }

        /// <summary>
        /// Norm ( 요소의 제곱의 합 )을 구한다. (벡터의 길이, 차원의 가장 짧은 거리)
        /// </summary>
        /// <param name="source">변량의 시퀀스</param>
        /// <returns>Norm</returns>
        public static decimal? Norm(this IEnumerable<decimal?> source) {
            return source.SumOfSquares();
        }

        /// <summary>
        /// Norm ( 요소의 제곱의 합 )을 구한다. (벡터의 길이, 차원의 가장 짧은 거리)
        /// </summary>
        /// <param name="source">변량을 가진 객체의 시퀀스</param>
        /// <param name="selector">객체에서 변량을 선택하는 선택자</param>
        /// <returns>Norm</returns>
        public static double Norm<T>(this IEnumerable<T> source, Func<T, double> selector) {
            return source.Select(selector).Norm();
        }

        /// <summary>
        /// Norm ( 요소의 제곱의 합 )을 구한다. (벡터의 길이, 차원의 가장 짧은 거리)
        /// </summary>
        /// <param name="source">변량을 가진 객체의 시퀀스</param>
        /// <param name="selector">객체에서 변량을 선택하는 선택자</param>
        /// <returns>Norm</returns>
        public static double? Norm<T>(this IEnumerable<T> source, Func<T, double?> selector) {
            return source.Select(selector).Norm();
        }

        /// <summary>
        /// Norm ( 요소의 제곱의 합 )을 구한다. (벡터의 길이, 차원의 가장 짧은 거리)
        /// </summary>
        /// <param name="source">변량을 가진 객체의 시퀀스</param>
        /// <param name="selector">객체에서 변량을 선택하는 선택자</param>
        /// <returns>Norm</returns>
        public static float Norm<T>(this IEnumerable<T> source, Func<T, float> selector) {
            return source.Select(selector).Norm();
        }

        /// <summary>
        /// Norm ( 요소의 제곱의 합 )을 구한다. (벡터의 길이, 차원의 가장 짧은 거리)
        /// </summary>
        /// <param name="source">변량을 가진 객체의 시퀀스</param>
        /// <param name="selector">객체에서 변량을 선택하는 선택자</param>
        /// <returns>Norm</returns>
        public static float? Norm<T>(this IEnumerable<T> source, Func<T, float?> selector) {
            return source.Select(selector).Norm();
        }

        /// <summary>
        /// Norm ( 요소의 제곱의 합 )을 구한다. (벡터의 길이, 차원의 가장 짧은 거리)
        /// </summary>
        /// <param name="source">변량을 가진 객체의 시퀀스</param>
        /// <param name="selector">객체에서 변량을 선택하는 선택자</param>
        /// <returns>Norm</returns>
        public static decimal Norm<T>(this IEnumerable<T> source, Func<T, decimal> selector) {
            return source.Select(selector).Norm();
        }

        /// <summary>
        /// Norm ( 요소의 제곱의 합 )을 구한다. (벡터의 길이, 차원의 가장 짧은 거리)
        /// </summary>
        /// <param name="source">변량을 가진 객체의 시퀀스</param>
        /// <param name="selector">객체에서 변량을 선택하는 선택자</param>
        /// <returns>Norm</returns>
        public static decimal? Norm<T>(this IEnumerable<T> source, Func<T, decimal?> selector) {
            return source.Select(selector).Norm();
        }
    }
}