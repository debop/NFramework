using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Services;
using NSoft.NFramework.Data;
using NSoft.NFramework.InversionOfControl;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.DataServices.WebHost.WebServices {
    /// <summary>
    /// DataService의 WebService 버전입니다.
    /// </summary>
    [WebService(Namespace = "http://ws.realweb21.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class DataService : System.Web.Services.WebService {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public DataService() {
            if(IsDebugEnabled)
                log.Debug("DataService 웹 서비스가 생성되었습니다.");
        }

        [WebMethod(Description = "웹서비스 통신 테스트용 메소드입니다. 결과값은  현재시각의 UTC 값입니다.")]
        public string Ping() {
            return "Ping 웹서비스 DataService executed at " + DateTime.UtcNow;
        }

        [WebMethod(Description = "지정한 제품에서 제공하는 모든 메소드와 메소드의 내용을 모두 반환합니다.")]
        public string[] GetMethods(string productName) {
            try {
                var repository = IoC.Resolve<IAdoRepository>("AdoRepository." + productName);
                var queryTable = new SortedDictionary<string, string>(repository.QueryProvider.GetQueries());

                return queryTable.Keys.ToArray();
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.ErrorException("메소드 목록을 로드하는데 실패했습니다. Product=" + productName, ex);
                throw;
            }
        }

        [WebMethod(Description = "특정제품에서 제공하는 메소드의 본문을 반환합니다.")]
        public string GetMethodBody(string productName, string methodName) {
            try {
                var repository = IoC.Resolve<IAdoRepository>("AdoRepository." + productName);
                var queryTable = new SortedDictionary<string, string>(repository.QueryProvider.GetQueries());

                return queryTable.GetValue(methodName);
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.ErrorException("메소드 목록을 로드하는데 실패했습니다. Product=" + productName, ex);
                throw;
            }
        }

        [WebMethod(Description = "특정제품에서 제공하는 메소드가 존재하는지 검사합니다.")]
        public bool MethodExists(string productName, string methodName) {
            try {
                var repository = IoC.Resolve<IAdoRepository>("AdoRepository." + productName);
                var queryTable = new SortedDictionary<string, string>(repository.QueryProvider.GetQueries());

                return queryTable.Keys.Contains(methodName);
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.ErrorException("메소드 목록을 로드하는데 실패했습니다. Product=" + productName, ex);
                throw;
            }
        }

        /// <summary>
        /// 직렬화된 요청정보를 역직렬화하여, DB 작업을 수행하고, 결과를 직렬화하여 반환합니다.
        /// </summary>
        /// <param name="requestBytes">직렬화된 요청정보</param>
        /// <param name="productName">요청 대상 제품 정보</param>
        /// <returns>직렬화된 응답 정보</returns>
        [WebMethod(Description = "DataSerice 요청을 처리합니다.")]
        public byte[] Execute(byte[] requestBytes, string productName) {
            if(IsDebugEnabled)
                log.Debug("웹서비스로 요청정보를 받아 DataService 를 수행합니다...");

            if(requestBytes == null || requestBytes.Length == 0)
                return new byte[0];


            var responseBytes =
                DataServiceTool
                    .ResolveDataServiceAdapter(productName)
                    .Execute(requestBytes) ?? new byte[0];

            if(IsDebugEnabled)
                log.Debug("웹서비스로 DataService를 처리하고 응답정보를 전송합니다. responseBytes length=[{0}]", responseBytes.Length);

            return responseBytes;
        }

        /// <summary>
        /// 직렬화된 요청정보를 역직렬화하여, DB 작업을 수행하고, 결과를 직렬화하여 반환합니다.
        /// </summary>
        /// <param name="requestText">직렬화된 요청정보</param>
        /// <param name="productName">요청 대상 제품 정보</param>
        /// <returns>직렬화된 응답 정보</returns>
        [WebMethod(Description = "DataSerice 요청을 처리합니다.")]
        public string ExecuteAsText(string requestText, string productName) {
            if(IsDebugEnabled)
                log.Debug("웹서비스로 요청정보를 받아 DataService 를 수행합니다... requestText=[{0}]", requestText.EllipsisChar(255));

            if(requestText.IsWhiteSpace())
                return string.Empty;

            var responseText = DataServiceTool.ResolveDataServiceAdapter(productName).Execute(requestText);

            if(IsDebugEnabled)
                log.Debug("웹서비스로 DataService를 처리하고 응답정보를 전송합니다. responseText length=[{0}]", responseText.Length);

            return responseText;
        }
    }
}