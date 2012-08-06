using System;
using System.Linq.Expressions;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Engine;
using NHibernate.Impl;
using NSoft.NFramework.TimePeriods;

namespace NSoft.NFramework.Data.NHibernateEx {
    /// <summary>
    /// NHibernate 3.0의 QueryOver{T}, QueryOver.Of{T}를 위한 확장 메소드를 제공합니다.
    /// </summary>
    public static partial class QueryOverTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// <paramref name="queryOver"/>가 사용하는 <see cref="ISessionImplementor"/>을 반환합니다.
        /// </summary>
        /// <param name="queryOver"></param>
        /// <returns></returns>
        public static ISessionImplementor GetSession(this IQueryOver queryOver) {
            queryOver.ShouldNotBeNull("queryOver");
            return queryOver.GetRootCriteria().GetSession();
        }

        public static CriteriaImpl GetRootCriteria(this IQueryOver queryOver) {
            queryOver.ShouldNotBeNull("queryOver");
            return queryOver.RootCriteria.GetRootCriteria();
        }

        public static Type GetRootType(this IQueryOver queryOver) {
            queryOver.ShouldNotBeNull("queryOver");

            return queryOver.GetRootCriteria().GetRootType();
        }

        public static Type GetRootType<T>(this IQueryOver<T, T> queryOver) {
            return queryOver.GetRootCriteria().GetRootType();
        }

        /// <summary>
        /// 람다 식에서 정의한 속성 명을 문자열로 가져옵니다. (QueryOver 의 Lambda Expr 에서 속성명을 추출해서, ICriterion 으로 변환할 수 있습니다)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string RetrievePropertyName<T>(this Expression<Func<T, object>> expression) {
            if(IsDebugEnabled)
                log.Debug("속성명을 추출할 람다식=[{0}]", expression.Body);

            var propertyName = ExpressionProcessor.FindMemberExpression(expression.Body);

            if(IsDebugEnabled)
                log.Debug("람다식에서 속성명을 추출했습니다. 람다식=[{0}], propertyName=[{1}]", expression.Body, propertyName);

            return propertyName;
        }

        /// <summary>
        /// <paramref name="expr"/>에 해당하는 속성이 상하한 값(lo,hi) 구간 안에 있는 값인지 판단하는 질의를 빌드합니다.
        /// </summary>
        /// <typeparam name="T">엔티티 수형</typeparam>
        /// <param name="expr">속성을 나타내는 람다 식</param>
        /// <param name="lo">하한 값</param>
        /// <param name="hi">상한 값</param>
        /// <returns></returns>
        public static ICriterion IsBetweenCriterion<T>(this Expression<Func<T, object>> expr, object lo, object hi) {
            expr.ShouldNotBeNull("expr");
            var propertyName = RetrievePropertyName(expr);

            return CriteriaTool.IsBetweenCriterion(propertyName, lo, hi);
        }

        /// <summary>
        /// <paramref name="value"/> 가 상하한 값 구간 안에 있는 값인지 판단하는 질의를 빌드합니다.
        /// </summary>
        /// <typeparam name="T">엔티티 수형</typeparam>
        /// <param name="value">검사할 값</param>
        /// <param name="loExpr">하한 값을 나타내는 람다 식</param>
        /// <param name="hiExpr">상한 값을 나타내는 람다 식</param>
        /// <returns></returns>
        public static ICriterion IsInRangeCriterion<T>(this object value, Expression<Func<T, object>> loExpr,
                                                       Expression<Func<T, object>> hiExpr) {
            value.ShouldNotBeNull("value");

            var loPropertyName = RetrievePropertyName(loExpr);
            var hiPropertyName = RetrievePropertyName(hiExpr);

            return CriteriaTool.IsInRangeCriterion(value, loPropertyName, hiPropertyName);
        }

        /// <summary>
        /// 주어진 기간이 오버랩되는지를 파악하는 질의어를 빌드합니다. (모든 구간은 폐쇄구간일 필요는 없고, 개방 구간이라도 상관없습니다.
        /// </summary>
        /// <typeparam name="T">엔티티 수형</typeparam>
        /// <param name="period">검사할 시간 구간</param>
        /// <param name="loExpr">하한값을 나타내는 속성</param>
        /// <param name="hiExpr">상한값을 나타내는 속성</param>
        /// <returns></returns>
        public static ICriterion IsOverlapCriterion<T>(this ITimePeriod period, Expression<Func<T, object>> loExpr,
                                                       Expression<Func<T, object>> hiExpr) {
            period.ShouldNotBeNull("period");
            Guard.Assert(period.IsAnytime == false, @"기간이 설정되어 있지 않습니다. 상하한 값 모두 없으므로, 질의어를 만들 필요가 없습니다.");

            var loPropertyName = RetrievePropertyName(loExpr);
            var hiPropertyName = RetrievePropertyName(hiExpr);

            return CriteriaTool.IsOverlapCriterion(period, loPropertyName, hiPropertyName);
        }
    }
}