using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using NSoft.NFramework.LinqEx;

namespace NSoft.NFramework.Parallelism.Tools {
    /// <summary>
    /// 병렬 프로그래밍 관련 알고리즘을 제공하는 클래스입니다.
    /// </summary>
    public static partial class ParallelTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 기본 속성을 가지는 <see cref="ParallelOptions"/> 인스턴스입니다.
        /// </summary>
        public static readonly ParallelOptions DefaultParallelOptions = new ParallelOptions();

        #region << Filter >>

        /// <summary>
        /// 컬렉션에서 검사자를 만족하는 요소들만 병렬로 필터링합니다.
        /// </summary>
        public static IList<T> Filter<T>(IList<T> inputs, Func<T, bool> predicate) {
            return Filter(inputs, DefaultParallelOptions, predicate);
        }

        /// <summary>
        /// 컬렉션에서 검사자를 만족하는 요소들만 병렬로 필터링합니다.
        /// </summary>
        public static IList<T> Filter<T>(IList<T> inputs, ParallelOptions parallelOptions, Func<T, bool> predicate) {
            inputs.ShouldNotBeNull("inputs");
            parallelOptions.ShouldNotBeNull("parallelOptions");
            predicate.ShouldNotBeNull("predicate");

            if(IsDebugEnabled)
                log.Debug("리스트를 병렬로 필터링을 수행합니다. 요소의 순서는 뒤바뀔 수 있습니다...");

            var results = new List<T>(inputs.Count);

            // NOTE: 병렬 작업별로, 최대한 독립수행이 가능하도록 local value를 사용하여 병렬 작업을 합니다.
            //
            Parallel.For<IList<T>>(0,
                                   inputs.Count,
                                   parallelOptions,
                                   () => new List<T>(inputs.Count),
                                   (i, loop, localList) => {
                                       T item = inputs[i];
                                       if(predicate(item))
                                           localList.Add(item);

                                       return localList;
                                   },
                                   localList => {
                                       lock(results)
                                           results.AddRange(localList);
                                   });

            results.TrimExcess();

            if(IsDebugEnabled)
                log.Debug("리스트를 병렬로 필터링을 수행했습니다. 리스트 요소의 순서는 뒤바뀔 수 있습니다.");

            return results;
        }

        #endregion

        #region << For, Range method for BigInteger >>

        /// <summary>
        /// <see cref="Parallel.For(int,int,System.Action{int,System.Threading.Tasks.ParallelLoopState})"/>의 BigInteger 버전
        /// </summary>
        /// <param name="fromInclusive"></param>
        /// <param name="toExclusive"></param>
        /// <param name="body"></param>
        public static void For(BigInteger fromInclusive, BigInteger toExclusive, Action<BigInteger> body) {
            body.ShouldNotBeNull("body");
            For(fromInclusive, toExclusive, DefaultParallelOptions, body);
        }

        /// <summary>
        /// <see cref="Parallel.For(int,int,System.Threading.Tasks.ParallelOptions,System.Action{int,System.Threading.Tasks.ParallelLoopState})"/>의 BigInteger 버전
        /// </summary>
        /// <param name="fromInclusive"></param>
        /// <param name="toExclusive"></param>
        /// <param name="options"></param>
        /// <param name="body"></param>
        public static void For(BigInteger fromInclusive, BigInteger toExclusive, ParallelOptions options, Action<BigInteger> body) {
            body.ShouldNotBeNull("body");

            var range = toExclusive - fromInclusive;

            if(range <= 0)
                return;


            if(range <= Int64.MaxValue) {
                // If the range is within the realm of Int64, we'll delegate to Parallel.For's Int64 overloads.
                // Iterate from 0 to range, and then call the user-provided body with the scaled-back value.
                //
                Parallel.For(0, (long)range, options, i => body(i + fromInclusive));
            }
            else {
                // For a range larger than Int64.MaxValue, we'll rely on an enumerable of BigInteger.
                // We create a C# iterator that yields all of the BigInteger values in the requested range
                // and then ForEach over that range.
                //
                Parallel.ForEach(Range(fromInclusive, toExclusive), options, body);
            }
        }

