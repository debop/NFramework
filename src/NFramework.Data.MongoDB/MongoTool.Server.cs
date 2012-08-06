using System;
using MongoDB.Driver;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Data.MongoDB {
    public static partial class MongoTool {
        /// <summary>
        /// MongoDB 서버에 접속하기 위한 <see cref="MongoConnectionStringBuilder"/>를 빌드 합니다.
        /// </summary>
        /// <returns></returns>
        public static MongoConnectionStringBuilder GetMongoConnectionBuilder() {
            return GetMongoConnectionBuilder(DefaultConnectionString, DefaultDatabaseName);
        }

        /// <summary>
        /// MongoDB 서버에 접속하기 위한 <see cref="MongoConnectionStringBuilder"/>를 빌드 합니다.
        /// </summary>
        /// <param name="serverName">Mongo Server IP 또는 DNS 명</param>
        /// <returns></returns>
        public static MongoConnectionStringBuilder GetMongoConnectionBuilder(string serverName) {
            return GetMongoConnectionBuilder(serverName, DefaultDatabaseName);
        }

        /// <summary>
        /// MongoDB 서버에 접속하기 위한 <see cref="MongoConnectionStringBuilder"/>를 빌드 합니다.
        /// </summary>
        /// <param name="serverName">Mongo Server IP 또는 DNS 명</param>
        /// <param name="databaseName">MongoDB의 database 명</param>
        /// <returns></returns>
        public static MongoConnectionStringBuilder GetMongoConnectionBuilder(string serverName, string databaseName) {
            return GetMongoConnectionBuilder(serverName, null, databaseName, null, null);
        }

        /// <summary>
        /// MongoDB 서버에 접속하기 위한 <see cref="MongoConnectionStringBuilder"/>를 빌드 합니다.
        /// </summary>
        /// <param name="serverName">Mongo Server IP 또는 DNS 명</param>
        /// <param name="port">Port</param>
        /// <param name="databaseName">MongoDB의 database 명</param>
        /// <param name="username">사용자명</param>
        /// <param name="password">비밀번호</param>
        /// <returns><see cref="MongoConnectionStringBuilder"/> 인스턴스</returns>
        public static MongoConnectionStringBuilder GetMongoConnectionBuilder(string serverName, int? port, string databaseName,
                                                                             string username, string password) {
            serverName.ShouldNotBeWhiteSpace("serverName");

            var builder = new MongoConnectionStringBuilder
                          {
                              Server = port.HasValue
                                           ? new MongoServerAddress(serverName, port.Value)
                                           : new MongoServerAddress(serverName),
                              SafeMode = SafeMode.True,
                              DatabaseName = databaseName ?? DefaultDatabaseName
                          };


            if(username.IsNotWhiteSpace())
                builder.Username = username;

            if(password.IsNotWhiteSpace())
                builder.Password = password;

            return builder;
        }

        /// <summary>
        /// MongoServer의 <see cref="DefaultConnectionString"/>의 <see cref="DefaultDatabaseName"/> 연결합니다.
        /// </summary>
        /// <returns><see cref="MongoServer"/> 인스턴스</returns>
        public static MongoServer CreateMongoServer() {
            return CreateMongoServer(DefaultConnectionString);
        }

        /// <summary>
        /// MongoServer의 <see cref="DefaultDatabaseName"/>에 연결합니다.
        /// </summary>
        /// <param name="connectionString">서버 IP 또는 DNS 명</param>
        /// <returns><see cref="MongoServer"/> 인스턴스</returns>
        public static MongoServer CreateMongoServer(string connectionString) {
            return CreateMongoServer(new MongoConnectionStringBuilder(connectionString));
        }

        /// <summary>
        /// <paramref name="connectionBuilder"/>를 이용하여, <see cref="MongoServer"/>를 생성하고, 연결합니다.
        /// </summary>
        /// <param name="connectionBuilder">MongoDB ConnectionString Builder</param>
        /// <returns><see cref="MongoServer"/> 인스턴스</returns>
        public static MongoServer CreateMongoServer(this MongoConnectionStringBuilder connectionBuilder) {
            connectionBuilder.ShouldNotBeNull("connectionBuilder");

            if(IsDebugEnabled)
                log.Debug("MongoServer를 생성합니다. connectionString=[{0}]", connectionBuilder.ConnectionString);

            var server = MongoServer.Create(connectionBuilder);
            server.Connect();

            return server;
        }

        /// <summary>
        /// 서버의 가장 최근의 예외정보를 반환합니다.
        /// </summary>
        /// <param name="server"></param>
        /// <returns></returns>
        public static GetLastErrorResult GetLastError(this MongoServer server) {
            return server.GetLastError();
        }

        /// <summary>
        /// MongoDB 서버 <paramref name="serverName"/>의 Database (<paramref name="databaseName"/>)를 삭제합니다.
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        public static CommandResult DropDatabase(string serverName, string databaseName) {
            MongoServer server = null;

            try {
                server = CreateMongoServer(serverName);
                var result = server.DropDatabase(databaseName);

                return result;
            }
            catch(Exception ex) {
                if(log.IsWarnEnabled) {
                    log.Warn("Database[{0}] 삭제에 실패했습니다.", databaseName);
                    log.Warn(ex);
                }
            }
            finally {
                if(server != null)
                    server.Disconnect();
            }

            return EmptyCommandResult;
        }
    }
}