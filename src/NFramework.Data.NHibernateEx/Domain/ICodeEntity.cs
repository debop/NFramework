namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    /// <summary>
    /// Code 속성을 가지는 엔티티
    /// </summary>
    public interface ICodeEntity {
        /// <summary>
        /// 엔티티의 Business Identity를 위한 Code 정보 (예: UserCode, OrderCode 등)
        /// </summary>
        string Code { get; set; }
    }
}