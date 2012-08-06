using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NSoft.NFramework.Reflections;

namespace NSoft.NFramework.Parallelism.Tools {
    /// <summary>
    /// Parallel LINQ Extensions
    /// </summary>
    public static partial class PLinqTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 정렬이 되었다면, 지정된 갯수의 최상위 요소중 <paramref name="count"/> 수만큼 반환합니다.
        /// </summary>
        /// <typeparam name="TSource">요소의 수형</typeparam>
        /// <typeparam name="TKey">요소 비교를 위한 Key의 수형</typeparam>
        /// <param name="source">요소 집합</param>
        /// <param name="keySelector">요소로부터 Key를 추출하는 Selector</param>
        /// <param name="count">취할 요소의 수</param>
        /// <returns></returns>
        public static IEnumerable<TSource> TakeTop<TSource, TKey>(this ParallelQuery<TSource> source,
                                                                  Func<TSource, TKey> keySelector,
                                                                  int count = 1) {
            keySelector.ShouldNotBeNull("keySelector");

            if(IsDebugEnabled)
                log.Debug("최상의 요소 중 [{0}]만큼 열거합니다.", count);

            var comparer = new DescendingDefaultComparer<TKey>();

            return
                source.Aggregate(() => new SortedTopN<TKey, TSource>(count, comparer),
                                 (accum, item) => {
                                     accum.Add(keySelector(item), item);
                                     return accum;
                                 },
                                 (accum1, accum2) => {
                                     foreach(var item in accum2)
                                         accum1.Add(item);
                                     return accum1;
                                 },
                                 accum => accum.Values);
        }

        /// <summary>
        /// A comparer that comparers using the inverse of the default comparer.
        /// </summary>
        /// <typeparam name="T">Specifies the type being compared.</typeparam>
        private class DescendingDefaultComparer<T> : IComparer<T> {
            private static readonly Comparer<T> _defaultComparer = Comparer<T>.Default;

            public int Compare(T x, T y) {
                return _defaultComparer.Compare(y, x);
            }
        }

        /// <summary>
        /// 병렬로 Map-Reduce를 수행합니다.
        /// </summary>
        /// <typeparam name="TSource">souece 요소의 수형</typeparam>
        /// <typeparam name="TMapped">mapped 요소의 수형</typeparam>
        /// <typeparam name="TKey">key 수형</typeparam>
        /// <typeparam name="TResult">결과 시퀀스의 요소의 수형</typeparam>
        /// <param name="source">원본 시퀀스</param>
        /// <param name="mapper">source 요소를 mapped 요소로 변환하는 함수</param>
        /// <param name="keySelector">mapped 요소로부터 key 를 선택하는 함수, 그룹핑을 위해 사용된다.</param>
        /// <param name="reducer">key로 그룹핑된 정보를 바탕으로 결과를 도출하는 함수</param>
        /// <returns></returns>
        public static ParallelQuery<TResult> MapReduce<TSource, TMapped, TKey, TResult>(this ParallelQuery<TSource> source,
                                                                                        Func<TSource, TMapped> mapper,
                                                                                        Func<TMapped, TKey> keySelector,
                                                                                        Func<IGrouping<TKey, TMapped>, TResult> reducer) {
            return
                source
                    .Select(mapper)
                    .GroupBy(keySelector)
                    .Select(reducer);
        }

        /// <summary>
        /// 병렬로 Map-Reduce를 수행합니다.
        /// </summary>
        /// <typeparam name="TSource">souece 요소의 수형</typeparam>
        /// <typeparam name="TMapped">mapped 요소의 수형</typeparam>
        /// <typeparam name="TKey">key 수형</typeparam>
        /// <typeparam name="TResult">결과 시퀀스의 요소의 수형</typeparam>
        /// <param name="source">원본 시퀀스</param>
        /// <param name="mapper">원본 요소로부터 mapped 요소의 시퀀스로 변환하는 mapper</param>
        /// <param name="keySelector">mapped 요소로부터 key 를 선택하는 함수, 그룹핑을 위해 사용된다.</param>
        /// <param name="reducer">key로 그룹핑된 정보를 바탕으로 TResult수형의 요소의 시퀀스를 반환하는 함수</param>
        /// <returns></returns>
        public static ParallelQuery<TResult> MapReduce<TSource, TMapped, TKey, TResult>(this ParallelQuery<TSource> source,
                                                                                        Func<TSource, IEnumerable<TMapped>> mapper,
                                                                                        Func<TMapped, TKey> keySelector,
                                                                                        Func
                                                                                            <IGrouping<TKey, TMapped>,
                                                                                            IEnumerable<TResult>> reducer) {
            return
                source
                    .SelectMany(mapper)
                    .GroupBy(keySelector)
                    .SelectMany(reducer);
        }

        /// <summary>
        /// <paramref name="source"/>를 모두 병렬로 실행하여, 결과를 <paramref name="target"/> 컬렉션에 요소로 추가한다.
        /// </summary>
        /// <typeparam name="TSource">요소의 수형</typeparam>
        /// <param name="source">병렬 시퀀스</param>
        /// <param name="target">공급자/소비자 패턴의 컬렉션</param>
        public static void OutputToProducerConsumerCollection<TSource>(this ParallelQuery<TSource> source,
                                                                       IProducerConsumerCollection<TSource> target) {
            source.ShouldNotBeNull("source");
            target.ShouldNotBeNull("target");

            // 병렬로 수행한 결과를 모두 컬렉션에 추가한다.
            source.ForAll(item => target.TryAdd(item));
        }

        /// <summary>
        /// <paramref name="options"/>에 따라 ParallelQuery{TSource} 를 빌드합니다.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static ParallelQuery<TSource> AsParallel<TSource>(this IEnumerable<TSource> source, ParallelOptions options) {
            source.ShouldNotBeNull("source");
            options = options ?? ParallelTool.DefaultParallelOptions;

            return AsParallel(source, ParallelLinqOptions.CreateFrom(options));
        }

        /// <summary>
        /// <paramref name="options"/>에 따라 ParallelQuery{TSource} 를 빌드합니다.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static ParallelQuery<TSource> AsParallel<TSource>(this IEnumerable<TSource> source, ParallelLinqOptions options = null) {
            source.ShouldNotBeNull("source");
            options = options ?? ParallelLinqOptions.CreateFrom(ParallelTool.DefaultParallelOptions);

            if(options.TaskScheduler != null && options.TaskScheduler != TaskScheduler.Default)
                throw new InvalidOperationException("Parallel LINQ 는 기본 TaskScheduler만 지원합니다. " +
                                                    "options.TaskScheduler는 TaskScheduler.Default가 아닙니다.");

            if(IsDebugEnabled)
                log.Debug("병렬 옵션을 기준으로 PLINQ로 변환합니다...  options=[{0}]", options.ObjectToString());

            var result = source.AsParallel();

            if(options.Ordered)
                result = result.AsOrdered();

            if(options.CancellationToken.CanBeCanceled)
                result = result.WithCancellation(options.CancellationToken);

            if(options.MaxDegreeOfParallelism >= 1)
                result = result.WithDegreeOfParallelism(options.MaxDegreeOfParallelism);

            if(options.ExecutionMode != ParallelExecutionMode.Default)
                result = result.WithExecutionMode(options.ExecutionMode);

            if(options.MergeOptions != ParallelMergeOptions.Default)
                result = result.WithMergeOptions(options.MergeOptions);

            return result;
        }
    }
}