using System.Data;
using System.Threading.Tasks;
using NSoft.NFramework.Data.Mappers;
using NSoft.NFramework.Reflections;

namespace NSoft.NFramework.Data {
    //! HINT: ExecuteReader는 Future 방식으로 수행하면 안됩니다. APM 방식 또는 EAP 방식으로 수행해야 한다.

    /// <summary>
    /// AdoRepository에 대한 비동기 작업 (Future) (<see cref="Task"/>) 에 대한 확장 메소드입니다.
    /// </summary>
    public static partial class AdoRepositoryEx {
        /// <summary>
        /// 비동기 방식으로 쿼리문을 실행해서, DataSet을 반환하는 Task를 빌드합니다.
        /// </summary>
        /// <param name="repository"><see cref="IAdoRepository"/></param>
        /// <param name="query">실행할 쿼리문 또는 Procedure Name</param>
        /// <param name="parameters">파라미터 컬렉션</param>
        /// <returns>DataSet을 반환하는 Task</returns>
        public static Task<DataSet> ExecuteDataSetAsync(this IAdoRepository repository,
                                                        string query,
                                                        params IAdoParameter[] parameters) {
            return ExecuteDataSetAsync(repository, query, null, null, parameters);
        }

        /// <summary>
        /// 비동기 방식으로 쿼리문을 실행해서, DataSet을 반환하는 Task를 빌드합니다.
        /// </summary>
        /// <param name="repository"><see cref="IAdoRepository"/></param>
        /// <param name="query">실행할 쿼리문 또는 Procedure Name</param>
        /// <param name="firstResult">첫번째 레코드 인덱스</param>
        /// <param name="maxResults">최대 레코드 갯수</param>
        /// <param name="parameters">파라미터 컬렉션</param>
        /// <returns>DataSet을 반환하는 Task</returns>
        public static Task<DataSet> ExecuteDataSetAsync(this IAdoRepository repository,
                                                        string query,
                                                        int? firstResult,
                                                        int? maxResults,
                                                        params IAdoParameter[] parameters) {
            query.ShouldNotBeWhiteSpace("query");

            if(IsDebugEnabled)
                log.Debug("비동기 방식으로 쿼리를 실행하여, DataSet을 빌드합니다... query=[{0}], firstResult=[{1}], maxResults=[{2}]",
                          query, firstResult, maxResults);

            return Task.Factory.StartNew(() => repository.ExecuteDataSet(query,
                                                                         firstResult ?? 0,
                                                                         maxResults ?? 0,
                                                                         parameters));
        }

        /// <summary>
        /// 비동기 방식으로 쿼리문을 실행해서, DataSet을 반환하는 Task를 빌드합니다.
        /// </summary>
        /// <param name="repository"><see cref="IAdoRepository"/></param>
        /// <param name="sqlString">실행할 쿼리문</param>
        /// <param name="parameters">파라미터 컬렉션</param>
        /// <returns>DataSet을 반환하는 Task</returns>
        public static Task<DataSet> ExecuteDataSetByQueryStringAsync(this IAdoRepository repository,
                                                                     string sqlString,
                                                                     params IAdoParameter[] parameters) {
            return ExecuteDataSetByQueryStringAsync(repository, sqlString, null, null, parameters);
        }

        /// <summary>
        /// 비동기 방식으로 쿼리문을 실행해서, DataSet을 반환하는 Task를 빌드합니다.
        /// </summary>
        /// <param name="repository"><see cref="IAdoRepository"/></param>
        /// <param name="sqlString">실행할 쿼리문</param>
        /// <param name="firstResult">첫번째 레코드 인덱스</param>
        /// <param name="maxResults">최대 레코드 갯수</param>
        /// <param name="parameters">파라미터 컬렉션</param>
        /// <returns>DataSet을 반환하는 Task</returns>
        public static Task<DataSet> ExecuteDataSetByQueryStringAsync(this IAdoRepository repository,
                                                                     string sqlString,
                                                                     int? firstResult,
                                                                     int? maxResults,
                                                                     params IAdoParameter[] parameters) {
            sqlString.ShouldNotBeWhiteSpace("sqlString");

            if(IsDebugEnabled)
                log.Debug("비동기 방식으로 쿼리문을 수행하여 DataSet을 빌드합니다... sqlString=[{0}], firstResult=[{1}], maxResults=[{2}]",
                          sqlString, firstResult, maxResults);

            return Task.Factory.StartNew(() => repository.ExecuteDataSetBySqlString(sqlString,
                                                                                    firstResult ?? 0,
                                                                                    maxResults ?? 0,
                                                                                    parameters));
        }

