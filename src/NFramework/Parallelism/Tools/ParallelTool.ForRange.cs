using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace NSoft.NFramework.Parallelism.Tools {
    public static partial class ParallelTool {
        #region << Int32, No Options >>

        /// <summary>
        /// 특정 범위를 분할해서 병렬로 로컬 작업을 수행합니다 (병렬 작업을 너무 세분화 시키지 않고, 분할해서 작업하면 효과적이다.)
        /// </summary>
        /// <param name="fromInclusive">시작 인덱스</param>
        /// <param name="toExclusive">종료 인덱스</param>
        /// <param name="body">구간 단위의 실행을 담당하는 delegate</param>
        /// <returns></returns>
        public static ParallelLoopResult ForRange(int fromInclusive,
                                                  int toExclusive,
                                                  Action<int, int> body) {
            return ForRange(fromInclusive, toExclusive, DefaultParallelOptions, body);
        }

        /// <summary>
        /// 특정 범위를 분할해서 병렬로 로컬 작업을 수행합니다 (병렬 작업을 너무 세분화 시키지 않고, 분할해서 작업하면 효과적이다.)
        /// </summary>
        /// <param name="fromInclusive">시작 인덱스</param>
        /// <param name="toExclusive">종료 인덱스</param>
        /// <param name="body">구간 단위의 실행을 담당하는 delegate</param>
        /// <returns></returns>
        public static ParallelLoopResult ForRange(int fromInclusive,
                                                  int toExclusive,
                                                  Action<int, int, ParallelLoopState> body) {
            return ForRange(fromInclusive, toExclusive, DefaultParallelOptions, body);
        }

        /// <summary>
        /// 특정 범위를 분할해서 병렬로 로컬 작업을 수행합니다 (병렬 작업을 너무 세분화 시키지 않고, 분할해서 작업하면 효과적이다.)
        /// </summary>
        /// <typeparam name="TLocal"></typeparam>
        /// <param name="fromInclusive">시작 인덱스</param>
        /// <param name="toExclusive">종료 인덱스</param>
        /// <param name="localInit">구간 단위의 초기화 값을 제공하는 delegate</param>
        /// <param name="localBody">구간 단위의 실행을 담당하는 delegate</param>
        /// <param name="localFinally">구간별 실행 결과 합치는 delegate</param>
        /// <returns></returns>
        public static ParallelLoopResult ForRange<TLocal>(int fromInclusive,
                                                          int toExclusive,
                                                          Func<TLocal> localInit,
                                                          Func<int, int, ParallelLoopState, TLocal, TLocal> localBody,
                                                          Action<TLocal> localFinally) {
            return ForRange(fromInclusive, toExclusive, DefaultParallelOptions, localInit, localBody, localFinally);
        }

        #endregion

        #region << Int64, No Options >>

        /// <summary>
        /// 특정 범위를 분할해서 병렬로 로컬 작업을 수행합니다 (병렬 작업을 너무 세분화 시키지 않고, 분할해서 작업하면 효과적이다.)
        /// </summary>
        /// <param name="fromInclusive">시작 인덱스</param>
        /// <param name="toExclusive">종료 인덱스</param>
        /// <param name="body">구간 단위의 실행을 담당하는 delegate</param>
        /// <returns></returns>
        public static ParallelLoopResult ForRange(long fromInclusive,
                                                  long toExclusive,
                                                  Action<long, long> body) {
            return ForRange(fromInclusive, toExclusive, DefaultParallelOptions, body);
        }

        /// <summary>
        /// 특정 범위를 분할해서 병렬로 로컬 작업을 수행합니다 (병렬 작업을 너무 세분화 시키지 않고, 분할해서 작업하면 효과적이다.)
        /// </summary>
        /// <param name="fromInclusive">시작 인덱스</param>
        /// <param name="toExclusive">종료 인덱스</param>
        /// <param name="body">구간 단위의 실행을 담당하는 delegate</param>
        /// <returns></returns>
        public static ParallelLoopResult ForRange(long fromInclusive,
                                                  long toExclusive,
                                                  Action<long, long, ParallelLoopState> body) {
            return ForRange(fromInclusive, toExclusive, DefaultParallelOptions, body);
        }

        /// <summary>
        /// 특정 범위를 분할해서 병렬로 로컬 작업을 수행합니다 (병렬 작업을 너무 세분화 시키지 않고, 분할해서 작업하면 효과적이다.)
        /// </summary>
        /// <typeparam name="TLocal"></typeparam>
        /// <param name="fromInclusive">시작 인덱스</param>
        /// <param name="toExclusive">종료 인덱스</param>
        /// <param name="localInit">구간 단위의 초기화 값을 제공하는 delegate</param>
        /// <param name="localBody">구간 단위의 실행을 담당하는 delegate</param>
        /// <param name="localFinally">구간별 실행 결과 합치는 delegate</param>
        /// <returns></returns>
        public static ParallelLoopResult ForRange<TLocal>(long fromInclusive,
                                                          long toExclusive,
                                                          Func<TLocal> localInit,
                                                          Func<long, long, ParallelLoopState, TLocal, TLocal> localBody,
                                                          Action<TLocal> localFinally) {
            return ForRange(fromInclusive, toExclusive, DefaultParallelOptions, localInit, localBody, localFinally);
        }

        #endregion

        #region << Int32, Parallel Options >>

        /// <summary>
        /// 특정 범위를 분할해서 병렬로 로컬 작업을 수행합니다 (병렬 작업을 너무 세분화 시키지 않고, 분할해서 작업하면 효과적이다.)
        /// </summary>
        /// <param name="fromInclusive">시작 인덱스</param>
        /// <param name="toExclusive">종료 인덱스</param>
        /// <param name="parallelOptions">병렬 실행 옵션</param>
        /// <param name="body">구간 단위의 실행을 담당하는 delegate</param>
        /// <returns></returns>
        public static ParallelLoopResult ForRange(int fromInclusive,
                                                  int toExclusive,
                                                  ParallelOptions parallelOptions,
                                                  Action<int, int> body) {
            parallelOptions.ShouldNotBeNull("parallelOptions");
            body.ShouldNotBeNull("body");

            if(IsDebugEnabled)
                log.Debug("특정 범위를 분할해서 병렬로 로컬 작업을 수행합니다. 구간=[{0},{1})", fromInclusive, toExclusive);

            return Parallel.ForEach(CreateRangePartition(fromInclusive, toExclusive),
                                    parallelOptions,
                                    range => body(range.Item1, range.Item2));
        }

        /// <summary>
        /// 특정 범위를 분할해서 병렬로 로컬 작업을 수행합니다 (병렬 작업을 너무 세분화 시키지 않고, 분할해서 작업하면 효과적이다.)
        /// </summary>
        /// <param name="fromInclusive">시작 인덱스</param>
        /// <param name="toExclusive">종료 인덱스</param>
        /// <param name="parallelOptions">병렬 실행 옵션</param>
        /// <param name="body">구간 단위의 실행을 담당하는 delegate</param>
        /// <returns></returns>
        public static ParallelLoopResult ForRange(int fromInclusive,
                                                  int toExclusive,
                                                  ParallelOptions parallelOptions,
                                                  Action<int, int, ParallelLoopState> body) {
            parallelOptions.ShouldNotBeNull("parallelOptions");
            body.ShouldNotBeNull("body");

            if(IsDebugEnabled)
                log.Debug("특정 범위를 분할해서 병렬로 로컬 작업을 수행합니다. 구간=[{0},{1})", fromInclusive, toExclusive);

            return Parallel.ForEach(CreateRangePartition(fromInclusive, toExclusive),
                                    parallelOptions,
                                    (range, loopState) => body(range.Item1, range.Item2, loopState));
        }

        /// <summary>
        /// 특정 범위를 분할해서 병렬로 로컬 작업을 수행합니다 (병렬 작업을 너무 세분화 시키지 않고, 분할해서 작업하면 효과적이다.)
        /// </summary>
        /// <typeparam name="TLocal"></typeparam>
        /// <param name="fromInclusive">시작 인덱스</param>
        /// <param name="toExclusive">종료 인덱스</param>
        /// <param name="parallelOptions">병렬 실행 옵션</param>
        /// <param name="localInit">구간 단위의 초기화 값을 제공하는 delegate</param>
        /// <param name="localBody">구간 단위의 실행을 담당하는 delegate</param>
        /// <param name="localFinally">구간별 실행 결과 합치는 delegate</param>
        /// <returns></returns>
        public static ParallelLoopResult ForRange<TLocal>(int fromInclusive,
                                                          int toExclusive,
                                                          ParallelOptions parallelOptions,
                                                          Func<TLocal> localInit,
                                                          Func<int, int, ParallelLoopState, TLocal, TLocal> localBody,
                                                          Action<TLocal> localFinally) {
            parallelOptions.ShouldNotBeNull("parallelOptions");
            localInit.ShouldNotBeNull("localInit");
            localBody.ShouldNotBeNull("localBody");
            localFinally.ShouldNotBeNull("localFinally");

            if(IsDebugEnabled)
                log.Debug("특정 범위를 분할해서 병렬로 로컬 작업을 수행합니다. 구간=[{0},{1})", fromInclusive, toExclusive);

            return Parallel.ForEach(CreateRangePartition(fromInclusive, toExclusive),
                                    parallelOptions,
                                    localInit,
                                    (range, loopState, x) => localBody(range.Item1, range.Item2, loopState, x),
                                    localFinally);
        }

        #endregion

        #region << Int64, Parallel Options >>

        /// <summary>
        /// 특정 범위를 분할해서 병렬로 로컬 작업을 수행합니다 (병렬 작업을 너무 세분화 시키지 않고, 분할해서 작업하면 효과적이다.)
        /// </summary>
        /// <param name="fromInclusive">시작 인덱스</param>
        /// <param name="toExclusive">종료 인덱스</param>
        /// <param name="parallelOptions">병렬 실행 옵션</param>
        /// <param name="body">구간 단위의 실행을 담당하는 delegate</param>
        /// <returns></returns>
        public static ParallelLoopResult ForRange(long fromInclusive,
                                                  long toExclusive,
                                                  ParallelOptions parallelOptions,
                                                  Action<long, long> body) {
            parallelOptions.ShouldNotBeNull("parallelOptions");
            body.ShouldNotBeNull("body");

            if(IsDebugEnabled)
                log.Debug("특정 범위를 분할해서 병렬로 로컬 작업을 수행합니다. 구간=[{0},{1})", fromInclusive, toExclusive);

            return Parallel.ForEach(CreateRangePartition(fromInclusive, toExclusive),
                                    parallelOptions,
                                    range => body(range.Item1, range.Item2));
        }

        /// <summary>
        /// 특정 범위를 분할해서 병렬로 로컬 작업을 수행합니다 (병렬 작업을 너무 세분화 시키지 않고, 분할해서 작업하면 효과적이다.)
        /// </summary>
        /// <param name="fromInclusive">시작 인덱스</param>
        /// <param name="toExclusive">종료 인덱스</param>
        /// <param name="parallelOptions">병렬 실행 옵션</param>
        /// <param name="body">구간 단위의 실행을 담당하는 delegate</param>
        /// <returns></returns>
        public static ParallelLoopResult ForRange(long fromInclusive,
                                                  long toExclusive,
                                                  ParallelOptions parallelOptions,
                                                  Action<long, long, ParallelLoopState> body) {
            parallelOptions.ShouldNotBeNull("parallelOptions");
            body.ShouldNotBeNull("body");

            if(IsDebugEnabled)
                log.Debug("특정 범위를 분할해서 병렬로 로컬 작업을 수행합니다. 구간=[{0},{1})", fromInclusive, toExclusive);

            return Parallel.ForEach(CreateRangePartition(fromInclusive, toExclusive),
                                    parallelOptions,
                                    (range, loopState) => body(range.Item1, range.Item2, loopState));
        }

        /// <summary>
        /// 특정 범위를 분할해서 병렬로 로컬 작업을 수행합니다 (병렬 작업을 너무 세분화 시키지 않고, 분할해서 작업하면 효과적이다.)
        /// </summary>
        /// <typeparam name="TLocal"></typeparam>
        /// <param name="fromInclusive">시작 인덱스</param>
        /// <param name="toExclusive">종료 인덱스</param>
        /// <param name="parallelOptions">병렬 실행 옵션</param>
        /// <param name="localInit">구간 단위의 초기화 값을 제공하는 delegate</param>
        /// <param name="localBody">구간 단위의 실행을 담당하는 delegate</param>
        /// <param name="localFinally">구간별 실행 결과 합치는 delegate</param>
        /// <returns></returns>
        public static ParallelLoopResult ForRange<TLocal>(long fromInclusive,
                                                          long toExclusive,
                                                          ParallelOptions parallelOptions,
                                                          Func<TLocal> localInit,
                                                          Func<long, long, ParallelLoopState, TLocal, TLocal> localBody,
                                                          Action<TLocal> localFinally) {
            parallelOptions.ShouldNotBeNull("parallelOptions");
            localInit.ShouldNotBeNull("localInit");
            localBody.ShouldNotBeNull("localBody");
            localFinally.ShouldNotBeNull("localFinally");

            if(IsDebugEnabled)
                log.Debug("특정 범위를 분할해서 병렬로 로컬 작업을 수행합니다. 구간=[{0},{1})", fromInclusive, toExclusive);

            return Parallel.ForEach(CreateRangePartition(fromInclusive, toExclusive),
                                    parallelOptions,
                                    localInit,
                                    (range, loopState, x) => localBody(range.Item1, range.Item2, loopState, x),
                                    localFinally);
        }

        #endregion

        /// <summary>
        /// 지정된 범위를 Process Count 수만큼 분할하여, 분할된 범위의 컬렉션을 반환합니다. (NET-4.0에서는 기본으로 제공합니다)
        /// </summary>
        /// <param name="fromInclusive"></param>
        /// <param name="toExclusive"></param>
        /// <returns></returns>
        internal static OrderablePartitioner<Tuple<int, int>> CreateRangePartition(int fromInclusive, int toExclusive) {
            return Partitioner.Create(fromInclusive, toExclusive);
        }

        /// <summary>
        /// 지정된 범위를 Process Count 수만큼 분할하여, 분할된 범위의 컬렉션을 반환합니다. (NET-4.0에서는 기본으로 제공합니다)
        /// </summary>
        /// <param name="fromInclusive"></param>
        /// <param name="toExclusive"></param>
        /// <returns></returns>
        internal static OrderablePartitioner<Tuple<long, long>> CreateRangePartition(long fromInclusive, long toExclusive) {
            return Partitioner.Create(fromInclusive, toExclusive);
        }
    }
}