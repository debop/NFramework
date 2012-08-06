using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Threading.Tasks;
using Devart.Data.Oracle;
using NSoft.NFramework.Data.Mappers;
using NSoft.NFramework.Reflections;

namespace NSoft.NFramework.Data.DevartOracle {
    /// <summary>
    /// <see cref="OracleCommand"/>를 비동기 방식으로 수행하는 메소드를 제공합니다.
    /// </summary>
    public static class OracleCommandAsync {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        #region << ExecuteDataTable Asynchronous >>

        /// <summary>
        /// <paramref name="oraCommand"/>를 비동기 방식으로 실행하여, 결과 셋을 <see cref="Task{DataTable}"/>로 반환합니다.
        /// </summary>
        public static Task<OracleDataTable> ExecuteDataTableAsync(this EnterpriseLibrary.OracleDatabase oraDatabase,
                                                                  OracleCommand oraCommand,
                                                                  params IAdoParameter[] parameters) {
            return ExecuteDataTableAsync(oraDatabase, oraCommand, 0, 0, parameters);
        }

        /// <summary>
        /// <paramref name="oraCommand"/>를 비동기 방식으로 실행하여, 결과 셋을 <see cref="Task{DataTable}"/>로 반환합니다.
        /// </summary>
        public static Task<OracleDataTable> ExecuteDataTableAsync(this EnterpriseLibrary.OracleDatabase oraDatabase,
                                                                  OracleCommand oraCommand,
                                                                  int firstResult,
                                                                  int maxResults,
                                                                  params IAdoParameter[] parameters) {
            oraDatabase.ShouldNotBeNull("oraDatabase");
            oraCommand.ShouldNotBeNull("oraCommand");

            firstResult.ShouldBePositiveOrZero("firstResult");
            maxResults.ShouldBePositiveOrZero("maxResults");

            if(IsDebugEnabled)
                log.Debug("비동기 방식으로 ExecuteDataTable을 실행합니다.. CommandText=[{0}], firstResult=[{1}], maxResults=[{2}], Parameters=[{3}]",
                          oraCommand.CommandText, firstResult, maxResults, parameters.CollectionToString());

            return
                ExecuteReaderAsync(oraDatabase, oraCommand, parameters)
                    .ContinueWith(readerTask => (OracleDataTable)AdoTool.BuildDataTableFromDataReader(oraDatabase,
                                                                                                      readerTask.Result,
                                                                                                      () => new OracleDataTable(),
                                                                                                      firstResult,
                                                                                                      maxResults),
                                  TaskContinuationOptions.ExecuteSynchronously);
        }

        /// <summary>
        /// <paramref name="query"/>를 비동기 방식으로 실행하여, 결과 셋을 <see cref="Task{DataTable}"/>로 반환합니다.
        /// </summary>
        public static Task<OracleDataTable> ExecuteDataTableAsync(this EnterpriseLibrary.OracleDatabase oraDatabase,
                                                                  string query,
                                                                  params IAdoParameter[] parameters) {
            return ExecuteDataTableAsync(oraDatabase, query, 0, 0, parameters);
        }

        /// <summary>
        /// <paramref name="query"/>를 비동기 방식으로 실행하여, 결과 셋을 <see cref="Task{DataTable}"/>로 반환합니다.
        /// </summary>
        public static Task<OracleDataTable> ExecuteDataTableAsync(this EnterpriseLibrary.OracleDatabase oraDatabase,
                                                                  string query,
                                                                  int firstResult,
                                                                  int maxResult,
                                                                  params IAdoParameter[] parameters) {
            query.ShouldNotBeWhiteSpace("query");

            var cmd = oraDatabase.GetOracleCommand(query);
            return
                ExecuteDataTableAsync(oraDatabase, cmd, firstResult, maxResult, parameters)
                    .ContinueWith(task => {
                                      With.TryAction(() => cmd.Dispose());
                                      return task;
                                  },
                                  TaskContinuationOptions.ExecuteSynchronously)
                    .Unwrap();
        }

