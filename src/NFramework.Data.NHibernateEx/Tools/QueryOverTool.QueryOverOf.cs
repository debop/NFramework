using System;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Criterion;
using NHibernate.Criterion.Lambda;
using NSoft.NFramework.Data.NHibernateEx.Domain;
using NSoft.NFramework.TimePeriods;

namespace NSoft.NFramework.Data.NHibernateEx {
    /// <summary>
    /// NHibernate 3.0의 QueryOver{T}, QueryOver.Of{T}를 위한 확장 메소드를 제공합니다.
    /// </summary>
    public static partial class QueryOverTool {
        /// <summary>
        /// 지정된 속성이 지정된 값과 같거나, 속성 값이 NULL인 경우 (예: Name=:Name OR Name IS NULL)
        /// </summary>
        public static QueryOver<TRoot, TSub> AddEqIncludeNull<TRoot, TSub>(this QueryOver<TRoot, TSub> queryOver,
                                                                           Expression<Func<TSub, object>> expr,
                                                                           object value) {
            var propertyName = RetrievePropertyName(expr);

            queryOver.UnderlyingCriteria.Add(Restrictions.Disjunction()
                                                 .Add(Restrictions.Eq(propertyName, value))
                                                 .Add(Restrictions.IsNull(propertyName)));
            return queryOver;
        }

        /// <summary>
        /// 값이 null 이라면 "속성 IS NULL" 을, 값이 있다면, "속성 = value" 라는 질의를 추가합니다. 
        /// (예: value가 'RealWeb'인 경우 Company='RealWeb', value가 null인 경우 Company IS NULL)
        /// </summary>
        public static QueryOver<TRoot, TSub> AddEqOrNull<TRoot, TSub>(this QueryOver<TRoot, TSub> queryOver,
                                                                      Expression<Func<TSub, object>> expr, object value) {
            var propertyName = RetrievePropertyName(expr);
            queryOver.UnderlyingCriteria.Add((value != null)
                                                 ? Restrictions.Eq(propertyName, value)
                                                 : Restrictions.IsNull(propertyName));
            return queryOver;
        }

        /// <summary>
        /// 지정된 속성에 대한 LIKE 검색을 수행하도록 합니다.
        /// </summary>
        public static QueryOver<TRoot, TSub> AddLike<TRoot, TSub>(this QueryOver<TRoot, TSub> queryOver,
                                                                  Expression<Func<TSub, object>> expr, string value) {
            return AddLike(queryOver, expr, value, null);
        }

        /// <summary>
        /// 지정된 속성에 대한 LIKE 검색을 수행하도록 합니다.
        /// </summary>
        public static QueryOver<TRoot, TSub> AddLike<TRoot, TSub>(this QueryOver<TRoot, TSub> queryOver,
                                                                  Expression<Func<TSub, object>> expr, string value,
                                                                  MatchMode matchMode) {
            return queryOver.Where(Restrictions.On(expr).IsLike(value, matchMode ?? MatchMode.Anywhere));
        }

        public static QueryOver<TRoot, TSub> AddInsensitiveLike<TRoot, TSub>(this QueryOver<TRoot, TSub> queryOver,
                                                                             Expression<Func<TSub, object>> expr,
                                                                             string value) {
            return AddInsensitiveLike(queryOver, expr, value, null);
        }

        public static QueryOver<TRoot, TSub> AddInsensitiveLike<TRoot, TSub>(this QueryOver<TRoot, TSub> queryOver,
                                                                             Expression<Func<TSub, object>> expr,
                                                                             string value,
                                                                             MatchMode matchMode) {
            return queryOver.Where(Restrictions.On(expr).IsInsensitiveLike(value, matchMode ?? MatchMode.Anywhere));
        }

        /// <summary>
        /// <see cref="ITreeNodeEntity{T}"/>를 구현한 노드의 자식 노드들이 있는 경우를 조회하는 조건을 추가합니다.
        /// </summary>
        public static QueryOver<TRoot, TSub> AddHasChild<TRoot, TSub>(this QueryOver<TRoot, TSub> queryOver)
            where TSub : ITreeNodeEntity<TSub> {
            return queryOver.Where(Restrictions.On<TSub>(node => node.Children).IsNotEmpty);
        }

