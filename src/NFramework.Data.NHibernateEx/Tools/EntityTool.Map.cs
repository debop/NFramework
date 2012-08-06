using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Impl;
using NSoft.NFramework.Data.NHibernateEx.Domain;
using NSoft.NFramework.DynamicProxy;
using NSoft.NFramework.Reflections;

namespace NSoft.NFramework.Data.NHibernateEx {
    public static partial class EntityTool {
        /// <summary>
        /// 원본 객체를 대상 객체로 매핑합니다.
        /// </summary>
        /// <typeparam name="TSource">원본 객체 형식</typeparam>
        /// <typeparam name="TTarget">대상 객체 형식</typeparam>
        /// <param name="source">원본 객체</param>
        /// <param name="targetFactory">대상 객체 생성 Factory</param>
        /// <param name="propertyExprsToExclude">매핑에서 제외할 속성 표현식</param>
        /// <returns>대상 객체</returns>
        public static TTarget MapEntity<TSource, TTarget>(this TSource source,
                                                          Func<TTarget> targetFactory,
                                                          params Expression<Func<TTarget, object>>[] propertyExprsToExclude) {
            return MapEntity<TSource, TTarget>(source, targetFactory, MapPropertyOptions.Default, (Action<TSource, TTarget>)null,
                                               propertyExprsToExclude);
        }

        /// <summary>
        /// 원본 객체를 대상 객체로 매핑합니다.
        /// </summary>
        /// <typeparam name="TSource">원본 객체 형식</typeparam>
        /// <typeparam name="TTarget">대상 객체 형식</typeparam>
        /// <param name="source">원본 객체</param>
        /// <param name="targetFactory">대상 객체 생성 Factory</param>
        /// <param name="mapOptions">매핑 옵션</param>
        /// <param name="propertyExprsToExclude">매핑에서 제외할 속성 표현식</param>
        /// <returns>대상 객체</returns>
        public static TTarget MapEntity<TSource, TTarget>(this TSource source,
                                                          Func<TTarget> targetFactory,
                                                          MapPropertyOptions mapOptions,
                                                          params Expression<Func<TTarget, object>>[] propertyExprsToExclude) {
            return MapEntity<TSource, TTarget>(source, targetFactory, mapOptions, (Action<TSource, TTarget>)null, propertyExprsToExclude);
        }

        /// <summary>
        /// 원본 객체를 대상 객체로 매핑합니다. <paramref name="additionalMapping"/>을 통해 추가적인 매핑을 수행할 수 있습니다.
        /// </summary>
        /// <typeparam name="TSource">원본 객체 형식</typeparam>
        /// <typeparam name="TTarget">대상 객체 형식</typeparam>
        /// <param name="source">원본 객체</param>
        /// <param name="targetFactory">대상 객체 생성 Factory</param>
        /// <param name="mapOptions">매핑 옵션</param>
        /// <param name="additionalMapping">추가 매핑 함수</param>
        /// <param name="propertyExprsToExclude">매핑에서 제외할 속성 표현식</param>
        /// <returns>대상 객체 시퀀스</returns>
        public static TTarget MapEntity<TSource, TTarget>(this TSource source,
                                                          Func<TTarget> targetFactory,
                                                          MapPropertyOptions mapOptions,
                                                          Action<TSource, TTarget> additionalMapping,
                                                          params Expression<Func<TTarget, object>>[] propertyExprsToExclude) {
            targetFactory.ShouldNotBeNull("targetFactory");

            var propertyNamesToExclude =
                propertyExprsToExclude.Select(expr => ExpressionProcessor.FindMemberExpression(expr.Body)).ToList();
            ExcludeStatePropertyForStateEntity<TSource>(propertyNamesToExclude);

            return ObjectMapper.MapObject<TSource, TTarget>(source, targetFactory, mapOptions, additionalMapping,
                                                            propertyNamesToExclude.ToArray());
        }