        /// <summary>
        /// <paramref name="fromInclusive"/> ~ <paramref name="toExclusive"/> 범위의 BigInteger 값을 열거합니다.
        /// </summary>
        /// <param name="fromInclusive"></param>
        /// <param name="toExclusive"></param>
        /// <returns></returns>
        private static IEnumerable<BigInteger> Range(BigInteger fromInclusive, BigInteger toExclusive) {
            for(var i = fromInclusive; i < toExclusive; i++)
                yield return i;
        }

        #endregion

        #region << ForWithStep >>

        /// <summary>
        /// <paramref name="step"/>이 있는 for loop 를 병렬 수행합니다. for(i=fromInclusive; i &lt; toExclusive; i+=step)
        /// </summary>
        /// <param name="fromInclusive">loop 하한 인덱스</param>
        /// <param name="toExclusive">loop 상한 인덱스</param>
        /// <param name="step">반복 인덱스 Step</param>
        /// <param name="body">병렬 실행할 메소드</param>
        /// <returns>/실행 결과</returns>
        public static ParallelLoopResult ForWithStep(int fromInclusive, int toExclusive, int step, Action<int> body) {
            return ForWithStep(fromInclusive, toExclusive, step, DefaultParallelOptions, body);
        }

        /// <summary>
        /// <paramref name="step"/>이 있는 for loop 를 병렬 수행합니다.	for(i=fromInclusive; i &lt; toExclusive; i+=step)
        /// </summary>
        /// <param name="fromInclusive">loop 하한 인덱스</param>
        /// <param name="toExclusive">loop 상한 인덱스</param>
        /// <param name="step">반복 인덱스 Step</param>
        /// <param name="parallelOptions">병렬 옵션</param>
        /// <param name="body">병렬 실행할 메소드</param>
        /// <returns>/실행 결과</returns>
        public static ParallelLoopResult ForWithStep(int fromInclusive, int toExclusive, int step, ParallelOptions parallelOptions,
                                                     Action<int> body) {
            step.ShouldBePositive("step");
            body.ShouldNotBeNull("body");

            if(step == 1)
                return Parallel.For(fromInclusive, toExclusive, body);

            var count = (int)Math.Ceiling((toExclusive - fromInclusive) / (double)step);

            if(IsDebugEnabled)
                log.Debug("For Loop를 병렬 수행합니다. fromInclusive=[{0}], toExclusive=[{1}], step=[{2}], count=[{3}]",
                          fromInclusive, toExclusive, step, count);

            return Parallel.For(0, count, i => body(fromInclusive + i * step));
        }

        /// <summary>
        /// <paramref name="step"/>이 있는 for loop 를 병렬 수행합니다.	for(i=fromInclusive; i &lt; toExclusive; i+=step)
        /// </summary>
        /// <param name="fromInclusive">loop 하한 인덱스</param>
        /// <param name="toExclusive">loop 상한 인덱스</param>
        /// <param name="step">반복 인덱스 Step</param>
        /// <param name="body">병렬 실행할 메소드</param>
        /// <returns>/실행 결과</returns>
        public static ParallelLoopResult ForWithStep(int fromInclusive, int toExclusive, int step, Action<int, ParallelLoopState> body) {
            return ForWithStep(fromInclusive, toExclusive, step, DefaultParallelOptions, body);
        }

