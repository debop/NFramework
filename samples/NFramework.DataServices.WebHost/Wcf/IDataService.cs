using System.ServiceModel;
using System.ServiceModel.Web;

namespace NSoft.NFramework.DataServices.WebHost.Wcf {
    // 참고: "리팩터링" 메뉴에서 "이름 바꾸기" 명령을 사용하여 코드 및 config 파일에서 인터페이스 이름 "IDataService"을 변경할 수 있습니다.
    [ServiceContract(Namespace = "http://svc.realweb21.com")]
    public interface IDataService {
        /// <summary>
        /// 통신 연결 여부 및 서버 활성화 여부를 확인하기 위한 함수
        /// </summary>
        /// <returns>서버상의 실행 시간</returns>
        [OperationContract]
        [WebGet()]
        string Ping();

        [OperationContract]
        string[] GetMethods(string productName);

        [OperationContract]
        string GetMethodBody(string productName, string methodName);

        [OperationContract]
        bool MethodExists(string productName, string methodName);

        /// <summary>
        /// 직렬화된 요청정보를 역직렬화하여, DB 작업을 수행하고, 결과를 직렬화하여 반환합니다.
        /// </summary>
        /// <param name="requestBytes">직렬화된 요청정보</param>
        /// <param name="productName">요청 대상 제품 정보</param>
        /// <returns>직렬화된 응답 정보</returns>
        [OperationContract]
        byte[] Execute(byte[] requestBytes, string productName);

        /// <summary>
        /// 직렬화된 요청정보를 역직렬화하여, DB 작업을 수행하고, 결과를 직렬화하여 반환합니다.
        /// </summary>
        /// <param name="requestText">직렬화된 요청정보</param>
        /// <param name="productName">요청 대상 제품 정보</param>
        /// <returns>직렬화된 응답 정보</returns>
        [OperationContract]
        string ExecuteAsText(string requestText, string productName);
    }
}