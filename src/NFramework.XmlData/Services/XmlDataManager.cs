using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using NSoft.NFramework.Data;
using NSoft.NFramework.Tools;
using NSoft.NFramework.XmlData.Messages;

namespace NSoft.NFramework.XmlData {
    /// <summary>
    /// Client의 DB 처리 요청 정보(<see cref="XdsRequestDocument"/>)를 파싱하여 처리 후 
    /// 결과를 <see cref="XdsResponseDocument"/> 객체에 담아 반환하는 기능을 수행한다.
    /// </summary>
    public class XmlDataManager : IXmlDataManager {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public XmlDataManager() : this(AdoTool.DefaultDatabaseName) {}

        public XmlDataManager(string dbName = null) {
            Ado = XmlDataTool.ResolveAdoRepository(dbName ?? AdoTool.DefaultDatabaseName);
        }

        public XmlDataManager(IAdoRepository ado) {
            ado.ShouldNotBeNull("ado");
            Ado = ado;
        }

        /// <summary>
        /// Data 처리 시에 사용할 <see cref="IAdoRepository"/> 인스턴스
        /// </summary>
        public IAdoRepository Ado { get; set; }

        /// <summary>
        /// 요청정보를 실행하여 응답정보를 반환합니다.
        /// </summary>
        /// <param name="requestDocument">요청정보</param>
        /// <returns>요청 처리 응답정보</returns>
        public virtual XdsResponseDocument Execute(XdsRequestDocument requestDocument) {
            return Execute(requestDocument, 90);
        }

        /// <summary>
        /// 요청정보를 실행하여 응답정보를 반환합니다.
        /// </summary>
        /// <param name="requestDocument">요청정보</param>
        /// <param name="commandTimeout">요청 처리 제한 시간 (단위 : seconds, 기본값 : 90)</param>
        /// <returns>요청 처리 응답정보</returns>
        public virtual XdsResponseDocument Execute(XdsRequestDocument requestDocument, int? commandTimeout) {
            if(IsDebugEnabled)
                log.Debug("요청정보에 따른 Database 작업을 시작합니다...");

            requestDocument.ShouldNotBeNull("requestDocument");

            if(requestDocument == null)
                return XmlDataTool.CreateResponseWithError(new InvalidOperationException(MsgConsts.NoRequestProvided));

            var responseDocument = new XdsResponseDocument();
            TransactionScope txScope = null;

            try {
                if(IsDebugEnabled)
                    log.Debug("요청문서 분석 및 응답문서를 초기화 합니다.");

                requestDocument.CopyRequestHeader(responseDocument);

                if(requestDocument.CommandTimeout > commandTimeout.GetValueOrDefault(90))
                    commandTimeout = requestDocument.CommandTimeout;

                if(requestDocument.Transaction) {
                    if(IsDebugEnabled)
                        log.Debug("요청문서에 Transaction 적용 옵션이 설정되어 있습니다. TransactionScope를 이용하여, 전체 요청을 Transaction 하에서 실행합니다.");

                    txScope = AdoTool.CreateTransactionScope(TransactionScopeOption.Required, IsolationLevel.ReadCommitted);
                }

                //! 실제 요청 처리
                ExecuteRequestCore(requestDocument, responseDocument);


                // Transaction을 Commit 한다
                // 
                if(txScope != null) {
                    if(IsDebugEnabled)
                        log.Debug("Transaction Commit을 시작합니다...");

                    txScope.Complete();

                    if(IsDebugEnabled)
                        log.Debug("Transaction Commit에 성공했습니다.!!!");
                }
            }
            catch(Exception ex) {
                responseDocument.ReportError(ex);
            }
            finally {
                if(txScope != null)
                    txScope.Dispose();
            }

            if(IsDebugEnabled)
                log.Debug("모든 요청을 처리하고, ResponseDocument를 반환합니다.");

            return responseDocument;
        }

        /// <summary>
        /// 요청에 대한 실제 작업을 수행합니다.
        /// </summary>
        protected virtual void ExecuteRequestCore(XdsRequestDocument requestDocument, XdsResponseDocument responseDocument) {
            if(IsDebugEnabled)
                log.Debug("요청 정보를 옵션에 따라 동기적으로 실행합니다...");

            foreach(var request in requestDocument.Requests) {
                // 메소드인 경우는 QueryProvider를 이용하여, 실제 Query 문장으로 변경한다.
                //
                XmlDataTool.ConvertToQuery(Ado, request);

                ExecuteSimpleQuery(Ado, request.PreQueries);

                //! 실제 수행
                DoProcessRequestItem(request, responseDocument);

                ExecuteSimpleQuery(Ado, request.PostQueries);
            }
        }

