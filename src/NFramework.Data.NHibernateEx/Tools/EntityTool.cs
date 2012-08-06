using System;
using System.Collections.Generic;
using System.Globalization;
using NHibernate.Criterion;
using NHibernate.Type;
using NSoft.NFramework.Data.NHibernateEx.Domain;
using NSoft.NFramework.Json;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.Reflections;
using NSoft.NFramework.Serializations;

namespace NSoft.NFramework.Data.NHibernateEx {
    public static partial class EntityTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private static readonly object _syncLock = new object();

        /// <summary>
        /// NULL 을 표현한 문자열 ("NULL" 이지 뭐...)
        /// </summary>
        public const string NULL_STRING = "NULL";

        /// <summary>
        /// <see cref="IDataObject"/> 를 상속받은 인스턴스의 정보를 문자열로 표현합니다. 인스턴스가 null 인 경우는 "NULL"을  반환합니다.
        /// ObjectDumper와 유사합니다.
        /// </summary>
        /// <param name="entity">내부 내용을 문자열로 표현하고자 하는 인스턴스</param>
        /// <returns></returns>
        public static string EntityAsString(this IDataObject entity) {
            return (entity != null) ? entity.ObjectToString() : NULL_STRING;
        }

        /// <summary>
        /// <see cref="IDataObject"/> 형식의 인스턴스를 JSON 방식으로 직렬화한 문자열을 반환합니다.
        /// </summary>
        /// <param name="entity">NHibernate 엔티티</param>
        /// <returns></returns>
        public static string AsJsonText(this IDataObject entity) {
            if(entity == null)
                return string.Empty;

            return JsonTool.SerializeAsText(entity);
        }

        #region << Hierarchy >>

        /// <summary>
        /// 조상을 나타내는 속성명 (Ancestors)
        /// </summary>
        public const string AncestorsPropertyName = @"Ancestors";

        /// <summary>
        /// 자손을 나타내는 속성명 (Descendents)
        /// </summary>
        public const string DescendentsPropertyName = @"Descendents";

        /// <summary>
        /// Circular Hierarchy를 검사한다. = 자식의 자손중에 부모나 부모의 조상이 있으면 안된다.
        /// </summary>
        /// <typeparam name="T">계층을 가진 형식 : Group, Role, Organization</typeparam>
        /// <param name="parent">부모</param>
        /// <param name="child">자식</param>
        public static void AssertNotCircularHierarchy<T>(this T child, T parent) where T : class, IHierarchyEntity<T> {
            if(Equals(parent, child))
                throw new InvalidOperationException("자식이 부모와 같습니다.");

            if(child.Descendents.Contains(parent))
                throw new InvalidOperationException("자식이 부모를 자손으로 가지면 안됩니다.");

            if(child.Descendents.Intersect(parent.Ancestors).Count > 0)
                throw new InvalidOperationException("자식의 부모의 조상을 자손으로 가지면 안됩니다.");
        }

        /// <summary>
        /// 자식의 부모를 변경한다. 새로운 부모와 자식간의 Circular Hierarchy가 아님을 검사해야한다. (Use AssertCircularHierarchy)
        /// </summary>
        public static void SetHierarchy<T>(this T child, T oldParent, T newParent) where T : class, IHierarchyEntity<T> {
            Guard.ShouldNotBeNull(child, "child");

            if(IsDebugEnabled)
                log.Debug("현재 노드의 부모를 변경하고, 계층구조 정보를 모두 변경합니다. child=[{0}], oldParent=[{1}], newParent=[{2}]",
                          child, oldParent, newParent);

            if(Equals(oldParent, default(T)) == false)
                child.RemoveHierarchy(oldParent);

            if(Equals(newParent, default(T)) == false)
                child.SetHierarchy(newParent);
        }

        /// <summary>
        /// 자식과 자식의 자손을 부모 및 부모의 조상에게 자손으로 등록한다.
        /// </summary>
        public static void SetHierarchy<T>(this T child, T parent) where T : class, IHierarchyEntity<T> {
            if(Equals(parent, default(T)) || Equals(child, default(T)))
                return;

            if(IsDebugEnabled)
                log.Debug("현재 노드의 부모 및 조상으로 설정합니다... child=[{0}], parent=[{1}]", child, parent);

            parent.Descendents.Add(child);
            parent.Descendents.AddAll(child.Descendents);

            parent.Ancestors
                //.ToList()
                .RunEach(anscestor => {
                             anscestor.Descendents.Add(child);
                             anscestor.Descendents.AddAll(child.Descendents);
                             UnitOfWork.CurrentSession.Update(anscestor);
                         });

            child.Ancestors.Add(parent);
            child.Ancestors.AddAll(parent.Ancestors);
        }

