using System.Collections.Generic;
using System.Linq;

namespace NSoft.NFramework.Numerics {
    public static partial class MathTool {
        /// <summary>
        /// 지정된 시퀀스를 Normalize 한다. norm[i] = item[i] / sum(items) 이다.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<double> Normalize(this IEnumerable<double> source) {
            source.ShouldNotBeNull("source");

            var sum = source.Sum();

            if(sum.ApproximateEqual(0.0))
                yield break;

            foreach(var v in source)
                yield return v / sum;
        }

        /// <summary>
        /// 지정된 시퀀스를 Normalize 한다. norm[i] = item[i] / sum(items) 이다.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<double?> Normalize(this IEnumerable<double?> source) {
            source.ShouldNotBeNull("source");
            var sum = source.Sum();

            if(sum.ApproximateEqual(0.0))
                yield break;

            foreach(var v in source)
                yield return (v.HasValue) ? v.Value / sum : null;
        }

        /// <summary>
        /// 지정된 시퀀스를 Normalize 한다. norm[i] = item[i] / sum(items) 이다.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<float> Normalize(this IEnumerable<float> source) {
            source.ShouldNotBeNull("source");

            var sum = source.Sum();

            if(sum.ApproximateEqual(0.0f))
                yield break;

            foreach(var v in source)
                yield return v / sum;
        }

        /// <summary>
        /// 지정된 시퀀스를 Normalize 한다. norm[i] = item[i] / sum(items) 이다.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<float?> Normalize(this IEnumerable<float?> source) {
            source.ShouldNotBeNull("source");
            var sum = source.Sum();

            if(sum.ApproximateEqual(0.0f))
                yield break;

            foreach(var v in source)
                yield return (v.HasValue) ? v.Value / sum : null;
        }

        /// <summary>
        /// 지정된 시퀀스를 Normalize 한다. norm[i] = item[i] / sum(items) 이다.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<decimal> Normalize(this IEnumerable<decimal> source) {
            source.ShouldNotBeNull("source");

            var sum = source.Sum();

            if(sum.ApproximateEqual(0.0m))
                yield break;

            foreach(var v in source)
                yield return v / sum;
        }

        /// <summary>
        /// 지정된 시퀀스를 Normalize 한다. norm[i] = item[i] / sum(items) 이다.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<decimal?> Normalize(this IEnumerable<decimal?> source) {
            source.ShouldNotBeNull("source");
            var sum = source.Sum();

            if(sum.ApproximateEqual(0.0m))
                yield break;

            foreach(var v in source)
                yield return (v.HasValue) ? v.Value / sum : null;
        }

        /// <summary>
        /// 지정된 시퀀스를 Normalize 한다. norm[i] = item[i] / sum(items) 이다.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<double> Normalize(this IEnumerable<long> source) {
            source.ShouldNotBeNull("source");
            double sum = source.Sum();

            if(sum.ApproximateEqual(0.0))
                yield break;

            foreach(var v in source)
                yield return v / sum;
        }

        /// <summary>
        /// 지정된 시퀀스를 Normalize 한다. norm[i] = item[i] / sum(items) 이다.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<double?> Normalize(this IEnumerable<long?> source) {
            source.ShouldNotBeNull("source");
            double? sum = source.Sum();

            if(sum.ApproximateEqual(0.0))
                yield break;

            foreach(var v in source)
                yield return v.HasValue ? v.Value / sum : null;
        }

        /// <summary>
        /// 지정된 시퀀스를 Normalize 한다. norm[i] = item[i] / sum(items) 이다.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<double> Normalize(this IEnumerable<int> source) {
            source.ShouldNotBeNull("source");
            double sum = source.LongSum();

            if(sum.ApproximateEqual(0.0))
                yield break;

            foreach(var v in source)
                yield return v / sum;
        }

        /// <summary>
        /// 지정된 시퀀스를 Normalize 한다. norm[i] = item[i] / sum(items) 이다.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<double?> Normalize(this IEnumerable<int?> source) {
            source.ShouldNotBeNull("source");
            double? sum = source.LongSum();

            if(sum.ApproximateEqual(0.0))
                yield break;

            foreach(var v in source)
                yield return v.HasValue ? v.Value / sum : null;
        }
    }
}