        /// <summary>
        /// <paramref name="step"/>이 있는 for loop 를 병렬 수행합니다.	for(i=fromInclusive; i &lt; toExclusive; i+=step)
        /// </summary>
        /// <param name="fromInclusive">loop 하한 인덱스</param>
        /// <param name="toExclusive">loop 상한 인덱스</param>
        /// <param name="step">반복 인덱스 Step</param>
        /// <param name="parallelOptions">병렬 옵션</param>
        /// <param name="body">병렬 실행할 메소드</param>
        /// <returns>/실행 결과</returns>
        public static ParallelLoopResult ForWithStep(int fromInclusive, int toExclusive, int step,
                                                     ParallelOptions parallelOptions,
                                                     Action<int, ParallelLoopState> body) {
            step.ShouldBePositive("step");

            if(step == 1)
                return Parallel.For(fromInclusive, toExclusive, body);

            var count = (int)Math.Ceiling((toExclusive - fromInclusive) / (double)step);

            if(IsDebugEnabled)
                log.Debug("For Loop를 병렬 수행합니다. fromInclusive=[{0}], toExclusive=[{1}], step=[{2}], count=[{3}]",
                          fromInclusive, toExclusive, step, count);

            return Parallel.For(0, count, (i, loopState) => body(fromInclusive + i * step, loopState));
        }

        /// <summary>
        /// <paramref name="step"/>이 있는 for loop 를 병렬 수행합니다.	for(i=fromInclusive; i &lt; toExclusive; i+=step)
        /// </summary>
        /// <param name="fromInclusive">loop 하한 인덱스</param>
        /// <param name="toExclusive">loop 상한 인덱스</param>
        /// <param name="step">반복 인덱스 Step</param>
        /// <param name="localInit">로컬 변수값 초기화</param>
        /// <param name="body">로컬값을 반환하는 병렬 실행할 함수</param>
        /// <param name="localFinally">최종 로컬 정리 action</param>
        /// <returns>/실행 결과</returns>
        public static ParallelLoopResult ForWithStep<T>(int fromInclusive, int toExclusive, int step,
                                                        Func<T> localInit,
                                                        Func<int, ParallelLoopState, T, T> body,
                                                        Action<T> localFinally) {
            return ForWithStep(fromInclusive, toExclusive, step, DefaultParallelOptions, localInit, body, localFinally);
        }

        /// <summary>
        /// <paramref name="step"/>이 있는 for loop 를 병렬 수행합니다.	for(i=fromInclusive; i &lt; toExclusive; i+=step)
        /// </summary>
        /// <param name="fromInclusive">loop 하한 인덱스</param>
        /// <param name="toExclusive">loop 상한 인덱스</param>
        /// <param name="step">반복 인덱스 Step</param>
        /// <param name="parallelOptions">병렬 옵션</param>
        /// <param name="localInit">로컬 변수값 초기화</param>
        /// <param name="body">로컬값을 반환하는 병렬 실행할 함수</param>
        /// <param name="localFinally">최종 로컬 정리 action</param>
        /// <returns>/실행 결과</returns>
        public static ParallelLoopResult ForWithStep<T>(int fromInclusive, int toExclusive, int step,
                                                        ParallelOptions parallelOptions,
                                                        Func<T> localInit,
                                                        Func<int, ParallelLoopState, T, T> body,
                                                        Action<T> localFinally) {
            step.ShouldBePositive("step");

            if(step == 1)
                return Parallel.For(fromInclusive, toExclusive, localInit, body, localFinally);

            if(IsDebugEnabled)
                log.Debug("Step을 가진 For Loop를 병렬 수행합니다... fromInclusive=[{0}], toExclusive=[{1}], step=[{2}]",
                          fromInclusive, toExclusive, step);

            var ranges = EnumerableTool.Step(fromInclusive, toExclusive, step);

            return Parallel.ForEach(ranges,
                                    parallelOptions,
                                    localInit,
                                    (i, loolState, local) => body(i, loolState, local),
                                    local => localFinally(local));
        }

        #endregion

        #region << Reduce >> 

        // HINT : Parallel Patterns - Reduce  (http://software.intel.com/en-us/blogs/2009/07/23/parallel-pattern-7-reduce/)

