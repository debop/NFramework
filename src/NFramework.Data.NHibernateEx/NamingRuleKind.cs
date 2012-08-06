namespace NSoft.NFramework.Data.NHibernateEx {
    /// <summary>
    /// Naming 종류
    /// </summary>
    public enum NamingRuleKind {
        /// <summary>
        /// Pascal Naming 규칙 (속셩명 Naming과 동일)
        /// </summary>
        Pascal,

        /// <summary>
        /// Oracle Name 규칙 (모두 대문자로, 공백은 '_' 로 대체)
        /// </summary>
        Oracle
    }
}