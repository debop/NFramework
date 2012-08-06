using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using NSoft.NFramework.Data.Mappers;
using NSoft.NFramework.Reflections;

namespace NSoft.NFramework.Data.SqlServer {
    /// <summary>
    /// <see cref="SqlCommand"/>의 비동기 방식의 함수를 TPL(Task Parallel Library)를 사용하여 제공합니다.
    /// </summary>
    public static class SqlCommandAsync {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        #region << ExecuteDataTable Asynchronous >>

        /// <summary>
        /// <paramref name="sqlCommand"/>를 비동기 방식으로 실행하여, 결과 셋을 <see cref="Task{DataTable}"/>로 반환합니다.
        /// </summary>
        public static Task<DataTable> ExecuteDataTableAsync(this SqlDatabase sqlDatabase,
                                                            SqlCommand sqlCommand,
                                                            params IAdoParameter[] parameters) {
            return ExecuteDataTableAsync(sqlDatabase, sqlCommand, 0, 0, parameters);
        }

        /// <summary>
        /// <paramref name="sqlCommand"/>를 비동기 방식으로 실행하여, 결과 셋을 <see cref="Task{DataTable}"/>로 반환합니다.
        /// </summary>
        public static Task<DataTable> ExecuteDataTableAsync(this SqlDatabase sqlDatabase,
                                                            SqlCommand sqlCommand,
                                                            int firstResult,
                                                            int maxResults, params IAdoParameter[] parameters) {
            sqlDatabase.ShouldNotBeNull("sqlDatabase");
            sqlCommand.ShouldNotBeNull("sqlCommand");

            firstResult.ShouldBePositiveOrZero("firstResult");
            maxResults.ShouldBePositiveOrZero("maxResults");

            if(IsDebugEnabled)
                log.Debug("비동기 방식으로 ExecuteDataTable을 실행합니다.. " +
                          "CommandText=[{0}], firstResult=[{1}], maxResults=[{2}], Parameters=[{3}]",
                          sqlCommand.CommandText, firstResult, maxResults, parameters.CollectionToString());

            // TaskContinuationOptions.OnlyOnRanToCompletion 등을 사용하면, Task의 필터링이 가능하지만, 정확한 예외정보가 전달되지 않습니다!!!
            return
                ExecuteReaderAsync(sqlDatabase, sqlCommand, parameters)
                    .ContinueWith(task => AdoTool.BuildDataTableFromDataReader(sqlDatabase, task.Result, null, firstResult, maxResults),
                                  TaskContinuationOptions.ExecuteSynchronously);
        }

        /// <summary>
        /// <paramref name="query"/>를 비동기 방식으로 실행하여, 결과 셋을 <see cref="Task{DataTable}"/>로 반환합니다.
        /// </summary>
        public static Task<DataTable> ExecuteDataTableAsync(this SqlDatabase sqlDatabase,
                                                            string query,
                                                            params IAdoParameter[] parameters) {
            return ExecuteDataTableAsync(sqlDatabase, query, 0, 0, parameters);
        }

