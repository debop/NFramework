namespace NSoft.NFramework {
    /// <summary>
    /// Class Member의 Naming 규칙 (camelCase, camelCaseUnderScore, pascalCase, pascalCaseUnderscore)
    /// </summary>
    public enum MemberNamingRule {
        /// <summary>
        /// CamelCase (productName)
        /// </summary>
        CamelCase,

        /// <summary>
        /// CamelCase with underscore (_productName)
        /// </summary>
        CamelCaseUndercore,

        /// <summary>
        /// CamelCase with m underscore (m_productName)
        /// </summary>
        CamelCase_M_Underscore,

        /// <summary>
        /// PascalCase (ProductName)
        /// </summary>
        PascalCase,

        /// <summary>
        /// PascalCase with underscore (_ProductName)
        /// </summary>
        PascalCaseUnderscore,

        /// <summary>
        /// PascalCase with m underscore (m_ProductName)
        /// </summary>
        PascalCase_M_Underscore
    }
}