using System;
using System.Collections.Generic;
using System.Reflection;

namespace NSoft.NFramework.Reflections {
    /// <summary>
    /// Dynamic Method를 이용하여, 객체의 속성, 필드 정보를 조회/설정할 수 있는 인터페이스
    /// </summary>
    /// <typeparam name="T">대상 객체의 형식</typeparam>
    public interface IDynamicAccessor<T> {
        /// <summary>
        /// 대상 수형
        /// </summary>
        Type TargetType { get; }

        /// <summary>
        /// 지정한 인스턴스의 필드 값을 가져온다.
        /// </summary>
        /// <param name="target">{T}의 인스턴스</param>
        /// <param name="fieldName">필드명</param>
        /// <returns>인스턴스의 필드명에 해당하는 값</returns>
        /// <exception cref="InvalidOperationException">해당 인스턴스가 필드 명에 해당하는 필드가 없을 때</exception>
        object GetFieldValue(T target, string fieldName);

        /// <summary>
        /// 지정한 인스턴스의 필드 값을 가져온다.
        /// </summary>
        /// <param name="target">{T}의 인스턴스</param>
        /// <param name="fieldName">필드 명</param>
        /// <param name="fieldValue">필드 값</param>
        /// <returns>필드 값 조회 여부</returns>
        bool TryGetFieldValue(T target, string fieldName, out object fieldValue);

        /// <summary>
        /// 지정한 인스턴스의 필드 값을 설정한다.
        /// </summary>
        /// <param name="target">대상 객체</param>
        /// <param name="fieldName">필드 명</param>
        /// <param name="fieldValue">필드 값</param>
        /// <exception cref="InvalidOperationException">해당 인스턴스가 필드 명에 해당하는 필드가 없을 때</exception>
        void SetFieldValue(T target, string fieldName, object fieldValue);

        /// <summary>
        /// 지정한 인스턴스의 속성 값을 가져온다.
        /// </summary>
        /// <param name="target">대상 객체</param>
        /// <param name="propertyName">속성 명</param>
        /// <returns>대상 객체의 속성명에 해당하는 속성의 값</returns>
        /// <exception cref="InvalidOperationException">해당 인스턴스가 속성명에 해당하는 속성이 정의되어 있지 않을 때</exception>
        object GetPropertyValue(T target, string propertyName);

        /// <summary>
        /// 지정한 인스턴스의 속성 명에 해당하는 값을 조회합니다.
        /// </summary>
        /// <param name="target">대상 인스턴스</param>
        /// <param name="propertyName">속성 명</param>
        /// <param name="propertyValue">속성 값</param>
        /// <returns>조회 성공 여부</returns>
        bool TryGetPropertyValue(T target, string propertyName, out object propertyValue);

        /// <summary>
        /// 지정한 인스턴스의 속성 값을 설정한다.
        /// </summary>
        /// <param name="target">대상 객체</param>
        /// <param name="propertyName">속성 명</param>
        /// <param name="propertyValue">설정할 속성 값</param>
        /// <exception cref="InvalidOperationException">해당 인스턴스가 속성명에 해당하는 속성이 정의되어 있지 않을 때</exception>
        void SetPropertyValue(T target, string propertyName, object propertyValue);

        /// <summary>
        /// Indexer
        /// </summary>
        /// <param name="target"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        object this[T target, string propertyName] { get; set; }

        /// <summary>
        /// 필드 정보
        /// </summary>
        IDictionary<string, FieldInfo> FieldMap { get; }

        /// <summary>
        /// 속성 정보
        /// </summary>
        IDictionary<string, PropertyInfo> PropertyMap { get; }

        /// <summary>
        /// Public Property들의 속성명들을 가져온다
        /// </summary>
        /// <returns>해당 형식의 Public 속성명 리스트</returns>
        IList<string> GetPropertyNames();

        /// <summary>
        /// 해당 형식의 지정된 BindingFlag에 의해 조사된 속성명들을 반환한다.
        /// </summary>
        /// <param name="bindingFlags">Reflection BindingFlags</param>
        /// <returns>속성명 리스트</returns>
        IList<string> GetPropertyNames(BindingFlags bindingFlags);

        /// <summary>
        /// Public Field들의 속성명을 가져온다
        /// </summary>
        /// <returns>Collection of Field names</returns>
        IList<string> GetFieldNames();

        /// <summary>
        /// 해당 형식의 지정된 BindingFlag에 의해 조사된 필드명들을 반환한다.
        /// </summary>
        /// <param name="bindingFlags"></param>
        /// <returns>Collectio of Field names</returns>
        IList<string> GetFieldNames(BindingFlags bindingFlags);

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