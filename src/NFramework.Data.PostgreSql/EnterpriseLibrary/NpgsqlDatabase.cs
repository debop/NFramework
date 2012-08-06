using System;
using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Configuration;
using Npgsql;
using NpgsqlTypes;

namespace NSoft.NFramework.Data.PostgreSql.EnterpriseLibrary {
    /// <summary>
    /// Enterprise DAAB Database for PostgreSQL
    /// </summary>
    /// <example>
    /// <code>
    /// // 환경 설정 파일에 DAAB 관련해서 OracleDatabase를 추가하면 됩니다.
    /// <dataConfiguration defaultDatabase="LOCAL_XE">
    /// 	<providerMappings>
    /// 		<add databaseType="NSoft.NFramework.Data.PostgreSql.EnterpriseLibrary.NpgsqlDatabase, NSoft.NFramework.Data.PostgreSql" name="MySql.Data.PostgreSql" />
    /// 	</providerMappings>
    /// </dataConfiguration>
    /// </code>
    /// </example>
    [DatabaseAssembler(typeof(NpgsqlDatabaseAssembler))]
    [ConfigurationElementType(typeof(NpgsqlDatabase))]
    public class NpgsqlDatabase : Database {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// The parameter token used to delimit parameters for the PostgreSQL database. (:)
        /// </summary>
        protected const char ParameterToken = ':';

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="connectionString"></param>
        public NpgsqlDatabase(string connectionString)
            : base(connectionString, NpgsqlFactory.Instance) {
            if(IsDebugEnabled)
                log.Debug("Microsoft EntLib DAAB 방식의 NpgsqlDatabase 인스턴스를 생성했습니다. connectionString=[{0}]", connectionString);
        }

        /// <summary>
        /// Adds a new In <see cref="DbParameter"/> object to the given <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The command to add the in parameter.</param>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="dbType">One of the <see cref="NpgsqlDbType"/> values.</param>
        /// <remarks>
        /// This version of the method is used when you can have the same parameter object multiple times with different values.
        /// </remarks>
        public void AddInParameter(DbCommand command, string name, NpgsqlDbType dbType) {
            AddParameter(command, name, dbType, ParameterDirection.Input, String.Empty, DataRowVersion.Default, null);
        }

        /// <summary>
        /// Adds a new In <see cref="DbParameter"/> object to the given <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The commmand to add the parameter.</param>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="dbType">One of the <see cref="NpgsqlDbType"/> values.</param>
        /// <param name="value">The value of the parameter.</param>
        public void AddInParameter(DbCommand command, string name, NpgsqlDbType dbType, object value) {
            AddParameter(command, name, dbType, ParameterDirection.Input, String.Empty, DataRowVersion.Default, value);
        }

        /// <summary>
        /// Adds a new In <see cref="DbParameter"/> object to the given <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The command to add the parameter.</param>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="dbType">One of the <see cref="NpgsqlDbType"/> values.</param>
        /// <param name="sourceColumn">The name of the source column mapped to the DataSet and used for loading or returning the value.</param>
        /// <param name="sourceVersion">One of the <see cref="DataRowVersion"/> values.</param>
        public void AddInParameter(DbCommand command, string name, NpgsqlDbType dbType, string sourceColumn,
                                   DataRowVersion sourceVersion) {
            AddParameter(command, name, dbType, 0, ParameterDirection.Input, true, 0, 0, sourceColumn, sourceVersion, null);
        }

        /// <summary>
        /// Adds a new Out <see cref="DbParameter"/> object to the given <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The command to add the out parameter.</param>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="dbType">One of the <see cref="NpgsqlDbType"/> values.</param>
        /// <param name="size">The maximum size of the data within the column.</param>
        public void AddOutParameter(DbCommand command, string name, NpgsqlDbType dbType, int size) {
            AddParameter(command, name, dbType, size, ParameterDirection.Output, true, 0, 0, String.Empty, DataRowVersion.Default,
                         DBNull.Value);
        }

