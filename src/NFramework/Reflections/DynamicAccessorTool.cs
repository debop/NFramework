using System.Collections.Generic;
using System.Linq;

namespace NSoft.NFramework.Reflections {
    /// <summary>
    /// DynamicAccessor에 대한 확장 메소드를 제공합니다.
    /// </summary>
    public static class DynamicAccessorTool {
        /// <summary>
        /// 객체의 모든 필드 정보를 필드명-필드값 형식으로 반환합니다.
        /// </summary>
        /// <param name="accessor">IDynamicAccessor 인스턴스</param>
        /// <param name="target">대상 객체</param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, object>> GetFieldNameValueCollection(this IDynamicAccessor accessor,
                                                                                            object target) {
            return
                accessor.GetFieldNames()
                    .Select(name => new KeyValuePair<string, object>(name, accessor.GetFieldValue(target, name)));
        }

        /// <summary>
        /// 객체의 모든 필드 정보를 필드명-필드값 형식으로 반환합니다.
        /// </summary>
        /// <typeparam name="T">대상 객체의 수형</typeparam>
        /// <param name="accessor">IDynamicAccessor{T} 인스턴스</param>
        /// <param name="target">대상 객체</param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, object>> GetFieldNameValueCollection<T>(this IDynamicAccessor<T> accessor,
                                                                                               T target) {
            return
                accessor.GetFieldNames()
                    .Select(name => new KeyValuePair<string, object>(name, accessor.GetFieldValue(target, name)));
        }

        /// <summary>
        /// 객체의 모든 속성에 대해 속성명-속성값 형식을 반환합니다.
        /// </summary>
        /// <param name="accessor">IDynamicAccessor 인스턴스</param>
        /// <param name="target">대상 객체</param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, object>> GetPropertyNameValueCollection(this IDynamicAccessor accessor,
                                                                                               object target) {
            return
                accessor.GetPropertyNames()
                    .Select(name => new KeyValuePair<string, object>(name, accessor.GetPropertyValue(target, name)));
        }

        /// <summary>
        /// 객체의 모든 속성에 대해 속성명-속성값 형식을 반환합니다.
        /// </summary>
        /// <typeparam name="T">대상 객체의 수형</typeparam>
        /// <param name="accessor">IDynamicAccessor{T} 인스턴스</param>
        /// <param name="target">대상 객체</param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, object>> GetPropertyNameValueCollection<T>(this IDynamicAccessor<T> accessor,
                                                                                                  T target) {
            return
                accessor.GetPropertyNames()
                    .Select(name => new KeyValuePair<string, object>(name, accessor.GetPropertyValue(target, name)));
        }
    }
}