        /// <summary>
        /// <paramref name="sqlString"/>를 비동기 방식으로 실행하여, 결과 셋을 <see cref="Task{DataTable}"/>로 반환합니다.
        /// </summary>
        public static Task<OracleDataTable> ExecuteDataTableAsyncBySqlString(this EnterpriseLibrary.OracleDatabase oraDatabase,
                                                                             string sqlString,
                                                                             params IAdoParameter[] parameters) {
            return ExecuteDataTableAsyncBySqlString(oraDatabase, sqlString, 0, 0, parameters);
        }

        /// <summary>
        /// <paramref name="sqlString"/>를 비동기 방식으로 실행하여, 결과 셋을 <see cref="Task{DataTable}"/>로 반환합니다.
        /// </summary>
        public static Task<OracleDataTable> ExecuteDataTableAsyncBySqlString(this EnterpriseLibrary.OracleDatabase oraDatabase,
                                                                             string sqlString,
                                                                             int firstResult,
                                                                             int maxResults,
                                                                             params IAdoParameter[] parameters) {
            sqlString.ShouldNotBeWhiteSpace("sqlString");

            var cmd = oraDatabase.GetSqlStringOracleCommand(sqlString);

            return
                ExecuteDataTableAsync(oraDatabase, cmd, firstResult, maxResults, parameters)
                    .ContinueWith(task => {
                                      With.TryAction(() => cmd.Dispose());
                                      return task;
                                  },
                                  TaskContinuationOptions.ExecuteSynchronously)
                    .Unwrap();
        }

        /// <summary>
        /// <paramref name="spName"/>를 비동기 방식으로 실행하여, 결과 셋을 <see cref="Task{DataTable}"/>로 반환합니다.
        /// </summary>
        public static Task<OracleDataTable> ExecuteDataTableAsyncByProcedure(this EnterpriseLibrary.OracleDatabase oraDatabase,
                                                                             string spName,
                                                                             params IAdoParameter[] parameters) {
            return ExecuteDataTableAsyncByProcedure(oraDatabase, spName, 0, 0, parameters);
        }

        /// <summary>
        /// <paramref name="spName"/>를 비동기 방식으로 실행하여, 결과 셋을 <see cref="Task{DataTable}"/>로 반환합니다.
        /// </summary>
        public static Task<OracleDataTable> ExecuteDataTableAsyncByProcedure(this EnterpriseLibrary.OracleDatabase oraDatabase,
                                                                             string spName,
                                                                             int firstResult,
                                                                             int maxResults,
                                                                             params IAdoParameter[] parameters) {
            spName.ShouldNotBeWhiteSpace("spName");

            var cmd = oraDatabase.GetProcedureOracleCommand(spName, AdoTool.DEFAULT_DISCOVER_PARAMETER);

            return
                ExecuteDataTableAsync(oraDatabase, cmd, firstResult, maxResults, parameters)
                    .ContinueWith(task => {
                                      With.TryAction(() => cmd.Dispose());
                                      return task;
                                  },
                                  TaskContinuationOptions.ExecuteSynchronously)
                    .Unwrap();
        }

        #endregion

        #region << ExecuteDataTableAsList Asynchronous >>

        /// <summary>
        /// Multi-ResultSet일 경우에 DataTable 컬렉션으로 반환합니다.
        /// NOTE: Oracle에서는 Multi-ResultSet 작업을 위해서 SQL 문이 상당히 복잡합니다. 차라리 ExecuteDataTableAsync를 여러개 호출하세요.
        /// </summary>
        public static Task<IList<OracleDataTable>> ExecuteDataTableAsListAsync(this EnterpriseLibrary.OracleDatabase oraDatabase,
                                                                               OracleCommand oraCommand,
                                                                               params IAdoParameter[] parameters) {
            return ExecuteDataTableAsListAsync(oraDatabase, oraCommand, 0, 0, parameters);
        }

