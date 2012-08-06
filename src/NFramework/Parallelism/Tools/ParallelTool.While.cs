using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using NSoft.NFramework.Parallelism.Partitioners;

namespace NSoft.NFramework.Parallelism.Tools {
    public static partial class ParallelTool {
        /// <summary>
        /// 조건이 참을 반환하는 동안에는 반복해서 body 함수를 병렬로 호출합니다.
        /// </summary>
        /// <param name="options">병렬 처리 옵션</param>
        /// <param name="condition">While문의 평가할 조건</param>
        /// <param name="body">실행할 메소드</param>
        public static void While(Func<bool> condition, Action body, ParallelOptions options = null) {
            condition.ShouldNotBeNull("condition");
            body.ShouldNotBeNull("body");

            options = options ?? DefaultParallelOptions;

            if(IsDebugEnabled)
                log.Debug("조건 함수가 True를 반환할 때까지 body 함수를 병렬로 호출합니다.");

            Parallel.ForEach(SingleItemPartitioner.Create(IterateUntilFalse(condition)), options, ignored => body());
        }

        /// <summary>
        /// <paramref name="condition"/> delegate가 True을 반환할 동안만, True를 열거합니다. 만약 False를 반환하면, 열거를 중지합니다.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        private static IEnumerable<bool> IterateUntilFalse(Func<bool> condition) {
            while(condition())
                yield return true;
        }

        /// <summary>
        /// 지정된 초기값(<paramref name="initialValues"/>)들이 <paramref name="body"/> 를 병렬 수행하면서, 모두 다 처리가 될 때까지, 계속 수행합니다.
        /// <paramref name="body"/>메소드 내에서, 처리된 요소는 제거해야만, 끝나게 됩니다.
        /// 꼭 쌀에서 쌀겨를 찾아내는 방식처럼, 오른손, 왼손 번갈아가며 쌀알들을 옮기면서, 쌀겨가 없을 때까지 실행하는 것과 같다.
        /// </summary>
        /// <typeparam name="T">함수 인자의 수형</typeparam>
        /// <param name="options">병렬 옵션</param>
        /// <param name="initialValues">추기 값의 컬렉션</param>
        /// <param name="body">수행할 함수 (인자로 값과 값에 대한 처리를 담는 메소드를 가진다)</param>
        public static void WhileNotEmpty<T>(IEnumerable<T> initialValues,
                                            Action<T, Action<T>> body,
                                            ParallelOptions options = null) {
            initialValues.ShouldNotBeNull("initialValues");
            body.ShouldNotBeNull("body");

            options = options ?? DefaultParallelOptions;

            // 원본과 대상값을 가지도록 두개의 Stack을 생성한다.
            var lists = new[]
                        {
                            new ConcurrentStack<T>(initialValues),
                            new ConcurrentStack<T>()
                        };

            // 더 이상 반복할 것이 없을 때 까지 무한 루프이다^^
            for(int i = 0;; i++) {
                // Source와 Destination을 번갈아가며 사용한다.
                int fromIndex = i % 2;
                var from = lists[fromIndex];
                var to = lists[fromIndex ^ 1];

                if(from.IsEmpty)
                    break;

                // 모든 Source의 요소들을 Destination에 추가한다.
                Action<T> addAction = item => to.Push(item);
                Parallel.ForEach(from, options, item => body(item, addAction));

                from.Clear();
            }
        }
    }
}