        /// <summary>
        /// Adds a new instance of a <see cref="DbParameter"/> object to the command.
        /// </summary>
        /// <param name="command">The command to add the parameter.</param>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="dbType">One of the <see cref="DbType"/> values.</param>
        /// <param name="size">The maximum size of the data within the column.</param>
        /// <param name="direction">One of the <see cref="ParameterDirection"/> values.</param>
        /// <param name="nullable">A value indicating whether the parameter accepts <see langword="null"/> (<b>Nothing</b> in Visual Basic) values.</param>
        /// <param name="precision">The maximum number of digits used to represent the <paramref name="value"/>.</param>
        /// <param name="scale">The number of decimal places to which <paramref name="value"/> is resolved.</param>
        /// <param name="sourceColumn">The name of the source column mapped to the DataSet and used for loading or returning the <paramref name="value"/>.</param>
        /// <param name="sourceVersion">One of the <see cref="DataRowVersion"/> values.</param>
        /// <param name="value">The value of the parameter.</param>
        public virtual void AddParameter(DbCommand command,
                                         string name,
                                         NpgsqlDbType dbType,
                                         int size,
                                         ParameterDirection direction,
                                         bool nullable,
                                         byte precision,
                                         byte scale,
                                         string sourceColumn,
                                         DataRowVersion sourceVersion,
                                         object value) {
            var parameter = CreateParameter(name, dbType, size, direction, nullable, precision, scale, sourceColumn, sourceVersion,
                                            value);
            command.Parameters.Add(parameter);
        }

        /// <summary>
        /// Adds a new instance of a <see cref="DbParameter"/> object to the command.
        /// </summary>
        /// <param name="command">The command to add the parameter.</param>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="dbType">One of the <see cref="NpgsqlDbType"/> values.</param>
        /// <param name="direction">One of the <see cref="ParameterDirection"/> values.</param>
        /// <param name="sourceColumn">The name of the source column mapped to the DataSet and used for loading or returning the <paramref name="value"/>.</param>
        /// <param name="sourceVersion">One of the <see cref="DataRowVersion"/> values.</param>
        /// <param name="value">The value of the parameter.</param>
        public void AddParameter(DbCommand command,
                                 string name,
                                 NpgsqlDbType dbType,
                                 ParameterDirection direction,
                                 string sourceColumn,
                                 DataRowVersion sourceVersion,
                                 object value) {
            AddParameter(command, name, dbType, 0, direction, false, 0, 0, sourceColumn, sourceVersion, value);
        }

        /// <summary>
        /// Builds a value parameter name for the current database.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <returns>A correctly formated parameter name.</returns>
        public override string BuildParameterName(string name) {
            name.ShouldNotBeWhiteSpace("name");

            return (name[0] != ParameterToken) ? name.Insert(0, new string(ParameterToken, 1)) : name;
        }

        /// <summary>
        /// Configures a given <see cref="NpgsqlParameter"/>.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="dbType">One of the <see cref="NpgsqlDbType"/> values.</param>
        /// <param name="size">The maximum size of the data within the column.</param>
        /// <param name="direction">One of the <see cref="ParameterDirection"/> values.</param>
        /// <param name="nullable">A value indicating whether the parameter accepts <see langword="null"/> (<b>Nothing</b> in Visual Basic) values.</param>
        /// <param name="precision">The maximum number of digits used to represent the <paramref name="value"/>.</param>
        /// <param name="scale">The number of decimal places to which <paramref name="value"/> is resolved.</param>
        /// <param name="sourceColumn">The name of the source column mapped to the DataSet and used for loading or returning the <paramref name="value"/>.</param>
        /// <param name="sourceVersion">One of the <see cref="DataRowVersion"/> values.</param>
        /// <param name="value">The value of the parameter.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2116:AptcaMethodsShouldOnlyCallAptcaMethods")]
        protected virtual void ConfigureParameter(NpgsqlParameter parameter,
                                                  string name,
                                                  NpgsqlDbType dbType,
                                                  int size,
                                                  ParameterDirection direction,
                                                  bool nullable,
                                                  byte precision,
                                                  byte scale,
                                                  string sourceColumn,
                                                  DataRowVersion sourceVersion,
                                                  object value) {
            parameter.NpgsqlDbType = dbType;
            parameter.Size = size;
            parameter.Value = value ?? DBNull.Value;
            parameter.Direction = direction;
            parameter.IsNullable = nullable;
            parameter.SourceColumn = sourceColumn;
            parameter.SourceVersion = sourceVersion;
        }

