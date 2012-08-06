using System;
using System.Net.Security;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace NSoft.NFramework.XmlData.WebHost.Services {
    // 참고: 여기서 클래스 이름 "XmlDataService"를 변경하는 경우 Web.config에서 "XmlDataService"에 대한 참조도 업데이트해야 합니다.
    [ServiceContract(Namespace = "http://svc.realweb21.com")]
    public class XmlDataService {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public XmlDataService() {
            if(IsDebugEnabled)
                log.Debug("XmlDataService WCF 서비스가 생성되었습니다.");
        }

        [OperationContract]
        [WebGet()]
        public string Ping() {
            return "XmlDataService.Ping() executed at " + DateTime.Now;
        }

        [OperationContract]
        public byte[] Execute(byte[] requestBytes, string productName, bool compress) {
            if(IsDebugEnabled)
                log.Debug("요청을 처리합니다... productName=[{0}], compress=[{1}]", productName, compress);

            try {
                var adapter = XmlDataTool.ResolveXmlDataManagerAdapter(productName);
                return adapter.Execute(requestBytes);
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.ErrorException("요청을 처리하는 동안 예외가 발생했습니다.", ex);
            }
            return new byte[0];
        }

        // NOTE: WSHttpBinding은 기본적으로 256bit로 암호화를 수행한다.
        [OperationContract(ProtectionLevel = ProtectionLevel.EncryptAndSign)]
        public byte[] ExecuteSecurity(byte[] requestBytes, string productName, bool compress) {
            if(IsDebugEnabled)
                log.Debug("요청을 처리합니다... productName=[{0}], compress=[{1}]", productName, compress);

            try {
                var adapter = XmlDataTool.ResolveXmlDataManagerAdapter(productName);
                return adapter.Execute(requestBytes);
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.ErrorException("요청을 처리하는 동안 예외가 발생했습니다.", ex);
            }
            return new byte[0];
        }
    }
}