        /// <summary>
        /// 비동기 방식으로 Procedure를 실행해서, DataSet을 반환하는 Task를 빌드합니다.
        /// </summary>
        /// <param name="repository"><see cref="IAdoRepository"/></param>
        /// <param name="spName">실행할 Procedure Name</param>
        /// <param name="parameters">파라미터 컬렉션</param>
        /// <returns>DataSet을 반환하는 Task</returns>
        public static Task<DataSet> ExecuteDataSetByProcedureAsync(this IAdoRepository repository,
                                                                   string spName,
                                                                   params IAdoParameter[] parameters) {
            return ExecuteDataSetByProcedureAsync(repository, spName, null, null, parameters);
        }

        /// <summary>
        /// 비동기 방식으로 Procedure를 실행해서, DataSet을 반환하는 Task를 빌드합니다.
        /// </summary>
        /// <param name="repository"><see cref="IAdoRepository"/></param>
        /// <param name="spName">실행할 Procedure Name</param>
        /// <param name="firstResult">첫번째 레코드 인덱스</param>
        /// <param name="maxResults">최대 레코드 갯수</param>
        /// <param name="parameters">파라미터 컬렉션</param>
        /// <returns>DataSet을 반환하는 Task</returns>
        public static Task<DataSet> ExecuteDataSetByProcedureAsync(this IAdoRepository repository,
                                                                   string spName,
                                                                   int? firstResult,
                                                                   int? maxResults,
                                                                   params IAdoParameter[] parameters) {
            spName.ShouldNotBeWhiteSpace("spName");

            if(IsDebugEnabled)
                log.Debug("비동기 방식으로 Procedure를 수행하여, DataSet을 빌드합니다... spName=[{0}], firstResult=[{1}], maxResults=[{2}]",
                          spName, firstResult, maxResults);

            return Task.Factory.StartNew(() => repository.ExecuteDataSetByProcedure(spName,
                                                                                    firstResult ?? 0,
                                                                                    maxResults ?? 0,
                                                                                    parameters));
        }

        //! ======================================================================================

        /// <summary>
        /// <paramref name="query"/> 문을 수행하여 DataTable을 반환하는 작업을 빌드합니다.
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static Task<DataTable> ExecuteDataTableAsync(this IAdoRepository repository,
                                                            string query,
                                                            params IAdoParameter[] parameters) {
            return ExecuteDataTableAsync(repository, query, null, null, parameters);
        }

        /// <summary>
        /// 비동기 방식으로 쿼리문을 실행해서, DataTable을 반환하는 Task를 빌드합니다.
        /// </summary>
        /// <param name="repository"><see cref="IAdoRepository"/></param>
        /// <param name="query">실행할 쿼리문 또는 Procedure Name</param>
        /// <param name="firstResult">첫번째 레코드 인덱스</param>
        /// <param name="maxResults">최대 레코드 갯수</param>
        /// <param name="parameters">파라미터 컬렉션</param>
        /// <returns>DataTable을 반환하는 Task</returns>
        public static Task<DataTable> ExecuteDataTableAsync(this IAdoRepository repository,
                                                            string query,
                                                            int? firstResult,
                                                            int? maxResults,
                                                            params IAdoParameter[] parameters) {
            query.ShouldNotBeWhiteSpace("query");

            if(IsDebugEnabled)
                log.Debug("비동기 방식으로 DataTable을 로딩합니다... query=[{0}], firstResult=[{1}], maxResults=[{2}]",
                          query, firstResult, maxResults);

            return Task.Factory.StartNew(() => repository.ExecuteDataTable(query,
                                                                           firstResult ?? 0,
                                                                           maxResults ?? 0,
                                                                           parameters));
        }

        /// <summary>
        /// 비동기 방식으로 쿼리문을 실행해서, DataTable을 반환하는 Task를 빌드합니다.
        /// </summary>
        /// <param name="repository"><see cref="IAdoRepository"/></param>
        /// <param name="sqlString">실행할 쿼리문</param>
        /// <param name="parameters">파라미터 컬렉션</param>
        /// <returns>DataTable을 반환하는 Task</returns>
        public static Task<DataTable> ExecuteDataTableByQueryStringAsync(this IAdoRepository repository,
                                                                         string sqlString,
                                                                         params IAdoParameter[] parameters) {
            return ExecuteDataTableByQueryStringAsync(repository, sqlString, null, null, parameters);
        }

