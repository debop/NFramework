using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Metadata;

namespace NSoft.NFramework.Data.NHibernateEx {
    /// <summary>
    /// NHibernate용 Repository의 기본 Interface
    /// </summary>
    /// <typeparam name="T">Type of Entity</typeparam>
    public interface INHRepository<T> where T : class {
        /// <summary>
        /// Type of Entity which handled by this Reposiotry
        /// </summary>
        Type ConcreteType { get; }

        /// <summary>
        /// Entity가 매핑된 Session Factory 인스턴스
        /// </summary>
        ISessionFactory SessionFactory { get; }

        /// <summary>
        /// UnitOfWork의 현재 Session 인스턴스
        /// </summary>
        ISession Session { get; }

        /// <summary>
        /// Ini 파일로부터 NHibernate HQL 문장을 제공하는 Provider
        /// </summary>
        IIniQueryProvider QueryProvider { get; }

        /// <summary>
        /// Get Entity by specified identity value or return nulll if it doesn't exist
        /// </summary>
        /// <param name="id"></param>
        /// <returns>if not exists, return null</returns>
        T Get(object id);

        /// <summary>
        /// Get Entity by specified identity value or return nulll if it doesn't exist
        /// </summary>
        /// <param name="id">identity of entity</param>
        /// <param name="lockMode">entity lock mode</param>
        /// <returns>if not exists, return null</returns>
        T Get(object id, LockMode lockMode);

        /// <summary>
        /// Load Entity by specified identity value or throw an exception if there isn't an entity that matches the specified id
        /// </summary>
        /// <param name="id">identity of entity</param>
        /// <returns>if not exists, exception occurred</returns>
        T Load(object id);

        /// <summary>
        /// Load Entity by specified identity value or throw an exception if there isn't an entity that matches the specified id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="lockMode">entity lock mode</param>
        /// <returns>if not exists, exception occurred</returns>
        T Load(object id, LockMode lockMode);

        /// <summary>
        /// 지정된 Identity 배열에 해당하는 모든 Entity를 로딩한다. (In 을 사용한다)
        /// </summary>
        /// <param name="ids">identity values</param>
        /// <returns>list of entity</returns>
        IList<T> GetIn(ICollection ids);

        /// <summary>
        /// 지정된 Id 컬렉션에 해당하는 모든 Entity를 로딩한다. (SQL 의 IN (xxx,yyy,zzz) 를 사용한다)
        /// </summary>
        /// <typeparam name="TId">Entity Id의 수형</typeparam>
        /// <param name="ids">Id값의 컬렉션</param>
        /// <returns>list of entity</returns>
        IList<T> GetInG<TId>(ICollection<TId> ids);

        /// <summary>
        /// Get paginated entity list
        /// </summary>
        /// <param name="pageIndex">Page index (start from 0)</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="expressions">expression for criteria</param>
        /// <returns>paginated list</returns>
        IPagingList<T> GetPage(int pageIndex, int pageSize, params Expression<Func<T, bool>>[] expressions);

        /// <summary>
        /// Get paginated entity list
        /// </summary>
        /// <param name="pageIndex">Page index (start from 0)</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="criterions">criteria</param>
        /// <returns>paginated list</returns>
        IPagingList<T> GetPage(int pageIndex, int pageSize, ICriterion[] criterions);

        /// <summary>
        /// Get paginated entity list
        /// </summary>
        /// <param name="pageIndex">Page index (start from 0)</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="orders">sort order</param>
        /// <param name="criterions">criteria</param>
        /// <returns>paginated list</returns>
        IPagingList<T> GetPage(int pageIndex, int pageSize, Order[] orders, params ICriterion[] criterions);

        /// <summary>
        /// Get paginated entity list
        /// </summary>
        /// <param name="pageIndex">Page index (start from 0)</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="orders">sort order</param>
        /// <param name="expressions">expression for criteria</param>
        /// <returns>paginated list</returns>
        IPagingList<T> GetPage(int pageIndex, int pageSize, INHOrder<T>[] orders, params Expression<Func<T, bool>>[] expressions);

        /// <summary>
        /// Get paginated entity list
        /// </summary>
        /// <param name="criteria">detached criteria</param>
        /// <param name="orders">sort order</param>
        /// <returns>pagenated list</returns>
        IPagingList<T> GetPage(DetachedCriteria criteria, params Order[] orders);

        /// <summary>
        /// Get paginated entity list
        /// </summary>
        /// <param name="pageIndex">Page index (start from 0)</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="criteria">detached criteria</param>
        /// <param name="orders">sort order</param>
        /// <returns>pagenated list</returns>
        IPagingList<T> GetPage(int pageIndex, int pageSize, DetachedCriteria criteria, params Order[] orders);

        /// <summary>
        /// Get paginated entity list
        /// </summary>
        /// <param name="orders">sort order</param>
        /// <returns>pagenated list</returns>
        IPagingList<T> GetPage(params INHOrder<T>[] orders);

        /// <summary>
        /// Get paginated entity list
        /// </summary>
        /// <param name="queryOver">detached QueryOver{T,T}</param>
        /// <param name="orders">sort order</param>
        /// <returns>pagenated list</returns>
        IPagingList<T> GetPage(QueryOver<T> queryOver, params INHOrder<T>[] orders);

