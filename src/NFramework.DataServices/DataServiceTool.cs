using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using NSoft.NFramework.Data;
using NSoft.NFramework.Data.Mappers;
using NSoft.NFramework.DataServices.Adapters;
using NSoft.NFramework.DataServices.Messages;
using NSoft.NFramework.InversionOfControl;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.DataServices {
    /// <summary>
    /// Data Service를 위한 Tool Class 입니다.
    /// </summary>
    public static class DataServiceTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// Invalid Identity Value of Object (-1)
        /// </summary>
        public const int INVALID_ID = -1;

        /// <summary>
        /// No Page Index (0)
        /// </summary>
        public const int NO_PAGE_INDEX = 0;

        /// <summary>
        /// No Request Message provided.
        /// </summary>
        public const string NoRequestProvided = "요청 메시지 (Request Message)가 제공되지 않았습니다.";

        public const string DataServiceAdapterPrefix = "DataServiceAdapter";
        public const string DataServicePrefix = "DataService";
        public const string MessageSerializerPrefix = "MessageSerializer";

        /// <summary>
        /// Database 명을 가져옵니다.
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        internal static string GetDatabaseName(this string dbName) {
            return dbName.IsWhiteSpace() ? AdoTool.DefaultDatabaseName : dbName;
        }

        /// <summary>
        /// 지정된 DB에 대해 작업을 수행하는 <see cref="IDataServiceAdapter"/>를 Container로부터 Resolve 합니다.
        /// Component 명은 <see cref="DataServiceAdapterPrefix"/>.<see cref="AdoTool.DefaultDatabaseName"/> 형식입니다.
        /// </summary>
        /// <returns><see cref="IDataServiceAdapter"/> 인스턴스</returns>
        public static IDataServiceAdapter ResolveDataServiceAdapter() {
            return ResolveDataServiceAdapter(AdoTool.DefaultDatabaseName);
        }

        /// <summary>
        /// 지정된 DB에 대해 작업을 수행하는 <see cref="IDataServiceAdapter"/>를 Container로부터 Resolve 합니다.
        /// Component 명은 <see cref="DataServiceAdapterPrefix"/>.<paramref name="dbName"/> 형식입니다.
        /// </summary>
        /// <param name="dbName">Data 요청을 처리할 DB ConnectionString Name</param>
        /// <returns><see cref="IDataServiceAdapter"/> 인스턴스</returns>
        public static IDataServiceAdapter ResolveDataServiceAdapter(string dbName) {
            var componentId = DataServiceAdapterPrefix + "." + dbName.GetDatabaseName();

            if(IsDebugEnabled)
                log.Debug("IoC로부터 DataServiceAdapter 컴포넌트를 Resolve합니다. dbName=[{0}], componentId=[{1}]", dbName, componentId);

            return IoC.Resolve<IDataServiceAdapter>(componentId);
        }

        /// <summary>
        /// IoC Container로부터 <see cref="IDataService"/> 컴포넌트를 Resolve 합니다.
        /// </summary>
        /// <returns></returns>
        public static IDataService ResolveDataService() {
            return ResolveDataService(AdoTool.DefaultDatabaseName);
        }

        /// <summary>
        /// IoC Container로부터 <see cref="IDataService"/> 컴포넌트를 Resolve 합니다.
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public static IDataService ResolveDataService(string dbName) {
            var componentId = DataServicePrefix + "." + dbName.GetDatabaseName();

            if(IsDebugEnabled)
                log.Debug("IoC로부터 DataService 컴포넌트를 Resolve합니다. dbName=[{0}], componentId=[{1}]", dbName, componentId);

            return IoC.Resolve<IDataService>(componentId);
        }

        /// <summary>
        /// <see cref="ISerializer{RequestMessage}"/>를 생성합니다.
        /// </summary>
        /// <returns></returns>
        public static ISerializer<RequestMessage> ResolveRequestSerializer() {
            return ResolveRequestSerializer(AdoTool.DefaultDatabaseName);
        }

        /// <summary>
        /// <see cref="ISerializer{RequestMessage}"/>를 생성합니다.
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public static ISerializer<RequestMessage> ResolveRequestSerializer(string dbName) {
            var componentId = MessageSerializerPrefix + "." + dbName.GetDatabaseName();
            return IoC.Resolve<ISerializer<RequestMessage>>(componentId);
        }

        /// <summary>
        /// <see cref="ISerializer{ResponseMessage}"/>를 생성합니다.
        /// </summary>
        /// <returns></returns>
        public static ISerializer<ResponseMessage> ResolveResponseSerializer() {
            return ResolveResponseSerializer(AdoTool.DefaultDatabaseName);
        }

        /// <summary>
        /// <see cref="ISerializer{ResponseMessage}"/>를 생성합니다.
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public static ISerializer<ResponseMessage> ResolveResponseSerializer(string dbName) {
            var componentId = MessageSerializerPrefix + "." + dbName.GetDatabaseName();
            return IoC.Resolve<ISerializer<ResponseMessage>>(componentId);
        }

        /// <summary>
        /// <paramref name="reader"/>를 읽어, <see cref="ResultRow"/>의 컬렉션인 <see cref="ResultSet"/>을 빌드합니다.
        /// </summary>
        /// <param name="reader">DataReader</param>
        /// <returns><see cref="ResultSet"/> 인스턴스</returns>
        public static ResultSet CreateResultSet(this IDataReader reader) {
            return CreateResultSet(reader, (INameMapper)null, 0, int.MaxValue);
        }

        /// <summary>
        /// <paramref name="reader"/>를 읽어, <see cref="ResultRow"/>의 컬렉션인 <see cref="ResultSet"/>을 빌드합니다.
        /// </summary>
        /// <param name="reader">DataReader</param>
        /// <param name="firstResult">첫번째 레코드 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">최대 레코드 수</param>
        /// <returns><see cref="ResultSet"/> 인스턴스</returns>
        public static ResultSet CreateResultSet(this IDataReader reader, int firstResult, int maxResults) {
            return CreateResultSet(reader, (INameMapper)null, firstResult, maxResults);
        }

        /// <summary>
        /// <paramref name="reader"/>를 읽어, <see cref="ResultRow"/>의 컬렉션인 <see cref="ResultSet"/>을 빌드합니다.
        /// </summary>
        /// <param name="reader">DataReader</param>
        /// <param name="nameMapper">컬럼명을 속성명으로 매핑해주는 매퍼입니다.</param>
        /// <returns><see cref="ResultSet"/> 인스턴스</returns>
        public static ResultSet CreateResultSet(this IDataReader reader, INameMapper nameMapper) {
            return CreateResultSet(reader, nameMapper, 0, int.MaxValue);
        }

        /// <summary>
        /// <paramref name="reader"/>를 읽어, <see cref="ResultRow"/>의 컬렉션인 <see cref="ResultSet"/>을 빌드합니다.
        /// </summary>
        /// <param name="reader">DataReader</param>
        /// <param name="nameMapper">컬럼명을 속성명으로 매핑해주는 매퍼입니다.</param>
        /// <param name="firstResult">첫번째 레코드 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">최대 레코드 수</param>
        /// <returns><see cref="ResultSet"/> 인스턴스</returns>
        public static ResultSet CreateResultSet(this IDataReader reader, INameMapper nameMapper, int firstResult, int maxResults) {
            reader.ShouldNotBeNull("reader");

            if(nameMapper == null)
                nameMapper = new SameNameMapper();

            if(IsDebugEnabled)
                log.Debug("IDataReader로부터 정보를 읽어 ResultRow들을 만들어 ResultSet 으로 빌드합니다...");

            var resultSet = new ResultSet();

            while(firstResult-- > 0) {
                if(reader.Read() == false)
                    return resultSet;
            }

            if(maxResults <= 0)
                maxResults = int.MaxValue;

            resultSet.FieldNames = reader.GetFieldNames();

            var rowIndex = 0;
            while(reader.Read() && rowIndex++ < maxResults) {
                var row = new ResultRow();

                foreach(var fieldName in resultSet.FieldNames)
                    row.Add(nameMapper.MapToPropertyName(fieldName), reader.AsValue(fieldName));

                resultSet.Add(row);
            }

            if(IsDebugEnabled)
                log.Debug("IDataReader로부터 정보를 읽어 ResultSet을 빌드했습니다!!! Row Count=[{0}]", resultSet.Count);

            return resultSet;
        }

        /// <summary>
        /// <paramref name="reader"/>를 읽어, <see cref="ResultRow"/>를 빌드합니다.
        /// </summary>
        /// <param name="reader">DataReader</param>
        /// <param name="fieldNames">컬럼명 리스트</param>
        /// <param name="nameMapper">컬럼명을 속성명으로 매핑해주는 매퍼입니다.</param>
        /// <returns></returns>
        internal static ResultRow CreateResultRow(this IDataReader reader, IList<string> fieldNames, INameMapper nameMapper) {
            Guard.Assert(reader.IsClosed == false, "reader 가 닫혀있습니다.");

            var row = new ResultRow();

            foreach(var fieldName in fieldNames)
                row.Add(nameMapper.MapToPropertyName(fieldName), reader.AsValue(fieldName));

            return row;
        }

        /// <summary>
        /// <paramref name="requestMessage"/>의 Method 를 서버에서 관련 Query 문장으로 매핑합니다.
        /// </summary>
        public static void LoadQueryStatements(this IDataService dataService, RequestMessage requestMessage) {
            if(IsDebugEnabled)
                log.Debug("요청 메시지의 모든 요청 정보를 서버의 설정 쿼리문으로 매핑합니다... requestMessage.MessageId=[{0}]", requestMessage.MessageId);

            dataService.AssertDataService();
            requestMessage.ShouldNotBeNull("requestMessage");

            var queryProvider = dataService.AdoRepository.QueryProvider;

            // 1. 전체 메시지의 사전 작업
            foreach(var method in requestMessage.PrepareStatements)
                requestMessage.PreQueries.Add(queryProvider.GetQueryString(method, method));

            // 2. 요청 항목 별
            foreach(var requestItem in requestMessage.Items) {
                if(IsDebugEnabled)
                    log.Debug("요청 메소드에 해당하는 실제 Query 문을 로드합니다... requestItem.Method=[{0}]", requestItem.Method);

                foreach(var method in requestItem.PrepareStatements)
                    requestItem.PreQueries.Add(queryProvider.GetQueryString(method, method));

                requestItem.Query = (requestItem.RequestMethod == RequestMethodKind.Method)
                                        ? queryProvider.GetQueryString(requestItem.Method, requestItem.Method)
                                        : requestItem.Method;

                foreach(var method in requestItem.PostscriptStatements)
                    requestItem.PostQueries.Add(queryProvider.GetQueryString(method, method));
            }

            // 3. 전체 메시지의 사후 작업
            foreach(var method in requestMessage.PostscriptStatements)
                requestMessage.PostQueries.Add(queryProvider.GetQueryString(method, method));

            if(IsDebugEnabled)
                log.Debug("요청 메소드에 해당하는 실제 Query 문을 로드했습니다!!!");
        }

        /// <summary>
        /// <paramref name="dataService"/>의 주요 속성에 대한 유효성 검사를 수행합니다. 유효성을 만족하지 못하면, <see cref="InvalidOperationException"/>을 발생시킵니다.
        /// </summary>
        public static void AssertDataService(this IDataService dataService) {
            dataService.ShouldNotBeNull("dataService");

            if(dataService.AdoRepository == null)
                throw new InvalidOperationException("DataService의 AdoRepository 속성 값이 NULL입니다.");

            if(dataService.AdoRepository.QueryProvider == null)
                throw new InvalidOperationException("DataService.AdoRepository의 QueryProvider가 NULL입니다.");
        }

        /// <summary>
        /// <paramref name="responseMessage"/>에 예외정보를 추가합니다.
        /// </summary>
        public static void ReportError(this ResponseMessage responseMessage, Exception ex) {
            if(ex == null)
                return;

            responseMessage.AddError(ex);

            if(log.IsWarnEnabled)
                log.WarnException("작업처리 중 예외가 발생하여, 응답 메시지에 예외정보를 추가했습니다.", ex);
        }

        /// <summary>
        /// 지정된 예외정보를 포함하는 응답메시지를 빌드합니다. 
        /// </summary>
        /// <param name="exceptions"></param>
        /// <returns></returns>
        public static ResponseMessage CreateResponseMessageWithException(params Exception[] exceptions) {
            var responseMsg = new ResponseMessage();

            foreach(var ex in exceptions)
                responseMsg.AddError(ex);

            return responseMsg;
        }

        /// <summary>
        /// <paramref name="requestItem"/>의 Parameters 정보를 바탕으로 <see cref="IAdoParameter"/> 컬렉션을 빌드합니다.
        /// </summary>
        /// <param name="requestItem"></param>
        /// <returns></returns>
        public static IList<IAdoParameter> BuildAdoParameters(this RequestItem requestItem) {
            return
                requestItem.Parameters
                    .Select(parameter => new AdoParameter(parameter.Name, parameter.Value))
                    .Cast<IAdoParameter>()
                    .ToList();
        }
    }
}