using System;
using System.Collections.Generic;
using System.Linq;

namespace NSoft.NFramework.Numerics {
    public static partial class MathTool {
        /// <summary>
        /// 변량의 분포의 대칭성을 나타냅니다. 0 을 기준으로 좌우로 기울어져 분포하는 것을 표현합니다.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static double Skewness(this IEnumerable<double> source) {
            source.ShouldNotBeNull("source");
            source.Take(3).Count().ShouldBeGreaterOrEqual(3, "count of source");

            double sum = 0.0;
            double avg = source.Average();
            double stdev = source.StDev();
            long n = 0;

            using(var iter = source.GetEnumerator())
                while(iter.MoveNext()) {
                    var x = iter.Current;
                    sum += Math.Pow((x - avg) / stdev, 3);
                    n++;
                }

            if(n < 3)
                return 0.0;

            return sum * n / (n - 1) / (n - 2);
        }

        /// <summary>
        /// 변량의 분포의 대칭성을 나타냅니다. 0 을 기준으로 좌우로 기울어져 분포하는 것을 표현합니다.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static double? Skewness(this IEnumerable<double?> source) {
            source.ShouldNotBeNull("source");
            source.Take(3).Count().ShouldBeGreaterOrEqual(3, "count of source");

            double sum = 0.0;
            double avg = source.Average().GetValueOrDefault();
            double stdev = source.StDev().GetValueOrDefault();
            long n = 0;

            using(var iter = source.GetEnumerator())
                while(iter.MoveNext())
                    if(iter.Current.HasValue) {
                        var x = iter.Current.Value;
                        sum += Math.Pow((x - avg) / stdev, 3);
                        n++;
                    }

            if(n < 3)
                return null;

            return sum * n / (n - 1) / (n - 2);
        }

        /// <summary>
        /// 변량의 분포의 대칭성을 나타냅니다. 0 을 기준으로 좌우로 기울어져 분포하는 것을 표현합니다.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static float Skewness(this IEnumerable<float> source) {
            return (float)source.Select(x => (double)x).Skewness();
        }

        /// <summary>
        /// 변량의 분포의 대칭성을 나타냅니다. 0 을 기준으로 좌우로 기울어져 분포하는 것을 표현합니다.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static float? Skewness(this IEnumerable<float?> source) {
            return (float?)source.Select(x => (double?)x).Skewness();
        }

        /// <summary>
        /// 변량의 분포의 대칭성을 나타냅니다. 0 을 기준으로 좌우로 기울어져 분포하는 것을 표현합니다.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static decimal Skewness(this IEnumerable<decimal> source) {
            return (decimal)source.Select(x => (double)x).Skewness();
        }

        /// <summary>
        /// 변량의 분포의 대칭성을 나타냅니다. 0 을 기준으로 좌우로 기울어져 분포하는 것을 표현합니다.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static decimal? Skewness(this IEnumerable<decimal?> source) {
            return (decimal?)source.Select(x => (double?)x).Skewness();
        }

        /// <summary>
        /// 변량의 분포의 대칭성을 나타냅니다. 0 을 기준으로 좌우로 기울어져 분포하는 것을 표현합니다.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static double Skewness(this IEnumerable<long> source) {
            return source.Select(x => (double)x).Skewness();
        }

        /// <summary>
        /// 변량의 분포의 대칭성을 나타냅니다. 0 을 기준으로 좌우로 기울어져 분포하는 것을 표현합니다.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static double? Skewness(this IEnumerable<long?> source) {
            return source.Select(x => (double?)x).Skewness();
        }

        /// <summary>
        /// 변량의 분포의 대칭성을 나타냅니다. 0 을 기준으로 좌우로 기울어져 분포하는 것을 표현합니다.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static double Skewness(this IEnumerable<int> source) {
            return source.Select(x => (double)x).Skewness();
        }

        /// <summary>
        /// 변량의 분포의 대칭성을 나타냅니다. 0 을 기준으로 좌우로 기울어져 분포하는 것을 표현합니다.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static double? Skewness(this IEnumerable<int?> source) {
            return source.Select(x => (double?)x).Skewness();
        }
    }
}