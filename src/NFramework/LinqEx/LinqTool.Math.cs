using System;
using System.Collections.Generic;
using System.Linq;

namespace NSoft.NFramework.LinqEx {
    public static partial class LinqTool {
        /// <summary>
        /// Norm ( 요소의 제곱의 합 )을 구한다. (벡터의 길이, 차원의 가장 짧은 거리)
        /// </summary>
        /// <param name="source">변량의 시퀀스</param>
        /// <returns>Norm</returns>
        public static double AsNorm(this IEnumerable<double> source) {
            source.ShouldNotBeNull("source");
            return source.Sum(x => x * x);
        }

        /// <summary>
        /// 지정된 시퀀스를 Normalize 한다. norm[i] = item[i] / sum(items) 이다.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<double> AsNormalize(this IEnumerable<double> source) {
            source.ShouldNotBeNull("source");

            var sum = source.Sum();

            if(Math.Abs(sum - 0.0) <= double.Epsilon)
                yield break;

            foreach(var v in source)
                yield return v / sum;
        }

        /// <summary>
        /// 변량들의 표준편차를 구한다.
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <returns>표준편차</returns>
        public static double AsStDev(this IEnumerable<double> source) {
            return Math.Sqrt(source.AsVariance());
        }

        /// <summary>
        /// 시퀀스의 분산값을 구한다
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static double AsVariance(this IEnumerable<double> source) {
            source.ShouldNotBeNull("source");

            double avg = 0;
            double variance = 0;
            long n = 0;

            using(var iter = source.GetEnumerator())
                while(iter.MoveNext()) {
                    var x = iter.Current;
                    n++;
                    x -= avg;
                    avg += x / n;
                    variance += (n - 1) * x * x / n;
                }

            return (n > 1) ? variance / (n - 1) : 0;
        }

        /// <summary>
        /// 제곱 평균 (root-mean-square) - 표준편차와 같은 값이다.
        /// 참고: http://en.wikipedia.org/wiki/Root_mean_square
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static double AsRootMeanSquare(this IEnumerable<double> source) {
            double rms = 0.0;
            long n = 0;

            foreach(var x in source) {
                rms += x * x;
                n++;
            }

            return (n > 1) ? Math.Sqrt(rms / (n - 1)) : 0;
        }

        /// <summary>
        /// 제곱 평균 (root-mean-square) - 표준편차와 같은 값이다.
        /// 참고: http://en.wikipedia.org/wiki/Root_mean_square
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static double AsRootMeanSquare(this IEnumerable<double?> source) {
            double rms = 0.0;
            long n = 0;

            foreach(var x in source.Where(x => x.HasValue)) {
                rms += x.Value * x.Value;
                n++;
            }

            return (n > 1) ? Math.Sqrt(rms / (n - 1)) : 0;
        }
    }
}