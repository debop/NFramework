using System;
using System.Web.Services;

namespace NSoft.NFramework.XmlData.WebHost.WebServices {
    /// <summary>
    /// XmlDataService의 요약 설명입니다.
    /// </summary>
    [WebService(Namespace = "http://ws.realweb21.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class XmlDataService : System.Web.Services.WebService {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public XmlDataService() {
            if(IsDebugEnabled)
                log.Debug("XmlDataService 웹 서비스가 생성되었습니다.");
        }

        [WebMethod]
        public string Ping() {
            return "Ping service executed at " + DateTime.Now;
        }

        [WebMethod(Description = "Execute XmlDataService")]
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

        [WebMethod(Description = "Execute XmlDataService")]
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