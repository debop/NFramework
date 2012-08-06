using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Tool.hbm2ddl;
using NSoft.NFramework.IO;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Data.NHibernateEx.ForTesting {
    /// <summary>
    /// A strategy class that parameterizes a <see cref="UnitOfWorkTestContext"/> with database specific implementations
    /// </summary>
    /// <remarks>
    /// This class is a companion to <see cref="UnitOfWorkTestContext"/>. Its
    /// purpose is to encapsulate behind a common interface the database
    /// specific implementations of behaviour required to construct and manage
    /// the test context
    /// </remarks>
    public abstract class UnitOfWorkTestContextDbStrategy {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// Castle ProxyFactory의 Type FullName
        /// </summary>
        public const string ProxyFactoryCastle = @"NHibernate.ByteCode.Castle.ProxyFactoryFactory, NHibernate.ByteCode.Castle";

        /// <summary>
        /// 테스트시에는 기본적으로 SQLite 메모리 DB를 사용한다. SQLite 메모리 DB 명
        /// </summary>
        public const string SQLiteDbName = ":memory:";

        /// <summary>
        /// Creates the physical database named <paramref name="databaseName"/>.
        /// </summary>
        /// <remarks>
        /// Use this method to create the physical database file. 
        /// <para>
        /// For MsSqlCe this will create a database file in the file system
        /// named <paramref name="databaseName"/>.sdf
        /// </para>
        /// <para>
        /// For MsSql2005 this will create a database named <paramref
        /// name="databaseName"/> in the (local) instance of Sql Server 2005 on
        /// this machine
        /// </para>
        /// </remarks>
        public static void CreatePhysicalDatabaseMediaFor(DatabaseEngine databaseEngine, string databaseName) {
            For(databaseEngine, databaseName, null).CreateDatabaseMedia();
        }

        /// <summary>
        /// 특정 Database System 종류에 대한 테스트용 DB 생성 및 UnitOfWork의 환경설정 정보를 제공하는 
        /// UnitOfWorkTestContextDbStrategy의 인스턴스를 빌드한다.
        /// </summary>
        /// <param name="databaseEngine">Database system 종류</param>
        /// <param name="databaseName">테스트용 Database name</param>
        /// <param name="properties">NHibernate configuration properties</param>
        /// <returns></returns>
        public static UnitOfWorkTestContextDbStrategy For(DatabaseEngine databaseEngine, string databaseName,
                                                          IDictionary<string, string> properties) {
            UnitOfWorkTestContextDbStrategy strategy;

            switch(databaseEngine) {
                case DatabaseEngine.SQLite:
                    strategy = new SQLiteUnitOfWorkTestContextDbStrategy();
                    break;

                case DatabaseEngine.SQLiteForFile:
                    strategy = new SQLiteForFileUnitOfWorkTestContextDbStrategy(databaseName);
                    break;

                case DatabaseEngine.MsSqlCe:
                    strategy = new MsSqlCeUnitOfWorkTestContextDbStrategy(databaseName);
                    break;

                case DatabaseEngine.MsSqlCe40:
                    strategy = new MsSqlCe40UnitOfWorkTestContextDbStrategy(databaseName);
                    break;

                case DatabaseEngine.MsSql2005:
                    strategy = new MsSql2005UnitOfWorkTestContextDbStrategy(databaseName);
                    break;

                case DatabaseEngine.MsSql2005Express:
                    strategy = new MsSql2005ExpressUnitOfWorkTestContextDbStrategy(databaseName);
                    break;

                case DatabaseEngine.OdpNet:
                    strategy = new OdpNetUnitOfWorkTestContextDbStrategy(databaseName);
                    break;

                case DatabaseEngine.DevartOracle:
                    strategy = new DevartOdpNetUnitOfWorkTestContextDbStrategy(databaseName);
                    break;

                case DatabaseEngine.MySql:
                    strategy = new MySqlUnitOfWorkTestContextDbStrategy(databaseName);
                    break;

                case DatabaseEngine.PostgreSql:
                    strategy = new PostgreSqlUnitOfWorkTestContextDbStrategy(databaseName);
                    break;

                case DatabaseEngine.Firebird:
                    strategy = new FireBirdUnitOfWorkTestContextDbStrategy(databaseName);
                    break;

                case DatabaseEngine.Cubrid:
                    strategy = new CubridUnitOfWorkTestContextDbStrategy(databaseName);
                    break;

                default:
                    throw new NotSupportedException("currently not support a specified database engine=" + databaseEngine);
            }

            if(properties != null)
                foreach(var property in properties)
                    strategy.NHibernateProperties[property.Key] = property.Value;

            return strategy;
        }

        /// <summary>
        /// 특정 Database System 종류에 대한 테스트용 DB 생성 및 UnitOfWork의 환경설정 정보를 제공하는 
        /// UnitOfWorkTestContextDbStrategy의 인스턴스를 빌드한다.
        /// </summary>
        /// <param name="databaseEngine">Database system 종류</param>
        /// <param name="databaseName">테스트용 Database name</param>
        /// <returns></returns>
        public static UnitOfWorkTestContextDbStrategy For(DatabaseEngine databaseEngine, string databaseName) {
            return For(databaseEngine, databaseName, null);
        }

        /// <summary>
        /// 로컬 컴퓨터에 Microsoft SQL Server 2005 Or Higher version이 설치되어 있는지 검사한다.
        /// </summary>
        /// <returns></returns>
        public static bool IsSqlServer2005OrAboveInstalled() {
            const string sqlServerCurrent = @"SOFTWARE\Microsoft\MSSQLServer\MSSQLServer\CurrentVersion";
            var regKey = Registry.LocalMachine.OpenSubKey(sqlServerCurrent);

            if(regKey == null)
                return false;

            string currentVersion = (string)regKey.GetValue("CurrentVersion");
            var versionNumbers = currentVersion.Split('.');

            return Int32.Parse(versionNumbers[0]) >= 9;
        }

        private readonly string _databaseName;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="databaseName">ConnectionString에서 Initial Catalog에 해당되는 Database명</param>
        protected UnitOfWorkTestContextDbStrategy(string databaseName) {
            _databaseName = databaseName;

            // added by NHibernate 2.1.0 
            // ProxyFactory 
            // properties.Add(NHibernate.Cfg.Environment.ProxyFactoryFactoryClass, ProxyFactoryCastle);
            //_properties.Add(Environment.ProxyFactoryFactoryClass, ProxyFactoryLinFu);

            // NOTE : AST Query Translator를 이용하면 delete / insert 등을 사용할 수 있다. (예: session.CreateQuery("delete Parent").ExecuteUpdate())
            // NOTE : DML-Style HQL을 직접사용할 수 있으므로 성능이 좋다.
            _properties.Add(NHibernate.Cfg.Environment.QueryTranslator, "NHibernate.Hql.Ast.ANTLR.ASTQueryTranslatorFactory, NHibernate");
            _properties.Add(NHibernate.Cfg.Environment.FormatSql, "true");

            // NOTE : SQLServer 에서는 꼭 (hbm2ddl.keywords = none)으로 해줘야 한다.
            // 참고 : http://fabiomaulo.blogspot.com/2009/06/auto-quote-tablecolumn-names.html 
            // none을 지정하지 않으면 cfg.BuildSessionFactory() 에 DB가 만들어지지 않았는데도 Database에 connection을 생성하려고한다.
            _properties.Add(NHibernate.Cfg.Environment.Hbm2ddlKeyWords, "none");
        }

        /// <summary>
        /// Database Engine
        /// </summary>
        public abstract DatabaseEngine DatabaseEngine { get; }

        /// <summary>
        /// 테스트용 Database 명
        /// </summary>
        public string DatabaseName {
            get { return _databaseName; }
        }

        private readonly IDictionary<string, string> _properties = new Dictionary<string, string>();

        /// <summary>
        /// NHibernate 설정 정보
        /// </summary>
        public IDictionary<string, string> NHibernateProperties {
            get { return _properties; }
        }

        /// <summary>
        /// UnitOfWork Context for Testing
        /// </summary>
        public UnitOfWorkTestContext TestContext { get; set; }

        /// <summary>
        /// Create new session in testing context
        /// </summary>
        /// <returns></returns>
        public virtual ISession CreateSession() {
            var interceptor = NHIoC.ResolveInterceptor();

            return interceptor != null
                       ? TestContext.SessionFactory.OpenSession(interceptor)
                       : TestContext.SessionFactory.OpenSession();
        }

        /// <summary>
        /// Setup database ( create database file, create database schema )
        /// </summary>
        /// <param name="currentSession"></param>
        public virtual void SetUpDatabase(ISession currentSession) {
            // CreateDatabaseMedia();
            CreateDatabaseSchema(currentSession ?? CreateSession());
        }

        /// <summary>
        /// create database media (file)
        /// </summary>
        internal abstract void CreateDatabaseMedia();

        /// <summary>
        /// create database schema for current session
        /// </summary>
        /// <param name="currentSession"></param>
        /// <seealso cref="SchemaExport"/>
        protected virtual void CreateDatabaseSchema(ISession currentSession) {
            new SchemaExport(TestContext.NHConfiguration).Execute(false, true, false);

            //var schemaExport = new SchemaExport(TestContext.NHConfiguration);
            //schemaExport.Drop(false, true);
            //schemaExport.Create(false, true);
        }

        #region << SQLite >>

        /// <summary>
        /// SQLite를 이용한 Testing시에 사용되는 DbStrategy
        /// </summary>
        private class SQLiteUnitOfWorkTestContextDbStrategy : UnitOfWorkTestContextDbStrategy {
            /// <summary>
            /// Constructor
            /// </summary>
            public SQLiteUnitOfWorkTestContextDbStrategy()
                : base(SQLiteDbName) {
                _properties.AddValue(NHibernate.Cfg.Environment.ConnectionDriver, "NHibernate.Driver.SQLite20Driver");
                _properties.AddValue(NHibernate.Cfg.Environment.Dialect, "NHibernate.Dialect.SQLiteDialect");
                _properties.AddValue(NHibernate.Cfg.Environment.ConnectionProvider, "NHibernate.Connection.DriverConnectionProvider");

                // HINT : 이걸 넣어야 돼 말아야 돼? "Pooling=True;Max Pool Size=1;"
                string connectionString = string.Format("Data Source={0};Version=3;New=True;", DatabaseName);

                _properties.AddValue(NHibernate.Cfg.Environment.ConnectionString, connectionString);
                _properties.AddValue(NHibernate.Cfg.Environment.ShowSql, "true");
                _properties.AddValue(NHibernate.Cfg.Environment.ReleaseConnections, "on_close");
                _properties.AddValue(NHibernate.Cfg.Environment.BatchSize, "30");
                _properties.AddValue(NHibernate.Cfg.Environment.WrapResultSets, "false");
            }

            /// <summary>
            /// Database Engine
            /// </summary>
            public override DatabaseEngine DatabaseEngine {
                get { return DatabaseEngine.SQLite; }
            }

            /// <summary>
            /// create database media (file)
            /// </summary>
            internal override void CreateDatabaseMedia() {
                // nothing to do. because using SQLite memory db.
            }

            /// <summary>
            /// create database schema for current session
            /// </summary>
            /// <param name="currentSession"></param>
            /// <seealso cref="SchemaExport"/>
            protected override void CreateDatabaseSchema(ISession currentSession) {
                new SchemaExport(TestContext.NHConfiguration).Execute(false, true, false, currentSession.Connection, null);
            }

            /// <summary>
            /// Create new session in testing context
            /// </summary>
            /// <returns></returns>
            public override ISession CreateSession() {
                //need to get our own connection, because NH will try to close it as soon as possible, and we will lose the changes.
                var connection
                    = ((ISessionFactoryImplementor)TestContext.SessionFactory).ConnectionProvider.GetConnection();

                // IInterceptor interceptor = UnitOfWorkTestContext.ResolveInterceptor();
                var interceptor = NHIoC.ResolveInterceptor();

                if(interceptor != null)
                    TestContext.SessionFactory.OpenSession(connection, interceptor);

                return TestContext.SessionFactory.OpenSession(connection);
            }
        }

        #endregion

        #region << SQLiteForFile >>

        private class SQLiteForFileUnitOfWorkTestContextDbStrategy : UnitOfWorkTestContextDbStrategy {
            /// <summary>
            /// Constructor
            /// </summary>
            public SQLiteForFileUnitOfWorkTestContextDbStrategy(string dbName)
                : base(dbName) {
                _properties.AddValue(NHibernate.Cfg.Environment.ConnectionDriver, "NHibernate.Driver.SQLite20Driver");
                _properties.AddValue(NHibernate.Cfg.Environment.Dialect, "NHibernate.Dialect.SQLiteDialect");
                _properties.AddValue(NHibernate.Cfg.Environment.ConnectionProvider, "NHibernate.Connection.DriverConnectionProvider");

                // HINT : 이걸 넣어야 돼 말아야 돼? "Pooling=True;Max Pool Size=1;"
                string connectionString = string.Format("Data Source={0};Version=3;Pooling=True;Max Pool Size=100;",
                                                        GetDatabaseName(DatabaseName));

                _properties.AddValue(NHibernate.Cfg.Environment.ConnectionString, connectionString);
                _properties.AddValue(NHibernate.Cfg.Environment.ShowSql, "true");
                _properties.AddValue(NHibernate.Cfg.Environment.ReleaseConnections, "on_close");

                _properties.AddValue(NHibernate.Cfg.Environment.BatchSize, "30");
                _properties.AddValue(NHibernate.Cfg.Environment.WrapResultSets, "false");
            }

            /// <summary>
            /// Database Engine
            /// </summary>
            public override DatabaseEngine DatabaseEngine {
                get { return DatabaseEngine.SQLiteForFile; }
            }

            private static string GetDatabaseName(string databaseName) {
                return databaseName.EndsWith(".db") ? databaseName : string.Concat(databaseName, ".db");
            }

            /// <summary>
            /// create database media (file)
            /// </summary>
            internal override void CreateDatabaseMedia() {
                // SQLite는 Database 파일을 자동으로 생성해준다.
            }
        }

        #endregion

        #region << MsSqlCe >>

        /// <summary>
        /// Microsoft Sql CE 를 이용한 UnitOfWork 의 Testing Context
        /// </summary>
        private class MsSqlCeUnitOfWorkTestContextDbStrategy : UnitOfWorkTestContextDbStrategy {
            public MsSqlCeUnitOfWorkTestContextDbStrategy(string dbName)
                : base(dbName) {
                // NOTE: SQL CE에서 AnsiString도 가능하도록 하기 위해서!!! (근데 안된다...)
                // _properties.AddValue(Environment.ConnectionDriver, "NSoft.NFramework.Data.NH.SqlServerCeDriver, NSoft.NFramework.Data");
                // _properties.AddValue(Environment.Dialect, "NSoft.NFramework.Data.NH.MsSqlCeDialect, NSoft.NFramework.Data");

                _properties.AddValue(NHibernate.Cfg.Environment.ConnectionDriver, "NHibernate.Driver.SqlServerCeDriver");
                _properties.AddValue(NHibernate.Cfg.Environment.Dialect, "NHibernate.Dialect.MsSqlCeDialect");
                _properties.AddValue(NHibernate.Cfg.Environment.ConnectionProvider, "NHibernate.Connection.DriverConnectionProvider");

                string connectionString = string.Format("Data Source={0};", GetDatabaseName(DatabaseName));

                _properties.AddValue(NHibernate.Cfg.Environment.ConnectionString, connectionString);
                _properties.AddValue(NHibernate.Cfg.Environment.ShowSql, "True");
                _properties.AddValue(NHibernate.Cfg.Environment.ReleaseConnections, "on_close");

                _properties.AddValue(NHibernate.Cfg.Environment.BatchSize, "30");
                _properties.AddValue(NHibernate.Cfg.Environment.WrapResultSets, "false");
            }

            /// <summary>
            /// Database Engine
            /// </summary>
            public override DatabaseEngine DatabaseEngine {
                get { return DatabaseEngine.MsSqlCe; }
            }

            private static string GetDatabaseName(string databaseName) {
                return databaseName.EndsWith(".sdf") ? databaseName : string.Concat(databaseName, ".sdf");
            }

            /// <summary>
            /// create database media (file)
            /// </summary>
            internal override void CreateDatabaseMedia() {
                SqlCeTool.CreateDatabaseFile(GetDatabaseName(DatabaseName));
            }
        }

        #endregion

        #region << MsSqlCe40 >>

        private class MsSqlCe40UnitOfWorkTestContextDbStrategy : MsSqlCeUnitOfWorkTestContextDbStrategy {
            public MsSqlCe40UnitOfWorkTestContextDbStrategy(string dbName)
                : base(dbName) {
                _properties.Remove(NHibernate.Cfg.Environment.Dialect);
                _properties.Add(NHibernate.Cfg.Environment.Dialect, "NHibernate.Dialect.MsSqlCe40Dialect");
            }

            /// <summary>
            /// Database Engine
            /// </summary>
            public override DatabaseEngine DatabaseEngine {
                get { return DatabaseEngine.MsSqlCe40; }
            }
        }

        #endregion

        #region << MsSql2005 >>

        /// <summary>
        /// Microsoft SQL Server 2005를 사용하여 UnitOfWork를 테스트 할 때 사용할 전략
        /// </summary>
        private class MsSql2005UnitOfWorkTestContextDbStrategy : UnitOfWorkTestContextDbStrategy {
            /// <summary>
            /// 생성자
            /// </summary>
            /// <param name="dbName"></param>
            public MsSql2005UnitOfWorkTestContextDbStrategy(string dbName)
                : base(dbName) {
                _properties.AddValue(NHibernate.Cfg.Environment.ConnectionDriver, "NHibernate.Driver.SqlClientDriver");
                _properties.AddValue(NHibernate.Cfg.Environment.Dialect, "NHibernate.Dialect.MsSql2005Dialect");
                _properties.AddValue(NHibernate.Cfg.Environment.ConnectionProvider, "NHibernate.Connection.DriverConnectionProvider");
                _properties.AddValue(NHibernate.Cfg.Environment.ConnectionString, ConnectionStringFor(DatabaseName));
                _properties.AddValue(NHibernate.Cfg.Environment.PrepareSql, "false");
                _properties.AddValue(NHibernate.Cfg.Environment.ShowSql, "true");
                _properties.AddValue(NHibernate.Cfg.Environment.ReleaseConnections, "on_close");

                //by specifying a default schema, nhibernate's dynamic sql queries benefit from caching
                //
                _properties.AddValue(NHibernate.Cfg.Environment.DefaultSchema, string.Format("{0}.dbo", DatabaseName));
                _properties.AddValue(NHibernate.Cfg.Environment.BatchSize, "30");
                _properties.AddValue(NHibernate.Cfg.Environment.WrapResultSets, "false");
            }

            public override DatabaseEngine DatabaseEngine {
                get { return DatabaseEngine.MsSql2005; }
            }

            /// <summary>
            /// create database media (file)
            /// </summary>
            internal override void CreateDatabaseMedia() {
                string sqlServerDataDirectory = GetSqlServerDataDirectory();
                var scripts = new StringBuilder();

                scripts
                    .AppendFormat("IF (SELECT DB_ID('{0}')) IS NULL", DatabaseName).AppendLine()
                    .AppendFormat("  CREATE DATABASE {0} ON PRIMARY", DatabaseName).AppendLine()
                    .AppendFormat("    (NAME={0}_Data, FILENAME='{1}.mdf', SIZE=16MB, FILEGROWTH=16MB)", DatabaseName,
                                  sqlServerDataDirectory + DatabaseName).AppendLine()
                    .AppendFormat("  LOG ON (NAME={0}_Log, FILENAME='{1}.ldf', SIZE=8MB, FILEGROWTH=8MB)", DatabaseName,
                                  sqlServerDataDirectory + DatabaseName).AppendLine();

                //string createDatabaseScript = "IF (SELECT DB_ID('" + DatabaseName + "')) IS NULL  "
                //                              + " CREATE DATABASE " + DatabaseName
                //                              + " ON PRIMARY "
                //                              + " (NAME = " + DatabaseName + "_Data, "
                //                              + @" FILENAME = '" + sqlServerDataDirectory + DatabaseName + ".mdf', "
                //                              + " SIZE = 5MB,"
                //                              + " FILEGROWTH =" + 10 + ") "
                //                              + " LOG ON (NAME =" + DatabaseName + "_Log, "
                //                              + @" FILENAME = '" + sqlServerDataDirectory + DatabaseName + ".ldf', "
                //                              + " SIZE = 1MB, "
                //                              + " FILEGROWTH =" + 5 + ")";

                if(log.IsInfoEnabled) {
                    log.Info("Create Database... Database name:" + DatabaseName);
                    log.Info("SQL Scripts:" + System.Environment.NewLine + scripts.ToString());
                }

                ExecuteDbScript(scripts.ToString(), ConnectionStringFor("master"));

                if(log.IsInfoEnabled)
                    log.Info("Create Database is SUCCESS.!!! Database name:" + DatabaseName);
            }

            /// <summary>
            /// 지정된 database명으로 connection string을 만든다.
            /// </summary>
            /// <param name="databaseName"></param>
            /// <returns></returns>
            protected virtual string ConnectionStringFor(string databaseName) {
                if(databaseName.Contains(";"))
                    return databaseName;

                var connStr = ConfigTool.GetConnectionString(databaseName);
                if(connStr.IsNotWhiteSpace())
                    return connStr;

                return string.Format("Server=(local);initial catalog={0};Integrated Security=SSPI;Asynchronous Processing=true;",
                                     databaseName);
            }

            /// <summary>
            /// 지정된 SQL Script를 실행한다.
            /// </summary>
            /// <param name="sqlScript">실행할 sql script</param>
            /// <param name="connectionString">대상 DB 서버에 대한 Connection String</param>
            private static void ExecuteDbScript(string sqlScript, string connectionString) {
                using(var conn = new SqlConnection(connectionString)) {
                    using(var command = new SqlCommand(sqlScript, conn)) {
                        conn.Open();

                        Task<int>.Factory
                            .FromAsync(command.BeginExecuteNonQuery,
                                       command.EndExecuteNonQuery,
                                       null,
                                       TaskCreationOptions.None)
                            .Wait();
                    }
                }
            }

            /// <summary>
            /// SQL 서버의 기본 Data Directory 얻기 (Registry에 정의되어 있음)
            /// </summary>
            /// <returns></returns>
            protected virtual string GetSqlServerDataDirectory() {
                return SQLServerSetUpTool.GetSqlServerDataDirectory();
            }
        }

        #endregion

        #region << MsSql2005 Express >>

        /// <summary>
        /// Microsoft SQL Server Express를 이용한 UnitOfWork Test위한 사전 준비를 구현한 Class
        /// </summary>
        private class MsSql2005ExpressUnitOfWorkTestContextDbStrategy : MsSql2005UnitOfWorkTestContextDbStrategy {
            /// <summary>
            /// 생성자
            /// </summary>
            /// <param name="databaseName">Database Name</param>
            public MsSql2005ExpressUnitOfWorkTestContextDbStrategy(string databaseName)
                : base(databaseName) {}

            /// <summary>
            /// Database Engine
            /// </summary>
            public override DatabaseEngine DatabaseEngine {
                get { return DatabaseEngine.MsSql2005Express; }
            }

            /// <summary>
            /// 지정된 database명으로 connection string을 만든다.
            /// </summary>
            /// <param name="databaseName"></param>
            /// <returns></returns>
            protected override string ConnectionStringFor(string databaseName) {
                return
                    string.Format(
                        @"Server=(local)\SQLEXPRESS;initial catalog={0};Integrated Security=SSPI;Asynchronous Processing=true;",
                        databaseName);
            }

            /// <summary>
            /// SQL 서버의 기본 Data Directory 얻기 (Registry에 정의되어 있음)
            /// </summary>
            /// <returns></returns>
            protected override string GetSqlServerDataDirectory() {
                return SQLServerSetUpTool.GetSqlExpressDataDirectory();
            }
        }

        #endregion

        #region << ODP.NET >>

        private class OdpNetUnitOfWorkTestContextDbStrategy : UnitOfWorkTestContextDbStrategy {
            public OdpNetUnitOfWorkTestContextDbStrategy(string dbName)
                : base(dbName) {
                _properties.AddValue(NHibernate.Cfg.Environment.ConnectionDriver, "NHibernate.Driver.OracleDataClientDriver");
                _properties.AddValue(NHibernate.Cfg.Environment.Dialect, "NHibernate.Dialect.Oracle10gDialect");
                _properties.AddValue(NHibernate.Cfg.Environment.ConnectionProvider, "NHibernate.Connection.DriverConnectionProvider");
                _properties.AddValue(NHibernate.Cfg.Environment.ConnectionString, ConnectionStringFor(DatabaseName));
                _properties.AddValue(NHibernate.Cfg.Environment.PrepareSql, "false");
                _properties.AddValue(NHibernate.Cfg.Environment.ShowSql, "true");

                //! NOTE : Oracle에서는 Ado BatchSize를 주면 안됩니다!!!
                _properties.AddValue(NHibernate.Cfg.Environment.BatchSize, "0");
                // _properties.Add(NHibernate.Cfg.Environment.WrapResultSets, "false");
            }

            public override DatabaseEngine DatabaseEngine {
                get { return DatabaseEngine.OdpNet; }
            }

            internal override void CreateDatabaseMedia() {
                // Nothing to do.
            }

            protected virtual string ConnectionStringFor(string databaseName) {
                if(databaseName.Contains(";"))
                    return databaseName;

                var connStr = ConfigTool.GetConnectionString(databaseName);
                if(connStr.IsNotWhiteSpace())
                    return connStr;

                throw new NotSupportedException(string.Format("제공된 Database Name은 사용할 수 없습니다. databaseName=[{0}]", databaseName));
            }
        }

        #endregion

        #region << DevartOracle >>

        private class DevartOdpNetUnitOfWorkTestContextDbStrategy : OdpNetUnitOfWorkTestContextDbStrategy {
            public DevartOdpNetUnitOfWorkTestContextDbStrategy(string dbName)
                : base(dbName) {
                // NOTE: NHibernate용 Devart사의 Oracle Driver 는 NSoft.NFramework.Data.DevartOracle.dll 에 정의되어 있습니다!!!
                //
                _properties.AddValue(NHibernate.Cfg.Environment.ConnectionDriver,
                                     @"NHibernate.Driver.DevartOracleDriver, NSoft.NFramework.Data.NHibernateEx");
                _properties.AddValue(NHibernate.Cfg.Environment.Dialect, @"NHibernate.Dialect.Oracle10gDialect");

                //! NOTE: Devart에서는 PrepareSql 을 항상 false 로 지정해야 합니다.
                _properties.AddValue(NHibernate.Cfg.Environment.PrepareSql, "false");

                //! NOTE : Oracle에서는 Ado BatchSize를 주면 안됩니다!!!
                _properties.AddValue(NHibernate.Cfg.Environment.BatchSize, "0");
            }

            public override DatabaseEngine DatabaseEngine {
                get { return DatabaseEngine.DevartOracle; }
            }
        }

        #endregion

        #region << MySql >>

        private sealed class MySqlUnitOfWorkTestContextDbStrategy : UnitOfWorkTestContextDbStrategy {
            public MySqlUnitOfWorkTestContextDbStrategy(string dbName)
                : base(dbName) {
                _properties.AddValue(NHibernate.Cfg.Environment.ConnectionDriver, "NHibernate.Driver.MySqlDataDriver");
                _properties.AddValue(NHibernate.Cfg.Environment.Dialect, "NHibernate.Dialect.MySQL5Dialect");
                _properties.AddValue(NHibernate.Cfg.Environment.ConnectionProvider, "NHibernate.Connection.DriverConnectionProvider");
                _properties.AddValue(NHibernate.Cfg.Environment.ConnectionString, ConnectionStringFor(DatabaseName));
                _properties.AddValue(NHibernate.Cfg.Environment.PrepareSql, "false");
                _properties.AddValue(NHibernate.Cfg.Environment.ShowSql, "true");

                _properties.AddValue(NHibernate.Cfg.Environment.BatchSize, "30");
                _properties.Add(NHibernate.Cfg.Environment.WrapResultSets, "false");
            }

            public override DatabaseEngine DatabaseEngine {
                get { return DatabaseEngine.MySql; }
            }

            internal override void CreateDatabaseMedia() {
                // Nothing to do.
            }

            private string ConnectionStringFor(string databaseName) {
                if(databaseName.Contains(";"))
                    return databaseName;

                var connStr = ConfigTool.GetConnectionString(databaseName);
                if(connStr.IsNotWhiteSpace())
                    return connStr;

                return string.Format("Server=localhost;database={0};Integrated Security=SSPI;Max PoolSize=500;old guids=true;",
                                     databaseName);
            }
        }

        #endregion

        #region << PostgreSql >>

        private sealed class PostgreSqlUnitOfWorkTestContextDbStrategy : UnitOfWorkTestContextDbStrategy {
            public PostgreSqlUnitOfWorkTestContextDbStrategy(string dbName)
                : base(dbName) {
                _properties.AddValue(NHibernate.Cfg.Environment.ConnectionDriver, "NHibernate.Driver.NpgsqlDriver");
                _properties.AddValue(NHibernate.Cfg.Environment.Dialect, "NHibernate.Dialect.PostgreSQL82Dialect");
                _properties.AddValue(NHibernate.Cfg.Environment.ConnectionProvider, "NHibernate.Connection.DriverConnectionProvider");
                _properties.AddValue(NHibernate.Cfg.Environment.ConnectionString, ConnectionStringFor(DatabaseName));
                _properties.AddValue(NHibernate.Cfg.Environment.PrepareSql, "false");
                _properties.AddValue(NHibernate.Cfg.Environment.ShowSql, "true");

                _properties.AddValue(NHibernate.Cfg.Environment.BatchSize, "0");
                _properties.Add(NHibernate.Cfg.Environment.WrapResultSets, "false");
            }

            public override DatabaseEngine DatabaseEngine {
                get { return DatabaseEngine.PostgreSql; }
            }

            internal override void CreateDatabaseMedia() {
                // Nothing to do.
            }

            private string ConnectionStringFor(string databaseName) {
                if(databaseName.Contains(";"))
                    return databaseName;

                var connStr = ConfigTool.GetConnectionString(databaseName);
                if(connStr.IsNotWhiteSpace())
                    return connStr;

                return string.Format("Server=localhost;Port=5432;database={0};Integrated Security=SSPI;", databaseName);
            }
        }

        #endregion

        #region << Firebird >>

        private sealed class FireBirdUnitOfWorkTestContextDbStrategy : UnitOfWorkTestContextDbStrategy {
            public FireBirdUnitOfWorkTestContextDbStrategy(string dbName)
                : base(dbName) {
                _properties.AddValue(NHibernate.Cfg.Environment.ConnectionDriver, "NHibernate.Driver.FirebirdClientDriver");
                _properties.AddValue(NHibernate.Cfg.Environment.Dialect, "NHibernate.Dialect.FirebirdDialect");
                _properties.AddValue(NHibernate.Cfg.Environment.ConnectionProvider, "NHibernate.Connection.DriverConnectionProvider");
                _properties.AddValue(NHibernate.Cfg.Environment.ConnectionString, ConnectionStringFor(DatabaseName));
                _properties.AddValue(NHibernate.Cfg.Environment.PrepareSql, "false");
                _properties.AddValue(NHibernate.Cfg.Environment.ShowSql, "true");

                _properties.AddValue(NHibernate.Cfg.Environment.BatchSize, "0");
                _properties.Add(NHibernate.Cfg.Environment.WrapResultSets, "false");
            }

            public override DatabaseEngine DatabaseEngine {
                get { return DatabaseEngine.Firebird; }
            }

            internal override void CreateDatabaseMedia() {
                // Nothing to do.
            }

            private string ConnectionStringFor(string databaseName) {
                if(databaseName.Contains(";"))
                    return databaseName;

                var connStr = ConfigTool.GetConnectionString(databaseName);
                if(connStr.IsNotWhiteSpace())
                    return connStr;

                return string.Format("Server=localhost;database={0};User=SYSDBA;Password=masterkey",
                                     FileTool.GetPhysicalPath(databaseName));
            }
        }

        #endregion

        #region << Cubrid >>

        private sealed class CubridUnitOfWorkTestContextDbStrategy : UnitOfWorkTestContextDbStrategy {
            public CubridUnitOfWorkTestContextDbStrategy(string dbName)
                : base(dbName) {
                // NOTE: NHibernate용 Devart사의 Oracle Driver 는 NSoft.NFramework.Data.DevartOracle.dll 에 정의되어 있습니다!!!
                //
                _properties.AddValue(NHibernate.Cfg.Environment.ConnectionDriver,
                                     @"NHibernate.Driver.CubridDriver, NSoft.NFramework.Data.NHibernateEx");
                _properties.AddValue(NHibernate.Cfg.Environment.Dialect,
                                     @"NHibernate.Dialect.CubridDialect, NSoft.NFramework.Data.NHibernateEx");
                _properties.AddValue(NHibernate.Cfg.Environment.ConnectionProvider, "NHibernate.Connection.DriverConnectionProvider");
                _properties.AddValue(NHibernate.Cfg.Environment.ConnectionString, ConnectionStringFor(DatabaseName));

                _properties.AddValue(NHibernate.Cfg.Environment.PrepareSql, "false");
                _properties.AddValue(NHibernate.Cfg.Environment.ShowSql, "true");

                _properties.AddValue(NHibernate.Cfg.Environment.BatchSize, "0");
                _properties.Add(NHibernate.Cfg.Environment.WrapResultSets, "false");
            }

            public override DatabaseEngine DatabaseEngine {
                get { return DatabaseEngine.Cubrid; }
            }

            internal override void CreateDatabaseMedia() {
                // Nothing to do.
            }

            private string ConnectionStringFor(string databaseName) {
                if(databaseName.Contains(";"))
                    return databaseName;

                var connStr = ConfigTool.GetConnectionString(databaseName);
                if(connStr.IsNotWhiteSpace())
                    return connStr;

                return string.Format("Server=localhost;database={0};port=33000;user=public;password=;", databaseName);
            }
        }

        #endregion
    }
}