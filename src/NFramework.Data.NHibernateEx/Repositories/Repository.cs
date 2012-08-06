using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Castle.Core;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Metadata;
using NSoft.NFramework.InversionOfControl;

namespace NSoft.NFramework.Data.NHibernateEx {
    /// <summary>
    /// NHibernate용 Repository의 기능을 제공하는 Helper Class입니다.
    /// </summary>
    /// <typeparam name="T">Type of entity</typeparam>
    [Serializable]
    public static class Repository<T> where T : class {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private static readonly object _syncLock = new object();
        private static readonly Type _concreteType = typeof(T);

        [ThreadStatic] private static INHRepository<T> _repository;

        /// <summary>
        /// Inner Repository for Entity
        /// </summary>
        internal static INHRepository<T> Inner {
            get {
                if(_repository == null)
                    lock(_syncLock)
                        if(_repository == null) {
                            var repository = IoC.TryResolve<INHRepository<T>, NHRepository<T>>(LifestyleType.Thread);
                            Thread.MemoryBarrier();
                            _repository = repository;

                            if(IsDebugEnabled)
                                log.Debug("NHRepository<{0}> 를 생성했습니다.", _concreteType.Name);
                        }
                return _repository;
            }
        }

        #region << Properties >>

        /// <summary>
        /// Type of Entity which handled by this Reposiotry
        /// </summary>
        public static Type ConcreteType {
            get { return Inner.ConcreteType; }
        }

        /// <summary>
        /// Entity가 매핑된 Session Factory 인스턴스
        /// </summary>
        public static ISessionFactory SessionFactory {
            get { return Inner.SessionFactory; }
        }

        /// <summary>
        /// UnitOfWork의 현재 Session 인스턴스
        /// </summary>
        public static ISession Session {
            get { return Inner.Session; }
        }

        /// <summary>
        /// Ini 파일로부터 NHibernate HQL 문장을 제공하는 Provider
        /// </summary>
        public static IIniQueryProvider QueryProvider {
            get { return Inner.QueryProvider; }
        }

        #endregion

        #region << Get / Load >>

        /// <summary>
        /// Get Entity by specified identity value or return nulll if it doesn't exist
        /// </summary>
        /// <param name="id"></param>
        /// <returns>if not exists, return null</returns>
        public static T Get(object id) {
            return Inner.Get(id);
        }

        /// <summary>
        /// Get Entity by specified identity value or return nulll if it doesn't exist
        /// </summary>
        /// <param name="id">identity of entity</param>
        /// <param name="lockMode">entity lock mode</param>
        /// <returns>if not exists, return null</returns>
        public static T Get(object id, LockMode lockMode) {
            return Inner.Get(id, lockMode);
        }

        /// <summary>
        /// Load Entity by specified identity value or throw an exception if there isn't an entity that matches the specified id
        /// </summary>
        /// <param name="id">identity of entity</param>
        /// <returns>if not exists, exception occurred</returns>
        public static T Load(object id) {
            return Inner.Load(id);
        }

        /// <summary>
        /// Load Entity by specified identity value or throw an exception if there isn't an entity that matches the specified id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="lockMode">entity lock mode</param>
        /// <returns>if not exists, exception occurred</returns>
        public static T Load(object id, LockMode lockMode) {
            return Inner.Load(id, lockMode);
        }

        #endregion

        #region << GetIn / GetInG >>

        /// <summary>
        /// 지정된 Identity 배열에 해당하는 모든 Entity를 로딩한다. (In 을 사용한다)
        /// </summary>
        /// <param name="ids">identity values</param>
        /// <returns>list of entity</returns>
        public static IList<T> GetIn(ICollection ids) {
            return Inner.GetIn(ids);
        }

        /// <summary>
        /// 지정된 Id 컬렉션에 해당하는 모든 Entity를 로딩한다. (SQL 의 IN (xxx,yyy,zzz) 를 사용한다)
        /// </summary>
        /// <typeparam name="TId">Entity Id의 수형</typeparam>
        /// <param name="ids">Id값의 컬렉션</param>
        /// <returns>list of entity</returns>
        public static IList<T> GetInG<TId>(ICollection<TId> ids) {
            return Inner.GetInG(ids);
        }

        #endregion

        #region << GetPage >>

        /// <summary>
        /// Get paginated entity list
        /// </summary>
        /// <param name="pageIndex">Page index (start from 0)</param>
        /// <param name="pageSize">Page size (must greator than 0)</param>
        /// <param name="criteria">criteria</param>
        /// <returns>paginated list</returns>
        public static IPagingList<T> GetPage(int pageIndex, int pageSize, ICriterion[] criteria) {
            return Inner.GetPage(pageIndex, pageSize, criteria);
        }

        /// <summary>
        /// Get paginated entity list
        /// </summary>
        /// <param name="pageIndex">Page index (start from 0)</param>
        /// <param name="pageSize">Page size (must greator than 0)</param>
        /// <param name="orders">sort order</param>
        /// <param name="criteria">criteria</param>
        /// <returns>paginated list</returns>
        public static IPagingList<T> GetPage(int pageIndex, int pageSize, Order[] orders, params ICriterion[] criteria) {
            return Inner.GetPage(pageIndex, pageSize, orders, criteria);
        }

        /// <summary>
        /// Get paginated entity list
        /// </summary>
        /// <param name="pageIndex">Page index (start from 0)</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="expressions">expression for criteria</param>
        /// <returns>paginated list</returns>
        public static IPagingList<T> GetPage(int pageIndex, int pageSize, params Expression<Func<T, bool>>[] expressions) {
            return Inner.GetPage(pageIndex, pageSize, expressions);
        }

        /// <summary>
        /// Get paginated entity list
        /// </summary>
        /// <param name="pageIndex">Page index (start from 0)</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="orders">sort order</param>
        /// <param name="expressions">expression for criteria</param>
        /// <returns>paginated list</returns>
        public static IPagingList<T> GetPage(int pageIndex, int pageSize, INHOrder<T>[] orders,
                                             params Expression<Func<T, bool>>[] expressions) {
            return Inner.GetPage(pageIndex, pageSize, orders, expressions);
        }

        /// <summary>
        /// Get paginated entity list
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="orders"></param>
        /// <returns></returns>
        public static IPagingList<T> GetPage(DetachedCriteria criteria, params Order[] orders) {
            return Inner.GetPage(criteria, orders);
        }

        /// <summary>
        /// Get paginated entity list
        /// </summary>
        /// <param name="pageIndex">Page index (start from 0)</param>
        /// <param name="pageSize">Page size (must greator than 0)</param>
        /// <param name="criteria">DetachedCriteria (null 이면 모든 레코드를 조회합니다)</param>
        /// <param name="orders">sort order</param>
        /// <returns>pagenated list</returns>
        public static IPagingList<T> GetPage(int pageIndex, int pageSize, DetachedCriteria criteria, params Order[] orders) {
            return Inner.GetPage(pageIndex, pageSize, criteria, orders);
        }

        /// <summary>
        /// Get paginated entity list
        /// </summary>
        /// <param name="orders">sort order</param>
        /// <returns>pagenated list</returns>
        public static IPagingList<T> GetPage(params INHOrder<T>[] orders) {
            return Inner.GetPage(orders);
        }

        /// <summary>
        /// Get paginated entity list
        /// </summary>
        /// <param name="queryOver">detached QueryOver{T,T}</param>
        /// <param name="orders">sort order</param>
        /// <returns>pagenated list</returns>
        public static IPagingList<T> GetPage(QueryOver<T> queryOver, params INHOrder<T>[] orders) {
            return Inner.GetPage(queryOver, orders);
        }

