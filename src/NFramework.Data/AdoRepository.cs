using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Castle.Core;
using Microsoft.Practices.EnterpriseLibrary.Data;
using NSoft.NFramework.Data.Mappers;
using NSoft.NFramework.Data.Persisters;
using NSoft.NFramework.InversionOfControl;

namespace NSoft.NFramework.Data {
    /// <summary>
    /// 환경설정에서 지정된 IAdoRepository 구현 클래스를 IoC를 통해 Resolve 하여 이용하여 <see cref="IAdoRepository"/> 메소드들을 제공한다.
    /// </summary>
    public static class AdoRepository {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        private static readonly object _syncLock = new object();

        //[ThreadStatic]
        private static IAdoRepository _innerRepository;

        /// <summary>
        /// IoC 에 등록된 IAdoRepository의 인스턴스를 나타냅니다. 
        /// 만약 IoC 환경설정에 정의되어 있지 않다면, <see cref="AdoRepositoryFactory"/>로부터 
        /// 기본 Database를 사용하는 <see cref="AdoRepositoryImpl"/>를 생성합니다.
        /// </summary>
        private static IAdoRepository InnerRepository {
            get {
                if(_innerRepository == null)
                    lock(_syncLock)
                        if(_innerRepository == null) {
                            var repository =
                                IoC.TryResolve<IAdoRepository>(() => AdoRepositoryFactory.Instance.CreateRepositoryByProvider(),
                                                               true,
                                                               LifestyleType.Singleton);

                            System.Threading.Thread.MemoryBarrier();
                            _innerRepository = repository;

                            if(log.IsInfoEnabled)
                                log.Info("IAdoRepository에 해당하는 인스턴스를 생성했습니다. InnerRepository=[{0}]", repository);
                        }

                return _innerRepository;
            }
        }

        /// <summary>
        /// Microsoft Data Acess Application Block의 Database
        /// </summary>
        public static Database Db {
            get { return InnerRepository.Db; }
        }

        /// <summary>
        /// Database ConnectionString name
        /// </summary>
        public static string DbName {
            get { return InnerRepository.DbName; }
        }

        /// <summary>
        /// query 문장을 name-value 형식으로 제공하는 provider입니다. IoC를 이용하여 제공합니다. IoC에서 제공하지 않을 시에는 null 값을 가집니다.
        /// </summary>
        public static IIniQueryProvider QueryProvider {
            get { return InnerRepository.QueryProvider; }
            set {
                // Nothing to do 
                // Wrapper.QueryProvider = value;
            }
        }

        /// <summary>
        /// 현재 스레드 컨텍스트에서 사용하는 IAdoRepository 인스턴스를 말한다. 실질적으로는 Singleton이므로, 그냥 기본 인스턴스를 제공한다.
        /// </summary>
        public static IAdoRepository Current {
            get { return InnerRepository; }
        }

        #region << Transaction >>

        /// <summary>
        /// Current Thread Context에서 활성화된 Transaction. (없다면 null을 반환한다.)
        /// </summary>
        /// <seealso cref="IsActiveTransaction"/>
        public static DbTransaction ActiveTransaction {
            get { return InnerRepository.ActiveTransaction; }
            set { InnerRepository.ActiveTransaction = value; }
        }