        /// <summary>
        /// <see cref="ITreeNodeEntity{T}"/>를 구현한 노드의 자식 노드들이 없는 경우를 조회하는 조건을 추가합니다.
        /// </summary>
        public static QueryOver<TRoot, TSub> AddHasNotChild<TRoot, TSub>(this QueryOver<TRoot, TSub> queryOver)
            where TSub : ITreeNodeEntity<TSub> {
            return queryOver.Where(Restrictions.On<TSub>(node => node.Children).IsEmpty);
        }

        /// <summary>
        /// QueryOver{TRoot}의 Where절에 Between 구문을 수행합니다.
        /// </summary>
        public static QueryOver<TRoot, TSub> AddBetween<TRoot, TSub>(this QueryOver<TRoot, TSub> queryOver,
                                                                     Expression<Func<TSub, object>> expr, object lo,
                                                                     object hi) {
            queryOver.UnderlyingCriteria.Add(IsBetweenCriterion(expr, lo, hi));
            return queryOver;
        }

        /// <summary>
        /// QueryOver{TRoot}의 Where절에 Not Between 구문을 수행합니다.
        /// </summary>
        public static QueryOver<TRoot, TSub> AddNotBetween<TRoot, TSub>(this QueryOver<TRoot, TSub> queryOver,
                                                                        Expression<Func<TSub, object>> expr, object lo,
                                                                        object hi) {
            queryOver.UnderlyingCriteria.AddNot(IsBetweenCriterion(expr, lo, hi));
            return queryOver;
        }

        /// <summary>
        /// 지정된 Expression의 속성에 대해 IN () 검색 수행
        /// </summary>
        public static QueryOver<TRoot, TSub> AddIn<TRoot, TSub>(this QueryOver<TRoot, TSub> queryOver,
                                                                Expression<Func<TSub, object>> expr,
                                                                params object[] values) {
            return queryOver.WhereRestrictionOn(expr).IsIn(values.ToArray());
        }

        /// <summary>
        /// 지정된 Expression의 속성에 대해 NOT IN () 검색 수행
        /// </summary>
        public static QueryOver<TRoot, TSub> AddNotIn<TRoot, TSub>(this QueryOver<TRoot, TSub> queryOver,
                                                                   Expression<Func<TSub, object>> expr,
                                                                   params object[] values) {
            return queryOver.WhereRestrictionOn(expr).Not.IsIn(values.ToArray());
        }

        /// <summary>
        /// 지정된 Expression의 속성에 대해 IN () 검색 수행
        /// </summary>
        public static QueryOver<TRoot, TSub> AddInG<TRoot, TSub, TValue>(this QueryOver<TRoot, TSub> queryOver,
                                                                         Expression<Func<TSub, object>> expr,
                                                                         params TValue[] values) {
            return queryOver.WhereRestrictionOn(expr).IsInG<TValue>(values.ToArray());
        }

        /// <summary>
        /// 지정된 Expression의 속성에 대해 NOT IN () 검색 수행
        /// </summary>
        public static QueryOver<TRoot, TSub> AddNotInG<TRoot, TSub, TValue>(this QueryOver<TRoot, TSub> queryOver,
                                                                            Expression<Func<TSub, object>> expr,
                                                                            params TValue[] values) {
            return queryOver.WhereRestrictionOn(expr).Not.IsInG<TValue>(values.ToArray());
        }

        /// <summary>
        /// <paramref name="value"/>가 상하한 구간을 나타내는 표현식의 값의 내부 영역에 있는지 검사하는 질의어를 추가합니다.
        /// </summary>
        /// <typeparam name="TRoot"></typeparam>
        /// <typeparam name="TSub"></typeparam>
        /// <param name="queryOver"></param>
        /// <param name="value"></param>
        /// <param name="loExpr"></param>
        /// <param name="hiExpr"></param>
        /// <returns></returns>
        public static QueryOver<TRoot, TSub> AddInRange<TRoot, TSub>(this QueryOver<TRoot, TSub> queryOver,
                                                                     object value,
                                                                     Expression<Func<TSub, object>> loExpr,
                                                                     Expression<Func<TSub, object>> hiExpr) {
            queryOver.UnderlyingCriteria.Add(IsInRangeCriterion(value, loExpr, hiExpr));
            return queryOver;
        }

