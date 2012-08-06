using System;
using System.Linq.Expressions;

namespace NSoft.NFramework.LinqEx {
    /// <summary>
    /// 복수의 Predicate용 Expression을 만드는 Predicate 빌더이다. NHibernate의 DetachedCriteria와 유사
    /// 사용할 때 반환받은 Expression을 Compile을 하면 Func{T, bool} 의 predicate 가 만들어진다.
    /// </summary>
    /// <remarks>
    /// 참고 및 예제는 http://www.albahari.com/nutshell/predicatebuilder.html 
    /// </remarks>
    public static class PredicateBuilder {
        /// <summary>
        /// 항상 True를 반환하는 Predicate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> True<T>() {
            return source => true;
        }

        /// <summary>
        /// 항상 False를 반환하는 Predicate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> False<T>() {
            return source => false;
        }

        /// <summary>
        /// 두 expression의 OR 연산을 하는 Predicate를 빌드한다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr1"></param>
        /// <param name="expr2"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2) {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters);

            return Expression.Lambda<Func<T, bool>>(Expression.Or(expr1.Body, invokedExpr), expr1.Parameters);
        }

        /// <summary>
        /// 두 expression의 AND 연산을 하는 Predicate를 빌드한다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr1"></param>
        /// <param name="expr2"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2) {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters);

            return Expression.Lambda<Func<T, bool>>(Expression.And(expr1.Body, invokedExpr), expr1.Parameters);
        }
    }
}