        /// <summary>
        /// Get paginated entity list
        /// </summary>
        /// <param name="pageIndex">Page index (start from 0)</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="queryOver">detached QueryOver{T,T}</param>
        /// <param name="orders">sort order</param>
        /// <returns>pagenated list</returns>
        public static IPagingList<T> GetPage(int pageIndex, int pageSize, QueryOver<T> queryOver, params INHOrder<T>[] orders) {
            return Inner.GetPage(pageIndex, pageSize, queryOver, orders);
        }

        #endregion

        #region << Future >>

        /// <summary>
        /// Get a future entity from the persistence store, or return null
        /// </summary>
        /// <param name="id"></param>
        /// <returns>a future for the entity that matches the id. if it doesn't exist, return null.</returns>
        [Obsolete("NHibernate 3.x의 Future 기능을 사용하세요")]
        public static IFutureValue<T> FutureGet(object id) {
            return Inner.FutureGet(id);
        }

        /// <summary>
        /// Load a future entity from the persistence store.
        /// Will throw an exception if there isn't an entity that matches the specified id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>a future for the entity that matches the id. if it doesn't exist, throw exception.</returns>
        [Obsolete("NHibernate 3.x의 Future 기능을 사용하세요")]
        public static IFutureValue<T> FutureLoad(object id) {
            return Inner.FutureLoad(id);
        }

        /// <summary>
        /// Get a future entity collection from the persistence store
        /// </summary>
        /// <param name="detachedCriteria"></param>
        /// <returns></returns>
        [Obsolete("NHibernate 3.x의 Future 기능을 사용하세요")]
        public static IFutureValue<T> FutureValue(DetachedCriteria detachedCriteria) {
            return Inner.FutureValue(detachedCriteria);
        }

        /// <summary>
        /// Get a future entity collection from the persistence store
        /// </summary>
        [Obsolete("NHibernate 3.x의 Future 기능을 사용하세요")]
        public static IFutureValue<T> FutureValue(QueryOver<T> queryOver) {
            return Inner.FutureValue(queryOver);
        }

        /// <summary>
        /// Get a future entity collection from the persistence store
        /// </summary>
        [Obsolete("NHibernate 3.x의 Future 기능을 사용하세요")]
        public static IFutureValue<T> FutureValue(params Expression<Func<T, bool>>[] expressions) {
            return Inner.FutureValue(expressions);
        }

        /// <summary>
        /// Get a future entity collection from the persistence store
        /// </summary>
        public static IEnumerable<T> Future(DetachedCriteria detachedCriteria, params Order[] orders) {
            return Inner.Future(detachedCriteria, orders);
        }

        /// <summary>
        /// Get a future entity collection from the persistence store
        /// </summary>
        public static IEnumerable<T> Future(QueryOver<T> queryOver, params INHOrder<T>[] orders) {
            return Inner.Future(queryOver, orders);
        }

        /// <summary>
        /// Get a future entity collection from the persistence store
        /// </summary>
        public static IEnumerable<T> Future(Expression<Func<T, bool>>[] expressions, params INHOrder<T>[] orders) {
            return Inner.Future(expressions, orders);
        }

        #endregion

        #region << FindAll >>

        /// <summary>
        /// Get all entities.
        /// </summary>
        /// <returns>entity collection</returns>
        public static IList<T> FindAll() {
            return Inner.FindAll();
        }

        /// <summary>
        /// Get entities matched with criteria
        /// </summary>
        /// <param name="criteria">where</param>
        /// <returns>entity collection</returns>
        public static IList<T> FindAll(ICriterion[] criteria) {
            return Inner.FindAll(criteria);
        }

        /// <summary>
        /// Get ordered and ranged entities matched with criteria
        /// </summary>
        /// <param name="orders">ordering spec</param>
        /// <param name="criteria">where spec</param>
        /// <returns>entity collection</returns>
        public static IList<T> FindAll(Order[] orders, params ICriterion[] criteria) {
            return Inner.FindAll(orders, criteria);
        }

        /// <summary>
        /// Get ordered and ranged entities matched with criteria
        /// </summary>
        /// <param name="orders">ordering spec</param>
        /// <param name="firstResult">first index (start from 0)</param>
        /// <param name="maxResults">max resultset count (start from 1)</param>
        /// <param name="criteria">where spec</param>
        /// <returns>entity collection</returns>
        public static IList<T> FindAll(Order[] orders, int firstResult, int maxResults, params ICriterion[] criteria) {
            return Inner.FindAll(orders, firstResult, maxResults, criteria);
        }

        /// <summary>
        /// Get ranged entities matched with criteria
        /// </summary>
        /// <param name="firstResult">first index (start from 0)</param>
        /// <param name="maxResults">max resultset count (start from 1) (0이면 마지막 Record까지 읽어온다.)</param>
        /// <param name="criteria">where</param>
        /// <returns>entity collection</returns>
        public static IList<T> FindAll(int firstResult, int maxResults, ICriterion[] criteria) {
            return Inner.FindAll(firstResult, maxResults, criteria);
        }

        /// <summary>
        /// 지정된 criteria를 이용하여 정보를 조회합니다.
        /// </summary>
        /// <param name="firstResult">first index (start from 0)</param>
        /// <param name="maxResults">max resultset count (start from 1) (0이면 마지막 Record까지 읽어온다.)</param>
        /// <param name="orders">정렬 순서</param>
        /// <param name="criteria">조회 조건</param>
        /// <returns>collection of entity.</returns>
        public static IList<T> FindAll(int firstResult, int maxResults, Order[] orders, params ICriterion[] criteria) {
            return Inner.FindAll(firstResult, maxResults, orders, criteria);
        }

        /// <summary>
        /// Get entities matched with criteria
        /// </summary>
        /// <param name="expression">조건 표현</param>
        /// <param name="expressions">조건 표현식</param>
        /// <returns>entity collection</returns>
        public static IList<T> FindAll(Expression<Func<T, bool>> expression, params Expression<Func<T, bool>>[] expressions) {
            return Inner.FindAll(expression, expressions);
        }

        /// <summary>
        /// Get ordered and ranged entities matched with criteria
        /// </summary>
        /// <param name="orders">ordering spec</param>
        /// <param name="expressions">조건 표현식</param>
        /// <returns>entity collection</returns>
        public static IList<T> FindAll(INHOrder<T>[] orders, params Expression<Func<T, bool>>[] expressions) {
            return Inner.FindAll(orders, expressions);
        }

        /// <summary>
        /// 지정된 criteria를 이용하여 정보를 조회합니다.
        /// </summary>
        /// <param name="firstResult">first index (start from 0)</param>
        /// <param name="maxResults">max resultset count (start from 1) (0이면 마지막 Record까지 읽어온다.)</param>
        /// <param name="orders">정렬 순서</param>
        /// <param name="expressions">조건 표현식</param>
        /// <returns>collection of entity.</returns>
        public static IList<T> FindAll(INHOrder<T>[] orders, int firstResult, int maxResults,
                                       params Expression<Func<T, bool>>[] expressions) {
            return Inner.FindAll(orders, firstResult, maxResults, expressions);
        }

        /// <summary>
        /// Get ranged entities matched with criteria
        /// </summary>
        /// <param name="firstResult">first index (start from 0)</param>
        /// <param name="maxResults">max resultset count (start from 1) (0이면 마지막 Record까지 읽어온다.)</param>
        /// <param name="expressions">조건 표현식</param>
        /// <returns>entity collection</returns>
        public static IList<T> FindAll(int firstResult, int maxResults, params Expression<Func<T, bool>>[] expressions) {
            return Inner.FindAll(firstResult, maxResults, expressions);
        }

