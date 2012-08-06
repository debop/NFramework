using System;
using System.Collections.Generic;
using System.Linq;
using NSoft.NFramework.LinqEx;

namespace NSoft.NFramework.Numerics {
    public static partial class MathTool {
        /// <summary>
        /// 변량들의 순위 매기기, 점수 배열에 대한 순위를 매긴다. 가장 큰 수가 1등이 된다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static int[] Ranking<T>(this IEnumerable<T> source) where T : struct, IComparable<T> {
            source.ShouldNotBeNull("source");

            var items = source.ToList();
            var ranks = new int[items.Count];

            if(ranks.Length == 0)
                return ranks;

            int N = ranks.Length;
            for(int i = 0; i < N; i++) {
                ranks[i] = 1;
                for(int j = 0; j < N; j++) {
                    if(LinqTool.Operators<T>.GreaterThan(items[j], items[i]))
                        ranks[i]++;

                    //if(new NumberWrapper<T>(items[j]) > new NumberWrapper<T>(items[i]))
                    //    ranks[i]++;
                }
            }

            return ranks;
        }
    }
}