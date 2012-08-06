using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Criterion;
using NSoft.NFramework.LinqEx;

namespace NSoft.NFramework.Data.NHibernateEx {
    public static partial class NHTool {
        /// <summary>
        /// 지정한 엔티티를 StatelessSession을 이용하여 Insert 합니다.
        /// </summary>
        /// <typeparam name="T">type of entity to insert</typeparam>
        /// <param name="entity">entity to insert</param>
        /// <param name="session">nhibernate current session</param>
        public static void InsertStateless<T>(this T entity, ISession session) where T : IDataObject {
            entity.ShouldNotBeNull("entity");
            session.ShouldNotBeNull("session");

            if(IsDebugEnabled)
                log.Debug("엔티티를 Stateless Session에서 Insert 합니다... entity=[{0}], session=[{1}]", entity, session);

            NHWith.StatelessSession(session, stateless => stateless.Insert(entity));
        }

        /// <summary>
        ///지정한 엔티티를 StatelessSession을 이용하여 Insert 합니다. 
        /// </summary>
        /// <typeparam name="T">type of entity</typeparam>
        /// <param name="entity">entity to insert</param>
        public static void InsertStateless<T>(this T entity) where T : IDataObject {
            entity.ShouldNotBeNull("entity");
            InsertStateless(entity, UnitOfWork.CurrentSession);
        }

        /// <summary>
        /// 엔티티 컬렉션을 StatelessSession을 이용하여 Insert 합니다.
        /// </summary>
        /// <typeparam name="T">type of entity to insert</typeparam>
        /// <param name="entities">collection of entity to insert</param>
        /// <param name="session">nhibernate session</param>
        public static void InsertStateless<T>(this IEnumerable<T> entities, ISession session) where T : IDataObject {
            if(entities.ItemExists() == false)
                return;

            session.ShouldNotBeNull("session");

            if(IsDebugEnabled)
                log.Debug("엔티티 컬렉션을 Stateless Session에서 Insert 합니다... entities=[{0}], session=[{1}]", entities, session);

            NHWith.StatelessSessionNoTransaction(session,
                                                 stateless => entities.RunEach<T>(entity => stateless.Insert(entity)));
        }

        /// <summary>
        /// 엔티티 컬렉션을 StatelessSession을 이용하여 Insert 합니다.
        /// </summary>
        /// <typeparam name="T">type of entity to insert</typeparam>
        /// <param name="entities">collection of entity to insert</param>
        public static void InsertStateless<T>(this IEnumerable<T> entities) where T : IDataObject {
            if(entities.ItemExists() == false)
                return;

            InsertStateless(entities, UnitOfWork.CurrentSession);
        }

        /// <summary>
        /// 엔티티를 StatelessSession을 이용하여 Update 합니다.
        /// </summary>
        /// <typeparam name="T">type of entity to update</typeparam>
        /// <param name="entity">entity to update</param>
        /// <param name="session">nhibernate current session</param>
        public static void UpdateStateless<T>(this T entity, ISession session) where T : IDataObject {
            entity.ShouldNotBeNull("entity");
            session.ShouldNotBeNull("session");

            if(IsDebugEnabled)
                log.Debug("엔티티를 Stateless Session에서 Update 합니다... entity=[{0}], session=[{1}]", entity, session);

            NHWith.StatelessSession(session, stateless => stateless.Update(entity));
        }

        /// <summary>
        /// 엔티티를 StatelessSession을 이용하여 Update 합니다.
        /// </summary>
        /// <typeparam name="T">type of entity to update</typeparam>
        /// <param name="entity">entity to update</param>
        public static void UpdateStateless<T>(this T entity) where T : IDataObject {
            entity.ShouldNotBeNull("entity");
            UpdateStateless(entity, UnitOfWork.CurrentSession);
        }

