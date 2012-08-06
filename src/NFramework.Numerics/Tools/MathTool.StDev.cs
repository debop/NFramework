using System;
using System.Collections.Generic;
using System.Linq;

namespace NSoft.NFramework.Numerics {
    public static partial class MathTool {
        #region << Standard Deviation (표준편차) >>

        /// <summary>
        /// 변량들의 표준편차를 구한다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <returns>표준편차</returns>
        public static double StDev(this IEnumerable<double> source) {
            return Math.Sqrt(source.Variance());
        }

        /// <summary>
        /// 표준편차를 구한다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <returns>표준편차</returns>
        public static double? StDev(this IEnumerable<double?> source) {
            var variance = source.Variance();
            if(variance.HasValue)
                return Math.Sqrt(variance.Value);

            return null;
        }

        /// <summary>
        /// 변량들의 표준편차를 구한다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <returns>표준편차</returns>
        public static float StDev(this IEnumerable<float> source) {
            return (float)Math.Sqrt(source.Variance());
        }

        /// <summary>
        /// 표준편차를 구한다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <returns>표준편차</returns>
        public static float? StDev(this IEnumerable<float?> source) {
            var variance = source.Variance();
            if(variance.HasValue)
                return (float)Math.Sqrt(variance.Value);

            return null;
        }

        /// <summary>
        /// 변량들의 표준편차를 구한다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <returns>표준편차</returns>
        public static decimal StDev(this IEnumerable<decimal> source) {
            return (decimal)Math.Sqrt((double)source.Variance());
        }

        /// <summary>
        /// 표준편차를 구한다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <returns>표준편차</returns>
        public static decimal? StDev(this IEnumerable<decimal?> source) {
            var variance = source.Variance();
            if(variance.HasValue)
                return (decimal)Math.Sqrt((double)variance.Value);

            return null;
        }

        /// <summary>
        /// 변량들의 표준편차를 구한다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <returns>표준편차</returns>
        public static double StDev(this IEnumerable<long> source) {
            return Math.Sqrt(source.Variance());
        }

        /// <summary>
        /// 표준편차를 구한다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <returns>표준편차</returns>
        public static double? StDev(this IEnumerable<long?> source) {
            var variance = source.Variance();
            if(variance.HasValue)
                return Math.Sqrt(variance.Value);

            return null;
        }

        /// <summary>
        /// 변량들의 표준편차를 구한다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <returns>표준편차</returns>
        public static double StDev(this IEnumerable<int> source) {
            return Math.Sqrt(source.Variance());
        }

        /// <summary>
        /// 표준편차를 구한다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <returns>표준편차</returns>
        public static double? StDev(this IEnumerable<int?> source) {
            var variance = source.Variance();
            if(variance.HasValue)
                return Math.Sqrt(variance.Value);

            return null;
        }

        /// <summary>
        /// 변량들의 표준편차를 구한다.
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <param name="selector">변량 선택자</param>
        /// <returns>표준편차</returns>
        public static double StDev<T>(this IEnumerable<T> source, Func<T, double> selector) {
            return source.Select(selector).StDev();
        }

        /// <summary>
        /// 변량들의 표준편차를 구한다.
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <param name="selector">변량 선택자</param>
        /// <returns>표준편차</returns>
        public static double? StDev<T>(this IEnumerable<T> source, Func<T, double?> selector) {
            return source.Select(selector).StDev();
        }

        /// <summary>
        /// 변량들의 표준편차를 구한다.
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <param name="selector">변량 선택자</param>
        /// <returns>표준편차</returns>
        public static float StDev<T>(this IEnumerable<T> source, Func<T, float> selector) {
            return source.Select(selector).StDev();
        }

        /// <summary>
        /// 변량들의 표준편차를 구한다.
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <param name="selector">변량 선택자</param>
        /// <returns>표준편차</returns>
        public static float? StDev<T>(this IEnumerable<T> source, Func<T, float?> selector) {
            return source.Select(selector).StDev();
        }

        /// <summary>
        /// 변량들의 표준편차를 구한다.
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <param name="selector">변량 선택자</param>
        /// <returns>표준편차</returns>
        public static decimal StDev<T>(this IEnumerable<T> source, Func<T, decimal> selector) {
            return source.Select(selector).StDev();
        }

