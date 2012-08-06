using System;

namespace NSoft.NFramework.XmlData.Messages {
    /// <summary>
    /// Xml Data 요청의 종류를 나타냅니다. 요청은 StoredProcedure 호출, 일반 Query문 호출로 구분합니다.
    /// </summary>
    [Serializable]
    public enum XmlDataRequestKind {
        /// <summary>
        /// Stored Procedure 실행 요청
        /// </summary>
        StoredProc,

        /// <summary>
        /// 일반 SQL Query 문 실행 요청
        /// </summary>
        Query,

        /// <summary>
        /// 서버상에 정의된 Method를 호출한다. (ini 파일로 저장되어 있음)
        /// </summary>
        Method
    }
}