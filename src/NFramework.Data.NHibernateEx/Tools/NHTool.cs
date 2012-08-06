using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Iesi.Collections.Generic;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Criterion;
using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.Proxy;
using NHibernate.Transform;
using NSoft.NFramework.Data.NHibernateEx.Domain;
using NSoft.NFramework.DynamicProxy;
using NSoft.NFramework.InversionOfControl;
using NSoft.NFramework.Reflections;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Data.NHibernateEx {
    /// <summary>
    /// NHibernate 를 사용하기 위한 Helper class
    /// </summary>
    public static partial class NHTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public const string EntityWrapperChar = "`";

        private static readonly object _syncLock = new object();

        /// <summary>
        /// 지정된 NHibernate용 환경설정파일을 이용하여 <see cref="Configuration"/>을 빌드합니다.
        /// </summary>
        /// <param name="configFilePath">NHibernate 환경설정 정보를 가진 파일의 전체 경로</param>
        /// <returns>환경설정 정보</returns>
        public static Configuration BuildConfiguration(this string configFilePath) {
            configFilePath.ShouldNotBeWhiteSpace("configFilePath");

            if(IsDebugEnabled)
                log.Debug("NHibernate 환경설정을 빌드합니다. 환경설정파일=[{0}]", configFilePath);

            return new Configuration().Configure(configFilePath);
        }

        /// <summary>
        /// NHibernate configuration에서 mapping된 Assembly들을 조회한다.
        /// </summary>
        /// <param name="configuration">NHibernate Configuration 파일</param>
        /// <returns></returns>
        public static Assembly[] GetMappingAssemblies(this Configuration configuration) {
            configuration.ShouldNotBeNull("configuration");

            var loadedAssembly = new HashedSet<Assembly>();

            foreach(var persistentClass in configuration.ClassMappings) {
                if(log.IsDebugEnabled)
                    log.Debug("매핑 Assembly를 추가합니다. assembly=[{0}]", persistentClass.MappedClass.Assembly);

                loadedAssembly.Add(persistentClass.MappedClass.Assembly);
            }

            return loadedAssembly.ToArray();
        }

        public static Type GetEntityType(this ISessionFactory sessionFactory, string entityName, EntityMode entityMode) {
            return sessionFactory.GetClassMetadata(entityName).GetMappedClass(entityMode);
        }

        /// <summary>
        /// 지정된 UnitOfWork Factory의 session factory name을 가져온다.
        /// </summary>
        /// <param name="uowFactory"></param>
        /// <returns></returns>
        public static string GetSessionFactoryName(this IUnitOfWorkFactory uowFactory) {
            return uowFactory.SessionFactory.GetSessionFactoryName();
        }

        /// <summary>
        /// 지정된 SessionFactory의 session factory name을 가져온다.
        /// </summary>
        /// <param name="sessionFactory"></param>
        /// <returns></returns>
        public static string GetSessionFactoryName(this ISessionFactory sessionFactory) {
            return ((ISessionFactoryImplementor)sessionFactory).Settings.SessionFactoryName;
        }

        /// <summary>
        /// <see cref="ISessionFactory"/>의 설정 정보들을 가져옵니다.
        /// </summary>
        public static Settings GetSessionFactorySettings(this ISessionFactory sessionFactory) {
            return ((ISessionFactoryImplementor)sessionFactory).Settings;
        }

        /// <summary>
        /// 지정된 엔티티 형식이 <paramref name="sessionFactory"/>에 매핑된 엔티티인지 파악합니다.
        /// </summary>
        /// <typeparam name="T">엔티티의 형식</typeparam>
        /// <param name="sessionFactory">엔티티가 매핑되었을 SessionFactory</param>
        /// <returns>SessionFactory에 엔티티 형식이 매핑되었으면 True, 아니면 False</returns>
        /// <seealso cref="NHibernateProxyHelper.GetClassWithoutInitializingProxy"/>
        public static bool IsMappedEntity<T>(this ISessionFactory sessionFactory) where T : class {
            return sessionFactory.IsMappedEntity(typeof(T));
        }

        /// <summary>
        /// 지정된 엔티티 형식이 <paramref name="sessionFactory"/>에 매핑된 엔티티인지 파악합니다.
        /// </summary>
        /// <param name="sessionFactory">엔티티가 매핑되었을 SessionFactory</param>
        /// <param name="entityType">엔티티의 형식</param>
        /// <returns>SessionFactory에 엔티티 형식이 매핑되었으면 True, 아니면 False</returns>
        /// <seealso cref="NHibernateProxyHelper.GetClassWithoutInitializingProxy"/>
        public static bool IsMappedEntity(this ISessionFactory sessionFactory, Type entityType) {
            if(entityType == null)
                return false;

            if(IsDebugEnabled)
                log.Debug("Current SessionFactory[{0}]에 엔티티 형식[{1}]가 매핑되었는지 확인합니다...",
                          sessionFactory.GetSessionFactoryName(), entityType.FullName);

            return sessionFactory.GetClassMetadata(entityType.GetUnproxiedType()) != null;
        }

        /// <summary>
        /// 지정된 엔티티가 <paramref name="sessionFactory"/>에 매핑된 엔티티인지 파악합니다.
        /// </summary>
        /// <param name="sessionFactory">엔티티가 매핑되었을 SessionFactory</param>
        /// <param name="entity">엔티티</param>
        /// <returns>SessionFactory에 엔티티 형식이 매핑되었으면 True, 아니면 False</returns>
        /// <seealso cref="NHibernateProxyHelper.GetClassWithoutInitializingProxy"/>
        public static bool IsMappedEntity(this ISessionFactory sessionFactory, IDataObject entity) {
            if(entity == null)
                return false;

            // NHibernateProxyHelper를 이용하여 엔티티를 Initialize하지 않고도, 원본 수형을 찾을 수 있다.
            var entityType = NHibernateProxyHelper.GetClassWithoutInitializingProxy(entity);
            return IsMappedEntity(sessionFactory, entityType);

            // return IsMappedEntity(sessionFactory, entity.GetType().GetUnproxiedType());
        }

        /// <summary>
        /// <see cref="ISessionFactory"/>의 설정 정보 중 <see cref="NHibernate.Dialect.Dialect"/> 정보를 가져옵니다.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public static NHibernate.Dialect.Dialect GetDialect(this ISession session) {
            return session.SessionFactory.GetDialect();
        }

        /// <summary>
        /// <see cref="ISessionFactory"/>의 설정 정보 중 <see cref="NHibernate.Dialect.Dialect"/> 정보를 가져옵니다.
        /// </summary>
        /// <param name="sessionFactory"></param>
        /// <returns></returns>
        public static NHibernate.Dialect.Dialect GetDialect(this ISessionFactory sessionFactory) {
            return ((ISessionFactoryImplementor)sessionFactory).Settings.Dialect;
        }

        /// <summary>
        /// Get <see cref="ICriteria"/> from a given detached criteria
        /// </summary>
        /// <param name="session">NHibernate ISession</param>
        /// <param name="criteria">detached criteria</param>
        /// <param name="orders">ordering 정보</param>
        /// <returns>Entity를 조회할 Criteria</returns>
        public static ICriteria GetExecutableCriteria<T>(this ISession session, DetachedCriteria criteria, params Order[] orders) {
            session.ShouldNotBeNull("session");

            return
                GetExecutableCriteria<T>(session,
                                         criteria,
                                         sess => sess.CreateCriteria(typeof(T)),
                                         orders);
        }

        /// <summary>
        /// Get <see cref="ICriteria"/> from a given detached criteria. 
        /// if detached criteria is null, create criteria by CriteriaCreator delegate
        /// </summary>
        /// <param name="session">NHibernate ISession</param>
        /// <param name="criteria">detached criteria</param>
        /// <param name="criteriaFactory">지정된 Session을 사용하여 ICriteria를 생성할 대리자. 추가작업을 위해 ICriteria 생성작업을 위임한다.</param>
        /// <param name="orders">ordering 정보</param>
        /// <returns>Entity를 조회할 Criteria</returns>
        public static ICriteria GetExecutableCriteria<T>(this ISession session,
                                                         DetachedCriteria criteria,
                                                         Func<ISession, ICriteria> criteriaFactory,
                                                         params Order[] orders) {
            session.ShouldNotBeNull("session");

            ICriteria executableCriteria;

            if(criteria != null)
                executableCriteria = criteria.GetExecutableCriteria(session);
            else if(criteriaFactory != null)
                executableCriteria = criteriaFactory(session);
            else
                throw new ArgumentNullException("criteria", "given criteria and criteriaFactory is null.");

            // executableCriteria = ApplyFetchingStrategies<T>(executableCriteria);
            AddCaching(executableCriteria);

            // Order는 caching하지 않는다
            if(orders != null && orders.Length > 0)
                foreach(var order in orders)
                    executableCriteria.AddOrder(order);

            return executableCriteria;
        }

        /// <summary>
        /// Create a <see cref="ICriteria"/> from criterion array
        /// </summary>
        /// <param name="session">NHibernate ISession</param>
        /// <param name="criterions">arary of ICriterion</param>
        /// <param name="orders">ordering criteria</param>
        public static ICriteria CreateCriteria<T>(this ISession session,
                                                  ICriterion[] criterions,
                                                  params Order[] orders) {
            session.ShouldNotBeNull("session");

            return CreateCriteria<T>(session,
                                     criterions,
                                     sess => sess.CreateCriteria(typeof(T)),
                                     orders);
        }

        /// <summary>
        /// Create a <see cref="ICriteria"/> from criterion array
        /// </summary>
        /// <param name="session">NHibernate ISession</param>
        /// <param name="criterions">arary of ICriterion</param>
        /// <param name="criteriaFactory">지정된 Session을 사용하여 ICriteria를 생성할 대리자. 추가작업을 위해 ICriteria 생성작업을 위임한다.</param>
        /// <param name="orders">ordering criteria</param>
        /// <returns>Entity를 조회할 Criteria</returns>
        public static ICriteria CreateCriteria<T>(this ISession session,
                                                  ICriterion[] criterions,
                                                  Func<ISession, ICriteria> criteriaFactory,
                                                  params Order[] orders) {
            session.ShouldNotBeNull("session");

            if(IsDebugEnabled)
                log.Debug("조회할 질의를 빌드합니다.... entity=[{0}], session=[{1}], criterions=[{2}], orders=[{3}]",
                          typeof(T).FullName, session, criterions.CollectionToString(), orders.CollectionToString());

            var crit = session.CreateCriteria(typeof(T));

            if(criterions != null && criterions.Length > 0)
                foreach(var criterion in criterions)
                    if(criterion != null)
                        crit.Add(criterion);

            // crit = ApplyFetchingStrategies<T>(crit);
            AddCaching(crit);

            if(orders != null && orders.Length > 0)
                foreach(var order in orders)
                    crit.AddOrder(order);

            return crit;
        }

        /// <summary>
        /// IoC를 이용하여 FetchingStrategy 가 등록되어 있다면 지정된 Criteria에 적용시킨다.
        /// </summary>
        /// <typeparam name="T">Type of entity</typeparam>
        /// <param name="crit">instance of ICriteria</param>
        /// <returns>FetchingStrategy가 적용된 ICriteria</returns>
        public static ICriteria ApplyFetchingStrategies<T>(ICriteria crit) {
            crit.ShouldNotBeNull("crit");

            try {
                var fetchingStrategies = IoC.ResolveAll<IFetchingStrategy<T>>();

                if(fetchingStrategies != null) {
                    foreach(IFetchingStrategy<T> strategy in fetchingStrategies) {
                        crit = strategy.Apply(crit);

                        if(IsDebugEnabled)
                            log.Debug("Apply fetching strategy. fetching strategy=[{0}]", strategy);
                    }
                }
            }
            catch(Exception ex) {
                if(log.IsWarnEnabled) {
                    log.Warn(ex);
                }
            }

            return crit;
        }

        /// <summary>
        /// Cache the specified criteria
        /// </summary>
        /// <param name="crit">criteria instance to be cached.</param>
        public static void AddCaching(this ICriteria crit) {
            crit.ShouldNotBeNull("crit");

            if(NHWith.Caching.ShouldForceCacheRefresh == false && NHWith.Caching.Enabled) {
                if(IsDebugEnabled)
                    log.Debug("Cache for the specified criteria.");

                crit.SetCacheable(true);

                var cacheRegion = NHWith.Caching.CurrentCacheRegion;
                if(cacheRegion != null) {
                    if(IsDebugEnabled)
                        log.Debug("Set cache region to the specified criteria. Current Cache Region=[{0}]", cacheRegion);

                    crit.SetCacheRegion(cacheRegion);
                }
            }
        }

        /// <summary>
        /// Get Detached Criteria for Association Path
        /// </summary>
        /// <param name="dc">detached criteria</param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static DetachedCriteria Path(this DetachedCriteria dc, string path) {
            return dc.GetCriteriaByPath(path) ?? dc.CreateCriteria(path);
        }

        /// <summary>
        /// Create a <see cref="DetachedQuery"/> by hql statement with parameters
        /// </summary>
        /// <param name="hql">실행할 HQL 문</param>
        /// <param name="parameters">파리미터</param>
        /// <returns>쿼리 객체</returns>
        public static IDetachedQuery CreateDetachedQuery(string hql, params INHParameter[] parameters) {
            hql.ShouldNotBeWhiteSpace("hql");

            var detachedQuery = new DetachedQuery(hql);

            if(parameters != null && parameters.Length > 0)
                SetDetachedQueryParameters(detachedQuery, parameters);

            return detachedQuery;
        }

        /// <summary>
        /// Create a <see cref="DetachedNamedQuery"/>  by queryName with parameters.
        /// </summary>
        /// <param name="queryName">실행할 쿼리명</param>
        /// <param name="parameters">파라미터</param>
        /// <returns>쿼리 객체</returns>
        public static IDetachedQuery CreateDetachedNamedQuery(string queryName, params INHParameter[] parameters) {
            queryName.ShouldNotBeWhiteSpace("queryName");

            var detachedNamedQuery = new DetachedNamedQuery(queryName);

            if(parameters != null && parameters.Length > 0)
                SetDetachedQueryParameters(detachedNamedQuery, parameters);

            return detachedNamedQuery;
        }

        /// <summary>
        /// Create <see cref="IQuery"/> by Named Query which defined in mapping file (*.hbm.xml)
        /// </summary>
        /// <param name="session">current session</param>
        /// <param name="namedQuery">name of NamedQuery</param>
        /// <param name="parameters">collection of <see cref="INHParameter"/></param>
        /// <returns><see cref="IQuery"/>instance of IQuery</returns>
        /// <exception cref="Exception">named query is not defined.</exception>
        public static IQuery GetNamedQuery(this ISession session, string namedQuery, params INHParameter[] parameters) {
            session.ShouldNotBeNull("session");
            namedQuery.ShouldNotBeWhiteSpace("namedQuery");

            if(IsDebugEnabled)
                log.Debug("NamedQuery를 조회합니다... namedQuery=[{0}], parameters=[{1}]", namedQuery, parameters.CollectionToString());

            var query = session.GetNamedQuery(namedQuery);

            if(parameters != null && parameters.Length > 0)
                SetQueryParameters(query, parameters);

            // add to cache if available.
            AddCaching(query);

            return query;
        }

        /// <summary>
        /// HBM에 정의된 NamedQuery를 로드하여 IQuery를 빌드하고, 결과 셋을 TEntity 수형으로 변환해주는 ResultTransformer를 설정하여 반환합니다.
        /// Stored Procedure 실행 결과를 Class로 바로 매핑할 때 사용하면 좋습니다.
        /// </summary>
        /// <param name="session">current session</param>
        /// <param name="namedQuery">name of NamedQuery</param>
        /// <param name="parameters">collection of <see cref="INHParameter"/></param>
        /// <returns><see cref="IQuery"/>instance of IQuery</returns>
        /// <exception cref="Exception">named query is not defined.</exception>
        public static IQuery GetNamedQuery<TEntity>(this ISession session, string namedQuery, params INHParameter[] parameters) {
            var query = GetNamedQuery(session, namedQuery, parameters);

            // TypedResultTransformer는 속도가 빠른 반면, DynamicResultTransformer는 안정성이 더 좋습니다.
            query.SetResultTransformer(IoC.TryResolve<IResultTransformer, DynamicResultTransformer<TEntity>>());

            return query;
        }

        /// <summary>
        /// Create <see cref="IQuery"/> by simple query string
        /// </summary>
        /// <param name="session">current session</param>
        /// <param name="queryString">simple query string</param>
        /// <param name="parameters">collection of <see cref="INHParameter"/></param>
        /// <returns><see cref="IQuery"/>instance of IQuery</returns>
        /// <returns>instance of IQuery</returns>
        public static IQuery CreateQuery(this ISession session, string queryString, params INHParameter[] parameters) {
            session.ShouldNotBeNull("session");
            queryString.ShouldNotBeWhiteSpace("queryString");

            if(IsDebugEnabled)
                log.Debug("쿼리 문으로 IQuery 인스턴스를 생성합니다... queryString=[{0}], parameters=[{1}]", queryString,
                          parameters.CollectionToString());

            var query = session.CreateQuery(queryString);

            if(parameters != null && parameters.Length > 0)
                SetQueryParameters(query, parameters);

            // add to cache if available.
            AddCaching(query);

            return query;
        }

        /// <summary>
        /// Query Cahcing 작업을 상세하게 제어합니다.
        /// </summary>
        /// <param name="query">instance of IQuery to be cached.</param>
        public static void AddCaching(this IQuery query) {
            query.ShouldNotBeNull("query");

            if(NHWith.Caching.ShouldForceCacheRefresh == false && NHWith.Caching.Enabled) {
                if(IsDebugEnabled)
                    log.Debug("Set the specified query to be cacheable.");

                query.SetCacheable(true);

                if(NHWith.Caching.CurrentCacheRegion != null)
                    query.SetCacheRegion(NHWith.Caching.CurrentCacheRegion);
            }
            else if(NHWith.Caching.ShouldForceCacheRefresh) {
                query.SetCacheMode(CacheMode.Refresh);

                if(IsDebugEnabled)
                    log.Debug("IQuery 인스턴스의 CacheMode를 Refresh로 설정했습니다.");
            }
        }

        /// <summary>
        /// Get HQL for <typeparamref name="T"/> (ie. " from SomeNamespce.SomeClass")
        /// </summary>
        /// <typeparam name="T">Type of entity</typeparam>
        /// <returns>HQL string to get all entity which type is the specified Type</returns>
        /// <example>
        /// <code>
        ///		var CustomerHql = NHTool.SelectHql&lt;Northwind.DataObjectBase.Customer&gt;(); 
        ///		// CustomerHql is [from Northwind.DataObjectBase.Customer]
        /// </code>
        /// </example>
        public static string SelectHql<T>() {
            return @" from " + typeof(T).FullName;
        }

        private static void SetQueryParameters(IQuery query, params INHParameter[] parameters) {
            query.ShouldNotBeNull("query");

            if(parameters == null || parameters.Length == 0)
                return;

            foreach(var parameter in parameters) {
                if(parameter.Type == null)
                    query.SetParameter(parameter.Name, parameter.Value);
                else
                    query.SetParameter(parameter.Name, parameter.Value, parameter.Type);

                if(IsDebugEnabled)
                    log.Debug("Set query parameter... Name=[{0}], Value=[{1}], Type=[{2}]", parameter.Name, parameter.Value,
                              parameter.Type);
            }
        }

        private static void SetDetachedQueryParameters(IDetachedQuery detachedQuery, params INHParameter[] parameters) {
            detachedQuery.ShouldNotBeNull("detachedQuery");

            if(parameters == null || parameters.Length == 0)
                return;

            foreach(var parameter in parameters) {
                if(parameter.Type == null)
                    detachedQuery.SetParameter(parameter.Name, parameter.Value);
                else
                    detachedQuery.SetParameter(parameter.Name, parameter.Value, parameter.Type);

                if(IsDebugEnabled)
                    log.Debug("쿼리에 Parameter를 설정합니다... Name=[{0}], Value=[{1}], Type=[{2}]", parameter.Name, parameter.Value,
                              parameter.Type);
            }
        }

        /// <summary>
        /// 지정된 Entity의 속성 정보만을 복제하고 (Association 정보는 복제하지 않습니다), Transient Object로 변경한다. (Identifier 값이 없다는 애기지요)
        /// NOTE : 다만 Association은 Transient로 변경하지 못하므로, 따로 해주어야 한다.
        /// </summary>
        /// <typeparam name="T">복사할 엔티티의 수형</typeparam>
        /// <param name="entity">원본 엔티티 인스턴스</param>
        /// <returns>Trasient object로 상태가 된 복제 엔티티</returns>
        public static T CopyAndToTransient<T>(this T entity) where T : class, IStateEntity {
            // BUG: DeepCopy를 수행하면, Association관련 정보도 복제가 됩니다. 이렇게 되면 상대 객체가 잘못된 Association을 가질 수 있습니다.
            //var result = RwSerializer.DeepCopy(entity);

            var result = entity.MapDataObject<T>();
            result.ToTransient();

            return result;
        }

        /// <summary>
        /// 현재 Session (First Cache)에 로드된 관련 Entity들을 열거합니다.
        /// </summary>
        /// <typeparam name="T">엔티티 수형</typeparam>
        /// <param name="session">현재 Session</param>
        /// <returns>현재 Session에서 엔티티</returns>
        /// <example>
        /// <code>
        /// var localParent = session.Local{Parent}();
        /// </code>
        /// </example>
        public static IEnumerable<T> Local<T>(this ISession session) {
            ISessionImplementor sessionImpl = session.GetSessionImplementation();

            var context = sessionImpl.PersistenceContext;

            return context.EntityEntries.Keys.OfType<T>().Select(key => key);
        }

        /// <summary>
        /// 지정된 <paramref name="sessionFactory"/>가 사용하는 Database가 MS SQL Server 인지 파악합니다.
        /// </summary>
        /// <param name="sessionFactory"></param>
        /// <returns></returns>
        public static bool IsMsSqlServer(this ISessionFactory sessionFactory) {
            return sessionFactory.GetDialect() is MsSql2000Dialect;
        }

        /// <summary>
        /// 지정된 <paramref name="sessionFactory"/>가 사용하는 Database가 MS SQL Server 2005 이상 인지 파악합니다.
        /// </summary>
        public static bool IsMsSqlServer2005OrHigher(this ISessionFactory sessionFactory) {
            return sessionFactory.GetDialect() is MsSql2005Dialect;
        }

        /// <summary>
        /// 지정된 <paramref name="sessionFactory"/>가 사용하는 Database가 Oracle 인지 파악합니다.
        /// </summary>
        public static bool IsOracle(this ISessionFactory sessionFactory) {
            return sessionFactory.GetDialect() is Oracle9iDialect;
        }

        /// <summary>
        /// 지정된 <paramref name="sessionFactory"/>가 사용하는 Database가 SQLite 인지 파악합니다.
        /// </summary>
        public static bool IsSQLite(this ISessionFactory sessionFactory) {
            return sessionFactory.GetDialect() is SQLiteDialect;
        }

        /// <summary>
        /// 지정된 <paramref name="sessionFactory"/>가 사용하는 Database가 MS SQL CE 인지 파악합니다.
        /// </summary>
        public static bool IsMsSqlCe(this ISessionFactory sessionFactory) {
            return sessionFactory.GetDialect() is NHibernate.Dialect.MsSqlCeDialect;
        }

        /// <summary>
        ///  지정된 컬렉션의 엔티티와 속성들을 <see cref="NHibernateUtil.Initialize"/>를 이용하여 초기화 합니다. ( proxy 값을 실제 값으로 대체한다는 소리!!!)
        /// </summary>
        /// <typeparam name="T">엔티티 수형</typeparam>
        /// <param name="entities">엔티티 시퀀스</param>
        /// <returns>초기화된 엔티티 컬렉션</returns>
        public static IList<T> InitializeEntities<T>(this IList<T> entities) {
            return InitializeEntities(entities, false);
        }

        /// <summary>
        ///  지정된 컬렉션의 엔티티와 속성들을 <see cref="NHibernateUtil.Initialize"/>를 이용하여 초기화 합니다. ( proxy 값을 실제 값으로 대체한다는 소리!!!)
        /// </summary>
        /// <typeparam name="T">엔티티 수형</typeparam>
        /// <param name="entities">엔티티 시퀀스</param>
        /// <param name="forceInitialize">Initialize가 되었던 안되었던 강제로 초기화를 합니다.</param>
        /// <returns>초기화된 엔티티 컬렉션</returns>
        public static IList<T> InitializeEntities<T>(this IList<T> entities, bool forceInitialize) {
            if(NHibernateUtil.IsInitialized(entities) && forceInitialize == false)
                return entities;

            if(IsDebugEnabled)
                log.Debug("NHibernate 엔티티 컬렉션을 Initialize하여 Proxy에서 실제 Data를 로딩하도록 합니다... forceInitialized=[{0}]", forceInitialize);

            var dynamicAccessor = DynamicAccessorFactory.CreateDynamicAccessor<T>(true);
            var propertyNames = dynamicAccessor.GetPropertyNames();

            NHibernateUtil.Initialize(entities);

            var ignorePropertyNames = new List<string>();

            foreach(T item in entities) {
                if(forceInitialize || NHibernateUtil.IsInitialized(item) == false) {
                    NHibernateUtil.Initialize(item);

                    foreach(var propertyName in propertyNames) {
                        if(ignorePropertyNames.Contains(propertyName))
                            continue;

                        var propertyType = dynamicAccessor.PropertyMap[propertyName].PropertyType;
                        if(propertyType.IsSimpleType()) {
                            ignorePropertyNames.Add(propertyName);
                            continue;
                        }

                        if(forceInitialize || NHibernateUtil.IsPropertyInitialized(item, propertyName) == false) {
                            NHibernateUtil.Initialize(dynamicAccessor.GetPropertyValue(item, propertyName));
                        }
                    }
                }
            }

            return entities;
        }

        /// <summary>
        /// 지정된 엔티티와 속성들을 <see cref="NHibernateUtil.Initialize"/>를 이용하여 초기화 합니다. ( proxy 값을 실제 값으로 대체한다는 소리!!!)
        /// </summary>
        /// <typeparam name="T">초기화할 엔티티의 수형</typeparam>
        /// <param name="entity">초기화할 엔티티</param>
        /// <returns>초기화된 엔티티</returns>
        public static T InitializeEntity<T>(this T entity) where T : IDataObject {
            return InitializeEntity(entity, false);
        }

        /// <summary>
        /// 지정된 엔티티와 속성들을 <see cref="NHibernateUtil.Initialize"/>를 이용하여 초기화 합니다. ( proxy 값을 실제 값으로 대체한다는 소리!!!)
        /// </summary>
        /// <typeparam name="T">초기화할 엔티티의 수형</typeparam>
        /// <param name="entity">초기화할 엔티티</param>
        /// <param name="forceInitialize">강제 초기화</param>
        /// <returns>초기화된 엔티티</returns>
        public static T InitializeEntity<T>(this T entity, bool forceInitialize) where T : IDataObject {
            // NOTE : IsInitialized, IsPropertyInitialized 가 제대로 수행되지 않는다. 그래서 무조건 항상 Initialize를 수행하도록 했다.

            if(forceInitialize == false && NHibernateUtil.IsInitialized(entity))
                return entity;

            if(IsDebugEnabled)
                log.Debug("NHibernate 엔티티를 Initialize하여 Proxy에서 실제 Data를 로딩하도록 합니다... entity=[{0}]", entity);

            var dynamicAccessor = DynamicAccessorFactory.CreateDynamicAccessor<T>(true);
            var propertyNames = dynamicAccessor.GetPropertyNames();

            NHibernateUtil.Initialize(entity);

            foreach(var propertyName in propertyNames) {
                if(forceInitialize || NHibernateUtil.IsPropertyInitialized(entity, propertyName) == false)
                    NHibernateUtil.Initialize(dynamicAccessor.GetPropertyValue(entity, propertyName));
            }

            return entity;
        }
    }
}