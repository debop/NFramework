using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace NSoft.NFramework.Data {
    /// <summary>
    /// ADO.NET 을 이용하는 Repository의 기본 Interface
    /// </summary>
    public interface IAdoRepository : IRepository {
        /// <summary>
        /// Microsoft Data Acess Application Block의 Database
        /// </summary>
        Database Db { get; }

        /// <summary>
        /// Database ConnectionString 이름
        /// </summary>
        string DbName { get; set; }

        /// <summary>
        /// Query String Provider
        /// </summary>
        IIniQueryProvider QueryProvider { get; set; }

        //! -------------------------------------------------

        /// <summary>
        /// Current Thread Context에서 활성화된 Transaction. (없다면 null을 반환한다.)
        /// </summary>
        /// <seealso cref="IsActiveTransaction"/>
        DbTransaction ActiveTransaction { get; set; }

        /// <summary>
        /// Is exist active transaction in Current Thread Context.
        /// </summary>
        bool IsActiveTransaction { get; }

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
        DbTransaction BeginTransaction(System.Data.IsolationLevel isolationLevel);

        /// <summary>
        /// ADO.NET의 기본 Transaction을 시작한다. 격리수준은 <see cref="IsolationLevel.ReadCommitted"/>이다. 
        /// (TransactionScope와는 달리 DTC를 이용하지 않는다.)
        /// </summary>
        /// <returns>Return a new instance of DbTransaction</returns>
        DbTransaction BeginTransaction();

        /// <summary>
        /// 현재 활성화된 Transaction이 있다면, Commit을 수행한다.
        /// </summary>
        /// <exception cref="InvalidOperationException">Current Thread Context에 활성화된 Transaction이 없을 때</exception>
        void Commit();

        /// <summary>
        /// 현재 활성화된 Transaction이 있다면, Rollback을 수행한다.
        /// </summary>
        /// <exception cref="InvalidOperationException">Current Thread Context에 활성화된 Transaction이 없을 때</exception>
        void Rollback();

        //! -------------------------------------------------

        /// <summary>
        /// 지정된 DataAdapter를 통해 얻은 정보를 DataSet으로 빌드한다.
        /// </summary>
        /// <param name="adapter">DataAdapter</param>
        /// <param name="tableName">Table name</param>
        /// <param name="targetDataSet">저장할 Dataset</param>
        /// <param name="firstResult">첫번째 레코드의 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">최대 레코드 수 (0 이면 무시하고, 마지막 레코드까지 가져온다</param>
        void LoadDataSet(DbDataAdapter adapter, string tableName, DataSet targetDataSet, int firstResult = 0, int maxResults = 0);

        /// <summary>
        /// <paramref name="cmd"/>를 실행하여 DataSet을 빌드한다.
        /// </summary>
        /// <param name="cmd">실행할 <see cref="DbCommand"/> instance.</param>
        /// <param name="firstResult">첫번째 레코드의 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">최대 레코드 수 (0 이면 무시하고, 마지막 레코드까지 가져온다)</param>
        /// <param name="parameters">collection of parameters of Command</param>
        /// <returns>결과 셋이 담긴 DataSet</returns>
        DataSet ExecuteDataSet(DbCommand cmd, int firstResult, int maxResults, params IAdoParameter[] parameters);

        /// <summary>
        /// <paramref name="cmd"/>를 실행하여 DataSet을 로드합니다.
        /// </summary>
        /// <param name="cmd">실행할 <see cref="DbCommand"/> instance.</param>
        /// <param name="parameters">collection of parameters of Command</param>
        /// <returns>결과 셋이 담긴 DataSet</returns>
        DataSet ExecuteDataSet(DbCommand cmd, params IAdoParameter[] parameters);

        /// <summary>
        /// <paramref name="query"/> 문을 실행하여, DataSet을 빌드합니다.
        /// </summary>
        /// <param name="query">실행할 쿼림 문 또는 Procedure 명</param>
        /// <param name="firstResult">첫번째 레코드의 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">최대 레코드 수 (0 이면 무시하고, 마지막 레코드까지 가져온다)</param>		
        /// <param name="parameters">parameter collection</param>
        /// <returns>결과 셋</returns>
        DataSet ExecuteDataSet(string query, int firstResult, int maxResults, params IAdoParameter[] parameters);

        /// <summary>
        /// <paramref name="query"/> 문을 실행하여, DataSet을 빌드합니다.
        /// </summary>
        /// <param name="query">실행할 쿼림 문 또는 Procedure 명</param>
        /// <param name="parameters">parameter collection</param>
        /// <returns>결과 셋</returns>
        DataSet ExecuteDataSet(string query, params IAdoParameter[] parameters);

        /// <summary>
        /// 지정된 DataAdpater를 실행하여 DataSet을 빌드한다.
        /// </summary>
        /// <param name="sqlString">simple query string to execute</param>
        /// <param name="firstResult">첫번째 레코드의 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">최대 레코드 수 (0 이면 무시하고, 마지막 레코드까지 가져온다</param>
        /// <param name="parameters">collectio of parameters of Command</param>
        /// <returns>결과 셋이 담긴 DataSet</returns>
        DataSet ExecuteDataSetBySqlString(string sqlString, int firstResult, int maxResults, params IAdoParameter[] parameters);

        /// <summary>
        /// 지정된 DataAdpater를 실행하여 DataSet을 빌드한다.
        /// </summary>
        /// <param name="sqlString">simple query string to execute</param>
        /// <param name="parameters">collectio of parameters of Command</param>
        /// <returns>결과 셋이 담긴 DataSet</returns>
        DataSet ExecuteDataSetBySqlString(string sqlString, params IAdoParameter[] parameters);

        /// <summary>
        /// 지정된 DataAdpater를 실행하여 DataSet을 빌드한다.
        /// </summary>
        /// <param name="spName">Procedure name to execute</param>
        /// <param name="firstResult">첫번째 레코드의 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">최대 레코드 수 (0 이면 무시하고, 마지막 레코드까지 가져온다</param>
        /// <param name="parameters">collectio of parameters of Command</param>
        /// <returns>결과 셋이 담긴 DataSet</returns>
        DataSet ExecuteDataSetByProcedure(string spName, int firstResult, int maxResults, params IAdoParameter[] parameters);

        /// <summary>
        /// 지정된 DataAdpater를 실행하여 DataSet을 빌드한다.
        /// </summary>
        /// <param name="spName">Procedure name to execute</param>
        /// <param name="parameters">collectio of parameters of Command</param>
        /// <returns>결과 셋이 담긴 DataSet</returns>
        DataSet ExecuteDataSetByProcedure(string spName, params IAdoParameter[] parameters);

        //! -------------------------------------------------

        /// <summary>
        /// 지정된 Command를 실행하여, DataTable을 빌드합니다.
        /// </summary>
        /// <param name="cmd">실행할 Select용 Command</param>
        /// <param name="parameters">DbCommand의 Parameter 정보</param>
        /// <returns>instance of <see cref="DataTable"/></returns>
        DataTable ExecuteDataTable(DbCommand cmd, params IAdoParameter[] parameters);

        /// <summary>
        /// 지정된 Command를 실행하여, DataTable을 빌드합니다.
        /// </summary>
        /// <param name="cmd">실행할 Select용 Command</param>
        /// <param name="firstResult">첫번째 레코드의 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">최대 레코드 수 (0 이면 무시하고, 마지막 레코드까지 가져온다</param>
        /// <param name="parameters">DbCommand의 Parameter 정보</param>
        /// <returns>instance of <see cref="DataTable"/></returns>
        DataTable ExecuteDataTable(DbCommand cmd, int firstResult, int maxResults, params IAdoParameter[] parameters);

        /// <summary>
        /// <paramref name="query"/> 문을 실행하여, DataTable을 빌드합니다.
        /// </summary>
        /// <param name="query">실행할 쿼림 문 또는 Procedure 명</param>
        /// <param name="parameters">parameter collection</param>
        /// <returns>결과 셋</returns>
        DataTable ExecuteDataTable(string query, params IAdoParameter[] parameters);

        /// <summary>
        /// <paramref name="query"/> 문을 실행하여, DataTable을 빌드합니다.
        /// </summary>
        /// <param name="query">실행할 쿼림 문 또는 Procedure 명</param>
        /// <param name="firstResult">첫번째 레코드의 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">최대 레코드 수 (0 이면 무시하고, 마지막 레코드까지 가져온다)</param>		
        /// <param name="parameters">parameter collection</param>
        /// <returns>결과 셋</returns>
        DataTable ExecuteDataTable(string query, int firstResult, int maxResults, params IAdoParameter[] parameters);

        /// <summary>
        /// 지정된 sql string 문을 실행하여, DataTable을 빌드합니다.
        /// </summary>
        /// <param name="sqlString">실행할 Query 문</param>
        /// <param name="parameters">DbCommand의 Parameter 정보</param>
        /// <returns>instance of <see cref="DataTable"/></returns>
        DataTable ExecuteDataTableBySqlString(string sqlString, params IAdoParameter[] parameters);

        /// <summary>
        /// 지정된 sql string 문을 실행하여, DataTable을 빌드합니다.
        /// </summary>
        /// <param name="sqlString">실행할 Query 문</param>
        /// <param name="firstResult">첫번째 레코드의 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">최대 레코드 수 (0 이면 무시하고, 마지막 레코드까지 가져온다</param>
        /// <param name="parameters">DbCommand의 Parameter 정보</param>
        /// <returns>instance of <see cref="DataTable"/></returns>
        DataTable ExecuteDataTableBySqlString(string sqlString, int firstResult, int maxResults, params IAdoParameter[] parameters);

        /// <summary>
        /// 지정된 Procedure를 실행하여, DataTable을 빌드합니다.
        /// </summary>
        /// <param name="spName">실행할 Select Command</param>
        /// <param name="parameters">DbCommand의 Parameter 정보</param>
        /// <returns>instance of <see cref="DataTable"/></returns>
        DataTable ExecuteDataTableByProcedure(string spName, params IAdoParameter[] parameters);

        /// <summary>
        /// 지정된 Procedure를 실행하여, DataTable을 빌드합니다.
        /// </summary>
        /// <param name="spName">실행할 Procedure 명</param>
        /// <param name="firstResult">첫번째 레코드의 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">최대 레코드 수 (0 이면 무시하고, 마지막 레코드까지 가져온다</param>
        /// <param name="parameters">DbCommand의 Parameter 정보</param>
        /// <returns>instance of <see cref="DataTable"/></returns>
        DataTable ExecuteDataTableByProcedure(string spName, int firstResult, int maxResults, params IAdoParameter[] parameters);

        /// <summary>
        /// 여러 ResultSet을 반환할 수 있으므로, DataTable의 컬렉션으로 반환합니다.
        /// </summary>
        IList<DataTable> ExecuteDataTableAsList(DbCommand cmd, params IAdoParameter[] parameters);

        /// <summary>
        /// 여러 ResultSet을 반환할 수 있으므로, DataTable의 컬렉션으로 반환합니다.
        /// </summary>
        IList<DataTable> ExecuteDataTableAsList(DbCommand cmd, int firstResult, int maxResults, params IAdoParameter[] parameters);

        //! -------------------------------------------------

        /// <summary>
        /// 지정된 조회용 Command 를 Paging 정보에 근거해서 수행하고, 결과를 DataTable로 반환한다.
        /// </summary>
        /// <param name="cmd">조회용 Command 인스턴스</param>
        /// <param name="pageIndex">Page index (0부터 시작).  null이면 0으로 간주</param>
        /// <param name="pageSize">Page Size. 한 페이지에 표현할 요소 수 (보통 10개). null이면 10으로 간주</param>
        /// <param name="parameters">조회용 쿼리의 Parameter 정보</param>
        /// <returns>Paging된 DataTable</returns>
        PagingDataTable ExecutePagingDataTable(DbCommand cmd, int? pageIndex, int? pageSize, params IAdoParameter[] parameters);

        /// <summary>
        /// <paramref name="query"/>를 Paging 정보에 근거해서 실행하고, 결과를 DataTable로 반환한다.
        /// NOTE: DISTINCT, TOP N 조회는 Paging에서 지원하지 않습니다.
        /// </summary>
        /// <param name="query">조회용 쿼리 문</param>
        /// <param name="pageIndex">Page Index (0부터 시작).  null이면 0으로 간주</param>
        /// <param name="pageSize">Page Size. 한 페이지에 표현할 요소 수 (보통 10개). null이면 10으로 간주</param>
        /// <param name="parameters">조회용 쿼리의 Parameter 정보</param>
        /// <returns>Paging된 DataTable</returns>
        PagingDataTable ExecutePagingDataTable(string query, int? pageIndex, int? pageSize, params IAdoParameter[] parameters);

        /// <summary>
        /// 지정된 조회용 쿼리문을 Paging 정보에 근거해서 수행하고, 결과를 DataTable로 반환한다. 
        /// NOTE: 복잡한 쿼리 문장은 안될 가능성이 있습니다.
        /// NOTE: DISTINCT, TOP N 조회는 Paging에서 지원하지 않습니다.
        /// </summary>
        /// <param name="selectSql">조회용 쿼리 문장</param>
        /// <param name="pageIndex">Page Index (0부터 시작).  null이면 0으로 간주</param>
        /// <param name="pageSize">Page Size. 한 페이지에 표현할 요소 수 (보통 10개). null이면 10으로 간주</param>
        /// <param name="parameters">조회용 쿼리의 Parameter 정보</param>
        /// <returns>Paging된 DataTable</returns>
        /// <seealso cref="AdoTool.GetCountingSqlString"/>
        PagingDataTable ExecutePagingDataTableBySqlString(string selectSql, int? pageIndex, int? pageSize,
                                                          params IAdoParameter[] parameters);

        /// <summary>
        /// <paramref name="spName"/> Procedure를 실행하여, Paging 정보에 근거해서 Data를 추출하고, 결과를 DataTable로 반환한다. 
        /// </summary>
        /// <param name="spName">조회용 Procedure Name</param>
        /// <param name="pageIndex">Page Index (0부터 시작).  null이면 0으로 간주</param>
        /// <param name="pageSize">Page Size. 한 페이지에 표현할 요소 수 (보통 10개). null이면 10으로 간주</param>
        /// <param name="parameters">Procedure Parameter 정보</param>
        /// <returns>Paging된 DataTable</returns>
        /// <seealso cref="AdoTool.GetCountingSqlString"/>
        PagingDataTable ExecutePagingDataTableByProcedure(string spName, int? pageIndex, int? pageSize,
                                                          params IAdoParameter[] parameters);

        //! -------------------------------------------------

        /// <summary>
        /// Execute DbCommand, and return single scalar value
        /// </summary>
        /// <param name="cmd">Instance of DbCommand to execute</param>
        /// <param name="parameters">Parameters for DbCommand to execute</param>
        /// <returns>Scalar value</returns>
        object ExecuteScalar(DbCommand cmd, params IAdoParameter[] parameters);

        /// <summary>
        /// Execute query, and return single scalar value
        /// </summary>
        /// <param name="query">query string ( simple sql string or Procedure name )</param>
        /// <param name="parameters">Parameters for DbCommand</param>
        /// <returns>Scalar value</returns>
        object ExecuteScalar(string query, params IAdoParameter[] parameters);

        /// <summary>
        /// Execute sql string, and return single scalar value
        /// </summary>
        /// <param name="sqlString">simple sql string</param>
        /// <param name="parameters">Parameters for DbCommand</param>
        /// <returns>Scalar value</returns>
        object ExecuteScalarBySqlString(string sqlString, params IAdoParameter[] parameters);

        /// <summary>
        /// Execute stored procedure, and return single scalar value
        /// </summary>
        /// <param name="spName">Procedure name</param>
        /// <param name="parameters">Parameters for DbCommand</param>
        /// <returns>Scalar value</returns>
        object ExecuteScalarByProcedure(string spName, params IAdoParameter[] parameters);

        //! -------------------------------------------------

        /// <summary>
        /// Execute specified DbCommand
        /// </summary>
        /// <param name="cmd">Instance of DbCommand to execute</param>
        /// <param name="parameters">Parameters for DbCommand to execute</param>
        /// <returns>affected row count</returns>
        int ExecuteNonQuery(DbCommand cmd, params IAdoParameter[] parameters);

        /// <summary>
        /// Execute specified query or procedure
        /// </summary>
        /// <param name="query">query string ( simple sql string or Procedure name )</param>
        /// <param name="parameters">Parameters for DbCommand</param>
        /// <returns>affected row count</returns>
        int ExecuteNonQuery(string query, params IAdoParameter[] parameters);

        /// <summary>
        /// Execute specified sql string
        /// </summary>
        /// <param name="sqlString">simple sql string</param>
        /// <param name="parameters">Parameters for DbCommand</param>
        /// <returns>affected row count</returns>
        int ExecuteNonQueryBySqlString(string sqlString, params IAdoParameter[] parameters);

        /// <summary>
        /// Execute specified stored procedure
        /// </summary>
        /// <param name="spName">Procedure name</param>
        /// <param name="parameters">Parameters for DbCommand</param>
        /// <returns>affected row count</returns>
        int ExecuteNonQueryByProcedure(string spName, params IAdoParameter[] parameters);

        //! -------------------------------------------------

        /// <summary>
        /// Execute specified DbCommand, and return  <see cref="IDataReader"/>
        /// </summary>
        /// <param name="cmd">DbCommand to execute</param>
        /// <param name="parameters">Command parameters</param>
        /// <returns>instance of <see cref="IDataReader"/></returns>
        IDataReader ExecuteReader(DbCommand cmd, params IAdoParameter[] parameters);

        /// <summary>
        /// Execute specified query, and return  <see cref="IDataReader"/>
        /// </summary>
        /// <param name="query">query string ( sql string or procedure name )</param>
        /// <param name="parameters">Command parameters</param>
        /// <returns>instance of <see cref="IDataReader"/></returns>
        IDataReader ExecuteReader(string query, params IAdoParameter[] parameters);

        /// <summary>
        /// Execute specified sql string, and return  <see cref="IDataReader"/>
        /// </summary>
        /// <param name="sqlString">simple query string</param>
        /// <param name="parameters">Command parameters</param>
        /// <returns>instance of <see cref="IDataReader"/></returns>
        IDataReader ExecuteReaderBySqlString(string sqlString, params IAdoParameter[] parameters);

        /// <summary>
        /// Execute specified stored procedure, and return  <see cref="IDataReader"/>
        /// </summary>
        /// <param name="spName">procedure name</param>
        /// <param name="parameters">Command parameters</param>
        /// <returns>instance of <see cref="IDataReader"/></returns>
        IDataReader ExecuteReaderByProcedure(string spName, params IAdoParameter[] parameters);
    }
}