        /// <summary>
        /// 비동기 방식으로 쿼리문을 실행해서, DataTable을 반환하는 Task를 빌드합니다.
        /// </summary>
        /// <param name="repository"><see cref="IAdoRepository"/></param>
        /// <param name="sqlString">실행할 쿼리문</param>
        /// <param name="firstResult">첫번째 레코드 인덱스</param>
        /// <param name="maxResults">최대 레코드 갯수</param>
        /// <param name="parameters">파라미터 컬렉션</param>
        /// <returns>DataTable을 반환하는 Task</returns>
        public static Task<DataTable> ExecuteDataTableByQueryStringAsync(this IAdoRepository repository,
                                                                         string sqlString,
                                                                         int? firstResult,
                                                                         int? maxResults,
                                                                         params IAdoParameter[] parameters) {
            repository.ShouldNotBeNull("repository");
            sqlString.ShouldNotBeWhiteSpace("sqlString");

            if(IsDebugEnabled)
                log.Debug("비동기 방식으로 쿼리를 수행하여 DataTable을 빌드합니다... sqlString=[{0}], firstResult=[{1}], maxResults=[{2}]",
                          sqlString, firstResult, maxResults);

            return Task.Factory.StartNew(() => repository.ExecuteDataTableBySqlString(sqlString,
                                                                                      firstResult ?? 0,
                                                                                      maxResults ?? 0,
                                                                                      parameters));
        }

        /// <summary>
        /// 비동기 방식으로 Procedure를 실행해서, DataTable을 반환하는 Task를 빌드합니다.
        /// </summary>
        /// <param name="repository"><see cref="IAdoRepository"/></param>
        /// <param name="spName">실행할 Procedure Name</param>
        /// <param name="parameters">파라미터 컬렉션</param>
        /// <returns>DataTable을 반환하는 Task</returns>
        public static Task<DataTable> ExecuteDataTableByProcedureAsync(this IAdoRepository repository,
                                                                       string spName,
                                                                       params IAdoParameter[] parameters) {
            return ExecuteDataTableByProcedureAsync(repository, spName, null, null, parameters);
        }

        /// <summary>
        /// 비동기 방식으로 Procedure를 실행해서, DataTable을 반환하는 Task를 빌드합니다.
        /// </summary>
        /// <param name="repository"><see cref="IAdoRepository"/></param>
        /// <param name="spName">실행할 Procedure Name</param>
        /// <param name="firstResult">첫번째 레코드 인덱스</param>
        /// <param name="maxResults">최대 레코드 갯수</param>
        /// <param name="parameters">파라미터 컬렉션</param>
        /// <returns>DataTable을 반환하는 Task</returns>
        public static Task<DataTable> ExecuteDataTableByProcedureAsync(this IAdoRepository repository,
                                                                       string spName,
                                                                       int? firstResult,
                                                                       int? maxResults,
                                                                       params IAdoParameter[] parameters) {
            spName.ShouldNotBeWhiteSpace("spName");

            if(IsDebugEnabled)
                log.Debug("비동기 방식으로 Procedure를 수행하여 DataTable을 빌드합니다... spName=[{0}], firstResult=[{1}], maxResults=[{2}]",
                          spName, firstResult, maxResults);

            return Task.Factory.StartNew(() => repository.ExecuteDataTableByProcedure(spName,
                                                                                      firstResult ?? 0,
                                                                                      maxResults ?? 0,
                                                                                      parameters));
        }

        //! ======================================================================================

