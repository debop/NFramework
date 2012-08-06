using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using NSoft.NFramework.TimePeriods;

namespace NSoft.NFramework.Data.NHibernateEx {
    public static partial class CriteriaTool {
        /// <summary>
        /// 속성 = value 인 질의를 추가합니다.
        /// </summary>
        public static ICriteria AddEq(this ICriteria criteria, string propertyName, object value) {
            return criteria.Add(Restrictions.Eq(propertyName, value));
        }

        /// <summary>
        /// 속성 != value 인 질의를 추가합니다.
        /// </summary>
        public static ICriteria AddNotEq(this ICriteria criteria, string propertyName, object value) {
            return criteria.Add(Restrictions.Not(Restrictions.Eq(propertyName, value)));
        }

        /// <summary>
        /// 속성 &lt;= value 인 질의를 추가합니다.
        /// </summary>
        public static ICriteria AddLe(this ICriteria criteria, string propertyName, object value) {
            return criteria.Add(Restrictions.Le(propertyName, value));
        }

        /// <summary>
        /// 속성 &lt; value 인 질의를 추가합니다.
        /// </summary>
        public static ICriteria AddLt(this ICriteria criteria, string propertyName, object value) {
            return criteria.Add(Restrictions.Lt(propertyName, value));
        }

        /// <summary>
        /// 속성 &gt;= value 인 질의를 추가합니다.
        /// </summary>
        public static ICriteria AddGe(this ICriteria criteria, string propertyName, object value) {
            return criteria.Add(Restrictions.Ge(propertyName, value));
        }

        /// <summary>
        /// 속성 &gt; value 인 질의를 추가합니다.
        /// </summary>
        public static ICriteria AddGt(this ICriteria criteria, string propertyName, object value) {
            return criteria.Add(Restrictions.Lt(propertyName, value));
        }

        /// <summary>
        /// 지정된 속성-값을 나타내는 사전이 모두 같다는 질의를 추가합니다.
        /// </summary>
        public static ICriteria AddAllEq(this ICriteria criteria, IDictionary propertyNameValues) {
            propertyNameValues.ShouldNotBeNull("propertyNameValues");
            return criteria.Add(Restrictions.AllEq(propertyNameValues));
        }

        /// <summary>
        /// 속성이 비었음을 나타내는 질의를 추가합니다.
        /// </summary>
        public static ICriteria AddIsEmpty(this ICriteria criteria, string propertyName) {
            return criteria.Add(Restrictions.IsEmpty(propertyName));
        }

        /// <summary>
        /// 속성이 비어있지 않음을 나타내는 질의를 추가합니다.
        /// </summary>
        public static ICriteria AddIsNotEmpty(this ICriteria criteria, string propertyName) {
            return criteria.Add(Restrictions.IsNotEmpty(propertyName));
        }

        /// <summary>
        /// 속성 IS NULL 임을 나타내는 질의를 추가합니다.
        /// </summary>
        public static ICriteria AddIsNull(this ICriteria criteria, string propertyName) {
            return criteria.Add(Restrictions.IsNull(propertyName));
        }

        /// <summary>
        /// 속성 IS NOT NULL 임을 나타내는 질의를 추가합니다.
        /// </summary>
        public static ICriteria AddIsNotNull(this ICriteria criteria, string propertyName) {
            return criteria.Add(Restrictions.IsNotNull(propertyName));
        }

        /// <summary>
        /// 지정된 속성이 지정된 값과 같거나, 속성 값이 NULL인 경우 (예: Name=:Name OR Name IS NULL)
        /// </summary>
        public static ICriteria AddEqIncludeNull(this ICriteria criteria, string propertyName, object value) {
            return criteria.Add(Restrictions.Disjunction()
                                    .Add(Restrictions.Eq(propertyName, value))
                                    .Add(Restrictions.IsNull(propertyName)));
            // return criteria.Add(NHRestrictions.EqIncludeNull(propertyName, value));
        }

        /// <summary>
        /// 값이 null 이라면 "속성 IS NULL" 을, 값이 있다면, "속성 = value" 라는 질의를 추가합니다. 
        /// (예: value가 'RealWeb'인 경우 Company='RealWeb', value가 null인 경우 Company IS NULL)
        /// </summary>
        public static ICriteria AddEqOrNull(this ICriteria criteria, string propertyName, object value) {
            return criteria.Add((value != null)
                                    ? Restrictions.Eq(propertyName, value)
                                    : Restrictions.IsNull(propertyName));
            // return criteria.Add(NHRestrictions.EqOrNull(propertyName, value));
        }

        /// <summary>
        /// LIKE 검색 질의를 추가합니다.
        /// </summary>
        public static ICriteria AddLike(this ICriteria criteria, string propertyName, string value) {
            return AddLike(criteria, propertyName, value, null);
        }

        /// <summary>
        /// LIKE 검색 질의를 추가합니다.
        /// </summary>
        public static ICriteria AddLike(this ICriteria criteria, string propertyName, string value, MatchMode matchMode) {
            return criteria.Add(Restrictions.Like(propertyName, value, matchMode ?? MatchMode.Anywhere));
        }