        /// <summary>
        /// Get paginated entity list
        /// </summary>
        /// <param name="pageIndex">Page index (start from 0)</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="queryOver">detached QueryOver{T,T}</param>
        /// <param name="orders">sort order</param>
        /// <returns>pagenated list</returns>
        IPagingList<T> GetPage(int pageIndex, int pageSize, QueryOver<T> queryOver, params INHOrder<T>[] orders);

        /// <summary>
        /// Get a future entity from the persistance store, or return null
        /// if it doesn't exist.
        /// Note that the null will be there when you resolve the FutureValue.Value property
        /// </summary>
        /// <param name="id">identity value of entity</param>
        /// <returns>instance of future value</returns>
        [Obsolete("NHibernate 3.x의 Future 기능을 사용하세요")]
        IFutureValue<T> FutureGet(object id);

        /// <summary>
        /// A future of the entity loaded from the persistance store
        /// Will throw an exception if there isn't an entity that matches
        /// the id.
        /// </summary>
        /// <param name="id">identity value of entity.</param>
        /// <returns>The entity that matches the id</returns>
        [Obsolete("NHibernate 3.x의 Future 기능을 사용하세요")]
        IFutureValue<T> FutureLoad(object id);

        /// <summary>
        /// Get a future entity collection from the persistence store
        /// </summary>
        /// <param name="detachedCriteria"></param>
        /// <returns></returns>
        [Obsolete("NHibernate 3.x의 Future 기능을 사용하세요")]
        IFutureValue<T> FutureValue(DetachedCriteria detachedCriteria);

        /// <summary>
        /// Get a future entity collection from the persistence store
        /// </summary>
        [Obsolete("NHibernate 3.x의 Future 기능을 사용하세요")]
        IFutureValue<T> FutureValue(QueryOver<T> queryOver);

        /// <summary>
        /// Get a future entity collection from the persistence store
        /// </summary>
        [Obsolete("NHibernate 3.x의 Future 기능을 사용하세요")]
        IFutureValue<T> FutureValue(params Expression<Func<T, bool>>[] expressions);

        /// <summary>
        /// Get a future entity collection from the persistence store
        /// </summary>
        /// <param name="detachedCriteria"></param>
        /// <param name="orders"></param>
        /// <returns></returns>
        IEnumerable<T> Future(DetachedCriteria detachedCriteria, params Order[] orders);

        /// <summary>
        /// Get a future entity collection from the persistence store
        /// </summary>
        IEnumerable<T> Future(QueryOver<T> queryOver, params INHOrder<T>[] orders);

        /// <summary>
        /// Get a future entity collection from the persistence store
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> Future(Expression<Func<T, bool>>[] expressions, params INHOrder<T>[] orders);

        /// <summary>
        /// Get all entities.
        /// </summary>
        /// <returns>entity collection</returns>
        IList<T> FindAll();

        /// <summary>
        /// Get entities matched with criteria
        /// </summary>
        /// <param name="criterions">where</param>
        /// <returns>entity collection</returns>
        IList<T> FindAll(ICriterion[] criterions);

        /// <summary>
        /// Get ordered and ranged entities matched with criteria
        /// </summary>
        /// <param name="orders">ordering spec</param>
        /// <param name="criterions">where spec</param>
        /// <returns>entity collection</returns>
        IList<T> FindAll(Order[] orders, params ICriterion[] criterions);

        /// <summary>
        /// 지정된 criteria를 이용하여 정보를 조회합니다.
        /// </summary>
        /// <param name="firstResult">first index (start from 0)</param>
        /// <param name="maxResults">max resultset count (start from 1) (0이면 마지막 Record까지 읽어온다.)</param>
        /// <param name="orders">정렬 순서</param>
        /// <param name="criterions">조회 조건</param>
        /// <returns>collection of entity.</returns>
        IList<T> FindAll(Order[] orders, int firstResult, int maxResults, params ICriterion[] criterions);

        /// <summary>
        /// Get ranged entities matched with criteria
        /// </summary>
        /// <param name="firstResult">first index (start from 0)</param>
        /// <param name="maxResults">max resultset count (start from 1) (0이면 마지막 Record까지 읽어온다.)</param>
        /// <param name="criterions">where</param>
        /// <returns>entity collection</returns>
        IList<T> FindAll(int firstResult, int maxResults, ICriterion[] criterions);

        /// <summary>
        /// 지정된 criteria를 이용하여 정보를 조회합니다.
        /// </summary>
        /// <param name="firstResult">first index (start from 0)</param>
        /// <param name="maxResults">max resultset count (start from 1) (0이면 마지막 Record까지 읽어온다.)</param>
        /// <param name="orders">정렬 순서</param>
        /// <param name="criterions">조회 조건</param>
        /// <returns>collection of entity.</returns>
        IList<T> FindAll(int firstResult, int maxResults, Order[] orders, params ICriterion[] criterions);

