using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Criterion;
using NSoft.NFramework.Data.NHibernateEx.Criterion;
using NSoft.NFramework.TimePeriods;

namespace NSoft.NFramework.Data.NHibernateEx {
    /// <summary>
    /// NHibernate Criteria 조작을 위한 Utility Class
    /// </summary>
    public static partial class CriteriaTool {
        /// <summary>
        /// Add Eq (Equal) expression to detached criteria
        /// </summary>
        public static DetachedCriteria AddEq(this DetachedCriteria dc, string propertyName, object value) {
            return dc.Add(Restrictions.Eq(propertyName, value));
        }

        /// <summary>
        /// Add NotEq (Not equal) expression to detached criteria
        /// </summary>
        public static DetachedCriteria AddNotEq(this DetachedCriteria dc, string propertyName, object value) {
            return dc.Add(Restrictions.Not(Restrictions.Eq(propertyName, value)));
        }

        /// <summary>
        /// Add Le (Little or Equal) expression to detached criteria
        /// </summary>
        public static DetachedCriteria AddLe(this DetachedCriteria dc, string propertyName, object value) {
            return dc.Add(Restrictions.Le(propertyName, value));
        }

        /// <summary>
        /// Add Lt (Little than) expression to detached criteria
        /// </summary>
        public static DetachedCriteria AddLt(this DetachedCriteria dc, string propertyName, object value) {
            return dc.Add(Restrictions.Lt(propertyName, value));
        }

        /// <summary>
        /// Add Ge (Greater or equal) expression to detached criteria
        /// </summary>
        public static DetachedCriteria AddGe(this DetachedCriteria dc, string propertyName, object value) {
            return dc.Add(Restrictions.Ge(propertyName, value));
        }

        /// <summary>
        /// Add Gt (Greater than) expression to detached criteria
        /// </summary>
        public static DetachedCriteria AddGt(this DetachedCriteria dc, string propertyName, object value) {
            return dc.Add(Restrictions.Gt(propertyName, value));
        }

        /// <summary>
        /// Add AllEq (all equal) expression to detached criteria
        /// </summary>
        public static DetachedCriteria AddAllEq(this DetachedCriteria dc, IDictionary propertyNameValues) {
            return dc.Add(Restrictions.AllEq(propertyNameValues));
        }

        /// <summary>
        /// Add IsEmpty expression to detached criteria
        /// </summary>
        public static DetachedCriteria AddIsEmpty(this DetachedCriteria dc, string propertyName) {
            return dc.Add(Restrictions.IsEmpty(propertyName));
        }

        /// <summary>
        /// Add IsNotEmpty expression to detached criteria
        /// </summary>
        public static DetachedCriteria AddIsNotEmpty(this DetachedCriteria dc, string propertyName) {
            return dc.Add(Restrictions.IsNotEmpty(propertyName));
        }

        /// <summary>
        /// Add IsNull expression to detached criteria
        /// </summary>
        public static DetachedCriteria AddIsNull(this DetachedCriteria dc, string propertyName) {
            return dc.Add(Restrictions.IsNull(propertyName));
        }

        /// <summary>
        /// Add IsNotNull expression to detached criteria
        /// </summary>
        public static DetachedCriteria AddIsNotNull(this DetachedCriteria dc, string propertyName) {
            return dc.Add(Restrictions.IsNotNull(propertyName));
        }

        /// <summary>
        /// 속성이 같은 값을 가지거나 IS NULL 인 경우를 나타내는 Criterion을 생성합니다.<br/>
        /// 예 : ( Product = :Product or Product IS NULL) 
        /// </summary>
        public static DetachedCriteria AddEqIncludeNull(this DetachedCriteria dc, string propertyName, object value) {
            return dc.Add(EqIncludeNull(propertyName, value));
        }

        /// <summary>
        /// 속성 값이 null이면 is null로, 값이 있으면 '=' 를 사용하는 ICriterion 을 생성한다. (즉 지정한 변수 값에 따라 달라진다)
        /// </summary>
        public static DetachedCriteria AddEqOrNull(this DetachedCriteria dc, string propertyName, object value) {
            return dc.Add(EqOrNull(propertyName, value));
        }