        /// <summary>
        /// Multi-ResultSet일 경우에 DataTable 컬렉션으로 반환합니다.
        /// NOTE: Oracle에서는 Multi-ResultSet 작업을 위해서 SQL 문이 상당히 복잡합니다. 차라리 ExecuteDataTableAsync를 여러개 호출하세요.
        /// </summary>
        public static Task<IList<OracleDataTable>> ExecuteDataTableAsListAsync(this EnterpriseLibrary.OracleDatabase oraDatabase,
                                                                               OracleCommand oraCommand,
                                                                               int? firstResult,
                                                                               int? maxResults,
                                                                               params IAdoParameter[] parameters) {
            oraCommand.ShouldNotBeNull("oraCommand");

            if(IsDebugEnabled)
                log.Debug(
                    "비동기 방식으로 ExecuteDataTable을 실행합니다... CommandText=[{0}], firstResult=[{1}], maxResults=[{2}], Parameters=[{3}]",
                    oraCommand.CommandText, firstResult, maxResults, parameters.CollectionToString());

            return
                ExecuteReaderAsync(oraDatabase, oraCommand, parameters)
                    .ContinueWith(task => {
                                      IList<OracleDataTable> tables = new List<OracleDataTable>();

                                      if(IsDebugEnabled)
                                          log.Debug("비동기방식으로 OracleDataReader를 가져와, OracleDataTable로 빌드합니다...");

                                      using(var reader = task.Result)
                                      using(var adapter = new AdoDataAdapter(oraDatabase.GetDataAdapter())) {
                                          do {
                                              var dataTable = new OracleDataTable { Locale = CultureInfo.InvariantCulture };
                                              adapter.Fill(new[] { dataTable }, reader, firstResult ?? 0, maxResults ?? 0);

                                              tables.Add(dataTable);
                                          } while(reader.IsClosed == false && reader.NextResult());
                                      }

                                      if(IsDebugEnabled)
                                          log.Debug("OracleDataReader로부터 OracleDataTable [{0}] 개를 빌드했습니다.", tables.Count);

                                      return tables;
                                  },
                                  TaskContinuationOptions.ExecuteSynchronously);
        }

        #endregion

        #region << ExecuteNonQuery Asynchronous >>

        /// <summary>
        /// <see cref="OracleCommand"/>을 ExecuteNonQuery 메소드로 비동기 실행을 하도록 하는 <see cref="Task{Int32}"/>를 빌드합니다.
        /// </summary>
        /// <param name="oraDatabase">Devart dotConnector for Oracle을 이용하여 생성한 OracleProvider</param>
        /// <param name="oraCommand">실행할 <see cref="OracleCommand"/> 인스턴스</param>
        /// <param name="parameters">파리미터 정보</param>
        /// <returns>실행에 영향을 받은 행의 수를 결과로 가지는 <see cref="Task{Int32}"/></returns>
        public static Task<int> ExecuteNonQueryAsync(this EnterpriseLibrary.OracleDatabase oraDatabase, OracleCommand oraCommand,
                                                     params IAdoParameter[] parameters) {
            oraCommand.ShouldNotBeNull("oraCommand");

            if(IsDebugEnabled)
                log.Debug("OracleCommand.ExecuteNonQuery를 비동기 방식으로 실행합니다. CommandText=[{0}], Parameters=[{1}]",
                          oraCommand.CommandText, oraCommand.Parameters.CollectionToString());


            // NOTE: FromAsync 메소드에서는 TaskCreationOptions.None 만 가능하다.
            //
            var newConnectionCreated = false;

            if(oraCommand.Connection == null)
                oraCommand.Connection = oraDatabase.CreateOracleConnection(ref newConnectionCreated);

            if(parameters != null)
                AdoTool.SetParameterValues(oraDatabase, oraCommand, parameters);

            return
                Task<int>.Factory
                    .FromAsync(oraCommand.BeginExecuteNonQuery,
                               oraCommand.EndExecuteNonQuery,
                               null)
                    .ContinueWith(task => {
                                      if(newConnectionCreated)
                                          AdoTool.ForceCloseConnection(oraCommand);
                                      return task;
                                  },
                                  TaskContinuationOptions.ExecuteSynchronously)
                    .Unwrap();
        }

