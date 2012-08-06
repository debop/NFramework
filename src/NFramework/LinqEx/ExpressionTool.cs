using System;
using System.Linq;
using System.Linq.Expressions;

namespace NSoft.NFramework.LinqEx {
    /// <summary>
    /// Refer to http://www.albahari.com/nutshell/linqkit.html and http://tomasp.net/blog/linq-expand.aspx for more information.
    /// </summary>
    public static class ExpressionTool {
        /// <summary>
        /// 지정된 IQueriable{T}를 확장이 가능하도록 변환한다. (decoration pattern을 이용하였음)
        /// </summary>
        public static IQueryable<T> AsExpandable<T>(this IQueryable<T> query) {
            if(query is ExpandableQuery<T>) return query;
            return new ExpandableQuery<T>(query);
        }

        /// <summary>
        /// Expand a specified expression.
        /// </summary>
        public static Expression<TDelegate> Expand<TDelegate>(this Expression<TDelegate> expr) {
            return (Expression<TDelegate>)new ExpressionExpander().Visit(expr);
        }

        /// <summary>
        /// Expand a specified expression.
        /// </summary>
        public static Expression Expand(this Expression expr) {
            return new ExpressionExpander().Visit(expr);
        }

        /// <summary>
        /// Invoke a specified expression.
        /// </summary>
        public static TResult Invoke<TResult>(this Expression<Func<TResult>> expr) {
            return expr.Compile().Invoke();
        }

        /// <summary>
        /// Invoke a specified expression.
        /// </summary>
        public static TResult Invoke<T1, TResult>(this Expression<Func<T1, TResult>> expr, T1 arg1) {
            return expr.Compile().Invoke(arg1);
        }

        /// <summary>
        /// Invoke a specified expression.
        /// </summary>
        public static TResult Invoke<T1, T2, TResult>(this Expression<Func<T1, T2, TResult>> expr, T1 arg1, T2 arg2) {
            return expr.Compile().Invoke(arg1, arg2);
        }

        /// <summary>
        /// Invoke a specified expression.
        /// </summary>
        public static TResult Invoke<T1, T2, T3, TResult>(
            this Expression<Func<T1, T2, T3, TResult>> expr, T1 arg1, T2 arg2, T3 arg3) {
            return expr.Compile().Invoke(arg1, arg2, arg3);
        }

        /// <summary>
        /// Invoke a specified expression.
        /// </summary>
        public static TResult Invoke<T1, T2, T3, T4, TResult>(
            this Expression<Func<T1, T2, T3, T4, TResult>> expr, T1 arg1, T2 arg2, T3 arg3, T4 arg4) {
            return expr.Compile().Invoke(arg1, arg2, arg3, arg4);
        }
    }
}