        /// <summary>
        /// Is exist active transaction in Current Thread Context.
        /// </summary>
        public static bool IsActiveTransaction {
            get { return InnerRepository.IsActiveTransaction; }
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
        public static DbTransaction BeginTransaction(System.Data.IsolationLevel isolationLevel) {
            return InnerRepository.BeginTransaction(isolationLevel);
        }

        /// <summary>
        /// ADO.NET의 기본 Transaction을 시작한다. 격리수준은 <see cref="IsolationLevel.ReadCommitted"/>이다. 
        /// (TransactionScope와는 달리 DTC를 이용하지 않는다.)
        /// </summary>
        /// <returns>Return a new instance of DbTransaction</returns>
        public static DbTransaction BeginTransaction() {
            return InnerRepository.BeginTransaction();
        }

        /// <summary>
        /// 현재 활성화된 Transaction이 있다면, Commit을 수행한다.
        /// </summary>
        /// <exception cref="InvalidOperationException">Current Thread Context에 활성화된 Transaction이 없을 때</exception>
        public static void Commit() {
            InnerRepository.Commit();
        }

        /// <summary>
        /// 현재 활성화된 Transaction이 있다면, Rollback을 수행한다.
        /// </summary>
        /// <exception cref="InvalidOperationException">Current Thread Context에 활성화된 Transaction이 없을 때</exception>
        public static void Rollback() {
            InnerRepository.Rollback();
        }

        #endregion

        #region << DataAdapter >>

        /// <summary>
        /// Create <see cref="DbDataAdapter"/>
        /// </summary>
        /// <returns>Instance of DbDataAdapter</returns>
        public static DbDataAdapter GetDataAdapter() {
            return InnerRepository.GetDataAdapter();
        }

        #endregion

        #region << Command >>

        /// <summary>
        /// 지정된 쿼리문을 CommendText로 가지는 <see cref="DbCommand"/> 인스턴스를 생성합니다.
        /// </summary>
        /// <param name="query">simple query string or procedure name</param>
        /// <param name="parameters">파라미터 컬렉션</param>
        /// <returns>instance of <see cref="DbCommand"/></returns>
        public static DbCommand GetCommand(string query, params IAdoParameter[] parameters) {
            return InnerRepository.GetCommand(query, parameters);
        }

        /// <summary>
        /// 지정된 쿼리문을 CommendText로 가지는 <see cref="DbCommand"/> 인스턴스를 생성합니다.
        /// </summary>
        /// <param name="query">simple query string</param>
        /// <param name="discoverParams">discover parameters</param>
        /// <returns>instance of <see cref="DbCommand"/></returns>
        public static DbCommand GetCommand(string query, bool discoverParams) {
            return InnerRepository.GetCommand(query, discoverParams);
        }

        /// <summary>
        /// Get <see cref="DbCommand"/> which CommandText is query, build up parameters by parameters
        /// </summary>
        /// <param name="sqlString">simple query string</param>
        /// <param name="parameters">collection of parameter</param>
        /// <returns>instance of <see cref="DbCommand"/></returns>
        public static DbCommand GetSqlStringCommand(string sqlString, params IAdoParameter[] parameters) {
            return InnerRepository.GetSqlStringCommand(sqlString, parameters);
        }

        /// <summary>
        /// 지정된 Stored Procedure를 실행할 <see cref="DbCommand"/> 인스턴스를 생성합니다.
        /// </summary>
        /// <param name="spName">procedure name</param>
        /// <param name="parameters">collection of parameter</param>
        /// <returns>instance of <see cref="DbCommand"/></returns>
        public static DbCommand GetProcedureCommand(string spName, params IAdoParameter[] parameters) {
            return InnerRepository.GetProcedureCommand(spName, parameters);
        }

        /// <summary>
        /// 지정된 Stored Procedure를 실행할 <see cref="DbCommand"/> 인스턴스를 생성합니다.
        /// </summary>
        /// <param name="spName">Procedure name</param>
        /// <param name="discoverParams">DbCommand 인자를 DB에서 확인할 것인지 여부</param>
        /// <param name="parameters">collection of parameter</param>
        /// <returns>instance of <see cref="DbCommand"/></returns>
        public static DbCommand GetProcedureCommand(string spName, bool discoverParams, params IAdoParameter[] parameters) {
            return InnerRepository.GetProcedureCommand(spName, discoverParams, parameters);
        }

        /// <summary>
        /// DataSet 변경 정보를 DB에 한꺼번에 적용하기 위해 Insert, Update, Delete Command를 제공해야 하는데,
        /// 이 때는 DataSet의 Source Column과 Command의 Parameter 이름을 매핑되어 있어야 한다.
        /// 이를 위해 Stored Procudure용 Command의 Parameter Name을 sourceColumns 것으로 교체한다.
        /// </summary>
        /// <param name="spName"></param>
        /// <param name="sourceColumns"></param>
        /// <returns></returns>
        public static DbCommand GetProcedureCommandWithSourceColumn(string spName, params string[] sourceColumns) {
            return InnerRepository.GetProcedureCommandWithSourceColumn(spName, sourceColumns);
        }

        /// <summary>
        /// QueryProvider에서 제공하는 Query로 <see cref="DbCommand"/> 인스턴스를 생성합니다.
        /// </summary>
        /// <param name="queryKey">[Section,] QueryName 형태의 쿼리 키</param>
        /// <param name="parameters">parameter collection</param>
        /// <returns>DbCommand instance</returns>
        /// <exception cref="InvalidOperationException">QueryProvider 속성이 null일때</exception>
        public static DbCommand GetNamedQueryCommand(string queryKey, params IAdoParameter[] parameters) {
            return InnerRepository.GetNamedQueryCommand(queryKey, parameters);
        }

        /// <summary>
        /// QueryProvider에서 제공하는 Query로 <see cref="DbCommand"/> 인스턴스를 생성합니다.
        /// </summary>
        /// <param name="section">Section Name</param>
        /// <param name="queryName">쿼리 명</param>
        /// <param name="parameters">parameter collection</param>
        /// <returns>DbCommand instance</returns>
        /// <exception cref="InvalidOperationException">QueryProvider 속성이 null일 때</exception>
        public static DbCommand GetNamedQueryCommand(string section, string queryName, params IAdoParameter[] parameters) {
            return InnerRepository.GetNamedQueryCommand(section, queryName, parameters);
        }

        #endregion

        #region << Load DataSet >>

        /// <summary>
        /// 지정된 DataAdapter를 통해 얻은 정보를 DataSet으로 빌드한다.
        /// </summary>
        /// <param name="da">DataAdapter</param>
        /// <param name="tableName">Table name</param>
        /// <param name="targetDataSet">저장할 Dataset</param>
        /// <param name="firstResult">첫번째 레코드의 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">최대 레코드 수 (0 이면 무시하고, 마지막 레코드까지 가져온다</param>
        public static void LoadDataSet(DbDataAdapter da, string tableName, DataSet targetDataSet, int firstResult = 0,
                                       int maxResults = 0) {
            InnerRepository.LoadDataSet(da, tableName, targetDataSet, firstResult, maxResults);
        }

        #endregion

        #region << Execute DataSet >>

        /// <summary>
        /// 지정된 DataAdpater를 실행하여 DataSet을 빌드한다.
        /// </summary>
        /// <param name="cmd">실행할 <see cref="DbCommand"/> instance.</param>
        /// <param name="firstResult">첫번째 레코드의 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">최대 레코드 수 (0 이면 무시하고, 마지막 레코드까지 가져온다</param>
        /// <param name="parameters">collectio of parameters of Command</param>
        /// <returns>결과 셋이 담긴 DataSet</returns>
        public static DataSet ExecuteDataSet(DbCommand cmd, int firstResult, int maxResults, params IAdoParameter[] parameters) {
            return InnerRepository.ExecuteDataSet(cmd, firstResult, maxResults, parameters);
        }

        /// <summary>
        /// 지정된 DataAdpater를 실행하여 DataSet을 빌드한다.
        /// </summary>
        /// <param name="cmd">실행할 <see cref="DbCommand"/> instance.</param>
        /// <param name="parameters">collectio of parameters of Command</param>
        /// <returns>결과 셋이 담긴 DataSet</returns>
        public static DataSet ExecuteDataSet(DbCommand cmd, params IAdoParameter[] parameters) {
            return InnerRepository.ExecuteDataSet(cmd, parameters);
        }

        /// <summary>
        /// <paramref name="query"/> 문을 실행하여, DataSet을 빌드합니다.
        /// </summary>
        /// <param name="query">실행할 쿼림 문 또는 Procedure 명</param>
        /// <param name="firstResult">첫번째 레코드의 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">최대 레코드 수 (0 이면 무시하고, 마지막 레코드까지 가져온다)</param>		
        /// <param name="parameters">parameter collection</param>
        /// <returns>결과 셋</returns>
        public static DataSet ExecuteDataSet(string query, int firstResult, int maxResults, params IAdoParameter[] parameters) {
            return InnerRepository.ExecuteDataSet(query, firstResult, maxResults, parameters);
        }

        /// <summary>
        /// <paramref name="query"/> 문을 실행하여, DataSet을 빌드합니다.
        /// </summary>
        /// <param name="query">실행할 쿼림 문 또는 Procedure 명</param>
        /// <param name="parameters">parameter collection</param>
        /// <returns>결과 셋</returns>
        public static DataSet ExecuteDataSet(string query, params IAdoParameter[] parameters) {
            return InnerRepository.ExecuteDataSet(query, parameters);
        }

        /// <summary>
        /// 지정된 DataAdpater를 실행하여 DataSet을 빌드한다.
        /// </summary>
        /// <param name="sqlString">simple query string to execute</param>
        /// <param name="firstResult">첫번째 레코드의 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">최대 레코드 수 (0 이면 무시하고, 마지막 레코드까지 가져온다</param>
        /// <param name="parameters">collectio of parameters of Command</param>
        /// <returns>결과 셋이 담긴 DataSet</returns>
        public static DataSet ExecuteDataSetBySqlString(string sqlString, int firstResult, int maxResults,
                                                        params IAdoParameter[] parameters) {
            return InnerRepository.ExecuteDataSetBySqlString(sqlString, firstResult, maxResults, parameters);
        }

        /// <summary>
        /// 지정된 DataAdpater를 실행하여 DataSet을 빌드한다.
        /// </summary>
        /// <param name="sqlString">simple query string to execute</param>
        /// <param name="parameters">collectio of parameters of Command</param>
        /// <returns>결과 셋이 담긴 DataSet</returns>
        public static DataSet ExecuteDataSetBySqlString(string sqlString, params IAdoParameter[] parameters) {
            return InnerRepository.ExecuteDataSetBySqlString(sqlString, parameters);
        }

        /// <summary>
        /// 지정된 DataAdpater를 실행하여 DataSet을 빌드한다.
        /// </summary>
        /// <param name="spName">Procedure name to execute</param>
        /// <param name="firstResult">첫번째 레코드의 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">최대 레코드 수 (0 이면 무시하고, 마지막 레코드까지 가져온다</param>
        /// <param name="parameters">collectio of parameters of Command</param>
        /// <returns>결과 셋이 담긴 DataSet</returns>
        public static DataSet ExecuteDataSetByProcedure(string spName, int firstResult, int maxResults,
                                                        params IAdoParameter[] parameters) {
            return InnerRepository.ExecuteDataSetByProcedure(spName, firstResult, maxResults, parameters);
        }

        /// <summary>
        /// 지정된 DataAdpater를 실행하여 DataSet을 빌드한다.
        /// </summary>
        /// <param name="spName">Procedure name to execute</param>
        /// <param name="parameters">collectio of parameters of Command</param>
        /// <returns>결과 셋이 담긴 DataSet</returns>
        public static DataSet ExecuteDataSetByProcedure(string spName, params IAdoParameter[] parameters) {
            return InnerRepository.ExecuteDataSetByProcedure(spName, parameters);
        }

        #endregion

        #region << Execute DataTable >>

        /// <summary>
        /// 지정된 Command를 실행하여, DataTable을 빌드합니다.
        /// </summary>
        /// <param name="cmd">실행할 Select용 Command</param>
        /// <param name="firstResult">첫번째 레코드의 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">최대 레코드 수 (0 이면 무시하고, 마지막 레코드까지 가져온다</param>
        /// <param name="parameters">DbCommand의 Parameter 정보</param>
        /// <returns>instance of <see cref="DataTable"/></returns>
        public static DataTable ExecuteDataTable(DbCommand cmd, int firstResult, int maxResults,
                                                 params IAdoParameter[] parameters) {
            return InnerRepository.ExecuteDataTable(cmd, firstResult, maxResults, parameters);
        }

        /// <summary>
        /// 지정된 Command를 실행하여, DataTable을 빌드합니다.
        /// </summary>
        /// <param name="cmd">실행할 Select용 Command</param>
        /// <param name="parameters">DbCommand의 Parameter 정보</param>
        /// <returns>instance of <see cref="DataTable"/></returns>
        public static DataTable ExecuteDataTable(DbCommand cmd, params IAdoParameter[] parameters) {
            return InnerRepository.ExecuteDataTable(cmd, parameters);
        }

        /// <summary>
        /// <paramref name="query"/> 문을 실행하여, DataTable을 빌드합니다.
        /// </summary>
        /// <param name="query">실행할 쿼림 문 또는 Procedure 명</param>
        /// <param name="firstResult">첫번째 레코드의 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">최대 레코드 수 (0 이면 무시하고, 마지막 레코드까지 가져온다)</param>		
        /// <param name="parameters">parameter collection</param>
        /// <returns>결과 셋</returns>
        public static DataTable ExecuteDataTable(string query,
                                                 int firstResult, int maxResults,
                                                 params IAdoParameter[] parameters) {
            return InnerRepository.ExecuteDataTable(query, firstResult, maxResults, parameters);
        }

        /// <summary>
        /// <paramref name="query"/> 문을 실행하여, DataTable을 빌드합니다.
        /// </summary>
        /// <param name="query">실행할 쿼림 문 또는 Procedure 명</param>
        /// <param name="parameters">parameter collection</param>
        /// <returns>결과 셋</returns>
        public static DataTable ExecuteDataTable(string query, params IAdoParameter[] parameters) {
            return InnerRepository.ExecuteDataTable(query, parameters);
        }

        /// <summary>
        /// 지정된 sql string 문을 실행하여, DataTable을 빌드합니다.
        /// </summary>
        /// <param name="sqlString">실행할 Query 문</param>
        /// <param name="firstResult">첫번째 레코드의 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">최대 레코드 수 (0 이면 무시하고, 마지막 레코드까지 가져온다</param>
        /// <param name="parameters">DbCommand의 Parameter 정보</param>
        /// <returns>instance of <see cref="DataTable"/></returns>
        public static DataTable ExecuteDataTableBySqlString(string sqlString,
                                                            int firstResult, int maxResults,
                                                            params IAdoParameter[] parameters) {
            return InnerRepository.ExecuteDataTableBySqlString(sqlString, firstResult, maxResults, parameters);
        }

        /// <summary>
        /// 지정된 sql string 문을 실행하여, DataTable을 빌드합니다.
        /// </summary>
        /// <param name="sqlString">실행할 Query 문</param>
        /// <param name="parameters">DbCommand의 Parameter 정보</param>
        /// <returns>instance of <see cref="DataTable"/></returns>
        public static DataTable ExecuteDataTableBySqlString(string sqlString, params IAdoParameter[] parameters) {
            return InnerRepository.ExecuteDataTableBySqlString(sqlString, parameters);
        }

        /// <summary>
        /// 지정된 Procedure를 실행하여, DataTable을 빌드합니다.
        /// </summary>
        /// <param name="spName">실행할 Procedure 명</param>
        /// <param name="firstResult">첫번째 레코드의 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">최대 레코드 수 (0 이면 무시하고, 마지막 레코드까지 가져온다</param>
        /// <param name="parameters">DbCommand의 Parameter 정보</param>
        /// <returns>instance of <see cref="DataTable"/></returns>
        public static DataTable ExecuteDataTableByProcedure(string spName, int firstResult, int maxResults,
                                                            params IAdoParameter[] parameters) {
            return InnerRepository.ExecuteDataTableByProcedure(spName, firstResult, maxResults, parameters);
        }

        /// <summary>
        /// 지정된 Procedure를 실행하여, DataTable을 빌드합니다.
        /// </summary>
        /// <param name="spName">실행할 Select Command</param>
        /// <param name="parameters">DbCommand의 Parameter 정보</param>
        /// <returns>instance of <see cref="DataTable"/></returns>
        public static DataTable ExecuteDataTableByProcedure(string spName, params IAdoParameter[] parameters) {
            return InnerRepository.ExecuteDataTableByProcedure(spName, parameters);
        }

        #endregion

        #region << Execute DataTable AsList >>

        /// <summary>
        /// 여러 ResultSet을 반환할 수 있으므로, DataTable의 컬렉션으로 반환합니다.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static IList<DataTable> ExecuteDataTableAsList(DbCommand cmd, params IAdoParameter[] parameters) {
            return InnerRepository.ExecuteDataTableAsList(cmd, parameters);
        }