        /// <summary>
        /// <paramref name="query"/>을 ExecuteNonQuery 메소드로 비동기 실행을 하도록 하는 <see cref="Task{Int32}"/>를 빌드합니다.
        /// </summary>
        /// <param name="oraDatabase">Devart dotConnector for Oracle을 이용하여 생성한 OracleProvider</param>
        /// <param name="query">실행할 쿼리문 또는 Procecedure 명</param>
        /// <param name="parameters">파리미터 정보</param>
        /// <returns>실행에 영향을 받은 행의 수를 결과로 가지는 <see cref="Task{Int32}"/></returns>
        public static Task<int> ExecuteNonQueryAsync(this EnterpriseLibrary.OracleDatabase oraDatabase, string query,
                                                     params IAdoParameter[] parameters) {
            var cmd = GetOracleCommand(oraDatabase, query);
            return
                ExecuteNonQueryAsync(oraDatabase, cmd, parameters)
                    .ContinueWith(task => {
                                      With.TryAction(() => cmd.Dispose());
                                      return task;
                                  },
                                  TaskContinuationOptions.ExecuteSynchronously)
                    .Unwrap();
        }

        /// <summary>
        /// <paramref name="sqlString"/>을 ExecuteNonQuery 메소드로 비동기 실행을 하도록 하는 <see cref="Task{Int32}"/>를 빌드합니다.
        /// </summary>
        /// <param name="oraDatabase">Devart dotConnector for Oracle을 이용하여 생성한 OracleProvider</param>
        /// <param name="sqlString">실행할 쿼리문</param>
        /// <param name="parameters">파리미터 정보</param>
        /// <returns>실행에 영향을 받은 행의 수를 결과로 가지는 <see cref="Task{Int32}"/></returns>
        public static Task<int> ExecuteNonQueryBySqlStringAsync(this EnterpriseLibrary.OracleDatabase oraDatabase, string sqlString,
                                                                params IAdoParameter[] parameters) {
            var cmd = GetSqlStringOracleCommand(oraDatabase, sqlString);
            return
                ExecuteNonQueryAsync(oraDatabase, cmd, parameters)
                    .ContinueWith(task => {
                                      With.TryAction(() => cmd.Dispose());
                                      return task;
                                  },
                                  TaskContinuationOptions.ExecuteSynchronously)
                    .Unwrap();
        }

        /// <summary>
        /// <paramref name="spName"/>을 ExecuteNonQuery 메소드로 비동기 실행을 하도록 하는 <see cref="Task{Int32}"/>를 빌드합니다.
        /// </summary>
        /// <param name="oraDatabase">Devart dotConnector for Oracle을 이용하여 생성한 OracleProvider</param>
        /// <param name="spName">실행할 프로시져 명</param>
        /// <param name="parameters">파리미터 정보</param>
        /// <returns>실행에 영향을 받은 행의 수를 결과로 가지는 <see cref="Task{Int32}"/></returns>
        public static Task<int> ExecuteNonQueryByProcedureAsync(this EnterpriseLibrary.OracleDatabase oraDatabase, string spName,
                                                                params IAdoParameter[] parameters) {
            var cmd = GetProcedureOracleCommand(oraDatabase, spName, AdoTool.DEFAULT_DISCOVER_PARAMETER);

            return
                ExecuteNonQueryAsync(oraDatabase, cmd, parameters)
                    .ContinueWith(task => {
                                      With.TryAction(() => cmd.Dispose());
                                      return task;
                                  },
                                  TaskContinuationOptions.ExecuteSynchronously)
                    .Unwrap();
        }

        #endregion

        #region << ExecuteInstance Asynchronous >>

