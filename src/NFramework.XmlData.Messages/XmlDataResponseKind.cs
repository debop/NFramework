using System;

namespace NSoft.NFramework.XmlData.Messages {
    /// <summary>
    /// 요청에 대한 반환 형태를 나타냄
    /// </summary>
    [Serializable]
    public enum XmlDataResponseKind {
        /// <summary>
        /// 결과값 필요 없음
        /// </summary>
        None,

        /// <summary>
        /// DataSet 형태의 결과를 요청
        /// </summary>
        DataSet,

        /// <summary>
        /// Scalar 값 반환을 요청
        /// </summary>
        Scalar,

        /// <summary>
        /// Xml 형식 반환을 요청
        /// </summary>
        Xml
    }
}