        /// <summary>
        /// Get entities matched with criteria
        /// </summary>
        /// <param name="expression">expression</param>
        /// <param name="expressions">조건 표현식</param>
        /// <returns>entity collection</returns>
        IList<T> FindAll(Expression<Func<T, bool>> expression, params Expression<Func<T, bool>>[] expressions);

        /// <summary>
        /// Get ordered and ranged entities matched with criteria
        /// </summary>
        /// <param name="orders">ordering spec</param>
        /// <param name="expressions">조건 표현식</param>
        /// <returns>entity collection</returns>
        IList<T> FindAll(INHOrder<T>[] orders, params Expression<Func<T, bool>>[] expressions);

        /// <summary>
        /// 지정된 criteria를 이용하여 정보를 조회합니다.
        /// </summary>
        /// <param name="firstResult">first index (start from 0)</param>
        /// <param name="maxResults">max resultset count (start from 1) (0이면 마지막 Record까지 읽어온다.)</param>
        /// <param name="orders">정렬 순서</param>
        /// <param name="expressions">조건 표현식</param>
        /// <returns>collection of entity.</returns>
        IList<T> FindAll(INHOrder<T>[] orders, int firstResult, int maxResults, params Expression<Func<T, bool>>[] expressions);

        /// <summary>
        /// Get ranged entities matched with criteria
        /// </summary>
        /// <param name="firstResult">first index (start from 0)</param>
        /// <param name="maxResults">max resultset count (start from 1) (0이면 마지막 Record까지 읽어온다.)</param>
        /// <param name="expressions">조건 표현식</param>
        /// <returns>entity collection</returns>
        IList<T> FindAll(int firstResult, int maxResults, params Expression<Func<T, bool>>[] expressions);

        /// <summary>
        /// 지정된 criteria를 이용하여 정보를 조회합니다.
        /// </summary>
        /// <param name="firstResult">first index (start from 0)</param>
        /// <param name="maxResults">max resultset count (start from 1) (0이면 마지막 Record까지 읽어온다.)</param>
        /// <param name="orders">정렬 순서</param>
        /// <param name="expressions">조건 표현식</param>
        /// <returns>collection of entity.</returns>
        IList<T> FindAll(int firstResult, int maxResults, INHOrder<T>[] orders, params Expression<Func<T, bool>>[] expressions);

        /// <summary>
        /// Get ranged entites matched with detached criteria, ordering is optional
        /// </summary>
        /// <param name="criteria">where spec</param>
        /// <param name="orders">ordering spec</param>
        /// <returns>entity collection</returns>
        IList<T> FindAll(DetachedCriteria criteria, params Order[] orders);

        /// <summary>
        /// Get ranged entites matched with detached criteria, ordering is optional
        /// </summary>
        /// <param name="criteria">where spec</param>
        /// <param name="firstResult">first index (start from 0)</param>
        /// <param name="maxResults">max resultset count (start from 1)</param>
        /// <param name="orders">ordering spec</param>
        /// <returns>entity collection</returns>
        IList<T> FindAll(DetachedCriteria criteria, int firstResult, int maxResults, params Order[] orders);

        /// <summary>
        /// Get ordered and ranged entities matched with criteria
        /// </summary>
        /// <param name="queryOver">Detached QueryOver</param>
        /// <param name="orders">ordering spec</param>
        /// <returns>entity collection</returns>
        IList<T> FindAll(QueryOver<T> queryOver, params INHOrder<T>[] orders);

        /// <summary>
        /// Get ordered and ranged entities matched with criteria
        /// </summary>
        /// <param name="queryOver">Detached QueryOver</param>
        /// <param name="firstResult">first index (start from 0)</param>
        /// <param name="maxResults">max resultset count (start from 1)</param>
        /// <param name="orders">ordering spec</param>
        /// <returns>entity collection</returns>
        IList<T> FindAll(QueryOver<T> queryOver, int firstResult, int maxResults, params INHOrder<T>[] orders);

        /// <summary>
        /// Get entities by examping with exampleInstance
        /// </summary>
        /// <param name="exampleInstance">instance for exampling</param>
        /// <param name="firstResult">first index (start from 0)</param>
        /// <param name="maxResults">max resultset count (start from 1) (0이면 마지막 Record까지 가져온다.)</param>
        /// <param name="propertyNamesToExclude">excluded property for exampling</param>
        /// <returns>entity collection</returns>
        IList<T> FindAll(T exampleInstance, int firstResult, int maxResults, params string[] propertyNamesToExclude);

        /// <summary>
        /// Get entities by examping with exampleInstance
        /// </summary>
        /// <param name="exampleInstance">instance for exampling</param>
        /// <param name="propertyNamesToExclude">excluded property for exampling</param>
        /// <returns>entity collection</returns>
        IList<T> FindAll(T exampleInstance, params string[] propertyNamesToExclude);

        /// <summary>
        /// Get entities by Named Query which defined in mapping files (*.hbm.xml) ex: &lt;query name="xxxx"&gt;
        /// </summary>
        /// <param name="namedQuery">name of NamedQuery which defined in mapping files(*.hbm.xml)</param>
        /// <param name="firstResult">first index (start from 0)</param>
        /// <param name="maxResults">max resultset count (start from 1) (0이면 마지막 Record까지 가져온다.)</param>
        /// <param name="parameters">HQL Parameters</param>
        /// <returns>entity collection</returns>
        IList<T> FindAll(string namedQuery, int firstResult, int maxResults, params INHParameter[] parameters);