        /// <summary>
        /// 지정된 criteria를 이용하여 정보를 조회합니다.
        /// </summary>
        /// <param name="firstResult">first index (start from 0)</param>
        /// <param name="maxResults">max resultset count (start from 1) (0이면 마지막 Record까지 읽어온다.)</param>
        /// <param name="orders">정렬 순서</param>
        /// <param name="expressions">조건 표현식</param>
        /// <returns>collection of entity.</returns>
        public static IList<T> FindAll(int firstResult, int maxResults, INHOrder<T>[] orders,
                                       params Expression<Func<T, bool>>[] expressions) {
            return Inner.FindAll(firstResult, maxResults, orders, expressions);
        }

        /// <summary>
        /// Get ranged entites matched with detached criteria, ordering is optional
        /// </summary>
        /// <param name="criteria">where spec</param>
        /// <param name="orders">ordering spec</param>
        /// <returns>entity collection</returns>
        public static IList<T> FindAll(DetachedCriteria criteria, params Order[] orders) {
            return Inner.FindAll(criteria, orders);
        }

        /// <summary>
        /// Get ranged entites matched with detached criteria, ordering is optional
        /// </summary>
        /// <param name="criteria">where spec</param>
        /// <param name="firstResult">first index (start from 0)</param>
        /// <param name="maxResults">max resultset count (start from 1)</param>
        /// <param name="orders">ordering spec</param>
        /// <returns>entity collection</returns>
        public static IList<T> FindAll(DetachedCriteria criteria, int firstResult, int maxResults, params Order[] orders) {
            return Inner.FindAll(criteria, firstResult, maxResults, orders);
        }

        /// <summary>
        /// Get ordered and ranged entities matched with criteria
        /// </summary>
        /// <param name="queryOver">Detached QueryOver</param>
        /// <param name="orders">ordering spec</param>
        /// <returns>entity collection</returns>
        public static IList<T> FindAll(QueryOver<T> queryOver, params INHOrder<T>[] orders) {
            return Inner.FindAll(queryOver, orders);
        }

        /// <summary>
        /// Get ordered and ranged entities matched with criteria
        /// </summary>
        /// <param name="queryOver">Detached QueryOver</param>
        /// <param name="firstResult">first index (start from 0)</param>
        /// <param name="maxResults">max resultset count (start from 1)</param>
        /// <param name="orders">ordering spec</param>
        /// <returns>entity collection</returns>
        public static IList<T> FindAll(QueryOver<T> queryOver, int firstResult, int maxResults, params INHOrder<T>[] orders) {
            return Inner.FindAll(queryOver, firstResult, maxResults, orders);
        }

        /// <summary>
        /// Get entities by examping with exampleInstance
        /// </summary>
        /// <param name="exampleInstance">instance for exampling</param>
        /// <param name="propertyNamesToExclude">excluded property for exampling</param>
        /// <returns>entity collection</returns>
        public static IList<T> FindAll(T exampleInstance, params string[] propertyNamesToExclude) {
            return Inner.FindAll(exampleInstance, propertyNamesToExclude);
        }

        /// <summary>
        /// Get entities by examping with exampleInstance
        /// </summary>
        /// <param name="exampleInstance">instance for exampling</param>
        /// <param name="firstResult">first index (start from 0)</param>
        /// <param name="maxResults">max resultset count (start from 1) (0이면 마지막 Record까지 가져온다.)</param>
        /// <param name="propertyNamesToExclude">excluded property for exampling</param>
        /// <returns>entity collection</returns>
        public static IList<T> FindAll(T exampleInstance, int firstResult, int maxResults, params string[] propertyNamesToExclude) {
            return Inner.FindAll(exampleInstance, firstResult, maxResults, propertyNamesToExclude);
        }

        /// <summary>
        /// Get entities by Named Query which defined in mapping files (*.hbm.xml) ex: &lt;query name="xxxx"&gt;
        /// </summary>
        /// <param name="namedQuery">name of NamedQuery which defined in mapping files(*.hbm.xml)</param>
        /// <param name="parameters">HQL Parameters</param>
        /// <returns>entity collection</returns>
        public static IList<T> FindAll(string namedQuery, params INHParameter[] parameters) {
            return Inner.FindAll(namedQuery, parameters);
        }

        /// <summary>
        /// Get entities by Named Query which defined in mapping files (*.hbm.xml) ex: &lt;query name="xxxx"&gt;
        /// </summary>
        /// <param name="namedQuery">name of NamedQuery which defined in mapping files(*.hbm.xml)</param>
        /// <param name="firstResult">first index (start from 0)</param>
        /// <param name="maxResults">max resultset count (start from 1) (0이면 마지막 Record까지 가져온다.)</param>
        /// <param name="parameters">HQL Parameters</param>
        /// <returns>entity collection</returns>
        public static IList<T> FindAll(string namedQuery, int firstResult, int maxResults, params INHParameter[] parameters) {
            return Inner.FindAll(namedQuery, firstResult, maxResults, parameters);
        }

        /// <summary>
        /// NHibernate Query Language (HQL) 를 이용한 조회
        /// </summary>
        /// <param name="queryString">hql string</param>
        /// <param name="parameters">HQL Parameters</param>
        /// <returns>entity collection</returns>
        public static IList<T> FindAllByHql(string queryString, params INHParameter[] parameters) {
            return Inner.FindAllByHql(queryString, parameters);
        }

        /// <summary>
        /// NHibernate Query Language (HQL) 를 이용한 조회
        /// </summary>
        /// <param name="queryString">hql string</param>
        /// <param name="firstResult">first index (start from 0)</param>
        /// <param name="maxResults">max resultset count (start from 1) (0이면 마지막 Record까지 가져온다.)</param>
        /// <param name="parameters">HQL Parameters</param>
        /// <returns>entity collection</returns>
        public static IList<T> FindAllByHql(string queryString, int firstResult, int maxResults, params INHParameter[] parameters) {
            return Inner.FindAllByHql(queryString, firstResult, maxResults, parameters);
        }

        /// <summary>
        /// Entity의 속성명이 지정된 값과 같은 엔티티를 모두 조회한다. (propertyName = value)
        /// </summary>
        /// <param name="propertyName">속성명</param>
        /// <param name="value">비교할 값</param>
        /// <returns>entity collection</returns>
        [Obsolete("FindAll(params Expression<Func<T, bool>>[] expressions) 를 사용하거나 INHRepository<T>.Query() 를 사용하세요.")]
        public static IList<T> FindAllPropertyEq(string propertyName, object value) {
            propertyName.ShouldNotBeWhiteSpace("propertyName");
            return Inner.FindAllPropertyEq(propertyName, value);
        }

        #endregion

        #region << FindOne >>

        /// <summary>
        /// Get unique entity matches with specified criteria. if one more entity exists, throw exception
        /// </summary>
        /// <param name="criteria">where spec</param>
        /// <returns>a single instance that matches the query, or null if the query returns no results.</returns>
        public static T FindOne(ICriterion[] criteria) {
            return Inner.FindOne(criteria);
        }

        /// <summary>
        /// Get unique entity matches with specified criteria
        /// </summary>
        /// <param name="expressions">where spec</param>
        /// <returns>if not unique entity or not exists, raise exception</returns>
        public static T FindOne(params Expression<Func<T, bool>>[] expressions) {
            return Inner.FindOne(expressions);
        }

        /// <summary>
        /// Get unique entity matches with specified detached criteria. if one more entity exists, throw exception
        /// </summary>
        /// <param name="criteria">where spec</param>
        /// <returns>a single instance that matches the query, or null if the query returns no results.</returns>
        public static T FindOne(DetachedCriteria criteria) {
            return Inner.FindOne(criteria);
        }