        /// <summary>
        /// 원본 객체를 대상 객체로 매핑합니다.
        /// </summary>
        /// <typeparam name="TSource">원본 객체 형식</typeparam>
        /// <typeparam name="TTarget">대상 객체 형식</typeparam>
        /// <param name="sources">원본 객체 시퀀스</param>
        /// <param name="targetFactory">대상 객체 생성 Factory</param>
        /// <param name="propertyExprsToExclude">매핑에서 제외할 속성 표현식</param>
        /// <returns>대상 객체 시퀀스</returns>
        public static IEnumerable<TTarget> MapEntities<TSource, TTarget>(this IEnumerable<TSource> sources,
                                                                         Func<TTarget> targetFactory,
                                                                         params Expression<Func<TTarget, object>>[]
                                                                             propertyExprsToExclude) {
            return MapEntities<TSource, TTarget>(sources, targetFactory, MapPropertyOptions.Default, (Action<TSource, TTarget>)null,
                                                 propertyExprsToExclude);
        }

        /// <summary>
        /// 원본 객체를 대상 객체로 매핑합니다.
        /// </summary>
        /// <typeparam name="TSource">원본 객체 형식</typeparam>
        /// <typeparam name="TTarget">대상 객체 형식</typeparam>
        /// <param name="sources">원본 객체 시퀀스</param>
        /// <param name="targetFactory">대상 객체 생성 Factory</param>
        /// <param name="mapOptions">매핑 옵션</param>
        /// <param name="propertyExprsToExclude">매핑에서 제외할 속성 표현식</param>
        /// <returns>대상 객체 시퀀스</returns>
        public static IEnumerable<TTarget> MapEntities<TSource, TTarget>(this IEnumerable<TSource> sources,
                                                                         Func<TTarget> targetFactory,
                                                                         MapPropertyOptions mapOptions,
                                                                         params Expression<Func<TTarget, object>>[]
                                                                             propertyExprsToExclude) {
            return MapEntities<TSource, TTarget>(sources, targetFactory, mapOptions, (Action<TSource, TTarget>)null,
                                                 propertyExprsToExclude);
        }

        /// <summary>
        /// 원본 객체를 대상 객체로 매핑합니다. <paramref name="additionalMapping"/>을 통해 추가적인 매핑을 수행할 수 있습니다.
        /// </summary>
        /// <typeparam name="TSource">원본 객체 형식</typeparam>
        /// <typeparam name="TTarget">대상 객체 형식</typeparam>
        /// <param name="sources">원본 객체 시퀀스</param>
        /// <param name="targetFactory">대상 객체 생성 Factory</param>
        /// <param name="mapOptions">매핑 옵션</param>
        /// <param name="additionalMapping">추가 매핑 함수</param>
        /// <param name="propertyExprsToExclude">매핑에서 제외할 속성 표현식</param>
        /// <returns>대상 객체 시퀀스</returns>
        public static IEnumerable<TTarget> MapEntities<TSource, TTarget>(this IEnumerable<TSource> sources,
                                                                         Func<TTarget> targetFactory,
                                                                         MapPropertyOptions mapOptions,
                                                                         Action<TSource, TTarget> additionalMapping,
                                                                         params Expression<Func<TTarget, object>>[]
                                                                             propertyExprsToExclude) {
            if(IsDebugEnabled)
                log.Debug("원본 엔티티[{0}] 컬렉션으로부터 대상 엔티티[{1}] 컬렉션으로 매핑을 수행합니다...",
                          typeof(TSource).Name, typeof(TTarget).Name);

            targetFactory.ShouldNotBeNull("targetFactory");

            var propertyNamesToExclude =
                propertyExprsToExclude.Select(expr => ExpressionProcessor.FindMemberExpression(expr.Body)).ToList();
            ExcludeStatePropertyForStateEntity<TSource>(propertyNamesToExclude);

            return ObjectMapper.MapObjects(sources, targetFactory, mapOptions, additionalMapping, propertyNamesToExclude.ToArray());
        }

        /// <summary>
        /// 원본 객체를 대상 객체로 매핑합니다.
        /// </summary>
        /// <typeparam name="TSource">원본 객체 형식</typeparam>
        /// <typeparam name="TTarget">대상 객체 형식</typeparam>
        /// <param name="sources">원본 객체 시퀀스</param>
        /// <param name="targetFactory">대상 객체 생성 Factory</param>
        /// <param name="propertyExprsToExclude">매핑에서 제외할 속성 표현식</param>
        /// <returns>대상 객체 시퀀스</returns>
        public static IList<TTarget> MapEntitiesAsParallel<TSource, TTarget>(this IList<TSource> sources,
                                                                             Func<TTarget> targetFactory,
                                                                             params Expression<Func<TTarget, object>>[]
                                                                                 propertyExprsToExclude) {
            return MapEntitiesAsParallel(sources, targetFactory, MapPropertyOptions.Default, (Action<TSource, TTarget>)null,
                                         propertyExprsToExclude);
        }