        /// <summary>
        /// 변량들의 표준편차를 구한다.
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <param name="selector">변량 선택자</param>
        /// <returns>표준편차</returns>
        public static decimal? StDev<T>(this IEnumerable<T> source, Func<T, decimal?> selector) {
            return source.Select(selector).StDev();
        }

        /// <summary>
        /// 변량들의 표준편차를 구한다.
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <param name="selector">변량 선택자</param>
        /// <returns>표준편차</returns>
        public static double StDev<T>(this IEnumerable<T> source, Func<T, long> selector) {
            return source.Select(selector).StDev();
        }

        /// <summary>
        /// 변량들의 표준편차를 구한다.
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <param name="selector">변량 선택자</param>
        /// <returns>표준편차</returns>
        public static double? StDev<T>(this IEnumerable<T> source, Func<T, long?> selector) {
            return source.Select(selector).StDev();
        }

        /// <summary>
        /// 변량들의 표준편차를 구한다.
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <param name="selector">변량 선택자</param>
        /// <returns>표준편차</returns>
        public static double StDev<T>(this IEnumerable<T> source, Func<T, int> selector) {
            return source.Select(selector).StDev();
        }

        /// <summary>
        /// 변량들의 표준편차를 구한다.
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <param name="selector">변량 선택자</param>
        /// <returns>표준편차</returns>
        public static double? StDev<T>(this IEnumerable<T> source, Func<T, int?> selector) {
            return source.Select(selector).StDev();
        }

        #endregion

        #region << AverageAndStDev >>

        /// <summary>
        /// 시퀀스에서 평균과 표준편차를 구한다.
        /// </summary>
        /// <param name="source">변량을 가진 객체의 시퀀스</param>
        /// <param name="average">평균</param>
        /// <param name="stdev">표준편차</param>
        public static void AverageAndStDev(this IEnumerable<double> source, out double average, out double stdev) {
            source.ShouldNotBeNull("source");

            average = source.Average();
            stdev = source.StDev();
        }

        /// <summary>
        /// 시퀀스에서 평균과 표준편차를 구한다.
        /// </summary>
        /// <param name="source">변량을 가진 객체의 시퀀스</param>
        /// <param name="average">평균</param>
        /// <param name="stdev">표준편차</param>
        public static void AverageAndStDev(this IEnumerable<double?> source, out double? average, out double? stdev) {
            source.ShouldNotBeNull("source");

            average = source.Average();
            stdev = source.StDev();
        }

        /// <summary>
        /// 시퀀스에서 평균과 표준편차를 구한다.
        /// </summary>
        /// <param name="source">변량을 가진 객체의 시퀀스</param>
        /// <param name="average">평균</param>
        /// <param name="stdev">표준편차</param>
        public static void AverageAndStDev(this IEnumerable<float> source, out float average, out float stdev) {
            source.ShouldNotBeNull("source");

            average = source.Average();
            stdev = source.StDev();
        }

        /// <summary>
        /// 시퀀스에서 평균과 표준편차를 구한다.
        /// </summary>
        /// <param name="source">변량을 가진 객체의 시퀀스</param>
        /// <param name="average">평균</param>
        /// <param name="stdev">표준편차</param>
        public static void AverageAndStDev(this IEnumerable<float?> source, out float? average, out float? stdev) {
            source.ShouldNotBeNull("source");

            average = source.Average();
            stdev = source.StDev();
        }

        /// <summary>
        /// 시퀀스에서 평균과 표준편차를 구한다.
        /// </summary>
        /// <param name="source">변량을 가진 객체의 시퀀스</param>
        /// <param name="average">평균</param>
        /// <param name="stdev">표준편차</param>
        public static void AverageAndStDev(this IEnumerable<decimal> source, out decimal average, out decimal stdev) {
            source.ShouldNotBeNull("source");

            average = source.Average();
            stdev = source.StDev();
        }