        /// <summary>
        /// Get entities by Named Query which defined in mapping files (*.hbm.xml) ex: &lt;query name="xxxx"&gt;
        /// </summary>
        /// <param name="namedQuery">name of NamedQuery which defined in mapping files(*.hbm.xml)</param>
        /// <param name="parameters">HQL Parameters</param>
        /// <returns>entity collection</returns>
        IList<T> FindAll(string namedQuery, params INHParameter[] parameters);

        /// <summary>
        /// NHibernate Query Language (HQL) 를 이용한 조회
        /// </summary>
        /// <param name="queryString">hql string</param>
        /// <param name="firstResult">first index (start from 0)</param>
        /// <param name="maxResults">max resultset count (start from 1) (0이면 마지막 Record까지 가져온다.)</param>
        /// <param name="parameters">HQL Parameters</param>
        /// <returns>entity collection</returns>
        IList<T> FindAllByHql(string queryString, int firstResult, int maxResults, params INHParameter[] parameters);

        /// <summary>
        /// NHibernate Query Language (HQL) 를 이용한 조회
        /// </summary>
        /// <param name="queryString">hql string</param>
        /// <param name="parameters">HQL Parameters</param>
        /// <returns>entity collection</returns>
        IList<T> FindAllByHql(string queryString, params INHParameter[] parameters);

        /// <summary>
        /// Entity의 속성명이 지정된 값과 같은 엔티티를 모두 조회한다. (propertyName = value)
        /// </summary>
        /// <param name="propertyName">속성명</param>
        /// <param name="value">비교할 값</param>
        /// <returns>entity collection</returns>
        [Obsolete("FindAll(params Expression<Func<T, bool>>[] expressions) 를 사용하거나 INHRepository<T>.Query() 를 사용하세요.")]
        IList<T> FindAllPropertyEq(string propertyName, object value);

        /// <summary>
        /// Get unique entity matches with specified criteria
        /// </summary>
        /// <param name="criterions">where spec</param>
        /// <returns>if not unique entity or not exists, raise exception</returns>
        T FindOne(ICriterion[] criterions);

        /// <summary>
        /// Get unique entity matches with specified criteria
        /// </summary>
        /// <param name="expressions">where spec</param>
        /// <returns>if not unique entity or not exists, raise exception</returns>
        T FindOne(params Expression<Func<T, bool>>[] expressions);

        /// <summary>
        /// Get unique entity matches with specified detached criteria
        /// </summary>
        /// <param name="criteria">where spec</param>
        /// <returns>if not unique entity or not exists, raise exception</returns>
        T FindOne(DetachedCriteria criteria);

        /// <summary>
        /// Get unique entity matches with specified detached criteria. if one more entity exists, throw exception
        /// </summary>
        /// <param name="queryOver">where spec</param>
        /// <returns>a single instance that matches the query, or null if the query returns no results.</returns>
        T FindOne(QueryOver<T> queryOver);

        /// <summary>
        /// Get unique entity by named query which defined mapping file
        /// </summary>
        /// <param name="namedQuery">name of NamedQuery</param>
        /// <param name="parameters">parameters</param>
        /// <returns>if not unique entity or not exists, raise exception</returns>
        T FindOne(string namedQuery, params INHParameter[] parameters);

        /// <summary>
        /// Find unique entity by example instance. if one more entity exists, throw exception
        /// </summary>
        /// <param name="exampleInstance">instance of exampling</param>
        /// <param name="propertyNamesToExclude">prpoerty names to exclude when matching example</param>
        /// <returns>if not unique entity or not exists, raise exception</returns>
        T FindOne(T exampleInstance, params string[] propertyNamesToExclude);

        /// <summary>
        /// Get unique entity by hql
        /// </summary>
        /// <param name="query">hql string</param>
        /// <param name="parameters">named parameters</param>
        /// <returns>if not unique entity or not exists, raise exception</returns>
        T FindOneByHql(string query, params INHParameter[] parameters);

        /// <summary>
        /// Entity의 속성명이 지정된 값과 같은 유일한 엔티티를 조회한다. (propertyName = value)
        /// </summary>
        /// <param name="propertyName">속성명</param>
        /// <param name="value">비교할 값</param>
        /// <returns>엔티티, 없으면 null 반환</returns>
        [Obsolete("FindOne(params Expression<Func<T, bool>>[] expressions) 를 사용하거나 INHRepository<T>.Query() 를 사용하세요.")]
        T FindOnePropertyEq(string propertyName, object value);

        /// <summary>
        /// Get first entity by ordering
        /// </summary>
        /// <param name="orders">order by</param>
        /// <returns>if not exist, return null</returns>
        T FindFirst(Order[] orders);

        /// <summary>
        /// Get first entity by ordering
        /// </summary>
        /// <param name="orders">order by</param>
        /// <returns>if not exist, return null</returns>
        T FindFirst(INHOrder<T>[] orders);

