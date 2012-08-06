using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Impl;
using NHibernate.Linq;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.TimePeriods;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Data.NHibernateEx {
    /// <summary>
    /// NHibernate 3.0 부터 제공하는 LINQ 기능에 대한 Utility Methods 들을 제공합니다.
    /// 참고 : http://www.beansoftware.com/ASP.NET-Tutorials/Dynamic-LINQ.aspx
    /// http://blogs.msdn.com/b/marcinon/archive/2010/01/14/building-custom-linq-expressions-made-easy-with-dynamicqueryable_2e00_.aspx
    /// </summary>
    public static partial class NHTool {
        /// <summary>
        /// 현재 세션의 IQueryable{T}를 제공합니다. NHibernate.Linq.NHibernateLinqMethods.Query{T} 를 사용하세요.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IQueryable<T> GetQuery<T>() {
            return UnitOfWork.CurrentSession.Query<T>();
        }

        /// <summary>
        /// LINQ용 IQueryable{T}를 제공합니다. NHibernate.Linq.NHibernateLinqMethods.Query{T} 를 사용하세요.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="session"></param>
        /// <returns></returns>
        public static IQueryable<T> GetQuery<T>(this ISession session) {
            session.ShouldNotBeNull("session");
            return session.Query<T>();
        }

        /// <summary>
        /// Dynamic LINQ Lambda Expression으로 Between을 표현하는 식을 만듭니다.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private static string IsBetweenExpression(string propertyName) {
            var expr = string.Format(" ((@0==null || {0} >= @0) && (@1==null || {0} <= @1)) ", propertyName);

            if(IsDebugEnabled)
                log.Debug("IsBetween Expression=[{0}]", expr);

            return expr;
        }

        /// <summary>
        /// Lambda expression용 문자열로 IsInRange를 표현합니다. <see cref="NSoft.NFramework.LinqEx.DynamicQueryable"/>를 이용하여, Lambda Expression으로 변환합니다.
        /// </summary>
        /// <param name="loPropertyName"></param>
        /// <param name="hiPropertyName"></param>
        /// <returns></returns>
        private static string IsInRangeExpression(string loPropertyName, string hiPropertyName) {
            var rangeExpr = string.Format(" (({0} == null || @0 >= {0}) && ({1} == null || @0 <= {1}))", loPropertyName, hiPropertyName);

            if(IsDebugEnabled)
                log.Debug("IsInRangeExpression=[{0}]", rangeExpr);

            return rangeExpr;
        }

        /// <summary>
        /// 지정된 속성이 지정된 값과 같거나, 속성 값이 NULL인 경우 (예: Name=:Name OR Name IS NULL)
        /// </summary>
        public static IQueryable<T> AddEqIncludeNull<T>(this IQueryable<T> query, Expression<Func<T, object>> expr, object value) {
            query.ShouldNotBeNull("query");
            expr.ShouldNotBeNull("expr");

            var proeprtyName = ExpressionProcessor.FindMemberExpression(expr.Body);

            var exprString = string.Format("{0}==@0 || {0} == null", proeprtyName);
            return DynamicQueryable.Where(query, exprString, value);
        }

        /// <summary>
        /// 값이 null 이라면 "속성 IS NULL" 을, 값이 있다면, "속성 = value" 라는 질의를 추가합니다. 
        /// (예: value가 'RealWeb'인 경우 Company='RealWeb', value가 null인 경우 Company IS NULL)
        /// </summary>
        public static IQueryable<T> AddEqOrNull<T>(this IQueryable<T> query, Expression<Func<T, object>> expr, object value) {
            query.ShouldNotBeNull("query");
            expr.ShouldNotBeNull("expr");

            var propertyName = ExpressionProcessor.FindMemberExpression(expr.Body);

            var exprString = (value != null) ? propertyName + " == @0" : propertyName + " == null";
            return DynamicQueryable.Where(query, exprString, value);
        }

        /// <summary>
        /// LINQ에 Like 검색을 위한 표현식을 추가합니다.
        /// </summary>
        public static IQueryable<T> AddLike<T>(this IQueryable<T> query, Expression<Func<T, string>> expr, string value) {
            return AddLike(query, expr, value, MatchMode.Anywhere);
        }

        /// <summary>
        /// LINQ에 Like 검색을 위한 표현식을 추가합니다.
        /// </summary>
        public static IQueryable<T> AddLike<T>(this IQueryable<T> query, Expression<Func<T, string>> expr, string value,
                                               MatchMode matchMode) {
            query.ShouldNotBeNull("query");
            expr.ShouldNotBeNull("expr");

            if(value.IsEmpty())
                return query;

            matchMode = matchMode ?? MatchMode.Anywhere;

            var propertyName = ExpressionProcessor.FindMemberExpression(expr.Body);

            var exprString = string.Empty;

            if(matchMode == MatchMode.Anywhere)
                exprString = string.Format("{0}.Contains(@0)", propertyName);
            else if(matchMode == MatchMode.Start)
                exprString = string.Format("{0}.StartsWith(@0)", propertyName);
            else if(matchMode == MatchMode.End)
                exprString = string.Format("{0}.EndsWith(@0)", propertyName);
            else if(matchMode == MatchMode.Exact)
                exprString = string.Format("{0}==@0", propertyName);

            if(exprString.IsNotWhiteSpace())
                return DynamicQueryable.Where(query, exprString, value);

            return query;
        }

        /// <summary>
        /// LINQ에 Insensitive Like 검색을 위한 표현식을 추가합니다.
        /// </summary>
        public static IQueryable<T> AddInsensitiveLike<T>(this IQueryable<T> query, Expression<Func<T, string>> expr, string value) {
            return AddInsensitiveLike(query, expr, value, MatchMode.Anywhere);
        }

        /// <summary>
        /// LINQ에 Insensitive Like 검색을 위한 표현식을 추가합니다.
        /// </summary>
        public static IQueryable<T> AddInsensitiveLike<T>(this IQueryable<T> query, Expression<Func<T, string>> expr, string value,
                                                          MatchMode matchMode) {
            query.ShouldNotBeNull("query");
            expr.ShouldNotBeNull("expr");

            if(value.IsEmpty())
                return query;

            matchMode = matchMode ?? MatchMode.Anywhere;

            var propertyName = ExpressionProcessor.FindMemberExpression(expr.Body);

            var exprString = string.Empty;

            if(matchMode == MatchMode.Anywhere)
                exprString = string.Format("{0}.ToLower().Contains(@0)", propertyName);
            else if(matchMode == MatchMode.Start)
                exprString = string.Format("{0}.ToLower().StartsWith(@0)", propertyName);
            else if(matchMode == MatchMode.End)
                exprString = string.Format("{0}.ToLower().EndsWith(@0)", propertyName);
            else if(matchMode == MatchMode.Exact)
                exprString = string.Format("{0}.ToLower() == @0", propertyName);

            if(exprString.IsNotWhiteSpace())
                return DynamicQueryable.Where(query, exprString, value.AsText().ToLower());

            return query;
        }

        /// <summary>
        /// IQueryable{T}에 Between 조건을 추가합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="expr"></param>
        /// <param name="lo"></param>
        /// <param name="hi"></param>
        /// <returns></returns>
        public static IQueryable<T> AddBetween<T>(this IQueryable<T> query, Expression<Func<T, object>> expr, object lo, object hi) {
            query.ShouldNotBeNull("query");

            var propertyName = ExpressionProcessor.FindMemberExpression(expr.Body);
            var betweenExpr = IsBetweenExpression(propertyName);

            return DynamicQueryable.Where(query, betweenExpr, lo, hi);
        }

        /// <summary>
        /// IQueryable{T}에 InRange 조건을 추가합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="value"></param>
        /// <param name="loExpr"></param>
        /// <param name="hiExpr"></param>
        /// <returns></returns>
        public static IQueryable<T> AddInRange<T>(this IQueryable<T> query, object value, Expression<Func<T, object>> loExpr,
                                                  Expression<Func<T, object>> hiExpr) {
            query.ShouldNotBeNull("query");

            var loPropertyName = ExpressionProcessor.FindMemberExpression(loExpr.Body);
            var hiPropertyName = ExpressionProcessor.FindMemberExpression(hiExpr.Body);
            var isInRangeExpr = IsInRangeExpression(loPropertyName, hiPropertyName);

            return DynamicQueryable.Where(query, isInRangeExpr, value);
        }

        /// <summary>
        /// IQueryable{T}에 IsOverlap 조건을 추가합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="period"></param>
        /// <param name="loExpr"></param>
        /// <param name="hiExpr"></param>
        /// <returns></returns>
        public static IQueryable<T> AddIsOverlap<T>(this IQueryable<T> query, ITimePeriod period, Expression<Func<T, object>> loExpr,
                                                    Expression<Func<T, object>> hiExpr) {
            period.ShouldNotBeNull("period");

            if(period.IsAnytime)
                return query;

            loExpr.ShouldNotBeNull("loExpr");
            hiExpr.ShouldNotBeNull("hiExpr");

            var loPropertyName = ExpressionProcessor.FindMemberExpression(loExpr.Body);
            var hiPropertyName = ExpressionProcessor.FindMemberExpression(hiExpr.Body);

            var exprBuilder = new StringBuilder();

            if(period.HasStart && period.HasEnd) {
                exprBuilder
                    .Append(IsInRangeExpression(loPropertyName, hiPropertyName)).Append(" || ")
                    .Append(IsInRangeExpression(loPropertyName, hiPropertyName).Replace("@0", "@1")).Append(" || ")
                    .Append(IsBetweenExpression(loPropertyName)).Append(" || ")
                    .Append(IsBetweenExpression(hiPropertyName).Replace("@0", "@1"));

                var expr = exprBuilder.ToString();

                if(IsDebugEnabled)
                    log.Debug("Overlap Expression=[{0}]", expr);

                return DynamicQueryable.Where(query, expr, period.Start, period.End);
            }
            if(period.HasStart) {
                exprBuilder
                    .Append(IsInRangeExpression(loPropertyName, hiPropertyName)).Append(" || ")
                    .Append(loPropertyName + " >= @0").Append(" || ")
                    .Append(hiPropertyName + " >= @0");

                var expr = exprBuilder.ToString();

                if(IsDebugEnabled)
                    log.Debug("Overlap Expression=[{0}]", expr);

                return DynamicQueryable.Where(query, expr, period.Start);
            }
            if(period.HasEnd) {
                exprBuilder
                    .Append(IsInRangeExpression(loPropertyName, hiPropertyName)).Append(" || ")
                    .Append(loPropertyName + " <= @0").Append(" || ")
                    .Append(hiPropertyName + " <= @0");

                var expr = exprBuilder.ToString();

                if(IsDebugEnabled)
                    log.Debug("Overlap Expression=[{0}]", expr);

                return DynamicQueryable.Where(query, expr, period.End);
            }

            throw new InvalidOperationException("기간이 Overlap되는지 판단하는 Criterion을 생성하기 위한 조건이 맞지 않습니다!!!");
        }

        /// <summary>
        /// 속성 < <paramref name="current"/> 인 질의를 추가합니다. (값이  <paramref name="current"/>보다 작다면, 이미 지나간 시간이라는 뜻)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="current"></param>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static IQueryable<T> AddIsElapsed<T>(this IQueryable<T> query, DateTime current, Expression<Func<T, object>> expr) {
            query.ShouldNotBeNull("query");
            expr.ShouldNotBeNull("expr");

            var propertyName = ExpressionProcessor.FindMemberExpression(expr.Body);
            return DynamicQueryable.Where(query, propertyName + " < @0", current);
        }

        /// <summary>
        /// 지정한 속성 값이 NULL이면 False로 간주하는 Where 절을 추가한다.
        /// Explicit 하게 PropertyName = True 로 되어 있는 것을 제외한 False이거나 NULL 것은 False 로 간주한다.
        /// </summary>
        public static IQueryable<T> AddNullAsFalse<T>(this IQueryable<T> query, Expression<Func<T, object>> expr, bool? value) {
            var propertyName = ExpressionProcessor.FindMemberExpression(expr.Body);

            if(value.GetValueOrDefault(false))
                return DynamicQueryable.Where(query, propertyName + " == @0", true);

            return AddEqIncludeNull(query, expr, false);
        }

        /// <summary>
        /// 지정한 속성 값이 NULL이면 True로 간주하는 Where 절을 추가한다.
        /// Explicit 하게 PropertyName = False 로 되어 있는 것을 제외한 True이거나 NULL 것은 True 로 간주한다.
        /// </summary>
        public static IQueryable<T> AddNullAsTrue<T>(this IQueryable<T> query, Expression<Func<T, object>> expr, bool? value) {
            var propertyName = ExpressionProcessor.FindMemberExpression(expr.Body);

            if(value.GetValueOrDefault(true) == false)
                return DynamicQueryable.Where(query, propertyName + " == @0", false);

            return AddEqIncludeNull(query, expr, true);
        }

        public static IQueryable<T> AddWhere<T>(this IQueryable<T> query, string expression, params object[] values) {
            return DynamicQueryable.Where(query, expression, values);
        }

        public static IQueryable<T> AddWhere<T>(this IQueryable<T> query, Expression<Func<T, bool>> predicate) {
            predicate.ShouldNotBeNull("predicate");
            query = query.Where(predicate);
            return query;
        }

        public static IQueryable<T> AddOrderBy<T, TKey>(this IQueryable<T> query, Expression<Func<T, TKey>> keySelector) {
            query = query.OrderBy(keySelector);
            return query;
        }

        public static IQueryable<T> AddOrderBy<T>(this IQueryable<T> query, string ordering, params object[] values) {
            query = query.OrderBy(ordering, values);
            return query;
        }

        public static IQueryable<T> AddOrderByDescending<T, TKey>(this IQueryable<T> query, Expression<Func<T, TKey>> keySelector) {
            query = query.OrderByDescending(keySelector);
            return query;
        }

        public static IQueryable<T> AddOrders<T>(this IQueryable<T> query, params INHOrder<T>[] orders) {
            foreach(var order in orders)
                query = query.AddOrderBy(order.ToString());

            return query;
        }
    }
}