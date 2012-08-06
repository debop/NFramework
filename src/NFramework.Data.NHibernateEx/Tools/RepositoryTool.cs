using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NHibernate.Criterion;
using NSoft.NFramework.Data.NHibernateEx.Domain;
using NSoft.NFramework.Reflections;

namespace NSoft.NFramework.Data.NHibernateEx {
    /// <summary>
    /// 범용의 Domain 관련 Utiltiy class
    /// </summary>
    public static partial class RepositoryTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 지정된 조건에 해당하는 유일한 결과를 조회한다. 결과가 두 개 이상이면 예외를 발생시킨다
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static T FindOne<T>(params INHParameter[] parameters) where T : class, IDataObject {
            if(IsDebugEnabled)
                log.Debug("FindOne entity. entity=[{0}], parameters=[{1}]", typeof(T).Name, parameters.CollectionToString());

            var crit = DetachedCriteria.For<T>();

            if(parameters != null)
                foreach(var p in parameters)
                    crit.AddEqOrNull(p.Name, p.Value);

            return Repository<T>.FindOne(crit);
        }

        /// <summary>
        /// 지정된 조건에 해당하는 유일한 결과를 조회한다. 결과가 두 개 이상이면 예외를 발생시킨다
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expressions">조건 표현 Lambda 식</param>
        /// <returns></returns>
        public static T FindOne<T>(params Expression<Func<T, bool>>[] expressions) where T : class, IDataObject {
            if(IsDebugEnabled)
                log.Debug("FindOne entity. entity=[{0}], expressions=[{1}]", typeof(T).Name, expressions.CollectionToString());

            var query = QueryOver.Of<T>();

            if(expressions != null)
                foreach(var expr in expressions)
                    query.AddWhere(expr);

            return query.GetExecutableQueryOver(UnitOfWork.CurrentSession).SingleOrDefault();
        }

