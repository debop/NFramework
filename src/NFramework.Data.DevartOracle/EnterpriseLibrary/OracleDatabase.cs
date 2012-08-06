using System;
using System.Data;
using System.Data.Common;

namespace NSoft.NFramework.Data.DevartOracle.EnterpriseLibrary {
    /// <summary>
    /// Devart dotConnector for Oracle 을 사용하여, Microsoft Enterprise Library의 DAAB 의 Database를 구현했습니다.
    /// </summary>
    /// <example>
    /// <code>
    /// // 환경 설정 파일에 DAAB 관련해서 OracleDatabase를 추가하면 됩니다.
    /// <dataConfiguration defaultDatabase="LOCAL_XE">
    /// 	<providerMappings>
    /// 		<add name="Devart.Data.Oracle" databaseType="NSoft.NFramework.Data.DevartOracle.EnterpriseLibrary.OracleProvider, NSoft.NFramework.Data.DevartOracle"/>
    /// 	</providerMappings>
    /// </dataConfiguration>
    /// </code>
    /// </example>
    [Microsoft.Practices.EnterpriseLibrary.Common.Configuration.ConfigurationElementType(typeof(OracleDatabase))]
    [Microsoft.Practices.EnterpriseLibrary.Data.Configuration.DatabaseAssembler(typeof(OracleDatabaseAssembler))]
    public sealed class OracleDatabase : Microsoft.Practices.EnterpriseLibrary.Data.Database {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="connectionString">DB 연결 문자열</param>
        public OracleDatabase(string connectionString)
            : base(connectionString, Devart.Data.Oracle.OracleProviderFactory.Instance) {
            if(IsDebugEnabled)
                log.Debug("Microsoft EntLib DAAB 방식의 Devart 라이브러리를 사용하는 OracleDatbase 인스턴스를 생성했습니다. connectionString=[{0}]",
                          connectionString);
        }

        /// <summary>
        /// Parameter Token Character (':')
        /// </summary>
        public char ParameterToken {
            get { return ':'; }
        }

        /// <summary>
        /// 새로운 Oracle Connection 인스턴스를 생성합니다.
        /// </summary>
        /// <returns></returns>
        public new DbConnection CreateConnection() {
            if(IsDebugEnabled)
                log.Debug("새로운 OracleConnection을 생성합니다... ConnectionString=" + ConnectionString);

            return new Devart.Data.Oracle.OracleConnection(ConnectionString);
        }

        /// <summary>
        /// Procedure의 Parameter 정보를 로드합니다.
        /// </summary>
        /// <param name="discoveryCommand"></param>
        protected override void DeriveParameters(DbCommand discoveryCommand) {
            discoveryCommand.ShouldNotBeNull("discoveryCommand");
            Devart.Data.Oracle.OracleCommandBuilder.DeriveParameters((Devart.Data.Oracle.OracleCommand)discoveryCommand);
        }

        protected override void SetUpRowUpdatedEvent(DbDataAdapter adapter) {
            ((Devart.Data.Oracle.OracleDataAdapter)adapter).RowUpdated += OnOracleRowUpdated;
        }

        private static void OnOracleRowUpdated(object sender, Devart.Data.Oracle.OracleRowUpdatedEventArgs e) {
            if(e.RecordsAffected == 0)
                if(e.Errors != null)
                    e.Status = UpdateStatus.SkipCurrentRow;
        }

        public override void AddParameter(DbCommand command, string name, DbType dbType, int size, ParameterDirection direction,
                                          bool nullable, byte precision, byte scale, string sourceColumn, DataRowVersion sourceVersion,
                                          object value) {
            if(sourceVersion == DataRowVersion.Default)
                sourceVersion = DataRowVersion.Current;

            var parameter = CreateParameter(name, dbType, size, direction, nullable, precision, scale, sourceColumn, sourceVersion,
                                            value);
            command.Parameters.Add(parameter);
        }

        protected override void ConfigureParameter(DbParameter param, string name, DbType dbType, int size, ParameterDirection direction,
                                                   bool nullable, byte precision, byte scale, string sourceColumn,
                                                   DataRowVersion sourceVersion, object value) {
            value = value ?? DBNull.Value;
            param.DbType = dbType;

            ((Devart.Data.Oracle.OracleParameter)param).OracleDbType
                = (value != DBNull.Value)
                      ? OracleTool.TypeToOracleDbType(value.GetType())
                      : OracleTool.DbTypeToOracleDbType(dbType);

            param.Size = size;
            param.Value = value;
            param.Direction = direction;
            param.IsNullable = nullable;
            param.SourceColumn = sourceColumn;

            if(sourceVersion == DataRowVersion.Default)
                sourceVersion = DataRowVersion.Current;

            param.SourceVersion = sourceVersion;
        }

        public override DbCommand GetStoredProcCommand(string storedProcedureName) {
            storedProcedureName.ShouldNotBeWhiteSpace("storedProcedureName");

            if(IsDebugEnabled)
                log.Debug("Procedure 실행을 위한 OracleCommand를 생성합니다. storedProcedureName=" + storedProcedureName);

            var command = (Devart.Data.Oracle.OracleCommand)base.GetStoredProcCommand(storedProcedureName);
            command.ParameterCheck = true;

            return command;
        }
    }
}