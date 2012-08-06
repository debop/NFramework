using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.Threading.Tasks;
using NSoft.NFramework.Data.Mappers;
using NSoft.NFramework.Data.SQLite.EnterpriseLibrary;
using NSoft.NFramework.Reflections;

namespace NSoft.NFramework.Data.SQLite {
    /// <summary>
    /// SQLite DB에 대한 Command 수행을 비동기 방식으로 수행하도록 해주는 메소드를 제공하는 Static Class 입니다.
    /// </summary>
    public static class SQLiteCommandAsync {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        #region << ExecuteDataTable Asynchronous >>

        /// <summary>
        /// <paramref name="cmd"/>를 비동기 방식으로 실행하여, 결과 셋을 <see cref="Task{TResult}"/>로 반환합니다.
        /// </summary>
        /// <param name="db">DAAB의 SQLite 용 Database 인스턴스</param>
        /// <param name="cmd">실행할 SQLiteCommand 인스턴스</param>
        /// <param name="parameters">파리미터 정보</param>
        public static Task<DataTable> ExecuteDataTableAsync(this SQLiteDatabase db, SQLiteCommand cmd, params IAdoParameter[] parameters) {
            return ExecuteDataTableAsync(db, cmd, 0, 0, parameters);
        }

        /// <summary>
        /// <paramref name="cmd"/>를 비동기 방식으로 실행하여, 결과 셋을 <see cref="Task{DataTable}"/>로 반환합니다.
        /// </summary>
        /// <param name="db">DAAB의 SQLite 용 Database 인스턴스</param>
        /// <param name="cmd">실행할 SQLiteCommand 인스턴스</param>
        /// <param name="firstResult">조회할 첫번째 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">조회할 최대 레코드 수 (0이면 최대)</param>
        /// <param name="parameters">파리미터 정보</param>
        public static Task<DataTable> ExecuteDataTableAsync(this SQLiteDatabase db, SQLiteCommand cmd, int firstResult, int maxResults,
                                                            params IAdoParameter[] parameters) {
            db.ShouldNotBeNull("db");
            cmd.ShouldNotBeNull("cmd");

            firstResult.ShouldBePositiveOrZero("firstResult");
            maxResults.ShouldBePositiveOrZero("maxResults");

            if(IsDebugEnabled)
                log.Debug("비동기 방식으로 ExecuteDataTable을 실행합니다.. CommandText=[{0}], firstResult=[{1}], maxResults=[{2}], Parameters=[{3}]",
                          cmd.CommandText, firstResult, maxResults, parameters.CollectionToString());

            // HINT: TaskContinuationOptions.OnlyOnRanToCompletion 등을 사용하면, Task의 필터링이 가능하지만, 정확한 예외정보가 전달되지 않습니다!!!
            //
            return
                ExecuteReaderAsync(db, cmd, parameters)
                    .ContinueWith(task => AdoTool.BuildDataTableFromDataReader(db, task.Result, firstResult, maxResults),
                                  TaskContinuationOptions.ExecuteSynchronously);
        }

        /// <summary>
        /// <paramref name="query"/>를 비동기 방식으로 실행하여, 결과 셋을 <see cref="Task{DataTable}"/>로 반환합니다.
        /// </summary>
        /// <param name="db">DAAB의 SQLite 용 Database 인스턴스</param>
        /// <param name="query">실행할 SQL String 또는 Procedure 명</param>
        /// <param name="parameters">파리미터 정보</param>
        public static Task<DataTable> ExecuteDataTableAsync(this SQLiteDatabase db, string query, params IAdoParameter[] parameters) {
            return ExecuteDataTableAsync(db, query, 0, 0, parameters);
        }

