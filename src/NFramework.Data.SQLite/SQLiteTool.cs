using System;
using System.Data.SQLite;
using NSoft.NFramework.Data.SQLite.EnterpriseLibrary;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Data.SQLite {
    /// <summary>
    /// SQLite 용 Utility 클래스입니다.
    /// </summary>
    public static class SQLiteTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// SQLIte 성능을 높히기 위해, PRAGMA 설정을 합니다.<br />
        /// 참고 : http://www.sqlite.org/pragma.html
        /// </summary>
        public static readonly string[] PragmaSqlStrings = new[]
                                                           {
                                                               "PRAGMA synchronous=NORMAL;",
                                                               //! NOTE: Transaction 하에서 값을 설정하면 예외가 발생합니다.
                                                               "PRAGMA temp_store=MEMORY;",
                                                               "PRAGMA journal_mode=MEMORY;"
                                                           };

        /// <summary>
        /// 성능향상을 위해, SQLite 접속 시에 먼저 설정해 줄 PRAGMA 설정 정보
        /// </summary>
        public static readonly string PragmaSettings = PragmaSqlStrings.Join(string.Empty);

        /// <summary>
        /// SQLite용 AdoDataAdapter를 반환합니다.
        /// </summary>
        /// <param name="db">SqLiteProvider 인스턴스</param>
        /// <returns>AdoDataAdapter 인스턴스</returns>
        public static AdoDataAdapter GetAdoDataAdapter(this SQLiteDatabase db) {
            db.ShouldNotBeNull("db");

            return new AdoDataAdapter(db.GetDataAdapter());
        }

        /// <summary>
        /// <see cref="SQLiteConnection"/>을 생성하고, PRAGMA를 설정한 후, 반환합니다.
        /// </summary>
        /// <param name="db">SqLiteProvider 인스턴스</param>
        /// <param name="newConnectionCreated">새로운 연결인지 여부</param>
        /// <returns>SQLiteConnection 인스턴스</returns>
        public static SQLiteConnection CreateSQLiteConnection(this SQLiteDatabase db, ref bool newConnectionCreated) {
            db.ShouldNotBeNull("db");
            if(IsDebugEnabled)
                log.Debug("SQLite DB [{0}] 용 Connection을 생성합니다.", db.ConnectionString);

            var conn = (SQLiteConnection)AdoTool.CreateTransactionScopeConnection(db, ref newConnectionCreated);

            // 새로운 Connection이 생성되었을 때만 PRAGMA를 설정합니다.
            //
            if(newConnectionCreated)
                SetPragma(db, conn);

            return conn;
        }

        /// <summary>
        /// SQLite 성능을 높히기 위해, PRAGMA 설정을 수행합니다.<br />
        /// 참조 : http://www.sqlite.org/pragma.html
        /// </summary>
        /// <param name="db"></param>
        /// <param name="connection"></param>
        internal static void SetPragma(this SQLiteDatabase db, SQLiteConnection connection) {
            connection.ShouldNotBeNull("connection");

            if(IsDebugEnabled)
                log.Debug("SQLite DB에 PRAMA 설정을 수행합니다... PRAGMA 설정=[{0}]", PragmaSettings);

            try {
                //using(var cmd = new SQLiteCommand(PragmaSettings, connection))
                //	cmd.ExecuteNonQuery();
                new SQLiteCommand(PragmaSettings, connection).ExecuteNonQuery();
            }
            catch(Exception ex) {
                if(log.IsWarnEnabled)
                    log.WarnException("PRAGMA 설정 시에 예외가 발생했습니다. 무시합니다.", ex);
            }
        }
    }
}