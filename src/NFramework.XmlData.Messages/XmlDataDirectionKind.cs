using System;

namespace NSoft.NFramework.XmlData.Messages {
    /// <summary>
    /// 메시지 방향 종류 : 요청/응답 구분
    /// </summary>
    [Serializable]
    public enum XmlDataDirectionKind {
        /// <summary>
        /// 요청 메시지임을 나타낸다.
        /// </summary>
        Request,

        /// <summary>
        /// 응답 메시지임을 나타낸다.
        /// </summary>
        Response
    }
}