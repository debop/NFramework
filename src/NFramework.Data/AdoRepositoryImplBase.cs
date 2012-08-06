using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Threading;
using Microsoft.Practices.EnterpriseLibrary.Data;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.Reflections;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Data {
    /// <summary>
    /// <see cref="IAdoRepository"/>의 Implementor의 추상 클래스
    /// </summary>
    public abstract class AdoRepositoryImplBase : IAdoRepository {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        #region << Properties >>

        private Database _db;
        private string _dbName;
        private readonly object _syncLock = new object();

        /// <summary>
        ///  Initialize AdoRepositoryImplBase with default database name
        /// </summary>
        protected AdoRepositoryImplBase() : this(AdoTool.DefaultDatabaseName) {}

        /// <summary>
        /// Initialize AdoRepositoryImplBase with database name.
        /// </summary>
        /// <param name="dbName">database name</param>
        protected AdoRepositoryImplBase(string dbName) {
            if(dbName.IsNotWhiteSpace())
                dbName = AdoTool.DefaultDatabaseName;

            if(IsDebugEnabled)
                log.Debug("IAdoRepository를 구현한 인스턴스[{0}]를 생성합니다. dbName=[{1}]", GetType().FullName, dbName);

            _dbName = dbName;
        }

        /// <summary>
        /// Microsoft Data Acess Application Block의 Database
        /// </summary>
        public virtual Database Db {
            get {
                if(_db == null)
                    lock(_syncLock)
                        if(_db == null) {
                            var database = DatabaseFactory.CreateDatabase(DbName);
                            Thread.MemoryBarrier();
                            _db = database;
                        }

                return _db;
            }
            protected set { _db = value; }
        }

        /// <summary>
        /// Database ConnectionString 이름
        /// </summary>
        public string DbName {
            get { return _dbName ?? (_dbName = AdoTool.DefaultDatabaseName); }
            set {
                if(_dbName != value) {
                    _dbName = value ?? AdoTool.DefaultDatabaseName;
                    _db = null;
                }
            }
        }

        /// <summary>
        /// Query String Provider  
        /// </summary>
        /// <remarks>IoC를 이용하지 않으면, 사용하기 전에 먼저 설정해 주어야 합니다.</remarks>
        /// <example>
        /// <code>
        /// // IoC 에서 IAdoRepository에 대해 QueryProvider를 정의해주면 사용할 수 있다. (동적으로 지정해도 된다.)
        /// <component id="NorthwindAdoRepository"
        ///            service="NSoft.NFramework.Data.IAdoRepository, NSoft.NFramework.Data"
        ///            type="NFrameowk.Data.AdoRepositoryImpl, NSoft.NFramework.Data">
        ///     <parameters>
        ///			<QueryProvider>${IniAdoQueryProvider.Northwind}</QueryProvider>
        ///     </parameters>
        /// </component>
        /// </code>
        /// </example>
        public IIniQueryProvider QueryProvider { get; set; }

        #endregion

        #region << Transaction >>

        //TODO: ActiveTransaction은 중첩이 가능하게 Stack을 사용해야 한다. IoC 의 LocalContainerStack 을 참고할 것

        /// <summary>
        /// Stored Key for Active Transaction in Current Thread Context.
        /// </summary>
        public static readonly object ActiveTransactionKey = new object();

        /// <summary>
        /// Current Thread Context에서 활성화된 Transaction. (없다면 null을 반환한다.)
        /// </summary>
        /// <seealso cref="IsActiveTransaction"/>
        public DbTransaction ActiveTransaction {
            get { return Local.Data[ActiveTransactionKey] as DbTransaction; }
            set { Local.Data[ActiveTransactionKey] = value; }
        }

        /// <summary>
        /// Is exist active transaction in Current Thread Context.
        /// </summary>
        public bool IsActiveTransaction {
            get { return (ActiveTransaction != null); }
        }

        /// <summary>
        /// ADO.NET의 기본 Transaction을 시작한다. (TransactionScope와는 달리 DTC를 이용하지 않는다.)
        /// </summary>
        /// <param name="isolationLevel">Transaction 격리수준 (기본적으로 ReadCommitted)</param>
        /// <returns>Return a new instance of DbTransaction</returns>
        /// <example>
        /// <code>
        /// AdoRepository.BeginTransaction(IsolationLevel.ReadUncommitted);
        /// try
        /// {
        /// 	DbCommand insertCommand = AdoRepository.GetSqlStringCommand(InsertString);
        /// 	DbCommand deleteCommand = AdoRepository.GetSqlStringCommand(DeleteString);
        /// 	DbCommand countCommand = AdoRepository.GetSqlStringCommand(CountString);
        /// 
        /// 	var count = Convert.ToInt32(AdoRepository.ExecuteScalar(countCommand));
        /// 
        /// 	AdoRepository.ExecuteNonQuery(insertCommand);
        /// 	AdoRepository.ExecuteNonQuery(deleteCommand);
        /// 
        /// 	AdoRepository.Commit();
        /// 
        /// 	Assert.AreEqual(4, count);
        /// }
        /// catch (Exception ex)
        /// {
        /// 	if (IsErrorEnabled)
        /// 		log.ErrorException(ex);
        /// 
        /// 	AdoRepository.Rollback();
        /// 
        /// 	throw;
        /// }
        /// </code>
        /// </example>
        public virtual DbTransaction BeginTransaction(System.Data.IsolationLevel isolationLevel) {
            var connection = AdoTool.CreateTransactionScopeConnection(Db);
            ActiveTransaction = connection.BeginTransaction(isolationLevel);

            if(IsDebugEnabled)
                log.Debug("Database=[{0}]에 새로운 Transaction을 시작했습니다.  Transaction=[{1}]",
                          connection.Database, ActiveTransaction.GetType().FullName);

            return ActiveTransaction;
        }

        /// <summary>
        /// ADO.NET의 기본 Transaction을 시작한다. 격리수준은 <see cref="IsolationLevel.ReadCommitted"/>이다. 
        /// (TransactionScope와는 달리 DTC를 이용하지 않는다.)
        /// </summary>
        /// <returns>Return a new instance of DbTransaction</returns>
        public virtual DbTransaction BeginTransaction() {
            return BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
        }

        /// <summary>
        /// 현재 활성화된 Transaction이 있다면, Commit을 수행한다.
        /// </summary>
        /// <exception cref="InvalidOperationException">Current Thread Context에 활성화된 Transaction이 없을 때</exception>
        public virtual void Commit() {
            if(IsActiveTransaction == false) {
                if(IsDebugEnabled)
                    log.Debug("활성화된 Transaction이 없으므로 반환합니다.");

                return;
            }

            if(IsActiveTransaction) {
                try {
                    ActiveTransaction.Commit();

                    if(log.IsInfoEnabled)
                        log.Info("Transaction을 Commit 했습니다!!!");
                }
                catch(Exception ex) {
                    if(log.IsErrorEnabled)
                        log.ErrorException("Transaction Commit 작업이 실패했습니다.", ex);

                    throw;
                }
                finally {
                    if(IsActiveTransaction) {
                        ActiveTransaction.Dispose();
                        ActiveTransaction = null;
                    }
                }
            }
        }

        /// <summary>
        /// 현재 활성화된 Transaction이 있다면, Rollback을 수행한다.
        /// </summary>
        public virtual void Rollback() {
            // Guard.Assert(ActiveTransaction != null, "Active transaction is null.");

            if(IsActiveTransaction == false) {
                if(IsDebugEnabled)
                    log.Debug("ActiveTransaction이 없습니다. Rollback을 수행하지 않습니다.");

                return;
            }

            try {
                ActiveTransaction.Rollback();

                if(log.IsInfoEnabled)
                    log.Info("Transaction을 Rollback 했습니다.");
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.ErrorException("Transaction Rollback 작업이 실패했습니다.", ex);
                throw;
            }
            finally {
                if(IsActiveTransaction) {
                    ActiveTransaction.Dispose();
                    ActiveTransaction = null;
                }
            }
        }

        #endregion

        #region << ExecuteDataSet >>

        /// <summary>
        /// 지정된 DataAdapter를 통해 얻은 정보를 DataSet으로 빌드한다.
        /// </summary>
        /// <param name="adapter">DataAdapter</param>
        /// <param name="tableName">Table name</param>
        /// <param name="targetDataSet">저장할 Dataset</param>
        /// <param name="firstResult">첫번째 레코드의 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">최대 레코드 수 (0 이면 무시하고, 마지막 레코드까지 가져온다</param>
        public virtual void LoadDataSet(DbDataAdapter adapter, string tableName, DataSet targetDataSet,
                                        int firstResult = 0, int maxResults = 0) {
            adapter.ShouldNotBeNull("adapter");
            targetDataSet.ShouldNotBeNull("targetDataSet");
            Guard.Assert(adapter.SelectCommand != null, "adapter.SelectCommand is null.");

            if(IsDebugEnabled)
                log.Debug(
                    "DataAdatpter를 이용하여 정보를 DataSet에 로드합니다...  adapter.SelectCommand=[{0}], tableName=[{1}], firstResult=[{2}], maxResults=[{3}]",
                    adapter.SelectCommand.CommandText, tableName, firstResult, maxResults);

            var resultCount = 0;
            var adoAdapter = new AdoDataAdapter(adapter);

            IDataReader reader = null;
            try {
                reader = ExecuteReaderInternal(adapter.SelectCommand);
                do {
                    var name = tableName.IsWhiteSpace() ? AdoTool.DefaultTableName + ++resultCount : tableName;

                    adoAdapter.Fill(targetDataSet, name, reader, firstResult, maxResults);
                } while(!reader.IsClosed && reader.NextResult());
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.ErrorException("DataReader로부터 Data를 로드하는데 실패했습니다.", ex);
            }
            finally {
                if(reader != null)
                    reader.Dispose();

                //! 이거 하면 안된다!!! 호출한 놈이 해야한다.
                // adoAdapter.Dispose();
            }
        }

        /// <summary>
        /// 지정된 DataAdpater를 실행하여 DataSet을 빌드한다.
        /// </summary>
        /// <param name="da">실행할 DataAdapter</param>
        /// <param name="tableName">결과 DataSet의 Table 명</param>
        /// <param name="firstResult">첫번째 레코드의 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">최대 레코드 수 (0 이면 무시하고, 마지막 레코드까지 가져온다</param>
        /// <returns>결과 셋이 담긴 DataSet</returns>
        [Obsolete("Use ExecuteDataSet(DbCommand,int,int)")]
        public DataSet ExecuteDataSet(DbDataAdapter da, string tableName, int firstResult = 0, int maxResults = 0) {
            var result = new DataSet { Locale = CultureInfo.InvariantCulture };
            LoadDataSet(da, tableName, result, firstResult, maxResults);
            return result;
        }

        /// <summary>
        /// <paramref name="cmd"/>를 실행하여, 결과를 DataSet으로 반환합니다.
        /// </summary>
        /// <param name="cmd">실행할 <see cref="DbCommand"/> instance.</param>
        /// <param name="firstResult">첫번째 레코드의 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">최대 레코드 수 (0 이면 무시하고, 마지막 레코드까지 가져온다</param>
        /// <param name="parameters">collectio of parameters of Command</param>
        /// <returns>결과 셋이 담긴 DataSet</returns>
        public virtual DataSet ExecuteDataSet(DbCommand cmd, int firstResult, int maxResults,
                                              params IAdoParameter[] parameters) {
            cmd.ShouldNotBeNull("cmd");

            if(IsDebugEnabled)
                log.Debug("Execute DataSet... commandText=[{0}], firstResult=[{1}], maxResults=[{2}], parameters=[{3}]",
                          cmd.CommandText, firstResult, maxResults, parameters.CollectionToString());

            // DAAB에 보면 DataSet, DataTable 생성시, Locale을 이렇게 지정하고 있네...
            //
            var dataset = new DataSet { Locale = CultureInfo.InvariantCulture };

            var dataTables = ExecuteDataTableAsList(cmd, firstResult, maxResults, parameters);
            dataset.Tables.AddRange(dataTables.ToArray());

            return dataset;
        }

        /// <summary>
        /// <paramref name="cmd"/>를 실행하여, 결과를 DataSet으로 반환합니다.
        /// </summary>
        /// <param name="cmd">실행할 <see cref="DbCommand"/> instance.</param>
        /// <param name="parameters">collectio of parameters of Command</param>
        /// <returns>결과 셋이 담긴 DataSet</returns>
        public DataSet ExecuteDataSet(DbCommand cmd, params IAdoParameter[] parameters) {
            cmd.ShouldNotBeNull("cmd");
            return ExecuteDataSet(cmd, 0, 0, parameters);
        }

        /// <summary>
        /// <paramref name="query"/> 문을 실행하여, DataSet을 빌드합니다.
        /// </summary>
        /// <param name="query">실행할 쿼림 문 또는 Procedure 명</param>
        /// <param name="firstResult">첫번째 레코드의 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">최대 레코드 수 (0 이면 무시하고, 마지막 레코드까지 가져온다)</param>		
        /// <param name="parameters">parameter collection</param>
        /// <returns>결과 셋</returns>
        public DataSet ExecuteDataSet(string query, int firstResult, int maxResults, params IAdoParameter[] parameters) {
            query.ShouldNotBeWhiteSpace("query");
            firstResult.ShouldBePositiveOrZero("firstResult");

            using(var cmd = this.GetCommand(query))
                return ExecuteDataSet(cmd, firstResult, maxResults, parameters);
        }

        /// <summary>
        /// <paramref name="query"/> 문을 실행하여, DataSet을 빌드합니다.
        /// </summary>
        /// <param name="query">실행할 쿼림 문 또는 Procedure 명</param>
        /// <param name="parameters">parameter collection</param>
        /// <returns>결과 셋</returns>
        public DataSet ExecuteDataSet(string query, params IAdoParameter[] parameters) {
            query.ShouldNotBeWhiteSpace("query");

            using(var cmd = this.GetCommand(query))
                return ExecuteDataSet(cmd, parameters);
        }

        /// <summary>
        /// 지정된 DataAdpater를 실행하여 DataSet을 빌드한다.
        /// </summary>
        /// <param name="sqlString">simple query string to execute</param>
        /// <param name="firstResult">첫번째 레코드의 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">최대 레코드 수 (0 이면 무시하고, 마지막 레코드까지 가져온다</param>
        /// <param name="parameters">collectio of parameters of Command</param>
        /// <returns>결과 셋이 담긴 DataSet</returns>
        public DataSet ExecuteDataSetBySqlString(string sqlString, int firstResult, int maxResults,
                                                 params IAdoParameter[] parameters) {
            using(var cmd = this.GetSqlStringCommand(sqlString))
                return ExecuteDataSet(cmd, firstResult, maxResults, parameters);
        }

        /// <summary>
        /// 지정된 DataAdpater를 실행하여 DataSet을 빌드한다.
        /// </summary>
        /// <param name="sqlString">simple query string to execute</param>
        /// <param name="parameters">collectio of parameters of Command</param>
        /// <returns>결과 셋이 담긴 DataSet</returns>
        public DataSet ExecuteDataSetBySqlString(string sqlString, params IAdoParameter[] parameters) {
            return ExecuteDataSetBySqlString(sqlString, 0, 0, parameters);
        }

        /// <summary>
        /// 지정된 DataAdpater를 실행하여 DataSet을 빌드한다.
        /// </summary>
        /// <param name="spName">Procedure name to execute</param>
        /// <param name="firstResult">첫번째 레코드의 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">최대 레코드 수 (0 이면 무시하고, 마지막 레코드까지 가져온다</param>
        /// <param name="parameters">collectio of parameters of Command</param>
        /// <returns>결과 셋이 담긴 DataSet</returns>
        public DataSet ExecuteDataSetByProcedure(string spName, int firstResult, int maxResults,
                                                 params IAdoParameter[] parameters) {
            using(var cmd = this.GetProcedureCommand(spName))
                return ExecuteDataSet(cmd, firstResult, maxResults, parameters);
        }

        /// <summary>
        /// 지정된 DataAdpater를 실행하여 DataSet을 빌드한다.
        /// </summary>
        /// <param name="spName">Procedure name to execute</param>
        /// <param name="parameters">collectio of parameters of Command</param>
        /// <returns>결과 셋이 담긴 DataSet</returns>
        public DataSet ExecuteDataSetByProcedure(string spName, params IAdoParameter[] parameters) {
            return ExecuteDataSetByProcedure(spName, 0, 0, parameters);
        }

        #endregion

        #region << ExecuteDataTable >>

        /// <summary>
        /// 지정된 Command를 실행하여, DataTable을 빌드합니다.
        /// </summary>
        /// <param name="cmd">실행할 Select용 Command</param>
        /// <param name="firstResult">첫번째 레코드의 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">최대 레코드 수 (0 이면 무시하고, 마지막 레코드까지 가져온다</param>
        /// <param name="parameters">DbCommand의 Parameter 정보</param>
        /// <returns>instance of <see cref="DataTable"/></returns>
        public virtual DataTable ExecuteDataTable(DbCommand cmd, int firstResult, int maxResults,
                                                  params IAdoParameter[] parameters) {
            cmd.ShouldNotBeNull("cmd");

            if(IsDebugEnabled)
                log.Debug("Command를 수행합니다. CommandText=[{0}], firstResult=[{1}], maxResults=[{2}], parameters=[{3}]",
                          cmd.CommandText, firstResult, maxResults, parameters.CollectionToString());


            var loadAllRecord = (firstResult <= 0 && maxResults <= 0);
            var dataTable = new DataTable(AdoTool.DefaultTableName) { Locale = CultureInfo.InvariantCulture };

            using(var reader = ExecuteReader(cmd, parameters)) {
                // 모든 레코드
                if(loadAllRecord) {
                    dataTable.Load(reader);
                }
                else {
                    using(var adoAdapter = new AdoDataAdapter(this.GetDataAdapter()))
                        adoAdapter.Fill(new[] { dataTable }, reader, firstResult, maxResults);
                }
            }

            return dataTable;
        }

        /// <summary>
        /// 지정된 Command를 실행하여, DataTable을 빌드합니다.
        /// </summary>
        /// <param name="cmd">실행할 Select용 Command</param>
        /// <param name="parameters">DbCommand의 Parameter 정보</param>
        /// <returns>instance of <see cref="DataTable"/></returns>
        public DataTable ExecuteDataTable(DbCommand cmd, params IAdoParameter[] parameters) {
            cmd.ShouldNotBeNull("cmd");
            return ExecuteDataTable(cmd, 0, 0, parameters);
        }

        /// <summary>
        /// <paramref name="query"/> 문을 실행하여, DataTable을 빌드합니다.
        /// </summary>
        /// <param name="query">실행할 쿼림 문 또는 Procedure 명</param>
        /// <param name="firstResult">첫번째 레코드의 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">최대 레코드 수 (0 이면 무시하고, 마지막 레코드까지 가져온다)</param>		
        /// <param name="parameters">parameter collection</param>
        /// <returns>결과 셋</returns>
        public DataTable ExecuteDataTable(string query, int firstResult, int maxResults,
                                          params IAdoParameter[] parameters) {
            using(var cmd = this.GetCommand(query))
                return ExecuteDataTable(cmd, firstResult, maxResults, parameters);
        }

        /// <summary>
        /// <paramref name="query"/> 문을 실행하여, DataTable을 빌드합니다.
        /// </summary>
        /// <param name="query">실행할 쿼림 문 또는 Procedure 명</param>
        /// <param name="parameters">parameter collection</param>
        /// <returns>결과 셋</returns>
        public DataTable ExecuteDataTable(string query, params IAdoParameter[] parameters) {
            using(var cmd = this.GetCommand(query))
                return ExecuteDataTable(cmd, parameters);
        }

        /// <summary>
        /// 지정된 sql string 문을 실행하여, DataTable을 빌드합니다.
        /// </summary>
        /// <param name="sqlString">실행할 Query 문</param>
        /// <param name="firstResult">첫번째 레코드의 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">최대 레코드 수 (0 이면 무시하고, 마지막 레코드까지 가져온다</param>
        /// <param name="parameters">DbCommand의 Parameter 정보</param>
        /// <returns>instance of <see cref="DataTable"/></returns>
        public DataTable ExecuteDataTableBySqlString(string sqlString, int firstResult, int maxResults,
                                                     params IAdoParameter[] parameters) {
            using(var cmd = this.GetSqlStringCommand(sqlString))
                return ExecuteDataTable(cmd, firstResult, maxResults, parameters);
        }

        /// <summary>
        /// 지정된 sql string 문을 실행하여, DataTable을 빌드합니다.
        /// </summary>
        /// <param name="sqlString">실행할 Query 문</param>
        /// <param name="parameters">DbCommand의 Parameter 정보</param>
        /// <returns>instance of <see cref="DataTable"/></returns>
        public DataTable ExecuteDataTableBySqlString(string sqlString, params IAdoParameter[] parameters) {
            return ExecuteDataTableBySqlString(sqlString, 0, 0, parameters);
        }

        /// <summary>
        /// 지정된 Procedure를 실행하여, DataTable을 빌드합니다.
        /// </summary>
        /// <param name="spName">실행할 Procedure 명</param>
        /// <param name="firstResult">첫번째 레코드의 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">최대 레코드 수 (0 이면 무시하고, 마지막 레코드까지 가져온다</param>
        /// <param name="parameters">DbCommand의 Parameter 정보</param>
        /// <returns>instance of <see cref="DataTable"/></returns>
        public DataTable ExecuteDataTableByProcedure(string spName, int firstResult, int maxResults,
                                                     params IAdoParameter[] parameters) {
            using(var cmd = this.GetProcedureCommand(spName))
                return ExecuteDataTable(cmd, firstResult, maxResults, parameters);
        }

        /// <summary>
        /// 지정된 Procedure를 실행하여, DataTable을 빌드합니다.
        /// </summary>
        /// <param name="spName">실행할 Select Command</param>
        /// <param name="parameters">DbCommand의 Parameter 정보</param>
        /// <returns>instance of <see cref="DataTable"/></returns>
        public DataTable ExecuteDataTableByProcedure(string spName, params IAdoParameter[] parameters) {
            return ExecuteDataTableByProcedure(spName, 0, 0, parameters);
        }

        #endregion

        #region << ExecuteDataTableAsList >>

        /// <summary>
        /// 여러 ResultSet을 반환할 수 있으므로, DataTable의 컬렉션으로 반환합니다.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual IList<DataTable> ExecuteDataTableAsList(DbCommand cmd, params IAdoParameter[] parameters) {
            return ExecuteDataTableAsList(cmd, 0, 0, parameters);
        }

        /// <summary>
        /// 여러 ResultSet을 반환할 수 있으므로, DataTable의 컬렉션으로 반환합니다.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="firstResult"></param>
        /// <param name="maxResults"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual IList<DataTable> ExecuteDataTableAsList(DbCommand cmd, int firstResult, int maxResults,
                                                               params IAdoParameter[] parameters) {
            cmd.ShouldNotBeNull("cmd");

            if(IsDebugEnabled)
                log.Debug("DbCommand를 수행하여 DataTable 컬렉션으로 반환합니다... " +
                          "CommandText=[{0}], firstResult=[{1}], maxResults=[{2}], parameters=[{3}]",
                          cmd.CommandText, firstResult, maxResults, parameters.CollectionToString());

            var tables = new List<DataTable>();
            var adoAdapter = new AdoDataAdapter(this.GetDataAdapter());

            if(firstResult <= 0)
                firstResult = 0;

            if(maxResults <= 0)
                maxResults = int.MaxValue;

            using(var reader = ExecuteReader(cmd, parameters)) {
                var tableCount = 0;

                do {
                    var tableName = AdoTool.DefaultTableName + ++tableCount;
                    var dataTable = new DataTable(tableName) { Locale = CultureInfo.InvariantCulture };

                    if(IsDebugEnabled)
                        log.Debug("DataReader로부터 정보를 읽어 DataTable[{0}]을 빌드합니다...", tableName);

                    adoAdapter.Fill(new DataTable[] { dataTable }, reader, firstResult, maxResults);
                    tables.Add(dataTable);
                } while(reader.IsClosed == false && reader.NextResult());
            }

            return tables;
        }

        #endregion

        #region << ExecutePagingDataTable >>

        /// <summary>
        /// 지정된 Command 를 Paging 정보에 근거해서 수행하고, 결과를 DataTable로 반환한다.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="pageIndex">Page index (0부터 시작).  null이면 0으로 간주</param>
        /// <param name="pageSize">Page Size. 한 페이지에 표현할 요소 수 (보통 10개). null이면 <see cref="AdoTool.DefaultPageSize"/>으로 간주</param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual PagingDataTable ExecutePagingDataTable(DbCommand cmd,
                                                              int? pageIndex,
                                                              int? pageSize,
                                                              params IAdoParameter[] parameters) {
            cmd.ShouldNotBeNull("cmd");

            if(IsDebugEnabled)
                log.Debug(
                    "조회를 수행하여 PagingDataTable을 반환합니다... commandText=[{0}], pageIndex=[{1}], pageSize=[{2}], parameters=[{3}]",
                    cmd.CommandText, pageIndex, pageSize, parameters.CollectionToString());

            var maxResults = pageSize.GetValueOrDefault(AdoTool.DefaultPageSize);
            var firstResult = pageIndex.GetValueOrDefault(0) * maxResults;

            // NOTE: Command에 의한 전체 레코드 수를 구합니다. 다만, 비동기 작업 시에 Command가 닫힐 우려가 있어 CommandText를 이용하여 새로운 Command를 만들게 합니다.
            //
            var countTask = this.CountAsync(cmd.CommandText, parameters);
            var loadTable = ExecuteDataTable(cmd, firstResult, maxResults, parameters);

            return new PagingDataTable(loadTable,
                                       pageIndex ?? 0,
                                       pageSize ?? AdoTool.DefaultPageSize,
                                       countTask.Result);
        }

        /// <summary>
        /// <paramref name="query"/>를 Paging 정보에 근거해서 실행하고, 결과를 DataTable로 반환한다.
        /// NOTE: DISTINCT, TOP N 조회는 Paging에서 지원하지 않습니다.
        /// </summary>
        /// <param name="query">조회용 쿼리 문</param>
        /// <param name="pageIndex">Page Index (0부터 시작).  null이면 0으로 간주</param>
        /// <param name="pageSize">Page Size. 한 페이지에 표현할 요소 수 (보통 10개). null이면 10으로 간주</param>
        /// <param name="parameters">조회용 쿼리의 Parameter 정보</param>
        /// <returns>Paging된 DataTable</returns>
        public PagingDataTable ExecutePagingDataTable(string query, int? pageIndex, int? pageSize,
                                                      params IAdoParameter[] parameters) {
            query.ShouldNotBeWhiteSpace("query");

            if(AdoTool.IsSqlString(query))
                return ExecutePagingDataTableBySqlString(query, pageIndex, pageSize, parameters);

            using(var cmd = this.GetCommand(query))
                return ExecutePagingDataTable(cmd, pageIndex, pageSize, parameters);
        }

        /// <summary>
        /// 지정된 조회용 쿼리문을 Paging 정보에 근거해서 수행하고, 결과를 DataTable로 반환한다.
        /// NOTE: 복잡한 쿼리 문장은 안될 가능성이 있습니다.
        /// NOTE: DISTINCT 조회는 지원하지 않습니다.
        /// </summary>
        /// <param name="selectSql">조회용 쿼리 문장 (NOTE: DISTINCT 조회는 지원하지 않습니다.)</param>
        /// <param name="pageIndex">Page index (0부터 시작).  null이면 0으로 간주</param>
        /// <param name="pageSize">Page Size. 한 페이지에 표현할 요소 수 (보통 10개). null이면 <see cref="AdoTool.DefaultPageSize"/>으로 간주</param>
        /// <param name="parameters">조회용 쿼리의 Parameter 정보</param>
        /// <returns>Paging된 DataTable</returns>
        public virtual PagingDataTable ExecutePagingDataTableBySqlString(string selectSql,
                                                                         int? pageIndex,
                                                                         int? pageSize,
                                                                         params IAdoParameter[] parameters) {
            selectSql.ShouldNotBeWhiteSpace("selectSql");

            if(IsDebugEnabled)
                log.Debug("Paging 조회를 수행합니다... selectSql=[{0}], pageIndex=[{1}], pageSize=[{2}], parameters=[{3}]",
                          selectSql, pageIndex, pageSize, parameters.CollectionToString());

            var maxResults = pageSize.GetValueOrDefault(AdoTool.DefaultPageSize);
            var firstResult = pageIndex.GetValueOrDefault(0) * maxResults;

            // 1. Paging 된 정보가 조회하여 DataTable을 구성한다.
            // 2. 전체 갯수를 계산한다.

            var countTask = this.CountAsync(selectSql, parameters);
            var loadTable = ExecuteDataTable(selectSql, firstResult, maxResults, parameters);

            return new PagingDataTable(loadTable,
                                       pageIndex ?? 0,
                                       pageSize ?? AdoTool.DefaultPageSize,
                                       countTask.Result);
        }

        /// <summary>
        /// <paramref name="spName"/> Procedure를 실행하여, Paging 정보에 근거해서 Data를 추출하고, 결과를 DataTable로 반환한다. 
        /// </summary>
        /// <param name="spName">조회용 Procedure Name</param>
        /// <param name="pageIndex">Page Index (0부터 시작).  null이면 0으로 간주</param>
        /// <param name="pageSize">Page Size. 한 페이지에 표현할 요소 수 (보통 10개). null이면 10으로 간주</param>
        /// <param name="parameters">Procedure Parameter 정보</param>
        /// <returns>Paging된 DataTable</returns>
        /// <seealso cref="AdoTool.GetCountingSqlString"/>
        public PagingDataTable ExecutePagingDataTableByProcedure(string spName, int? pageIndex, int? pageSize,
                                                                 params IAdoParameter[] parameters) {
            spName.ShouldNotBeWhiteSpace("spName");

            if(IsDebugEnabled)
                log.Debug(
                    "Procedure의 실행 결과를 PagingTable 에 담습니다... spName=[{0}], pageIndex=[{1}], pageSize=[{2}], parameters=[{3}]",
                    spName, pageIndex, pageSize, parameters.CollectionToString());

            using(var cmd = this.GetProcedureCommand(spName))
                return ExecutePagingDataTable(cmd, pageIndex, pageSize, parameters);
        }

        #endregion

        #region << ExecuteScalar >>

        /// <summary>
        /// 지정된 Command의 ExecuteScalar 메소드를 실행합니다.
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        protected virtual object ExecuteScalarInternal(DbCommand cmd) {
            cmd.ShouldNotBeNull("cmd");

            return (IsActiveTransaction)
                       ? Db.ExecuteScalar(cmd, ActiveTransaction)
                       : Db.ExecuteScalar(cmd);
        }

        /// <summary>
        /// Execute DbCommand, and return single scalar value
        /// </summary>
        /// <param name="cmd">Instance of DbCommand to execute</param>
        /// <param name="parameters">Parameters for DbCommand to execute</param>
        /// <returns>Scalar value</returns>
        public object ExecuteScalar(DbCommand cmd, params IAdoParameter[] parameters) {
            cmd.ShouldNotBeNull("cmd");

            if(IsDebugEnabled)
                log.Debug("Scalar 값을 구합니다... CommandText=[{0}], parameters=[{1}]",
                          cmd.CommandText, parameters.CollectionToString());

            if(parameters.IsNotEmptySequence())
                AdoTool.SetParameterValues(Db, cmd, parameters);

            return ExecuteScalarInternal(cmd);
        }

        /// <summary>
        /// Execute query, and return single scalar value
        /// </summary>
        /// <param name="query">query string ( simple sql string or Procedure name )</param>
        /// <param name="parameters">Parameters for DbCommand</param>
        /// <returns>Scalar value</returns>
        public object ExecuteScalar(string query, params IAdoParameter[] parameters) {
            using(var cmd = this.GetCommand(query))
                return ExecuteScalar(cmd, parameters);
        }

        /// <summary>
        /// Execute sql string, and return single scalar value
        /// </summary>
        /// <param name="sqlString">simple sql string</param>
        /// <param name="parameters">Parameters for DbCommand</param>
        /// <returns>Scalar value</returns>
        public object ExecuteScalarBySqlString(string sqlString, params IAdoParameter[] parameters) {
            using(var cmd = this.GetSqlStringCommand(sqlString, parameters))
                return ExecuteScalar(cmd);
        }

        /// <summary>
        /// Execute stored procedure, and return single scalar value
        /// </summary>
        /// <param name="spName">Procedure name</param>
        /// <param name="parameters">Parameters for DbCommand</param>
        /// <returns>Scalar value</returns>
        public object ExecuteScalarByProcedure(string spName, params IAdoParameter[] parameters) {
            spName.ShouldNotBeWhiteSpace("spName");

            using(var cmd = this.GetProcedureCommand(spName))
                return ExecuteScalar(cmd, parameters);
        }

        #endregion

        #region << ExecuteNonQuery >>

        /// <summary>
        /// 지정된 Command의 ExecuteNonQuery 메소드를 실행합니다.
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        protected virtual int ExecuteNonQueryInternal(DbCommand cmd) {
            cmd.ShouldNotBeNull("cmd");

            return (IsActiveTransaction)
                       ? Db.ExecuteNonQuery(cmd, ActiveTransaction)
                       : Db.ExecuteNonQuery(cmd);
        }

        /// <summary>
        /// Execute specified DbCommand
        /// </summary>
        /// <param name="cmd">Instance of DbCommand to execute</param>
        /// <param name="parameters">Parameters for DbCommand to execute</param>
        /// <returns>affected row count</returns>
        public int ExecuteNonQuery(DbCommand cmd, params IAdoParameter[] parameters) {
            cmd.ShouldNotBeNull("cmd");

            if(IsDebugEnabled)
                log.Debug("Command를 실행합니다... CommandText=[{0}], parameters=[{1}]",
                          cmd.CommandText, parameters.CollectionToString());

            if(parameters.IsNotEmptySequence())
                AdoTool.SetParameterValues(Db, cmd, parameters);

            return ExecuteNonQueryInternal(cmd);
        }

        /// <summary>
        /// Execute specified query or procedure
        /// </summary>
        /// <param name="query">query string ( simple sql string or Procedure name )</param>
        /// <param name="parameters">Parameters for DbCommand</param>
        /// <returns>affected row count</returns>
        public int ExecuteNonQuery(string query, params IAdoParameter[] parameters) {
            using(var cmd = this.GetCommand(query, parameters))
                return ExecuteNonQuery(cmd);
        }

        /// <summary>
        /// Execute specified sql string
        /// </summary>
        /// <param name="sqlString">simple sql string</param>
        /// <param name="parameters">Parameters for DbCommand</param>
        /// <returns>affected row count</returns>
        public int ExecuteNonQueryBySqlString(string sqlString, params IAdoParameter[] parameters) {
            using(var cmd = this.GetSqlStringCommand(sqlString, parameters))
                return ExecuteNonQuery(cmd);
        }

        /// <summary>
        /// Execute specified stored procedure
        /// </summary>
        /// <param name="spName">Procedure name</param>
        /// <param name="parameters">Parameters for DbCommand</param>
        /// <returns>affected row count</returns>
        public int ExecuteNonQueryByProcedure(string spName, params IAdoParameter[] parameters) {
            using(var cmd = this.GetProcedureCommand(spName, parameters))
                return ExecuteNonQuery(cmd);
        }

        #endregion

        #region << ExecuteReader >>

        /// <summary>
        /// 지정된 Command의 ExecuteReader 메소드를 실행합니다.
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        protected virtual IDataReader ExecuteReaderInternal(DbCommand cmd) {
            cmd.ShouldNotBeNull("cmd");

            return (IsActiveTransaction)
                       ? Db.ExecuteReader(cmd, ActiveTransaction)
                       : Db.ExecuteReader(cmd);
        }

        /// <summary>
        /// Execute specified DbCommand, and return <see cref="AdoDataReader"/>
        /// </summary>
        /// <param name="cmd">DbCommand to execute</param>
        /// <param name="parameters">Command parameters</param>
        /// <returns>instance of <see cref="IDataReader"/></returns>
        public IDataReader ExecuteReader(DbCommand cmd, params IAdoParameter[] parameters) {
            cmd.ShouldNotBeNull("cmd");

            if(IsDebugEnabled)
                log.Debug("DataReader를 얻습니다... command text=[{0}], parameters=[{1}]",
                          cmd.CommandText, parameters.CollectionToString());

            if(parameters != null)
                AdoTool.SetParameterValues(Db, cmd, parameters);

            return ExecuteReaderInternal(cmd);
        }

        /// <summary>
        /// Execute specified query, and return <see cref="AdoDataReader"/>
        /// </summary>
        /// <param name="query">query string ( sql string or procedure name )</param>
        /// <param name="parameters">Command parameters</param>
        /// <returns>instance of <see cref="IDataReader"/></returns>
        public virtual IDataReader ExecuteReader(string query, params IAdoParameter[] parameters) {
            using(var cmd = this.GetCommand(query))
                return ExecuteReader(cmd, parameters);
        }

        /// <summary>
        /// Execute specified sql string, and return <see cref="AdoDataReader"/>
        /// </summary>
        /// <param name="sqlString">simple query string</param>
        /// <param name="parameters">Command parameters</param>
        /// <returns>instance of <see cref="IDataReader"/></returns>
        public virtual IDataReader ExecuteReaderBySqlString(string sqlString, params IAdoParameter[] parameters) {
            using(var cmd = this.GetSqlStringCommand(sqlString))
                return ExecuteReader(cmd, parameters);
        }

        /// <summary>
        /// Execute specified stored procedure, and return <see cref="AdoDataReader"/>
        /// </summary>
        /// <param name="spName">procedure name</param>
        /// <param name="parameters">Command parameters</param>
        /// <returns>instance of <see cref="IDataReader"/></returns>
        public virtual IDataReader ExecuteReaderByProcedure(string spName, params IAdoParameter[] parameters) {
            using(var cmd = this.GetProcedureCommand(spName))
                return ExecuteReader(cmd, parameters);
        }

        #endregion
    }
}