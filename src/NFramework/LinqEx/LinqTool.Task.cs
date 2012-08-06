using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NSoft.NFramework.Parallelism.Tools;

namespace NSoft.NFramework.LinqEx {
    public static partial class LinqTool {
        /// <summary>
        /// 원본 Task (<paramref name="source"/>) 결과를 <paramref name="selector"/>를 이용하여 변환한 값을 반환하는 Task를 제공합니다.
        /// <paramref name="source"/>작업이 취소되지 않아야 작업을 수행합니다.
        /// </summary>
        /// <param name="source">원본 작업</param>
        /// <param name="selector">원본 작업 결과를 변환하는 함수</param>
        /// <returns>원본 작업 후, 작업결과를 변환하여 반환하는 Task</returns>
        public static Task<TResult> Select<TSource, TResult>(this Task<TSource> source, Func<TSource, TResult> selector) {
            source.ShouldNotBeNull("source");
            selector.ShouldNotBeNull("selector");

            // 기존 작업이 취소되지 않고, 완료되면 결과값을 변환하는 함수를 수행하여, 원하는 결과값을 뱐환하는 작업을 생성합니다.
            //
            return source.ContinueWith(antecedent => selector(antecedent.Result),
                                       TaskContinuationOptions.ExecuteSynchronously);
        }

        /// <summary>
        /// 원본 Task (<paramref name="source"/>) 결과를 변환하는 <paramref name="selector"/>를 수행하여, <typeparamref name="TResult"/> 수형을 결과값으로 반환하는 Task를 빌드합니다.
        /// </summary>
        /// <param name="source">원본 작업</param>
        /// <param name="selector">원본 작업 결과를 비동기적으로 변환하는 작업을 반환하는 함수 </param>
        /// <returns>원본 작업이 완료 후 결과를 <paramref name="selector"/>에 의해 변환된 결과물을 반환하는 Task</returns>
        public static Task<TResult> SelectMany<TSource, TResult>(this Task<TSource> source, Func<TSource, Task<TResult>> selector) {
            source.ShouldNotBeNull("source");
            selector.ShouldNotBeNull("selector");

            // 원본 작업이 취소되지 않고, 완료되면 결과값을 변환하는 함수를 수행하여, 원하는 결과값을 비동기적으로 변환하는 작업을 생성합니다.
            //
            return
                source
                    .ContinueWith(antecedent => selector(antecedent.Result),
                                  TaskContinuationOptions.ExecuteSynchronously)
                    .Unwrap();
        }

        /// <summary>
        /// 원본 Task (<paramref name="source"/>) 실행 결과를, 
        /// <paramref name="collectionSelector"/> 함수를 통해, Task{TCollection} 형태로 만들고, 
        /// <paramref name="resultSelector"/> 함수를 이용해서, 최종 결과 값을 빌드합니다.
        /// </summary>		
        public static Task<TResult> SelectMany<TSource, TCollection, TResult>(this Task<TSource> source,
                                                                              Func<TSource, Task<TCollection>> collectionSelector,
                                                                              Func<TSource, TCollection, TResult> resultSelector) {
            source.ShouldNotBeNull("source");
            collectionSelector.ShouldNotBeNull("collectionSelector");
            resultSelector.ShouldNotBeNull("resultSelector");

            // source 작업이 완료하면, collecitonSelector가 수행되어, 다음 Task들을 얻고, 결과 selector에 Task를 구한다
            // 
            return
                source
                    .ContinueWith(t => collectionSelector(t.Result)
                                           .ContinueWith(c => resultSelector(t.Result, c.Result),
                                                         TaskContinuationOptions.ExecuteSynchronously),
                                  TaskContinuationOptions.ExecuteSynchronously)
                    .Unwrap();
        }

        /// <summary>
        /// <paramref name="source"/> 작업 결과가 <paramref name="predicate"/>에 만족하는 Task만 필터링합니다.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static Task<TSource> Where<TSource>(this Task<TSource> source, Func<TSource, bool> predicate) {
            source.ShouldNotBeNull("source");
            predicate.ShouldNotBeNull("predicate");

            // predicate를 실행하고, source의 필터링된 정보를 반환하기 위해, Continuation을 생성합니다.
            // predicate에서 false를 반환하면, 작업을 취소하고, 예외를 일으키는 작업을 반환합니다.

            var cts = new CancellationTokenSource();

            return
                source
                    .ContinueWith(antecedent => {
                                      var result = antecedent.Result;

                                      // 결과가 만족할 수 없다면, 작업을 취소시킵니다.
                                      if(predicate(result) == false)
                                          cts.CancelAndThrow();

                                      return result;
                                  },
                                  cts.Token,
                                  TaskContinuationOptions.ExecuteSynchronously,
                                  TaskScheduler.Default);
        }