        /// <summary>Reduces the input data using the specified aggregation operation.</summary>
        /// <typeparam name="T">Specifies the type of data being aggregated.</typeparam>
        /// <param name="inputs">The input data to be reduced.</param>
        /// <param name="seed">The seed to use to initialize the operation; this seed may be used multiple times.</param>
        /// <param name="associativeCommutativeOperation">The reduction operation.</param>
        /// <returns>The reduced value.</returns>
        public static T Reduce<T>(IList<T> inputs, T seed, Func<T, T, T> associativeCommutativeOperation) {
            return Reduce(inputs, DefaultParallelOptions, seed, associativeCommutativeOperation);
        }

        /// <summary>Reduces the input data using the specified aggregation operation.</summary>
        /// <typeparam name="T">Specifies the type of data being aggregated.</typeparam>
        /// <param name="inputs">The input data to be reduced.</param>
        /// <param name="parallelOptions">A ParallelOptions instance that configures the behavior of this operation.</param>
        /// <param name="seed">The seed to use to initialize the operation; this seed may be used multiple times.</param>
        /// <param name="associativeCommutativeOperation">The reduction operation.</param>
        /// <returns>The reduced value.</returns>
        public static T Reduce<T>(IList<T> inputs, ParallelOptions parallelOptions, T seed,
                                  Func<T, T, T> associativeCommutativeOperation) {
            inputs.ShouldNotBeNull("inputs");
            return Reduce(0, inputs.Count, parallelOptions, i => inputs[i], seed, associativeCommutativeOperation);
        }

        /// <summary>Reduces the input range using the specified aggregation operation.</summary>
        /// <typeparam name="T">Specifies the type of data being aggregated.</typeparam>
        /// <param name="fromInclusive">The start index, inclusive.</param>
        /// <param name="toExclusive">The end index, exclusive.</param>
        /// <param name="mapOperation">The function used to retrieve the data to be reduced for a given index.</param>
        /// <param name="seed">The seed to use to initialize the operation; this seed may be used multiple times.</param>
        /// <param name="associativeCommutativeOperation">The reduction operation.</param>
        /// <returns>The reduced value.</returns>
        public static T Reduce<T>(int fromInclusive, int toExclusive, Func<int, T> mapOperation, T seed,
                                  Func<T, T, T> associativeCommutativeOperation) {
            return Reduce(fromInclusive, toExclusive, DefaultParallelOptions, mapOperation, seed, associativeCommutativeOperation);
        }

        /// <summary>Reduces the input range using the specified aggregation operation.</summary>
        /// <typeparam name="T">Specifies the type of data being aggregated.</typeparam>
        /// <param name="fromInclusive">The start index, inclusive.</param>
        /// <param name="toExclusive">The end index, exclusive.</param>
        /// <param name="parallelOptions">A ParallelOptions instance that configures the behavior of this operation.</param>
        /// <param name="mapOperation">The function used to retrieve the data to be reduced for a given index.</param>
        /// <param name="seed">The seed to use to initialize the operation; this seed may be used multiple times.</param>
        /// <param name="associativeCommutativeOperation">The reduction operation.</param>
        /// <returns>The reduced value.</returns>
        public static T Reduce<T>(int fromInclusive, int toExclusive, ParallelOptions parallelOptions,
                                  Func<int, T> mapOperation, T seed, Func<T, T, T> associativeCommutativeOperation) {
            mapOperation.ShouldNotBeNull("mapOperation");
            associativeCommutativeOperation.ShouldNotBeNull("associativeCommutativeOperation");
            fromInclusive.ShouldBeLessThan(toExclusive, "fromInclusive");
            toExclusive.ShouldBeGreaterThan(fromInclusive, "toExclusive");

            var syncLock = new object();
            var result = seed;

            Parallel.For(fromInclusive,
                         toExclusive,
                         parallelOptions ?? DefaultParallelOptions,
                         () => seed,
                         (i, loopState, localResult) => associativeCommutativeOperation(mapOperation(i), localResult),
                         localResult => {
                             lock(syncLock)
                                 result = associativeCommutativeOperation(localResult, result);
                         });
            return result;
        }

