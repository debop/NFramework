using System;
using System.Collections.Generic;
using System.Reflection;

namespace NSoft.NFramework.Reflections {
    /// <summary>
    /// Reflection을 이용하는 것보다 Emit을 이용하여 인스턴스의 객체 정보를 조작하는 것이 성능이 좋다.
    /// </summary>
    public interface IDynamicAccessor {
        /// <summary>
        /// 동적으로 정보에 접근하고자하는 수형
        /// </summary>
        Type TargetType { get; }

        /// <summary>
        /// 지정한 인스턴스의 필드 값을 가져온다.
        /// </summary>
        /// <param name="target">인스턴스</param>
        /// <param name="fieldName">필드명</param>
        /// <returns>필드 값</returns>
        object GetFieldValue(object target, string fieldName);

        /// <summary>
        /// 인스턴스의 지정한 필드명의 값을 가져옵니다. 해당 필드가 없다면, false를 반환합니다.
        /// </summary>
        /// <param name="target">인스턴스</param>
        /// <param name="fieldName">필드명</param>
        /// <param name="fieldValue">필드 값</param>
        /// <returns>조회 성공 여부</returns>
        bool TryGetFieldValue(object target, string fieldName, out object fieldValue);

        /// <summary>
        /// 지정한 인스턴스의 필드 값을 설정한다.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="fieldName"></param>
        /// <param name="fieldValue"></param>
        void SetFieldValue(object target, string fieldName, object fieldValue);

        /// <summary>
        /// 지정한 인스턴스의 속성 값을 가져온다.
        /// </summary>
        /// <param name="target">인스턴스</param>
        /// <param name="propertyName">속성 명</param>
        /// <returns>속성 값</returns>
        object GetPropertyValue(object target, string propertyName);

        /// <summary>
        /// 인스턴스의 지정한 속성 명의 값을 가져옵니다. 해당 속성이 없다면, false를 반환합니다.
        /// </summary>
        /// <param name="target">인스턴스</param>
        /// <param name="propertyName">속성 명</param>
        /// <param name="propertyValue">속성 값</param>
        /// <returns>조회 여부</returns>
        bool TryGetPropertyValue(object target, string propertyName, out object propertyValue);

        /// <summary>
        /// 지정한 인스턴스의 속성 값을 설정한다.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        void SetPropertyValue(object target, string propertyName, object propertyValue);

        /// <summary>
        /// Indexer
        /// </summary>
        /// <param name="target"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        object this[object target, string propertyName] { get; set; }

        /// <summary>
        /// 필드 정보
        /// </summary>
        IDictionary<string, FieldInfo> FieldMap { get; }

        /// <summary>
        /// 속성 정보
        /// </summary>
        IDictionary<string, PropertyInfo> PropertyMap { get; }

        /// <summary>
        /// Get property names
        /// </summary>
        /// <returns></returns>
        IList<string> GetPropertyNames();

        /// <summary>
        /// Get field names
        /// </summary>
        /// <returns></returns>
        IList<string> GetFieldNames();

        /// <summary>
        /// 지정된 속성의 형식을 반환한다.
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <returns>Type of Field</returns>
        /// <exception cref="InvalidOperationException">필드가 존재하지 않을 때</exception>
        Type GetFieldType(string fieldName);

        /// <summary>
        /// 지정된 속성의 형식을 반환한다.
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <returns>Type of Property</returns>
        /// <exception cref="InvalidOperationException">속성이 존재하지 않을 때</exception>
        Type GetPropertyType(string propertyName);
    }
}