        /// <summary>
        /// 부모-자식 관계 해제 시에, 조상과 자손 정보도 해제시킨다.
        /// </summary>
        public static void RemoveHierarchy<T>(this T child, T parent) where T : class, IHierarchyEntity<T> {
            Guard.ShouldNotBeNull(child, "child");

            if(IsDebugEnabled)
                log.Debug("현재 노드의 부모 및 조상에 대한 정보를 제거합니다... child=[{0}], parent=[{1}]", child, parent);

            child.Ancestors.RemoveAll(parent.Ancestors);
            child.Ancestors.Remove(parent);

            // child를 삭제하기 위해서라면...
            if(Equals(parent, default(T)) == false) {
                parent.Ancestors
                    //.ToList()
                    .RunEach(ancestor => {
                                 ancestor.Descendents.Remove(child);
                                 ancestor.Descendents.RemoveAll(child.Descendents);
                                 // UnitOfWork.CurrentSession.Update(ancestor);
                             });

                child.Descendents
                    //.ToList()
                    .RunEach(descendent => {
                                 descendent.Ancestors.Remove(parent);
                                 descendent.Ancestors.RemoveAll(parent.Ancestors);
                                 // UnitOfWork.CurrentSession.Update(descendent);
                             });
            }
        }

        /// <summary>
        /// Entity의 조상 Entity의 Id 값을 조회한다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TId"></typeparam>
        /// <param name="current"></param>
        /// <returns></returns>
        public static IList<TId> GetAncestorsId<T, TId>(this T current)
            where T : class, IDataEntity<TId>, IHierarchyEntity<T> {
            return
                current
                    .GetAncestorsIdCriteria<T, TId>()
                    .GetExecutableCriteria(UnitOfWork.CurrentSession)
                    .List<TId>();
        }

        /// <summary>
        /// 지정된 엔티티의 자손들의 Id 값 얻기
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TId"></typeparam>
        /// <param name="current"></param>
        /// <returns></returns>
        public static IList<TId> GetDescendentsId<T, TId>(this T current)
            where T : class, IDataEntity<TId>, IHierarchyEntity<T> {
            return
                current
                    .GetDescendentsIdCriteria<T, TId>()
                    .GetExecutableCriteria(UnitOfWork.CurrentSession)
                    .List<TId>();
        }

        /// <summary>
        /// <see cref="IHierarchyEntity{TEntity}"/> 형식의 엔티티의 조상 엔티티들을 구하는 질의 객체를 빌드한다. 자손에 대한 alias는 "des"이다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TId"></typeparam>
        /// <param name="current"></param>
        /// <returns></returns>
        public static DetachedCriteria GetAncestorsCriteria<T, TId>(this T current)
            where T : class, IDataEntity<TId>, IHierarchyEntity<T> {
            return
                DetachedCriteria.For<T>()
                    .CreateAlias(DescendentsPropertyName, "des")
                    .AddEq("des.Id", current.Id);
        }

        /// <summary>
        /// <see cref="IHierarchyEntity{TEntity}"/> 형식의 엔티티의 자손 엔티티들을 구하는 질의 객체를 빌드한다. 조상에대한 alias는 "ans"이다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TId"></typeparam>
        /// <param name="current"></param>
        /// <returns></returns>
        public static DetachedCriteria GetDescendentsCriteria<T, TId>(this T current)
            where T : class, IDataEntity<TId>, IHierarchyEntity<T> {
            return
                DetachedCriteria.For<T>()
                    .CreateAlias(AncestorsPropertyName, "ans")
                    .AddEq("ans.Id", current.Id);
        }

        /// <summary>
        /// Hierarchy 정보를 가진 엔티티의 조상 엔티티의 Id 값을 조회하는  Criteria를 빌드한다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TId"></typeparam>
        /// <param name="current"></param>
        /// <returns></returns>
        public static DetachedCriteria GetAncestorsIdCriteria<T, TId>(this T current)
            where T : class, IDataEntity<TId>, IHierarchyEntity<T> {
            current.ShouldNotBeNull("current");
            return current
                .GetAncestorsCriteria<T, TId>()
                .SetProjection(Projections.Distinct(Projections.Id()));
        }

        /// <summary>
        /// Hierarchy 정보를 가진 엔티티의 조상 엔티티의 Id 값을 조회하는  Criteria를 빌드한다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TId"></typeparam>
        /// <param name="current"></param>
        /// <returns></returns>
        public static DetachedCriteria GetDescendentsIdCriteria<T, TId>(this T current)
            where T : class, IDataEntity<TId>, IHierarchyEntity<T> {
            current.ShouldNotBeNull("current");

            return current
                .GetDescendentsCriteria<T, TId>()
                .SetProjection(Projections.Distinct(Projections.Id()));
        }

