using System;
using System.Configuration;
using System.IO;
using System.Web;
using NSoft.NFramework.Data;
using NSoft.NFramework.Tools;
using NSoft.NFramework.Xml;
using NSoft.NFramework.XmlData.Messages;

namespace NSoft.NFramework.XmlData.WebHost {
    /// <summary>
    /// <see cref="XdsRequestDocument"/> 정보를 처리하여 <see cref="XdsResponseDocument"/>로 반환하는 작업을 수행하는 Utility Class입니다.
    /// </summary>
    public static class XmlDataServiceFacade {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public const string PARAM_PRODUCT = "Product";
        public const string PARAM_COMPRESS = "Compress";
        public const string PARAM_SECURITY = "Security";

        /// <summary>
        /// aspx, ashx의 <see cref="HttpContext"/>로부터 XdsRequestDocument를 만들고, 
        /// 처리를 수행 한 후, XdsResponseDocument를 HttpContext의 Response.OutputStream에 Write를 수행합니다.
        /// </summary>
        /// <param name="context"></param>
        public static void Execute(HttpContext context) {
            context.ShouldNotBeNull("context");

            if(IsDebugEnabled)
                log.Debug(@"Execution for request from HttpContext is starting...");

            var request = context.Request;
            var productName = request.GetProductName();
            var compress = request.GetUseCompress();
            var security = request.GetUseSecurity();

            var serializer = XmlDataServiceTool.GetSerializer(compress, security);

            XdsResponseDocument result = null;
            try {
                CheckProductExists(productName);
                Guard.Assert(request.TotalBytes > 0, "처리할 요청정보가 제공되지 않았습니다.");

                var requestBytes = request.InputStream.ToBytes();
                var xdsRequest = ((byte[])serializer.Deserialize(requestBytes)).ConvertToXdsRequestDocument();

                // var xdsRequest = XmlTool.Deserialize<XdsRequestDocument>(request.InputStream);
                result = Execute(xdsRequest, productName);
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.ErrorException("요청처리 중 예외가 발생했습니다.", ex);

                // 예외정보를 Client에게도 보낸다.
                result = result ?? new XdsResponseDocument();
                result.ReportError(ex);
            }
            finally {
                if(result != null)
                    WriteResponse(context.Response, result, serializer);
            }

            if(IsDebugEnabled)
                log.Debug("Execution for request from HttpContext is finished!!!");
        }

        /// <summary>
        /// 해당 제품 DB에 대해 요청정보를 처리한 후 결과를 반환합니다.
        /// </summary>
        /// <param name="xdsRequest"></param>
        /// <param name="productName"></param>
        /// <returns></returns>
        public static XdsResponseDocument Execute(XdsRequestDocument xdsRequest, string productName) {
            xdsRequest.ShouldNotBeNull("xdsRequest");

            if(productName.IsWhiteSpace())
                productName = AdoTool.DefaultDatabaseName;

            CheckProductExists(productName);
            return XmlDataTool.Execute(xdsRequest, productName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestBytes"></param>
        /// <param name="productName"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public static byte[] Execute(byte[] requestBytes, string productName, ISerializer serializer) {
            if(productName.IsWhiteSpace())
                productName = AdoTool.DefaultDatabaseName;

            XdsResponseDocument xdsResponse = null;
            byte[] result = null;

            try {
                Guard.Assert(requestBytes != null && requestBytes.Length > 0, "요청 정보가 없습니다.");
                XmlDataServiceFacade.CheckProductExists(productName);

                var requestData = (byte[])serializer.Deserialize(requestBytes);
                var xdsRequest = requestData.ConvertToXdsRequestDocument(XmlTool.XmlEncoding);
                xdsResponse = XmlDataServiceFacade.Execute(xdsRequest, productName);
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.ErrorException("예외가 발생했습니다.", ex);

                if(xdsResponse == null)
                    xdsResponse = new XdsResponseDocument();
                xdsResponse.ReportError(ex);
            }
            finally {
                byte[] xdsResponseBytes;
                XmlTool.Serialize(xdsResponse, out xdsResponseBytes);

                result = serializer.Serialize(xdsResponseBytes);
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestStream"></param>
        /// <param name="productName"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public static Stream ExecuteStreamInternal(Stream requestStream, string productName, ISerializer serializer) {
            XdsResponseDocument xdsResponse = null;
            MemoryStream result = null;
            try {
                Guard.Assert(requestStream != null && requestStream.Length > 0, "요청 정보가 없습니다.");
                CheckProductExists(productName);

                var xdsRequest = (XdsRequestDocument)serializer.Deserialize(requestStream.ToBytes());
                xdsResponse = XmlDataServiceFacade.Execute(xdsRequest, productName);
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.ErrorException("예외가 발생했습니다.", ex);

                if(xdsResponse == null)
                    xdsResponse = new XdsResponseDocument();
                xdsResponse.ReportError(ex);
            }
            finally {
                result = new MemoryStream(serializer.Serialize(xdsResponse));
            }
            return result;
        }

        #region << Helper Methods >>

        /// <summary>
        /// 처리 결과를 Http Response에 쓴다.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="xdsResponse"></param>
        /// <param name="serializer"></param>
        internal static void WriteResponse(HttpResponse response, XdsResponseDocument xdsResponse, ISerializer serializer) {
            response.Clear();
            response.Buffer = true;
            response.Expires = -1;
            response.ContentEncoding = XmlTool.XmlEncoding;
            response.ContentType = "text/xml";

            var responseBytes = serializer.Serialize(xdsResponse.ConvertToBytes());

            response.OutputStream.Write(responseBytes, 0, responseBytes.Length);

            // XmlTool.Serialize(xdsResponse, response.OutputStream);
        }

        /// <summary>
        /// <see cref="HttpRequest"/> 요청정보에서 Product 인자의 값을 읽어온다. 값이 없다면, DAAB의 기본 DatabaseName으로 대체한다.
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        internal static string GetProductName(this HttpRequest httpRequest) {
            return httpRequest[PARAM_PRODUCT].AsText(AdoTool.DefaultDatabaseName);
        }

        /// <summary>
        /// <see cref="HttpRequest"/> 요청정보에서 Compress 인자의 값을 읽어온다. 
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        internal static bool GetUseCompress(this HttpRequest httpRequest) {
            return httpRequest[PARAM_COMPRESS].AsInt(0) != 0;
        }

        /// <summary>
        /// <see cref="HttpRequest"/> 요청정보에서 Security 인자의 값을 읽어온다. 
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        internal static bool GetUseSecurity(this HttpRequest httpRequest) {
            return httpRequest[PARAM_SECURITY].AsInt(0) != 0;
        }

        /// <summary>
        /// 해당 Product에 대한 Database Connection string이 정의되었는지 확인한다. 없다면 예외를 발생시킨다.
        /// </summary>
        /// <param name="productName"></param>
        internal static void CheckProductExists(this string productName) {
            Guard.Assert(IsDefinedConnectionStringName(productName),
                         "지정된 Product [{0}] 이 Database Connection 설정정보에 정의되지 않았습니다. " +
                         "web.config의 connectionStrings section에 추가하셔야합니다.",
                         productName);
        }

        /// <summary>
        /// 환경설정에 지정된 Database 이름을 가진 ConnectionString이 정의되어 있는지 확인한다.
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        internal static bool IsDefinedConnectionStringName(string dbName) {
            if(dbName.IsWhiteSpace())
                return false;

            var settings = ConfigurationManager.ConnectionStrings[dbName];
            return (settings != null && settings.ConnectionString.IsNotWhiteSpace());
        }

        #endregion
    }
}