        /// <summary>
        /// NOTE : Sensitive Like 이다. 대소문자 구분이 필요없다면 <see cref="AddInsensitiveLike(NHibernate.Criterion.DetachedCriteria,string,string,NHibernate.Criterion.MatchMode)"/>
        /// </summary>
        /// 
        public static DetachedCriteria AddLike(this DetachedCriteria dc, string propertyName, string value) {
            return AddLike(dc, propertyName, value, null);
        }

        /// <summary>
        /// NOTE : Sensitive Like 이다. 대소문자 구분이 필요없다면 <see cref="AddInsensitiveLike(NHibernate.Criterion.DetachedCriteria,string,string,NHibernate.Criterion.MatchMode)"/>
        /// </summary>
        /// 
        public static DetachedCriteria AddLike(this DetachedCriteria dc, string propertyName, string value, MatchMode matchMode) {
            return dc.Add(Restrictions.Like(propertyName, value, matchMode ?? MatchMode.Anywhere));
        }

        /// <summary>
        /// add InsensitiveLike (like search with ignore case) expression to detached criteria
        /// </summary>
        /// <param name="dc">Instance of Detached Criteria</param>
        /// <param name="propertyName">비교할 속성 명</param>
        /// <param name="value">검색할 속성 값</param>
        /// <returns>검색 필터를 추가한 Detached Criteria</returns>
        public static DetachedCriteria AddInsensitiveLike(this DetachedCriteria dc, string propertyName, string value) {
            return AddInsensitiveLike(dc, propertyName, value, null);
        }

        /// <summary>
        /// add InsensitiveLike (like search with ignore case) expression to detached criteria
        /// </summary>
        /// <param name="dc">Instance of Detached Criteria</param>
        /// <param name="propertyName">비교할 속성 명</param>
        /// <param name="value">검색할 속성 값</param>
        /// <param name="matchMode">매칭 모드</param>
        /// <returns>검색 필터를 추가한 Detached Criteria</returns>
        public static DetachedCriteria AddInsensitiveLike(this DetachedCriteria dc, string propertyName, string value,
                                                          MatchMode matchMode) {
            return dc.Add(Restrictions.InsensitiveLike(propertyName, value, matchMode ?? MatchMode.Anywhere));
        }

        /// <summary>
        /// Add IdEq expression to detached criteria
        /// </summary>
        public static DetachedCriteria AddIdEq(this DetachedCriteria dc, object value) {
            return dc.Add(Restrictions.IdEq(value));
        }

        /// <summary>
        /// Add In expression to detached criteria
        /// </summary>
        public static DetachedCriteria AddIn(this DetachedCriteria dc, string propertyName, ICollection values) {
            return dc.Add(Restrictions.In(propertyName, values));
        }

        /// <summary>
        /// Add InG{T} expression to detached criteria
        /// </summary>
        public static DetachedCriteria AddInG<T>(this DetachedCriteria dc, string propertyName, ICollection<T> values) {
            return dc.Add(Restrictions.InG(propertyName, values));
        }

        /// <summary>
        /// Add between expression to detached criteria
        /// </summary>
        public static DetachedCriteria AddBetween(this DetachedCriteria dc, string propertyName, object lo, object hi) {
            // return dc.Add(IsBetweenCriterion(propertyName, lo, hi));

            if(lo != null)
                dc.AddGe(propertyName, lo);
            if(hi != null)
                dc.AddLe(propertyName, hi);

            return dc;
        }

        /// <summary>
        /// 지정한 값이 두개의 속성의 사이 값인지 판단하는 expression을 detached criteria에 추가한다. Between과 비숫하지만, 대상이 바뀐 것이다.
        /// 예: StartDate &lt; CurrentDate and CurrentDate &lt; EndDate
        /// </summary>
        public static DetachedCriteria AddInRange(this DetachedCriteria dc, object value, string loPropertyName, string hiPropertyName) {
            return dc.Add(IsInRangeCriterion(value, loPropertyName, hiPropertyName));
        }

        /// <summary>
        /// 지정한 DateRange가 기간을 나타내는 두개의 속성과 overlap이 되는지 검사한다.
        /// </summary>
        /// <param name="dc">Detached Criteria 인스턴스</param>
        /// <param name="period">기간</param>
        /// <param name="loPropertyName">하한 값을 가지는 속성명</param>
        /// <param name="hiPropertyName">상한 값을 가지는 속성명</param>
        /// <returns></returns>
        public static DetachedCriteria AddIsOverlap(this DetachedCriteria dc, ITimePeriod period, string loPropertyName,
                                                    string hiPropertyName) {
            return dc.Add(IsOverlapCriterion(period, loPropertyName, hiPropertyName));
        }

