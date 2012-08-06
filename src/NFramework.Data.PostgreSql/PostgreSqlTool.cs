using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using NSoft.NFramework.Data.PostgreSql.EnterpriseLibrary;
using Npgsql;

namespace NSoft.NFramework.Data.PostgreSql {
    /// <summary>
    /// PostgreSql Database 관련 Utility Class 입니다.
    /// 참고 : http://ydhoney.egloos.com/1972985
    /// </summary>
    public static class PostgreSqlTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public const int MaxTimeout = 500; // milliseconds

        /// <summary>
        /// <paramref name="cmd"/>의 수형이 <see cref="NpgsqlCommand"/>수형인지 확인합니다.
        /// </summary>
        /// <param name="cmd">수형 확인할 DbCommand 인스턴스</param>
        /// <exception cref="InvalidOperationException"><paramref name="cmd"/>의 수형이 <see cref="NpgsqlCommand"/>가 아닐 때</exception>
        public static void AssertIsNpgsqlCommand(this DbCommand cmd) {
            cmd.ShouldBeInstanceOf<NpgsqlCommand>("cmd");
        }

        /// <summary>
        /// Oracle Connection을 비동기 방식으로 엽니다.
        /// </summary>
        /// <param name="db">DAAB MySqlProvider 인스턴스</param>
        /// <param name="newConnectionCreated">새로운 Connenction이 생성되었는지 여부</param>
        /// <returns>새로 연결된 <see cref="NpgsqlConnection"/>의 인스턴스, 만약 연결에 실패했다면 null을 반환합니다.</returns>
        public static NpgsqlConnection CreateNpgsqlConnection(this NpgsqlDatabase db, ref bool newConnectionCreated) {
            return (NpgsqlConnection)AdoTool.CreateTransactionScopeConnection(db,
                                                                              ref newConnectionCreated,
                                                                              database => ((NpgsqlDatabase)database).OpenConnection(15));
        }

        /// <summary>
        /// MySQL Connection을 연결합니다.
        /// </summary>
        /// <param name="db">DAAB MySqlProvider 인스턴스</param>
        /// <param name="tryCount">연결 실패 시, 재 시도 횟수</param>
        /// <returns>새로 연결된 <see cref="NpgsqlConnection"/>의 인스턴스, 만약 연결에 실패했다면 null을 반환합니다.</returns>
        internal static NpgsqlConnection OpenConnection(this NpgsqlDatabase db, int tryCount) {
            if(IsDebugEnabled)
                log.Debug("PostgreSql Database에 연결하려고 합니다...");

            NpgsqlConnection connection = null;

            var count = 0;
            for(var i = 0; i < tryCount; i++) {
                try {
                    if(IsDebugEnabled)
                        log.Debug("NpgsqlConnection을 생성하고, 비동기 방식으로 Open합니다. 시도 횟수=[{0}]", ++count);

                    connection = (NpgsqlConnection)db.CreateConnection();
                    connection.Open();

                    if(connection.State == ConnectionState.Open) {
                        if(IsDebugEnabled)
                            log.Debug("NpgsqlConnection을 연결했습니다!!!");

                        return connection;
                    }
                }
                catch(Exception ex) {
                    HandleConnectionError(db, ex, count);
                }
            }

            Guard.Assert(connection != null && connection.State != ConnectionState.Closed, "NpgsqlConnection을 열지 못했습니다!!!");
            return connection;
        }

        private static void HandleConnectionError(NpgsqlDatabase db, Exception ex, int? tryCount) {
            if(log.IsErrorEnabled) {
                log.Error("Database Connection 생성 및 Open 수행 시에 예외가 발생했습니다. ConnectionString=[{0}]", db.ConnectionString);
                log.Error(ex);
            }

            With.TryAction(() => NpgsqlConnection.ClearAllPools());

            var timeout = Math.Min(MaxTimeout, Math.Abs(tryCount.GetValueOrDefault(1)) * 50);
            Thread.Sleep(timeout);
        }
    }
}