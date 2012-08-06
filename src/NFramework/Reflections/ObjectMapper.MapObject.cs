using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NSoft.NFramework.Reflections {
    public static partial class ObjectMapper {
        /// <summary>
        /// 원본 객체 정보를 대상 객체를 생성하여, 같은 속성명의 값을 매핑하여 반환합니다.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget">대상 인스턴스 수형</typeparam>
        /// <param name="source">원본 인스턴스</param>
        /// <param name="targetFactory">대상 인스턴스 생성 팩토리</param>
        /// <param name="mapOptions">매핑 시의 옵션</param>
        /// <param name="additionalMapping">추가 매핑 작업</param>
        /// <param name="propertyNamesToExclude">제외할 속성명</param>
        /// <returns>매핑된 대상 객체</returns>
        public static TTarget MapObject<TSource, TTarget>(this TSource source,
                                                          Func<TTarget> targetFactory,
                                                          MapPropertyOptions mapOptions = null,
                                                          Action<TSource, TTarget> additionalMapping = null,
                                                          string[] propertyNamesToExclude = null) {
            targetFactory.ShouldNotBeNull("targetFactory");

            var target = MapProperty(source, targetFactory, mapOptions, propertyNamesToExclude);

            if(additionalMapping != null)
                additionalMapping(source, target);

            return target;
        }

        //------------------------------------------------

        /// <summary>
        /// 원본 객체 정보를 대상 객체를 생성하여, 같은 속성명의 값을 매핑합니다.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget">대상 인스턴스 수형</typeparam>
        /// <param name="source">원본 인스턴스</param>
        /// <param name="target">대상 인스턴스</param>
        /// <param name="propertyNamesToExclude">제외할 속성명</param>
        /// <returns>매핑된 대상 객체</returns>
        public static void MapObject<TSource, TTarget>(this TSource source,
                                                       TTarget target,
                                                       string[] propertyNamesToExclude) {
            target.ShouldNotBeNull("target");
            MapObject(source, () => target, DefaultOptions, null, propertyNamesToExclude);
        }

        /// <summary>
        /// 원본 객체 정보를 대상 객체를 생성하여, 같은 속성명의 값을 매핑합니다.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget">대상 인스턴스 수형</typeparam>
        /// <param name="source">원본 인스턴스</param>
        /// <param name="target">대상 인스턴스</param>
        /// <param name="mapOptions">매핑 시의 옵션</param>
        /// <param name="propertyNamesToExclude">제외할 속성명</param>
        /// <returns>매핑된 대상 객체</returns>
        public static void MapObject<TSource, TTarget>(this TSource source,
                                                       TTarget target,
                                                       MapPropertyOptions mapOptions,
                                                       string[] propertyNamesToExclude) {
            target.ShouldNotBeNull("target");
            MapObject(source, () => target, mapOptions, null, propertyNamesToExclude);
        }

        /// <summary>
        /// 원본 객체 정보를 대상 객체를 생성하여, 같은 속성명의 값을 매핑합니다.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget">대상 인스턴스 수형</typeparam>
        /// <param name="source">원본 인스턴스</param>
        /// <param name="target">대상 인스턴스</param>
        /// <param name="additionalMapping">추가 매핑 작업</param>
        /// <param name="propertyNamesToExclude">제외할 속성명</param>
        /// <returns>매핑된 대상 객체</returns>
        public static void MapObject<TSource, TTarget>(this TSource source,
                                                       TTarget target,
                                                       Action<TSource, TTarget> additionalMapping,
                                                       string[] propertyNamesToExclude) {
            target.ShouldNotBeNull("target");
            MapObject(source, () => target, DefaultOptions, additionalMapping, propertyNamesToExclude);
        }

        /// <summary>
        /// 원본 객체 정보를 대상 객체를 생성하여, 같은 속성명의 값을 매핑합니다.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget">대상 인스턴스 수형</typeparam>
        /// <param name="source">원본 인스턴스</param>
        /// <param name="target">대상 인스턴스</param>
        /// <param name="mapOptions">매핑 시의 옵션</param>
        /// <param name="additionalMapping">추가 매핑 작업</param>
        /// <param name="propertyNamesToExclude">제외할 속성명</param>
        /// <returns>매핑된 대상 객체</returns>
        public static void MapObject<TSource, TTarget>(this TSource source,
                                                       TTarget target,
                                                       MapPropertyOptions mapOptions,
                                                       Action<TSource, TTarget> additionalMapping,
                                                       string[] propertyNamesToExclude) {
            target.ShouldNotBeNull("target");
            MapObject(source, () => target, mapOptions, additionalMapping, propertyNamesToExclude);
        }

        //------------------------------------------------

        /// <summary>
        /// 원본 시퀀스를 이용하여 대상 객체를 생성한 후, 속성 값을 매핑하여 반환합니다.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget">대상 인스턴스 수형</typeparam>
        /// <param name="sources">원본 인스턴스 시퀀스</param>
        /// <param name="targetFactory">대상 인스턴스 생성 팩토리</param>
        /// <param name="propertyNamesToExclude">제외할 속성명</param>
        /// <returns>매핑된 대상 객체 시퀀스</returns>
        public static IEnumerable<TTarget> MapObjects<TSource, TTarget>(this IEnumerable<TSource> sources,
                                                                        Func<TTarget> targetFactory,
                                                                        string[] propertyNamesToExclude) {
            return MapObjects(sources, targetFactory, DefaultOptions, null, propertyNamesToExclude);
        }

        /// <summary>
        /// 원본 시퀀스를 이용하여 대상 객체를 생성한 후, 속성 값을 매핑하여 반환합니다.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget">대상 인스턴스 수형</typeparam>
        /// <param name="sources">원본 인스턴스 시퀀스</param>
        /// <param name="targetFactory">대상 인스턴스 생성 팩토리</param>
        /// <param name="mapOptions">매핑 시의 옵션</param>
        /// <param name="propertyNamesToExclude">제외할 속성명</param>
        /// <returns>매핑된 대상 객체 시퀀스</returns>
        public static IEnumerable<TTarget> MapObjects<TSource, TTarget>(this IEnumerable<TSource> sources,
                                                                        Func<TTarget> targetFactory,
                                                                        MapPropertyOptions mapOptions,
                                                                        string[] propertyNamesToExclude) {
            return MapObjects(sources, targetFactory, mapOptions, null, propertyNamesToExclude);
        }

        /// <summary>
        /// 원본 시퀀스를 이용하여 대상 객체를 생성한 후, 속성 값을 매핑하여 반환합니다.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget">대상 인스턴스 수형</typeparam>
        /// <param name="sources">원본 인스턴스 시퀀스</param>
        /// <param name="targetFactory">대상 인스턴스 생성 팩토리</param>
        /// <param name="additionalMapping">추가 매핑 작업</param>
        /// <param name="propertyNamesToExclude">제외할 속성명</param>
        /// <returns>매핑된 대상 객체 시퀀스</returns>
        public static IEnumerable<TTarget> MapObjects<TSource, TTarget>(this IEnumerable<TSource> sources,
                                                                        Func<TTarget> targetFactory,
                                                                        Action<TSource, TTarget> additionalMapping,
                                                                        string[] propertyNamesToExclude) {
            return MapObjects(sources, targetFactory, DefaultOptions, additionalMapping, propertyNamesToExclude);
        }

        /// <summary>
        /// 원본 시퀀스를 이용하여 대상 객체를 생성한 후, 속성 값을 매핑하여 반환합니다.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget">대상 인스턴스 수형</typeparam>
        /// <param name="sources">원본 인스턴스 시퀀스</param>
        /// <param name="targetFactory">대상 인스턴스 생성 팩토리</param>
        /// <param name="mapOptions">매핑 시의 옵션</param>
        /// <param name="additionalMapping">추가 매핑 작업</param>
        /// <param name="propertyNamesToExclude">제외할 속성명</param>
        /// <returns>매핑된 대상 객체 시퀀스</returns>
        public static IEnumerable<TTarget> MapObjects<TSource, TTarget>(this IEnumerable<TSource> sources,
                                                                        Func<TTarget> targetFactory,
                                                                        MapPropertyOptions mapOptions,
                                                                        Action<TSource, TTarget> additionalMapping,
                                                                        string[] propertyNamesToExclude) {
            foreach(var source in sources)
                yield return MapObject(source, targetFactory, mapOptions, additionalMapping, propertyNamesToExclude);
        }

        //------------------------------------------------

#if !SILVERLIGHT

        /// <summary>
        /// 원본 시퀀스를 이용하여 대상 객체를 생성한 후, 병렬 방식으로 속성 값을 매핑하여 반환합니다.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget">대상 인스턴스 수형</typeparam>
        /// <param name="sources">원본 인스턴스 시퀀스</param>
        /// <param name="targetFactory">대상 인스턴스 생성 팩토리</param>
        /// <param name="propertyNamesToExclude">제외할 속성명</param>
        /// <returns>매핑된 대상 객체 시퀀스</returns>
        public static ParallelQuery<TTarget> MapObjectsAsParallel<TSource, TTarget>(this IEnumerable<TSource> sources,
                                                                                    Func<TTarget> targetFactory,
                                                                                    string[] propertyNamesToExclude) {
            return MapObjectsAsParallel(sources, targetFactory, DefaultOptions, null, propertyNamesToExclude);
        }

        /// <summary>
        /// 원본 시퀀스를 이용하여 대상 객체를 생성한 후, 병렬 방식으로 속성 값을 매핑하여 반환합니다.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget">대상 인스턴스 수형</typeparam>
        /// <param name="sources">원본 인스턴스 시퀀스</param>
        /// <param name="targetFactory">대상 인스턴스 생성 팩토리</param>
        /// <param name="mapOptions">매핑 시의 옵션</param>
        /// <param name="propertyNamesToExclude">제외할 속성명</param>
        /// <returns>매핑된 대상 객체 시퀀스</returns>
        public static ParallelQuery<TTarget> MapObjectsAsParallel<TSource, TTarget>(this IEnumerable<TSource> sources,
                                                                                    Func<TTarget> targetFactory,
                                                                                    MapPropertyOptions mapOptions,
                                                                                    string[] propertyNamesToExclude) {
            return MapObjectsAsParallel(sources, targetFactory, mapOptions, null, propertyNamesToExclude);
        }

        /// <summary>
        /// 원본 시퀀스를 이용하여 대상 객체를 생성한 후, 병렬 방식으로 속성 값을 매핑하여 반환합니다.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget">대상 인스턴스 수형</typeparam>
        /// <param name="sources">원본 인스턴스 시퀀스</param>
        /// <param name="targetFactory">대상 인스턴스 생성 팩토리</param>
        /// <param name="additionalMapping">추가 매핑 작업</param>
        /// <param name="propertyNamesToExclude">제외할 속성명</param>
        /// <returns>매핑된 대상 객체 시퀀스</returns>
        public static ParallelQuery<TTarget> MapObjectsAsParallel<TSource, TTarget>(this IEnumerable<TSource> sources,
                                                                                    Func<TTarget> targetFactory,
                                                                                    Action<TSource, TTarget> additionalMapping,
                                                                                    string[] propertyNamesToExclude) {
            return MapObjectsAsParallel(sources, targetFactory, DefaultOptions, additionalMapping, propertyNamesToExclude);
        }

        /// <summary>
        /// 원본 시퀀스를 이용하여 대상 객체를 생성한 후, 병렬 방식으로 속성 값을 매핑하여 반환합니다.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget">대상 인스턴스 수형</typeparam>
        /// <param name="sources">원본 인스턴스 시퀀스</param>
        /// <param name="targetFactory">대상 인스턴스 생성 팩토리</param>
        /// <param name="mapOptions">매핑 시의 옵션</param>
        /// <param name="additionalMapping">추가 매핑 작업</param>
        /// <param name="propertyNamesToExclude">제외할 속성명</param>
        /// <returns>매핑된 대상 객체 시퀀스</returns>
        public static ParallelQuery<TTarget> MapObjectsAsParallel<TSource, TTarget>(this IEnumerable<TSource> sources,
                                                                                    Func<TTarget> targetFactory,
                                                                                    MapPropertyOptions mapOptions,
                                                                                    Action<TSource, TTarget> additionalMapping,
                                                                                    string[] propertyNamesToExclude) {
            targetFactory.ShouldNotBeNull("targetFactory");

            return
                sources.AsParallel()
                    .AsOrdered()
                    .Select(source => MapObject(source,
                                                targetFactory,
                                                mapOptions,
                                                additionalMapping,
                                                propertyNamesToExclude));
        }

#endif

        //------------------------------------------------

        /// <summary>
        /// 원본 객체 정보를 대상 객체를 생성하여, 같은 속성명의 값을 매핑하여 반환합니다.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget">대상 인스턴스 수형</typeparam>
        /// <param name="source">원본 인스턴스</param>
        /// <param name="targetFactory">대상 인스턴스 생성 팩토리</param>
        /// <param name="propertyExprsToExclude">제외할 속성명</param>
        /// <returns>매핑된 대상 객체</returns>
        public static TTarget MapObject<TSource, TTarget>(this TSource source,
                                                          Func<TTarget> targetFactory,
                                                          params Expression<Func<TTarget, object>>[] propertyExprsToExclude) {
            return MapObject(source, targetFactory, MapPropertyOptions.Default, null, propertyExprsToExclude);
        }

        /// <summary>
        /// 원본 객체 정보를 대상 객체를 생성하여, 같은 속성명의 값을 매핑하여 반환합니다.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget">대상 인스턴스 수형</typeparam>
        /// <param name="source">원본 인스턴스</param>
        /// <param name="targetFactory">대상 인스턴스 생성 팩토리</param>
        /// <param name="mapOptions">매핑 시의 옵션</param>
        /// <param name="propertyExprsToExclude">제외할 속성명</param>
        /// <returns>매핑된 대상 객체</returns>
        public static TTarget MapObject<TSource, TTarget>(this TSource source,
                                                          Func<TTarget> targetFactory,
                                                          MapPropertyOptions mapOptions,
                                                          params Expression<Func<TTarget, object>>[] propertyExprsToExclude) {
            return MapObject(source, targetFactory, mapOptions, null, propertyExprsToExclude);
        }

        /// <summary>
        /// 원본 객체 정보를 대상 객체를 생성하여, 같은 속성명의 값을 매핑하여 반환합니다.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget">대상 인스턴스 수형</typeparam>
        /// <param name="source">원본 인스턴스</param>
        /// <param name="targetFactory">대상 인스턴스 생성 팩토리</param>
        /// <param name="additionalMapping">추가 매핑 작업</param>
        /// <param name="propertyExprsToExclude">제외할 속성명</param>
        /// <returns>매핑된 대상 객체</returns>
        public static TTarget MapObject<TSource, TTarget>(this TSource source,
                                                          Func<TTarget> targetFactory,
                                                          Action<TSource, TTarget> additionalMapping,
                                                          params Expression<Func<TTarget, object>>[] propertyExprsToExclude) {
            return MapObject(source, targetFactory, DefaultOptions, additionalMapping, propertyExprsToExclude);
        }

        /// <summary>
        /// 원본 객체 정보를 대상 객체를 생성하여, 같은 속성명의 값을 매핑하여 반환합니다.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget">대상 인스턴스 수형</typeparam>
        /// <param name="source">원본 인스턴스</param>
        /// <param name="targetFactory">대상 인스턴스 생성 팩토리</param>
        /// <param name="mapOptions">매핑 시의 옵션</param>
        /// <param name="additionalMapping">추가 매핑 작업</param>
        /// <param name="propertyExprsToExclude">제외할 속성명</param>
        /// <returns>매핑된 대상 객체</returns>
        public static TTarget MapObject<TSource, TTarget>(this TSource source,
                                                          Func<TTarget> targetFactory,
                                                          MapPropertyOptions mapOptions,
                                                          Action<TSource, TTarget> additionalMapping,
                                                          params Expression<Func<TTarget, object>>[] propertyExprsToExclude) {
            targetFactory.ShouldNotBeNull("targetFactory");

            var target = MapProperty(source, targetFactory, mapOptions, propertyExprsToExclude);

            if(additionalMapping != null)
                additionalMapping(source, target);

            return target;
        }

        //--------------------------------------------------

        /// <summary>
        /// 원본 객체 정보를 대상 객체를 생성하여, 같은 속성명의 값을 매핑합니다.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget">대상 인스턴스 수형</typeparam>
        /// <param name="source">원본 인스턴스</param>
        /// <param name="target">대상 인스턴스</param>
        /// <param name="propertyExprsToExclude">제외할 속성명</param>
        /// <returns>매핑된 대상 객체</returns>
        public static void MapObject<TSource, TTarget>(this TSource source,
                                                       TTarget target,
                                                       params Expression<Func<TTarget, object>>[] propertyExprsToExclude) {
            MapObject(source, target, DefaultOptions, null, propertyExprsToExclude);
        }

        /// <summary>
        /// 원본 객체 정보를 대상 객체를 생성하여, 같은 속성명의 값을 매핑합니다.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget">대상 인스턴스 수형</typeparam>
        /// <param name="source">원본 인스턴스</param>
        /// <param name="target">대상 인스턴스</param>
        /// <param name="mapOptions">매핑 시의 옵션</param>
        /// <param name="propertyExprsToExclude">제외할 속성명</param>
        /// <returns>매핑된 대상 객체</returns>
        public static void MapObject<TSource, TTarget>(this TSource source,
                                                       TTarget target,
                                                       MapPropertyOptions mapOptions,
                                                       params Expression<Func<TTarget, object>>[] propertyExprsToExclude) {
            MapObject(source, target, mapOptions, null, propertyExprsToExclude);
        }

        /// <summary>
        /// 원본 객체 정보를 대상 객체를 생성하여, 같은 속성명의 값을 매핑합니다.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget">대상 인스턴스 수형</typeparam>
        /// <param name="source">원본 인스턴스</param>
        /// <param name="target">대상 인스턴스</param>
        /// <param name="additionalMapping">추가 매핑 작업</param>
        /// <param name="propertyExprsToExclude">제외할 속성명</param>
        /// <returns>매핑된 대상 객체</returns>
        public static void MapObject<TSource, TTarget>(this TSource source,
                                                       TTarget target,
                                                       Action<TSource, TTarget> additionalMapping,
                                                       params Expression<Func<TTarget, object>>[] propertyExprsToExclude) {
            MapObject(source, target, DefaultOptions, additionalMapping, propertyExprsToExclude);
        }

        /// <summary>
        /// 원본 객체 정보를 대상 객체를 생성하여, 같은 속성명의 값을 매핑합니다.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget">대상 인스턴스 수형</typeparam>
        /// <param name="source">원본 인스턴스</param>
        /// <param name="target">대상 인스턴스</param>
        /// <param name="mapOptions">매핑 시의 옵션</param>
        /// <param name="additionalMapping">추가 매핑 작업</param>
        /// <param name="propertyExprsToExclude">제외할 속성명</param>
        /// <returns>매핑된 대상 객체</returns>
        public static void MapObject<TSource, TTarget>(this TSource source,
                                                       TTarget target,
                                                       MapPropertyOptions mapOptions,
                                                       Action<TSource, TTarget> additionalMapping,
                                                       params Expression<Func<TTarget, object>>[] propertyExprsToExclude) {
            target.ShouldNotBeNull("target");
            MapObject(source, () => target, mapOptions, additionalMapping, propertyExprsToExclude);
        }

        //--------------------------------------------------

        /// <summary>
        /// 원본 시퀀스를 이용하여 대상 객체를 생성한 후, 속성 값을 매핑하여 반환합니다.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget">대상 인스턴스 수형</typeparam>
        /// <param name="sources">원본 인스턴스 시퀀스</param>
        /// <param name="targetFactory">대상 인스턴스 생성 팩토리</param>
        /// <param name="propertyExprsToExclude">제외할 속성명</param>
        /// <returns>매핑된 대상 객체 시퀀스</returns>
        public static IEnumerable<TTarget> MapObjects<TSource, TTarget>(this IEnumerable<TSource> sources,
                                                                        Func<TTarget> targetFactory,
                                                                        params Expression<Func<TTarget, object>>[]
                                                                            propertyExprsToExclude) {
            return MapObjects(sources, targetFactory, DefaultOptions, null, propertyExprsToExclude);
        }

        /// <summary>
        /// 원본 시퀀스를 이용하여 대상 객체를 생성한 후, 속성 값을 매핑하여 반환합니다.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget">대상 인스턴스 수형</typeparam>
        /// <param name="sources">원본 인스턴스 시퀀스</param>
        /// <param name="targetFactory">대상 인스턴스 생성 팩토리</param>
        /// <param name="mapOptions">매핑 시의 옵션</param>
        /// <param name="propertyExprsToExclude">제외할 속성명</param>
        /// <returns>매핑된 대상 객체 시퀀스</returns>
        public static IEnumerable<TTarget> MapObjects<TSource, TTarget>(this IEnumerable<TSource> sources,
                                                                        Func<TTarget> targetFactory,
                                                                        MapPropertyOptions mapOptions,
                                                                        params Expression<Func<TTarget, object>>[]
                                                                            propertyExprsToExclude) {
            return MapObjects(sources, targetFactory, mapOptions, null, propertyExprsToExclude);
        }

        /// <summary>
        /// 원본 시퀀스를 이용하여 대상 객체를 생성한 후, 속성 값을 매핑하여 반환합니다.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget">대상 인스턴스 수형</typeparam>
        /// <param name="sources">원본 인스턴스 시퀀스</param>
        /// <param name="targetFactory">대상 인스턴스 생성 팩토리</param>
        /// <param name="additionalMapping">추가 매핑 작업</param>
        /// <param name="propertyExprsToExclude">제외할 속성명</param>
        /// <returns>매핑된 대상 객체 시퀀스</returns>
        public static IEnumerable<TTarget> MapObjects<TSource, TTarget>(this IEnumerable<TSource> sources,
                                                                        Func<TTarget> targetFactory,
                                                                        Action<TSource, TTarget> additionalMapping,
                                                                        params Expression<Func<TTarget, object>>[]
                                                                            propertyExprsToExclude) {
            return MapObjects(sources, targetFactory, DefaultOptions, additionalMapping, propertyExprsToExclude);
        }

        /// <summary>
        /// 원본 시퀀스를 이용하여 대상 객체를 생성한 후, 속성 값을 매핑하여 반환합니다.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget">대상 인스턴스 수형</typeparam>
        /// <param name="sources">원본 인스턴스 시퀀스</param>
        /// <param name="targetFactory">대상 인스턴스 생성 팩토리</param>
        /// <param name="mapOptions">매핑 시의 옵션</param>
        /// <param name="additionalMapping">추가 매핑 작업</param>
        /// <param name="propertyExprsToExclude">제외할 속성명</param>
        /// <returns>매핑된 대상 객체 시퀀스</returns>
        public static IEnumerable<TTarget> MapObjects<TSource, TTarget>(this IEnumerable<TSource> sources,
                                                                        Func<TTarget> targetFactory,
                                                                        MapPropertyOptions mapOptions,
                                                                        Action<TSource, TTarget> additionalMapping,
                                                                        params Expression<Func<TTarget, object>>[]
                                                                            propertyExprsToExclude) {
            return sources.Select(source => MapObject(source, targetFactory, mapOptions, additionalMapping, propertyExprsToExclude));
        }

        //--------------------------------------------------
#if !SILVERLIGHT

        /// <summary>
        /// 원본 시퀀스를 이용하여 대상 객체를 생성한 후, 병렬 방식으로 속성 값을 매핑하여 반환합니다.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget">대상 인스턴스 수형</typeparam>
        /// <param name="sources">원본 인스턴스 시퀀스</param>
        /// <param name="targetFactory">대상 인스턴스 생성 팩토리</param>
        /// <param name="propertyExprsToExclude">제외할 속성명</param>
        /// <returns>매핑된 대상 객체 시퀀스</returns>
        public static ParallelQuery<TTarget> MapObjectsAsParallel<TSource, TTarget>(this IEnumerable<TSource> sources,
                                                                                    Func<TTarget> targetFactory,
                                                                                    params Expression<Func<TTarget, object>>[]
                                                                                        propertyExprsToExclude) {
            return MapObjectsAsParallel(sources, targetFactory, DefaultOptions, null, propertyExprsToExclude);
        }

        /// <summary>
        /// 원본 시퀀스를 이용하여 대상 객체를 생성한 후, 병렬 방식으로 속성 값을 매핑하여 반환합니다.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget">대상 인스턴스 수형</typeparam>
        /// <param name="sources">원본 인스턴스 시퀀스</param>
        /// <param name="targetFactory">대상 인스턴스 생성 팩토리</param>
        /// <param name="mapOptions">매핑 시의 옵션</param>
        /// <param name="propertyExprsToExclude">제외할 속성명</param>
        /// <returns>매핑된 대상 객체 시퀀스</returns>
        public static ParallelQuery<TTarget> MapObjectsAsParallel<TSource, TTarget>(this IEnumerable<TSource> sources,
                                                                                    Func<TTarget> targetFactory,
                                                                                    MapPropertyOptions mapOptions,
                                                                                    params Expression<Func<TTarget, object>>[]
                                                                                        propertyExprsToExclude) {
            return MapObjectsAsParallel(sources, targetFactory, mapOptions, null, propertyExprsToExclude);
        }

        /// <summary>
        /// 원본 시퀀스를 이용하여 대상 객체를 생성한 후, 병렬 방식으로 속성 값을 매핑하여 반환합니다.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget">대상 인스턴스 수형</typeparam>
        /// <param name="sources">원본 인스턴스 시퀀스</param>
        /// <param name="targetFactory">대상 인스턴스 생성 팩토리</param>
        /// <param name="additionalMapping">추가 매핑 작업</param>
        /// <param name="propertyExprsToExclude">제외할 속성명</param>
        /// <returns>매핑된 대상 객체 시퀀스</returns>
        public static ParallelQuery<TTarget> MapObjectsAsParallel<TSource, TTarget>(this IEnumerable<TSource> sources,
                                                                                    Func<TTarget> targetFactory,
                                                                                    Action<TSource, TTarget> additionalMapping,
                                                                                    params Expression<Func<TTarget, object>>[]
                                                                                        propertyExprsToExclude) {
            return MapObjectsAsParallel(sources, targetFactory, DefaultOptions, additionalMapping, propertyExprsToExclude);
        }

        /// <summary>
        /// 원본 시퀀스를 이용하여 대상 객체를 생성한 후, 병렬 방식으로 속성 값을 매핑하여 반환합니다.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget">대상 인스턴스 수형</typeparam>
        /// <param name="sources">원본 인스턴스 시퀀스</param>
        /// <param name="targetFactory">대상 인스턴스 생성 팩토리</param>
        /// <param name="mapOptions">매핑 시의 옵션</param>
        /// <param name="additionalMapping">추가 매핑 작업</param>
        /// <param name="propertyExprsToExclude">제외할 속성명</param>
        /// <returns>매핑된 대상 객체 시퀀스</returns>
        public static ParallelQuery<TTarget> MapObjectsAsParallel<TSource, TTarget>(this IEnumerable<TSource> sources,
                                                                                    Func<TTarget> targetFactory,
                                                                                    MapPropertyOptions mapOptions,
                                                                                    Action<TSource, TTarget> additionalMapping,
                                                                                    params Expression<Func<TTarget, object>>[]
                                                                                        propertyExprsToExclude) {
            targetFactory.ShouldNotBeNull("targetFactory");

            return
                sources.AsParallel()
                    .AsOrdered()
                    .Select(source => MapObject(source,
                                                targetFactory,
                                                mapOptions,
                                                additionalMapping,
                                                propertyExprsToExclude));
        }

#endif
        //--------------------------------------------------
    }
}