        /// <summary>
        /// 엔티티 컬렉션을 StatelessSession을 이용하여 Update 합니다.
        /// </summary>
        /// <typeparam name="T">type of entity to update</typeparam>
        /// <param name="entities">collection of entity to update</param>
        /// <param name="session">nhibernate session</param>		
        public static void UpdateStateless<T>(this IEnumerable<T> entities, ISession session) where T : IDataObject {
            if(entities.ItemExists() == false)
                return;
            session.ShouldNotBeNull("session");

            if(IsDebugEnabled)
                log.Debug("엔티티 컬렉션을 StatelessSession에서 Update 합니다... entities=[{0}], session=[{1}]", entities, session);

            NHWith.StatelessSession(session, stateless => entities.RunEach(entity => stateless.Update(entity)));
        }

        /// <summary>
        /// 엔티티 컬렉션을 StatelessSession을 이용하여 Update 합니다.
        /// </summary>
        /// <typeparam name="T">type of entity to update</typeparam>
        /// <param name="entities">collection of entity to update</param>
        public static void UpdateStateless<T>(this IEnumerable<T> entities) where T : IDataObject {
            if(entities.ItemExists() == false)
                return;

            UpdateStateless(entities, UnitOfWork.CurrentSession);
        }

        /// <summary>
        /// 지정한 엔티티를 StatelessSession을 이용하여 Delete 합니다.
        /// </summary>
        /// <typeparam name="T">type of entity to delete</typeparam>
        /// <param name="entity">entity to delete</param>
        /// <param name="session">nhibernate current session</param>
        public static void DeleteStateless<T>(this T entity, ISession session) where T : IDataObject {
            entity.ShouldNotBeNull("entity");
            session.ShouldNotBeNull("session");

            if(IsDebugEnabled)
                log.Debug("엔티티를 StatelessSession에서 삭제합니다... entity=[{0}], session=[{1}]", entity, session);

            NHWith.StatelessSession(session, stateless => stateless.Delete(entity));
        }

        /// <summary>
        /// 지정한 엔티티를 StatelessSession을 이용하여 Delete 합니다.
        /// </summary>
        /// <typeparam name="T">type of entity to delete</typeparam>
        /// <param name="entity">entity to delete</param>
        public static void DeleteStateless<T>(this T entity) where T : IDataObject {
            entity.ShouldNotBeNull("entity");
            DeleteStateless(entity, UnitOfWork.CurrentSession);
        }

        /// <summary>
        /// 엔티티 컬렉션을 StatelessSession을 이용하여 Delete 합니다.
        /// </summary>
        /// <typeparam name="T">type of entity to delete</typeparam>
        /// <param name="entities">collection of entity to delete</param>
        /// <param name="session">nhibernate session</param>		
        public static void DeleteStateless<T>(this IEnumerable<T> entities, ISession session) where T : IDataObject {
            if(entities.ItemExists() == false)
                return;
            session.ShouldNotBeNull("session");

            if(IsDebugEnabled)
                log.Debug("엔티티 컬렉션을 StatelessSession에서 삭제합니다... entities=[{0}], session=[{1}]", entities, session);

            NHWith.StatelessSession(session, stateless => entities.RunEach(entity => stateless.Delete(entity)));
        }

        /// <summary>
        /// 엔티티 컬렉션을 StatelessSession을 이용하여 Delete 합니다.
        /// </summary>
        /// <typeparam name="T">type of entity to delete</typeparam>
        /// <param name="entities">collection of entity to delete</param>
        public static void DeleteStateless<T>(this IEnumerable<T> entities) where T : IDataObject {
            if(entities.ItemExists() == false)
                return;
            DeleteStateless(entities, UnitOfWork.CurrentSession);
        }

        /// <summary>
        /// Refresh entity
        /// </summary>
        public static void RefreshStateless<T>(this T entity, LockMode lockMode, ISession session) where T : IDataObject {
            entity.ShouldNotBeNull("entity");
            session.ShouldNotBeNull("session");

            NHWith.StatelessSession(session, stateless => stateless.Refresh(entity, lockMode));
        }

        /// <summary>
        /// Refresh entity
        /// </summary>
        public static void RefreshStateless<T>(this T entity, ISession session) where T : IDataObject {
            entity.ShouldNotBeNull("entity");
            session.ShouldNotBeNull("session");

            NHWith.StatelessSession(session, stateless => stateless.Refresh(entity));
        }

