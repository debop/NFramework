using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NSoft.NFramework.LinqEx;

namespace NSoft.NFramework.Numerics {
    // NOTE: Parallel 작업은 IEnumerable 일 경우에는 너무 느리다. IList 나 Array여야만 빠르다.

    public static partial class MathTool {
        /// <summary>
        /// 시퀀스의 변량의 합을 병렬로 계산합니다. 
        /// </summary>
        /// <param name="sequence"></param>
        /// <returns></returns>
        public static long LongSumParallel(this IEnumerable<int> sequence) {
            //! PLINQ는 IEnumerable<TSource>에 대해서도 성능이 비슷하지만,  Parallel.ForEach는 IList<TSource>나 TSource[] 이어야만 빠르다!!!
            //

            if(IsDebugEnabled)
                log.Debug(@"Int32 시퀀스의 합을 구합니다.");

            if(sequence is IList<int>) {
                long finalSum = 0L;

                Parallel.ForEach(sequence,
                                 () => 0L,
                                 (item, loopState, localSum) => localSum + item,
                                 (localSum) => Interlocked.Add(ref finalSum, localSum));

                return finalSum;
            }

            return
                sequence
                    .AsParallel()
                    .LongSum();
        }

        public static T SumAsParallel<T>(this IEnumerable<T> sequence) {
            if(IsDebugEnabled)
                log.Debug(@"Int32 시퀀스의 합을 구합니다.");

            T finalSum = default(T);

            Parallel.ForEach(sequence,
                             () => default(T),
                             (item, loopState, localSum) => LinqTool.Operators<T>.Add(localSum, item),
                             (localSum) => {
                                 lock(_syncLock)
                                     finalSum = LinqTool.Operators<T>.Add(finalSum, localSum);
                             });

            return finalSum;
        }
    }
}