        /// <summary>
        /// 지정한 날짜 속성 값이 current 기준으로 과거 인지 판단한다.
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="current"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static DetachedCriteria AddIsElapsed(this DetachedCriteria dc, DateTime current, string propertyName) {
            return dc.Add(Restrictions.Lt(propertyName, current));
        }

        /// <summary>
        /// 지정한 속성 값이 NULL이면 False로 간주하는 Where 절을 추가한다.
        /// Explicit 하게 PropertyName = True 로 되어 있는 것을 제외한 False이거나 NULL 것은 False 로 간주한다.
        /// </summary>
        public static DetachedCriteria AddNullAsFalse(this DetachedCriteria dc, string propertyName, bool? value) {
            if(value.GetValueOrDefault(false))
                return dc.AddEq(propertyName, true);

            return dc.AddEqIncludeNull(propertyName, false);
        }

        /// <summary>
        /// 지정한 속성 값이 NULL이면 True로 간주하는 Where 절을 추가한다. 
        /// PropertyName 를 조회할 때, 명확히 PropertyName=False가 아니라 NULL이거나, True라면 True로 간주한다.
        /// </summary>
        /// <param name="dc">criteria</param>
        /// <param name="propertyName">property name</param>
        /// <param name="value">property value to filter</param>
        /// <returns>criteria</returns>
        public static DetachedCriteria AddNullAsTrue(this DetachedCriteria dc, string propertyName, bool? value) {
            if(value.GetValueOrDefault(true) == false)
                return dc.AddEq(propertyName, false);

            return dc.AddEqIncludeNull(propertyName, true);
        }

        /// <summary>
        /// 지정된 Criteria에 대해 NOT을 수행합니다.
        /// </summary>
        public static DetachedCriteria AddNot(this DetachedCriteria dc, ICriterion criterion) {
            return dc.Add(Restrictions.Not(criterion));
        }

        /// <summary>
        /// 지정된 Criteria에 Ordering 을 추가합니다.
        /// </summary>
        /// <param name="dc"></param>
        /// <param name="orders"></param>
        /// <returns></returns>
        public static DetachedCriteria AddOrders(this DetachedCriteria dc, params Order[] orders) {
            if(orders != null)
                foreach(var order in orders)
                    dc = dc.AddOrder(order);

            return dc;
        }

        /// <summary>
        /// 코드 속성명 (Code)
        /// </summary>
        public const string CodePropertyName = "Code";

        /// <summary>
        /// 이름 속성명 (Name)
        /// </summary>
        public const string NamePropertyName = "Name";

        /// <summary>
        /// 타이틀 속성명 (Title)
        /// </summary>
        public const string TitlePropertyName = "Title";

        /// <summary>
        /// IsEnabled 속성명 (IsEnabled)
        /// </summary>
        public const string IsEnabledPropertyName = "IsEnabled";

        /// <summary>
        /// IsActive 속성명 (IsActive)
        /// </summary>
        public const string IsActivePropertyName = "IsActive";

        /// <summary>
        /// Parent 속성명 (Parent)
        /// </summary>
        public const string ParentPropertyName = "Parent";

        /// <summary>
        /// 지정한 속성 값이 NULL이면 False로 간주하는 Where 절을 추가한다.
        /// Explicit 하게 IsReleased = True 로 되어 있는 것을 제외한 False이거나 NULL 것은 False 로 간주한다.
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DetachedCriteria NullAsFalse(this DetachedCriteria criteria, string propertyName, bool? value) {
            if(IsDebugEnabled)
                log.Debug("지정한 속성 값이 NULL이면 False로 간주하는 Eq 절을 빌드한다... propertyName=[{0}], value=[{1}]", propertyName, value);

            return value.GetValueOrDefault(false)
                       ? criteria.AddEq(propertyName, true)
                       : criteria.AddEqIncludeNull(propertyName, false);
        }