        /// <summary>
        /// 시퀀스에서 평균과 표준편차를 구한다.
        /// </summary>
        /// <param name="source">변량을 가진 객체의 시퀀스</param>
        /// <param name="average">평균</param>
        /// <param name="stdev">표준편차</param>
        public static void AverageAndStDev(this IEnumerable<decimal?> source, out decimal? average, out decimal? stdev) {
            source.ShouldNotBeNull("source");

            average = source.Average();
            stdev = source.StDev();
        }

        /// <summary>
        /// 시퀀스에서 평균과 표준편차를 구한다.
        /// </summary>
        /// <param name="source">변량을 가진 객체의 시퀀스</param>
        /// <param name="average">평균</param>
        /// <param name="stdev">표준편차</param>
        public static void AverageAndStDev(this IEnumerable<long> source, out double average, out double stdev) {
            source.ShouldNotBeNull("source");

            average = source.Average();
            stdev = source.StDev();
        }

        /// <summary>
        /// 시퀀스에서 평균과 표준편차를 구한다.
        /// </summary>
        /// <param name="source">변량을 가진 객체의 시퀀스</param>
        /// <param name="average">평균</param>
        /// <param name="stdev">표준편차</param>
        public static void AverageAndStDev(this IEnumerable<long?> source, out double? average, out double? stdev) {
            source.ShouldNotBeNull("source");

            average = source.Average();
            stdev = source.StDev();
        }

        /// <summary>
        /// 시퀀스에서 평균과 표준편차를 구한다.
        /// </summary>
        /// <param name="source">변량을 가진 객체의 시퀀스</param>
        /// <param name="average">평균</param>
        /// <param name="stdev">표준편차</param>
        public static void AverageAndStDev(this IEnumerable<int> source, out double average, out double stdev) {
            source.ShouldNotBeNull("source");

            average = source.Average();
            stdev = source.StDev();
        }

        /// <summary>
        /// 시퀀스에서 평균과 표준편차를 구한다.
        /// </summary>
        /// <param name="source">변량을 가진 객체의 시퀀스</param>
        /// <param name="average">평균</param>
        /// <param name="stdev">표준편차</param>
        public static void AverageAndStDev(this IEnumerable<int?> source, out double? average, out double? stdev) {
            source.ShouldNotBeNull("source");

            average = source.Average();
            stdev = source.StDev();
        }

        /// <summary>
        /// 시퀀스에서 평균과 표준편차를 구한다.
        /// </summary>
        /// <param name="source">변량을 가진 객체의 시퀀스</param>
        /// <param name="selector">시퀀스로부터 계산할 변량을 선택하는 선택자</param>
        /// <param name="average">평균</param>
        /// <param name="stdev">표준편차</param>
        public static void AverageAndStDev<T>(this IEnumerable<T> source, Func<T, double> selector, out double average, out double stdev) {
            source.Select(selector).AverageAndStDev(out average, out stdev);
        }

        /// <summary>
        /// 시퀀스에서 평균과 표준편차를 구한다.
        /// </summary>
        /// <param name="source">변량을 가진 객체의 시퀀스</param>
        /// <param name="selector">시퀀스로부터 계산할 변량을 선택하는 선택자</param>
        /// <param name="average">평균</param>
        /// <param name="stdev">표준편차</param>
        public static void AverageAndStDev<T>(this IEnumerable<T> source, Func<T, double?> selector, out double? average,
                                              out double? stdev) {
            source.Select(selector).AverageAndStDev(out average, out stdev);
        }

        /// <summary>
        /// 시퀀스에서 평균과 표준편차를 구한다.
        /// </summary>
        /// <param name="source">변량을 가진 객체의 시퀀스</param>
        /// <param name="selector">시퀀스로부터 계산할 변량을 선택하는 선택자</param>
        /// <param name="average">평균</param>
        /// <param name="stdev">표준편차</param>
        public static void AverageAndStDev<T>(this IEnumerable<T> source, Func<T, float> selector, out float average, out float stdev) {
            source.Select(selector).AverageAndStDev(out average, out stdev);
        }