        /// <summary>
        /// 원본 객체를 대상 객체로 매핑합니다.
        /// </summary>
        /// <typeparam name="TSource">원본 객체 형식</typeparam>
        /// <typeparam name="TTarget">대상 객체 형식</typeparam>
        /// <param name="sources">원본 객체 시퀀스</param>
        /// <param name="targetFactory">대상 객체 생성 Factory</param>
        /// <param name="mapOptions">매핑 옵션</param>
        /// <param name="propertyExprsToExclude">매핑에서 제외할 속성 표현식</param>
        /// <returns>대상 객체 시퀀스</returns>
        public static IList<TTarget> MapEntitiesAsParallel<TSource, TTarget>(this IList<TSource> sources,
                                                                             Func<TTarget> targetFactory,
                                                                             MapPropertyOptions mapOptions,
                                                                             params Expression<Func<TTarget, object>>[]
                                                                                 propertyExprsToExclude) {
            return MapEntitiesAsParallel(sources, targetFactory, mapOptions, (Action<TSource, TTarget>)null, propertyExprsToExclude);
        }

        /// <summary>
        /// 원본 객체를 대상 객체로 매핑합니다. <paramref name="additionalMapping"/>을 통해 추가적인 매핑을 수행할 수 있습니다.
        /// </summary>
        /// <typeparam name="TSource">원본 객체 형식</typeparam>
        /// <typeparam name="TTarget">대상 객체 형식</typeparam>
        /// <param name="sources">원본 객체 시퀀스</param>
        /// <param name="targetFactory">대상 객체 생성 Factory</param>
        /// <param name="mapOptions">매핑 옵션</param>
        /// <param name="additionalMapping">추가 매핑 함수</param>
        /// <param name="propertyExprsToExclude">매핑에서 제외할 속성 표현식</param>
        /// <returns>대상 객체 시퀀스</returns>
        public static IList<TTarget> MapEntitiesAsParallel<TSource, TTarget>(this IList<TSource> sources,
                                                                             Func<TTarget> targetFactory,
                                                                             MapPropertyOptions mapOptions,
                                                                             Action<TSource, TTarget> additionalMapping,
                                                                             params Expression<Func<TTarget, object>>[]
                                                                                 propertyExprsToExclude) {
            if(IsDebugEnabled)
                log.Debug("원본 엔티티[{0}] 컬렉션으로부터 대상 엔티티[{1}] 컬렉션으로 매핑을 수행합니다...",
                          typeof(TSource).Name, typeof(TTarget).Name);

            targetFactory.ShouldNotBeNull("targetFactory");

            if(sources.Count == 0)
                return new List<TTarget>();

            var propertyNamesToExclude =
                propertyExprsToExclude.Select(expr => ExpressionProcessor.FindMemberExpression(expr.Body)).ToList();
            ExcludeStatePropertyForStateEntity<TSource>(propertyNamesToExclude);

            // Source가 NHibernate 엔티티라면, Initialize를 통해, Lazy된 Proxy 값을 실제값으로 빌드합니다.
            //
            IList<TSource> initializedSources = sources;

            if(typeof(TSource).HasInterface(typeof(IStateEntity))) {
                if(IsDebugEnabled)
                    log.Debug("원본 객체가 NHibernate Entity이므로, Initialize를 수행합니다...");

                initializedSources = NHTool.InitializeEntities(sources, sources[0].IsDynamicProxy());
            }

            return
                ObjectMapper
                    .MapObjectsAsParallel(initializedSources, targetFactory, mapOptions, additionalMapping,
                                          propertyNamesToExclude.ToArray())
                    .ToList();
        }

        private static void ExcludeStatePropertyForStateEntity<T>(ICollection<string> propertyNamesToExclude) {
            if(typeof(T).HasInterface(typeof(IStateEntity))) {
                if(IsDebugEnabled)
                    log.Debug("원본 수형이 IStateEntity를 구현하였으므로, IsTransient, IsSaved 속성은 매핑에서 제외합니다.");

                propertyNamesToExclude.Add("IsTransient");
                propertyNamesToExclude.Add("IsSaved");
            }
        }
    }
}