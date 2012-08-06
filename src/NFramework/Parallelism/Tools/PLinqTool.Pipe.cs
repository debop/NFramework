using System;
using System.Collections.Generic;
using System.Linq;

namespace NSoft.NFramework.Parallelism.Tools {
    public static partial class PLinqTool {
        /// <summary>
        /// <paramref name="source"/>의 항목들을 <paramref name="action"/>의 인자로 전달하여 수행하고, 항목을 다시 열거합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IEnumerable<T> PipeAsParallel<T>(this IEnumerable<T> source, Action<T> @action) {
            action.ShouldNotBeNull("action");

            return
                source
                    .AsParallel()
                    .AsOrdered()
                    .Select(x => {
                                @action(x);
                                return x;
                            });
        }

        /// <summary>
        /// <paramref name="source"/>의 항목들을 <paramref name="func"/>의 인자로 전달하여 수행하고, 결과를 열거합니다.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="source"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static IEnumerable<TTarget> PipeAsParallel<TSource, TTarget>(this IEnumerable<TSource> source,
                                                                            Func<TSource, TTarget> @func) {
            func.ShouldNotBeNull("func");
            return
                source
                    .AsParallel()
                    .AsOrdered()
                    .Select(x => func(x));
        }
    }
}