        #endregion

        #region << Scan >>

        // HINT : Parallel Patterns - Scan (http://software.intel.com/en-us/blogs/2009/09/15/parallel-pattern-8-scan/)

        /// <summary>
        /// Computes a parallel prefix scan over the source enumerable using the specified function.
        /// </summary>
        public static T[] Scan<T>(IEnumerable<T> source, Func<T, T, T> function) {
            return Scan(source, function, false);
        }

        /// <summary>
        /// Computes a parallel prefix scan over the source enumerable using the specified function.
        /// </summary>
        public static T[] Scan<T>(IEnumerable<T> source, Func<T, T, T> function, bool loadBalance) {
            source.ShouldNotBeNull("source");
            function.ShouldNotBeNull("function");

            if(IsDebugEnabled)
                log.Debug("시퀀스에 대해 Scan 작업을 수행합니다. loadBalance=[{0}]", loadBalance);

            var output = source.ToArray<T>();

            ScanInPlace(output, function, loadBalance);
            return output;
        }

        /// <summary>
        /// Computes a parallel prefix scan in-place on an array using the specified function.
        /// </summary>
        public static void ScanInPlace<T>(T[] array, Func<T, T, T> function) {
            ScanInPlace(array, function, false);
        }

        /// <summary>
        /// Computes a parallel prefix scan in-place on an array using the specified function.
        /// </summary>
        public static void ScanInPlace<T>(T[] array, Func<T, T, T> function, bool loadBalance) {
            array.ShouldNotBeNull("array");
            function.ShouldNotBeNull("function");

            if(Environment.ProcessorCount <= 2)
                InclusiveScanInPlaceSerial(array, function, 0, array.Length, 1);
            else if(loadBalance)
                InclusiveScanInPlaceWithLoadBalancingParallel(array, function, 0, array.Length, 1);
            else {
#if !SILVERLIGHT
                InclusiveScanInPlaceParallel(array, function);
#else
				InclusiveScanInPlaceSerial(array, function, 0, array.Length, 1);
#endif
            }
        }

        /// <summary>
        /// Computes a sequential prefix scan over the array using the specified function.
        /// </summary>
        public static void InclusiveScanInPlaceSerial<T>(T[] array, Func<T, T, T> function, int arrStart, int arrLength, int skip) {
            array.ShouldNotBeEmpty("array");
            function.ShouldNotBeNull("function");
            arrStart.ShouldBePositiveOrZero("arrStart");
            arrLength.ShouldBePositive("arrLength");
            skip.ShouldNotBeZero("skip");

            if(IsDebugEnabled)
                log.Debug("순차방식의 스캔을 수행합니다... arrStart=[{0}], arrLength=[{1}], skip=[{2}]", arrStart, arrLength, skip);

            for(var i = arrStart; i + skip < arrLength; i += skip) {
                array[i + skip] = function(array[i], array[i + skip]);

                if(IsDebugEnabled)
                    log.Debug("array[{0}+{1}] = function(array[{0}], array[{0}+{1}]) => {2}", i, skip, array[i + skip]);
            }
        }

        /// <summary>
        /// Computes a sequential exclusive prefix scan over the array using the specified function.
        /// </summary>
        public static void ExclusiveScanInPlaceSerial<T>(T[] array, Func<T, T, T> function, int fromInclusive, int toExclusive) {
            array.ShouldNotBeEmpty("array");
            function.ShouldNotBeNull("function");

            if(IsDebugEnabled)
                log.Debug("순차방식의 배타적 스캔을 시작합니다... fromInclusive={0}, toExclusive={1}", fromInclusive, toExclusive);

            var total = array[fromInclusive];
            array[fromInclusive] = default(T);

            for(var i = fromInclusive + 1; i < toExclusive; i++) {
                var prevTotal = total;
                total = function(total, array[i]);

                if(IsDebugEnabled)
                    log.Debug("순차방식 ExclusiveScan... array[{0}]={1}, total={2}", i, array[i], total);

                array[i] = prevTotal;
            }
        }

