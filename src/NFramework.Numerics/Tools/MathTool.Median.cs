using System.Collections.Generic;
using System.Linq;

namespace NSoft.NFramework.Numerics {
    public static partial class MathTool {
        /// <summary>
        /// 중앙값
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <returns></returns>
        public static double Median(this IEnumerable<double> source) {
            var count = source.Count();
            var index = (count / 2) + 1;

            // 변량이 짝수라면, 중앙값 두개의 평균을 반환합니다.
            if(count % 2 == 0) {
                var ordered = source.OrderBy(x => x).ToList();
                var lower = ordered[index - 1];
                var upper = ordered[index];

                return (lower + upper) / 2.0;
            }

            return source.OrderBy(x => x).ElementAt(index);
        }

        /// <summary>
        /// 중앙값
        /// </summary>
        /// <param name="source">변량 시퀀스</param>
        /// <returns></returns>
        public static double Median(this IEnumerable<double?> source) {
            return source.Where(x => x.HasValue).Select(x => x.Value).Median();
        }
    }
}