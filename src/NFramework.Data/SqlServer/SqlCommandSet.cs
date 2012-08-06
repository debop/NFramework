using System;
using System.Data.SqlClient;
using System.Reflection;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Data.SqlServer {
    /// <summary>
    /// 실행할 복수의 Command 를 Batch로 실행하여, round-trip을 줄이고, 속도를 향샹시킵니다.
    /// <see cref="System.Data.SqlClient.SqlCommandSet"/>가 internal이라 외부에서는 사용할 수 없습니다.
    /// 이를 사용하기 위해 delegate를 이용하여 내부 함수를 사용할 수 있도록 했습니다.
    /// </summary>
    /// <seealso cref="AdoDataAdapter"/>
    public sealed class SqlCommandSet : IDisposable {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        internal const string SystemDataAssemblyName = @"System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
        // internal const string SystemDataAssemblyName = @"System.Data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";

        internal const string SqlCommandSetTypeName = @"System.Data.SqlClient.SqlCommandSet";

        private static readonly Type _sqlCommandSetType;
        private static object _sqlCommandSetInstance;

        private readonly PropertySetter<SqlConnection> _connectionSettter;
        private readonly PropertySetter<SqlTransaction> _transactionSetter;
        private readonly PropertySetter<int> _commandTimeoutSetter;
        private readonly PropertyGetter<SqlConnection> _connectionGetter;
        private readonly PropertyGetter<SqlCommand> _batchCommandGetter;
        private readonly AppendCommand _doAppend;
        private readonly ExecuteNonQueryCommand _doExecuteNonQuery;
        private readonly DisposeCommand _doDispose;
        private int _countOfCommands;

        /// <summary>
        /// Static 생성자
        /// </summary>
        static SqlCommandSet() {
            if(log.IsInfoEnabled)
                log.Info("SqlCommandSet을 Aseembly [{0}] 로부터 동적으로 로드합니다... ", SystemDataAssemblyName);

            _sqlCommandSetType = Assembly.Load(SystemDataAssemblyName).GetType(SqlCommandSetTypeName);
            _sqlCommandSetType.ShouldNotBeNull("Type of SqlCommandSet");

            if(log.IsInfoEnabled)
                log.Info("[{0}]을 Aseembly [{1}]로부터 동적으로 로드했습니다!!!", SqlCommandSetTypeName, SystemDataAssemblyName);
        }

        /// <summary>
        /// 생성자
        /// </summary>
        public SqlCommandSet() {
            if(IsDebugEnabled)
                log.Debug("{0}를 동적으로 생성하고, 내부 함수를 Delegate로 이용하여, 노출시킵니다.", SqlCommandSetTypeName);

            _sqlCommandSetInstance = ActivatorTool.CreateInstance(_sqlCommandSetType, true);

            _sqlCommandSetInstance.ShouldNotBeNull(SqlCommandSetTypeName);

            _connectionSettter =
                (PropertySetter<SqlConnection>)CreateDelegate(typeof(PropertySetter<SqlConnection>),
                                                              _sqlCommandSetInstance,
                                                              "set_Connection");
            _transactionSetter =
                (PropertySetter<SqlTransaction>)CreateDelegate(typeof(PropertySetter<SqlTransaction>),
                                                               _sqlCommandSetInstance,
                                                               "set_Transaction");
            _commandTimeoutSetter =
                (PropertySetter<int>)CreateDelegate(typeof(PropertySetter<int>),
                                                    _sqlCommandSetInstance,
                                                    "set_CommandTimeout");

            _connectionGetter =
                (PropertyGetter<SqlConnection>)CreateDelegate(typeof(PropertyGetter<SqlConnection>),
                                                              _sqlCommandSetInstance,
                                                              "get_Connection");

            _batchCommandGetter =
                (PropertyGetter<SqlCommand>)CreateDelegate(typeof(PropertyGetter<SqlCommand>),
                                                           _sqlCommandSetInstance,
                                                           "get_BatchCommand");

            _doAppend = (AppendCommand)CreateDelegate(typeof(AppendCommand), _sqlCommandSetInstance, "Append");
            _doExecuteNonQuery =
                (ExecuteNonQueryCommand)CreateDelegate(typeof(ExecuteNonQueryCommand), _sqlCommandSetInstance, "ExecuteNonQuery");
            _doDispose = (DisposeCommand)CreateDelegate(typeof(DisposeCommand), _sqlCommandSetInstance, "Dispose");
        }

        /// <summary>
        /// Batch 작업을 수행할 Command 를 추가합니다. NOTE: 단 Command의 Parameter는 하나 이상이어야 합니다!!!
        /// </summary>
        /// <param name="command"></param>
        public void Append(SqlCommand command) {
            command.ShouldNotBeNull("command");

            if(IsDebugEnabled)
                log.Debug("실행할 SqlCommand를 추가합니다... commandType=[{0}], commandText=[{1}]", command.CommandType, command.CommandText);

            _doAppend(command);
            _countOfCommands++;
        }

        /// <summary>
        /// 등록된 Command 들을 Batch작업으로 수행하고, 모든 Batch 작업에 의해 영향을 받은 행의 갯수를 반환합니다.
        /// </summary>
        /// <returns>
        /// 모든 Batch 작업에 의해 영향을 받은 행의 갯수를 반환합니다.
        /// </returns>
        public int ExecuteNonQuery() {
            if(CountofCommands == 0)
                return 0;

            Guard.Assert(Connection != null, "Connection이 설정되지 않았습니다. Command를 실행하기 위해서는 Connection이 설정되어 있어야 합니다.");

            if(IsDebugEnabled)
                log.Debug("등록된 SqlCommand를 배치로 실행합니다...");

            return _doExecuteNonQuery();
        }

        /// <summary>
        /// 실행할 Batch Command 인스턴스
        /// </summary>
        public SqlCommand BatchCommand {
            get { return _batchCommandGetter(); }
        }

        /// <summary>
        /// Batch로 실행할 Command의 갯수
        /// </summary>
        public int CountofCommands {
            get { return _countOfCommands; }
        }

        public SqlConnection Connection {
            get { return _connectionGetter(); }
            set { _connectionSettter(value); }
        }

        public SqlTransaction Transaction {
            set { _transactionSetter(value); }
        }

        public int CommandTimeout {
            set { _commandTimeoutSetter(value); }
        }

        #region << IDisposable >>

        public bool IsDisposed { get; private set; }

        ~SqlCommandSet() {
            Dispose(false);
        }

        /// <summary>
        /// 관리되지 않는 리소스의 확보, 해제 또는 다시 설정과 관련된 응용 프로그램 정의 작업을 수행합니다.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose() {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        private void Dispose(bool disposing) {
            if(IsDisposed)
                return;

            if(disposing) {
                With.TryAction(() => _doDispose());

                if(IsDebugEnabled)
                    log.Debug("SqlCommandSet을 Dispose했습니다");
            }

            IsDisposed = true;
        }

        #endregion

        #region << Delegate Definitions >>

        private static Delegate CreateDelegate(Type type, object target, string method) {
            return Delegate.CreateDelegate(type, target, method);
        }

        private delegate void PropertySetter<T>(T item);

        private delegate T PropertyGetter<T>();

        private delegate void AppendCommand(System.Data.SqlClient.SqlCommand command);

        private delegate int ExecuteNonQueryCommand();

        private delegate void DisposeCommand();

        #endregion
    }
}