        #endregion

        #region << LocaledEntity >>

        /// <summary>
        /// 지역화 정보를 가진 엔티티간에 지역화 정보를 복사한다.
        /// </summary>
        /// <typeparam name="TLocale">Locale 정보를 나타내는 class의 형식</typeparam>
        /// <param name="dest">복사될 지역화 엔티티</param>
        /// <param name="src">원본 지역화 엔티티</param>
        public static void CopyLocales<TLocale>(this ILocaledEntity<TLocale> dest, ILocaledEntity<TLocale> src)
            where TLocale : ILocaleValue {
            foreach(var culture in src.LocaleKeys)
                dest.AddLocale(culture, SerializerTool.DeepCopy(src.Locales[culture]));
        }

        /// <summary>
        /// Locale 정보를 가지는 엔티티 중에 지정된 Culture를 가지는 놈을 찾는다.
        /// </summary>
        /// <typeparam name="T">entity type</typeparam>
        /// <typeparam name="TLocale">type of entity locale</typeparam>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static IList<T> FindAllHasLocaleKey<T, TLocale>(CultureInfo culture)
            where T : class, ILocaledEntity<TLocale>
            where TLocale : ILocaleValue {
            var hql =
                string.Format("select distinct localedEntity from {0} localedEntity where :key in indices(localedEntity.LocaleMap)",
                              typeof(T).Name);

            if(IsDebugEnabled)
                log.Debug("LocaleKey[{0}]을 가진 모든 엔티티 조회 HQL=[{1}]", culture, hql);

            return Repository<T>.FindAllByHql(hql, new NHParameter("key", culture.Name, TypeFactory.GetStringType(255)));
        }

        /// <summary>
        /// Locale 정보를 가지는 엔티티 중에 해당 속성에 지정된 값을 가지는 엔티티를 조회한다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TLocale"></typeparam>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IList<T> FindAllHasLocaleValue<T, TLocale>(string propertyName, object value, IType type)
            where T : class, ILocaledEntity<TLocale>
            where TLocale : ILocaleValue {
            propertyName.ShouldNotBeWhiteSpace("propertyName");

            var hql =
                string.Format(
                    "select distinct localedEntity from {0} localedEntity join localedEntity.LocaleMap locale where locale.{1} = :{1}",
                    typeof(T).Name, propertyName);

            if(IsDebugEnabled)
                log.Debug("메타데이타 값[{0}]을 가진 모든 엔티티 조회 HQL=[{1}]", value, hql);

            return Repository<T>.FindAllByHql(hql, new NHParameter(propertyName, value, type));
        }

        #endregion

        #region << Metadata >>

        private const string FindAllHasMetadataKeyFmt =
            @"select distinct metadataEntity from {0} metadataEntity where :key in indices(metadataEntity.MetadataMap)";

        private const string FindAllHasMetadataValueFmt =
            @"select distinct metadataEntity from {0} metadataEntity join metadataEntity.MetadataMap meta where meta.Value = :value";

        /// <summary>
        /// 지정된 메타데이타 키를 가진 엔티티를 반환한다.
        /// </summary>
        /// <typeparam name="T">메타데이타를 가지는 형식(<see cref="IMetadataEntity"/>)</typeparam>
        /// <param name="key">메타데이타 키 값</param>
        /// <returns>지정된 메타데이타 키 값을 가지는 엔티티들</returns>
        public static IList<T> FindAllHasMetadataKey<T>(string key) where T : class, IMetadataEntity {
            var hql = string.Format(FindAllHasMetadataKeyFmt, typeof(T).Name);

            if(IsDebugEnabled)
                log.Debug("메타데이타 키[{0}]을 가진 모든 엔티티 조회 HQL=[{1}]", key, hql);

            return Repository<T>.FindAllByHql(hql, new NHParameter("key", key, TypeFactory.GetStringType(1024)));
        }

        /// <summary>
        /// 지정된 메타데이타의 값을 가진 엔티티를 반환한다.
        /// </summary>
        /// <typeparam name="T">메타데이타를 가지는 형식(<see cref="IMetadataEntity"/>)</typeparam>
        /// <param name="value">메타데이타 값</param>
        /// <returns>지정된 메타데이타 값을 가지는 엔티티들</returns>
        public static IList<T> FindAllHasMetadataValue<T>(string value) where T : class, IMetadataEntity {
            var hql = string.Format(FindAllHasMetadataValueFmt, typeof(T).Name);

            if(IsDebugEnabled)
                log.Debug("메타데이타 값[{0}]을 가진 모든 엔티티 조회 HQL=[{1}]", value, hql);

            return Repository<T>.FindAllByHql(hql, new NHParameter("value", value, TypeFactory.GetStringType(4000)));
        }

        #endregion
    }
}