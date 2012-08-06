using System;
using System.Collections.Generic;
using System.Linq;

namespace NSoft.NFramework.Numerics {
    public static partial class MathTool {
        /// <summary>
        /// Sqrt of 2 * PI
        /// </summary>
        public static readonly double SqrtOfTwoPI = Math.Sqrt(2.0 * Pi);

        /// <summary>
        /// 해당 평균, 표준편차를 가지는 정규분포에서의 x 지점에서의 확률을 구한다.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="avg"></param>
        /// <param name="stdev"></param>
        /// <returns></returns>
        public static double NormalDensity(this double x, double avg, double stdev) {
            return Math.Exp(-(Math.Pow((x - avg) / stdev, 2) / 2)) / SqrtOfTwoPI / stdev;
        }

        /// <summary>
        /// 해당 평균, 표준편차를 가지는 정규분포에서의 확률을 구한다.
        /// </summary>
        /// <param name="source">확률을 구하고자 하는 지점의 시퀀스</param>
        /// <param name="avg"></param>
        /// <param name="stdev"></param>
        /// <returns></returns>
        public static IEnumerable<double> NormalDensity(this IEnumerable<double> source, double avg, double stdev) {
            source.ShouldNotBeNull("source");

            return
                source
                    .AsParallel()
                    .AsOrdered()
                    .Select(v => v.NormalDensity(avg, stdev));
        }

        /// <summary>
        /// 해당 평균, 표준편차를 가지는 정규분포에서의 확률을 구한다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <param name="avg"></param>
        /// <param name="stdev"></param>
        /// <returns></returns>
        public static IEnumerable<double> NormalDensity<T>(this IEnumerable<T> source, Func<T, double> selector, double avg,
                                                           double stdev) {
            return source.Select(selector).NormalDensity(avg, stdev);
        }

        /// <summary>
        /// <paramref name="source"/> 항목의 빈도 수를 Dictionary 형태로 제공합니다.
        /// </summary>
        /// <typeparam name="T">항목의 수형</typeparam>
        /// <param name="source">시퀀스</param>
        /// <returns>시퀀스의 {항목,빈도수} 정보</returns>
        public static IEnumerable<KeyValuePair<T, int>> Frequency<T>(this IEnumerable<T> source) {
            return source.GroupBy(x => x).Select(k => new KeyValuePair<T, int>(k.Key, k.Count()));
        }

        /// <summary>
        /// <paramref name="source"/>의 항목들을 대표값(<paramref name="buckets"/>)으로 그룹핑하여 빈도수를 나타냅니다. (히스토그램처럼)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TBucket"></typeparam>
        /// <param name="source"></param>
        /// <param name="buckets"></param>
        /// <param name="bucketSelector"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<TBucket, int>> Frequency<T, TBucket>(this IEnumerable<T> source,
                                                                                    IEnumerable<TBucket> buckets,
                                                                                    Func<T, IEnumerable<TBucket>, TBucket>
                                                                                        bucketSelector) {
            source.ShouldNotBeNull("source");
            buckets.ShouldNotBeNull("buckets");
            bucketSelector.ShouldNotBeNull("bucketSelector");

            var rawFrequency =
                source.GroupBy(x => bucketSelector(x, buckets)).Select(k => new KeyValuePair<TBucket, int>(k.Key, k.Count()));
            //var freq = from bucket in buckets
            //           join freqItem in rawFrequency on bucket equals freqItem.Key into freqBucketList
            //           from item in freqBucketList.DefaultIfEmpty(new KeyValuePair<TBucket, int>(bucket, 0))
            //           select item;
            var freq = buckets
                .GroupJoin(rawFrequency, bucket => bucket, freqItem => freqItem.Key,
                           (bucket, freqBucketList) => new { bucket, freqBucketList })
                .SelectMany(@t => @t.freqBucketList.DefaultIfEmpty(new KeyValuePair<TBucket, int>(@t.bucket, 0)));

            return freq;
        }

        /// <summary>
        /// <paramref name="source"/>의 항목들을 <paramref name="bucketSelector"/>를 통해 그룹핑하여 빈도수를 나타냅니다. (히스토그램 처럼)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TBucket"></typeparam>
        /// <param name="source"></param>
        /// <param name="bucketSelector"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<TBucket, int>> Frequency<T, TBucket>(this IEnumerable<T> source,
                                                                                    Func<T, TBucket> bucketSelector) {
            source.ShouldNotBeNull("source");
            bucketSelector.ShouldNotBeNull("bucketSelector");

            return source.GroupBy(x => bucketSelector(x)).Select(g => new KeyValuePair<TBucket, int>(g.Key, g.Count()));
        }

        /// <summary>
        /// 항목(<paramref name="item"/>)의 빈도 수 / 전체 항목 수
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">시퀀스</param>
        /// <param name="item">비교 항목</param>
        /// <returns>항목 수 / 전체 항목 수</returns>
        public static double Probability<T>(this IEnumerable<T> source, T item) {
            return Probability<T>(source, item, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// 항목(<paramref name="item"/>)의 빈도 수 / 전체 항목 수
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">시퀀스</param>
        /// <param name="item">비교 항목</param>
        /// <param name="comparer">값 비교자</param>
        /// <returns>항목 수 / 전체 항목 수</returns>
        public static double Probability<T>(this IEnumerable<T> source, T item, IEqualityComparer<T> comparer) {
            source.ShouldNotBeNull("source");
            source.ShouldNotBeEmpty("source");

            double totalCount = source.Count();
            double matches = source.Where(x => comparer.Equals(x, item)).Count();

            return matches / totalCount;
        }
    }
}