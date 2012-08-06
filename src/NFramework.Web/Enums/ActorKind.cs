namespace NSoft.NFramework.Web
{
    /// <summary>
    /// 행위를 수행하는 주체 (사용자, 부서, 회사, 그룹, 롤 등)
    /// </summary>
    public enum ActorKind
    {
        /// <summary>
        /// 알 수 없음
        /// </summary>
        Unkown,

        /// <summary>
        /// 사용자
        /// </summary>
        User,

        /// <summary>
        /// 조직
        /// </summary>
        Organization,

        /// <summary>
        /// 회사
        /// </summary>
        Company,

        /// <summary>
        /// 그룹
        /// </summary>
        Group,

        /// <summary>
        /// Role
        /// </summary>
        Role
    }
}