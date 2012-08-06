using System;
using System.Configuration;
using System.Data;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Data {
    //
    // NOTE: Transaction과 관련된 작업을 지원하기 위한 Helper Class를 만들었는데, AdoRepository에 흡수시켜서 더 이상 사용할 이유가 없다.
    //

    public static partial class DbFunc {
        private const string ActiveConnectionKey = "NSoft.NFramework.Data.DbFunc.ActiveConnection"; // + Guid.NewGuid();
        private const string ActiveTransactionKey = "NSoft.NFramework.Data.DbFunc.ActiveTransaction"; // + Guid.NewGuid();
        private const string AransactionCounterKey = "NSoft.NFramework.Data.DbFunc.TransactionCounter"; // + Guid.NewGuid();

        /// <summary>
        /// Active connection in current thread context
        /// </summary>
        private static IDbConnection ActiveConnection {
            get { return Local.Data[ActiveConnectionKey] as IDbConnection; }
            set { Local.Data[ActiveConnectionKey] = value; }
        }

        /// <summary>
        /// Active transaction in current thread context
        /// </summary>
        private static IDbTransaction ActiveTransaction {
            get { return Local.Data[ActiveTransactionKey] as IDbTransaction; }
            set { Local.Data[ActiveTransactionKey] = value; }
        }

        /// <summary>
        /// Nested transaction count in current thread context
        /// </summary>
        private static int TransactionCount {
            get { return (int)Local.Data[AransactionCounterKey]; }
            set { Local.Data[AransactionCounterKey] = value; }
        }

        /// <summary>
        /// create and open connection ( <see cref="IDbConnection"/> ) for a given named connection string,
        /// using provider connectionStringName to select the property type.
        /// </summary>
        /// <param name="connectionStringName">ConnectionString 설정 이름 (DB 이름)</param>
        /// <returns></returns>
        public static IDbConnection Connection(string connectionStringName) {
            if(IsDebugEnabled)
                log.Debug("지정된 환경설정의 ConnectionSettings 에 의한 Database Connection을 생성합니다. connectionStringName=[{0}]",
                          connectionStringName);

            var connectionString = ConfigurationManager.ConnectionStrings[connectionStringName];
            connectionString.ShouldNotBeNull(connectionStringName);

            var providerType = Type.GetType(connectionString.ProviderName);
            if(providerType == null)
                throw new InvalidOperationException(
                    string.Format("The provider type connectionStringName [{0}] cound not be found for connection string=[{1}]",
                                  connectionString.ProviderName, connectionStringName));

            // create and open Connection
            var connection = (IDbConnection)providerType.CreateInstance(); // Activator.CreateInstance(providerType);
            connection.ConnectionString = connectionString.ConnectionString;
            connection.Open();

            if(IsDebugEnabled)
                log.Debug("Database 연결에 성공했습니다!!! connectionStringName=[{0}]", connectionStringName);

            return connection;
        }

        /// <summary>
        /// 지정된 함수를 Transaction하에서 실행한다.
        /// 일반적인 DB 처리 로직을 Transaction 환경하에서 실행될 수 있도록 한다.
        /// </summary>
        /// <param name="connectionStringName">Database connection string name</param>
        /// <param name="isolationLevel">격리 수준</param>
        /// <param name="actionToExecute">실행할 Action</param>
        public static void Transaction(string connectionStringName, IsolationLevel isolationLevel, Action<IDbCommand> actionToExecute) {
            connectionStringName.ShouldNotBeWhiteSpace("connectionStringName");
            actionToExecute.ShouldNotBeNull("actionToExecute");

            if(IsDebugEnabled)
                log.Debug("Execute the specified action under transaction with dbName=[{0}], isolationLevel=[{1}]", connectionStringName,
                          isolationLevel);

            StartTransaction(connectionStringName, isolationLevel);

            try {
                using(var cmd = ActiveConnection.CreateCommand()) {
                    cmd.Transaction = ActiveTransaction;
                    actionToExecute(cmd);
                }

                CommitTransaction();
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled) {
                    log.Error("Transaction 하에서 actionToExecute를 실행하는데 실패했습니다.");
                    log.Error(ex);
                }

                RolebackTransaction();
                throw;
            }
            finally {
                DisposeTransaction();
            }
        }

        /// <summary>
        /// 지정된 함수를 Transaction하에서 실행한다.
        /// 일반적인 DB 처리 로직을 Transaction 환경하에서 실행될 수 있도록 한다.
        /// </summary>
        /// <param name="connectionStringName">connection string name</param>
        /// <param name="actionToExecute">action to execute</param>
        public static void Transaction(string connectionStringName, Action<IDbCommand> actionToExecute) {
            Transaction(connectionStringName, IsolationLevel.Unspecified, actionToExecute);
        }

        /// <summary>
        /// 지정된 함수를 Transaction하에서 실행하고, 결과를 반환한다.
        /// 일반적인 DB 처리 로직을 Transaction 환경하에서 실행될 수 있도록 한다.
        /// </summary>
        /// <typeparam name="TResult">실행결과 값의 형식</typeparam>
        /// <param name="connectionStringName">connection string name</param>
        /// <param name="actionToExecute">실행할 Action</param>
        /// <returns>실행한 결과 값</returns>
        public static TResult Transaction<TResult>(string connectionStringName, Func<IDbCommand, TResult> actionToExecute) {
            connectionStringName.ShouldNotBeWhiteSpace("connectionStringName");
            actionToExecute.ShouldNotBeNull("actionToExecute");

            var result = default(TResult);
            Transaction(connectionStringName,
                        delegate(IDbCommand command) { result = actionToExecute(command); });

            return result;
        }

        /// <summary>
        /// 지정된 이름의 Database에 대해 Transaction을 시작합니다.
        /// </summary>
        /// <param name="connectionStringName">connection string name</param>
        /// <param name="isolationLevel">격리수준</param>
        private static void StartTransaction(string connectionStringName, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted) {
            connectionStringName.ShouldNotBeWhiteSpace("connectionStringName");

            if(IsDebugEnabled)
                log.Debug("Start Transaction. database connectionStringName=[{0}], isolationLevel=[{1}]",
                          connectionStringName, isolationLevel);

            if(TransactionCount <= 0) {
                TransactionCount = 0;
                ActiveConnection = Connection(connectionStringName);
                ActiveTransaction = ActiveConnection.BeginTransaction(isolationLevel);

                if(IsDebugEnabled)
                    log.Debug("Start transaction is success for database connectionStringName=[{0}]", connectionStringName);
            }
            TransactionCount++;
        }

        /// <summary>
        /// Commit transaction
        /// </summary>
        private static void CommitTransaction() {
            TransactionCount--;

            if(TransactionCount == 0 && ActiveTransaction != null) {
                ActiveTransaction.Commit();
                ActiveTransaction.Dispose();
                ActiveTransaction = null;

                if(log.IsInfoEnabled)
                    log.Info("활성화된 Transaction을 Commit하고 Dispose했습니다.");
            }
        }

        /// <summary>
        /// Rollback active transaction
        /// </summary>
        private static void RolebackTransaction() {
            try {
                ActiveTransaction.Rollback();

                if(log.IsWarnEnabled)
                    log.Warn("활성화된 Transaction을 Rollback 했습니다.");
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled) {
                    log.Error("Transaction Rollback에 실패했습니다.");
                    log.Error(ex);
                }
            }
            finally {
                if(ActiveTransaction != null) {
                    ActiveTransaction.Dispose();
                    ActiveTransaction = null;
                }
                TransactionCount = 0;
            }
        }

        /// <summary>
        /// Dispose active transaction
        /// </summary>
        private static void DisposeTransaction() {
            if(TransactionCount <= 0) {
                if(IsDebugEnabled)
                    log.Debug("활성 Transaction을 Dispose 합니다..");

                if(ActiveTransaction != null)
                    ActiveTransaction.Dispose();
                ActiveTransaction = null;
            }
        }
    }
}