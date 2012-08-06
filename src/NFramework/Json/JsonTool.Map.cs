using System;
using NSoft.NFramework.Reflections;
using Newtonsoft.Json;

namespace NSoft.NFramework.Json {
    /// <summary>
    /// JSON 포맷으로 객체 Mapping을 수행합니다.
    /// </summary>
    public static partial class JsonTool {
        /// <summary>
        ///  객체를 JSON 직렬화/역직렬화를 통해, T 수형의 인스턴스를 빌드합니다.
        /// </summary>
        /// <typeparam name="T">대상 수형</typeparam>
        /// <param name="source">원본객체</param>
        /// <param name="serializerSettings">JSON 직렬화 설정 정보</param>
        /// <param name="additionalMapping">부가 매핑 정보</param>
        /// <returns>매핑된 T 수형의 객체</returns>
        public static T Map<T>(object source, JsonSerializerSettings serializerSettings = null,
                               Action<object, T> additionalMapping = null) {
            source.ShouldNotBeNull("source");

            T target;
            if(TryMap(source, serializerSettings ?? DefaultJsonSerializerSettings, out target)) {
                if(additionalMapping != null)
                    additionalMapping(source, target);

                return target;
            }

            return default(T);
        }

        /// <summary>
        /// 객체를 JSON 직렬화/역직렬화를 통해, <paramref name="targetType"/> 수형의 인스턴스를 빌드합니다.
        /// </summary>
        /// <param name="source">원본 객체</param>
        /// <param name="targetType">대상 수형</param>
        /// <param name="serializerSettings">JSON 직렬화 설정 정보</param>
        /// <returns>매핑된 대상 수형의 객체</returns>
        public static object Map(object source, Type targetType, JsonSerializerSettings serializerSettings = null) {
            source.ShouldNotBeNull("source");
            targetType.ShouldNotBeNull("targetType");

            object target;

            if(TryMap(source, targetType, serializerSettings ?? DefaultJsonSerializerSettings, out target))
                return target;

            return null;
        }

        /// <summary>
        /// 원본 객체를 JSON 포맷으로 직렬화를 수행하고, 대상 형식으로 역직렬화를 수행합니다. 두 수형이 달라도 상관없습니다.
        /// </summary>
        /// <param name="source">원본 객체</param>
        /// <param name="target">대상 객체</param>
        /// <returns>Mapping 성공 여부</returns>
        /// <see cref="ObjectMapper"/>
        public static bool TryMap<T>(object source, out T target) {
            return TryMap<T>(source, DefaultJsonSerializerSettings, out target);
        }

        /// <summary>
        /// 원본 객체를 JSON 포맷으로 직렬화를 수행하고, 대상 형식으로 역직렬화를 수행합니다. 두 수형이 달라도 상관없습니다.
        /// </summary>
        /// <param name="source">원본 객체</param>
        /// <param name="target">대상 객체</param>
        /// <param name="serializerSettings">JSON 직렬화 설정 정보</param>
        /// <returns>Mapping 성공 여부</returns>
        /// <see cref="ObjectMapper"/>
        public static bool TryMap<T>(object source, JsonSerializerSettings serializerSettings, out T target) {
            if(source == null) {
                target = default(T);
                return false;
            }

            Type targetType = typeof(T);
            target = default(T);
            object targetObject;

            var result = TryMap(source, targetType, serializerSettings, out targetObject);
            if(result)
                target = (T)targetObject;

            return result;
        }

        /// <summary>
        /// 원본 객체를 JSON 포맷으로 직렬화를 수행하고, 대상 수형으로 역직렬화를 수행합니다. 두 수형이 달라도 상관없습니다.
        /// </summary>
        /// <param name="source">원본 객체</param>
        /// <param name="targetType">대상 객체의 수형</param>
        /// <param name="target">대상 객체</param>
        /// <returns>Mapping 성공 여부</returns>
        /// <see cref="ObjectMapper"/>
        public static bool TryMap(object source, Type targetType, out object target) {
            return TryMap(source, targetType, DefaultJsonSerializerSettings, out target);
        }

        /// <summary>
        /// 원본 객체를 JSON 포맷으로 직렬화를 수행하고, 대상 수형으로 역직렬화를 수행합니다. 두 수형이 달라도 상관없습니다.
        /// </summary>
        /// <param name="source">원본 객체</param>
        /// <param name="targetType">대상 객체의 수형</param>
        /// <param name="serializerSettings">JSON 직렬화 설정 정보</param>
        /// <param name="target">대상 객체</param>
        /// <returns>Mapping 성공 여부</returns>
        /// <see cref="ObjectMapper"/>
        public static bool TryMap(object source, Type targetType, JsonSerializerSettings serializerSettings, out object target) {
            target = null;

            try {
                source.ShouldNotBeNull("source");
                targetType.ShouldNotBeNull("targetType");
                serializerSettings.ShouldNotBeNull("serializerSettings");

                if(IsDebugEnabled) {
                    log.Debug("원본 객체를 JSON 포맷으로 직렬화를 수행하고, 대상 수형[{0}]으로 역직렬화를 수행합니다.", targetType.FullName);
                    log.Debug("source=[{0}], targetType=[{1}], serializerSettings=[{2}]", source, targetType, serializerSettings);
                }

                var jsonText = SerializeAsText(source, null, serializerSettings);
                target = DeserializeFromText(jsonText, targetType, serializerSettings);

                if(IsDebugEnabled)
                    log.Debug("원본 객체로부터 대상 객체로 매핑을 수행했습니다. target=[{0}]", target);

                return (target != null);
            }
            catch(Exception ex) {
                if(log.IsWarnEnabled)
                    log.WarnException("JSON 포맷을 이용한 객체 Mapping 작업에 실패했습니다.", ex);

                return false;
            }
        }
    }
}