        /// <summary>
        /// 시퀀스에서 평균과 표준편차를 구한다.
        /// </summary>
        /// <param name="source">변량을 가진 객체의 시퀀스</param>
        /// <param name="selector">시퀀스로부터 계산할 변량을 선택하는 선택자</param>
        /// <param name="average">평균</param>
        /// <param name="stdev">표준편차</param>
        public static void AverageAndStDev<T>(this IEnumerable<T> source, Func<T, float?> selector, out float? average, out float? stdev) {
            source.Select(selector).AverageAndStDev(out average, out stdev);
        }

        /// <summary>
        /// 시퀀스에서 평균과 표준편차를 구한다.
        /// </summary>
        /// <param name="source">변량을 가진 객체의 시퀀스</param>
        /// <param name="selector">시퀀스로부터 계산할 변량을 선택하는 선택자</param>
        /// <param name="average">평균</param>
        /// <param name="stdev">표준편차</param>
        public static void AverageAndStDev<T>(this IEnumerable<T> source, Func<T, decimal> selector, out decimal average,
                                              out decimal stdev) {
            source.Select(selector).AverageAndStDev(out average, out stdev);
        }

        /// <summary>
        /// 시퀀스에서 평균과 표준편차를 구한다.
        /// </summary>
        /// <param name="source">변량을 가진 객체의 시퀀스</param>
        /// <param name="selector">시퀀스로부터 계산할 변량을 선택하는 선택자</param>
        /// <param name="average">평균</param>
        /// <param name="stdev">표준편차</param>
        public static void AverageAndStDev<T>(this IEnumerable<T> source, Func<T, decimal?> selector, out decimal? average,
                                              out decimal? stdev) {
            source.Select(selector).AverageAndStDev(out average, out stdev);
        }

        /// <summary>
        /// 시퀀스에서 평균과 표준편차를 구한다.
        /// </summary>
        /// <param name="source">변량을 가진 객체의 시퀀스</param>
        /// <param name="selector">시퀀스로부터 계산할 변량을 선택하는 선택자</param>
        /// <param name="average">평균</param>
        /// <param name="stdev">표준편차</param>
        public static void AverageAndStDev<T>(this IEnumerable<T> source, Func<T, long> selector, out double average, out double stdev) {
            source.Select(selector).AverageAndStDev(out average, out stdev);
        }

        /// <summary>
        /// 시퀀스에서 평균과 표준편차를 구한다.
        /// </summary>
        /// <param name="source">변량을 가진 객체의 시퀀스</param>
        /// <param name="selector">시퀀스로부터 계산할 변량을 선택하는 선택자</param>
        /// <param name="average">평균</param>
        /// <param name="stdev">표준편차</param>
        public static void AverageAndStDev<T>(this IEnumerable<T> source, Func<T, long?> selector, out double? average,
                                              out double? stdev) {
            source.Select(selector).AverageAndStDev(out average, out stdev);
        }

        /// <summary>
        /// 시퀀스에서 평균과 표준편차를 구한다.
        /// </summary>
        /// <param name="source">변량을 가진 객체의 시퀀스</param>
        /// <param name="selector">시퀀스로부터 계산할 변량을 선택하는 선택자</param>
        /// <param name="average">평균</param>
        /// <param name="stdev">표준편차</param>
        public static void AverageAndStDev<T>(this IEnumerable<T> source, Func<T, int> selector, out double average, out double stdev) {
            source.Select(selector).AverageAndStDev(out average, out stdev);
        }

        /// <summary>
        /// 시퀀스에서 평균과 표준편차를 구한다.
        /// </summary>
        /// <param name="source">변량을 가진 객체의 시퀀스</param>
        /// <param name="selector">시퀀스로부터 계산할 변량을 선택하는 선택자</param>
        /// <param name="average">평균</param>
        /// <param name="stdev">표준편차</param>
        public static void AverageAndStDev<T>(this IEnumerable<T> source, Func<T, int?> selector, out double? average, out double? stdev) {
            source.Select(selector).AverageAndStDev(out average, out stdev);
        }

        #endregion

        #region << Cumulative Standard Deviation (누적 표준편차) >>

        /// <summary>
        /// 시퀀스의 표준편차을 누적해서 열거합니다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <returns>누적 표준편차</returns>
        public static IEnumerable<double> CumulativeStDev(this IEnumerable<double> source) {
            return source.CumulativeVariance().Select(x => Math.Sqrt(x));
        }