        /// <summary>
        /// Get first entity matched with specified criteria (criteria is optional).
        /// </summary>
        /// <param name="criterions">The collection of ICriterion to look for.</param>
        /// <returns>if not exist, return null</returns>
        T FindFirst(ICriterion[] criterions);

        /// <summary>
        /// Get first entity matched with specified criteria (criteria is optional).
        /// </summary>
        /// <param name="expressions">The collection of Lambda expression to look for.</param>
        /// <returns>if not exist, return null</returns>
        T FindFirst(params Expression<Func<T, bool>>[] expressions);

        /// <summary>
        /// Get first entity matched with specified detached criteria (criteria is optional) and ordering
        /// </summary>
        /// <param name="criteria">where spec</param>
        /// <param name="orders">order by</param>
        /// <returns>if not exist, return null</returns>
        T FindFirst(DetachedCriteria criteria, params Order[] orders);

        /// <summary>
        /// Get first entity matched with specified detached criteria (criteria is optional) and ordering
        /// </summary>
        /// <param name="queryOver">where spec</param>
        /// <param name="orders">order by</param>
        /// <returns>if not exist, return null</returns>
        T FindFirst(QueryOver<T> queryOver, params INHOrder<T>[] orders);

        /// <summary>
        /// Get first entity from NamedQuery
        /// </summary>
        /// <param name="namedQuery">NamedQuery to look for</param>
        /// <param name="parameters">HQL Parameters</param>
        /// <returns>if not exist, return null</returns>
        T FindFirst(string namedQuery, params INHParameter[] parameters);

        /// <summary>
        /// Get first entity matches with exampleInstance by Exampling.
        /// </summary>
        /// <param name="exampleInstance">instance for Exampling</param>
        /// <param name="propertyNamesToExclude">excluded property name for Exampling</param>
        /// <returns>if not exist, return null</returns>
        T FindFirst(T exampleInstance, params string[] propertyNamesToExclude);

        /// <summary>
        /// Get the first entity by Hql
        /// </summary>
        /// <param name="queryString">hql string</param>
        /// <param name="parameters">named parameters</param>
        /// <returns>first entity in retrieved entity collection. if not exists, return null</returns>
        T FindFirstByHql(string queryString, params INHParameter[] parameters);

        /// <summary>
        /// Entity의 속성명이 지정된 값과 같은 첫번째 엔티티를 조회한다. (propertyName = value)
        /// </summary>
        /// <param name="propertyName">속성명</param>
        /// <param name="value">비교할 값</param>
        /// <returns>엔티티, 없으면 null 반환</returns>
        [Obsolete("FindFirst(params Expression<Func<T, bool>>[] expressions) 를 사용하거나 INHRepository<T>.Query() 를 사용하세요.")]
        T FindFirstPropertyEq(string propertyName, object value);

        /// <summary>
        /// Counts the overall number of entities.
        /// </summary>
        /// <returns>count of entities</returns>
        long Count();

        /// <summary>
        /// Counts the number of instances matching the criteria
        /// </summary>
        /// <param name="criteria">The criteria to look for</param>
        /// <returns>count of entities</returns>
        long Count(DetachedCriteria criteria);

        /// <summary>
        /// Counts the number of instances matching the criteria
        /// </summary>
        /// <param name="criterions">The collection of ICriterion to look for</param>
        /// <returns>count of entities</returns>
        long Count(ICriterion[] criterions);

        /// <summary>
        /// Counts the number of instances matching the query
        /// </summary>
        /// <param name="queryOver">QueryOver to look for</param>
        /// <returns>count of entities</returns>
        long Count(QueryOver<T> queryOver);

        /// <summary>
        /// Counts the number of instances matching the expression.
        /// </summary>
        /// <param name="expressions">The collection of Lambda expression to look for.</param>
        /// <returns>if not exist, return null</returns>
        long Count(params Expression<Func<T, bool>>[] expressions);

        /// <summary>
        /// Counts the number of instances matching the criteria
        /// </summary>
        /// <param name="queryOver">The criteria to look for</param>
        /// <returns>count of entities</returns>
        long CountAsLong(QueryOver<T> queryOver);

        /// <summary>
        /// Counts the number of instances matching the criteria
        /// </summary>
        /// <param name="expressions">The collection of Lambda expression to look for</param>
        /// <returns>count of entities</returns>
        long CountAsLong(params Expression<Func<T, bool>>[] expressions);

        /// <summary>
        /// Counts the number of instances matching the criteria
        /// </summary>
        /// <param name="queryOver">The criteria to look for</param>
        /// <returns>count of entities</returns>
        int CountAsInt(QueryOver<T> queryOver);

        /// <summary>
        /// Counts the number of instances matching the criteria
        /// </summary>
        /// <param name="expressions">The collection of Lambda expression to look for</param>
        /// <returns>count of entities</returns>
        int CountAsInt(params Expression<Func<T, bool>>[] expressions);

        /// <summary>
        /// Check if any instance of the type exists
        /// </summary>
        /// <returns>true if an instance is found, otherwise false.</returns>
        bool Exists();

