using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace NSoft.NFramework.Data.SqlServer {
    public static partial class SqlTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public static readonly TimeSpan MaxTimeout = TimeSpan.FromMilliseconds(50);

        /// <summary>
        /// <paramref name="cmd"/>의 수형이 <see cref="SqlCommand"/>수형인지 확인합니다.
        /// </summary>
        /// <param name="cmd">수형 확인할 DbCommand 인스턴스</param>
        /// <exception cref="InvalidOperationException"><paramref name="cmd"/>의 수형이 <see cref="SqlCommand"/>가 아닐 때</exception>
        public static void AssertIsMSqlCommand(this DbCommand cmd) {
            cmd.ShouldBeInstanceOf<SqlCommand>("cmd");
        }

        /// <summary>
        /// Oracle Connection을 비동기 방식으로 엽니다.
        /// </summary>
        /// <param name="db">DAAB SqlDatabase 인스턴스</param>
        /// <param name="newConnectionCreated">새로운 Connenction이 생성되었는지 여부</param>
        /// <returns>새로 연결된 <see cref="SqlConnection"/>의 인스턴스, 만약 연결에 실패했다면 null을 반환합니다.</returns>
        public static SqlConnection CreateSqlConnection(this SqlDatabase db, ref bool newConnectionCreated) {
            return (SqlConnection)AdoTool.CreateTransactionScopeConnection(db,
                                                                           ref newConnectionCreated,
                                                                           database => ((SqlDatabase)database).OpenConnection());
        }

        /// <summary>
        /// MySQL Connection을 연결합니다.
        /// </summary>
        /// <param name="db">DAAB SqlDatabase 인스턴스</param>
        /// <param name="tryCount">연결 실패 시, 재 시도 횟수</param>
        /// <returns>새로 연결된 <see cref="SqlConnection"/>의 인스턴스, 만약 연결에 실패했다면 null을 반환합니다.</returns>
        internal static SqlConnection OpenConnection(this SqlDatabase db, int tryCount = 15) {
            if(IsDebugEnabled)
                log.Debug("SQL Server Database를 연결하려고 합니다...");

            SqlConnection connection = null;

            var count = 0;
            for(var i = 0; i < tryCount; i++) {
                try {
                    if(IsDebugEnabled)
                        log.Debug("SqlConnection을 생성하고, 비동기 방식으로 Open합니다. 시도 횟수=[{0}]", ++count);

                    connection = (SqlConnection)db.CreateConnection();
                    connection.Open();

                    if(connection.State == ConnectionState.Open) {
                        if(IsDebugEnabled)
                            log.Debug("SqlConnection을 연결했습니다!!!");

                        return connection;
                    }
                }
                catch(AggregateException age) {
                    var count1 = count;
                    age.Handle(ex => {
                                   HandleConnectionError(db, ex, count1);
                                   return true;
                               });
                }
                catch(Exception ex) {
                    HandleConnectionError(db, ex, count);
                }
            }

            Guard.Assert(connection != null && connection.State != ConnectionState.Closed, "Connection을 열지 못했습니다!!!");
            return connection;
        }

        private static void HandleConnectionError(SqlDatabase db, Exception ex, int? tryCount) {
            if(log.IsErrorEnabled) {
                log.Error("Database Connection 생성 및 Open 시에 예외가 발생했습니다. ConnectionString=[{0}]", db.ConnectionString);
                log.Error(ex);
            }

            var timeout = TimeSpan.FromMilliseconds(Math.Abs(tryCount ?? 1 * 5));
            Thread.Sleep((timeout < MaxTimeout) ? timeout : MaxTimeout);
        }
    }
}