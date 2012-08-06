using System;
using System.Collections.Generic;
using System.Linq;

namespace NSoft.NFramework.LinqEx {
    public static partial class LinqTool {
        /// <summary>
        /// <paramref name="count"/> 수만큼 <paramref name="generator"/>를 호출하여 결과값을 열거합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="count"></param>
        /// <param name="generator"></param>
        /// <returns></returns>
        public static IEnumerable<T> Generate<T>(int count, Func<T> generator) {
            generator.ShouldNotBeNull("generator");

            for(var i = 0; i < count; i++)
                yield return generator();
        }

        public static IEnumerable<T> Generate<T>(int count, Func<int, T> generator) {
            return Generate(0, count, generator);
        }

        public static IEnumerable<T> Generate<T>(int start, int count, Func<int, T> generator) {
            for(var i = 0; i < count; i++)
                yield return generator(start + i);
        }

        public static IEnumerable<T> Generate<T>(int start, int count, int step, Func<int, T> generator) {
            for(var i = 0; i < count; i++)
                yield return generator(start + i * step);
        }

        public static IEnumerable<T> Generate<T>(T start, int count, Func<T, int, T> generator) {
            for(var i = 0; i < count; i++)
                yield return generator(start, i);
        }

        public static IEnumerable<T> Generate<T>(double start, double step, int count, Func<double, T> generator) {
            for(var i = 0; i < count; i++)
                yield return generator(start + i * step);
        }

        public static IEnumerable<T> Generate<T, TStep>(T start, int count, TStep step, Func<T, int, TStep, T> generator) {
            typeof(T).ShouldBeNumericType();

            for(var i = 0; i < count; i++) {
                yield return start;
                start = generator(start, i, step);
            }
        }

        public static IEnumerable<DateTime> Generate(DateTime start, int count, TimeSpan step) {
            Guard.Assert(() => step != TimeSpan.Zero, "step 값이 TimeSpan.Zero이면 안됩니다.");

            for(var i = 0; i < count; i++) {
                yield return start;
                start = start.Add(step);
            }
        }

        public static IEnumerable<T> Generate<T>(T start, int count, T step) {
            typeof(T).ShouldBeNumericType();

            for(var i = 0; i < count; i++) {
                yield return start;
                start = Operators<T>.Add(start, step);
            }
        }

        /// <summary>
        /// 시퀀스를 졍렬했을 때, <paramref name="order"/>순서에 해당하는 값을 반환합니다.
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <param name="order">순서 (1..N)</param>
        /// <returns></returns>
        public static double OrderedElementAt(this IEnumerable<double> source, int order) {
            return source.OrderBy(x => x).ElementAt(order - 1);
        }

        /// <summary>
        /// 시퀀스를 졍렬했을 때, <paramref name="order"/>순서에 해당하는 값을 반환합니다.
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <param name="order">순서 (1..N)</param>
        /// <returns></returns>
        public static double OrderedElementAtOrDefault(this IEnumerable<double> source, int order) {
            return source.OrderBy(x => x).ElementAtOrDefault(order - 1);
        }

        /// <summary>
        /// 시퀀스를 졍렬했을 때, <paramref name="order"/>순서에 해당하는 값을 반환합니다.
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <param name="orderer">시퀀스 정렬 방식</param>
        /// <param name="selector">변량중 정렬 및 선택할 요소 선택자</param>
        /// <param name="order">순서 (1..N)</param>
        /// <returns></returns>
        public static double OrderedElementAt<TSource, TOrder>(this IEnumerable<TSource> source, Func<TSource, TOrder> orderer,
                                                               Func<TSource, double> selector, int order) {
            return source.OrderBy(orderer).Select(selector).ElementAt(order - 1);
        }

        /// <summary>
        /// 시퀀스를 졍렬했을 때, <paramref name="order"/>순서에 해당하는 값을 반환합니다.
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <param name="orderer">시퀀스 정렬 방식</param>
        /// <param name="selector">변량중 정렬 및 선택할 요소 선택자</param>
        /// <param name="order">순서 (1..N)</param>
        /// <returns></returns>
        public static double OrderedElementAtOrDefault<TSource, TOrder>(this IEnumerable<TSource> source, Func<TSource, TOrder> orderer,
                                                                        Func<TSource, double> selector, int order) {
            return source.OrderBy(orderer).Select(selector).ElementAtOrDefault(order - 1);
        }

