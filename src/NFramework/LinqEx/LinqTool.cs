using System;
using System.Linq.Expressions;
using NSoft.NFramework.Threading;

namespace NSoft.NFramework.LinqEx {
    public static partial class LinqTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public static readonly Random Rnd = new ThreadSafeRandom();

        /// <summary>
        /// Lambda Expression을 이용하여, 사칙연산등을 수행하는 함수를 제공합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static class Operators<T> {
            public static Func<T, T, T> Add = BuildAdd();
            public static Func<T, T, T> Subtract = BuildSubtract();
            public static Func<T, T, T> Multiply = BuildMultiply();
            public static Func<T, T, T> Divide = BuildDivide();

            public static Func<T, T, bool> Equal = BuildEqual();
            public static Func<T, T, bool> GreaterThan = BuildGreaterThan();
            public static Func<T, T, bool> GreaterThanOrEqual = BuildGreaterThanOrEqual();
            public static Func<T, T, bool> LessThan = BuildLessThan();
            public static Func<T, T, bool> LessThanOrEqual = BuildLessThanOrEqual();

            public static Func<T, T> Negate = BuildNegate();

            internal static Func<T, T, T> BuildAdd() {
                var type = typeof(T);
                var left = Expression.Parameter(type, "left");
                var right = Expression.Parameter(type, "right");

                return Expression.Lambda<Func<T, T, T>>(Expression.Add(left, right), left, right).Compile();
            }

            internal static Func<T, T, T> BuildSubtract() {
                var type = typeof(T);
                var left = Expression.Parameter(type, "left");
                var right = Expression.Parameter(type, "right");

                return Expression.Lambda<Func<T, T, T>>(Expression.Subtract(left, right), left, right).Compile();
            }

            internal static Func<T, T, T> BuildMultiply() {
                var type = typeof(T);
                var left = Expression.Parameter(type, "left");
                var right = Expression.Parameter(type, "right");

                return Expression.Lambda<Func<T, T, T>>(Expression.Multiply(left, right), left, right).Compile();
            }

            internal static Func<T, T, T> BuildDivide() {
                var type = typeof(T);
                var left = Expression.Parameter(type, "left");
                var right = Expression.Parameter(type, "right");

                return Expression.Lambda<Func<T, T, T>>(Expression.Divide(left, right), left, right).Compile();
            }

            internal static Func<T, T, bool> BuildEqual() {
                var type = typeof(T);
                var left = Expression.Parameter(type, "left");
                var right = Expression.Parameter(type, "right");

                return Expression.Lambda<Func<T, T, bool>>(Expression.Equal(left, right), left, right).Compile();
            }

            internal static Func<T, T, bool> BuildGreaterThan() {
                var type = typeof(T);
                var left = Expression.Parameter(type, "left");
                var right = Expression.Parameter(type, "right");

                return Expression.Lambda<Func<T, T, bool>>(Expression.GreaterThan(left, right), left, right).Compile();
            }

            internal static Func<T, T, bool> BuildGreaterThanOrEqual() {
                var type = typeof(T);
                var left = Expression.Parameter(type, "left");
                var right = Expression.Parameter(type, "right");

                return Expression.Lambda<Func<T, T, bool>>(Expression.GreaterThanOrEqual(left, right), left, right).Compile();
            }

            internal static Func<T, T, bool> BuildLessThan() {
                var type = typeof(T);
                var left = Expression.Parameter(type, "left");
                var right = Expression.Parameter(type, "right");

                return Expression.Lambda<Func<T, T, bool>>(Expression.LessThan(left, right), left, right).Compile();
            }

            internal static Func<T, T, bool> BuildLessThanOrEqual() {
                var type = typeof(T);
                var left = Expression.Parameter(type, "left");
                var right = Expression.Parameter(type, "right");

                return Expression.Lambda<Func<T, T, bool>>(Expression.LessThanOrEqual(left, right), left, right).Compile();
            }

            internal static Func<T, T> BuildNegate() {
                var type = typeof(T);
                var left = Expression.Parameter(type, "left");

                return Expression.Lambda<Func<T, T>>(Expression.Negate(left), left).Compile();
            }
        }
    }
}