        /// <summary>
        /// <paramref name="oraCommand"/> 를 이용하여, <see cref="Task{OracleDataReader}"/>를 반환받습니다. 
        /// 받환받은 DataReader는 꼭 Dispose() 해 주어야 Connection이 닫힙니다.
        /// </summary>
        /// <param name="oraDatabase">Devart dotConnector for Oracle을 이용하여 생성한 OracleProvider</param>
        /// <param name="oraCommand">실행할 OracleCommand 인스턴스</param>
        /// <param name="parameters">Command Parameters</param>
        /// <returns>OracleDataReader를 결과로 반환하는 Task</returns>
        public static Task<OracleDataReader> ExecuteReaderAsync(this EnterpriseLibrary.OracleDatabase oraDatabase,
                                                                OracleCommand oraCommand, params IAdoParameter[] parameters) {
            oraCommand.ShouldNotBeNull("oraCommand");

            if(IsDebugEnabled)
                log.Debug("OracleCommand.ExecuteReader를 비동기 방식으로 실행합니다. CommandText=[{0}], Parameters=[{1}]",
                          oraCommand.CommandText, parameters.CollectionToString());

            var newConnectionCreated = false;

            if(oraCommand.Connection == null)
                oraCommand.Connection = oraDatabase.CreateOracleConnection(ref newConnectionCreated);

            if(parameters != null)
                AdoTool.SetParameterValues(oraDatabase, oraCommand, parameters);

            var commandBehavior = newConnectionCreated ? CommandBehavior.CloseConnection : CommandBehavior.Default;

            // NOTE: FromAsync를 사용하지 못한 이유가 OracleCommand.BeginExecuteReader() 의 overloading이 많아서, 모호한 함수 호출 때문이다.
            //
            var ar = oraCommand.BeginExecuteReader(commandBehavior);

            return
                Task<OracleDataReader>.Factory.StartNew(state => oraCommand.EndExecuteReader((IAsyncResult)state),
                                                        ar,
                                                        TaskCreationOptions.PreferFairness);
        }

        /// <summary>
        /// <paramref name="query"/> 를 실행하여, <see cref="Task{OracleDataReader}"/>를 반환받습니다. 
        /// 받환받은 DataReader는 꼭 Dispose() 해 주어야 Connection이 닫힙니다.
        /// </summary>
        /// <param name="oraDatabase">Devart dotConnector for Oracle을 이용하여 생성한 OracleProvider</param>
        /// <param name="query">실행할 쿼리문 또는 Procedure 명</param>
        /// <param name="parameters">Command Parameters</param>
        /// <returns>결과 셋을 가지는 IDataReader를 결과로 가지는 Task</returns>
        public static Task<OracleDataReader> ExecuteReaderAsync(this EnterpriseLibrary.OracleDatabase oraDatabase, string query,
                                                                params IAdoParameter[] parameters) {
            var cmd = GetOracleCommand(oraDatabase, query);
            return ExecuteReaderAsync(oraDatabase, cmd, parameters);
        }

        /// <summary>
        /// <paramref name="sqlString"/> 를 실행하여, <see cref="Task{OracleDataReader}"/>를 반환받습니다. 
        /// 받환받은 DataReader는 꼭 Dispose() 해 주어야 Connection이 닫힙니다.
        /// </summary>
        /// <param name="oraDatabase">Devart dotConnector for Oracle을 이용하여 생성한 OracleProvider</param>
        /// <param name="sqlString">실행할 쿼리문</param>
        /// <param name="parameters">Command Parameters</param>
        /// <returns>결과 셋을 가지는 IDataReader를 결과로 가지는 Task</returns>
        public static Task<OracleDataReader> ExecuteReaderBySqlStringAsync(this EnterpriseLibrary.OracleDatabase oraDatabase,
                                                                           string sqlString, params IAdoParameter[] parameters) {
            var cmd = GetSqlStringOracleCommand(oraDatabase, sqlString);
            return ExecuteReaderAsync(oraDatabase, cmd, parameters);
        }

        /// <summary>
        /// <paramref name="spName"/> 를 실행하여, <see cref="Task{OracleDataReader}"/>를 반환받습니다. 
        /// 받환받은 DataReader는 꼭 Dispose() 해 주어야 Connection이 닫힙니다.
        /// </summary>
        /// <param name="oraDatabase">Devart dotConnector for Oracle을 이용하여 생성한 OracleProvider</param>
        /// <param name="spName">실행할 프로시져 명</param>
        /// <param name="parameters">Command Parameters</param>
        /// <returns>결과 셋을 가지는 IDataReader를 결과로 가지는 Task</returns>
        public static Task<OracleDataReader> ExecuteReaderByProcedureAsync(this EnterpriseLibrary.OracleDatabase oraDatabase,
                                                                           string spName, params IAdoParameter[] parameters) {
            var cmd = GetProcedureOracleCommand(oraDatabase, spName, AdoTool.DEFAULT_DISCOVER_PARAMETER);
            return ExecuteReaderAsync(oraDatabase, cmd, parameters);
        }

        #endregion

        #region << ExecuteScalar Asynchronous >>