        /// <summary>
        /// <paramref name="fromInclusive"/> ~ <paramref name="toExclusive"/> 구간을 <paramref name="step"/> 간격으로 나열합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fromInclusive">하한값</param>
        /// <param name="toExclusive">상한값</param>
        /// <param name="step">단계</param>
        /// <returns></returns>
        public static IEnumerable<T> Range<T>(T fromInclusive, T toExclusive, T step) where T : IComparable<T> {
            typeof(T).ShouldBeNumericType();
            Guard.Assert(() => Equals(step, default(T)) == false, "step 값이 기본값이면 안됩니다.");

            for(var i = fromInclusive; Operators<T>.LessThan(i, toExclusive); i = Operators<T>.Add(i, step)) {
                yield return i;
            }
        }

        /// <summary>
        /// <paramref name="source"/> 시퀀스를 <paramref name="times"/> 만큼 반복해서 열거합니다.
        /// </summary>
        /// <typeparam name="T">항목 수형</typeparam>
        /// <param name="source">시퀀스</param>
        /// <param name="times">반복 횟수</param>
        /// <returns>반복된 시퀀스</returns>
        public static IEnumerable<T> Repeat<T>(this IEnumerable<T> source, int times = 1) {
            source.ShouldNotBeNull("source");
            if(IsDebugEnabled)
                log.Debug("시퀀스를 times[{0}] 만큼 반복해서 열거합니다.", times);

            for(var i = 0; i < times; i++)
                using(var iter = source.GetEnumerator())
                    while(iter.MoveNext())
                        yield return iter.Current;
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, int? seed = null) {
            source.ShouldNotBeNull("source");

            var items = source.ToArray();
            var count = items.Length;

            if(IsDebugEnabled)
                log.Debug("시퀀스 요소 순서를 무작위로 섞어서 열거합니다...");

            var rnd = new Random(seed ?? Guid.NewGuid().GetHashCode());

            for(var i = 0; i < count; i++) {
                var r = rnd.Next(i, count);
                yield return items[r];
                items[r] = items[i];
            }
        }

        /// <summary>
        /// <paramref name="source"/>를 열거할 때, <paramref name="offset"/>만큼 왼쪽으로 Shift 해서 열거합니다. 즉 <paramref name="offset"/> 만큼 건너뛰고, 열거하고, 나머지를 열거합니다.
        /// </summary>
        /// <typeparam name="T">요소 수형</typeparam>
        /// <param name="source">원본 시퀀스</param>
        /// <param name="offset">Rotate할 양</param>
        /// <returns></returns>
        /// <example>
        /// <code>
        ///		new[] {1,2,3,4,5}.RotateLeft(2); // => { 3,4,5,1,2} 
        /// </code>
        /// </example>
        public static IEnumerable<T> RotateLeft<T>(this IEnumerable<T> source, int offset = 1) {
            source.ShouldNotBeNull("source");
            if(IsDebugEnabled)
                log.Debug("시퀀스를 왼쪽으로 [{0}]만큼 Rotate시켜서 열거합니다.", offset);

            foreach(T item in source.Skip(offset))
                yield return item;

            foreach(var item in source.Take(offset))
                yield return item;
        }

        /// <summary>
        ///  <paramref name="source"/>를 열거할 때, <paramref name="offset"/>만큼 오른쪽으로 Shift 해서 열거합니다. 즉 <paramref name="offset"/> 만큼 건너뛰고, 열거하고, 나머지를 열거합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        /// <example>
        /// <code>
        ///		new[] {1,2,3,4,5}.RotateRight(2); // => { 4,5,1,2,3 } 
        /// </code>
        /// </example>
        public static IEnumerable<T> RotateRight<T>(this IEnumerable<T> source, int offset = 1) {
            source.ShouldNotBeNull("source");
            if(IsDebugEnabled)
                log.Debug("시퀀스를 오른쪽으로 [{0}]만큼 Rotate시켜서 열거합니다.", offset);

            int shift = source.Count() - offset;
            return RotateLeft(source, shift);
        }
    }
}