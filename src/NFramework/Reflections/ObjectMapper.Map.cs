using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Reflections {
    public static partial class ObjectMapper {
        /// <summary>
        /// 원본 속성명-속성값 정보를 대상 인스턴스의 속성명에 값을 설정한다.
        /// </summary>
        /// <param name="source">원본 정보 (Name-Value)</param>
        /// <param name="target">복사 대상 인스턴스</param>
        /// <param name="propertyNamesToExclude">복사 제외 속성 명</param>
        public static void Map(IDictionary source, object target, params string[] propertyNamesToExclude) {
            Map(source, target, DefaultOptions.SuppressException, propertyNamesToExclude);
        }

        /// <summary>
        /// 원본 속성명-속성값 정보를 대상 인스턴스의 속성명에 값을 설정한다.
        /// </summary>
        /// <param name="source">원본 정보 (Name-Value)</param>
        /// <param name="target">복사 대상 인스턴스</param>
        /// <param name="suppressException">예외 무시 여부</param>
        /// <param name="propertyNamesToExclude">복사 제외 속성 명</param>
        public static void Map(IDictionary source, object target, bool suppressException, params string[] propertyNamesToExclude) {
            Map(source, target, suppressException, DefaultOptions.IgnoreCase, propertyNamesToExclude);
        }

        /// <summary>
        /// 원본 속성명-속성값 정보를 대상 인스턴스의 속성명에 값을 설정한다.
        /// </summary>
        /// <param name="source">원본 정보 (Name-Value)</param>
        /// <param name="target">복사 대상 인스턴스</param>
        /// <param name="suppressException">예외 무시 여부</param>
        /// <param name="ignoreCase">원본과 대상의 속성명 매칭 시에 대소문자 구분을 할 것인가 여부 (기본적으로 대소문자 구분을 한다)</param>
        /// <param name="propertyNamesToExclude">복사 제외 속성 명</param>
        public static void Map(IDictionary source, object target, bool suppressException, bool ignoreCase,
                               params string[] propertyNamesToExclude) {
            Map(source,
                () => target,
                new MapPropertyOptions
                {
                    SuppressException = suppressException,
                    IgnoreCase = ignoreCase
                },
                propertyNamesToExclude);
        }

        /// <summary>
        /// 원본 속성명-속성값 정보를 대상 인스턴스의 속성명에 값을 설정한다.
        /// </summary>
        /// <param name="source">원본 정보 (Name-Value)</param>
        /// <param name="targetFactory">복사 대상 인스턴스 생성 델리게이트</param>
        /// <param name="propertyNamesToExclude">복사 제외 속성 명</param>
        public static object Map(IDictionary source, Func<object> targetFactory, params string[] propertyNamesToExclude) {
            return Map(source, targetFactory, DefaultOptions, propertyNamesToExclude);
        }

        /// <summary>
        /// 원본 속성명-속성값 정보를 대상 인스턴스의 속성명에 값을 설정한다.
        /// </summary>
        /// <param name="source">원본 정보 (Name-Value)</param>
        /// <param name="targetFactory">복사 대상 인스턴스 생성 델리게이트</param>
        /// <param name="mapOptions">매핑 옵션</param>
        /// <param name="propertyNamesToExclude">복사 제외 속성 명</param>
        public static object Map(IDictionary source, Func<object> targetFactory, MapPropertyOptions mapOptions,
                                 params string[] propertyNamesToExclude) {
            targetFactory.ShouldNotBeNull("targetFactory");

            var target = targetFactory();

            if(IsDebugEnabled)
                log.Debug("원본의 속성-값을 대상 인스턴스의 속성 값으로 복사합니다... " +
                          @"source=[{0}], target=[{1}], mapOptions=[{2}], propertyNamesToExclude=[{3}]",
                          source, target, mapOptions, propertyNamesToExclude.CollectionToString());

            var excludes = new List<string>(propertyNamesToExclude);
            var accessor = DynamicAccessorFactory.CreateDynamicAccessor(target.GetType(), mapOptions.SuppressException);
            var targetPropertyNames = accessor.GetPropertyNames().Except(excludes).ToList();

            foreach(string name in source.Keys) {
                var sourceName = name;
                if(excludes.Any(epn => StringTool.EqualTo(epn, sourceName)))
                    continue;

                var canSetPropertyValue = targetPropertyNames.Any(tpn => StringTool.EqualTo(tpn, sourceName));

                if(canSetPropertyValue) {
                    if(mapOptions.IgnoreCase) {
                        var targetPropertyName = targetPropertyNames.FirstOrDefault(tpn => StringTool.EqualTo(tpn, sourceName));

                        if(targetPropertyName.IsNotWhiteSpace())
                            accessor.SetPropertyValue(target, targetPropertyName, source[sourceName]);
                    }
                    else {
                        accessor.SetPropertyValue(target, sourceName, source[sourceName]);
                    }
                }
            }
            return target;
        }

        /// <summary>
        /// 원본 인스턴스의 속성 값을 읽어와 대상 인스턴스의 같은 속성명에 값을 설정한다.
        /// </summary>
        /// <param name="source">원본 객체</param>
        /// <param name="target">대상 객체</param>
        /// <param name="suppressException">예외 발생 전파 억제</param>
        /// <param name="ignoreCase">원본과 대상의 속성명 매칭 시에 대소문자 구분을 할 것인가 여부 (기본적으로 대소문자 구분을 한다)</param>
        /// <param name="propertyNamesToExclude">매핑시 제외할 속석명들</param>
        public static void Map(object source,
                               object target,
                               bool suppressException = true,
                               bool ignoreCase = true,
                               string[] propertyNamesToExclude = null) {
            target.ShouldNotBeNull("target");

            MapProperty(source,
                        target,
                        new MapPropertyOptions
                        {
                            SuppressException = suppressException,
                            IgnoreCase = ignoreCase
                        },
                        propertyNamesToExclude);
        }
    }
}