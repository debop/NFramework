namespace NSoft.NFramework.Data.NHibernateEx {
    /// <summary>
    /// Database 종류
    /// </summary>
    public enum DatabaseEngine {
        /// <summary>
        /// SQLite for Memory DB
        /// </summary>
        SQLite,

        /// <summary>
        /// SQLite for File
        /// </summary>
        SQLiteForFile,

        /// <summary>
        /// Microsoft Sql CE
        /// </summary>
        MsSqlCe,

        /// <summary>
        /// Microsoft Sql Ce 4.0
        /// </summary>
        MsSqlCe40,

        /// <summary>
        /// Microsoft SQL Server 2005 or Higher
        /// </summary>
        MsSql2005,

        /// <summary>
        /// Microsoft SQL Server 2005 Express or Higher
        /// </summary>
        MsSql2005Express,

        /// <summary>
        /// ODP.NET 을 통한 Oracle ( Oracle 10g 를 기본으로 한다)
        /// </summary>
        OdpNet,

        /// <summary>
        /// Devart dotConnector for Oracle 을 이용한 Driver입니다.
        /// </summary>
        DevartOracle,

        /// <summary>
        /// MySQL 
        /// </summary>
        MySql,

        /// <summary>
        /// PostgreSql
        /// </summary>
        PostgreSql,

        /// <summary>
        /// Firebird
        /// </summary>
        Firebird,

        /// <summary>
        /// IBM DB2
        /// </summary>
        DB2,

        /// <summary>
        /// Cubrid DB
        /// </summary>
        Cubrid
    }
}