        /// <summary>
        /// 여러 ResultSet을 반환할 수 있으므로, DataTable의 컬렉션으로 반환합니다.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="firstResult"></param>
        /// <param name="maxResults"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static IList<DataTable> ExecuteDataTableAsList(DbCommand cmd, int firstResult, int maxResults,
                                                              params IAdoParameter[] parameters) {
            return InnerRepository.ExecuteDataTableAsList(cmd, firstResult, maxResults, parameters);
        }

        #endregion

        #region << Execute Paging DataTable >>

        /// <summary>
        /// 지정된 Command 를 Paging 정보에 근거해서 수행하고, 결과를 DataTable로 반환한다.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="pageIndex">Page index (0부터 시작).  null이면 0으로 간주</param>
        /// <param name="pageSize">Page Size. 한 페이지에 표현할 요소 수 (보통 10개). null이면 10으로 간주</param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static PagingDataTable ExecutePagingDataTable(DbCommand cmd, int? pageIndex, int? pageSize,
                                                             params IAdoParameter[] parameters) {
            return InnerRepository.ExecutePagingDataTable(cmd, pageIndex, pageSize, parameters);
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
        public static PagingDataTable ExecutePagingDataTable(string query, int? pageIndex, int? pageSize,
                                                             params IAdoParameter[] parameters) {
            return InnerRepository.ExecutePagingDataTable(query, pageIndex, pageSize, parameters);
        }

        /// <summary>
        /// 지정된 조회용 쿼리문을 Paging 정보에 근거해서 수행하고, 결과를 DataTable로 반환한다.
        /// NOTE: 복잡한 쿼리 문장은 안될 가능성이 있습니다.
        /// NOTE: DISTINCT 조회는 지원하지 않습니다.
        /// </summary>
        /// <param name="selectSql">조회용 쿼리 문장 (NOTE: DISTINCT 조회는 지원하지 않습니다.)</param>
        /// <param name="pageIndex">Page index (0부터 시작).  null이면 0으로 간주</param>
        /// <param name="pageSize">Page Size. 한 페이지에 표현할 요소 수 (보통 10개). null이면 10으로 간주</param>
        /// <param name="parameters">조회용 쿼리의 Parameter 정보</param>
        /// <returns>Paging된 DataTable</returns>
        public static PagingDataTable ExecutePagingDataTableBySqlString(string selectSql, int? pageIndex, int? pageSize,
                                                                        params IAdoParameter[] parameters) {
            return InnerRepository.ExecutePagingDataTableBySqlString(selectSql, pageIndex, pageSize, parameters);
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
        public static PagingDataTable ExecutePagingDataTableByProcedure(string spName, int? pageIndex, int? pageSize,
                                                                        params IAdoParameter[] parameters) {
            return InnerRepository.ExecutePagingDataTableByProcedure(spName, pageIndex, pageSize, parameters);
        }

        #endregion

        #region << Execute Scalar >>

        /// <summary>
        /// Execute DbCommand, and return single scalar value
        /// </summary>
        /// <param name="cmd">Instance of DbCommand to execute</param>
        /// <param name="parameters">Parameters for DbCommand to execute</param>
        /// <returns>Scalar value</returns>
        public static object ExecuteScalar(DbCommand cmd, params IAdoParameter[] parameters) {
            return InnerRepository.ExecuteScalar(cmd, parameters);
        }

        /// <summary>
        /// Execute query, and return single scalar value
        /// </summary>
        /// <param name="query">query string ( simple sql string or Procedure name )</param>
        /// <param name="parameters">Parameters for DbCommand</param>
        /// <returns>Scalar value</returns>
        public static object ExecuteScalar(string query, params IAdoParameter[] parameters) {
            return InnerRepository.ExecuteScalar(query, parameters);
        }

        /// <summary>
        /// Execute sql string, and return single scalar value
        /// </summary>
        /// <param name="sqlString">simple sql string</param>
        /// <param name="parameters">Parameters for DbCommand</param>
        /// <returns>Scalar value</returns>
        public static object ExecuteScalarBySqlString(string sqlString, params IAdoParameter[] parameters) {
            return InnerRepository.ExecuteScalarBySqlString(sqlString, parameters);
        }

        /// <summary>
        /// Execute stored procedure, and return single scalar value
        /// </summary>
        /// <param name="spName">Procedure name</param>
        /// <param name="parameters">Parameters for DbCommand</param>
        /// <returns>Scalar value</returns>
        public static object ExecuteScalarByProcedure(string spName, params IAdoParameter[] parameters) {
            return InnerRepository.ExecuteScalarByProcedure(spName, parameters);
        }

        #endregion

        #region << Execute Non Query >>

        /// <summary>
        /// Execute specified DbCommand
        /// </summary>
        /// <param name="cmd">Instance of DbCommand to execute</param>
        /// <param name="parameters">Parameters for DbCommand to execute</param>
        /// <returns>affected row count</returns>
        public static int ExecuteNonQuery(DbCommand cmd, params IAdoParameter[] parameters) {
            return InnerRepository.ExecuteNonQuery(cmd, parameters);
        }

        /// <summary>
        /// Execute specified query or procedure
        /// </summary>
        /// <param name="query">query string ( simple sql string or Procedure name )</param>
        /// <param name="parameters">Parameters for DbCommand</param>
        /// <returns>affected row count</returns>
        public static int ExecuteNonQuery(string query, params IAdoParameter[] parameters) {
            return InnerRepository.ExecuteNonQuery(query, parameters);
        }

        /// <summary>
        /// Execute specified sql string
        /// </summary>
        /// <param name="sqlString">simple sql string</param>
        /// <param name="parameters">Parameters for DbCommand</param>
        /// <returns>affected row count</returns>
        public static int ExecuteNonQueryBySqlString(string sqlString, params IAdoParameter[] parameters) {
            return InnerRepository.ExecuteNonQueryBySqlString(sqlString, parameters);
        }

        /// <summary>
        /// Execute specified stored procedure
        /// </summary>
        /// <param name="spName">Procedure name</param>
        /// <param name="parameters">Parameters for DbCommand</param>
        /// <returns>affected row count</returns>
        public static int ExecuteNonQueryByProcedure(string spName, params IAdoParameter[] parameters) {
            return InnerRepository.ExecuteNonQueryByProcedure(spName, parameters);
        }

        #endregion

        #region << Execute Reader >>

        /// <summary>
        /// Execute specified DbCommand, and return <see cref="IDataReader"/>
        /// </summary>
        /// <param name="cmd">DbCommand to execute</param>
        /// <param name="parameters">Command parameters</param>
        /// <returns>instance of <see cref="IDataReader"/></returns>
        public static IDataReader ExecuteReader(DbCommand cmd, params IAdoParameter[] parameters) {
            return InnerRepository.ExecuteReader(cmd, parameters);
        }

        /// <summary>
        /// Execute specified query, and return  <see cref="IDataReader"/>
        /// </summary>
        /// <param name="query">query string ( sql string or procedure name )</param>
        /// <param name="parameters">Command parameters</param>
        /// <returns>instance of <see cref="IDataReader"/></returns>
        public static IDataReader ExecuteReader(string query, params IAdoParameter[] parameters) {
            return InnerRepository.ExecuteReader(query, parameters);
        }

        /// <summary>
        /// Execute specified sql string, and return  <see cref="IDataReader"/>
        /// </summary>
        /// <param name="sqlString">simple query string</param>
        /// <param name="parameters">Command parameters</param>
        /// <returns>instance of <see cref="IDataReader"/></returns>
        public static IDataReader ExecuteReaderBySqlString(string sqlString, params IAdoParameter[] parameters) {
            return InnerRepository.ExecuteReaderBySqlString(sqlString, parameters);
        }

        /// <summary>
        /// Execute specified stored procedure, and return <see cref="IDataReader"/>
        /// </summary>
        /// <param name="spName">procedure name</param>
        /// <param name="parameters">Command parameters</param>
        /// <returns>instance of <see cref="IDataReader"/></returns>
        public static IDataReader ExecuteReaderByProcedure(string spName, params IAdoParameter[] parameters) {
            return InnerRepository.ExecuteReaderByProcedure(spName, parameters);
        }

        #endregion

        #region << Count Of DataReader >>

        /// <summary>
        /// 지정된 Command를 실행한 결과 셋의 레코드 갯수를 구한다.
        /// </summary>
        /// <param name="cmd">실행할 Command 객체</param>
        /// <param name="parameters">Command 인자 정보</param>
        /// <returns>결과 셋의 레코드 수</returns>
        /// <remarks>
        /// 실제 SQL의 count(*) 함수를 생성하는 것이 아니라, IDataReader를 이용하여 결과 셋을 가져와서 갯수만 센다.
        /// 장점은 DataSet을 이용하여 Paging하는 것보다 빠르고, Count 용 Query문을 따로 만들 필요가 없다.
        /// 단점은 SQL의 count(*) 함수보다는 당연히 느리다.
        /// </remarks>
        public static int Count(DbCommand cmd, params IAdoParameter[] parameters) {
            return InnerRepository.Count(cmd, parameters);
        }

        /// <summary>
        /// 지정된 쿼리 문을 실행한 결과 셋의 레코드 갯수를 구한다.
        /// </summary>
        /// <param name="query">실행할 Command 객체</param>
        /// <param name="parameters">Command 인자 정보</param>
        /// <returns>결과 셋의 레코드 수</returns>
        /// <remarks>
        /// 실제 SQL의 count(*) 함수를 생성하는 것이 아니라, IDataReader를 이용하여 결과 셋을 가져와서 갯수만 센다.
        /// 장점은 DataSet을 이용하여 Paging하는 것보다 빠르고, Count 용 Query문을 따로 만들 필요가 없다.
        /// 단점은 SQL의 count(*) 함수보다는 당연히 느리다.
        /// </remarks>
        public static int Count(string query, params IAdoParameter[] parameters) {
            return InnerRepository.Count(query, parameters);
        }

        /// <summary>
        /// 지정된 쿼리 문을 실행한 결과 셋의 레코드 갯수를 구한다.
        /// </summary>
        /// <param name="sqlString">실행할 Command 객체</param>
        /// <param name="parameters">Command 인자 정보</param>
        /// <returns>결과 셋의 레코드 수</returns>
        /// <remarks>
        /// 실제 SQL의 count(*) 함수를 생성하는 것이 아니라, IDataReader를 이용하여 결과 셋을 가져와서 갯수만 센다.
        /// 장점은 DataSet을 이용하여 Paging하는 것보다 빠르고, Count 용 Query문을 따로 만들 필요가 없다.
        /// 단점은 SQL의 count(*) 함수보다는 당연히 느리다.
        /// </remarks>
        public static int CountBySqlString(string sqlString, params IAdoParameter[] parameters) {
            return InnerRepository.CountBySqlString(sqlString, parameters);
        }

        /// <summary>
        /// 지정된 쿼리 문을 실행한 결과 셋의 레코드 갯수를 구한다.
        /// </summary>
        /// <param name="spName">실행할 Command 객체</param>
        /// <param name="parameters">Command 인자 정보</param>
        /// <returns>결과 셋의 레코드 수</returns>
        /// <remarks>
        /// 실제 SQL의 count(*) 함수를 생성하는 것이 아니라, IDataReader를 이용하여 결과 셋을 가져와서 갯수만 센다.
        /// 장점은 DataSet을 이용하여 Paging하는 것보다 빠르고, Count 용 Query문을 따로 만들 필요가 없다.
        /// 단점은 SQL의 count(*) 함수보다는 당연히 느리다.
        /// </remarks>
        public static int CountByProcedure(string spName, params IAdoParameter[] parameters) {
            return InnerRepository.CountByProcedure(spName, parameters);
        }

        #endregion

        #region << Exists >>

        /// <summary>
        /// 지정한 Command의 실행 결과 셋이 존재하는지 검사한다 (결과 셋의 레코드가 하나 이상이다)
        /// </summary>
        public static bool Exists(DbCommand cmd, params IAdoParameter[] parameters) {
            return InnerRepository.Exists(cmd, parameters);
        }

        /// <summary>
        /// 지정한 쿼리 문의 실행 결과 셋이 존재하는지 검사한다 (결과 셋의 레코드가 하나 이상이다)
        /// </summary>
        public static bool Exists(string query, params IAdoParameter[] parameters) {
            return InnerRepository.Exists(query, parameters);
        }

        /// <summary>
        /// 지정한 쿼리 문의 실행 결과 셋이 존재하는지 검사한다 (결과 셋의 레코드가 하나 이상이다)
        /// </summary>
        public static bool ExistsBySqlString(string sqlString, params IAdoParameter[] parameters) {
            return InnerRepository.ExistsBySqlString(sqlString, parameters);
        }

        /// <summary>
        /// 지정한 Procedure의 실행 결과 셋이 존재하는지 검사한다 (결과 셋의 레코드가 하나 이상이다)
        /// </summary>
        public static bool ExistsByProcedure(string spName, params IAdoParameter[] parameters) {
            return InnerRepository.ExistsByProcedure(spName, parameters);
        }

        #endregion

        #region << DataInstance >>

        ///<summary>
        /// DbCommand를 실행해 얻은 DataReader를 통해 지정된 형식의 인스턴스를 만든다.
        ///</summary>
        /// <typeparam name="T">Type of Persistent object</typeparam>
        /// <param name="nameMapper">컬럼명과 속성명 Mapper</param>
        /// <param name="cmd">실행할 DbCommand</param>
        /// <param name="parameters">Procedure 인자</param>
        /// <returns>Collection of Persistet object</returns>
        public static IList<T> ExecuteInstance<T>(INameMapper nameMapper, DbCommand cmd, params IAdoParameter[] parameters)
            where T : class {
            return InnerRepository.ExecuteInstance<T>(nameMapper, cmd, parameters);
        }

        ///<summary>
        /// DbCommand를 실행해 얻은 DataReader를 통해 지정된 형식의 인스턴스를 만든다.
        ///</summary>
        /// <typeparam name="T">Type of Persistent object</typeparam>
        /// <param name="nameMapper">컬럼명과 속성명 Mapper</param>
        /// <param name="additionalMapping">추가적인 매핑 함수</param>
        /// <param name="cmd">실행할 DbCommand</param>
        /// <param name="parameters">Procedure 인자</param>
        /// <returns>Collection of Persistet object</returns>
        public static IList<T> ExecuteInstance<T>(INameMapper nameMapper, Action<IDataReader, T> additionalMapping, DbCommand cmd,
                                                  params IAdoParameter[] parameters) where T : class {
            return InnerRepository.ExecuteInstance<T>(nameMapper, additionalMapping, cmd, parameters);
        }

        /// <summary>
        /// Execute DbCommand, Build instance of specified type by mapping DataReader Column Value to Instance Property Value
        /// </summary>
        /// <typeparam name="T">Type of Persistent object</typeparam>
        /// <param name="nameMaps">Key = ColumnName of DataReader, Value = Property Name of Specifieid Type</param>
        /// <param name="cmd">Instance of DbCommand to executed</param>
        /// <param name="parameters">Command parameters</param>
        /// <returns>Collection of Persistet object</returns>
        public static IList<T> ExecuteInstance<T>(INameMap nameMaps, DbCommand cmd, params IAdoParameter[] parameters) where T : class {
            return InnerRepository.ExecuteInstance<T>(nameMaps, cmd, parameters);
        }

        /// <summary>
        /// Execute DbCommand, Build instance of specified type by mapping DataReader Column Value to Instance Property Value
        /// </summary>
        /// <typeparam name="T">Type of Persistent object</typeparam>
        /// <param name="nameMaps">Key = ColumnName of DataReader, Value = Property Name of Specifieid Type</param>
        /// <param name="additionalMapping">추가적인 매핑 함수</param>
        /// <param name="cmd">Instance of DbCommand to executed</param>
        /// <param name="parameters">Command parameters</param>
        /// <returns>Collection of Persistet object</returns>
        public static IList<T> ExecuteInstance<T>(INameMap nameMaps, Action<IDataReader, T> additionalMapping, DbCommand cmd,
                                                  params IAdoParameter[] parameters) where T : class {
            return InnerRepository.ExecuteInstance<T>(nameMaps, additionalMapping, cmd, parameters);
        }

        /// <summary>
        /// Execute DbCommand, Build instance of specified type from IDataReader using Persister
        /// </summary>
        /// <typeparam name="T">Type of persistent object</typeparam>
        /// <param name="persister">Persister from IDataReader</param>
        /// <param name="cmd">Instance of DbCommand to executed</param>
        /// <param name="parameters">Parameters for DbCommand to execute</param>
        /// <returns>Collection of Persistent Object</returns>
        public static IList<T> ExecuteInstance<T>(IReaderPersister<T> persister, DbCommand cmd, params IAdoParameter[] parameters)
            where T : class {
            return InnerRepository.ExecuteInstance(persister, cmd, parameters);
        }

        /// <summary>
        /// DbCommand를 실행해 얻은 DataReader를 Converter를 통해 지정된 형식의 인스턴스를 만든다.
        /// </summary>
        /// <typeparam name="T">Type of persistent object</typeparam>
        /// <param name="mapFunc">IDataReader의 한 레코드인 IDataRecord 정보를 가지고, Persistent Object를 만들 Converter</param>
        /// <param name="cmd">Instance of DbCommand to executed</param>
        /// <param name="parameters">Parameters for DbCommand to execute</param>
        /// <returns>Collection of Persistent Object</returns>
        public static IList<T> ExecuteInstance<T>(Func<IDataReader, T> mapFunc, DbCommand cmd, params IAdoParameter[] parameters) {
            return InnerRepository.ExecuteInstance<T>(mapFunc, cmd, parameters);
        }

        #endregion

        #region << DataInstance for Paging >>

        /// <summary>
        /// 지정된 Command를 실행하여, 결과 셋을 Paging하여, 지정된 Page에 해당하는 정보만 Persistent Object로 빌드하여 반환한다.
        /// </summary>
        /// <typeparam name="T">Persistent Object의 수형</typeparam>
        /// <param name="nameMapper">컬럼명:속성명의 매핑정보를 가진 Mapper</param>
        /// <param name="cmd">실행할 DbCommand</param>
        /// <param name="pageIndex">결과셋의 Page Index (0부터 시작)</param>
        /// <param name="pageSize">결과셋 Paging 시의 Page Size</param>
        /// <param name="parameters">DbCommand 실행시의 Parameter 정보</param>
        /// <returns>Paging된 Persistent Object의 List</returns>
        public static IPagingList<T> ExecuteInstance<T>(INameMapper nameMapper,
                                                        DbCommand cmd,
                                                        int pageIndex,
                                                        int pageSize,
                                                        params IAdoParameter[] parameters)
            where T : class {
            return InnerRepository.ExecuteInstance<T>(nameMapper, cmd, pageIndex, pageSize, parameters);
        }

        /// <summary>
        /// 지정된 Command를 실행하여, 결과 셋을 Paging하여, 지정된 Page에 해당하는 정보만 Persistent Object로 빌드하여 반환한다.
        /// </summary>
        /// <typeparam name="T">Persistent Object의 수형</typeparam>
        /// <param name="nameMapper">컬럼명:속성명의 매핑정보를 가진 Mapper</param>
        /// <param name="additionalMapping">추가적인 매핑 함수</param>
        /// <param name="cmd">실행할 DbCommand</param>
        /// <param name="pageIndex">결과셋의 Page Index (0부터 시작)</param>
        /// <param name="pageSize">결과셋 Paging 시의 Page Size</param>
        /// <param name="parameters">DbCommand 실행시의 Parameter 정보</param>
        /// <returns>Paging된 Persistent Object의 List</returns>
        public static IPagingList<T> ExecuteInstance<T>(INameMapper nameMapper,
                                                        Action<IDataReader, T> additionalMapping,
                                                        DbCommand cmd,
                                                        int pageIndex,
                                                        int pageSize,
                                                        params IAdoParameter[] parameters) where T : class {
            return InnerRepository.ExecuteInstance<T>(nameMapper, additionalMapping, cmd, pageIndex, pageSize, parameters);
        }

        /// <summary>
        /// 지정된 Command를 실행하여, 결과 셋을 Paging하여, 지정된 Page에 해당하는 정보만 Persistent Object로 빌드하여 반환한다.
        /// </summary>
        /// <typeparam name="T">Persistent Object의 수형</typeparam>
        /// <param name="nameMaps">컬럼명:속성명의 매핑정보</param>
        /// <param name="cmd">실행할 DbCommand</param>
        /// <param name="pageIndex">결과셋의 Page Index (0부터 시작)</param>
        /// <param name="pageSize">결과셋 Paging 시의 Page Size</param>
        /// <param name="parameters">DbCommand 실행시의 Parameter 정보</param>
        /// <returns>Paging된 Persistent Object의 List</returns>
        public static IPagingList<T> ExecuteInstance<T>(INameMap nameMaps,
                                                        DbCommand cmd,
                                                        int pageIndex,
                                                        int pageSize,
                                                        params IAdoParameter[] parameters)
            where T : class {
            return InnerRepository.ExecuteInstance<T>(nameMaps, cmd, pageIndex, pageSize, parameters);
        }

        /// <summary>
        /// 지정된 Command를 실행하여, 결과 셋을 Paging하여, 지정된 Page에 해당하는 정보만 Persistent Object로 빌드하여 반환한다.
        /// </summary>
        /// <typeparam name="T">Persistent Object의 수형</typeparam>
        /// <param name="nameMaps">컬럼명:속성명의 매핑정보</param>
        /// <param name="additionalMapping">추가적인 매핑 함수</param>
        /// <param name="cmd">실행할 DbCommand</param>
        /// <param name="pageIndex">결과셋의 Page Index (0부터 시작)</param>
        /// <param name="pageSize">결과셋 Paging 시의 Page Size</param>
        /// <param name="parameters">DbCommand 실행시의 Parameter 정보</param>
        /// <returns>Paging된 Persistent Object의 List</returns>
        public static IPagingList<T> ExecuteInstance<T>(INameMap nameMaps,
                                                        Action<IDataReader, T> additionalMapping,
                                                        DbCommand cmd,
                                                        int pageIndex,
                                                        int pageSize,
                                                        params IAdoParameter[] parameters) where T : class {
            return InnerRepository.ExecuteInstance<T>(nameMaps, additionalMapping, cmd, pageIndex, pageSize, parameters);
        }

        /// <summary>
        /// 지정된 Command를 실행하여, 결과 셋을 Paging하여, 지정된 Page에 해당하는 정보만 Persistent Object로 빌드하여 반환한다.
        /// </summary>
        /// <typeparam name="T">Persistent Object의 수형</typeparam>
        /// <param name="persister">IDataReader로부터 Persistent Object를 빌드하는 Persister</param>
        /// <param name="cmd">실행할 DbCommand</param>
        /// <param name="pageIndex">결과셋의 Page Index (0부터 시작)</param>
        /// <param name="pageSize">결과셋 Paging 시의 Page Size</param>
        /// <param name="parameters">DbCommand 실행시의 Parameter 정보</param>
        /// <returns>Paging된 Persistent Object의 List</returns>
        public static IPagingList<T> ExecuteInstance<T>(IReaderPersister<T> persister,
                                                        DbCommand cmd,
                                                        int pageIndex,
                                                        int pageSize,
                                                        params IAdoParameter[] parameters)
            where T : class {
            return InnerRepository.ExecuteInstance(persister, cmd, pageIndex, pageSize, parameters);
        }

        /// <summary>
        /// 지정된 Command를 실행하여, 결과 셋을 Paging하여, 지정된 Page에 해당하는 정보만 Persistent Object로 빌드하여 반환한다.
        /// </summary>
        /// <typeparam name="T">Persistent Object의 수형</typeparam>
        /// <param name="mapFunc">IDataReader로부터 Persistent Object를 빌드하는 Mapping function</param>
        /// <param name="cmd">실행할 DbCommand</param>
        /// <param name="pageIndex">결과셋의 Page Index (0부터 시작)</param>
        /// <param name="pageSize">결과셋 Paging 시의 Page Size</param>
        /// <param name="parameters">DbCommand 실행시의 Parameter 정보</param>
        /// <returns>Paging된 Persistent Object의 List</returns>
        public static IPagingList<T> ExecuteInstance<T>(Func<IDataReader, T> mapFunc,
                                                        DbCommand cmd,
                                                        int pageIndex,
                                                        int pageSize,
                                                        params IAdoParameter[] parameters)
            where T : class {
            return InnerRepository.ExecuteInstance<T>(mapFunc, cmd, pageIndex, pageSize, parameters);
        }

        #endregion

        #region << Execute Command / Procedure >>

        /// <summary>
        /// 지정된 Command를 수행하고, RETURN_VALUE를 반환합니다.
        /// </summary>
        /// <param name="cmd">DbCommand 인스턴스</param>
        /// <param name="parameters">인자</param>
        /// <returns>Procedure인 경우 return value를 반환한다. 반환값이 없으면 0을 반환한다.</returns>
        public static object ExecuteCommand(DbCommand cmd, params IAdoParameter[] parameters) {
            return InnerRepository.ExecuteCommand(cmd, parameters);
        }

        /// <summary>
        /// Stored Procedure를 실행하고, Parameter의 Direction이 INPUT이 아닌 Parameter들을 반환한다. (InputOutput, Output, ReturnValue)
        /// </summary>
        /// <param name="spName">실행할 Procedure 이름</param>
        /// <param name="parameters">Collection of parameter for DbCommand</param>
        /// <returns>INPUT을 제외한 Oupput, InputOutput, ReturnValue에 해당하는 Parameter 정보</returns>
        public static IAdoParameter[] ExecuteProcedure(string spName, params IAdoParameter[] parameters) {
            return InnerRepository.ExecuteProcedure(spName, parameters);
        }

        #endregion

        #region << Execute Entity >>

        /// <summary>
        /// 지정된 Entity의 속성 값을 이용하여 Command의 Parameter 값을 설정하고, 실행시킨다.
        /// 일반적으로 Save / Update시에 활용하면 좋다.
        /// </summary>
        /// <typeparam name="T">Persistent object 수형</typeparam>
        /// <param name="cmd">수행할 Command 객체</param>
        /// <param name="entity">처리할 Persistent object</param>
        /// <param name="nameMaps">ParameterName of Procedure = Property Name of Persistent object 매핑 정보</param>
        /// <returns>Command 의 RETURN VALUE</returns>
        public static object ExecuteEntity<T>(DbCommand cmd, T entity, INameMap nameMaps) {
            return InnerRepository.ExecuteEntity(cmd, entity, nameMaps);
        }

        /// <summary>
        /// 지정된 Entity의 속성 값을 이용하여 Procedure의 Parameter 값을 설정하고, 실행시킨다.
        /// 일반적으로 Save / Update시에 활용하면 좋다.
        /// </summary>
        /// <typeparam name="T">Persistent object 수형</typeparam>
        /// <param name="spName">수행할 Procedure 명</param>
        /// <param name="entity">Persistent object</param>
        /// <param name="nameMaps">ParameterName of Procedure = Property Name of Persistent object</param>
        /// <returns>Procedure의 RETURN VALUE</returns>
        public static object ExecuteEntity<T>(string spName, T entity, INameMap nameMaps) {
            return InnerRepository.ExecuteEntity(spName, entity, nameMaps);
        }

        /// <summary>
        /// 지정된 Procedure를 수행한다. 인자로 entity의 속성값을 이용한다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmd">실행할 DbCommand</param>
        /// <param name="entity">실행할 Entity</param>
        /// <param name="nameMapper">Name Mapping Class</param>
        /// <returns>Command 인자 중에 ParameterDirection이 ReturnValue인 인자의 값</returns>
        public static object ExecuteEntity<T>(DbCommand cmd, T entity, INameMapper nameMapper) {
            return InnerRepository.ExecuteEntity(cmd, entity, nameMapper);
        }

        /// <summary>
        /// 지정된 Procedure를 수행한다. 인자로 entity의 속성값을 이용한다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="spName">실행할 Procedure Name</param>
        /// <param name="entity">실행할 Entity</param>
        /// <param name="nameMapper">Name Mapping Class</param>
        /// <returns>Command 인자 중에 ParameterDirection이 ReturnValue인 인자의 값</returns>
        public static object ExecuteEntity<T>(string spName, T entity, INameMapper nameMapper) {
            return InnerRepository.ExecuteEntity(spName, entity, nameMapper);
        }

        #endregion
    }
}