        /// <summary>
        /// 시퀀스의 표준편차을 누적해서 열거합니다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <returns>누적 표준편차</returns>
        public static IEnumerable<double?> CumulativeStDev(this IEnumerable<double?> source) {
            return source.CumulativeVariance().Select(v => v.HasValue ? (double?)Math.Sqrt(v.Value) : null);
        }

        /// <summary>
        /// 시퀀스의 표준편차을 누적해서 열거합니다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <returns>누적 표준편차</returns>
        public static IEnumerable<float> CumulativeStDev(this IEnumerable<float> source) {
            return source.CumulativeVariance().Select(x => (float)Math.Sqrt(x));
        }

        /// <summary>
        /// 시퀀스의 표준편차을 누적해서 열거합니다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <returns>누적 표준편차</returns>
        public static IEnumerable<float?> CumulativeStDev(this IEnumerable<float?> source) {
            return source.CumulativeVariance().Select(v => v.HasValue ? (float?)Math.Sqrt(v.Value) : null);
        }

        /// <summary>
        /// 시퀀스의 표준편차을 누적해서 열거합니다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <returns>누적 표준편차</returns>
        public static IEnumerable<decimal> CumulativeStDev(this IEnumerable<decimal> source) {
            return source.CumulativeVariance().Select(x => (decimal)Math.Sqrt((double)x));
        }

        /// <summary>
        /// 시퀀스의 표준편차을 누적해서 열거합니다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <returns>누적 표준편차</returns>
        public static IEnumerable<decimal?> CumulativeStDev(this IEnumerable<decimal?> source) {
            return source.CumulativeVariance().Select(v => v.HasValue ? (decimal?)Math.Sqrt((double)v.Value) : null);
        }

        /// <summary>
        /// 시퀀스의 표준편차을 누적해서 열거합니다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <returns>누적 표준편차</returns>
        public static IEnumerable<double> CumulativeStDev(this IEnumerable<long> source) {
            return source.CumulativeVariance().Select(x => Math.Sqrt(x));
        }

        /// <summary>
        /// 시퀀스의 표준편차을 누적해서 열거합니다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <returns>누적 표준편차</returns>
        public static IEnumerable<double?> CumulativeStDev(this IEnumerable<long?> source) {
            return source.CumulativeVariance().Select(v => v.HasValue ? (double?)Math.Sqrt(v.Value) : null);
        }

        /// <summary>
        /// 시퀀스의 표준편차을 누적해서 열거합니다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <returns>누적 표준편차</returns>
        public static IEnumerable<double> CumulativeStDev(this IEnumerable<int> source) {
            return source.CumulativeVariance().Select(x => Math.Sqrt(x));
        }

        /// <summary>
        /// 시퀀스의 표준편차을 누적해서 열거합니다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <returns>누적 표준편차</returns>
        public static IEnumerable<double?> CumulativeStDev(this IEnumerable<int?> source) {
            return source.CumulativeVariance().Select(v => v.HasValue ? (double?)Math.Sqrt(v.Value) : null);
        }

        /// <summary>
        /// 시퀀스의 표준편차을 누적해서 열거합니다.
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <param name="selector">변량 선택자</param>
        /// <returns>누적 표준편차</returns>
        public static IEnumerable<double> CumulativeStDev<T>(this IEnumerable<T> source, Func<T, double> selector) {
            return source.Select(selector).CumulativeStDev();
        }

        /// <summary>
        /// 시퀀스의 표준편차을 누적해서 열거합니다.
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <param name="selector">변량 선택자</param>
        /// <returns>누적 표준편차</returns>
        public static IEnumerable<double?> CumulativeStDev<T>(this IEnumerable<T> source, Func<T, double?> selector) {
            return source.Select(selector).CumulativeStDev();
        }

        /// <summary>
        /// 시퀀스의 표준편차을 누적해서 열거합니다.
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <param name="selector">변량 선택자</param>
        /// <returns>누적 표준편차</returns>
        public static IEnumerable<float> CumulativeStDev<T>(this IEnumerable<T> source, Func<T, float> selector) {
            return source.Select(selector).CumulativeStDev();
        }

