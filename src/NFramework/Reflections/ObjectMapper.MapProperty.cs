using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Reflections {
    public static partial class ObjectMapper {
        /// <summary>
        /// 원본 인스턴스의 속성 값을 읽어와 새로 생성한 인스턴스의 속성에 값을 설정하여 반환합니다.
        /// 서로 다른 형식이지만, 속성이 비슷한 형식끼리 값을 복사할 때 편리합니다. (DTO 사용 시)
        /// </summary>
        /// <typeparam name="TTarget">매핑될 대상 형식</typeparam>
        /// <param name="source">매핑정보를 제공할 인스턴스</param>
        /// <param name="suppressException">속성 복사시에 발생하는 예외를 무시할 것인가 여부</param>
        /// <param name="ignoreCase">원본과 대상의 속성명 매칭 시에 대소문자 구분을 할 것인가 여부 (기본적으로 대소문자 구분을 한다)</param>
        /// <param name="propertyNamesToExclude">제외할 속성명</param>
        /// <returns>매핑된 대상 인스턴스</returns>
        public static TTarget MapProperty<TTarget>(this object source,
                                                   bool suppressException = true,
                                                   bool ignoreCase = true,
                                                   string[] propertyNamesToExclude = null) where TTarget : new() {
            return MapProperty<TTarget>(source,
                                        ActivatorTool.CreateInstance<TTarget>,
                                        new MapPropertyOptions
                                        {
                                            SuppressException = suppressException,
                                            IgnoreCase = ignoreCase
                                        },
                                        propertyNamesToExclude);
        }

        /// <summary>
        /// 원본 인스턴스의 속성 값을 기준으로, 새로 생성한 TTarget 수형의 인스턴스의 속성 값에 매핑합니다. Data Transfer Object 를 이용할 때 상당히 유용한 함수입니다.
        /// </summary>
        /// <typeparam name="TTarget">매핑될 대상 형식</typeparam>
        /// <param name="source">매핑정보를 제공할 인스턴스</param>
        /// <param name="targetFactory">대상 형식의 인스턴스를 생성해주는 Factory 메소드</param>
        /// <param name="suppressException">속성 복사시에 발생하는 예외를 무시할 것인가 여부</param>
        /// <param name="ignoreCase">원본과 대상의 속성명 매칭 시에 대소문자 구분을 할 것인가 여부 (기본적으로 대소문자 구분을 한다)</param>
        /// <param name="propertyNamesToExclude">제외할 속성명</param>
        /// <returns>매핑된 대상 인스턴스</returns>
        public static TTarget MapProperty<TTarget>(this object source,
                                                   Func<TTarget> targetFactory,
                                                   bool suppressException = true,
                                                   bool ignoreCase = true,
                                                   string[] propertyNamesToExclude = null) {
            targetFactory.ShouldNotBeNull("targetFactory");

            return MapProperty(source,
                               targetFactory,
                               new MapPropertyOptions
                               {
                                   SuppressException = suppressException,
                                   IgnoreCase = ignoreCase
                               },
                               propertyNamesToExclude);
        }

        /// <summary>
        /// 원본 인스턴스의 속성 값을 읽어와 대상 인스턴스의 같은 속성명에 값을 설정한다.
        /// 서로 다른 형식이지만, 속성이 비슷한 형식끼리 값을 복사할 때 편리합니다. (DTO 사용 시)
        /// </summary>
        /// <typeparam name="TTarget">대상 인스턴스 수형</typeparam>
        /// <param name="source">원본 인스턴스</param>
        /// <param name="target">대상 인스턴스</param>
        /// <param name="suppressException">속성값 Get/Set 시 예외 처리를 무시할 것인가 여부</param>
        /// <param name="ignoreCase">원본과 대상의 속성명 매칭 시에 대소문자 구분을 할 것인가 여부 (기본적으로 대소문자 구분을 한다)</param>
        /// <param name="propertyNamesToExclude">제외할 속성명</param>
        public static void MapProperty<TTarget>(this object source,
                                                TTarget target,
                                                bool suppressException = true,
                                                bool ignoreCase = true,
                                                string[] propertyNamesToExclude = null) {
            target.ShouldNotBeNull("target");

            MapProperty<TTarget>(source, target,
                                 new MapPropertyOptions
                                 {
                                     SuppressException = suppressException,
                                     IgnoreCase = ignoreCase
                                 },
                                 propertyNamesToExclude);
        }

        /// <summary>
        /// 원본 인스턴스의 속성 값을 읽어와 대상 인스턴스의 속성에 매핑합니다.
        /// </summary>
        /// <typeparam name="TTarget">대상 인스턴스 수형</typeparam>
        /// <param name="source">원본 인스턴스</param>
        /// <param name="targetFactory">대상 인스턴스 생성 함수</param>
        /// <param name="mapOptions">매핑 시의 옵션</param>
        /// <param name="propertyNamesToExclude">제외할 속성명</param>
        /// <result>매핑 대상 인스턴스</result>
        public static TTarget MapProperty<TTarget>(this object source,
                                                   Func<TTarget> targetFactory,
                                                   MapPropertyOptions mapOptions,
                                                   string[] propertyNamesToExclude = null) {
            targetFactory.ShouldNotBeNull("targetFactory");

            mapOptions = mapOptions ?? MapPropertyOptions.Safety;

            var target = targetFactory();
            Guard.Assert(() => !Equals(target, default(TTarget)));

            if(IsDebugEnabled)
                log.Debug("소스 인스턴스의 속성 정보를 대상 인스턴스[{2}]의 속성 값에 설정합니다. source=[{0}], target=[{1}]", source, target,
                          typeof(TTarget).FullName);

            var sourceAccessor = DynamicAccessorFactory.CreateDynamicAccessor(source.GetType(), mapOptions.SuppressException);
            var targetAccessor = DynamicAccessorFactory.CreateDynamicAccessor(target.GetType(), mapOptions.SuppressException);

            var excludes = (propertyNamesToExclude != null) ? new List<string>(propertyNamesToExclude) : new List<string>();

            if(IsDebugEnabled)
                log.Debug("속성 설젱에서 제외할 속성들=[{0}]", excludes.CollectionToString());

            var sourcePropertyNames = sourceAccessor.GetPropertyNames();
            var targetPropertyNames = targetAccessor.GetPropertyNames().Except(excludes).ToList();

            if(IsDebugEnabled)
                log.Debug("설정할 속성들=[{0}]", targetPropertyNames.CollectionToString());

            var sourceTypeName = source.GetType().FullName;
            var targetTypeName = typeof(TTarget).FullName;

            foreach(var propertyName in targetPropertyNames) {
                var targetName = propertyName;

                if(excludes.Any(epn => epn.EqualTo(targetName)))
                    continue;

                var sourceName = sourcePropertyNames.FirstOrDefault(spn => spn.EqualTo(targetName));

                if(sourceName.IsNotWhiteSpace()) {
                    if(mapOptions.SkipAlreadyExistValue) {
                        var targetType = targetAccessor.GetPropertyType(targetName);
                        var targetValue = targetAccessor.GetPropertyValue(target, targetName);

                        if(Equals(targetValue, targetType.GetTypeDefaultValue()) == false) {
                            if(IsDebugEnabled)
                                log.Debug("대상 객체의 속성[{0}]에 이미 값이 설정되어 있어, 설정을 건너뜁니다. 속성값=[{1}]", targetName, targetValue);
                            continue;
                        }
                    }

                    if(IsDebugEnabled)
                        log.Debug("원본객체[{0}] => 대상객체[{1}]로 속성[{2}]의 값을 할당합니다...", sourceTypeName, targetTypeName, propertyName);

                    var propertyValue = sourceAccessor.GetPropertyValue(source, sourceName);

                    targetAccessor.SetPropertyValue(target, targetName, propertyValue);

                    if(IsDebugEnabled)
                        log.Debug("속성[{0}]에 할당된 값은 [{1}]입니다.", targetName, propertyValue);
                }
            }
            return target;
        }

        /// <summary>
        /// 원본 인스턴스의 속성 값을 읽어와 대상 인스턴스의 속성 값을 매핑합니다.
        /// </summary>
        /// <typeparam name="TTarget">대상 인스턴스 수형</typeparam>
        /// <param name="source">원본 인스턴스</param>
        /// <param name="target">대상 인스턴스</param>
        /// <param name="mapOptions">매핑 시의 옵션</param>
        /// <param name="propertyNamesToExclude">제외할 속성명</param>
        public static void MapProperty<TTarget>(this object source,
                                                TTarget target,
                                                MapPropertyOptions mapOptions,
                                                string[] propertyNamesToExclude = null) {
            target.ShouldNotBeNull("target");
            MapProperty(source, () => target, mapOptions, propertyNamesToExclude);
        }

        /// <summary>
        /// 원본 인스턴스의 속성 값을 읽어와 대상 인스턴스의 속성에 매핑합니다.
        /// </summary>
        /// <typeparam name="TTarget">대상 인스턴스 수형</typeparam>
        /// <param name="source">원본 인스턴스</param>
        /// <param name="targetFactory">대상 인스턴스 생성 함수</param>
        /// <param name="propertyExprsToExclude">제외할 속성명</param>
        /// <result>매핑 대상 인스턴스</result>
        public static TTarget MapProperty<TTarget>(this object source,
                                                   Func<TTarget> targetFactory,
                                                   params Expression<Func<TTarget, object>>[] propertyExprsToExclude) {
            return MapProperty(source, targetFactory, DefaultOptions, propertyExprsToExclude);
        }

        /// <summary>
        /// 원본 인스턴스의 속성 값을 읽어와 대상 인스턴스의 속성에 매핑합니다.
        /// </summary>
        /// <typeparam name="TTarget">대상 인스턴스 수형</typeparam>
        /// <param name="source">원본 인스턴스</param>
        /// <param name="targetFactory">대상 인스턴스 생성 함수</param>
        /// <param name="mapOptions">매핑 시의 옵션</param>
        /// <param name="propertyExprsToExclude">제외할 속성명</param>
        /// <result>매핑 대상 인스턴스</result>
        public static TTarget MapProperty<TTarget>(this object source,
                                                   Func<TTarget> targetFactory,
                                                   MapPropertyOptions mapOptions,
                                                   params Expression<Func<TTarget, object>>[] propertyExprsToExclude) {
            targetFactory.ShouldNotBeNull("targetFactory");

            var target = targetFactory();
            target.ShouldNotBeDefault("target");

            if(IsDebugEnabled)
                log.Debug("소스 인스턴스의 속성 정보를 대상 인스턴스[{2}]의 속성 값에 설정합니다. source=[{0}], target=[{1}]", source, target,
                          typeof(TTarget).FullName);

            var sourceAccessor = DynamicAccessorFactory.CreateDynamicAccessor(source.GetType(), mapOptions.SuppressException);
            var targetAccessor = DynamicAccessorFactory.CreateDynamicAccessor<TTarget>(mapOptions.SuppressException);

            var propertyNamesToExclude = propertyExprsToExclude.Select(expr => LinqTool.FindPropertyName(expr.Body));

            var excludes = new List<string>(propertyNamesToExclude);

            if(IsDebugEnabled)
                log.Debug("속성 설젱에서 제외할 속성들=[{0}]", excludes.CollectionToString());

            var sourcePropertyNames = sourceAccessor.GetPropertyNames();
            var targetPropertyNames = targetAccessor.GetPropertyNames().Except(excludes).ToList();

            if(IsDebugEnabled)
                log.Debug("설정할 속성들=[{0}]", targetPropertyNames.CollectionToString());

            var sourceTypeName = source.GetType().FullName;
            var targetTypeName = typeof(TTarget).FullName;

            foreach(var propertyName in targetPropertyNames) {
                var targetName = propertyName;

                if(excludes.Any(epn => StringTool.EqualTo(epn, targetName)))
                    continue;

                var sourceName = sourcePropertyNames.FirstOrDefault(spn => StringTool.EqualTo(spn, targetName));

                if(sourceName.IsNotWhiteSpace()) {
                    if(mapOptions.SkipAlreadyExistValue) {
                        var targetType = targetAccessor.GetPropertyType(targetName);
                        var targetValue = targetAccessor.GetPropertyValue(target, targetName);

                        if(Equals(targetValue, targetType.GetTypeDefaultValue()) == false) {
                            if(IsDebugEnabled)
                                log.Debug("대상 객체의 속성[{0}]에 이미 값이 설정되어 있어, 설정을 건너뜁니다. 속성값=[{1}]", targetName, targetValue);
                            continue;
                        }
                    }

                    if(IsDebugEnabled)
                        log.Debug("원본객체[{0}] => 대상객체[{1}]로 속성[{2}]의 값을 할당합니다...", sourceTypeName, targetTypeName, propertyName);

                    var propertyValue = sourceAccessor.GetPropertyValue(source, sourceName);
                    targetAccessor.SetPropertyValue(target, targetName, propertyValue);

                    if(IsDebugEnabled)
                        log.Debug("속성[{0}]에 할당된 값은 [{1}]입니다.", targetName, propertyValue);
                }
            }
            return target;
        }

        /// <summary>
        /// 원본 인스턴스의 속성 값을 읽어와 대상 인스턴스의 속성 값을 매핑합니다.
        /// </summary>
        /// <typeparam name="TTarget">대상 인스턴스 수형</typeparam>
        /// <param name="source">원본 인스턴스</param>
        /// <param name="target">대상 인스턴스</param>
        /// <param name="mapOptions">매핑 시의 옵션</param>
        /// <param name="propertyExprsToExclude">제외할 속성명</param>
        public static void MapProperty<TTarget>(this object source,
                                                TTarget target,
                                                MapPropertyOptions mapOptions,
                                                params Expression<Func<TTarget, object>>[] propertyExprsToExclude) {
            target.ShouldNotBeNull("target");
            MapProperty(source, () => target, mapOptions, propertyExprsToExclude);
        }
    }
}