        /// <summary>
        /// <paramref name="oraCommand"/> 를 비동기 방식으로 실행하여, Scalar 값을 반환하는 <see cref="Task{Object}"/>를 빌드합니다.
        /// </summary>
        /// <param name="oraDatabase">Devart dotConnector for Oracle을 이용하여 생성한 OracleProvider</param>
        /// <param name="oraCommand">실행할 OracleCommand 인스턴스</param>
        /// <param name="parameters">Command Parameters</param>
        /// <returns>결과 Scalar 값을 가지는 Task의 인스턴스</returns>
        public static Task<object> ExecuteScalarAsync(this EnterpriseLibrary.OracleDatabase oraDatabase, OracleCommand oraCommand,
                                                      params IAdoParameter[] parameters) {
            oraCommand.ShouldNotBeNull("oraCommand");

            if(IsDebugEnabled)
                log.Debug("OracleCommand.ExecuteScalar 를 비동기 방식으로 실행합니다. CommandText=[{0}], Parameters=[{1}]",
                          oraCommand.CommandText, oraCommand.Parameters.CollectionToString());

            var newConnectionCreated = false;

            if(oraCommand.Connection == null)
                oraCommand.Connection = oraDatabase.CreateOracleConnection(ref newConnectionCreated);

            if(parameters != null)
                AdoTool.SetParameterValues(oraDatabase, oraCommand, parameters);

            return
                Task.Factory
                    .StartNew(cmd => ((OracleCommand)cmd).ExecuteScalar(), oraCommand, TaskCreationOptions.PreferFairness)
                    .ContinueWith(task => {
                                      if(newConnectionCreated)
                                          AdoTool.ForceCloseConnection(oraCommand);
                                      return task;
                                  },
                                  TaskContinuationOptions.ExecuteSynchronously)
                    .Unwrap();
        }

        /// <summary>
        /// <paramref name="query"/> 를 비동기 방식으로 실행하여, Scalar 값을 반환하는 <see cref="Task{Object}"/>를 빌드합니다.
        /// </summary>
        /// <param name="oraDatabase">Devart dotConnector for Oracle을 이용하여 생성한 OracleProvider</param>
        /// <param name="query">실행할 쿼리문 또는 Procedure 명</param>
        /// <param name="parameters">Command Parameters</param>
        /// <returns>결과 Scalar 값을 가지는 Task의 인스턴스</returns>
        public static Task<object> ExecuteScalarAsync(this EnterpriseLibrary.OracleDatabase oraDatabase, string query,
                                                      params IAdoParameter[] parameters) {
            var cmd = GetOracleCommand(oraDatabase, query);
            return
                ExecuteScalarAsync(oraDatabase, cmd, parameters)
                    .ContinueWith(task => {
                                      With.TryAction(() => cmd.Dispose());
                                      return task;
                                  },
                                  TaskContinuationOptions.ExecuteSynchronously)
                    .Unwrap();
        }

        /// <summary>
        /// <paramref name="sqlString"/> 를 비동기 방식으로 실행하여, Scalar 값을 반환하는 <see cref="Task{Object}"/>를 빌드합니다.
        /// </summary>
        /// <param name="oraDatabase">Devart dotConnector for Oracle을 이용하여 생성한 OracleProvider</param>
        /// <param name="sqlString">실행할 쿼리문</param>
        /// <param name="parameters">Command Parameters</param>
        /// <returns>결과 Scalar 값을 가지는 Task의 인스턴스</returns>
        public static Task<object> ExecuteScalarBySqlStringAsync(this EnterpriseLibrary.OracleDatabase oraDatabase, string sqlString,
                                                                 params IAdoParameter[] parameters) {
            var cmd = GetOracleCommand(oraDatabase, sqlString);
            return
                ExecuteScalarAsync(oraDatabase, cmd, parameters)
                    .ContinueWith(task => {
                                      With.TryAction(() => cmd.Dispose());
                                      return task;
                                  },
                                  TaskContinuationOptions.ExecuteSynchronously)
                    .Unwrap();
        }