        /// <summary>
        /// <paramref name="request"/> 요청에 대해, 처리를 수행합니다.
        /// </summary>
        /// <param name="request">요청항목</param>
        /// <param name="responseDocument">응답 정보</param>
        protected void DoProcessRequestItem(XdsRequestItem request, XdsResponseDocument responseDocument) {
            request.ShouldNotBeNull("request");

            var isDataSet = (request.ResponseKind == XmlDataResponseKind.DataSet);

            switch(request.RequestKind) {
                case XmlDataRequestKind.StoredProc:

                    if(isDataSet)
                        OpenProcedure(request, responseDocument);
                    else
                        ExecuteProcedure(request, responseDocument);
                    break;

                case XmlDataRequestKind.Query:

                    if(isDataSet)
                        OpenQuery(request, responseDocument);
                    else
                        ExecuteQuery(request, responseDocument);
                    break;

                default:
                    throw new NotSupportedException("지원하지 않는 요청 종류입니다. RequestType=" + request.RequestKind);
            }

            // 요청정보중에 확인을 위해 응답정보에 조건들을 기록해둔다.
            //
            if(isDataSet)
                WriteRequestInfomation(request, responseDocument);
        }

        /// <summary>
        /// Stored Procedure를 수행하고, OUTPUT, RETURN_VALUE를 반환한다.
        /// </summary>
        /// <param name="request">요청 항목</param>
        /// <param name="responseDocument">응답 정보</param>
        protected virtual void ExecuteProcedure(XdsRequestItem request, XdsResponseDocument responseDocument) {
            if(IsDebugEnabled)
                log.Debug("프로시저를 실행합니다... requestId=[{0}], procedure=[{1}], requestType=[{2}], responseType=[{3}]",
                          request.Id, request.Query, request.RequestKind, request.ResponseKind);
            try {
                var maxSequence = Math.Max(1, request.Values.Count);
                for(var seqId = 0; seqId < maxSequence; seqId++) {
                    var adoParameters = PrepareParameters(request, seqId).ToArray();

                    using(var cmd = Ado.GetProcedureCommand(request.Query)) {
                        Ado.ExecuteNonQuery(cmd, adoParameters);
                        var response = responseDocument.AddResponseItem(XmlDataResponseKind.Scalar, request.Id, seqId);
                        ExtractParameters(AdoTool.GetOutputParameters(Ado.Db, cmd), response, seqId);
                    }
                }
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.ErrorException(string.Format("프로시저 실행에 실패했습니다. procedure=[{0}]", request.Query), ex);
                throw;
            }

            if(IsDebugEnabled)
                log.Debug("프로시저 실행에 성공했습니다. procedure=[{0}]", request.Query);
        }

        /// <summary>
        /// 일반 SQL Query 문 중에 DDL 과 관련된 문장을 수행한다. (결과 SET이 필요없는 것)
        /// </summary>
        /// <param name="request">요청 항목</param>
        /// <param name="responseDocument">응답 정보</param>
        protected virtual void ExecuteQuery(XdsRequestItem request, XdsResponseDocument responseDocument) {
            if(IsDebugEnabled)
                log.Debug("쿼리문장을 실행합니다... requestId=[{0}], queryString=[{1}], requestType=[{2}], responseType=[{3}]",
                          request.Id, request.Query, request.RequestKind, request.ResponseKind);
            try {
                var isScalar = request.ResponseKind == XmlDataResponseKind.Scalar;
                var maxSequence = Math.Max(1, request.Values.Count);

                for(var seqId = 0; seqId < maxSequence; seqId++) {
                    var adoParameters = PrepareParameters(request, seqId).ToArray();

                    using(var cmd = Ado.GetSqlStringCommand(request.Query)) {
                        var result = (isScalar)
                                         ? Ado.ExecuteScalar(cmd, adoParameters)
                                         : Ado.ExecuteNonQuery(cmd, adoParameters);

                        var responseItem = responseDocument.AddResponseItem(request.ResponseKind, request.Id, seqId);

                        if(isScalar) {
                            responseItem.Fields.AddField("RETURN_VALUE", typeof(int).Name, 4);
                            responseItem.Records.AddColumnArray(result);
                        }
                    }
                }
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.ErrorException(string.Format("쿼리 문 실행에 실패했습니다. query=[{0}]", request.Query), ex);
                throw;
            }

            if(IsDebugEnabled)
                log.Debug("쿼리문 실행에 성공했습니다. query=[{0}]", request.Query);
        }

