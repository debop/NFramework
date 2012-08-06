using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    /// <summary>
    /// Metadata를 표현하는 인터페이스
    /// </summary>
    public interface IMetadataValue : IDataObject, IEquatable<IMetadataValue> {
        /// <summary>
        /// 메타데이타 값
        /// </summary>
        string Value { get; set; }

        /// <summary>
        /// 메타데이타 값의 형식
        /// </summary>
        string ValueType { get; set; }

        /// <summary>
        /// 메타데이타 화면 표시명
        /// </summary>
        string Label { get; set; }

        /// <summary>
        /// 메타데이타 설명
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// 메타데이타 추가 속성값
        /// </summary>
        string ExAttr { get; set; }
    }
}