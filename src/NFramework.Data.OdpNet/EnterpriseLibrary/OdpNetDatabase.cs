using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Transactions;
using System.Xml;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Configuration;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;

namespace NSoft.NFramework.Data.OdpNet.EnterpriseLibrary {
    /// <summary>
    /// Microsoft Enterprise Libary DAAB의 <see cref="Database"/>를 상속받아 Oracle을 위한 Database 를 표현합니다. <br/>
    /// Oracle.DataAccess.dll (ODP.NET)을 사용합니다.
    /// </summary>
    [DatabaseAssembler(typeof(OdpNetDatabaseAssembler))]
    public class OdpNetDatabase : Database {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// The default ref cursor name used for all stored procedures that use ref cursors.
        /// </summary>
        private const string RefCursorName = "CUR_OUT";

        /// <summary>
        /// Packages with this prefix will apply the package name to all stored procedure names.
        /// </summary>
        private const string AllPrefix = "*";

        private static readonly IOraclePackage[] EmptyPackages = new IOraclePackage[0];
        private readonly IOraclePackage[] _packages;

        private readonly IDictionary<string, ParameterTypeRegistry> _registeredParameterTypes =
            new Dictionary<string, ParameterTypeRegistry>();

        public OdpNetDatabase(string connectionString) : this(connectionString, EmptyPackages) {}

        public OdpNetDatabase(string connectionString, IOraclePackage[] packages)
            : base(connectionString ?? AdoTool.DefaultConnectionStringSettings.ConnectionString, OracleClientFactory.Instance) {
            connectionString = connectionString ?? AdoTool.DefaultConnectionStringSettings.ConnectionString;

            if(log.IsInfoEnabled)
                log.Info("Microsoft EntLib DAAB 방식의 odpNetDatabase 인스턴스를 생성했습니다. connectionString=[{0}]", connectionString);

            packages.ShouldNotBeNull("packages");
            _packages = packages;
        }

        #region Public Methods

        /// <summary>
        /// Adds a new In <see cref="DbParameter"/> object to the given
        /// <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The command to add the in parameter.</param>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="dbType">One of the <see cref="OracleDbType"/>
        /// values.</param>
        /// <remarks>
        /// This version of the method is used when you can have the same
        /// parameter object multiple times with different values.
        /// </remarks>
        [CLSCompliant(false)]
        public void AddInParameter(DbCommand command, string name, OracleDbType dbType) {
            AddParameter(command, name, dbType, ParameterDirection.Input, String.Empty, DataRowVersion.Default, null);
        }

        /// <summary>
        /// Adds a new In <see cref="DbParameter"/> object to the given
        /// <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The commmand to add the parameter.</param>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="dbType">One of the <see cref="OracleDbType"/>
        /// values.</param>
        /// <param name="value">The value of the parameter.</param>
        [CLSCompliant(false)]
        public void AddInParameter(DbCommand command, string name, OracleDbType dbType, object value) {
            AddParameter(command, name, dbType, ParameterDirection.Input, String.Empty, DataRowVersion.Default, value);
        }

        /// <summary>
        /// Adds a new In <see cref="DbParameter"/> object to the given
        /// <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The command to add the parameter.</param>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="dbType">One of the <see cref="OracleDbType"/>
        /// values.</param>
        /// <param name="sourceColumn">The name of the source column
        /// mapped to the DataSet and used for loading or returning the value.</param>
        /// <param name="sourceVersion">One of the
        /// <see cref="DataRowVersion"/> values.</param>
        [CLSCompliant(false)]
        public void AddInParameter(DbCommand command, string name, OracleDbType dbType, string sourceColumn,
                                   DataRowVersion sourceVersion) {
            AddParameter(command, name, dbType, 0, ParameterDirection.Input, true, 0, 0, sourceColumn, sourceVersion, null);
        }

        /// <summary>
        /// Adds a new Out <see cref="DbParameter"/> object to the given
        /// <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The command to add the out parameter.</param>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="dbType">One of the <see cref="OracleDbType"/>
        /// values.</param>
        /// <param name="size">The maximum size of the data within the
        /// column.</param>
        [CLSCompliant(false)]
        public void AddOutParameter(DbCommand command, string name, OracleDbType dbType, int size) {
            AddParameter(command, name, dbType, size, ParameterDirection.Output, true, 0, 0, String.Empty, DataRowVersion.Default,
                         DBNull.Value);
        }