        /// <summary>
        /// 비동기 방식으로 쿼리문을 실행해서, PagingDataTable을 반환하는 Task를 빌드합니다.
        /// </summary>
        /// <param name="repository"><see cref="IAdoRepository"/></param>
        /// <param name="query">실행할 쿼리문 또는 Procedure Name</param>
        /// <param name="pageIndex">Page index (0부터 시작합니다.)</param>
        /// <param name="pageSize">Page당 Item 수 (보통 10개)</param>
        /// <param name="parameters">파라미터 컬렉션</param>
        /// <returns>PagingDataTable을 반환하는 Task</returns>
        public static Task<PagingDataTable> ExecutePagingDataTableAsync(this IAdoRepository repository,
                                                                        string query,
                                                                        int? pageIndex,
                                                                        int? pageSize,
                                                                        params IAdoParameter[] parameters) {
            query.ShouldNotBeWhiteSpace("query");

            if(IsDebugEnabled)
                log.Debug("비동기 방식으로 ExecutePagingDataTable()을 수행합니다... query=[{0}], pageIndex=[{1}], pageSize=[{2}]",
                          query, pageIndex, pageSize);

            return Task.Factory.StartNew(() => repository.ExecutePagingDataTable(query, pageIndex, pageSize, parameters));
        }

        /// <summary>
        /// 비동기 방식으로 쿼리문을 실행해서, PagingDataTable을 반환하는 Task를 빌드합니다.
        /// </summary>
        /// <param name="repository"><see cref="IAdoRepository"/></param>
        /// <param name="sqlString">실행할 쿼리문</param>
        /// <param name="pageIndex">Page index (0부터 시작합니다.)</param>
        /// <param name="pageSize">Page당 Item 수 (보통 10개)</param>
        /// <param name="parameters">파라미터 컬렉션</param>
        /// <returns>PagingDataTable을 반환하는 Task</returns>
        public static Task<PagingDataTable> ExecutePagingDataTableByQueryStringAsync(this IAdoRepository repository,
                                                                                     string sqlString,
                                                                                     int? pageIndex,
                                                                                     int? pageSize,
                                                                                     params IAdoParameter[] parameters) {
            repository.ShouldNotBeNull("repository");
            sqlString.ShouldNotBeWhiteSpace("sqlString");

            if(IsDebugEnabled)
                log.Debug("비동기 방식으로 쿼리를 수행하여 PagingDataTable을 빌드합니다... sqlString=[{0}], pageIndex=[{1}], pageSize=[{2}]",
                          sqlString, pageIndex, pageSize);

            return Task.Factory.StartNew(() => repository.ExecutePagingDataTableBySqlString(sqlString, pageIndex, pageSize, parameters));
        }

        /// <summary>
        /// 비동기 방식으로 Procedure를 실행해서, PagingDataTable을 반환하는 Task를 빌드합니다.
        /// </summary>
        /// <param name="repository"><see cref="IAdoRepository"/></param>
        /// <param name="spName">실행할 Procedure Name</param>
        /// <param name="pageIndex">Page index (0부터 시작합니다.)</param>
        /// <param name="pageSize">Page당 Item 수 (보통 10개)</param>
        /// <param name="parameters">파라미터 컬렉션</param>
        /// <returns>DataTable을 반환하는 Task</returns>
        public static Task<PagingDataTable> ExecutePagingDataTableByProcedureAsync(this IAdoRepository repository,
                                                                                   string spName,
                                                                                   int? pageIndex,
                                                                                   int? pageSize,
                                                                                   params IAdoParameter[] parameters) {
            repository.ShouldNotBeNull("repository");
            spName.ShouldNotBeWhiteSpace("spName");

            if(IsDebugEnabled)
                log.Debug("비동기 방식으로 Procedure를 수행하여, PagingDataTable을 얻습니다... spName=[{0}], pageIndex=[{1}], pageSize=[{2}]",
                          spName, pageIndex, pageSize);

            return Task.Factory.StartNew(() => repository.ExecutePagingDataTableByProcedure(spName,
                                                                                            pageIndex,
                                                                                            pageSize,
                                                                                            parameters));
        }

        //! ======================================================================================

        /// <summary>
        /// <see cref="IAdoRepository.ExecuteNonQuery(string, IAdoParameter[])"/>를 비동기 방식으로 수행합니다.
        /// </summary>
        /// <param name="repository">Repository</param>
        /// <param name="query">실행할 쿼리문 또는 Procedure Name</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>영향받는 레코드수를 결과값으로 제공하는 Task</returns>
        public static Task<int> ExecuteNonQueryAsync(this IAdoRepository repository, string query, params IAdoParameter[] parameters) {
            query.ShouldNotBeWhiteSpace("query");

            if(IsDebugEnabled)
                log.Debug("비동기 방식으로 쿼리를 수행합니다... query=[{0}], parameters=[{1}]", query, parameters.CollectionToString());

            return Task.Factory.StartNew(() => repository.ExecuteNonQuery(query, parameters));
        }

