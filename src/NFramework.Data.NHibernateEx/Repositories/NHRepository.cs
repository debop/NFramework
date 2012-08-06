using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Engine;
using NHibernate.Linq;
using NHibernate.Metadata;
using NHibernate.Transform;
using NSoft.NFramework.Data.NHibernateEx.Domain;
using NSoft.NFramework.InversionOfControl;
using NSoft.NFramework.Reflections;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Data.NHibernateEx {
    // NOTE: Oralce, MsSqlCe 는 MultiCriteria를 지원하지 않습니다.
    // NOTE: MsSqlCe에서는 Future 를 지원하지 않습니다.

    /// <summary>
    /// NHibernate 용 기본 Repository (INHRepository{T} 를 구현한 기본 Class이다.)
    /// </summary>
    /// <typeparam name="T">Type of entity object</typeparam>
    [Serializable]
    public class NHRepository<T> : INHRepository<T> where T : class {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// Null Order Array.
        /// </summary>
        public static readonly Order[] NullOrderArray = new Order[0];

        /// <summary>
        /// Null <see cref="INHOrder"/> Array.
        /// </summary>
        public static readonly INHOrder<T>[] EmptyNHOrders = new INHOrder<T>[0];

        #region << Properties >>

        private static Type _concreteType = typeof(T);

        /// <summary>
        /// Type of Entity which handled by this Reposiotry
        /// </summary>
        public Type ConcreteType {
            get { return _concreteType; }
            protected set { _concreteType = value; }
        }

        /// <summary>
        /// Entity가 매핑된 Session Factory 인스턴스
        /// </summary>
        public virtual ISessionFactory SessionFactory {
            get { return UnitOfWork.CurrentSessionFactory; }
        }

        /// <summary>
        /// UnitOfWork의 현재 Session 인스턴스
        /// </summary>
        public virtual ISession Session {
            get { return UnitOfWork.GetCurrentSessionFor(ConcreteType); }
        }

        /// <summary>
        /// Ini 파일로부터 NHibernate HQL 문장을 제공하는 Provider
        /// </summary>
        public virtual IIniQueryProvider QueryProvider {
            get { return UnitOfWork.QueryProvider; }
        }

        /// <summary>
        /// <see cref="ISession"/> 작업 후 실행될 Action.<br/>
        /// 여기에 Database Fetching 작업 완료 후의 후처리 작업을 정의하면 된다.
        /// </summary>
        protected virtual DisposableAction<ISession> ActionToBePerformedOnSessionUsedForDbFetches {
            get {
                return new DisposableAction<ISession>(
                    delegate {
                        // 여기에 Database Fetching 작업 완료 후의 후처리 작업을 정의하면 된다.
                        //if (IsDebugEnabled)
                        //    log.Debug("NHibernate를 이용한 Database Fetching 작업이 완료되었습니다.");
                    },
                    Session);
            }
        }

        #endregion

        #region << Get / Load >>

        /// <summary>
        /// Get Entity by specified identity value or return nulll if it doesn't exist
        /// </summary>
        /// <param name="id"></param>
        /// <returns>if not exists, return null</returns>
        public T Get(object id) {
            return (T)Session.Get(ConcreteType, id);
        }

        /// <summary>
        /// Get Entity by specified identity value or return nulll if it doesn't exist
        /// </summary>
        /// <param name="id">identity of entity</param>
        /// <param name="lockMode">entity lock mode</param>
        /// <returns>if not exists, return null</returns>
        public T Get(object id, LockMode lockMode) {
            return (T)Session.Get(ConcreteType, id, lockMode);
        }

        /// <summary>
        /// Load Entity by specified identity value or throw an exception if there isn't an entity that matches the specified id
        /// </summary>
        /// <param name="id">identity of entity</param>
        /// <returns>if not exists, exception occurred</returns>
        public T Load(object id) {
            return (T)Session.Load(ConcreteType, id);
        }

        /// <summary>
        /// Load Entity by specified identity value or throw an exception if there isn't an entity that matches the specified id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="lockMode">entity lock mode</param>
        /// <returns>if not exists, exception occurred</returns>
        public T Load(object id, LockMode lockMode) {
            return (T)Session.Load(ConcreteType, id, lockMode);
        }

        #endregion

        #region << GetIn / GetInG >>

        /// <summary>
        /// 지정된 Identity 배열에 해당하는 모든 Entity를 로딩한다. (In 을 사용한다)
        /// </summary>
        /// <param name="ids">identity values</param>
        /// <returns>list of entity</returns>
        public IList<T> GetIn(ICollection ids) {
            // return FindAll(CreateDetachedCriteria().Add(Restrictions.In("Id", ids)));

            return (ids != null)
                       ? FindAll(new[] { Restrictions.In("Id", ids) })
                       : new List<T>();
        }

        /// <summary>
        /// 지정된 Id 컬렉션에 해당하는 모든 Entity를 로딩한다. (SQL 의 IN (xxx,yyy,zzz) 를 사용한다)
        /// </summary>
        /// <typeparam name="TId">Entity Id의 수형</typeparam>
        /// <param name="ids">Id값의 컬렉션</param>
        /// <returns>list of entity</returns>
        public IList<T> GetInG<TId>(ICollection<TId> ids) {
            return (ids != null)
                       ? FindAll(new[] { Restrictions.InG("Id", ids) })
                       : new List<T>();
        }

        #endregion

        #region << GetPage >>

        private ICriteria GetPagingListCriteria(int pageIndex, int pageSize, DetachedCriteria criteria, params Order[] orders) {
            pageIndex.ShouldBeGreaterOrEqual(0, "pageIndex");

            var pagingCritieria = CriteriaTransformer.Clone(criteria);

            var firstResult = pageIndex * pageSize;

            pagingCritieria.SetFirstResult(firstResult > 0 ? firstResult : 0);
            pagingCritieria.SetMaxResults(pageSize > 0 ? pageSize : RowSelection.NoValue);

            return Session.GetExecutableCriteria<T>(pagingCritieria, orders);
        }

        /// <summary>
        /// Get paginated entity list
        /// </summary>
        /// <param name="pageIndex">Page index (start from 0)</param>
        /// <param name="pageSize">Page size (must greator than 0)</param>
        /// <param name="criterions">criteria</param>
        /// <returns>paginated list</returns>
        public IPagingList<T> GetPage(int pageIndex, int pageSize, ICriterion[] criterions) {
            return GetPage(pageIndex, pageSize, NullOrderArray, criterions);
        }

        /// <summary>
        /// Get paginated entity list
        /// </summary>
        /// <param name="pageIndex">Page index (start from 0)</param>
        /// <param name="pageSize">Page size (must greator than 0)</param>
        /// <param name="orders">sort order</param>
        /// <param name="criterions">criteria</param>
        /// <returns>paginated list</returns>
        public IPagingList<T> GetPage(int pageIndex, int pageSize, Order[] orders, params ICriterion[] criterions) {
            pageIndex.ShouldBeGreaterOrEqual(0, "pageIndex");

            if(IsDebugEnabled)
                log.Debug("페이징된 엔티티 컬렉션을 로드합니다... pageIndex=[{0}], pageSize=[{1}], orders=[{2}], criteria=[{3}]",
                          pageIndex, pageSize, orders.CollectionToString(), criterions.CollectionToString());

            var countCriteria = CriteriaTransformer.TransformToRowCount(Session.CreateCriteria<T>(criterions, CreateCriteria));

            var listCriteria =
                Session
                    .CreateCriteria<T>(criterions, CreateCriteria, orders)
                    .SetFirstResult(pageIndex * pageSize);

            if(pageSize > 0)
                listCriteria.SetMaxResults(pageSize);

            //! HINT: Oracle, SqlCE 등에서 MultiCriteria를 수행하지 못합니다. 그래서 따로 했습니다.
            //! HINT: SessionFactory.IsMsSqlServer2005OrHigher() 등을 이용하여 선별적으로 MultiCriteria를 사용할 수 있습니다.

            var totalItemCount = countCriteria.UniqueResult().AsInt();
            var items = listCriteria.List<T>();

            return new PagingList<T>(items, pageIndex, pageSize, totalItemCount);
        }

        /// <summary>
        /// Get paginated entity list
        /// </summary>
        /// <param name="pageIndex">Page index (start from 0)</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="expressions">expression for criteria</param>
        /// <returns>paginated list</returns>
        public IPagingList<T> GetPage(int pageIndex, int pageSize, params Expression<Func<T, bool>>[] expressions) {
            return GetPage(pageIndex, pageSize, EmptyNHOrders, expressions);
        }

        /// <summary>
        /// Get paginated entity list
        /// </summary>
        /// <param name="pageIndex">Page index (start from 0)</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="orders">sort order</param>
        /// <param name="expressions">expression for criteria</param>
        /// <returns>paginated list</returns>
        public IPagingList<T> GetPage(int pageIndex, int pageSize, INHOrder<T>[] orders, params Expression<Func<T, bool>>[] expressions) {
            var queryOver = NHTool.CreateQueryOverOf(expressions);
            return GetPage(pageIndex, pageSize, queryOver, orders);
        }

        /// <summary>
        /// Get paginated entity list
        /// </summary>
        /// <param name="criteria"></param>
        /// <param name="orders"></param>
        /// <returns></returns>
        public IPagingList<T> GetPage(DetachedCriteria criteria, params Order[] orders) {
            return GetPage(0, 0, criteria, orders);
        }

        /// <summary>
        /// Get paginated entity list
        /// </summary>
        /// <param name="pageIndex">Page index (start from 0)</param>
        /// <param name="pageSize">Page size (must greator than 0)</param>
        /// <param name="criteria">DetachedCriteria (null 이면 모든 레코드를 조회합니다)</param>
        /// <param name="orders">sort order</param>
        /// <returns>pagenated list</returns>
        public IPagingList<T> GetPage(int pageIndex, int pageSize, DetachedCriteria criteria, params Order[] orders) {
            if(IsDebugEnabled)
                log.Debug("페이징된 엔티티 컬렉션을 로드합니다... pageIndex=[{1}], pageSize=[{2}], criteria=[{3}], orders=[{4}]",
                          _concreteType.FullName, pageIndex, pageSize, criteria, orders.CollectionToString());

            if(criteria == null)
                criteria = DetachedCriteria.For<T>();

            //! NOTE: Oralce, SqlCe 는 MultiCriteria를 지원하지 않습니다. 그래서 두개로 나누어서 작업합니다.
            //
            var countCriteria = CriteriaTransformer.TransformToRowCount(criteria);
            var totalItemCount = countCriteria.GetExecutableCriteria(Session).UniqueResult().AsInt();

            var items = GetPagingListCriteria(pageIndex, pageSize, criteria, orders).List<T>();

            return new PagingList<T>(items, pageIndex, pageSize, totalItemCount);
        }

        /// <summary>
        /// Get paginated entity list
        /// </summary>
        /// <param name="orders">sort order</param>
        /// <returns>pagenated list</returns>
        public IPagingList<T> GetPage(params INHOrder<T>[] orders) {
            return GetPage(CreateQueryOverOf(), orders);
        }

        /// <summary>
        /// Get paginated entity list
        /// </summary>
        /// <param name="queryOver">detached QueryOver{T,T}</param>
        /// <param name="orders">sort order</param>
        /// <returns>pagenated list</returns>
        public IPagingList<T> GetPage(QueryOver<T> queryOver, params INHOrder<T>[] orders) {
            return GetPage(0, 0, queryOver, orders);
        }

        /// <summary>
        /// Get paginated entity list
        /// </summary>
        /// <param name="pageIndex">Page index (start from 0)</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="queryOver">detached QueryOver{T,T}</param>
        /// <param name="orders">sort order</param>
        /// <returns>pagenated list</returns>
        public IPagingList<T> GetPage(int pageIndex, int pageSize, QueryOver<T> queryOver, params INHOrder<T>[] orders) {
            queryOver.ShouldNotBeNull("queryOver");

            var rowCountQuery = queryOver.ToRowCountQuery();
            var sortQuery = queryOver.Clone().AddOrders(orders);

            var firstResult = pageIndex * pageSize;
            var maxResults = pageSize;

            if(firstResult > 0)
                sortQuery.AddSkip(firstResult);

            if(maxResults > 0)
                sortQuery.AddTake(maxResults);

            //! HINT: Oracle, SqlCe 등에서는 MultiCriteria를 지원하지 않습니다. 그래서 MultiCriteria 대신 사용합니다.
            //
            var rowCount = rowCountQuery.GetExecutableQueryOver(Session).RowCount();
            var list = sortQuery.GetExecutableQueryOver(Session).List<T>();

            return new PagingList<T>(list, pageIndex, pageSize, rowCount);
        }

        #endregion

        #region << Future >>

        /// <summary>
        /// Get a future entity from the persistence store, or return null
        /// </summary>
        /// <param name="id"></param>
        /// <returns>a future for the entity that matches the id. if it doesn't exist, return null.</returns>
        [Obsolete("NHibernate 3.x의 Future 기능을 사용하세요")]
        public IFutureValue<T> FutureGet(object id) {
            id.ShouldNotBeNull("id");
            return CreateCriteria(Session).AddEq("Id", id).FutureValue<T>();
        }

        /// <summary>
        /// Load a future entity from the persistence store.
        /// Will throw an exception if there isn't an entity that matches the specified id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>a future for the entity that matches the id. if it doesn't exist, throw exception.</returns>
        [Obsolete("NHibernate 3.x의 Future 기능을 사용하세요")]
        public IFutureValue<T> FutureLoad(object id) {
            id.ShouldNotBeNull("id");
            return CreateCriteria(Session).AddEq("Id", id).FutureValue<T>();
        }

        /// <summary>
        /// Get a future entity collection from the persistence store
        /// </summary>
        /// <param name="detachedCriteria"></param>
        /// <returns></returns>
        [Obsolete("NHibernate 3.x의 Future 기능을 사용하세요")]
        public IFutureValue<T> FutureValue(DetachedCriteria detachedCriteria) {
            detachedCriteria.ShouldNotBeNull("detachedCriteria");

            using(var action = ActionToBePerformedOnSessionUsedForDbFetches) {
                return
                    action.Value
                        .GetExecutableCriteria<T>(detachedCriteria)
                        .SetMaxResults(1)
                        .FutureValue<T>();
            }
        }

        /// <summary>
        /// Get a future entity collection from the persistence store
        /// </summary>
        [Obsolete("NHibernate 3.x의 Future 기능을 사용하세요")]
        public IFutureValue<T> FutureValue(QueryOver<T> queryOver) {
            queryOver.ShouldNotBeNull("queryOver");
            return FutureValue(queryOver.DetachedCriteria);
        }

        /// <summary>
        /// Get a future entity collection from the persistence store
        /// </summary>
        [Obsolete("NHibernate 3.x의 Future 기능을 사용하세요")]
        public IFutureValue<T> FutureValue(params Expression<Func<T, bool>>[] expressions) {
            var queryOver = NHTool.CreateQueryOverOf(expressions);
            return FutureValue(queryOver);
        }

        /// <summary>
        /// Get a future entity collection from the persistence store
        /// </summary>
        public IEnumerable<T> Future(DetachedCriteria detachedCriteria, params Order[] orders) {
            detachedCriteria.ShouldNotBeNull("detachedCriteria");

            return
                Session
                    .GetExecutableCriteria<T>(detachedCriteria, orders)
                    .Future<T>();
        }

        /// <summary>
        /// Get a future entity collection from the persistence store
        /// </summary>
        public IEnumerable<T> Future(QueryOver<T> queryOver, params INHOrder<T>[] orders) {
            return Future(queryOver.AddOrders(orders).DetachedCriteria);
        }

        /// <summary>
        /// Get a future entity collection from the persistence store
        /// </summary>
        public IEnumerable<T> Future(Expression<Func<T, bool>>[] expressions, params INHOrder<T>[] orders) {
            return Future(NHTool
                              .CreateQueryOverOf(expressions)
                              .AddOrders(orders)
                              .DetachedCriteria);
        }

        #endregion

        #region << FindAll >>

        internal static ICriteria SetFetchRange(ICriteria criteria, int firstResult, int maxResults) {
            if(IsDebugEnabled)
                log.Debug("ICriteria에 firstResult=[{0}], maxResult=[{1}]을 설정합니다.", firstResult, maxResults);

            criteria.SetFirstResult(firstResult > 0 ? firstResult : 0);
            criteria.SetMaxResults(maxResults > 0 ? maxResults : RowSelection.NoValue);

            return criteria;
        }

        internal static IQuery SetFetchRange(IQuery query, int firstResult, int maxResults) {
            if(IsDebugEnabled)
                log.Debug("IQuery에 firstResult=[{0}], maxResult=[{1}]을 설정합니다.", firstResult, maxResults);

            query.SetFirstResult(firstResult > 0 ? firstResult : 0);
            query.SetMaxResults(maxResults > 0 ? maxResults : RowSelection.NoValue);

            return query;
        }

        /// <summary>
        /// Get all entities.
        /// </summary>
        /// <returns>entity collection</returns>
        public IList<T> FindAll() {
            if(IsDebugEnabled)
                log.Debug("엔티티[{0}]의 모든 정보를 조회합니다.", _concreteType.FullName);

            return CreateQueryOver().List<T>();
        }

        /// <summary>
        /// Get entities matched with criteria
        /// </summary>
        /// <param name="criterions">where</param>
        /// <returns>entity collection</returns>
        public IList<T> FindAll(ICriterion[] criterions) {
            return FindAll(NullOrderArray, criterions);
        }

        /// <summary>
        /// Get ordered and ranged entities matched with criteria
        /// </summary>
        /// <param name="orders">ordering spec</param>
        /// <param name="criterions">where spec</param>
        /// <returns>entity collection</returns>
        public IList<T> FindAll(Order[] orders, params ICriterion[] criterions) {
            return FindAll(orders, 0, 0, criterions);
        }

        /// <summary>
        /// Get ordered and ranged entities matched with criteria
        /// </summary>
        /// <param name="orders">ordering spec</param>
        /// <param name="firstResult">first index (start from 0)</param>
        /// <param name="maxResults">max resultset count (start from 1)</param>
        /// <param name="criterions">where spec</param>
        /// <returns>entity collection</returns>
        public IList<T> FindAll(Order[] orders, int firstResult, int maxResults, params ICriterion[] criterions) {
            if(IsDebugEnabled)
                log.Debug("엔티티[{0}] 정보를 조회합니다. firstResult=[{1}], maxResults=[{2}], orders=[{3}], criterions=[{4}]",
                          _concreteType.FullName, firstResult, maxResults, orders.CollectionToString(), criterions.CollectionToString());

            var crit = Session.CreateCriteria<T>(criterions, orders);
            crit = SetFetchRange(crit, firstResult, maxResults);
            return crit.List<T>();
        }

        /// <summary>
        /// Get ranged entities matched with criteria
        /// </summary>
        /// <param name="firstResult">first index (start from 0)</param>
        /// <param name="maxResults">max resultset count (start from 1) (0이면 마지막 Record까지 읽어온다.)</param>
        /// <param name="criterions">where</param>
        /// <returns>entity collection</returns>
        public IList<T> FindAll(int firstResult, int maxResults, params ICriterion[] criterions) {
            return FindAll(firstResult, maxResults, NullOrderArray, criterions);
        }

        /// <summary>
        /// 지정된 criteria를 이용하여 정보를 조회합니다.
        /// </summary>
        /// <param name="firstResult">first index (start from 0)</param>
        /// <param name="maxResults">max resultset count (start from 1) (0이면 마지막 Record까지 읽어온다.)</param>
        /// <param name="orders">정렬 순서</param>
        /// <param name="criterions">조회 조건</param>
        /// <returns>collection of entity.</returns>
        public IList<T> FindAll(int firstResult, int maxResults, Order[] orders, params ICriterion[] criterions) {
            return FindAll(orders, firstResult, maxResults, criterions);
        }

        /// <summary>
        /// Get entities matched with criteria
        /// </summary>
        /// <param name="expression">expression</param>
        /// <param name="expressions">조건 표현식</param>
        /// <returns>entity collection</returns>
        public IList<T> FindAll(Expression<Func<T, bool>> expression, params Expression<Func<T, bool>>[] expressions) {
            expression.ShouldNotBeNull("expression");

            if(expressions == null || expressions.Length == 0)
                return FindAll(NHTool.CreateQueryOverOf(new Expression<Func<T, bool>>[] { expression }));

            var exprs = expressions.ToList();
            exprs.Insert(0, expression);

            return FindAll(NHTool.CreateQueryOverOf(exprs.ToArray()));
        }

        /// <summary>
        /// Get ordered and ranged entities matched with criteria
        /// </summary>
        /// <param name="orders">ordering spec</param>
        /// <param name="expressions">조건 표현식</param>
        /// <returns>entity collection</returns>
        public IList<T> FindAll(INHOrder<T>[] orders, params Expression<Func<T, bool>>[] expressions) {
            return FindAll(NHTool.CreateQueryOverOf(expressions), orders);
        }

        /// <summary>
        /// 지정된 criteria를 이용하여 정보를 조회합니다.
        /// </summary>
        /// <param name="firstResult">first index (start from 0)</param>
        /// <param name="maxResults">max resultset count (start from 1) (0이면 마지막 Record까지 읽어온다.)</param>
        /// <param name="orders">정렬 순서</param>
        /// <param name="expressions">조건 표현식</param>
        /// <returns>collection of entity.</returns>
        public IList<T> FindAll(INHOrder<T>[] orders, int firstResult, int maxResults, params Expression<Func<T, bool>>[] expressions) {
            return FindAll(NHTool.CreateQueryOverOf(expressions), firstResult, maxResults, orders);
        }

        /// <summary>
        /// Get ranged entities matched with criteria
        /// </summary>
        /// <param name="firstResult">first index (start from 0)</param>
        /// <param name="maxResults">max resultset count (start from 1) (0이면 마지막 Record까지 읽어온다.)</param>
        /// <param name="expressions">조건 표현식</param>
        /// <returns>entity collection</returns>
        public IList<T> FindAll(int firstResult, int maxResults, params Expression<Func<T, bool>>[] expressions) {
            return FindAll(NHTool.CreateQueryOverOf(expressions), firstResult, maxResults);
        }

        /// <summary>
        /// 지정된 criteria를 이용하여 정보를 조회합니다.
        /// </summary>
        /// <param name="firstResult">first index (start from 0)</param>
        /// <param name="maxResults">max resultset count (start from 1) (0이면 마지막 Record까지 읽어온다.)</param>
        /// <param name="orders">정렬 순서</param>
        /// <param name="expressions">조건 표현식</param>
        /// <returns>collection of entity.</returns>
        public IList<T> FindAll(int firstResult, int maxResults, INHOrder<T>[] orders, params Expression<Func<T, bool>>[] expressions) {
            return FindAll(NHTool.CreateQueryOverOf(expressions), firstResult, maxResults, orders);
        }

        /// <summary>
        /// Get ranged entites matched with detached criteria, ordering is optional
        /// </summary>
        /// <param name="criteria">where spec</param>
        /// <param name="orders">ordering spec</param>
        /// <returns>entity collection</returns>
        public IList<T> FindAll(DetachedCriteria criteria, params Order[] orders) {
            if(IsDebugEnabled)
                log.Debug("엔티티[{0}] 정보를 조회합니다. orders=[{1}]",
                          _concreteType.FullName, orders.CollectionToString());

            return Session.GetExecutableCriteria<T>(criteria, orders).List<T>();
        }

        /// <summary>
        /// Get ranged entites matched with detached criteria, ordering is optional
        /// </summary>
        /// <param name="criteria">where spec</param>
        /// <param name="firstResult">first index (start from 0)</param>
        /// <param name="maxResults">max resultset count (start from 1)</param>
        /// <param name="orders">ordering spec</param>
        /// <returns>entity collection</returns>
        public IList<T> FindAll(DetachedCriteria criteria, int firstResult, int maxResults, params Order[] orders) {
            if(IsDebugEnabled)
                log.Debug("엔티티[{0}] 정보를 조회합니다. firstResult=[{1}], maxResults=[{2}], orders=[{3}]",
                          _concreteType.FullName, firstResult, maxResults, orders.CollectionToString());

            var crit = Session.GetExecutableCriteria<T>(criteria, orders);
            crit = SetFetchRange(crit, firstResult, maxResults);
            return crit.List<T>();
        }

        /// <summary>
        /// Get ordered and ranged entities matched with criteria
        /// </summary>
        /// <param name="queryOver">Detached QueryOver</param>
        /// <param name="orders">ordering spec</param>
        /// <returns>entity collection</returns>
        public IList<T> FindAll(QueryOver<T> queryOver, params INHOrder<T>[] orders) {
            if(IsDebugEnabled)
                log.Debug("엔티티[{0}] 정보를 조회합니다. orders=[{1}]",
                          _concreteType.FullName, orders.CollectionToString());

            return Session.GetExecutableQueryOver(queryOver.AddOrders(orders)).List<T>();
        }

        /// <summary>
        /// Get ordered and ranged entities matched with criteria
        /// </summary>
        /// <param name="queryOver">Detached QueryOver</param>
        /// <param name="firstResult">first index (start from 0)</param>
        /// <param name="maxResults">max resultset count (start from 1)</param>
        /// <param name="orders">ordering spec</param>
        /// <returns>entity collection</returns>
        public IList<T> FindAll(QueryOver<T> queryOver, int firstResult, int maxResults, params INHOrder<T>[] orders) {
            if(IsDebugEnabled)
                log.Debug("엔티티[{0}] 정보를 조회합니다. firstResult=[{1}], maxResults=[{2}], orders=[{3}]",
                          _concreteType.FullName, firstResult, maxResults, orders.CollectionToString());

            var query = queryOver.Clone();

            if(firstResult > 0)
                query.AddSkip(firstResult);

            if(maxResults > 0)
                query.AddTake(maxResults);

            return Session.GetExecutableQueryOver(query.AddOrders(orders)).List<T>();
        }

        /// <summary>
        /// Get entities by examping with exampleInstance
        /// </summary>
        /// <param name="exampleInstance">instance for exampling</param>
        /// <param name="propertyNamesToExclude">excluded property for exampling</param>
        /// <returns>entity collection</returns>
        public IList<T> FindAll(T exampleInstance, params string[] propertyNamesToExclude) {
            return FindAll(exampleInstance, 0, 0, propertyNamesToExclude);
        }

        /// <summary>
        /// Get entities by examping with exampleInstance
        /// </summary>
        /// <param name="exampleInstance">instance for exampling</param>
        /// <param name="firstResult">first index (start from 0)</param>
        /// <param name="maxResults">max resultset count (start from 1) (0이면 마지막 Record까지 가져온다.)</param>
        /// <param name="propertyNamesToExclude">excluded property for exampling</param>
        /// <returns>entity collection</returns>
        public IList<T> FindAll(T exampleInstance, int firstResult, int maxResults, params string[] propertyNamesToExclude) {
            var example = Example.Create(exampleInstance);

            if(propertyNamesToExclude != null)
                foreach(string propertyName in propertyNamesToExclude)
                    example.ExcludeProperty(propertyName);

            return FindAll(firstResult, maxResults, example);
        }

        /// <summary>
        /// Get entities by Named Query which defined in mapping files (*.hbm.xml) ex: &lt;query name="xxxx"&gt;
        /// </summary>
        /// <param name="namedQuery">name of NamedQuery which defined in mapping files(*.hbm.xml)</param>
        /// <param name="parameters">HQL Parameters</param>
        /// <returns>entity collection</returns>
        public IList<T> FindAll(string namedQuery, params INHParameter[] parameters) {
            namedQuery.ShouldNotBeWhiteSpace("namedQuery");

            if(IsDebugEnabled)
                log.Debug("NamedQuery [{0}] 를 실행합니다... parameters=[{1}]", namedQuery, parameters.CollectionToString());

            return Session.GetNamedQuery(namedQuery, parameters).List<T>();
        }

        /// <summary>
        /// Get entities by Named Query which defined in mapping files (*.hbm.xml) ex: &lt;query name="xxxx"&gt;
        /// </summary>
        /// <param name="namedQuery">name of NamedQuery which defined in mapping files(*.hbm.xml)</param>
        /// <param name="firstResult">first index (start from 0)</param>
        /// <param name="maxResults">max resultset count (start from 1) (0이면 마지막 Record까지 가져온다.)</param>
        /// <param name="parameters">HQL Parameters</param>
        /// <returns>entity collection</returns>
        public IList<T> FindAll(string namedQuery, int firstResult, int maxResults, params INHParameter[] parameters) {
            namedQuery.ShouldNotBeWhiteSpace("namedQuery");

            if(IsDebugEnabled)
                log.Debug("NamedQuery[{0}] 를 실행합니다... parameters=[{1}]",
                          namedQuery, firstResult, maxResults, parameters.CollectionToString());

            var query = Session.GetNamedQuery(namedQuery, parameters);
            query = SetFetchRange(query, firstResult, maxResults);

            return query.List<T>();
        }

        /// <summary>
        /// NHibernate Query Language (HQL) 를 이용한 조회
        /// </summary>
        /// <param name="queryString">hql string</param>
        /// <param name="parameters">HQL Parameters</param>
        /// <returns>entity collection</returns>
        public IList<T> FindAllByHql(string queryString, params INHParameter[] parameters) {
            queryString.ShouldNotBeWhiteSpace("queryString");

            if(IsDebugEnabled)
                log.Debug("Hql을 이용하여 엔티티[{0}]를 조회합니다. hql=[{1}], parameters=[{2}]",
                          _concreteType.FullName, queryString, parameters.CollectionToString());

            return
                Session
                    .CreateQuery(queryString, parameters)
                    .List<T>();
        }

        /// <summary>
        /// NHibernate Query Language (HQL) 를 이용한 조회
        /// </summary>
        /// <param name="queryString">hql string</param>
        /// <param name="firstResult">first index (start from 0)</param>
        /// <param name="maxResults">max resultset count (start from 1) (0이면 마지막 Record까지 가져온다.)</param>
        /// <param name="parameters">HQL Parameters</param>
        /// <returns>entity collection</returns>
        public IList<T> FindAllByHql(string queryString, int firstResult, int maxResults, params INHParameter[] parameters) {
            queryString.ShouldNotBeWhiteSpace("queryString");

            var query = Session.CreateQuery(queryString, parameters);
            query = SetFetchRange(query, firstResult, maxResults);

            return query.List<T>();
        }

        /// <summary>
        /// Entity의 속성명이 지정된 값과 같은 엔티티를 모두 조회한다. (propertyName = value)
        /// </summary>
        /// <param name="propertyName">속성명</param>
        /// <param name="value">비교할 값</param>
        /// <returns>entity collection</returns>
        [Obsolete("FindAll(params Expression<Func<T, bool>>[] expressions) 를 사용하거나 INHRepository<T>.Query() 를 사용하세요.")]
        public IList<T> FindAllPropertyEq(string propertyName, object value) {
            propertyName.ShouldNotBeWhiteSpace("propertyName");
            return FindAll(CreateDetachedCriteria().AddEq(propertyName, value));
        }

        #endregion

        #region << FindOne >>

        /// <summary>
        /// Get unique entity matches with specified criteria. if one more entity exists, throw exception
        /// </summary>
        /// <param name="criterions">where spec</param>
        /// <returns>a single instance that matches the query, or null if the query returns no results.</returns>
        public T FindOne(ICriterion[] criterions) {
            if(IsDebugEnabled)
                log.Debug("엔티티[{0}]에 조건을 만족하는 유일한 엔티티를 조회합니다. criterions=[{1}]",
                          _concreteType.FullName, criterions.CollectionToString());

            return Session.CreateCriteria<T>(criterions).UniqueResult<T>();
        }

        /// <summary>
        /// Get unique entity matches with specified criteria
        /// </summary>
        /// <param name="expressions">where spec</param>
        /// <returns>if not unique entity or not exists, raise exception</returns>
        public T FindOne(params Expression<Func<T, bool>>[] expressions) {
            return FindOne(NHTool.CreateQueryOverOf(expressions));
        }

        /// <summary>
        /// Get unique entity matches with specified detached criteria. if one more entity exists, throw exception
        /// </summary>
        /// <param name="criteria">where spec</param>
        /// <returns>a single instance that matches the query, or null if the query returns no results.</returns>
        public T FindOne(DetachedCriteria criteria) {
            return Session.GetExecutableCriteria<T>(criteria).UniqueResult<T>();
        }

        /// <summary>
        /// Get unique entity matches with specified detached criteria. if one more entity exists, throw exception
        /// </summary>
        /// <param name="queryOver">where spec</param>
        /// <returns>a single instance that matches the query, or null if the query returns no results.</returns>
        public T FindOne(QueryOver<T> queryOver) {
            return Session.GetExecutableQueryOver(queryOver).SingleOrDefault();
        }

        /// <summary>
        /// Get unique entity by named query which defined mapping file. if one more entity exists, throw exception
        /// </summary>
        /// <param name="namedQuery">name of NamedQuery</param>
        /// <param name="parameters">parameters</param>
        /// <returns>a single instance that matches the query, or null if the query returns no results.</returns>
        public T FindOne(string namedQuery, params INHParameter[] parameters) {
            namedQuery.ShouldNotBeWhiteSpace("namedQuery");

            return Session.GetNamedQuery(namedQuery, parameters).UniqueResult<T>();
        }

        /// <summary>
        /// Find unique entity by example instance. if one more entity exists, throw exception
        /// </summary>
        /// <param name="exampleInstance">instance of exampling</param>
        /// <param name="propertyNamesToExclude">prpoerty names to exclude when matching example</param>
        /// <returns>a single instance that matches the query, or null if the query returns no results.</returns>
        public T FindOne(T exampleInstance, params string[] propertyNamesToExclude) {
            var crit = Session.CreateCriteria<T>(null, CreateCriteria);

            var example = Example.Create(exampleInstance);

            if(propertyNamesToExclude != null)
                foreach(string propName in propertyNamesToExclude)
                    example.ExcludeProperty(propName);

            return
                crit.Add(example)
                    .UniqueResult<T>();
        }

        /// <summary>
        /// Get unique entity by hql
        /// </summary>
        /// <param name="query">hql string</param>
        /// <param name="parameters">named parameters</param>
        /// <returns>a single instance that matches the query, or null if the query returns no results.</returns>
        public T FindOneByHql(string query, params INHParameter[] parameters) {
            query.ShouldNotBeWhiteSpace("query");

            if(IsDebugEnabled)
                log.Debug("Hql로 엔티티[{0}]를 로드합니다... query=[{1}], parameters=[{2}]",
                          _concreteType.FullName, query, parameters.CollectionToString());

            return Session.CreateQuery(query, parameters).UniqueResult<T>();
        }

        /// <summary>
        /// Entity의 속성명이 지정된 값과 같은 유일한 엔티티를 조회한다. (propertyName = value)
        /// </summary>
        /// <param name="propertyName">속성명</param>
        /// <param name="value">비교할 값</param>
        /// <returns>엔티티, 없으면 null 반환</returns>
        [Obsolete("FindOne(params Expression<Func<T, bool>>[] expressions) 를 사용하거나 INHRepository<T>.Query() 를 사용하세요.")]
        public T FindOnePropertyEq(string propertyName, object value) {
            propertyName.ShouldNotBeWhiteSpace("propertyName");

            return FindOne(CreateDetachedCriteria().AddEq(propertyName, value));
        }

        #endregion

        #region << FindFirst >>

        /// <summary>
        /// Get first entity by ordering
        /// </summary>
        /// <param name="orders">order by</param>
        /// <returns>if not exist, return null</returns>
        public T FindFirst(Order[] orders) {
            return FindFirst(CreateDetachedCriteria(), orders);
        }

        public T FindFirst(INHOrder<T>[] orders) {
            return FindFirst(CreateQueryOverOf(), orders);
        }

        /// <summary>
        /// Get first entity matched with specified criteria (criteria is optional).
        /// </summary>
        /// <param name="criterions">The collection of ICriterion to look for.</param>
        /// <returns>if not exist, return null</returns>
        public T FindFirst(ICriterion[] criterions) {
            return
                Session
                    .CreateCriteria<T>(criterions)
                    .SetMaxResults(1)
                    .UniqueResult<T>();
        }

        /// <summary>
        /// Get first entity matched with specified criteria (criteria is optional).
        /// </summary>
        /// <param name="expressions">The collection of Lambda expression to look for.</param>
        /// <returns>if not exist, return null</returns>
        public T FindFirst(params Expression<Func<T, bool>>[] expressions) {
            return FindFirst(NHTool.CreateQueryOverOf(expressions));
        }

        /// <summary>
        /// Get first entity matched with specified detached criteria (criteria is optional) and ordering
        /// </summary>
        /// <param name="criteria">where spec</param>
        /// <param name="orders">order by</param>
        /// <returns>if not exist, return null</returns>
        public T FindFirst(DetachedCriteria criteria, params Order[] orders) {
            criteria.ShouldNotBeNull("criteria");

            return
                Session
                    .GetExecutableCriteria<T>(criteria, orders)
                    .SetMaxResults(1)
                    .UniqueResult<T>();
        }

        /// <summary>
        /// Get first entity matched with specified detached criteria (criteria is optional) and ordering
        /// </summary>
        /// <param name="queryOver">where spec</param>
        /// <param name="orders">order by</param>
        /// <returns>if not exist, return null</returns>
        public T FindFirst(QueryOver<T> queryOver, params INHOrder<T>[] orders) {
            return
                Session
                    .GetExecutableQueryOver(queryOver.AddOrders(orders))
                    .Take(1)
                    .SingleOrDefault();
        }

        /// <summary>
        /// Get first entity from NamedQuery
        /// </summary>
        /// <param name="namedQuery">NamedQuery to look for</param>
        /// <param name="parameters">HQL Parameters</param>
        /// <returns>if not exist, return null</returns>
        public T FindFirst(string namedQuery, params INHParameter[] parameters) {
            namedQuery.ShouldNotBeWhiteSpace("namedQuery");

            return
                Session
                    .GetNamedQuery(namedQuery, parameters)
                    .SetMaxResults(1)
                    .UniqueResult<T>();
        }

        /// <summary>
        /// Get first entity matches with exampleInstance by Exampling.
        /// </summary>
        /// <param name="exampleInstance">instance for Exampling</param>
        /// <param name="propertyNamesToExclude">excluded property name for Exampling</param>
        /// <returns>if not exist, return null</returns>
        public T FindFirst(T exampleInstance, params string[] propertyNamesToExclude) {
            exampleInstance.ShouldNotBeNull("exampleInstance");

            var crit = Session.CreateCriteria<T>(null, CreateCriteria);

            var example = Example.Create(exampleInstance);
            foreach(string propName in propertyNamesToExclude)
                example.ExcludeProperty(propName);

            return
                crit.Add(example)
                    .SetMaxResults(1)
                    .UniqueResult<T>();
        }

        /// <summary>
        /// Get the first entity by Hql
        /// </summary>
        /// <param name="queryString">hql string</param>
        /// <param name="parameters">named parameters</param>
        /// <returns>first entity in retrieved entity collection. if not exists, return null</returns>
        public T FindFirstByHql(string queryString, params INHParameter[] parameters) {
            queryString.ShouldNotBeWhiteSpace("queryString");

            return
                Session
                    .CreateQuery(queryString, parameters)
                    .SetMaxResults(1)
                    .UniqueResult<T>();
        }

        /// <summary>
        /// Entity의 속성명이 지정된 값과 같은 첫번째 엔티티를 조회한다. (propertyName = value)
        /// </summary>
        /// <param name="propertyName">속성명</param>
        /// <param name="value">비교할 값</param>
        /// <returns>엔티티, 없으면 null 반환</returns>
        [Obsolete("FindFirst(params Expression<Func<T, bool>>[] expressions) 를 사용하거나 INHRepository<T>.Query() 를 사용하세요.")]
        public T FindFirstPropertyEq(string propertyName, object value) {
            propertyName.ShouldNotBeWhiteSpace("propertyName");
            return FindFirst(CreateDetachedCriteria().AddEq(propertyName, value));
        }

        #endregion

        #region << Count >>

        /// <summary>
        /// Counts the overall number of entities.
        /// </summary>
        /// <returns>count of entities</returns>
        public long Count() {
            return CreateQueryOver().RowCountInt64();
        }

        /// <summary>
        /// Counts the number of instances matching the criteria
        /// </summary>
        /// <param name="criteria">The criteria to look for</param>
        /// <returns>count of entities</returns>
        public long Count(DetachedCriteria criteria) {
            criteria.ShouldNotBeNull("criteria");
            return
                CriteriaTransformer
                    .TransformToRowCount(criteria)
                    .GetExecutableCriteria(Session)
                    .UniqueResult()
                    .AsLong();
        }

        /// <summary>
        /// Counts the number of instances matching the criteria
        /// </summary>
        /// <param name="criterions">The collection of ICriterion to look for</param>
        /// <returns>count of entities</returns>
        public long Count(ICriterion[] criterions) {
            return
                CriteriaTransformer
                    .TransformToRowCount(Session.CreateCriteria<T>(criterions, CreateCriteria))
                    .UniqueResult()
                    .AsLong();
        }

        /// <summary>
        /// Counts the number of instances matching the query
        /// </summary>
        /// <param name="queryOver">QueryOver to look for</param>
        /// <returns>count of entities</returns>
        public long Count(QueryOver<T> queryOver) {
            return
                queryOver
                    .Clone()
                    .GetExecutableQueryOver(Session)
                    .RowCountInt64();
        }

        /// <summary>
        /// Counts the number of instances matching the expression.
        /// </summary>
        /// <param name="expressions">The collection of Lambda expression to look for.</param>
        /// <returns>if not exist, return null</returns>
        public long Count(params Expression<Func<T, bool>>[] expressions) {
            if(expressions == null)
                return Count();

            return Count(NHTool.CreateQueryOverOf(expressions));
        }

        /// <summary>
        /// Counts the number of instances matching the criteria
        /// </summary>
        /// <param name="queryOver">The criteria to look for</param>
        /// <returns>count of entities</returns>
        public long CountAsLong(QueryOver<T> queryOver) {
            return Count(queryOver);
        }

        /// <summary>
        /// Counts the number of instances matching the criteria
        /// </summary>
        /// <param name="expressions">The collection of Lambda expression to look for</param>
        /// <returns>count of entities</returns>
        public long CountAsLong(params Expression<Func<T, bool>>[] expressions) {
            return Count(expressions);
        }

        /// <summary>
        /// Counts the number of instances matching the criteria
        /// </summary>
        /// <param name="queryOver">The criteria to look for</param>
        /// <returns>count of entities</returns>
        public int CountAsInt(QueryOver<T> queryOver) {
            return queryOver.Clone().GetExecutableQueryOver(Session).RowCount();
        }

        /// <summary>
        /// Counts the number of instances matching the criteria
        /// </summary>
        /// <param name="expressions">The collection of Lambda expression to look for</param>
        /// <returns>count of entities</returns>
        public int CountAsInt(params Expression<Func<T, bool>>[] expressions) {
            return CountAsInt(NHTool.CreateQueryOverOf(expressions));
        }

        #endregion

        #region << Exists >>

        /// <summary>
        /// Check if any instance of the type exists
        /// </summary>
        /// <returns>true if an instance is found, otherwise false.</returns>
        public bool Exists() {
            return CreateQueryOver().Take(1).SingleOrDefault() != default(T);
        }

        /// <summary>
        /// Check if any instance matches with the specified criteria
        /// </summary>
        /// <param name="criteria">The criteria to looking for</param>
        /// <returns>true if an instance is found, otherwise false.</returns>
        public bool Exists(DetachedCriteria criteria) {
            return FindFirst(criteria) != default(T);
        }

        /// <summary>
        /// Check if any instance matches with the specified criteria
        /// </summary>
        /// <param name="queryOver">The criteria to looking for</param>
        /// <returns>true if an instance is found, otherwise false.</returns>
        public bool Exists(QueryOver<T> queryOver) {
            return FindFirst(queryOver) != default(T);
        }

        /// <summary>
        /// Check if any instance matches with the specified criteria
        /// </summary>
        /// <param name="criterions">Collection of ICriterion</param>
        /// <returns>true if an instance is found, otherwise false.</returns>
        public bool Exists(ICriterion[] criterions) {
            return FindFirst(criterions) != default(T);
        }

        /// <summary>
        /// Check if any instance matches with the specified criteria
        /// </summary>
        /// <param name="expressions">The collection of Lambda expression to look for</param>
        /// <returns>true if an instance is found, otherwise false.</returns>
        public bool Exists(params Expression<Func<T, bool>>[] expressions) {
            return FindFirst(expressions) != default(T);
        }

        /// <summary>
        /// Check if any instance matches with the specified named query
        /// </summary>
        /// <param name="namedQuery">named query for looking for</param>
        /// <param name="parameters">parameters</param>
        /// <returns>true if an instance is found, otherwise false.</returns>
        public bool Exists(string namedQuery, params INHParameter[] parameters) {
            return FindFirst(namedQuery, parameters) != default(T);
        }

        /// <summary>
        /// Check if any instance matches with the specified simple query string
        /// </summary>
        /// <param name="queryString">queryString for looking for</param>
        /// <param name="parameters">parameters</param>
        /// <returns>true if an instance is found, otherwise false.</returns>
        public bool ExistsByHql(string queryString, params INHParameter[] parameters) {
            return FindFirstByHql(queryString, parameters) != default(T);
        }

        /// <summary>
        /// Is entity exists that's is the specified identity. (별 필요없지 않나?)
        /// </summary>
        /// <returns>true if an instance is found, otherwise false.</returns>
        public bool ExistsById(object id) {
            return Get(id) != null;
        }

        #endregion

        #region << ReportAll >>

        private static IList<TProject> DoReportAll<TProject>(ICriteria criteria, IProjection projectionList) {
            return DoReportAll<TProject>(criteria, projectionList, false);
        }

        private static IList<TProject> DoReportAll<TProject>(ICriteria criteria, IProjection projectionList, bool distinctResults) {
            BuildProjectionCriteria<TProject>(criteria, projectionList, distinctResults);
            return criteria.List<TProject>();
        }

        /// <summary>
        /// Create the projects of type <typeparamref name="TProject"/> (ie DataTransferObject(s)) that satisfies the criteria supplied.
        /// </summary>
        /// <typeparam name="TProject">the type returned. (ie DTO)</typeparam>
        /// <param name="projectionList">Maps the properties from the object graph to the DTO</param>
        /// <returns>The projection result (DTO's) built from the object graph</returns>
        public IList<TProject> ReportAll<TProject>(ProjectionList projectionList) {
            projectionList.ShouldNotBeNull("projectionList");

            var crit = Session.GetExecutableCriteria<T>(null);
            return DoReportAll<TProject>(crit, projectionList);
        }

        /// <summary>
        /// Create the projects of type <typeparamref name="TProject"/> (ie DataTransferObject(s)) that satisfies the criteria supplied.
        /// </summary>
        /// <typeparam name="TProject">the type returned. (ie DTO)</typeparam>
        /// <param name="projectionList">Maps the properties from the object graph to the DTO</param>
        /// <param name="distinctResult">Indicate projection is distinctly search.</param>
        /// <returns>The projection result (DTO's) built from the object graph</returns>
        public IList<TProject> ReportAll<TProject>(ProjectionList projectionList, bool distinctResult) {
            projectionList.ShouldNotBeNull("projectionList");

            var crit = Session.GetExecutableCriteria<T>(null, CreateCriteria);
            return DoReportAll<TProject>(crit, projectionList, distinctResult);
        }

        /// <summary>
        /// Create the projects of type <typeparamref name="TProject"/> (ie DataTransferObject(s)) that satisfies the criteria supplied.
        /// </summary>
        /// <typeparam name="TProject">the type returned. (ie DTO)</typeparam>
        /// <param name="projectionList">Maps the properties from the object graph to the DTO</param>
        /// <param name="orders">The fields the repository should order by</param>
        /// <returns>The projection result (DTO's) built from the object graph</returns>
        public IList<TProject> ReportAll<TProject>(ProjectionList projectionList, Order[] orders) {
            projectionList.ShouldNotBeNull("projectionList");

            var crit = Session.GetExecutableCriteria<T>(null, CreateCriteria, orders);
            return DoReportAll<TProject>(crit, projectionList);
        }

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
        public IList<TProject> ReportAll<TProject>(ProjectionList projectionList, ICriterion[] criterions) {
            projectionList.ShouldNotBeNull("projectionList");

            var crit = Session.CreateCriteria<T>(criterions, CreateCriteria);
            return DoReportAll<TProject>(crit, projectionList);
        }

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
        public IList<TProject> ReportAll<TProject>(ProjectionList projectionList, Order[] orders, params ICriterion[] criterions) {
            projectionList.ShouldNotBeNull("projectionList");

            var crit = Session.CreateCriteria<T>(criterions, CreateCriteria, orders);
            return DoReportAll<TProject>(crit, projectionList);
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
        public IList<TProject> ReportAll<TProject>(ProjectionList projectionList, params Expression<Func<T, bool>>[] expressions) {
            var queryOver = NHTool.CreateQueryOverOf(expressions);
            return ReportAll<TProject>(queryOver, projectionList);
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
        public IList<TProject> ReportAll<TProject>(ProjectionList projectionList, INHOrder<T>[] orders,
                                                   params Expression<Func<T, bool>>[] expressions) {
            var queryOver = NHTool.CreateQueryOverOf(expressions);
            return ReportAll<TProject>(queryOver, projectionList, orders);
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
        public IList<TProject> ReportAll<TProject>(DetachedCriteria criteria, ProjectionList projectionList, params Order[] orders) {
            var crit = Session.GetExecutableCriteria<T>(criteria, CreateCriteria, orders);
            return DoReportAll<TProject>(crit, projectionList);
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
        public IList<TProject> ReportAll<TProject>(QueryOver<T> queryOver, ProjectionList projectionList, params INHOrder<T>[] orders) {
            return ReportAll<TProject>(queryOver.AddOrders(orders).DetachedCriteria, projectionList);
        }

        /// <summary>
        /// Create the projects of type <typeparamref name="TProject"/> (ie DataTransferObject(s)) that satisfies the criteria supplied.
        /// </summary>
        /// <typeparam name="TProject">the type returned. (ie DTO)</typeparam>
        /// <param name="namedQuery"></param>
        /// <param name="parameters"></param>
        /// <returns>The projection result (DTO's) built from the object graph satisfying <paramref name="namedQuery"/></returns>
        /// <returns>collection of <typeparamref name="TProject"/></returns>
        public IList<TProject> ReportAll<TProject>(string namedQuery, params INHParameter[] parameters) {
            namedQuery.ShouldNotBeWhiteSpace("namedQuery");

            return
                Session
                    .GetNamedQuery(namedQuery, parameters)
                    .List<TProject>();
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
        public TProject ReportOne<TProject>(DetachedCriteria criteria, ProjectionList projectionList) {
            var crit = Session.GetExecutableCriteria<T>(criteria);
            return DoReportOne<TProject>(crit, projectionList);
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
        public TProject ReportOne<TProject>(QueryOver<T> queryOver, ProjectionList projectionList) {
            return ReportOne<TProject>(queryOver.DetachedCriteria, projectionList);
        }

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
        public TProject ReportOne<TProject>(ProjectionList projectionList, ICriterion[] criterions) {
            var crit = Session.CreateCriteria<T>(criterions);
            return DoReportOne<TProject>(crit, projectionList);
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
        public TProject ReportOne<TProject>(ProjectionList projectionList, params Expression<Func<T, bool>>[] expressions) {
            return ReportOne<TProject>(NHTool.CreateQueryOverOf(expressions), projectionList);
        }

        private static TProject DoReportOne<TProject>(ICriteria criteria, IProjection projectionList) {
            BuildProjectionCriteria<TProject>(criteria, projectionList, true);
            return criteria.UniqueResult<TProject>();
        }

        #endregion

        #region << Persist / Save / SaveOrUpdate / Update >>

        /// <summary>
        /// Transient Object를 Persistent Object로 만듭니다. 즉 Save합니다!!!
        /// </summary>
        /// <param name="entity">저장할 엔티티</param>
        /// <returns></returns>
        public T Persist(T entity) {
            entity.ShouldNotBeNull("entity");

            if(IsDebugEnabled)
                log.Debug("Persist entity... " + entity);

            Session.Persist(entity);

            // BUG: Refresh는 SQL Server에서는 제대로 되는데, Oracle, SQLite에서는 작동하지 않는다.
            // Session.Refresh(entity);

            return entity;
        }

        /// <summary>
        /// save entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>The generated identifier</returns>
        public T Save(T entity) {
            entity.ShouldNotBeNull("entity");

            if(IsDebugEnabled)
                log.Debug("Save entity... " + entity);

            Session.Save(entity);
            // BUG: SQL Server에서는 제대로 되는데, Oracle, SQLite에서는 작동하지 않는다.
            // Session.Refresh(entity);
            return entity;
        }

        /// <summary>
        /// Save entity with Identity
        /// </summary>
        /// <param name="entity">entity to save or update</param>
        /// <param name="id">identity value of entity</param>
        public void Save(T entity, object id) {
            entity.ShouldNotBeNull("entity");
            id.ShouldNotBeNull("id");

            if(IsDebugEnabled)
                log.Debug("엔티티 저장... entity=[{0}], id=[{1}]", entity, id);

            Session.Save(entity, id);
        }

        /// <summary>
        /// Save or Update entity
        /// </summary>
        /// <param name="entity">entity to save or update</param>
        /// <returns>saved or updated entity</returns>
        public T SaveOrUpdate(T entity) {
            entity.ShouldNotBeNull("entity");

            if(IsDebugEnabled)
                log.Debug("엔티티를 추가 또는 갱신합니다. entity=[{0}]", entity);

            Session.SaveOrUpdate(entity);
            return entity;
        }

        /// <summary>
        /// Save or Update and Copy 
        /// </summary>
        /// <param name="entity">entity to save or update</param>
        /// <returns>an saved / updated entity</returns>
        public T SaveOrUpdateCopy(T entity) {
            entity.ShouldNotBeNull("entity");

            if(IsDebugEnabled)
                log.Debug("엔티티를 추가 또는 갱신합니다. entity=[{0}]", entity);

            return (T)Session.Merge(entity);
            //return (T) Session.SaveOrUpdateCopy(entity);
        }

        /// <summary>
        /// Save or Update and Copy 
        /// </summary>
        /// <param name="entity">entity</param>
        /// <param name="id">identity value of entity</param>
        /// <returns>an saved / updated entity</returns>
        public T SaveOrUpdateCopy(T entity, object id) {
            entity.ShouldNotBeNull("entity");
            id.ShouldNotBeNull("id");

            if(IsDebugEnabled)
                log.Debug("엔티티를 추가 또는 갱신합니다. entity=[{0}], id=[{1}]", entity, id);

            return (T)Session.Merge(entity);

            // return (T) Session.SaveOrUpdateCopy(entity, id);
        }

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity">entity to update</param>
        public void Update(T entity) {
            entity.ShouldNotBeNull("entity");

            if(IsDebugEnabled)
                log.Debug("엔티티를 갱신합니다. entity=[{0}]", entity);

            Session.Update(entity);
        }

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity">entity to update</param>
        /// <param name="id">identity value of entity to update</param>
        public void Update(T entity, object id) {
            entity.ShouldNotBeNull("entity");
            id.ShouldNotBeNull("id");

            if(IsDebugEnabled)
                log.Debug("엔티티를 갱신합니다. entity=[{0}], id=[{1}]", entity, id);

            Session.Update(entity, id);
        }

        #endregion

        #region << Merge / Replication >>

        /// <summary>
        /// SaveOrUpdate와는 달리 First Session에 이미 캐시되어 있는 엔티티라면, 최신 값으로 반영한 후 Save/Update를 수행한다.
        /// SaveOrUpdate는 Interceptor에서 엔티티 속성 값이 null로 바뀌는 문제가 있는 반면 Merge는 그렇지 않다.
        /// </summary>
        /// <param name="entity"></param>
        public void Merge(T entity) {
            entity.ShouldNotBeNull("entity");

            if(IsDebugEnabled)
                log.Debug("엔티티를 Merge (SaveOrUpdateCopy) 합니다.... entity=[{0}]", entity);

            Session.Merge(entity);
        }

        /// <summary>
        /// 다른 SessionFactory에 있는 현재 SessionFactory로 엔티티를 복제한다.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="replicateMode"></param>
        public void Replicate(T entity, ReplicationMode replicateMode) {
            entity.ShouldNotBeNull("entity");

            if(IsDebugEnabled)
                log.Debug("엔티티를 복제합니다... entity=[{0}], replicateMode=[{1}]", entity, replicateMode);

            Session.Replicate(entity, replicateMode);
        }

        #endregion

        #region << Delete / DeleteAll >>

        /// <summary>
        /// delete specified entity
        /// </summary>
        /// <param name="entity">entity to delete</param>
        public void Delete(T entity) {
            entity.ShouldNotBeNull("entity");

            if(IsDebugEnabled)
                log.Debug("엔티티를 삭제합니다... entity=[{0}]", entity);

            Session.Delete(entity);
        }

        /// <summary>
        /// delete entity by identity value
        /// </summary>
        /// <param name="id">Identity value of entity to delete</param>
        /// <param name="lockMode">Lock mode</param>
        public void Delete(object id, LockMode lockMode) {
            if(IsDebugEnabled)
                log.Debug("엔티티를 삭제합니다... entity=[{0}], id=[{1}], lockMode=[{2}]", ConcreteType, id, lockMode);

            var entity = Get(id, lockMode);

            if(Equals(entity, default(T)) == false)
                Delete(entity);
        }

        /// <summary>
        /// delete all entities using session - ExecuteUpdate("delete EntityName"); 을 사용하세요.
        /// </summary>
        public void DeleteAll() {
            if(IsDebugEnabled)
                log.Debug("모든 엔티티를 삭제합니다. ConcreteType=[{0}]", ConcreteType);

            Session.Delete(NHTool.SelectHql<T>());
        }

        /// <summary>
        /// delete entities matched with specified detached criteria
        /// </summary>
        /// <param name="criteria">The criteria to look for deleting</param>
        public void DeleteAll(DetachedCriteria criteria) {
            if(criteria == null) {
                DeleteAll();
                return;
            }

            foreach(var entity in FindAll(criteria))
                Session.Delete(entity);
        }

        /// <summary>
        /// delete entities matched with specified detached criteria
        /// </summary>
        /// <param name="queryOver">The criteria to look for deleting</param>
        public void DeleteAll(QueryOver<T> queryOver) {
            if(queryOver == null) {
                DeleteAll();
                return;
            }
            DeleteAll(queryOver.DetachedCriteria);
        }

        /// <summary>
        /// Criterion에 해당하는 모든 엔티티를 삭제한다.
        /// </summary>
        /// <param name="criterions"></param>
        public void DeleteAll(ICriterion[] criterions) {
            if(criterions == null || criterions.Length == 0)
                DeleteAll();
            else {
                foreach(var entity in FindAll(criterions))
                    Delete(entity);
            }
        }

        /// <summary>
        /// <paramref name="expressions"/>에 해당하는 모든 엔티티를 삭제한다. 복수의 조건은 AND 조건이다.
        /// </summary>
        public void DeleteAll(Expression<Func<T, bool>>[] expressions) {
            var queryOver = NHTool.CreateQueryOverOf(expressions);
            DeleteAll(queryOver);
        }

        /// <summary>
        /// 지정된 Parameter에 해당하는 모든 엔티티를 삭제한다. 복수의 조건은 AND 조건이다.
        /// </summary>
        /// <param name="parameters"></param>
        public void DeleteAll(INHParameter[] parameters) {
            if(parameters == null || parameters.Length == 0) {
                DeleteAll();
                return;
            }

            if(IsDebugEnabled)
                log.Debug("모든 엔티티를 삭제합니다... parameters=[{0}]", parameters.CollectionToString());

            var criteria = CreateDetachedCriteria();

            foreach(var param in parameters)
                criteria.AddEqOrNull(param.Name, param.Value);

            DeleteAll(criteria);
        }

        /// <summary>
        /// Delete entities by Named Query (defined at hbm.xml)
        /// </summary>
        /// <param name="namedQuery">named query to look for deleting</param>
        /// <param name="parameters">parameters</param>
        public void DeleteAll(string namedQuery, params INHParameter[] parameters) {
            namedQuery.ShouldNotBeWhiteSpace("namedQuery");

            if(IsDebugEnabled)
                log.Debug("NamedQuery에 해당하는 엔티티들을 삭제합니다... namedQuery=[{0}], parameters=[{1}]", namedQuery,
                          parameters.CollectionToString());

            foreach(var entity in FindAll(namedQuery, parameters))
                Session.Delete(entity);
        }

        /// <summary>
        /// Delete entities by HQL 
        /// </summary>
        /// <param name="hql">HQL string to look for deleting</param>
        /// <param name="parameters">parameters</param>
        public void DeleteAllByHql(string hql, params INHParameter[] parameters) {
            hql.ShouldNotBeWhiteSpace("hql");

            if(IsDebugEnabled)
                log.Debug("HQL에 해당하는 엔티티들을 삭제합니다... hql=[{0}], parameters=[{1}]", hql, parameters.CollectionToString());

            foreach(var entity in FindAllByHql(hql, parameters))
                Session.Delete(entity);
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
        public int ExecuteUpdate(string hql, params INHParameter[] parameters) {
            hql.ShouldNotBeWhiteSpace("hql");

            if(IsDebugEnabled)
                log.Debug("HQL을 Session을 거치지 않고, 직접 수행합니다. hql=[{0}], parameters=[{1}]", hql, parameters.CollectionToString());

            return Session.CreateQuery(hql, parameters).ExecuteUpdate();
        }

        /// <summary>
        /// <see cref="ExecuteUpdate"/>와 같은 일을 하지만, Transaction을 적용하여, 작업한다.
        /// </summary>
        /// <param name="hql">HQL statement. ex: delete Parent p where exists (from p.Children)</param>
        /// <param name="parameters">named parameters</param>
        /// <returns>number of entities effected by this operation</returns>
        /// <seealso cref="ExecuteUpdate"/>
        public int ExecuteUpdateTransactional(string hql, params INHParameter[] parameters) {
            hql.ShouldNotBeWhiteSpace("hql");

            if(IsDebugEnabled)
                log.Debug("ExecuteUpdate with Transaction... hql=[{0}], parameters=[{1}]", hql, parameters.CollectionToString());

            int result = 0;

            NHWith.Transaction(() => result = ExecuteUpdate(hql, parameters));
            return result;
        }

        #endregion

        #region << Entity Management >>

        /// <summary>
        /// Create an new instance of <typeparamref name="T"/>, mapping it to the concrete class if needed
        /// </summary>
        /// <returns>new instance</returns>
        public T Create() {
            if(IsDebugEnabled)
                log.Debug("엔티티 [{0}] 수형의 인스턴스를 생성합니다..." + _concreteType.FullName);

            return ActivatorTool.CreateInstance<T>();
        }

        /// <summary>
        /// Get Entity metadata
        /// </summary>
        /// <returns>Metadata of T</returns>
        public virtual IClassMetadata GetClassMetadata() {
            return SessionFactory.GetClassMetadata(ConcreteType);
        }

        /// <summary>
        /// the specified instance is transient object ?
        /// </summary>
        /// <returns>if the specified entity is transient object, return true. otherwise return false.</returns>
        public virtual bool IsTransient(T entity) {
            if(Equals(entity, default(T)))
                return true;

            // 이제 무조건 EntityStateInterceptor는 등록되어 사용되니까!!!
            // if(entity is IStateEntity && Session.GetSessionImplementation().Interceptor is EntityStateInterceptor)
            if(entity is IStateEntity)
                return ((IStateEntity)entity).IsTransient;


            return
                Session
                    .GetSessionImplementation()
                    .GetEntityPersister(entity.GetType().FullName, entity)
                    .IsTransient(entity, Session.GetSessionImplementation())
                    .GetValueOrDefault();
        }

        /// <summary>
        /// Create criteria for Entity
        /// </summary>
        /// <returns>instance of ICriteria for T</returns>
        public virtual ICriteria CreateCriteria() {
            return Session.CreateCriteria(ConcreteType);
        }

        /// <summary>
        /// Create detached criteria for Entity
        /// </summary>
        /// <returns>Instance of DetachedCriteria for T</returns>
        public DetachedCriteria CreateDetachedCriteria() {
            return DetachedCriteria.For<T>();
        }

        /// <summary>
        /// Create an aliases <see cref="DetachedCriteria"/> compatible with current Dao instance.
        /// </summary>
        /// <param name="alias">alias</param>
        /// <returns>Instance of <see cref="DetachedCriteria"/> which has alias for T</returns>
        public DetachedCriteria CreateDetachedCriteria(string alias) {
            return DetachedCriteria.For<T>(alias);
        }

        /// <summary>
        /// Create <see cref="IQuery"/> instance of current session with parameters.
        /// </summary>
        /// <param name="hql">HQL statement</param>
        /// <param name="parameters">named parameters</param>
        /// <returns></returns>
        public IQuery CreateQuery(string hql, params INHParameter[] parameters) {
            hql.ShouldNotBeWhiteSpace("hql");

            if(IsDebugEnabled)
                log.Debug("IQuery 인스턴스를 생성합니다... hql=[{0}], parameters=[{1}]", hql, parameters.CollectionToString());

            return Session.CreateQuery(hql, parameters);
        }

        /// <summary>
        /// IQueryOver 를 생성합니다.
        /// </summary>
        /// <returns></returns>
        public IQueryOver<T, T> CreateQueryOver() {
            return Session.QueryOver<T>();
        }

        /// <summary>
        /// IQueryOver 를 생성합니다.
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        public IQueryOver<T, T> CreateQueryOver(Expression<Func<T>> alias) {
            return Session.QueryOver<T>(alias);
        }

        /// <summary>
        ///  Detached QueryOver 생성 (<see cref="QueryOver.Of{T}()"/>)
        /// </summary>
        /// <returns></returns>
        public QueryOver<T, T> CreateQueryOverOf() {
            return QueryOver.Of<T>();
        }

        /// <summary>
        /// Detached QueryOver 생성 (<see cref="QueryOver.Of{T}()"/>)
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        public QueryOver<T, T> CreateQueryOverOf(Expression<Func<T>> alias) {
            if(alias == null)
                return QueryOver.Of<T>();

            return QueryOver.Of<T>(alias);
        }

        /// <summary>
        /// 엔티티 질의를 위해 LINQ의 <see cref="IQueryable{T}"/>를 반환합니다.
        /// </summary>
        public IQueryable<T> Query() {
            return Session.Query<T>();
        }

        #endregion

        private static void BuildProjectionCriteria<TProject>(ICriteria criteria, IProjection projectionList, bool distinctResults) {
            criteria.SetProjection(distinctResults ? Projections.Distinct(projectionList) : projectionList);

            if(typeof(TProject) != typeof(object[])) {
                if(IsDebugEnabled)
                    log.Debug("Criteria에 Projection을 위한 ResultTransformer를 설정합니다. Projection Type=[{0}]" + typeof(TProject).FullName);

                // Projection될 대상이 tuple (bject array)이 아니라면 result transformer를 적용한다.
                criteria.SetResultTransformer(IoC.IsInitialized
                                                  ? IoC.TryResolve<IResultTransformer, TypedResultTransformer<TProject>>()
                                                  : new TypedResultTransformer<TProject>());
            }
        }

        private static ICriteria CreateCriteria(ISession session) {
            session.ShouldNotBeNull("session");
            return session.CreateCriteria(_concreteType);
        }
    }
}