        /// <summary>
        /// 지정한 속성 값이 NULL이면 True로 간주하는 Where 절을 추가한다. (IsEnabled 를 조회할 때, 명확히 IsEnabled=False가 아니라면 NULL이거나, True라면 True로 간주한다.
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DetachedCriteria NullAsTrue(this DetachedCriteria criteria, string propertyName, bool? value) {
            if(IsDebugEnabled)
                log.Debug("지정한 속성 값이 NULL이면 True로 간주하는 Eq 절을 빌드한다... propertyName=[{0}], value=[{1}]", propertyName, value);

            return value.GetValueOrDefault(true)
                       ? criteria.AddEqIncludeNull(propertyName, value)
                       : criteria.AddEq(propertyName, value);
        }

        /// <summary>
        /// 지정된 코드로 검색하는 Criteria를 추가한다.
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static DetachedCriteria AddCodeEq(this DetachedCriteria criteria, string code) {
            if(IsDebugEnabled)
                log.Debug("Code 속성 일치 Criteria를 빌드합니다. code=[{0}]", code);

            return criteria.AddEq(CodePropertyName, code);
        }

        /// <summary>
        /// 지정된 criteria에 Name 속성 = name
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DetachedCriteria AddNameEq(this DetachedCriteria criteria, string name) {
            if(IsDebugEnabled)
                log.Debug("Name 속성 일치 Criteria를 빌드합니다. name=[{0}]", name);

            return criteria.AddEq(NamePropertyName, name);
        }

        /// <summary>
        /// 엔티티의 Code 속성값이 지정된 Code와 매칭(LIKE 검색) 되는 엔티티를 조회하는 DetachedCriteria를 빌드한다.
        /// </summary>
        public static DetachedCriteria AddCodeLike(this DetachedCriteria criteria, string codeToMatch, MatchMode matchMode) {
            if(IsDebugEnabled)
                log.Debug("Code 속성을 매칭 검색하도록 Criteria를 빌드합니다. codeToMatch=[{0}], matchMode=[{1}]", codeToMatch, matchMode);

            return criteria.AddLike(CodePropertyName, codeToMatch, matchMode);
        }

        /// <summary>
        /// 엔티티의 Name 속성값이 지정된 Name과 매칭(LIKE 검색) 되는 엔티티를 조회하는 DetachedCriteria를 빌드한다.
        /// </summary>
        public static DetachedCriteria AddNameLike(this DetachedCriteria criteria, string nameToMatch, MatchMode matchMode) {
            if(IsDebugEnabled)
                log.Debug("Name 속성을 매칭 검색하도록 Criteria를 빌드합니다. nameToMatch=[{0}], matchMode=[{1}]", nameToMatch, matchMode);

            return criteria.AddLike(NamePropertyName, nameToMatch, matchMode);
        }

        /// <summary>
        /// 엔티티의 Name 속성값이 지정된 Name과 매칭(LIKE 검색) 되는 엔티티를 조회하는 DetachedCriteria를 빌드한다.
        /// </summary>
        public static DetachedCriteria AddNameLike(this DetachedCriteria criteria, string nameToMatch) {
            return AddNameLike(criteria, nameToMatch, MatchMode.Start);
        }

        /// <summary>
        /// 엔티티의 Title 속성값이 지정된 값과 매칭(Like 검색) 되는 엔티티를 조회하는 Criteria를 빌드한다.
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="titleToMatch"></param>
        /// <param name="matchMode"></param>
        /// <returns></returns>
        public static DetachedCriteria AddTitleLike(this DetachedCriteria criteria, string titleToMatch, MatchMode matchMode) {
            if(IsDebugEnabled)
                log.Debug("Title 속성을 매칭 검색하도록 Criteria를 빌드합니다. titleToMatch=[{0}], matchMode=[{1}]", titleToMatch, matchMode);

            return criteria.AddLike(TitlePropertyName, titleToMatch, matchMode);
        }

        /// <summary>
        /// 엔티티의 IsActive 속성이 True이거나 NULL 인 엔티티를 조회하는 Criteria를 빌드한다.
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public static DetachedCriteria AddActive(this DetachedCriteria criteria) {
            return criteria.AddNullAsTrue(IsActivePropertyName, true);
        }

        /// <summary>
        /// 엔티티의 IsEnabled 속성이 True이거나 NULL 인 엔티티를 조회하는 Criteria를 빌드한다.
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public static DetachedCriteria AddEnabled(this DetachedCriteria criteria) {
            return criteria.AddNullAsTrue(IsEnabledPropertyName, true);
        }