        /// <summary>
        /// <paramref name="spName"/> 를 비동기 방식으로 실행하여, Scalar 값을 반환하는 <see cref="Task{Object}"/>를 빌드합니다.
        /// </summary>
        /// <param name="oraDatabase">Devart dotConnector for Oracle을 이용하여 생성한 OracleProvider</param>
        /// <param name="spName">실행할 쿼리문</param>
        /// <param name="parameters">Command Parameters</param>
        /// <returns>결과 Scalar 값을 가지는 Task의 인스턴스</returns>
        public static Task<object> ExecuteScalarByProcedureAsync(this EnterpriseLibrary.OracleDatabase oraDatabase, string spName,
                                                                 params IAdoParameter[] parameters) {
            var cmd = GetOracleCommand(oraDatabase, spName, AdoTool.DEFAULT_DISCOVER_PARAMETER);
            return
                ExecuteScalarAsync(oraDatabase, cmd, parameters)
                    .ContinueWith(task => {
                                      With.TryAction(() => cmd.Dispose());
                                      return task;
                                  },
                                  TaskContinuationOptions.ExecuteSynchronously)
                    .Unwrap();
        }

        #endregion

        #region << ExecuteMapObject Asynchronous >>

        /// <summary>
        /// <paramref name="oraCommand"/>를 실행하여, 결과 셋을 Persistent Object의 컬렉션으로 매핑합니다.
        /// </summary>
        /// <typeparam name="T">Persistent Object의 수형</typeparam>
        /// <param name="oraDatabase">OracleProvider 인스턴스</param>
        /// <param name="oraCommand">OracleCommand 인스턴스</param>
        /// <param name="mapObjectFactory">PersistentObject 생성 Factory</param>
        /// <param name="nameMapper">NameMapper 인스턴스</param>
        /// <param name="firstResult">첫번째 결과 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">최대 결과 갯수</param>
        /// <param name="additionalMapping">부가적인 매핑 작업을 수행할 델리게이트</param>
        /// <param name="parameters">OracleCommand에 설정할 Parameter 정보</param>
        /// <returns>DataReader로부터 인스턴싱된 Persistent Object의 컬렉션을 결과로 반환하는 Task</returns>
        public static Task<IList<T>> ExecuteMapObject<T>(this EnterpriseLibrary.OracleDatabase oraDatabase,
                                                         OracleCommand oraCommand,
                                                         Func<T> mapObjectFactory,
                                                         INameMapper nameMapper,
                                                         int firstResult,
                                                         int maxResults,
                                                         Action<IDataReader, T> additionalMapping,
                                                         params IAdoParameter[] parameters) {
            oraCommand.ShouldNotBeNull("oraCommand");

            return
                ExecuteReaderAsync(oraDatabase, oraCommand, parameters)
                    .ContinueWith(antecedent => {
                                      using(var reader = antecedent.Result)
                                          return AdoTool.Map<T>(reader,
                                                                mapObjectFactory,
                                                                nameMapper,
                                                                firstResult,
                                                                maxResults,
                                                                additionalMapping);
                                  },
                                  TaskContinuationOptions.ExecuteSynchronously);
        }

        /// <summary>
        /// <paramref name="oraCommand"/>를 실행하여, 결과 셋을 Persistent Object의 컬렉션으로 매핑합니다.
        /// </summary>
        /// <typeparam name="T">Persistent Object의 수형</typeparam>
        /// <param name="oraDatabase">OracleProvider 인스턴스</param>
        /// <param name="oraCommand">OracleCommand 인스턴스</param>
        /// <param name="mapObjectFactory">PersistentObject 생성 Factory</param>
        /// <param name="nameMap">NameMap 인스턴스</param>
        /// <param name="firstResult">첫번째 결과 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">최대 결과 갯수</param>
        /// <param name="additionalMapping">부가적인 매핑 작업을 수행할 델리게이트</param>
        /// <param name="parameters">OracleCommand에 설정할 Parameter 정보</param>
        /// <returns>DataReader로부터 인스턴싱된 Persistent Object의 컬렉션을 결과로 반환하는 Task</returns>
        public static Task<IList<T>> ExecuteMapObject<T>(this EnterpriseLibrary.OracleDatabase oraDatabase,
                                                         OracleCommand oraCommand,
                                                         Func<T> mapObjectFactory,
                                                         INameMap nameMap,
                                                         int firstResult,
                                                         int maxResults,
                                                         Action<IDataReader, T> additionalMapping,
                                                         params IAdoParameter[] parameters) {
            oraCommand.ShouldNotBeNull("oraCommand");

            return
                ExecuteReaderAsync(oraDatabase, oraCommand, parameters)
                    .ContinueWith(antecedent => {
                                      using(var reader = antecedent.Result)
                                          return AdoTool.Map<T>(reader,
                                                                mapObjectFactory,
                                                                nameMap,
                                                                firstResult,
                                                                maxResults,
                                                                additionalMapping);
                                  },
                                  TaskContinuationOptions.ExecuteSynchronously);
        }