        /// <summary>
        /// 시퀀스의 표준편차을 누적해서 열거합니다.
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <param name="selector">변량 선택자</param>
        /// <returns>누적 표준편차</returns>
        public static IEnumerable<float?> CumulativeStDev<T>(this IEnumerable<T> source, Func<T, float?> selector) {
            return source.Select(selector).CumulativeStDev();
        }

        /// <summary>
        /// 시퀀스의 표준편차을 누적해서 열거합니다.
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <param name="selector">변량 선택자</param>
        /// <returns>누적 표준편차</returns>
        public static IEnumerable<decimal> CumulativeStDev<T>(this IEnumerable<T> source, Func<T, decimal> selector) {
            return source.Select(selector).CumulativeStDev();
        }

        /// <summary>
        /// 시퀀스의 표준편차을 누적해서 열거합니다.
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <param name="selector">변량 선택자</param>
        /// <returns>누적 표준편차</returns>
        public static IEnumerable<decimal?> CumulativeStDev<T>(this IEnumerable<T> source, Func<T, decimal?> selector) {
            return source.Select(selector).CumulativeStDev();
        }

        /// <summary>
        /// 시퀀스의 표준편차을 누적해서 열거합니다.
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <param name="selector">변량 선택자</param>
        /// <returns>누적 표준편차</returns>
        public static IEnumerable<double> CumulativeStDev<T>(this IEnumerable<T> source, Func<T, long> selector) {
            return source.Select(selector).CumulativeStDev();
        }

        /// <summary>
        /// 시퀀스의 표준편차을 누적해서 열거합니다.
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <param name="selector">변량 선택자</param>
        /// <returns>누적 표준편차</returns>
        public static IEnumerable<double?> CumulativeStDev<T>(this IEnumerable<T> source, Func<T, long?> selector) {
            return source.Select(selector).CumulativeStDev();
        }

        /// <summary>
        /// 시퀀스의 표준편차을 누적해서 열거합니다.
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <param name="selector">변량 선택자</param>
        /// <returns>누적 표준편차</returns>
        public static IEnumerable<double> CumulativeStDev<T>(this IEnumerable<T> source, Func<T, int> selector) {
            return source.Select(selector).CumulativeStDev();
        }

        /// <summary>
        /// 시퀀스의 표준편차을 누적해서 열거합니다.
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <param name="selector">변량 선택자</param>
        /// <returns>누적 표준편차</returns>
        public static IEnumerable<double?> CumulativeStDev<T>(this IEnumerable<T> source, Func<T, int?> selector) {
            return source.Select(selector).CumulativeStDev();
        }

        #endregion

        #region << Standard Deviation (Block) >>

        /// <summary>
        /// <paramref name="blockSize"/> 단위로 표준편차을 계산하여, 열거합니다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <param name="blockSize">표준편차 계산을 위한 변량 갯수</param>
        /// <returns>블럭별 표준편차</returns>
        public static IEnumerable<double> StDev(this IEnumerable<double> source, int blockSize = BlockSize) {
            return source.Variance(blockSize).Select(x => Math.Sqrt(x));
        }

        /// <summary>
        /// <paramref name="blockSize"/> 단위로 표준편차을 계산하여, 열거합니다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <param name="blockSize">표준편차 계산을 위한 변량 갯수</param>
        /// <returns>블럭별 표준편차</returns>
        public static IEnumerable<double?> StDev(this IEnumerable<double?> source, int blockSize = BlockSize) {
            return source.Variance(blockSize).Select(v => v.HasValue ? (double?)Math.Sqrt(v.Value) : null);
        }

        /// <summary>
        /// <paramref name="blockSize"/> 단위로 표준편차을 계산하여, 열거합니다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <param name="blockSize">표준편차 계산을 위한 변량 갯수</param>
        /// <returns>블럭별 표준편차</returns>
        public static IEnumerable<float> StDev(this IEnumerable<float> source, int blockSize = BlockSize) {
            return source.Variance(blockSize).Select(x => (float)Math.Sqrt(x));
        }

