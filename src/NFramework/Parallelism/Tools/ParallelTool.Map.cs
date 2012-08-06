using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NSoft.NFramework.Reflections;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Parallelism.Tools {
    public static partial class ParallelTool {
        // HINT : Parallel Map Pattern (http://software.intel.com/en-us/blogs/2009/06/10/parallel-patterns-3-map/)

        /// <summary>
        /// <paramref name="inputs"/> 시퀀스를 매핑 함수인 <paramref name="transform"/>을 통해 매핑한 결과를 반환합니다. 이러한 매핑 작업을 병렬로 수행합니다.
        /// </summary>
        /// <typeparam name="TInput">매핑 입력 값의 수형</typeparam>
        /// <typeparam name="TOutput">매핑 출력 값의 수형</typeparam>
        /// <param name="inputs">입력 값의 시퀀스</param>
        /// <param name="transform">매핑 함수</param>
        /// <returns>매핑된 출력 값의 시퀀스</returns>
        public static IEnumerable<TOutput> MapAsEnumerable<TInput, TOutput>(IEnumerable<TInput> inputs, Func<TInput, TOutput> transform) {
            return MapAsEnumerable(inputs, DefaultParallelOptions, transform);
        }

        /// <summary>
        /// <paramref name="inputs"/> 시퀀스를 매핑 함수인 <paramref name="transform"/>을 통해 매핑한 결과를 반환합니다. 이러한 매핑 작업을 병렬로 수행합니다.
        /// </summary>
        /// <typeparam name="TInput">매핑 입력 값의 수형</typeparam>
        /// <typeparam name="TOutput">매핑 출력 값의 수형</typeparam>
        /// <param name="inputs">입력 값의 시퀀스</param>
        /// <param name="parallelOptions">병렬 처리 옵션</param>
        /// <param name="transform">매핑 함수</param>
        /// <returns>매핑된 출력 값의 시퀀스</returns>
        public static IEnumerable<TOutput> MapAsEnumerable<TInput, TOutput>(IEnumerable<TInput> inputs,
                                                                            ParallelOptions parallelOptions,
                                                                            Func<TInput, TOutput> transform) {
            inputs.ShouldNotBeNull("inputs");
            transform.ShouldNotBeNull("tranform");

            if(IsDebugEnabled)
                log.Debug("병렬 매핑을 수행합니다...");

#if !SILVERLIGHT
            return
                inputs
                    .AsParallel()
                    .WithCancellation(parallelOptions.CancellationToken)
                    .WithDegreeOfParallelism(parallelOptions.MaxDegreeOfParallelism)
                    .AsOrdered()
                    .Select(transform);
#else
			return inputs.Select(input => transform(input));
#endif
        }

        /// <summary>
        /// <paramref name="inputs"/> 시퀀스를 <paramref name="mapFunc"/> 함수를 이용하여 TOutput 수형의 시퀀스로 매핑합니다. 매핑 작업을 병렬로 수행합니다.
        /// </summary>
        /// <typeparam name="TInput">입력 데이타 수형</typeparam>
        /// <typeparam name="TOutput">반환 데이타 수형</typeparam>
        /// <param name="inputs">입력 데이타</param>
        /// <param name="mapFunc">입력을 출력으로 변환하는 함수</param>
        /// <returns>Mapping된 출력 데이타</returns>
        public static IList<TOutput> Map<TInput, TOutput>(IList<TInput> inputs, Func<TInput, TOutput> mapFunc) {
            return Map(inputs, DefaultParallelOptions, mapFunc);
        }

        /// <summary>
        /// <paramref name="inputs"/> 시퀀스를 <paramref name="tranform"/> 함수를 이용하여 TOutput 수형의 시퀀스로 매핑합니다. 매핑 작업을 병렬로 수행합니다.
        /// </summary>
        /// <typeparam name="TInput">입력 데이타 수형</typeparam>
        /// <typeparam name="TOutput">반환 데이타 수형</typeparam>
        /// <param name="inputs">입력 데이타</param>
        /// <param name="parallelOptions">병렬 처리 옵션</param>
        /// <param name="tranform">입력을 출력으로 변환하는 함수</param>
        /// <returns>Mapping된 출력 데이타</returns>
        public static IList<TOutput> Map<TInput, TOutput>(IList<TInput> inputs, ParallelOptions parallelOptions,
                                                          Func<TInput, TOutput> tranform) {
            inputs.ShouldNotBeNull("inputs");
            parallelOptions.ShouldNotBeNull("parallelOptions");
            tranform.ShouldNotBeNull("transform");

            if(IsDebugEnabled)
                log.Debug("수형[{0}]=>수형[{1}]으로 매핑함수를 이용하여 병렬 매핑을 수행합니다.", typeof(TInput).FullName, typeof(TOutput).FullName);

            if(inputs.Count == 0)
                return new List<TOutput>();

            var outputs = new TOutput[inputs.Count];
            var syncLock = new object();

            // TODO: 범위로 나누어서 작업하는 것이 어떨까? 다만 False Sharing 문제로 더 느려질 수 있으니 확인해 봐야 한다. 

            ForRange(0,
                     inputs.Count,
                     () => new KeyValuePair<TOutput[], int[]>(new TOutput[inputs.Count], new int[2]),
                     (from, to, loopstate, localPair) => {
                         var localOutputs = localPair.Key;

                         // 로컬 배열에 변환된 인스턴스를 저장합니다. False Sharing 문제를 해결하기 위해, 로컬 배열에 저장해야 합니다.
                         for(var i = from; i < to; i++)
                             localOutputs[i] = tranform(inputs[i]);

                         localPair.Value[0] = from;
                         localPair.Value[1] = to;

                         return localPair;
                     },
                     localPair => {
                         var localOuputs = localPair.Key;

                         var from = localPair.Value[0];
                         var to = localPair.Value[1];

                         // 로컬 배열로부터 최종 배열로 값을 복사합니다.
                         lock(syncLock)
                             Array.Copy(localOuputs, from, outputs, from, to - from);
                     });

            return outputs.ToList();
        }

        /// <summary>
        /// TInput 수형의 인스턴스의 속성 정보를 기반으로 TOutput 수형의 인스턴스를 생성하고, 같은 속성은 매핑합니다.
        /// 서로 다른 형식이지만, 속성이 비슷한 형식끼리 값을 복사할 때 편리합니다. (DTO 사용 시)
        /// </summary>
        /// <typeparam name="TInput">입력 데이타 수형</typeparam>
        /// <typeparam name="TOutput">반환 데이타 수형</typeparam>
        /// <param name="inputs">입력 데이타</param>
        /// <returns>Mapping된 출력 데이타</returns>
        public static IList<TOutput> MapProperty<TInput, TOutput>(IList<TInput> inputs) where TOutput : new() {
            return MapProperty<TInput, TOutput>(inputs, DefaultParallelOptions);
        }

        /// <summary>
        ///	<paramref name="inputs"/> 시퀀스의 TInput 수형의 속성값으로 새로 생성한 TOutput 수형의 인스턴스의 속성값에 매핑하여 시퀀스로 반환합니다.
        /// 즉 다른 수형끼리 속성 값을 같게 하여 수행하도록 합니다.
        /// </summary>
        /// <typeparam name="TInput">입력 데이타 수형</typeparam>
        /// <typeparam name="TOutput">반환 데이타 수형</typeparam>
        /// <param name="inputs">입력 데이타</param>
        /// <param name="parallelOptions">병렬 처리 옵션</param>
        /// <returns>Mapping된 출력 데이타</returns>
        public static IList<TOutput> MapProperty<TInput, TOutput>(IList<TInput> inputs, ParallelOptions parallelOptions)
            where TOutput : new() {
            return Map(inputs, parallelOptions, input => input.MapProperty<TOutput>(ActivatorTool.CreateInstance<TOutput>, true));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="inputs"></param>
        /// <param name="parallelOptions"></param>
        /// <param name="outputFactory"></param>
        /// <returns></returns>
        public static IList<TOutput> MapProperty<TInput, TOutput>(IList<TInput> inputs, ParallelOptions parallelOptions,
                                                                  Func<TOutput> outputFactory) {
            outputFactory.ShouldNotBeNull("outputFactory");

            return Map(inputs, parallelOptions, input => input.MapProperty(outputFactory, true));
        }
    }
}