        /// <summary>
        /// Configures a given <see cref="DbParameter"/>.
        /// </summary>
        /// <param name="param">The <see cref="DbParameter"/> to configure.</param>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="dbType">One of the <see cref="DbType"/> values.</param>
        /// <param name="size">The maximum size of the data within the column.</param>
        /// <param name="direction">One of the <see cref="ParameterDirection"/> values.</param>
        /// <param name="nullable">A value indicating whether the parameter accepts <see langword="null"/> (<b>Nothing</b> in Visual Basic) values.</param>
        /// <param name="precision">The maximum number of digits used to represent the <paramref name="value"/>.</param>
        /// <param name="scale">The number of decimal places to which <paramref name="value"/> is resolved.</param>
        /// <param name="sourceColumn">The name of the source column mapped to the DataSet and used for loading or returning the <paramref name="value"/>.</param>
        /// <param name="sourceVersion">One of the <see cref="DataRowVersion"/> values.</param>
        /// <param name="value">The value of the parameter.</param>
        protected override void ConfigureParameter(DbParameter param,
                                                   string name,
                                                   DbType dbType,
                                                   int size,
                                                   ParameterDirection direction,
                                                   bool nullable,
                                                   byte precision,
                                                   byte scale,
                                                   string sourceColumn,
                                                   DataRowVersion sourceVersion,
                                                   object value) {
            switch(dbType) {
                case DbType.DateTime:
                    ConfigureParameter((NpgsqlParameter)param, name, NpgsqlDbType.Timestamp, size, direction, nullable, precision, scale,
                                       sourceColumn, sourceVersion, value);
                    break;
                case DbType.Guid:
                    base.ConfigureParameter(param, name, DbType.String, size, direction, nullable, precision, scale, sourceColumn,
                                            sourceVersion, value);
                    break;
                default:
                    base.ConfigureParameter(param, name, dbType, size, direction, nullable, precision, scale, sourceColumn,
                                            sourceVersion, value);
                    break;
            }
        }

        /// <summary>
        /// Adds a new instance of a <see cref="DbParameter"/> object.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="dbType">One of the <see cref="NpgsqlDbType"/> values.</param>
        /// <param name="size">The maximum size of the data within the column.</param>
        /// <param name="direction">One of the <see cref="ParameterDirection"/> values.</param>
        /// <param name="nullable">A value indicating whether the parameter accepts <see langword="null"/> (<b>Nothing</b> in Visual Basic) values.</param>
        /// <param name="precision">The maximum number of digits used to represent the <paramref name="value"/>.</param>
        /// <param name="scale">The number of decimal places to which <paramref name="value"/> is resolved.</param>
        /// <param name="sourceColumn">The name of the source column mapped to the DataSet and used for loading or returning the <paramref name="value"/>.</param>
        /// <param name="sourceVersion">One of the <see cref="DataRowVersion"/> values.</param>
        /// <param name="value">The value of the parameter.</param>
        /// <returns></returns>
        protected DbParameter CreateParameter(string name,
                                              NpgsqlDbType dbType,
                                              int size,
                                              ParameterDirection direction,
                                              bool nullable,
                                              byte precision,
                                              byte scale,
                                              string sourceColumn,
                                              DataRowVersion sourceVersion,
                                              object value) {
            var param = CreateParameter(name) as NpgsqlParameter;
            ConfigureParameter(param, name, dbType, size, direction, nullable, precision, scale, sourceColumn, sourceVersion, value);
            return param;
        }

        /// <summary>
        /// Retrieves parameter information from the stored procedure specified in the <see cref="DbCommand"/> and populates the Parameters collection of the specified <see cref="DbCommand"/> object.
        /// </summary>
        /// <param name="discoveryCommand">The <see cref="DbCommand"/> to do the discovery.</param>
        /// <remarks>The <see cref="DbCommand"/> must be a <see cref="NpgsqlCommand"/> instance.</remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2116:AptcaMethodsShouldOnlyCallAptcaMethods")]
        protected override void DeriveParameters(DbCommand discoveryCommand) {
            NpgsqlCommandBuilder.DeriveParameters((NpgsqlCommand)discoveryCommand);
        }

        /// <summary>
        /// Executes the <paramref name="storedProcedureName"/> with <paramref name="parameterValues"/> and returns the results in a new <see cref="T:System.Data.DataSet"/>.
        /// </summary>
        /// <param name="storedProcedureName">The stored procedure to execute.</param>
        /// <param name="parameterValues">An array of paramters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</param>
        /// <returns>
        /// A <see cref="T:System.Data.DataSet"/> with the results of the <paramref name="storedProcedureName"/>.
        /// </returns>
        public override DataSet ExecuteDataSet(string storedProcedureName, params object[] parameterValues) {
            return parameterValues.Length == 0
                       ? base.ExecuteDataSet(CommandType.StoredProcedure, storedProcedureName)
                       : base.ExecuteDataSet(storedProcedureName, parameterValues);
        }

