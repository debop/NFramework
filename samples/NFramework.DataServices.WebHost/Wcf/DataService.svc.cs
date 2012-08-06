using System;
using System.Collections.Generic;
using System.Linq;
using NSoft.NFramework.Data;
using NSoft.NFramework.InversionOfControl;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.DataServices.WebHost.Wcf {
    // 참고: "리팩터링" 메뉴에서 "이름 바꾸기" 명령을 사용하여 코드, svc 및 config 파일에서 클래스 이름 "DataService"을 변경할 수 있습니다.
    public class DataService : IDataService {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public DataService() {
            if(IsDebugEnabled)
                log.Debug("WCF DataService 웹 서비스가 생성되었습니다.");
        }

        /// <summary>
        /// 통신 연결 여부 및 서버 활성화 여부를 확인하기 위한 함수
        /// </summary>
        /// <returns>서버상의 실행 시간</returns>
        public string Ping() {
            return "Ping WCF DataService executed at " + DateTime.UtcNow;
        }

        public string[] GetMethods(string productName) {
            try {
                var repository = IoC.Resolve<IAdoRepository>("AdoRepository." + productName);
                var queryTable = new Dictionary<string, string>(repository.QueryProvider.GetQueries());

                return queryTable.Keys.ToArray();
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.ErrorException("메소드 목록을 로드하는데 실패했습니다. Product=" + productName, ex);
                throw;
            }
        }

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
        public byte[] Execute(byte[] requestBytes, string productName) {
            if(IsDebugEnabled)
                log.Debug("WCF Service로 요청정보를 받아 DataService 를 수행합니다...");

            if(requestBytes == null || requestBytes.Length == 0)
                return new byte[0];


            var responseBytes = DataServiceTool.ResolveDataServiceAdapter(productName).Execute(requestBytes) ?? new byte[0];

            if(IsDebugEnabled)
                log.Debug("WCF Service로 DataService를 처리하고 응답정보를 전송합니다. responseBytes length=[{0}]", responseBytes.Length);

            return responseBytes;
        }

        /// <summary>
        /// 직렬화된 요청정보를 역직렬화하여, DB 작업을 수행하고, 결과를 직렬화하여 반환합니다.
        /// </summary>
        /// <param name="requestText">직렬화된 요청정보</param>
        /// <param name="productName">요청 대상 제품 정보</param>
        /// <returns>직렬화된 응답 정보</returns>
        public string ExecuteAsText(string requestText, string productName) {
            if(IsDebugEnabled)
                log.Debug("WCF Service로 요청정보를 받아 DataService 를 수행합니다... requestText=[{0}]", requestText.EllipsisChar(255));

            if(requestText.IsWhiteSpace())
                return string.Empty;

            var responseText = DataServiceTool.ResolveDataServiceAdapter(productName).Execute(requestText);

            if(IsDebugEnabled)
                log.Debug("WCF Service로 DataService를 처리하고 응답정보를 전송합니다. responseText length=[{0}]", responseText.Length);

            return responseText;
        }
    }
}