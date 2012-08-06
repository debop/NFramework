using System;
using System.Collections.Generic;
using System.Linq;

namespace NSoft.NFramework.Numerics {
    public static partial class MathTool {
        /// <summary>
        /// 시퀀스 항목 값을 올림합니다.
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <returns></returns>
        public static IEnumerable<double> Ceiling(this IEnumerable<double> source) {
            return source.Select(x => Math.Ceiling(x));
        }

        /// <summary>
        /// 시퀀스 항목 값을 올림합니다.
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <returns></returns>
        public static IEnumerable<float> Ceiling(this IEnumerable<float> source) {
            return source.Select(x => (float)Math.Ceiling(x));
        }

        /// <summary>
        /// 시퀀스 항목 값을 올림합니다.
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <returns></returns>
        public static IEnumerable<decimal> Ceiling(this IEnumerable<decimal> source) {
            return source.Select(x => Math.Ceiling(x));
        }

        //! =====================================================

        /// <summary>
        /// 시퀀스 항목 값을 버림을 수행합니다.
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <returns></returns>
        public static IEnumerable<double> Floor(this IEnumerable<double> source) {
            return source.Select(x => Math.Truncate(x));
        }

        /// <summary>
        /// 시퀀스 항목 값을 버림을 수행합니다.
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <returns></returns>
        public static IEnumerable<float> Floor(this IEnumerable<float> source) {
            return source.Select(x => (float)Math.Truncate(x));
        }

        /// <summary>
        /// 시퀀스 항목 값을 버림을 수행합니다.
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <returns></returns>
        public static IEnumerable<decimal> Floor(this IEnumerable<decimal> source) {
            return source.Select(x => Math.Truncate(x));
        }

        //! =====================================================

        /// <summary>
        /// 시퀀스 항목 값을 지정된 소수 자릿수로 반올림합니다.
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <param name="digits">소수 자릿 수</param>
        /// <returns></returns>
        public static IEnumerable<double> Round(this IEnumerable<double> source, int digits = 0) {
            return source.Select(x => Math.Round(x, digits));
        }

        /// <summary>
        /// 시퀀스 항목 값을 지정된 소수 자릿수로 반올림합니다.
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <param name="digits">소수 자릿 수</param>
        /// <returns></returns>
        public static IEnumerable<float> Round(this IEnumerable<float> source, int digits = 0) {
            return source.Select(x => (float)Math.Round(x, digits));
        }

        /// <summary>
        /// 시퀀스 항목 값을 지정된 소수 자릿수로 반올림합니다.
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <param name="digits">소수 자릿 수</param>
        /// <returns></returns>
        public static IEnumerable<decimal> Round(this IEnumerable<decimal> source, int digits = 0) {
            return source.Select(x => Math.Round(x, digits));
        }
    }
}