using System;
using NSoft.NFramework.Data;
using NSoft.NFramework.XmlData.Messages;

namespace NSoft.NFramework.XmlData {
    public static partial class XmlDataTool {
        /// <summary>
        /// 요청정보를 받은 해당 Database에서 작업하고, 결과를 반환한다.
        /// </summary>
        /// <param name="requestDocument">요청문서</param>
        /// <param name="dbName">요청을 실행할 Database connectionString name</param>
        /// <returns>응답문서</returns>
        public static XdsResponseDocument Execute(this XdsRequestDocument requestDocument, string dbName) {
            requestDocument.ShouldNotBeNull("requestDocument");

            if(IsDebugEnabled)
                log.Debug("요청문서에 대한 실행을 수행합니다... dbName=[{0}]", dbName);

            return ResolveXmlDataManager(dbName).Execute(requestDocument);
        }

        /// <summary>
        /// 요청 정보중에 응답정보로 다시 보내기 위해서 헤더 정보를 복사한다.
        /// </summary>
        /// <param name="requestDocument">요청문서</param>
        /// <param name="responseDocument">응답문서</param>
        public static void CopyRequestHeader(this XdsRequestDocument requestDocument, XdsResponseDocument responseDocument) {
            requestDocument.ShouldNotBeNull("requestDocument");
            responseDocument.ShouldNotBeNull("responseDocument");

            responseDocument.ConnectionString = requestDocument.ConnectionString;
            responseDocument.Transaction = requestDocument.Transaction;
            responseDocument.IsolationLevel = requestDocument.IsolationLevel;
        }

        /// <summary>
        /// DataSet을 얻을 때 범위를 두고 얻는다. 
        /// </summary>
        /// <param name="pageSize">페이지 크기</param>
        /// <param name="pageNo">페이지 번호 (1부터 시작)</param>
        /// <param name="firstResult">시작 레코드 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">페이지 크기와 같음</param>
        public static void GetRecordRange(int pageSize, int pageNo, out int firstResult, out int maxResults) {
            firstResult = 0;
            maxResults = 0;

            if(pageSize != -1 && pageNo != -1) {
                firstResult = pageSize * (pageNo - 1);
                maxResults = pageSize;
            }

            if(IsDebugEnabled)
                log.Debug("Get paginated record range. pageSize=[{0}], pageNo=[{1}], firstRecord=[{2}], maxRecords=[{3}]",
                          pageSize, pageNo, firstResult, maxResults);
        }

        public static XdsResponseDocument CreateResponseWithError(Exception ex) {
            var responseDoc = new XdsResponseDocument();
            if(ex != null) {
                responseDoc.Errors.AddError(ex);

                if(log.IsWarnEnabled)
                    log.WarnException("예외가 발생하여 예외정보를 담은 응답문서를 생성했습니다.", ex);
            }
            return responseDoc;
        }

        /// <summary>
        /// 지정된 예외 정보를 응답 문서(<see cref="XdsResponseDocument"/>)에 등록한다.
        /// </summary>
        /// <param name="responseDocument">응답 문서</param>
        /// <param name="ex">등록할 예외 정보</param>
        public static void ReportError(this XdsResponseDocument responseDocument, Exception ex) {
            if(ex != null) {
                responseDocument.Errors.AddError(ex);

                if(log.IsWarnEnabled)
                    log.WarnException("예외가 발생하여 응답문서에 예외정보를 추가했습니다.", ex);
            }
        }

        /// <summary>
        /// 요청정보 중에 RequestType이 <see cref="XmlDataRequestKind.Method"/> 인 경우에는 Method를 실제 실행할 Query 문장으로 변경한다.
        /// </summary>
        /// <param name="repository"><see cref="IIniQueryProvider"/>를 가진 기본 <see cref="IAdoRepository"/></param>
        /// <param name="request">요청 정보</param>
        public static void ConvertToQuery(IAdoRepository repository, XdsRequestItem request) {
            repository.ShouldNotBeNull("repository");
            request.ShouldNotBeNull("request");

            if(IsDebugEnabled)
                log.Debug("요청정보의 Method에 해당하는 실제 Query String을 읽어옵니다.");

            if(repository.QueryProvider == null) {
                if(IsDebugEnabled)
                    log.Debug("AdoRepository.QueryProvider가 제공되지 않아, 조회 작업이 취소되었습니다.");

                return;
            }

            foreach(XdsQuery query in request.PreQueries) {
                if(query.RequestKind == XmlDataRequestKind.Method) {
                    query.Query = repository.QueryProvider.GetQuery(query.Query);
                    query.RequestKind = AdoTool.IsSqlString(query.Query)
                                            ? XmlDataRequestKind.Query
                                            : XmlDataRequestKind.StoredProc;
                }
            }

            if(request.RequestKind == XmlDataRequestKind.Method) {
                request.Query = repository.QueryProvider.GetQuery(request.Query);
                request.RequestKind = AdoTool.IsSqlString(request.Query)
                                          ? XmlDataRequestKind.Query
                                          : XmlDataRequestKind.StoredProc;
            }

            foreach(XdsQuery query in request.PostQueries) {
                if(query.RequestKind == XmlDataRequestKind.Method) {
                    query.Query = repository.QueryProvider.GetQuery(query.Query);
                    query.RequestKind = AdoTool.IsSqlString(query.Query)
                                            ? XmlDataRequestKind.Query
                                            : XmlDataRequestKind.StoredProc;
                }
            }

            if(IsDebugEnabled)
                log.Debug("요청 정보의 Method에 해당하는 실제 Query String을 모두 읽었습니다.");
        }
    }
}