        /// <summary>
        /// <paramref name="query"/>를 비동기 방식으로 실행하여, 결과 셋을 <see cref="Task{DataTable}"/>로 반환합니다.
        /// </summary>
        public static Task<DataTable> ExecuteDataTableAsync(this SqlDatabase sqlDatabase,
                                                            string query,
                                                            int firstResult,
                                                            int maxResult,
                                                            params IAdoParameter[] parameters) {
            query.ShouldNotBeWhiteSpace("query");

            var cmd = sqlDatabase.GetSqlCommand(query);

            return
                ExecuteDataTableAsync(sqlDatabase, cmd, firstResult, maxResult, parameters)
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
        public static Task<DataTable> ExecuteDataTableAsyncBySqlString(this SqlDatabase sqlDatabase,
                                                                       string sqlString,
                                                                       params IAdoParameter[] parameters) {
            return ExecuteDataTableAsyncBySqlString(sqlDatabase, sqlString, 0, 0, parameters);
        }

        /// <summary>
        /// <paramref name="sqlString"/>를 비동기 방식으로 실행하여, 결과 셋을 <see cref="Task{DataTable}"/>로 반환합니다.
        /// </summary>
        public static Task<DataTable> ExecuteDataTableAsyncBySqlString(this SqlDatabase sqlDatabase,
                                                                       string sqlString,
                                                                       int firstResult,
                                                                       int maxResults,
                                                                       params IAdoParameter[] parameters) {
            sqlString.ShouldNotBeWhiteSpace("sqlString");

            var cmd = sqlDatabase.GetSqlStringSqlCommand(sqlString);

            return
                ExecuteDataTableAsync(sqlDatabase, cmd, firstResult, maxResults, parameters)
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
        public static Task<DataTable> ExecuteDataTableAsyncByProcedure(this SqlDatabase sqlDatabase,
                                                                       string spName,
                                                                       params IAdoParameter[] parameters) {
            return ExecuteDataTableAsyncByProcedure(sqlDatabase, spName, 0, 0, parameters);
        }

        /// <summary>
        /// <paramref name="spName"/>를 비동기 방식으로 실행하여, 결과 셋을 <see cref="Task{DataTable}"/>로 반환합니다.
        /// </summary>
        public static Task<DataTable> ExecuteDataTableAsyncByProcedure(this SqlDatabase sqlDatabase,
                                                                       string spName,
                                                                       int firstResult,
                                                                       int maxResults,
                                                                       params IAdoParameter[] parameters) {
            spName.ShouldNotBeWhiteSpace("spName");

            var cmd = sqlDatabase.GetProcedureSqlCommand(spName, AdoTool.DEFAULT_DISCOVER_PARAMETER);

            return
                ExecuteDataTableAsync(sqlDatabase, cmd, firstResult, maxResults, parameters)
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
        public static Task<IList<DataTable>> ExecuteDataTableAsListAsync(this SqlDatabase sqlDatabase,
                                                                         SqlCommand sqlCommand,
                                                                         params IAdoParameter[] parameters) {
            return ExecuteDataTableAsListAsync(sqlDatabase, sqlCommand, 0, 0, parameters);
        }

        /// <summary>
        /// Multi-ResultSet일 경우에 DataTable 컬렉션으로 반환합니다.
        /// </summary>
        public static Task<IList<DataTable>> ExecuteDataTableAsListAsync(this SqlDatabase sqlDatabase,
                                                                         SqlCommand sqlCommand,
                                                                         int firstResult,
                                                                         int maxResults,
                                                                         params IAdoParameter[] parameters) {
            sqlCommand.ShouldNotBeNull("sqlCommand");
            firstResult.ShouldBePositiveOrZero("firstResult");
            maxResults.ShouldBePositiveOrZero("maxResults");

            if(IsDebugEnabled)
                log.Debug("비동기 방식으로 ExecuteDataTableAsListAsync을 실행합니다.. " +
                          "CommandText=[{0}], firstResult=[{1}], maxResults=[{2}], Parameters=[{3}]",
                          sqlCommand.CommandText, firstResult, maxResults, parameters.CollectionToString());

            return
                ExecuteReaderAsync(sqlDatabase, sqlCommand, parameters)
                    .ContinueWith(task => {
                                      IList<DataTable> tables = new List<DataTable>();

                                      var reader = task.Result;
                                      var adapter = new AdoDataAdapter(sqlDatabase.GetDataAdapter());

                                      var resultCount = 0;
                                      do {
                                          var dataTable = new DataTable { Locale = CultureInfo.InvariantCulture };
                                          adapter.Fill(new[] { dataTable }, reader, firstResult, maxResults);

                                          if(IsDebugEnabled)
                                              log.Debug("비동기 방식으로 DataReader를 가져와 DataTable에 Load 했습니다!!! resultCount=[{0}]",
                                                        ++resultCount);

                                          tables.Add(dataTable);
                                      } while(reader.IsClosed == false && reader.NextResult());

                                      With.TryAction(reader.Dispose);
                                      With.TryAction(adapter.Dispose);

                                      return tables;
                                  },
                                  TaskContinuationOptions.ExecuteSynchronously);
        }

        #endregion

        #region << ExecuteNonQuery Asynchronous >>

        /// <summary>
        /// <see cref="SqlCommand"/>을 ExecuteNonQuery 메소드로 비동기 실행을 하도록 하는 <see cref="Task{Int32}"/>를 빌드합니다.
        /// </summary>
        /// <param name="sqlDatabase">DAAB의 MS SQL Server용 Database 인스턴스</param>
        /// <param name="sqlCommand">실행할 SqlComnnad 인스턴스</param>
        /// <param name="parameters">파리미터 정보</param>
        /// <returns>실행에 영향을 받은 행의 수를 결과로 가지는 <see cref="Task{Int32}"/></returns>
        public static Task<int> ExecuteNonQueryAsync(this SqlDatabase sqlDatabase,
                                                     SqlCommand sqlCommand,
                                                     params IAdoParameter[] parameters) {
            sqlCommand.ShouldNotBeNull("sqlCommand");

            if(IsDebugEnabled)
                log.Debug("sqlCommand.ExecuteNonQuery를 비동기 방식으로 실행합니다. CommandText=[{0}], Parameters=[{1}]",
                          sqlCommand.CommandText, sqlCommand.Parameters.CollectionToString());

            var newConnectionCreated = false;

            if(sqlCommand.Connection == null)
                sqlCommand.Connection = SqlTool.CreateSqlConnection(sqlDatabase, ref newConnectionCreated);

            if(parameters != null)
                AdoTool.SetParameterValues(sqlDatabase, sqlCommand, parameters);

            //! NOTE: FromAsync 메소드에서는 TaskCreationOptions.None 만 가능하다.
            //
            return
                Task<int>.Factory
                    .FromAsync(sqlCommand.BeginExecuteNonQuery,
                               sqlCommand.EndExecuteNonQuery,
                               null)
                    .ContinueWith(task => {
                                      if(newConnectionCreated)
                                          AdoTool.ForceCloseConnection(sqlCommand);
                                      return task;
                                  },
                                  TaskContinuationOptions.ExecuteSynchronously)
                    .Unwrap();
        }

        /// <summary>
        /// <paramref name="query"/>을 ExecuteNonQuery 메소드로 비동기 실행을 하도록 하는 <see cref="Task{Int32}"/>를 빌드합니다.
        /// </summary>
        /// <param name="sqlDatabase">DAAB의 MS SQL Server용 Database 인스턴스</param>
        /// <param name="query">실행할 쿼리문 또는 Procecedure 명</param>
        /// <param name="parameters">파리미터 정보</param>
        /// <returns>실행에 영향을 받은 행의 수를 결과로 가지는 <see cref="Task{Int32}"/></returns>
        public static Task<int> ExecuteNonQueryAsync(this SqlDatabase sqlDatabase,
                                                     string query,
                                                     params IAdoParameter[] parameters) {
            var cmd = GetSqlStringSqlCommand(sqlDatabase, query);

            return
                ExecuteNonQueryAsync(sqlDatabase, cmd, parameters)
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
        /// <param name="sqlDatabase">DAAB의 MS SQL Server용 Database 인스턴스</param>
        /// <param name="sqlString">실행할 쿼리문</param>
        /// <param name="parameters">파리미터 정보</param>
        /// <returns>실행에 영향을 받은 행의 수를 결과로 가지는 <see cref="Task{Int32}"/></returns>
        public static Task<int> ExecuteNonQueryBySqlStringAsync(this SqlDatabase sqlDatabase,
                                                                string sqlString,
                                                                params IAdoParameter[] parameters) {
            var cmd = GetSqlStringSqlCommand(sqlDatabase, sqlString);

            return
                ExecuteNonQueryAsync(sqlDatabase, cmd, parameters)
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
        /// <param name="sqlDatabase">DAAB의 MS SQL Server용 Database 인스턴스</param>
        /// <param name="spName">실행할 프로시져 명</param>
        /// <param name="parameters">파리미터 정보</param>
        /// <returns>실행에 영향을 받은 행의 수를 결과로 가지는 <see cref="Task{Int32}"/></returns>
        public static Task<int> ExecuteNonQueryByProcedureAsync(this SqlDatabase sqlDatabase,
                                                                string spName,
                                                                params IAdoParameter[] parameters) {
            var cmd = GetProcedureSqlCommand(sqlDatabase, spName, AdoTool.DEFAULT_DISCOVER_PARAMETER);

            return
                ExecuteNonQueryAsync(sqlDatabase, cmd, parameters)
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
        /// <paramref name="sqlCommand"/> 를 이용하여, <see cref="Task{SqlDataReader}"/>를 반환받습니다. 
        /// 받환받은 DataReader는 꼭 Dispose() 해 주어야 Connection이 닫힙니다.
        /// </summary>
        /// <param name="sqlDatabase">DAAB의 SQL Server용 Database</param>
        /// <param name="sqlCommand">실행할 sqlCommand 인스턴스</param>
        /// <param name="parameters">Command Parameters</param>
        /// <returns></returns>
        public static Task<SqlDataReader> ExecuteReaderAsync(this SqlDatabase sqlDatabase,
                                                             SqlCommand sqlCommand,
                                                             params IAdoParameter[] parameters) {
            sqlCommand.ShouldNotBeNull("sqlCommand");

            if(IsDebugEnabled)
                log.Debug("ExecuteReader 를 비동기 방식으로 실행합니다... CommandText=[{0}], Parameters=[{1}]",
                          sqlCommand.CommandText, parameters.CollectionToString());

            var newConnectionCreated = false;

            if(sqlCommand.Connection == null)
                sqlCommand.Connection = SqlTool.CreateSqlConnection(sqlDatabase, ref newConnectionCreated);

            if(parameters != null)
                AdoTool.SetParameterValues(sqlDatabase, sqlCommand, parameters);

            var commandBehavior = newConnectionCreated ? CommandBehavior.CloseConnection : CommandBehavior.Default;


            var tcs = new TaskCompletionSource<SqlDataReader>();

            sqlCommand.BeginExecuteReader(iar => {
                                              try {
                                                  tcs.TrySetResult(sqlCommand.EndExecuteReader(iar));
                                              }
                                              catch(Exception ex) {
                                                  tcs.TrySetException(ex);
                                              }
                                          },
                                          null,
                                          commandBehavior);

            return tcs.Task;

            //! NOTE: FromAsync를 사용하지 못한 이유가 sqlCommand.BeginExecuteReader() 의 overloading이 많아서, 모호한 함수 호출 때문이다.

            //var ar = sqlCommand.BeginExecuteReader(commandBehavior);
            //return Task.Factory.StartNew(() => sqlCommand.EndExecuteReader(ar));

            //Task<SqlDataReader>.Factory.StartNew(state => sqlCommand.EndExecuteReader((IAsyncResult)state),
            //									 ar,
            //									 TaskCreationOptions.None);
        }

        /// <summary>
        /// <paramref name="query"/> 를 실행하여, <see cref="Task{SqlDataReader}"/>를 반환받습니다. 
        /// 받환받은 DataReader는 꼭 Dispose() 해 주어야 Connection이 닫힙니다.
        /// </summary>
        /// <param name="sqlDatabase">DAAB의 SQL Server용 Database</param>
        /// <param name="query">실행할 쿼리문 또는 Procedure 명</param>
        /// <param name="parameters">Command Parameters</param>
        /// <returns>결과 셋을 가지는 IDataReader를 결과로 가지는 Task</returns>
        public static Task<SqlDataReader> ExecuteReaderAsync(this SqlDatabase sqlDatabase,
                                                             string query,
                                                             params IAdoParameter[] parameters) {
            var cmd = GetSqlCommand(sqlDatabase, query);
            return ExecuteReaderAsync(sqlDatabase, cmd, parameters);
        }

        /// <summary>
        /// <paramref name="sqlString"/> 를 실행하여, <see cref="Task{SqlDataReader}"/>를 반환받습니다. 
        /// 받환받은 DataReader는 꼭 Dispose() 해 주어야 Connection이 닫힙니다.
        /// </summary>
        /// <param name="sqlDatabase">DAAB의 SQL Server용 Database</param>
        /// <param name="sqlString">실행할 쿼리문</param>
        /// <param name="parameters">Command Parameters</param>
        /// <returns>결과 셋을 가지는 IDataReader를 결과로 가지는 Task</returns>
        public static Task<SqlDataReader> ExecuteReaderBySqlStringAsync(this SqlDatabase sqlDatabase,
                                                                        string sqlString,
                                                                        params IAdoParameter[] parameters) {
            var cmd = GetSqlStringSqlCommand(sqlDatabase, sqlString);
            return ExecuteReaderAsync(sqlDatabase, cmd, parameters);
        }

        /// <summary>
        /// <paramref name="spName"/> 를 실행하여, <see cref="Task{SqlDataReader}"/>를 반환받습니다. 
        /// 받환받은 DataReader는 꼭 Dispose() 해 주어야 Connection이 닫힙니다.
        /// </summary>
        /// <param name="sqlDatabase">DAAB의 SQL Server용 Database</param>
        /// <param name="spName">실행할 프로시져 명</param>
        /// <param name="parameters">Command Parameters</param>
        /// <returns>결과 셋을 가지는 IDataReader를 결과로 가지는 Task</returns>
        public static Task<SqlDataReader> ExecuteReaderByProcedureAsync(this SqlDatabase sqlDatabase,
                                                                        string spName,
                                                                        params IAdoParameter[] parameters) {
            var cmd = GetProcedureSqlCommand(sqlDatabase, spName, AdoTool.DEFAULT_DISCOVER_PARAMETER);
            return ExecuteReaderAsync(sqlDatabase, cmd, parameters);
        }

        #endregion

        #region << ExecuteScalar Asynchronous >>

        /// <summary>
        /// <paramref name="sqlCommand"/> 를 비동기 방식으로 실행하여, Scalar 값을 반환하는 <see cref="Task{Object}"/>를 빌드합니다.
        /// </summary>
        /// <param name="sqlDatabase">DAAB의 SQL Server용 Database</param>
        /// <param name="sqlCommand">실행할 sqlCommand 인스턴스</param>
        /// <param name="parameters">Command Parameters</param>
        /// <returns>결과 Scalar 값을 가지는 Task의 인스턴스</returns>
        public static Task<object> ExecuteScalarAsync(this SqlDatabase sqlDatabase,
                                                      SqlCommand sqlCommand,
                                                      params IAdoParameter[] parameters) {
            sqlCommand.ShouldNotBeNull("sqlCommand");

            if(IsDebugEnabled)
                log.Debug("sqlCommand.ExecuteScalar 를 비동기 방식으로 실행합니다. CommandText=[{0}], Parameters=[{1}]",
                          sqlCommand.CommandText, parameters.CollectionToString());

            var newConnectionCreated = false;

            if(sqlCommand.Connection == null)
                sqlCommand.Connection = SqlTool.CreateSqlConnection(sqlDatabase, ref newConnectionCreated);

            if(parameters != null)
                AdoTool.SetParameterValues(sqlDatabase, sqlCommand, parameters);

            return
                Task.Factory
                    .StartNew(() => sqlCommand.ExecuteScalar())
                    .ContinueWith(task => {
                                      if(newConnectionCreated)
                                          AdoTool.ForceCloseConnection(sqlCommand);

                                      return task;
                                  },
                                  TaskContinuationOptions.ExecuteSynchronously)
                    .Unwrap();
        }

        /// <summary>
        /// <paramref name="query"/> 를 비동기 방식으로 실행하여, Scalar 값을 반환하는 <see cref="Task{Object}"/>를 빌드합니다.
        /// </summary>
        /// <param name="sqlDatabase">DAAB의 SQL Server용 Database</param>
        /// <param name="query">실행할 쿼리문 또는 Procedure 명</param>
        /// <param name="parameters">Command Parameters</param>
        /// <returns>결과 Scalar 값을 가지는 Task의 인스턴스</returns>
        public static Task<object> ExecuteScalarAsync(this SqlDatabase sqlDatabase,
                                                      string query,
                                                      params IAdoParameter[] parameters) {
            var cmd = GetSqlCommand(sqlDatabase, query);

            return
                ExecuteScalarAsync(sqlDatabase, cmd, parameters)
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
        /// <param name="sqlDatabase">DAAB의 SQL Server용 Database</param>
        /// <param name="sqlString">실행할 쿼리문</param>
        /// <param name="parameters">Command Parameters</param>
        /// <returns>결과 Scalar 값을 가지는 Task의 인스턴스</returns>
        public static Task<object> ExecuteScalarBySqlStringAsync(this SqlDatabase sqlDatabase,
                                                                 string sqlString,
                                                                 params IAdoParameter[] parameters) {
            var cmd = GetSqlCommand(sqlDatabase, sqlString);

            return
                ExecuteScalarAsync(sqlDatabase, cmd, parameters)
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
        /// <param name="sqlDatabase">DAAB의 SQL Server용 Database</param>
        /// <param name="spName">실행할 쿼리문</param>
        /// <param name="parameters">Command Parameters</param>
        /// <returns>결과 Scalar 값을 가지는 Task의 인스턴스</returns>
        public static Task<object> ExecuteScalarByProcedureAsync(this SqlDatabase sqlDatabase,
                                                                 string spName,
                                                                 params IAdoParameter[] parameters) {
            var cmd = GetSqlCommand(sqlDatabase, spName, AdoTool.DEFAULT_DISCOVER_PARAMETER);

            return
                ExecuteScalarAsync(sqlDatabase, cmd, parameters)
                    .ContinueWith(task => {
                                      With.TryAction(() => cmd.Dispose());
                                      return task;
                                  },
                                  TaskContinuationOptions.ExecuteSynchronously)
                    .Unwrap();
        }

        #endregion

        #region << ExecuteXmlReader Asynchronous >>

        /// <summary>
        /// ExecuteXmlReader 를 비동기 IO 방식으로 실행하는 Task 를 빌드합니다.
        /// </summary>
        /// <param name="sqlDatabase">DAAB Database 인스턴스</param>
        /// <param name="sqlCommand">실행할 sqlCommand 인스턴스</param>
        /// <returns></returns>
        public static Task<XmlReader> ExecuteXmlReaderAsync(this SqlDatabase sqlDatabase, SqlCommand sqlCommand) {
            return ExecuteXmlReaderAsync(sqlDatabase, sqlCommand, true);
        }

        /// <summary>
        /// <see cref="SqlCommand.ExecuteXmlReader"/> 를 비동기 방식으로 실행하는 <see cref="Task{XmlReader}"/> 를 빌드합니다.
        /// </summary>
        /// <param name="sqlDatabase">DAAB Database 인스턴스</param>
        /// <param name="sqlCommand">실행할 sqlCommand 인스턴스</param>
        /// <param name="disposeCommandWhenCompleted">작업 완료 후 command를 dispose할 것인가?</param>
        /// <param name="parameters">Command paramters</param>
        /// <returns>비동기 실행용 <see cref="Task{XmlReader}"/></returns>
        public static Task<XmlReader> ExecuteXmlReaderAsync(this SqlDatabase sqlDatabase,
                                                            SqlCommand sqlCommand,
                                                            bool disposeCommandWhenCompleted,
                                                            params IAdoParameter[] parameters) {
            sqlCommand.ShouldNotBeNull("sqlCommand");

            if(IsDebugEnabled)
                log.Debug("sqlCommand.ExecuteXmlReader를 비동기 방식으로 실행합니다... CommandText=[{0}], parameters=[{1}]",
                          sqlCommand.CommandText, parameters.CollectionToString());


            var newConnectionCreated = false;

            if(sqlCommand.Connection == null)
                sqlCommand.Connection = SqlTool.CreateSqlConnection(sqlDatabase, ref newConnectionCreated);

            if(parameters != null)
                AdoTool.SetParameterValues(sqlDatabase, sqlCommand, parameters);

            return
                Task.Factory
                    .StartNew(() => sqlCommand.ExecuteXmlReader(), TaskCreationOptions.PreferFairness)
                    .ContinueWith(antecedent => {
                                      if(IsDebugEnabled)
                                          log.Debug("sqlCommand.ExecuteXmlReader를 비동기 방식으로 실행했습니다!!! CommandText=[{0}]",
                                                    sqlCommand.CommandText);

                                      if(newConnectionCreated)
                                          AdoTool.ForceCloseConnection(sqlCommand);

                                      if(disposeCommandWhenCompleted)
                                          With.TryAction(sqlCommand.Dispose);

                                      return antecedent.Result;
                                  },
                                  TaskContinuationOptions.ExecuteSynchronously);
        }

        /// <summary>
        /// <see cref="SqlCommand.ExecuteXmlReader"/> 를 비동기 방식으로 실행하는 <see cref="Task{XmlReader}"/> 를 빌드합니다.
        /// </summary>
        /// <param name="sqlDatabase">DAAB Database 인스턴스</param>
        /// <param name="query">실행할 쿼리문 또는 Procedure 명</param>
        /// <param name="parameters">parameters</param>
        /// <returns>비동기 실행용 <see cref="Task{XmlReader}"/></returns>
        public static Task<XmlReader> ExecuteXmlReaderAsync(this SqlDatabase sqlDatabase,
                                                            string query,
                                                            params IAdoParameter[] parameters) {
            var cmd = GetSqlCommand(sqlDatabase, query);

            return
                ExecuteXmlReaderAsync(sqlDatabase, cmd, true, parameters)
                    .ContinueWith(task => {
                                      With.TryAction(() => cmd.Dispose());
                                      return task;
                                  },
                                  TaskContinuationOptions.ExecuteSynchronously)
                    .Unwrap();
        }

        /// <summary>
        /// <see cref="SqlCommand.ExecuteXmlReader"/> 를 비동기 방식으로 실행하는 <see cref="Task{XmlReader}"/> 를 빌드합니다.
        /// </summary>
        /// <param name="sqlDatabase">DAAB Database 인스턴스</param>
        /// <param name="sqlString">실행할 쿼리문</param>
        /// <param name="parameters">parameters</param>
        /// <returns>비동기 실행용 <see cref="Task{XmlReader}"/></returns>
        public static Task<XmlReader> ExecuteXmlReaderBySqlStringAsync(this SqlDatabase sqlDatabase,
                                                                       string sqlString,
                                                                       params IAdoParameter[] parameters) {
            var cmd = GetSqlStringSqlCommand(sqlDatabase, sqlString);

            return
                ExecuteXmlReaderAsync(sqlDatabase, cmd, true, parameters)
                    .ContinueWith(task => {
                                      With.TryAction(() => cmd.Dispose());
                                      return task;
                                  },
                                  TaskContinuationOptions.ExecuteSynchronously)
                    .Unwrap();
        }

        /// <summary>
        /// Procedure <paramref name="spName"/> 을 비동기 방식으로 <see cref="SqlCommand.ExecuteXmlReader"/>을 수행하는 <see cref="Task{XmlReader}"/>를 빌드합니다.
        /// </summary>
        /// <param name="sqlDatabase">DAAB sqlDatabase</param>
        /// <param name="spName">수행할 Procedure name</param>
        /// <param name="parameters">Procedure 인자</param>
        public static Task<XmlReader> ExecuteXmlReaderByProcedureAsync(this SqlDatabase sqlDatabase,
                                                                       string spName,
                                                                       params IAdoParameter[] parameters) {
            var cmd = GetProcedureSqlCommand(sqlDatabase, spName, AdoTool.DEFAULT_DISCOVER_PARAMETER);

            return
                ExecuteXmlReaderAsync(sqlDatabase, cmd, true, parameters)
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
        /// <paramref name="sqlCommand"/>를 실행하여, 결과 셋을 Persistent Object의 컬렉션으로 매핑합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlDatabase"></param>
        /// <param name="sqlCommand"></param>
        /// <param name="mapObjectFactory"></param>
        /// <param name="nameMapper"></param>
        /// <param name="firstResult"></param>
        /// <param name="maxResults"></param>
        /// <param name="additionalMapping"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static Task<IList<T>> ExecuteMapObject<T>(this SqlDatabase sqlDatabase,
                                                         SqlCommand sqlCommand,
                                                         Func<T> mapObjectFactory,
                                                         INameMapper nameMapper,
                                                         int firstResult,
                                                         int maxResults,
                                                         Action<IDataReader, T> additionalMapping,
                                                         params IAdoParameter[] parameters) {
            sqlCommand.ShouldNotBeNull("sqlCommand");

            if(IsDebugEnabled)
                log.Debug("SqlCommand.ExecuteMapObject 를 비동기 방식으로 실행합니다. CommandText=[{0}], Parameters=[{1}]",
                          sqlCommand.CommandText, sqlCommand.Parameters.CollectionToString());

            return
                ExecuteReaderAsync(sqlDatabase, sqlCommand, parameters)
                    .ContinueWith(antecedent => {
                                      using(var reader = antecedent.Result)
                                          return AdoTool.Map<T>(reader, mapObjectFactory, nameMapper, firstResult, maxResults,
                                                                additionalMapping);
                                  },
                                  TaskContinuationOptions.ExecuteSynchronously);
        }

        /// <summary>
        /// <paramref name="sqlCommand"/>를 실행하여, 결과 셋을 Persistent Object의 컬렉션으로 매핑합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlDatabase"></param>
        /// <param name="sqlCommand"></param>
        /// <param name="mapObjectFactory"></param>
        /// <param name="nameMap"></param>
        /// <param name="firstResult"></param>
        /// <param name="maxResults"></param>
        /// <param name="additionalMapping"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static Task<IList<T>> ExecuteMapObject<T>(this SqlDatabase sqlDatabase,
                                                         SqlCommand sqlCommand,
                                                         Func<T> mapObjectFactory,
                                                         INameMap nameMap,
                                                         int firstResult,
                                                         int maxResults,
                                                         Action<IDataReader, T> additionalMapping,
                                                         params IAdoParameter[] parameters) {
            sqlCommand.ShouldNotBeNull("sqlCommand");

            if(IsDebugEnabled)
                log.Debug("SqlCommand.ExecuteMapObject 를 비동기 방식으로 실행합니다. CommandText=[{0}], Parameters=[{1}]",
                          sqlCommand.CommandText, sqlCommand.Parameters.CollectionToString());

            return
                ExecuteReaderAsync(sqlDatabase, sqlCommand, parameters)
                    .ContinueWith(antecedent => {
                                      using(var reader = antecedent.Result)
                                          return AdoTool.Map<T>(reader, mapObjectFactory, nameMap, firstResult, maxResults,
                                                                additionalMapping);
                                  },
                                  TaskContinuationOptions.ExecuteSynchronously);
        }

        #endregion

        #region << GetSqlCommand >>

        /// <summary>
        /// 쿼리 문 또는 Procedure Name을 실행할 <see cref="SqlCommand"/>를 생성합니다.
        /// </summary>
        /// <param name="sqlDatabase">DAAB sqlDatabase</param>
        /// <param name="query">수행할 쿼리문 또는 Procedure Name</param>
        /// <param name="discoverParams">Procedure일 경우 Parameter 빌드</param>
        /// <returns>생성한 <see cref="SqlCommand"/></returns>
        /// <seealso cref="AdoRepositoryImplBase.GetCommand(string,bool)"/>
        public static SqlCommand GetSqlCommand(this SqlDatabase sqlDatabase, string query, bool discoverParams = true) {
            sqlDatabase.ShouldNotBeNull("sqlDatabase");
            query.ShouldNotBeWhiteSpace("query");

            return (AdoTool.IsSqlString(query))
                       ? GetSqlStringSqlCommand(sqlDatabase, query)
                       : GetProcedureSqlCommand(sqlDatabase, query, discoverParams);
        }

        /// <summary>
        /// 쿼리 문 <paramref name="sqlString"/>을 수행할 <see cref="SqlCommand"/>를 생성합니다.
        /// </summary>
        /// <param name="sqlDatabase">DAAB sqlDatabase</param>
        /// <param name="sqlString">수행할 쿼리문</param>
        /// <returns>생성한 <see cref="SqlCommand"/></returns>
        public static SqlCommand GetSqlStringSqlCommand(this SqlDatabase sqlDatabase, string sqlString) {
            if(IsDebugEnabled)
                log.Debug("쿼리문을 수행할 SqlCommand를 생성합니다. sqlString=[{0}]", sqlString);

            return (SqlCommand)sqlDatabase.GetSqlStringCommand(sqlString);
        }

        /// <summary>
        /// Procedure <paramref name="spName"/>를 수행할 <see cref="SqlCommand"/> 를 생성합니다.
        /// </summary>
        /// <param name="sqlDatabase">DAAB sqlDatabase</param>
        /// <param name="spName">Procedure name</param>
        /// <param name="discoverParams">discover parameters</param>
        /// <returns>생성한 <see cref="SqlCommand"/></returns>
        public static SqlCommand GetProcedureSqlCommand(this SqlDatabase sqlDatabase, string spName,
                                                        bool discoverParams = AdoTool.DEFAULT_DISCOVER_PARAMETER) {
            if(IsDebugEnabled)
                log.Debug("Procedure를 수행할 SqlCommand를 생성합니다. spName=[{0}], discoverParams=[{1}]", spName, discoverParams);

            var cmd = sqlDatabase.GetStoredProcCommand(spName);

            if(discoverParams)
                sqlDatabase.DiscoverParameters(cmd);

            return (SqlCommand)cmd;
        }

        #endregion
    }
}