        /// <summary>
        /// Computes a parallel prefix scan over the array using the specified function.
        /// </summary>
        public static void InclusiveScanInPlaceWithLoadBalancingParallel<T>(T[] array, Func<T, T, T> function, int arrStart,
                                                                            int arrLength, int skip) {
            if(arrLength <= 1)
                return;

            array.ShouldNotBeEmpty("array");
            function.ShouldNotBeNull("function");
            arrStart.ShouldBePositiveOrZero("arrStart");
            arrLength.ShouldBePositive("arrLength");
            skip.ShouldNotBeZero("skip");

            var halfInputLength = arrLength / 2;

            if(IsDebugEnabled)
                log.Debug("로드 밸런싱을 적용한 병렬 Scan을 수행합니다... arrStart={0}, arrLength={1}, skip={2}", arrStart, arrLength, skip);

            Parallel.For(0,
                         halfInputLength,
                         i => {
                             var loc = arrStart + (i * 2 * skip);
                             array[loc + skip] = function(array[loc], array[loc + skip]);

                             if(IsDebugEnabled) {
                                 log.Debug("Scan 작업을 수행중입니다. loc[{0}] = arrStart[{1}] + (i[{2}] * 2 * skip[{3}]); ", loc, arrStart, i,
                                           skip);
                                 log.Debug("Scan 작업을 수행중입니다. array[{0}+{1}] = function(array[{0}], array[{0} + {1}])", loc, skip);
                             }
                         });

            // 재귀호출 방식으로 나머지 반을 수행합니다.
            InclusiveScanInPlaceWithLoadBalancingParallel(array, function, arrStart + skip, halfInputLength, skip * 2);

            // Generate output. As before, use static partitioning.
            Parallel.For(0,
                         (arrLength % 2) == 0 ? halfInputLength - 1 : halfInputLength,
                         i => {
                             var loc = arrStart + (i * 2 * skip) + skip;
                             array[loc + skip] = function(array[loc], array[loc + skip]);

                             if(IsDebugEnabled) {
                                 log.Debug("Scan 작업을 수행중입니다. loc[{0}] = arrStart[{1}] + (i[{2}] * 2 * skip[{3}]) + skip[{3}];", loc,
                                           arrStart, i, skip);
                                 log.Debug("Scan 작업을 수행중입니다. array[{0}+{1}] = function(array[{0}], array[{0} + {1}])", loc, skip);
                             }
                         });
        }

#if !SILVERLIGHT
        /// <summary>
        /// Computes a parallel inclusive prefix scan over the array using the specified function.
        /// </summary>
        public static void InclusiveScanInPlaceParallel<T>(T[] array, Func<T, T, T> function) {
            var processCount = Environment.ProcessorCount;
            var intermediatePartials = new T[processCount];

            using(var phaseBarrier = new Barrier(processCount,
                                                 _ =>
                                                 ExclusiveScanInPlaceSerial(intermediatePartials, function, 0,
                                                                            intermediatePartials.Length))) {
                var rangeSize = array.Length / processCount;
                var nextRangeStart = 0;

                var tasks = new Task[processCount];

                for(var i = 0; i < processCount; i++, nextRangeStart += rangeSize) {
                    var rangeNum = i;
                    var fromInclusive = nextRangeStart;
                    var toExclusive = (i < processCount - 1) ? nextRangeStart + rangeSize : array.Length;

                    tasks[rangeNum] =
                        Task.Factory
                            .StartNew(() => {
                                          // Phase 1: Prefix scan assigned ranges, and copy upper bound to intermediate partials
                                          InclusiveScanInPlaceSerial(array, function, fromInclusive, toExclusive, 1);
                                          intermediatePartials[rangeNum] = array[toExclusive - 1];

                                          // Phase 2: One thread only should prefix scan the intermediaries... done implicitly by the barrier
                                          phaseBarrier.SignalAndWait();

                                          // Phase 3: Incorporate partials
                                          if(rangeNum != 0) {
                                              for(var j = fromInclusive; j < toExclusive; j++) {
                                                  array[j] = function(intermediatePartials[rangeNum], array[j]);
                                              }
                                          }
                                      });
                }

                // 모든 Task가 완료될 때까지 기다립니다.
                With.TryActionAsync(() => Task.WaitAll(tasks));
            }
        }
#endif