        #endregion

        #region << GetOracleCommand >>

        /// <summary>
        /// 쿼리 문 또는 Procedure Name을 실행할 <see cref="OracleCommand"/>를 생성합니다.
        /// </summary>
        /// <param name="oraDatabase">DAAB OracleProvider</param>
        /// <param name="query">수행할 쿼리문 또는 Procedure Name</param>
        /// <returns>생성한 <see cref="OracleCommand"/></returns>
        public static OracleCommand GetOracleCommand(this EnterpriseLibrary.OracleDatabase oraDatabase, string query) {
            query.ShouldNotBeWhiteSpace("query");
            return GetOracleCommand(oraDatabase, query, AdoTool.DEFAULT_DISCOVER_PARAMETER);
        }

        /// <summary>
        /// 쿼리 문 또는 Procedure Name을 실행할 <see cref="OracleCommand"/>를 생성합니다.
        /// </summary>
        /// <param name="oraDatabase">DAAB OracleProvider</param>
        /// <param name="query">수행할 쿼리문 또는 Procedure Name</param>
        /// <param name="discoverParams">Procedure일 경우 Parameter 빌드</param>
        /// <returns>생성한 <see cref="OracleCommand"/></returns>
        /// <seealso cref="AdoRepositoryImplBase.GetCommand(string,bool)"/>
        public static OracleCommand GetOracleCommand(this EnterpriseLibrary.OracleDatabase oraDatabase, string query,
                                                     bool discoverParams) {
            query.ShouldNotBeWhiteSpace("query");

            return (AdoTool.IsSqlString(query))
                       ? GetSqlStringOracleCommand(oraDatabase, query)
                       : GetProcedureOracleCommand(oraDatabase, query, discoverParams);
        }

        /// <summary>
        /// 쿼리 문 <paramref name="sqlString"/>을 수행할 <see cref="OracleCommand"/>를 생성합니다.
        /// </summary>
        /// <param name="oraDatabase">DAAB OracleProvider</param>
        /// <param name="sqlString">수행할 쿼리문</param>
        /// <returns>생성한 <see cref="OracleCommand"/></returns>
        public static OracleCommand GetSqlStringOracleCommand(this EnterpriseLibrary.OracleDatabase oraDatabase, string sqlString) {
            if(IsDebugEnabled)
                log.Debug("쿼리문을 수행할 OracleCommand를 생성합니다. sqlString=[{0}]", sqlString);

            return (OracleCommand)oraDatabase.GetSqlStringCommand(sqlString);
        }

        /// <summary>
        /// Procedure <paramref name="spName"/>를 수행할 <see cref="OracleCommand"/> 를 생성합니다.
        /// </summary>
        /// <param name="oraDatabase">DAAB OracleProvider</param>
        /// <param name="spName">Procedure name</param>
        /// <param name="discoverParams">discover parameters</param>
        /// <returns>생성한 <see cref="OracleCommand"/></returns>
        public static OracleCommand GetProcedureOracleCommand(this EnterpriseLibrary.OracleDatabase oraDatabase, string spName,
                                                              bool discoverParams) {
            if(IsDebugEnabled)
                log.Debug("Procedure를 수행할 OracleCommand를 생성합니다. spName=[{0}], discoverParams=[{1}]", spName, discoverParams);

            var cmd = oraDatabase.GetStoredProcCommand(spName);

            if(discoverParams)
                oraDatabase.DiscoverParameters(cmd);

            return (OracleCommand)cmd;
        }

        #endregion
    }
}