        /// <summary>
        /// Refresh entity
        /// </summary>
        public static void RefreshStateless<T>(this T entity, LockMode lockMode) where T : IDataObject {
            entity.ShouldNotBeNull("entity");
            NHWith.StatelessSession(stateless => stateless.Refresh(entity, lockMode));
        }

        /// <summary>
        /// Refresh entity
        /// </summary>
        public static void RefreshStateless<T>(this T entity) where T : IDataObject {
            entity.ShouldNotBeNull("entity");
            NHWith.StatelessSession(stateless => stateless.Refresh(entity));
        }

        /// <summary>
        /// Refresh entities
        /// </summary>
        public static void RefreshStateless<T>(this IEnumerable<T> entities, LockMode lockMode, ISession session) where T : IDataObject {
            if(entities.ItemExists() == false)
                return;

            NHWith.StatelessSession(session, stateless => entities.RunEach(entity => stateless.Refresh(entity, lockMode)));
        }

        /// <summary>
        /// Refresh entities
        /// </summary>
        public static void RefreshStateless<T>(this IEnumerable<T> entities, ISession session) where T : IDataObject {
            if(entities.ItemExists() == false)
                return;
            session.ShouldNotBeNull("session");

            NHWith.StatelessSession(session, stateless => entities.RunEach(entity => stateless.Refresh(entity)));
        }

        /// <summary>
        /// Stateless Session 을 이용하여, <typeparamref name="T"/>의 엔티티들을 모두 조회합니다.
        /// </summary>
        /// <typeparam name="T">조회할 엔티티의 수형</typeparam>
        public static IList<T> FindAllStateless<T>() where T : IDataObject {
            return FindAllStateless<T>(DetachedCriteria.For<T>(), UnitOfWork.CurrentSession);
        }

        /// <summary>
        /// Stateless Session 을 이용하여, <paramref name="criteria"/>에 해당하는 Entity를 조회합니다.
        /// </summary>
        public static IList<T> FindAllStateless<T>(this DetachedCriteria criteria) where T : IDataObject {
            return FindAllStateless<T>(criteria, UnitOfWork.CurrentSession);
        }

        /// <summary>
        /// Stateless Session 을 이용하여, <paramref name="criteria"/>에 해당하는 Entity를 조회합니다.
        /// </summary>
        public static IList<T> FindAllStateless<T>(this DetachedCriteria criteria, ISession session) where T : IDataObject {
            criteria.ShouldNotBeNull("criteria");
            criteria.ShouldNotBeNull("session");

            IList<T> result = null;

            NHWith.StatelessSessionNoTransaction(session,
                                                 stateless => { result = criteria.GetExecutableCriteria(stateless).List<T>(); });
            return result ?? new List<T>();
        }

        /// <summary>
        /// Stateless Session 을 이용하여, <paramref name="queryOver"/>에 해당하는 Entity를 조회합니다.
        /// </summary>
        public static IList<T> FindAllStateless<T>(this QueryOver<T> queryOver) where T : class, IDataObject {
            return FindAllStateless<T>(queryOver.DetachedCriteria, UnitOfWork.CurrentSession);
        }

        /// <summary>
        /// Stateless Session 을 이용하여, <paramref name="queryOver"/>에 해당하는 Entity를 조회합니다.
        /// </summary>
        public static IList<T> FindAllStateless<T>(this QueryOver<T> queryOver, ISession session) where T : class, IDataObject {
            return FindAllStateless<T>(queryOver.DetachedCriteria, session);
        }

        /// <summary>
        /// <paramref name="criteria"/>에 해당하는 엔티티가 존재하는지 검사합니다. 하나라도 있으면 True, 없으면 False
        /// </summary>
        public static bool ExistsStateless<T>(this DetachedCriteria criteria) where T : IDataObject {
            return ExistsStateless<T>(criteria, UnitOfWork.CurrentSession);
        }

