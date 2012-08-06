using System;
using NSoft.NFramework.DataServices.Messages;
using NSoft.NFramework.Serializations;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.DataServices.Adapters {
    /// <summary>
    /// <see cref="IDataService"/>의 요청정보 및 응답정보를 byte[] 또는 문자열로 통신할 수 있도록 하는 Adapter 입니다.
    /// </summary>
    public class DataServiceAdapter : IDataServiceAdapter {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public static SerializationOptions DefaultSerializationOption = SerializationOptions.Json;

        [CLSCompliant(false)] protected IDataService _dataService;

        [CLSCompliant(false)] protected ISerializer<RequestMessage> _requestSerializer;

        [CLSCompliant(false)] protected ISerializer<ResponseMessage> _responseSerializer;

        public DataServiceAdapter() {}

        public DataServiceAdapter(IDataService dataService) {
            DataService = dataService;
        }

        /// <summary>
        /// 요청정보를 처리하여 응답정보를 빌드하는 DataService 
        /// </summary>
        public virtual IDataService DataService {
            get { return _dataService ?? (_dataService = DataServiceTool.ResolveDataService()); }
            set { _dataService = value; }
        }

        /// <summary>
        /// 직렬화된 요청정보를 역직렬화를 수행하는 <see cref="ISerializer{T}"/> 입니다.
        /// </summary>
        public virtual ISerializer<RequestMessage> RequestSerializer {
            get { return _requestSerializer ?? (_requestSerializer = DataServiceTool.ResolveRequestSerializer()); }
            set { _requestSerializer = value; }
        }

        /// <summary>
        /// 응답정보를 직렬화하는 <see cref="ISerializer{T}"/> 입니다.
        /// </summary>
        public virtual ISerializer<ResponseMessage> ResponseSerializer {
            get { return _responseSerializer ?? (_responseSerializer = DataServiceTool.ResolveResponseSerializer()); }
            set { _responseSerializer = value; }
        }

        /// <summary>
        /// 1. 직렬화된 정보를 <see cref="IDataServiceAdapter.RequestSerializer"/>를 이용하여, 역직렬화를 수행. <see cref="RequestMessage"/>를 빌드<br/>
        /// 2. 요청정보를  <see cref="IDataServiceAdapter.DataService"/>에 전달하여, 실행 후, 응답정보를 반환 받음<br/>
        /// 3. 응답정보를 <see cref="IDataServiceAdapter.ResponseSerializer"/>를 통해 직렬화하여 byte[] 로 반환함.
        /// </summary>
        /// <param name="requestBytes">직렬화된 요청 Data</param>
        /// <returns>응답정보를 직렬화한 byte[]</returns>
        public virtual byte[] Execute(byte[] requestBytes) {
            if(IsDebugEnabled)
                log.Debug("JSON 포맷 방식의 정보를 처리를 시작합니다... IDataService=[{0}]", DataService);

            requestBytes.ShouldNotBeNull("requestBytes");

            try {
                var requestMsg = RequestSerializer.Deserialize(requestBytes);
                var responseMessage = DataService.Execute(requestMsg);
                var responseBytes = ResponseSerializer.Serialize(responseMessage);

                if(IsDebugEnabled)
                    log.Debug("요청을 모두 처리하고, 응답 정보를 포맷으로 반환합니다!!!");

                return responseBytes;
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.ErrorException("요청 처리 중 예외가 발생했습니다.", ex);

                return ResponseSerializer.Serialize(DataServiceTool.CreateResponseMessageWithException(ex));
            }
        }

        /// <summary>
        /// 입력정보를 <see cref="RequestMessage"/>로 변환하여, 요청 작업 후, <see cref="ResponseMessage"/>를 직렬화하여 반환합니다.
        /// </summary>
        /// <param name="requestText">요청 데이터</param>
        /// <returns>응답정보를 직렬화한 문자열</returns>
        public string Execute(string requestText) {
            try {
                return Execute(requestText.Base64Decode()).Base64Encode();
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.ErrorException("요청 처리 중 예외가 발생했습니다.", ex);

                return ResponseSerializer.Serialize(DataServiceTool.CreateResponseMessageWithException(ex)).Base64Encode();
            }
        }
    }
}