        /// <summary>
        /// <paramref name="period"/>의 구간이 엔티티의 상하한 구간 (<paramref name="loExpr"/> ~ <paramref name="hiExpr"/> )과 겹치는지 검사하는 질의어를 추가합니다.
        /// </summary>
        /// <typeparam name="TRoot"></typeparam>
        /// <typeparam name="TSub"></typeparam>
        /// <param name="queryOver">QueryOver 인스턴스</param>
        /// <param name="period">검색 기간</param>
        /// <param name="loExpr">하한 값 표현식</param>
        /// <param name="hiExpr">상한 값 표현식</param>
        /// <returns></returns>
        public static QueryOver<TRoot, TSub> AddIsOverlap<TRoot, TSub>(
            this QueryOver<TRoot, TSub> queryOver,
            ITimePeriod period,
            Expression<Func<TSub, object>> loExpr,
            Expression<Func<TSub, object>> hiExpr) {
            queryOver.ShouldNotBeNull("queryOver");

            queryOver.UnderlyingCriteria.Add(IsOverlapCriterion(period, loExpr, hiExpr));
            return queryOver;
        }

        /// <summary>
        /// 속성 &lt; <paramref name="current"/> 인 질의를 추가합니다. (값이  <paramref name="current"/>보다 작다면, 이미 지나간 시간이라는 뜻)
        /// </summary>
        public static QueryOver<TRoot, TSub> AddIsElapsed<TRoot, TSub>(this QueryOver<TRoot, TSub> queryOver,
                                                                       DateTime current,
                                                                       Expression<Func<TSub, object>> expr) {
            var propertyName = RetrievePropertyName(expr);
            queryOver.UnderlyingCriteria.AddLt(propertyName, current);
            return queryOver;
        }

        /// <summary>
        /// 지정한 속성 값이 NULL이면 False로 간주하는 Where 절을 추가한다.
        /// Explicit 하게 PropertyName = True 로 되어 있는 것을 제외한 False이거나 NULL 것은 False 로 간주한다.
        /// </summary>
        public static QueryOver<TRoot, TSub> AddNullAsFalse<TRoot, TSub>(this QueryOver<TRoot, TSub> queryOver,
                                                                         Expression<Func<TSub, object>> expr,
                                                                         bool? value) {
            var propertyName = RetrievePropertyName(expr);

            queryOver.UnderlyingCriteria.AddNullAsFalse(propertyName, value);

            return queryOver;
        }

        /// <summary>
        /// 지정한 속성 값이 NULL이면 True로 간주하는 Where 절을 추가한다. 
        /// PropertyName 를 조회할 때, 명확히 PropertyName=False가 아니라 NULL이거나, True라면 True로 간주한다.
        /// </summary>
        public static QueryOver<TRoot, TSub> AddNullAsTrue<TRoot, TSub>(this QueryOver<TRoot, TSub> queryOver,
                                                                        Expression<Func<TSub, object>> expr, bool? value) {
            var propertyName = RetrievePropertyName(expr);

            queryOver.UnderlyingCriteria.AddNullAsTrue(propertyName, value);
            return queryOver;
        }

        public static QueryOver<T, T> AddWhere<T>(this QueryOver<T, T> queryOver, Expression<Func<bool>> expr) {
            return queryOver.Where(expr);
        }

        public static QueryOver<TRoot, TSub> AddWhere<TRoot, TSub>(this QueryOver<TRoot, TSub> queryOver,
                                                                   Expression<Func<TSub, bool>> expr) {
            return queryOver.Where(expr);
        }

        public static QueryOver<T, T> AddWhereNot<T>(this QueryOver<T, T> queryOver, Expression<Func<bool>> expr) {
            return queryOver.WhereNot(expr);
        }

        public static QueryOver<TRoot, TSub> AddWhereNot<TRoot, TSub>(this QueryOver<TRoot, TSub> queryOver,
                                                                      Expression<Func<TSub, bool>> expr) {
            return queryOver.WhereNot(expr);
        }

