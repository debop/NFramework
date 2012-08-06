using System;
using System.Collections.Generic;
using System.Linq;

namespace NSoft.NFramework.Numerics {
    public static partial class MathTool {
        /// <summary>
        /// 변량 분포의 첨예도를 나타냅니다. 값이 작을 수록 뾰족한 분포이고, 값이 클수록 언덕 분포입니다.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static double Kurtosis(this IEnumerable<double> source) {
            source.ShouldNotBeNull("source");
            source.Take(4).Count().ShouldBeGreaterOrEqual(4, "count of source");

            double sum = 0.0;
            double avg = source.Average();
            double stdev = source.StDev();
            long n = 0;

            using(var iter = source.GetEnumerator())
                while(iter.MoveNext()) {
                    var x = iter.Current;
                    sum += Math.Pow((x - avg) / stdev, 4);
                    n++;
                }

            if(n < 4)
                return 0.0;

            return sum * n * (n + 1) / (n - 1) / (n - 2) / (n - 3) - 3.0 * (n - 1) * (n - 1) / (n - 2) / (n - 3);
        }

        /// <summary>
        /// 변량 분포의 첨예도를 나타냅니다. 값이 작을 수록 뾰족한 분포이고, 값이 클수록 언덕 분포입니다.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static double? Kurtosis(this IEnumerable<double?> source) {
            source.ShouldNotBeNull("source");
            source.Take(4).Count().ShouldBeGreaterOrEqual(4, "count of source");

            double sum = 0.0;
            double avg = source.Average().GetValueOrDefault();
            double stdev = source.StDev().GetValueOrDefault();
            long n = 0;

            using(var iter = source.GetEnumerator())
                while(iter.MoveNext())
                    if(iter.Current.HasValue) {
                        var x = iter.Current.Value;
                        sum += Math.Pow((x - avg) / stdev, 4);
                        n++;
                    }

            if(n < 4)
                return null;

            return sum * n * (n + 1) / (n - 1) / (n - 2) / (n - 3) - 3.0 * (n - 1) * (n - 1) / (n - 2) / (n - 3);
        }

        /// <summary>
        /// 변량 분포의 첨예도를 나타냅니다. 값이 작을 수록 뾰족한 분포이고, 값이 클수록 언덕 분포입니다.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static float Kurtosis(this IEnumerable<float> source) {
            return (float)source.Select(x => (double)x).Kurtosis();
        }

        /// <summary>
        /// 변량 분포의 첨예도를 나타냅니다. 값이 작을 수록 뾰족한 분포이고, 값이 클수록 언덕 분포입니다.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static float? Kurtosis(this IEnumerable<float?> source) {
            var result = source.Select(x => (double?)x).Kurtosis();
            return (result.HasValue) ? (float?)result : null;
        }

        /// <summary>
        /// 변량 분포의 첨예도를 나타냅니다. 값이 작을 수록 뾰족한 분포이고, 값이 클수록 언덕 분포입니다.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static decimal Kurtosis(this IEnumerable<decimal> source) {
            return (decimal)source.Select(x => (double)x).Kurtosis();
        }

        /// <summary>
        /// 변량 분포의 첨예도를 나타냅니다. 값이 작을 수록 뾰족한 분포이고, 값이 클수록 언덕 분포입니다.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static decimal? Kurtosis(this IEnumerable<decimal?> source) {
            var result = source.Select(x => (double?)x).Kurtosis();
            return (result.HasValue) ? (decimal?)result : null;
        }

        /// <summary>
        /// 변량 분포의 첨예도를 나타냅니다. 값이 작을 수록 뾰족한 분포이고, 값이 클수록 언덕 분포입니다.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static double Kurtosis(this IEnumerable<long> source) {
            return source.Select(x => (double)x).Kurtosis();
        }

        /// <summary>
        /// 변량 분포의 첨예도를 나타냅니다. 값이 작을 수록 뾰족한 분포이고, 값이 클수록 언덕 분포입니다.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static double? Kurtosis(this IEnumerable<long?> source) {
            return source.Select(x => (double?)x).Kurtosis();
        }

        /// <summary>
        /// 변량 분포의 첨예도를 나타냅니다. 값이 작을 수록 뾰족한 분포이고, 값이 클수록 언덕 분포입니다.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static double Kurtosis(this IEnumerable<int> source) {
            return source.Select(x => (double)x).Kurtosis();
        }

        /// <summary>
        /// 변량 분포의 첨예도를 나타냅니다. 값이 작을 수록 뾰족한 분포이고, 값이 클수록 언덕 분포입니다.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static double? Kurtosis(this IEnumerable<int?> source) {
            return source.Select(x => (double?)x).Kurtosis();
        }
    }
}