using System;
using System.Data.Common;
using NSoft.NFramework.Data.Mappers;
using NSoft.NFramework.Data.QueryProviders;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.Reflections;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Data {
    public static partial class AdoRepositoryEx {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        //! ===============================================================================================

        /// <summary>
        /// Create <see cref="DbDataAdapter"/>
        /// </summary>
        /// <returns>Inatance of <see cref="DbDataAdapter"/></returns>
        public static DbDataAdapter GetDataAdapter(this IAdoRepository repository) {
            return repository.Db.GetDataAdapter();
        }

        //! ===============================================================================================

        /// <summary>
        /// 지정된 쿼리문을 CommendText로 가지는 <see cref="DbCommand"/> 인스턴스를 생성합니다.
        /// </summary>
        /// <param name="repository">IAdoRepository 인스턴스</param>
        /// <param name="query">simple query string or procedure name or queryKey of QueryProvider ([Section,] QueryName)</param>
        /// <param name="parameters">파라미터 컬렉션</param>
        /// <returns>instance of <see cref="DbCommand"/></returns>
        public static DbCommand GetCommand(this IAdoRepository repository, string query, params IAdoParameter[] parameters) {
            query.ShouldNotBeWhiteSpace("query");

            var cmd = repository.GetCommand(query, AdoTool.DEFAULT_DISCOVER_PARAMETER);

            if(parameters.IsNotEmptySequence())
                AdoTool.SetParameterValues(repository.Db, cmd, parameters);

            return cmd;
        }

        /// <summary>
        /// 지정된 쿼리문을 CommendText로 가지는 <see cref="DbCommand"/> 인스턴스를 생성합니다.
        /// </summary>
        /// <param name="repository">IAdoRepository 인스턴스</param>
        /// <param name="query">simple query string or procedure name or queryKey of QueryProvider ([Section,] QueryName)</param>
        /// <param name="discoverParams">discover parameters</param>
        /// <returns>instance of <see cref="DbCommand"/></returns>
        public static DbCommand GetCommand(this IAdoRepository repository, string query, bool discoverParams) {
            query.ShouldNotBeWhiteSpace("query");

            if(repository.QueryProvider != null &&
               query.Contains(InIQueryProviderBase.SECTION_DELIMITER) &&
               AdoTool.IsSqlString(query) == false) {
                string namedQuery;
                if(repository.QueryProvider.TryGetQuery(query, out namedQuery))
                    query = namedQuery;
            }

            return (AdoTool.IsSqlString(query))
                       ? repository.GetSqlStringCommand(query)
                       : repository.GetProcedureCommand(query, discoverParams);
        }

        /// <summary>
        /// Get <see cref="DbCommand"/> which CommandText is sqlString, build up parameters by parameters
        /// </summary>
        /// <param name="repository">IAdoRepository 인스턴스</param>
        /// <param name="sqlString">simple query string</param>
        /// <param name="parameters">collection of parameter</param>
        /// <returns>instance of <see cref="DbCommand"/></returns>
        public static DbCommand GetSqlStringCommand(this IAdoRepository repository, string sqlString, params IAdoParameter[] parameters) {
            sqlString.ShouldNotBeWhiteSpace("sqlString");

            if(IsDebugEnabled)
                log.Debug("다음 SQL 문을 수행할 DbCommand를 생성합니다... sqlString=[{0}]", sqlString);

            var cmd = repository.Db.GetSqlStringCommand(sqlString);

            if(parameters.IsNotEmptySequence())
                AdoTool.SetParameterValues(repository.Db, cmd, parameters);

            return cmd;
        }

        /// <summary>
        /// 지정된 Stored Procedure를 실행할 <see cref="DbCommand"/> 인스턴스를 생성합니다.
        /// </summary>
        /// <param name="repository">IAdoRepository 인스턴스</param>
        /// <param name="spName">procedure name</param>
        /// <param name="parameters">collection of parameter</param>
        /// <returns>instance of <see cref="DbCommand"/></returns>
        public static DbCommand GetProcedureCommand(this IAdoRepository repository, string spName, params IAdoParameter[] parameters) {
            return GetProcedureCommand(repository, spName, AdoTool.DEFAULT_DISCOVER_PARAMETER, parameters);
        }

        /// <summary>
        /// 지정된 Stored Procedure를 실행할 <see cref="DbCommand"/> 인스턴스를 생성합니다.
        /// </summary>
        /// <param name="repository">IAdoRepository 인스턴스</param>
        /// <param name="spName">프로시저 명</param>
        /// <param name="discoverParams">DbCommand 인자를 DB에서 확인할 것인지 여부</param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static DbCommand GetProcedureCommand(this IAdoRepository repository, string spName, bool discoverParams,
                                                    params IAdoParameter[] parameters) {
            spName.ShouldNotBeWhiteSpace("spName");

            if(IsDebugEnabled)
                log.Debug("Procedure 실행을 위한 DbCommand를 생성합니다.. spName=[{0}], discoverParams=[{1}], parameters=[{2}]",
                          spName, discoverParams, parameters.CollectionToString());

            var cmd = repository.Db.GetStoredProcCommand(spName);

            if(discoverParams)
                repository.Db.DiscoverParameters(cmd);

            // command parameter 를 discover하지 않더라도 새로 정의할 parameter가 있다면...
            AdoTool.SetParameterValues(repository.Db, cmd, parameters);

            return cmd;
        }

        /// <summary>
        /// DataSet 변경 정보를 DB에 한꺼번에 적용하기 위해 Insert, Update, Delete Command를 제공해야 하는데,
        /// 이 때는 DataSet의 Source Column과 Command의 Parameter 이름을 매핑되어 있어야 한다.
        /// 이를 위해 Stored Procudure용 Command의 Parameter Name을 sourceColumns 것으로 교체한다.
        /// </summary>
        /// <param name="repository">IAdoRepository 인스턴스</param>
        /// <param name="spName">프로시저 명</param>
        /// <param name="sourceColumns">소스 컬럼명의 배열</param>
        /// <returns></returns>
        public static DbCommand GetProcedureCommandWithSourceColumn(this IAdoRepository repository, string spName,
                                                                    params string[] sourceColumns) {
            spName.ShouldNotBeWhiteSpace("spName");
            return repository.Db.GetStoredProcCommandWithSourceColumns(spName, sourceColumns);
        }

        /// <summary>
        /// QueryProvider에서 제공하는 Query로 <see cref="DbCommand"/> 인스턴스를 생성합니다.
        /// </summary>
        /// <param name="repository">IAdoRepository 인스턴스</param>
        /// <param name="queryKey">[Section,] QueryName 형태의 쿼리 키</param>
        /// <param name="parameters">parameter collection</param>
        /// <returns>DbCommand instance</returns>
        /// <exception cref="InvalidOperationException">QueryProvider 속성이 null일때</exception>
        public static DbCommand GetNamedQueryCommand(this IAdoRepository repository, string queryKey, params IAdoParameter[] parameters) {
            var query = repository.QueryProvider.GetQuery(queryKey);
            Guard.Assert(query.IsNotWhiteSpace(), "QueryKey=[{0}] 에 해당하는 쿼리 문을 찾지 못했습니다.", queryKey);

            return (AdoTool.IsSqlString(query))
                       ? repository.GetSqlStringCommand(query, parameters)
                       : repository.GetProcedureCommand(query, AdoTool.DEFAULT_DISCOVER_PARAMETER, parameters);
        }

        /// <summary>
        /// QueryProvider에서 제공하는 Query로 <see cref="DbCommand"/> 인스턴스를 생성합니다.
        /// </summary>
        /// <param name="repository">IAdoRepository 인스턴스</param>
        /// <param name="section">Section Name</param>
        /// <param name="queryName">쿼리 명</param>
        /// <param name="parameters">parameter collection</param>
        /// <returns>DbCommand instance</returns>
        /// <exception cref="InvalidOperationException">QueryProvider 속성이 null일 때</exception>
        public static DbCommand GetNamedQueryCommand(this IAdoRepository repository, string section, string queryName,
                                                     params IAdoParameter[] parameters) {
            var query = repository.QueryProvider.GetQuery(section, queryName);
            Guard.Assert(query.IsNotWhiteSpace(),
                         "Section=[{0}], QueryName=[{1}]에 해당하는 쿼리 문을 찾지 못했습니다.", section, queryName);

            return (AdoTool.IsSqlString(query))
                       ? repository.GetSqlStringCommand(query, parameters)
                       : repository.GetProcedureCommand(query, AdoTool.DEFAULT_DISCOVER_PARAMETER, parameters);
        }

        //! ===============================================================================================

        /// <summary>
        /// 지정된 Command를 실행한 결과 셋의 레코드 갯수를 구한다.
        /// </summary>
        /// <param name="repository">IAdoRepository 인스턴스</param>
        /// <param name="cmd">실행할 Command 객체</param>
        /// <param name="parameters">Command 인자 정보</param>
        /// <returns>결과 셋의 레코드 수</returns>
        /// <remarks>
        /// 실제 SQL의 count(*) 함수를 생성하는 것이 아니라, IDataReader를 이용하여 결과 셋을 가져와서 갯수만 센다.
        /// 장점은 DataSet을 이용하여 Paging하는 것보다 빠르고, Count 용 Query문을 따로 만들 필요가 없다.
        /// 단점은 SQL의 count(*) 함수보다는 당연히 느리다.
        /// </remarks>
        public static int Count(this IAdoRepository repository, DbCommand cmd, params IAdoParameter[] parameters) {
            cmd.ShouldNotBeNull("cmd");

            if(IsDebugEnabled)
                log.Debug("결과 셋의 갯수를 계산합니다. commandText=[{0}], parameters=[{1}]", cmd.CommandText, parameters.CollectionToString());

            using(var dr = repository.ExecuteReader(cmd, parameters)) {
                return dr.Count();
            }
        }

        /// <summary>
        /// 지정된 쿼리 문을 실행한 결과 셋의 레코드 갯수를 구한다.
        /// </summary>
        /// <param name="repository">IAdoRepository 인스턴스</param>
        /// <param name="query">실행할 Command 객체</param>
        /// <param name="parameters">Command 인자 정보</param>
        /// <returns>결과 셋의 레코드 수</returns>
        /// <remarks>
        /// 실제 SQL의 count(*) 함수를 생성하는 것이 아니라, IDataReader를 이용하여 결과 셋을 가져와서 갯수만 센다.
        /// 장점은 DataSet을 이용하여 Paging하는 것보다 빠르고, Count 용 Query문을 따로 만들 필요가 없다.
        /// 단점은 SQL의 count(*) 함수보다는 당연히 느리다.
        /// </remarks>
        public static int Count(this IAdoRepository repository, string query, params IAdoParameter[] parameters) {
            query.ShouldNotBeWhiteSpace("query");

            if(AdoTool.IsSqlString(query))
                return repository.CountBySqlString(query, parameters);

            using(var cmd = repository.GetCommand(query))
                return repository.Count(cmd, parameters);
        }

        /// <summary>
        /// 지정된 쿼리 문을 실행한 결과 셋의 레코드 갯수를 구한다. (SQL 문장이 SQL Count() 함수 문장으로 변환이 가능하다면 속도가 가장 빠르다.)
        /// </summary>
        /// <param name="repository">IAdoRepository 인스턴스</param>
        /// <param name="sqlString">실행할 Command 객체</param>
        /// <param name="parameters">Command 인자 정보</param>
        /// <returns>결과 셋의 레코드 수</returns>
        /// <remarks>
        /// 실제 SQL의 count(*) 함수로 변환할 수 있는 SQL문장이라면, 변환하여 Count 값을 가져오고, 
        /// 불가능하다면 IDataReader를 이용하여 결과 셋을 가져와서 갯수만 센다. 
        /// </remarks>
        /// <seealso cref="AdoTool.GetCountingSqlString"/>
        public static int CountBySqlString(this IAdoRepository repository, string sqlString, params IAdoParameter[] parameters) {
            sqlString.ShouldNotBeWhiteSpace("sqlString");

            // 1. SELECT Count(*) FROM 구문으로 변환이 가능하다면, 변환된 SQL 문장을 수행한다.
            try {
                var countSql = AdoTool.GetCountingSqlString(sqlString);
                var count = repository.ExecuteScalarBySqlString(countSql, parameters).AsInt(-1);

                if(count > -1)
                    return count;
            }
            catch(Exception ex) {
                if(log.IsInfoEnabled) {
                    log.Info("Count() SQL로 변환 작업이 지원되지 않는 문장입니다. DataReader를 이용하여, 결과 셋의 레코드 수를 알아봅니다.");
                    log.Info("sqlString=[{0}]", sqlString);
                    log.Info(ex);
                }
            }

            // 2. SQL Count() 함수를 이용하지 못할 경우라면, 그냥 DataReader를 이용하여 Count를 계산한다.
            //
            using(var cmd = repository.GetSqlStringCommand(sqlString))
                return repository.Count(cmd, parameters);
        }

        /// <summary>
        /// 지정된 쿼리 문을 실행한 결과 셋의 레코드 갯수를 구한다.
        /// </summary>
        /// <param name="repository">IAdoRepository 인스턴스</param>
        /// <param name="spName">실행할 Command 객체</param>
        /// <param name="parameters">Command 인자 정보</param>
        /// <returns>결과 셋의 레코드 수</returns>
        /// <remarks>
        /// 실제 SQL의 count(*) 함수를 생성하는 것이 아니라, IDataReader를 이용하여 결과 셋을 가져와서 갯수만 센다.
        /// 장점은 DataSet을 이용하여 Paging하는 것보다 빠르고, Count 용 Query문을 따로 만들 필요가 없다.
        /// 단점은 SQL의 count(*) 함수보다는 당연히 느리다.
        /// </remarks>
        public static int CountByProcedure(this IAdoRepository repository, string spName, params IAdoParameter[] parameters) {
            using(var cmd = repository.GetProcedureCommand(spName, true))
                return repository.Count(cmd, parameters);
        }

        //! ===============================================================================================

        /// <summary>
        /// 지정한 Command의 실행 결과 셋이 존재하는지 검사한다 (결과 셋의 레코드가 하나 이상이다)
        /// </summary>
        public static bool Exists(this IAdoRepository repository, DbCommand cmd, params IAdoParameter[] parameters) {
            cmd.ShouldNotBeNull("cmd");

            if(IsDebugEnabled)
                log.Debug("결과 셋이 존재하는지 검사합니다... commandText=[{0}], parameters=[{1}]", cmd.CommandText, parameters.CollectionToString());

            using(var dr = repository.ExecuteReader(cmd, parameters))
                return dr.Read();
        }

        /// <summary>
        /// 지정한 쿼리 문의 실행 결과 셋이 존재하는지 검사한다 (결과 셋의 레코드가 하나 이상이다)
        /// </summary>
        public static bool Exists(this IAdoRepository repository, string query, params IAdoParameter[] parameters) {
            using(var cmd = repository.GetCommand(query))
                return repository.Exists(cmd, parameters);
        }

        /// <summary>
        /// 지정한 쿼리 문의 실행 결과 셋이 존재하는지 검사한다 (결과 셋의 레코드가 하나 이상이다)
        /// </summary>
        public static bool ExistsBySqlString(this IAdoRepository repository, string sqlString, params IAdoParameter[] parameters) {
            using(var cmd = repository.GetCommand(sqlString))
                return repository.Exists(cmd, parameters);
        }

        /// <summary>
        /// 지정한 Procedure의 실행 결과 셋이 존재하는지 검사한다 (결과 셋의 레코드가 하나 이상이다)
        /// </summary>
        public static bool ExistsByProcedure(this IAdoRepository repository, string spName, params IAdoParameter[] parameters) {
            using(var cmd = repository.GetProcedureCommand(spName, true))
                return repository.Exists(cmd, parameters);
        }

        //! ===============================================================================================

        /// <summary>
        /// 지정된 Command를 수행하고, RETURN_VALUE를 반환합니다.
        /// </summary>
        /// <param name="repository">IAdoRepository 인스턴스</param>
        /// <param name="cmd">실행할 DbCommand</param>
        /// <param name="parameters">인자</param>
        /// <returns>Procedure인 경우 return value를 반환한다. 반환값이 없으면 0을 반환한다.</returns>
        public static object ExecuteCommand(this IAdoRepository repository, DbCommand cmd, params IAdoParameter[] parameters) {
            cmd.ShouldNotBeNull("cmd");

            repository.ExecuteNonQuery(cmd, parameters);

            return AdoTool.GetReturnValue(repository.Db, cmd, 0);
        }

        /// <summary>
        /// Stored Procedure를 실행하고, Parameter의 Direction이 INPUT이 아닌 Parameter들을 반환한다. (InputOutput, Output, ReturnValue)
        /// </summary>
        /// <param name="repository">IAdoRepository 인스턴스</param>
        /// <param name="spName">실행할 Procedure 이름</param>
        /// <param name="parameters">인자</param>
        /// <returns>INPUT을 제외한 Oupput, InputOutput, ReturnValue에 해당하는 Parameter 값을 반환한다.</returns>
        public static IAdoParameter[] ExecuteProcedure(this IAdoRepository repository, string spName, params IAdoParameter[] parameters) {
            if(IsDebugEnabled)
                log.Debug("Procedure를 수행합니다... spName=[{0}], parameters=[{1}]", spName, parameters.CollectionToString());

            using(var cmd = repository.GetProcedureCommand(spName)) {
                repository.ExecuteCommand(cmd, parameters);
                var result = AdoTool.GetOutputParameters(repository.Db, cmd);

                if(IsDebugEnabled)
                    log.Debug("Procedure 수행 후 결과 값입니다. spName=[{0}], output parameters=[{1}]", spName, result.CollectionToString());

                return result;
            }
        }

        /// <summary>
        /// 지정된 Entity의 속성 값을 이용하여 Command의 Parameter 값을 설정하고, 실행시킨다.
        /// 일반적으로 Save / Update시에 활용하면 좋다.
        /// </summary>
        /// <typeparam name="T">Persistent object 수형</typeparam>
        /// <param name="repository">IAdoRepository 인스턴스</param>
        /// <param name="cmd">수행할 Command 객체</param>
        /// <param name="entity">처리할 Persistent object</param>
        /// <param name="nameMaps">ParameterName of Procedure = Property Name of Persistent object 매핑 정보</param>
        /// <returns>Command 인자 중에 ParameterDirection이 ReturnValue인 인자의 값</returns>
        public static object ExecuteEntity<T>(this IAdoRepository repository, DbCommand cmd, T entity, INameMap nameMaps) {
            cmd.ShouldNotBeNull("cmd");
            entity.ShouldNotBeNull("entity");
            nameMaps.ShouldNotBeNull("nameMap");

            if(IsDebugEnabled)
                log.Debug("Entity를 처리를 수행합니다. CommandText=[{0}], entity=[{1}], nameMaps=[{2}]",
                          cmd.CommandText, entity.ObjectToString(), nameMaps.DictionaryToString());

            AdoTool.SetParameterValues(repository.Db, cmd, entity, nameMaps);
            return repository.ExecuteCommand(cmd);
        }

        /// <summary>
        /// 지정된 Entity의 속성 값을 이용하여 Procedure의 Parameter 값을 설정하고, 실행시킨다.
        /// 일반적으로 Save / Update시에 활용하면 좋다.
        /// </summary>
        /// <typeparam name="T">Persistent object 수형</typeparam>
        /// <param name="repository">IAdoRepository 인스턴스</param>
        /// <param name="spName">수행할 Procedure 명</param>
        /// <param name="entity">Persistent object</param>
        /// <param name="nameMaps">ParameterName of Procedure = Property Name of Persistent object</param>
        /// <returns>Command 인자 중에 ParameterDirection이 ReturnValue인 인자의 값</returns>
        public static object ExecuteEntity<T>(this IAdoRepository repository, string spName, T entity, INameMap nameMaps) {
            using(var cmd = repository.GetProcedureCommand(spName, true))
                return repository.ExecuteEntity(cmd, entity, nameMaps);
        }

        /// <summary>
        /// 지정된 Procedure를 수행한다. 인자로 entity의 속성값을 이용한다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="repository">IAdoRepository 인스턴스</param>
        /// <param name="cmd">실행할 DbCommand</param>
        /// <param name="entity">실행할 Entity</param>
        /// <param name="nameMapper">Name Mapping Class</param>
        /// <returns>Command 인자 중에 ParameterDirection이 ReturnValue인 인자의 값</returns>
        public static object ExecuteEntity<T>(this IAdoRepository repository, DbCommand cmd, T entity, INameMapper nameMapper) {
            return repository.ExecuteEntity(cmd, entity, cmd.NameMapping(nameMapper));
        }

        /// <summary>
        /// 지정된 Procedure를 수행한다. 인자로 entity의 속성값을 이용한다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="repository">IAdoRepository 인스턴스</param>
        /// <param name="spName">실행할 Procedure Name</param>
        /// <param name="entity">실행할 Entity</param>
        /// <param name="nameMapper">Name Mapping Class</param>
        /// <returns>Command 인자 중에 ParameterDirection이 ReturnValue인 인자의 값</returns>
        public static object ExecuteEntity<T>(this IAdoRepository repository, string spName, T entity, INameMapper nameMapper) {
            using(var cmd = repository.GetProcedureCommand(spName, true))
                return repository.ExecuteEntity(cmd, entity, nameMapper);
        }
    }
}