using System;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using Microsoft.Win32;
using NSoft.NFramework.IO;

namespace NSoft.NFramework.Data {
    /// <summary>
    /// SQL Server 용 DB 를 생성해주는 Helper class입니다.
    /// </summary>
    public static class SQLServerSetUpTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// create database media (file)
        /// </summary>
        public static void CreateDatabaseMedia(string databaseName) {
            if(log.IsInfoEnabled)
                log.Info("새로운 Database를 생성합니다... databaseName=[{0}]", databaseName);

            var sqlServerDataDirectory = GetSqlServerDataDirectory();
            var scripts = new StringBuilder();

            scripts
                .AppendFormat("IF (SELECT DB_ID('{0}')) IS NULL", databaseName).AppendLine()
                .AppendFormat("  CREATE DATABASE {0} ON PRIMARY", databaseName).AppendLine()
                .AppendFormat("    (NAME={0}_Data, FILENAME='{1}.mdf', SIZE=5MB, FILEGROWTH=1MB)", databaseName,
                              sqlServerDataDirectory + databaseName).AppendLine()
                .AppendFormat("  LOG ON (NAME={0}_Log, FILENAME='{1}.ldf', SIZE=1MB, FILEGROWTH=1MB)", databaseName,
                              sqlServerDataDirectory + databaseName).AppendLine();

            if(log.IsInfoEnabled) {
                log.Info("Create Database... databaseName=[{0}]", databaseName);
                log.Info("SQL Scripts=" + Environment.NewLine + scripts);
            }

            ExecuteDbScript(scripts.ToString(), ConnectionStringFor("master"));

            if(log.IsInfoEnabled)
                log.Info("새로운 Database를 생성 완료!!! databaseName=[{0}]", databaseName);
        }

        /// <summary>
        /// 지정된 database명으로 connection string을 만든다.
        /// </summary>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        internal static string ConnectionStringFor(string databaseName) {
            return string.Format("Server=(local);initial catalog={0};Integrated Security=SSPI;", databaseName);
        }

        /// <summary>
        /// 지정된 SQL Script를 실행한다.
        /// </summary>
        /// <param name="sqlScript">실행할 sql script</param>
        /// <param name="connectionString">대상 DB 서버에 대한 Connection String</param>
        internal static void ExecuteDbScript(string sqlScript, string connectionString) {
            using(var conn = new SqlConnection(connectionString))
            using(var command = new SqlCommand(sqlScript, conn)) {
                conn.Open();
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// SQL 서버의 기본 Data Directory 얻기 (Registry에 정의되어 있음)
        /// </summary>
        /// <returns></returns>
        public static string GetSqlServerDataDirectory() {
            return GetSqlServerDataDirectory("MSSQLSERVER");
        }

        /// <summary>
        /// SQL 서버의 기본 Data Directory 얻기 (Registry에 정의되어 있음)
        /// </summary>
        public static string GetSqlExpressDataDirectory() {
            return GetSqlServerDataDirectory("SQLEXPRESS");
        }

        /// <summary>
        /// SQL 서버의 기본 Data Directory 얻기 (Registry에 정의되어 있음)
        /// </summary>
        /// <returns></returns>
        public static string GetSqlServerDataDirectory(string databaseType) {
            try {
                // NOTE: 64bit에서 보안문제로 값을 읽어오지 못한다.
                //
                const string sqlServerRegKey = @"SOFTWARE\Microsoft\Microsoft SQL Server\";

                var sqlServerInstanceName =
                    (string)
                    Registry.LocalMachine.OpenSubKey(sqlServerRegKey + @"Instance Names\SQL").GetValue(databaseType ?? "MSSQLSERVER");
                var sqlServerInstanceSetupRegKey = string.Concat(sqlServerRegKey, sqlServerInstanceName, @"\Setup");

                return (string)Registry.LocalMachine.OpenSubKey(sqlServerInstanceSetupRegKey).GetValue("SQLDataRoot") + @"\Data\";
            }
            catch(Exception ex) {
                if(log.IsWarnEnabled) {
                    log.Warn("64bit 환경에서는 레지스트리에서 MS SQL 서버의 기본 Data Directory를 찾지 못했습니다.");
                    log.Warn(ex);
                }
            }

            var dataDir = FileTool.GetPhysicalPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Data\"));

            if(dataDir.DirectoryExists() == false) {
                dataDir.CreateDirectory();
            }

            if(IsDebugEnabled)
                log.Debug("새로운 Data Directory=[{0}]", dataDir);

            return dataDir;
        }
    }
}