        /// <summary>
        /// Executes the <paramref name="storedProcedureName"/> with <paramref name="parameterValues"/> as part of the <paramref name="transaction"/> and returns the results in a new <see cref="T:System.Data.DataSet"/> within a transaction.
        /// </summary>
        /// <param name="transaction">The <see cref="T:System.Data.IDbTransaction"/> to execute the command within.</param>
        /// <param name="storedProcedureName">The stored procedure to execute.</param>
        /// <param name="parameterValues">An array of paramters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</param>
        /// <returns>
        /// A <see cref="T:System.Data.DataSet"/> with the results of the <paramref name="storedProcedureName"/>.
        /// </returns>
        public override DataSet ExecuteDataSet(DbTransaction transaction, string storedProcedureName, params object[] parameterValues) {
            return parameterValues.Length == 0
                       ? base.ExecuteDataSet(transaction, CommandType.StoredProcedure, storedProcedureName)
                       : base.ExecuteDataSet(transaction, storedProcedureName, parameterValues);
        }

        /// <summary>
        /// Executes the <paramref name="storedProcedureName"/> using the given <paramref name="parameterValues"/> and returns the number of rows affected.
        /// </summary>
        /// <param name="storedProcedureName">The name of the stored procedure to execute.</param>
        /// <param name="parameterValues">An array of paramters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</param>
        /// <returns>The number of rows affected</returns>
        /// <seealso cref="M:System.Data.IDbCommand.ExecuteScalar"/>
        public override int ExecuteNonQuery(string storedProcedureName, params object[] parameterValues) {
            return parameterValues.Length == 0
                       ? base.ExecuteNonQuery(CommandType.StoredProcedure, storedProcedureName)
                       : base.ExecuteNonQuery(storedProcedureName, parameterValues);
        }

        /// <summary>
        /// Executes the <paramref name="storedProcedureName"/> using the given <paramref name="parameterValues"/> within a transaction and returns the number of rows affected.
        /// </summary>
        /// <param name="transaction">The <see cref="T:System.Data.IDbTransaction"/> to execute the command within.</param>
        /// <param name="storedProcedureName">The name of the stored procedure to execute.</param>
        /// <param name="parameterValues">An array of parameters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</param>
        /// <returns>The number of rows affected.</returns>
        /// <seealso cref="M:System.Data.IDbCommand.ExecuteScalar"/>
        public override int ExecuteNonQuery(DbTransaction transaction, string storedProcedureName, params object[] parameterValues) {
            return parameterValues.Length == 0
                       ? base.ExecuteNonQuery(transaction, CommandType.StoredProcedure, storedProcedureName)
                       : base.ExecuteNonQuery(transaction, storedProcedureName, parameterValues);
        }

        /// <summary>
        /// Executes the <paramref name="storedProcedureName"/> with the given <paramref name="parameterValues"/> and returns an <see cref="T:System.Data.IDataReader"></see> through which the result can be read.
        /// It is the responsibility of the caller to close the connection and reader when finished.
        /// </summary>
        /// <param name="storedProcedureName">The command that contains the query to execute.</param>
        /// <param name="parameterValues">An array of parameters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</param>
        /// <returns>
        /// An <see cref="T:System.Data.IDataReader"/> object.
        /// </returns>
        public new IDataReader ExecuteReader(string storedProcedureName, params object[] parameterValues) {
            return parameterValues.Length == 0
                       ? base.ExecuteReader(CommandType.StoredProcedure, storedProcedureName)
                       : base.ExecuteReader(storedProcedureName, parameterValues);
        }

        /// <summary>
        /// Executes the <paramref name="storedProcedureName"/> with the given <paramref name="parameterValues"/> within the given <paramref name="transaction"/> and returns an <see cref="T:System.Data.IDataReader"></see> through which the result can be read.
        /// It is the responsibility of the caller to close the connection and reader when finished.
        /// </summary>
        /// <param name="transaction">The <see cref="T:System.Data.IDbTransaction"/> to execute the command within.</param>
        /// <param name="storedProcedureName">The command that contains the query to execute.</param>
        /// <param name="parameterValues">An array of parameters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</param>
        /// <returns>
        /// An <see cref="T:System.Data.IDataReader"/> object.
        /// </returns>
        public new IDataReader ExecuteReader(DbTransaction transaction, string storedProcedureName, params object[] parameterValues) {
            return parameterValues.Length == 0
                       ? base.ExecuteReader(transaction, CommandType.StoredProcedure, storedProcedureName)
                       : base.ExecuteReader(transaction, storedProcedureName, parameterValues);
        }