        /// <summary>
        /// Get unique entity matches with specified detached criteria. if one more entity exists, throw exception
        /// </summary>
        /// <param name="queryOver">where spec</param>
        /// <returns>a single instance that matches the query, or null if the query returns no results.</returns>
        public static T FindOne(QueryOver<T> queryOver) {
            queryOver.ShouldNotBeNull("queryOver");
            return Inner.FindOne(queryOver);
        }

        /// <summary>
        /// Get unique entity by named query which defined mapping file. if one more entity exists, throw exception
        /// </summary>
        /// <param name="namedQuery">name of NamedQuery</param>
        /// <param name="parameters">parameters</param>
        /// <returns>a single instance that matches the query, or null if the query returns no results.</returns>
        public static T FindOne(string namedQuery, params INHParameter[] parameters) {
            namedQuery.ShouldNotBeWhiteSpace("namedQuery");
            return Inner.FindOne(namedQuery, parameters);
        }

        /// <summary>
        /// Find unique entity by example instance. if one more entity exists, throw exception
        /// </summary>
        /// <param name="exampleInstance">instance of exampling</param>
        /// <param name="propertyNamesToExclude">prpoerty names to exclude when matching example</param>
        /// <returns>a single instance that matches the query, or null if the query returns no results.</returns>
        public static T FindOne(T exampleInstance, params string[] propertyNamesToExclude) {
            return Inner.FindOne(exampleInstance, propertyNamesToExclude);
        }

        /// <summary>
        /// Get unique entity by hql
        /// </summary>
        /// <param name="query">hql string</param>
        /// <param name="parameters">named parameters</param>
        /// <returns>a single instance that matches the query, or null if the query returns no results.</returns>
        public static T FindOneByHql(string query, params INHParameter[] parameters) {
            query.ShouldNotBeWhiteSpace("query");
            return Inner.FindOneByHql(query, parameters);
        }

        /// <summary>
        /// Entity의 속성명이 지정된 값과 같은 유일한 엔티티를 조회한다. (propertyName = value)
        /// </summary>
        /// <param name="propertyName">속성명</param>
        /// <param name="value">비교할 값</param>
        /// <returns>엔티티, 없으면 null 반환</returns>
        [Obsolete("FindOne(params Expression<Func<T, bool>>[] expressions) 를 사용하거나 INHRepository<T>.Query() 를 사용하세요.")]
        public static T FindOnePropertyEq(string propertyName, object value) {
            return Inner.FindOnePropertyEq(propertyName, value);
        }

        #endregion

        #region << FindFirst >>

        /// <summary>
        /// Get first entity by ordering
        /// </summary>
        /// <param name="orders">order by</param>
        /// <returns>if not exist, return null</returns>
        public static T FindFirst(Order[] orders) {
            return Inner.FindFirst(orders);
        }

        /// <summary>
        /// Get first entity by ordering
        /// </summary>
        /// <param name="orders">order by</param>
        /// <returns>if not exist, return null</returns>
        public static T FindFirst(INHOrder<T>[] orders) {
            return Inner.FindFirst(orders);
        }

        /// <summary>
        /// Get first entity matched with specified criteria (criteria is optional).
        /// </summary>
        /// <param name="criteria">The collection of ICriterion to look for.</param>
        /// <returns>if not exist, return null</returns>
        public static T FindFirst(ICriterion[] criteria) {
            return Inner.FindFirst(criteria);
        }

        /// <summary>
        /// Get first entity matched with specified criteria (criteria is optional).
        /// </summary>
        /// <param name="expressions">The collection of Lambda expression to look for.</param>
        /// <returns>if not exist, return null</returns>
        public static T FindFirst(params Expression<Func<T, bool>>[] expressions) {
            return Inner.FindFirst(expressions);
        }

        /// <summary>
        /// Get first entity matched with specified detached criteria (criteria is optional) and ordering
        /// </summary>
        /// <param name="criteria">where spec</param>
        /// <param name="orders">order by</param>
        /// <returns>if not exist, return null</returns>
        public static T FindFirst(DetachedCriteria criteria, params Order[] orders) {
            criteria.ShouldNotBeNull("criteria");
            return Inner.FindFirst(criteria, orders);
        }

        /// <summary>
        /// Get first entity matched with specified detached criteria (criteria is optional) and ordering
        /// </summary>
        /// <param name="queryOver">where spec</param>
        /// <param name="orders">order by</param>
        /// <returns>if not exist, return null</returns>
        public static T FindFirst(QueryOver<T> queryOver, params INHOrder<T>[] orders) {
            queryOver.ShouldNotBeNull("queryOver");
            return Inner.FindFirst(queryOver, orders);
        }

        /// <summary>
        /// Get first entity from NamedQuery
        /// </summary>
        /// <param name="namedQuery">NamedQuery to look for</param>
        /// <param name="parameters">HQL Parameters</param>
        /// <returns>if not exist, return null</returns>
        public static T FindFirst(string namedQuery, params INHParameter[] parameters) {
            namedQuery.ShouldNotBeWhiteSpace("namedQuery");
            return Inner.FindFirst(namedQuery, parameters);
        }

        /// <summary>
        /// Get first entity matches with exampleInstance by Exampling.
        /// </summary>
        /// <param name="exampleInstance">instance for Exampling</param>
        /// <param name="propertyNamesToExclude">excluded property name for Exampling</param>
        /// <returns>if not exist, return null</returns>
        public static T FindFirst(T exampleInstance, params string[] propertyNamesToExclude) {
            exampleInstance.ShouldNotBeNull("exampleInstance");
            return Inner.FindFirst(exampleInstance, propertyNamesToExclude);
        }

        /// <summary>
        /// Get the first entity by Hql
        /// </summary>
        /// <param name="queryString">hql string</param>
        /// <param name="parameters">named parameters</param>
        /// <returns>first entity in retrieved entity collection. if not exists, return null</returns>
        public static T FindFirstByHql(string queryString, params INHParameter[] parameters) {
            queryString.ShouldNotBeWhiteSpace("queryString");

            return Inner.FindFirstByHql(queryString, parameters);
        }

        /// <summary>
        /// Entity의 속성명이 지정된 값과 같은 첫번째 엔티티를 조회한다. (propertyName = value)
        /// </summary>
        /// <param name="propertyName">속성명</param>
        /// <param name="value">비교할 값</param>
        /// <returns>엔티티, 없으면 null 반환</returns>
        [Obsolete("FindFirst(params Expression<Func<T, bool>>[] expressions) 를 사용하거나 INHRepository<T>.Query() 를 사용하세요.")]
        public static T FindFirstPropertyEq(string propertyName, object value) {
            propertyName.ShouldNotBeWhiteSpace("propertyName");
            return Inner.FindFirstPropertyEq(propertyName, value);
        }

        #endregion

        #region << Count >>

        /// <summary>
        /// Counts the overall number of entities.
        /// </summary>
        /// <returns>count of entities</returns>
        public static long Count() {
            return Inner.Count();
        }

        /// <summary>
        /// Counts the number of instances matching the criteria
        /// </summary>
        /// <param name="criteria">The criteria to look for</param>
        /// <returns>count of entities</returns>
        public static long Count(DetachedCriteria criteria) {
            criteria.ShouldNotBeNull("criteria");
            return Inner.Count(criteria);
        }

        /// <summary>
        /// Counts the number of instances matching the criteria
        /// </summary>
        /// <param name="criteria">The collection of ICriterion to look for</param>
        /// <returns>count of entities</returns>
        public static long Count(ICriterion[] criteria) {
            return Inner.Count(criteria);
        }

        /// <summary>
        /// Counts the number of instances matching the criteria
        /// </summary>
        /// <param name="queryOver">The criteria to look for</param>
        /// <returns>count of entities</returns>
        public static long Count(QueryOver<T> queryOver) {
            return Inner.Count(queryOver);
        }