        /// <summary>
        /// <see cref="IAdoRepository.ExecuteNonQueryByProcedure"/> 를 비동기 방식으로 수행합니다.
        /// </summary>
        /// <param name="repository"><see cref="IAdoRepository"/> 인스턴스</param>
        /// <param name="spName">실행할 Procedure Name</param>
        /// <param name="parameters">Procedure의 Parameters</param>
        /// <returns>영향받는 레코드수를 결과값으로 제공하는 Task</returns>
        public static Task<int> ExecuteNonQueryByProcedureTask(this IAdoRepository repository, string spName,
                                                               params IAdoParameter[] parameters) {
            spName.ShouldNotBeWhiteSpace("spName");

            if(IsDebugEnabled)
                log.Debug("비동기 방식으로 Procedure를 실행합니다... spName=[{0}], parameters=[{1}]", spName, parameters.CollectionToString());

            return Task.Factory.StartNew(() => repository.ExecuteNonQueryByProcedure(spName, parameters));
        }

        /// <summary>
        /// <see cref="IAdoRepository.ExecuteNonQueryBySqlString"/> 를 비동기 방식으로 수행합니다.
        /// </summary>
        /// <param name="repository"><see cref="IAdoRepository"/> 인스턴스</param>
        /// <param name="sqlString">실행할 SQL 문</param>
        /// <param name="parameters">SQL의 Parameters</param>
        /// <returns>영향받는 레코드수를 결과값으로 제공하는 Task</returns>
        public static Task<int> ExecuteNonQueryBySqlStringAsync(this IAdoRepository repository, string sqlString,
                                                                params IAdoParameter[] parameters) {
            sqlString.ShouldNotBeWhiteSpace("sqlString");

            if(IsDebugEnabled)
                log.Debug("비동기방식으로 쿼리문 을 수행합니다... sqlString=[{0}], parameters=[{1}]",
                          sqlString, parameters.CollectionToString());

            return Task.Factory.StartNew(() => repository.ExecuteNonQueryBySqlString(sqlString, parameters));
        }

        //! ======================================================================================

        /// <summary>
        /// 비동기 방식으로 작업을 <paramref name="query"/>를 수행하여, <see cref="IDataReader"/>를 결과값으로 제공하는 Task를 반환합니다.
        /// </summary>
        /// <param name="repository">Repository</param>
        /// <param name="query">실행할 쿼리문 또는 Procedure Name</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>결과셋을 IDataReader 로 결과를 제공하는 Task</returns>
        public static Task<IDataReader> ExecuteReaderAsync(this IAdoRepository repository, string query,
                                                           params IAdoParameter[] parameters) {
            query.ShouldNotBeWhiteSpace("query");

            if(IsDebugEnabled)
                log.Debug("비동기 방식으로 쿼리를 수행하여 IDataReader를 얻습니다... query=[{0}], parameters=[{1}]",
                          query, parameters.CollectionToString());

            return Task.Factory.StartNew(() => repository.ExecuteReader(query, parameters));
        }

        /// <summary>
        /// 비동기 방식으로 작업을 <paramref name="spName"/>를 수행하여, <see cref="IDataReader"/>를 결과값으로 제공하는 Task를 반환합니다.
        /// </summary>
        /// <param name="repository">Repository</param>
        /// <param name="spName">실행할 Procedure Name</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>결과셋을 IDataReader 로 결과를 제공하는 Task</returns>
        public static Task<IDataReader> ExecuteReaderByProcedureAsync(this IAdoRepository repository, string spName,
                                                                      params IAdoParameter[] parameters) {
            spName.ShouldNotBeWhiteSpace("spName");

            if(IsDebugEnabled)
                log.Debug("비동기 방식으로 Procedure 를 수행하여 IDataReader를 얻습니다... spName=[{0}], parameters=[{1}]",
                          spName, parameters.CollectionToString());

            return Task.Factory.StartNew(() => repository.ExecuteReaderByProcedure(spName, parameters));
        }

