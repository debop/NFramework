namespace NSoft.NFramework.Web
{
    /// <summary>
    /// 엔티티 편집 모드 종류 (생성, 읽기, 수정, 삭제)
    /// </summary>
    public enum EntityEditMode
    {
        /// <summary>
        /// 알 수 없음
        /// </summary>
        Undefiend,

        /// <summary>
        /// 추가
        /// </summary>
        Create,

        /// <summary>
        /// 읽기
        /// </summary>
        Read,

        /// <summary>
        /// 수정
        /// </summary>
        Update,

        /// <summary>
        /// 삭제
        /// </summary>
        Delete
    }
}