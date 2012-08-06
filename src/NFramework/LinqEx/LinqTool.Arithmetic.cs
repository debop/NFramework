using System;
using System.Collections.Generic;

namespace NSoft.NFramework.LinqEx {
    public static partial class LinqTool {
        /// <summary>
        /// Numeric 수형의 두 시퀀스의 항목들을 순서대로 더하기(Add)를 수행합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="leftSequence"></param>
        /// <param name="rightSequence"></param>
        /// <returns></returns>
        public static IEnumerable<T> Add<T>(this IEnumerable<T> leftSequence, IEnumerable<T> rightSequence) {
            typeof(T).ShouldBeNumericType();
            leftSequence.ShouldNotBeNull("leftSequence");
            rightSequence.ShouldNotBeNull("rightSequence");

            using(var left = leftSequence.GetEnumerator())
            using(var right = rightSequence.GetEnumerator()) {
                while(left.MoveNext() && right.MoveNext()) {
                    yield return Operators<T>.Add(left.Current, right.Current);
                }
            }
        }

        /// <summary>
        /// Numeric 수형의 두 시퀀스의 항목들을 순서대로 빼기(Subtract)를 수행합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="leftSequence"></param>
        /// <param name="rightSequence"></param>
        /// <returns></returns>
        public static IEnumerable<T> Subtract<T>(this IEnumerable<T> leftSequence, IEnumerable<T> rightSequence) {
            typeof(T).ShouldBeNumericType();
            leftSequence.ShouldNotBeNull("leftSequence");
            rightSequence.ShouldNotBeNull("rightSequence");

            using(var left = leftSequence.GetEnumerator())
            using(var right = rightSequence.GetEnumerator()) {
                while(left.MoveNext() && right.MoveNext()) {
                    yield return Operators<T>.Subtract(left.Current, right.Current);
                }
            }
        }

        /// <summary>
        /// Numeric 수형의 두 시퀀스의 항목들을 순서대로 곱하기(Multiply)를 수행합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="leftSequence"></param>
        /// <param name="rightSequence"></param>
        /// <returns></returns>
        public static IEnumerable<T> Multiply<T>(this IEnumerable<T> leftSequence, IEnumerable<T> rightSequence) {
            typeof(T).ShouldBeNumericType();
            leftSequence.ShouldNotBeNull("leftSequence");
            rightSequence.ShouldNotBeNull("rightSequence");

            using(var left = leftSequence.GetEnumerator())
            using(var right = rightSequence.GetEnumerator()) {
                while(left.MoveNext() && right.MoveNext()) {
                    yield return Operators<T>.Multiply(left.Current, right.Current);
                }
            }
        }

        /// <summary>
        /// Numeric 수형의 두 시퀀스의 항목들을 순서대로 나누기(Divide)를 수행합니다. 단 0으로 나누면 <see cref="DivideByZeroException"/>이 발생합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="leftSequence"></param>
        /// <param name="rightSequence"></param>
        /// <returns></returns>
        public static IEnumerable<T> Divide<T>(this IEnumerable<T> leftSequence, IEnumerable<T> rightSequence) {
            typeof(T).ShouldBeNumericType();
            leftSequence.ShouldNotBeNull("leftSequence");
            rightSequence.ShouldNotBeNull("rightSequence");

            using(var left = leftSequence.GetEnumerator())
            using(var right = rightSequence.GetEnumerator()) {
                while(left.MoveNext() && right.MoveNext()) {
                    yield return Operators<T>.Divide(left.Current, right.Current);
                }
            }
        }
    }
}