        /// <summary>
        /// IsActive 속성으로 검색하는 Criteria를 빌드한다.
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="isActive"></param>
        /// <returns></returns>
        public static DetachedCriteria AddIsActive(this DetachedCriteria criteria, bool? isActive) {
            return criteria.AddEqOrNull(IsActivePropertyName, isActive);
        }

        /// <summary>
        /// IsEnabled 속성으로 검색하는 Criteria를 빌드한다.
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="isEnabled"></param>
        /// <returns></returns>
        public static DetachedCriteria AddIsEnabled(this DetachedCriteria criteria, bool? isEnabled) {
            return criteria.AddEqOrNull(IsEnabledPropertyName, isEnabled);
        }

        /// <summary>
        /// Parent 속성이 NULL 인 검색 조건을 추가합니다.
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public static DetachedCriteria AddParentIsNull(this DetachedCriteria criteria) {
            return criteria.AddIsNull(ParentPropertyName);
        }

        /// <summary>
        /// Parent 속성이 NULL이 아닌 검색 조건을 추가합니다.
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public static DetachedCriteria AddParentIsNotNull(this DetachedCriteria criteria) {
            return criteria.AddIsNotNull(ParentPropertyName);
        }

        /// <summary>
        /// 속성 값이 null이면 is null로, 값이 있으면 '=' 를 사용하는 ICriterion 을 생성한다. (즉 지정한 변수 값에 따라 달라진다)
        /// </summary>
        public static ICriterion EqOrNull(string propertyName, object value) {
            return new EqOrNullExpression(propertyName, value);
        }

        /// <summary>
        /// 속성 값이 null이면 is null로, 값이 있으면 '=' 를 사용하는 ICriterion 을 생성한다. (즉 지정한 변수 값에 따라 달라진다)
        /// </summary>
        public static ICriterion EqOrNull(IProjection projection, object value) {
            return new EqOrNullExpression(projection, value);
        }

        /// <summary>
        /// 속성이 같은 값을 가지거나 NULL 인 경우를 나타내는 Criterion을 생성합니다.<br/>
        /// 예 : ( Product = 'RealAdmin' or Product is null) 
        /// </summary>
        public static ICriterion EqIncludeNull(string propertyName, object value) {
            return new EqIncludeNullExpression(propertyName, value);
        }

        /// <summary>
        /// 속성이 같은 값을 가지거나 NULL 인 경우를 나타내는 Criterion을 생성합니다.<br/>
        /// 예 : ( Product = 'RealAdmin' or Product is null) 
        /// </summary>
        public static ICriterion EqIncludeNull(IProjection projection, object value) {
            return new EqIncludeNullExpression(projection, value);
        }

        /// <summary>
        /// 값이 NULL인 경우를 포함해서,  매칭되는 검색, ((A LIKE P0) OR (A IS NULL)) <br/>
        /// 예: 예 : ( Product = 'Real%' or Product is null) 
        /// </summary>
        public static ICriterion InsensitiveLikeIncludeNull(string propertyName, string value, MatchMode matchMode) {
            return new InsensitiveLikeIncludeNullExpression(propertyName, value, matchMode);
        }

        /// <summary>
        /// 값이 NULL인 경우를 포함해서,  매칭되는 검색, ((A LIKE P0) OR (A IS NULL)) <br/>
        /// 예 : ( Product = 'Real%' or Product is null) 
        /// </summary>
        public static ICriterion InsensitiveLikeIncludeNull(string propertyName, object value) {
            return new InsensitiveLikeIncludeNullExpression(propertyName, value);
        }

        /// <summary>
        /// 값이 NULL인 경우를 포함해서,  매칭되는 검색, ((A LIKE P0) OR (A IS NULL)) <br/>
        /// 예: 예 : ( Product = 'Real%' or Product is null) 
        /// </summary>
        public static ICriterion InsensitiveLikeIncludeNull(IProjection projection, string value, MatchMode matchMode) {
            return new InsensitiveLikeIncludeNullExpression(projection, value, matchMode);
        }

        /// <summary>
        /// 값이 NULL인 경우를 포함해서,  매칭되는 검색, ((A LIKE P0) OR (A IS NULL)) <br/>
        /// 예: 예 : ( Product = 'Real%' or Product is null) 
        /// </summary>
        public static ICriterion InsensitiveLikeIncludeNull(IProjection projection, object value) {
            return new InsensitiveLikeIncludeNullExpression(projection, value);
        }
    }
}