        public static QueryOverRestrictionBuilder<T, T> AddWhereRestrictionOn<T>(this QueryOver<T, T> queryOver,
                                                                                 Expression<Func<object>> expr) {
            return queryOver.WhereRestrictionOn(expr);
        }

        public static QueryOverRestrictionBuilder<TRoot, TSub> AddWhereRestrictionOn<TRoot, TSub>(
            this QueryOver<TRoot, TSub> queryOver,
            Expression<Func<TSub, object>> expr) {
            return queryOver.WhereRestrictionOn(expr);
        }

        public static QueryOver<T> AddSkip<T>(this QueryOver<T> queryOver, int? skip) {
            if(skip.HasValue && skip.Value > 0)
                return queryOver.Skip(skip.Value);

            return queryOver;
        }

        public static QueryOver<T> AddTake<T>(this QueryOver<T> queryOver, int? take) {
            if(take.HasValue && take.Value > 0)
                return queryOver.Take(take.Value);

            return queryOver;
        }

        public static QueryOver<T, T> AddOrderBy<T>(this QueryOver<T, T> queryOver, Expression<Func<object>> expr) {
            return queryOver.OrderBy(expr).Asc;
        }

        public static QueryOver<TRoot, TSub> AddOrderBy<TRoot, TSub>(this QueryOver<TRoot, TSub> queryOver,
                                                                     Expression<Func<TSub, object>> expr) {
            return queryOver.OrderBy(expr).Asc;
        }

        public static QueryOver<T, T> AddOrderBy<T>(this QueryOver<T, T> queryOver, IProjection projection) {
            return queryOver.OrderBy(projection).Asc;
        }

        public static QueryOver<T, T> AddOrderByDesc<T>(this QueryOver<T, T> queryOver, Expression<Func<object>> expr) {
            return queryOver.OrderBy(expr).Desc;
        }

        public static QueryOver<TRoot, TSub> AddOrderByDesc<TRoot, TSub>(this QueryOver<TRoot, TSub> queryOver,
                                                                         Expression<Func<TSub, object>> expr) {
            return queryOver.OrderBy(expr).Desc;
        }

        public static QueryOver<T, T> AddOrderByDesc<T>(this QueryOver<T, T> queryOver, IProjection projection) {
            return queryOver.OrderBy(projection).Desc;
        }

        public static QueryOver<T, T> AddOrderByAlias<T>(this QueryOver<T, T> queryOver, Expression<Func<object>> expr) {
            return queryOver.OrderByAlias(expr).Asc;
        }

        public static QueryOver<T, T> AddOrderByAliasDesc<T>(this QueryOver<T, T> queryOver,
                                                             Expression<Func<object>> expr) {
            return queryOver.OrderByAlias(expr).Desc;
        }

        public static QueryOver<T, T> AddOrders<T>(this QueryOver<T, T> queryOver, params INHOrder[] orders) {
            foreach(var order in orders) {
                queryOver = order.Ascending
                                ? queryOver.OrderBy(order.OrderExpr).Asc
                                : queryOver.OrderBy(order.OrderExpr).Desc;
            }
            return queryOver;
        }

        public static QueryOver<TRoot, TSub> AddOrders<TRoot, TSub>(this QueryOver<TRoot, TSub> queryOver,
                                                                    params INHOrder<TSub>[] orders) {
            if(orders != null)
                foreach(var order in orders) {
                    queryOver = order.Ascending
                                    ? queryOver.OrderBy(order.OrderExpr).Asc
                                    : queryOver.OrderBy(order.OrderExpr).Desc;
                }
            return queryOver;
        }

        public static QueryOver<T> AddOrders<T>(this QueryOver<T> queryOver, params INHOrder<T>[] orders) {
            var query = (QueryOver<T, T>)queryOver;

            if(orders != null)
                foreach(var order in orders) {
                    query = order.Ascending
                                ? query.OrderBy(order.OrderExpr).Asc
                                : query.OrderBy(order.OrderExpr).Desc;
                }

            return queryOver;
        }
    }
}