        /// <summary>
        /// 비동기 방식으로 작업을 <paramref name="sqlString"/>를 수행하여, <see cref="IDataReader"/>를 결과값으로 제공하는 Task를 반환합니다.
        /// </summary>
        /// <param name="repository">Repository</param>
        /// <param name="sqlString">실행할 쿼리문</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>결과셋을 IDataReader 로 결과를 제공하는 Task</returns>
        public static Task<IDataReader> ExecuteReaderBySqlStringAsync(this IAdoRepository repository, string sqlString,
                                                                      params IAdoParameter[] parameters) {
            sqlString.ShouldNotBeWhiteSpace("sqlString");

            if(IsDebugEnabled)
                log.Debug("비동기 방식으로 쿼리를 수행하여 IDataReader를 얻습니다... sqlString=[{0}], parameters=[{1}]",
                          sqlString, parameters.CollectionToString());

            return Task.Factory.StartNew(() => repository.ExecuteReaderBySqlString(sqlString, parameters));
        }

        //! ======================================================================================

        /// <summary>
        /// <see cref="IAdoRepository.ExecuteScalar(string,IAdoParameter[])"/> 를 
        /// 비동기 방식으로 수행하는 <see cref="Task{TResult}"/>를 빌드합니다.
        /// </summary>
        /// <param name="repository"><see cref="IAdoRepository"/> 인스턴스</param>
        /// <param name="query">실행할 쿼리 문</param>
        /// <param name="parameters">파라미터 정보</param>
        /// <returns>Scalar 값을 결과값으로 가진 Task</returns>
        public static Task<object> ExecuteScalarAsync(this IAdoRepository repository, string query, params IAdoParameter[] parameters) {
            repository.ShouldNotBeNull("repository");
            query.ShouldNotBeWhiteSpace("query");

            if(IsDebugEnabled)
                log.Debug("비동기 방식으로 ExecuteScalar()을 수행합니다... query=[{0}], parameters=[{1}]",
                          query, parameters.CollectionToString());

            return Task.Factory.StartNew(() => repository.ExecuteScalar(query, parameters));
        }

        /// <summary>
        /// <see cref="IAdoRepository.ExecuteScalarByProcedure"/> 를 비동기 방식으로 수행하는 <see cref="Task{TResult}"/>를 빌드합니다.
        /// </summary>
        /// <param name="repository"><see cref="IAdoRepository"/> 인스턴스</param>
        /// <param name="spName">실행할 Procedure Name</param>
        /// <param name="parameters">파라미터 정보</param>
        /// <returns>Scalar 값을 결과값으로 가진 Task</returns>
        public static Task<object> ExecuteScalarTaskByProcedureAsync(this IAdoRepository repository, string spName,
                                                                     params IAdoParameter[] parameters) {
            repository.ShouldNotBeNull("repository");
            spName.ShouldNotBeWhiteSpace("spName");

            if(IsDebugEnabled)
                log.Debug("비동기 방식으로 ExecuteScalarByProcedure()을 수행합니다... spName=[{0}], parameters=[{1}]",
                          spName, parameters.CollectionToString());

            return Task.Factory.StartNew(() => repository.ExecuteScalarByProcedure(spName, parameters));
        }

        /// <summary>
        /// <see cref="IAdoRepository.ExecuteScalarBySqlString"/> 를 비동기 방식으로 수행하는 <see cref="Task{TResult}"/>를 빌드합니다.
        /// </summary>
        /// <param name="repository"><see cref="IAdoRepository"/> 인스턴스</param>
        /// <param name="sqlString">실행할 쿼리 문</param>
        /// <param name="parameters">파라미터 정보</param>
        /// <returns>Scalar 값을 결과값으로 가진 Task</returns>
        public static Task<object> ExecuteScalarBySqlStringAsync(this IAdoRepository repository, string sqlString,
                                                                 params IAdoParameter[] parameters) {
            repository.ShouldNotBeNull("repository");
            sqlString.ShouldNotBeWhiteSpace("sqlString");

            if(IsDebugEnabled)
                log.Debug("비동기 방식으로 ExecuteScalarBySqlString()을 수행합니다... sqlString=[{0}], parameters=[{1}]",
                          sqlString, parameters.CollectionToString());

            return Task.Factory.StartNew(() => repository.ExecuteScalarBySqlString(sqlString, parameters));
        }

        //! ======================================================================================

