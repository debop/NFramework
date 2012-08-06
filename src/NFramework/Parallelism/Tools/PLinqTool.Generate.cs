using System;
using System.Collections.Generic;
using System.Linq;
using NSoft.NFramework.LinqEx;

namespace NSoft.NFramework.Parallelism.Tools {
    public static partial class PLinqTool {
        /// <summary>
        /// <paramref name="count"/> 수만큼 <paramref name="generator"/>를 호출하여 결과값을 열거합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="count"></param>
        /// <param name="generator"></param>
        /// <returns></returns>
        public static IEnumerable<T> Generate<T>(int count, Func<T> generator) {
            generator.ShouldNotBeNull("generator");

            return
                ParallelEnumerable
                    .Range(0, count)
                    .AsOrdered()
                    .Select(i => generator());
        }

        public static IEnumerable<T> Generate<T>(int count, Func<int, T> generator) {
            return Generate(0, count, generator);
        }

        public static IEnumerable<T> Generate<T>(int start, int count, Func<int, T> generator) {
            return
                ParallelEnumerable
                    .Range(0, count)
                    .AsOrdered()
                    .Select(i => generator(start + i));
        }

        public static IEnumerable<T> Generate<T>(int start, int count, int step, Func<int, T> generator) {
            return
                ParallelEnumerable
                    .Range(0, count)
                    .AsOrdered()
                    .Select(i => generator(start + i * step));
        }

        public static IEnumerable<T> Generate<T>(T start, int count, Func<T, int, T> generator) {
            return
                ParallelEnumerable
                    .Range(0, count)
                    .AsOrdered()
                    .Select(i => generator(start, i));
        }

        public static IEnumerable<T> Generate<T>(double start, double step, int count, Func<double, T> generator) {
            return
                ParallelEnumerable
                    .Range(0, count)
                    .AsOrdered()
                    .Select(i => generator(start + i * step));
        }

        public static IEnumerable<T> Generate<T, TStep>(T start, int count, TStep step, Func<T, int, TStep, T> generator) {
            typeof(T).ShouldBeNumericType();
            count.ShouldBePositiveOrZero("count");

            // HINT: 병렬로 수행하면, 불리하다 (선후관계가 연계되어 있기 때문에)
            for(var i = 0; i < count; i++) {
                yield return start;
                start = generator(start, i, step);
            }
        }

        public static IEnumerable<DateTime> Generate(DateTime start, int count, TimeSpan step) {
            Guard.Assert(() => step != TimeSpan.Zero, "step 값이 TimeSpan.Zero이면 안됩니다.");

            // HINT: 병렬로 수행하면, 불리하다 (선후관계가 연계되어 있기 때문에)
            for(var i = 0; i < count; i++) {
                yield return start;
                start = start.Add(step);
            }
        }

        public static IEnumerable<T> Generate<T>(T start, int count, T step) {
            typeof(T).ShouldBeNumericType();

            for(var i = 0; i < count; i++) {
                yield return start;
                start = LinqTool.Operators<T>.Add(start, step);
            }
        }
    }
}