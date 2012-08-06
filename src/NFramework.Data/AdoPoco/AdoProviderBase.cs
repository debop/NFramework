using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using NSoft.NFramework.Data.Mappers;
using NSoft.NFramework.Reflections;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Data.AdoPoco {
    /// <summary>
    /// <see cref="IAdoProvider"/>의 기본 추상 클래스입니다.
    /// </summary>
    public abstract partial class AdoProviderBase : IAdoProvider {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private int _connectionDepth;
        private int _transactionDepth;

        private DbTransaction _transaction;
        private bool _transactionCancelled;

        private string _lastSql;
        private object[] _lastArgs;

        private INameMapper _nameMapper;

        private readonly object _syncLock = new object();

        protected AdoProviderBase() : this(string.Empty) {}

        protected AdoProviderBase(string connectionStringName) {
            if(connectionStringName.IsWhiteSpace())
                connectionStringName = ConfigTool.GetConnectionStringSettings()[0].Name;

            var connSettings = ConfigTool.GetConnectionSettings(connectionStringName);
            Guard.Assert(connSettings != null, "해당 ConnectionString 정보를 찾지 못했습니다. connectionStringName=[{0}]", connectionStringName);

            ConnectionString = connSettings.ConnectionString;
            ProviderName = connSettings.ProviderName;

            InitializeDatabase();
        }

        protected AdoProviderBase(DbConnection connection) {
            connection.ShouldNotBeNull("connection");

            Connection = connection;
            ConnectionString = connection.ConnectionString;

            // 외부에서 connection을 닫는 것을 방지하기 위해
            _connectionDepth = 2;

            InitializeDatabase();
        }

        protected AdoProviderBase(string connectionString, string providerName) {
            connectionString.ShouldNotBeWhiteSpace("connectionString");

            ConnectionString = connectionString;
            ProviderName = providerName;

            InitializeDatabase();
        }

        protected AdoProviderBase(string connectionString, DbProviderFactory providerFactory) {
            connectionString.ShouldNotBeWhiteSpace("connectionString");

            ConnectionString = connectionString;
            ProviderFactory = providerFactory;

            InitializeDatabase();
        }

        /// <summary>
        /// IAdoDatabase에 대한 초기화
        /// </summary>
        protected virtual void InitializeDatabase() {
            EnableAutoSelect = true;
            EnableNamedParams = true;
            ForceDateTimesToUtc = true;

            NameMapper = new TrimNameMapper();

            if(ProviderName.IsNotWhiteSpace()) {
                ProviderFactory = DbProviderFactories.GetFactory(ProviderName);
                Guard.Assert(ProviderFactory != null, "DbProviderFactory 를 찾을 수 없습니다. ProviderName=[{0}]", ProviderName);
            }

            DatabaseKind = DatabaseKind.SqlServer;
            ParameterPrefix = "@";
        }

        /// <summary>
        /// Database Connection
        /// </summary>
        public DbConnection Connection { get; protected set; }

        /// <summary>
        /// Database Provider name (예 : System.Data.SqlClient 등)
        /// </summary>
        public string ProviderName { get; protected set; }

        public DbProviderFactory ProviderFactory { get; protected set; }

        /// <summary>
        /// Database connection string
        /// </summary>
        public string ConnectionString { get; protected set; }

        /// <summary>
        /// Database 종류
        /// </summary>
        public DatabaseKind DatabaseKind { get; protected set; }

        /// <summary>
        /// Parameter Prefix
        /// </summary>
        public string ParameterPrefix { get; protected set; }

        /// <summary>
        /// 자동으로 SELECT 구문이 가능하도록 할 것인지 여부
        /// </summary>
        public bool EnableAutoSelect { get; set; }

        /// <summary>
        /// Named Parameter 를 지원할 것인지 여부
        /// </summary>
        public bool EnableNamedParams { get; set; }

        /// <summary>
        /// DateTime 기준을 UTC 로 할 것인지 여부
        /// </summary>
        public bool ForceDateTimesToUtc { get; set; }

        /// <summary>
        /// Connection을 IAdoProvider 인스턴스의 생명주기와 같이 계속 열어둘 것인가?
        /// </summary>
        public bool KeepConnectionAlive { get; set; }

        /// <summary>
        /// Column 명 - Property 명 매핑을 담당하는 Mapper 
        /// </summary>
        public INameMapper NameMapper {
            get { return _nameMapper ?? (_nameMapper = new TrimNameMapper()); }
            set { _nameMapper = value; }
        }

        #region << Connection >>

        /// <summary>
        /// Connection을 엽니다.
        /// </summary>
        public virtual void OpenSharedConnection() {
            if(_connectionDepth > 0) {
                _connectionDepth++;
                return;
            }

            if(IsDebugEnabled)
                log.Debug("Connection을 엽니다... ConnectionString=[{0}]", ConnectionString);

            lock(_syncLock) {
                Connection = ProviderFactory.CreateConnection();
                Connection.ConnectionString = ConnectionString;
                Connection.Open();

                Connection = OnConnectionOpened(Connection);

                if(KeepConnectionAlive)
                    _connectionDepth++;
            }
        }

        /// <summary>
        /// Connection을 닫습니다.
        /// </summary>
        public virtual void CloseSharedConnection() {
            if(_connectionDepth <= 0)
                return;

            if(IsDebugEnabled)
                log.Debug("Shared Connection을 닫습니다... ConnectionString=[{1}]", ConnectionString);

            lock(_syncLock) {
                if(--_connectionDepth == 0) {
                    OnConnectionClosing(Connection);
                    Connection.Dispose();
                    Connection = null;
                }
            }
        }

        protected virtual DbConnection OnConnectionOpened(DbConnection connection) {
            return connection;
        }

        protected virtual void OnConnectionClosing(DbConnection connection) {}

        #endregion

        #region << Transaction >>

        /// <summary>
        /// 현 IAdoDatabase용 <see cref="IAdoTransaction"/> 인스턴스를 생성합니다.
        /// </summary>
        /// <returns></returns>
        public IAdoTransaction GetTransaction() {
            return new AdoTransaction(this);
        }

        /// <summary>
        /// Transaction을 시작합니다.
        /// </summary>
        public void BeginTransaction() {
            _transactionDepth++;

            if(IsDebugEnabled)
                log.Debug("새로운 Transaction을 시작합니다...");

            if(_transactionDepth == 1) {
                OpenSharedConnection();
                _transaction = Connection.BeginTransaction();
                _transactionCancelled = false;
                With.TryAction(OnBeginTransaction);
            }
        }

        /// <summary>
        /// Transaction을 완료합니다.
        /// </summary>
        public void CompleteTransaction() {
            if(IsDebugEnabled)
                log.Debug("Transaction을 완료합니다.");

            if(--_transactionDepth == 0)
                CleanupTransaction();
        }

        /// <summary>
        /// Transaction을 취소합니다.
        /// </summary>
        public void AbortTransaction() {
            if(IsDebugEnabled)
                log.Debug("Transaction을 취소합니다.");

            _transactionCancelled = true;
            if(--_transactionDepth == 0)
                CleanupTransaction();
        }

        /// <summary>
        /// Transaction 관련 마지막 처리를 수행하고, Transaction 을 정리합니다.
        /// </summary>
        private void CleanupTransaction() {
            if(IsDebugEnabled)
                log.Debug("Transaction 관련 마지막 처리를 수행하고, Transaction 을 정리합니다...");

            _transaction.ShouldNotBeNull("_transaction");

            OnEndTransaction();

            if(_transactionCancelled)
                _transaction.Rollback();
            else
                _transaction.Commit();

            _transaction.Dispose();
            _transaction = null;

            CloseSharedConnection();
        }

        protected virtual void OnBeginTransaction() {}
        protected virtual void OnEndTransaction() {}

        #endregion

        #region << Command >>

        /// <summary>
        /// <paramref name="connection"/>에서 실행할 <see cref="DbCommand"/> 인스턴스를 생성합니다.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="sql"></param>
        /// <param name="adoParams"></param>
        /// <returns></returns>
        public DbCommand CreateCommand(DbConnection connection, string sql, params IAdoParameter[] adoParams) {
            var args = adoParams.Select(x => x.Value).ToArray();

            if(EnableNamedParams) {
                var new_args = new List<object>();
                sql = ProcessParams(sql, args, new_args);
                args = new_args.ToArray();
            }

            if(ParameterPrefix != "@")
                sql = AdoProviderTool.RxParamsPrefix.Replace(sql, m => ParameterPrefix + m.Value.Substring(1));
            sql = sql.Replace("@@", "@");


            var cmd = (DbCommand)Connection.CreateCommand();
            cmd.Connection = Connection;
            cmd.CommandText = sql;

            if(_transaction != null)
                cmd.Transaction = _transaction;

            AdoTool.CreateCommandParameters(cmd, adoParams);

            DoSetupCommand(cmd);

            if(sql.IsNotWhiteSpace())
                DoPreCommandExecute(cmd);

            return cmd;
        }

        public string ProcessParams(string sql, object[] argsSrc, IList<object> argsDest) {
            throw new NotImplementedException();
        }

        protected virtual void DoSetupCommand(DbCommand cmd) {
            //  NOTE: Oracle 인 경우 다음과 같은 코드가 추가되어야 한다.

            var accessor = DynamicAccessorFactory.CreateDynamicAccessor<DbCommand>(MapPropertyOptions.Safety);
            accessor.SetPropertyValue(cmd, "BindByName", true);
        }

        protected virtual void DoPreCommandExecute(DbCommand command) {
            // NOTE: SQLite 인 경우,  PRAGMA 설정을 추가할 수 있다.  
        }

        #endregion

        public int Execute(SqlBuilder sqlBuilder) {
            throw new NotImplementedException();
        }

        public int Execute(string sql, params IAdoParameter[] args) {
            throw new NotImplementedException();
        }

        public T ExecuteScalar<T>(SqlBuilder sqlBuilder) {
            throw new NotImplementedException();
        }

        public T ExecuteScalar<T>(string sql, params IAdoParameter[] args) {
            throw new NotImplementedException();
        }

        #region << Event >>

        /// <summary>
        /// 지정한 Command 를 실행하기 전에 호출되는 Event handler 입니다.
        /// </summary>
        /// <param name="cmd"></param>
        protected virtual void OnExecutingCommand(DbCommand cmd) {}

        /// <summary>
        /// 지정한 Command에 대한 실행을 완료했을 때 발생하는 Event Handler 입니다.
        /// </summary>
        /// <param name="cmd"></param>
        protected virtual void OnExecutedCommand(DbCommand cmd) {}

        /// <summary>
        /// 예외가 발생했을 때 호출되는 Event Handler 입니다.
        /// </summary>
        /// <param name="ex"></param>
        protected virtual void OnException(Exception ex) {}

        #endregion

        #region << IDisposable >>

        public bool IsDisposed { get; protected set; }

        ~AdoProviderBase() {
            Dispose(false);
        }

        public void Dispose() {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing) {
            if(IsDisposed)
                return;

            if(disposing) {
                With.TryAction(CloseSharedConnection);
            }
            IsDisposed = true;
        }

        #endregion
    }
}