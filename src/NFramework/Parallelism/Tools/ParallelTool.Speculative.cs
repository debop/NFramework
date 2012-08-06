using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NSoft.NFramework.Parallelism.Tools {
    public static partial class ParallelTool {
        /// <summary>
        /// 지정한 범위의 값에 대해 <paramref name="function"/>을 병렬로 실행시키고, 제일 먼저 반환된 값을 반환합니다. 나머지 실행은 모두 취소합니다.
        /// </summary>
        /// <typeparam name="TResult">결과 값 수형</typeparam>
        /// <param name="fromInclusive">범위의 시작</param>
        /// <param name="toExclusive">범위의 끝</param>
        /// <param name="options">병렬 옵션</param>
        /// <param name="function">수행할 함수</param>
        /// <returns>최초 반환 값</returns>
        public static TResult SpeculativeFor<TResult>(int fromInclusive,
                                                      int toExclusive,
                                                      Func<int, TResult> function,
                                                      ParallelOptions options = null) {
            function.ShouldNotBeNull("function");

            object result = null;

            Parallel.For(fromInclusive,
                         toExclusive,
                         options ?? DefaultParallelOptions,
                         (i, loopState) => {
                             Interlocked.CompareExchange(ref result, function(i), null);
                             loopState.Stop();
                         });

            return (TResult)result;
        }

        /// <summary>
        /// 여러가지 입력값에 대해 동시에 함수를 수행하고, 선착순으로 제일 먼저 결과를 반환하는 값을 취합니다. 나머지 실행들은 모두 취소합니다.
        /// </summary>
        /// <typeparam name="TSource">입력 수형</typeparam>
        /// <typeparam name="TResult">출력 수형</typeparam>
        /// <param name="source">입력 데이타 시퀀스</param>
        /// <param name="options">병렬 옵션</param>
        /// <param name="function">실행할 함수</param>
        /// <returns>선착순 최초 반환 결과 값</returns>
        public static TResult SpeculativeForEach<TSource, TResult>(IEnumerable<TSource> source,
                                                                   Func<TSource, TResult> function,
                                                                   ParallelOptions options = null) {
            function.ShouldNotBeNull("function");

            // 첫번째 결과를 담는다.
            object result = null;

            Parallel.ForEach(source,
                             options ?? DefaultParallelOptions,
                             (item, loopState) => {
                                 // result가 null이면, 제일 먼저 함수 수행 결과를 반환하는 값으로 치환한다. 그리고, 모든 loop을 중지한다.
                                 //
                                 Interlocked.CompareExchange(ref result, function(item), null);
                                 loopState.Stop();
                             });

            return (TResult)result;
        }

        /// <summary>
        /// 주어진 여러 함수들을 모두 실행시켜봐서, 제일 먼저 결과를 반환하는 값을 취하고, 나머지 실행은 취소시킵니다.
        /// 이렇게 하는 이유는, 입력값에 따라 계산방식에 차이에 따른 수행 시간을 예상할 수 없을 때, 
        /// 모든 방식의 수행을 시도하고, 그 중 가장 먼저 완료하는 수행방식의 결과값을 취합니다.
        /// </summary>
        /// <typeparam name="TSource">입력 수형</typeparam>
        /// <typeparam name="TResult">출력 수형</typeparam>
        /// <param name="funcs">실행 함수들</param>
        /// <param name="options">병렬 옵션</param>
        /// <param name="source">입력 값</param>
        /// <returns>선착순 최초 반환 결과 값</returns>
        public static TResult SpeculativeForEach<TSource, TResult>(IEnumerable<Func<TSource, TResult>> funcs,
                                                                   TSource source,
                                                                   ParallelOptions options = null) {
            object result = null;

            Parallel.ForEach(funcs,
                             options ?? DefaultParallelOptions,
                             (func, loopState) => {
                                 Interlocked.CompareExchange(ref result, func(source), null);
                                 loopState.Stop();
                             });

            return (TResult)result;
        }

        /// <summary>
        /// 지정된 여러 함수들을 모두 병렬로 실행시켜, 최초 결과 값을 얻으면, 나머지 함수들은 실행 중지 시키고 결과값을 반환한다.
        /// </summary>
        /// <typeparam name="T">반환 값의 수형</typeparam>
        /// <param name="options">병렬 실행 옵션</param>
        /// <param name="functions">실행할 함수의 컬렉션</param>
        /// <returns>최초 결과 값</returns>
        public static T SpeculativeInvoke<T>(ParallelOptions options, params Func<T>[] functions) {
            functions.ShouldNotBeNull("functions");
            return SpeculativeForEach(functions, function => function(), options);
        }

        /// <summary>
        /// 지정된 여러 함수들을 모두 병렬로 실행시켜, 최초 결과 값을 얻으면, 나머지 함수들은 실행 중지 시키고 결과값을 반환한다.
        /// </summary>
        /// <typeparam name="T">반환 값의 수형</typeparam>
        /// <param name="functions">실행할 함수의 컬렉션</param>
        /// <returns>최초 결과 값</returns>
        public static T SpeculativeInvoke<T>(params Func<T>[] functions) {
            return SpeculativeInvoke(DefaultParallelOptions, functions);
        }

        /// <summary>
        /// 지정된 여러 함수들을 모두 병렬로 실행시켜, 최초 결과 값을 얻으면, 나머지 함수들은 실행 중지 시키고 결과값을 반환한다.
        /// </summary>
        /// <typeparam name="TSource">입력 값의 수형</typeparam>
        /// <typeparam name="TResult">반환 값의 수형</typeparam>
        /// <param name="source">입력 값</param>
        /// <param name="options">병렬 실행 옵션</param>
        /// <param name="functions">실행할 함수의 컬렉션</param>
        /// <returns>최초 결과 값</returns>
        public static TResult SpeculativeInvoke<TSource, TResult>(TSource source,
                                                                  ParallelOptions options,
                                                                  params Func<TSource, TResult>[] functions) {
            return SpeculativeForEach(functions, source, options);
        }

        /// <summary>
        /// 지정된 여러 함수들을 모두 병렬로 실행시켜, 최초 결과 값을 얻으면, 나머지 함수들은 실행 중지 시키고 결과값을 반환한다.
        /// </summary>
        /// <typeparam name="TSource">입력 값의 수형</typeparam>
        /// <typeparam name="TResult">반환 값의 수형</typeparam>
        /// <param name="source">입력 값</param>
        /// <param name="functions">실행할 함수의 컬렉션</param>
        /// <returns>최초 결과 값</returns>
        public static TResult SpeculativeInvoke<TSource, TResult>(TSource source, params Func<TSource, TResult>[] functions) {
            return SpeculativeInvoke(source, DefaultParallelOptions, functions);
        }

        /// <summary>
        /// <paramref name="functions"/>들을 모두 병렬로 실행시켜봐서, 제일 먼저 결과를 반환하는 함수에 대해, <paramref name="callback"/> 함수를 호출하고, 
        /// 나머지 실행 중인 함수들은 모두 실행 취소 시킵니다.
        /// </summary>
        /// <typeparam name="TSource">실행할 함수 입력 값의 수형</typeparam>
        /// <typeparam name="TResult">실행할 함수 반환 값의 수형</typeparam>
        /// <param name="source">입력 값</param>
        /// <param name="callback">선착순 선택 결과를 반환할 callback 함수 (첫번째 인자는 실행 함수 배열의 index, 두번째 인자는 실행 결과 값)</param>
        /// <param name="functions">실행할 함수들</param>
        public static Task SpecualtiveSelect<TSource, TResult>(TSource source,
                                                               Action<long, TResult> callback,
                                                               params Func<TSource, TResult>[] functions) {
            return SpecualtiveSelect(source, DefaultParallelOptions, callback, functions);
        }

        /// <summary>
        /// <paramref name="functions"/>들을 모두 병렬로 실행시켜봐서, 제일 먼저 결과를 반환하는 함수에 대해, <paramref name="callback"/> 함수를 호출하고, 
        /// 나머지 실행 중인 함수들은 모두 실행 취소 시킵니다.
        /// </summary>
        /// <typeparam name="TSource">실행할 함수 입력 값의 수형</typeparam>
        /// <typeparam name="TResult">실행할 함수 반환 값의 수형</typeparam>
        /// <param name="source">입력 값</param>
        /// <param name="options">병렬 옵션</param>
        /// <param name="callback">선착순 선택 결과를 반환할 callback 함수 (첫번째 인자는 실행 함수 배열의 index, 두번째 인자는 실행 결과 값)</param>
        /// <param name="functions">실행할 함수들</param>
        public static Task SpecualtiveSelect<TSource, TResult>(TSource source,
                                                               ParallelOptions options,
                                                               Action<long, TResult> callback,
                                                               params Func<TSource, TResult>[] functions) {
            callback.ShouldNotBeNull("callback");
            functions.ShouldNotBeNull("functions");

            // 결과값을 반환한 함수의 수
            object result = null;

            // 선착순 결과 선택을 위한 작업을 반환합니다.
            return
                Task.Factory
                    .StartNew(() => {
                                  // 여러 함수를 병렬로 실행하며, 선착순으로 결과를 제공한 함수의 결과를 채택하고, 나머지 함수는 실행을 준단시킨다.
                                  // 
                                  Parallel.ForEach(functions,
                                                   options ?? DefaultParallelOptions,
                                                   (function, loopState, index) => {
                                                       // 최초 결과 값을 받으면, 나머지 병렬 실행 중인 함수들에게 실행을 중지시키고, 
                                                       // 결과를 callback 함수를 통해 반환합니다.
                                                       Interlocked.CompareExchange(ref result, function(source), null);
                                                       loopState.Stop();
                                                       callback(index, (TResult)result);
                                                   });
                              });
        }
    }
}