        #endregion

        #region << Sort >>

        // HINT: QuickSort - http://en.wikibooks.org/wiki/Algorithm_Implementation/Sorting/Quicksort#C.23
        // HINT: MergeSort - http://en.wikibooks.org/wiki/Algorithm_Implementation/Sorting/Merge_sort#C.23
        // HINT: RadixSort - http://en.wikibooks.org/wiki/Algorithm_Implementation/Sorting/Radix_sort#C.23_least_significant_digit_.28LSD.29_radix_sort_implementation

        // 참고자료: http://blogs.msdn.com/b/pfxteam/archive/2011/06/07/10171827.aspx

        /// <summary>
        /// Parallel merge sort by dividing the array into N buckets, where N is the total number of threads, each thread will either sort its bucket using QickSort or MergeSort
        /// (QuickSort shows slightly better performance) all threads are synchronized using a System.Threading.Barrier. When all threads sort their partitions, half of the threads (the 
        /// odd threads will be removed) and the even threads will merge its chunck with the removed thread's chunk, then the odd threads are moved again, after Log(N) iterations,
        /// The sorted array will be in one chunk in the first thread
        /// </summary>
        /// <typeparam name="T">Array type</typeparam>
        /// <param name="array">The array object</param>
        public static void Sort<T>(T[] array) where T : IComparable<T> {
            Sort(array, Comparer<T>.Default);
        }

        /// <summary>
        /// Parallel merge sort by dividing the array into N buckets, where N is the total number of threads, each thread will either sort its bucket using QickSort or MergeSort
        /// (QuickSort shows slightly better performance) all threads are synchronized using a System.Threading.Barrier. When all threads sort their partitions, half of the threads (the 
        /// odd threads will be removed) and the even threads will merge its chunck with the removed thread's chunk, then the odd threads are moved again, after Log(N) iterations,
        /// The sorted array will be in one chunk in the first thread
        /// </summary>
        /// <typeparam name="T">Array type</typeparam>
        /// <param name="array">The array object</param>
        /// <param name="comparer">Comparer object, if null the default comparer will be used</param>
        public static void Sort<T>(T[] array, IComparer<T> comparer) where T : IComparable<T> {
            array.ShouldNotBeNull("array");
            array.ShouldNotBeEmpty<T>("array");

            if(comparer == null)
                comparer = Comparer<T>.Default;

            // 정렬 항목이 아주 작다면 그냥 단순정렬을 수행합니다.
            if(array.Length <= Environment.ProcessorCount * 8) {
                Array.Sort(array, comparer);
            }

            // the auxilary array
            var auxArray = new T[array.Length];
            //Array.Copy(array, auxArray, array.Length);

            var totalWorkers = 2 * Environment.ProcessorCount; // must be power of two

            //worker tasks, -1 because the main thread will be used as a worker too
            var workers = new Task[totalWorkers - 1];

            // number of iterations is determined by Log(workers), this is why th workers has to be power of 2
            var iterations = (int)Math.Log(totalWorkers, 2);

            // Number of elements for each array, if the elements number is not divisible by the workers, the remainders will be added to the first
            // worker (the main thread)
            var partitionSize = array.Length / totalWorkers;

            var remainder = array.Length % totalWorkers;

            //Barrier used to synchronize the threads after each phase
            //The PostPhaseAction will be responsible for swapping the array with the aux array 
            var barrier = new Barrier(totalWorkers,
                                      b => {
                                          partitionSize <<= 1;

                                          var temp = auxArray;
                                          auxArray = array;
                                          array = temp;
                                      });

            Action<object> workAction = obj => {
                                            var index = (int)obj;

                                            //calculate the partition boundary
                                            var low = index * partitionSize;
                                            if(index > 0)
                                                low += remainder;
                                            var high = (index + 1) * partitionSize - 1 + remainder;

                                            QuickSort(array, low, high, comparer);
                                            // MergeSort(array, auxArray, low, high, comparer);
                                            // Array.Sort(array, low, high - low + 1, comparer);
                                            barrier.SignalAndWait();

                                            for(var j = 0; j < iterations; j++) {
                                                //we always remove the odd workers
                                                if(index % 2 == 1) {
                                                    barrier.RemoveParticipant();
                                                    break;
                                                }

                                                var newHigh = high + partitionSize / 2;
                                                index >>= 1; //update the index after removing the zombie workers
                                                Merge(array, auxArray, low, high, high + 1, newHigh, comparer);
                                                high = newHigh;
                                                barrier.SignalAndWait();
                                            }
                                        };

            for(var i = 0; i < workers.Length; i++)
                workers[i] = Task.Factory.StartNew(obj => workAction(obj), i + 1);

            workAction(0);

            if(iterations % 2 != 0)
                Array.Copy(auxArray, array, array.Length);
        }

