using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using NSoft.NFramework.Data.OdpNet.EnterpriseLibrary;
using Oracle.DataAccess.Client;

namespace NSoft.NFramework.Data.OdpNet {
    public static class OdpNetTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public const int MaxTimeout = 500; // milliseconds

        /// <summary>
        /// The default ref cursor name used for all stored procedures that use ref cursors.
        /// </summary>
        private const string RefCursorName = "CUR_OUT";

        /// <summary>
        /// <paramref name="cmd"/>의 수형이 <see cref="OracleCommand"/>수형인지 확인합니다.
        /// </summary>
        /// <param name="cmd">수형 확인할 DbCommand 인스턴스</param>
        /// <exception cref="InvalidOperationException"><paramref name="cmd"/>의 수형이 <see cref="OracleCommand"/>가 아닐 때</exception>
        public static void AssertIsOracleCommand(this DbCommand cmd) {
            cmd.ShouldBeInstanceOf<OracleCommand>("cmd");
        }

        /// <summary>
        /// 모든 Oracle Connection Pool을 제거합니다.
        /// </summary>
        public static void ClearAllPools() {
            OracleConnection.ClearAllPools();
        }

        /// <summary>
        /// Oracle Connection을 비동기 방식으로 엽니다.
        /// </summary>
        /// <param name="db">DAAB OdpNetDatabase 인스턴스</param>
        /// <param name="newConnectionCreated">새로운 Connenction이 생성되었는지 여부</param>
        /// <returns>새로 연결된 <see cref="OracleConnection"/>의 인스턴스, 만약 연결에 실패했다면 null을 반환합니다.</returns>
        public static OracleConnection CreateOracleConnection(this OdpNetDatabase db, ref bool newConnectionCreated) {
            return
                (OracleConnection)
                AdoTool.CreateTransactionScopeConnection(db,
                                                         ref newConnectionCreated,
                                                         database => ((OdpNetDatabase)database).OpenConnection(15));
        }

        /// <summary>
        /// Oracle 연결이 성공하지 못하는 경우가 많아, 재시도 횟수 만큼 간격을 두고 연결을 시도합니다.
        /// Oracle DB의 Process 수를 기본(40)에서 100 이상으로 늘려주면 연결이 성공할 확률이 높습니다.
        /// </summary>
        /// <param name="db">DAAB OdpNetDatabase 인스턴스</param>
        /// <param name="tryCount">연결 실패 시, 재 시도 횟수</param>
        /// <returns>새로 연결된 <see cref="OracleConnection"/>의 인스턴스, 만약 연결에 실패했다면 null을 반환합니다.</returns>
        public static OracleConnection OpenConnection(this OdpNetDatabase db, int tryCount) {
            // NOTE: Oracle의 경우 Connection이 연결 안 되는 에러(ORA-12519) 가 자주 발생한다. 그래서, 시간 Time을 두고, 재시도하도록 하였다.
            // HINT: http://forums.oracle.com/forums/thread.jspa?messageID=1145120, 
            // HINT: http://kyeomstar.tistory.com/160

            if(IsDebugEnabled)
                log.Debug("Oracle Database를 연결하려고 합니다...");

            OracleConnection connection = null;

            for(var i = 0; i < tryCount; i++) {
                var count = i + 1;

                try {
                    if(IsDebugEnabled)
                        log.Debug("OracleConnection을 생성하고, 비동기 방식으로 Open합니다. 시도 횟수=[{0}]", (i + 1));

                    connection = (OracleConnection)db.CreateConnection();
                    connection.Open();
                    // DelegateAsync.Run(conn => conn.Open(), connection, null).WaitAsync(TimeSpan.FromSeconds(15));

                    if(connection.State == ConnectionState.Open) {
                        if(IsDebugEnabled)
                            log.Debug("OracleConnection을 연결했습니다!!!");

                        return connection;
                    }
                }
                catch(AggregateException age) {
                    age.Handle(ex => {
                                   HandleConnectionError(db, ex, count);
                                   return true;
                               });
                }
                catch(Exception ex) {
                    HandleConnectionError(db, ex, count);
                }
            }

            Guard.Assert(connection != null && connection.State != ConnectionState.Closed, "Oracle Connection을 열지 못했습니다!!!");
            return connection;
        }

        private static void HandleConnectionError(OdpNetDatabase db, Exception ex, int? tryCount) {
            // NOTE: Oracle의 경우, Connection 에러가 나면, 대기 후에 접속하면 성공한다. 왜 그런지는 잘 모른다...
            // HINT: http://kyeomstar.tistory.com/160
            //

            if(ex.Message.Contains("ORA-12519") || ex.Message.Contains("Unknown connection string parameter")) {
                if(log.IsWarnEnabled) {
                    log.Warn("Database Connection 생성 및 Open 수행 시에 예외가 발생했습니다. ConnectionString=[{0}]", db.ConnectionString);
                    log.Warn("Oracle 서버에 ALTER SYSTEM SET PROCESSES=500 SCOPE=SPFILE; 를 수행해 보세요. 기본 PROCESS 수가 40입니다. 이 값을 늘려보세요.");
                    log.Warn(ex);
                }
            }
            else {
                if(log.IsErrorEnabled) {
                    log.Error("Database Connection 생성 및 Open 수행 시에 예외가 발생했습니다. ConnectionString=[{0}]", db.ConnectionString);
                    log.Error(ex);
                }
            }

            var timeout = Math.Min(MaxTimeout, Math.Abs(tryCount.GetValueOrDefault(1)) * 50);
            Thread.Sleep(timeout);
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
        public static void PrepareCWRefCursor(this OdpNetDatabase db, OracleCommand command) {
            command.ShouldNotBeNull("command");

            if(CommandType.StoredProcedure == command.CommandType) {
                // Check for ref. cursor in the command writer, if it does not exist, add a known reference cursor out of "cur_OUT"
                if(QueryProcedureNeedsCursorParameter(command)) {
                    db.AddParameter(command, RefCursorName, OracleDbType.RefCursor, 0, ParameterDirection.Output, true, 0, 0,
                                    String.Empty, DataRowVersion.Default, Convert.DBNull);
                }
            }
        }

        /// <summary>
        /// Queries the procedure to see if it needs a cursor parameter.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        private static bool QueryProcedureNeedsCursorParameter(OracleCommand command) {
            return
                command.Parameters
                    .Cast<OracleParameter>()
                    .All(parameter => parameter.OracleDbType != OracleDbType.RefCursor);
        }
    }
}