        /// <summary>
        /// <paramref name="criteria"/>에 해당하는 엔티티가 존재하는지 검사합니다. 하나라도 있으면 True, 없으면 False
        /// </summary>
        public static bool ExistsStateless<T>(this DetachedCriteria criteria, ISession session) where T : IDataObject {
            criteria.ShouldNotBeNull("criteria");
            session.ShouldNotBeNull("session");

            var result = false;

            NHWith.StatelessSessionNoTransaction(session,
                                                 stateless => {
                                                     result =
                                                         criteria
                                                             .SetMaxResults(1)
                                                             .GetExecutableCriteria(stateless)
                                                             .List<T>()
                                                             .Any();
                                                 });

            return result;
        }

        /// <summary>
        /// <paramref name="queryOver"/>에 해당하는 엔티티가 존재하는지 검사합니다. 하나라도 있으면 True, 없으면 False
        /// </summary>
        public static bool ExistsStateless<T>(this QueryOver<T> queryOver) where T : class, IDataObject {
            return ExistsStateless<T>(queryOver, UnitOfWork.CurrentSession);
        }

        /// <summary>
        /// <paramref name="queryOver"/>에 해당하는 엔티티가 존재하는지 검사합니다. 하나라도 있으면 True, 없으면 False
        /// </summary>
        public static bool ExistsStateless<T>(this QueryOver<T> queryOver, ISession session) where T : class, IDataObject {
            return ExistsStateless<T>(queryOver.DetachedCriteria, session);
        }

        /// <summary>
        /// Id가 <paramref name="id"/>인 엔티티를 조회합니다.
        /// </summary>
        public static T GetStateless<T>(object id) where T : IDataObject {
            id.ShouldNotBeNull("id");

            var entity = default(T);
            NHWith.StatelessSessionNoTransaction(stateless => entity = stateless.Get<T>(id));
            return entity;
        }

        /// <summary>
        /// Id가 <paramref name="id"/>인 엔티티를 조회합니다.
        /// </summary>
        public static T GetStateless<T>(object id, LockMode lockMode) where T : IDataObject {
            id.ShouldNotBeNull("id");

            var entity = default(T);
            NHWith.StatelessSessionNoTransaction(stateless => entity = stateless.Get<T>(id, lockMode));
            return entity;
        }

        /// <summary>
        /// Id가 <paramref name="id"/>인 엔티티를 조회합니다.
        /// </summary>
        public static T GetStateless<T>(object id, ISession session) where T : IDataObject {
            id.ShouldNotBeNull("id");
            session.ShouldNotBeNull("session");

            var entity = default(T);
            NHWith.StatelessSessionNoTransaction(session, stateless => entity = stateless.Get<T>(id));

            return entity;
        }

        /// <summary>
        /// Id가 <paramref name="id"/>인 엔티티를 조회합니다.
        /// </summary>
        public static T GetStateless<T>(object id, LockMode lockMode, ISession session) where T : IDataObject {
            var entity = default(T);
            NHWith.StatelessSessionNoTransaction(session,
                                                 stateless => entity = stateless.Get<T>(id, lockMode));
            return entity;
        }

        /// <summary>
        /// 지정한 hql의 결과셋의 엔티티들을 삭제합니다.
        /// </summary>
        public static void DeleteStatelessByHql(string hql) {
            hql.ShouldNotBeWhiteSpace("hql");

            if(IsDebugEnabled)
                log.Debug("StatelessSession에서 HQL에 해당되는 엔티티를 삭제합니다... hql=[{0}]", hql);

            NHWith.StatelessSession(stateless => {
                                        foreach(var entity in stateless.CreateQuery(hql).List())
                                            stateless.Delete(entity);
                                    });
        }

        /// <summary>
        /// 지정한 queryName의 결과셋의 엔티티들을 삭제합니다
        /// </summary>
        public static void DeleteStatelessByNamedQuery(string queryName) {
            queryName.ShouldNotBeWhiteSpace("queryName");

            if(IsDebugEnabled)
                log.Debug("StatelessSession에서 NamedQuery를 실행하여 엔티티를 삭제합니다... queryName=[{0}]", queryName);

            NHWith.StatelessSession(stateless => {
                                        foreach(var entity in stateless.GetNamedQuery(queryName).Enumerable())
                                            stateless.Delete(entity);
                                    });
        }
    }
}