        /// <summary>
        /// Check if any instance matches with the specified criteria
        /// </summary>
        /// <param name="criteria">The criteria to looking for</param>
        /// <returns>true if an instance is found, otherwise false.</returns>
        bool Exists(DetachedCriteria criteria);

        /// <summary>
        /// Check if any instance matches with the specified criteria
        /// </summary>
        /// <param name="queryOver">The criteria to looking for</param>
        /// <returns>true if an instance is found, otherwise false.</returns>
        bool Exists(QueryOver<T> queryOver);

        /// <summary>
        /// Check if any instance matches with the specified criteria
        /// </summary>
        /// <param name="criterions">Collection of ICriterion</param>
        /// <returns>true if an instance is found, otherwise false.</returns>
        bool Exists(ICriterion[] criterions);

        /// <summary>
        /// Check if any instance matches with the specified criteria
        /// </summary>
        /// <param name="expressions">The collection of Lambda expression to look for</param>
        /// <returns>true if an instance is found, otherwise false.</returns>
        bool Exists(params Expression<Func<T, bool>>[] expressions);

        /// <summary>
        /// Check if any instance matches with the specified named query
        /// </summary>
        /// <param name="namedQuery">named query for looking for</param>
        /// <param name="parameters">parameters</param>
        /// <returns>true if an instance is found, otherwise false.</returns>
        bool Exists(string namedQuery, params INHParameter[] parameters);

        /// <summary>
        /// Check if any instance matches with the specified simple query string
        /// </summary>
        /// <param name="queryString">queryString for looking for</param>
        /// <param name="parameters">parameters</param>
        /// <returns>true if an instance is found, otherwise false.</returns>
        bool ExistsByHql(string queryString, params INHParameter[] parameters);

        /// <summary>
        /// Is entity exists that's is the specified identity. (별 필요없지 않나?)
        /// </summary>
        /// <returns>true if an instance is found, otherwise false.</returns>
        bool ExistsById(object id);

        /// <summary>
        /// Create the projects of type <typeparamref name="TProject"/> (ie DataTransferObject(s)) that satisfies the criteria supplied.
        /// </summary>
        /// <typeparam name="TProject">the type returned. (ie DTO)</typeparam>
        /// <param name="projectionList">Maps the properties from the object graph to the DTO</param>
        /// <returns>The projection result (DTO's) built from the object graph</returns>
        IList<TProject> ReportAll<TProject>(ProjectionList projectionList);

        /// <summary>
        /// Create the projects of type <typeparamref name="TProject"/> (ie DataTransferObject(s)) that satisfies the criteria supplied.
        /// </summary>
        /// <typeparam name="TProject">the type returned. (ie DTO)</typeparam>
        /// <param name="projectionList">Maps the properties from the object graph to the DTO</param>
        /// <param name="distinctResult">Indicate projection is distinctly search.</param>
        /// <returns>The projection result (DTO's) built from the object graph</returns>
        IList<TProject> ReportAll<TProject>(ProjectionList projectionList, bool distinctResult);

        /// <summary>
        /// Create the projects of type <typeparamref name="TProject"/> (ie DataTransferObject(s)) that satisfies the criteria supplied.
        /// </summary>
        /// <typeparam name="TProject">the type returned. (ie DTO)</typeparam>
        /// <param name="projectionList">Maps the properties from the object graph to the DTO</param>
        /// <param name="orders">The fields the repository should order by</param>
        /// <returns>The projection result (DTO's) built from the object graph</returns>
        IList<TProject> ReportAll<TProject>(ProjectionList projectionList, Order[] orders);

        /// <summary>
        /// Create the projects of type <typeparamref name="TProject"/> (ie DataTransferObject(s)) that satisfies the criteria supplied.
        /// </summary>
        /// <typeparam name="TProject">the type returned. (ie DTO)</typeparam>
        /// <param name="projectionList">Maps the properties from the object graph satisfying <paramref name="criterions"/> to the DTO</param>
        /// <param name="criterions">The collectio of ICriterion to look for</param>
        /// <returns>The projection result (DTO's) built from the object graph satisfying <paramref name="criterions"/></returns>
        /// <remarks>
        /// The intent is for <paramref name="criterions"/> to be based (rooted)
        /// on <typeparamref name="T"/>. This is not enforced but is a
        /// convention that should be followed
        /// </remarks>
        IList<TProject> ReportAll<TProject>(ProjectionList projectionList, ICriterion[] criterions);

        /// <summary>
        /// Create the projects of type <typeparamref name="TProject"/> (ie DataTransferObject(s)) that satisfies the criteria supplied.
        /// </summary>
        /// <typeparam name="TProject">the type returned. (ie DTO)</typeparam>
        /// <param name="projectionList">Maps the properties from the object graph satisfying <paramref name="criterions"/> to the DTO</param>
        /// <param name="orders">The fields the repository should order by</param>
        /// <param name="criterions">The collectio of ICriterion to look for</param>
        /// <returns>The projection result (DTO's) built from the object graph satisfying <paramref name="criterions"/></returns>
        /// <remarks>
        /// The intent is for <paramref name="criterions"/> to be based (rooted)
        /// on <typeparamref name="T"/>. This is not enforced but is a
        /// convention that should be followed
        /// </remarks>
        IList<TProject> ReportAll<TProject>(ProjectionList projectionList, Order[] orders, params ICriterion[] criterions);

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
        IList<TProject> ReportAll<TProject>(ProjectionList projectionList, params Expression<Func<T, bool>>[] expressions);

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
        IList<TProject> ReportAll<TProject>(ProjectionList projectionList, INHOrder<T>[] orders,
                                            params Expression<Func<T, bool>>[] expressions);

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
        IList<TProject> ReportAll<TProject>(DetachedCriteria criteria, ProjectionList projectionList, params Order[] orders);

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
        IList<TProject> ReportAll<TProject>(QueryOver<T> queryOver, ProjectionList projectionList, params INHOrder<T>[] orders);