        /// <summary>
        /// <paramref name="query"/>를 비동기 방식으로 실행하여, 결과 셋을 <see cref="Task{DataTable}"/>로 반환합니다.
        /// </summary>
        /// <param name="db">DAAB의 SQLite 용 Database 인스턴스</param>
        /// <param name="query">실행할 SQL String 또는 Procedure 명</param>
        /// <param name="firstResult">조회할 첫번째 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">조회할 최대 레코드 수 (0이면 최대)</param>
        /// <param name="parameters">파리미터 정보</param>
        public static Task<DataTable> ExecuteDataTableAsync(this SQLiteDatabase db,
                                                            string query,
                                                            int firstResult,
                                                            int maxResults,
                                                            params IAdoParameter[] parameters) {
            var cmd = db.GetSQLiteCommand(query);

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
        /// <paramref name="sqlString"/>를 비동기 방식으로 실행하여, 결과 셋을 <see cref="Task{DataTable}"/>로 반환합니다.
        /// </summary>
        public static Task<DataTable> ExecuteDataTableAsyncBySqlString(this SQLiteDatabase db, string sqlString,
                                                                       params IAdoParameter[] parameters) {
            return ExecuteDataTableAsyncBySqlString(db, sqlString, 0, 0, parameters);
        }

        /// <summary>
        /// <paramref name="sqlString"/>를 비동기 방식으로 실행하여, 결과 셋을 <see cref="Task{DataTable}"/>로 반환합니다.
        /// </summary>
        /// <param name="db">DAAB의 SQLite 용 Database 인스턴스</param>
        /// <param name="sqlString">실행할 SQL String</param>
        /// <param name="firstResult">조회할 첫번째 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">조회할 최대 레코드 수 (0이면 최대)</param>
        /// <param name="parameters">파리미터 정보</param>
        public static Task<DataTable> ExecuteDataTableAsyncBySqlString(this SQLiteDatabase db,
                                                                       string sqlString,
                                                                       int firstResult,
                                                                       int maxResults,
                                                                       params IAdoParameter[] parameters) {
            var cmd = db.GetSqlStringSQLiteCommand(sqlString);
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
        /// <param name="db">DAAB의 SQLite 용 Database 인스턴스</param>
        /// <param name="spName">실행할 Procedure 명</param>
        /// <param name="parameters">파리미터 정보</param>
        public static Task<DataTable> ExecuteDataTableAsyncByProcedure(this SQLiteDatabase db, string spName,
                                                                       params IAdoParameter[] parameters) {
            return ExecuteDataTableAsyncByProcedure(db, spName, 0, 0, parameters);
        }

        /// <summary>
        /// <paramref name="spName"/>를 비동기 방식으로 실행하여, 결과 셋을 <see cref="Task{DataTable}"/>로 반환합니다.
        /// </summary>
        /// <param name="db">DAAB의 SQLite 용 Database 인스턴스</param>
        /// <param name="spName">실행할 Procedure 명</param>
        /// <param name="firstResult">조회할 첫번째 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">조회할 최대 레코드 수 (0이면 최대)</param>
        /// <param name="parameters">파리미터 정보</param>
        public static Task<DataTable> ExecuteDataTableAsyncByProcedure(this SQLiteDatabase db,
                                                                       string spName,
                                                                       int firstResult,
                                                                       int maxResults,
                                                                       params IAdoParameter[] parameters) {
            var cmd = db.GetProcedureSqlCommand(spName, AdoTool.DEFAULT_DISCOVER_PARAMETER);

            return
                ExecuteDataTableAsync(db, cmd, firstResult, maxResults, parameters)
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
        /// </summary>
        public static Task<IList<DataTable>> ExecuteDataTableAsListAsync(this SQLiteDatabase db, SQLiteCommand cmd,
                                                                         params IAdoParameter[] parameters) {
            return ExecuteDataTableAsListAsync(db, cmd, 0, 0, parameters);
        }

        /// <summary>
        /// Multi-ResultSet일 경우에 DataTable 컬렉션으로 반환합니다.
        /// </summary>
        /// <param name="db">DAAB의 SQLite 용 Database 인스턴스</param>
        /// <param name="cmd">실행할 SqlComnnad 인스턴스</param>
        /// <param name="firstResult">조회할 첫번째 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">조회할 최대 레코드 수 (0이면 최대)</param>
        /// <param name="parameters">파리미터 정보</param>
        public static Task<IList<DataTable>> ExecuteDataTableAsListAsync(this SQLiteDatabase db,
                                                                         SQLiteCommand cmd,
                                                                         int firstResult,
                                                                         int maxResults,
                                                                         params IAdoParameter[] parameters) {
            cmd.ShouldNotBeNull("cmd");
            firstResult.ShouldBePositiveOrZero("firstResult");
            maxResults.ShouldBePositiveOrZero("maxResults");

            if(IsDebugEnabled)
                log.Debug("비동기 방식으로 ExecuteDataTableAsListAsync을 실행합니다... " +
                          "CommandText=[{0}], firstResult=[{1}], maxResults=[{2}], Parameters=[{3}]",
                          cmd.CommandText, firstResult, maxResults, parameters.CollectionToString());

            return
                ExecuteReaderAsync(db, cmd, parameters)
                    .ContinueWith(task => {
                                      IList<DataTable> tables = new List<DataTable>();

                                      using(var reader = task.Result)
                                      using(var adapter = new AdoDataAdapter(db.GetDataAdapter())) {
                                          var resultCount = 0;
                                          do {
                                              var dataTable = new DataTable
                                                              {
                                                                  Locale = CultureInfo.InvariantCulture
                                                              };
                                              adapter.Fill(new[] { dataTable }, reader, firstResult, maxResults);

                                              if(IsDebugEnabled)
                                                  log.Debug("비동기 방식으로 DataReader를 가져와 DataTable에 Load 했습니다!!! resultCount=[{0}]",
                                                            ++resultCount);

                                              tables.Add(dataTable);
                                          } while(!reader.IsClosed && reader.NextResult());
                                      }

                                      return tables;
                                  },
                                  TaskContinuationOptions.ExecuteSynchronously);
        }

        #endregion

        #region << ExecuteNonQuery Asynchronous >>

        /// <summary>
        /// <see cref="SQLiteCommand"/>을 ExecuteNonQuery 메소드로 비동기 실행을 하도록 하는 <see cref="Task{Int32}"/>를 빌드합니다.
        /// </summary>
        /// <param name="db">DAAB의 SQLite 용 Database 인스턴스</param>
        /// <param name="cmd">실행할 SqlComnnad 인스턴스</param>
        /// <param name="parameters">파리미터 정보</param>
        /// <returns>실행에 영향을 받은 행의 수를 결과로 가지는 <see cref="Task{Int32}"/></returns>
        public static Task<int> ExecuteNonQueryAsync(this SQLiteDatabase db, SQLiteCommand cmd, params IAdoParameter[] parameters) {
            cmd.ShouldNotBeNull("cmd");

            if(IsDebugEnabled)
                log.Debug("cmd.ExecuteNonQuery를 비동기 방식으로 실행합니다. CommandText=[{0}], Parameters=[{1}]",
                          cmd.CommandText, cmd.Parameters.CollectionToString());


            var newConnectionCreated = false;

            if(cmd.Connection == null)
                cmd.Connection = SQLiteTool.CreateSQLiteConnection(db, ref newConnectionCreated);

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
        /// <param name="db">DAAB의 SQLite 용 Database 인스턴스</param>
        /// <param name="query">실행할 쿼리문 또는 Procecedure 명</param>
        /// <param name="parameters">파리미터 정보</param>
        /// <returns>실행에 영향을 받은 행의 수를 결과로 가지는 <see cref="Task{Int32}"/></returns>
        public static Task<int> ExecuteNonQueryAsync(this SQLiteDatabase db, string query, params IAdoParameter[] parameters) {
            var cmd = GetSQLiteCommand(db, query);

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
        /// <param name="db">DAAB의 SQLite 용 Database 인스턴스</param>
        /// <param name="sqlString">실행할 쿼리문</param>
        /// <param name="parameters">파리미터 정보</param>
        /// <returns>실행에 영향을 받은 행의 수를 결과로 가지는 <see cref="Task{Int32}"/></returns>
        public static Task<int> ExecuteNonQueryBySqlStringAsync(this SQLiteDatabase db, string sqlString,
                                                                params IAdoParameter[] parameters) {
            var cmd = GetSqlStringSQLiteCommand(db, sqlString);

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
        /// <param name="db">DAAB의 SQLite 용 Database 인스턴스</param>
        /// <param name="spName">실행할 프로시져 명</param>
        /// <param name="parameters">파리미터 정보</param>
        /// <returns>실행에 영향을 받은 행의 수를 결과로 가지는 <see cref="Task{Int32}"/></returns>
        public static Task<int> ExecuteNonQueryByProcedureAsync(this SQLiteDatabase db, string spName, params IAdoParameter[] parameters) {
            var cmd = GetProcedureSqlCommand(db, spName, AdoTool.DEFAULT_DISCOVER_PARAMETER);

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

        #region << ExecuteReader Asynchronous >>

        /// <summary>
        /// <paramref name="cmd"/> 를 이용하여, <see cref="Task{SqlDataReader}"/>를 반환받습니다. 
        /// 받환받은 DataReader는 꼭 Dispose() 해 주어야 Connection이 닫힙니다.
        /// </summary>
        /// <param name="db">DAAB의 SQLite 용 Database 인스턴스</param>
        /// <param name="cmd">실행할 cmd 인스턴스</param>
        /// <param name="parameters">Command Parameters</param>
        /// <returns></returns>
        public static Task<SQLiteDataReader> ExecuteReaderAsync(this SQLiteDatabase db, SQLiteCommand cmd,
                                                                params IAdoParameter[] parameters) {
            cmd.ShouldNotBeNull("cmd");

            if(IsDebugEnabled)
                log.Debug("cmd.ExecuteReader 를 비동기 방식으로 실행합니다. CommandText=[{0}], Parameters=[{1}]",
                          cmd.CommandText, parameters.CollectionToString());

            var newConnectionCreated = false;

            if(cmd.Connection == null)
                cmd.Connection = SQLiteTool.CreateSQLiteConnection(db, ref newConnectionCreated);

            if(parameters != null)
                AdoTool.SetParameterValues(db, cmd, parameters);

            var commandBehavior = newConnectionCreated ? CommandBehavior.CloseConnection : CommandBehavior.Default;

            return Task.Factory.StartNew(() => cmd.ExecuteReader(commandBehavior), TaskCreationOptions.PreferFairness);
        }

        /// <summary>
        /// <paramref name="query"/> 를 실행하여, <see cref="Task{SqlDataReader}"/>를 반환받습니다. 
        /// 받환받은 DataReader는 꼭 Dispose() 해 주어야 Connection이 닫힙니다.
        /// </summary>
        /// <param name="db">DAAB의 SQLite 용 Database 인스턴스</param>
        /// <param name="query">실행할 쿼리문 또는 Procedure 명</param>
        /// <param name="parameters">Command Parameters</param>
        /// <returns>결과 셋을 가지는 IDataReader를 결과로 가지는 Task</returns>
        public static Task<SQLiteDataReader> ExecuteReaderAsync(this SQLiteDatabase db, string query, params IAdoParameter[] parameters) {
            var cmd = GetSQLiteCommand(db, query);
            return ExecuteReaderAsync(db, cmd, parameters);
        }

        /// <summary>
        /// <paramref name="sqlString"/> 를 실행하여, <see cref="Task{SqlDataReader}"/>를 반환받습니다. 
        /// 받환받은 DataReader는 꼭 Dispose() 해 주어야 Connection이 닫힙니다.
        /// </summary>
        /// <param name="db">DAAB의 SQLite 용 Database 인스턴스</param>
        /// <param name="sqlString">실행할 쿼리문</param>
        /// <param name="parameters">Command Parameters</param>
        /// <returns>결과 셋을 가지는 IDataReader를 결과로 가지는 Task</returns>
        public static Task<SQLiteDataReader> ExecuteReaderBySqlStringAsync(this SQLiteDatabase db, string sqlString,
                                                                           params IAdoParameter[] parameters) {
            var cmd = GetSqlStringSQLiteCommand(db, sqlString);
            return ExecuteReaderAsync(db, cmd, parameters);
        }

        /// <summary>
        /// <paramref name="spName"/> 를 실행하여, <see cref="Task{SqlDataReader}"/>를 반환받습니다. 
        /// 받환받은 DataReader는 꼭 Dispose() 해 주어야 Connection이 닫힙니다.
        /// </summary>
        /// <param name="db">DAAB의 SQLite 용 Database 인스턴스</param>
        /// <param name="spName">실행할 프로시져 명</param>
        /// <param name="parameters">Command Parameters</param>
        /// <returns>결과 셋을 가지는 IDataReader를 결과로 가지는 Task</returns>
        public static Task<SQLiteDataReader> ExecuteReaderByProcedureAsync(this SQLiteDatabase db, string spName,
                                                                           params IAdoParameter[] parameters) {
            var cmd = GetProcedureSqlCommand(db, spName, AdoTool.DEFAULT_DISCOVER_PARAMETER);
            return ExecuteReaderAsync(db, cmd, parameters);
        }

        #endregion

        #region << ExecuteScalar Asynchronous >>

        /// <summary>
        /// <paramref name="cmd"/> 를 비동기 방식으로 실행하여, Scalar 값을 반환하는 <see cref="Task{Object}"/>를 빌드합니다.
        /// </summary>
        /// <param name="db">DAAB의 SQLite 용 Database 인스턴스</param>
        /// <param name="cmd">실행할 cmd 인스턴스</param>
        /// <param name="parameters">Command Parameters</param>
        /// <returns>결과 Scalar 값을 가지는 Task의 인스턴스</returns>
        public static Task<object> ExecuteScalarAsync(this SQLiteDatabase db, SQLiteCommand cmd, params IAdoParameter[] parameters) {
            cmd.ShouldNotBeNull("cmd");

            if(IsDebugEnabled)
                log.Debug("cmd.ExecuteScalar 를 비동기 방식으로 실행합니다. CommandText=[{0}], Parameters=[{1}]",
                          cmd.CommandText, parameters.CollectionToString());

            var newConnectionCreated = false;

            if(cmd.Connection == null)
                cmd.Connection = SQLiteTool.CreateSQLiteConnection(db, ref newConnectionCreated);

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
        /// <param name="db">DAAB의 SQLite 용 Database 인스턴스</param>
        /// <param name="query">실행할 쿼리문 또는 Procedure 명</param>
        /// <param name="parameters">Command Parameters</param>
        /// <returns>결과 Scalar 값을 가지는 Task의 인스턴스</returns>
        public static Task<object> ExecuteScalarAsync(this SQLiteDatabase db, string query, params IAdoParameter[] parameters) {
            var cmd = GetSQLiteCommand(db, query);
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
        /// <param name="db">DAAB의 SQLite 용 Database 인스턴스</param>
        /// <param name="sqlString">실행할 쿼리문</param>
        /// <param name="parameters">Command Parameters</param>
        /// <returns>결과 Scalar 값을 가지는 Task의 인스턴스</returns>
        public static Task<object> ExecuteScalarBySqlStringAsync(this SQLiteDatabase db, string sqlString,
                                                                 params IAdoParameter[] parameters) {
            var cmd = GetSQLiteCommand(db, sqlString);
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
        /// <param name="db">DAAB의 SQLite 용 Database 인스턴스</param>
        /// <param name="spName">실행할 쿼리문</param>
        /// <param name="parameters">Command Parameters</param>
        /// <returns>결과 Scalar 값을 가지는 Task의 인스턴스</returns>
        public static Task<object> ExecuteScalarByProcedureAsync(this SQLiteDatabase db, string spName,
                                                                 params IAdoParameter[] parameters) {
            var cmd = GetSQLiteCommand(db, spName, AdoTool.DEFAULT_DISCOVER_PARAMETER);
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
        /// <paramref name="cmd"/>를 실행하여, 결과 셋을 Persistent Object의 컬렉션으로 매핑합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="cmd"></param>
        /// <param name="mapObjectFactory"></param>
        /// <param name="nameMapper"></param>
        /// <param name="firstResult"></param>
        /// <param name="maxResults"></param>
        /// <param name="additionalMapping"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static Task<IList<T>> ExecuteMapObject<T>(this SQLiteDatabase db,
                                                         SQLiteCommand cmd,
                                                         Func<T> mapObjectFactory,
                                                         INameMapper nameMapper,
                                                         int firstResult,
                                                         int maxResults,
                                                         Action<IDataReader, T> additionalMapping,
                                                         params IAdoParameter[] parameters) {
            cmd.ShouldNotBeNull("cmd");

            if(IsDebugEnabled)
                log.Debug("DataReader를 열어, 수형[{2}]로 빌드합니다... CommandText=[{0}], Parameters=[{1}]",
                          cmd.CommandText, cmd.Parameters.CollectionToString(), typeof(T).FullName);

            return
                ExecuteReaderAsync(db, cmd, parameters)
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
        /// <paramref name="cmd"/>를 실행하여, 결과 셋을 Persistent Object의 컬렉션으로 매핑합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="cmd"></param>
        /// <param name="mapObjectFactory"></param>
        /// <param name="nameMap"></param>
        /// <param name="firstResult"></param>
        /// <param name="maxResults"></param>
        /// <param name="additionalMapping"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static Task<IList<T>> ExecuteMapObject<T>(this SQLiteDatabase db,
                                                         SQLiteCommand cmd,
                                                         Func<T> mapObjectFactory,
                                                         INameMap nameMap,
                                                         int firstResult,
                                                         int maxResults,
                                                         Action<IDataReader, T> additionalMapping,
                                                         params IAdoParameter[] parameters) {
            cmd.ShouldNotBeNull("npgsqlCommand");

            if(IsDebugEnabled)
                log.Debug("DataReader를 열어, 수형[{2}]로 빌드합니다... CommandText=[{0}], Parameters=[{1}]",
                          cmd.CommandText, cmd.Parameters.CollectionToString(), typeof(T).FullName);

            return
                ExecuteReaderAsync(db, cmd, parameters)
                    .ContinueWith(task => {
                                      using(var reader = task.Result)
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

        #region << GetSQLiteCommand >>

        /// <summary>
        /// 쿼리 문 또는 Procedure Name을 실행할 <see cref="SQLiteCommand"/>를 생성합니다.
        /// </summary>
        /// <param name="db">DAAB의 SQLite 용 Database 인스턴스</param>
        /// <param name="query">수행할 쿼리문 또는 Procedure Name</param>
        /// <returns>생성한 <see cref="SQLiteCommand"/></returns>
        public static SQLiteCommand GetSQLiteCommand(this SQLiteDatabase db, string query) {
            query.ShouldNotBeWhiteSpace("query");

            return GetSQLiteCommand(db, query, AdoTool.DEFAULT_DISCOVER_PARAMETER);
        }

        /// <summary>
        /// 쿼리 문 또는 Procedure Name을 실행할 <see cref="SQLiteCommand"/>를 생성합니다.
        /// </summary>
        /// <param name="db">DAAB의 SQLite 용 Database 인스턴스</param>
        /// <param name="query">수행할 쿼리문 또는 Procedure Name</param>
        /// <param name="discoverParams">Procedure일 경우 Parameter 빌드</param>
        /// <returns>생성한 <see cref="SQLiteCommand"/></returns>
        /// <seealso cref="AdoRepositoryImplBase.GetCommand(string,bool)"/>
        public static SQLiteCommand GetSQLiteCommand(this SQLiteDatabase db, string query, bool discoverParams) {
            db.ShouldNotBeNull("db");
            query.ShouldNotBeWhiteSpace("query");

            return (AdoTool.IsSqlString(query))
                       ? GetSqlStringSQLiteCommand(db, query)
                       : GetProcedureSqlCommand(db, query, discoverParams);
        }

        /// <summary>
        /// 쿼리 문 <paramref name="sqlString"/>을 수행할 <see cref="SQLiteCommand"/>를 생성합니다.
        /// </summary>
        /// <param name="db">DAAB의 SQLite 용 Database 인스턴스</param>
        /// <param name="sqlString">수행할 쿼리문</param>
        /// <returns>생성한 <see cref="SQLiteCommand"/></returns>
        public static SQLiteCommand GetSqlStringSQLiteCommand(this SQLiteDatabase db, string sqlString) {
            if(IsDebugEnabled)
                log.Debug("쿼리문을 수행할 SQLiteCommand를 생성합니다. sqlString=[{0}]", sqlString);

            return (SQLiteCommand)db.GetSqlStringCommand(sqlString);
        }

        /// <summary>
        /// Procedure <paramref name="spName"/>를 수행할 <see cref="SQLiteCommand"/> 를 생성합니다.
        /// </summary>
        /// <param name="db">DAAB의 SQLite 용 Database 인스턴스</param>
        /// <param name="spName">Procedure name</param>
        /// <param name="discoverParams">discover parameters</param>
        /// <returns>생성한 <see cref="SQLiteCommand"/></returns>
        public static SQLiteCommand GetProcedureSqlCommand(this SQLiteDatabase db, string spName, bool discoverParams) {
            if(IsDebugEnabled)
                log.Debug("Procedure를 수행할 SQLiteCommand를 생성합니다. spName=[{0}], discoverParams=[{1}]", spName, discoverParams);

            var cmd = db.GetStoredProcCommand(spName);

            if(discoverParams)
                db.DiscoverParameters(cmd);

            return (SQLiteCommand)cmd;
        }

        #endregion
    }
}