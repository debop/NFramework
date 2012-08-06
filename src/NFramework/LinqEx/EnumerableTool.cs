using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using NSoft.NFramework.Threading;

namespace NSoft.NFramework.LinqEx {
    /// <summary>
    /// LINQ 용 확장 함수들을 제공합니다.
    /// </summary>
    public static partial class EnumerableTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private static readonly Random Rnd = new ThreadSafeRandom();

        /// <summary>
        /// 지정한 시퀀가 비었으면 true를 반환합니다.
        /// </summary>
        /// <seealso cref="ItemExists{T}(System.Collections.Generic.IEnumerable{T},System.Func{T,bool})"/>
        public static bool IsEmptySequence<T>(this IEnumerable<T> source, Func<T, bool> predicate = null) {
            return (source == null || !source.Any(predicate ?? (_ => true)));
        }

        /// <summary>
        /// 시퀀스가 비어있지 않았는지 검사합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static bool IsNotEmptySequence<T>(this IEnumerable<T> source, Func<T, bool> predicate = null) {
            return (source != null && source.Any(predicate ?? (_ => true)));
        }

        /// <summary>
        /// 지정된 컬렉션에 검색 조건에 맞는 요소가 존재하는지 알아본다.
        /// </summary>
        public static bool ItemExists<T>(this IEnumerable<T> source, Func<T, bool> predicate = null) {
            return (source != null && source.Any(predicate ?? (_ => true)));
        }

        /// <summary>
        /// 지정된 시퀀스의 요소가 최소한 지정된 갯수 이상인지 확인한다.
        /// </summary>
        /// <typeparam name="T">시퀀스 요소의 수형</typeparam>
        /// <param name="source">시퀀스</param>
        /// <param name="count">최소 요소 수</param>
        /// <returns>지정된 시퀀스의 요소수가 지정된 최소 요소수보다 크거나 같다면 True, 작다면 False를 반환한다.</returns>
        public static bool ItemExistsAtLeast<T>(this IEnumerable<T> source, int count) {
            if(source == null && count > 0)
                return false;

            // NOTE: 전체 갯수를 센다는 건 너무 많다. (무한대의 sequence도 있기 때문에 먼저 검사할 갯수만큼만 Take한다.)
            //
            return (source.Take(count).Count() == count);
        }

        /// <summary>
        /// sequence를 count 수 만큼 반복한 sequence를 생성한다.
        /// </summary>
        public static IEnumerable<T> RepeatSequenceUnsafe<T>(this IEnumerable source, int count) {
            source.ShouldNotBeNull("source");

            if(count <= 0)
                yield break;

            var list = source.ConvertUnsafe<T>().ToList();

            for(var i = 0; i < count; i++)
                for(var j = 0; j < list.Count; j++)
                    yield return list[j];
        }

        /// <summary>
        /// sequence를 count 수 만큼 반복한 sequence를 생성한다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IEnumerable<T> RepeatSequence<T>(this IEnumerable<T> source, int count) {
            source.ShouldNotBeNull("source");
            if(count <= 0)
                yield break;

            var list = source.ToList();

            for(var i = 0; i < count; i++)
                for(var j = 0; j < list.Count; j++)
                    yield return list[j];
        }

        /// <summary>
        /// <paramref name="count"/> 갯수만큼 요소를 생성해서 반환합니다.
        /// </summary>
        /// <typeparam name="T">요소의 수형</typeparam>
        /// <param name="itemFactory">요소 생성 함수</param>
        /// <param name="count">생성할 요소의 갯수</param>
        /// <returns>생성된 요소의 열거자</returns>
        public static IEnumerable<T> Replicate<T>(Func<T> itemFactory, int count) {
            itemFactory.ShouldNotBeNull("itemFactory");
            count.ShouldBePositiveOrZero("count");

            for(var i = 0; i < count; i++)
                yield return itemFactory();
        }

        /// <summary>
        /// <paramref name="count"/> 갯수만큼 요소를 생성해서 반환합니다.
        /// </summary>
        /// <typeparam name="T">요소의 수형</typeparam>
        /// <param name="itemFactory">요소 생성 함수</param>
        /// <param name="count">생성할 요소의 갯수</param>
        /// <returns>생성된 요소의 열거자</returns>
        public static IEnumerable<T> Replicate<T>(Func<int, T> itemFactory, int count) {
            itemFactory.ShouldNotBeNull("itemFactory");
            count.ShouldBePositiveOrZero("count");

            for(var i = 0; i < count; i++)
                yield return itemFactory(i);
        }

        /// <summary>
        /// <paramref name="sequence"/>중에 <paramref name="canRemove"/>를 만족시키는 요소는 삭제합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence"></param>
        /// <param name="canRemove"></param>
        /// <returns></returns>
        public static int RemoveAll<T>(this IList<T> sequence, Func<T, bool> canRemove) {
            var removed = 0;

            for(var i = sequence.Count - 1; i >= 0; i--) {
                if(canRemove(sequence[i])) {
                    sequence.RemoveAt(i);
                    removed++;
                }
            }

            return removed;
        }

        /// <summary>
        /// 시작값으로부터 변화하는 시퀀스를 만든다. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="seed"></param>
        /// <param name="step"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IEnumerable<T> SerialSequence<T>(T seed, T step, int count) where T : IComparable<T> {
            var item = seed;
            for(var i = 0; i < count; i++) {
                yield return item;
                item = LinqTool.Operators<T>.Add(item, step);
            }
        }

        /// <summary>
        /// 지정한 값이 MinValue, MaxValue 내에 있는지 판단한다.
        /// </summary>
        /// <typeparam name="T">IComparable{T} 를 구현한 수형이어야 한다.</typeparam>
        /// <param name="source">검사할 값</param>
        /// <param name="minValue">최소 값</param>
        /// <param name="maxValue">최대 값</param>
        /// <param name="comparer">T 수형의 비교자</param>
        /// <returns>최소값이상, 최대값 이하면 True, 아니면 False를 반환한다</returns>
        public static bool Between<T>(this T source, T minValue, T maxValue, IComparer<T> comparer = null) where T : IComparable<T> {
            source.ShouldNotBeNull("source");
            comparer = comparer ?? Comparer<T>.Default;

            return comparer.Compare(source, minValue) >= 0 &&
                   comparer.Compare(source, maxValue) <= 0;
        }

        /// <summary>
        /// 지정한 소스의 요소가 최소, 최대 구간 안에 있는 값만 반환한다. (최대, 최소 값 포함)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">시퀀스</param>
        /// <param name="minValue">최소 값</param>
        /// <param name="maxValue">최대 값</param>
        /// <returns></returns>
        public static IEnumerable<T> Between<T>(this IEnumerable<T> source, T minValue, T maxValue) where T : IComparable<T> {
            source.ShouldNotBeNull("source");
            return source.Where(item => item.Between(minValue, maxValue));
        }

        /// <summary>
        /// KeyValuePair{TKey, TValue} 의 시퀀스를 IDictionary{TKey, TValue} 로 바꾼다.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IDictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source) {
            source.ShouldNotBeNull("source");
            return source.ToDictionary(pair => pair.Key, pair => pair.Value);
        }

#if !SILVERLIGHT

        /// <summary>
        /// 지정된 시퀀스를 BindingList{T}로 변환한다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static BindingList<T> ToBindingList<T>(this IEnumerable<T> source) {
            // source.ShouldNotBeNull("source");
            return new BindingList<T>(source.ToList());
        }

        /// <summary>
        /// 지정된 시퀀스를 BindingList{T}로 변환한다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static BindingList<T> ToBindingListUnsafe<T>(this IEnumerable source) {
            // source.ShouldNotBeNull("source");
            return new BindingList<T>(source.ToListUnsafe<T>());
        }

#endif
    }
}