        /// <summary>
        /// Counts the number of instances matching the criteria
        /// </summary>
        /// <param name="expressions">The collection of Lambda expression to look for</param>
        /// <returns>count of entities</returns>
        public static long Count(params Expression<Func<T, bool>>[] expressions) {
            return Inner.Count(expressions);
        }

        /// <summary>
        /// Counts the number of instances matching the criteria
        /// </summary>
        /// <param name="queryOver">The criteria to look for</param>
        /// <returns>count of entities</returns>
        public static long CountAsLong(QueryOver<T> queryOver) {
            return Inner.CountAsLong(queryOver);
        }

        /// <summary>
        /// Counts the number of instances matching the criteria
        /// </summary>
        /// <param name="expressions">The collection of Lambda expression to look for</param>
        /// <returns>count of entities</returns>
        public static long CountAsLong(params Expression<Func<T, bool>>[] expressions) {
            return Inner.CountAsLong(expressions);
        }

        /// <summary>
        /// Counts the number of instances matching the criteria
        /// </summary>
        /// <param name="queryOver">The criteria to look for</param>
        /// <returns>count of entities</returns>
        public static int CountAsInt(QueryOver<T> queryOver) {
            return Inner.CountAsInt(queryOver);
        }

        /// <summary>
        /// Counts the number of instances matching the criteria
        /// </summary>
        /// <param name="expressions">The collection of Lambda expression to look for</param>
        /// <returns>count of entities</returns>
        public static int CountAsInt(params Expression<Func<T, bool>>[] expressions) {
            return Inner.CountAsInt(expressions);
        }

        #endregion

        #region << Exists >>

        /// <summary>
        /// Check if any instance of the type exists
        /// </summary>
        /// <returns>true if an instance is found, otherwise false.</returns>
        public static bool Exists() {
            return Inner.Exists();
        }

        /// <summary>
        /// Check if any instance matches with the specified criteria
        /// </summary>
        /// <param name="criteria">The criteria to looking for</param>
        /// <returns>true if an instance is found, otherwise false.</returns>
        public static bool Exists(DetachedCriteria criteria) {
            return Inner.Exists(criteria);
        }

        /// <summary>
        /// Check if any instance matches with the specified criteria
        /// </summary>
        /// <param name="queryOver">The criteria to looking for</param>
        /// <returns>true if an instance is found, otherwise false.</returns>
        public static bool Exists(QueryOver<T> queryOver) {
            return Inner.Exists(queryOver);
        }

        /// <summary>
        /// Check if any instance matches with the specified criteria
        /// </summary>
        /// <param name="criteria">Collection of ICriterion</param>
        /// <returns>true if an instance is found, otherwise false.</returns>
        public static bool Exists(ICriterion[] criteria) {
            return Inner.Exists(criteria);
        }

        /// <summary>
        /// Check if any instance matches with the specified criteria
        /// </summary>
        /// <param name="expressions">The collection of Lambda expression to look for</param>
        /// <returns>true if an instance is found, otherwise false.</returns>
        public static bool Exists(params Expression<Func<T, bool>>[] expressions) {
            return Inner.Exists(expressions);
        }

        /// <summary>
        /// Check if any instance matches with the specified named query
        /// </summary>
        /// <param name="namedQuery">named query for looking for</param>
        /// <param name="parameters">parameters</param>
        /// <returns>true if an instance is found, otherwise false.</returns>
        public static bool Exists(string namedQuery, params INHParameter[] parameters) {
            namedQuery.ShouldNotBeWhiteSpace("namedQuery");
            return Inner.Exists(namedQuery, parameters);
        }

        /// <summary>
        /// Check if any instance matches with the specified simple query string
        /// </summary>
        /// <param name="queryString">queryString for looking for</param>
        /// <param name="parameters">parameters</param>
        /// <returns>true if an instance is found, otherwise false.</returns>
        public static bool ExistsByHql(string queryString, params INHParameter[] parameters) {
            queryString.ShouldNotBeWhiteSpace("queryString");
            return Inner.ExistsByHql(queryString, parameters);
        }

        /// <summary>
        /// Is entity exists that's is the specified identity. (별 필요없지 않나?)
        /// </summary>
        /// <returns>true if an instance is found, otherwise false.</returns>
        public static bool ExistsById(object id) {
            id.ShouldNotBeNull("id");
            return Inner.ExistsById(id);
        }

        #endregion

        #region << ReportAll >>

        /// <summary>
        /// Create the projects of type <typeparamref name="TProject"/> (ie DataTransferObject(s)) that satisfies the criteria supplied.
        /// </summary>
        /// <typeparam name="TProject">the type returned. (ie DTO)</typeparam>
        /// <param name="projectionList">Maps the properties from the object graph to the DTO</param>
        /// <returns>The projection result (DTO's) built from the object graph</returns>
        public static IList<TProject> ReportAll<TProject>(ProjectionList projectionList) {
            projectionList.ShouldNotBeNull("projectionList");
            return Inner.ReportAll<TProject>(projectionList);
        }

        /// <summary>
        /// Create the projects of type <typeparamref name="TProject"/> (ie DataTransferObject(s)) that satisfies the criteria supplied.
        /// </summary>
        /// <typeparam name="TProject">the type returned. (ie DTO)</typeparam>
        /// <param name="projectionList">Maps the properties from the object graph to the DTO</param>
        /// <param name="distinctResult">Indicate projection is distinctly search.</param>
        /// <returns>The projection result (DTO's) built from the object graph</returns>
        public static IList<TProject> ReportAll<TProject>(ProjectionList projectionList, bool distinctResult) {
            projectionList.ShouldNotBeNull("projectionList");
            return Inner.ReportAll<TProject>(projectionList, distinctResult);
        }

        /// <summary>
        /// Create the projects of type <typeparamref name="TProject"/> (ie DataTransferObject(s)) that satisfies the criteria supplied.
        /// </summary>
        /// <typeparam name="TProject">the type returned. (ie DTO)</typeparam>
        /// <param name="projectionList">Maps the properties from the object graph to the DTO</param>
        /// <param name="orders">The fields the repository should order by</param>
        /// <returns>The projection result (DTO's) built from the object graph</returns>
        public static IList<TProject> ReportAll<TProject>(ProjectionList projectionList, Order[] orders) {
            projectionList.ShouldNotBeNull("projectionList");
            return Inner.ReportAll<TProject>(projectionList, orders);
        }

        /// <summary>
        /// Create the projects of type <typeparamref name="TProject"/> (ie DataTransferObject(s)) that satisfies the criteria supplied.
        /// </summary>
        /// <typeparam name="TProject">the type returned. (ie DTO)</typeparam>
        /// <param name="projectionList">Maps the properties from the object graph satisfying <paramref name="criteria"/> to the DTO</param>
        /// <param name="criteria">The collectio of ICriterion to look for</param>
        /// <returns>The projection result (DTO's) built from the object graph satisfying <paramref name="criteria"/></returns>
        /// <remarks>
        /// The intent is for <paramref name="criteria"/> to be based (rooted)
        /// on <typeparamref name="T"/>. This is not enforced but is a
        /// convention that should be followed
        /// </remarks>
        public static IList<TProject> ReportAll<TProject>(ProjectionList projectionList, ICriterion[] criteria) {
            projectionList.ShouldNotBeNull("projectionList");
            return Inner.ReportAll<TProject>(projectionList, criteria);
        }

