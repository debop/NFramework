using System;

namespace NSoft.NFramework.Web
{
    /// <summary>
    /// 메시지 다이얼로그 창에 표시할 버튼의 종류
    /// </summary>
    [Flags]
    public enum MessageButtons
    {
        /// <summary>
        /// 없음
        /// </summary>
        None = 0x00,

        /// <summary>
        /// 다이얼로그 창 닫기
        /// </summary>
        Close = 0x01,

        /// <summary>
        /// 확인
        /// </summary>
        Ok = 0x02,

        /// <summary>
        /// 뒤로
        /// </summary>
        Back = 0x04,

        /// <summary>
        /// 로그인 페이지로 이동
        /// </summary>
        Login = 0x08,

        /// <summary>
        /// 확인, 닫기
        /// </summary>
        OkCancel = Ok | Close,

        /// <summary>
        /// 확안, 뒤로
        /// </summary>
        OkBack = Ok | Back,

        /// <summary>
        /// 확인, 닫기, 뒤로
        /// </summary>
        OkCancelBack = Ok | Close | Back
    }
}