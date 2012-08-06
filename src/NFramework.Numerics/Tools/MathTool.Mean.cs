using System;
using System.Collections.Generic;
using System.Linq;

namespace NSoft.NFramework.Numerics {
    public static partial class MathTool {
        /// <summary>
        /// 평균
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static double Mean(this IEnumerable<double> source) {
            source.ShouldNotBeNull("source");

            var n = 0L;
            var mean = 0.0;

            foreach(var x in source)
                mean += (x - mean) / ++n;

            return mean;
        }

        /// <summary>
        /// 평균
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static double Mean(this IEnumerable<double?> source) {
            source.ShouldNotBeNull("source");

            double mean = 0.0;
            long m = 0;

            foreach(var x in source.Where(x => x.HasValue))
                mean += (x.Value - mean) / ++m;

            return mean;
        }

        /// <summary>
        /// 평균
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static float Mean(this IEnumerable<float> source) {
            source.ShouldNotBeNull("source");

            float mean = 0.0f;
            long n = 0;

            foreach(var x in source)
                mean += (x - mean) / ++n;

            return mean;
        }

        /// <summary>
        /// 평균
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static float Mean(this IEnumerable<float?> source) {
            source.ShouldNotBeNull("source");

            float mean = 0.0f;
            long m = 0;

            foreach(var x in source.Where(x => x.HasValue))
                mean += (x.Value - mean) / ++m;

            return mean;
        }

        /// <summary>
        /// 평균
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static decimal Mean(this IEnumerable<decimal> source) {
            source.ShouldNotBeNull("source");

            decimal mean = 0.0m;
            long n = 0;

            foreach(var x in source)
                mean += (x - mean) / ++n;

            return mean;
        }

        /// <summary>
        /// 평균
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static decimal Mean(this IEnumerable<decimal?> source) {
            source.ShouldNotBeNull("source");

            decimal mean = 0.0m;
            long m = 0;

            foreach(var x in source.Where(x => x.HasValue))
                mean += (x.Value - mean) / ++m;

            return mean;
        }

        /// <summary>
        /// 평균
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static double Mean(this IEnumerable<long> source) {
            source.ShouldNotBeNull("source");

            double mean = 0.0;
            long n = 0;

            foreach(var x in source)
                mean += (x - mean) / ++n;

            return mean;
        }

        /// <summary>
        /// 평균
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static double Mean(this IEnumerable<long?> source) {
            source.ShouldNotBeNull("source");

            double mean = 0.0;
            long m = 0;

            foreach(var x in source.Where(x => x.HasValue))
                mean += (x.Value - mean) / ++m;

            return mean;
        }

        /// <summary>
        /// 평균
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static double Mean(this IEnumerable<int> source) {
            source.ShouldNotBeNull("source");

            double mean = 0.0;
            long n = 0;

            foreach(var x in source)
                mean += (x - mean) / ++n;

            return mean;
        }

        /// <summary>
        /// 평균
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static double Mean(this IEnumerable<int?> source) {
            source.ShouldNotBeNull("source");

            double mean = 0.0;
            long m = 0;

            foreach(var x in source.Where(x => x.HasValue))
                mean += (x.Value - mean) / ++m;

            return mean;
        }

        //! =====================================================

        /// <summary>
        /// 기하평균, 참고 : http://en.wikipedia.org/wiki/Mean
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static double GeometricMean(this IEnumerable<double> source) {
            double mean = 0.0;
            long n = 0;
            foreach(var x in source) {
                mean *= x;
                n++;
            }
            return Math.Pow(mean, 1.0 / n);
        }

        /// <summary>
        /// 기하평균, 참고 : http://en.wikipedia.org/wiki/Mean
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static double GeometricMean(this IEnumerable<double?> source) {
            double mean = 0.0;
            long n = 0;

            foreach(var x in source.Where(x => x.HasValue)) {
                mean *= x.Value;
                n++;
            }
            return Math.Pow(mean, 1.0 / n);
        }

        /// <summary>
        /// 기하평균, 참고 : http://en.wikipedia.org/wiki/Mean
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static float GeometricMean(this IEnumerable<float> source) {
            float mean = 0.0f;
            long n = 0;
            foreach(var x in source) {
                mean *= x;
                n++;
            }
            return (float)Math.Pow(mean, 1.0 / n);
        }

        /// <summary>
        /// 기하평균, 참고 : http://en.wikipedia.org/wiki/Mean
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static float GeometricMean(this IEnumerable<float?> source) {
            float mean = 0.0f;
            long n = 0;

            foreach(var x in source.Where(x => x.HasValue)) {
                mean *= x.Value;
                n++;
            }
            return (float)Math.Pow(mean, 1.0 / n);
        }

        /// <summary>
        /// 기하평균, 참고 : http://en.wikipedia.org/wiki/Mean
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static decimal GeometricMean(this IEnumerable<decimal> source) {
            decimal mean = 0.0m;
            long n = 0;
            foreach(var x in source) {
                mean *= x;
                n++;
            }
            return (decimal)Math.Pow((double)mean, 1.0 / n);
        }

        /// <summary>
        /// 기하평균, 참고 : http://en.wikipedia.org/wiki/Mean
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static decimal GeometricMean(this IEnumerable<decimal?> source) {
            decimal mean = 0.0m;
            long n = 0;

            foreach(var x in source.Where(x => x.HasValue)) {
                mean *= x.Value;
                n++;
            }
            return (decimal)Math.Pow((double)mean, 1.0 / n);
        }

        //! =====================================================

        /// <summary>
        /// 조화평균, 참고 : http://en.wikipedia.org/wiki/Mean
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static double HarmonicMean(this IEnumerable<double> source) {
            double mean = 0.0;
            long n = 0;

            foreach(var x in source) {
                n++;
                mean += 1.0 / x;
            }
            return (n / mean);
        }

        /// <summary>
        /// 조화평균, 참고 : http://en.wikipedia.org/wiki/Mean
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static double HarmonicMean(this IEnumerable<double?> source) {
            double mean = 0.0;
            long n = 0;

            foreach(var x in source.Where(x => x.HasValue)) {
                n++;
                mean += 1.0 / x.Value;
            }
            return (n / mean);
        }

        /// <summary>
        /// 조화평균, 참고 : http://en.wikipedia.org/wiki/Mean
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static float HarmonicMean(this IEnumerable<float> source) {
            float mean = 0.0f;
            long n = 0;

            foreach(var x in source) {
                n++;
                mean += 1.0f / x;
            }
            return (n / mean);
        }

        /// <summary>
        /// 조화평균, 참고 : http://en.wikipedia.org/wiki/Mean
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static float HarmonicMean(this IEnumerable<float?> source) {
            float mean = 0.0f;
            long n = 0;

            foreach(var x in source.Where(x => x.HasValue)) {
                n++;
                mean += 1.0f / x.Value;
            }
            return (n / mean);
        }

        /// <summary>
        /// 조화평균, 참고 : http://en.wikipedia.org/wiki/Mean
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static decimal HarmonicMean(this IEnumerable<decimal> source) {
            decimal mean = 0.0m;
            long n = 0;

            foreach(var x in source) {
                n++;
                mean += 1.0m / x;
            }
            return (n / mean);
        }

        /// <summary>
        /// 조화평균, 참고 : http://en.wikipedia.org/wiki/Mean
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static decimal HarmonicMean(this IEnumerable<decimal?> source) {
            decimal mean = 0.0m;
            long n = 0;

            foreach(var x in source.Where(x => x.HasValue)) {
                n++;
                mean += 1.0m / x.Value;
            }
            return (n / mean);
        }
    }
}