using System;
using System.Collections.Generic;
using System.Linq;

namespace NSoft.NFramework.Numerics {
    public static partial class MathTool {
        /// <summary>
        /// Overflow 를 방지 하기 위해 Int 수형 변량들의 합을 구해서 long 수형으로 반환한다. 
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <returns>합계</returns>
        public static long LongSum(this IEnumerable<int> source) {
            source.ShouldNotBeNull("source");

            return source.Aggregate<int, long>(0, (sum, v) => sum + v);
        }

        /// <summary>
        /// Overflow 를 방지 하기 위해 Int 수형 변량들의 합을 구해서 long 수형으로 반환한다. 
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <returns>합계</returns>
        public static long? LongSum(this IEnumerable<int?> source) {
            source.ShouldNotBeNull("source");

            return
                source
                    .Where(v => v.HasValue)
                    .Select(v => v.Value)
                    .Aggregate(0, (sum, v) => sum + v);
        }

        /// <summary>
        /// Overflow 를 방지 하기 위해 Int 수형 변량들의 합을 구해서 long 수형으로 반환한다. 
        /// </summary>
        /// <param name="source">변량을 가진 객체의 시퀀스</param>
        /// <param name="selector">객체로부터 변량을 선택할 선택자</param>
        /// <returns>합계</returns>
        public static long LongSum<T>(this IEnumerable<T> source, Func<T, int> selector) {
            return source.Select(selector).LongSum();
        }

        /// <summary>
        /// Overflow 를 방지 하기 위해 Int 수형 변량들의 합을 구해서 long 수형으로 반환한다. 
        /// </summary>
        /// <param name="source">변량을 가진 객체의 시퀀스</param>
        /// <param name="selector">객체로부터 변량을 선택할 선택자</param>
        /// <returns>합계</returns>
        public static long? LongSum<T>(this IEnumerable<T> source, Func<T, int?> selector) {
            return source.Select(selector).LongSum();
        }

        /// <summary>
        /// 변량들의 절대값의 합을 구한다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <returns>변량들의 절대값의 합</returns>
        public static double AbsSum(this IEnumerable<double> source) {
            return source.Abs().Sum();
        }

        /// <summary>
        /// 변량들의 절대값의 합을 구한다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <returns>변량들의 절대값의 합</returns>
        public static float AbsSum(this IEnumerable<float> source) {
            return source.Abs().Sum();
        }

        /// <summary>
        /// 변량들의 절대값의 합을 구한다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <returns>변량들의 절대값의 합</returns>
        public static int AbsSum(this IEnumerable<int> source) {
            return source.Abs().Sum();
        }

        /// <summary>
        /// 변량들의 절대값의 합을 구한다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <returns>변량들의 절대값의 합</returns>
        public static long AbsSum(this IEnumerable<long> source) {
            return source.Abs().Sum();
        }

        /// <summary>
        /// 변량들의 절대값의 합을 구한다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <returns>변량들의 절대값의 합</returns>
        public static decimal AbsSum(this IEnumerable<decimal> source) {
            return source.Abs().Sum();
        }

        /// <summary>
        /// 변량들의 절대값의 합을 구한다.
        /// </summary>
        /// <param name="source">변량을 추출할 객체의 시퀀스</param>
        /// <param name="selector">객체로부터 변량을 선택할 선택자</param>
        /// <returns>변량들의 절대값의 합</returns>
        public static double AbsSum<T>(this IEnumerable<T> source, Func<T, double> selector) {
            return source.Select(selector).AbsSum();
        }

        /// <summary>
        /// 변량들의 절대값의 합을 구한다.
        /// </summary>
        /// <param name="source">변량을 추출할 객체의 시퀀스</param>
        /// <param name="selector">객체로부터 변량을 선택할 선택자</param>
        /// <returns>변량들의 절대값의 합</returns>
        public static float AbsSum<T>(this IEnumerable<T> source, Func<T, float> selector) {
            return source.Select(selector).AbsSum();
        }

        /// <summary>
        /// 변량들의 절대값의 합을 구한다.
        /// </summary>
        /// <param name="source">변량을 추출할 객체의 시퀀스</param>
        /// <param name="selector">객체로부터 변량을 선택할 선택자</param>
        /// <returns>변량들의 절대값의 합</returns>
        public static int AbsSum<T>(this IEnumerable<T> source, Func<T, int> selector) {
            return source.Select(selector).AbsSum();
        }

        /// <summary>
        /// 변량들의 절대값의 합을 구한다.
        /// </summary>
        /// <param name="source">변량을 추출할 객체의 시퀀스</param>
        /// <param name="selector">객체로부터 변량을 선택할 선택자</param>
        /// <returns>변량들의 절대값의 합</returns>
        public static long AbsSum<T>(this IEnumerable<T> source, Func<T, long> selector) {
            return source.Select(selector).AbsSum();
        }