        /// <summary>
        /// <see cref="Count(NSoft.NFramework.Data.IAdoRepository,string,NSoft.NFramework.Data.IAdoParameter[])"/>를 비동기 방식으로 수행합니다.
        /// </summary>
        /// <param name="repository"><see cref="IAdoRepository"/>의 인스턴스</param>
        /// <param name="query">실행할 쿼리문 또는 Procedure Name</param>
        /// <param name="parameters">패러미터</param>
        /// <returns>Count 수를 결과로 가지는 Task</returns>
        public static Task<int> CountAsync(this IAdoRepository repository, string query, params IAdoParameter[] parameters) {
            query.ShouldNotBeWhiteSpace("query");

            if(IsDebugEnabled)
                log.Debug("비동기 방식으로 IAdoRepository.Count()을 수행합니다... query=[{0}], parameters=[{1}]", query,
                          parameters.CollectionToString());

            return Task.Factory.StartNew(() => repository.Count(query, parameters));
        }

        /// <summary>
        /// <see cref="CountByProcedure"/>를 비동기 방식으로 수행합니다.
        /// </summary>
        /// <param name="repository"><see cref="IAdoRepository"/>의 인스턴스</param>
        /// <param name="spName">실행할 Procedure Name</param>
        /// <param name="parameters">패러미터</param>
        /// <returns>Count 수를 결과로 가지는 Task</returns>
        public static Task<int> CountByProcedureAsync(this IAdoRepository repository, string spName, params IAdoParameter[] parameters) {
            spName.ShouldNotBeWhiteSpace("spName");

            if(IsDebugEnabled)
                log.Debug("비동기 방식으로 IAdoRepository.CountByProcedure()을 수행합니다... spName=[{0}], parameters=[{1}]", spName,
                          parameters.CollectionToString());

            return Task.Factory.StartNew(() => repository.CountByProcedure(spName, parameters));
        }

        /// <summary>
        /// <see cref="CountBySqlString"/>를 비동기 방식으로 수행합니다.
        /// </summary>
        /// <param name="repository"><see cref="IAdoRepository"/>의 인스턴스</param>
        /// <param name="sqlString">실행할 쿼리문</param>
        /// <param name="parameters">패러미터</param>
        /// <returns>Count 수를 결과로 가지는 Task</returns>
        public static Task<int> CountBySqlStringAsync(this IAdoRepository repository, string sqlString,
                                                      params IAdoParameter[] parameters) {
            sqlString.ShouldNotBeWhiteSpace("sqlString");

            if(IsDebugEnabled)
                log.Debug("비동기 방식으로 IAdoRepository.CountBySqlString()을 수행합니다... sqlString=[{0}], parameters=[{1}]", sqlString,
                          parameters.CollectionToString());

            return Task.Factory.StartNew(() => repository.CountBySqlString(sqlString, parameters));
        }

        //! ======================================================================================

        /// <summary>
        /// <see cref="Exists(IAdoRepository,string,IAdoParameter[])"/>를 비동기 방식으로 수행합니다.
        /// </summary>
        /// <param name="repository"><see cref="IAdoRepository"/>의 인스턴스</param>
        /// <param name="query">실행할 쿼리문 또는 Procedure Name</param>
        /// <param name="parameters">패러미터</param>
        /// <returns>Exists 결과를 가지는 Task</returns>
        public static Task<bool> ExistsAsync(this IAdoRepository repository, string query, params IAdoParameter[] parameters) {
            query.ShouldNotBeWhiteSpace("query");

            if(IsDebugEnabled)
                log.Debug("비동기 방식으로 IAdoRepository.Exists()을 수행합니다... query=[{0}], parameters=[{1}]",
                          query, parameters.CollectionToString());

            return Task.Factory.StartNew(() => repository.Exists(query, parameters));
        }

        /// <summary>
        /// <see cref="ExistsByProcedure"/>를 비동기 방식으로 수행합니다.
        /// </summary>
        /// <param name="repository"><see cref="IAdoRepository"/>의 인스턴스</param>
        /// <param name="spName">실행할 Procedure Name</param>
        /// <param name="parameters">패러미터</param>
        /// <returns>Exists 결과를 가지는 Task</returns>
        public static Task<bool> ExistsByProdedureAsync(this IAdoRepository repository, string spName, params IAdoParameter[] parameters) {
            spName.ShouldNotBeWhiteSpace("spName");

            if(IsDebugEnabled)
                log.Debug("비동기 방식으로 IAdoRepository.ExistsByProcedure()을 수행합니다... spName=[{0}], parameters=[{1}]",
                          spName, parameters.CollectionToString());

            return Task.Factory.StartNew(() => repository.ExistsByProcedure(spName, parameters));
        }