        /// <summary>
        /// Create the projects of type <typeparamref name="TProject"/> (ie DataTransferObject(s)) that satisfies the criteria supplied.
        /// </summary>
        /// <typeparam name="TProject">the type returned. (ie DTO)</typeparam>
        /// <param name="namedQuery"></param>
        /// <param name="parameters"></param>
        /// <returns>The projection result (DTO's) built from the object graph satisfying <paramref name="namedQuery"/></returns>
        /// <returns>collection of <typeparamref name="TProject"/></returns>
        IList<TProject> ReportAll<TProject>(string namedQuery, params INHParameter[] parameters);

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
        TProject ReportOne<TProject>(DetachedCriteria criteria, ProjectionList projectionList);

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
        TProject ReportOne<TProject>(QueryOver<T> queryOver, ProjectionList projectionList);

        /// <summary>
        /// Create the project of type <typeparamref name="TProject"/>(ie a Data Transfer Object) that satisfies the criteria supplied.
        /// Throws a NHibernate.NonUniqueResultException if there is more than one 
        /// </summary>
        /// <typeparam name="TProject">the type returned. (ie DTO)</typeparam>
        /// <param name="projectionList">
        /// Maps the properties from the object graph satisfiying <paramref name="criterions"/> to the DTO.
        /// </param>
        /// <param name="criterions">The collection of ICriterion to look for</param>
        /// <returns>return DTO or null. if not unique, raise exception</returns>
        /// <remarks>
        /// The intent is for <paramref name="criterions"/> to be based (rooted)
        /// on <typeparamref name="T"/>. This is not enforced but is a
        /// convention that should be followed
        /// </remarks>
        TProject ReportOne<TProject>(ProjectionList projectionList, ICriterion[] criterions);

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
        TProject ReportOne<TProject>(ProjectionList projectionList, params Expression<Func<T, bool>>[] expressions);

        /// <summary>
        /// Transient Object를 Persistent Object로 만듭니다. 즉 Save합니다!!!
        /// </summary>
        /// <param name="entity">저장할 엔티티</param>
        /// <returns></returns>
        T Persist(T entity);

        /// <summary>
        /// save entity
        /// </summary>
        /// <param name="entity">entity to save</param>
        /// <returns>The generated identifier</returns>
        T Save(T entity);

        /// <summary>
        /// Save entity with Identity
        /// </summary>
        /// <param name="entity">entity to save or update</param>
        /// <param name="id">identity value of entity</param>
        /// <returns>Saved entity</returns>
        void Save(T entity, object id);

        /// <summary>
        /// Save or Update entity
        /// </summary>
        /// <param name="entity">entity to save or update</param>
        /// <returns>saved or updated entity</returns>
        T SaveOrUpdate(T entity);

        /// <summary>
        /// Save or Update and Copy 
        /// </summary>
        /// <param name="entity">entity to save or update</param>
        /// <returns>an saved / updated entity</returns>
        [Obsolete("Merge()를 사용하세요")]
        T SaveOrUpdateCopy(T entity);

        /// <summary>
        /// Save or Update and Copy 
        /// </summary>
        /// <param name="entity">entity</param>
        /// <param name="id">identity value of entity</param>
        /// <returns>an saved / updated entity</returns>
        [Obsolete("Merge()를 사용하세요")]
        T SaveOrUpdateCopy(T entity, object id);

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity">entity to update</param>
        void Update(T entity);

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity">entity to update</param>
        /// <param name="id">identity value of entity to update</param>
        void Update(T entity, object id);

        /// <summary>
        /// SaveOrUpdate와는 달리 First Session에 이미 캐시되어 있는 엔티티라면, 최신 값으로 반영한 후 Save/Update를 수행한다.
        /// SaveOrUpdate는 Interceptor에서 엔티티 속성 값이 null로 바뀌는 문제가 있는 반면 Merge는 그렇지 않다.
        /// </summary>
        /// <param name="entity">저장/갱신할 엔티티</param>
        void Merge(T entity);

        /// <summary>
        /// 다른 SessionFactory에 있는 현재 SessionFactory로 엔티티를 복제한다.
        /// </summary>
        /// <param name="entity">복제할 엔티티</param>
        /// <param name="replicateMode">복제 모드</param>
        void Replicate(T entity, ReplicationMode replicateMode);

        /// <summary>
        /// delete specified entity
        /// </summary>
        /// <param name="entity">entity to delete</param>
        void Delete(T entity);