        /// <summary>
        /// 변량들의 절대값의 합을 구한다.
        /// </summary>
        /// <param name="source">변량을 추출할 객체의 시퀀스</param>
        /// <param name="selector">객체로부터 변량을 선택할 선택자</param>
        /// <returns>변량들의 절대값의 합</returns>
        public static decimal AbsSum<T>(this IEnumerable<T> source, Func<T, decimal> selector) {
            return source.Select(selector).AbsSum();
        }

        /// <summary>
        /// 변량들의 제곱의 합을 구한다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <returns>제곱의 합</returns>
        public static double SumOfSquares(this IEnumerable<double> source) {
            return source.Select(x => Square(x)).Sum();
        }

        /// <summary>
        /// 변량들의 제곱의 합을 구한다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <returns>제곱의 합</returns>
        public static double? SumOfSquares(this IEnumerable<double?> source) {
            return source.Where(v => v.HasValue).Select(v => Square(v.Value)).Sum();
        }

        /// <summary>
        /// 변량들의 제곱의 합을 구한다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <returns>제곱의 합</returns>
        public static float SumOfSquares(this IEnumerable<float> source) {
            return source.Select(x => Square(x)).Sum();
        }

        /// <summary>
        /// 변량들의 제곱의 합을 구한다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <returns>제곱의 합</returns>
        public static float? SumOfSquares(this IEnumerable<float?> source) {
            return source.Where(v => v.HasValue).Select(v => Square(v.Value)).Sum();
        }

        /// <summary>
        /// 변량들의 제곱의 합을 구한다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <returns>제곱의 합</returns>
        public static decimal SumOfSquares(this IEnumerable<decimal> source) {
            return source.Select(x => Square(x)).Sum();
        }

        /// <summary>
        /// 변량들의 제곱의 합을 구한다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <returns>제곱의 합</returns>
        public static decimal? SumOfSquares(this IEnumerable<decimal?> source) {
            return source.Where(v => v.HasValue).Select(v => Square(v.Value)).Sum();
        }

        /// <summary>
        /// 변량들의 제곱의 합을 구한다.
        /// </summary>
        /// <param name="source">변량을 추출할 객체의 시퀀스</param>
        /// <param name="selector">객체로부터 변량을 선택할 선택자</param>
        /// <returns>제곱의 합</returns>
        public static double SumOfSquares<T>(this IEnumerable<T> source, Func<T, double> selector) {
            return source.Select(selector).SumOfSquares();
        }

        /// <summary>
        /// 변량들의 제곱의 합을 구한다.
        /// </summary>
        /// <param name="source">변량을 추출할 객체의 시퀀스</param>
        /// <param name="selector">객체로부터 변량을 선택할 선택자</param>
        /// <returns>제곱의 합</returns>
        public static double? SumOfSquares<T>(this IEnumerable<T> source, Func<T, double?> selector) {
            return source.Select(selector).SumOfSquares();
        }

        /// <summary>
        /// 변량들의 제곱의 합을 구한다.
        /// </summary>
        /// <param name="source">변량을 추출할 객체의 시퀀스</param>
        /// <param name="selector">객체로부터 변량을 선택할 선택자</param>
        /// <returns>제곱의 합</returns>
        public static float SumOfSquares<T>(this IEnumerable<T> source, Func<T, float> selector) {
            return source.Select(selector).SumOfSquares();
        }

        /// <summary>
        /// 변량들의 제곱의 합을 구한다.
        /// </summary>
        /// <param name="source">변량을 추출할 객체의 시퀀스</param>
        /// <param name="selector">객체로부터 변량을 선택할 선택자</param>
        /// <returns>제곱의 합</returns>
        public static float? SumOfSquares<T>(this IEnumerable<T> source, Func<T, float?> selector) {
            return source.Select(selector).SumOfSquares();
        }

        /// <summary>
        /// 변량들의 제곱의 합을 구한다.
        /// </summary>
        /// <param name="source">변량을 추출할 객체의 시퀀스</param>
        /// <param name="selector">객체로부터 변량을 선택할 선택자</param>
        /// <returns>제곱의 합</returns>
        public static decimal SumOfSquares<T>(this IEnumerable<T> source, Func<T, decimal> selector) {
            return source.Select(selector).SumOfSquares();
        }

        /// <summary>
        /// 변량들의 제곱의 합을 구한다.
        /// </summary>
        /// <param name="source">변량을 추출할 객체의 시퀀스</param>
        /// <param name="selector">객체로부터 변량을 선택할 선택자</param>
        /// <returns>제곱의 합</returns>
        public static decimal? SumOfSquares<T>(this IEnumerable<T> source, Func<T, decimal?> selector) {
            return source.Select(selector).SumOfSquares();
        }
    }
}