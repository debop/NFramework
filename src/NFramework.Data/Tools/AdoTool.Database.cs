using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Configuration;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Data {
    public static partial class AdoTool {
        private static DatabaseSettings _defaultDatabaseSettings;

        private static DatabaseSettings DefaultDatabaseSettings {
            get {
                if(_defaultDatabaseSettings == null)
                    lock(_syncLock)
                        if(_defaultDatabaseSettings == null) {
                            var dbsettings = ConfigurationManager.GetSection("dataConfiguration") as DatabaseSettings;
                            Guard.Assert(dbsettings != null,
                                         "환경설정에서 DAAB 관련 <dataConfiguration defaultDatabase=\"defaultDatabase\" /> 이 정의되지 않았습니다.");
                            Thread.MemoryBarrier();

                            _defaultDatabaseSettings = dbsettings;
                        }

                return _defaultDatabaseSettings;
            }
        }

        private static string _defaultDatabaseName;

        /// <summary>
        /// Default Database name (not connection string) that defined in configuration at defaultDatabase section with DAAB.
        /// </summary>
        public static string DefaultDatabaseName {
            get {
                if(_defaultDatabaseName == null)
                    lock(_syncLock)
                        if(_defaultDatabaseName == null) {
                            var dbName = DefaultDatabaseSettings.DefaultDatabase;

                            if(IsDebugEnabled)
                                log.Debug("기본 DatabaseName은 [{0}] 입니다.", dbName);

                            Thread.MemoryBarrier();
                            _defaultDatabaseName = dbName;
                        }

                return _defaultDatabaseName;
            }
        }

        private static ConnectionStringSettings _defaultConnectionStringSettings;

        /// <summary>
        /// DAAB에서 설정한 기본 Database의 <see cref="ConnectionStringSettings"/> 입니다.
        /// </summary>
        public static ConnectionStringSettings DefaultConnectionStringSettings {
            get {
                if(_defaultConnectionStringSettings == null)
                    lock(_syncLock)
                        if(_defaultConnectionStringSettings == null) {
                            var settings = ConfigTool.GetConnectionSettings(DefaultDatabaseName);

                            if(IsDebugEnabled)
                                log.Debug("기본 ConnectionStringSettings은 Name=[{0}], ConnectionString=[{1}], ProviderName=[{2}] 입니다.",
                                          settings.Name, settings.ConnectionString, settings.ProviderName);

                            Thread.MemoryBarrier();
                            _defaultConnectionStringSettings = settings;
                        }

                return _defaultConnectionStringSettings;
            }
        }

        /// <summary>
        /// create <see cref="Database"/> instance for default connection string
        /// </summary>
        /// <returns>instance of <see cref="Database"/> defined in DAAB</returns>
        public static Database CreateDatabase() {
            return CreateDatabase(DefaultDatabaseName);
        }

        /// <summary>
        /// create <see cref="Database"/> instance for the specified connection string name. if dbName is null or empty string, using default database.
        /// </summary>
        /// <param name="dbName">database connection string name</param>
        /// <returns>instance of <see cref="Database"/> defined in DAAB</returns>
        public static Database CreateDatabase(string dbName) {
            if(IsDebugEnabled)
                log.Debug("Enterprise Library의 Database 인스턴스를 생성합니다... dbName=[{0}]", dbName);

            return (dbName.IsWhiteSpace())
                       ? DatabaseFactory.CreateDatabase()
                       : DatabaseFactory.CreateDatabase(dbName);
        }

        /// <summary>
        /// 현재 Transaction에 해당하는 Connection을 반환한다.
        /// 이미 열려진 Connection을 재활용하거나, TransactionScope가 활성화되어 있다면 새로운 connection을 빌드해서 반환한다.
        /// TransactionScope도 없고, <see cref="Database"/>관련해서 아직 만들어진게 없다면, 새로운 <see cref="DbConnection"/>을 생성하고, Open() 해서 반환한다.
        /// </summary>
        /// <param name="db">instance of <see cref="Database"/></param>
        /// <returns>
        /// TransactionScope이 활성화되어 있다면, TransactionScope에서 활성화된 <see cref="DbConnection"/>을 반환,
        /// TransactionScope가 없다면 새로운 <see cref="DbConnection"/>을 생성하여 반환한다.</returns>
        public static DbConnection CreateTransactionScopeConnection(Database db) {
            db.ShouldNotBeNull("db");

            var newConnectionCreated = false;
            return CreateTransactionScopeConnection(db, ref newConnectionCreated);
        }

        /// <summary>
        /// 현재 Transaction에 해당하는 Connection을 반환한다.
        /// 이미 열려진 Connection을 재활용하거나, TransactionScope가 활성화되어 있다면 새로운 connection을 빌드해서 반환한다.
        /// TransactionScope도 없고, <see cref="Database"/>관련해서 아직 만들어진게 없다면, 새로운 <see cref="DbConnection"/>을 생성하고, Open() 해서 반환한다.
        /// </summary>
        /// <param name="db">instance of <see cref="Database"/></param>
        /// <param name="newConnectionCreated">새로 Connection이 만들어졌는지</param>
        /// <param name="connectionFactory">Connection을 새로 만드는 메소드</param>
        /// <returns>
        /// TransactionScope이 활성화되어 있다면, TransactionScope에서 활성화된 <see cref="DbConnection"/>을 반환,
        /// TransactionScope가 없다면 새로운 <see cref="DbConnection"/>을 생성하여 반환한다.</returns>
        public static DbConnection CreateTransactionScopeConnection(Database db, ref bool newConnectionCreated,
                                                                    Func<Database, DbConnection> connectionFactory = null) {
            db.ShouldNotBeNull("db");
            connectionFactory = connectionFactory ?? ((database) => OpenConnection(database));

            // newConnectionCreated = false;
            DbConnection connection;

            // TransactionScope에 참여한 Connection이 있다면 그 Connection을 반환합니다.
            try {
                connection = TransactionScopeConnections.GetConnection(db);
            }
            catch(Exception ex) {
                if(log.IsWarnEnabled)
                    log.WarnException("TransactionScopeConnections.GetConnection() 수행 시에 예외가 발생했습니다. 무시하고 새로운 connection을 생성합니다.", ex);

                connection = null;
            }

            // 새롭게 Connection을 빌드합니다.
            if(connection == null) {
                if(IsDebugEnabled)
                    log.Debug("TransactionScope에 속한 connection이 없습니다. 새로운 connection을 생성하고, Open합니다...");

                connection = connectionFactory(db);
                newConnectionCreated = (connection != null);

                if(IsDebugEnabled)
                    log.Debug("새로운 connection을 생성하고, Open했습니다. newConnectionCreated=[{0}]", newConnectionCreated);
            }

            return connection;
        }

        /// <summary>
        /// DAAB Database에서 새로운 Connection을 생성하고 Open 합니다.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tryCount"></param>
        /// <returns></returns>
        private static DbConnection OpenConnection(Database db, int tryCount = 5) {
            DbConnection connection = null;

            for(var i = 0; i < tryCount; i++) {
                try {
                    connection = db.CreateConnection();
                    connection.Open();

                    if(connection.State == ConnectionState.Open)
                        return connection;
                }
                catch(Exception ex) {
                    if(log.IsErrorEnabled) {
                        log.Error("Database Connection 생성 및 Open 수행 시에 예외가 발생했습니다. 시도횟수=[{0}], ConnectionString=[{1}]", i + 1,
                                  db.ConnectionString);
                        log.Error(ex);
                    }
                    Thread.Sleep(5 * (i + 1));
                }
            }

            Guard.Assert(connection != null && connection.State != ConnectionState.Closed, "Connection을 생성하지 못했습니다!!!");
            return connection;
        }

        /// <summary>
        /// <paramref name="command"/>의 Connnection 을 닫습니다.
        /// </summary>
        /// <param name="command"></param>
        public static void ForceCloseConnection(DbCommand command) {
            if(command == null || command.Connection == null)
                return;

            if(IsDebugEnabled)
                log.Debug("Command의 Connection 닫기를 시도합니다...");

            if(command.Connection.State == ConnectionState.Closed) {
                command.Connection = null;
                return;
            }

            try {
                command.Connection.Close();
                command.Connection = null;

                if(IsDebugEnabled)
                    log.Debug("Command의 Connection을 닫는데 성공했습니다!!!");
            }
            catch(Exception ex) {
                if(log.IsWarnEnabled)
                    log.WarnException("Command의 Connection을 닫는 과정에서 예외가 발생했습니다. 하지만 무시합니다^^", ex);
            }
        }

        /// <summary>
        /// 기본 Database의 Connection Pool을 제거합니다.
        /// </summary>
        public static void ClearAllConnectionPools() {
            ClearAllConnectionPools(DefaultDatabaseName);
        }

        /// <summary>
        /// 지정된 Database의 Connection Pool을 모두 제거합니다.
        /// </summary>
        /// <param name="databaseName"></param>
        public static void ClearAllConnectionPools(string databaseName) {
            var connSettings = ConfigTool.GetConnectionSettings(databaseName ?? DefaultDatabaseName);
            connSettings.ShouldNotBeNull("connSettings");

            ClearConnectionPoolInternal(connSettings.ProviderName);
        }

        /// <summary>
        /// 환경설정에 정의된 모든 Database Connection 에 해당하는 ConnectionPool을 제거합니다.
        /// </summary>
        public static void ClearAllConnectionPoolsFromConfiguration() {
            foreach(var connSettings in ConfigTool.GetConnectionStringSettings())
                ClearConnectionPoolInternal(connSettings.ProviderName);
        }

        private static void ClearConnectionPoolInternal(string providerName) {
            if(providerName.IsWhiteSpace())
                return;

            if(IsDebugEnabled)
                log.Debug("해당 ConnectionPool을 제거합니다... providerName=[{0}]", providerName);

#pragma warning disable 0618
            if(providerName.Contains("SqlClient"))
                With.TryAction(SqlConnection.ClearAllPools);
            //else if(providerName.Contains("Oracle") && !providerName.Contains("Devart"))
            //	With.TryAction(OracleConnection.ClearAllPools);
#pragma warning restore 0618
        }
    }
}