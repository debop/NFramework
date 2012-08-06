using NSoft.NFramework.DataServices.Messages;

namespace NSoft.NFramework.DataServices.Adapters {
    /// <summary>
    /// <see cref="IDataService"/>의 요청정보 및 응답정보를 byte[] 또는 문자열로 통신할 수 있도록 하는 Adapter 입니다.
    /// </summary>
    public interface IDataServiceAdapter {
        /// <summary>
        /// 요청정보를 처리하여 응답정보를 빌드하는 DataService 
        /// </summary>
        IDataService DataService { get; set; }

        /// <summary>
        /// 직렬화된 요청정보를 역직렬화를 수행하는 <see cref="ISerializer{T}"/> 입니다.
        /// </summary>
        ISerializer<RequestMessage> RequestSerializer { get; set; }

        /// <summary>
        /// 응답정보를 직렬화하는 <see cref="ISerializer{T}"/> 입니다.
        /// </summary>
        ISerializer<ResponseMessage> ResponseSerializer { get; set; }

        /// <summary>
        /// 1. 직렬화된 정보를 <see cref="RequestSerializer"/>를 이용하여, 역직렬화를 수행. <see cref="RequestMessage"/>를 빌드<br/>
        /// 2. 요청정보를  <see cref="DataService"/>에 전달하여, 실행 후, 응답정보를 반환 받음<br/>
        /// 3. 응답정보를 <see cref="ResponseSerializer"/>를 통해 직렬화하여 byte[] 로 반환함.
        /// </summary>
        /// <param name="requestBytes">직렬화된 요청 Data</param>
        /// <returns>응답정보를 직렬화한 byte[]</returns>
        byte[] Execute(byte[] requestBytes);

        /// <summary>
        /// 입력정보를 <see cref="RequestMessage"/>로 변환하여, 요청 작업 후, <see cref="ResponseMessage"/>를 직렬화하여 반환합니다.
        /// </summary>
        /// <param name="requestText">요청 데이터</param>
        /// <returns>응답정보를 직렬화한 문자열</returns>
        string Execute(string requestText);
    }
}