        /// <summary>
        /// <see cref="ExistsBySqlString"/>를 비동기 방식으로 수행합니다.
        /// </summary>
        /// <param name="repository"><see cref="IAdoRepository"/>의 인스턴스</param>
        /// <param name="sqlString">실행할 쿼리문</param>
        /// <param name="parameters">패러미터</param>
        /// <returns>Exists 결과를 가지는 Task</returns>
        public static Task<bool> ExistsBySqlStringAsync(this IAdoRepository repository, string sqlString,
                                                        params IAdoParameter[] parameters) {
            sqlString.ShouldNotBeWhiteSpace("sqlString");

            if(IsDebugEnabled)
                log.Debug("비동기 방식으로 IAdoRepository.ExistsBySqlString()을 수행합니다... sqlString=[{0}], parameters=[{1}]",
                          sqlString, parameters.CollectionToString());

            return Task.Factory.StartNew(() => repository.ExistsBySqlString(sqlString, parameters));
        }

        //! ======================================================================================

        /// <summary>
        /// <paramref name="spName"/>의 Procedure를 실행하고, 
        /// Procedure Parameter 중에 <see cref="ParameterDirection"/>중에 Input만 제외한 나머지 Parameter들을 반환합니다.
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="spName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static Task<IAdoParameter[]> ExecuteProcedureAsync(this IAdoRepository repository, string spName,
                                                                  params IAdoParameter[] parameters) {
            spName.ShouldNotBeWhiteSpace("spName");

            if(IsDebugEnabled)
                log.Debug("비동기 방식으로 Procedure를 수행하고, Output Parameter와 Return Parameter의 컬렉션을 반환합니다. spName=[{0}], parameters=[{1}]",
                          spName, parameters.CollectionToString());

            return Task.Factory.StartNew(() => repository.ExecuteProcedure(spName, parameters));
        }

        //! ======================================================================================

        /// <summary>
        /// 지정된 Entity의 속성 값을 이용하여 Procedure의 Parameter 값을 설정하고, 실행시키는 작업을 비동기적으로 수행합니다.
        /// 일반적으로 Save / Update시에 활용하면 좋다.
        /// 단, 비동기적인 작업이므로, Transaction 에 문제가 발생할 소지가 있습니다.
        /// </summary>
        /// <typeparam name="T">Persistent object 수형</typeparam>
        /// <param name="repository">IAdoRepository 인스턴스</param>
        /// <param name="spName">수행할 Procedure 명</param>
        /// <param name="entity">Persistent object</param>
        /// <param name="nameMap">ParameterName of Procedure = Property Name of Persistent object</param>
        /// <returns>Command 인자 중에 ParameterDirection이 ReturnValue인 인자의 값</returns>
        public static Task<object> ExecuteEntityTask<T>(this IAdoRepository repository, string spName, T entity, INameMap nameMap) {
            spName.ShouldNotBeWhiteSpace("spName");
            entity.ShouldNotBeNull("entity");
            nameMap.ShouldNotBeNull("nameMap");

            return Task.Factory.StartNew(() => repository.ExecuteEntity<T>(spName, entity, nameMap));
        }

        /// <summary>
        /// 지정된 Entity의 속성 값을 이용하여 Procedure의 Parameter 값을 설정하고, 실행시키는 작업을 비동기적으로 수행합니다.
        /// 일반적으로 Save / Update시에 활용하면 좋다.
        /// 단, 비동기적인 작업이므로, Transaction 에 문제가 발생할 소지가 있습니다.
        /// </summary>
        /// <typeparam name="T">Persistent object 수형</typeparam>
        /// <param name="repository">IAdoRepository 인스턴스</param>
        /// <param name="spName">실행할 Procedure Name</param>
        /// <param name="entity">실행할 Entity</param>
        /// <param name="nameMapper">Name Mapping Class</param>
        /// <returns>Command 인자 중에 ParameterDirection이 ReturnValue인 인자의 값</returns>
        public static Task<object> ExecuteEntityTask<T>(this IAdoRepository repository, string spName, T entity, INameMapper nameMapper) {
            spName.ShouldNotBeWhiteSpace("spName");
            entity.ShouldNotBeNull("entity");
            nameMapper.ShouldNotBeNull("nameMapper");

            return Task.Factory.StartNew(() => repository.ExecuteEntity<T>(spName, entity, nameMapper));
        }

        //! ======================================================================================
    }
}