        /// <summary>
        /// delete entity by identity value
        /// </summary>
        /// <param name="id">Identity value of entity to delete</param>
        /// <param name="lockMode">Lock mode</param>
        void Delete(object id, LockMode lockMode);

        /// <summary>
        /// delete all entities
        /// </summary>
        void DeleteAll();

        /// <summary>
        /// delete entities matched with specified detached criteria
        /// </summary>
        /// <param name="criteria">The criteria to look for deleting</param>
        void DeleteAll(DetachedCriteria criteria);

        /// <summary>
        /// delete entities matched with specified detached criteria
        /// </summary>
        /// <param name="queryOver">The criteria to look for deleting</param>
        void DeleteAll(QueryOver<T> queryOver);

        /// <summary>
        /// Criterion에 해당하는 모든 엔티티를 삭제한다. 복수의 조건은 AND 조건이다.
        /// </summary>
        /// <param name="criterions"></param>
        void DeleteAll(ICriterion[] criterions);

        /// <summary>
        /// <paramref name="expressions"/>에 해당하는 모든 엔티티를 삭제한다. 복수의 조건은 AND 조건이다.
        /// </summary>
        void DeleteAll(Expression<Func<T, bool>>[] expressions);

        /// <summary>
        /// 지정된 Parameter에 해당하는 모든 엔티티를 삭제한다. 복수의 조건은 AND 조건이다.
        /// </summary>
        /// <param name="parameters"></param>
        void DeleteAll(INHParameter[] parameters);

        /// <summary>
        /// Delete entities by Named Query (defined at hbm.xml)
        /// </summary>
        /// <param name="namedQuery">named query to look for deleting</param>
        /// <param name="parameters">parameters</param>
        void DeleteAll(string namedQuery, params INHParameter[] parameters);

        /// <summary>
        /// Delete entities by HQL 
        /// </summary>
        /// <param name="hql">HQL string to look for deleting</param>
        /// <param name="parameters">parameters</param>
        void DeleteAllByHql(string hql, params INHParameter[] parameters);

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
        int ExecuteUpdate(string hql, params INHParameter[] parameters);

        /// <summary>
        /// <see cref="ExecuteUpdate"/>와 같은 일을 하지만, Transaction을 적용하여, 작업한다.
        /// </summary>
        /// <param name="hql">HQL statement. ex: delete Parent p where exists (from p.Children)</param>
        /// <param name="parameters">named parameters</param>
        /// <returns>number of entities effected by this operation</returns>
        /// <seealso cref="ExecuteUpdate"/>
        int ExecuteUpdateTransactional(string hql, params INHParameter[] parameters);

        /// <summary>
        /// Create an new instance of <typeparamref name="T"/>, mapping it to the concrete class if needed
        /// </summary>
        /// <returns>new instance</returns>
        T Create();

        /// <summary>
        /// the specified instance is transient object ?
        /// </summary>
        /// <returns>if the specified entity is transient object, return true. otherwise return false.</returns>
        bool IsTransient(T entity);

        /// <summary>
        /// Get Entity metadata
        /// </summary>
        /// <returns>Metadata of T</returns>
        IClassMetadata GetClassMetadata();

        /// <summary>
        /// Create criteria for current entity type in current session.
        /// </summary>
        /// <returns>instance of ICriteria for T</returns>
        ICriteria CreateCriteria();

        /// <summary>
        /// Create detached criteria for Entity
        /// </summary>
        /// <returns>Instance of DetachedCriteria for T</returns>
        DetachedCriteria CreateDetachedCriteria();

        /// <summary>
        /// Create an aliases <see cref="DetachedCriteria"/> compatible with current Dao instance.
        /// </summary>
        /// <param name="alias">alias</param>
        /// <returns>Instance of <see cref="DetachedCriteria"/> which has alias for T</returns>
        DetachedCriteria CreateDetachedCriteria(string alias);

        /// <summary>
        /// Create <see cref="IQuery"/> instance of current session with parameters.
        /// </summary>
        /// <param name="hql">HQL statement</param>
        /// <param name="parameters">named parameters</param>
        /// <returns></returns>
        IQuery CreateQuery(string hql, params INHParameter[] parameters);

        /// <summary>
        /// IQueryOver 를 생성합니다.
        /// </summary>
        /// <returns></returns>
        IQueryOver<T, T> CreateQueryOver();

        /// <summary>
        /// IQueryOver 를 생성합니다.
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        IQueryOver<T, T> CreateQueryOver(Expression<Func<T>> alias);

        /// <summary>
        ///  Detached QueryOver 생성 (<see cref="QueryOver.Of{T}()"/>)
        /// </summary>
        /// <returns></returns>
        QueryOver<T, T> CreateQueryOverOf();

        /// <summary>
        /// Detached QueryOver 생성 (<see cref="QueryOver.Of{T}()"/>)
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        QueryOver<T, T> CreateQueryOverOf(Expression<Func<T>> alias);

        /// <summary>
        /// 엔티티 질의를 위해 LINQ의 <see cref="IQueryable{T}"/>를 반환합니다.
        /// </summary>
        IQueryable<T> Query();
    }
}