        /// <summary>
        /// Join
        /// </summary>
        public static Task<TResult> Join<TOuter, TInner, TKey, TResult>(this Task<TOuter> outer,
                                                                        Task<TInner> inner,
                                                                        Func<TOuter, TKey> outerKeySelector,
                                                                        Func<TInner, TKey> innerKeySelector,
                                                                        Func<TOuter, TInner, TResult> resultSelector,
                                                                        IEqualityComparer<TKey> keyComparer = null) {
            outer.ShouldNotBeNull("outer");
            inner.ShouldNotBeNull("inner");
            outerKeySelector.ShouldNotBeNull("outerKeySelector");
            innerKeySelector.ShouldNotBeNull("innerKeySelector");
            resultSelector.ShouldNotBeNull("resultSelector");

            keyComparer = keyComparer ?? EqualityComparer<TKey>.Default;

            return
                outer
                    .ContinueWith(_ => {
                                      var cts = new CancellationTokenSource();

                                      return
                                          inner
                                              .ContinueWith(delegate {
                                                                // 예외가 발생하면, throw 되도록 의도적으로 Wait를 합니다.
                                                                Task.WaitAll(outer, inner);

                                                                if(keyComparer.Equals(outerKeySelector(outer.Result),
                                                                                      innerKeySelector(inner.Result)))
                                                                    return resultSelector(outer.Result, inner.Result);

                                                                cts.CancelAndThrow();
                                                                return default(TResult);
                                                            },
                                                            cts.Token,
                                                            TaskContinuationOptions.ExecuteSynchronously,
                                                            TaskScheduler.Default);
                                  },
                                  TaskContinuationOptions.ExecuteSynchronously)
                    .Unwrap();
        }

        /// <summary>
        /// Group Join
        /// </summary>
        public static Task<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this Task<TOuter> outer,
                                                                             Task<TInner> inner,
                                                                             Func<TOuter, TKey> outerKeySelector,
                                                                             Func<TInner, TKey> innerKeySelector,
                                                                             Func<TOuter, Task<TInner>, TResult> resultSelector,
                                                                             IEqualityComparer<TKey> keyComparer = null) {
            outer.ShouldNotBeNull("outer");
            inner.ShouldNotBeNull("inner");
            outerKeySelector.ShouldNotBeNull("outerKeySelector");
            innerKeySelector.ShouldNotBeNull("innerKeySelector");
            resultSelector.ShouldNotBeNull("resultSelector");

            keyComparer = keyComparer ?? EqualityComparer<TKey>.Default;

            return
                outer
                    .ContinueWith(_ => {
                                      var cts = new CancellationTokenSource();
                                      return
                                          inner
                                              .ContinueWith(delegate {
                                                                // 예외가 발생하면, throw 되도록 의도적으로 Wait를 합니다.
                                                                Task.WaitAll(outer, inner);

                                                                if(keyComparer.Equals(outerKeySelector(outer.Result),
                                                                                      innerKeySelector(inner.Result))) {
                                                                    return resultSelector(outer.Result, inner);
                                                                }

                                                                cts.CancelAndThrow();
                                                                return default(TResult);
                                                            },
                                                            cts.Token,
                                                            TaskContinuationOptions.ExecuteSynchronously,
                                                            TaskScheduler.Default);
                                  },
                                  TaskContinuationOptions.ExecuteSynchronously)
                    .Unwrap();
        }

        /// <summary>
        /// 시퀀스를 지정한 키를 기준으로 그룹핑합니다.
        /// </summary>
        public static Task<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this Task<TSource> source,
                                                                                       Func<TSource, TKey> keySelector,
                                                                                       Func<TSource, TElement> elementSelector) {
            source.ShouldNotBeNull("source");
            keySelector.ShouldNotBeNull("keySelector");
            elementSelector.ShouldNotBeNull("elemenetSelector");

            return
                source
                    .ContinueWith(t => {
                                      var result = t.Result;
                                      var key = keySelector(result);
                                      var element = elementSelector(result);

                                      return
                                          (IGrouping<TKey, TElement>)new OneElementGrouping<TKey, TElement>
                                                                     {
                                                                         Key = key,
                                                                         Element = element
                                                                     };
                                  },
                                  TaskContinuationOptions.ExecuteSynchronously);
        }

        /// <summary>
        /// 한 요소를 그룹핑합니다.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TElement"></typeparam>
        private class OneElementGrouping<TKey, TElement> : IGrouping<TKey, TElement> {
            public TKey Key { get; internal set; }
            internal TElement Element { private get; set; }

            public IEnumerator<TElement> GetEnumerator() {
                yield return Element;
            }

            IEnumerator IEnumerable.GetEnumerator() {
                return GetEnumerator();
            }
        }

        /// <summary>
        /// 정렬
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static Task<TSource> OrderBy<TSource, TKey>(this Task<TSource> source, Func<TSource, TKey> keySelector) {
            // 하나의 요소는 정렬이 필요없다. 그냥 반환하면 된다.
            source.ShouldNotBeNull("source");
            return source;
        }

        /// <summary>
        /// 역순 정렬
        /// </summary>
        public static Task<TSource> OrderByDescending<TSource, TKey>(this Task<TSource> source,
                                                                     Func<TSource, TKey> keySelector) {
            // 하나의 요소는 정렬이 필요없다. 그냥 반환하면 된다.
            source.ShouldNotBeNull("source");
            return source;
        }

        /// <summary>
        /// 다음 정렬
        /// </summary>
        public static Task<TSource> ThenBy<TSource, TKey>(this Task<TSource> source, Func<TSource, TKey> keySelector) {
            // 하나의 요소는 정렬이 필요없다. 그냥 반환하면 된다.
            source.ShouldNotBeNull("source");
            return source;
        }

        /// <summary>
        /// 역순 정렬
        /// </summary>
        public static Task<TSource> ThenByDescending<TSource, TKey>(this Task<TSource> source, Func<TSource, TKey> keySelector) {
            // 하나의 요소는 정렬이 필요없다. 그냥 반환하면 된다.
            source.ShouldNotBeNull("source");
            return source;
        }
    }
}