        /// <summary>
        /// Adds a new instance of a <see cref="DbParameter"/> object to
        /// the command.
        /// </summary>
        /// <param name="command">The command to add the parameter.</param>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="dbType">One of the <see cref="DbType"/> values.</param>
        /// <param name="size">The maximum size of the data within the
        /// column.</param>
        /// <param name="direction">One of the
        /// <see cref="ParameterDirection"/> values.</param>
        /// <param name="nullable">A value indicating whether the
        /// parameter accepts <see langword="null"/> (<b>Nothing</b> in Visual
        /// Basic) values.</param>
        /// <param name="precision">The maximum number of digits used to
        /// represent the <paramref name="value"/>.</param>
        /// <param name="scale">The number of decimal places to which
        /// <paramref name="value"/> is resolved.</param>
        /// <param name="sourceColumn">The name of the source column
        /// mapped to the DataSet and used for loading or returning the
        /// <paramref name="value"/>.</param>
        /// <param name="sourceVersion">One of the
        /// <see cref="DataRowVersion"/> values.</param>
        /// <param name="value">The value of the parameter.</param>
        [CLSCompliant(false)]
        public virtual void AddParameter(DbCommand command,
                                         string name,
                                         OracleDbType dbType,
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
        /// Adds a new instance of a <see cref="DbParameter"/> object to
        /// the command.
        /// </summary>
        /// <param name="command">The command to add the parameter.</param>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="dbType">One of the <see cref="OracleDbType"/>
        /// values.</param>
        /// <param name="direction">One of the
        /// <see cref="ParameterDirection"/> values.</param>
        /// <param name="sourceColumn">The name of the source column
        /// mapped to the DataSet and used for loading or returning the
        /// <paramref name="value"/>.</param>
        /// <param name="sourceVersion">One of the
        /// <see cref="DataRowVersion"/> values.</param>
        /// <param name="value">The value of the parameter.</param>
        [CLSCompliant(false)]
        public void AddParameter(DbCommand command,
                                 string name,
                                 OracleDbType dbType,
                                 ParameterDirection direction,
                                 string sourceColumn,
                                 DataRowVersion sourceVersion,
                                 object value) {
            AddParameter(command, name, dbType, 0, direction, false, 0, 0, sourceColumn, sourceVersion, value);
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
        public override void AddParameter(DbCommand command,
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
            // Guid types are not directly supported in Oracle so they are translated to OracleDbType.Raw which is internally stored as an OracleBinary instance
            if(DbType.Guid.Equals(dbType)) {
                object convertedValue = ConvertGuidToByteArray(value);

                AddParameter(command, name, OracleDbType.Raw, 16, direction, nullable, precision, scale, sourceColumn, sourceVersion,
                             convertedValue);

                RegisterParameterType(command, name, dbType);
            }
                // Boolean types are not directly supported in Oracle so they are translated to OracleDbType.Int16 which is internally stored as an OracleDecimal instance
            else if(DbType.Boolean.Equals(dbType)) {
                object convertedValue = ConvertBoolToShort(value);

                AddParameter(command, name, OracleDbType.Int16, 4, direction, nullable, precision, scale, sourceColumn, sourceVersion,
                             convertedValue);

                RegisterParameterType(command, name, dbType);
            }
            else {
                base.AddParameter(command, name, dbType, size, direction, nullable, precision, scale, sourceColumn, sourceVersion, value);
            }
        }

        /// <summary>
        /// Configures a given <see cref="DbParameter"/>.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="dbType">One of the <see cref="OracleDbType"/>
        /// values.</param>
        /// <param name="size">The maximum size of the data within the
        /// column.</param>
        /// <param name="direction">One of the
        /// <see cref="ParameterDirection"/> values.</param>
        /// <param name="nullable">A value indicating whether the
        /// parameter accepts <see langword="null"/> (<b>Nothing</b> in Visual
        /// Basic) values.</param>
        /// <param name="precision">The maximum number of digits used to
        /// represent the <paramref name="value"/>.</param>
        /// <param name="scale">The number of decimal places to which
        /// <paramref name="value"/> is resolved.</param>
        /// <param name="sourceColumn">The name of the source column
        /// mapped to the DataSet and used for loading or returning the
        /// <paramref name="value"/>.</param>
        /// <param name="sourceVersion">One of the
        /// <see cref="DataRowVersion"/> values.</param>
        /// <param name="value">The value of the parameter.</param>
        [CLSCompliant(false)]
        protected virtual void ConfigureParameter(OracleParameter parameter,
                                                  string name,
                                                  OracleDbType dbType,
                                                  int size,
                                                  ParameterDirection direction,
                                                  bool nullable,
                                                  byte precision,
                                                  byte scale,
                                                  string sourceColumn,
                                                  DataRowVersion sourceVersion,
                                                  object value) {
            parameter.OracleDbType = dbType;
            parameter.Size = size;
            parameter.Value = value ?? DBNull.Value;
            parameter.Direction = direction;
            parameter.IsNullable = nullable;
            parameter.Precision = precision;
            parameter.Scale = scale;
            parameter.SourceColumn = sourceColumn;
            parameter.SourceVersion = sourceVersion;
        }

        /// <summary>
        /// Adds a new instance of a <see cref="DbParameter"/> object.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <param name="dbType">One of the <see cref="DbType"/> values.</param>
        /// <param name="size">The maximum size of the data within the
        /// column.</param>
        /// <param name="direction">One of the
        /// <see cref="ParameterDirection"/> values.</param>
        /// <param name="nullable">A value indicating whether the
        /// parameter accepts <see langword="null"/> (<b>Nothing</b> in Visual
        /// Basic) values.</param>
        /// <param name="precision">The maximum number of digits used to
        /// represent the <paramref name="value"/>.</param>
        /// <param name="scale">The number of decimal places to which
        /// <paramref name="value"/> is resolved.</param>
        /// <param name="sourceColumn">The name of the source column
        /// mapped to the DataSet and used for loading or returning the
        /// <paramref name="value"/>.</param>
        /// <param name="sourceVersion">One of the
        /// <see cref="DataRowVersion"/> values.</param>
        /// <param name="value">The value of the parameter.</param>
        /// <returns></returns>
        [CLSCompliant(false)]
        protected DbParameter CreateParameter(string name,
                                              OracleDbType dbType,
                                              int size,
                                              ParameterDirection direction,
                                              bool nullable,
                                              byte precision,
                                              byte scale,
                                              string sourceColumn,
                                              DataRowVersion sourceVersion,
                                              object value) {
            OracleParameter param = CreateParameter(name) as OracleParameter;
            ConfigureParameter(param, name, dbType, size, direction,
                               nullable, precision, scale, sourceColumn,
                               sourceVersion, value);
            return param;
        }

        /// <summary>
        /// Retrieves parameter information from the stored procedure specified in the <see cref="DbCommand"/> and populates the Parameters collection of the specified <see cref="DbCommand"/> object.
        /// </summary>
        /// <param name="discoveryCommand">The <see cref="DbCommand"/> to do the discovery.</param>
        /// <remarks>
        /// The <see cref="DbCommand"/> must be an instance of a <see cref="OracleCommand"/> object.
        /// </remarks>
        protected override void DeriveParameters(DbCommand discoveryCommand) {
            OracleCommandBuilder.DeriveParameters((OracleCommand)discoveryCommand);
            var curOut = discoveryCommand.Parameters[RefCursorName] as OracleParameter;

            if(curOut != null) {
                curOut.OracleDbType = OracleDbType.RefCursor;
            }
        }

        /// <summary>
        /// Creates an <see cref="OracleDataReader"/> based on the <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The command wrapper to execute.</param>
        /// <returns>
        /// An <see cref="OracleDataReader"/> object.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// 	<paramref name="command"/> can not be <see langword="null"/> (Nothing in Visual Basic).
        /// </exception>
        public override IDataReader ExecuteReader(DbCommand command) {
            PrepareCWRefCursor(command);
            return ((OracleDataReader)base.ExecuteReader(command));
        }

        /// <summary>
        /// Creates an <see cref="OracleDataReader"/> based on the <paramref name="command"/>.
        /// </summary>
        /// <param name="command">The command wrapper to execute.</param>
        /// <param name="transaction">The transaction to participate in when executing this reader.</param>
        /// <returns>
        /// An <see cref="OracleDataReader"/> object.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// 	<para><paramref name="command"/> can not be <see langword="null"/> (Nothing in Visual Basic).</para>
        /// 	<para>- or -</para>
        /// 	<para><paramref name="transaction"/> can not be <see langword="null"/> (Nothing in Visual Basic).</para>
        /// </exception>
        public override IDataReader ExecuteReader(DbCommand command, DbTransaction transaction) {
            PrepareCWRefCursor(command);
            return ((OracleDataReader)base.ExecuteReader(command, transaction));
        }

        /// <summary>
        /// Executes the <see cref="OracleCommand"/> and returns a new <see cref="XmlReader"/>.
        /// </summary>
        /// <param name="command">The <see cref="OracleCommand"/> to execute.</param>
        /// <returns>An <see cref="XmlReader"/> object.</returns>
        /// <remarks>
        /// Unlike other Execute... methods that take a <see cref="DbCommand"/> instance, this method
        /// does not set the command behavior to close the connection when you close the reader.
        /// That means you'll need to close the connection yourself, by calling the
        /// command.Connection.Close() method.
        /// <para>
        /// There is one exception to the rule above. If you're using <see cref="TransactionScope"/> to provide
        /// implicit transactions, you should NOT close the connection on this reader when you're
        /// done. Only close the connection if <see cref="Transaction"/>.Current is null.
        /// </para>
        /// </remarks>
        public XmlReader ExecuteXmlReader(DbCommand command) {
            var oracleCommand = CheckIfOracleCommand(command);

            var wrapper = GetOpenConnection(false);
            PrepareCommand(oracleCommand, wrapper.Connection);
            return DoExecuteXmlReader(oracleCommand);
        }

        /// <summary>
        /// Executes the <see cref="OracleCommand"/> in a transaction and returns a new <see cref="XmlReader"/>.
        /// </summary>
        /// <param name="command">The <see cref="OracleCommand"/> to execute.</param>
        /// <param name="transaction">The <see cref="IDbTransaction"/> to execute the command within.</param>
        /// <returns>An <see cref="XmlReader"/> object.</returns>
        /// <remarks>
        /// Unlike other Execute... methods that take a <see cref="DbCommand"/> instance, this method
        /// does not set the command behavior to close the connection when you close the reader.
        /// That means you'll need to close the connection yourself, by calling the
        /// command.Connection.Close() method.
        /// </remarks>
        public XmlReader ExecuteXmlReader(DbCommand command, DbTransaction transaction) {
            var oracleCommand = CheckIfOracleCommand(command);

            PrepareCommand(oracleCommand, transaction);
            return DoExecuteXmlReader(oracleCommand);
        }

        /// <summary>
        /// Executes a command and returns the results in a new <see cref="DataSet"/>.
        /// </summary>
        /// <param name="command">The command to execute to fill the <see cref="DataSet"/></param>
        /// <returns>
        /// A <see cref="DataSet"/> filed with records and, if necessary, schema.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// 	<paramref name="command"/> can not be <see langword="null"/> (Nothing in Visual Basic).
        /// </exception>
        public override DataSet ExecuteDataSet(DbCommand command) {
            PrepareCWRefCursor(command);
            return base.ExecuteDataSet(command);
        }

        /// <summary>
        /// Executes a command and returns the result in a new <see cref="DataSet"/>.
        /// </summary>
        /// <param name="command">The command to execute to fill the <see cref="DataSet"/></param>
        /// <param name="transaction">The transaction to participate in when executing this reader.</param>
        /// <returns>
        /// A <see cref="DataSet"/> filed with records and, if necessary, schema.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// 	<paramref name="command"/> can not be <see langword="null"/> (<b>Nothing</b> in Visual Basic).
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// 	<para><paramref name="command"/> can not be <see langword="null"/> (<b>Nothing</b> in Visual Basic).</para>
        /// 	<para>- or -</para>
        /// 	<para><paramref name="transaction"/> can not be <see langword="null"/> (<b>Nothing</b> in Visual Basic).</para>
        /// </exception>
        public override DataSet ExecuteDataSet(DbCommand command, DbTransaction transaction) {
            PrepareCWRefCursor(command);
            return base.ExecuteDataSet(command, transaction);
        }

        /// <summary>
        /// Loads a <see cref="DataSet"/> from a <see cref="DbCommand"/>.
        /// </summary>
        /// <param name="command">The command to execute to fill the <see cref="DataSet"/>.</param>
        /// <param name="dataSet">The <see cref="DataSet"/> to fill.</param>
        /// <param name="tableNames">An array of table name mappings for the <see cref="DataSet"/>.</param>
        public override void LoadDataSet(DbCommand command, DataSet dataSet, string[] tableNames) {
            PrepareCWRefCursor(command);
            base.LoadDataSet(command, dataSet, tableNames);
        }

        /// <summary>
        /// Loads a <see cref="DataSet"/> from a <see cref="DbCommand"/> in a transaction.
        /// </summary>
        /// <param name="command">The command to execute to fill the <see cref="DataSet"/>.</param>
        /// <param name="dataSet">The <see cref="DataSet"/> to fill.</param>
        /// <param name="tableNames">An array of table name mappings for the <see cref="DataSet"/>.</param>
        /// <param name="transaction">The <see cref="IDbTransaction"/> to execute the command in.</param>
        public override void LoadDataSet(DbCommand command, DataSet dataSet, string[] tableNames, DbTransaction transaction) {
            PrepareCWRefCursor(command);
            base.LoadDataSet(command, dataSet, tableNames, transaction);
        }

        /// <summary>
        /// Gets a parameter value.
        /// </summary>
        /// <param name="command">The command that contains the parameter.</param>
        /// <param name="name">The name of the parameter.</param>
        /// <returns>The value of the parameter.</returns>
        public override object GetParameterValue(DbCommand command, string name) {
            var convertedValue = base.GetParameterValue(command, name);

            var registry = GetParameterTypeRegistry(command.CommandText);

            if(registry != null) {
                if(registry.HasRegisteredParameterType(name)) {
                    var dbType = registry.GetRegisteredParameterType(name);

                    if(DbType.Guid == dbType) {
                        convertedValue = ConvertByteArrayToGuid(convertedValue);
                    }
                    else if(DbType.Boolean == dbType) {
                        convertedValue = ConvertShortToBool(convertedValue);
                    }
                }
            }

            return convertedValue;
        }

        /// <summary>
        /// Sets a parameter value.
        /// </summary>
        /// <param name="command">The command with the parameter.</param>
        /// <param name="parameterName">The parameter name.</param>
        /// <param name="value">The parameter value.</param>
        public override void SetParameterValue(DbCommand command, string parameterName, object value) {
            var convertedValue = value;
            var registry = GetParameterTypeRegistry(command.CommandText);

            if(registry != null) {
                if(registry.HasRegisteredParameterType(parameterName)) {
                    var dbType = registry.GetRegisteredParameterType(parameterName);

                    if(DbType.Guid == dbType) {
                        convertedValue = ConvertGuidToByteArray(value);
                    }
                    else if(DbType.Boolean == dbType) {
                        convertedValue = ConvertBoolToShort(value);
                    }
                }
            }

            base.SetParameterValue(command, parameterName, convertedValue);
        }

        /// <summary>
        /// Creates a <see cref="DbCommand"/> for a stored procedure.
        /// </summary>
        /// <param name="storedProcedureName">The name of the stored procedure.</param>
        /// <param name="parameterValues">The list of parameters for the procedure.</param>
        /// <returns>
        /// The <see cref="DbCommand"/> for the stored procedure.
        /// </returns>
        /// <remarks>
        /// The parameters for the stored procedure will be discovered and the values are assigned in positional order.
        /// </remarks>
        public override DbCommand GetStoredProcCommand(string storedProcedureName, params object[] parameterValues) {
            // need to do this before of eventual parameter discovery
            var updatedStoredProcedureName = TranslatePackageSchema(storedProcedureName);
            var command = base.GetStoredProcCommand(updatedStoredProcedureName, parameterValues);

            return command;
        }

        /// <summary>
        /// Creates a <see cref="DbCommand"/> for a stored procedure.
        /// </summary>
        /// <param name="storedProcedureName">The name of the stored procedure.</param>
        /// <returns>
        /// The <see cref="DbCommand"/> for the stored procedure.
        /// </returns>
        /// <remarks>
        /// The parameters for the stored procedure will be discovered and the values are assigned in positional order.
        /// </remarks>
        public override DbCommand GetStoredProcCommand(string storedProcedureName) {
            // need to do this before of eventual parameter discovery
            var updatedStoredProcedureName = TranslatePackageSchema(storedProcedureName);
            var command = base.GetStoredProcCommand(updatedStoredProcedureName);

            return command;
        }

        /// <summary>
        /// Sets the RowUpdated event for the data adapter.
        /// </summary>
        /// <param name="adapter">The <see cref="DbDataAdapter"/> to set the event.</param>
        /// <remarks>The <see cref="DbDataAdapter"/> must be an <see cref="OracleDataAdapter"/>.</remarks>
        protected override void SetUpRowUpdatedEvent(DbDataAdapter adapter) {
            ((OracleDataAdapter)adapter).RowUpdated += OnOracleRowUpdated;
        }

        /// <summary>
        /// Determines if the number of parameters in the command matches the array of parameter values.
        /// </summary>
        /// <param name="command">The <see cref="T:System.Data.Common.DbCommand"/> containing the parameters.</param>
        /// <param name="values">The array of parameter values.</param>
        /// <returns>
        /// 	<see langword="true"/> if the number of parameters and values match; otherwise, <see langword="false"/>.
        /// </returns>
        protected override bool SameNumberOfParametersAndValues(DbCommand command, object[] values) {
            var numberOfParametersToStoredProcedure = command.Parameters.Count;
            var numberOfValuesProvidedForStoredProcedure = values.Length;
            // Enterprise library will automatically add a Ref Cursor if one is not provided this must be accounted
            // for in the comparison
            foreach(DbParameter parameter in command.Parameters) {
                if(parameter.ParameterName.ToUpper(CultureInfo.CurrentCulture) == RefCursorName.ToUpper(CultureInfo.CurrentCulture))
                    numberOfParametersToStoredProcedure--;
            }

            return numberOfParametersToStoredProcedure == numberOfValuesProvidedForStoredProcedure;
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Checks if oracle command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        public static OracleCommand CheckIfOracleCommand(DbCommand command) {
            var oracleCommand = command as OracleCommand;
            oracleCommand.AssertIsOracleCommand();
            return oracleCommand;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Does the execute XML reader.
        /// </summary>
        /// <param name="oracleCommand">The oracle command.</param>
        /// <returns></returns>
        /// <remarks>
        /// Execute the actual XML Reader call.
        /// </remarks>
        private XmlReader DoExecuteXmlReader(OracleCommand oracleCommand) {
            try {
                return oracleCommand.ExecuteXmlReader();
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.ErrorException("XmlReader를 읽는데 실패했습니다.", ex);
                throw;
            }
        }

        /// <summary>
        /// Prepares the CW ref cursor.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <remarks>
        /// This is a private method that will build the Oracle package name if your stored procedure
        /// has proper prefix and postfix.
        /// This functionality is include for
        /// the portability of the architecture between SQL and Oracle datbase.
        /// This method also adds the reference cursor to the command writer if not already added. This
        /// is required for Oracle .NET managed data provider.
        /// </remarks>
        private void PrepareCWRefCursor(DbCommand command) {
            command.ShouldNotBeNull("command");

            if(CommandType.StoredProcedure == command.CommandType) {
                // Check for ref. cursor in the command writer, if it does not exist, add a known reference cursor out
                // of "cur_OUT"
                if(QueryProcedureNeedsCursorParameter(command)) {
                    AddParameter(command as OracleCommand, RefCursorName, OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0,
                                 0, String.Empty, DataRowVersion.Default, Convert.DBNull);
                }
            }
        }

        /// <summary>
        /// Gets the parameter type registry for the specified command.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <returns>Existing or new registry</returns>
        private ParameterTypeRegistry GetParameterTypeRegistry(string commandText) {
            ParameterTypeRegistry registry;

            if(_registeredParameterTypes.TryGetValue(commandText, out registry))
                return registry;

            registry = new ParameterTypeRegistry();
            _registeredParameterTypes.Add(commandText, registry);
            return registry;
        }

        /// <summary>
        /// Registers the type of the parameter in a parameter registry.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="dbType">Type of the parameter.</param>
        private void RegisterParameterType(DbCommand command, string parameterName, DbType dbType) {
            var registry = GetParameterTypeRegistry(command.CommandText);
            registry.RegisterParameterType(parameterName, dbType);
        }

        /// <summary>
        /// Called when a dataset row is updated.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="Oracle.DataAccess.Client.OracleRowUpdatedEventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// Listens for the RowUpdate event on a data adapter to support UpdateBehavior.Continue
        /// </remarks>
        private void OnOracleRowUpdated(object sender, OracleRowUpdatedEventArgs args) {
            if(args.RecordsAffected == 0) {
                if(args.Errors != null) {
                    args.Row.RowError = "행을 갱신하는데 실패했습니다.";
                    args.Status = UpdateStatus.SkipCurrentRow;
                }
            }
        }

        /// <summary>
        /// Translates the package schema.
        /// </summary>
        /// <param name="storedProcedureName">Name of the stored procedure.</param>
        /// <returns></returns>
        /// <remarks>
        /// Looks into configuration and gets the information on how the command wrapper should be updated if calling a package on this
        /// connection.
        /// </remarks>
        private string TranslatePackageSchema(string storedProcedureName) {
            var packageName = String.Empty;
            var updatedStoredProcedureName = storedProcedureName;

            if(_packages != null && !string.IsNullOrEmpty(storedProcedureName)) {
                foreach(IOraclePackage oraPackage in _packages) {
                    if((oraPackage.Prefix == AllPrefix) ||
                       (storedProcedureName.StartsWith(oraPackage.Prefix, StringComparison.CurrentCulture))) {
                        //use the package name for the matching prefix
                        packageName = oraPackage.Name;
                        break;
                    }
                }
            }

            if(packageName.Length > 0)
                updatedStoredProcedureName = String.Format(CultureInfo.InvariantCulture, "{0}.{1}", packageName, storedProcedureName);

            return updatedStoredProcedureName;
        }

        #endregion

        #region Private Static Methods

        /// <summary>
        /// Converts a Boolean object to a short or DBNull if null.
        /// </summary>
        /// <remarks>This is specifically used in the conversion of a Boolean to OracleDbType.Int16.
        /// Contrary to most interpretations of a boolean value a value of 0 (zero) stored in a Database
        /// is generally regarded as representing false.</remarks>
        /// <param name="value">The boolean value.</param>
        /// <returns>short integer of 0 for false and 1 for true</returns>
        private static object ConvertBoolToShort(object value) {
            return ((value is DBNull) || (value == null)) ? Convert.DBNull : ((bool)value ? (short)1 : (short)0);
        }

        /// <summary>
        /// Converts a short object to Boolean.
        /// </summary>
        /// <remarks>This is specifically used in the conversion of OracleDbType.Int16 back to Boolean</remarks>
        /// <param name="value">The short value.</param>
        /// <returns>Boolean object or DBNull</returns>
        private static object ConvertShortToBool(object value) {
            if(Equals(value, null))
                return DBNull.Value;

            short shortValue;
            // OracleDbType.Int16 is internally stored as an OracleDecimal instance
            if(value is OracleDecimal)
                shortValue = ((OracleDecimal)value).ToInt16();
            else
                shortValue = (short)value;

            return (shortValue != 0);
        }

        /// <summary>
        /// Converts a GUID object to a byte array or DBNull if null.
        /// </summary>
        /// <remarks>This is specifically used in the conversion of a Guid to OracleDbType.Raw</remarks>
        /// <param name="value">The Guid value.</param>
        /// <returns>byte array or DBNull</returns>
        private static object ConvertGuidToByteArray(object value) {
            return ((value is DBNull) || (value == null)) ? Convert.DBNull : ((Guid)value).ToByteArray();
        }

        /// <summary>
        /// Converts a byte array object to GUID.
        /// </summary>
        /// <remarks>This is specifically used in the conversion of OracleDbType.Raw back to Guid</remarks>
        /// <param name="value">The value.</param>
        /// <returns>Guid object or DBNull</returns>
        private static object ConvertByteArrayToGuid(object value) {
            byte[] buffer;
            // OracleDbType.Raw is internally stored as an OracleBinary instance
            if(value is OracleBinary)
                buffer = (byte[])((OracleBinary)value);
            else
                buffer = (byte[])value;

            if(buffer.Length == 0) {
                return DBNull.Value;
            }
            return new Guid(buffer);
        }

        /// <summary>
        /// Queries the procedure to see if it needs a cursor parameter.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        private static bool QueryProcedureNeedsCursorParameter(DbCommand command) {
            return
                command.Parameters
                    .Cast<OracleParameter>()
                    .All(parameter => parameter.OracleDbType != OracleDbType.RefCursor);
        }

        #endregion
    }
}