        /// <summary>
        /// 특정 속성값이 일치하는 모든 Entity 정보를 Load한다.
        /// </summary>
        /// <typeparam name="T">검색하고자하는 Entity의 수형</typeparam>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static IList<T> FindAll<T>(params INHParameter[] parameters) where T : class, IDataObject {
            return FindAll<T>(null, null, null, parameters);
        }

        /// <summary>
        /// 특정 속성값이 일치하는 모든 Entity 정보를 Load한다.
        /// </summary>
        /// <typeparam name="T">검색하고자하는 Entity의 수형</typeparam>
        /// <param name="firstResult">결과 셋의 첫번째 레코드의 인덱스(0부터 시작)</param>
        /// <param name="maxResults">결과 셋의 최대 크기</param>
        /// <param name="orders">결과 셋의 정렬 순서</param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static IList<T> FindAll<T>(int? firstResult, int? maxResults, Order[] orders, params INHParameter[] parameters)
            where T : class, IDataObject {
            if(IsDebugEnabled)
                log.Debug("FindAll entities. entity=[{0}], firstResult=[{1}], maxResults=[{2}], orders=[{3}], parameters=[{4}]",
                          typeof(T).Name, firstResult, maxResults, orders.CollectionToString(), parameters.CollectionToString());

            var crit = DetachedCriteria.For<T>();

            if(parameters != null)
                foreach(var p in parameters)
                    crit.AddEqOrNull(p.Name, p.Value);

            return Repository<T>.FindAll(crit,
                                         firstResult.GetValueOrDefault(0),
                                         maxResults.GetValueOrDefault(0),
                                         orders);
        }

        /// <summary>
        /// 조건 람다 식이 일치하는 모든 Entity 정보를 Load한다.
        /// </summary>
        /// <typeparam name="T">검색하고자하는 Entity의 수형</typeparam>
        /// <param name="expressions">조건 식</param>
        /// <returns></returns>
        public static IList<T> FindAll<T>(params Expression<Func<T, bool>>[] expressions) where T : class, IDataObject {
            return FindAll<T>(null, null, expressions);
        }

        /// <summary>
        /// 조건 람다 식이 일치하는 모든 Entity 정보를 Load한다.
        /// </summary>
        /// <typeparam name="T">검색하고자하는 Entity의 수형</typeparam>
        /// <param name="firstResult">결과 셋의 첫번째 레코드의 인덱스(0부터 시작)</param>
        /// <param name="maxResults">결과 셋의 최대 크기</param>
        /// <param name="expressions">조건 절 람다 식</param>
        /// <returns></returns>
        public static IList<T> FindAll<T>(int? firstResult, int? maxResults, params Expression<Func<T, bool>>[] expressions)
            where T : class, IDataObject {
            if(IsDebugEnabled)
                log.Debug("FindAll entities. entity=[{0}], firstResult=[{1}], maxResults=[{2}], expression=[{3}]",
                          typeof(T).Name, firstResult, maxResults, expressions.CollectionToString());

            var query = QueryOver.Of<T>();

            if(expressions != null)
                foreach(var expr in expressions)
                    query.AddWhere(expr);

            return
                query
                    .AddSkip(firstResult)
                    .AddTake(maxResults)
                    .GetExecutableQueryOver(UnitOfWork.CurrentSession)
                    .List();
        }

        /// <summary>
        /// <see cref="ITreeNodeEntity{T}"/>를 구현한 엔티티 (TREE VIEW 상에 나타내는)에 대해 Root 에 해당하는 (Parent가 null인) Entity들을 조회한다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expressions">다른 부가 조건들</param>
        /// <returns></returns>
        public static IList<T> FindRoots<T>(params Expression<Func<T, bool>>[] expressions) where T : class, ITreeNodeEntity<T> {
            return FindRoots(null, null, expressions);
        }

        /// <summary>
        /// <see cref="ITreeNodeEntity{T}"/>를 구현한 엔티티 (TREE VIEW 상에 나타내는)에 대해 Root 에 해당하는 (Parent가 null인) Entity들을 조회한다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="firstResult">결과 셋의 첫번째 레코드의 인덱스(0부터 시작)</param>
        /// <param name="maxResults">결과 셋의 최대 크기</param>
        /// <param name="expressions">조건 절 람다 식</param>
        /// <returns></returns>
        public static IList<T> FindRoots<T>(int? firstResult, int? maxResults, params Expression<Func<T, bool>>[] expressions)
            where T : class, ITreeNodeEntity<T> {
            if(IsDebugEnabled)
                log.Debug("Find root nodes. entity=[{0}], firstResult=[{1}], maxResults=[{2}], expressions=[{3}]",
                          typeof(T).Name, firstResult, maxResults, expressions.CollectionToString());

            // Parent 가 null 인 node 가 최상위 노드입니다.
            var query = QueryOver.Of<T>().WhereRestrictionOn(node => node.Parent).IsNull;

            foreach(var expr in expressions)
                query.AddWhere(expr);

            query.AddSkip(firstResult).AddTake(maxResults);

            return query.GetExecutableQueryOver(UnitOfWork.CurrentSession).List<T>();
        }

        /// <summary>
        /// <see cref="ITreeNodeEntity{T}"/>를 구현한 엔티티 (TREE VIEW 상에 나타내는)에 대해 Root 에 해당하는 (Parent가 null인) Entity들을 조회한다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="firstResult">결과 셋의 첫번째 레코드의 인덱스(0부터 시작)</param>
        /// <param name="maxResults">결과 셋의 최대 크기</param>
        /// <param name="orders"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static IList<T> FindRoots<T>(int? firstResult, int? maxResults, Order[] orders, params INHParameter[] parameters)
            where T : class, ITreeNodeEntity<T> {
            if(IsDebugEnabled)
                log.Debug("Find root nodes. entity=[{0}], firstResult=[{1}], maxResults=[{2}], orders=[{3}], parameters=[{4}]",
                          typeof(T).Name, firstResult, maxResults, orders.CollectionToString(), parameters.CollectionToString());

            var crit = DetachedCriteria.For<T>()
                .AddIsNull("Parent");

            if(parameters != null)
                foreach(var p in parameters)
                    crit.AddEq(p.Name, p.Value);

            return Repository<T>.FindAll(crit,
                                         firstResult.GetValueOrDefault(-1),
                                         maxResults.GetValueOrDefault(-1),
                                         orders);
        }

        /// <summary>
        /// <see cref="ICodeEntity"/>를 구현한 엔티티의 Code 속성이 일치하는 엔티티를 검색합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="code"></param>
        /// <returns></returns>
        public static T FindOneByCode<T>(string code) where T : class, ICodeEntity {
            code.ShouldNotBeWhiteSpace("code");

            if(IsDebugEnabled)
                log.Debug("특정 코드 값을 가지는 유일한 엔티티를 조회합니다... entity=[{0}], Code=[{1}]", typeof(T).Name, code);

            return
                UnitOfWork.CurrentSession
                    .QueryOver<T>()
                    .Where(c => c.Code == code)
                    .SingleOrDefault();
        }

        /// <summary>
        /// Entity의 Code 속성 값이 <paramref name="code"/>와 일치하는 entity 들을 조회한다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="code"></param>
        /// <returns></returns>
        public static IList<T> FindAllByCode<T>(string code) where T : class, ICodeEntity {
            if(IsDebugEnabled)
                log.Debug("특정 코드 값을 가지는 모든 엔티티를 로드합니다... entity=[{0}], Code=[{1}]", typeof(T).Name, code);

            return
                QueryOver.Of<T>()
                    .AddEqOrNull(entity => entity.Code, code)
                    .GetExecutableQueryOver(UnitOfWork.CurrentSession)
                    .List();
        }

        /// <summary>
        /// <see cref="ICodeEntity"/>를 구현한 엔티티의 Code 속성이 일치하는 엔티티를 검색합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T FindOneByName<T>(string name) where T : class, INamedEntity {
            name.ShouldNotBeWhiteSpace("name");

            if(IsDebugEnabled)
                log.Debug("특정 Name 값을 가지는 유일한 엔티티를 조회합니다... entity=[{0}], Name=[{1}]", typeof(T).Name, name);

            return
                UnitOfWork.CurrentSession
                    .QueryOver<T>()
                    .Where(c => c.Name == name)
                    .SingleOrDefault();
        }

        /// <summary>
        /// Entity의 Name 속성 값이 <paramref name="name"/>와 일치하는 entity 들을 조회한다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IList<T> FindAllByName<T>(string name) where T : class, INamedEntity {
            if(IsDebugEnabled)
                log.Debug("특정 Name 값을 가지는 모든 엔티티를 로드합니다... entity=[{0}], Name=[{1}]", typeof(T).Name, name);

            return
                QueryOver.Of<T>()
                    .AddEqOrNull(entity => entity.Name, name)
                    .GetExecutableQueryOver(UnitOfWork.CurrentSession)
                    .List();
        }

        /// <summary>
        /// 지정된 이름과 매칭되는 (LIKE 검색) 엔티티를 모두 조회한다. (MatchMode는 Anywhere (%keyword%) 이다.)
        /// </summary>
        /// <typeparam name="T">검색할 Entity 수형</typeparam>
        /// <param name="nameToMatch">매칭될 이름</param>
        /// <returns></returns>
        public static IList<T> FindAllByNameToMatch<T>(string nameToMatch) where T : class, INamedEntity {
            return FindAllByNameToMatch<T>(nameToMatch, null);
        }

        /// <summary>
        /// 지정된 이름과 매칭되는 (LIKE 검색) 엔티티를 모두 조회한다.
        /// </summary>
        /// <typeparam name="T">검색할 Entity 수형</typeparam>
        /// <param name="nameToMatch">매칭될 이름</param>
        /// <param name="matchMode">매칭 방법(Start, End, Anywhere)</param>
        /// <returns></returns>
        public static IList<T> FindAllByNameToMatch<T>(string nameToMatch, MatchMode matchMode) where T : class, INamedEntity {
            if(IsDebugEnabled)
                log.Debug("Name 속성값에 대해 매칭되는 모든 엔티티를 조회합니다... entity=[{0}], nameToMatch=[{1}], matchMode=[{2}]",
                          typeof(T).FullName, nameToMatch, matchMode ?? MatchMode.Anywhere);
            return
                UnitOfWork.CurrentSession
                    .QueryOver<T>()
                    .WhereRestrictionOn(entity => entity.Name)
                    .IsInsensitiveLike(nameToMatch, matchMode ?? MatchMode.Anywhere)
                    .List();
        }
    }
}