using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Threading.Tasks;
using NSoft.NFramework.Data.Mappers;
using NSoft.NFramework.Data.PostgreSql.EnterpriseLibrary;
using NSoft.NFramework.Reflections;
using Npgsql;

namespace NSoft.NFramework.Data.PostgreSql {
    /// <summary>
    /// PostgreSql Database 에 대한 DbCommand 실행을 비동기 방식으로 수행하도록 해주는 Class 입니다.
    /// </summary>
    public static class PostgreSqlCommandAsync {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        #region << ExecuteDataTableAsync >>

        /// <summary>
        /// <paramref name="cmd"/>를 비동기 방식으로 실행하여, 결과 셋을 <see cref="Task{DataTable}"/>로 반환합니다.
        /// </summary>
        public static Task<DataTable> ExecuteDataTableAsync(this NpgsqlDatabase db, NpgsqlCommand cmd, params IAdoParameter[] parameters) {
            return ExecuteDataTableAsync(db, cmd, 0, 0, parameters);
        }

        /// <summary>
        /// <paramref name="cmd"/>를 비동기 방식으로 실행하여, 결과 셋을 <see cref="Task{DataTable}"/>로 반환합니다.
        /// </summary>
        public static Task<DataTable> ExecuteDataTableAsync(this NpgsqlDatabase db,
                                                            NpgsqlCommand cmd,
                                                            int firstResult,
                                                            int maxResults,
                                                            params IAdoParameter[] parameters) {
            cmd.ShouldNotBeNull("cmd");
            firstResult.ShouldBePositiveOrZero("firstResult");
            maxResults.ShouldBePositiveOrZero("maxResults");

            if(IsDebugEnabled)
                log.Debug("비동기 방식으로 ExecuteDataTable을 실행합니다.. CommandText=[{0}], firstResult=[{1}], maxResults=[{2}], Parameters=[{3}]",
                          cmd.CommandText, firstResult, maxResults, parameters.CollectionToString());

            return
                ExecuteReaderAsync(db, cmd, parameters)
                    .ContinueWith(task => AdoTool.BuildDataTableFromDataReader(db, task.Result, firstResult, maxResults),
                                  TaskContinuationOptions.ExecuteSynchronously);
        }

        /// <summary>
        /// <paramref name="query"/>를 비동기 방식으로 실행하여, 결과 셋을 <see cref="Task{DataTable}"/>로 반환합니다.
        /// </summary>
        public static Task<DataTable> ExecuteDataTableAsync(this NpgsqlDatabase db, string query, params IAdoParameter[] parameters) {
            return ExecuteDataTableAsync(db, query, 0, 0, parameters);
        }