        /// <summary>
        /// StoredProcedure 를 실행하고, 결과 Set을 반환받고자 하는 요청에 대해
        /// 요청 정보를 파싱하고, StoredProcedure를 수행한 후, 결과 정보를 응답 객체에 저장한다. 
        /// </summary>
        /// <param name="request">요청 항목</param>
        /// <param name="responseDocument">응답 정보</param>
        protected virtual void OpenProcedure(XdsRequestItem request, XdsResponseDocument responseDocument) {
            request.ShouldNotBeNull("request");

            if(IsDebugEnabled)
                log.Debug(
                    "프로시저를 실행하여 결과셋으로 빌드합니다... requestId=[{0}], procedure=[{1}], requestType=[{2}], responseType=[{3}], sort=[{4}]",
                    request.Id, request.Query, request.RequestKind, request.ResponseKind, request.Sort);

            try {
                var maxSequence = Math.Max(1, request.Values.Count);

                for(var seqId = 0; seqId < maxSequence; seqId++) {
                    var adoParameters = PrepareParameters(request, seqId).ToArray();

                    using(var cmd = Ado.GetProcedureCommand(request.Query))
                    using(var pagingTable = Ado.ExecutePagingDataTable(cmd,
                                                                       Math.Max(0, request.PageNo - 1),
                                                                       request.PageSize,
                                                                       adoParameters))
                    using(var dv = pagingTable.Table.DefaultView) {
                        if(request.Sort.IsNotWhiteSpace())
                            dv.Sort = request.Sort;

                        responseDocument.Responses.AddResponseItem(dv, request.Id, seqId);
                    }
                }
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.ErrorException(string.Format("프로시저 실행에 실패했습니다. procedure=[{0}]", request.Query), ex);

                throw;
            }
            if(IsDebugEnabled)
                log.Debug("프로시저 실행에 성공했습니다. procedure=[{0}]", request.Query);
        }

        /// <summary>
        /// 일반 Query문을 수행하여 DataSet을 받아서 ResponseDom 객체를 만든다.
        /// </summary>
        /// <param name="request">요청 항목</param>
        /// <param name="responseDocument">응답 정보</param>
        protected virtual void OpenQuery(XdsRequestItem request, XdsResponseDocument responseDocument) {
            request.ShouldNotBeNull("request");

            if(IsDebugEnabled)
                log.Debug("쿼리문을 수행하여 결과셋을 얻습니다... request Id=[{0}], query=[{1}], requestType=[{2}], responseType=[{3}], sort=[{4}]",
                          request.Id, request.Query, request.RequestKind, request.ResponseKind, request.Sort);

            try {
                var maxSequence = Math.Max(1, request.Values.Count);

                for(var seqId = 0; seqId < maxSequence; seqId++) {
                    var adoParameters = PrepareParameters(request, seqId).ToArray();

                    using(var cmd = Ado.GetCommand(request.Query))
                    using(var pagingTable = Ado.ExecutePagingDataTable(cmd,
                                                                       Math.Max(0, request.PageNo - 1),
                                                                       request.PageSize,
                                                                       adoParameters))
                    using(var dv = pagingTable.Table.DefaultView) {
                        if(request.Sort.IsNotWhiteSpace())
                            dv.Sort = request.Sort;

                        responseDocument.Responses.AddResponseItem(dv, request.Id, seqId);
                    }
                }
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.ErrorException(string.Format("쿼리문을 수행하여 결과셋을 얻는데 실패했습니다. query=[{0}]", request.Query), ex);

                throw;
            }

            if(IsDebugEnabled)
                log.Debug("쿼리문을 수행하여 결과셋을 얻는데 성공했습니다. query=[{0}]", request.Query);
        }

