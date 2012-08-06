using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using MySql.Data.MySqlClient;
using NSoft.NFramework.Data.MySql.EnterpriseLibrary;

namespace NSoft.NFramework.Data.MySql {
    /// <summary>
    /// MySql 을 위한 Utility class 입니다.
    /// </summary>
    public static class MySqlTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public const int MaxTimeout = 500; // milliseconds

        /// <summary>
        /// <paramref name="cmd"/>의 수형이 <see cref="MySqlCommand"/>수형인지 확인합니다.
        /// </summary>
        /// <param name="cmd">수형 확인할 DbCommand 인스턴스</param>
        /// <exception cref="InvalidOperationException"><paramref name="cmd"/>의 수형이 <see cref="MySqlCommand"/>가 아닐 때</exception>
        public static void AssertIsMySqlCommand(this DbCommand cmd) {
            cmd.ShouldBeInstanceOf<MySqlCommand>("cmd");
        }

        /// <summary>
        /// MySql Connection을 비동기 방식으로 엽니다.
        /// </summary>
        /// <param name="db">DAAB MySqlProvider 인스턴스</param>
        /// <param name="newConnectionCreated">새로운 Connenction이 생성되었는지 여부</param>
        /// <returns>새로 연결된 <see cref="MySqlConnection"/>의 인스턴스, 만약 연결에 실패했다면 null을 반환합니다.</returns>
        [CLSCompliant(false)]
        public static MySqlConnection CreateMySqlConnection(this MySqlDatabase db, ref bool newConnectionCreated) {
            return (MySqlConnection)AdoTool.CreateTransactionScopeConnection(db,
                                                                             ref newConnectionCreated,
                                                                             database => ((MySqlDatabase)database).OpenConnection(15));
        }

        /// <summary>
        /// MySql Connection을 연결합니다.
        /// </summary>
        /// <param name="db">DAAB MySqlProvider 인스턴스</param>
        /// <param name="tryCount">연결 실패 시, 재 시도 횟수</param>
        /// <returns>새로 연결된 <see cref="MySqlConnection"/>의 인스턴스, 만약 연결에 실패했다면 null을 반환합니다.</returns>
        internal static MySqlConnection OpenConnection(this MySqlDatabase db, int tryCount) {
            if(IsDebugEnabled)
                log.Debug("MySql Database를 연결하려고 합니다...");

            MySqlConnection connection = null;
            var count = 0;
            for(var i = 0; i < tryCount; i++) {
                try {
                    if(IsDebugEnabled)
                        log.Debug("MySqlConnection을 생성하고, 비동기 방식으로 Open합니다. 시도 횟수=[{0}]", ++count);

                    connection = (MySqlConnection)db.CreateConnection();
                    connection.Open();

                    if(connection.State == ConnectionState.Open) {
                        if(IsDebugEnabled)
                            log.Debug("MySqlConnection을 연결했습니다!!!");

                        return connection;
                    }
                }
                catch(Exception ex) {
                    HandleConnectionError(db, ex, count);
                }
            }

            Guard.Assert(connection != null && connection.State != ConnectionState.Closed, "MySql Connection을 열지 못했습니다!!!");
            return connection;
        }

        private static void HandleConnectionError(MySqlDatabase db, Exception ex, int? tryCount) {
            if(log.IsErrorEnabled) {
                log.Error("Database Connection 생성 및 Open 수행 시에 예외가 발생했습니다. ConnectionString=[{0}]", db.ConnectionString);
                log.Error(ex);
            }

            var timeout = Math.Min(MaxTimeout, Math.Abs(tryCount.GetValueOrDefault(1)) * 50);
            Thread.Sleep(timeout);
        }
    }
}