        /// <summary>
        /// <paramref name="blockSize"/> 단위로 표준편차을 계산하여, 열거합니다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <param name="blockSize">표준편차 계산을 위한 변량 갯수</param>
        /// <returns>블럭별 표준편차</returns>
        public static IEnumerable<float?> StDev(this IEnumerable<float?> source, int blockSize = BlockSize) {
            return source.Variance(blockSize).Select(v => v.HasValue ? (float?)Math.Sqrt(v.Value) : null);
        }

        /// <summary>
        /// <paramref name="blockSize"/> 단위로 표준편차을 계산하여, 열거합니다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <param name="blockSize">표준편차 계산을 위한 변량 갯수</param>
        /// <returns>블럭별 표준편차</returns>
        public static IEnumerable<decimal> StDev(this IEnumerable<decimal> source, int blockSize = BlockSize) {
            return source.Variance(blockSize).Select(x => (decimal)Math.Sqrt((double)x));
        }

        /// <summary>
        /// <paramref name="blockSize"/> 단위로 표준편차을 계산하여, 열거합니다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <param name="blockSize">표준편차 계산을 위한 변량 갯수</param>
        /// <returns>블럭별 표준편차</returns>
        public static IEnumerable<decimal?> StDev(this IEnumerable<decimal?> source, int blockSize = BlockSize) {
            return source.Variance(blockSize).Select(v => v.HasValue ? (decimal?)Math.Sqrt((double)v.Value) : null);
        }

        /// <summary>
        /// <paramref name="blockSize"/> 단위로 표준편차을 계산하여, 열거합니다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <param name="blockSize">표준편차 계산을 위한 변량 갯수</param>
        /// <returns>블럭별 표준편차</returns>
        public static IEnumerable<double> StDev(this IEnumerable<long> source, int blockSize = BlockSize) {
            return source.Variance(blockSize).Select(x => Math.Sqrt(x));
        }

        /// <summary>
        /// <paramref name="blockSize"/> 단위로 표준편차을 계산하여, 열거합니다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <param name="blockSize">표준편차 계산을 위한 변량 갯수</param>
        /// <returns>블럭별 표준편차</returns>
        public static IEnumerable<double?> StDev(this IEnumerable<long?> source, int blockSize = BlockSize) {
            return source.Variance(blockSize).Select(v => v.HasValue ? (double?)Math.Sqrt(v.Value) : null);
        }

        /// <summary>
        /// <paramref name="blockSize"/> 단위로 표준편차을 계산하여, 열거합니다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <param name="blockSize">표준편차 계산을 위한 변량 갯수</param>
        /// <returns>블럭별 표준편차</returns>
        public static IEnumerable<double> StDev(this IEnumerable<int> source, int blockSize = BlockSize) {
            return source.Variance(blockSize).Select(x => Math.Sqrt(x));
        }

        /// <summary>
        /// <paramref name="blockSize"/> 단위로 표준편차을 계산하여, 열거합니다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <param name="blockSize">표준편차 계산을 위한 변량 갯수</param>
        /// <returns>블럭별 표준편차</returns>
        public static IEnumerable<double?> StDev(this IEnumerable<int?> source, int blockSize = BlockSize) {
            return source.Variance(blockSize).Select(v => v.HasValue ? (double?)Math.Sqrt(v.Value) : null);
        }

        /// <summary>
        /// <paramref name="blockSize"/> 단위로 표준편차을 계산하여, 열거합니다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <param name="blockSize">표준편차 계산을 위한 변량 갯수</param>
        /// <param name="selector">항목중 변량 선택 함수</param>
        /// <returns>블럭별 표준편차</returns>
        public static IEnumerable<double> StDev<T>(this IEnumerable<T> source, int blockSize, Func<T, double> selector) {
            return source.Select(selector).StDev(blockSize);
        }

        /// <summary>
        /// <paramref name="blockSize"/> 단위로 표준편차을 계산하여, 열거합니다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <param name="blockSize">표준편차 계산을 위한 변량 갯수</param>
        /// <param name="selector">항목중 변량 선택 함수</param>
        /// <returns>블럭별 표준편차</returns>
        public static IEnumerable<double?> StDev<T>(this IEnumerable<T> source, int blockSize, Func<T, double?> selector) {
            return source.Select(selector).StDev(blockSize);
        }