        /// <summary>
        /// <paramref name="query"/>를 비동기 방식으로 실행하여, 결과 셋을 <see cref="Task{DataTable}"/>로 반환합니다.
        /// </summary>
        public static Task<DataTable> ExecuteDataTableAsync(this NpgsqlDatabase db,
                                                            string query,
                                                            int firstResult,
                                                            int maxResult,
                                                            params IAdoParameter[] parameters) {
            var cmd = db.GetNpgsqlCommand(query);

            return
                ExecuteDataTableAsync(db, cmd, firstResult, maxResult, parameters)
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
        public static Task<DataTable> ExecuteDataTableAsyncBySqlString(this NpgsqlDatabase db,
                                                                       string sqlString,
                                                                       params IAdoParameter[] parameters) {
            return ExecuteDataTableAsyncBySqlString(db, sqlString, 0, 0, parameters);
        }

        /// <summary>
        /// <paramref name="sqlString"/>를 비동기 방식으로 실행하여, 결과 셋을 <see cref="Task{DataTable}"/>로 반환합니다.
        /// </summary>
        public static Task<DataTable> ExecuteDataTableAsyncBySqlString(this NpgsqlDatabase db,
                                                                       string sqlString,
                                                                       int firstResult,
                                                                       int maxResults,
                                                                       params IAdoParameter[] parameters) {
            var cmd = db.GetSqlStringNpgsqlCommand(sqlString);

            return
                ExecuteDataTableAsync(db, cmd, firstResult, maxResults, parameters)
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
        public static Task<DataTable> ExecuteDataTableAsyncByProcedure(this NpgsqlDatabase db,
                                                                       string spName,
                                                                       params IAdoParameter[] parameters) {
            return ExecuteDataTableAsyncByProcedure(db, spName, 0, 0, parameters);
        }

        /// <summary>
        /// <paramref name="spName"/>를 비동기 방식으로 실행하여, 결과 셋을 <see cref="Task{DataTable}"/>로 반환합니다.
        /// </summary>
        public static Task<DataTable> ExecuteDataTableAsyncByProcedure(this NpgsqlDatabase db,
                                                                       string spName,
                                                                       int firstResult,
                                                                       int maxResults,
                                                                       params IAdoParameter[] parameters) {
            var cmd = db.GetProcedureNpgsqlCommand(spName, AdoTool.DEFAULT_DISCOVER_PARAMETER);

            return
                ExecuteDataTableAsync(db, cmd, firstResult, maxResults, parameters)
                    .ContinueWith(task => {
                                      With.TryAction(() => cmd.Dispose());
                                      return task.Result;
                                  },
                                  TaskContinuationOptions.ExecuteSynchronously);
        }

        #endregion

        #region << ExecuteDataTableAsListAsync >>

        /// <summary>
        /// Multi-ResultSet일 경우에 DataTable 컬렉션으로 반환합니다.
        /// </summary>
        public static Task<IList<DataTable>> ExecuteDataTableAsListAsync(this NpgsqlDatabase db, NpgsqlCommand cmd,
                                                                         params IAdoParameter[] parameters) {
            return ExecuteDataTableAsListAsync(db, cmd, 0, 0, parameters);
        }

        /// <summary>
        /// Multi-ResultSet일 경우에 DataTable 컬렉션으로 반환합니다.
        /// </summary>
        public static Task<IList<DataTable>> ExecuteDataTableAsListAsync(this NpgsqlDatabase db,
                                                                         NpgsqlCommand cmd,
                                                                         int firstResult,
                                                                         int maxResults,
                                                                         params IAdoParameter[] parameters) {
            cmd.ShouldNotBeNull("cmd");
            firstResult.ShouldBePositiveOrZero("firstResult");
            maxResults.ShouldBePositiveOrZero("maxResults");

            if(IsDebugEnabled)
                log.Debug(
                    "비동기 방식으로 ExecuteDataTableAsListAsync을 실행합니다... CommandText=[{0}], firstResult=[{1}], maxResults=[{2}], Parameters=[{3}]",
                    cmd.CommandText, firstResult, maxResults, parameters.CollectionToString());

            return
                ExecuteReaderAsync(db, cmd, parameters)
                    .ContinueWith(task => {
                                      IList<DataTable> tables = new List<DataTable>();

                                      using(var reader = task.Result)
                                      using(var adapter = new AdoDataAdapter(db.GetDataAdapter())) {
                                          do {
                                              var dataTable = new DataTable { Locale = CultureInfo.InvariantCulture };
                                              adapter.Fill(new[] { dataTable }, reader, firstResult, maxResults);

                                              if(IsDebugEnabled)
                                                  log.Debug("비동기 방식으로 DataReader를 가져와 DataTable에 Load 했습니다!!!");

                                              tables.Add(dataTable);
                                          } while(reader.IsClosed == false && reader.NextResult());
                                      }

                                      return tables;
                                  },
                                  TaskContinuationOptions.ExecuteSynchronously);
        }

        #endregion

        #region << ExecuteNonQueryAsync >>

        /// <summary>
        /// <see cref="NpgsqlCommand"/>을 ExecuteNonQuery 메소드로 비동기 실행을 하도록 하는 <see cref="Task{Int32}"/>를 빌드합니다.
        /// </summary>
        /// <param name="db">DAAB의 MySQL 용 Database</param>
        /// <param name="cmd">실행할 SqlComnnad 인스턴스</param>
        /// <param name="parameters">파리미터 정보</param>
        /// <returns>실행에 영향을 받은 행의 수를 결과로 가지는 <see cref="Task{Int32}"/></returns>
        public static Task<int> ExecuteNonQueryAsync(this NpgsqlDatabase db, NpgsqlCommand cmd, params IAdoParameter[] parameters) {
            cmd.ShouldNotBeNull("cmd");

            if(IsDebugEnabled)
                log.Debug("NpgsqlCommand.ExecuteNonQuery를 비동기 방식으로 실행합니다. CommandText=[{0}], Parameters=[{1}]",
                          cmd.CommandText, cmd.Parameters.CollectionToString());


            var newConnectionCreated = false;

            if(cmd.Connection == null)
                cmd.Connection = db.CreateNpgsqlConnection(ref newConnectionCreated);

            if(parameters != null)
                AdoTool.SetParameterValues(db, cmd, parameters);

            return
                Task.Factory
                    .StartNew(() => cmd.ExecuteNonQuery(), TaskCreationOptions.PreferFairness)
                    .ContinueWith(task => {
                                      if(newConnectionCreated)
                                          AdoTool.ForceCloseConnection(cmd);
                                      return task;
                                  },
                                  TaskContinuationOptions.ExecuteSynchronously)
                    .Unwrap();
        }

        /// <summary>
        /// <paramref name="query"/>을 ExecuteNonQuery 메소드로 비동기 실행을 하도록 하는 <see cref="Task{Int32}"/>를 빌드합니다.
        /// </summary>
        /// <param name="db">DAAB의 MySQL 용 Database</param>
        /// <param name="query">실행할 쿼리문 또는 Procecedure 명</param>
        /// <param name="parameters">파리미터 정보</param>
        /// <returns>실행에 영향을 받은 행의 수를 결과로 가지는 <see cref="Task{Int32}"/></returns>
        public static Task<int> ExecuteNonQueryAsync(this NpgsqlDatabase db, string query, params IAdoParameter[] parameters) {
            var cmd = GetNpgsqlCommand(db, query);

            return
                ExecuteNonQueryAsync(db, cmd, parameters)
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
        /// <param name="db">DAAB의 MySQL 용 Database</param>
        /// <param name="sqlString">실행할 쿼리문</param>
        /// <param name="parameters">파리미터 정보</param>
        /// <returns>실행에 영향을 받은 행의 수를 결과로 가지는 <see cref="Task{Int32}"/></returns>
        public static Task<int> ExecuteNonQueryBySqlStringAsync(this NpgsqlDatabase db, string sqlString,
                                                                params IAdoParameter[] parameters) {
            var cmd = GetSqlStringNpgsqlCommand(db, sqlString);

            return
                ExecuteNonQueryAsync(db, cmd, parameters)
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
        /// <param name="db">DAAB의 MySQL 용 Database</param>
        /// <param name="spName">실행할 프로시져 명</param>
        /// <param name="parameters">파리미터 정보</param>
        /// <returns>실행에 영향을 받은 행의 수를 결과로 가지는 <see cref="Task{Int32}"/></returns>
        public static Task<int> ExecuteNonQueryByProcedureAsync(this NpgsqlDatabase db, string spName, params IAdoParameter[] parameters) {
            var cmd = GetProcedureNpgsqlCommand(db, spName, AdoTool.DEFAULT_DISCOVER_PARAMETER);

            return
                ExecuteNonQueryAsync(db, cmd, parameters)
                    .ContinueWith(task => {
                                      With.TryAction(() => cmd.Dispose());
                                      return task;
                                  },
                                  TaskContinuationOptions.ExecuteSynchronously)
                    .Unwrap();
        }

        #endregion

        #region << ExecuteReaderAsync >>

        /// <summary>
        /// <paramref name="cmd"/> 를 비동기 방식으로 실행하여, <see cref="Task{NpgsqlDataReader}"/>를 반환받습니다. 
        /// 받환받은 DataReader는 꼭 Dispose() 해 주어야 Connection이 닫힙니다.
        /// </summary>
        /// <param name="db">DAAB의 MySQL 용 Database</param>
        /// <param name="cmd">실행할 <see cref="NpgsqlCommand"/> 인스턴스</param>
        /// <param name="parameters">Command Parameters</param>
        /// <returns><see cref="NpgsqlDataReader"/>를 결과로 반환하는 <see cref="Task"/></returns>
        public static Task<NpgsqlDataReader> ExecuteReaderAsync(this NpgsqlDatabase db, NpgsqlCommand cmd,
                                                                params IAdoParameter[] parameters) {
            cmd.ShouldNotBeNull("cmd");

            if(IsDebugEnabled)
                log.Debug("NpgsqlCommand.ExecuteReader를 비동기 방식으로 실행합니다. CommandText=[{0}], Parameters=[{1}]",
                          cmd.CommandText, parameters.CollectionToString());

            var newConnectionCreated = false;

            if(cmd.Connection == null)
                cmd.Connection = db.CreateNpgsqlConnection(ref newConnectionCreated);

            if(parameters != null)
                AdoTool.SetParameterValues(db, cmd, parameters);

            var commandBehavior = newConnectionCreated
                                      ? CommandBehavior.CloseConnection
                                      : CommandBehavior.Default;

            return Task.Factory.StartNew(() => cmd.ExecuteReader(commandBehavior), TaskCreationOptions.PreferFairness);
        }

        /// <summary>
        /// <paramref name="query"/> 를 비동기 방식으로 실행하여, <see cref="Task{NpgsqlDataReader}"/>를 반환받습니다. 
        /// 받환받은 DataReader는 꼭 Dispose() 해 주어야 Connection이 닫힙니다.
        /// </summary>
        /// <param name="db">DAAB의 MySQL 용 Database</param>
        /// <param name="query">실행할 SQL 문</param>
        /// <param name="parameters">Command Parameters</param>
        /// <returns><see cref="NpgsqlDataReader"/>를 결과로 반환하는 <see cref="Task"/></returns>
        public static Task<NpgsqlDataReader> ExecuteReaderAsync(this NpgsqlDatabase db, string query, params IAdoParameter[] parameters) {
            var cmd = GetNpgsqlCommand(db, query);
            return ExecuteReaderAsync(db, cmd, parameters);
        }

        /// <summary>
        /// <paramref name="sqlString"/> 를 비동기 방식으로 실행하여, <see cref="Task{NpgsqlDataReader}"/>를 반환받습니다. 
        /// 받환받은 DataReader는 꼭 Dispose() 해 주어야 Connection이 닫힙니다.
        /// </summary>
        /// <param name="db">DAAB의 MySQL 용 Database</param>
        /// <param name="sqlString">실행할 SQL 문</param>
        /// <param name="parameters">Command Parameters</param>
        /// <returns><see cref="NpgsqlDataReader"/>를 결과로 반환하는 <see cref="Task"/></returns>
        public static Task<NpgsqlDataReader> ExecuteReaderBySqlStringAsync(this NpgsqlDatabase db,
                                                                           string sqlString,
                                                                           params IAdoParameter[] parameters) {
            var cmd = GetSqlStringNpgsqlCommand(db, sqlString);
            return ExecuteReaderAsync(db, cmd, parameters);
        }

        /// <summary>
        /// <paramref name="spName"/> 를 비동기 방식으로 실행하여, <see cref="Task{NpgsqlDataReader}"/>를 반환받습니다. 
        /// 받환받은 DataReader는 꼭 Dispose() 해 주어야 Connection이 닫힙니다.
        /// </summary>
        /// <param name="db">DAAB의 MySQL 용 Database</param>
        /// <param name="spName">실행할 Procedure 명</param>
        /// <param name="parameters">Command Parameters</param>
        /// <returns><see cref="NpgsqlDataReader"/>를 결과로 반환하는 <see cref="Task"/></returns>
        public static Task<NpgsqlDataReader> ExecuteReaderByProcedureAsync(this NpgsqlDatabase db,
                                                                           string spName,
                                                                           params IAdoParameter[] parameters) {
            var cmd = GetProcedureNpgsqlCommand(db, spName, AdoTool.DEFAULT_DISCOVER_PARAMETER);
            return ExecuteReaderAsync(db, cmd, parameters);
        }

        #endregion

        #region << ExecuteScalarAsync >>

        /// <summary>
        /// <paramref name="cmd"/> 를 비동기 방식으로 실행하여, Scalar 값을 반환하는 <see cref="Task{Object}"/>를 빌드합니다.
        /// </summary>
        /// <param name="db">DAAB의 MySQL 용 Database</param>
        /// <param name="cmd">실행할 NpgsqlCommand 인스턴스</param>
        /// <param name="parameters">Command Parameters</param>
        /// <returns>Scalar 값을 결과로 가지는 Task의 인스턴스</returns>
        public static Task<object> ExecuteScalarAsync(this NpgsqlDatabase db,
                                                      NpgsqlCommand cmd,
                                                      params IAdoParameter[] parameters) {
            cmd.ShouldNotBeNull("cmd");

            if(IsDebugEnabled)
                log.Debug("NpgsqlCommand.ExecuteScalar를 비동기 방식으로 실행합니다... CommandText=[{0}], Parameters=[{1}]",
                          cmd.CommandText, parameters.CollectionToString());

            var newConnectionCreated = false;

            if(cmd.Connection == null)
                cmd.Connection = db.CreateNpgsqlConnection(ref newConnectionCreated);

            if(parameters != null)
                AdoTool.SetParameterValues(db, cmd, parameters);

            return
                Task.Factory
                    .StartNew(() => cmd.ExecuteScalar(), TaskCreationOptions.PreferFairness)
                    .ContinueWith(task => {
                                      if(newConnectionCreated)
                                          AdoTool.ForceCloseConnection(cmd);

                                      return task;
                                  },
                                  TaskContinuationOptions.ExecuteSynchronously)
                    .Unwrap();
        }

        /// <summary>
        /// <paramref name="query"/> 를 비동기 방식으로 실행하여, Scalar 값을 반환하는 <see cref="Task{Object}"/>를 빌드합니다.
        /// </summary>
        /// <param name="db">DAAB의 MySQL 용 Database</param>
        /// <param name="query">실행할 쿼리문 또는 Procedure 명</param>
        /// <param name="parameters">Command Parameters</param>
        /// <returns>결과 Scalar 값을 가지는 Task의 인스턴스</returns>
        public static Task<object> ExecuteScalarAsync(this NpgsqlDatabase db,
                                                      string query,
                                                      params IAdoParameter[] parameters) {
            var cmd = GetNpgsqlCommand(db, query);
            return
                ExecuteScalarAsync(db, cmd, parameters)
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
        /// <param name="db">DAAB의 MySQL 용 Database</param>
        /// <param name="sqlString">실행할 쿼리문</param>
        /// <param name="parameters">Command Parameters</param>
        /// <returns>결과 Scalar 값을 가지는 Task의 인스턴스</returns>
        public static Task<object> ExecuteScalarBySqlStringAsync(this NpgsqlDatabase db,
                                                                 string sqlString,
                                                                 params IAdoParameter[] parameters) {
            var cmd = GetSqlStringNpgsqlCommand(db, sqlString);

            return
                ExecuteScalarAsync(db, cmd, parameters)
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
        /// <param name="db">DAAB의 MySQL 용 Database</param>
        /// <param name="spName">실행할 쿼리문</param>
        /// <param name="parameters">Command Parameters</param>
        /// <returns>결과 Scalar 값을 가지는 Task의 인스턴스</returns>
        public static Task<object> ExecuteScalarByProcedureAsync(this NpgsqlDatabase db,
                                                                 string spName,
                                                                 params IAdoParameter[] parameters) {
            var cmd = GetProcedureNpgsqlCommand(db, spName, AdoTool.DEFAULT_DISCOVER_PARAMETER);

            return
                ExecuteScalarAsync(db, cmd, parameters)
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
        /// <paramref name="npgsqlCommand"/>를 실행하여, 결과 셋을 Persistent Object의 컬렉션으로 매핑합니다.
        /// </summary>
        /// <typeparam name="T">Persistent Object의 수형</typeparam>
        /// <param name="npgsqlDatabase">NpgsqlDatabase 인스턴스</param>
        /// <param name="npgsqlCommand">NpgsqlCommand 인스턴스</param>
        /// <param name="mapObjectFactory">PersistentObject 생성 Factory</param>
        /// <param name="nameMapper">NameMapper 인스턴스</param>
        /// <param name="firstResult">첫번째 결과 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">최대 결과 갯수</param>
        /// <param name="additionalMapping">부가적인 매핑 작업을 수행할 델리게이트</param>
        /// <param name="parameters">OracleCommand에 설정할 Parameter 정보</param>
        /// <returns>DataReader로부터 인스턴싱된 Persistent Object의 컬렉션을 결과로 반환하는 Task</returns>
        public static Task<IList<T>> ExecuteMapObject<T>(this NpgsqlDatabase npgsqlDatabase,
                                                         NpgsqlCommand npgsqlCommand,
                                                         Func<T> mapObjectFactory,
                                                         INameMapper nameMapper,
                                                         int firstResult,
                                                         int maxResults,
                                                         Action<IDataReader, T> additionalMapping,
                                                         params IAdoParameter[] parameters) {
            npgsqlCommand.ShouldNotBeNull("NpgsqlCommand");

            if(IsDebugEnabled)
                log.Debug("DataReader를 열어, 수형[{2}]로 빌드합니다... CommandText=[{0}], Parameters=[{1}]",
                          npgsqlCommand.CommandText, npgsqlCommand.Parameters.CollectionToString(), typeof(T).FullName);

            return
                ExecuteReaderAsync(npgsqlDatabase, npgsqlCommand, parameters)
                    .ContinueWith(task => {
                                      using(var reader = task.Result)
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
        /// <paramref name="npgsqlCommand"/>를 실행하여, 결과 셋을 Persistent Object의 컬렉션으로 매핑합니다.
        /// </summary>
        /// <typeparam name="T">Persistent Object의 수형</typeparam>
        /// <param name="npgsqlDatabase">NpgsqlDatabase 인스턴스</param>
        /// <param name="npgsqlCommand">npgsqlCommand 인스턴스</param>
        /// <param name="mapObjectFactory">PersistentObject 생성 Factory</param>
        /// <param name="nameMap">NameMap 인스턴스</param>
        /// <param name="firstResult">첫번째 결과 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">최대 결과 갯수</param>
        /// <param name="additionalMapping">부가적인 매핑 작업을 수행할 델리게이트</param>
        /// <param name="parameters">OracleCommand에 설정할 Parameter 정보</param>
        /// <returns>DataReader로부터 인스턴싱된 Persistent Object의 컬렉션을 결과로 반환하는 Task</returns>
        public static Task<IList<T>> ExecuteMapObject<T>(this NpgsqlDatabase npgsqlDatabase,
                                                         NpgsqlCommand npgsqlCommand,
                                                         Func<T> mapObjectFactory,
                                                         INameMap nameMap,
                                                         int firstResult,
                                                         int maxResults,
                                                         Action<IDataReader, T> additionalMapping,
                                                         params IAdoParameter[] parameters) {
            npgsqlCommand.ShouldNotBeNull("npgsqlCommand");

            if(IsDebugEnabled)
                log.Debug("DataReader를 열어, 수형[{2}]로 빌드합니다... CommandText=[{0}], Parameters=[{1}]",
                          npgsqlCommand.CommandText, npgsqlCommand.Parameters.CollectionToString(), typeof(T).FullName);

            return
                ExecuteReaderAsync(npgsqlDatabase, npgsqlCommand, parameters)
                    .ContinueWith(task => {
                                      using(var reader = task.Result)
                                          return AdoTool.Map<T>(reader, mapObjectFactory, nameMap, firstResult, maxResults,
                                                                additionalMapping);
                                  },
                                  TaskContinuationOptions.ExecuteSynchronously);
        }

        #endregion

        #region << GetNpgsqlCommand >>

        /// <summary>
        /// 쿼리 문 또는 Procedure Name을 실행할 <see cref="NpgsqlCommand"/>를 생성합니다.
        /// </summary>
        /// <param name="db">DAAB db</param>
        /// <param name="query">수행할 쿼리문 또는 Procedure Name</param>
        /// <returns>생성한 <see cref="NpgsqlCommand"/></returns>
        public static NpgsqlCommand GetNpgsqlCommand(this NpgsqlDatabase db, string query) {
            query.ShouldNotBeWhiteSpace("query");
            return GetNpgsqlCommand(db, query, AdoTool.DEFAULT_DISCOVER_PARAMETER);
        }

        /// <summary>
        /// 쿼리 문 또는 Procedure Name을 실행할 <see cref="NpgsqlCommand"/>를 생성합니다.
        /// </summary>
        /// <param name="db">DAAB NpgsqlDatabase</param>
        /// <param name="query">수행할 쿼리문 또는 Procedure Name</param>
        /// <param name="discoverParams">Procedure일 경우 Parameter 빌드</param>
        /// <returns>생성한 <see cref="NpgsqlCommand"/></returns>
        /// <seealso cref="AdoRepositoryImplBase.GetCommand(string,bool)"/>
        public static NpgsqlCommand GetNpgsqlCommand(this NpgsqlDatabase db, string query, bool discoverParams) {
            query.ShouldNotBeWhiteSpace("query");

            return (AdoTool.IsSqlString(query))
                       ? GetSqlStringNpgsqlCommand(db, query)
                       : GetProcedureNpgsqlCommand(db, query, discoverParams);
        }

        /// <summary>
        /// 쿼리 문 <paramref name="sqlString"/>을 수행할 <see cref="NpgsqlCommand"/>를 생성합니다.
        /// </summary>
        /// <param name="db">DAAB NpgsqlDatabase</param>
        /// <param name="sqlString">수행할 쿼리문</param>
        /// <returns>생성한 <see cref="NpgsqlCommand"/></returns>
        public static NpgsqlCommand GetSqlStringNpgsqlCommand(this NpgsqlDatabase db, string sqlString) {
            if(IsDebugEnabled)
                log.Debug("쿼리문을 수행할 NpgsqlCommand를 생성합니다. sqlString=[{0}]", sqlString);

            return (NpgsqlCommand)db.GetSqlStringCommand(sqlString);
        }

        /// <summary>
        /// Procedure <paramref name="spName"/>를 수행할 <see cref="NpgsqlCommand"/> 를 생성합니다.
        /// </summary>
        /// <param name="db">DAAB NpgsqlDatabase</param>
        /// <param name="spName">Procedure name</param>
        /// <param name="discoverParams">discover parameters</param>
        /// <returns>생성한 <see cref="NpgsqlCommand"/></returns>
        public static NpgsqlCommand GetProcedureNpgsqlCommand(this NpgsqlDatabase db, string spName, bool discoverParams) {
            if(IsDebugEnabled)
                log.Debug("Procedure를 수행할 NpgsqlCommand를 생성합니다. spName=[{0}], discoverParams=[{1}]", spName, discoverParams);

            var cmd = db.GetStoredProcCommand(spName);

            if(discoverParams)
                db.DiscoverParameters(cmd);

            return (NpgsqlCommand)cmd;
        }

        #endregion
    }
}