        /// <summary>
        /// Create the projects of type <typeparamref name="TProject"/> (ie DataTransferObject(s)) that satisfies the criteria supplied.
        /// </summary>
        /// <typeparam name="TProject">the type returned. (ie DTO)</typeparam>
        /// <param name="projectionList">Maps the properties from the object graph satisfying <paramref name="criteria"/> to the DTO</param>
        /// <param name="orders">The fields the repository should order by</param>
        /// <param name="criteria">The collectio of ICriterion to look for</param>
        /// <returns>The projection result (DTO's) built from the object graph satisfying <paramref name="criteria"/></returns>
        /// <remarks>
        /// The intent is for <paramref name="criteria"/> to be based (rooted)
        /// on <typeparamref name="T"/>. This is not enforced but is a
        /// convention that should be followed
        /// </remarks>
        public static IList<TProject> ReportAll<TProject>(ProjectionList projectionList, Order[] orders, params ICriterion[] criteria) {
            projectionList.ShouldNotBeNull("projectionList");
            return Inner.ReportAll<TProject>(projectionList, orders, criteria);
        }

        /// <summary>
        /// Create the projects of type <typeparamref name="TProject"/> (ie DataTransferObject(s)) that satisfies the criteria supplied.
        /// </summary>
        /// <typeparam name="TProject">the type returned. (ie DTO)</typeparam>
        /// <param name="projectionList">Maps the properties from the object graph satisfying <paramref name="expressions"/> to the DTO</param>
        /// <param name="expressions">The collection of Lambda expression to look for</param>
        /// <returns>The projection result (DTO's) built from the object graph satisfying <paramref name="expressions"/></returns>
        /// <remarks>
        /// The intent is for <paramref name="expressions"/> to be based (rooted)
        /// on <typeparamref name="T"/>. This is not enforced but is a
        /// convention that should be followed
        /// </remarks>
        public static IList<TProject> ReportAll<TProject>(ProjectionList projectionList, params Expression<Func<T, bool>>[] expressions) {
            projectionList.ShouldNotBeNull("projectionList");
            return Inner.ReportAll<TProject>(projectionList, expressions);
        }

        /// <summary>
        /// Create the projects of type <typeparamref name="TProject"/> (ie DataTransferObject(s)) that satisfies the criteria supplied.
        /// </summary>
        /// <typeparam name="TProject">the type returned. (ie DTO)</typeparam>
        /// <param name="projectionList">Maps the properties from the object graph satisfying <paramref name="expressions"/> to the DTO</param>
        /// <param name="orders">The fields the repository should order by</param>
        /// <param name="expressions">The collection of Lambda expression to look for</param>
        /// <returns>The projection result (DTO's) built from the object graph satisfying <paramref name="expressions"/></returns>
        /// <remarks>
        /// The intent is for <paramref name="expressions"/> to be based (rooted)
        /// on <typeparamref name="T"/>. This is not enforced but is a
        /// convention that should be followed
        /// </remarks>
        public static IList<TProject> ReportAll<TProject>(ProjectionList projectionList, INHOrder<T>[] orders,
                                                          params Expression<Func<T, bool>>[] expressions) {
            projectionList.ShouldNotBeNull("projectionList");
            return Inner.ReportAll<TProject>(projectionList, orders, expressions);
        }

        /// <summary>
        /// Create the projects of type <typeparamref name="TProject"/> (ie DataTransferObject(s)) that satisfies the criteria supplied.
        /// </summary>
        /// <typeparam name="TProject">the type returned. (ie DTO)</typeparam>
        /// <param name="projectionList">Maps the properties from the object graph satisfying <paramref name="criteria"/> to the DTO</param>
        /// <param name="criteria">The criteria to look for</param>
        /// <param name="orders">The fields the repository should order by</param>
        /// <returns>The projection result (DTO's) built from the object graph satisfying <paramref name="criteria"/></returns>
        /// <remarks>
        /// The intent is for <paramref name="criteria"/> to be based (rooted)
        /// on <typeparamref name="T"/>. This is not enforced but is a
        /// convention that should be followed
        /// </remarks>
        public static IList<TProject> ReportAll<TProject>(DetachedCriteria criteria, ProjectionList projectionList,
                                                          params Order[] orders) {
            criteria.ShouldNotBeNull("criteria");
            projectionList.ShouldNotBeNull("projectionList");

            return Inner.ReportAll<TProject>(criteria, projectionList, orders);
        }

        /// <summary>
        /// Create the projects of type <typeparamref name="TProject"/> (ie DataTransferObject(s)) that satisfies the criteria supplied.
        /// </summary>
        /// <typeparam name="TProject">the type returned. (ie DTO)</typeparam>
        /// <param name="projectionList">Maps the properties from the object graph satisfying <paramref name="queryOver"/> to the DTO</param>
        /// <param name="queryOver">The criteria to look for</param>
        /// <param name="orders">The fields the repository should order by</param>
        /// <returns>The projection result (DTO's) built from the object graph satisfying <paramref name="queryOver"/></returns>
        /// <remarks>
        /// The intent is for <paramref name="queryOver"/> to be based (rooted)
        /// on <typeparamref name="T"/>. This is not enforced but is a
        /// convention that should be followed
        /// </remarks>
        public static IList<TProject> ReportAll<TProject>(QueryOver<T> queryOver, ProjectionList projectionList,
                                                          params INHOrder<T>[] orders) {
            queryOver.ShouldNotBeNull("queryOver");
            projectionList.ShouldNotBeNull("projectionList");

            return Inner.ReportAll<TProject>(queryOver, projectionList, orders);
        }

        /// <summary>
        /// Create the projects of type <typeparamref name="TProject"/> (ie DataTransferObject(s)) that satisfies the criteria supplied.
        /// </summary>
        /// <typeparam name="TProject">the type returned. (ie DTO)</typeparam>
        /// <param name="namedQuery"></param>
        /// <param name="parameters"></param>
        /// <returns>The projection result (DTO's) built from the object graph satisfying <paramref name="namedQuery"/></returns>
        /// <returns>collection of <typeparamref name="TProject"/></returns>
        public static IList<TProject> ReportAll<TProject>(string namedQuery, params INHParameter[] parameters) {
            namedQuery.ShouldNotBeWhiteSpace("namedQuery");

            return Inner.ReportAll<TProject>(namedQuery, parameters);
        }

        #endregion

        #region << ReportOne >>

        /// <summary>
        /// Create the project of type <typeparamref name="TProject"/>(ie a Data Transfer Object) that satisfies the criteria supplied.
        /// Throws a NHibernate.NonUniqueResultException if there is more than one 
        /// </summary>
        /// <typeparam name="TProject">the type returned. (ie DTO)</typeparam>
        /// <param name="criteria">The criteria to look for</param>
        /// <param name="projectionList">
        /// Maps the properties from the object graph satisfiying <paramref name="criteria"/> to the DTO.
        /// </param>
        /// <returns>return DTO or null. if not unique, raise exception</returns>
        /// <remarks>
        /// The intent is for <paramref name="criteria"/> to be based (rooted)
        /// on <typeparamref name="T"/>. This is not enforced but is a
        /// convention that should be followed
        /// </remarks>
        public static TProject ReportOne<TProject>(DetachedCriteria criteria, ProjectionList projectionList) {
            criteria.ShouldNotBeNull("criteria");
            projectionList.ShouldNotBeNull("projectionList");

            return Inner.ReportOne<TProject>(criteria, projectionList);
        }

        /// <summary>
        /// Create the project of type <typeparamref name="TProject"/>(ie a Data Transfer Object) that satisfies the criteria supplied.
        /// Throws a NHibernate.NonUniqueResultException if there is more than one 
        /// </summary>
        /// <typeparam name="TProject">the type returned. (ie DTO)</typeparam>
        /// <param name="queryOver">The criteria to look for</param>
        /// <param name="projectionList">
        /// Maps the properties from the object graph satisfiying <paramref name="queryOver"/> to the DTO.
        /// </param>
        /// <returns>return DTO or null. if not unique, raise exception</returns>
        /// <remarks>
        /// The intent is for <paramref name="queryOver"/> to be based (rooted)
        /// on <typeparamref name="T"/>. This is not enforced but is a
        /// convention that should be followed
        /// </remarks>
        public static TProject ReportOne<TProject>(QueryOver<T> queryOver, ProjectionList projectionList) {
            queryOver.ShouldNotBeNull("queryOver");
            projectionList.ShouldNotBeNull("projectionList");

            return Inner.ReportOne<TProject>(queryOver, projectionList);
        }