        /// <summary>
        /// Merge sort an array recursively
        /// </summary>
        private static void MergeSort<T>(T[] array, T[] auxArray, int low, int high, IComparer<T> comparer) {
            if(low >= high)
                return;

            var mid = (high + low) / 2;
            MergeSort<T>(auxArray, array, low, mid, comparer);
            MergeSort<T>(auxArray, array, mid + 1, high, comparer);

            Merge<T>(array, auxArray, low, mid, mid + 1, high, comparer);
        }

        /// <summary>
        /// Mrge two sorted sub arrays
        /// </summary>
        private static void Merge<T>(IList<T> array, IList<T> auxArray, int low1, int low2, int high1, int high2, IComparer<T> comparer) {
            var ptr1 = low1;
            var ptr2 = high1;
            var ptr = low1;

            for(; ptr <= high2; ptr++) {
                if(ptr1 > low2)
                    array[ptr] = auxArray[ptr2++];
                else if(ptr2 > high2)
                    array[ptr] = auxArray[ptr1++];

                else {
                    if(comparer.Compare(auxArray[ptr1], auxArray[ptr2]) <= 0) {
                        array[ptr] = auxArray[ptr1++];
                    }
                    else
                        array[ptr] = auxArray[ptr2++];
                }
            }
        }

        /// <summary>
        /// QuickSort method
        /// </summary>
        private static void QuickSort<T>(IList<T> array, int low, int high, IComparer<T> comparer) {
            var l_hold = low;
            var h_hold = high;
            var pivot = array[low];

            while(low < high) {
                while(comparer.Compare(pivot, array[high]) <= 0 && (low < high)) {
                    high--;
                }

                if(low != high) {
                    array[low] = array[high];
                    low++;
                }

                while(comparer.Compare(pivot, array[low]) >= 0 && (low < high)) {
                    low++;
                }

                if(low != high) {
                    array[high] = array[low];
                    high--;
                }
            }

            array[low] = pivot;
            var mid = low;
            low = l_hold;
            high = h_hold;

            if(low < mid) {
                QuickSort(array, low, mid - 1, comparer);
            }

            if(high > mid) {
                QuickSort(array, mid + 1, high, comparer);
            }
        }

        #endregion
    }
}