        /// <summary>
        /// <paramref name="blockSize"/> 단위로 표준편차을 계산하여, 열거합니다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <param name="blockSize">표준편차 계산을 위한 변량 갯수</param>
        /// <param name="selector">항목중 변량 선택 함수</param>
        /// <returns>블럭별 표준편차</returns>
        public static IEnumerable<float> StDev<T>(this IEnumerable<T> source, int blockSize, Func<T, float> selector) {
            return source.Select(selector).StDev(blockSize);
        }

        /// <summary>
        /// <paramref name="blockSize"/> 단위로 표준편차을 계산하여, 열거합니다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <param name="blockSize">표준편차 계산을 위한 변량 갯수</param>
        /// <param name="selector">항목중 변량 선택 함수</param>
        /// <returns>블럭별 표준편차</returns>
        public static IEnumerable<float?> StDev<T>(this IEnumerable<T> source, int blockSize, Func<T, float?> selector) {
            return source.Select(selector).StDev(blockSize);
        }

        /// <summary>
        /// <paramref name="blockSize"/> 단위로 표준편차을 계산하여, 열거합니다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <param name="blockSize">표준편차 계산을 위한 변량 갯수</param>
        /// <param name="selector">항목중 변량 선택 함수</param>
        /// <returns>블럭별 표준편차</returns>
        public static IEnumerable<decimal> StDev<T>(this IEnumerable<T> source, int blockSize, Func<T, decimal> selector) {
            return source.Select(selector).StDev(blockSize);
        }

        /// <summary>
        /// <paramref name="blockSize"/> 단위로 표준편차을 계산하여, 열거합니다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <param name="blockSize">표준편차 계산을 위한 변량 갯수</param>
        /// <param name="selector">항목중 변량 선택 함수</param>
        /// <returns>블럭별 표준편차</returns>
        public static IEnumerable<decimal?> StDev<T>(this IEnumerable<T> source, int blockSize, Func<T, decimal?> selector) {
            return source.Select(selector).StDev(blockSize);
        }

        /// <summary>
        /// <paramref name="blockSize"/> 단위로 표준편차을 계산하여, 열거합니다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <param name="blockSize">표준편차 계산을 위한 변량 갯수</param>
        /// <param name="selector">항목중 변량 선택 함수</param>
        /// <returns>블럭별 표준편차</returns>
        public static IEnumerable<double> StDev<T>(this IEnumerable<T> source, int blockSize, Func<T, long> selector) {
            return source.Select(selector).StDev(blockSize);
        }

        /// <summary>
        /// <paramref name="blockSize"/> 단위로 표준편차을 계산하여, 열거합니다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <param name="blockSize">표준편차 계산을 위한 변량 갯수</param>
        /// <param name="selector">항목중 변량 선택 함수</param>
        /// <returns>블럭별 표준편차</returns>
        public static IEnumerable<double?> StDev<T>(this IEnumerable<T> source, int blockSize, Func<T, long?> selector) {
            return source.Select(selector).StDev(blockSize);
        }

        /// <summary>
        /// <paramref name="blockSize"/> 단위로 표준편차을 계산하여, 열거합니다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <param name="blockSize">표준편차 계산을 위한 변량 갯수</param>
        /// <param name="selector">항목중 변량 선택 함수</param>
        /// <returns>블럭별 표준편차</returns>
        public static IEnumerable<double> StDev<T>(this IEnumerable<T> source, int blockSize, Func<T, int> selector) {
            return source.Select(selector).StDev(blockSize);
        }

        /// <summary>
        /// <paramref name="blockSize"/> 단위로 표준편차을 계산하여, 열거합니다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <param name="blockSize">표준편차 계산을 위한 변량 갯수</param>
        /// <param name="selector">항목중 변량 선택 함수</param>
        /// <returns>블럭별 표준편차</returns>
        public static IEnumerable<double?> StDev<T>(this IEnumerable<T> source, int blockSize, Func<T, int?> selector) {
            return source.Select(selector).StDev(blockSize);
        }

        #endregion
    }
}