        /// <summary>
        /// Create the project of type <typeparamref name="TProject"/>(ie a Data Transfer Object) that satisfies the criteria supplied.
        /// Throws a NHibernate.NonUniqueResultException if there is more than one 
        /// </summary>
        /// <typeparam name="TProject">the type returned. (ie DTO)</typeparam>
        /// <param name="projectionList">
        /// Maps the properties from the object graph satisfiying <paramref name="criteria"/> to the DTO.
        /// </param>
        /// <param name="criteria">The collection of ICriterion to look for</param>
        /// <returns>return DTO or null. if not unique, raise exception</returns>
        /// <remarks>
        /// The intent is for <paramref name="criteria"/> to be based (rooted)
        /// on <typeparamref name="T"/>. This is not enforced but is a
        /// convention that should be followed
        /// </remarks>
        public static TProject ReportOne<TProject>(ProjectionList projectionList, ICriterion[] criteria) {
            projectionList.ShouldNotBeNull("projectionList");

            return Inner.ReportOne<TProject>(projectionList, criteria);
        }

        /// <summary>
        /// Create the project of type <typeparamref name="TProject"/>(ie a Data Transfer Object) that satisfies the criteria supplied.
        /// Throws a NHibernate.NonUniqueResultException if there is more than one 
        /// </summary>
        /// <typeparam name="TProject">the type returned. (ie DTO)</typeparam>
        /// <param name="projectionList">
        /// Maps the properties from the object graph satisfiying <paramref name="expressions"/> to the DTO.
        /// </param>
        /// <param name="expressions">The collection of Lambda expression to look for</param>
        /// <returns>return DTO or null. if not unique, raise exception</returns>
        /// <remarks>
        /// The intent is for <paramref name="expressions"/> to be based (rooted)
        /// on <typeparamref name="T"/>. This is not enforced but is a
        /// convention that should be followed
        /// </remarks>
        public static TProject ReportOne<TProject>(ProjectionList projectionList, params Expression<Func<T, bool>>[] expressions) {
            projectionList.ShouldNotBeNull("projectionList");

            return Inner.ReportOne<TProject>(projectionList, expressions);
        }

        #endregion

        #region << Persist  Save / SaveOrUpdate / Update >>

        /// <summary>
        /// Transient Object를 Persistent Object로 만듭니다. 즉 Save합니다!!!
        /// </summary>
        /// <param name="entity">저장할 엔티티</param>
        /// <returns></returns>
        public static T Persist(T entity) {
            return Inner.Persist(entity);
        }

        /// <summary>
        /// save entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>The generated identifier</returns>
        public static T Save(T entity) {
            return Inner.Save(entity);
        }

        /// <summary>
        /// Save entity with Identity
        /// </summary>
        /// <param name="entity">entity to save or update</param>
        /// <param name="id">identity value of entity</param>
        public static void Save(T entity, object id) {
            Inner.Save(entity, id);
        }

        /// <summary>
        /// Save or Update entity
        /// </summary>
        /// <param name="entity">entity to save or update</param>
        /// <returns>saved or updated entity</returns>
        public static T SaveOrUpdate(T entity) {
            return Inner.SaveOrUpdate(entity);
        }

        /// <summary>
        /// Save or Update and Copy 
        /// </summary>
        /// <param name="entity">entity to save or update</param>
        /// <returns>an saved / updated entity</returns>
        [Obsolete("Merge()를 사용하세요")]
        public static T SaveOrUpdateCopy(T entity) {
            return Inner.SaveOrUpdateCopy(entity);
        }

        /// <summary>
        /// Save or Update and Copy 
        /// </summary>
        /// <param name="entity">entity</param>
        /// <param name="id">identity value of entity</param>
        /// <returns>an saved / updated entity</returns>
        [Obsolete("Merge()를 사용하세요")]
        public static T SaveOrUpdateCopy(T entity, object id) {
            return Inner.SaveOrUpdateCopy(entity, id);
        }

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity">entity to update</param>
        public static void Update(T entity) {
            Inner.Update(entity);
        }

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity">entity to update</param>
        /// <param name="id">identity value of entity to update</param>
        public static void Update(T entity, object id) {
            Inner.Update(entity, id);
        }

        #endregion

        #region << Merge / Replication >>

        /// <summary>
        /// SaveOrUpdate와는 달리 First Session에 이미 캐시되어 있는 엔티티라면, 최신 값으로 반영한 후 Save/Update를 수행한다.
        /// SaveOrUpdate는 Interceptor에서 엔티티 속성 값이 null로 바뀌는 문제가 있는 반면 Merge는 그렇지 않다.
        /// </summary>
        /// <param name="entity"></param>
        public static void Merge(T entity) {
            Inner.Merge(entity);
        }

        /// <summary>
        /// 다른 SessionFactory에 있는 엔티티를 현재 SessionFactory로 엔티티를 복제한다.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="replicationMode"></param>
        public static void Replicate(T entity, ReplicationMode replicationMode) {
            Inner.Replicate(entity, replicationMode ?? ReplicationMode.Overwrite);
        }

        #endregion

        #region << Delete / DeleteAll >>

        /// <summary>
        /// delete specified entity
        /// </summary>
        /// <param name="entity">entity to delete</param>
        public static void Delete(T entity) {
            Inner.Delete(entity);
        }

        /// <summary>
        /// delete entity by identity value
        /// </summary>
        /// <param name="id">Identity value of entity to delete</param>
        /// <param name="lockMode">Lock mode</param>
        public static void Delete(object id, LockMode lockMode) {
            Inner.Delete(id, lockMode);
        }

        /// <summary>
        /// delete all entities using session - ExecuteUpdate("delete EntityName"); 을 사용하세요.
        /// </summary>
        public static void DeleteAll() {
            Inner.DeleteAll();
        }

        /// <summary>
        /// delete entities matched with specified detached criteria
        /// </summary>
        /// <param name="criteria">The criteria to look for deleting</param>
        public static void DeleteAll(DetachedCriteria criteria) {
            Inner.DeleteAll(criteria);
        }

        /// <summary>
        /// delete entities matched with specified detached criteria
        /// </summary>
        /// <param name="queryOver">The criteria to look for deleting</param>
        public static void DeleteAll(QueryOver<T> queryOver) {
            Inner.DeleteAll(queryOver);
        }

        /// <summary>
        /// Criterion에 해당하는 모든 엔티티를 삭제한다.
        /// </summary>
        /// <param name="criterions"></param>
        public static void DeleteAll(ICriterion[] criterions) {
            Inner.DeleteAll(criterions);
        }

        /// <summary>
        /// <paramref name="expressions"/>에 해당하는 모든 엔티티를 삭제한다. 복수의 조건은 AND 조건이다.
        /// </summary>
        public static void DeleteAll(Expression<Func<T, bool>>[] expressions) {
            Inner.DeleteAll(expressions);
        }

        /// <summary>
        /// 지정된 Parameter에 해당하는 모든 엔티티를 삭제한다. 복수의 조건은 AND 조건이다.
        /// </summary>
        /// <param name="parameters"></param>
        public static void DeleteAll(INHParameter[] parameters) {
            Inner.DeleteAll(parameters);
        }