        /// <summary>
        /// Executes the <paramref name="storedProcedureName"/> with the given <paramref name="parameterValues"/> and returns the first column of the first row in the result set returned by the query. Extra columns or rows are ignored.
        /// </summary>
        /// <param name="storedProcedureName">The stored procedure to execute.</param>
        /// <param name="parameterValues">An array of paramters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</param>
        /// <returns>
        /// The first column of the first row in the result set.
        /// </returns>
        /// <seealso cref="M:System.Data.IDbCommand.ExecuteScalar"/>
        public override object ExecuteScalar(string storedProcedureName, params object[] parameterValues) {
            return parameterValues.Length == 0
                       ? base.ExecuteScalar(CommandType.StoredProcedure, storedProcedureName)
                       : base.ExecuteScalar(storedProcedureName, parameterValues);
        }

        /// <summary>
        /// Executes the <paramref name="storedProcedureName"/> with the given <paramref name="parameterValues"/> within a
        /// <paramref name="transaction"/> and returns the first column of the first row in the result set returned by the query. Extra columns or rows are ignored.
        /// </summary>
        /// <param name="transaction">The <see cref="T:System.Data.IDbTransaction"/> to execute the command within.</param>
        /// <param name="storedProcedureName">The stored procedure to execute.</param>
        /// <param name="parameterValues">An array of paramters to pass to the stored procedure. The parameter values must be in call order as they appear in the stored procedure.</param>
        /// <returns>
        /// The first column of the first row in the result set.
        /// </returns>
        /// <seealso cref="M:System.Data.IDbCommand.ExecuteScalar"/>
        public override object ExecuteScalar(DbTransaction transaction, string storedProcedureName, params object[] parameterValues) {
            return parameterValues.Length == 0
                       ? base.ExecuteScalar(transaction, CommandType.StoredProcedure, storedProcedureName)
                       : base.ExecuteScalar(transaction, storedProcedureName, parameterValues);
        }

        /// <summary>
        /// Determines if the number of parameters in the command matches the array of parameter values.
        /// </summary>
        /// <param name="command">The <see cref="DbCommand"/> containing the parameters.</param>
        /// <param name="values">The array of parameter values.</param>
        /// <returns>
        /// 	<see langword="true"/> if the number of parameters and values match; otherwise, <see langword="false"/>.
        /// </returns>
        protected override bool SameNumberOfParametersAndValues(DbCommand command, object[] values) {
            const int returnParameterCount = 0;

            var numberOfParametersToStoredProcedure = command.Parameters.Count - returnParameterCount;
            var numberOfValuesProvidedForStoredProcedure = values.Length;
            return numberOfParametersToStoredProcedure == numberOfValuesProvidedForStoredProcedure;
        }

        /// <summary>
        /// Sets the RowUpdated event for the data adapter.
        /// </summary>
        /// <param name="adapter">The <see cref="DbDataAdapter"/> to set the event.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2116:AptcaMethodsShouldOnlyCallAptcaMethods")]
        protected override void SetUpRowUpdatedEvent(DbDataAdapter adapter) {
            ((NpgsqlDataAdapter)adapter).RowUpdated += new NpgsqlRowUpdatedEventHandler(OnNpgsqlRowUpdated);
        }

        /// <summary>
        /// Returns the starting index for parameters in a command.
        /// </summary>
        /// <returns>
        /// The starting index for parameters in a command.
        /// </returns>
        protected override int UserParametersStartIndex() {
            return 0;
        }

        /// <summary>
        /// Checks if a database command is a Npgsql command and converts.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>converted NpgsqlCommand</returns>
        public static NpgsqlCommand CheckIfNpgsqlCommand(DbCommand command) {
            command.AssertIsNpgsqlCommand();
            return (NpgsqlCommand)command;
        }

        /// <summary>
        /// Called when [NPGSQL row updated].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="rowThatCouldNotBeWritten">The <see cref="NpgsqlRowUpdatedEventArgs"/> instance containing the event data.</param>
        /// <devdoc>
        /// Listens for the RowUpdate event on a dataadapter to support
        /// UpdateBehavior.Continue
        /// </devdoc>
        private void OnNpgsqlRowUpdated(object sender, NpgsqlRowUpdatedEventArgs rowThatCouldNotBeWritten) {
            if(rowThatCouldNotBeWritten.RecordsAffected == 0) {
                if(rowThatCouldNotBeWritten.Errors != null) {
                    rowThatCouldNotBeWritten.Row.RowError = "DataSet Row 갱신에 실패했습니다.";
                    rowThatCouldNotBeWritten.Status = UpdateStatus.SkipCurrentRow;
                }
            }
        }
    }
}