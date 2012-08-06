using System;
using System.Data.Common;
using System.Data.SQLite;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Configuration;

namespace NSoft.NFramework.Data.SQLite.EnterpriseLibrary {
    /// <summary>
    /// SQLite 용 Microsoft DAAB Database 클래스입니다.
    /// </summary>
    /// <remarks>Internally uses SQLite .NET Provider to connect to the database.</remarks>
    /// <example>
    /// <code>
    /// // 환경 설정 파일에 DAAB 관련해서 OracleDatabase를 추가하면 됩니다.
    /// <dataConfiguration defaultDatabase="Northwind">
    /// 	<providerMappings>
    /// 		<add databaseType="NSoft.NFramework.Data.SQLite.EnterpriseLibrary.SqLiteProvider, NSoft.NFramework.Data.SQLite" name="System.Data.SQLite" />
    /// 	</providerMappings>
    /// </dataConfiguration>
    /// </code>
    /// </example>
    [ConfigurationElementType(typeof(SQLiteDatabase))]
    [DatabaseAssembler(typeof(SQLiteDatabaseAssembler))]
    public class SQLiteDatabase : Database {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// Initializes a new instance of the SqLiteProvider class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public SQLiteDatabase(string connectionString) : base(connectionString, SQLiteFactory.Instance) {
            if(IsDebugEnabled)
                log.Debug("Microsoft EntLib DAAB 방식의 SqLiteProvider 인스턴스를 생성했습니다. connectionString=[{0}]", connectionString);
        }

        /// <summary>
        /// Retrieves parameter information from the stored procedure specified in the <see cref="T:System.Data.Common.DbCommand"/> 
        /// and populates the Parameters collection of the specified <see cref="T:System.Data.Common.DbCommand"/> object. 
        /// </summary>
        /// <param name="discoveryCommand">The <see cref="T:System.Data.Common.DbCommand"/> to do the discovery.</param>
        protected override void DeriveParameters(DbCommand discoveryCommand) {
            if(log.IsErrorEnabled)
                log.Error("SQLite는 Procedure를 지원하지 않습니다. Parameter 조회는 지원되지 않습니다!!!");

            throw new NotSupportedException("SQLite는 Procedure를 지원하지 않습니다!!!");
        }
    }
}