        /// <summary>
        /// Delete entities by Named Query (defined at hbm.xml)
        /// </summary>
        /// <param name="namedQuery">named query to look for deleting</param>
        /// <param name="parameters">parameters</param>
        public static void DeleteAll(string namedQuery, params INHParameter[] parameters) {
            namedQuery.ShouldNotBeWhiteSpace("namedQuery");

            Inner.DeleteAll(namedQuery, parameters);
        }

        /// <summary>
        /// Delete entities by HQL 
        /// </summary>
        /// <param name="queryString">HQL string to look for deleting</param>
        /// <param name="parameters">parameters</param>
        public static void DeleteAllByHql(string queryString, params INHParameter[] parameters) {
            queryString.ShouldNotBeWhiteSpace("queryString");

            Inner.DeleteAllByHql(queryString);
        }

        #endregion

        #region << ExecuteUpdate / ExecuteUpdateTransactional >>

        /// <summary>
        /// Execute DML-Style hql with parameters with NH 2.1.0 or higher
        /// </summary>
        /// <remarks>
        /// DML-style statements (insert, update, delete) 를 Session Memory 영역을 사용하지 않고, 직접 DB에 적용하여 성능을 향상 시킨다.
        /// 제약사항은 Hibernate Reference 를 참고할 것 (13.4 DML-style operations)<br/>
        /// <br/>
        /// NHibernate configuration에 다음과 같이 설정을 추가해야 한다.<br/>
        /// &lt;!-- DML-style operations : delete/insert/update/bulk copy 시에 hql을 이용할 수 있다. (예: delete Enitty)--&gt;
        /// &lt;property name="query.factory_class"&gt;NHibernate.Hql.Ast.ANTLR.ASTQueryTranslatorFactory, NHibernate&lt;/property&gt;
        /// </remarks>
        /// <param name="hql">HQL statement. ex: delete Parent p where exists (from p.Children)</param>
        /// <param name="parameters">named parameters</param>
        /// <returns>number of entities effected by this operation</returns>
        /// <seealso cref="IQuery.ExecuteUpdate"/>
        /// <example>
        /// <code>
        ///	// UPDATE or DELETE Statemement sample.
        ///	(UPDATE | DELETE) [VERSIONED] [FROM] EntityName [WHERE where_conditions)
        /// 
        /// // INSERT Statement
        /// INSERT INTO EntityName properties_list select_statement.
        /// // INSERT INTO ... SELECT ... 만 가능하고, 일반적인 SQL문장인 INSERT INTO ... VALUES ... 는 지원하지 않는다.
        /// 
        ///	// update customer
        ///	ExecuteUpdate("update Customer c set c.Name = :NewName where c.Name = :OldName", new NHParameter("OldName", "Debop", TypeFactory.GetStringType(255)), new NHParameter("NewName", "Sunghyouk", TypeFactory.GetStringType(255)));
        /// 
        /// // update versioned entity ( reset version of Customer )
        /// ExecuteUpdate("update versioned Customer c set c.Name = :NewName where c.Name = :OldName", new NHParameter("OldName", "Debop", TypeFactory.GetStringType(255)), new NHParameter("NewName", "Sunghyouk", TypeFactory.GetStringType(255));
        /// 
        /// // delete Customer
        /// ExecuteUpdate("delete Customer c where c.Name = :Name", new NHParameter("Name", "Debop", TypeFactory.GetStringType(255)));
        /// 
        /// 
        /// // insert Account
        /// ExecuteUpdate("insert into Account(Id, Name) select c.Id, c.Name from Customer c where c.Name=:Name", new NHParameter("Name", "Debop", TypeFactory.GetStringType(255)));
        /// </code>
        /// </example>
        public static int ExecuteUpdate(string hql, params INHParameter[] parameters) {
            hql.ShouldNotBeWhiteSpace("hql");

            return Inner.ExecuteUpdate(hql, parameters);
        }

        /// <summary>
        /// <see cref="ExecuteUpdate"/>와 같은 일을 하지만, Transaction을 적용하여, 작업한다.
        /// </summary>
        /// <param name="hql">HQL statement. ex: delete Parent p where exists (from p.Children)</param>
        /// <param name="parameters">named parameters</param>
        /// <returns>number of entities effected by this operation</returns>
        /// <seealso cref="ExecuteUpdate"/>
        public static int ExecuteUpdateTransactional(string hql, params INHParameter[] parameters) {
            hql.ShouldNotBeWhiteSpace("hql");

            return Inner.ExecuteUpdateTransactional(hql, parameters);
        }

        #endregion

        #region << Entity Management >>

        /// <summary>
        /// Create an new instance of <typeparamref name="T"/>, mapping it to the concrete class if needed
        /// </summary>
        /// <returns>new instance</returns>
        public static T Create() {
            return Inner.Create();
        }

        /// <summary>
        /// Get Entity metadata
        /// </summary>
        /// <returns>Metadata of T</returns>
        public static IClassMetadata GetClassMetadata() {
            return Inner.GetClassMetadata();
        }

        /// <summary>
        /// the specified instance is transient object ?
        /// </summary>
        /// <returns>if the specified entity is transient object, return true. otherwise return false.</returns>
        public static bool IsTransient(T entity) {
            return Inner.IsTransient(entity);
        }

        /// <summary>
        /// Create criteria for Entity
        /// </summary>
        /// <returns>instance of ICriteria for T</returns>
        public static ICriteria CreateCriteria() {
            return Inner.CreateCriteria();
        }

        /// <summary>
        /// Create detached criteria for Entity
        /// </summary>
        /// <returns>Instance of DetachedCriteria for T</returns>
        public static DetachedCriteria CreateDetachedCriteria() {
            return Inner.CreateDetachedCriteria();
        }

        /// <summary>
        /// Create an aliases <see cref="DetachedCriteria"/> compatible with current Dao instance.
        /// </summary>
        /// <param name="alias">alias</param>
        /// <returns>Instance of <see cref="DetachedCriteria"/> which has alias for T</returns>
        public static DetachedCriteria CreateDetachedCriteria(string alias) {
            return Inner.CreateDetachedCriteria(alias);
        }

        /// <summary>
        /// Create <see cref="IQuery"/> instance of current session with parameters.
        /// </summary>
        /// <param name="hql">HQL statement</param>
        /// <param name="parameters">named parameters</param>
        /// <returns></returns>
        public static IQuery CreateQuery(string hql, params INHParameter[] parameters) {
            hql.ShouldNotBeWhiteSpace("hql");
            return Inner.CreateQuery(hql, parameters);
        }

        /// <summary>
        /// IQueryOver 를 생성합니다.
        /// </summary>
        /// <returns></returns>
        public static IQueryOver<T, T> CreateQueryOver() {
            return Inner.CreateQueryOver();
        }

        /// <summary>
        /// IQueryOver 를 생성합니다.
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        public static IQueryOver<T, T> CreateQueryOver(Expression<Func<T>> alias) {
            return Inner.CreateQueryOver(alias);
        }

        /// <summary>
        ///  Detached QueryOver 생성 (<see cref="QueryOver.Of{T}()"/>)
        /// </summary>
        /// <returns></returns>
        public static QueryOver<T, T> CreateDetachedQueryOver() {
            return Inner.CreateQueryOverOf();
        }

        /// <summary>
        /// Detached QueryOver 생성 (<see cref="QueryOver.Of{T}()"/>)
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        public static QueryOver<T, T> CreateDetachedQueryOver(Expression<Func<T>> alias) {
            return Inner.CreateQueryOverOf(alias);
        }

        /// <summary>
        /// 엔티티 질의를 위해 LINQ의 <see cref="IQueryable{T}"/>를 반환합니다.
        /// </summary>
        public static IQueryable<T> Query() {
            return Inner.Query();
        }

        #endregion
    }
}