        /// <summary>
        /// 요청정보중에 확인을 위해 응답정보에 조건들을 기록해둔다.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="responseDocument"></param>
        private static void WriteRequestInfomation(XdsRequestItem request, XdsResponseDocument responseDocument) {
            var index = responseDocument.Responses.Count - 1;

            if(index >= 0) {
                responseDocument[index].PageSize = request.PageSize;
                responseDocument[index].PageNo = request.PageNo;
                responseDocument[index].Sort = request.Sort;
                responseDocument[index].TotalRecordCount = responseDocument[index].Records.Count;

                if(responseDocument[index].PageSize > 0)
                    responseDocument[index].PageCount = responseDocument[index].TotalRecordCount / responseDocument[index].PageSize + 1;
            }
        }

        /// <summary>
        /// Stored Procudure 실행 후 OUTPUT, RETURN_VALUE 값을 응답개체(XdsResponseItem)에 추가한다.
        /// </summary>
        /// <param name="outputs">Command Parameter 중에 INPUT이 아닌 Parameter 들</param>
        /// <param name="response">Instance of XdsResponseItem to build with result of execution of DbCommand
        /// <param name="responseIndex">index of response</param>
        protected static void ExtractParameters(IAdoParameter[] outputs, XdsResponseItem response, int responseIndex) {
            if(outputs == null || outputs.Length == 0)
                return;

            foreach(var output in outputs) {
                response.Fields.Add(new XdsField(output.Name.RemoveParameterPrefix(),
                                                 DbFunc.GetLanguageType(output.ValueType).FullName,
                                                 output.Size.GetValueOrDefault(0)));
            }

            var record = new XdsRecord();

            foreach(var output in outputs)
                record.Columns.AddColumn(output.Value);

            response.Records.Add(record);
        }

        /// <summary>
        /// DbCommand 인스턴스의 Parameters 컬렉션 속성에 값을 설정한다.
        /// </summary>
        /// <param name="requestItem">XdsRequestItem 개체 - Stored Procedure 호출에 필요한 파라미터 정보와 Value값이 있다.</param>
        /// <param name="requestIndex">RclXdsRequest의 요청 인덱스</param>
        protected static IList<IAdoParameter> PrepareParameters(XdsRequestItem requestItem, int requestIndex) {
            var adoParameters = new List<IAdoParameter>();

            var skipPrepareParams = requestItem.Parameters.Count == 0 || requestItem.Values.Count == 0;

            if(skipPrepareParams)
                return adoParameters;

            var value = requestItem.Values[requestIndex];
            CheckParmeterAndArguments(requestItem, value);

            for(var i = 0; i < value.Arguments.Count; i++) {
                var xp = requestItem.Parameters[i];
                adoParameters.Add(new AdoParameter(xp.ParamName, value.Arguments[i].Argument));
            }
            return adoParameters;
        }

        /// <summary>
        /// Parameter로 정의된 요소와 실제 Parameter에 대응할 값의 갯수를 비교해서 다르다면 예외를 발생시킨다.
        /// </summary>
        /// <param name="requestItem">요청항목</param>
        /// <param name="value"></param>
        private static void CheckParmeterAndArguments(XdsRequestItem requestItem, XdsValue value) {
            if(requestItem.Parameters.Count != value.Arguments.Count) {
                throw
                    new InvalidOperationException(
                        string.Format("요청정보 (Id=[{0}]]) 의 인자정보[{1}]번째  파라미터 갯수[{2}]와 값의 갯수[{3}]가 틀립니다.",
                                      requestItem.Id,
                                      requestItem.Id,
                                      requestItem.Parameters.Count,
                                      value.Arguments.Count));
            }
        }

        /// <summary>
        /// PREQUERIES, POSTQUERIES에 있는 결과를 반환할 필요없는 단순 Query문을 실행합니다.
        /// </summary>
        /// <param name="ado">AdoRepository</param>
        /// <param name="queries">queries to execute</param>
        private static void ExecuteSimpleQuery(IAdoRepository ado, IEnumerable<XdsQuery> queries) {
            foreach(var query in queries.Where(q => q.Query.IsNotWhiteSpace()).Select(q => q.Query)) {
                if(IsDebugEnabled)
                    log.Debug("요청 처리 사전/사후 설정용 쿼리문을 수행합니다... query=[{0}]", query);

                ado.ExecuteNonQuery(query);
            }
        }
    }
}