        /// <summary>
        /// 대소문자 구분없는 LIKE 검색 질의를 추가합니다. (MS SQL에서는 기본적으로 대소문자 구분이 없으므로 AddLike와 같습니다.)
        /// </summary>
        public static ICriteria AddInsensitiveLike(this ICriteria criteria, string propertyName, string value) {
            return AddInsensitiveLike(criteria, propertyName, value, null);
        }

        /// <summary>
        /// 대소문자 구분없는 LIKE 검색 질의를 추가합니다. (MS SQL에서는 기본적으로 대소문자 구분이 없으므로 AddLike와 같습니다.)
        /// </summary>
        public static ICriteria AddInsensitiveLike(this ICriteria criteria, string propertyName, string value, MatchMode matchMode) {
            return criteria.Add(Restrictions.InsensitiveLike(propertyName, value, matchMode ?? MatchMode.Anywhere));
        }

        /// <summary>
        /// Id == value 인 질의를 추가합니다. 
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ICriteria AddIdEq(this ICriteria criteria, object value) {
            return criteria.Add(Restrictions.IdEq(value));
        }

        /// <summary>
        /// 속성 IN (v1, v2, v3 ... vN) 처럼 IN Operation이 하는 질의를 추가합니다.
        /// </summary>
        public static ICriteria AddIn(this ICriteria criteria, string propertyName, ICollection values) {
            return criteria.Add(Restrictions.In(propertyName, values));
        }

        /// <summary>
        /// 속성 IN (v1, v2, v3 ... vN) 처럼 IN Operation이 하는 질의를 추가합니다.
        /// </summary>
        public static ICriteria AddInG<T>(this ICriteria criteria, string propertyName, ICollection<T> values) {
            return criteria.Add(Restrictions.InG(propertyName, values));
        }

        /// <summary>
        /// lo &lt;= 속성 &lt;= hi 인 질어어를 추가합니다. (lo, hi 둘 중 적어도 하나 이상이 null이 아니어야 합니다.)
        /// </summary>
        public static ICriteria AddBetween(this ICriteria criteria, string propertyName, object lo, object hi) {
            return criteria.Add(IsBetweenCriterion(propertyName, lo, hi));
        }

        /// <summary>
        /// loProperty &lt;= value &lt;= hiProperty 인 질의를 추가합니다. (Between과 반대의 개념입니다.)
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="value"></param>
        /// <param name="loPropertyName"></param>
        /// <param name="hiPropertyName"></param>
        /// <returns></returns>
        public static ICriteria AddInRange(this ICriteria criteria, object value, string loPropertyName, string hiPropertyName) {
            return criteria.Add(IsInRangeCriterion(value, loPropertyName, hiPropertyName));
        }

        /// <summary>
        /// <paramref name="period"/> 기간이 
        /// <paramref name="loPropertyName"/> ~ <paramref name="hiPropertyName"/> 기간과 겹치는지 조사하는 질의를 추가합니다.
        /// </summary>
        public static ICriteria AddIsOverlap(this ICriteria criteria,
                                             ITimePeriod period,
                                             string loPropertyName,
                                             string hiPropertyName) {
            return criteria.Add(IsOverlapCriterion(period, loPropertyName, hiPropertyName));
        }

        /// <summary>
        /// 속성 &lt; <paramref name="current"/> 인 질의를 추가합니다. (값이  <paramref name="current"/>보다 작다면, 이미 지나간 시간이라는 뜻)
        /// </summary>
        public static ICriteria AddIsElapsed(this ICriteria criteria, DateTime current, string propertyName) {
            return criteria.Add(Restrictions.Lt(propertyName, current));
        }

        /// <summary>
        /// 지정한 속성 값이 NULL이면 False로 간주하는 Where 절을 추가한다.
        /// Explicit 하게 PropertyName = True 로 되어 있는 것을 제외한 False이거나 NULL 것은 False 로 간주한다.
        /// </summary>
        public static ICriteria AddNullAsFalse(this ICriteria criteria, string propertyName, bool? value) {
            if(value.GetValueOrDefault(false))
                return criteria.AddEq(propertyName, true);

            return criteria.AddEqIncludeNull(propertyName, false);
        }

        /// <summary>
        /// 지정한 속성 값이 NULL이면 True로 간주하는 Where 절을 추가한다. 
        /// PropertyName 를 조회할 때, 명확히 PropertyName=False가 아니라 NULL이거나, True라면 True로 간주한다.
        /// </summary>
        public static ICriteria AddNullAsTrue(this ICriteria criteria, string propertyName, bool? value) {
            if(value.GetValueOrDefault(true) == false)
                return criteria.AddEq(propertyName, false);

            return criteria.AddEqIncludeNull(propertyName, true);
        }

        /// <summary>
        /// 지정된 Criteria에 대해 NOT을 수행합니다.
        /// </summary>
        public static ICriteria AddNot(this ICriteria criteria, ICriterion criterion) {
            return criteria.Add(Restrictions.Not(criterion));
        }
    }
}