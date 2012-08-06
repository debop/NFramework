using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NSoft.NFramework.LinqEx {
    public static partial class EnumerableTool {
        /// <summary>
        /// 요소의 수를 계산한다.
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <returns>요소의 수</returns>
        public static int CountUnsafe(this IEnumerable source) {
            source.ShouldNotBeNull("source");
            return source.GetEnumerator().CountUnsafe();
        }

        /// <summary>
        /// 요소의 수를 계산한다.
        /// </summary>
        /// <param name="iterator">반복자</param>
        /// <returns>요소의 수</returns>
        public static int CountUnsafe(this IEnumerator iterator) {
            iterator.ShouldNotBeNull("iterator");

            int length = 0;

            while(iterator.MoveNext())
                length++;
            return length;
        }

        /// <summary>
        /// 지정된 collection에 유일하게 1개의 요소만 있는지 검사하고, 그 값을 반환한다.
        /// 여러개의 요소가 있다면 예외를 발생시키고, 요소가 없다면 null을 반환시킨다.
        /// </summary>
        /// <param name="source">시퀀스</param>
        /// <returns>유일한 값, 없으면 null을 반환한다.</returns>
        public static object SingleUnsafe(this IEnumerable source) {
            source.ShouldNotBeNull("source");
            // Guard.Assert(source.Count() <= 1, "Collection does not have exactly one item.");

            foreach(var v in source)
                return v;

            return null;
        }

        /// <summary>
        /// sequence 를 지정된 수형으로 변환한 sequence로 만듭니다. 
        /// Nullable로 변환시에는 x=>(double?)x 와 같은 expression을 넣어줘야 제대로 됩니다.
        /// </summary>
        /// <typeparam name="TResult">변환할 수형</typeparam>
        /// <param name="source">시퀀스</param>
        /// <returns>변환된 시퀀스</returns>
        public static IEnumerable<TResult> ConvertUnsafe<TResult>(this IEnumerable source) {
            return source.GetEnumerator().ConvertUnsafe<TResult>();
        }

        /// <summary>
        /// sequence 를 지정된 수형으로 변환한 sequence로 만듭니다. 
        /// Nullable로 변환시에는 x=>(double?)x 와 같은 expression을 넣어줘야 제대로 됩니다.
        /// </summary>
        /// <typeparam name="TResult">변환할 수형</typeparam>
        /// <param name="iterator">반복자</param>
        /// <returns>변환된 시퀀스</returns>
        public static IEnumerable<TResult> ConvertUnsafe<TResult>(this IEnumerator iterator) {
            iterator.ShouldNotBeNull("iterator");

            while(iterator.MoveNext()) {
                yield return iterator.Current.AsValue<TResult>();
            }
        }

        /// <summary>
        /// sequence 를 지정된 수형으로 변환한 sequence로 만듭니다. 
        /// Nullable로 변환시에는 x=>(double?)x 와 같은 expression을 넣어줘야 제대로 됩니다.
        /// </summary>
        /// <typeparam name="TResult">변환할 수형</typeparam>
        /// <param name="source">시퀀스</param>
        /// <param name="converter">시퀀스 요소의 변환방식을 표현한 변환 메소드</param>
        /// <returns>변환된 시퀀스</returns>
        /// <example>
        /// <code>
        ///		int[] numbers = new int[] {1,2,3,4,5,6};
        ///     var negatives = numbers.Convert&lt;int&gt;(x=>-x);	 // 모두 음수로 변경
        /// </code>
        /// </example>
        public static IEnumerable<TResult> ConvertUnsafe<TResult>(this IEnumerable source, Func<object, TResult> converter) {
            converter.ShouldNotBeNull("converter");
            return source.GetEnumerator().ConvertUnsafe(converter);
        }

        /// <summary>
        /// sequence 를 지정된 수형으로 변환한 sequence로 만듭니다. 
        /// Nullable로 변환시에는 x=>(double?)x 와 같은 expression을 넣어줘야 제대로 됩니다.
        /// </summary>
        /// <typeparam name="TResult">변환할 수형</typeparam>
        /// <param name="iterator">반복자</param>
        /// <param name="converter">시퀀스 요소의 변환방식을 표현한 변환 메소드</param>
        /// <returns>변환된 시퀀스</returns>
        public static IEnumerable<TResult> ConvertUnsafe<TResult>(this IEnumerator iterator, Func<object, TResult> converter) {
            converter.ShouldNotBeNull("converter");
            while(iterator.MoveNext()) {
                yield return converter(iterator.Current);
            }
        }

        /// <summary>
        /// 지정된 시퀀스를 T 수형의 1차원 배열로 변환
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T[] ToArrayUnsafe<T>(this IEnumerable source) {
            return source.Cast<T>().ToArray();
        }

        /// <summary>
        /// 시퀀스를 IList{T} 형식으로 변환한다.
        /// </summary>
        /// <typeparam name="T">변환될 시퀀스 요소의 수형</typeparam>
        /// <param name="source">시퀀스</param>
        /// <returns>T 수형의 요소를 가진 시퀀스</returns>
        public static IList<T> ToListUnsafe<T>(this IEnumerable source) {
            if(source == null)
                return new List<T>();

            return source.ConvertUnsafe<T>().ToList();
        }

        /// <summary>
        /// items를 IList{T} 형식으로 변환한다.
        /// </summary>
        /// <typeparam name="T">변환될 시퀀스 요소의 수형</typeparam>
        /// <param name="source">시퀀스</param>
        /// <returns>T 수형의 요소를 가진 시퀀스</returns>
        public static IList<T> ToList<T>(params T[] source) {
            if(source == null)
                return new List<T>();

            return new List<T>(source);
        }
    }
}