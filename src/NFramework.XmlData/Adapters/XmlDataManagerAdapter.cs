using System;
using NSoft.NFramework.Serializations;
using NSoft.NFramework.Tools;
using NSoft.NFramework.XmlData.Messages;

namespace NSoft.NFramework.XmlData {
    /// <summary>
    /// <see cref="IXmlDataManager"/>에 필요한 요청정보 및 반환된 응답정보를 외부와 통신하기 위한 포맷으로 변환하는 Adapter입니다.
    /// </summary>
    public class XmlDataManagerAdapter : IXmlDataManagerAdapter {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public static SerializationOptions DefaultSerializationOption = SerializationOptions.Xml;

        [CLSCompliant(false)] protected IXmlDataManager _xmlDataManager;

        [CLSCompliant(false)] protected ISerializer<XdsRequestDocument> _requestSerializer;

        [CLSCompliant(false)] protected ISerializer<XdsResponseDocument> _responseSerializer;

        public XmlDataManagerAdapter() {}

        public XmlDataManagerAdapter(IXmlDataManager xmlDataManager) {
            XmlDataManager = xmlDataManager;
        }

        /// <summary>
        /// 요청정보를 처리하여 응답정보를 빌드하는 XmlDataManager 
        /// </summary>
        public virtual IXmlDataManager XmlDataManager {
            get { return _xmlDataManager ?? (_xmlDataManager = XmlDataTool.ResolveXmlDataManager()); }
            set { _xmlDataManager = value; }
        }

        /// <summary>
        /// 직렬화된 요청정보를 역직렬화를 수행하는 <see cref="ISerializer{T}"/> 입니다.
        /// </summary>
        public virtual ISerializer<XdsRequestDocument> RequestSerializer {
            get { return _requestSerializer ?? (_requestSerializer = XmlDataTool.ResolveRequestSerializer()); }
            set { _requestSerializer = value; }
        }

        /// <summary>
        /// 응답정보를 직렬화하는 <see cref="ISerializer{T}"/> 입니다.
        /// </summary>
        public virtual ISerializer<XdsResponseDocument> ResponseSerializer {
            get { return _responseSerializer ?? (_responseSerializer = XmlDataTool.ResolveResponseSerializer()); }
            set { _responseSerializer = value; }
        }

        /// <summary>
        /// 1. 직렬화된 정보를 <see cref="RequestSerializer"/>를 이용하여, 역직렬화를 수행. <see cref="XdsRequestDocument"/>를 빌드<br/>
        /// 2. 요청정보를  <see cref="XmlDataManager"/>에 전달하여, 실행 후, 응답정보를 반환 받음<br/>
        /// 3. 응답정보를 <see cref="ResponseSerializer"/>를 통해 직렬화하여 byte[] 로 반환함.
        /// </summary>
        /// <param name="requestBytes">직렬화된 요청 Data</param>
        /// <returns>응답정보를 직렬화한 byte[]</returns>
        public virtual byte[] Execute(byte[] requestBytes) {
            if(IsDebugEnabled)
                log.Debug("XML 포맷 방식의 정보를 처리를 시작합니다... IXmlDataManager=[{0}]", XmlDataManager);

            requestBytes.ShouldNotBeNull("requestBytes");

            try {
                var requestMsg = RequestSerializer.Deserialize(requestBytes);
                var XdsResponseDocument = XmlDataManager.Execute(requestMsg);
                var responseBytes = ResponseSerializer.Serialize(XdsResponseDocument);

                if(IsDebugEnabled)
                    log.Debug("요청을 모두 처리하고, 응답 정보를 포맷으로 반환합니다!!!");

                return responseBytes;
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.ErrorException("요청 처리 중 예외가 발생했습니다.", ex);

                return ResponseSerializer.Serialize(XmlDataTool.CreateResponseWithError(ex));
            }
        }

        /// <summary>
        /// 입력정보를 <see cref="XdsRequestDocument"/>로 변환하여, 요청 작업 후, <see cref="XdsResponseDocument"/>를 직렬화하여 반환합니다.
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

                return ResponseSerializer.Serialize(XmlDataTool.CreateResponseWithError(ex)).ToText();
            }
        }
    }
}